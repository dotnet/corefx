// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Xunit;

public class TupleItemNamesAttributeTests
{
    [Fact]
    public static void Constructor()
    {
        var attribute = new TupleItemNamesAttribute(itemNames: new string[] { "name1", "name2" });
        Assert.NotNull(attribute.ItemNames);
        Assert.Equal(new string[] { "name1", "name2" }, attribute.ItemNames);

        Assert.Throws<ArgumentNullException>(() => new TupleItemNamesAttribute(null));
    }

    [TupleItemNames(new string[] { null, "name1", "name2" })]
    public object appliedToField = null;

    public static void AppliedToParameter([TupleItemNames(new string[] { "name1", null })] object parameter) { }

    [TupleItemNames(new string[] { null, "name1", "name2" })]
    public static object AppliedToProperty { get; set; }

    [event: TupleItemNames(new[] { null, "name1", "name2" })]
    public static event Func<int> AppliedToEvent;

    [return: TupleItemNames(new[] { null, "name1", "name2" })]
    public static void AppliedToReturn()
    {
        AppliedToEvent();
    }
    
    [TupleItemNames(new string[] { null, "name1", "name2" })]
    public class AppliedToClass { }

    [TupleItemNames(new string[] { null, "name1", "name2" })]
    public class AppliedToStruct { }
}
