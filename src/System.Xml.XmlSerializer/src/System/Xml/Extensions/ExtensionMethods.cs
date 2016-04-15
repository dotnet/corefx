// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using System.Linq;
using Emit = System.Xml.Serialization.Emit;

namespace System.Xml.Extensions
{
    internal static class ExtensionMethods
    {
        internal static void CopyTo(this Dictionary<object, object>.ValueCollection source, Array a, int index)
        {
            int arrayIndex = index;
            foreach (object value in source)
            {
                a.SetValue(value, arrayIndex++);
            }
        }

        internal static bool Contains(this Dictionary<object, object> source, object a)
        {
            return source.ContainsKey(a);
        }

        internal static string ReadElementString(this XmlReader source)
        {
            return source.ReadElementContentAsString();
        }

        internal static string ReadString(this XmlReader source)
        {
            // Note: maintain behavior from \ndp\fx\src\Xml\System\Xml\Core\XmlReader.cs
            source.MoveToElement();
            if (source.NodeType == XmlNodeType.Element)
            {
                if (source.IsEmptyElement)
                {
                    return string.Empty;
                }
                else if (!source.Read())
                {
                    throw new InvalidOperationException(SR.Xml_InvalidOperation);
                }
                if (source.NodeType == XmlNodeType.EndElement)
                {
                    return string.Empty;
                }
            }
            return source.ReadContentAsString();
        }

        public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            var constructorInfos = type.GetConstructors(bindingFlags);
            var constructorInfo = ReflectionUtilities.FilterMethodBases(constructorInfos.Cast<MethodBase>().ToArray(), parameterTypes, ".ctor");
            return constructorInfo != null ? (ConstructorInfo)constructorInfo : null;
        }

        public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            var methodInfos = type.GetMethods(bindingFlags);
            var methodInfo = ReflectionUtilities.FilterMethodBases(methodInfos.Cast<MethodBase>().ToArray(), parameterTypes, methodName);
            return methodInfo != null ? (MethodInfo)methodInfo : null;
        }

        internal static string ToBinHexString(byte[] inArray)
        {
            if (inArray == null)
            {
                throw new ArgumentNullException(nameof(inArray));
            }
            return BinHexEncoder.Encode(inArray, 0, inArray.Length);
        }

        internal static byte[] FromBinHexString(string s, bool allowOddCount)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            return BinHexDecoder.Decode(s.ToCharArray(), allowOddCount);
        }

        internal static Uri ToUri(string s)
        {
            if (s != null && s.Length > 0)
            { //string.Empty is a valid uri but not "   "
                s = s.Trim(new char[] { ' ', '\t', '\n', '\r' });
                if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
                {
                    throw new FormatException(SR.Format(SR.XmlConvert_BadFormat, s, "Uri"));
                }
            }
            Uri uri;
            if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri))
            {
                throw new FormatException(SR.Format(SR.XmlConvert_BadFormat, s, "Uri"));
            }
            return uri;
        }
    }
}
