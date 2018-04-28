// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Tests
{
    public partial class StringTestsSpan : RemoteExecutorTestBase
    {
        private const string SoftHyphen = "\u00AD";

        [Theory]
        [InlineData("Hello", 0, 0, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
        [InlineData("Hello", 1, 5, 3, new char[] { '\0', '\0', '\0', '\0', '\0', 'e', 'l', 'l', '\0', '\0' })]
        [InlineData("Hello", 2, 0, 3, new char[] { 'l', 'l', 'o', '\0', '\0', '\0', '\0', '\0', '\0', '\0' })]
        [InlineData("Hello", 0, 7, 3, new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', 'H', 'e', 'l' })]
        [InlineData("Hello", 5, 10, 0, new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' })]
        [InlineData("H" + SoftHyphen + "ello", 0, 0, 3, new char[] { 'H', '\u00AD', 'e' })]
        public static void CopyTo(string s, int sourceIndex, int destinationIndex, int count, char[] expected)
        {
            Span<char> dstSpan = new char[expected.Length];
            s.AsSpan(sourceIndex, count).CopyTo(dstSpan.Slice(destinationIndex, count));
            Assert.Equal(expected, dstSpan.ToArray());
        }

        [Theory]
        // CurrentCulture
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.CurrentCulture, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.CurrentCulture, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.CurrentCulture, -1)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.CurrentCulture, 1)]
        [InlineData("hello", 2, "HELLO", 2, 3, StringComparison.CurrentCulture, -1)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.CurrentCulture, 0)]
        [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.CurrentCulture, -1)]
        [InlineData("A", 0, "B", 0, 1, StringComparison.CurrentCulture, -1)]
        [InlineData("B", 0, "A", 0, 1, StringComparison.CurrentCulture, 1)]
        [InlineData(null, 0, null, 0, 0, StringComparison.CurrentCulture, 0)]
        [InlineData("Hello", 0, null, 0, 0, StringComparison.CurrentCulture, 1)]
        [InlineData(null, 0, "Hello", 0, 0, StringComparison.CurrentCulture, -1)]
        [InlineData(null, -1, null, -1, -1, StringComparison.CurrentCulture, 0)]
        [InlineData("foo", -1, null, -1, -1, StringComparison.CurrentCulture, 1)]
        [InlineData(null, -1, "foo", -1, -1, StringComparison.CurrentCulture, -1)]
        // CurrentCultureIgnoreCase
        [InlineData("HELLO", 0, "hello", 0, 5, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Yellow", 2, 3, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.CurrentCultureIgnoreCase, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.CurrentCultureIgnoreCase, -1)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.CurrentCultureIgnoreCase, -1)]
        [InlineData(null, 0, null, 0, 0, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, null, 0, 0, StringComparison.CurrentCultureIgnoreCase, 1)]
        [InlineData(null, 0, "Hello", 0, 0, StringComparison.CurrentCultureIgnoreCase, -1)]
        [InlineData(null, -1, null, -1, -1, StringComparison.CurrentCultureIgnoreCase, 0)]
        [InlineData("foo", -1, null, -1, -1, StringComparison.CurrentCultureIgnoreCase, 1)]
        [InlineData(null, -1, "foo", -1, -1, StringComparison.CurrentCultureIgnoreCase, -1)]
        // InvariantCulture
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.InvariantCulture, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.InvariantCulture, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.InvariantCulture, -1)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.InvariantCulture, 1)]
        [InlineData("hello", 2, "HELLO", 2, 3, StringComparison.InvariantCulture, -1)]
        [InlineData(null, 0, null, 0, 0, StringComparison.InvariantCulture, 0)]
        [InlineData("Hello", 0, null, 0, 5, StringComparison.InvariantCulture, 1)]
        [InlineData(null, 0, "Hello", 0, 5, StringComparison.InvariantCulture, -1)]
        // InvariantCultureIgnoreCase
        [InlineData("HELLO", 0, "hello", 0, 5, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Yellow", 2, 3, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.InvariantCultureIgnoreCase, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.InvariantCultureIgnoreCase, -1)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.InvariantCultureIgnoreCase, -1)]
        [InlineData(null, 0, null, 0, 0, StringComparison.InvariantCultureIgnoreCase, 0)]
        [InlineData("Hello", 0, null, 0, 5, StringComparison.InvariantCultureIgnoreCase, 1)]
        [InlineData(null, 0, "Hello", 0, 5, StringComparison.InvariantCultureIgnoreCase, -1)]
        // Ordinal
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.Ordinal, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.Ordinal, -1)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.Ordinal, 0)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.Ordinal, -1)]
        [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.Ordinal, -1)]
        [InlineData("Hello", 0, "Hello", 0, 0, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 0, "Hello", 0, 3, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 0, "He" + SoftHyphen + "llo", 0, 5, StringComparison.Ordinal, -1)]
        [InlineData("Hello", 0, "-=<Hello>=-", 3, 5, StringComparison.Ordinal, 0)]
        [InlineData("\uD83D\uDD53Hello\uD83D\uDD50", 1, "\uD83D\uDD53Hello\uD83D\uDD54", 1, 7, StringComparison.Ordinal, 0)] // Surrogate split
        [InlineData("Hello", 0, "Hello123", 0, int.MaxValue, StringComparison.Ordinal, -1)]           // Recalculated length, second string longer
        [InlineData("Hello123", 0, "Hello", 0, int.MaxValue, StringComparison.Ordinal, 1)]            // Recalculated length, first string longer
        [InlineData("---aaaaaaaaaaa", 3, "+++aaaaaaaaaaa", 3, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 2, equal compare
        [InlineData("aaaaaaaaaaaaaa", 3, "aaaxaaaaaaaaaa", 3, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 2, different compare at n=1
        [InlineData("-aaaaaaaaaaaaa", 1, "+aaaaaaaaaaaaa", 1, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 6, equal compare
        [InlineData("aaaaaaaaaaaaaa", 1, "axaaaaaaaaaaaa", 1, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 6, different compare at n=1
        [InlineData("aaaaaaaaaaaaaa", 0, "aaaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 4, equal compare
        [InlineData("aaaaaaaaaaaaaa", 0, "xaaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 4, different compare at n=1
        [InlineData("aaaaaaaaaaaaaa", 0, "axaaaaaaaaaaaa", 0, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 4, different compare at n=2
        [InlineData("--aaaaaaaaaaaa", 2, "++aaaaaaaaaaaa", 2, 100, StringComparison.Ordinal, 0)]      // Equal long alignment 0, equal compare
        [InlineData("aaaaaaaaaaaaaa", 2, "aaxaaaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=1
        [InlineData("aaaaaaaaaaaaaa", 2, "aaaxaaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=2
        [InlineData("aaaaaaaaaaaaaa", 2, "aaaaxaaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=3
        [InlineData("aaaaaaaaaaaaaa", 2, "aaaaaxaaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=4
        [InlineData("aaaaaaaaaaaaaa", 2, "aaaaaaxaaaaaaa", 2, 100, StringComparison.Ordinal, -1)]     // Equal long alignment 0, different compare at n=5
        [InlineData("aaaaaaaaaaaaaa", 0, "+aaaaaaaaaaaaa", 1, 13, StringComparison.Ordinal, 0)]       // Different int alignment, equal compare
        [InlineData("aaaaaaaaaaaaaa", 0, "aaaaaaaaaaaaax", 1, 100, StringComparison.Ordinal, -1)]     // Different int alignment
        [InlineData("aaaaaaaaaaaaaa", 1, "aaaxaaaaaaaaaa", 3, 100, StringComparison.Ordinal, -1)]     // Different long alignment, abs of 4, one of them is 2, different at n=1
        [InlineData("-aaaaaaaaaaaaa", 1, "++++aaaaaaaaaa", 4, 10, StringComparison.Ordinal, 0)]       // Different long alignment, equal compare
        [InlineData("aaaaaaaaaaaaaa", 1, "aaaaaaaaaaaaax", 4, 100, StringComparison.Ordinal, -1)]     // Different long alignment
        [InlineData("\0", 0, "", 0, 1, StringComparison.Ordinal, 1)]                                  // Same memory layout, except for m_stringLength (m_firstChars are both 0)
        [InlineData("\0\0", 0, "", 0, 2, StringComparison.Ordinal, 1)]                                // Same as above, except m_stringLength for one is 2
        [InlineData("", 0, "\0b", 0, 2, StringComparison.Ordinal, -1)]                                // strA's second char != strB's second char codepath
        [InlineData("", 0, "b", 0, 1, StringComparison.Ordinal, -1)]                                  // Should hit strA.m_firstChar != strB.m_firstChar codepath
        [InlineData("abcxxxxxxxxxxxxxxxxxxxxxx", 0, "abdxxxxxxxxxxxxxxx", 0, int.MaxValue, StringComparison.Ordinal, -1)] // 64-bit: first long compare is different
        [InlineData("abcdefgxxxxxxxxxxxxxxxxxx", 0, "abcdefhxxxxxxxxxxx", 0, int.MaxValue, StringComparison.Ordinal, -1)] // 64-bit: second long compare is different
        [InlineData("abcdefghijkxxxxxxxxxxxxxx", 0, "abcdefghijlxxxxxxx", 0, int.MaxValue, StringComparison.Ordinal, -1)] // 64-bit: third long compare is different
        [InlineData("abcdexxxxxxxxxxxxxxxxxxxx", 0, "abcdfxxxxxxxxxxxxx", 0, int.MaxValue, StringComparison.Ordinal, -1)] // 32-bit: second int compare is different
        [InlineData("abcdefghixxxxxxxxxxxxxxxx", 0, "abcdefghjxxxxxxxxx", 0, int.MaxValue, StringComparison.Ordinal, -1)] // 32-bit: fourth int compare is different
        [InlineData(null, 0, null, 0, 0, StringComparison.Ordinal, 0)]
        [InlineData("Hello", 0, null, 0, 5, StringComparison.Ordinal, 1)]
        [InlineData(null, 0, "Hello", 0, 5, StringComparison.Ordinal, -1)]
        [InlineData(null, -1, null, -1, -1, StringComparison.Ordinal, 0)]
        [InlineData("foo", -1, null, -1, -1, StringComparison.Ordinal, 1)]
        [InlineData(null, -1, "foo", -1, -1, StringComparison.Ordinal, -1)]
        // OrdinalIgnoreCase
        [InlineData("HELLO", 0, "hello", 0, 5, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 0, "Hello", 0, 5, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 2, "Hello", 2, 3, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 2, "Yellow", 2, 3, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 0, "Goodbye", 0, 5, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("Goodbye", 0, "Hello", 0, 5, StringComparison.OrdinalIgnoreCase, -1)]
        [InlineData("HELLO", 2, "hello", 2, 3, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 2, "Goodbye", 2, 3, StringComparison.OrdinalIgnoreCase, -1)]
        [InlineData("A", 0, "x", 0, 1, StringComparison.OrdinalIgnoreCase, -1)]
        [InlineData("a", 0, "X", 0, 1, StringComparison.OrdinalIgnoreCase, -1)]
        [InlineData("[", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("[", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("\\", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("\\", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("]", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("]", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("^", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("^", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("_", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("_", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("`", 0, "A", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData("`", 0, "a", 0, 1, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData(null, 0, null, 0, 0, StringComparison.OrdinalIgnoreCase, 0)]
        [InlineData("Hello", 0, null, 0, 5, StringComparison.OrdinalIgnoreCase, 1)]
        [InlineData(null, 0, "Hello", 0, 5, StringComparison.OrdinalIgnoreCase, -1)]
        public static void Compare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType, int expected)
        {
            if (indexA >= 0 && indexB >= 0 && length >= 0)
            {
                // Comparing spans from null strings gives different results since span doesn't special case null and treats it the same as an empty string.
                if (strA == null && strB != null)
                    expected = -1;
                if (strA != null && strB == null)
                    expected = 1;
                if (length == 0)
                    expected = 0;

                ReadOnlySpan<char> span = length <= (strA.AsSpan().Length - indexA) ? strA.AsSpan(indexA, length) : strA.AsSpan(indexA);
                ReadOnlySpan<char> value = length <= (strB.AsSpan().Length - indexB) ? strB.AsSpan(indexB, length) : strB.AsSpan(indexB);
                Assert.Equal(expected, Math.Sign(span.CompareTo(value, comparisonType)));
            }
        }

        [Fact]
        public static void Compare_LongString()
        {
            string veryLongString =
                "<NamedPermissionSets><PermissionSet class=\u0022System.Security.NamedPermissionS" +
                "et\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022 Name=\u0022FullTrust" +
                "\u0022 Description=\u0022{Policy_PS_FullTrust}\u0022/><PermissionSet class=\u0022" +
                "System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022Everyth" +
                "ing\u0022 Description=\u0022{Policy_PS_Everything}\u0022><Permission class=\u0022" +
                "System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version={VE" +
                "RSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022" +
                " Unrestricted=\u0022true\u0022/><Permission class=\u0022System.Security.Permissi" +
                "ons.EnvironmentPermission, mscorlib, Version={VERSION}, Culture=neutral, PublicK" +
                "eyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022" +
                "/><Permission class=\u0022System.Security.Permissions.FileIOPermission, mscorlib" +
                ", Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022versi" +
                "on=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission class=\u0022System.S" +
                "ecurity.Permissions.FileDialogPermission, mscorlib, Version={VERSION}, Culture=n" +
                "eutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=" +
                "\u0022true\u0022/><Permission class=\u0022System.Security.Permissions.Reflection" +
                "Permission, mscorlib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c5" +
                "61934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><Permission " +
                "class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version={V" +
                "ERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022" +
                " Flags=\u0022Assertion, UnmanagedCode, Execution, ControlThread, ControlEvidence" +
                ", ControlPolicy, ControlAppDomain, SerializationFormatter, ControlDomainPolicy, " +
                "ControlPrincipal, RemotingConfiguration, Infrastructure, BindingRedirects\u0022/" +
                "><Permission class=\u0022System.Security.Permissions.UIPermission, mscorlib, Ver" +
                "sion={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u0022" +
                "1\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Net.Socke" +
                "tPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c56" +
                "1934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission " +
                "class=\u0022System.Net.WebPermission, System, Version={VERSION}, Culture=neutral" +
                ", PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022" +
                "true\u0022/><IPermission class=\u0022System.Net.DnsPermission, System, Version={" +
                "VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022" +
                " Unrestricted=\u0022true\u0022/><IPermission class=\u0022System.Security.Permiss" +
                "ions.KeyContainerPermission, mscorlib, Version={VERSION}, Culture=neutral, Publi" +
                "cKeyToken=b77a5c561934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022" +
                "/><Permission class=\u0022System.Security.Permissions.RegistryPermission, mscorl" +
                "ib, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022ver" +
                "sion=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022Syste" +
                "m.Drawing.Printing.PrintingPermission, System.Drawing, Version={VERSION}, Cultur" +
                "e=neutral, PublicKeyToken=b03f5f7f11d50a3a\u0022version=\u00221\u0022 Unrestrict" +
                "ed=\u0022true\u0022/><IPermission class=\u0022System.Diagnostics.EventLogPermiss" +
                "ion, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089" +
                "\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission class=\u0022" +
                "System.Security.Permissions.StorePermission, System, Version={VERSION}, Culture=" +
                "neutral, PublicKeyToken=b77a5c561934e089\u0022 version=\u00221\u0022 Unrestricte" +
                "d=\u0022true\u0022/><IPermission class=\u0022System.Diagnostics.PerformanceCount" +
                "erPermission, System, Version={VERSION}, Culture=neutral, PublicKeyToken=b77a5c5" +
                "61934e089\u0022version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPermission" +
                " class=\u0022System.Data.OleDb.OleDbPermission, System.Data, Version={VERSION}, " +
                "Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022 version=\u00221\u0022 Unr" +
                "estricted=\u0022true\u0022/><IPermission class=\u0022System.Data.SqlClient.SqlCl" +
                "ientPermission, System.Data, Version={VERSION}, Culture=neutral, PublicKeyToken=" +
                "b77a5c561934e089\u0022 version=\u00221\u0022 Unrestricted=\u0022true\u0022/><IPe" +
                "rmission class=\u0022System.Security.Permissions.DataProtectionPermission, Syste" +
                "m.Security, Version={VERSION}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\u0022" +
                " version=\u00221\u0022 Unrestricted=\u0022true\u0022/></PermissionSet><Permissio" +
                "nSet class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 N" +
                "ame=\u0022Nothing\u0022 Description=\u0022{Policy_PS_Nothing}\u0022/><Permission" +
                "Set class=\u0022System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Na" +
                "me=\u0022Execution\u0022 Description=\u0022{Policy_PS_Execution}\u0022><Permissi" +
                "on class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version" +
                "={VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u0022" +
                "1\u0022 Flags=\u0022Execution\u0022/></PermissionSet><PermissionSet class=\u0022" +
                "System.Security.NamedPermissionSet\u0022version=\u00221\u0022 Name=\u0022SkipVer" +
                "ification\u0022 Description=\u0022{Policy_PS_SkipVerification}\u0022><Permission" +
                " class=\u0022System.Security.Permissions.SecurityPermission, mscorlib, Version={" +
                "VERSION}, Culture=neutral, PublicKeyToken=b77a5c561934e089\u0022version=\u00221\u0022" +
                " Flags=\u0022SkipVerification\u0022/></PermissionSet></NamedPermissionSets>";

            int result = "{Policy_PS_Nothing}".AsSpan().CompareTo(veryLongString.AsSpan(4380, 19), StringComparison.Ordinal);
            Assert.True(result < 0);
        }

        [Theory]
        // CurrentCulture
        [InlineData("", "Foo", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "llo", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "HELLO", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Abc", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.CurrentCulture, true)]
        [InlineData("", "", StringComparison.CurrentCulture, true)]
        [InlineData("", "a", StringComparison.CurrentCulture, false)]
        // CurrentCultureIgnoreCase
        [InlineData("Hello", "llo", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "LLO", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("", "a", StringComparison.CurrentCultureIgnoreCase, false)]
        // InvariantCulture
        [InlineData("", "Foo", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "llo", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "Hello", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "HELLO", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Abc", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.InvariantCulture, true)]
        [InlineData("", "", StringComparison.InvariantCulture, true)]
        [InlineData("", "a", StringComparison.InvariantCulture, false)]
        // InvariantCultureIgnoreCase
        [InlineData("Hello", "llo", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "LLO", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("", "", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("", "a", StringComparison.InvariantCultureIgnoreCase, false)]
        // Ordinal
        [InlineData("Hello", "o", StringComparison.Ordinal, true)]
        [InlineData("Hello", "llo", StringComparison.Ordinal, true)]
        [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
        [InlineData("Hello", "Larger Hello", StringComparison.Ordinal, false)]
        [InlineData("Hello", "", StringComparison.Ordinal, true)]
        [InlineData("Hello", "LLO", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Abc", StringComparison.Ordinal, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.Ordinal, false)]
        [InlineData("", "", StringComparison.Ordinal, true)]
        [InlineData("", "a", StringComparison.Ordinal, false)]
        // OrdinalIgnoreCase
        [InlineData("Hello", "llo", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Larger Hello", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "LLO", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "llo" + SoftHyphen, StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("", "a", StringComparison.OrdinalIgnoreCase, false)]
        public static void EndsWith(string s, string value, StringComparison comparisonType, bool expected)
        {
            // Cannot use implicit cast from string to ReadOnlySpan for other runtimes, like netfx. Therefore, explicitly call AsSpan.
            Assert.Equal(expected, s.AsSpan().EndsWith(value.AsSpan(), comparisonType));
        }

        [Theory]
        [ActiveIssue("https://github.com/dotnet/coreclr/issues/2051", TestPlatforms.AnyUnix)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        public static void EndsWith_NullInStrings(StringComparison comparison)
        {
            Assert.True("\0test".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.True("te\0st".AsSpan().EndsWith("e\0st".AsSpan(), comparison));
            Assert.False("te\0st".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.False("test\0".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.False("test".AsSpan().EndsWith("\0st".AsSpan(), comparison));
        }

        // NOTE: This is by design. Unix ignores the null characters (i.e. null characters have no weights for the string comparison).
        // For desired behavior, use ordinal comparison instead of linguistic comparison.
        // This is a known difference between Windows and Unix (https://github.com/dotnet/coreclr/issues/2051).
        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.InvariantCulture)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        public static void EndsWith_NullInStrings_NonOrdinal(StringComparison comparison)
        {
            Assert.True("\0test".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.True("te\0st".AsSpan().EndsWith("e\0st".AsSpan(), comparison));
            Assert.False("te\0st".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.False("test\0".AsSpan().EndsWith("test".AsSpan(), comparison));
            Assert.False("test".AsSpan().EndsWith("\0st".AsSpan(), comparison));
        }

        [Theory]
        // CurrentCulture
        [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "hello", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Helloo", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Hell", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", null, StringComparison.CurrentCulture, false)]
        [InlineData(null, "Hello", StringComparison.CurrentCulture, false)]
        [InlineData(null, null, StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.CurrentCulture, false)]
        [InlineData("", "Hello", StringComparison.CurrentCulture, false)]
        [InlineData("", "", StringComparison.CurrentCulture, true)]
        [InlineData("123", 123, StringComparison.CurrentCulture, false)] // Not a string
        [InlineData("\0AAAAAAAAA", "\0BBBBBBBBBBBB", StringComparison.CurrentCulture, false)]
        // CurrentCultureIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "helloo", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", "hell", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", null, StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData(null, "Hello", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData(null, null, StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("123", 123, StringComparison.CurrentCultureIgnoreCase, false)] // Not a string
        [InlineData("\0AAAAAAAAA", "\0BBBBBBBBBBBB", StringComparison.CurrentCultureIgnoreCase, false)]
        // InvariantCulture
        [InlineData("Hello", "Hello", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "hello", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Helloo", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Hell", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", null, StringComparison.InvariantCulture, false)]
        [InlineData(null, "Hello", StringComparison.InvariantCulture, false)]
        [InlineData(null, null, StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "", StringComparison.InvariantCulture, false)]
        [InlineData("", "Hello", StringComparison.InvariantCulture, false)]
        [InlineData("", "", StringComparison.InvariantCulture, true)]
        [InlineData("123", 123, StringComparison.InvariantCulture, false)] // Not a string
        [InlineData("\0AAAAAAAAA", "\0BBBBBBBBBBBB", StringComparison.InvariantCulture, false)]
        // InvariantCultureIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Helloo", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", "Hell", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", null, StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData(null, "Hello", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData(null, null, StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("", "", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("123", 123, StringComparison.InvariantCultureIgnoreCase, false)] // Not a string
        [InlineData("\0AAAAAAAAA", "\0BBBBBBBBBBBB", StringComparison.InvariantCultureIgnoreCase, false)]
        // Ordinal
        [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
        [InlineData("Hello", "hello", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Helloo", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Hell", StringComparison.Ordinal, false)]
        [InlineData("Hello", null, StringComparison.Ordinal, false)]
        [InlineData(null, "Hello", StringComparison.Ordinal, false)]
        [InlineData(null, null, StringComparison.Ordinal, true)]
        [InlineData("Hello", "", StringComparison.Ordinal, false)]
        [InlineData("", "Hello", StringComparison.Ordinal, false)]
        [InlineData("", "", StringComparison.Ordinal, true)]
        [InlineData("123", 123, StringComparison.Ordinal, false)] // Not a string
        // OridinalIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("HELLO", "hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Helloo", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "Hell", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234\u5678", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("\u1234\u5678", "\u1234\u5679", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1235\u5678", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234\u56789\u1234", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", null, StringComparison.OrdinalIgnoreCase, false)]
        [InlineData(null, "Hello", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData(null, null, StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("123", 123, StringComparison.OrdinalIgnoreCase, false)] // Not a string
        [InlineData("\0AAAAAAAAA", "\0BBBBBBBBBBBB", StringComparison.OrdinalIgnoreCase, false)]
        public static void Equals(string s1, object obj, StringComparison comparisonType, bool expected)
        {
            string s2 = obj as string;
            Assert.Equal(expected, s1.AsSpan().Equals(s2.AsSpan(), comparisonType));
        }

        public static IEnumerable<object[]> Equals_EncyclopaediaData()
        {
            yield return new object[] { StringComparison.CurrentCulture, false };
            yield return new object[] { StringComparison.CurrentCultureIgnoreCase, false };
            yield return new object[] { StringComparison.Ordinal, false };
            yield return new object[] { StringComparison.OrdinalIgnoreCase, false };

            // Windows and ICU disagree about how these strings compare in the default locale.
            yield return new object[] { StringComparison.InvariantCulture, PlatformDetection.IsWindows };
            yield return new object[] { StringComparison.InvariantCultureIgnoreCase, PlatformDetection.IsWindows };
        }

        [Theory]
        [MemberData(nameof(Equals_EncyclopaediaData))]
        public void Equals_Encyclopaedia_ReturnsExpected(StringComparison comparison, bool expected)
        {
            RemoteInvoke((comparisonString, expectedString) =>
            {
                string source = "encyclop\u00e6dia";
                string target = "encyclopaedia";

                CultureInfo.CurrentCulture = new CultureInfo("se-SE");
                StringComparison comparisonType = (StringComparison)Enum.Parse(typeof(StringComparison), comparisonString);

                Assert.Equal(bool.Parse(expectedString), source.AsSpan().Equals(target.AsSpan(), comparisonType));

                return SuccessExitCode;
            }, comparison.ToString(), expected.ToString()).Dispose();
        }

        private static bool IsSafeForCurrentCultureComparisons(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                // We only want ASCII chars that you can see
                // No controls, no delete, nothing >= 0x80
                if (c < 0x20 || c == 0x7f || c >= 0x80)
                {
                    return false;
                }
            }
            return true;
        }

        [Theory]
        [InlineData("Hello", 'l', 0, 5, 2)]
        [InlineData("Hello", 'x', 0, 5, -1)]
        [InlineData("Hello", 'l', 1, 4, 2)]
        [InlineData("Hello", 'l', 3, 2, 3)]
        [InlineData("Hello", 'l', 4, 1, -1)]
        [InlineData("Hello", 'x', 1, 4, -1)]
        [InlineData("Hello", 'l', 3, 0, -1)]
        [InlineData("Hello", 'l', 0, 2, -1)]
        [InlineData("Hello", 'l', 0, 3, 2)]
        [InlineData("Hello", 'l', 4, 1, -1)]
        [InlineData("Hello", 'x', 1, 4, -1)]
        [InlineData("Hello", 'o', 5, 0, -1)]
        [InlineData("H" + SoftHyphen + "ello", 'e', 0, 3, 2)]
        // For some reason, this is failing on *nix with ordinal comparisons.
        // Possibly related issue: dotnet/coreclr#2051
        // [InlineData("Hello", '\0', 0, 5, -1)] // .NET strings are terminated with a null character, but they should not be included as part of the string
        [InlineData("\ud800\udfff", '\ud800', 0, 1, 0)] // Surrogate characters
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'A', 0, 26, 0)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'B', 1, 25, 1)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'C', 2, 24, 2)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'D', 3, 23, 3)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'G', 2, 24, 6)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'K', 2, 24, 10)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'O', 2, 24, 14)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'P', 2, 24, 15)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'Q', 2, 24, 16)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 'R', 2, 24, 17)]
        [InlineData("________\u8080\u8080\u8080________", '\u0080', 0, 19, -1)]
        [InlineData("________\u8000\u8000\u8000________", '\u0080', 0, 19, -1)]
        [InlineData("__\u8080\u8000\u0080______________", '\u0080', 0, 19, 4)]
        [InlineData("__\u8080\u8000__\u0080____________", '\u0080', 0, 19, 6)]
        [InlineData("__________________________________", '\ufffd', 0, 34, -1)]
        [InlineData("____________________________\ufffd", '\ufffd', 0, 29, 28)]
        [InlineData("ABCDEFGHIJKLM", 'M', 0, 13, 12)]
        [InlineData("ABCDEFGHIJKLMN", 'N', 0, 14, 13)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", '@', 0, 26, -1)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXY", '@', 0, 25, -1)]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ#", '@', 0, 27, -1)]
        [InlineData("_____________\u807f", '\u007f', 0, 14, -1)]
        [InlineData("_____________\u807f__", '\u007f', 0, 16, -1)]
        [InlineData("_____________\u807f\u007f_", '\u007f', 0, 16, 14)]
        [InlineData("__\u807f_______________", '\u007f', 0, 18, -1)]
        [InlineData("__\u807f___\u007f___________", '\u007f', 0, 18, 6)]
        [InlineData("ABCDEFGHIJKLMN", 'N', 2, 11, -1)]
        [InlineData("!@#$%^&", '%', 0, 7, 4)]
        [InlineData("!@#$", '!', 0, 4, 0)]
        [InlineData("!@#$", '@', 0, 4, 1)]
        [InlineData("!@#$", '#', 0, 4, 2)]
        [InlineData("!@#$", '$', 0, 4, 3)]
        [InlineData("!@#$%^&*", '%', 0, 8, 4)]
        [InlineData("", 'H', 0, 0, -1)]
        public static void IndexOf_SingleLetter(string s, char target, int startIndex, int count, int expected)
        {
            bool safeForCurrentCulture =
                IsSafeForCurrentCultureComparisons(s)
                && IsSafeForCurrentCultureComparisons(target.ToString());

            ReadOnlySpan<char> span = s.AsSpan();
            var charArray = new char[1];
            charArray[0] = target;
            ReadOnlySpan<char> targetSpan = charArray;

            int expectedFromSpan = expected == -1 ? expected : expected - startIndex;

            if (count + startIndex == s.Length)
            {
                if (startIndex == 0)
                {
                    Assert.Equal(expectedFromSpan, span.IndexOf(targetSpan, StringComparison.Ordinal));
                    Assert.Equal(expectedFromSpan, span.IndexOf(targetSpan, StringComparison.OrdinalIgnoreCase));

                    // To be safe we only want to run CurrentCulture comparisons if
                    // we know the results will not vary depending on location
                    if (safeForCurrentCulture)
                    {
                        Assert.Equal(expectedFromSpan, span.IndexOf(targetSpan, StringComparison.CurrentCulture));
                    }
                }

                Assert.Equal(expectedFromSpan, span.Slice(startIndex).IndexOf(targetSpan, StringComparison.Ordinal));
                Assert.Equal(expectedFromSpan, span.Slice(startIndex).IndexOf(targetSpan, StringComparison.OrdinalIgnoreCase));

                if (safeForCurrentCulture)
                {
                    Assert.Equal(expectedFromSpan, span.Slice(startIndex).IndexOf(targetSpan, StringComparison.CurrentCulture));
                }
            }

            Assert.Equal(expectedFromSpan, span.Slice(startIndex, count).IndexOf(targetSpan, StringComparison.Ordinal));
            Assert.Equal(expectedFromSpan, span.Slice(startIndex, count).IndexOf(targetSpan, StringComparison.OrdinalIgnoreCase));

            if (safeForCurrentCulture)
            {
                Assert.Equal(expectedFromSpan, span.Slice(startIndex, count).IndexOf(targetSpan, StringComparison.CurrentCulture));
            }
        }

        [Fact]
        public static void IndexOf_Match_SingleLetter()
        {
            Assert.Equal(-1, "".AsSpan().IndexOf('a'));

            for (int length = 1; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    char target = a[targetIndex];
                    int idx = span.IndexOf(target);
                    Assert.Equal(targetIndex, idx);
                }
            }
        }

        [Theory]
        [ActiveIssue("https://github.com/dotnet/coreclr/issues/2051", TestPlatforms.AnyUnix)]
        [InlineData("He\0lo", "He\0lo", 0)]
        [InlineData("He\0lo", "He\0", 0)]
        [InlineData("He\0lo", "\0", 2)]
        [InlineData("He\0lo", "\0lo", 2)]
        [InlineData("He\0lo", "lo", 3)]
        [InlineData("Hello", "lo\0", -1)]
        [InlineData("Hello", "\0lo", -1)]
        [InlineData("Hello", "l\0o", -1)]
        public static void IndexOf_NullInStrings(string s, string value, int expected)
        {
            Assert.Equal(expected, s.AsSpan().IndexOf(value.AsSpan(), StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("Hello", 0, 'H')]
        [InlineData("Hello", 1, 'e')]
        [InlineData("Hello", 2, 'l')]
        [InlineData("Hello", 3, 'l')]
        [InlineData("Hello", 4, 'o')]
        [InlineData("\0", 0, '\0')]
        public static void Item_Get(string s, int index, char expected)
        {
            Assert.Equal(expected, s.AsSpan()[index]);
        }

        [Fact]
        public static void Item_Get_InvalidIndex_ThrowsIndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => "Hello".AsSpan()[-1]); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => "Hello".AsSpan()[5]); // Index >= string.Length
            Assert.Throws<IndexOutOfRangeException>(() => "".AsSpan()[0]); // Index >= string.Length
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("\0", 1)]
        [InlineData("abc", 3)]
        [InlineData("hello", 5)]
        public static void Length(string s, int expected)
        {
            Assert.Equal(expected, s.AsSpan().Length);
        }

        public static IEnumerable<object[]> AllSubstringsAndComparisons(string source)
        {
            var comparisons = new StringComparison[]
            {
            StringComparison.CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase,
            StringComparison.Ordinal,
            StringComparison.OrdinalIgnoreCase
            };

            foreach (StringComparison comparison in comparisons)
            {
                for (int i = 0; i <= source.Length; i++)
                {
                    for (int subLen = source.Length - i; subLen > 0; subLen--)
                    {
                        yield return new object[] { source, source.Substring(i, subLen), i, comparison };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(AllSubstringsAndComparisons), new object[] { "abcde" })]
        public static void IndexOf_AllSubstrings(string s, string value, int startIndex, StringComparison comparison)
        {
            bool ignoringCase = comparison == StringComparison.OrdinalIgnoreCase || comparison == StringComparison.CurrentCultureIgnoreCase;

            // First find the substring.  We should be able to with all comparison types.            
            Assert.Equal(startIndex, s.AsSpan().IndexOf(value.AsSpan(), comparison)); // in the whole string
            Assert.Equal(0, s.AsSpan(startIndex).IndexOf(value.AsSpan(), comparison)); // starting at substring

            if (startIndex > 0)
            {
                Assert.Equal(1, s.AsSpan(startIndex - 1).IndexOf(value.AsSpan(), comparison)); // starting just before substring
            }
            Assert.Equal(-1, s.AsSpan(startIndex + 1).IndexOf(value.AsSpan(), comparison)); // starting just after start of substring            

            // Shouldn't be able to find the substring if the count is less than substring's length            
            Assert.Equal(-1, s.AsSpan(0, value.Length - 1).IndexOf(value.AsSpan(), comparison));

            // Now double the source.  Make sure we find the first copy of the substring.
            int halfLen = s.Length;
            s += s;
            Assert.Equal(startIndex, s.AsSpan().IndexOf(value.AsSpan(), comparison));

            // Now change the case of a letter.
            s = s.ToUpperInvariant();
            Assert.Equal(ignoringCase ? startIndex : -1, s.AsSpan().IndexOf(value.AsSpan(), comparison));
        }

        [Fact]
        public static void IndexOf_TurkishI_TurkishCulture()
        {
            RemoteInvoke(() =>
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

                string s = "Turkish I \u0131s TROUBL\u0130NG!";
                string value = "\u0130";

                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(4, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                value = "\u0131";
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_TurkishI_InvariantCulture()
        {
            RemoteInvoke(() =>
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                string s = "Turkish I \u0131s TROUBL\u0130NG!";
                string value = "\u0130";

                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                value = "\u0131";
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_TurkishI_EnglishUSCulture()
        {
            RemoteInvoke(() =>
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");

                string s = "Turkish I \u0131s TROUBL\u0130NG!";
                string value = "\u0130";

                value = "\u0130";
                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(19, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                value = "\u0131";
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_HungarianDoubleCompression_HungarianCulture()
        {
            RemoteInvoke(() =>
            {
                string source = "dzsdzs";
                string target = "ddzs";

                CultureInfo.CurrentCulture = new CultureInfo("hu-HU");
                /*
                 There are differences between Windows and ICU regarding contractions.
                 Windows has equal contraction collation weights, including case (target="Ddzs" same behavior as "ddzs").
                 ICU has different contraction collation weights, depending on locale collation rules.
                 If CurrentCultureIgnoreCase is specified, ICU will use 'secondary' collation rules
                 which ignore the contraction collation weights (defined as 'tertiary' rules)
                */
                ReadOnlySpan<char> span = source.AsSpan();

                Assert.Equal(PlatformDetection.IsWindows ? 0 : -1, span.IndexOf(target.AsSpan(), StringComparison.CurrentCulture));

                Assert.Equal(0, span.IndexOf(target.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(-1, span.IndexOf(target.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(-1, span.IndexOf(target.AsSpan(), StringComparison.OrdinalIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_HungarianDoubleCompression_InvariantCulture()
        {
            RemoteInvoke(() =>
            {
                string source = "dzsdzs";
                string target = "ddzs";

                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                ReadOnlySpan<char> span = source.AsSpan();
                Assert.Equal(-1, span.IndexOf(target.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(-1, span.IndexOf(target.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_EquivalentDiacritics_EnglishUSCulture()
        {
            RemoteInvoke(() =>
            {
                string s = "Exhibit a\u0300\u00C0";
                string value = "\u00C0";

                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                value = "a\u0300"; // this diacritic combines with preceding character                
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_EquivalentDiacritics_InvariantCulture()
        {
            RemoteInvoke(() =>
            {
                string s = "Exhibit a\u0300\u00C0";
                string value = "\u00C0";

                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(10, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                value = "a\u0300"; // this diacritic combines with preceding character                
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(8, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_CyrillicE_EnglishUSCulture()
        {
            RemoteInvoke(() =>
            {
                string s = "Foo\u0400Bar";
                string value = "\u0400";

                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                value = "bar";
                Assert.Equal(-1, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(4, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(-1, span.IndexOf(value.AsSpan(), StringComparison.Ordinal));
                Assert.Equal(4, span.IndexOf(value.AsSpan(), StringComparison.OrdinalIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public static void IndexOf_CyrillicE_InvariantCulture()
        {
            RemoteInvoke(() =>
            {
                string s = "Foo\u0400Bar";
                string value = "\u0400";

                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                ReadOnlySpan<char> span = s.AsSpan();
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(3, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                value = "bar";
                Assert.Equal(-1, span.IndexOf(value.AsSpan(), StringComparison.CurrentCulture));
                Assert.Equal(4, span.IndexOf(value.AsSpan(), StringComparison.CurrentCultureIgnoreCase));

                return SuccessExitCode;
            }).Dispose();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("foo", false)]
        [InlineData("   ", false)]
        public static void IsNullOrEmpty(string value, bool expected)
        {
            Assert.Equal(expected, value.AsSpan().IsEmpty);
        }

        [Fact]
        public static void LastIndexOf_Match_SingleLetter()
        {
            Assert.Equal(-1, "".AsSpan().LastIndexOf('a'));

            for (int length = 1; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                string str = new string(a);
                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    char target = a[targetIndex];
                    int idx = span.LastIndexOf(target);
                    Assert.Equal(targetIndex, idx);
                }
            }
        }

        [Theory]
        // CurrentCulture
        [InlineData("Hello", "Hel", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "HELLO", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Abc", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.CurrentCulture, true)]
        [InlineData("", "", StringComparison.CurrentCulture, true)]
        [InlineData("", "hello", StringComparison.CurrentCulture, false)]
        // CurrentCultureIgnoreCase
        [InlineData("Hello", "Hel", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "HEL", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("", "hello", StringComparison.CurrentCultureIgnoreCase, false)]
        // InvariantCulture
        [InlineData("Hello", "Hel", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "Hello", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "HELLO", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Abc", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.InvariantCulture, true)]
        [InlineData("", "", StringComparison.InvariantCulture, true)]
        [InlineData("", "hello", StringComparison.InvariantCulture, false)]
        // InvariantCultureIgnoreCase
        [InlineData("Hello", "Hel", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "HEL", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("", "", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("", "hello", StringComparison.InvariantCultureIgnoreCase, false)]
        // Ordinal
        [InlineData("Hello", "H", StringComparison.Ordinal, true)]
        [InlineData("Hello", "Hel", StringComparison.Ordinal, true)]
        [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
        [InlineData("Hello", "Hello Larger", StringComparison.Ordinal, false)]
        [InlineData("Hello", "", StringComparison.Ordinal, true)]
        [InlineData("Hello", "HEL", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Abc", StringComparison.Ordinal, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.Ordinal, false)]
        [InlineData("", "", StringComparison.Ordinal, true)]
        [InlineData("", "hello", StringComparison.Ordinal, false)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz", StringComparison.Ordinal, true)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwx", StringComparison.Ordinal, true)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklm", StringComparison.Ordinal, true)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "ab_defghijklmnopqrstu", StringComparison.Ordinal, false)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdef_hijklmn", StringComparison.Ordinal, false)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghij_lmn", StringComparison.Ordinal, false)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "a", StringComparison.Ordinal, true)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyza", StringComparison.Ordinal, false)]
        // OrdinalIgnoreCase
        [InlineData("Hello", "Hel", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Hello Larger", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "HEL", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Abc", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hel", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("", "hello", StringComparison.OrdinalIgnoreCase, false)]
        public static void StartsWith(string s, string value, StringComparison comparisonType, bool expected)
        {
            if (comparisonType == StringComparison.CurrentCulture)
            {
                Assert.Equal(expected, s.StartsWith(value));
            }
            Assert.Equal(expected, s.AsSpan().StartsWith(value.AsSpan(), comparisonType));
        }

        [Theory]
        [ActiveIssue("https://github.com/dotnet/coreclr/issues/2051", TestPlatforms.AnyUnix)]
        [InlineData(StringComparison.CurrentCulture)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.Ordinal)]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        public static void StartsWith_NullInStrings(StringComparison comparison)
        {
            Assert.False("\0test".AsSpan().StartsWith("test".AsSpan(), comparison));
            Assert.False("te\0st".AsSpan().StartsWith("test".AsSpan(), comparison));
            Assert.True("te\0st".AsSpan().StartsWith("te\0s".AsSpan(), comparison));
            Assert.True("test\0".AsSpan().StartsWith("test".AsSpan(), comparison));
            Assert.False("test".AsSpan().StartsWith("te\0".AsSpan(), comparison));
        }

        [Theory]
        [InlineData("Hello", 0, 5, "Hello")]
        [InlineData("Hello", 0, 3, "Hel")]
        [InlineData("Hello", 2, 3, "llo")]
        [InlineData("Hello", 5, 0, "")]
        [InlineData("", 0, 0, "")]
        public static void Substring(string s, int startIndex, int length, string expected)
        {
            if (startIndex + length == s.Length)
            {
                Assert.Equal(expected, s.Substring(startIndex));
                Assert.Equal(expected, s.AsSpan(startIndex).ToString());
            }
            Assert.Equal(expected, s.AsSpan(startIndex, length).ToString());
        }

        [Fact]
        public static void ToLower()
        {
            var expectedSource = new char[3] { 'a', 'B', 'c' };
            var expectedDestination = new char[3] { 'a', 'b', 'c' };

            {
                ReadOnlySpan<char> source = new char[3] { 'a', 'B', 'c' };
                Span<char> destination = new char[3] { 'x', 'Y', 'z' };

                Assert.Equal(source.Length, source.ToLower(destination, CultureInfo.CurrentCulture));
                Assert.Equal(expectedDestination, destination.ToArray());
                Assert.Equal(expectedSource, source.ToArray());
            }

            {
                ReadOnlySpan<char> source = new char[3] { 'a', 'B', 'c' };
                Span<char> destination = new char[3] { 'x', 'Y', 'z' };

                Assert.Equal(source.Length, source.ToLowerInvariant(destination));
                Assert.Equal(expectedDestination, destination.ToArray());
                Assert.Equal(expectedSource, source.ToArray());
            }
        }

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("HELLO", "hello")]
        [InlineData("hElLo", "hello")]
        [InlineData("HeLlO", "hello")]
        [InlineData("", "")]
        public static void ToLower(string s, string expected)
        {
            Span<char> destination = new char[s.Length];
            Assert.Equal(s.Length, s.AsSpan().ToLower(destination, CultureInfo.CurrentCulture));
            Assert.Equal(expected, destination.ToString());
        }

        private static void ToLower_Culture(string input, string expected, CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            Span<char> destination = new char[input.Length];
            Assert.Equal(input.Length, input.AsSpan().ToLower(destination, culture));
            Assert.Equal(expected, destination.ToString());
        }

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("HELLO", "hello")]
        [InlineData("hElLo", "hello")]
        [InlineData("HeLlO", "hello")]
        [InlineData("", "")]
        public static void ToLowerInvariant(string s, string expected)
        {
            Span<char> destination = new char[s.Length];
            Assert.Equal(s.Length, s.AsSpan().ToLowerInvariant(destination));
            Assert.Equal(expected, destination.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData("hello")]
        public static void ToString(string s)
        {
            Assert.Equal(s, s.AsSpan().ToString());
        }

        [Fact]
        public static void ToUpper()
        {
            var expectedSource = new char[3] { 'a', 'B', 'c' };
            var expectedDestination = new char[3] { 'A', 'B', 'C' };

            {
                ReadOnlySpan<char> source = new char[3] { 'a', 'B', 'c' };
                Span<char> destination = new char[3] { 'x', 'Y', 'z' };

                Assert.Equal(source.Length, source.ToUpper(destination, CultureInfo.CurrentCulture));
                Assert.Equal(expectedDestination, destination.ToArray());
                Assert.Equal(expectedSource, source.ToArray());
            }

            {
                ReadOnlySpan<char> source = new char[3] { 'a', 'B', 'c' };
                Span<char> destination = new char[3] { 'x', 'Y', 'z' };

                Assert.Equal(source.Length, source.ToUpperInvariant(destination));
                Assert.Equal(expectedDestination, destination.ToArray());
                Assert.Equal(expectedSource, source.ToArray());
            }
        }

        [Theory]
        [InlineData("hello", "HELLO")]
        [InlineData("HELLO", "HELLO")]
        [InlineData("hElLo", "HELLO")]
        [InlineData("HeLlO", "HELLO")]
        [InlineData("", "")]
        public static void ToUpper(string s, string expected)
        {
            Span<char> destination = new char[s.Length];
            Assert.Equal(s.Length, s.AsSpan().ToUpper(destination, CultureInfo.CurrentCulture));
            Assert.Equal(expected, destination.ToString());
        }

        private static IEnumerable<object[]> ToUpper_TurkishI_MemberData(
            params KeyValuePair<char, char>[] mappings)
        {
            foreach (KeyValuePair<char, char> mapping in mappings)
            {
                yield return new[] { $"{mapping.Key}", $"{mapping.Value}" };
                yield return new[] { $"{mapping.Key}a TeSt", $"{mapping.Value}A TEST" };
                yield return new[] { $"a T{mapping.Key}est", $"A T{mapping.Value}EST" };
                yield return new[] { $"A test{mapping.Key}", $"A TEST{mapping.Value}" };
                yield return new[] { new string(mapping.Key, 100), new string(mapping.Value, 100) };
            }
        }

        public static IEnumerable<object[]> ToUpper_TurkishI_TurkishCulture_MemberData() =>
          ToUpper_TurkishI_MemberData(
              new KeyValuePair<char, char>('\u0069', '\u0130'),
              new KeyValuePair<char, char>('\u0130', '\u0130'),
              new KeyValuePair<char, char>('\u0131', '\u0049'));

        [Theory]
        [MemberData(nameof(ToUpper_TurkishI_TurkishCulture_MemberData))]
        public static void ToUpper_TurkishI_TurkishCulture(string s, string expected)
        {
            RemoteInvoke((str, expectedString) =>
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

                Span<char> destination = new char[str.Length];
                Assert.Equal(str.Length, str.AsSpan().ToUpper(destination, CultureInfo.CurrentCulture));
                Assert.Equal(expectedString, destination.ToString());

                return SuccessExitCode;
            }, s.ToString(), expected.ToString()).Dispose();
        }

        public static IEnumerable<object[]> ToUpper_TurkishI_EnglishUSCulture_MemberData() =>
           ToUpper_TurkishI_MemberData(
               new KeyValuePair<char, char>('\u0069', '\u0049'),
               new KeyValuePair<char, char>('\u0130', '\u0130'),
               new KeyValuePair<char, char>('\u0131', '\u0049'));

        [Theory]
        [MemberData(nameof(ToUpper_TurkishI_EnglishUSCulture_MemberData))]
        public static void ToUpper_TurkishI_EnglishUSCulture(string s, string expected)
        {
            RemoteInvoke((str, expectedString) =>
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");

                Span<char> destination = new char[str.Length];
                Assert.Equal(str.Length, str.AsSpan().ToUpper(destination, CultureInfo.CurrentCulture));
                Assert.Equal(expectedString, destination.ToString());

                return SuccessExitCode;
            }, s.ToString(), expected.ToString()).Dispose();
        }

        public static IEnumerable<object[]> ToUpper_TurkishI_InvariantCulture_MemberData() =>
        ToUpper_TurkishI_MemberData(
            new KeyValuePair<char, char>('\u0069', '\u0049'),
            new KeyValuePair<char, char>('\u0130', '\u0130'),
            new KeyValuePair<char, char>('\u0131', '\u0131'));

        [Theory]
        [MemberData(nameof(ToUpper_TurkishI_InvariantCulture_MemberData))]
        public static void ToUpper_TurkishI_InvariantCulture(string s, string expected)
        {
            RemoteInvoke((str, expectedString) =>
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                Span<char> destination = new char[str.Length];
                Assert.Equal(str.Length, str.AsSpan().ToUpper(destination, CultureInfo.CurrentCulture));
                Assert.Equal(expectedString, destination.ToString());

                return SuccessExitCode;
            }, s.ToString(), expected.ToString()).Dispose();
        }

        [Theory]
        [InlineData("hello", "HELLO")]
        [InlineData("HELLO", "HELLO")]
        [InlineData("hElLo", "HELLO")]
        [InlineData("HeLlO", "HELLO")]
        [InlineData("", "")]
        public static void ToUpperInvariant(string s, string expected)
        {
            Span<char> destination = new char[s.Length];
            Assert.Equal(s.Length, s.AsSpan().ToUpperInvariant(destination));
            Assert.Equal(expected, destination.ToString());
        }

        [Fact]
        public static void ToLowerToUpperInvariant_ASCII()
        {
            var asciiChars = new char[128];
            var asciiCharsUpper = new char[128];
            var asciiCharsLower = new char[128];

            for (int i = 0; i < asciiChars.Length; i++)
            {
                char c = (char)i;
                asciiChars[i] = c;

                // Purposefully avoiding char.ToUpper/ToLower here so as not  to use the same thing we're testing.
                asciiCharsLower[i] = (c >= 'A' && c <= 'Z') ? (char)(c - 'A' + 'a') : c;
                asciiCharsUpper[i] = (c >= 'a' && c <= 'z') ? (char)(c - 'a' + 'A') : c;
            }

            var ascii = new string(asciiChars);

            Span<char> destinationLower = new char[ascii.Length];
            Span<char> destinationUpper = new char[ascii.Length];

            Assert.Equal(ascii.Length, ascii.AsSpan().ToLowerInvariant(destinationLower));
            Assert.Equal(ascii.Length, ascii.AsSpan().ToUpperInvariant(destinationUpper));

            Assert.Equal(ascii.ToLowerInvariant(), destinationLower.ToString());
            Assert.Equal(ascii.ToUpperInvariant(), destinationUpper.ToString());

            Assert.Equal(ascii, ascii.AsSpan().ToString());
        }

        [Theory]
        [InlineData("  Hello  ", new char[] { ' ' }, "Hello")]
        [InlineData(".  Hello  ..", new char[] { '.' }, "  Hello  ")]
        [InlineData(".  Hello  ..", new char[] { '.', ' ' }, "Hello")]
        [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "Hello")]
        [InlineData("  Hello  ", null, "Hello")]
        [InlineData("  Hello  ", new char[0], "Hello")]
        [InlineData("      \t      ", null, "")]
        [InlineData("", null, "")]
        public static void Trim(string s, char[] trimChars, string expected)
        {
            if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
            {
                Assert.Equal(expected, s.AsSpan().Trim().ToString());
            }

            if (trimChars?.Length == 1)
            {
                Assert.Equal(expected, s.AsSpan().Trim(trimChars[0]).ToString());
            }

            Assert.Equal(expected, s.AsSpan().Trim(trimChars).ToString());
        }

        [Theory]
        [InlineData("  Hello  ", new char[] { ' ' }, "  Hello")]
        [InlineData(".  Hello  ..", new char[] { '.' }, ".  Hello  ")]
        [InlineData(".  Hello  ..", new char[] { '.', ' ' }, ".  Hello")]
        [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "123abcHello")]
        [InlineData("  Hello  ", null, "  Hello")]
        [InlineData("  Hello  ", new char[0], "  Hello")]
        [InlineData("      \t      ", null, "")]
        [InlineData("", null, "")]
        public static void TrimEnd(string s, char[] trimChars, string expected)
        {
            if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
            {
                Assert.Equal(expected, s.AsSpan().TrimEnd().ToString());
            }

            if (trimChars?.Length == 1)
            {
                Assert.Equal(expected, s.AsSpan().TrimEnd(trimChars[0]).ToString());
            }

            Assert.Equal(expected, s.AsSpan().TrimEnd(trimChars).ToString());
        }

        [Theory]
        [InlineData("  Hello  ", new char[] { ' ' }, "Hello  ")]
        [InlineData(".  Hello  ..", new char[] { '.' }, "  Hello  ..")]
        [InlineData(".  Hello  ..", new char[] { '.', ' ' }, "Hello  ..")]
        [InlineData("123abcHello123abc", new char[] { '1', '2', '3', 'a', 'b', 'c' }, "Hello123abc")]
        [InlineData("  Hello  ", null, "Hello  ")]
        [InlineData("  Hello  ", new char[0], "Hello  ")]
        [InlineData("      \t      ", null, "")]
        [InlineData("", null, "")]
        public static void TrimStart(string s, char[] trimChars, string expected)
        {
            if (trimChars == null || trimChars.Length == 0 || (trimChars.Length == 1 && trimChars[0] == ' '))
            {
                Assert.Equal(expected, s.AsSpan().TrimStart().ToString());
            }

            if (trimChars?.Length == 1)
            {
                Assert.Equal(expected, s.AsSpan().TrimStart(trimChars[0]).ToString());
            }

            Assert.Equal(expected, s.AsSpan().TrimStart(trimChars).ToString());
        }

        public static IEnumerable<object[]> UpperLowerCasing_TestData()
        {
            //                          lower                upper          Culture
            yield return new object[] { "abcd", "ABCD", "en-US" };
            yield return new object[] { "latin i", "LATIN I", "en-US" };
            yield return new object[] { "turky \u0131", "TURKY I", "tr-TR" };
            yield return new object[] { "turky i", "TURKY \u0130", "tr-TR" };
            yield return new object[] { "\ud801\udc29", PlatformDetection.IsWindows7 ? "\ud801\udc29" : "\ud801\udc01", "en-US" };
        }

        [Theory]
        [MemberData(nameof(UpperLowerCasing_TestData))]
        public static void CasingTest(string lowerForm, string upperForm, string cultureName)
        {
            CultureInfo ci = CultureInfo.GetCultureInfo(cultureName);

            Span<char> destinationLower = new char[upperForm.Length];
            Span<char> destinationUpper = new char[lowerForm.Length];

            Assert.Equal(upperForm.Length, upperForm.AsSpan().ToLower(destinationLower, ci));
            Assert.Equal(lowerForm.Length, lowerForm.AsSpan().ToUpper(destinationUpper, ci));

            Assert.Equal(lowerForm, lowerForm.AsSpan().ToString());
            Assert.Equal(upperForm, upperForm.AsSpan().ToString());
        }

    }
}
