using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DesktopTestData
{
    public static class ExceptionHelpers
    {
        public static void CheckForException(Type exceptionType, MethodDelegate tryCode)
        {
            ExceptionHelpers.CheckForException(exceptionType, new Dictionary<string, string>(), tryCode, false);
        }

        public static void CheckForException(Type exceptionType, string message, MethodDelegate tryCode)
        {
            ExceptionHelpers.CheckForException(exceptionType, message, tryCode, false);
        }

        public static void CheckForException(Type exceptionType, string message, MethodDelegate tryCode, bool checkValidationException)
        {
            Dictionary<string, string> exceptionProperties = new Dictionary<string, string>();
            exceptionProperties.Add("Message", message);
            ExceptionHelpers.CheckForException(exceptionType, exceptionProperties, tryCode, checkValidationException);
        }

        public static void CheckForException(
            Type exceptionType,
            Dictionary<string, string> exceptionProperties,
            MethodDelegate tryCode,
            bool checkValidationException)
        {
            /*
            if (exceptionType == typeof(System.Activities.ValidationException)
                && checkValidationException == false)
            {
                throw new Exception("Please do not use this method to check for ValidationExceptions and"
                            + "use \"TestRuntime.ValidateWorkflowErrors\" method instead.");
            }
            */

            // The normal state for this method is for an exception to be thrown
            bool exceptionThrown = true;
            try
            {
                // call delegate
                tryCode();

                // We did not get an exception.  Normally we would throw here, but due to the
                // catch handler below we set the flag and throw outside the try block
                exceptionThrown = false;
            }
            catch (Exception exc) // jasonv - approved; delegate may throw any exception; we rethrow as inner in case of failure
            {
                ValidateException(exc, exceptionType, exceptionProperties);
            }

            if (!exceptionThrown)
            {
                throw new ValidateExceptionFailed(string.Format(
                    "Expected {0} to be thrown, but no exception was thrown.",
                    exceptionType.FullName));
            }
        }

        public static void ValidateException(
            Exception exception,
            Type exceptionType,
            Dictionary<string, string> exceptionProperties)
        {
            // check for exception type mismatch
            if (exception.GetType().FullName != exceptionType.FullName)
            {
                throw new ValidateExceptionFailed(String.Format(
                    "Expected {0} to be thrown, but {1} was thrown.",
                    exceptionType.FullName,
                    exception.GetType().FullName),
                    exception);
            }

            // check for property values
            if (exceptionProperties != null)
            {
                foreach (string propertyName in exceptionProperties.Keys)
                {
                    string expectedPropertyValue = exceptionProperties[propertyName];

                    PropertyInfo pi = exception.GetType().GetProperty(propertyName);

                    if (pi == null)
                    {
                        throw new Exception(String.Format(
                            "Test issue: {0} doesn't have a property {1}",
                            exceptionType.FullName,
                            propertyName),
                            exception);
                    }

                    object actualPropertyObjectValue = pi.GetValue(exception, null);

                    if (actualPropertyObjectValue == null)
                    {
                        throw new ValidateExceptionFailed(String.Format(
                            "Property {0} on {1} was expected to contain {2}. The actual value is null",
                            propertyName,
                            exceptionType.FullName,
                            expectedPropertyValue),
                            exception);
                    }

                    string actualPropertyValue = actualPropertyObjectValue.ToString();

                    if (!actualPropertyValue.Contains(expectedPropertyValue))
                    {
                        throw new ValidateExceptionFailed(String.Format(
                            "Property {0} on {1} was expected to contain {2}. The actual value is {3}",
                            propertyName,
                            exceptionType.FullName,
                            expectedPropertyValue,
                            actualPropertyValue),
                            exception);
                    }
                }
            }

            //Log.TraceInternal("Exception was validated successfully");
        }

        public delegate void MethodDelegate();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ValidateExceptionFailed : Exception
    {
        // Public ctor for serialization
        public ValidateExceptionFailed()
        {
        }

        protected ValidateExceptionFailed(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ValidateExceptionFailed(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ValidateExceptionFailed(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
