using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OnYourWayHome
{
    public static class Assumes
    {
        public static void True(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void NotNull<T>(T value)
            where T : class
        {
            Debug.Assert(value != null);
        }
    }
}
