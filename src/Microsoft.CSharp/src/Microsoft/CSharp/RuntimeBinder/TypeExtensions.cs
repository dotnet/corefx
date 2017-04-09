// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder
{
    internal static class TypeExtensions
    {
        public static bool IsSZArray(this Type type)
        {
            // Add return type.IsSZArray if appropriate configurations are added.
            // Replace entirely with type.IsSZArray if ever all supported configurations
            // support it.
            Debug.Assert(type.IsArray);
            // Avoid creating array type to test if rank is not 1
            return type.GetArrayRank() == 1 && type.GetElementType().MakeArrayType() == type;
        }
    }
}
