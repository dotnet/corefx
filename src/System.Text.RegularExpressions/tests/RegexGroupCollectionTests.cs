using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

public class RegexGroupCollectionTests
{
    [Fact]
    public static void GetGroupTest()
    {
        var caps = new Dictionary<int, int>
        {
            { 0, 0 }
        };
        var groups = new GroupCollection(null, caps);
        var group = groups.GetGroup(1);
        Assert.NotEqual(Group._emptygroup, group);
    }
}
