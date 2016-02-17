// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Xunit;

namespace System.Reflection.Tests
{
    public class AssemblyNameTests
    {
        private const ProcessorArchitecture CurrentMaxValue = ProcessorArchitecture.Arm;

        private static IEnumerable<ProcessorArchitecture> ValidProcessorArchitectureValues()
        {
            yield return ProcessorArchitecture.None;
            yield return ProcessorArchitecture.MSIL;
            yield return ProcessorArchitecture.X86;
            yield return ProcessorArchitecture.IA64;
            yield return ProcessorArchitecture.Amd64;
            yield return ProcessorArchitecture.Arm;
        }

        [Fact]
        public void Ctor_Empty()
        {
            var assemblyName = new AssemblyName();
            Assert.Null(assemblyName.Name);
            Assert.Equal(ProcessorArchitecture.None, assemblyName.ProcessorArchitecture);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("name with spaces")]
        public void Ctor_String(string name)
        {
            var assemblyName = new AssemblyName(name);
            Assert.Equal(name, assemblyName.Name);
            Assert.Equal(ProcessorArchitecture.None, assemblyName.ProcessorArchitecture);
        }

        [Fact]
        public void Ctor_String_Invalid()
        {
            Assert.Throws<ArgumentNullException>("assemblyName", () => new AssemblyName(null)); // Assembly name is null
            Assert.Throws<ArgumentException>(() => new AssemblyName("")); // Assembly name is empty
            Assert.Throws<ArgumentException>(() => new AssemblyName("\0")); // Assembly name is null string
        }

        public static IEnumerable<object[]> Ctor_ProcessorArchitecture_TestData()
        {
            // Note that "None" is not valid as part of the name. To get it, the ProcessorArchitecture must be omitted.
            yield return new object[] { "MSIL", ProcessorArchitecture.MSIL };
            yield return new object[] { "msil", ProcessorArchitecture.MSIL };
            yield return new object[] { "mSiL", ProcessorArchitecture.MSIL };
            yield return new object[] { "x86", ProcessorArchitecture.X86 };
            yield return new object[] { "X86", ProcessorArchitecture.X86 };
            yield return new object[] { "IA64", ProcessorArchitecture.IA64 };
            yield return new object[] { "ia64", ProcessorArchitecture.IA64 };
            yield return new object[] { "Ia64", ProcessorArchitecture.IA64 };
            yield return new object[] { "Amd64", ProcessorArchitecture.Amd64 };
            yield return new object[] { "AMD64", ProcessorArchitecture.Amd64 };
            yield return new object[] { "aMd64", ProcessorArchitecture.Amd64 };
            yield return new object[] { "Arm", ProcessorArchitecture.Arm };
            yield return new object[] { "ARM", ProcessorArchitecture.Arm };
            yield return new object[] { "ArM", ProcessorArchitecture.Arm };
        }

        [Theory]
        [MemberData("Ctor_ProcessorArchitecture_TestData")]
        public void Ctor_ValidArchitectureName_Succeeds(string name, ProcessorArchitecture expected)
        {
            string fullName = "Test, ProcessorArchitecture=" + name;
            AssemblyName assemblyName = new AssemblyName(fullName);
            Assert.Equal(expected, assemblyName.ProcessorArchitecture);
        }

        [Theory]
        [InlineData("None")]
        [InlineData("NONE")]
        [InlineData("NoNe")]
        [InlineData("null")]
        [InlineData("Bogus")]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("@!#$!@#$")]
        [InlineData("All your base are belong to us.")]
        public void Ctor_InvalidArchitecture_ThrowsFileLoadException(string invalidName)
        {
            string fullName = "Test, ProcessorArchitecture=" + invalidName;
            Assert.Throws<FileLoadException>(() => new AssemblyName(fullName));
        }

        [Theory]
        [InlineData(AssemblyContentType.WindowsRuntime)]
        public void ContentType(AssemblyContentType contentType)
        {
            AssemblyName n = new AssemblyName("MyAssemblyName");
            Assert.Equal(AssemblyContentType.Default, n.ContentType);
            n.ContentType = contentType;
            Assert.Equal(contentType, n.ContentType);
        }

