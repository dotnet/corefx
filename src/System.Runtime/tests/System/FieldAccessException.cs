// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

public static class FieldAccessExceptionTests
{
    private const int FieldAccessHResult = unchecked((int)0x80131507);

    [Fact]
    public static void TestCtor_Empty()
    {
        var exception = new FieldAccessException();
        Assert.NotEmpty(exception.Message);
        Assert.Equal(FieldAccessHResult, exception.HResult);
    }

    [Fact]
    public static void TestCtor_String()
    {
        string message = "Created FieldAccessException";
        var exception = new FieldAccessException(message);
        Assert.Equal(message, exception.Message);
        Assert.Equal(FieldAccessHResult, exception.HResult);
    }

    [Fact]
    public static void TestCtor_String_Exception()
    {
        string message = "Created FieldAccessException";
        var innerException = new Exception("Created inner exception");
        var exception = new FieldAccessException(message, innerException);
        Assert.Equal(message, exception.Message);
        Assert.Equal(FieldAccessHResult, exception.HResult);
        Assert.Same(innerException, exception.InnerException);
        Assert.Equal(innerException.HResult, exception.InnerException.HResult);
    }
}
