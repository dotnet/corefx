﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.IO
{
    /// <summary>
    /// Represents a temporary file.  Creating an instance creates a file at the specified path,
    /// and disposing the instance deletes the file.
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        /// <summary>Gets the created file's path.</summary>
        public string Path { get; private set; }

        public TempFile(string path, long length = 0)
        {
            Path = path;
            File.WriteAllBytes(path, new byte[length]);
        }

        ~TempFile() { DeleteFile(); }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DeleteFile();
        }

        private void DeleteFile()
        {
            try { File.Delete(Path); }
            catch { /* Ignore exceptions on disposal paths */ }
        }
    }
}
