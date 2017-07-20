// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace System.Data
{
    /// <summary>
    /// Provides an entry point so that Cast operator call can be intercepted within an extension method.
    /// </summary>
    public abstract class EnumerableRowCollection : IEnumerable
    {
        internal abstract Type ElementType { get; }
        internal abstract DataTable Table { get; }

        internal EnumerableRowCollection()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    /// <summary>
    /// This class provides a wrapper for DataTables to allow for querying via LINQ.
    /// </summary>
    public class EnumerableRowCollection<TRow> : EnumerableRowCollection, IEnumerable<TRow>
    {
        private readonly DataTable _table;
        private readonly IEnumerable<TRow> _enumerableRows;
        private readonly List<Func<TRow, bool>> _listOfPredicates;

        // Stores list of sort expression in the order provided by user. E.g. order by, thenby, thenby descending..
        private readonly SortExpressionBuilder<TRow> _sortExpression;

        private readonly Func<TRow, TRow> _selector;

        #region Properties

        internal override Type ElementType
        {
            get
            {
                return typeof(TRow);
            }

        }

        internal IEnumerable<TRow> EnumerableRows
        {
            get
            {
                return _enumerableRows;
            }
        }

        internal override DataTable Table
        {
            get
            {
                return _table;
            }
        }


        #endregion Properties

        #region Constructors

        /// <summary>
        /// This constructor is used when Select operator is called with output Type other than input row Type.
        /// Basically fail on GetLDV(), but other LINQ operators must work.
        /// </summary>
        internal EnumerableRowCollection(IEnumerable<TRow> enumerableRows, bool isDataViewable, DataTable table)
        {
            Debug.Assert(!isDataViewable || table != null, "isDataViewable bug table is null");

            _enumerableRows = enumerableRows;
            if (isDataViewable)
            {
                _table = table;
            }
            _listOfPredicates = new List<Func<TRow, bool>>();
            _sortExpression = new SortExpressionBuilder<TRow>();
        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        internal EnumerableRowCollection(DataTable table)
        {
            _table = table;
            _enumerableRows = table.Rows.Cast<TRow>();
            _listOfPredicates = new List<Func<TRow, bool>>();
            _sortExpression = new SortExpressionBuilder<TRow>();
        }

        /// <summary>
        /// Copy Constructor that sets the input IEnumerable as enumerableRows
        /// Used to maintain IEnumerable that has linq operators executed in the same order as the user
        /// </summary>
        internal EnumerableRowCollection(EnumerableRowCollection<TRow> source, IEnumerable<TRow> enumerableRows, Func<TRow, TRow> selector)
        {
            Debug.Assert(null != enumerableRows, "null enumerableRows");

            _enumerableRows = enumerableRows;
            _selector = selector;
            if (null != source)
            {
                if (null == source._selector)
                {
                    _table = source._table;
                }
                _listOfPredicates = new List<Func<TRow, bool>>(source._listOfPredicates);
                _sortExpression = source._sortExpression.Clone(); //deep copy the List
            }
            else
            {
                _listOfPredicates = new List<Func<TRow, bool>>();
                _sortExpression = new SortExpressionBuilder<TRow>();
            }
        }

        #endregion Constructors

        #region PublicInterface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///  This method returns an strongly typed iterator
        ///  for the underlying DataRow collection.
        /// </summary>
        /// <returns>
        ///   A strongly typed iterator.
        /// </returns>
        public IEnumerator<TRow> GetEnumerator()
        {
            return _enumerableRows.GetEnumerator();
        }
        #endregion PublicInterface

        /// <summary>
        /// Evaluates filter and sort if necessary and returns
        /// a LinqDataView representing the LINQ query this class has collected.
        /// </summary>
        /// <returns>LinqDataView repesenting the LINQ query</returns>
        internal LinqDataView GetLinqDataView() //Called by AsLinqDataView
        {
            if ((null == _table) || !typeof(DataRow).IsAssignableFrom(typeof(TRow)))
            {
                throw DataSetUtil.NotSupported(Strings.ToLDVUnsupported);
            }

            LinqDataView view = null;

            #region BuildSinglePredicate

            Func<DataRow, bool> finalPredicate = null; //Conjunction of all .Where(..) predicates
            if ((null != _selector) && (0 < _listOfPredicates.Count))
            {
                // Hook up all individual predicates into one predicate
                // This delegate is a conjunction of multiple predicates set by the user
                // Note: This is a Short-Circuit Conjunction
                finalPredicate =
                    delegate (DataRow row)
                    {
                        if (!object.ReferenceEquals(row, _selector((TRow)(object)row)))
                        {
                            throw DataSetUtil.NotSupported(Strings.ToLDVUnsupported);
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
                    delegate (DataRow row)
                    {
                        if (!object.ReferenceEquals(row, _selector((TRow)(object)row)))
                        {
                            throw DataSetUtil.NotSupported(Strings.ToLDVUnsupported);
                        }
                        return true;
                    };
            }
            else if (0 < _listOfPredicates.Count)
            {
                finalPredicate =
                    delegate (DataRow row)
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
            //  All of this mess below is because we want to create index only once.
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
                               finalPredicate,                      //Func() Predicate
                               delegate (DataRow row)                //System.Predicate
                               {
                                   return finalPredicate(row);
                               },
                               delegate (DataRow a, DataRow b)       //Comparison for DV for Index creation
                               {
                                   return _sortExpression.Compare(
                                            _sortExpression.Select((TRow)(object)a),
                                            _sortExpression.Select((TRow)(object)b)
                                       );
                               },
                               delegate (object key, DataRow row)    //Comparison_K_T for DV's Find()
                               {
                                   return _sortExpression.Compare(
                                        (List<object>)key,
                                        _sortExpression.Select((TRow)(object)row)
                                      );
                               },
                                _sortExpression.CloneCast<DataRow>());
            }
            else if (null != finalPredicate)
            {
                //Only Filtering
                view = new LinqDataView(
                                    _table,
                                    finalPredicate,
                                    delegate (DataRow row)                //System.Predicate
                                    {
                                        return finalPredicate(row);
                                    },
                                    null,
                                    null,
                                    _sortExpression.CloneCast<DataRow>());
            }
            else if (0 < _sortExpression.Count)
            {
                //Only Sorting
                view = new LinqDataView(
                            _table,
                            null,
                            null,
                            delegate (DataRow a, DataRow b)
                            {
                                return _sortExpression.Compare(_sortExpression.Select((TRow)(object)a), _sortExpression.Select((TRow)(object)b));
                            },
                            delegate (object key, DataRow row)
                            {
                                return _sortExpression.Compare((List<object>)key, _sortExpression.Select((TRow)(object)row));
                            },
                            _sortExpression.CloneCast<DataRow>());
            }
            else
            {
                view = new LinqDataView(_table, _sortExpression.CloneCast<DataRow>());
            }
            #endregion Evaluate Filter and Sort

            return view;
        }

        #region Add Single Filter/Sort Expression

        /// <summary>
        /// Used to add a filter predicate.
        /// A conjunction of all predicates are evaluated in LinqDataView
        /// </summary>
        internal void AddPredicate(Func<TRow, bool> pred)
        {
            Debug.Assert(pred != null);
            _listOfPredicates.Add(pred);
        }

        /// <summary>
        /// Adds a sort expression when Keyselector is provided but not Comparer
        /// </summary>
        internal void AddSortExpression<TKey>(Func<TRow, TKey> keySelector, bool isDescending, bool isOrderBy)
        {
            AddSortExpression<TKey>(keySelector, Comparer<TKey>.Default, isDescending, isOrderBy);
        }

        /// <summary>
        /// Adds a sort expression when Keyselector and Comparer are provided.
        /// </summary>
        internal void AddSortExpression<TKey>(
                            Func<TRow, TKey> keySelector,
                            IComparer<TKey> comparer,
                            bool isDescending,
                            bool isOrderBy)
        {
            DataSetUtil.CheckArgumentNull(keySelector, "keySelector");
            DataSetUtil.CheckArgumentNull(comparer, "comparer");

            _sortExpression.Add(
                    delegate (TRow input)
                    {
                        return (object)keySelector(input);
                    },
                    delegate (object val1, object val2)
                    {
                        return (isDescending ? -1 : 1) * comparer.Compare((TKey)val1, (TKey)val2);
                    },
                      isOrderBy);
        }

        #endregion Add Single Filter/Sort Expression
    }
}