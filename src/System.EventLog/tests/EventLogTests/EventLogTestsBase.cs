using System.Diagnostics;
using Xunit;
using System;

namespace System.Diagnostics.Tests
{
    public class EventLogTestsBase
    {
        protected static readonly Lazy<bool> s_isElevated = new Lazy<bool>(() => AdminHelpers.IsProcessElevated());
        protected static bool IsProcessElevated => s_isElevated.Value;
    }
}
