// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// In the desktop version of the framework, this file is generated from ProviderBase\DbParameterCollectionHelper.cs
//#line 1 "e:\\fxdata\\src\\ndp\\fx\\src\\data\\system\\data\\providerbase\\dbparametercollectionhelper.cs"

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System.Data.SqlClient
{
    public sealed partial class SqlParameterCollection : DbParameterCollection
    {
        private List<SqlParameter> _items;

        override public int Count
        {
            get
            {
                return ((null != _items) ? _items.Count : 0);
            }
        }

        private List<SqlParameter> InnerList
        {
            get
            {
                List<SqlParameter> items = _items;

                if (null == items)
                {
                    items = new List<SqlParameter>();
                    _items = items;
                }
                return items;
            }
        }


        override public object SyncRoot
        {
            get
            {
                return ((System.Collections.ICollection)InnerList).SyncRoot;
            }
        }

        override public int Add(object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, value);
            InnerList.Add((SqlParameter)value);
            return Count - 1;
        }

        override public void AddRange(System.Array values)
        {
            OnChange();
            if (null == values)
            {
                throw ADP.ArgumentNull("values");
            }
            foreach (object value in values)
            {
                ValidateType(value);
            }
            foreach (SqlParameter value in values)
            {
                Validate(-1, value);
                InnerList.Add((SqlParameter)value);
            }
        }

        private int CheckName(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
            }
            return index;
        }

        override public void Clear()
        {
            OnChange();
            List<SqlParameter> items = InnerList;

            if (null != items)
            {
                foreach (SqlParameter item in items)
                {
                    item.ResetParent();
                }
                items.Clear();
            }
        }

        override public bool Contains(object value)
        {
            return (-1 != IndexOf(value));
        }

        override public void CopyTo(Array array, int index)
        {
            ((System.Collections.ICollection)InnerList).CopyTo(array, index);
        }

        override public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.ICollection)InnerList).GetEnumerator();
        }

        override protected DbParameter GetParameter(int index)
        {
            RangeCheck(index);
            return InnerList[index];
        }

        override protected DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
            }
            return InnerList[index];
        }

        private static int IndexOf(System.Collections.IEnumerable items, string parameterName)
        {
            if (null != items)
            {
                int i = 0;

                foreach (SqlParameter parameter in items)
                {
                    if (parameterName == parameter.ParameterName)
                    {
                        return i;
                    }
                    ++i;
                }
                i = 0;

                foreach (SqlParameter parameter in items)
                {
                    if (0 == ADP.DstCompare(parameterName, parameter.ParameterName))
                    {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        override public int IndexOf(string parameterName)
        {
            return IndexOf(InnerList, parameterName);
        }

        override public int IndexOf(object value)
        {
            if (null != value)
            {
                ValidateType(value);

                List<SqlParameter> items = InnerList;

                if (null != items)
                {
                    int count = items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (value == items[i])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        override public void Insert(int index, object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, (SqlParameter)value);
            InnerList.Insert(index, (SqlParameter)value);
        }

        private void RangeCheck(int index)
        {
            if ((index < 0) || (Count <= index))
            {
                throw ADP.ParametersMappingIndex(index, this);
            }
        }

        override public void Remove(object value)
        {
            OnChange();
            ValidateType(value);
            int index = IndexOf(value);
            if (-1 != index)
            {
                RemoveIndex(index);
            }
            else if (this != ((SqlParameter)value).CompareExchangeParent(null, this))
            {
                throw ADP.CollectionRemoveInvalidObject(s_itemType, this);
            }
        }

        override public void RemoveAt(int index)
        {
            OnChange();
            RangeCheck(index);
            RemoveIndex(index);
        }

        override public void RemoveAt(string parameterName)
        {
            OnChange();
            int index = CheckName(parameterName);
            RemoveIndex(index);
        }

        private void RemoveIndex(int index)
        {
            List<SqlParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "RemoveIndex, invalid");
            SqlParameter item = items[index];
            items.RemoveAt(index);
            item.ResetParent();
        }

        private void Replace(int index, object newValue)
        {
            List<SqlParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "Replace Index invalid");
            ValidateType(newValue);
            Validate(index, newValue);
            SqlParameter item = items[index];
            items[index] = (SqlParameter)newValue;
            item.ResetParent();
        }

        override protected void SetParameter(int index, DbParameter value)
        {
            OnChange();
            RangeCheck(index);
            Replace(index, value);
        }

        override protected void SetParameter(string parameterName, DbParameter value)
        {
            OnChange();
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
            }
            Replace(index, value);
        }

        private void Validate(int index, object value)
        {
            if (null == value)
            {
                throw ADP.ParameterNull("value", this, s_itemType);
            }

            object parent = ((SqlParameter)value).CompareExchangeParent(this, null);
            if (null != parent)
            {
                if (this != parent)
                {
                    throw ADP.ParametersIsNotParent(s_itemType, this);
                }
                if (index != IndexOf(value))
                {
                    throw ADP.ParametersIsParent(s_itemType, this);
                }
            }

            String name = ((SqlParameter)value).ParameterName;
            if (0 == name.Length)
            {
                index = 1;
                do
                {
                    name = ADP.Parameter + index.ToString(CultureInfo.CurrentCulture);
                    index++;
                } while (-1 != IndexOf(name));
                ((SqlParameter)value).ParameterName = name;
            }
        }

        private void ValidateType(object value)
        {
            if (null == value)
            {
                throw ADP.ParameterNull("value", this, s_itemType);
            }
            else if (!s_itemType.IsInstanceOfType(value))
            {
                throw ADP.InvalidParameterType(this, s_itemType, value);
            }
        }
    };
}

