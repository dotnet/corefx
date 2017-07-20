// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Diagnostics;

internal static class DataSetUtil
{
    #region CheckArgument
    internal static void CheckArgumentNull<T>(T argumentValue, string argumentName) where T : class
    {
        if (null == argumentValue)
        {
            throw ArgumentNull(argumentName);
        }
    }
    #endregion

    #region Trace
    private static T TraceException<T>(string trace, T e)
    {
        Debug.Assert(null != e, "TraceException: null Exception");
        if (null != e)
        {
            //Bid.Trace(trace, e.ToString()); // will include callstack if permission is available
        }
        return e;
    }

    private static T TraceExceptionAsReturnValue<T>(T e)
    {
        return TraceException("<comm.ADP.TraceException|ERR|THROW> '%ls'\n", e);
    }
    #endregion

    #region new Exception
    internal static ArgumentException Argument(string message)
    {
        return TraceExceptionAsReturnValue(new ArgumentException(message));
    }

    internal static ArgumentNullException ArgumentNull(string message)
    {
        return TraceExceptionAsReturnValue(new ArgumentNullException(message));
    }

    internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
    {
        return TraceExceptionAsReturnValue(new ArgumentOutOfRangeException(parameterName, message));
    }

    internal static InvalidCastException InvalidCast(string message)
    {
        return TraceExceptionAsReturnValue(new InvalidCastException(message));
    }

    internal static InvalidOperationException InvalidOperation(string message)
    {
        return TraceExceptionAsReturnValue(new InvalidOperationException(message));
    }

    internal static NotSupportedException NotSupported(string message)
    {
        return TraceExceptionAsReturnValue(new NotSupportedException(message));
    }
    #endregion

    #region new EnumerationValueNotValid
    static internal ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
    {
        return ArgumentOutOfRange(Strings.DataSetLinq_InvalidEnumerationValue(type.Name, value.ToString(System.Globalization.CultureInfo.InvariantCulture)), type.Name);
    }

    static internal ArgumentOutOfRangeException InvalidDataRowState(DataRowState value)
    {
#if DEBUG
        switch (value)
        {
            case DataRowState.Detached:
            case DataRowState.Unchanged:
            case DataRowState.Added:
            case DataRowState.Deleted:
            case DataRowState.Modified:
                Debug.Assert(false, "valid DataRowState " + value.ToString());
                break;
        }
#endif
        return InvalidEnumerationValue(typeof(DataRowState), (int)value);
    }

    static internal ArgumentOutOfRangeException InvalidLoadOption(LoadOption value)
    {
#if DEBUG
        switch (value)
        {
            case LoadOption.OverwriteChanges:
            case LoadOption.PreserveChanges:
            case LoadOption.Upsert:
                Debug.Assert(false, "valid LoadOption " + value.ToString());
                break;
        }
#endif
        return InvalidEnumerationValue(typeof(LoadOption), (int)value);
    }
    #endregion

    // only StackOverflowException & ThreadAbortException are sealed classes
    static private readonly Type StackOverflowType = typeof(System.StackOverflowException);
    static private readonly Type OutOfMemoryType = typeof(System.OutOfMemoryException);
    static private readonly Type ThreadAbortType = typeof(System.Threading.ThreadAbortException);
    static private readonly Type NullReferenceType = typeof(System.NullReferenceException);
    static private readonly Type AccessViolationType = typeof(System.AccessViolationException);
    static private readonly Type SecurityType = typeof(System.Security.SecurityException);

    static internal bool IsCatchableExceptionType(Exception e)
    {
        // a 'catchable' exception is defined by what it is not.
        Type type = e.GetType();

        return ((type != StackOverflowType) &&
                 (type != OutOfMemoryType) &&
                 (type != ThreadAbortType) &&
                 (type != NullReferenceType) &&
                 (type != AccessViolationType) &&
                 !SecurityType.IsAssignableFrom(type));
    }
}