// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Policy;
using System.Collections;

namespace System.Security
{
    public static partial class SecurityManager
    {
        [ObsoleteAttribute]
        public static bool CheckExecutionRights { get; set; }
        [ObsoleteAttribute]
        public static bool SecurityEnabled { get; set; }
        public static bool CurrentThreadRequiresSecurityContextCapture() { return false; }
        public static PermissionSet GetStandardSandbox(Evidence evidence) { return default(PermissionSet); }
        public static void GetZoneAndOrigin(out System.Collections.ArrayList zone, out System.Collections.ArrayList origin) { zone = default(System.Collections.ArrayList); origin = default(System.Collections.ArrayList); }
        [ObsoleteAttribute]
        public static bool IsGranted(IPermission perm) { return false; }
        [ObsoleteAttribute]
        public static PolicyLevel LoadPolicyLevelFromFile(string path, PolicyLevelType type) { return default(PolicyLevel); }
        [ObsoleteAttribute]
        public static PolicyLevel LoadPolicyLevelFromString(string str, PolicyLevelType type) { return default(PolicyLevel); }
        [ObsoleteAttribute]
        public static IEnumerator PolicyHierarchy() { return default(IEnumerator); }
        [ObsoleteAttribute]
        public static PermissionSet ResolvePolicy(Evidence evidence) { return default(PermissionSet); }
        [ObsoleteAttribute]
        public static PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied) { return default(PermissionSet); }
        [ObsoleteAttribute]
        public static PermissionSet ResolvePolicy(System.Security.Policy.Evidence[] evidences) { return default(PermissionSet); }
        [ObsoleteAttribute]
        public static IEnumerator ResolvePolicyGroups(System.Security.Policy.Evidence evidence) { return default(IEnumerator); }
        [ObsoleteAttribute]
        public static PermissionSet ResolveSystemPolicy(System.Security.Policy.Evidence evidence) { return default(PermissionSet); }
        [ObsoleteAttribute]
        public static void SavePolicy() { }
        [ObsoleteAttribute]
        public static void SavePolicyLevel(PolicyLevel level) { }
    }
}
