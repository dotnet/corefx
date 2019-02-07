// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;

namespace System.UnitTesting
{
    internal class TemporaryFileCopier
    {
        public const string RootTemporaryDirectoryName = "RootTempDirectory";
        private static string _temporaryDirectory;
        public static string GetRootTemporaryDirectory()
        {
            if (_temporaryDirectory == null)
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), RootTemporaryDirectoryName);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                _temporaryDirectory = path;
            }

            return _temporaryDirectory;
        }

        public static string GetNewTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
