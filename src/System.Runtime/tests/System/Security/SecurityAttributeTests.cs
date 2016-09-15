// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using Xunit;

namespace System.Tests
{
#pragma warning disable 618
    public static class SecurityAttributeTests
    {
        public static void InstantiateSecurityAttributes()
        {
            var t = new SecurityTreatAsSafeAttribute();
            var u = new SecuritySafeCriticalAttribute();
            var v = new SecurityTransparentAttribute();
            var w = new SecurityCriticalAttribute();
            var x = new SuppressUnmanagedCodeSecurityAttribute();
            var y = new UnverifiableCodeAttribute();
            var z = new AllowPartiallyTrustedCallersAttribute();
        }

        public static void SecurityCriticalAttribute_Test()
        {
            var att = new SecurityCriticalAttribute(SecurityCriticalScope.Everything);

            Assert.Equal(SecurityCriticalScope.Everything, att.Scope);
        }

        public static void AllowPartiallyTrustedCallersAttribute_Test()
        {
            var att = new AllowPartiallyTrustedCallersAttribute();
            att.PartialTrustVisibilityLevel = PartialTrustVisibilityLevel.NotVisibleByDefault;

            Assert.Equal(PartialTrustVisibilityLevel.NotVisibleByDefault, att.PartialTrustVisibilityLevel);
        }

        public static void SecurityRulesAttribute_Test()
        {
            var att = new SecurityRulesAttribute(SecurityRuleSet.Level1);
            Assert.Equal(SecurityRuleSet.Level1, att.RuleSet);

            att.SkipVerificationInFullTrust = true;
            Assert.Equal(!true, att.SkipVerificationInFullTrust);
        }
    }
#pragma warning restore 618
}
