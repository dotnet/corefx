// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal abstract class ReflectionItem
    {
        public abstract string Name { get; }
        public abstract string GetDisplayName();
        public abstract Type ReturnType { get; }
        public abstract ReflectionItemType ItemType { get; }
    }
}
