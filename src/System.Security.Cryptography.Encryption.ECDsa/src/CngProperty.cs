// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Wrapper represeting an arbitrary property of a CNG key or provider
    /// </summary>
    internal struct CngProperty : IEquatable<CngProperty>
    {
        private string _name;
        private CngPropertyOptions _propertyOptions;
        private byte[] _value;
        private int? _hashCode;

        public CngProperty(string name, byte[] value, CngPropertyOptions options)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            // @TODO Spec#: Not clear how to express this currently.
            //Contract.Ensures(m_name != null);

            _name = name;
            _propertyOptions = options;
            _hashCode = null;

            if (value != null)
            {
                _value = value.Clone() as byte[];
            }
            else
            {
                _value = null;
            }
        }

        /// <summary>
        ///     Name of the property
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return _name;
            }
        }

        /// <summary>
        ///     Options used to set / get the property
        /// </summary>
        public CngPropertyOptions Options
        {
            get { return _propertyOptions; }
        }

        /// <summary>
        ///     Direct value of the property -- if the value will be returned to user code or modified, use
        ///     GetValue() instead.
        /// </summary>
        internal byte[] Value
        {
            get { return _value; }
        }

        /// <summary>
        ///     Contents of the property
        /// </summary>
        /// <returns></returns>
        public byte[] GetValue()
        {
            byte[] value = null;

            if (_value != null)
            {
                value = _value.Clone() as byte[];
            }

            return value;
        }

        public static bool operator ==(CngProperty left, CngProperty right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CngProperty left, CngProperty right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is CngProperty))
            {
                return false;
            }

            return Equals((CngProperty)obj);
        }

        public bool Equals(CngProperty other)
        {
            //
            // We will consider CNG properties equal only if the name, options and value are all also equal
            //

            if (!String.Equals(Name, other.Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (Options != other.Options)
            {
                return false;
            }

            if (_value == null)
            {
                return other._value == null;
            }
            if (other._value == null)
            {
                return false;
            }

            if (_value.Length != other._value.Length)
            {
                return false;
            }

            for (int i = 0; i < _value.Length; i++)
            {
                if (_value[i] != other._value[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                int hashCode = Name.GetHashCode() ^ Options.GetHashCode();

                // The hash code for a byte is just the value of that byte. Since this will only modify the
                // lower bits of the hash code, we'll xor each byte into different sections of the hash code
                if (_value != null)
                {
                    for (int i = 0; i < _value.Length; i++)
                    {
                        // Shift each byte forward by one byte, so that every 4 bytes has to potential to update
                        // each of the calculated hash code's bytes.
                        int shifted = (int)(_value[i] << ((i % 4) * 8));
                        hashCode ^= shifted;
                    }
                }

                _hashCode = hashCode;
            }

            return _hashCode.Value;
        }
    }

    /// <summary>
    ///     Strongly typed collection of CNG properties
    /// </summary>
    internal sealed class CngPropertyCollection : Collection<CngProperty>
    {
    }
}
