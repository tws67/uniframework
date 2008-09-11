//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging
{
	/// <summary>
	/// Instance based class to write log messages based on a given configuration.
	/// Messages are routed based on category.
	/// </summary>
	/// <remarks>
	/// <para>
	/// To write log messages to the default configuration, use the <see cref="Logger"/> facade.  
	/// Only create an instance of a LogWriter if you need to write log messages using a custom configuration.
	/// </para>
	/// <para>
	/// The LogWriter works as an entry point to the <see cref="System.Diagnostics"/> trace listeners. 
	/// It will trace the <see cref="LogEntry"/> through the <see cref="TraceListeners"/>s associated with the <see cref="LogSource"/>s 
	/// for all the matching categories in the elements of the <see cref="LogEntry.Categories"/> property of the log entry. 
	/// If the "all events" special log source is configured, the log entry will be traced through the log source regardles of other categories 
	/// that might have matched.
	/// If the "all events" special log source is not configured and the "unprocessed categories" special log source is configured,
	/// and the category specified in the logEntry being logged is not defined, then the logEntry will be logged to the "unprocessed categories"
	/// special log source.
	/// If both the "all events" and "unprocessed categories" special log sources are not configured and the property LogWarningsWhenNoCategoriesMatch
	/// is set to true, then the logEntry is logged to the "logging errors and warnings" special log source.
	/// </para>
	/// </remarks>
	[CustomFactory(typeof(LogWriterCustomFactory))]
	public class LogWriter : ILogFilterErrorHandler, IInstrumentationEventProvider, IDisposable
	{
		private const int defaultTimeout = 2500;
		private static int readerLockAcquireTimeout = defaultTimeout;
		private static int writerLockAcquireTimeout = defaultTimeout;

		/// <summary>
		/// EventID used on LogEntries that occur when internal LogWriter mechanisms fail.
		/// </summary>
		public const int LogWriterFailureEventID = 6352;

		private LoggingInstrumentationProvider instrumentationProvider;

		private LogFilterHelper filter;
		private LogWriterStructureHolder structureHolder;
		private ReaderWriterLock structureHolderLock = new ReaderWriterLock();
		private ILogWriterStructureUpdater structureUpdater;

		/// <summary>
		/// Initializes a new instance of the <see cref="LogWriter"/> class.
		/// </summary>
		/// <param name="filters">The collection of filters to use when processing an entry.</param>
		/// <param name="traceSources">The trace sources to dispatch entries to.</param>
		/// <param name="errorsTraceSource">The special <see cref="LogSource"/> to which internal errors must be logged.</param>
		/// <param name="defaultCategory">The default category to set when entry categories list is empty.</param>
		public LogWriter(ICollection<ILogFilter> filters,
			IDictionary<string, LogSource> traceSources,
			LogSource errorsTraceSource,
			string defaultCategory)
			: this(filters, traceSources, null, null, errorsTraceSource, defaultCategory, false, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogWriter"/> class.
		/// </summary>
		/// <param name="filters">The collection of filters to use when processing an entry.</param>
		/// <param name="traceSources">The trace sources to dispatch entries to.</param>
		/// <param name="allEventsTraceSource">The special <see cref="LogSource"/> to which all log entries should be logged.</param>
		/// <param name="notProcessedTraceSource">The special <see cref="LogSource"/> to which log entries with at least one non-matching category should be logged.</param>
		/// <param name="errorsTraceSource">The special <see cref="LogSource"/> to which internal errors must be logged.</param>
		/// <param name="defaultCategory">The default category to set when entry categories list of a log entry is empty.</param>
		/// <param name="tracingEnabled">The tracing status.</param>
		/// <param name="logWarningsWhenNoCategoriesMatch">true if warnings should be logged when a non-matching category is found.</param>
		public LogWriter(
			ICollection<ILogFilter> filters,
			IDictionary<string, LogSource> traceSources,
			LogSource allEventsTraceSource,
			LogSource notProcessedTraceSource,
			LogSource errorsTraceSource,
			string defaultCategory,
			bool tracingEnabled,
			bool logWarningsWhenNoCategoriesMatch)
			: this(CreateStructureHolder(filters, traceSources, allEventsTraceSource, notProcessedTraceSource, errorsTraceSource, defaultCategory, tracingEnabled, logWarningsWhenNoCategoriesMatch), null)
		{ }

		internal LogWriter(LogWriterStructureHolder structureHolder, ILogWriterStructureUpdater structureUpdater)
		{
			this.structureHolder = structureHolder;
			this.filter = new LogFilterHelper(structureHolder.Filters, this);
			this.structureUpdater = structureUpdater;

			instrumentationProvider = new LoggingInstrumentationProvider();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogWriter"/> class.
		/// </summary>
		/// <param name="filters">The collection of filters to use when processing an entry.</param>
		/// <param name="traceSources">The trace sources to dispatch entries to.</param>
		/// <param name="errorsTraceSource">The special <see cref="LogSource"/> to which internal errors must be logged.</param>
		/// <param name="defaultCategory">The default category to set when entry categories list is empty.</param>
		public LogWriter(ICollection<ILogFilter> filters,
			ICollection<LogSource> traceSources,
			LogSource errorsTraceSource,
			string defaultCategory)
			: this(filters, CreateTraceSourcesDictionary(traceSources), errorsTraceSource, defaultCategory)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogWriter"/> class.
		/// </summary>
		/// <param name="filters">The collection of filters to use when processing an entry.</param>
		/// <param name="traceSources">The trace sources to dispatch entries to.</param>
		/// <param name="allEventsTraceSource">The special <see cref="LogSource"/> to which all log entries should be logged.</param>
		/// <param name="notProcessedTraceSource">The special <see cref="LogSource"/> to which log entries with at least one non-matching category should be logged.</param>
		/// <param name="errorsTraceSource">The special <see cref="LogSource"/> to which internal errors must be logged.</param>
		/// <param name="defaultCategory">The default category to set when entry categories list is empty.</param>
		/// <param name="tracingEnabled">The tracing status.</param>
		/// <param name="logWarningsWhenNoCategoriesMatch">true if warnings should be logged when a non-matching category is found.</param>
		public LogWriter(ICollection<ILogFilter> filters,
			ICollection<LogSource> traceSources,
			LogSource allEventsTraceSource,
			LogSource notProcessedTraceSource,
			LogSource errorsTraceSource,
			string defaultCategory,
			bool tracingEnabled,
			bool logWarningsWhenNoCategoriesMatch)
			: this(filters,
			CreateTraceSourcesDictionary(traceSources),
			allEventsTraceSource,
			notProcessedTraceSource,
			errorsTraceSource,
			defaultCategory,
			tracingEnabled,
			logWarningsWhenNoCategoriesMatch)
		{
		}

		internal static void SetLockTimeouts(int readerTimeout, int writerTimeout)
		{
			readerLockAcquireTimeout = readerTimeout;
			writerLockAcquireTimeout = writerTimeout;
		}

		internal static void ResetLockTimeouts()
		{
			readerLockAcquireTimeout = defaultTimeout;
			writerLockAcquireTimeout = defaultTimeout;
		}

		private static LogWriterStructureHolder CreateStructureHolder(ICollection<ILogFilter> filters, IDictionary<string, LogSource> traceSources, LogSource allEventsTraceSource, LogSource notProcessedTraceSource, LogSource errorsTraceSource, string defaultCategory, bool tracingEnabled, bool logWarningsWhenNoCategoriesMatch)
		{
			return new LogWriterStructureHolder(filters, traceSources, allEventsTraceSource, notProcessedTraceSource,
									errorsTraceSource, defaultCategory, tracingEnabled, logWarningsWhenNoCategoriesMatch);
		}

		/// <summary>
		/// Adds a key/value pair to the <see cref="System.Runtime.Remoting.Messaging.CallContext"/> dictionary.  
		/// Context items will be recorded with every log entry.
		/// </summary>
		/// <param name="key">Hashtable key</param>
		/// <param name="value">Value.  Objects will be serialized.</param>
		/// <example>The following example demonstrates use of the AddContextItem method.
		/// <code>Logger.SetContextItem("SessionID", myComponent.SessionId);</code></example>
		public void SetContextItem(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			ContextItems items = new ContextItems();
			items.SetContextItem(key, value);
		}

		/// <summary>
		/// Empties the context items dictionary.
		/// </summary>
		public void FlushContextItems()
		{
			ContextItems items = new ContextItems();
			items.FlushContextItems();
		}

		/// <summary>
		/// Writes a new log entry as defined in the <see cref="LogEntry"/> parameter.
		/// </summary>
		/// <param name="log">Log entry object to write.</param>
		public void Write(LogEntry log)
		{
			try
			{
				structureHolderLock.AcquireReaderLock(readerLockAcquireTimeout);
				try
				{
					bool replacementDone = false;

					// set default category if necessary
					if (log.Categories.Count == 0)
					{
						log.Categories = new List<string>(1);
						log.Categories.Add(structureHolder.DefaultCategory);
						replacementDone = true;
					}

					if (structureHolder.TracingEnabled)
					{
						replacementDone = AddTracingCategories(log, replacementDone);
					}

					if (ShouldLog(log))
					{
						ProcessLog(log);
						instrumentationProvider.FireLogEventRaised();
					}
				}
				catch (Exception ex)
				{
					ReportUnknownException(ex, log);
				}
				finally
				{
					structureHolderLock.ReleaseReaderLock();
				}
			}
			catch (ApplicationException)
			{
				TryLogLockAcquisitionFailure(Resources.ExceptionFailedToAcquireLockToWriteLog);
			}
		}

		internal void ReplaceStructureHolder(LogWriterStructureHolder newStructureHolder)
		{
			try
			{
				structureHolderLock.AcquireWriterLock(writerLockAcquireTimeout);
				try
				{
					// Switch old and new structures.
					LogWriterStructureHolder oldStructureHolder = structureHolder;
					structureHolder = newStructureHolder;
					filter = new LogFilterHelper(structureHolder.Filters, this);

					// Dispose has to be fully performed before allowing the new structure to be used.
					oldStructureHolder.Dispose();
				}
				finally
				{
					structureHolderLock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException)
			{
				TryLogLockAcquisitionFailure(Resources.ExceptionFailedToAcquireLockToUpdate);
			}
		}

		private void TryLogLockAcquisitionFailure(string message)
		{
			instrumentationProvider.FireLockAcquisitionError(message);
		}

		private bool AddTracingCategories(LogEntry log, bool replacementDone)
		{
			Stack logicalOperationStack = null;

			if (!Tracer.IsTracingAvailable())
			{
				return replacementDone;
			}

			try
			{
				logicalOperationStack = GetLogicalOperationStack();
			}
			catch (SecurityException)
			{
				return replacementDone;
			}

			// add tracing categories
			foreach (object logicalOperation in logicalOperationStack)
			{
				// ignore non string objects in the stack
				string category = logicalOperation as string;
				if (category != null)
				{
					// must take care of logging categories..
					if (!log.Categories.Contains(category))
					{
						if (!replacementDone)
						{
							log.Categories = new List<string>(log.Categories);
							replacementDone = true;
						}
						log.Categories.Add(category);
					}
				}
			}

			return replacementDone;
		}

		private static Stack GetLogicalOperationStack()
		{
			return Trace.CorrelationManager.LogicalOperationStack;
		}

		/// <summary>
		/// Returns the filter of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of filter requiered.</typeparam>
		/// <returns>The instance of <typeparamref name="T"/> in the filters collection, or <see langword="null"/> 
		/// if there is no such instance.</returns>
		public T GetFilter<T>()
			where T : class, ILogFilter
		{
			return this.filter.GetFilter<T>();
		}

		/// <summary>
		/// Returns the filter of type <typeparamref name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T">The type of filter required.</typeparam>
		/// <param name="name">The name of the filter required.</param>
		/// <returns>The instance of <typeparamref name="T"/> named <paramref name="name"/> in 
		/// the filters collection, or <see langword="null"/> if there is no such instance.</returns>
		public T GetFilter<T>(string name)
			where T : class, ILogFilter
		{
			return this.filter.GetFilter<T>(name);
		}

		/// <summary>
		/// Returns the filter named <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the filter required.</param>
		/// <returns>The filter named <paramref name="name"/> in 
		/// the filters collection, or <see langword="null"/> if there is no such filter.</returns>
		public ILogFilter GetFilter(string name)
		{
			return this.filter.GetFilter(name);
		}

		/// <summary>
		/// Queries whether logging is enabled.
		/// </summary>
		/// <returns><b>true</b> if logging is enabled.</returns>
		public bool IsLoggingEnabled()
		{
			LogEnabledFilter enabledFilter = this.filter.GetFilter<LogEnabledFilter>();
			return enabledFilter == null || enabledFilter.Enabled;
		}

		/// <summary>
		/// Queries whether tracing is enabled.
		/// </summary>
		/// <returns><b>true</b> if tracing is enabled.</returns>
		public bool IsTracingEnabled()
		{
			return structureHolder.TracingEnabled;
		}

		/// <summary>
		/// Queries whether a <see cref="LogEntry"/> shold be logged.
		/// </summary>
		/// <param name="log">The log entry to check.</param>
		/// <returns><b>true</b> if the entry should be logged.</returns>
		public bool ShouldLog(LogEntry log)
		{
			return this.filter.CheckFilters(log);
		}

		private void ProcessLog(LogEntry log)
		{
			ContextItems items = new ContextItems();
			items.ProcessContextItems(log);

			IEnumerable<LogSource> matchingTraceSources = GetMatchingTraceSources(log);
			TraceListenerFilter traceListenerFilter = new TraceListenerFilter();

			foreach (LogSource traceSource in matchingTraceSources)
			{
				try
				{
					traceSource.TraceData(log.Severity, log.EventId, log, traceListenerFilter);
				}
				catch (Exception ex)
				{
					ReportExceptionDuringTracing(ex, log, traceSource);
				}
			}
		}

		internal IEnumerable<LogSource> GetMatchingTraceSources(LogEntry logEntry)
		{
			structureHolderLock.AcquireReaderLock(readerLockAcquireTimeout);
			try
			{
				return DoGetMatchingTraceSources(logEntry);
			}
			finally
			{
				structureHolderLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Returns the collection of <see cref="LogSource"/>s that matches the collection of categories provided.
		/// </summary>
		/// <param name="logEntry">The log entry.</param>
		/// <returns>The matching <see cref="LogSource"/>s</returns>
		private IEnumerable<LogSource> DoGetMatchingTraceSources(LogEntry logEntry)
		{
			List<LogSource> matchingTraceSources = new List<LogSource>(logEntry.Categories.Count);
			List<string> missingCategories = new List<string>();
			LogSource traceSource;

			// match the categories to the receive's trace sources
			foreach (string category in logEntry.Categories)
			{
				structureHolder.TraceSources.TryGetValue(category, out traceSource);
				if (traceSource != null)
				{
					matchingTraceSources.Add(traceSource);
				}
				else
				{
					missingCategories.Add(category);
				}
			}

			// add the mandatory trace source, if defined
			// otherwise, add the not processed trace source if missing categories were detected
			if (IsValidTraceSource(structureHolder.AllEventsTraceSource))
			{
				matchingTraceSources.Add(structureHolder.AllEventsTraceSource);
			}
			else if (missingCategories.Count > 0)
			{
				if (IsValidTraceSource(structureHolder.NotProcessedTraceSource))
				{
					matchingTraceSources.Add(structureHolder.NotProcessedTraceSource);
				}
				else if (structureHolder.LogWarningsWhenNoCategoriesMatch)
				{
					ReportMissingCategories(missingCategories, logEntry);
				}
			}

			return matchingTraceSources;
		}

		/// <summary>
		/// Gets the <see cref="LogSource"/> mappings available for the <see cref="LogWriter"/>.
		/// </summary>
		internal IDictionary<string, LogSource> TraceSources
		{
			get { return structureHolder.TraceSources; }
		}

		private static bool IsValidTraceSource(LogSource traceSource)
		{
			return traceSource != null && traceSource.Listeners.Count > 0;
		}

		private void ReportExceptionDuringTracing(Exception exception, LogEntry log, LogSource traceSource)
		{
			try
			{
				NameValueCollection additionalInfo = new NameValueCollection();
				additionalInfo.Add(ExceptionFormatter.Header,
					string.Format(Resources.Culture, Resources.TraceSourceFailed, traceSource.Name));
				additionalInfo.Add(Resources.TraceSourceFailed2,
					string.Format(Resources.Culture, Resources.TraceSourceFailed3, log.ToString()));
				ExceptionFormatter formatter =
					new ExceptionFormatter(additionalInfo, Resources.DistributorEventLoggerDefaultApplicationName);

				LogEntry reportingLogEntry = new LogEntry();
				reportingLogEntry.Severity = TraceEventType.Error;
				reportingLogEntry.Message = formatter.GetMessage(exception);
				reportingLogEntry.EventId = LogWriterFailureEventID;

				structureHolder.ErrorsTraceSource.TraceData(reportingLogEntry.Severity, reportingLogEntry.EventId, reportingLogEntry);
			}
			catch (Exception ex)
			{
				instrumentationProvider.FireFailureLoggingErrorEvent(Resources.FailureWhileTracing, ex);
			}
		}

		private void ReportExceptionCheckingFilters(Exception exception, LogEntry log, ILogFilter logFilter)
		{
			try
			{
				NameValueCollection additionalInfo = new NameValueCollection();
				additionalInfo.Add(ExceptionFormatter.Header,
					string.Format(Resources.Culture, Resources.FilterEvaluationFailed, logFilter.Name));
				additionalInfo.Add(Resources.FilterEvaluationFailed2,
					string.Format(Resources.Culture, Resources.FilterEvaluationFailed3, log.ToString()));
				ExceptionFormatter formatter =
					new ExceptionFormatter(additionalInfo, Resources.DistributorEventLoggerDefaultApplicationName);

				LogEntry reportingLogEntry = new LogEntry();
				reportingLogEntry.Severity = TraceEventType.Error;
				reportingLogEntry.Message = formatter.GetMessage(exception);
				reportingLogEntry.EventId = LogWriterFailureEventID;

				structureHolder.ErrorsTraceSource.TraceData(reportingLogEntry.Severity, reportingLogEntry.EventId, reportingLogEntry);
			}
			catch (Exception ex)
			{
				instrumentationProvider.FireFailureLoggingErrorEvent(Resources.FailureWhileCheckingFilters, ex);
			}
		}

		private void ReportUnknownException(Exception exception, LogEntry log)
		{
			try
			{
				NameValueCollection additionalInfo = new NameValueCollection();
				additionalInfo.Add(ExceptionFormatter.Header, Resources.ProcessMessageFailed);
				additionalInfo.Add(Resources.ProcessMessageFailed2,
					string.Format(Resources.Culture, Resources.ProcessMessageFailed3, log.ToString()));
				ExceptionFormatter formatter =
					new ExceptionFormatter(additionalInfo, Resources.DistributorEventLoggerDefaultApplicationName);

				LogEntry reportingLogEntry = new LogEntry();
				reportingLogEntry.Severity = TraceEventType.Error;
				reportingLogEntry.Message = formatter.GetMessage(exception);
				reportingLogEntry.EventId = LogWriterFailureEventID;

				structureHolder.ErrorsTraceSource.TraceData(reportingLogEntry.Severity, reportingLogEntry.EventId, reportingLogEntry);
			}
			catch (Exception ex)
			{
				instrumentationProvider.FireFailureLoggingErrorEvent(Resources.UnknownFailure, ex);
			}
		}

		private void ReportMissingCategories(List<string> missingCategories, LogEntry logEntry)
		{
			try
			{
				LogEntry reportingLogEntry = new LogEntry();
				reportingLogEntry.Severity = TraceEventType.Error;
				reportingLogEntry.Message = string.Format(Resources.Culture, Resources.MissingCategories, TextFormatter.FormatCategoriesCollection(missingCategories), logEntry.ToString());
				reportingLogEntry.EventId = LogWriterFailureEventID;

				structureHolder.ErrorsTraceSource.TraceData(reportingLogEntry.Severity, reportingLogEntry.EventId, reportingLogEntry);
			}
			catch (Exception ex)
			{
				instrumentationProvider.FireFailureLoggingErrorEvent(Resources.FailureWhileReportingMissingCategories, ex);
			}
		}

		internal void ReportConfigurationFailure(System.Configuration.ConfigurationErrorsException configurationException)
		{
			instrumentationProvider.FireConfigurationFailureEvent(configurationException);
		}

		private static IDictionary<string, LogSource> CreateTraceSourcesDictionary(ICollection<LogSource> traceSources)
		{
			IDictionary<string, LogSource> result = new Dictionary<string, LogSource>(traceSources.Count);

			foreach (LogSource source in traceSources)
			{
				result.Add(source.Name, source);
			}

			return result;
		}

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Performs any action to handle an error during checking.
		/// </summary>
		/// <param name="ex">The exception raised during filter evaluation.</param>
		/// <param name="logEntry">The log entry being evaluated.</param>
		/// <param name="filter">The fiter that raised the exception.</param>
		/// <returns>True signaling processing should continue.</returns>
		public bool FilterCheckingFailed(Exception ex, LogEntry logEntry, ILogFilter filter)
		{
			ReportExceptionCheckingFilters(ex, logEntry, filter);
			return true;
		}

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Returns the object that provides instrumentation services for the <see cref="LogWriter"/>.
		/// </summary>
		/// <see cref="IInstrumentationEventProvider.GetInstrumentationEventProvider()"/>
		/// <returns>The object that providers intrumentation services.</returns>
		public object GetInstrumentationEventProvider()
		{
			return instrumentationProvider;
		}

		/// <summary>
		/// Releases the resources used by the <see cref="LogWriter"/>.
		/// </summary>
		public void Dispose()
		{
			structureHolder.Dispose();
			if (structureUpdater != null) structureUpdater.Dispose();
		}
	}
}