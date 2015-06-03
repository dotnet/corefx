// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libc
    {
        [DllImport(Libraries.Libc, SetLastError = true)]
        internal static extern unsafe int pipe(int* pipefd); // pipefd is an array of two ints

        /// <summary>
        /// The index into the array filled by <see cref="pipe"/> which represents the read end of the pipe.
        /// </summary>
        internal const int ReadEndOfPipe = 0;

        /// <summary>
        /// The index into the array filled by <see cref="pipe"/> which represents the read end of the pipe.
        /// </summary>
        internal const int WriteEndOfPipe = 1;
    }
}
