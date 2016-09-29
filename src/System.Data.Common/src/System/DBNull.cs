// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;


namespace System
{
    public sealed class DBNull
    {
        private DBNull()
        {
        }


        public static readonly DBNull Value = new DBNull();


        public override String ToString()
        {
            return String.Empty;
        }

        public String ToString(IFormatProvider provider)
        {
            return String.Empty;
        }
    }
}

