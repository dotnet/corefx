// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Specifies a property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonNameAttribute : JsonAttribute
    {
        public JsonNameAttribute()
        {
        }

        public JsonNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
