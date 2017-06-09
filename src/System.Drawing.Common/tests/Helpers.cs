// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;

namespace System.Drawing.Tests
{
    public static class Helpers
    {
        public static string GetEmbeddedResourcePath(string name)
        {
            return Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "bitmaps", name);
        }
    }
}
