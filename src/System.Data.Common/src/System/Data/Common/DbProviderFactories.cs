// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace System.Data.Common
{
    public static partial class DbProviderFactories
    {
        private const string AssemblyQualifiedName = "AssemblyQualifiedName";
        private const string InvariantName = "InvariantName";
        private const string Name = "Name";
        private const string Description = "Description";
        private const string ProviderGroup = "DbProviderFactories";
        private const string Instance = "Instance";

        private static ConcurrentDictionary<string, DbProviderFactory> _registeredFactories = new ConcurrentDictionary<string, DbProviderFactory>();

        
        public static bool TryGetFactory(string providerInvariantName, out DbProviderFactory factory)
        {
            factory = DbProviderFactories.GetFactory(providerInvariantName, throwOnError: false);
            return factory != null;
        }

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            return DbProviderFactories.GetFactory(providerInvariantName, throwOnError:true);
        }

        public static DbProviderFactory GetFactory(DataRow providerRow)
        {
            ADP.CheckArgumentNull(providerRow, nameof(providerRow));
            DataColumn assemblyQualifiedNameColumn = providerRow.Table.Columns[AssemblyQualifiedName];
            if (null == assemblyQualifiedNameColumn)
            {
                throw ADP.Argument(SR.ADP_DbProviderFactories_NoAssemblyQualifiedName);
            }
            string assemblyQualifiedName = providerRow[assemblyQualifiedNameColumn] as string;
            if (string.IsNullOrWhiteSpace(assemblyQualifiedName))
            {
                throw ADP.Argument(SR.ADP_DbProviderFactories_NoAssemblyQualifiedName);
            }
            Type providerType = Type.GetType(assemblyQualifiedName);
            if (null == providerType)
            {
                throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_FactoryNotLoadable, assemblyQualifiedName));
            }
            return GetFactoryInstance(providerType);
        }

        public static DbProviderFactory GetFactory(DbConnection connection)
        {
            ADP.CheckArgumentNull(connection, nameof(connection));
            return connection.ProviderFactory;
        }

        public static DataTable GetFactoryClasses()
        {
            DataColumn nameColumn = new DataColumn(Name, typeof(string)) { ReadOnly = true };
            DataColumn descriptionColumn = new DataColumn(Description, typeof(string)) { ReadOnly = true };
            DataColumn invariantNameColumn = new DataColumn(InvariantName, typeof(string)) { ReadOnly = true };
            DataColumn assemblyQualifiedNameColumn = new DataColumn(AssemblyQualifiedName, typeof(string)) { ReadOnly = true};

            DataTable toReturn = new DataTable(ProviderGroup) { Locale = CultureInfo.InvariantCulture };
            toReturn.Columns.AddRange(new[] { nameColumn, descriptionColumn, invariantNameColumn, assemblyQualifiedNameColumn });
            toReturn.PrimaryKey = new[] { invariantNameColumn };
            foreach(var kvp in _registeredFactories)
            {
                DataRow newRow = toReturn.NewRow();
                newRow[InvariantName] = kvp.Key;
                newRow[AssemblyQualifiedName] = kvp.Value.GetType().AssemblyQualifiedName;
                newRow[Name] = string.Empty;
                newRow[Description] = string.Empty;
                toReturn.AddRow(newRow);
            }
            return toReturn;
        }

        public static IEnumerable<string> GetProviderInvariantNames()
        {
            return _registeredFactories.Keys.ToList();
        }

        public static void RegisterFactory(string providerInvariantName, string factoryTypeAssemblyQualifiedName)
        {
            ADP.CheckArgumentLength(providerInvariantName, nameof(providerInvariantName));
            ADP.CheckArgumentLength(factoryTypeAssemblyQualifiedName, nameof(providerInvariantName));
            Type providerType = Type.GetType(factoryTypeAssemblyQualifiedName);
            if (null == providerType)
            {
                throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_FactoryNotLoadable, factoryTypeAssemblyQualifiedName));
            }
            RegisterFactory(providerInvariantName, providerType);
        }
        
        public static void RegisterFactory(string providerInvariantName, Type providerFactoryClass)
        {
            RegisterFactory(providerInvariantName, GetFactoryInstance(providerFactoryClass));
        }

        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            ADP.CheckArgumentLength(providerInvariantName, nameof(providerInvariantName));
            ADP.CheckArgumentNull(factory, nameof(factory));
            _registeredFactories[providerInvariantName] = factory;
        }
        
        public static bool UnregisterFactory(string providerInvariantName)
        {
            return !string.IsNullOrWhiteSpace(providerInvariantName) && _registeredFactories.TryRemove(providerInvariantName, out DbProviderFactory registeredFactory);
        }
        
        private static DbProviderFactory GetFactory(string providerInvariantName, bool throwOnError)
        {
            if (throwOnError)
            {
                ADP.CheckArgumentLength(providerInvariantName, nameof(providerInvariantName));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(providerInvariantName))
                {
                    return null;
                }
            }
            bool wasRegistered = _registeredFactories.TryGetValue(providerInvariantName, out DbProviderFactory registeredFactory);
            if (!wasRegistered)
            {
                return throwOnError ? throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_InvariantNameNotFound, providerInvariantName)) : (DbProviderFactory)null;
            }
            return registeredFactory;
        }
       
        private static DbProviderFactory GetFactoryInstance(Type providerFactoryClass)
        {
            ADP.CheckArgumentNull(providerFactoryClass, nameof(providerFactoryClass));
            if (!providerFactoryClass.IsSubclassOf(typeof(DbProviderFactory)))
            {
                throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_NotAFactoryType, providerFactoryClass.FullName));
            }

            System.Reflection.FieldInfo providerInstance = providerFactoryClass.GetField(Instance, System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | 
                                                                                                   System.Reflection.BindingFlags.Static);
            if (null == providerInstance)
            {
                throw ADP.InvalidOperation(SR.ADP_DbProviderFactories_NoInstance);
            }
            if (!providerInstance.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
            {
                throw ADP.InvalidOperation(SR.ADP_DbProviderFactories_NoInstance);
            }
            object factory = providerInstance.GetValue(null);
            if (null == factory)
            {
                throw ADP.InvalidOperation(SR.ADP_DbProviderFactories_NoInstance);
            }
            return (DbProviderFactory)factory;
        }
    }
}
