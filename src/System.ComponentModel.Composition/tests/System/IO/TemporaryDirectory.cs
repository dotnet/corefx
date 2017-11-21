// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
namespace System.IO 
{
    public class TemporaryDirectory : IDisposable
    {
        private string _directoryPath;

        public TemporaryDirectory()
        {
            _directoryPath = FileIO.GetNewTemporaryDirectory();
        }

        public string DirectoryPath
        {
            get { return _directoryPath; }
        }
        
        public void Dispose()
        {
            if (_directoryPath != null)
            {
                try
                {
                    Directory.Delete(_directoryPath, true);
                }
                catch (IOException)
                {
                }

                _directoryPath = null;
            }
        }
    }
}
