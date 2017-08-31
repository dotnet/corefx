// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.InteropServices;

namespace System.Buffers
{
    /// <summary>
    /// A handle for the array.
    /// </summary>
    public unsafe struct MemoryHandle : IDisposable
    {
        private IRetainable _owner;
        private void* _pointer;
        private GCHandle _handle;

        /// <summary>
        /// Creates a new memory handle for the array.
        /// </summary>
        /// <param name="owner">The object owner which is responsible for releasing the object on dispose.</param>
        /// <param name="pinnedPointer">If the object is pinned, this is its address. If unspecified, it is set to null</param>
        /// <param name="handle">Used to release the handle for the array on dispose. If unspecified, it is set to default(GCHandle)</param>
        public MemoryHandle(IRetainable owner, void* pinnedPointer = null, GCHandle handle = default(GCHandle))
        {
            _owner = owner;
            _pointer = pinnedPointer;
            _handle = handle;
        }

        /// <summary>
        /// Returns the address of the pinned object.
        /// </summary>
        public void* PinnedPointer => _pointer;

        /// <summary>
        /// Clean up of unmanaged resources.
        /// </summary>
        public void Dispose()
        { 
            if (_handle.IsAllocated) 
            {
                _handle.Free();
            }

            if (_owner != null) 
            {
                _owner.Release();
                _owner = null;
            }

            _pointer = null;           
        }
        
    }
}
