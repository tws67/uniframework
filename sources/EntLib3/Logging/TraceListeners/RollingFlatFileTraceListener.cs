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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
	/// <summary>
	/// Performs logging to a file and rolls the output file when either time or size thresholds are 
	/// exceeded.
	/// </summary>
	/// <remarks>
	/// Logging always occurs to the configured file name, and when roll occurs a new rolled file name is calculated
	/// by adding the timestamp pattern to the configured file name.
	/// <para/>
	/// The need of rolling is calculated before performing a logging operation, so even if the thresholds are exceeded
	/// roll will not occur until a new entry is logged.
	/// <para/>
	/// Both time and size thresholds can be configured, and when the first of them occurs both will be reset.
	/// <para/>
	/// The elapsed time is calculated from the creation date of the logging file.
	/// </remarks>
	public class RollingFlatFileTraceListener : FlatFileTraceListener
	{
		private StreamWriterRollingHelper rollingHelper;

		private string timeStampPattern;
		private int rollSizeInBytes;
		private RollInterval rollInterval;
		private RollFileExistsBehavior rollFileExistsBehavior;

		/// <summary>
		/// Initializes a new instance of <see cref="RollingFlatFileTraceListener"/> 
		/// </summary>
		/// <param name="fileName">The filename where the entries will be logged.</param>
		/// <param name="header">The header to add before logging an entry.</param>
		/// <param name="footer">The footer to add after logging an entry.</param>
		/// <param name="formatter">The formatter.</param>
		/// <param name="rollSizeKB">The maxium file size (KB) before rolling.</param>
		/// <param name="timeStampPattern">The date format that will be appended to the new roll file.</param>
		/// <param name="rollFileExistsBehavior">Expected behavior that will be used when the rool file has to be created.</param>
		/// <param name="rollInterval">The time interval that makes the file rolles.</param>
		public RollingFlatFileTraceListener(string fileName,
			string header,
			string footer,
			ILogFormatter formatter,
			int rollSizeKB,
			string timeStampPattern,
			RollFileExistsBehavior rollFileExistsBehavior,
			RollInterval rollInterval)
			: base(fileName, header, footer, formatter)
		{
			this.rollSizeInBytes = rollSizeKB * 1024;
			this.timeStampPattern = timeStampPattern;
			this.rollFileExistsBehavior = rollFileExistsBehavior;
			this.rollInterval = rollInterval;

			this.rollingHelper = new StreamWriterRollingHelper(this);
		}

		/// <summary>
		/// Writes trace information, a data object and event information to the file, performing a roll if necessary.
		/// </summary>
		/// <param name="eventCache">A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">The trace data to emit.</param>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			this.rollingHelper.RollIfNecessary();

			base.TraceData(eventCache, source, eventType, id, data);
		}

		#region test only properties

		internal StreamWriterRollingHelper RollingHelper
		{
			get { return this.rollingHelper; }
		}

		#endregion

		internal sealed class TallyKeepingFileStreamWriter : StreamWriter
		{
			private long tally;

			public TallyKeepingFileStreamWriter(FileStream stream)
				: base(stream)
			{
				tally = stream.Length;
			}

			public TallyKeepingFileStreamWriter(FileStream stream, Encoding encoding)
				: base(stream, encoding)
			{
				tally = stream.Length;
			}

			public override void Write(char value)
			{
				base.Write(value);
				this.tally += this.Encoding.GetByteCount(new char[] { value });
			}

			public override void Write(char[] buffer)
			{
				base.Write(buffer);
				this.tally += this.Encoding.GetByteCount(buffer);
			}

			public override void Write(char[] buffer, int index, int count)
			{
				base.Write(buffer, index, count);
				this.tally += this.Encoding.GetByteCount(buffer, index, count);
			}

			public override void Write(string value)
			{
				base.Write(value);
				this.tally += this.Encoding.GetByteCount(value);
			}

			public long Tally
			{
				get { return tally; }
			}
		}

		/// <summary>
		/// Encapsulates the logic to perform rolls.
		/// </summary>
		/// <remarks>
		/// If no rolling behavior has been configured no further processing will be performed.
		/// </remarks>
		internal sealed class StreamWriterRollingHelper
		{
			/// <summary>
			/// The trace listener for which rolling is being managed.
			/// </summary>
			private RollingFlatFileTraceListener owner;

			/// <summary>
			/// A flag indicating whether at least one rolling criteria has been configured.
			/// </summary>
			private bool performsRolling;
			/// <summary>
			/// The provider for the current date. Necessary for unit testing.
			/// </summary>
			private DateTimeProvider dateTimeProvider;

			/// <summary>
			/// A tally keeping writer used when file size rolling is configured.<para/>
			/// The original stream writer from the base trace listener will be replaced with
			/// this listener.
			/// </summary>
			private TallyKeepingFileStreamWriter managedWriter;
			/// <summary>
			/// The next date when date based rolling should occur if configured.
			/// </summary>
			private DateTime? nextRollDateTime;

			public StreamWriterRollingHelper(RollingFlatFileTraceListener owner)
			{
				this.owner = owner;
				this.dateTimeProvider = new DateTimeProvider();

				this.performsRolling = this.owner.rollInterval != RollInterval.None || this.owner.rollSizeInBytes > 0;
			}

			public void RollIfNecessary()
			{
				if (!this.performsRolling)
				{
					// avoid further processing if no rolling has been configured.
					return;
				}

				if (!UpdateRollingInformationIfNecessary())
				{
					// an error was detected while handling roll information - avoid further processing
					return;
				}

				DateTime? rollDateTime;
				if ((rollDateTime = this.CheckIsRollNecessary()) != null)
				{
					this.PerformRoll(rollDateTime.Value);
				}
			}

			/// <summary>
			/// Updates bookeeping information necessary for rolling, as required by the specified
			/// rolling configuration.
			/// </summary>
			/// <returns>true if update was successful, false if an error occurred.</returns>
			public bool UpdateRollingInformationIfNecessary()
			{
				StreamWriter currentWriter = null;

				// replace writer with the tally keeping version if necessary for size rolling
				if (this.owner.rollSizeInBytes > 0 && this.managedWriter == null)
				{
					currentWriter = owner.Writer as StreamWriter;
					if (currentWriter == null)
					{
						// TWTL couldn't acquire the writer - abort
						return false;
					}
					String actualFileName = ((FileStream)currentWriter.BaseStream).Name;

					currentWriter.Close();

					FileStream fileStream = null;
					try
					{
						fileStream = File.Open(actualFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
						this.managedWriter = new TallyKeepingFileStreamWriter(fileStream, GetEncodingWithFallback());
						this.owner.Writer = this.managedWriter;
					}
					catch (Exception)
					{
						// there's a slight chance of error here - abort if this occurs and just let TWTL handle it without attempting to roll
						return false;
					}
				}

				// compute the next roll date if necessary
				if (this.owner.rollInterval != RollInterval.None && this.nextRollDateTime == null)
				{
					try
					{
						// casting should be safe at this point - only file stream writers can be the writers for the owner trace listener.
						// it should also happen rarely
						this.nextRollDateTime
							= CalculateNextRollDate(File.GetCreationTime(((FileStream)((StreamWriter)this.owner.Writer).BaseStream).Name));
					}
					catch (Exception)
					{
						this.nextRollDateTime = DateTime.MaxValue;		// disable rolling if not date could be retrieved.

						// there's a slight chance of error here - abort if this occurs and just let TWTL handle it without attempting to roll
						return false;
					}
				}

				return true;
			}

			/// <summary>
			/// Checks whether rolling should be performed, and returns the date to use when performing the roll.
			/// </summary>
			/// <returns>The date roll to use if performing a roll, or <see langword="null"/> if no rolling should occur.</returns>
			/// <remarks>
			/// Defer request for the roll date until it is necessary to avoid overhead.<para/>
			/// Information used for rolling checks should be set by now.
			/// </remarks>
			public DateTime? CheckIsRollNecessary()
			{
				// check for size roll, if enabled.
				if (this.owner.rollSizeInBytes > 0
					&& (this.managedWriter != null && this.managedWriter.Tally > this.owner.rollSizeInBytes))
				{
					return this.dateTimeProvider.CurrentDateTime;
				}

				// check for date roll, if enabled.
				DateTime currentDateTime = this.dateTimeProvider.CurrentDateTime;
				if (this.owner.rollInterval != RollInterval.None
					&& (this.nextRollDateTime != null && currentDateTime.CompareTo(this.nextRollDateTime.Value) >= 0))
				{
					return currentDateTime;
				}

				// no roll is necessary, return a null roll date
				return null;
			}

			public void PerformRoll(DateTime rollDateTime)
			{
				string actualFileName = ((FileStream)((StreamWriter)this.owner.Writer).BaseStream).Name;

				// calculate archive name
				string archiveFileName = this.ComputeArchiveFileName(actualFileName, rollDateTime);
				// close file
				this.owner.Writer.Close();
				// move file
				this.SafeMove(actualFileName, archiveFileName, rollDateTime);
				// update writer - let TWTL open the file as needed to keep consistency
				this.owner.Writer = null;
				this.managedWriter = null;
				this.nextRollDateTime = null;
				this.UpdateRollingInformationIfNecessary();
			}

			private void SafeMove(string actualFileName, string archiveFileName, DateTime currentDateTime)
			{
				try
				{
					if (File.Exists(archiveFileName))
					{
						File.Delete(archiveFileName);
					}
					// take care of tunneling issues http://support.microsoft.com/kb/172190
					File.SetCreationTime(actualFileName, currentDateTime);
					File.Move(actualFileName, archiveFileName);
				}
				catch (IOException)
				{
					// catch errors and attempt move to a new file with a GUID
					archiveFileName = archiveFileName + Guid.NewGuid().ToString();

					try
					{
						File.Move(actualFileName, archiveFileName);
					}
					catch (IOException) { }
				}
			}

			public string ComputeArchiveFileName(string actualFileName, DateTime currentDateTime)
			{
				string archiveFileName = null;

				string directory = Path.GetDirectoryName(actualFileName);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(actualFileName);
				string extension = Path.GetExtension(actualFileName);

				string archiveFileNameWithTimestampWithoutExtension = fileNameWithoutExtension
					+ "."
					+ currentDateTime.ToString(this.owner.timeStampPattern, CultureInfo.InvariantCulture);

				if (this.owner.rollFileExistsBehavior == RollFileExistsBehavior.Overwrite)
				{
					archiveFileName = Path.Combine(directory, archiveFileNameWithTimestampWithoutExtension + extension);
				}
				else
				{
					// look for max sequence for date
					int maxSequence = FindMaxSequenceNumber(directory, archiveFileNameWithTimestampWithoutExtension, extension);
					archiveFileName = Path.Combine(directory, archiveFileNameWithTimestampWithoutExtension + "." + (maxSequence + 1) + extension);
				}

				return archiveFileName;
			}

			public static int FindMaxSequenceNumber(string directoryName, string fileName, string extension)
			{
				string[] existingFiles = Directory.GetFiles(directoryName,
					string.Format("{0}*{1}", fileName, extension));

				int maxSequence = 0;
				Regex regex = new Regex(string.Format(@"{0}\.(?<sequence>\d+){1}", fileName, extension));
				for (int i = 0; i < existingFiles.Length; i++)
				{
					Match sequenceMatch = regex.Match(existingFiles[i]);
					if (sequenceMatch.Success)
					{
						int currentSequence = 0;

						string sequenceInFile = sequenceMatch.Groups["sequence"].Value;
						if (!int.TryParse(sequenceInFile, out currentSequence))
							continue;		// very unlikely

						if (currentSequence > maxSequence)
						{
							maxSequence = currentSequence;
						}
					}
				}

				return maxSequence;
			}

			public DateTime CalculateNextRollDate(DateTime dateTime)
			{
				switch (this.owner.rollInterval)
				{
					case RollInterval.Minute:
						return dateTime.AddMinutes(1);
					case RollInterval.Hour:
						return dateTime.AddHours(1);
					case RollInterval.Day:
						return dateTime.AddDays(1);
					case RollInterval.Week:
						return dateTime.AddDays(7);
					case RollInterval.Month:
						return dateTime.AddMonths(1);
					case RollInterval.Year:
						return dateTime.AddYears(1);
					default:
						return DateTime.MaxValue;
				}
			}

			private static Encoding GetEncodingWithFallback()
			{
				Encoding encoding = (Encoding)new UTF8Encoding(false).Clone();
				encoding.EncoderFallback = EncoderFallback.ReplacementFallback;
				encoding.DecoderFallback = DecoderFallback.ReplacementFallback;
				return encoding;
			}

			public DateTimeProvider DateTimeProvider
			{
				set { this.dateTimeProvider = value; }
			}

			public DateTime? NextRollDateTime
			{
				get { return this.nextRollDateTime; }
			}
		}

		internal class DateTimeProvider
		{
			public virtual DateTime CurrentDateTime
			{
				get { return DateTime.Now; }
			}
		}
	}
}