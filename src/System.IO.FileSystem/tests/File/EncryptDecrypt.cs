// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security;
using Xunit;

namespace System.IO.Tests
{
    public class EncryptDecrypt : FileSystemTest
    {
        [Fact]
        public static void NullArg_ThrowsException()
        {
            AssertExtensions.Throws<ArgumentNullException>("path", () => File.Encrypt(null));
            AssertExtensions.Throws<ArgumentNullException>("path", () => File.Decrypt(null));
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.Netcoreapp)]
        [Fact]
        public static void EncryptDecrypt_NotSupported()
        {
            Assert.Throws<PlatformNotSupportedException>(() => File.Encrypt("path"));
            Assert.Throws<PlatformNotSupportedException>(() => File.Decrypt("path"));
        }

        // On Windows Nano Server and Home Edition, file encryption with File.Encrypt(string path) throws an IOException
        // because EFS (Encrypted File System), its underlying technology, is not available on these operating systems.
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer), nameof(PlatformDetection.IsNotWindowsHomeEdition))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EncryptDecrypt_Read()
        {
            string tmpFileName = Path.GetTempFileName();
            string textContentToEncrypt = "Content to encrypt";
            File.WriteAllText(tmpFileName, textContentToEncrypt);
            try
            {
                string fileContentRead = File.ReadAllText(tmpFileName);
                Assert.Equal(textContentToEncrypt, fileContentRead);

                try
                {
                    File.Encrypt(tmpFileName);
                }
                catch (IOException e) when (e.HResult == unchecked((int)0x80070490))
                {
                    // Ignore ERROR_NOT_FOUND 1168 (0x490). It is reported when EFS is disabled by domain policy.
                    return;
                }

                Assert.Equal(fileContentRead, File.ReadAllText(tmpFileName));
                Assert.Equal(FileAttributes.Encrypted, (FileAttributes.Encrypted & File.GetAttributes(tmpFileName)));

                File.Decrypt(tmpFileName);
                Assert.Equal(fileContentRead, File.ReadAllText(tmpFileName));
                Assert.NotEqual(FileAttributes.Encrypted, (FileAttributes.Encrypted & File.GetAttributes(tmpFileName)));
            }
            finally
            {
                File.Delete(tmpFileName);
            }
        }
    }
}
