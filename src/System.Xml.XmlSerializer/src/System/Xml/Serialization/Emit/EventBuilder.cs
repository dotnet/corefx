// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class EventBuilder
    {
        [Obsolete("TODO", error: false)]
        public void AddOtherMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetAddOnMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetRaiseMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetRemoveOnMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
