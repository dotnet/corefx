// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Security
{
    // Stub SecurityException
    // This will not de/serialize correctly
    public class SecurityException : Exception
    {
        private const int COR_E_SECURITY = unchecked((int)0x8013150A);

        public SecurityException()
        {
            HResult = COR_E_SECURITY;
        }

        public SecurityException(string message)
            : base(message)
        {
            HResult = COR_E_SECURITY;
        }

        public SecurityException(string message, System.Exception inner) 
            : base(message, inner)
        {
            HResult = COR_E_SECURITY;
        }

        public SecurityException(String message, Type type)
            : base(message)
        {
            HResult = COR_E_SECURITY;
            PermissionType = type;
        }

        public SecurityException(string message, System.Type type, string state)
            : base(message)
        {
            HResult = COR_E_SECURITY;
            PermissionType = type;
            PermissionState = state;
        }

        protected SecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }

        public object Demanded { get; set; }
        public object DenySetInstance { get; set; }
        public System.Reflection.AssemblyName FailedAssemblyInfo { get; set; }
        public string GrantedSet { get; set; }
        public System.Reflection.MethodInfo Method { get; set; }
        public string PermissionState { get; set; }
        public System.Type PermissionType { get; set; }
        public object PermitOnlySetInstance { get; set; }
        public string RefusedSet { get; set; }
        public string Url { get; set; }
    }
}
