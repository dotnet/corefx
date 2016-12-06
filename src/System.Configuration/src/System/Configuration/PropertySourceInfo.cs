// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration.Internal;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using System.Security;

namespace System.Configuration
{
    internal class PropertySourceInfo
    {
        private readonly string _fileName;

        internal PropertySourceInfo(XmlReader reader)
        {
            _fileName = GetFilename(reader);
            LineNumber = GetLineNumber(reader);
        }

        internal string FileName
        {
            get
            {
                // Ensure we return the same string to the caller as the one on which we issued the demand.
                string filename = _fileName;
                try
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, filename).Demand();
                }
                catch (SecurityException)
                {
                    // don't expose the path to this user but show the filename
                    filename = Path.GetFileName(_fileName) ?? string.Empty;
                }

                return filename;
            }
        }

        internal int LineNumber { get; }

        private string GetFilename(XmlReader reader)
        {
            IConfigErrorInfo err = reader as IConfigErrorInfo;

            return err != null ? err.Filename : "";
        }

        private int GetLineNumber(XmlReader reader)
        {
            IConfigErrorInfo err = reader as IConfigErrorInfo;

            return err?.LineNumber ?? 0;
        }
    }
}