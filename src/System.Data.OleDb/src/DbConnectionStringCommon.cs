// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlClient;

namespace System.Data.Common {

/*
    internal sealed class NamedConnectionStringConverter : StringConverter {

        public NamedConnectionStringConverter() {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            // Although theoretically this could be true, some people may want to just type in a name
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            StandardValuesCollection standardValues = null;
            if (null != context) {
                DbConnectionStringBuilder instance = (context.Instance as DbConnectionStringBuilder);
                if (null != instance) {
                    string myProviderName = instance.GetType().Namespace;

                    List<string> myConnectionNames = new List<string>();
                    foreach(System.Configuration.ConnectionStringSetting setting in System.Configuration.ConfigurationManager.ConnectionStrings) {
                        if (myProviderName.EndsWith(setting.ProviderName)) {
                            myConnectionNames.Add(setting.ConnectionName);
                        }
                    }
                    standardValues = new StandardValuesCollection(myConnectionNames);
                }
            }
            return standardValues;
        }
    }
*/

    [Serializable()]
    internal sealed class ReadOnlyCollection<T> : System.Collections.ICollection, ICollection<T> {
        private T[] _items;

        internal ReadOnlyCollection(T[] items) {
            _items = items;
#if DEBUG
            for(int i = 0; i < items.Length; ++i) {
                Debug.Assert(null != items[i], "null item");
            }
#endif
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Array.Copy(_items, 0, array, arrayIndex, _items.Length);
        }

        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex) {
            Array.Copy(_items, 0, array, arrayIndex, _items.Length);
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator<T>(_items);
        }

        public System.Collections.IEnumerator GetEnumerator() {
            return new Enumerator<T>(_items);
        }

        bool System.Collections.ICollection.IsSynchronized {
            get { return false; }
        }

        Object System.Collections.ICollection.SyncRoot {
            get { return _items; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return true;}
        }

