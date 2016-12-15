// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;

namespace System.DirectoryServices.AccountManagement
{
    //
    // A collection of individual property filters
    //
    internal class QbeFilterDescription
    {
        private ArrayList _filtersToApply = new ArrayList();

        public QbeFilterDescription()
        {
            // Nothing to do
        }

        public ArrayList FiltersToApply
        {
            get
            {
                return _filtersToApply;
            }
        }
    }

    //
    // Constructs individual property filters, given the name of the property
    //
    internal class FilterFactory
    {
        // Put a private constructor because this class should only be used as static methods
        private FilterFactory() { }

        private static Hashtable s_subclasses = new Hashtable();

        static FilterFactory()
        {
            s_subclasses[DescriptionFilter.PropertyNameStatic] = typeof(DescriptionFilter);
            s_subclasses[DisplayNameFilter.PropertyNameStatic] = typeof(DisplayNameFilter);
            s_subclasses[IdentityClaimFilter.PropertyNameStatic] = typeof(IdentityClaimFilter);
            s_subclasses[SamAccountNameFilter.PropertyNameStatic] = typeof(SamAccountNameFilter);
            s_subclasses[DistinguishedNameFilter.PropertyNameStatic] = typeof(DistinguishedNameFilter);
            s_subclasses[GuidFilter.PropertyNameStatic] = typeof(GuidFilter);
            s_subclasses[UserPrincipalNameFilter.PropertyNameStatic] = typeof(UserPrincipalNameFilter);
            s_subclasses[StructuralObjectClassFilter.PropertyNameStatic] = typeof(StructuralObjectClassFilter);
            s_subclasses[NameFilter.PropertyNameStatic] = typeof(NameFilter);
            s_subclasses[CertificateFilter.PropertyNameStatic] = typeof(CertificateFilter);
            s_subclasses[AuthPrincEnabledFilter.PropertyNameStatic] = typeof(AuthPrincEnabledFilter);
            s_subclasses[PermittedWorkstationFilter.PropertyNameStatic] = typeof(PermittedWorkstationFilter);
            s_subclasses[PermittedLogonTimesFilter.PropertyNameStatic] = typeof(PermittedLogonTimesFilter);
            s_subclasses[ExpirationDateFilter.PropertyNameStatic] = typeof(ExpirationDateFilter);
            s_subclasses[SmartcardLogonRequiredFilter.PropertyNameStatic] = typeof(SmartcardLogonRequiredFilter);
            s_subclasses[DelegationPermittedFilter.PropertyNameStatic] = typeof(DelegationPermittedFilter);
            s_subclasses[HomeDirectoryFilter.PropertyNameStatic] = typeof(HomeDirectoryFilter);
            s_subclasses[HomeDriveFilter.PropertyNameStatic] = typeof(HomeDriveFilter);
            s_subclasses[ScriptPathFilter.PropertyNameStatic] = typeof(ScriptPathFilter);
            s_subclasses[PasswordNotRequiredFilter.PropertyNameStatic] = typeof(PasswordNotRequiredFilter);
            s_subclasses[PasswordNeverExpiresFilter.PropertyNameStatic] = typeof(PasswordNeverExpiresFilter);
            s_subclasses[CannotChangePasswordFilter.PropertyNameStatic] = typeof(CannotChangePasswordFilter);
            s_subclasses[AllowReversiblePasswordEncryptionFilter.PropertyNameStatic] = typeof(AllowReversiblePasswordEncryptionFilter);
            s_subclasses[GivenNameFilter.PropertyNameStatic] = typeof(GivenNameFilter);
            s_subclasses[MiddleNameFilter.PropertyNameStatic] = typeof(MiddleNameFilter);
            s_subclasses[SurnameFilter.PropertyNameStatic] = typeof(SurnameFilter);
            s_subclasses[EmailAddressFilter.PropertyNameStatic] = typeof(EmailAddressFilter);
            s_subclasses[VoiceTelephoneNumberFilter.PropertyNameStatic] = typeof(VoiceTelephoneNumberFilter);
            s_subclasses[EmployeeIDFilter.PropertyNameStatic] = typeof(EmployeeIDFilter);
            s_subclasses[GroupIsSecurityGroupFilter.PropertyNameStatic] = typeof(GroupIsSecurityGroupFilter);
            s_subclasses[GroupScopeFilter.PropertyNameStatic] = typeof(GroupScopeFilter);
            s_subclasses[ServicePrincipalNameFilter.PropertyNameStatic] = typeof(ServicePrincipalNameFilter);
            s_subclasses[ExtensionCacheFilter.PropertyNameStatic] = typeof(ExtensionCacheFilter);
            s_subclasses[BadPasswordAttemptFilter.PropertyNameStatic] = typeof(BadPasswordAttemptFilter);
            s_subclasses[ExpiredAccountFilter.PropertyNameStatic] = typeof(ExpiredAccountFilter);
            s_subclasses[LastLogonTimeFilter.PropertyNameStatic] = typeof(LastLogonTimeFilter);
            s_subclasses[LockoutTimeFilter.PropertyNameStatic] = typeof(LockoutTimeFilter);
            s_subclasses[PasswordSetTimeFilter.PropertyNameStatic] = typeof(PasswordSetTimeFilter);
            s_subclasses[BadLogonCountFilter.PropertyNameStatic] = typeof(BadLogonCountFilter);
        }

