// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.IO 
{
    public class TemporaryFile : IDisposable
    {
        private string _fileName;

        public TemporaryFile()
        {
            _fileName = Path.GetTempFileName();
        }

        public string FileName
        {
            get { return _fileName; }
        }
        
        public void Dispose()
        {
            if (_fileName != null)
            {
                File.Delete(_fileName);
                _fileName = null;
            }
        }
    }
}
