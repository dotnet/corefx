// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal struct Label : IEquatable<Label>
    {
        public override bool Equals(object obj)
        {
            return false;
        }

        public bool Equals(Label obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(Label a, Label b)
        {
            return false;
        }

        public static bool operator !=(Label a, Label b)
        {
            return false;
        }
    }
}
#endif