        // Given a property name, constructs and returns the appropriate individual property
        // filter
        static public object CreateFilter(string propertyName)
        {
            GlobalDebug.WriteLineIf(GlobalDebug.Info, "QbeFilterDescription", "FilterFactory.CreateFilter: name=" + propertyName);

            Type type = (Type)s_subclasses[propertyName];
            Debug.Assert(type != null);

            return System.Activator.CreateInstance(type);
        }
    }

    //
    // The individual property filters
    //

    // Base class: Defines the external interface shared by all individual property filters
    internal abstract class FilterBase
    {
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private object _value;

        // Some providers need place to store extra state, e.g., a processed form of Value
        public object Extra
        {
            get { return _extra; }
            set { _extra = value; }
        }

        private object _extra = null;

        public abstract string PropertyName { get; }
    }

    // The derived classes

    internal class DescriptionFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalDescription;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class SidFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalSid;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class SamAccountNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalSamAccountName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class DistinguishedNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalDistinguishedName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
    internal class GuidFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalGuid;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
    internal class IdentityClaimFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalIdentityClaims;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class UserPrincipalNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalUserPrincipalName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
    internal class StructuralObjectClassFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalStructuralObjectClass;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
    internal class NameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class DisplayNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalDisplayName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
    internal class CertificateFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AuthenticablePrincipalCertificates;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class AuthPrincEnabledFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AuthenticablePrincipalEnabled;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class PermittedWorkstationFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoPermittedWorkstations;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class PermittedLogonTimesFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoPermittedLogonTimes;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class ExpirationDateFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoExpirationDate;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class SmartcardLogonRequiredFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoSmartcardRequired;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class DelegationPermittedFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoDelegationPermitted;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class HomeDirectoryFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoHomeDirectory;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class HomeDriveFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoHomeDrive;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class ScriptPathFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoScriptPath;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class PasswordNotRequiredFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoPasswordNotRequired;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class PasswordNeverExpiresFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoPasswordNeverExpires;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class CannotChangePasswordFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoCannotChangePassword;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class AllowReversiblePasswordEncryptionFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoAllowReversiblePasswordEncryption;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class GivenNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserGivenName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class MiddleNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserMiddleName;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class SurnameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserSurname;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class EmailAddressFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserEmailAddress;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class VoiceTelephoneNumberFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserVoiceTelephoneNumber;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class EmployeeIDFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.UserEmployeeID;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class GroupIsSecurityGroupFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.GroupIsSecurityGroup;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class GroupScopeFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.GroupGroupScope;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class ServicePrincipalNameFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.ComputerServicePrincipalNames;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class ExtensionCacheFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PrincipalExtensionCache;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class BadPasswordAttemptFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoLastBadPasswordAttempt;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class LastLogonTimeFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoLastLogon;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class LockoutTimeFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoAcctLockoutTime;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class ExpiredAccountFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoExpiredAccount;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class PasswordSetTimeFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.PwdInfoLastPasswordSet;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }

    internal class BadLogonCountFilter : FilterBase
    {
        public const string PropertyNameStatic = PropertyNames.AcctInfoBadLogonCount;
        public override string PropertyName { get { return PropertyNameStatic; } }
    }
}
