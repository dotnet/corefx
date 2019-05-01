// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Data.OleDb.Tests
{
    public static class Helpers
    {
        public const string IsDriverAvailable = nameof(Helpers) + "." + nameof(GetIsDriverAvailable);
        public static bool GetIsDriverAvailable() => Singleton.IsAvailable;
        public static string ProviderName => Singleton.ProviderName;
        public static string GetTableName(string memberName) => memberName + ".csv";

        private class Singleton
        {
            public static Singleton Instance { get { return lazy.Value; } }
            public static bool IsAvailable => Instance._isAvailable;
            public static string ProviderName => Instance._providerName;
            private static readonly Lazy<Singleton> lazy = new Lazy<Singleton>(() => new Singleton());
            private Singleton()
            {
                // Get the sources rowset for the SQLOLEDB enumerator
                DataTable table = (new OleDbEnumerator()).GetElements();
                DataColumn providersRegistered = table.Columns["SOURCES_NAME"];
                List<object> providerNames = new List<object>();
                foreach (DataRow row in table.Rows)
                {
                    providerNames.Add(row[providersRegistered]);
                }
                _isAvailable = providerNames.Contains(_providerName);
            }
            private readonly bool _isAvailable;
            private readonly string _providerName = 
                PlatformDetection.Is32BitProcess ? 
                    @"Microsoft.Jet.OLEDB.4.0" : 
                    @"Microsoft.ACE.OLEDB.12.0";
        }
    }
}
