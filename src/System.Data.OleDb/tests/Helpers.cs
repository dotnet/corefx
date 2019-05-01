// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Data.OleDb.Tests
{
    public static class Helpers
    {
        public const string IsDriverAvailable = nameof(Helpers) + "." + nameof(GetIsDriverAvailable);

        public static bool GetIsDriverAvailable()
        {
            // Get the sources rowset for the SQLOLEDB enumerator
            DataTable table = (new OleDbEnumerator()).GetElements();
            DataColumn providersRegistered = table.Columns["SOURCES_NAME"];
            List<object> providerNames = new List<object>();
            foreach (DataRow row in table.Rows)
            {
                providerNames.Add(row[providersRegistered]);
            }
            return providerNames.Contains(ProviderName);
        }

        public static string ProviderName => PlatformDetection.Is32BitProcess ? 
            @"Microsoft.Jet.OLEDB.4.0" : @"Microsoft.ACE.OLEDB.12.0";
    }
}
