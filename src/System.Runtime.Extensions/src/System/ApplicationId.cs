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
        private readonly byte[] _publicKeyToken;

        public ApplicationId(byte[] publicKeyToken, string name, Version version, string processorArchitecture, string culture)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length == 0) throw new ArgumentException(SR.Argument_EmptyApplicationName);
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (publicKeyToken == null) throw new ArgumentNullException(nameof(publicKeyToken));

            _publicKeyToken = (byte[])publicKeyToken.Clone();
            Name = name;
            Version = version;
            ProcessorArchitecture = processorArchitecture;
            Culture = culture;
        }

        public string Culture { get; }

        public string Name { get; }

        public string ProcessorArchitecture { get; }

        public Version Version { get; }

        public byte[] PublicKeyToken => (byte[])_publicKeyToken.Clone();

        public ApplicationId Copy() => new ApplicationId(_publicKeyToken, Name, Version, ProcessorArchitecture, Culture);

        private static readonly int constLength = nameof(Culture).Length + 5 + 
                                                  nameof(Version).Length + 5 + 16 +
                                                  nameof(_publicKeyToken).Length + 5 +
                                                  nameof(ProcessorArchitecture).Length + 5;

        public override string ToString ()
        {
            int variableLength = Name.Length + (Culture?.Length ?? 0) + _publicKeyToken.Length * 2 +
                                 ProcessorArchitecture.Length;
            Span<char> charSpan = stackalloc char[constLength + variableLength];
            var sb = new ValueStringBuilder(charSpan);
            sb.Append(Name);
            if (Culture != null)
            {
                sb.Append(", culture=\"");
                sb.Append(Culture);
                sb.Append('"');
            }
            sb.Append(", version=\"");
            sb.Append(Version.ToString());
            sb.Append('"');
            if (_publicKeyToken != null)
            {
                sb.Append(", publicKeyToken=\"");
                sb.Append(EncodeHexString(_publicKeyToken));
                sb.Append('"');
            }
            if (ProcessorArchitecture != null)
            {
                sb.Append(", processorArchitecture =\"");
                sb.Append(ProcessorArchitecture);
                sb.Append('"');
            }
            return sb.ToString();
        }

        private static char HexDigit(int num) =>
            (char)((num < 10) ? (num + '0') : (num + ('A' - 10)));
        
        private static string EncodeHexString(byte[] sArray) 
        {
            if (sArray == null) return null;

            Span<char> hexOrder = stackalloc char[sArray.Length * 2];

            for(int i = 0, j = 0; i < sArray.Length; i++) {
                int digit = (int)((sArray[i] & 0xf0) >> 4);
                hexOrder[j++] = HexDigit(digit);

                digit = (int)(sArray[i] & 0x0f);
                hexOrder[j++] = HexDigit(digit);
            }
            return new string(hexOrder);
        }
 
        public override bool Equals (object o)
        {
            ApplicationId other = (o as ApplicationId);
            if (other == null)
                return false;
 
            if (!(Equals(Name, other.Name) &&
                  Equals(Version, other.Version) &&
                  Equals(ProcessorArchitecture, other.ProcessorArchitecture) &&
                  Equals(Culture, other.Culture)))
                return false;
 
            if (_publicKeyToken.Length != other._publicKeyToken.Length)
                return false;
 
            for (int i = 0; i < _publicKeyToken.Length; i++)
            {
                if (_publicKeyToken[i] != other._publicKeyToken[i])
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
