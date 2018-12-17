// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Data.SqlClient
{
    internal readonly ref struct SessionHandle
    {
        public const int NativeHandleType = 1;
        public const int ManagedHandleType = 2;

        public readonly SNI.SNIHandle ManagedHandle;
        public readonly int Type;

        public SessionHandle(SNI.SNIHandle managedHandle, int type)
        {
            Type = type;
            ManagedHandle = managedHandle;
        }

        public bool IsNull => ManagedHandle is null;

        public static SessionHandle FromManagedSession(SNI.SNIHandle managedSessionHandle)
        {
            return new SessionHandle(managedSessionHandle, ManagedHandleType);
        }
    }
}