        [Fact]
        public void ContentType_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            Assert.Equal(AssemblyContentType.Default, assemblyName.ContentType);
        }

        [Fact]
        public void ContentType_SystemRuntimeAssembly()
        {
            AssemblyName assemblyName = typeof(int).GetTypeInfo().Assembly.GetName();
            Assert.Equal(AssemblyContentType.Default, assemblyName.ContentType);
        }

        public static IEnumerable<object[]> CultureName_TestData()
        {
            yield return new object[] { new AssemblyName("Test, Culture=en-US"), "en-US", null, null, "Test" };
            yield return new object[] { new AssemblyName("Test, Culture=en-US"), "en-US", "", "", "Test, Culture=neutral" };
            yield return new object[] { new AssemblyName("Test"), null, "en-US", "en-US", "Test, Culture=en-US" };
            yield return new object[] { new AssemblyName("Test"), null, "En-US", "en-US", "Test, Culture=en-US" };
        }

        [Theory]
        [MemberData("CultureName_TestData")]
        public void CultureName_Set(AssemblyName assemblyName, string originalCultureName, string cultureName, string expectedCultureName, string expectedEqualString)
        {
            Assert.Equal(originalCultureName, assemblyName.CultureName);
            assemblyName.CultureName = cultureName;
            Assert.Equal(expectedCultureName, assemblyName.CultureName);
            Assert.Equal(new AssemblyName(expectedEqualString).FullName, assemblyName.FullName);
        }

        [Fact]
        public void CultureName_Set_Invalid_ThrowsCultureNotFoundException()
        {
            var assemblyName = new AssemblyName("Test");
            Assert.Throws<CultureNotFoundException>(() => new AssemblyName("Test, Culture=NotAValidCulture"));
            Assert.Throws<CultureNotFoundException>(() => assemblyName.CultureName = "NotAValidCulture");
        }

        [Theory]
        [InlineData(AssemblyNameFlags.None)]
        [InlineData(AssemblyNameFlags.PublicKey)]
        [InlineData(AssemblyNameFlags.Retargetable)]
        public void Flags(AssemblyNameFlags flags)
        {
            AssemblyName assemblyName = new AssemblyName("MyAssemblyName");
            Assert.Equal(AssemblyNameFlags.None, assemblyName.Flags);
            assemblyName.Flags = flags;
            Assert.Equal(flags, assemblyName.Flags);
        }

        [Fact]
        public void Flags_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            Assert.NotNull(assemblyName.Flags);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("name with spaces")]
        public void FullName(string name)
        {
            AssemblyName assemblyName = new AssemblyName("MyAssemblyName");

            string expected = "MyAssemblyName";
            string extended = expected + ", Culture=neutral, PublicKeyToken=null";
            Assert.True(assemblyName.FullName == expected || assemblyName.FullName == extended);
        }

        [Fact]
        public void FullName_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            Assert.True(assemblyName.FullName.StartsWith("System.Reflection.Tests"));
            Assert.Equal(assemblyName.Name.Length, assemblyName.FullName.IndexOf(','));
        }

        public static IEnumerable<object[]> SetPublicKey_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new byte[0] };
            yield return new object[] { new byte[16] };
            yield return new object[] { Enumerable.Repeat((byte)'\0', 16).ToArray() };
        }

        [Theory]
        [MemberData("SetPublicKey_TestData")]
        public void SetPublicKey_GetPublicKey(byte[] publicKey)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.SetPublicKey(publicKey);
            Assert.Equal(publicKey, assemblyName.GetPublicKey());
        }

        public static IEnumerable<object[]> SetPublicKeyToken_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new byte[0] };
            yield return new object[] { new byte[16] };
            yield return new object[] { Enumerable.Repeat((byte)'\0', 16).ToArray() };
        }

        [Theory]
        [MemberData("SetPublicKeyToken_TestData")]
        public void SetPublicKeyToken_GetPublicKeyToken(byte[] publicKeyToken)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.SetPublicKeyToken(publicKeyToken);
            Assert.Equal(publicKeyToken, assemblyName.GetPublicKeyToken());
        }
        
        [Fact]
        public void GetPublicKeyToken_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
            Assert.Equal(8, publicKeyToken.Length);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("name with spaces")]
        public void Name(string name)
        {
            AssemblyName assemblyName = new AssemblyName("MyAssemblyName");
            assemblyName.Name = name;
            Assert.Equal(name, assemblyName.Name);
        }

        [Fact]
        public void Name_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            Assert.True(assemblyName.Name.StartsWith("System.Reflection.Tests"));
        }

        public static IEnumerable<object[]> Version_TestData()
        {
            yield return new object[] { new Version(255, 1), "255.1.65535.65535" };
            yield return new object[] { new Version(255, 1, 2), "255.1.2.65535" };
            yield return new object[] { new Version(255, 1, 2, 3), "255.1.2.3" };
        }

        [Theory]
        [MemberData("Version_TestData")]
        public void Version(Version version, string versionString)
        {
            AssemblyName n = new AssemblyName("MyAssemblyName");
            n.Version = version;

            string expected = "MyAssemblyName, Version=" + versionString;
            string extended = expected + ", Culture=neutral, PublicKeyToken=null";
            Assert.True(n.FullName == expected || n.FullName == extended);
        }

        [Fact]
        public void Version_CurrentlyExecutingAssembly()
        {
            AssemblyName assemblyName = typeof(AssemblyNameTests).GetTypeInfo().Assembly.GetName();
            assemblyName.Version = new Version(255, 1, 2, 3);
            Assert.True(assemblyName.FullName.Contains("Version=255.1.2.3"));
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("Hi There")]
        public void ToString(string name)
        {
            var assemblyName = new AssemblyName(name);
            Assert.True(assemblyName.ToString().StartsWith(name), string.Format("Assembly name {0} did not start with \"{1}\".", assemblyName, name));
            Assert.Equal(assemblyName.ToString(), assemblyName.FullName);
        }

        [Theory]
        [InlineData((ProcessorArchitecture)(-1))]
        [InlineData((ProcessorArchitecture)int.MaxValue)]
        [InlineData((ProcessorArchitecture)int.MinValue)]
        [InlineData(CurrentMaxValue + 1)]
        [InlineData((ProcessorArchitecture)(~7 | 0))]
        [InlineData((ProcessorArchitecture)(~7 | 1))]
        [InlineData((ProcessorArchitecture)(~7 | 2))]
        [InlineData((ProcessorArchitecture)(~7 | 3))]
        [InlineData((ProcessorArchitecture)(~7 | 4))]
        [InlineData((ProcessorArchitecture)(~7 | 5))]
        [InlineData((ProcessorArchitecture)(~7 | 6))]
        public void SetProcessorArchitecture_InvalidArchitecture_TakesLowerThreeBitsIfLessThanOrEqualToMax(ProcessorArchitecture invalidArchitecture)
        {
            foreach (ProcessorArchitecture validArchitecture in ValidProcessorArchitectureValues())
            {
                var assemblyName = new AssemblyName();
                assemblyName.ProcessorArchitecture = validArchitecture;
                assemblyName.ProcessorArchitecture = invalidArchitecture;

                ProcessorArchitecture maskedInvalidArchitecture = (ProcessorArchitecture)(((int)invalidArchitecture) & 0x7);
                ProcessorArchitecture expectedResult = maskedInvalidArchitecture > CurrentMaxValue ? validArchitecture : maskedInvalidArchitecture;

                Assert.Equal(expectedResult, assemblyName.ProcessorArchitecture);
            }
        }

        [Fact]
        public void SetProcessorArchitecture_NoneArchitecture_Succeeds()
        {
            foreach (ProcessorArchitecture validArchitecture in ValidProcessorArchitectureValues())
            {
                var assemblyName = new AssemblyName();

                assemblyName.ProcessorArchitecture = validArchitecture;
                assemblyName.ProcessorArchitecture = ProcessorArchitecture.None;

                Assert.Equal(ProcessorArchitecture.None, assemblyName.ProcessorArchitecture);
            }
        }

        [Theory]
        [MemberData("Ctor_ProcessorArchitecture_TestData")]
        public void GetFullNameAndToString_AreEquivalentAndDoNotPreserveArchitecture(string name, ProcessorArchitecture expected)
        {
            string originalFullName = "Test, Culture=en-US, PublicKeyToken=b77a5c561934e089, ProcessorArchitecture=" + name;
            string expectedSerializedFullName = "Test, Culture=en-US, PublicKeyToken=b77a5c561934e089";

            var assemblyName = new AssemblyName(originalFullName);

            Assert.Equal(expectedSerializedFullName, assemblyName.FullName);
            Assert.Equal(expectedSerializedFullName, assemblyName.ToString());
        }

        [Fact]
        public void SetProcessorArchitecture_ValidArchitecture_Succeeds()
        {
            foreach (ProcessorArchitecture validArchitecture in ValidProcessorArchitectureValues())
            {
                var assemblyName = new AssemblyName();
                assemblyName.ProcessorArchitecture = validArchitecture;
                Assert.Equal(validArchitecture, assemblyName.ProcessorArchitecture);
            }
        }

        [Fact]
        public void CheckProcessorArchitectureTestData()
        {
            // Make sure that we don't add values to enum without updating parser and tests.
            var enumValues = (ProcessorArchitecture[])Enum.GetValues(typeof(ProcessorArchitecture));
            Assert.Equal(enumValues, ValidProcessorArchitectureValues());
        }
    }
}
