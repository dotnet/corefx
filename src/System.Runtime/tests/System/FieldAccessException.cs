// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Xunit;

public static class FieldAccessExceptionTests
{
    internal const int COR_E_METHODACCESS = unchecked((int)0x80131510);
    internal const int COR_E_FIELDACCESS = unchecked((int)0x80131507);

    [Fact]
    public static void FieldAccessException_Ctor1()
    {
        FieldAccessException i = new FieldAccessException();
        //Assert.Equal("Attempted to access a non-existing field.", i.Message, "TestCtor1_001 failed");
        Assert.Equal(COR_E_FIELDACCESS, i.HResult);
    }

    [Fact]
    public static void FieldAccessException_Ctor2()
    {
        FieldAccessException i = new FieldAccessException("Created FieldAccessException");

        Assert.Equal("Created FieldAccessException", i.Message);
        Assert.Equal(COR_E_FIELDACCESS, i.HResult);
    }

    [Fact]
    public static void FieldAccessException_Ctor3()
    {
        Exception ex = new Exception("Created inner exception");
        FieldAccessException i = new FieldAccessException("Created FieldAccessException", ex);

        Assert.Equal("Created FieldAccessException", i.Message);
        Assert.Equal(COR_E_FIELDACCESS, i.HResult);
        Assert.Equal(i.InnerException.Message, "Created inner exception");
        Assert.Equal(i.InnerException.HResult, ex.HResult);
    }
}
