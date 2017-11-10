// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
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

        private static DataTable _providerTable = GetInitialProviderTable();
        private static readonly ReaderWriterLockSlim _providerTableLock = new ReaderWriterLockSlim();

        
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
            // no need for locking on the table, as the row can only be from a DataTable that's a copy of the one contained in this class.
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
            System.Reflection.FieldInfo providerInstance = providerType.GetField(Instance, System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | 
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

        public static DbProviderFactory GetFactory(DbConnection connection)
        {
            ADP.CheckArgumentNull(connection, nameof(connection));
            return connection.ProviderFactory;
        }

        public static DataTable GetFactoryClasses()
        {
            try
            {
                _providerTableLock.EnterReadLock();
                return _providerTable == null ? GetInitialProviderTable() : _providerTable.Copy();
            }
            finally
            {
                _providerTableLock.ExitReadLock();
            }
        }

        public static void RegisterFactory(string providerInvariantName, string factoryTypeAssemblyQualifiedName)
        {
#warning TODO            
        }

        
        public static void RegisterFactory(string providerInvariantName, Type providerFactoryClass)
        {
            ADP.CheckArgumentNull(providerFactoryClass, nameof(providerFactoryClass));
            if (!providerFactoryClass.IsSubclassOf(typeof(DbProviderFactory)))
            {
                throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_NotAFactoryType, providerFactoryClass.FullName));
            }
            DbProviderFactories.RegisterFactoryInTable(providerInvariantName, providerFactoryClass);
        }

        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            ADP.CheckArgumentNull(factory, nameof(factory));
            DbProviderFactories.RegisterFactoryInTable(providerInvariantName, factory.GetType());
        }
        
        public static bool UnregisterFactory(string providerInvariantName)
        {
            if (string.IsNullOrWhiteSpace(providerInvariantName))
            {
                return false;
            }
            try
            {
                _providerTableLock.EnterReadLock();
                DataRow providerRow = GetProviderRowFromTable(providerInvariantName, throwOnError: false);
                if (null == providerRow)
                {
                    return false;
                }
                _providerTable.Rows.Remove(providerRow);
                return true;
            }
            finally
            {
                _providerTableLock.ExitReadLock();
            }
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
            try
            {
                _providerTableLock.EnterReadLock();
                DataRow providerRow = GetProviderRowFromTable(providerInvariantName, throwOnError);
                if (null == providerRow)
                {
                    return throwOnError ? throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_InvariantNameNotFound, providerInvariantName)) : (DbProviderFactory)null;
                }
                return DbProviderFactories.GetFactory(providerRow);
            }
            finally
            {
                _providerTableLock.ExitReadLock();
            }
        }

        private static DataRow GetProviderRowFromTable(string providerInvariantName, bool throwOnError)
        {
            if (null == _providerTable)
            {
                return throwOnError ? throw ADP.Argument(SR.Format(SR.ADP_DbProviderFactories_InvariantNameNotFound, providerInvariantName)) : (DataRow)null;
            }
            return _providerTable.Rows.Find(providerInvariantName);
        }
        
        private static void RegisterFactoryInTable(string providerInvariantName, Type factoryType)
        {
            ADP.CheckArgumentNull(factoryType, nameof(factoryType));

            string invariantNameToUse = string.IsNullOrWhiteSpace(providerInvariantName) ? factoryType.Namespace : providerInvariantName;
            try
            {
                _providerTableLock.EnterWriteLock();

                bool newRow = false;
                DataRow rowToAlter = _providerTable.Rows.Find(invariantNameToUse);
                if (rowToAlter == null)
                {
                    newRow = true;
                    rowToAlter = _providerTable.NewRow();
                    rowToAlter[Name] = invariantNameToUse;
                    rowToAlter[InvariantName] = invariantNameToUse;
                    rowToAlter[Description] = string.Empty;
                }
                rowToAlter[AssemblyQualifiedName] = factoryType.AssemblyQualifiedName;
                if (newRow)
                {
                    _providerTable.AddRow(rowToAlter);
                }
            }
            finally
            {
                _providerTableLock.ExitWriteLock();
            }
        }

        
        private static DataTable GetInitialProviderTable()
        {
            DataColumn nameColumn = new DataColumn(Name, typeof(string)) { ReadOnly = true };
            DataColumn descriptionColumn = new DataColumn(Description, typeof(string)) { ReadOnly = true };
            DataColumn invariantNameColumn = new DataColumn(InvariantName, typeof(string)) { ReadOnly = true };

            // This column is writable in CoreFx, while it is readonly in NetFx, as this class allows overwriting registered providers at runtime.
            DataColumn assemblyQualifiedNameColumn = new DataColumn(AssemblyQualifiedName, typeof(string));

            DataColumn[] primaryKey = new DataColumn[] { invariantNameColumn };
            DataColumn[] columns = new DataColumn[] { nameColumn, descriptionColumn, invariantNameColumn, assemblyQualifiedNameColumn };
            DataTable initialTable = new DataTable(ProviderGroup) { Locale = CultureInfo.InvariantCulture };
            initialTable.Columns.AddRange(columns);
            initialTable.PrimaryKey = primaryKey;
            return initialTable;
        }
    }
}
