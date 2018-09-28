// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>Provides a type converter to convert Encoding objects to and
    ///       from various other representations.</para>
    /// </summary>
    public class EncodingConverter : TypeConverter
    {
        /// <summary>
        ///    <para>Gets a value indicating whether this converter can convert an object in the
        ///       given source type to an Encoding.</para>
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            // string + all types implicitly convertible to int: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/implicit-numeric-conversions-table
            return sourceType == typeof(string) || sourceType == typeof(int)
                || sourceType == typeof(sbyte) || sourceType == typeof(byte)
                || sourceType == typeof(short) || sourceType == typeof(ushort)
                || sourceType == typeof(char) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(int) || destinationType == typeof(string);
        }

        /// <summary>
        ///    <para>Converts the given object to an Encoding.</para>
        /// </summary>
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid codePage or a valid encoding name</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name)
            {
                try
                {
                    return Encoding.GetEncoding(name);
                }
                catch (Exception e)
                {
                    if (int.TryParse(name, NumberStyles.Integer, culture, out var codePage))
                    {
                        throw new FormatException(SR.Format(SR.ConvertInvalidPrimitive, $"{value} ({typeof(string)})", nameof(Encoding)), e);
                    }
                    throw new FormatException(SR.Format(SR.ConvertInvalidPrimitive, value, nameof(Encoding)), e);
                }
            }

            if (value is null || value is bool || value is float || value is double || value is decimal)
            {
                throw GetConvertFromException(value);
            }

            try
            {
                int codePage = (int)Convert.ChangeType(value, typeof(int));
                return Encoding.GetEncoding(codePage);
            }
            catch (InvalidCastException)
            {
                throw GetConvertFromException(value);
            }
            catch (Exception e)
            {
                throw new FormatException(SR.Format(SR.ConvertInvalidPrimitive, value, nameof(Encoding)), e);
            }
        }

        /// <summary>
        ///    <para>Converts the given value object to
        ///       the specified destination type using the specified context and arguments.</para>
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Encoding encoding)
            {
                if (destinationType == typeof(int))
                    return encoding.CodePage;

                if (destinationType == typeof(string))
                    return encoding.BodyName;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
