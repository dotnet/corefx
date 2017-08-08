// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Syntax
{
    internal enum PredefinedName
    {
        PN_CTOR,
        PN_PTR,
        PN_NUB,
        PN_OUTPARAM,
        PN_REFPARAM,
        PN_ARRAY0,
        PN_ARRAY1,
        PN_ARRAY2,
        PN_INVOKE,
        PN_INDEXERINTERNAL,
        PN_CAP_VALUE,

        PN_ADDEVENTHANDLER,
        PN_REMOVEEVENTHANDLER,
        PN_INVOCATIONLIST,
        PN_GETORCREATEEVENTREGISTRATIONTOKENTABLE,

        PN_COUNT,  // Not a name, this is the total count of predefined names
    }
}
