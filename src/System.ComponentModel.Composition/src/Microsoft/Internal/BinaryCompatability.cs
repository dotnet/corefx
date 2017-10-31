//------------------------------------------------------------------------------
// <copyright file="BinaryCompatibility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;

namespace Microsoft.Internal
{
    // This class contains utility methods that mimic the mscorlib internal System.Runtime.Versioning.BinaryCompatibility type.
    internal sealed class BinaryCompatibility 
    {
        public static bool TargetsAtLeast_Desktop_V4_5 => true;
    }
}

