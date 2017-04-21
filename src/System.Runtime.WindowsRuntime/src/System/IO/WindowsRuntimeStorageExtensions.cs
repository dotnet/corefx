// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace System.IO
{
    /// <summary>
    /// Contains extension methods that provide convenience helpers for WinRT IO.
    /// </summary>
    public static class WindowsRuntimeStorageExtensions
    {
        [CLSCompliant(false)]
        public static Task<Stream> OpenStreamForReadAsync(this IStorageFile windowsRuntimeFile)
        {
            if (windowsRuntimeFile == null)
                throw new ArgumentNullException(nameof(windowsRuntimeFile));

            return OpenStreamForReadAsyncCore(windowsRuntimeFile);
        }

        private static async Task<Stream> OpenStreamForReadAsyncCore(this IStorageFile windowsRuntimeFile)
        {
            Debug.Assert(windowsRuntimeFile != null);

            try
            {
                IRandomAccessStream windowsRuntimeStream = await windowsRuntimeFile.OpenAsync(FileAccessMode.Read)
                                                                 .AsTask().ConfigureAwait(continueOnCapturedContext: false);
                Stream managedStream = windowsRuntimeStream.AsStreamForRead();
                return managedStream;
            }
            catch (Exception ex)
            {
                // From this API, managed dev expect IO exceptions for "something wrong":
                WinRtIOHelper.NativeExceptionToIOExceptionInfo(ex).Throw();
                return null;
            }
        }

        [CLSCompliant(false)]
        public static Task<Stream> OpenStreamForWriteAsync(this IStorageFile windowsRuntimeFile)
        {
            if (windowsRuntimeFile == null)
                throw new ArgumentNullException(nameof(windowsRuntimeFile));

            return OpenStreamForWriteAsyncCore(windowsRuntimeFile, 0);
        }


        private static async Task<Stream> OpenStreamForWriteAsyncCore(this IStorageFile windowsRuntimeFile, long offset)
        {
            Debug.Assert(windowsRuntimeFile != null);
            Debug.Assert(offset >= 0);

            try
            {
                IRandomAccessStream windowsRuntimeStream = await windowsRuntimeFile.OpenAsync(FileAccessMode.ReadWrite)
                                                                 .AsTask().ConfigureAwait(continueOnCapturedContext: false);
                Stream managedStream = windowsRuntimeStream.AsStreamForWrite();
                managedStream.Position = offset;
                return managedStream;
            }
            catch (Exception ex)
            {
                // From this API, managed dev expect IO exceptions for "something wrong":
                WinRtIOHelper.NativeExceptionToIOExceptionInfo(ex).Throw();
                return null;
            }
        }

        [CLSCompliant(false)]
        public static Task<Stream> OpenStreamForReadAsync(this IStorageFolder rootDirectory, string relativePath)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException(nameof(rootDirectory));

            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException(SR.Argument_RelativePathMayNotBeWhitespaceOnly, nameof(relativePath));

            return OpenStreamForReadAsyncCore(rootDirectory, relativePath);
        }

        private static async Task<Stream> OpenStreamForReadAsyncCore(this IStorageFolder rootDirectory, string relativePath)
        {
            Debug.Assert(rootDirectory != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(relativePath));

            try
            {
                IStorageFile windowsRuntimeFile = await rootDirectory.GetFileAsync(relativePath)
                                                        .AsTask().ConfigureAwait(continueOnCapturedContext: false);
                Stream managedStream = await windowsRuntimeFile.OpenStreamForReadAsync()
                                             .ConfigureAwait(continueOnCapturedContext: false);
                return managedStream;
            }
            catch (Exception ex)
            {
                // From this API, managed dev expect IO exceptions for "something wrong":
                WinRtIOHelper.NativeExceptionToIOExceptionInfo(ex).Throw();
                return null;
            }
        }

        [CLSCompliant(false)]
        public static Task<Stream> OpenStreamForWriteAsync(this IStorageFolder rootDirectory, string relativePath,
                                                           CreationCollisionOption creationCollisionOption)
        {
            if (rootDirectory == null)
                throw new ArgumentNullException(nameof(rootDirectory));

            if (relativePath == null)
                throw new ArgumentNullException(nameof(relativePath));

            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException(SR.Argument_RelativePathMayNotBeWhitespaceOnly, nameof(relativePath));

            return OpenStreamForWriteAsyncCore(rootDirectory, relativePath, creationCollisionOption);
        }


        private static async Task<Stream> OpenStreamForWriteAsyncCore(this IStorageFolder rootDirectory, string relativePath,
                                                                      CreationCollisionOption creationCollisionOption)
        {
            Debug.Assert(rootDirectory != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(relativePath));

            Debug.Assert(creationCollisionOption == CreationCollisionOption.FailIfExists
                                    || creationCollisionOption == CreationCollisionOption.GenerateUniqueName
                                    || creationCollisionOption == CreationCollisionOption.OpenIfExists
                                    || creationCollisionOption == CreationCollisionOption.ReplaceExisting,
                              "The specified creationCollisionOption has a value that is not a value we considered when devising the"
                            + " policy about Append-On-OpenIfExists used in this method. Apparently a new enum value was added to the"
                            + " CreationCollisionOption type and we need to make sure that the policy still makes sense.");

            try
            {
                // Open file and set up default options for opening it:

                IStorageFile windowsRuntimeFile = await rootDirectory.CreateFileAsync(relativePath, creationCollisionOption)
                                                                     .AsTask().ConfigureAwait(continueOnCapturedContext: false);
                long offset = 0;

                // If the specified creationCollisionOption was OpenIfExists, then we will try to APPEND, otherwise we will OVERWRITE:

                if (creationCollisionOption == CreationCollisionOption.OpenIfExists)
                {
                    BasicProperties fileProperties = await windowsRuntimeFile.GetBasicPropertiesAsync()
                                                           .AsTask().ConfigureAwait(continueOnCapturedContext: false);
                    ulong fileSize = fileProperties.Size;

                    Debug.Assert(fileSize <= long.MaxValue, ".NET streams assume that file sizes are not larger than Int64.MaxValue,"
                                                              + " so we are not supporting the situation where this is not the case.");
                    offset = checked((long)fileSize);
                }

                // Now open a file with the correct options:

                Stream managedStream = await OpenStreamForWriteAsyncCore(windowsRuntimeFile, offset).ConfigureAwait(continueOnCapturedContext: false);
                return managedStream;
            }
            catch (Exception ex)
            {
                // From this API, managed dev expect IO exceptions for "something wrong":
                WinRtIOHelper.NativeExceptionToIOExceptionInfo(ex).Throw();
                return null;
            }
        }
    }
}

