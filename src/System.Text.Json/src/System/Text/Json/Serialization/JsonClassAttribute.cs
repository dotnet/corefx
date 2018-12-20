// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonClassAttribute : Attribute
    {
        public Type ConverterType { get; set; }
    }
}
