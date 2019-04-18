// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Text.Encodings.Web
{
    public abstract partial class HtmlEncoder : TextEncoder
    {
        // Disallow subclassing by types outside our implementation assembly
        internal HtmlEncoder() { }
    }
    public abstract partial class JavaScriptEncoder : TextEncoder
    {
        // Disallow subclassing by types outside our implementation assembly
        internal JavaScriptEncoder() { }
    }
    public abstract partial class TextEncoder
    {
        // Disallow subclassing by types outside our implementation assembly
        internal TextEncoder() { }
    }
    public abstract partial class UrlEncoder : TextEncoder
    {
        // Disallow subclassing by types outside our implementation assembly
        internal UrlEncoder() { }
    }
}
