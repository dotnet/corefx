// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Tests
{
    public abstract class BaseGetSetAttributes : FileSystemTest
    {
        protected abstract FileAttributes Get(string path);
        protected abstract void Set(string path, FileAttributes attributes);

        /// <summary>
        /// Create an appropriate filesystem object at the given path.
        /// </summary>
        protected virtual string CreateItem(string path = null)
        {
            path =  path ?? GetTestFilePath();
            File.Create(path).Dispose();
            return path;
        }

        protected virtual void DeleteItem(string path) => File.Delete(path);

        protected virtual bool IsDirectory => false;
    }
}
