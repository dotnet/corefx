// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Data
{
    /// <summary>
    /// This class provides a wrapper for DataTables to allow for querying via LINQ.
    /// </summary>
    public partial class EnumerableRowCollection<TRow>
    {
        /// <summary>
        /// Evaluates filter and sort if necessary and returns
        /// a LinqDataView representing the LINQ query this class has collected.
        /// </summary>
        /// <returns>LinqDataView repesenting the LINQ query</returns>
        internal LinqDataView GetLinqDataView() // Called by AsLinqDataView
        {
            if ((null == _table) || !typeof(DataRow).IsAssignableFrom(typeof(TRow)))
            {
                throw DataSetUtil.NotSupported(SR.ToLDVUnsupported);
            }

            LinqDataView view = null;

            #region BuildSinglePredicate

            Func<DataRow, bool> finalPredicate = null; // Conjunction of all .Where(..) predicates
            if ((null != _selector) && (0 < _listOfPredicates.Count))
            {
                // Hook up all individual predicates into one predicate
                // This lambda is a conjunction of multiple predicates set by the user
                // Note: This is a Short-Circuit Conjunction
                finalPredicate =
                    (DataRow row) =>
                    {
                        if (!Object.ReferenceEquals(row, _selector((TRow)(object)row)))
                        {
                            throw DataSetUtil.NotSupported(SR.ToLDVUnsupported);
                        }

                        foreach (Func<TRow, bool> pred in _listOfPredicates)
                        {
                            if (!pred((TRow)(object)row))
                            {
                                return false;
                            }
                        }
                        return true;
                    };
            }
            else if (null != _selector)
            {
                finalPredicate =
                    (DataRow row) =>
                    {
                        if (!Object.ReferenceEquals(row, _selector((TRow)(object)row)))
                        {
                            throw DataSetUtil.NotSupported(SR.ToLDVUnsupported);
                        }
                        return true;
                    };
            }
            else if (0 < _listOfPredicates.Count)
            {
                finalPredicate =
                    (DataRow row) =>
                    {
                        foreach (Func<TRow, bool> pred in _listOfPredicates)
                        {
                            if (!pred((TRow)(object)row))
                            {
                                return false;
                            }
                        }
                        return true;
                    };
            }
            #endregion BuildSinglePredicate

            #region Evaluate Filter/Sort
            //  All of this complexity below is because we want to create index only once.
            // 

            //  If we only have filter, we set _view.Predicate       - 1 index creation
            //  If we only have sort, we set _view.SortExpression()  - 1 index creation
            //  If we have BOTH, we set them through the constructor - 1 index creation
            // 


            // Filter AND Sort
            if ((null != finalPredicate) && (0 < _sortExpression.Count))
            {
                // A lot more work here because constructor does not know type K,
                // so the responsibility to create appropriate delegate comparers
                // is outside of the constructor.

                view = new LinqDataView(
                               _table,
                               row => finalPredicate(row),          // System.Predicate
                               (DataRow a, DataRow b) =>            // Comparison for DV for Index creation
                                   _sortExpression.Compare(
                                       _sortExpression.Select((TRow)(object)a),
                                       _sortExpression.Select((TRow)(object)b)),
                               (object key, DataRow row) =>         // Comparison_K_T for DV's Find()
                                   _sortExpression.Compare(
                                       (List<object>)key,
                                       _sortExpression.Select((TRow)(object)row)),
                                _sortExpression.CloneCast<DataRow>());
            }
            else if (null != finalPredicate)
            {
                // Only Filtering
                view = new LinqDataView(
                                    _table,
                                    row => finalPredicate(row),     // System.Predicate
                                    null,
                                    null,
                                    _sortExpression.CloneCast<DataRow>());
            }
            else if (0 < _sortExpression.Count)
            {
                // Only Sorting
                view = new LinqDataView(
                            _table,
                            null,
                            (DataRow a, DataRow b) => 
                                _sortExpression.Compare(
                                    _sortExpression.Select((TRow)(object)a), 
                                    _sortExpression.Select((TRow)(object)b)),
                            (object key, DataRow row) =>
                                _sortExpression.Compare(
                                    (List<object>)key, 
                                    _sortExpression.Select((TRow)(object)row)),
                            _sortExpression.CloneCast<DataRow>());
            }
            else
            {
                view = new LinqDataView(_table, _sortExpression.CloneCast<DataRow>());
            }
            #endregion Evaluate Filter and Sort

            return view;
        }
    }
}
