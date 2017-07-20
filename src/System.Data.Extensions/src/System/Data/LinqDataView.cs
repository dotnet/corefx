// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace System.Data
{
    /// <summary>
    /// Represents a bindable, queryable DataView of DataRow, that can be created from from LINQ queries over DataTable
    /// and from DataTable. 
    /// </summary>
    internal class LinqDataView : DataView, IBindingList, IBindingListView
    {
        /// <summary>
        /// A Comparer that compares a Key and a Row.
        /// </summary>
        internal Func<object, DataRow, int> comparerKeyRow;  // comparer for DataView.Find(..

        /// <summary>
        /// Builds the sort expression in case multiple selector/comparers are added
        /// </summary>
        internal readonly SortExpressionBuilder<DataRow> sortExpressionBuilder;

        /// <summary>
        /// Constructs a LinkDataView and its parent DataView.
        /// Does not create index on the DataView since filter and sort expressions are not yet provided.
        /// </summary>
        /// <param name="table">The input table from which LinkDataView is to be created.</param>
        internal LinqDataView(DataTable table, SortExpressionBuilder<DataRow> sortExpressionBuilder)
            : base(table)
        {
            Debug.Assert(table != null, "null DataTable");
            this.sortExpressionBuilder = sortExpressionBuilder ?? new SortExpressionBuilder<DataRow>();
        }


        //I have two forms of predicate because I need to pass in null if predicate is null. Otherwise I need to convert it into delegate and pass it into
        // data view's constructor. That logic for checking null can't be embedded in the base constructor call.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table">Table from which to create the view</param>
        /// <param name="predicate_func">User-provided filter-predicate as a Func<DataRow>, bool>"/></param>
        /// <param name="predicate_system">User-provided predicate but in the form of System.Predicate<DataRow>
        /// Predicates are being replicated in different forms so that nulls can be passed in.
        /// For e.g. when user provides null predicate, base.Predicate should be set to null. I cant do that in the constructor initialization
        /// if I will have to create System.Predicate delegate from Func.
        /// </param>
        /// <param name="comparison">The comparer function of DataRow to be used for sorting. </param>
        /// <param name="comparerKeyRow">A comparer function that compares a Key value to DataRow.</param>
        /// <param name="isDescending">Whether sorting is ascending or descending.</param>
        /// <param name="rowState">Row state filter. For the purpose of LinkDataView it should always be CurrentRows.</param>
        internal LinqDataView(
                    DataTable table,
                    Func<DataRow, bool> predicate_func,
                    Predicate<DataRow> predicate_system,
                    Comparison<DataRow> comparison,
                    Func<object, DataRow, int> comparerKeyRow,
                    SortExpressionBuilder<DataRow> sortExpressionBuilder)

            //Parent constructor
            : base(table,
                predicate_system,
                comparison,
                DataViewRowState.CurrentRows)
        {
            this.sortExpressionBuilder = (sortExpressionBuilder == null) ? this.sortExpressionBuilder : sortExpressionBuilder;
            this.comparerKeyRow = comparerKeyRow;
        }

        /// <summary>
        /// Gets or sets the expression used to filter which rows are viewed in the LinqDataView
        /// </summary>
        public override string RowFilter
        {
            get
            {
                if (base.RowPredicate == null)
                {
                    // using string based filter or no filter
                    return base.RowFilter;
                }
                else
                {
                    // using expression based filter
                    return null;
                }
            }

            set
            {
                if (value == null)
                {
                    base.RowPredicate = null;
                    base.RowFilter = String.Empty; // INDEX rebuild twice
                }
                else
                {
                    base.RowFilter = value;
                    base.RowPredicate = null;
                }
            }
        }

        #region Find

        /// <summary>
        /// Searches the index and finds a single row where the sort-key matches the input key
        /// </summary>
        /// <param name="key">Value of the key to find</param>
        /// <returns>Index of the first match of input key</returns>
        internal override int FindByKey(object key)
        {
            // Check that both string and expression based sort are never simultaneously set
            Debug.Assert(base.Sort != null);
            Debug.Assert(!(!String.IsNullOrEmpty(base.Sort) && base.SortComparison != null));

            if (!String.IsNullOrEmpty(base.Sort))  //use find for DV's sort string
            {
                return base.FindByKey(key);
            }
            else if (base.SortComparison == null) //neither string or expr set
            {
                //This is the exception message from DataView that we want to use
                throw ExceptionBuilder.IndexKeyLength(0, 0);
            }
            else  //find for expression based sort
            {
                if (sortExpressionBuilder.Count != 1)
                    throw DataSetUtil.InvalidOperation(Strings.LDV_InvalidNumOfKeys(sortExpressionBuilder.Count));

                Index.ComparisonBySelector<object, DataRow> compareDelg =
                    new Index.ComparisonBySelector<object, DataRow>(comparerKeyRow);

                List<object> keyList = new List<object>();
                keyList.Add(key);
                Range range = FindRecords<object, DataRow>(compareDelg, keyList);

                return (range.Count == 0) ? -1 : range.Min;
            }
        }

        /// <summary>
        /// Since LinkDataView does not support multiple selectors/comparers, it does not make sense for
        /// them to Find using multiple keys.
        /// This overriden method prevents users calling multi-key find on dataview.
        /// </summary>
        internal override int FindByKey(object[] key)
        {
            //---------Checks ----------------
            //must have string or expression based sort specified
            if (base.SortComparison == null && String.IsNullOrEmpty(base.Sort))
            {
                //This is the exception message from DataView that we want to use
                throw ExceptionBuilder.IndexKeyLength(0, 0);
            }
            else if (base.SortComparison != null && key.Length != sortExpressionBuilder.Count)
            {
                throw DataSetUtil.InvalidOperation(Strings.LDV_InvalidNumOfKeys(sortExpressionBuilder.Count));
            }
            //--------------------------------

            if (base.SortComparison == null)
            {
                // using string to sort
                return base.FindByKey(key);
            }
            else
            {
                Index.ComparisonBySelector<object, DataRow> compareDelg =
                    new Index.ComparisonBySelector<object, DataRow>(comparerKeyRow);

                List<object> keyList = new List<object>();
                foreach (object singleKey in key)
                {
                    keyList.Add(singleKey);
                }
                Range range = FindRecords<object, DataRow>(compareDelg, keyList);
                return (range.Count == 0) ? -1 : range.Min;
            }

        }

        /// <summary>
        /// Searches the index and finds rows where the sort-key matches the input key.
        /// Since LinkDataView does not support multiple selectors/comparers, it does not make sense for
        /// them to Find using multiple keys. This overriden method prevents users calling multi-key find on dataview.
        /// </summary>
        internal override DataRowView[] FindRowsByKey(object[] key)
        {
            //---------Checks ----------------
            //must have string or expression based sort specified
            if (base.SortComparison == null && String.IsNullOrEmpty(base.Sort))
            {
                //This is the exception message from DataView that we want to use
                throw ExceptionBuilder.IndexKeyLength(0, 0);
            }
            else if (base.SortComparison != null && key.Length != sortExpressionBuilder.Count)
            {
                throw DataSetUtil.InvalidOperation(Strings.LDV_InvalidNumOfKeys(sortExpressionBuilder.Count));
            }
            //--------------------------------

            if (base.SortComparison == null)//using string to sort
            {
                return base.FindRowsByKey(key);
            }
            else
            {
                Range range = FindRecords<object, DataRow>(
                    new Index.ComparisonBySelector<object, DataRow>(comparerKeyRow),
                    new List<object>(key));
                return base.GetDataRowViewFromRange(range);
            }
        }
        #endregion


        #region Misc Overrides
        /// <summary>
        /// Overriding DataView's SetIndex to prevent users from setting RowState filter to anything other
        /// than CurrentRows.
        /// </summary>
        internal override void SetIndex(string newSort, DataViewRowState newRowStates, IFilter newRowFilter)
        {
            //Throw only if expressions (filter or sort) are used and rowstate is not current rows        
            if ((base.SortComparison != null || base.RowPredicate != null)
                    && newRowStates != DataViewRowState.CurrentRows)
            {
                throw DataSetUtil.Argument(Strings.LDVRowStateError);
            }
            else
            {
                base.SetIndex(newSort, newRowStates, newRowFilter);
            }
        }

        #endregion

        #region IBindingList

        /// <summary>
        /// Clears both expression-based and DataView's string-based sorting.
        /// </summary>
        void IBindingList.RemoveSort()
        {
            base.Sort = String.Empty;
            base.SortComparison = null;
        }

        /// <summary>
        /// Overrides IBindingList's SortProperty so that it returns null if expression based sort
        /// is used in the LinkDataView, otherwise it defers the result to DataView
        /// </summary>
        PropertyDescriptor IBindingList.SortProperty
        {
            get
            {
                return (base.SortComparison == null) ? base.GetSortProperty() : null;
            }
        }

        /// <summary>
        /// Overrides IBindingList's SortDescriptions so that it returns null if expression based sort
        /// is used in the LinkDataView, otherwise it defers the result to DataView
        /// </summary>
        ListSortDescriptionCollection IBindingListView.SortDescriptions
        {
            get
            {
                if (base.SortComparison == null)
                {
                    return base.GetSortDescriptions();
                }
                else
                {
                    return new ListSortDescriptionCollection();
                }
            }
        }

        /// <summary>
        /// Tells whether the LinqDataView is sorted or not
        /// </summary>
        bool IBindingList.IsSorted
        {
            get
            {   //Sorted if either expression based sort or string based sort is set
                return !(base.SortComparison == null && base.Sort.Length == 0);
            }
        }

        #endregion
    }
}
