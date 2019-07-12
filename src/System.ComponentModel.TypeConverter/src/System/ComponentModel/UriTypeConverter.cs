﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Provides a type converter to convert Uri objects to and from
    /// various other representations.
    /// </summary>
    public class UriTypeConverter : TypeConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object in the
        /// given source type to a Uri.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Uri) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Gets a value indicating whether this converter can
        /// convert an object to the given destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Uri) || destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the a Uri.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string uriString)
            {
                if (string.IsNullOrEmpty(uriString))
                {
                    return null;
                }

                // Let the Uri constructor throw any informative exceptions
                return new Uri(uriString, UriKind.RelativeOrAbsolute);
            }

            if (value is Uri uri)
            {
                return new Uri(uri.OriginalString); // return new instance
            }

            throw GetConvertFromException(value);
        }

        /// <summary>
        /// Converts the given value object to
        /// the specified destination type using the specified context and arguments.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is Uri uri)
            {
                if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo ctor = typeof(Uri).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(UriKind) }, null);
                    Debug.Assert(ctor != null, "Couldn't find constructor");
                    return new InstanceDescriptor(ctor, new object[] { uri.OriginalString, uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative });
                }

                if (destinationType == typeof(string))
                {
                    return uri.OriginalString;
                }

                if (destinationType == typeof(Uri))
                {
                    return new Uri(uri.OriginalString, UriKind.RelativeOrAbsolute);
                }
            }

            throw GetConvertToException(value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value is string text)
            {
                return Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out Uri uri);
            }
            return value is Uri;
        }
    }
}
