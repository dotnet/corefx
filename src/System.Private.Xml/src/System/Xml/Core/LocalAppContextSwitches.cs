// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    internal partial class LocalAppContextSwitches
    {
        private const string DontThrowOnInvalidSurrogatePairsName = "DontThrowOnInvalidSurrogatePairs";
        private static int s_dontThrowOnInvalidSurrogatePairs;

        public static bool DontThrowOnInvalidSurrogatePairs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(DontThrowOnInvalidSurrogatePairsName, ref s_dontThrowOnInvalidSurrogatePairs);
            }
        }

        private const string IgnoreEmptyKeySequencesName = "IgnoreEmptyKeySequencess";
        private static int s_ignoreEmptyKeySequences;

        public static bool IgnoreEmptyKeySequences
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(IgnoreEmptyKeySequencesName, ref s_ignoreEmptyKeySequences);
            }
        }

        private const string IgnoreKindInUtcTimeSerializationName = "IgnoreKindInUtcTimeSerialization";
        private static int s_ignoreKindInUtcTimeSerialization;

        public static bool IgnoreKindInUtcTimeSerialization
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(IgnoreKindInUtcTimeSerializationName, ref s_ignoreKindInUtcTimeSerialization);
            }
        }

        private const string LimitXPathComplexityName = "LimitXPathComplexity";
        private static int s_limitXPathComplexity;

        public static bool LimitXPathComplexity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(LimitXPathComplexityName, ref s_limitXPathComplexity);
            }
        }
    }
}