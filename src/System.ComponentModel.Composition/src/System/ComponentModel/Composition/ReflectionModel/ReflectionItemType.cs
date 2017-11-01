// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal enum ReflectionItemType : int
    {
        Parameter = 0,
        Field = 1,
        Property = 2,
        Method = 3,
        Type = 4,
    }
}
