// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

using Xunit;

namespace System.Net.Http.WinHttpHandlerUnitTests
{
    public class SafeWinHttpHandleTest : IDisposable
    {
        public SafeWinHttpHandleTest()
        {
        }

        public void Dispose()
        {
            // This runs after every test and makes sure that we run any finalizers to free all eligible handles.
            FakeSafeWinHttpHandle.ForceGarbageCollection();
            Assert.Equal(0, FakeSafeWinHttpHandle.HandlesOpen);
        }

        [Fact]
        public void CreateAddRefDispose_HandleIsNotClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            bool success = false;
            safeHandle.DangerousAddRef(ref success);
            Assert.True(success, "DangerousAddRef");
            safeHandle.Dispose();
            
            Assert.False(safeHandle.IsClosed, "closed");
            Assert.Equal(1, FakeSafeWinHttpHandle.HandlesOpen);
            
            // Clean up safeHandle to keep outstanding handles at zero.
            safeHandle.DangerousRelease();
        }

        [Fact]
        public void CreateAddRefDisposeDispose_HandleIsNotClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            bool success = false;
            safeHandle.DangerousAddRef(ref success);
            Assert.True(success, "DangerousAddRef");
            safeHandle.Dispose();
            safeHandle.Dispose();
            
            Assert.False(safeHandle.IsClosed, "closed");
            Assert.Equal(1, FakeSafeWinHttpHandle.HandlesOpen);
            
            // Clean up safeHandle to keep outstanding handles at zero.
            safeHandle.DangerousRelease();
        }

        [Fact]
        public void CreateAddRefDisposeRelease_HandleIsClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            bool success = false;
            safeHandle.DangerousAddRef(ref success);
            Assert.True(success, "DangerousAddRef");
            safeHandle.Dispose();
            safeHandle.DangerousRelease();
            
            Assert.True(safeHandle.IsClosed, "closed");
            Assert.Equal(0, FakeSafeWinHttpHandle.HandlesOpen);
        }

        [Fact]
        public void CreateAddRefRelease_HandleIsNotClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            bool success = false;
            safeHandle.DangerousAddRef(ref success);
            Assert.True(success, "DangerousAddRef");
            safeHandle.DangerousRelease();
            
            Assert.False(safeHandle.IsClosed, "closed");
            Assert.Equal(1, FakeSafeWinHttpHandle.HandlesOpen);
            
            // Clean up safeHandle to keep outstanding handles at zero.
            safeHandle.Dispose();
        }

        [Fact]
        public void CreateAddRefReleaseDispose_HandleIsClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            bool success = false;
            safeHandle.DangerousAddRef(ref success);
            Assert.True(success, "DangerousAddRef");
            safeHandle.DangerousRelease();
            safeHandle.Dispose();
            
            Assert.True(safeHandle.IsClosed, "closed");
            Assert.Equal(0, FakeSafeWinHttpHandle.HandlesOpen);
        }

        [Fact]
        public void CreateDispose_HandleIsClosed()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            safeHandle.Dispose();
            
            Assert.True(safeHandle.IsClosed, "closed");
        }

        [Fact]
        public void CreateDisposeDispose_HandleIsClosedAndSecondDisposeIsNoop()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            safeHandle.Dispose();
            safeHandle.Dispose();
            Assert.True(safeHandle.IsClosed, "closed");
        }

        [Fact]
        public void CreateDisposeAddRef_ThrowsObjectDisposedException()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            safeHandle.Dispose();
            Assert.Throws<ObjectDisposedException>(() => 
                { bool ignore = false; safeHandle.DangerousAddRef(ref ignore); });
        }

        [Fact]
        public void CreateDisposeRelease_ThrowsObjectDisposedException()
        {
            var safeHandle = new FakeSafeWinHttpHandle(true);
            safeHandle.Dispose();
            Assert.Throws<ObjectDisposedException>(() => safeHandle.DangerousRelease());
        }

        [Fact]
        public void SetParentHandle_CreateParentCreateChildDisposeParent_ParentNotClosed()
        {
            var parentHandle = new FakeSafeWinHttpHandle(true);
            var childHandle = new FakeSafeWinHttpHandle(true);
            childHandle.SetParentHandle(parentHandle);
            parentHandle.Dispose();
            
            Assert.False(parentHandle.IsClosed, "closed");
            Assert.Equal(2, FakeSafeWinHttpHandle.HandlesOpen);
            
            // Clean up safeHandles to keep outstanding handles at zero.
            childHandle.Dispose();
        }

        [Fact]
        public void SetParentHandle_CreateParentCreateChildDisposeParentDisposeChild_HandlesClosed()
        {
            var parentHandle = new FakeSafeWinHttpHandle(true);
            var childHandle = new FakeSafeWinHttpHandle(true);
            childHandle.SetParentHandle(parentHandle);
            parentHandle.Dispose();
            childHandle.Dispose();
            
            Assert.True(parentHandle.IsClosed, "closed");
            Assert.True(childHandle.IsClosed, "closed");
        }

        [Fact]
        public void SetParentHandle_CreateParentCreateChildDisposeChildDisposeParent_HandlesClosed()
        {
            var parentHandle = new FakeSafeWinHttpHandle(true);
            var childHandle = new FakeSafeWinHttpHandle(true);
            childHandle.SetParentHandle(parentHandle);
            childHandle.Dispose();
            parentHandle.Dispose();
            
            Assert.True(parentHandle.IsClosed, "closed");
            Assert.True(childHandle.IsClosed, "closed");
        }
    }
}
