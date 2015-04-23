// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    public struct OSName : IEquatable<OSName>
    {
        private readonly string _name;

        private const string WindowsName = "WINDOWS";
        private const string LinuxName = "LINUX";
        private const string OSXName = "OSX";

        public OSName(string name)
        {
            _name = name;
        }

        public static OSName Windows { get { return new OSName(WindowsName); } }
        public static OSName Linux { get { return new OSName(LinuxName); } }
        public static OSName OSX { get { return new OSName(OSXName); } }

        public bool Equals(OSName osName)
        {
            return (string.IsNullOrEmpty(_name) && string.IsNullOrEmpty(osName._name)) ?
                true : osName._name == _name;
        }

        public override bool Equals(object obj)
        {
            if (obj is OSName)
            {
                return Equals((OSName)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _name == null ? string.Empty.GetHashCode() : _name.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }

        public static bool operator ==(OSName left, OSName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OSName left, OSName right)
        {
            return !(left == right);
        }
    }
}
