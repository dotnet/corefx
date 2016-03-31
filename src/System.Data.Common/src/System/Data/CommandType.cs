// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



//------------------------------------------------------------------------------

namespace System.Data
{
    public enum CommandType
    {
        Text = 0x1,
        //Table           = 0x2,
        StoredProcedure = 0x4,
        //File            = 0x100,
        TableDirect = 0x200,
    }
}
