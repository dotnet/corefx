// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Text;
using System.Security.Principal;

namespace System.DirectoryServices.AccountManagement
{
    internal class SAMUtils
    {
        // To stop the compiler from autogenerating a constructor for this class
        private SAMUtils() { }

        static internal bool IsOfObjectClass(DirectoryEntry de, string classToCompare)
        {
            return string.Equals(de.SchemaClassName, classToCompare, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool GetOSVersion(DirectoryEntry computerDE, out int versionMajor, out int versionMinor)
        {
            Debug.Assert(SAMUtils.IsOfObjectClass(computerDE, "Computer"));

            versionMajor = 0;
            versionMinor = 0;

            string version = null;

            try
            {
                if (computerDE.Properties["OperatingSystemVersion"].Count > 0)
                {
                    Debug.Assert(computerDE.Properties["OperatingSystemVersion"].Count == 1);

                    version = (string)computerDE.Properties["OperatingSystemVersion"].Value;
                }
            }
            catch (COMException e)
            {
                GlobalDebug.WriteLineIf(GlobalDebug.Error, "SAMUtils", "GetOSVersion: caught COMException with message " + e.Message);

                // Couldn't retrieve the value
                if (e.ErrorCode == unchecked((int)0x80070035))  // ERROR_BAD_NETPATH
                    return false;

                throw;
            }

            // Couldn't retrieve the value
            if (version == null || version.Length == 0)
                return false;

            // This string should be in the form "M.N", where M and N are integers.
            // We'll also accept "M", which we'll treat as "M.0".
            //
            // We'll split the string into its period-separated components, and parse
            // each component into an int.
            string[] versionComponents = version.Split(new char[] { '.' });

            Debug.Assert(versionComponents.Length >= 1);    // since version was a non-empty string

            try
            {
                versionMajor = int.Parse(versionComponents[0], CultureInfo.InvariantCulture);

                if (versionComponents.Length > 1)
                    versionMinor = int.Parse(versionComponents[1], CultureInfo.InvariantCulture);

                // Sanity check: there are no negetive OS versions, nor is there a version "0".
                if (versionMajor <= 0 || versionMinor < 0)
                {
                    Debug.Fail(string.Format(
                                    CultureInfo.CurrentCulture,
                                    "SAMUtils.GetOSVersion: {0} claims to have negetive OS version, {1}",
                                    computerDE.Path,
                                    version));

                    return false;
                }
            }
            catch (FormatException)
            {
                Debug.Fail(string.Format(
                                CultureInfo.CurrentCulture,
                                "SAMUtils.GetOSVersion: FormatException on {0} for {1}",
                                version,
                                computerDE.Path));

                return false;
            }
            catch (OverflowException)
            {
                Debug.Fail(string.Format(
                                CultureInfo.CurrentCulture,
                                "SAMUtils.GetOSVersion: OverflowException on {0} for {1}",
                                version,
                                computerDE.Path));
                return false;
            }

            return true;
        }

        static internal Principal DirectoryEntryAsPrincipal(DirectoryEntry de, StoreCtx storeCtx)
        {
            string className = de.SchemaClassName;

            // Unlike AD, we don't have to worry about cross-store refs here.  In AD, if there's
            // a cross-store ref, we'll get back a DirectoryEntry of the FPO object.  In the WinNT ADSI
            // provider, we'll get back the DirectoryEntry of the remote object itself --- ADSI does
            // the domain vs. local resolution for us.

            if (SAMUtils.IsOfObjectClass(de, "Computer") ||
                 SAMUtils.IsOfObjectClass(de, "User") ||
                 SAMUtils.IsOfObjectClass(de, "Group"))
            {
                return storeCtx.GetAsPrincipal(de, null);
            }
            else
            {
                Debug.Fail(string.Format(
                                CultureInfo.CurrentCulture,
                                "SAMUtils.DirectoryEntryAsPrincipal: fell off end, Path={0}, SchemaClassName={1}",
                                de.Path,
                                de.SchemaClassName));
                return null;
            }
        }

        //  These are verbatim C# string ( @ ) where \ is actually \\
        // Input            Matches                 RegEx
        // -----            -------                -----
        //   *                any ( 1 or more )     .*
        //
        //   \*                *                           \*
        //   \                 \                            \\
        //   (                 (                            \(
        //   \(                (                            \(
        //   )                 )                            \)
        //   \)                )                            \)
        //   \\                \                             \\
        //   x                 x                             x      (where x is anything else)
        // Add \G to beginning and \z to the end so that the regex will be anchored at the either end of the property
        // \G = Regex must match at the beginning
        // \z = Regex must match at the end
        // ( ) * are special characters to Regex so they must be escaped with \\.  We support these from teh user either raw or already escaped.
        // Any other \ in the input string are translated to an actual \ in the match because we cannot determine usage except for  ( ) *
        // The user cannot enter any regex escape sequence they would like in their match string.  Only * is supported.
        //
        //  @"c:\Home" -> "c:\\\\Home" OR @"c:\\Home"
        //
        //

        static internal string PAPIQueryToRegexString(string papiString)
        {
            StringBuilder sb = new StringBuilder(papiString.Length);

            sb.Append(@"\G");

            bool escapeMode = false;

            foreach (char c in papiString)
            {
                if (escapeMode == false)
                {
                    switch (c)
                    {
                        case '(':
                            sb.Append(@"\(");          //   (  --> \(
                            break;

                        case ')':
                            sb.Append(@"\)");          //   )  --> \)
                            break;

                        case '\\':
                            escapeMode = true;
                            break;

                        case '*':
                            sb.Append(@".*");          //   * --> .*
                            break;

                        default:
                            sb.Append(c.ToString());   //   x  --> x
                            break;
                    }
                }
                else
                {
                    escapeMode = false;

                    switch (c)
                    {
                        case '(':
                            sb.Append(@"\(");          //      \( --> \(
                            break;

                        case ')':
                            sb.Append(@"\)");          //      \) --> \)
                            break;

                        case '*':
                            sb.Append(@"\*");          //      \* --> \*
                            break;

                        case '\\':
                            sb.Append(@"\\\\");          //      \\ --> \\
                            break;

                        default:
                            sb.Append(@"\\");
                            sb.Append(c.ToString());    //      \x --> \x
                            break;
                    }
                }
            }

            // There was a '\\' but no character after it because we were at the 
            // end of the string.  
            // Append '\\\\' to match the '\\'.
            if (escapeMode)
            {
                sb.Append(@"\\");
            }

            GlobalDebug.WriteLineIf(
                            GlobalDebug.Info,
                            "SAMUtils",
                            "PAPIQueryToRegexString: mapped '{0}' to '{1}'",
                            papiString,
                            sb.ToString());

            sb.Append(@"\z");

            return sb.ToString();
        }
    }
}
