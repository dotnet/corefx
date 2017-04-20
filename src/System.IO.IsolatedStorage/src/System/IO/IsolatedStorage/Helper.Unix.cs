﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.IsolatedStorage
{
    internal static partial class Helper
    {
        internal static void CreateDirectory(string path, IsolatedStorageScope scope)
        {
            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }
    }
}
