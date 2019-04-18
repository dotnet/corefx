// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Text.Encodings.Web
{
    public abstract partial class TextEncoder
    {
        public abstract int EncodeSingleRune(System.Text.Rune value, System.Span<char> buffer);
        public abstract bool RuneMustBeEncoded(System.Text.Rune value);
    }
}
