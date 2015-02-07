﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Diagnostics
{
    /// <devdoc>
    ///     This data structure contains information about a module in a process that
    ///     is collected in bulk by querying the operating system.  The reason to
    ///     make this a separate structure from the ProcessModule component is so that we
    ///     can throw it away all at once when Refresh is called on the component.
    /// </devdoc>
    /// <internalonly/>
    internal sealed class ModuleInfo
    {
        internal string _baseName;
        internal string _fileName;
        internal IntPtr _baseOfDll;
        internal IntPtr _entryPoint;
        internal int _sizeOfImage;
    }
}