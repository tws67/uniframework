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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Messaging;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal class MsmqTraceListenerDataManageabilityProvider
		: TraceListenerDataManageabilityProvider<MsmqTraceListenerData>
	{
		public const String MessagePriorityPropertyName = "messagePriority";
		public const String QueuePathPropertyName = "queuePath";
		public const String RecoverablePropertyName = "recoverable";
		public const String TimeToBeReceivedPropertyName = "timeToBeReceived";
		public const String TimeToReachQueuePropertyName = "timeToReachQueue";
		public const String TransactionTypePropertyName = "transactionType";
		public const String UseAuthenticationPropertyName = "useAuthentication";
		public const String UseDeadLetterQueuePropertyName = "useDeadLetterQueue";
		public const String UseEncryptionPropertyName = "useEncryption";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			MsmqTraceListenerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.MsmqTraceListenerQueuePathPartName,
				QueuePathPropertyName,
				configurationObject.QueuePath,
				255,
				true);

			contentBuilder.AddDropDownListPartForEnumeration<MessagePriority>(Resources.MsmqTraceListenerPriorityPartName,
				MessagePriorityPropertyName,
				configurationObject.MessagePriority);

			contentBuilder.AddEditTextPart(Resources.MsmqTraceListenerTtbrPartName,
				TimeToBeReceivedPropertyName,
				Convert.ToString(configurationObject.TimeToBeReceived, CultureInfo.InvariantCulture),
				255,
				false);

			contentBuilder.AddEditTextPart(Resources.MsmqTraceListenerTtrqPartName,
				TimeToReachQueuePropertyName,
				Convert.ToString(configurationObject.TimeToReachQueue, CultureInfo.InvariantCulture),
				255,
				false);

			contentBuilder.AddCheckboxPart(Resources.MsmqTraceListenerRecoverablePartName,
				RecoverablePropertyName,
				configurationObject.Recoverable);

			contentBuilder.AddDropDownListPartForEnumeration<MessageQueueTransactionType>(Resources.MsmqTraceListenerTransactionTypePartName,
				TransactionTypePropertyName,
				configurationObject.TransactionType);

			contentBuilder.AddCheckboxPart(Resources.MsmqTraceListenerUseAuthenticationPartName,
				UseAuthenticationPropertyName,
				configurationObject.UseAuthentication);

			contentBuilder.AddCheckboxPart(Resources.MsmqTraceListenerUseDeadLetterQueuePartName,
				UseDeadLetterQueuePropertyName,
				configurationObject.UseDeadLetterQueue);

			contentBuilder.AddCheckboxPart(Resources.MsmqTraceListenerUseEncryptionPartName,
				UseEncryptionPropertyName,
				configurationObject.UseEncryption);

			AddTraceOptionsPart(contentBuilder, configurationObject.TraceOutputOptions);

			AddFormattersPart(contentBuilder, configurationObject.Formatter, configurationSource);
		}

		protected override void OverrideWithGroupPolicies(MsmqTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			String formatterOverride = GetFormatterPolicyOverride(policyKey);
			MessagePriority? messagePriorityOverride = policyKey.GetEnumValue<MessagePriority>(MessagePriorityPropertyName);
			String queuePathOverride = policyKey.GetStringValue(QueuePathPropertyName);
			bool? recoverableOverride = policyKey.GetBoolValue(RecoverablePropertyName);
			TimeSpan timeToBeReceivedOverride = GetTimeSpanOverride(policyKey, TimeToBeReceivedPropertyName);
			TimeSpan timeToReachQueueOverride = GetTimeSpanOverride(policyKey, TimeToReachQueuePropertyName);
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);
			MessageQueueTransactionType? transactionTypeOverride
				= policyKey.GetEnumValue<MessageQueueTransactionType>(TransactionTypePropertyName);
			bool? usedAuthenticationOverride = policyKey.GetBoolValue(UseAuthenticationPropertyName);
			bool? useDeadLetterOverride = policyKey.GetBoolValue(UseDeadLetterQueuePropertyName);
			bool? useEncryptionOverride = policyKey.GetBoolValue(UseEncryptionPropertyName);


			configurationObject.Formatter = formatterOverride;
			configurationObject.MessagePriority = messagePriorityOverride.Value;
			configurationObject.QueuePath = queuePathOverride;
			configurationObject.Recoverable = recoverableOverride.Value;
			configurationObject.TimeToReachQueue = timeToReachQueueOverride;
			configurationObject.TimeToBeReceived = timeToBeReceivedOverride;
			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
			configurationObject.TransactionType = transactionTypeOverride.Value;
			configurationObject.UseAuthentication = usedAuthenticationOverride.Value;
			configurationObject.UseDeadLetterQueue = useDeadLetterOverride.Value;
			configurationObject.UseEncryption = useEncryptionOverride.Value;
		}

		private TimeSpan GetTimeSpanOverride(IRegistryKey policyKey, String propertyName)
		{
			TimeSpan result;

			String overrideValue = policyKey.GetStringValue(propertyName);
			if (!TimeSpan.TryParse(overrideValue, out result))
			{
				throw new RegistryAccessException(
					String.Format(Resources.Culture,
						Resources.ExceptionErrorValueNotTimeSpan,
						policyKey.Name,
						propertyName,
						overrideValue));
			}

			return result;
		}

		protected override void GenerateWmiObjects(MsmqTraceListenerData configurationObject, ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new MsmqTraceListenerSetting(configurationObject.Name,
					configurationObject.Formatter,
					configurationObject.MessagePriority.ToString(),
					configurationObject.QueuePath,
					configurationObject.Recoverable,
					Convert.ToString(configurationObject.TimeToBeReceived, CultureInfo.CurrentCulture),
					Convert.ToString(configurationObject.TimeToReachQueue, CultureInfo.CurrentCulture),
					configurationObject.TraceOutputOptions.ToString(),
					configurationObject.TransactionType.ToString(),
					configurationObject.UseAuthentication,
					configurationObject.UseDeadLetterQueue,
					configurationObject.UseEncryption));
		}
	}
}
