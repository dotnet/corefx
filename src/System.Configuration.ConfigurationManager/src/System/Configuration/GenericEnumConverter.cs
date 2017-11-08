// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.ComponentModel;
using System.Text;

namespace System.Configuration
{
    public sealed class GenericEnumConverter : ConfigurationConverterBase
    {
        private readonly Type _enumType;

        public GenericEnumConverter(Type typeEnum)
        {
            if (typeEnum == null)
                throw new ArgumentNullException(nameof(typeEnum));

            _enumType = typeEnum;
        }

        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            return value.ToString();
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            object result;

            // For any error, throw the ArgumentException with SR.Invalid_enum_value
            try
            {
                string value = (string)data;
                bool valueIsInvalid = string.IsNullOrEmpty(value);

                // Disallow numeric values for enums.
                if (!valueIsInvalid &&
                    (char.IsDigit(value[0]) || (value[0] == '-') || (value[0] == '+')))
                {
                    valueIsInvalid = true;
                }

                if (!valueIsInvalid)
                {
                    foreach (char c in value)
                    {
                        // throw if the value has whitespace
                        if (Char.IsWhiteSpace(c))
                        {
                            valueIsInvalid = true;
                            break;
                        }
                    }
                }
                if (valueIsInvalid)
                {
                    throw CreateExceptionForInvalidValue();
                }

                result = Enum.Parse(_enumType, value);
            }
            catch
            {
                throw CreateExceptionForInvalidValue();
            }
            return result;
        }

        private ArgumentException CreateExceptionForInvalidValue()
        {
            StringBuilder names = new StringBuilder();

            foreach (string name in Enum.GetNames(_enumType))
            {
                if (names.Length != 0)
                {
                    names.Append(", ");
                }
                names.Append(name);
            }
            return new ArgumentException(string.Format(SR.Invalid_enum_value, names.ToString()));
        }
    }
}
