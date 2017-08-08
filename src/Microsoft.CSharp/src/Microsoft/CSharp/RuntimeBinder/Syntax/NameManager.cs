// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Microsoft.CSharp.RuntimeBinder.Syntax
{
    internal static class NameManager
    {
        private static readonly string[] s_predefinedNames = {
            ".ctor",
            "*",
            "?*",
            "#",
            "&",
            "[X\001",
            "[X\002",
            "[X\003",
            "Invoke",
            "$Item$",
            "Value",
            "get_Value",
            "AddEventHandler",
            "RemoveEventHandler",
            "InvocationList",
            "GetOrCreateEventRegistrationTokenTable",
        };

        internal static string GetPredefinedName(PredefinedName id)
        {
            Debug.Assert(id < PredefinedName.PN_COUNT);
            return s_predefinedNames[(int)id];
        }
    }
}
