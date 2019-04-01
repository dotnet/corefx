// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Data
{
    public static partial class DataRowComparer
    {
        public static System.Data.DataRowComparer<System.Data.DataRow> Default { get { throw null; } }
    }
    public sealed partial class DataRowComparer<TRow> : System.Collections.Generic.IEqualityComparer<TRow> where TRow : System.Data.DataRow
    {
        internal DataRowComparer() { }
        public static System.Data.DataRowComparer<TRow> Default { get { throw null; } }
        public bool Equals(TRow leftRow, TRow rightRow) { throw null; }
        public int GetHashCode(TRow row) { throw null; }
    }
    public static partial class DataRowExtensions
    {
        public static T Field<T>(this System.Data.DataRow row, System.Data.DataColumn column) { throw null; }
        public static T Field<T>(this System.Data.DataRow row, System.Data.DataColumn column, System.Data.DataRowVersion version) { throw null; }
        public static T Field<T>(this System.Data.DataRow row, int columnIndex) { throw null; }
        public static T Field<T>(this System.Data.DataRow row, int columnIndex, System.Data.DataRowVersion version) { throw null; }
        public static T Field<T>(this System.Data.DataRow row, string columnName) { throw null; }
        public static T Field<T>(this System.Data.DataRow row, string columnName, System.Data.DataRowVersion version) { throw null; }
        public static void SetField<T>(this System.Data.DataRow row, System.Data.DataColumn column, T value) { }
        public static void SetField<T>(this System.Data.DataRow row, int columnIndex, T value) { }
        public static void SetField<T>(this System.Data.DataRow row, string columnName, T value) { }
    }
}
