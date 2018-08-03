// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security;
using Xunit;

namespace System.IO.Tests
{
    public class EncryptDecrypt : FileSystemTest, IClassFixture<EncryptDecryptFixture>
    {
        [Fact]
        public static void NullArg_ThrowsException()
        {
            AssertExtensions.Throws<ArgumentNullException>("path", () => File.Encrypt(null));
            AssertExtensions.Throws<ArgumentNullException>("path", () => File.Decrypt(null));
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Netcoreapp)]
        [Fact]
        public static void EncryptDecrypt_NotSupported()
        {
            Assert.Throws<PlatformNotSupportedException>(() => File.Encrypt("path"));
            Assert.Throws<PlatformNotSupportedException>(() => File.Decrypt("path"));
        }

        [ConditionalFact(typeof(AdminHelpers), nameof(AdminHelpers.IsProcessElevated))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EncryptDecrypt_Read()
        {
            string tmpFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.GetRandomFileName());
            string textContentToEncrypt = "Content to encrypt";
            File.WriteAllText(tmpFileName, textContentToEncrypt);
            try
            {
                //check if runas works
                Whoami();

                File.Encrypt(tmpFileName);
                Assert.Equal(textContentToEncrypt, File.ReadAllText(tmpFileName));

                //try to read encrypted file as other user
                (int exitCode, string contentRead) readResult = ReadAsOtherUser(tmpFileName);
                Assert.NotEqual(0, readResult.exitCode);
                Assert.Empty(readResult.contentRead);

                File.Decrypt(tmpFileName);
                Assert.Equal(textContentToEncrypt, File.ReadAllText(tmpFileName));

                //try to read unecrypted file as other user
                readResult = ReadAsOtherUser(tmpFileName);
                Assert.Equal(0, readResult.exitCode);
                Assert.Equal(textContentToEncrypt, readResult.contentRead);
            }
            finally
            {
                File.Delete(tmpFileName);
            }

            void Whoami()
            {
                ProcessStartInfo psiWhoami = new ProcessStartInfo();
                psiWhoami.Verb = "runas";
                psiWhoami.LoadUserProfile = true;
                psiWhoami.FileName = "cmd.exe";
                psiWhoami.Arguments = $"/C whoami";
                psiWhoami.UseShellExecute = false;
                psiWhoami.UserName = EncryptDecryptFixture.UserName;
                psiWhoami.Password = EncryptDecryptFixture.SecureStringPassword;
                psiWhoami.RedirectStandardOutput = true;
                Process pWhoami = Process.Start(psiWhoami);
                pWhoami.WaitForExit();
                Assert.Equal((Environment.MachineName + "\\" + EncryptDecryptFixture.UserName).ToUpper(), pWhoami.StandardOutput.ReadToEnd().ToUpper().Trim());
            }

            (int exitCode, string contentRead) ReadAsOtherUser(string filePath)
            {
                ProcessStartInfo piReadFileContent = new ProcessStartInfo();
                piReadFileContent.Verb = "runas";
                piReadFileContent.LoadUserProfile = true;
                piReadFileContent.FileName = "cmd.exe";
                piReadFileContent.Arguments = $"/C type \"{filePath}\"";
                piReadFileContent.UseShellExecute = false;
                piReadFileContent.UserName = EncryptDecryptFixture.UserName;
                piReadFileContent.Password = EncryptDecryptFixture.SecureStringPassword;
                piReadFileContent.RedirectStandardOutput = true;
                Process processReadFileContent = Process.Start(piReadFileContent);
                processReadFileContent.WaitForExit();
                return (processReadFileContent.ExitCode, processReadFileContent.StandardOutput.ReadToEnd());
            }
        }
    }

    public class EncryptDecryptFixture : IDisposable
    {
        public const string UserName = "encryptdecryprusr";
        public const string Password = "*VeryComplexPassword42!";
        public static SecureString SecureStringPassword { get; private set; }
        static EncryptDecryptFixture()
        {
            SecureStringPassword = new SecureString();
            foreach (var c in Password)
            {
                SecureStringPassword.AppendChar(c);
            }
        }

        public EncryptDecryptFixture()
        {
            if (AdminHelpers.IsProcessElevated())
            {
                Process createUser = Process.Start("cmd.exe", $"/C \"net user {UserName} {Password} /add /Y\"");
                createUser.WaitForExit();
                Assert.Equal(0, createUser.ExitCode);
            }
        }

        public void Dispose()
        {
            if (AdminHelpers.IsProcessElevated())
            {
                Process createUser = Process.Start("cmd.exe", $"/C \"net user {UserName} /delete\"");
                createUser.WaitForExit();
                Assert.Equal(0, createUser.ExitCode);
            }
        }
    }
}
