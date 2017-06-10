// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Data.Common
{
    public static partial class DbProviderFactories
    {
        public static void ConfigureFactory(Type providerFactoryClass) { throw null; }
        public static void ConfigureFactory(Type providerFactoryClass, string providerInvariantName) { throw null; }
        public static void ConfigureFactory(Type providerFactoryClass, string providerInvariantName, string name, string description) { throw null; }
        public static void ConfigureFactory(DbConnection connection) { throw null; }
        public static void ConfigureFactory(DbConnection connection, string providerInvariantName) { throw null; }
        public static void ConfigureFactory(DbConnection connection, string providerInvariantName, string name, string description) { throw null; }
    }
}