// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// In the desktop version of the framework, this file is generated from ProviderBase\DbParameterHelper.cs
//#line 1 "e:\\fxdata\\src\\ndp\\fx\\src\\data\\system\\data\\providerbase\\dbparameterhelper.cs"

using System.Data.Common;

namespace System.Data.SqlClient
{
    public sealed partial class SqlParameter : DbParameter
    {
        private object _value;

        private object _parent;

        private ParameterDirection _direction;
        private int _size;

        private int _offset;
        private string _sourceColumn;
        private bool _sourceColumnNullMapping;

        private bool _isNullable;

        private object _coercedValue;


        private object CoercedValue
        {
            get
            {
                return _coercedValue;
            }
            set
            {
                _coercedValue = value;
            }
        }

        override public ParameterDirection Direction
        {
            get
            {
                ParameterDirection direction = _direction;
                return ((0 != direction) ? direction : ParameterDirection.Input);
            }
            set
            {
                if (_direction != value)
                {
                    switch (value)
                    {
                        case ParameterDirection.Input:
                        case ParameterDirection.Output:
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.ReturnValue:
                            PropertyChanging();
                            _direction = value;
                            break;
                        default:
                            throw ADP.InvalidParameterDirection(value);
                    }
                }
            }
        }

        override public bool IsNullable
        {
            get
            {
                return _isNullable;
            }
            set
            {
                _isNullable = value;
            }
        }


        public int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (value < 0)
                {
                    throw ADP.InvalidOffsetValue(value);
                }
                _offset = value;
            }
        }

        override public int Size
        {
            get
            {
                int size = _size;
                if (0 == size)
                {
                    size = ValueSize(Value);
                }
                return size;
            }
            set
            {
                if (_size != value)
                {
                    if (value < -1)
                    {
                        throw ADP.InvalidSizeValue(value);
                    }
                    PropertyChanging();
                    _size = value;
                }
            }
        }


        private bool ShouldSerializeSize()
        {
            return (0 != _size);
        }

        override public string SourceColumn
        {
            get
            {
                string sourceColumn = _sourceColumn;
                return ((null != sourceColumn) ? sourceColumn : ADP.StrEmpty);
            }
            set
            {
                _sourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get
            {
                return _sourceColumnNullMapping;
            }
            set
            {
                _sourceColumnNullMapping = value;
            }
        }


        internal object CompareExchangeParent(object value, object comparand)
        {
            object parent = _parent;
            if (comparand == parent)
            {
                _parent = value;
            }
            return parent;
        }

        internal void ResetParent()
        {
            _parent = null;
        }

        override public string ToString()
        {
            return ParameterName;
        }

        private byte ValuePrecisionCore(object value)
        {
            if (value is Decimal)
            {
                return ((System.Data.SqlTypes.SqlDecimal)(Decimal)value).Precision;
            }
            return 0;
        }

        private byte ValueScaleCore(object value)
        {
            if (value is Decimal)
            {
                return (byte)((Decimal.GetBits((Decimal)value)[3] & 0x00ff0000) >> 0x10);
            }
            return 0;
        }

        private int ValueSizeCore(object value)
        {
            if (!ADP.IsNull(value))
            {
                string svalue = (value as string);
                if (null != svalue)
                {
                    return svalue.Length;
                }
                byte[] bvalue = (value as byte[]);
                if (null != bvalue)
                {
                    return bvalue.Length;
                }
                char[] cvalue = (value as char[]);
                if (null != cvalue)
                {
                    return cvalue.Length;
                }
                if ((value is byte) || (value is char))
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}

