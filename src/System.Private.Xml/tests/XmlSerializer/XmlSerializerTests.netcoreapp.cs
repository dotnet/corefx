// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Xml.Serialization;
using Xunit;

public static partial class XmlSerializerTests
{
    private static readonly string SerializationModeSetterName = "set_Mode";

    static XmlSerializerTests()
    {
        MethodInfo method = typeof(XmlSerializer).GetMethod(SerializationModeSetterName, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.True(method != null, $"No method named {SerializationModeSetterName}");
        method.Invoke(null, new object[] { 1 });
    }
}
