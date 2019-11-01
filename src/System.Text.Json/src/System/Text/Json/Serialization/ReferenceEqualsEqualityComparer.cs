using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal class ReferenceEqualsEqualityComparer : IEqualityComparer<object>
    {
        public static ReferenceEqualsEqualityComparer Comparer => new ReferenceEqualsEqualityComparer();

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return GetHashCode();
        }
    }
}
