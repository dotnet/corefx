// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection.Context.Projection
{
    // TODO: Temporary interface until we can add ReflectionContext to Type/Assembly
    internal interface IProjectable
    {
        Projector Projector
        {
            get;
        }
    }
}
