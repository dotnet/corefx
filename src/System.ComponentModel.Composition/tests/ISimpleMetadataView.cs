// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition
{

    public interface ITrans_SimpleMetadataView
    {
        string String { get; }
        int Int { get; }
        float Float { get; }
        Type Type { get; }
        object Object { get; }
    }
}