        void ICollection<T>.Add(T value) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T value) {
            return Array.IndexOf(_items, value) >= 0;
        }

        bool ICollection<T>.Remove(T value) {
            throw new NotSupportedException();
        }

        public int Count {
            get { return _items.Length; }
        }

        [Serializable()]
        internal struct Enumerator<K> : IEnumerator<K>, System.Collections.IEnumerator { // based on List<T>.Enumerator
            private K[] _items;
            private int _index;

            internal Enumerator(K[] items) {
                _items = items;
                _index = -1;
            }

            public void Dispose() {
            }

            public bool MoveNext() {
                return (++_index < _items.Length);
            }

            public K Current {
                get {
                    return _items[_index];
                }
            }

            Object System.Collections.IEnumerator.Current {
                get {
                    return _items[_index];
                }
            }

            void System.Collections.IEnumerator.Reset() {
                _index = -1;
            }
        }
    }

    internal static class DbConnectionStringBuilderUtil
    {

        internal static bool ConvertToBoolean(object value)
        {
            Debug.Assert(null != value, "ConvertToBoolean(null)");
            string svalue = (value as string);
            if (null != svalue)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "true") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "yes"))
                    return true;
                else if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "false") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "no"))
                    return false;
                else
                {
                    string tmp = svalue.Trim();  // Remove leading & trailing white space.
                    if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                        return true;
                    else if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                        return false;
                }
                return Boolean.Parse(svalue);
            }
            try
            {
                return ((IConvertible)value).ToBoolean(CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(Boolean), e);
            }
        }

        internal static bool ConvertToIntegratedSecurity(object value)
        {
            Debug.Assert(null != value, "ConvertToIntegratedSecurity(null)");
            string svalue = (value as string);
            if (null != svalue)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "true") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "yes"))
                    return true;
                else if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "false") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "no"))
                    return false;
                else
                {
                    string tmp = svalue.Trim();  // Remove leading & trailing white space.
                    if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                        return true;
                    else if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                        return false;
                }
                return Boolean.Parse(svalue);
            }
            try
            {
                return ((IConvertible)value).ToBoolean(CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(Boolean), e);
            }
        }

        internal static int ConvertToInt32(object value)
        {
            try
            {
                return ((IConvertible)value).ToInt32(CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(Int32), e);
            }
        }

        internal static string ConvertToString(object value)
        {
            try
            {
                return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException e)
            {
                throw ADP.ConvertFailed(value.GetType(), typeof(String), e);
            }
        }
    }

         internal static class DbConnectionStringDefaults {
        // all
//        internal const string NamedConnection           = "";

        // Odbc
        internal const string Driver                    = "";
        internal const string Dsn                       = "";

        // OleDb
        internal const bool   AdoNetPooler              = false;
        internal const string FileName                  = "";
        internal const int    OleDbServices             = ~(/*DBPROPVAL_OS_AGR_AFTERSESSION*/0x00000008 | /*DBPROPVAL_OS_CLIENTCURSOR*/0x00000004); // -13
        internal const string Provider                  = "";

        internal const int ConnectTimeout = 15;
        internal const bool PersistSecurityInfo = false;
        internal const string DataSource = "";
        internal const string ApplicationName = "Core .Net SqlClient Data Provider";
        internal const ApplicationIntent ApplicationIntent = System.Data.SqlClient.ApplicationIntent.ReadWrite;
        internal const bool MultiSubnetFailover = false;
        // internal const PoolBlockingPeriod PoolBlockingPeriod = PoolBlockingPeriod.Auto;
    }

    internal static class DbConnectionOptionKeywords {
        // Odbc
        internal const string Driver                    = "driver";
        internal const string Pwd                       = "pwd";
        internal const string UID                       = "uid";

        // OleDb
        internal const string DataProvider              = "data provider";
        internal const string ExtendedProperties        = "extended properties";
        internal const string FileName                  = "file name";
        internal const string Provider                  = "provider";
        internal const string RemoteProvider            = "remote provider";

        // common keywords (OleDb, OracleClient, SqlClient)
        internal const string Password                  = "password";
        internal const string UserID                    = "user id";
    }

    internal static class DbConnectionStringKeywords {
        // all
//        internal const string NamedConnection           = "Named Connection";

        // Odbc
        internal const string Driver                    = "Driver";
        internal const string Dsn                       = "Dsn";
        internal const string FileDsn                   = "FileDsn";
        internal const string SaveFile                  = "SaveFile";

        // OleDb
        internal const string FileName                  = "File Name";
        internal const string OleDbServices             = "OLE DB Services";
        internal const string Provider                  = "Provider";

        internal const string DataSource = "Data Source";
        internal const string PersistSecurityInfo = "Persist Security Info";
        internal const string IntegratedSecurity = "Integrated Security";
    }

    internal static class DbConnectionStringSynonyms {
        //internal const string AsynchronousProcessing = Async;
        internal const string Async                  = "async";

        //internal const string ApplicationName        = APP;
        internal const string APP                    = "app";

        //internal const string AttachDBFilename       = EXTENDEDPROPERTIES+","+INITIALFILENAME;
        internal const string EXTENDEDPROPERTIES     = "extended properties";
        internal const string INITIALFILENAME        = "initial file name";

        //internal const string ConnectTimeout         = CONNECTIONTIMEOUT+","+TIMEOUT;
        internal const string CONNECTIONTIMEOUT      = "connection timeout";
        internal const string TIMEOUT                = "timeout";

        //internal const string CurrentLanguage        = LANGUAGE;
        internal const string LANGUAGE               = "language";

        //internal const string OraDataSource          = SERVER;
        //internal const string SqlDataSource          = ADDR+","+ADDRESS+","+SERVER+","+NETWORKADDRESS;
        internal const string ADDR                   = "addr";
        internal const string ADDRESS                = "address";
        internal const string SERVER                 = "server";
        internal const string NETWORKADDRESS         = "network address";

        //internal const string InitialCatalog         = DATABASE;
        internal const string DATABASE               = "database";

        //internal const string IntegratedSecurity     = TRUSTEDCONNECTION;
        internal const string TRUSTEDCONNECTION      = "trusted_connection"; // underscore introduced in everett

        //internal const string LoadBalanceTimeout     = ConnectionLifetime;
        internal const string ConnectionLifetime     = "connection lifetime";

        //internal const string NetworkLibrary         = NET+","+NETWORK;
        internal const string NET                    = "net";
        internal const string NETWORK                = "network";

        internal const string WorkaroundOracleBug914652 = "Workaround Oracle Bug 914652";

        //internal const string Password               = Pwd;
        internal const string Pwd                    = "pwd";

        //internal const string PersistSecurityInfo    = PERSISTSECURITYINFO;
        internal const string PERSISTSECURITYINFO    = "persistsecurityinfo";

        //internal const string UserID                 = UID+","+User;
        internal const string UID                    = "uid";
        internal const string User                   = "user";

        //internal const string WorkstationID          = WSID;
        internal const string WSID                   = "wsid";
    }
}
