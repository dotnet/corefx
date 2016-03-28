// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class Module
    {
        public abstract Assembly Assembly { get; }
    }
}
#endif