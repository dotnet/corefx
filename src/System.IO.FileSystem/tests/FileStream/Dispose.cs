// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using Xunit;

namespace System.IO.Tests
{
    public class FileStream_Dispose : FileSystemTest
    {
        [Fact]
        public void CanDispose()
        {
            new FileStream(GetTestFilePath(), FileMode.Create).Dispose();
        }

        [Fact]
        public void DisposeClosesHandle()
        {
            SafeFileHandle handle;
            using(FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create))
            {
                handle = fs.SafeFileHandle;
            }

            Assert.True(handle.IsClosed);
        }

        [Fact]
        public void HandlesMultipleDispose()
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create))
            {
                fs.Dispose();
                fs.Dispose();
            }  // disposed as we leave using
        }

        private class MyFileStream : FileStream
        {
            public MyFileStream(string path, FileMode mode)
                : base(path, mode)
            { }

            public Action<bool> DisposeMethod { get; set; }

            protected override void Dispose(bool disposing)
            {
                Action<bool> disposeMethod = DisposeMethod;

                if (disposeMethod != null)
                    disposeMethod(disposing);

                base.Dispose(disposing);
            }
        }

        [ActiveIssue(6153, PlatformID.AnyUnix)]
        [Fact]
        public void DisposeVirtualBehavior()
        {
            bool called = false;

            // Normal dispose should call Dispose(true)
            using (MyFileStream fs = new MyFileStream(GetTestFilePath(), FileMode.Create))
            {
                fs.DisposeMethod = (disposing) =>
                {
                    called = true;
                    Assert.True(disposing);
                };

                fs.Dispose();
                Assert.True(called);

                called = false;
            }

            // Second dispose leaving the using should still call dispose
            Assert.True(called);

            called = false;
            // make sure we suppress finalization
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.False(called);

            // Dispose from finalizer should call Dispose(false)
            called = false;
            MyFileStream fs2 = new MyFileStream(GetTestFilePath(), FileMode.Create);
            fs2.DisposeMethod = (disposing) =>
            {
                called = true;
                Assert.False(disposing);
            };
            fs2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(called);
        }

        [Fact]
        public void DisposeFlushesWriteBuffer()
        {
            string fileName = GetTestFilePath();
            using(FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(TestBuffer, 0, TestBuffer.Length);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                byte[] buffer = new byte[TestBuffer.Length];
                Assert.Equal(buffer.Length, fs.Length);
                fs.Read(buffer, 0, buffer.Length);
                Assert.Equal(TestBuffer, buffer);
            }
        }

        [Fact]
        public void FinalizeFlushesWriteBuffer()
        {
            string fileName = GetTestFilePath();

            // use a seperate method to be sure that fs isn't rooted at time of GC.
            Action leakFs = () =>
            {
                // we must specify useAsync:false, otherwise the finalizer just kicks off an async write.
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, bufferSize: 4096, useAsync: false);
                fs.Write(TestBuffer, 0, TestBuffer.Length);
                fs = null;
            };
            leakFs();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (FileStream fsr = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                byte[] buffer = new byte[TestBuffer.Length];
                Assert.Equal(buffer.Length, fsr.Length);
                fsr.Read(buffer, 0, buffer.Length);
                Assert.Equal(TestBuffer, buffer);
            }
        }
    }
}
