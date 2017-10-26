// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;

// We need this attributes because currently for testing we use the netstandard produced assembly as this is an inbox assembly with the same identity in full framework. Once we fix our infra to pickup the inbox assembly we need to delete this.
// Since we have security attributes in the NS implementation, without this we would get an exception like the following:
// TypeLoadException: Inheritance security rules violated while overriding member: {member} Security accessibility of the overriding method must match the security accessibility of the method being overriden.
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]