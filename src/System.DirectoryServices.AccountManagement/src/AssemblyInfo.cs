// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;

// We need this attributes to match netfx System.DirectoryServices.AccountManagement assembly attributes, if we don't do that we would get: 
// TypeLoadException: Inheritance security rules violated while overriding member: {member} Security accessibility of the overriding method must match the security accessibility of the method being overriden.
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]