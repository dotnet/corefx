﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Threading
{
    //
    // Implementation of ThreadPoolBoundHandle that sits on top of the CLR's ThreadPool and Overlapped infrastructure
    //
    
    /// <summary>
    ///     Represents an I/O handle that is bound to the system thread pool and enables low-level
    ///     components to receive notifications for asynchronous I/O operations.
    /// </summary>
    public sealed partial class ThreadPoolBoundHandle : IDisposable
    {
        private readonly SafeHandle _handle;
        private bool _isDisposed;

        private ThreadPoolBoundHandle(SafeHandle handle)
        {
            _handle = handle;
        }

        /// <summary>
        ///     Gets the bound operating system handle.
        /// </summary>
        /// <value>
        ///     A <see cref="SafeHandle"/> object that holds the bound operating system handle.
        /// </value>
        public SafeHandle Handle
        {
            get { return _handle; }
        }

        /// <summary>
        ///     Returns a <see cref="ThreadPoolBoundHandle"/> for the specific handle, 
        ///     which is bound to the system thread pool.
        /// </summary>
        /// <param name="handle">
        ///     A <see cref="SafeHandle"/> object that holds the operating system handle. The 
        ///     handle must have been opened for overlapped I/O on the unmanaged side.
        /// </param>
        /// <returns>
        ///     <see cref="ThreadPoolBoundHandle"/> for <paramref name="handle"/>, which 
        ///     is bound to the system thread pool.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="handle"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="handle"/> has been disposed.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="handle"/> does not refer to a valid I/O handle.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="handle"/> refers to a handle that has not been opened 
        ///     for overlapped I/O.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="handle"/> refers to a handle that has already been bound.
        /// </exception>
        /// <remarks>
        ///     This method should be called once per handle.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <see cref="ThreadPoolBoundHandle"/> does not take ownership of <paramref name="handle"/>, 
        ///     it remains the responsibility of the caller to call <see cref="SafeHandle.Dispose"/>.
        /// </remarks>
        public static ThreadPoolBoundHandle BindHandle(SafeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            if (handle.IsClosed || handle.IsInvalid)
                throw new ArgumentException(SR.Argument_InvalidHandle, "handle");

            try
            {
                // ThreadPool.BindHandle will always return true, otherwise, it throws. See the underlying FCall
                // implementation in ThreadPoolNative::CorBindIoCompletionCallback to see the implementation.
                bool succeeded = ThreadPool.BindHandle(handle);
                Debug.Assert(succeeded);
            }
            catch (Exception ex)
            {   // BindHandle throws ApplicationException on full CLR and Exception on CoreCLR.
                // We do not let either of these leak and convert them to ArgumentException to 
                // indicate that the specified handles are invalid.

                if (ex.HResult == System.HResults.E_HANDLE)         // Bad handle
                    throw new ArgumentException(SR.Argument_InvalidHandle, "handle");

                if (ex.HResult == System.HResults.E_INVALIDARG)     // Handle already bound or sync handle
                    throw new ArgumentException(SR.Argument_AlreadyBoundOrSyncHandle, "handle");

                throw;
            }

            return new ThreadPoolBoundHandle(handle);
        }

        /// <summary>
        ///     Returns an unmanaged pointer to a <see cref="NativeOverlapped"/> structure, specifying 
        ///     a delegate that is invoked when the asynchronous I/O operation is complete, a user-provided 
        ///     object providing context, and managed objects that serve as buffers.
        /// </summary>
        /// <param name="callback">
        ///     An <see cref="IOCompletionCallback"/> delegate that represents the callback method 
        ///     invoked when the asynchronous I/O operation completes.
        /// </param>
        /// <param name="state">
        ///     A user-provided object that distinguishes this <see cref="NativeOverlapped"/> from other 
        ///     <see cref="NativeOverlapped"/> instances. Can be <see langword="null"/>.
        /// </param>
        /// <param name="pinData">
        ///     An object or array of objects representing the input or output buffer for the operation. Each 
        ///     object represents a buffer, for example an array of bytes.  Can be <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     An unmanaged pointer to a <see cref="NativeOverlapped"/> structure.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The unmanaged pointer returned by this method can be passed to the operating system in 
        ///         overlapped I/O operations. The <see cref="NativeOverlapped"/> structure is fixed in 
        ///         physical memory until <see cref="FreeNativeOverlapped(NativeOverlapped*)"/> is called.
        ///     </para>
        ///     <para>
        ///         The buffer or buffers specified in <paramref name="pinData"/> must be the same as those passed 
        ///         to the unmanaged operating system function that performs the asynchronous I/O.
        ///     </para>
        ///     <note>
        ///         The buffers specified in <paramref name="pinData"/> are pinned for the duration of 
        ///         the I/O operation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method was called after the <see cref="ThreadPoolBoundHandle"/> was disposed.
        /// </exception>
        [CLSCompliant(false)]
        public unsafe NativeOverlapped* AllocateNativeOverlapped(IOCompletionCallback callback, object state, object pinData)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            EnsureNotDisposed();

            ThreadPoolBoundHandleOverlapped overlapped = new ThreadPoolBoundHandleOverlapped(callback, state, pinData, preAllocated: null);
            overlapped._boundHandle = this;
            return overlapped._nativeOverlapped;
        }

        /// <summary>
        ///     Returns an unmanaged pointer to a <see cref="NativeOverlapped"/> structure, using the callback,
        ///     state, and buffers associated with the specified <see cref="PreAllocatedOverlapped"/> object.
        /// </summary>
        /// <param name="preAllocated">
        ///     A <see cref="PreAllocatedOverlapped"/> object from which to create the NativeOverlapped pointer.
        /// </param>
        /// <returns>
        ///     An unmanaged pointer to a <see cref="NativeOverlapped"/> structure.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The unmanaged pointer returned by this method can be passed to the operating system in 
        ///         overlapped I/O operations. The <see cref="NativeOverlapped"/> structure is fixed in 
        ///         physical memory until <see cref="FreeNativeOverlapped(NativeOverlapped*)"/> is called.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="preAllocated"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="preAllocated"/> is currently in use for another I/O operation.  
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method was called after the <see cref="ThreadPoolBoundHandle"/> was disposed, or 
        ///     this method was called after <paramref name="preAllocated"/> was disposed.
        /// </exception>
        /// <seealso cref="PreAllocatedOverlapped"/>
        [CLSCompliant(false)]
        public unsafe NativeOverlapped* AllocateNativeOverlapped(PreAllocatedOverlapped preAllocated)
        {
            if (preAllocated == null)
                throw new ArgumentNullException("preAllocated");

            EnsureNotDisposed();

            preAllocated.AddRef();
            try
            {
                ThreadPoolBoundHandleOverlapped overlapped = preAllocated._overlapped;

                if (overlapped._boundHandle != null)
                    throw new ArgumentException(SR.Argument_PreAllocatedAlreadyAllocated, "preAllocated");

                overlapped._boundHandle = this;

                return overlapped._nativeOverlapped;
            }
            catch
            {
                preAllocated.Release();
                throw;
            }
        }

        /// <summary>
        ///     Frees the unmanaged memory associated with a <see cref="NativeOverlapped"/> structure 
        ///     allocated by the <see cref="AllocateNativeOverlapped"/> method.
        /// </summary>
        /// <param name="overlapped">
        ///     An unmanaged pointer to the <see cref="NativeOverlapped"/> structure to be freed.
        /// </param>
        /// <remarks>
        ///     <note type="caution">
        ///         You must call the <see cref="FreeNativeOverlapped(NativeOverlapped*)"/> method exactly once 
        ///         on every <see cref="NativeOverlapped"/> unmanaged pointer allocated using the 
        ///         <see cref="AllocateNativeOverlapped"/> method. 
        ///         If you do not call the <see cref="FreeNativeOverlapped(NativeOverlapped*)"/> method, you will 
        ///         leak memory. If you call the <see cref="FreeNativeOverlapped(NativeOverlapped*)"/> method more 
        ///         than once on the same <see cref="NativeOverlapped"/> unmanaged pointer, memory will be corrupted.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="overlapped"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     This method was called after the <see cref="ThreadPoolBoundHandle"/> was disposed.
        /// </exception>
        [CLSCompliant(false)]
        public unsafe void FreeNativeOverlapped(NativeOverlapped* overlapped)
        {
            if (overlapped == null)
                throw new ArgumentNullException("overlapped");

            // Note: we explicitly allow FreeNativeOverlapped calls after the ThreadPoolBoundHandle has been Disposed.

            ThreadPoolBoundHandleOverlapped wrapper = GetOverlappedWrapper(overlapped, this);

            if (wrapper._boundHandle != this)
                throw new ArgumentException(SR.Argument_NativeOverlappedWrongBoundHandle, "overlapped");

            if (wrapper._preAllocated != null)
                wrapper._preAllocated.Release();
            else
                Overlapped.Free(overlapped);
        }

        /// <summary>
        ///     Returns the user-provided object specified when the <see cref="NativeOverlapped"/> instance was
        ///     allocated using the <see cref="AllocateNativeOverlapped(IOCompletionCallback, object, byte[])"/>.
        /// </summary>
        /// <param name="overlapped">
        ///     An unmanaged pointer to the <see cref="NativeOverlapped"/> structure from which to return the 
        ///     asscociated user-provided object.
        /// </param>
        /// <returns>
        ///     A user-provided object that distinguishes this <see cref="NativeOverlapped"/> 
        ///     from other <see cref="NativeOverlapped"/> instances, otherwise, <see langword="null"/> if one was 
        ///     not specified when the instance was allocated using <see cref="AllocateNativeOverlapped"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="overlapped"/> is <see langword="null"/>.
        /// </exception>
        [CLSCompliant(false)]
        public unsafe static object GetNativeOverlappedState(NativeOverlapped* overlapped)
        {
            if (overlapped == null)
                throw new ArgumentNullException("overlapped");

            ThreadPoolBoundHandleOverlapped wrapper = GetOverlappedWrapper(overlapped, null);
            Debug.Assert(wrapper._boundHandle != null);
            return wrapper._userState;
        }

        private static unsafe ThreadPoolBoundHandleOverlapped GetOverlappedWrapper(NativeOverlapped* overlapped, ThreadPoolBoundHandle expectedBoundHandle)
        {
            ThreadPoolBoundHandleOverlapped wrapper;
            try
            {
                wrapper = (ThreadPoolBoundHandleOverlapped)Overlapped.Unpack(overlapped);
            }
            catch (NullReferenceException ex)
            {
                throw new ArgumentException(SR.Argument_NativeOverlappedAlreadyFree, "overlapped", ex);
            }

            return wrapper;
        }

        public void Dispose()
        {   
            // .NET Native's version of ThreadPoolBoundHandle that wraps the Win32 ThreadPool holds onto
            // native resources so it needs to be disposable. To match the contract, we are also disposable.
            // We also implement a disposable state to mimic behavior between this implementation and 
            // .NET Native's version (code written against us, will also work against .NET Native's version).
            _isDisposed = true;
        }


        private void EnsureNotDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}
