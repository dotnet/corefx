// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.IO.MemoryMappedFiles
{
    public partial class MemoryMappedFile
    {
        /// <summary>
        /// Used by the 2 Create factory method groups.  A null fileHandle specifies that the 
        /// memory mapped file should not be associated with an exsiting file on disk (ie start
        /// out empty).
        /// </summary>

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        [SecurityCritical]
        private static SafeMemoryMappedFileHandle CreateCore(
            FileStream fileStream, string mapName, HandleInheritability inheritability,
            MemoryMappedFileAccess access, MemoryMappedFileOptions options, long capacity)
        {
            SafeFileHandle fileHandle = fileStream != null ? fileStream.SafeFileHandle : null;
            Interop.mincore.SECURITY_ATTRIBUTES secAttrs = GetSecAttrs(inheritability);

            // split the long into two ints
            int capacityLow = unchecked((int)(capacity & 0x00000000FFFFFFFFL));
            int capacityHigh = unchecked((int)(capacity >> 32));

            SafeMemoryMappedFileHandle handle = fileHandle != null ?
                Interop.mincore.CreateFileMapping(fileHandle, ref secAttrs, GetPageAccess(access) | (int)options, capacityHigh, capacityLow, mapName) :
                Interop.mincore.CreateFileMapping(INVALID_HANDLE_VALUE, ref secAttrs, GetPageAccess(access) | (int)options, capacityHigh, capacityLow, mapName);

            int errorCode = Marshal.GetLastWin32Error();
            if (!handle.IsInvalid)
            {
                if (errorCode == Interop.mincore.Errors.ERROR_ALREADY_EXISTS)
                {
                    handle.Dispose();
                    throw Win32Marshal.GetExceptionForWin32Error(errorCode);
                }
            }
            else // handle.IsInvalid
            {
                handle.Dispose();
                throw Win32Marshal.GetExceptionForWin32Error(errorCode);
            }

            return handle;
        }

        /// <summary>
        /// Used by the OpenExisting factory method group and by CreateOrOpen if access is write.
        /// We'll throw an ArgumentException if the file mapping object didn't exist and the
        /// caller used CreateOrOpen since Create isn't valid with Write access
        /// </summary>
        private static SafeMemoryMappedFileHandle OpenCore(
            string mapName, HandleInheritability inheritability, MemoryMappedFileAccess access, bool createOrOpen)
        {
            return OpenCore(mapName, inheritability, GetFileMapAccess(access), createOrOpen);
        }

        /// <summary>
        /// Used by the OpenExisting factory method group and by CreateOrOpen if access is write.
        /// We'll throw an ArgumentException if the file mapping object didn't exist and the
        /// caller used CreateOrOpen since Create isn't valid with Write access
        /// </summary>
        private static SafeMemoryMappedFileHandle OpenCore(
            string mapName, HandleInheritability inheritability, MemoryMappedFileRights rights, bool createOrOpen)
        {
            return OpenCore(mapName, inheritability, GetFileMapAccess(rights), createOrOpen);
        }

        /// <summary>
        /// Used by the CreateOrOpen factory method groups.
        /// </summary>
        [SecurityCritical]
        private static SafeMemoryMappedFileHandle CreateOrOpenCore(
            string mapName, HandleInheritability inheritability, MemoryMappedFileAccess access, 
            MemoryMappedFileOptions options, long capacity)
        {
            /// Try to open the file if it exists -- this requires a bit more work. Loop until we can
            /// either create or open a memory mapped file up to a timeout. CreateFileMapping may fail
            /// if the file exists and we have non-null security attributes, in which case we need to
            /// use OpenFileMapping.  But, there exists a race condition because the memory mapped file
            /// may have closed inbetween the two calls -- hence the loop. 
            /// 
            /// The retry/timeout logic increases the wait time each pass through the loop and times 
            /// out in approximately 1.4 minutes. If after retrying, a MMF handle still hasn't been opened, 
            /// throw an InvalidOperationException.

            Debug.Assert(access != MemoryMappedFileAccess.Write, "Callers requesting write access shouldn't try to create a mmf");

            SafeMemoryMappedFileHandle handle = null;
            Interop.mincore.SECURITY_ATTRIBUTES secAttrs = GetSecAttrs(inheritability);

            // split the long into two ints
            int capacityLow = unchecked((int)(capacity & 0x00000000FFFFFFFFL));
            int capacityHigh = unchecked((int)(capacity >> 32));

            int waitRetries = 14;   //((2^13)-1)*10ms == approximately 1.4mins
            int waitSleep = 0;

            // keep looping until we've exhausted retries or break as soon we we get valid handle
            while (waitRetries > 0)
            {
                // try to create
                handle = Interop.mincore.CreateFileMapping(INVALID_HANDLE_VALUE, ref secAttrs,
                    GetPageAccess(access) | (int)options, capacityHigh, capacityLow, mapName);

                if (!handle.IsInvalid)
                {
                    break;
                }
                else
                {
                    handle.Dispose();
                    int createErrorCode = Marshal.GetLastWin32Error();
                    if (createErrorCode != Interop.mincore.Errors.ERROR_ACCESS_DENIED)
                    {
                        throw Win32Marshal.GetExceptionForWin32Error(createErrorCode);
                    }
                }

                // try to open
                handle = Interop.mincore.OpenFileMapping(GetFileMapAccess(access), (inheritability &
                    HandleInheritability.Inheritable) != 0, mapName);

                // valid handle
                if (!handle.IsInvalid)
                {
                    break;
                }
                // didn't get valid handle; have to retry
                else
                {
                    handle.Dispose();
                    int openErrorCode = Marshal.GetLastWin32Error();
                    if (openErrorCode != Interop.mincore.Errors.ERROR_FILE_NOT_FOUND)
                    {
                        throw Win32Marshal.GetExceptionForWin32Error(openErrorCode);
                    }

                    // increase wait time
                    --waitRetries;
                    if (waitSleep == 0)
                    {
                        waitSleep = 10;
                    }
                    else
                    {
                        ThreadSleep(waitSleep);
                        waitSleep *= 2;
                    }
                }
            }

            // finished retrying but couldn't create or open
            if (handle == null || handle.IsInvalid)
            {
                throw new InvalidOperationException(SR.InvalidOperation_CantCreateFileMapping);
            }

            return handle;
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        /// <summary>
        /// This converts a MemoryMappedFileRights to its corresponding native FILE_MAP_XXX value to be used when 
        /// creating new views.  
        /// </summary>
        private static int GetFileMapAccess(MemoryMappedFileRights rights)
        {
            return (int)rights;
        }

        /// <summary>
        /// This converts a MemoryMappedFileAccess to its corresponding native FILE_MAP_XXX value to be used when 
        /// creating new views.  
        /// </summary>
        internal static int GetFileMapAccess(MemoryMappedFileAccess access)
        {
            switch (access)
            {
                case MemoryMappedFileAccess.Read: return Interop.mincore.FileMapOptions.FILE_MAP_READ;
                case MemoryMappedFileAccess.Write: return Interop.mincore.FileMapOptions.FILE_MAP_WRITE;
                case MemoryMappedFileAccess.ReadWrite: return Interop.mincore.FileMapOptions.FILE_MAP_READ | Interop.mincore.FileMapOptions.FILE_MAP_WRITE;
                case MemoryMappedFileAccess.CopyOnWrite: return Interop.mincore.FileMapOptions.FILE_MAP_COPY;
                case MemoryMappedFileAccess.ReadExecute: return Interop.mincore.FileMapOptions.FILE_MAP_EXECUTE | Interop.mincore.FileMapOptions.FILE_MAP_READ;
                default:
                    Debug.Assert(access == MemoryMappedFileAccess.ReadWriteExecute);
                    return Interop.mincore.FileMapOptions.FILE_MAP_EXECUTE | Interop.mincore.FileMapOptions.FILE_MAP_READ | Interop.mincore.FileMapOptions.FILE_MAP_WRITE;
            }
        }

        /// <summary>
        /// This converts a MemoryMappedFileAccess to it's corresponding native PAGE_XXX value to be used by the 
        /// factory methods that construct a new memory mapped file object. MemoryMappedFileAccess.Write is not 
        /// valid here since there is no corresponding PAGE_XXX value.
        /// </summary>
        internal static int GetPageAccess(MemoryMappedFileAccess access)
        {
            switch (access)
            {
                case MemoryMappedFileAccess.Read: return Interop.mincore.PageOptions.PAGE_READONLY;
                case MemoryMappedFileAccess.ReadWrite: return Interop.mincore.PageOptions.PAGE_READWRITE;
                case MemoryMappedFileAccess.CopyOnWrite: return Interop.mincore.PageOptions.PAGE_WRITECOPY;
                case MemoryMappedFileAccess.ReadExecute: return Interop.mincore.PageOptions.PAGE_EXECUTE_READ;
                default:
                    Debug.Assert(access == MemoryMappedFileAccess.ReadWriteExecute);
                    return Interop.mincore.PageOptions.PAGE_EXECUTE_READWRITE;
            }
        }

        /// <summary>
        /// Used by the OpenExisting factory method group and by CreateOrOpen if access is write.
        /// We'll throw an ArgumentException if the file mapping object didn't exist and the
        /// caller used CreateOrOpen since Create isn't valid with Write access
        /// </summary>
        [SecurityCritical]
        private static SafeMemoryMappedFileHandle OpenCore(
            string mapName, HandleInheritability inheritability, int desiredAccessRights, bool createOrOpen)
        {
            SafeMemoryMappedFileHandle handle = Interop.mincore.OpenFileMapping(
                desiredAccessRights, (inheritability & HandleInheritability.Inheritable) != 0, mapName);
            int lastError = Marshal.GetLastWin32Error();

            if (handle.IsInvalid)
            {
                handle.Dispose();
                if (createOrOpen && (lastError == Interop.mincore.Errors.ERROR_FILE_NOT_FOUND))
                {
                    throw new ArgumentException(SR.Argument_NewMMFWriteAccessNotAllowed, "access");
                }
                else
                {
                    throw Win32Marshal.GetExceptionForWin32Error(lastError);
                }
            }
            return handle;
        }

        /// <summary>
        /// Helper method used to extract the native binary security descriptor from the MemoryMappedFileSecurity
        /// type. If pinningHandle is not null, caller must free it AFTER the call to CreateFile has returned.
        /// </summary>
        [SecurityCritical]
        private unsafe static Interop.mincore.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability)
        {
            Interop.mincore.SECURITY_ATTRIBUTES secAttrs = default(Interop.mincore.SECURITY_ATTRIBUTES);
            if ((inheritability & HandleInheritability.Inheritable) != 0)
            {
                secAttrs = new Interop.mincore.SECURITY_ATTRIBUTES();
                secAttrs.nLength = (uint)sizeof(Interop.mincore.SECURITY_ATTRIBUTES);
                secAttrs.bInheritHandle = Interop.BOOL.TRUE;
            }
            return secAttrs;
        }

        /// <summary>
        /// Replacement for Thread.Sleep(milliseconds), which isn't available.
        /// </summary>
        internal static void ThreadSleep(int milliseconds)
        {
            new ManualResetEventSlim(initialState: false).Wait(milliseconds);
        }
    }
}
