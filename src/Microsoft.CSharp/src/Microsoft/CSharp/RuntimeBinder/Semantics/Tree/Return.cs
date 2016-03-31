// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//

//

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal class EXPRRETURN : EXPRSTMT
    {
        // Return object is optional because of void returns.
        public EXPR OptionalObject;
        public EXPR GetOptionalObject() { return OptionalObject; }
        public void SetOptionalObject(EXPR value) { OptionalObject = value; }
    }
}
