// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

#if MS_IO_REDIST
namespace Microsoft.IO
#else
namespace System.IO
#endif
{
    internal static partial class FileSystem
    {
        public static void CreateDirectory(string fullPath, byte[] securityDescriptor = null)
        {
            // We can save a bunch of work if the directory we want to create already exists.  This also
            // saves us in the case where sub paths are inaccessible (due to ERROR_ACCESS_DENIED) but the
            // final path is accessible and the directory already exists.  For example, consider trying
            // to create c:\Foo\Bar\Baz, where everything already exists but ACLS prevent access to c:\Foo
            // and c:\Foo\Bar.  In that case, this code will think it needs to create c:\Foo, and c:\Foo\Bar
            // and fail to due so, causing an exception to be thrown.  This is not what we want.
            if (DirectoryExists(fullPath))
                return;

            List<string> stackDir = new List<string>();

            // Attempt to figure out which directories don't exist, and only
            // create the ones we need.  Note that FileExists may fail due
            // to Win32 ACL's preventing us from seeing a directory, and this
            // isn't threadsafe.

            bool somepathexists = false;

            int length = fullPath.Length;

            // We need to trim the trailing slash or the code will try to create 2 directories of the same name.
            if (length >= 2 && Path.EndsInDirectorySeparator(fullPath.AsSpan()))
                length--;

            int lengthRoot = PathInternal.GetRootLength(fullPath.AsSpan());

            if (length > lengthRoot)
            {
                // Special case root (fullpath = X:\\)
                int i = length - 1;
                while (i >= lengthRoot && !somepathexists)
                {
                    string dir = fullPath.Substring(0, i + 1);

                    if (!DirectoryExists(dir)) // Create only the ones missing
                        stackDir.Add(dir);
                    else
                        somepathexists = true;

                    while (i > lengthRoot && !PathInternal.IsDirectorySeparator(fullPath[i])) i--;
                    i--;
                }
            }

            int count = stackDir.Count;

            Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs;
            if (securityDescriptor == null)
            {
                secAttrs = default;
            }
            else
            {
                unsafe
                {
                    fixed (byte* pSecurityDescriptor = securityDescriptor)
                    {
                        secAttrs = new Interop.Kernel32.SECURITY_ATTRIBUTES
                        {
                            nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES),
                            lpSecurityDescriptor = (IntPtr)pSecurityDescriptor
                        };
                    }
                }
            }

            bool r = true;
            int firstError = 0;
            string errorString = fullPath;

            // If all the security checks succeeded create all the directories
            while (stackDir.Count > 0)
            {
                string name = stackDir[stackDir.Count - 1];
                stackDir.RemoveAt(stackDir.Count - 1);

                r = Interop.Kernel32.CreateDirectory(name, ref secAttrs);
                if (!r && (firstError == 0))
                {
                    int currentError = Marshal.GetLastWin32Error();
                    // While we tried to avoid creating directories that don't
                    // exist above, there are at least two cases that will
                    // cause us to see ERROR_ALREADY_EXISTS here.  FileExists
                    // can fail because we didn't have permission to the
                    // directory.  Secondly, another thread or process could
                    // create the directory between the time we check and the
                    // time we try using the directory.  Thirdly, it could
                    // fail because the target does exist, but is a file.
                    if (currentError != Interop.Errors.ERROR_ALREADY_EXISTS)
                        firstError = currentError;
                    else
                    {
                        // If there's a file in this directory's place, or if we have ERROR_ACCESS_DENIED when checking if the directory already exists throw.
                        if (FileExists(name) || (!DirectoryExists(name, out currentError) && currentError == Interop.Errors.ERROR_ACCESS_DENIED))
                        {
                            firstError = currentError;
                            errorString = name;
                        }
                    }
                }
            }

            // We need this check to mask OS differences
            // Handle CreateDirectory("X:\\") when X: doesn't exist. Similarly for n/w paths.
            if ((count == 0) && !somepathexists)
            {
                string root = GetDirectoryRoot(fullPath);

                if (!DirectoryExists(root))
                    throw Win32Marshal.GetExceptionForWin32Error(Interop.Errors.ERROR_PATH_NOT_FOUND, root);
                return;
            }

            // Only throw an exception if creating the exact directory we
            // wanted failed to work correctly.
            if (!r && (firstError != 0))
                throw Win32Marshal.GetExceptionForWin32Error(firstError, errorString);
        }

        public static bool DirectoryExists(string fullPath)
        {
            return DirectoryExists(fullPath, out int lastError);
        }

        private static bool DirectoryExists(string path, out int lastError)
        {
            Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA data = new Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA();
            lastError = FillAttributeInfo(path, ref data, returnErrorOnNotFound: true);

            return (lastError == 0) && (data.dwFileAttributes != -1)
                    && ((data.dwFileAttributes & Interop.Kernel32.FileAttributes.FILE_ATTRIBUTE_DIRECTORY) != 0);
        }

        public static bool FileExists(string fullPath)
        {
            Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA data = new Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FillAttributeInfo(fullPath, ref data, returnErrorOnNotFound: true);

            return (errorCode == 0) && (data.dwFileAttributes != -1)
                    && ((data.dwFileAttributes & Interop.Kernel32.FileAttributes.FILE_ATTRIBUTE_DIRECTORY) == 0);
        }

        /// <summary>
        /// Returns 0 on success, otherwise a Win32 error code.  Note that
        /// classes should use -1 as the uninitialized state for dataInitialized.
        /// </summary>
        /// <param name="path">The file path from which the file attribute information will be filled.</param>
        /// <param name="data">A struct that will contain the attribute information.</param>
        /// <param name="returnErrorOnNotFound">Return the error code for not found errors?</param>
        internal static int FillAttributeInfo(string path, ref Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA data, bool returnErrorOnNotFound)
        {
            int errorCode = Interop.Errors.ERROR_SUCCESS;

            // Neither GetFileAttributes or FindFirstFile like trailing separators
            path = Path.TrimEndingDirectorySeparator(path);

            using (DisableMediaInsertionPrompt.Create())
            {
                if (!Interop.Kernel32.GetFileAttributesEx(path, Interop.Kernel32.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref data))
                {
                    errorCode = Marshal.GetLastWin32Error();
                    if (errorCode != Interop.Errors.ERROR_FILE_NOT_FOUND
                        && errorCode != Interop.Errors.ERROR_PATH_NOT_FOUND
                        && errorCode != Interop.Errors.ERROR_NOT_READY
                        && errorCode != Interop.Errors.ERROR_INVALID_NAME
                        && errorCode != Interop.Errors.ERROR_BAD_PATHNAME
                        && errorCode != Interop.Errors.ERROR_BAD_NETPATH
                        && errorCode != Interop.Errors.ERROR_BAD_NET_NAME
                        && errorCode != Interop.Errors.ERROR_INVALID_PARAMETER
                        && errorCode != Interop.Errors.ERROR_NETWORK_UNREACHABLE
                        && errorCode != Interop.Errors.ERROR_NETWORK_ACCESS_DENIED
                        && errorCode != Interop.Errors.ERROR_INVALID_HANDLE         // eg from \\.\CON
                        && errorCode != Interop.Errors.ERROR_FILENAME_EXCED_RANGE   // Path is too long
                        )
                    {
                        // Assert so we can track down other cases (if any) to add to our test suite
                        Debug.Assert(errorCode == Interop.Errors.ERROR_ACCESS_DENIED || errorCode == Interop.Errors.ERROR_SHARING_VIOLATION,
                            $"Unexpected error code getting attributes {errorCode} from path {path}");

                        // Files that are marked for deletion will not let you GetFileAttributes,
                        // ERROR_ACCESS_DENIED is given back without filling out the data struct.
                        // FindFirstFile, however, will. Historically we always gave back attributes
                        // for marked-for-deletion files.
                        //
                        // Another case where enumeration works is with special system files such as
                        // pagefile.sys that give back ERROR_SHARING_VIOLATION on GetAttributes.
                        //
                        // Ideally we'd only try again for known cases due to the potential performance
                        // hit. The last attempt to do so baked for nearly a year before we found the
                        // pagefile.sys case. As such we're probably stuck filtering out specific
                        // cases that we know we don't want to retry on.

                        var findData = new Interop.Kernel32.WIN32_FIND_DATA();
                        using (SafeFindHandle handle = Interop.Kernel32.FindFirstFile(path, ref findData))
                        {
                            if (handle.IsInvalid)
                            {
                                errorCode = Marshal.GetLastWin32Error();
                            }
                            else
                            {
                                errorCode = Interop.Errors.ERROR_SUCCESS;
                                data.PopulateFrom(ref findData);
                            }
                        }
                    }
                }
            }

            if (errorCode != Interop.Errors.ERROR_SUCCESS && !returnErrorOnNotFound)
            {
                switch (errorCode)
                {
                    case Interop.Errors.ERROR_FILE_NOT_FOUND:
                    case Interop.Errors.ERROR_PATH_NOT_FOUND:
                    case Interop.Errors.ERROR_NOT_READY: // Removable media not ready
                        // Return default value for backward compatibility
                        data.dwFileAttributes = -1;
                        return Interop.Errors.ERROR_SUCCESS;
                }
            }

            return errorCode;
        }

    }
}
