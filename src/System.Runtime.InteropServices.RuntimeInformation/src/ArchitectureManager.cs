// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    internal partial struct ArchitectureManager
    {
        private static Architecture _processArch = Architecture.Undefined;
        private static Architecture _osArch = Architecture.Undefined;

        private static bool _is32BitProcess = IntPtr.Size == 4;

        public static Architecture ProcessArchitecture
        {
            get
            {
                if (Architecture.Undefined == _processArch)
                {
                    _processArch = ProcessArchCore;
                }

                return _processArch;
            }
        }

        public static Architecture OSArchitecture
        {
            get
            {
                if (Architecture.Undefined == _osArch)
                {
                    _osArch = OSArchCore;
                }

                return _osArch;
            }
        }
    }
}
