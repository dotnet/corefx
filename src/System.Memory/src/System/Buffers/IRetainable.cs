// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    /// <summary>
    /// Provides a mechanism to make sure objects only get disposed when intended.
    /// </summary>
    public interface IRetainable
    {
        /// <summary>
        /// Do not dispose the object until the object owner calls Release.
        /// </summary>
        void Retain();

        /// <summary>
        /// The object can now be disposed. If the object owner called Retain, they must also call Release.
        /// </summary>
        bool Release();
    }
}
