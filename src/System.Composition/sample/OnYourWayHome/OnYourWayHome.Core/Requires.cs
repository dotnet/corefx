using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome
{
    public static class Requires
    {
        public static void NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void NotNullOrEmpty(string value, string parameterName)
        {
            Requires.NotNull(value, parameterName);

            if (value.Length == 0)
                throw new ArgumentException(null, parameterName);
        }
    }
}
