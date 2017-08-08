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
            "Combine",
            "Remove",
            "op_UnaryNegation",
            "op_Increment",
            "op_Decrement",
            "op_Equality",
            "op_Inequality",
            "Concat",
            "Add",
            "CreateDelegate",
            "Value",
            "get_Value",
            "Lambda",
            "Parameter",
            "Constant",
            "Convert",
            "ConvertChecked",
            "AddChecked",
            "Divide",
            "Modulo",
            "Multiply",
            "MultiplyChecked",
            "Subtract",
            "SubtractChecked",
            "And",
            "Or",
            "ExclusiveOr",
            "LeftShift",
            "RightShift",
            "AndAlso",
            "OrElse",
            "Equal",
            "NotEqual",
            "GreaterThanOrEqual",
            "GreaterThan",
            "LessThan",
            "LessThanOrEqual",
            "ArrayIndex",
            "Assign",
            "Field",
            "Call",
            "New",
            "Quote",
            "UnaryPlus",
            "Negate",
            "NegateChecked",
            "Not",
            "NewArrayInit",
            "Property",
            "AddEventHandler",
            "RemoveEventHandler",
            "InvocationList",
            "GetOrCreateEventRegistrationTokenTable",
            /* Above here corresponds with PredefinedName enum */
            "true",
            "false",
            "null",
            "base",
            "this",
            "explicit",
            "implicit",
            "__arglist",
            "__makeref",
            "__reftype",
            "__refvalue",
            "as",
            "checked",
            "is",
            "typeof",
            "unchecked",
            "void",
            ""
        };

        internal static string GetPredefinedName(PredefinedName id)
        {
            Debug.Assert(id < PredefinedName.PN_COUNT);
            return s_predefinedNames[(int)id];
        }
    }
}
