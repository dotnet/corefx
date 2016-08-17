﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security
{
    public static partial class SecurityManager
    {
        [System.ObsoleteAttribute]
        public static bool CheckExecutionRights { get; set; }
        [System.ObsoleteAttribute("The security manager cannot be turned off on MS runtime")]
        public static bool SecurityEnabled { get; set; }
        public static void GetZoneAndOrigin(out System.Collections.ArrayList zone, out System.Collections.ArrayList origin) { zone = default(System.Collections.ArrayList); origin = default(System.Collections.ArrayList); }
        [System.ObsoleteAttribute]
        public static bool IsGranted(System.Security.IPermission perm) { return default(bool); }
        [System.ObsoleteAttribute]
        public static System.Security.Policy.PolicyLevel LoadPolicyLevelFromFile(string path, System.Security.PolicyLevelType type) { return default(System.Security.Policy.PolicyLevel); }
        [System.ObsoleteAttribute]
        public static System.Security.Policy.PolicyLevel LoadPolicyLevelFromString(string str, System.Security.PolicyLevelType type) { return default(System.Security.Policy.PolicyLevel); }
        [System.ObsoleteAttribute]
        public static System.Collections.IEnumerator PolicyHierarchy() { return default(System.Collections.IEnumerator); }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence) { return default(System.Security.PermissionSet); }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence, System.Security.PermissionSet reqdPset, System.Security.PermissionSet optPset, System.Security.PermissionSet denyPset, out System.Security.PermissionSet denied) { denied = default(System.Security.PermissionSet); return default(System.Security.PermissionSet); }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence[] evidences) { return default(System.Security.PermissionSet); }
        [System.ObsoleteAttribute]
        public static System.Collections.IEnumerator ResolvePolicyGroups(System.Security.Policy.Evidence evidence) { return default(System.Collections.IEnumerator); }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolveSystemPolicy(System.Security.Policy.Evidence evidence) { return default(System.Security.PermissionSet); }
        [System.ObsoleteAttribute]
        public static void SavePolicy() { }
        [System.ObsoleteAttribute]
        public static void SavePolicyLevel(System.Security.Policy.PolicyLevel level) { }
    }
}
