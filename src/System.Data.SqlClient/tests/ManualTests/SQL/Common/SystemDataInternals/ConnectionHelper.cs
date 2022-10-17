// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Diagnostics;

namespace System.Data.SqlClient.ManualTesting.Tests.SystemDataInternals
{
    internal static class ConnectionHelper
    {
        private static Assembly s_systemDotData = Assembly.Load(new AssemblyName(typeof(SqlConnection).GetTypeInfo().Assembly.FullName));
        private static Type s_sqlConnection = s_systemDotData.GetType("System.Data.SqlClient.SqlConnection");
        private static Type s_sqlInternalConnection = s_systemDotData.GetType("System.Data.SqlClient.SqlInternalConnection");
        private static Type s_sqlInternalConnectionTds = s_systemDotData.GetType("System.Data.SqlClient.SqlInternalConnectionTds");
        private static Type s_dbConnectionInternal = s_systemDotData.GetType("System.Data.ProviderBase.DbConnectionInternal");
        private static Type s_tdsParser = s_systemDotData.GetType("System.Data.SqlClient.TdsParser");
        private static Type s_tdsParserStateObject = s_systemDotData.GetType("System.Data.SqlClient.TdsParserStateObject");
        private static PropertyInfo s_sqlConnectionInternalConnection = s_sqlConnection.GetProperty("InnerConnection", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo s_dbConnectionInternalPool = s_dbConnectionInternal.GetProperty("Pool", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo s_dbConnectionInternalIsConnectionAlive = s_dbConnectionInternal.GetMethod("IsConnectionAlive", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo s_sqlInternalConnectionTdsParser = s_sqlInternalConnectionTds.GetField("_parser", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo s_innerConnectionProperty = s_sqlConnection.GetProperty("InnerConnection", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo s_tdsParserProperty = s_sqlInternalConnectionTds.GetProperty("Parser", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo s_tdsParserStateObjectProperty = s_tdsParser.GetField("_physicalStateObj", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo s_enforceTimeoutDelayProperty = s_tdsParserStateObject.GetField("_enforceTimeoutDelay", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo s_enforcedTimeoutDelayInMilliSeconds = s_tdsParserStateObject.GetField("_enforcedTimeoutDelayInMilliSeconds", BindingFlags.Instance | BindingFlags.NonPublic);

        public static object GetConnectionPool(object internalConnection)
        {
            VerifyObjectIsInternalConnection(internalConnection);
            return s_dbConnectionInternalPool.GetValue(internalConnection, null);
        }

        public static object GetInternalConnection(this SqlConnection connection)
        {
            VerifyObjectIsConnection(connection);
            object internalConnection = s_sqlConnectionInternalConnection.GetValue(connection, null);
            Debug.Assert(((internalConnection != null) && (s_dbConnectionInternal.IsInstanceOfType(internalConnection))), "Connection provided has an invalid internal connection");
            return internalConnection;
        }


        public static bool IsConnectionAlive(object internalConnection)
        {
            VerifyObjectIsInternalConnection(internalConnection);
            return (bool)s_dbConnectionInternalIsConnectionAlive.Invoke(internalConnection, new object[] { false });
        }

        private static void VerifyObjectIsInternalConnection(object internalConnection)
        {
            if (internalConnection == null)
                throw new ArgumentNullException(nameof(internalConnection));
            if (!s_dbConnectionInternal.IsInstanceOfType(internalConnection))
                throw new ArgumentException("Object provided was not a DbConnectionInternal", nameof(internalConnection));
        }
        private static void VerifyObjectIsConnection(object connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (!s_sqlConnection.IsInstanceOfType(connection))
                throw new ArgumentException("Object provided was not a SqlConnection", nameof(connection));
        }

        public static object GetParser(object internalConnection)
        {
            VerifyObjectIsInternalConnection(internalConnection);
            return s_sqlInternalConnectionTdsParser.GetValue(internalConnection);
        }
        public static void SetEnforcedTimeout(this SqlConnection connection, bool enforce, int timeout)
        {
            VerifyObjectIsConnection(connection);
            var stateObj = s_tdsParserStateObjectProperty.GetValue(
                            s_tdsParserProperty.GetValue(
                                s_innerConnectionProperty.GetValue(
                                    connection, null), null));
            s_enforceTimeoutDelayProperty.SetValue(stateObj, enforce);
            s_enforcedTimeoutDelayInMilliSeconds.SetValue(stateObj, timeout);
        }
    }
}
