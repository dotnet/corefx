// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert <see cref="Array" /> objects to and from various
    /// other representations.
    /// </summary>
    public partial class ArrayConverter : System.ComponentModel.CollectionConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayConverter" /> class.
        /// </summary>
        public ArrayConverter() { }
        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The culture into which <paramref name="value" /> will be converted.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a base type converter for nonfloating-point numerical types.
    /// </summary>
    public abstract partial class BaseNumberConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNumberConverter" />
        /// class.
        /// </summary>
        protected BaseNumberConverter() { }
        /// <summary>
        /// Determines if this converter can convert an object in the given source type to the native type
        /// of the converter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="Type" /> that represents the type from which you want to convert.
        /// </param>
        /// <returns>
        /// true if this converter can perform the operation; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="t">A <see cref="Type" /> that represents the type to which you want to convert.</param>
        /// <returns>
        /// true if this converter can perform the operation; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type t) { return default(bool); }
        /// <summary>
        /// Converts the given object to the converter's native type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" /> that specifies the culture to represent
        /// the number.
        /// </param>
        /// <param name="value">The object to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="Exception">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the specified object to another type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" /> that specifies the culture to represent
        /// the number.
        /// </param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="Boolean" /> objects to and from various
    /// other representations.
    /// </summary>
    public partial class BooleanConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter" />
        /// class.
        /// </summary>
        public BooleanConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a Boolean object using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this object can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Converts the given value object to a Boolean object.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" /> that specifies the culture to which to
        /// convert.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert 8-bit unsigned integer objects to and from various other
    /// representations.
    /// </summary>
    public partial class ByteConverter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteConverter" /> class.
        /// </summary>
        public ByteConverter() { }
    }
    /// <summary>
    /// Provides a type converter to convert Unicode character objects to and from various other representations.
    /// </summary>
    public partial class CharConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharConverter" /> class.
        /// </summary>
        public CharConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a Unicode character object using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Converts the given object to a Unicode character object.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The culture into which <paramref name="value" /> will be converted.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value object to a Unicode character object using the arguments.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The culture into which <paramref name="value" /> will be converted.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert collection objects to and from various other representations.
    /// </summary>
    public partial class CollectionConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionConverter" />
        /// class.
        /// </summary>
        public CollectionConverter() { }
        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The culture to which <paramref name="value" /> will be converted.</param>
        /// <param name="value">
        /// The <see cref="Object" /> to convert. This parameter must inherit from
        /// <see cref="Collections.ICollection" />.
        /// </param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="DateTime" /> objects to and from
    /// various other representations.
    /// </summary>
    public partial class DateTimeConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeConverter" />
        /// class.
        /// </summary>
        public DateTimeConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a <see cref="DateTime" /> using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this object can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given value object to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value object to a <see cref="DateTime" /> using the arguments.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="DateTimeOffset" /> structures to
    /// and from various other representations.
    /// </summary>
    public partial class DateTimeOffsetConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeOffsetConverter" />
        /// class.
        /// </summary>
        public DateTimeOffsetConverter() { }
        /// <summary>
        /// Returns a value that indicates whether an object of the specified source type can be converted
        /// to a <see cref="DateTimeOffset" />.
        /// </summary>
        /// <param name="context">The date format context.</param>
        /// <param name="sourceType">The source type to check.</param>
        /// <returns>
        /// true if the specified type can be converted to a <see cref="DateTimeOffset" />; otherwise,
        /// false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns a value that indicates whether a <see cref="DateTimeOffset" /> can be converted
        /// to an object of the specified type.
        /// </summary>
        /// <param name="context">The date format context.</param>
        /// <param name="destinationType">The destination type to check.</param>
        /// <returns>
        /// true if a <see cref="DateTimeOffset" /> can be converted to the specified type; otherwise,
        /// false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the specified object to a <see cref="DateTimeOffset" />.
        /// </summary>
        /// <param name="context">The date format context.</param>
        /// <param name="culture">The date culture.</param>
        /// <param name="value">The object to be converted.</param>
        /// <returns>
        /// A <see cref="DateTimeOffset" /> that represents the specified object.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts a <see cref="DateTimeOffset" /> to an object of the specified type.
        /// </summary>
        /// <param name="context">The date format context.</param>
        /// <param name="culture">The date culture.</param>
        /// <param name="value">The <see cref="DateTimeOffset" /> to be converted.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>
        /// An object of the specified type that represents the <see cref="DateTimeOffset" />.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="Decimal" /> objects to and from various
    /// other representations.
    /// </summary>
    public partial class DecimalConverter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalConverter" />
        /// class.
        /// </summary>
        public DecimalConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given value object to a <see cref="Decimal" /> using the arguments.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert double-precision, floating point number objects to and
    /// from various other representations.
    /// </summary>
    public partial class DoubleConverter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleConverter" /> class.
        /// </summary>
        public DoubleConverter() { }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="Enum" /> objects to and from various
    /// other representations.
    /// </summary>
    public partial class EnumConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumConverter" /> class
        /// for the given type.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type" /> that represents the type of enumeration to associate with this
        /// enumeration converter.
        /// </param>
        public EnumConverter(System.Type type) { }
        /// <summary>
        /// Specifies the type of the enumerator this converter is associated with.
        /// </summary>
        /// <returns>
        /// The type of the enumerator this converter is associated with.
        /// </returns>
        protected System.Type EnumType { get { return default(System.Type); } }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to an enumeration object using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the specified value object to an enumeration object.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value" /> is not a valid value for the enumeration.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="Guid" /> objects to and from various
    /// other representations.
    /// </summary>
    public partial class GuidConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidConverter" /> class.
        /// </summary>
        public GuidConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a GUID object using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given object to a GUID object.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given object to another type.
        /// </summary>
        /// <param name="context">A formatter context.</param>
        /// <param name="culture">The culture into which <paramref name="value" /> will be converted.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>
        /// The converted object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert 16-bit signed integer objects to and from other representations.
    /// </summary>
    public partial class Int16Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int16Converter" /> class.
        /// </summary>
        public Int16Converter() { }
    }
    /// <summary>
    /// Provides a type converter to convert 32-bit signed integer objects to and from other representations.
    /// </summary>
    public partial class Int32Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Converter" /> class.
        /// </summary>
        public Int32Converter() { }
    }
    /// <summary>
    /// Provides a type converter to convert 64-bit signed integer objects to and from various other
    /// representations.
    /// </summary>
    public partial class Int64Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64Converter" /> class.
        /// </summary>
        public Int64Converter() { }
    }
    /// <summary>
    /// Provides contextual information about a component, such as its container and property descriptor.
    /// </summary>
    public partial interface ITypeDescriptorContext : System.IServiceProvider
    {
        /// <summary>
        /// Gets the container representing this <see cref="System.ComponentModel.TypeDescriptor" />
        /// request.
        /// </summary>
        /// <returns>
        /// An <see cref="System.ComponentModel.IContainer" /> with the set of objects for this
        /// <see cref="System.ComponentModel.TypeDescriptor" />; otherwise, null if there is no container
        /// or if the <see cref="System.ComponentModel.TypeDescriptor" /> does not use outside objects.
        /// </returns>
        System.ComponentModel.IContainer Container { get; }
        /// <summary>
        /// Gets the object that is connected with this type descriptor request.
        /// </summary>
        /// <returns>
        /// The object that invokes the method on the <see cref="TypeDescriptor" />
        /// ; otherwise, null if there is no object responsible for the call.
        /// </returns>
        object Instance { get; }
        /// <summary>
        /// Gets the <see cref="System.ComponentModel.PropertyDescriptor" /> that is associated with
        /// the given context item.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ComponentModel.PropertyDescriptor" /> that describes the given context
        /// item; otherwise, null if there is no <see cref="System.ComponentModel.PropertyDescriptor" />
        /// responsible for the call.
        /// </returns>
        System.ComponentModel.PropertyDescriptor PropertyDescriptor { get; }
        /// <summary>
        /// Raises the <see cref="ComponentModel.Design.IComponentChangeService.ComponentChanged" />
        /// event.
        /// </summary>
        void OnComponentChanged();
        /// <summary>
        /// Raises the <see cref="ComponentModel.Design.IComponentChangeService.ComponentChanging" />
        /// event.
        /// </summary>
        /// <returns>
        /// true if this object can be changed; otherwise, false.
        /// </returns>
        bool OnComponentChanging();
    }
    /// <summary>
    /// Provides a type converter to convert multiline strings to a simple string.
    /// </summary>
    public partial class MultilineStringConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultilineStringConverter" />
        /// class.
        /// </summary>
        public MultilineStringConverter() { }
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" />. If null is passed, the current culture
        /// is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides automatic conversion between a nullable type and its underlying primitive type.
    /// </summary>
    public partial class NullableConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableConverter" />
        /// class.
        /// </summary>
        /// <param name="type">The specified nullable type.</param>
        /// <exception cref="ArgumentException"><paramref name="type" /> is not a nullable type.</exception>
        public NullableConverter(System.Type type) { }
        /// <summary>
        /// Gets the nullable type.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> that represents the nullable type.
        /// </returns>
        public System.Type NullableType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> that represents the underlying type.
        /// </returns>
        public System.Type UnderlyingType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the underlying type converter.
        /// </summary>
        /// <returns>
        /// A <see cref="TypeConverter" /> that represents the underlying type
        /// converter.
        /// </returns>
        public System.ComponentModel.TypeConverter UnderlyingTypeConverter { get { return default(System.ComponentModel.TypeConverter); } }
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this
        /// converter, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified
        /// context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">A <see cref="Type" /> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The <see cref="Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The <see cref="Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides an abstraction of a property on a class.
    /// </summary>
    public abstract partial class PropertyDescriptor
    {
        internal PropertyDescriptor() { }
    }
    /// <summary>
    /// Provides a type converter to convert 8-bit unsigned integer objects to and from a string.
    /// </summary>
    public partial class SByteConverter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SByteConverter" /> class.
        /// </summary>
        public SByteConverter() { }
    }
    /// <summary>
    /// Provides a type converter to convert single-precision, floating point number objects to and
    /// from various other representations.
    /// </summary>
    public partial class SingleConverter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleConverter" /> class.
        /// </summary>
        public SingleConverter() { }
    }
    /// <summary>
    /// Provides a type converter to convert string objects to and from other representations.
    /// </summary>
    public partial class StringConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringConverter" /> class.
        /// </summary>
        public StringConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a string using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Converts the specified value object to a <see cref="String" /> object.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The <see cref="Globalization.CultureInfo" /> to use.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion could not be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert <see cref="TimeSpan" /> objects to and from
    /// other representations.
    /// </summary>
    public partial class TimeSpanConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanConverter" />
        /// class.
        /// </summary>
        public TimeSpanConverter() { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the given source type
        /// to a <see cref="TimeSpan" /> using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you wish to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given object to a <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="value" /> is not a valid value for the target type.
        /// </exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given object to another type.
        /// </summary>
        /// <param name="context">A formatter context.</param>
        /// <param name="culture">The culture into which <paramref name="value" /> will be converted.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The type to convert the object to.</param>
        /// <returns>
        /// The converted object.
        /// </returns>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a unified way of converting types of values to other types, as well as for accessing
    /// standard values and subproperties.
    /// </summary>
    public partial class TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverter" /> class.
        /// </summary>
        public TypeConverter() { }
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this
        /// converter, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public virtual bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this
        /// converter.
        /// </summary>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public bool CanConvertFrom(System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified
        /// context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you want to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public virtual bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type.
        /// </summary>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you want to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public bool CanConvertTo(System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">The <see cref="Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public virtual object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value to the type of this converter.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public object ConvertFrom(object value) { return default(object); }
        /// <summary>
        /// Converts the given string to the type of this converter, using the invariant culture.
        /// </summary>
        /// <param name="text">The <see cref="String" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted text.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public object ConvertFromInvariantString(string text) { return default(object); }
        /// <summary>
        /// Converts the given text to an object, using the specified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" />. If null is passed, the current culture
        /// is assumed.
        /// </param>
        /// <param name="text">The <see cref="String" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted text.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public object ConvertFromString(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, string text) { return default(object); }
        /// <summary>
        /// Converts the specified text to an object.
        /// </summary>
        /// <param name="text">The text representation of the object to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted text.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The string cannot be converted into the appropriate object.
        /// </exception>
        public object ConvertFromString(string text) { return default(object); }
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" />. If null is passed, the current culture
        /// is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">
        /// The <see cref="Type" /> to convert the <paramref name="value" /> parameter to.
        /// </param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="destinationType" /> parameter is null.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public virtual object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
        /// <summary>
        /// Converts the given value object to the specified type, using the arguments.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">
        /// The <see cref="Type" /> to convert the <paramref name="value" /> parameter to.
        /// </param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="destinationType" /> parameter is null.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public object ConvertTo(object value, System.Type destinationType) { return default(object); }
        /// <summary>
        /// Converts the specified value to a culture-invariant string representation.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// A <see cref="String" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public string ConvertToInvariantString(object value) { return default(string); }
        /// <summary>
        /// Converts the given value to a string representation, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" />. If null is passed, the current culture
        /// is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public string ConvertToString(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(string); }
        /// <summary>
        /// Converts the specified value to a string representation.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public string ConvertToString(object value) { return default(string); }
        /// <summary>
        /// Returns an exception to throw when a conversion cannot be performed.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert, or null if the object is not available.</param>
        /// <returns>
        /// An <see cref="Exception" /> that represents the exception to throw when a conversion
        /// cannot be performed.
        /// </returns>
        /// <exception cref="NotSupportedException">Automatically thrown by this method.</exception>
        protected System.Exception GetConvertFromException(object value) { return default(System.Exception); }
        /// <summary>
        /// Returns an exception to throw when a conversion cannot be performed.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to convert, or null if the object is not available.</param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type the conversion was trying to convert
        /// to.
        /// </param>
        /// <returns>
        /// An <see cref="Exception" /> that represents the exception to throw when a conversion
        /// cannot be performed.
        /// </returns>
        /// <exception cref="NotSupportedException">Automatically thrown by this method.</exception>
        protected System.Exception GetConvertToException(object value, System.Type destinationType) { return default(System.Exception); }
    }
    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is bound to.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(32767))]
    public sealed partial class TypeConverterAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterAttribute" />
        /// class, using the specified type name as the data converter for the object this attribute
        /// is bound to.
        /// </summary>
        /// <param name="typeName">
        /// The fully qualified name of the class to use for data conversion for the object this attribute
        /// is bound to.
        /// </param>
        public TypeConverterAttribute(string typeName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterAttribute" />
        /// class, using the specified type as the data converter for the object this attribute is
        /// bound to.
        /// </summary>
        /// <param name="type">
        /// A <see cref="Type" /> that represents the type of the converter class to use for
        /// data conversion for the object this attribute is bound to.
        /// </param>
        public TypeConverterAttribute(System.Type type) { }
        /// <summary>
        /// Gets the fully qualified type name of the <see cref="Type" /> to use as a converter
        /// for the object this attribute is bound to.
        /// </summary>
        /// <returns>
        /// The fully qualified type name of the <see cref="Type" /> to use as a converter for
        /// the object this attribute is bound to, or an empty string ("") if none exists. The default
        /// value is an empty string ("").
        /// </returns>
        public string ConverterTypeName { get { return default(string); } }
        /// <summary>
        /// Returns whether the value of the given object is equal to the current
        /// <see cref="TypeConverterAttribute" />.
        /// </summary>
        /// <param name="obj">The object to test the value equality of.</param>
        /// <returns>
        /// true if the value of the given object is equal to that of the current
        /// <see cref="TypeConverterAttribute" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="TypeConverterAttribute" />.
        /// </returns>
        public override int GetHashCode() { return default(int); }
    }
    /// <summary>
    /// Provides information about the characteristics for a component, such as its attributes, properties,
    /// and events. This class cannot be inherited.
    /// </summary>
    public sealed partial class TypeDescriptor
    {
        internal TypeDescriptor() { }
        /// <summary>
        /// Returns a type converter for the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the target component.</param>
        /// <returns>
        /// A <see cref="TypeConverter" /> for the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="component" /> is null.</exception>
        public static System.ComponentModel.TypeConverter GetConverter(System.Type type) { return default(System.ComponentModel.TypeConverter); }
    }
    /// <summary>
    /// Provides a type converter that can be used to populate a list box with available types.
    /// </summary>
    public abstract partial class TypeListConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeListConverter" />
        /// class using the type array as the available types.
        /// </summary>
        /// <param name="types">The array of type <see cref="Type" /> to use as the available types.</param>
        protected TypeListConverter(System.Type[] types) { }
        /// <summary>
        /// Gets a value indicating whether this converter can convert the specified <see cref="Type" />
        /// of the source object using the given context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="sourceType">The <see cref="Type" /> of the source object.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination
        /// type using the context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the specified object to the native type of the converter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" /> that specifies the culture used to represent
        /// the font.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts the given value object to the specified destination type.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// An optional <see cref="Globalization.CultureInfo" />. If not supplied, the current
        /// culture is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="Type" /> to convert the value to.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="destinationType" /> is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
    /// <summary>
    /// Provides a type converter to convert 16-bit unsigned integer objects to and from other representations.
    /// </summary>
    public partial class UInt16Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UInt16Converter" /> class.
        /// </summary>
        public UInt16Converter() { }
    }
    /// <summary>
    /// Provides a type converter to convert 32-bit unsigned integer objects to and from various other
    /// representations.
    /// </summary>
    public partial class UInt32Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UInt32Converter" /> class.
        /// </summary>
        public UInt32Converter() { }
    }
    /// <summary>
    /// Provides a type converter to convert 64-bit unsigned integer objects to and from other representations.
    /// </summary>
    public partial class UInt64Converter : System.ComponentModel.BaseNumberConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UInt64Converter" /> class.
        /// </summary>
        public UInt64Converter() { }
    }
}

