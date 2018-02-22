
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices
{
    public readonly struct OSPlatform : IEquatable<OSPlatform>
    {
        private readonly string _osPlatform;

        public static OSPlatform Android { get; } = new OSPlatform("ANDROID");

        public static OSPlatform iOS { get; } = new OSPlatform("IOS");

	// Returns true for Linux, Tizen and Android systems
        public static OSPlatform Linux { get; } = new OSPlatform("LINUX");

        public static OSPlatform macOS { get; } = new OSPlatform("MACOS");

        public static OSPlatform N3DS { get; } = new OSPlatform("3DS");
	
	// This one has historically been used as "Apple platforms", so it returns true
	// on iOS, tvOS, macOS, watchOS.
        public static OSPlatform OSX { get; } = new OSPlatform("OSX");

        public static OSPlatform PlayStation4 { get; } = new OSPlatform("PS4");

        public static OSPlatform PlayStationPortable2 { get; } = new OSPlatform("PSP2");

        public static OSPlatform PlayStationVita { get; } = new OSPlatform("PSVITA");

        public static OSPlatform Switch { get; } = new OSPlatform("SWITCH");
	
        public static OSPlatform Tizen { get; } = new OSPlatform("TIZEN");

        public static OSPlatform tvOS { get; } = new OSPlatform("TVOS");

        public static OSPlatform watchOS { get; } = new OSPlatform("WATCHOS");

        public static OSPlatform WebAssembly { get; } = new OSPlatform("WEBASSEMBLY");

        public static OSPlatform WiiU { get; } = new OSPlatform("WIIU");

        public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");

        public static OSPlatform XboxOne { get; } = new OSPlatform("XBOXONE");

        private OSPlatform(string osPlatform)
        {
            if (osPlatform == null) throw new ArgumentNullException(nameof(osPlatform));
            if (osPlatform.Length == 0) throw new ArgumentException(SR.Argument_EmptyValue, nameof(osPlatform));

            _osPlatform = osPlatform;
        }

        public static OSPlatform Create(string osPlatform)
        {
            return new OSPlatform(osPlatform);
        }

        public bool Equals(OSPlatform other)
        {
            return Equals(other._osPlatform);
        }

        internal bool Equals(string other)
        {
            return string.Equals(_osPlatform, other, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is OSPlatform && Equals((OSPlatform)obj);
        }

        public override int GetHashCode()
        {
            return _osPlatform == null ? 0 : _osPlatform.GetHashCode();
        }

        public override string ToString()
        {
            return _osPlatform ?? string.Empty;
        }

        public static bool operator ==(OSPlatform left, OSPlatform right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OSPlatform left, OSPlatform right)
        {
            return !(left == right);
        }
    }
}
