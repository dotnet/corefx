// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

namespace System.Runtime.InteropServices.WindowsRuntime
{
    internal static class ExceptionSupport
    {
        public static Exception AttachRestrictedErrorInfo(Exception e)
        {
            // If there is no exception, then the restricted error info doesn't apply to it
            if (e != null)
            {
                try
                {
                    // Get the restricted error info for this thread and see if it may correlate to the current
                    // exception object.  Note that in general the thread's IRestrictedErrorInfo is not meant for
                    // exceptions that are marshaled Windows.Foundation.HResults and instead are intended for
                    // HRESULT ABI return values.   However, in many cases async APIs will set the thread's restricted
                    // error info as a convention in order to provide extended debugging information for the ErrorCode
                    // property.
                    IRestrictedErrorInfo restrictedErrorInfo = Interop.mincore.GetRestrictedErrorInfo();
                    if (restrictedErrorInfo != null)
                    {
                        string description;
                        string restrictedDescription;
                        string capabilitySid;
                        int restrictedErrorInfoHResult;
                        restrictedErrorInfo.GetErrorDetails(out description,
                                                            out restrictedErrorInfoHResult,
                                                            out restrictedDescription,
                                                            out capabilitySid);

                        // Since this is a special case where by convention there may be a correlation, there is not a
                        // guarantee that the restricted error info does belong to the async error code.  In order to
                        // reduce the risk that we associate incorrect information with the exception object, we need
                        // to apply a heuristic where we attempt to match the current exception's HRESULT with the
                        // HRESULT the IRestrictedErrorInfo belongs to.  If it is a match we will assume association
                        // for the IAsyncInfo case.
                        if (e.HResult == restrictedErrorInfoHResult)
                        {
                            string errorReference;
                            restrictedErrorInfo.GetReference(out errorReference);

                            e.AddExceptionDataForRestrictedErrorInfo(restrictedDescription,
                                                                     errorReference,
                                                                     capabilitySid,
                                                                     restrictedErrorInfo);
                        }
                    }
                }
                catch
                {
                    // If we can't get the restricted error info, then proceed as if it isn't associated with this
                    // error.
                }
            }

            return e;
        }

        internal static void AddExceptionDataForRestrictedErrorInfo(
            this Exception ex,
            string restrictedError,
            string restrictedErrorReference,
            string restrictedCapabilitySid,
            object restrictedErrorObject,
            bool hasrestrictedLanguageErrorObject = false)
        {
            IDictionary dict = ex.Data;
            if (dict != null)
            {
                dict.Add("RestrictedDescription", restrictedError);
                dict.Add("RestrictedErrorReference", restrictedErrorReference);
                dict.Add("RestrictedCapabilitySid", restrictedCapabilitySid);

                // Keep the error object alive so that user could retrieve error information
                // using Data["RestrictedErrorReference"]
                dict.Add("__RestrictedErrorObject", (restrictedErrorObject == null ? null : new __RestrictedErrorObject(restrictedErrorObject)));
                dict.Add("__HasRestrictedLanguageErrorObject", hasrestrictedLanguageErrorObject);
            }
        }

        public static bool ReportUnhandledError(Exception ex)
        {
            return WindowsRuntimeMarshal.ReportUnhandledError(ex);
        }

        //
        // Exception requires anything to be added into Data dictionary is serializable
        // This wrapper is made serializable to satisfy this requirement but does NOT serialize
        // the object and simply ignores it during serialization, because we only need
        // the exception instance in the app to hold the error object alive.
        // Once the exception is serialized to debugger, debugger only needs the error reference string
        //
        [Serializable]
        internal class __RestrictedErrorObject
        {
            // Hold the error object instance but don't serialize/deserialize it
            [NonSerialized]
            private readonly object _realErrorObject;

            internal __RestrictedErrorObject(object errorObject)
            {
                _realErrorObject = errorObject;
            }

            public object RealErrorObject
            {
                get
                {
                    return _realErrorObject;
                }
            }
        }
    }
}