namespace System
{
    /// <summary>
    /// Converts a <see cref="String" /> type to a <see cref="System.Uri" /> type, and
    /// vice versa.
    /// </summary>
    public partial class UriTypeConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriTypeConverter" /> class.
        /// </summary>
        public UriTypeConverter() { }
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this
        /// converter.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ComponentModel.ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="Type" /> that represents the type that you want to convert from.
        /// </param>
        /// <returns>
        /// true if <paramref name="sourceType" /> is a <see cref="String" /> type or a
        /// <see cref="System.Uri" /> type can be assigned from <paramref name="sourceType" />; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceType" /> parameter is null.</exception>
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType) { return default(bool); }
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified
        /// context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ComponentModel.ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A <see cref="Type" /> that represents the type that you want to convert to.
        /// </param>
        /// <returns>
        /// true if <paramref name="destinationType" /> is of type
        /// <see cref="ComponentModel.Design.Serialization.InstanceDescriptor" />, <see cref="String" />, or <see cref="System.Uri" />; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) { return default(bool); }
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ComponentModel.ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="culture">The <see cref="Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) { return default(object); }
        /// <summary>
        /// Converts a given value object to the specified type, using the specified context and culture
        /// information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ComponentModel.ITypeDescriptorContext" />  that provides a format context.
        /// </param>
        /// <param name="culture">
        /// A <see cref="Globalization.CultureInfo" />. If null is passed, the current culture
        /// is assumed.
        /// </param>
        /// <param name="value">The <see cref="Object" /> to convert.</param>
        /// <param name="destinationType">
        /// The <see cref="Type" /> to convert the <paramref name="value" /> parameter to.
        /// </param>
        /// <returns>
        /// An <see cref="Object" /> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="destinationType" /> parameter is null.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) { return default(object); }
    }
}
