// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Delegate | AttributeTargets.Interface)]
    internal sealed class EditorBrowsableAttribute : Attribute
    {
        private EditorBrowsableState browsableState;

        public EditorBrowsableAttribute(EditorBrowsableState state)
        {
            browsableState = state;
        }
    }

    internal enum EditorBrowsableState
    {
        Always,
        Never,
        Advanced
    }
}