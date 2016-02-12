// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection.Emit
{
    public partial class DynamicMethod : System.Reflection.MethodInfo
    {
        public override MethodImplAttributes MethodImplementationFlags { get { return default(MethodImplAttributes); } }
    }
}
