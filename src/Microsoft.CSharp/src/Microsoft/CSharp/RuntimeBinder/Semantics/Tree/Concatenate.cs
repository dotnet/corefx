// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal class EXPRCONCAT : EXPR
    {
        public EXPR FirstArgument;
        public EXPR GetFirstArgument() { return FirstArgument; }
        public void SetFirstArgument(EXPR value) { FirstArgument = value; }
        public EXPR SecondArgument;
        public EXPR GetSecondArgument() { return SecondArgument; }
        public void SetSecondArgument(EXPR value) { SecondArgument = value; }
    }
}
