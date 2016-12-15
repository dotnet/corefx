// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Data
{
    /// <summary>
    /// Represents an custom error that can be associated with a <see cref='System.Data.DataRow'/>.
    /// </summary>
    internal sealed class DataError
    {
        private string _rowError = string.Empty;

        // column-level errors
        private int _count;
        private ColumnError[] _errorList;
        internal const int initialCapacity = 1;

        internal DataError() { }

        internal DataError(string rowError)
        {
            SetText(rowError);
        }

        internal string Text
        {
            get { return _rowError; }
            set { SetText(value); }
        }

        internal bool HasErrors => _rowError.Length != 0 || _count != 0;

        // this method resets the error to the new value.
        internal void SetColumnError(DataColumn column, string error)
        {
            Debug.Assert(column != null, "Invalid (null) argument");
            Debug.Assert(column.Table != null, "Invalid (loose) column");
            if (error == null || error.Length == 0)
            {
                // remove error from the collection
                Clear(column);
            }
            else
            {
                if (_errorList == null)
                {
                    _errorList = new ColumnError[initialCapacity];
                }
                int i = IndexOf(column);
                _errorList[i]._column = column;
                _errorList[i]._error = error;
                column._errors++;
                if (i == _count)
                {
                    _count++;
                }
            }
        }

        internal string GetColumnError(DataColumn column)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_errorList[i]._column == column)
                {
                    return _errorList[i]._error;
                }
            }

            return string.Empty;
        }

        internal void Clear(DataColumn column)
        {
            if (_count == 0)
            {
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                if (_errorList[i]._column == column)
                {
                    Array.Copy(_errorList, i + 1, _errorList, i, _count - i - 1);
                    _count--;
                    column._errors--;
                    Debug.Assert(column._errors >= 0, "missing error counts");
                }
            }
        }

        internal void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _errorList[i]._column._errors--;
                Debug.Assert(_errorList[i]._column._errors >= 0, "missing error counts");
            }
            _count = 0;
            _rowError = string.Empty;
        }

        internal DataColumn[] GetColumnsInError()
        {
            var cols = new DataColumn[_count];

            for (int i = 0; i < _count; i++)
            {
                cols[i] = _errorList[i]._column;
            }

            return cols;
        }

        /// <summary>
        /// Sets the error message for the <see cref='System.Data.DataError'/>.
        /// </summary>
        private void SetText(string errorText)
        {
            if (null == errorText)
            {
                errorText = string.Empty;
            }
            _rowError = errorText;
        }

        internal int IndexOf(DataColumn column)
        {
            // try to find the column
            for (int i = 0; i < _count; i++)
            {
                if (_errorList[i]._column == column)
                {
                    return i;
                }
            }

            if (_count >= _errorList.Length)
            {
                int newCapacity = Math.Min(_count * 2, column.Table.Columns.Count);
                var biggerList = new ColumnError[newCapacity];
                Array.Copy(_errorList, 0, biggerList, 0, _count);
                _errorList = biggerList;
            }
            return _count;
        }

        internal struct ColumnError
        {
            internal DataColumn _column;
            internal string _error;
        };
    }
}
