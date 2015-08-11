// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Maps to the "dwFlags" parameter of NCryptGetProperty() and NCryptSetProperty().
    /// </summary>
    [Flags]
    public enum CngPropertyOptions : int
    {
        None = 0,
        CustomProperty = 0x40000000,              //NCRYPT_PERSIST_ONLY_FLAG (The property is not specified by CNG. Use this option to avoid future name conflicts with CNG properties.)        
        Persist = unchecked((int)0x80000000),     //NCRYPT_PERSIST_FLAG (The property should be persisted.)
    }
}

