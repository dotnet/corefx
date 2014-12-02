// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Internal
{
    internal static class Requires
    {
        [DebuggerStepThrough]
        [ContractArgumentValidator]
        public static void NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            Contract.EndContractBlock();
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_EmptyString, parameterName), parameterName);
            }
            Contract.EndContractBlock();
        }
        //    [DebuggerStepThrough]
        //    [ContractArgumentValidator]
        //    public static void NotNullOrNullElements<T>(IEnumerable<T> values, string parameterName)
        //        where T : class
        //    {
        //        NotNull(values, parameterName);
        //        NotNullElements(values, parameterName);
        //        Contract.EndContractBlock();
        //    }

        //    [DebuggerStepThrough]
        //    [ContractArgumentValidator]
        //    public static void NullOrNotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
        //        where TKey : class
        //        where TValue : class
        //    {
        //        NotNullElements(values, parameterName);
        //        Contract.EndContractBlock();
        //    }

        //    [DebuggerStepThrough]
        //    [ContractArgumentValidator]
        //    public static void NullOrNotNullElements<T>(IEnumerable<T> values, string parameterName)
        //        where T : class
        //    {
        //        NotNullElements(values, parameterName);
        //        Contract.EndContractBlock();
        //    }

        //    [ContractArgumentValidator]
        //    private static void NotNullElements<T>(IEnumerable<T> values, string parameterName)
        //        where T : class
        //    {
        //        if (values != null && !Contract.ForAll(values, (value) => value != null))
        //        {
        //            throw ExceptionBuilder.CreateContainsNullElement(parameterName);
        //        }
        //        Contract.EndContractBlock();
        //    }

        //    [ContractArgumentValidator]
        //    private static void NotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
        //        where TKey : class
        //        where TValue : class
        //    {
        //        if (values != null && !Contract.ForAll(values, (keyValue) => keyValue.Key != null && keyValue.Value != null))
        //        {
        //            throw ExceptionBuilder.CreateContainsNullElement(parameterName);
        //        }
        //        Contract.EndContractBlock();
        //    }

        //    [DebuggerStepThrough]
        //    [ContractArgumentValidator]
        //    public static void IsInMembertypeSet(MemberTypes value, string parameterName, MemberTypes enumFlagSet)
        //    {
        //        if ((value & enumFlagSet) != value || // Ensure the member is in the set
        //            (value & (value - 1)) != 0) // Ensure that there is only one flag in the value (i.e. value is a power of 2).
        //        {
        //            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentOutOfRange_InvalidEnumInSet, parameterName, value, enumFlagSet.ToString()), parameterName);
        //        }
        //        Contract.EndContractBlock();
        //    }
    }
}
