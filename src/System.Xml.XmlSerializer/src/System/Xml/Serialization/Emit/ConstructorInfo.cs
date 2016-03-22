// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class ConstructorInfo : MethodBase
    {
        [Obsolete("TODO", error: false)]
        public object Invoke(object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
