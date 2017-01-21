// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Resources
{
    internal class MissingManifestResourceException : Exception { }

    internal class ResourceManager
    {
        public ResourceManager(Type resourceSource) {}
        public string GetString(string resourceKey) { return resourceKey; }
    }
}
