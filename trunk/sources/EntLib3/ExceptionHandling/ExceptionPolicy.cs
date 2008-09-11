//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
//===============================================================================
// Copyright © Microsoft Corporation. All rights reserved.
// Adapted from ACA.NET with permission from Avanade Inc.
// ACA.NET copyright © Avanade Inc. All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling
{
    /// <summary>
    /// Represents a policy with exception types and
    /// exception handlers. 
    /// </summary>
    public static class ExceptionPolicy
    {
        private static readonly ExceptionPolicyFactory defaultFactory = new ExceptionPolicyFactory();

        /// <summary>
        /// The main entry point into the Exception Handling Application Block.
        /// Handles the specified <see cref="Exception"/>
        /// object according to the given <paramref name="configurationContext"></paramref>.
        /// </summary>
        /// <param name="exceptionToHandle">An <see cref="Exception"/> object.</param>
        /// <param name="policyName">The name of the policy to handle.</param>        
        /// <returns>
        /// Whether or not a rethrow is recommended.
        /// </returns>
        /// <example>
        /// The following code shows the usage of the 
        /// exception handling framework.
        /// <code>
        /// try
        ///	{
        ///		Foo();
        ///	}
        ///	catch (Exception e)
        ///	{
        ///		if (ExceptionPolicy.HandleException(e, name)) throw;
        ///	}
        /// </code>
        /// </example>
        public static bool HandleException(Exception exceptionToHandle, string policyName)
        {
            if (exceptionToHandle == null) throw new ArgumentNullException("exceptionToHandle");
            if (string.IsNullOrEmpty(policyName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty);


            return HandleException(exceptionToHandle, policyName, defaultFactory);
        }

        private static bool HandleException(Exception exceptionToHandle, string policyName, ExceptionPolicyFactory policyFactory)
        {
            ExceptionPolicyImpl policy = GetExceptionPolicy(exceptionToHandle, policyName, policyFactory);
            return policy.HandleException(exceptionToHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionToHandle"></param>
        /// <param name="policyName"></param>
        /// <param name="exceptionToThrow"></param>
        /// <returns></returns>
        public static bool HandleException(Exception exceptionToHandle, string policyName, out Exception exceptionToThrow)
        {
            if (exceptionToHandle == null) throw new ArgumentNullException("exceptionToHandle");
            if (string.IsNullOrEmpty(policyName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty);

            return HandleException(exceptionToHandle, policyName, defaultFactory, out exceptionToThrow);
        }

        private static bool HandleException(Exception exceptionToHandle, string policyName, ExceptionPolicyFactory policyFactory, out Exception exceptionToThrow)
        {
            try
            {
                bool retrowAdviced = HandleException(exceptionToHandle, policyName, policyFactory);
                exceptionToThrow = null;

                return retrowAdviced;
            }
            catch (Exception exception)
            {
                exceptionToThrow = exception;
                return true;
            }
        }


        private static ExceptionPolicyImpl GetExceptionPolicy(Exception exception, string policyName, ExceptionPolicyFactory factory)
        {
            try
            {
                return factory.Create(policyName);
            }
            catch (ConfigurationErrorsException configurationException)
            {
                try
                {
                    DefaultExceptionHandlingEventLogger logger = EnterpriseLibraryFactory.BuildUp<DefaultExceptionHandlingEventLogger>();
                    logger.LogConfigurationError(configurationException, policyName);
                }
                catch { }

                throw;
            }
            catch (Exception ex)
            {
                try
                {
                    string exceptionMessage = ExceptionUtility.FormatExceptionHandlingExceptionMessage(policyName, ex, null, exception);

                    DefaultExceptionHandlingEventLogger logger = EnterpriseLibraryFactory.BuildUp<DefaultExceptionHandlingEventLogger>();
                    logger.LogInternalError(policyName, exceptionMessage);
                }
                catch { }

                throw new ExceptionHandlingException(ex.Message, ex);
            }
        }
    }
}
