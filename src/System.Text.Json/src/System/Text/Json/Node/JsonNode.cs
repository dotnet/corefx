// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.IO;

namespace System.Text.Json
{
    /// <summary>
    ///   Defines the core behavior of JSON nodes and provides a base for derived classes.
    /// </summary>
    public abstract partial class JsonNode
    {
        private protected JsonNode() { }
    }
}
