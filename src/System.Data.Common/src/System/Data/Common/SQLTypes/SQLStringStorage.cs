// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace System.Data.Common
{
    internal sealed class SqlStringStorage : DataStorage
    {
        private SqlString[] _values;

        public SqlStringStorage(DataColumn column)
        : base(column, typeof(SqlString), SqlString.Null, SqlString.Null, StorageType.SqlString)
        {
        }

        public override object Aggregate(int[] recordNos, AggregateType kind)
        {
            try
            {
                int i;
                switch (kind)
                {
                    case AggregateType.Min:
                        int min = -1;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (IsNull(recordNos[i]))
                                continue;
                            min = recordNos[i];
                            break;
                        }
                        if (min >= 0)
                        {
                            for (i = i + 1; i < recordNos.Length; i++)
                            {
                                if (IsNull(recordNos[i]))
                                    continue;
                                if (Compare(min, recordNos[i]) > 0)
                                {
                                    min = recordNos[i];
                                }
                            }
                            return Get(min);
                        }
                        return _nullValue;


                    case AggregateType.Max:
                        int max = -1;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (IsNull(recordNos[i]))
                                continue;
                            max = recordNos[i];
                            break;
                        }
                        if (max >= 0)
                        {
                            for (i = i + 1; i < recordNos.Length; i++)
                            {
                                if (Compare(max, recordNos[i]) < 0)
                                {
                                    max = recordNos[i];
                                }
                            }
                            return Get(max);
                        }
                        return _nullValue;

                    case AggregateType.Count:
                        int count = 0;
                        for (i = 0; i < recordNos.Length; i++)
                        {
                            if (!IsNull(recordNos[i]))
                                count++;
                        }
                        return count;
                }
            }
            catch (OverflowException)
            {
                throw ExprException.Overflow(typeof(SqlString));
            }
            throw ExceptionBuilder.AggregateException(kind, _dataType);
        }

        public override int Compare(int recordNo1, int recordNo2)
        {
            return Compare(_values[recordNo1], _values[recordNo2]);
        }

        public int Compare(SqlString valueNo1, SqlString valueNo2)
        {
            if (valueNo1.IsNull && valueNo2.IsNull)
                return 0;

            if (valueNo1.IsNull)
                return -1;

            if (valueNo2.IsNull)
                return 1;

            return _table.Compare(valueNo1.Value, valueNo2.Value);
        }

        public override int CompareValueTo(int recordNo, object value)
        {
            return Compare(_values[recordNo], (SqlString)value);
        }

        public override object ConvertValue(object value)
        {
            if (null != value)
            {
                return SqlConvert.ConvertToSqlString(value);
            }
            return _nullValue;
        }

        public override void Copy(int recordNo1, int recordNo2)
        {
            _values[recordNo2] = _values[recordNo1];
        }

        public override object Get(int record)
        {
            return _values[record];
        }

        public override int GetStringLength(int record)
        {
            SqlString value = _values[record];
            return ((value.IsNull) ? 0 : value.Value.Length);
        }

        public override bool IsNull(int record)
        {
            return (_values[record].IsNull);
        }

        public override void Set(int record, object value)
        {
            _values[record] = SqlConvert.ConvertToSqlString(value);
        }

        public override void SetCapacity(int capacity)
        {
            SqlString[] newValues = new SqlString[capacity];
            if (null != _values)
            {
                Array.Copy(_values, 0, newValues, 0, Math.Min(capacity, _values.Length));
            }
            _values = newValues;
        }

        public override object ConvertXmlToObject(string s)
        {
            SqlString newValue = new SqlString();
            string tempStr = string.Concat("<col>", s, "</col>"); // this is done since you can give fragmet to reader
            StringReader strReader = new StringReader(tempStr);

            IXmlSerializable tmp = newValue;

            using (XmlTextReader xmlTextReader = new XmlTextReader(strReader))
            {
                tmp.ReadXml(xmlTextReader);
            }
            return ((SqlString)tmp);
        }

        public override string ConvertObjectToXml(object value)
        {
            Debug.Assert(!DataStorage.IsObjectNull(value), "we shouldn't have null here");
            Debug.Assert((value.GetType() == typeof(SqlString)), "wrong input type");

            StringWriter strwriter = new StringWriter(FormatProvider); // consider passing cultureinfo with CultureInfo.InvariantCulture

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(strwriter))
            {
                ((IXmlSerializable)value).WriteXml(xmlTextWriter);
            }
            return (strwriter.ToString());
        }

        protected override object GetEmptyStorage(int recordCount)
        {
            return new SqlString[recordCount];
        }

        protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
        {
            SqlString[] typedStore = (SqlString[])store;
            typedStore[storeIndex] = _values[record];
            nullbits.Set(storeIndex, IsNull(record));
        }

        protected override void SetStorage(object store, BitArray nullbits)
        {
            _values = (SqlString[])store;
            //SetNullStorage(nullbits);
        }
    }
}
