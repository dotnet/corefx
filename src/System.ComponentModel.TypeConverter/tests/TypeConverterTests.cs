﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class TypeConverterTests : TypeConverter
    {
        public static TypeConverter s_converter = new TypeConverter();
        public static ITypeDescriptorContext s_context = new MyTypeDescriptorContext();
        private const int c_conversionInputValue = 1;
        private const string c_conversionResult = "1";

        [Fact]
        public static void CanConvertFrom_string()
        {
            Assert.False(TypeConverterTests.s_converter.CanConvertFrom(typeof(string)));
        }

        [Fact]
        public static void CanConvertFrom_string_WithContext()
        {
            Assert.False(TypeConverterTests.s_converter.CanConvertFrom(
                TypeConverterTests.s_context, typeof(string)));
        }

        [Fact]
        public static void CanConvertTo_string()
        {
            Assert.True(TypeConverterTests.s_converter.CanConvertTo(typeof(string)));
        }

        [Fact]
        public static void CanConvertTo_string_WithContext()
        {
            Assert.True(TypeConverterTests.s_converter.CanConvertTo(
                TypeConverterTests.s_context, typeof(string)));
        }

        [Fact]
        public static void ConvertFrom_Negative()
        {
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFrom("1"));
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFrom(null));
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFrom(TypeConverterTests.s_context, null, "1"));
        }

        [Fact]
        public static void ConvertFromInvariantString()
        {
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFromInvariantString("1"));
        }

        [Fact]
        public static void ConvertFromString()
        {
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFromString("1"));
        }

        [Fact]
        public static void ConvertFromString_WithContext()
        {
            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertFromString(TypeConverterTests.s_context, null, "1"));
        }

        [Fact]
        public static void ConvertTo_string()
        {
            object o = TypeConverterTests.s_converter.ConvertTo(TypeConverterTests.c_conversionInputValue, typeof(string));
            TypeConverterTests.VerifyConversionToString(o);
        }

        [Fact]
        public static void ConvertTo_WithContext()
        {
            Assert.Throws<ArgumentNullException>(
                () => TypeConverterTests.s_converter.ConvertTo(TypeConverterTests.s_context, null, TypeConverterTests.c_conversionInputValue, null));

            Assert.Throws<NotSupportedException>(
                () => TypeConverterTests.s_converter.ConvertTo(TypeConverterTests.s_context, null, TypeConverterTests.c_conversionInputValue, typeof(int)));

            object o = TypeConverterTests.s_converter.ConvertTo(
                TypeConverterTests.s_context, null, TypeConverterTests.c_conversionInputValue, typeof(string));
            TypeConverterTests.VerifyConversionToString(o);

            o = TypeConverterTests.s_converter.ConvertTo(
                TypeConverterTests.s_context, CultureInfo.CurrentCulture, TypeConverterTests.c_conversionInputValue, typeof(string));
            TypeConverterTests.VerifyConversionToString(o);

            o = TypeConverterTests.s_converter.ConvertTo(
                TypeConverterTests.s_context, CultureInfo.InvariantCulture, TypeConverterTests.c_conversionInputValue, typeof(string));
            TypeConverterTests.VerifyConversionToString(o);

            string s = TypeConverterTests.s_converter.ConvertTo(
                TypeConverterTests.s_context, CultureInfo.InvariantCulture, new FormattableClass(), typeof(string)) as string;
            Assert.NotNull(s);
            Assert.Equal(FormattableClass.Token, s);
        }

        [Fact]
        public static void ConvertToInvariantString()
        {
            object o = TypeConverterTests.s_converter.ConvertToInvariantString(
                TypeConverterTests.c_conversionInputValue);
            TypeConverterTests.VerifyConversionToString(o);
        }

        [Fact]
        public static void ConvertToString()
        {
            object o = TypeConverterTests.s_converter.ConvertToString(
                TypeConverterTests.c_conversionInputValue);
            TypeConverterTests.VerifyConversionToString(o);
        }

        [Fact]
        public static void ConvertToString_WithContext()
        {
            object o = TypeConverterTests.s_converter.ConvertToString(
                TypeConverterTests.s_context, null, TypeConverterTests.c_conversionInputValue);
            TypeConverterTests.VerifyConversionToString(o);
        }

        [Fact]
        public static void ProtectedMethods()
        {
            new TypeConverterTests().RunProtectedMethods();
        }

        private void RunProtectedMethods()
        {
            Assert.Throws<NotSupportedException>(() => this.GetConvertFromException(null));
            Assert.Throws<NotSupportedException>(() => this.GetConvertFromException("1"));
            Assert.Throws<NotSupportedException>(() => this.GetConvertFromException(new BaseClass()));
            Assert.Throws<NotSupportedException>(() => this.GetConvertToException(null, typeof(int)));
            Assert.Throws<NotSupportedException>(() => this.GetConvertToException("1", typeof(int)));
            Assert.Throws<NotSupportedException>(() => this.GetConvertToException(new BaseClass(), typeof(BaseClass)));
        }

        private static void VerifyConversionToString(object o)
        {
            Assert.True(o is string);
            Assert.Equal(TypeConverterTests.c_conversionResult, (string)o);
        }
    }
}
