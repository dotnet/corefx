﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class ConverterTestBase
    {
        public static void CanConvertTo_WithContext(object[,] data, TypeConverter converter)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                Type destinationType = (Type)data[i, 0];
                bool result = (bool)data[i, 1];
                Assert.Equal(result, converter.CanConvertTo(TypeConverterTests.s_context, destinationType));
            }
        }

        public static void ConvertTo_WithContext(object[,] data, TypeConverter converter)
        {
            Assert.Throws<ArgumentNullException>(
                () => converter.ConvertTo(TypeConverterTests.s_context, null, "", null));
            // This type converter should had thrown ArgumentNullException in ConvertTo, because the destination type is null.");

            for (int i = 0; i < data.GetLength(0); i++)
            {
                object source = data[i, 0];
                object expected = data[i, 1];
                CultureInfo culture = data[i, 2] as CultureInfo;
                Assert.Equal(expected, converter.ConvertTo(TypeConverterTests.s_context, culture, source, expected.GetType()));
            }
        }

        public static void CanConvertFrom_WithContext(object[,] data, TypeConverter converter)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                Type sourceType = data[i, 0] as Type;
                bool expected = (bool)data[i, 1];
                Assert.Equal(expected, converter.CanConvertFrom(TypeConverterTests.s_context, sourceType));
            }
        }

        public static void ConvertFrom_WithContext(object[,] data, TypeConverter converter)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                object source = data[i, 0];
                object expected = data[i, 1];
                CultureInfo culture = data[i, 2] as CultureInfo;
                Assert.Equal(expected, converter.ConvertFrom(TypeConverterTests.s_context, culture, source));
            }
        }
    }
}
