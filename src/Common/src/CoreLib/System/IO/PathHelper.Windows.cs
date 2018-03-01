// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// Wrapper to help with path normalization.
    /// </summary>
    internal class PathHelper
    {
        /// <summary>
        /// Normalize the given path.
        /// </summary>
        /// <remarks>
        /// Normalizes via Win32 GetFullPathName().
        /// </remarks>
        /// <param name="path">Path to normalize</param>
        /// <exception cref="PathTooLongException">Thrown if we have a string that is too large to fit into a UNICODE_STRING.</exception>
        /// <exception cref="IOException">Thrown if the path is empty.</exception>
        /// <returns>Normalized path</returns>
        internal static string Normalize(string path)
        {
            Span<char> initialBuffer = stackalloc char[PathInternal.MaxShortPath];
            ValueStringBuilder builder = new ValueStringBuilder(initialBuffer);

            // Get the full path
            GetFullPathName(path, ref builder);

            // If we have the exact same string we were passed in, don't allocate another string.
            // TryExpandShortName does this input identity check.
            string result = builder.AsSpan().Contains('~')
                ? TryExpandShortFileName(ref builder, originalPath: path)
                : builder.AsSpan().Equals(path.AsSpan()) ? path : builder.ToString();

            // Clear the buffer
            builder.Dispose();
            return result;
        }

        private static void GetFullPathName(string path, ref ValueStringBuilder builder)
        {
            // If the string starts with an extended prefix we would need to remove it from the path before we call GetFullPathName as
            // it doesn't root extended paths correctly. We don't currently resolve extended paths, so we'll just assert here.
            Debug.Assert(PathInternal.IsPartiallyQualified(path) || !PathInternal.IsExtended(path));

            uint result = 0;
            while ((result = Interop.Kernel32.GetFullPathNameW(path, (uint)builder.Capacity, ref builder.GetPinnableReference(), IntPtr.Zero)) > builder.Capacity)
            {
                // Reported size is greater than the buffer size. Increase the capacity.
                builder.EnsureCapacity(checked((int)result));
            }

            if (result == 0)
            {
                // Failure, get the error and throw
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 0)
                    errorCode = Interop.Errors.ERROR_BAD_PATHNAME;
                throw Win32Marshal.GetExceptionForWin32Error(errorCode, path);
            }

            builder.Length = (int)result;
        }

        private static int PrependDevicePathChars(ref ValueStringBuilder content, bool isDosUnc, ref ValueStringBuilder buffer)
        {
            int length = content.Length;

            length += isDosUnc
                ? PathInternal.UncExtendedPrefixLength - PathInternal.UncPrefixLength
                : PathInternal.DevicePrefixLength;

            buffer.EnsureCapacity(length + 1);
            buffer.Length = 0;

            if (isDosUnc)
            {
                // Is a \\Server\Share, put \\?\UNC\ in the front
                buffer.Append(PathInternal.UncExtendedPathPrefix);

                // Copy Server\Share\... over to the buffer
                buffer.Append(content.AsSpan().Slice(PathInternal.UncPrefixLength));

                // Return the prefix difference
                return PathInternal.UncExtendedPrefixLength - PathInternal.UncPrefixLength;
            }
            else
            {
                // Not an UNC, put the \\?\ prefix in front, then the original string
                buffer.Append(PathInternal.ExtendedPathPrefix);
                buffer.Append(content.AsSpan());
                return PathInternal.DevicePrefixLength;
            }
        }

        private static string TryExpandShortFileName(ref ValueStringBuilder outputBuilder, string originalPath)
        {
            // We guarantee we'll expand short names for paths that only partially exist. As such, we need to find the part of the path that actually does exist. To
            // avoid allocating like crazy we'll create only one input array and modify the contents with embedded nulls.

            Debug.Assert(!PathInternal.IsPartiallyQualified(outputBuilder.AsSpan()), "should have resolved by now");

            // We'll have one of a few cases by now (the normalized path will have already:
            //
            //  1. Dos path (C:\)
            //  2. Dos UNC (\\Server\Share)
            //  3. Dos device path (\\.\C:\, \\?\C:\)
            //
            // We want to put the extended syntax on the front if it doesn't already have it (for long path support and speed), which may mean switching from \\.\.
            //
            // Note that we will never get \??\ here as GetFullPathName() does not recognize \??\ and will return it as C:\??\ (or whatever the current drive is).

            int rootLength = PathInternal.GetRootLength(outputBuilder.AsSpan());
            bool isDevice = PathInternal.IsDevice(outputBuilder.AsSpan());

            // As this is a corner case we're not going to add a stackalloc here to keep the stack pressure down.
            ValueStringBuilder inputBuilder = new ValueStringBuilder();

            bool isDosUnc = false;
            int rootDifference = 0;
            bool wasDotDevice = false;

            // Add the extended prefix before expanding to allow growth over MAX_PATH
            if (isDevice)
            {
                // We have one of the following (\\?\ or \\.\)
                inputBuilder.Append(outputBuilder.AsSpan());

                if (outputBuilder[2] == '.')
                {
                    wasDotDevice = true;
                    inputBuilder[2] = '?';
                }
            }
            else
            {
                isDosUnc = !PathInternal.IsDevice(outputBuilder.AsSpan()) && outputBuilder.Length > 1 && outputBuilder[0] == '\\' && outputBuilder[1] == '\\';
                rootDifference = PrependDevicePathChars(ref outputBuilder, isDosUnc, ref inputBuilder);
            }

            rootLength += rootDifference;
            int inputLength = inputBuilder.Length;

            bool success = false;
            int foundIndex = inputBuilder.Length - 1;

            // Need to null terminate the input builder
            inputBuilder.Append('\0');

            while (!success)
            {
                uint result = Interop.Kernel32.GetLongPathNameW(ref inputBuilder.GetPinnableReference(), ref outputBuilder.GetPinnableReference(), (uint)outputBuilder.Capacity);

                // Replace any temporary null we added
                if (inputBuilder[foundIndex] == '\0') inputBuilder[foundIndex] = '\\';

                if (result == 0)
                {
                    // Look to see if we couldn't find the file
                    int error = Marshal.GetLastWin32Error();
                    if (error != Interop.Errors.ERROR_FILE_NOT_FOUND && error != Interop.Errors.ERROR_PATH_NOT_FOUND)
                    {
                        // Some other failure, give up
                        break;
                    }

                    // We couldn't find the path at the given index, start looking further back in the string.
                    foundIndex--;

                    for (; foundIndex > rootLength && inputBuilder[foundIndex] != '\\'; foundIndex--) ;
                    if (foundIndex == rootLength)
                    {
                        // Can't trim the path back any further
                        break;
                    }
                    else
                    {
                        // Temporarily set a null in the string to get Windows to look further up the path
                        inputBuilder[foundIndex] = '\0';
                    }
                }
                else if (result > outputBuilder.Capacity)
                {
                    // Not enough space. The result count for this API does not include the null terminator.
                    outputBuilder.EnsureCapacity(checked((int)result));
                    result = Interop.Kernel32.GetLongPathNameW(ref inputBuilder.GetPinnableReference(), ref outputBuilder.GetPinnableReference(), (uint)outputBuilder.Capacity);
                }
                else
                {
                    // Found the path
                    success = true;
                    outputBuilder.Length = checked((int)result);
                    if (foundIndex < inputLength - 1)
                    {
                        // It was a partial find, put the non-existent part of the path back
                        outputBuilder.Append(inputBuilder.AsSpan().Slice(foundIndex, inputBuilder.Length - foundIndex));
                    }
                }
            }

            // Need to trim out the trailing separator in the input builder
            inputBuilder.Length = inputBuilder.Length - 1;

            // If we were able to expand the path, use it, otherwise use the original full path result
            ref ValueStringBuilder builderToUse = ref (success ? ref outputBuilder : ref inputBuilder);

            // Switch back from \\?\ to \\.\ if necessary
            if (wasDotDevice)
                builderToUse[2] = '.';

            // Change from \\?\UNC\ to \\?\UN\\ if needed
            if (isDosUnc)
                builderToUse[PathInternal.UncExtendedPrefixLength - PathInternal.UncPrefixLength] = '\\';

            // Strip out any added characters at the front of the string
            ReadOnlySpan<char> output = builderToUse.AsSpan().Slice(rootDifference);

            string returnValue = output.Equals(originalPath.AsSpan())
                ? originalPath : new string(output);

            inputBuilder.Dispose();
            return returnValue;
        }
    }
}
