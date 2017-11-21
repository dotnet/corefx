// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.IO 
{
    public class TemporaryFileCopier : TemporaryDirectory
    {
        public TemporaryFileCopier(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                File.Copy(fileName, Path.Combine(DirectoryPath, Path.GetFileName(fileName)));
            }
        }
    }
}
