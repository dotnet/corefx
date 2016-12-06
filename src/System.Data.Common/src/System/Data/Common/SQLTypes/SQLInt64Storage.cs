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
    internal sealed class SqlInt64Storage : DataStorage
    {
        private SqlInt64[] _values;

        public SqlInt64Storage(DataColumn column)
        : base(column, typeof(SqlInt64), SqlInt64.Null, SqlInt64.Null, StorageType.SqlInt64)
        {
        }

        public override object Aggregate(int[] records, AggregateType kind)
        {
            bool hasData = false;
            try
            {
                switch (kind)
                {
                    case AggregateType.Sum:
                        SqlInt64 sum = 0;
                        foreach (int record in records)
                        {
                            if (IsNull(record))
                                continue;
                            checked { sum += _values[record]; }
                            hasData = true;
                        }
                        if (hasData)
                        {
                            return sum;
                        }
                        return _nullValue;

                    case AggregateType.Mean:
                        SqlDecimal meanSum = 0;
                        int meanCount = 0;
                        foreach (int record in records)
                        {
                            if (IsNull(record))
                                continue;
                            checked { meanSum += _values[record].ToSqlDecimal(); }
                            meanCount++;
                            hasData = true;
                        }
                        if (hasData)
                        {
                            SqlInt64 mean = 0;
                            checked { mean = (meanSum / meanCount).ToSqlInt64(); }
                            return mean;
                        }
                        return _nullValue;

                    case AggregateType.Var:
                    case AggregateType.StDev:
                        int count = 0;
                        SqlDouble var = 0;
                        SqlDouble prec = 0;
                        SqlDouble dsum = 0;
                        SqlDouble sqrsum = 0;

                        foreach (int record in records)
                        {
                            if (IsNull(record))
                                continue;
                            dsum += (_values[record]).ToSqlDouble();
                            sqrsum += (_values[record]).ToSqlDouble() * (_values[record]).ToSqlDouble();
                            count++;
                        }

                        if (count > 1)
                        {
                            var = count * sqrsum - (dsum * dsum);
                            prec = var / (dsum * dsum);

                            // we are dealing with the risk of a cancellation error
                            // double is guaranteed only for 15 digits so a difference 
                            // with a result less than 1e-15 should be considered as zero

                            if ((prec < 1e-15) || (var < 0))
                                var = 0;
                            else
                                var = var / (count * (count - 1));

                            if (kind == AggregateType.StDev)
                            {
                                return Math.Sqrt(var.Value);
                            }
                            return var;
                        }
                        return _nullValue;

                    case AggregateType.Min:
                        SqlInt64 min = SqlInt64.MaxValue;
                        for (int i = 0; i < records.Length; i++)
                        {
                            int record = records[i];
                            if (IsNull(record))
                                continue;
                            if ((SqlInt64.LessThan(_values[record], min)).IsTrue)
                                min = _values[record];
                            hasData = true;
                        }
                        if (hasData)
                        {
                            return min;
                        }
                        return _nullValue;

                    case AggregateType.Max:
                        SqlInt64 max = SqlInt64.MinValue;
                        for (int i = 0; i < records.Length; i++)
                        {
                            int record = records[i];
                            if (IsNull(record))
                                continue;

                            if ((SqlInt64.GreaterThan(_values[record], max)).IsTrue)
                                max = _values[record];
                            hasData = true;
                        }
                        if (hasData)
                        {
                            return max;
                        }
                        return _nullValue;

                    case AggregateType.First:
                        if (records.Length > 0)
                        {
                            return _values[records[0]];
                        }
                        return null;// no data => null

                    case AggregateType.Count:
                        count = 0;
                        for (int i = 0; i < records.Length; i++)
                        {
                            if (!IsNull(records[i]))
                                count++;
                        }
                        return count;
                }
            }
            catch (OverflowException)
            {
                throw ExprException.Overflow(typeof(SqlInt64));
            }
            throw ExceptionBuilder.AggregateException(kind, _dataType);
        }

        public override int Compare(int recordNo1, int recordNo2)
        {
            return _values[recordNo1].CompareTo(_values[recordNo2]);
        }

        public override int CompareValueTo(int recordNo, object value)
        {
            return _values[recordNo].CompareTo((SqlInt64)value);
        }

        public override object ConvertValue(object value)
        {
            if (null != value)
            {
                return SqlConvert.ConvertToSqlInt64(value);
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

        public override bool IsNull(int record)
        {
            return (_values[record].IsNull);
        }

        public override void Set(int record, object value)
        {
            _values[record] = SqlConvert.ConvertToSqlInt64(value);
        }

        public override void SetCapacity(int capacity)
        {
            SqlInt64[] newValues = new SqlInt64[capacity];
            if (null != _values)
            {
                Array.Copy(_values, 0, newValues, 0, Math.Min(capacity, _values.Length));
            }
            _values = newValues;
        }

        public override object ConvertXmlToObject(string s)
        {
            SqlInt64 newValue = new SqlInt64();
            string tempStr = string.Concat("<col>", s, "</col>"); // this is done since you can give fragmet to reader
            StringReader strReader = new StringReader(tempStr);

            IXmlSerializable tmp = newValue;

            using (XmlTextReader xmlTextReader = new XmlTextReader(strReader))
            {
                tmp.ReadXml(xmlTextReader);
            }
            return ((SqlInt64)tmp);
        }

        public override string ConvertObjectToXml(object value)
        {
            Debug.Assert(!DataStorage.IsObjectNull(value), "we shouldn't have null here");
            Debug.Assert((value.GetType() == typeof(SqlInt64)), "wrong input type");

            StringWriter strwriter = new StringWriter(FormatProvider);

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(strwriter))
            {
                ((IXmlSerializable)value).WriteXml(xmlTextWriter);
            }
            return (strwriter.ToString());
        }

        protected override object GetEmptyStorage(int recordCount)
        {
            return new SqlInt64[recordCount];
        }

        protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
        {
            SqlInt64[] typedStore = (SqlInt64[])store;
            typedStore[storeIndex] = _values[record];
            nullbits.Set(storeIndex, IsNull(record));
        }

        protected override void SetStorage(object store, BitArray nullbits)
        {
            _values = (SqlInt64[])store;
            //SetNullStorage(nullbits);
        }
    }
}
