// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.Data.SqlClient.ManualTesting.Tests.SystemDataInternals
{
    internal static class TdsParserStateObjectHelper
    {
        private static Assembly s_systemDotData = typeof(System.Data.SqlClient.SqlConnection).GetTypeInfo().Assembly;
        private static Type s_tdsParserStateObject = s_systemDotData.GetType("System.Data.SqlClient.TdsParserStateObject");
        private static FieldInfo s_forceAllPends = s_tdsParserStateObject.GetField("_forceAllPends", BindingFlags.Static | BindingFlags.NonPublic);
        private static FieldInfo s_skipSendAttention = s_tdsParserStateObject.GetField("_skipSendAttention", BindingFlags.Static | BindingFlags.NonPublic);
        private static FieldInfo s_forceSyncOverAsyncAfterFirstPend = s_tdsParserStateObject.GetField("_forceSyncOverAsyncAfterFirstPend", BindingFlags.Static | BindingFlags.NonPublic);
        private static FieldInfo s_failAsyncPends = s_tdsParserStateObject.GetField("_failAsyncPends", BindingFlags.Static | BindingFlags.NonPublic);
        private static FieldInfo s_forcePendingReadsToWaitForUser = s_tdsParserStateObject.GetField("_forcePendingReadsToWaitForUser", BindingFlags.Static | BindingFlags.NonPublic);
        private static FieldInfo s_tdsParserStateObjectSessionHandle = s_tdsParserStateObject.GetField("_sessionHandle", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static bool ForceAllPends
        {
            get { return (bool)s_forceAllPends.GetValue(null); }
            set { s_forceAllPends.SetValue(null, value); }
        }

        internal static bool SkipSendAttention
        {
            get { return (bool)s_skipSendAttention.GetValue(null); }
            set { s_skipSendAttention.SetValue(null, value); }
        }

        internal static bool ForceSyncOverAsyncAfterFirstPend
        {
            get { return (bool)s_forceSyncOverAsyncAfterFirstPend.GetValue(null); }
            set { s_forceSyncOverAsyncAfterFirstPend.SetValue(null, value); }
        }

        internal static bool ForcePendingReadsToWaitForUser
        {
            get { return (bool)s_forcePendingReadsToWaitForUser.GetValue(null); }
            set { s_forcePendingReadsToWaitForUser.SetValue(null, value); }
        }

        internal static bool FailAsyncPends
        {
            get { return (bool)s_failAsyncPends.GetValue(null); }
            set { s_failAsyncPends.SetValue(null, value); }
        }

        private static void VerifyObjectIsTdsParserStateObject(object stateObject)
        {
            if (stateObject == null)
                throw new ArgumentNullException("stateObject");
            if (!s_tdsParserStateObject.IsInstanceOfType(stateObject))
                throw new ArgumentException("Object provided was not a DbConnectionInternal", "internalConnection");
        }

        internal static object GetSessionHandle(object stateObject)
        {
            VerifyObjectIsTdsParserStateObject(stateObject);
            return s_tdsParserStateObjectSessionHandle.GetValue(stateObject);
        }
    }
}
