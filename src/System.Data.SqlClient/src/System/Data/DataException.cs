// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



//------------------------------------------------------------------------------

using Res = System.SR;


namespace System.Data
{
    // These functions are major point of localization.
    // We need to have a rules to enforce consistency there.
    // The dangerous point there are the string arguments of the exported (internal) methods.
    // This string can be argument, table or constraint name but never text of exception itself.
    // Make an invariant that all texts of exceptions coming from resources only.


    internal static class ExceptionBuilder
    {
        // The class defines the exceptions that are specific to the DataSet.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource Data.txt.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error messages.
        // The resource Data.txt will ensure proper string text based on the appropriate
        // locale.

        static internal void TraceExceptionAsReturnValue(Exception e)
        {
        }

        //
        // COM+ exceptions
        //
        static internal ArgumentException _Argument(string error)
        {
            ArgumentException e = new ArgumentException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }
        static public Exception InvalidOffsetLength()
        {
            return _Argument(Res.GetString(Res.Data_InvalidOffsetLength));
        }
    }// ExceptionBuilder
}
