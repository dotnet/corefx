// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Data.OleDb.Tests
{
    public static class Helpers
    {
        public const string IsDriverAvailable = nameof(Helpers) + "." + nameof(GetIsDriverAvailable);
        public const string IsAceDriverAvailable = nameof(Helpers) + "." + nameof(GetIsAceDriverAvailable);
        public static bool GetIsDriverAvailable() => Nested.IsAvailable;
        public static bool GetIsAceDriverAvailable() => GetIsDriverAvailable() && !PlatformDetection.Is32BitProcess;
        public static string ProviderName => Nested.ProviderName;
        public static string GetTableName(string memberName) => memberName + ".csv";

        private class Nested
        {
            public static readonly bool IsAvailable;
            public static readonly string ProviderName;
            public static Nested Instance => s_instance;
            private static readonly Nested s_instance = new Nested();
            private static readonly string s_expectedProviderName = @"Microsoft.ACE.OLEDB.12.0";
            private Nested() { }
            static Nested()
            {
                bool shouldSkip = false; 
                // Get the sources rowset for the SQLOLEDB enumerator
                DataTable table = (new OleDbEnumerator()).GetElements();
                DataColumn providersRegistered = table.Columns["SOURCES_NAME"];
                List<object> providerNames = new List<object>();
                foreach (DataRow row in table.Rows)
                {
                    var curProvider = (string)row[providersRegistered];
                    if (curProvider.Contains("JET"))
                        shouldSkip = true; // JET driver installed 
                    providerNames.Add(curProvider);
                }
                // skip if x86 or if both drivers available 
                shouldSkip |= PlatformDetection.Is32BitProcess;
                IsAvailable = !shouldSkip && providerNames.Contains(s_expectedProviderName);
                ProviderName = IsAvailable ? s_expectedProviderName : null;
            }
        }
    }
}
