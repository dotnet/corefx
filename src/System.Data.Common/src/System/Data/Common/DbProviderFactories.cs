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
        private static ReaderWriterLockSlim _providerTableLock = new ReaderWriterLockSlim();

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            ADP.CheckArgumentLength(providerInvariantName, nameof(providerInvariantName));
            try
            {
                _providerTableLock.EnterReadLock();
                if (null != _providerTable)
                {
                    DataRow providerRow = _providerTable.Rows.Find(providerInvariantName);
                    if (null != providerRow)
                    {
                        return DbProviderFactories.GetFactory(providerRow);
                    }
                }
                throw ADP.Argument($"The specified invariant name '{providerInvariantName}' wasn't found in the list of registered .Net Framework Data Providers.",  providerInvariantName);
            }
            finally
            {
                _providerTableLock.ExitReadLock();
            }
        }

        public static DbProviderFactory GetFactory(DataRow providerRow)
        {
            ADP.CheckArgumentNull(providerRow, nameof(providerRow));
            // no need for locking on the table, as the row can only be from a DataTable that's a copy of the one contained in this class.
            DataColumn assemblyQualifiedNameColumn = providerRow.Table.Columns[AssemblyQualifiedName];
            if (null != assemblyQualifiedNameColumn)
            {
                // column value may not be a string
                string assemblyQualifiedName = providerRow[assemblyQualifiedNameColumn] as string;
                if (!string.IsNullOrWhiteSpace(assemblyQualifiedName))
                {
                    Type providerType = Type.GetType(assemblyQualifiedName);
                    if (null != providerType)
                    {
                        System.Reflection.FieldInfo providerInstance = providerType.GetField(Instance, System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (null != providerInstance)
                        {
                            if (providerInstance.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
                            {
                                object factory = providerInstance.GetValue(null);
                                if (null != factory)
                                {
                                    return (DbProviderFactory)factory;
                                }
                                // else throw DataProviderInvalid
                            }
                            // else throw DataProviderInvalid
                        }
                        throw ADP.InvalidOperation("The requested .Net Framework Data Provider's implementation does not have an Instance field of a System.Data.Common.DbProviderFactory derived type.");
                    }
                    throw ADP.Argument($"The registered .Net Framework Data Provider's DbProviderFactory implementation type '{assemblyQualifiedName}' couldn't be loaded.");
                }
            }
            throw ADP.Argument("The missing .Net Framework Data Provider's assembly qualified name is required.");
        }

        public static DbProviderFactory GetFactory(DbConnection connection)
        {
            ADP.CheckArgumentNull(connection, nameof(connection));
            return connection.ProviderFactory;
        }

        public static void SetFactory<TFactory>(string name="", string providerInvariantName="", string description = "")
            where TFactory : DbProviderFactory
        {
            RegisterFactoryInTable(name, providerInvariantName, typeof(TFactory), description);
        }
        
        public static void SetFactory(DbConnection connection, string name="", string providerInvariantName = "", string description = "")
        {
            ADP.CheckArgumentNull(connection, nameof(connection));
            DbProviderFactory factoryInstance = GetFactory(connection);
            if (factoryInstance == null)
            {
                throw ADP.Argument("The .Net Framework Data Provider doesn't supply a DbProviderFactory implementation via DbConnection");
            }
            RegisterFactoryInTable(name, providerInvariantName, factoryInstance.GetType(), description);
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
        
        private static void RegisterFactoryInTable(string name, string providerInvariantName, Type factoryType, string description)
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
                    rowToAlter[Name] = string.IsNullOrWhiteSpace(name) ? invariantNameToUse : name;
                    rowToAlter[InvariantName] = invariantNameToUse;
                    rowToAlter[Description] = description ?? string.Empty;
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
            DataColumn nameColumn = new DataColumn(Name, typeof(string));
            nameColumn.ReadOnly = true;
            DataColumn descriptionColumn = new DataColumn(Description, typeof(string));
            descriptionColumn.ReadOnly = true;
            DataColumn invariantNameColumn = new DataColumn(InvariantName, typeof(string));
            invariantNameColumn.ReadOnly = true;
            // This column isn't readonly in CoreFx, while it is readonly in NetFx, as this class allow overwriting registered providers at runtime.
            DataColumn assemblyQualifiedNameColumn = new DataColumn(AssemblyQualifiedName, typeof(string));

            DataColumn[] primaryKey = new DataColumn[] { invariantNameColumn };
            DataColumn[] columns = new DataColumn[] { nameColumn, descriptionColumn, invariantNameColumn, assemblyQualifiedNameColumn };
            DataTable initialTable = new DataTable(ProviderGroup);
            initialTable.Locale = CultureInfo.InvariantCulture;
            initialTable.Columns.AddRange(columns);
            initialTable.PrimaryKey = primaryKey;
            return initialTable;
        }
    }
}
