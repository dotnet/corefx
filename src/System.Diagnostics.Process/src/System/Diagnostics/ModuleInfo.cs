// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
#pragma warning disable 0649 // unused fields on some platforms
        internal string _baseName;
        internal string _fileName;
        internal IntPtr _baseOfDll;
        internal IntPtr _entryPoint;
        internal int _sizeOfImage;
#pragma warning restore 0649
    }
}
