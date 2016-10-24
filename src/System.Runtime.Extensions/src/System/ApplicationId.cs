// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace System
{
    public sealed class ApplicationId
    {
        private byte[] m_publicKeyToken;

        public ApplicationId(byte[] publicKeyToken, string name, Version version, string processorArchitecture, string culture)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length == 0) throw new ArgumentException(SR.Argument_EmptyApplicationName);
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (publicKeyToken == null) throw new ArgumentNullException(nameof(publicKeyToken));

            m_publicKeyToken = new byte[publicKeyToken.Length];
            Array.Copy(publicKeyToken, m_publicKeyToken, publicKeyToken.Length);
            Name = name;
            Version = version;
            ProcessorArchitecture = processorArchitecture;
            Culture = culture;
        }

        public string Culture { get; }

        public string Name { get; }

        public string ProcessorArchitecture { get; }

        public Version Version { get; }

        public byte[] PublicKeyToken
        {
            get
            {
                var result = new byte[m_publicKeyToken.Length];
                Array.Copy(m_publicKeyToken, result, m_publicKeyToken.Length);
                return result;
            }
        }

        public ApplicationId Copy() => new ApplicationId(m_publicKeyToken, Name, Version, ProcessorArchitecture, Culture);

        public override string ToString ()
        {
            StringBuilder sb = StringBuilderCache.Acquire();
            sb.Append(this.Name);
            if (Culture != null)
            {
                sb.Append(", culture=\"");
                sb.Append(Culture);
                sb.Append("\"");
            }
            sb.Append(", version=\"");
            sb.Append(Version.ToString());
            sb.Append("\"");
            if (m_publicKeyToken != null) {
                sb.Append(", publicKeyToken=\"");
                sb.Append(EncodeHexString(m_publicKeyToken));
                sb.Append("\"");
            }
            if (ProcessorArchitecture != null)
            {
                sb.Append(", processorArchitecture =\"");
                sb.Append(this.ProcessorArchitecture);
                sb.Append("\"");
            }
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static char HexDigit(int num) =>
            (char)((num < 10) ? (num + '0') : (num + ('A' - 10)));
        
        private static String EncodeHexString(byte[] sArray) 
        {
            String result = null;
    
            if(sArray != null) {
                char[] hexOrder = new char[sArray.Length * 2];
            
                int digit;
                for(int i = 0, j = 0; i < sArray.Length; i++) {
                    digit = (int)((sArray[i] & 0xf0) >> 4);
                    hexOrder[j++] = HexDigit(digit);
                    digit = (int)(sArray[i] & 0x0f);
                    hexOrder[j++] = HexDigit(digit);
                }
                result = new String(hexOrder);
            }
            return result;
        }
 
        public override bool Equals (Object o)
        {
            ApplicationId other = (o as ApplicationId);
            if (other == null)
                return false;
 
            if (!(Equals(this.Name, other.Name) &&
                  Equals(this.Version, other.Version) &&
                  Equals(this.ProcessorArchitecture, other.ProcessorArchitecture) &&
                  Equals(this.Culture, other.Culture)))
                return false;
 
            if (this.m_publicKeyToken.Length != other.m_publicKeyToken.Length)
                return false;
 
            for (int i = 0; i < this.m_publicKeyToken.Length; ++i)
            {
                if (this.m_publicKeyToken[i] != other.m_publicKeyToken[i])
                    return false;
            }
 
            return true;
        }
 
        public override int GetHashCode()
        {
            // Note: purposely skipping publicKeyToken, processor architecture and culture as they
            // are less likely to make things not equal than name and version.
            return Name.GetHashCode() ^ Version.GetHashCode();
        }
    }
}
