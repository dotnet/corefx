// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace Microsoft.CSharp.RuntimeBinder.Errors
{
    ////////////////////////////////////////////////////////////////////////////////
    // CError
    //
    // This object is the implementation of ICSError for all compiler errors,
    // including lexer, parser, and compiler errors.

    internal sealed class CError
    {
        public CError(ErrorCode code, string[] args)
        {
            Text = string.Format(CultureInfo.InvariantCulture, ErrorFacts.GetMessage(code), args);
        }

        public string Text { get; }

        public void Submit()
        {
            throw new RuntimeBinderException(Text);
        }
    }
}
