﻿using System.Security;
using System.Security.Permissions;

namespace System.Net.Mail
{
    [Serializable]
    public sealed class SmtpPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public SmtpPermission(bool unrestricted) { }
        public SmtpPermission(SmtpAccess access) { }
        public SmtpPermission(PermissionState state) { }
        public SmtpAccess Access { get; }
        public void AddPermission(SmtpAccess access) { }
        public override IPermission Copy() { return null; }
        public override void FromXml(SecurityElement securityElement) { }
        public override IPermission Intersect(IPermission target) { return null; }
        public override bool IsSubsetOf(IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override SecurityElement ToXml() { return null; }
        public override IPermission Union(IPermission target) { return null; }
    }
}
