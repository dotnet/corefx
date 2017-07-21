// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Data
{
    [Serializable]
    public abstract class TypedTableBase<T> : DataTable, IEnumerable<T> where T : DataRow
    {
        public IEnumerator<T> GetEnumerator() { throw null; }
        IEnumerator IEnumerable.GetEnumerator() { throw null; }
        public EnumerableRowCollection<TResult> Cast<TResult>() { throw null; }
    }

    public static class TypedTableBaseExtensions
    {
        public static EnumerableRowCollection<TRow> Where<TRow>(this TypedTableBase<TRow> source, Func<TRow, bool> predicate) where TRow : DataRow { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector) where TRow : DataRow { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this TypedTableBase<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) where TRow : DataRow { throw null; }
        public static EnumerableRowCollection<S> Select<TRow, S>(this TypedTableBase<TRow> source, Func<TRow, S> selector) where TRow : DataRow { throw null; }
        public static EnumerableRowCollection<TRow> AsEnumerable<TRow>(this TypedTableBase<TRow> source) where TRow : DataRow { throw null; }
        public static TRow ElementAtOrDefault<TRow>(this TypedTableBase<TRow> source, int index) where TRow : DataRow { throw null; }
    }

    public sealed class OrderedEnumerableRowCollection<TRow> : EnumerableRowCollection<TRow>
    {
        internal OrderedEnumerableRowCollection() { }
    }

    public abstract class EnumerableRowCollection : IEnumerable
    {
        internal EnumerableRowCollection() { }
        IEnumerator IEnumerable.GetEnumerator() { throw null; }
    }

    public class EnumerableRowCollection<TRow> : EnumerableRowCollection, IEnumerable<TRow>
    {
        internal EnumerableRowCollection() { }
        IEnumerator IEnumerable.GetEnumerator() { throw null; }
        public IEnumerator<TRow> GetEnumerator() { throw null; }
    }

    public static class EnumerableRowCollectionExtensions
    {
        public static EnumerableRowCollection<TRow> Where<TRow>(this EnumerableRowCollection<TRow> source, Func<TRow, bool> predicate) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderBy<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> OrderByDescending<TRow, TKey>(this EnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> ThenBy<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector) { throw null; }
        public static OrderedEnumerableRowCollection<TRow> ThenByDescending<TRow, TKey>(this OrderedEnumerableRowCollection<TRow> source, Func<TRow, TKey> keySelector, IComparer<TKey> comparer) { throw null; }
        public static EnumerableRowCollection<S> Select<TRow, S>(this EnumerableRowCollection<TRow> source, Func<TRow, S> selector) { throw null; }
        public static EnumerableRowCollection<TResult> Cast<TResult>(this EnumerableRowCollection source) { throw null; }
    }

    public static class DataTableExtensions
    {
        public static EnumerableRowCollection<DataRow> AsEnumerable(this DataTable source) { throw null; }
        public static DataTable CopyToDataTable<T>(this IEnumerable<T> source) where T : DataRow { throw null; }
        public static void CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption options) where T : DataRow { throw null; }
        public static void CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption options, FillErrorEventHandler errorHandler) where T : DataRow { throw null; }
    }

    public static class DataRowExtensions
    {
        public static T Field<T>(this DataRow row, string columnName) { throw null; }
        public static T Field<T>(this DataRow row, DataColumn column) { throw null; }
        public static T Field<T>(this DataRow row, int columnIndex) { throw null; }
        public static T Field<T>(this DataRow row, int columnIndex, DataRowVersion version) { throw null; }
        public static T Field<T>(this DataRow row, string columnName, DataRowVersion version) { throw null; }
        public static T Field<T>(this DataRow row, DataColumn column, DataRowVersion version) { throw null; }
        public static void SetField<T>(this DataRow row, int columnIndex, T value) { throw null; }
        public static void SetField<T>(this DataRow row, string columnName, T value) { throw null; }
        public static void SetField<T>(this DataRow row, DataColumn column, T value) { throw null; }
    }


    public sealed class DataRowComparer<TRow> : IEqualityComparer<TRow> where TRow : DataRow
    {
        private DataRowComparer() { }
        public static DataRowComparer<TRow> Default { get { throw null; } }
        public bool Equals(TRow leftRow, TRow rightRow) { throw null; }
        public int GetHashCode(TRow row) { throw null; }
    }

    public static class DataRowComparer
    {
        public static DataRowComparer<DataRow> Default { get { throw null; } }
    }
}
