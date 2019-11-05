// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Pipes
{
    public static class AnonymousPipeServerStreamAcl
    {
        /// <summary>
        /// Creates an anonymous pipe server stream instance.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="inheritability"></param>
        /// <param name="bufferSize"></param>
        /// <param name="pipeSecurity"></param>
        /// <returns></returns>
        public static AnonymousPipeServerStream Create(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
        {
            return new AnonymousPipeServerStream(direction, inheritability, bufferSize, pipeSecurity);
        }
    }
}
