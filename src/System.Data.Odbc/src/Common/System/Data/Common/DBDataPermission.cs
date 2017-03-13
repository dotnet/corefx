// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
    [SecurityPermissionAttribute(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
    [Serializable]
    public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        private bool _isUnrestricted;// = false;
        private bool _allowBlankPassword;// = false;
        private NameValuePermission _keyvaluetree = NameValuePermission.Default;
        private /*DBConnectionString[]*/ArrayList _keyvalues; // = null;

        [Obsolete("DBDataPermission() has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)] // V1.2.3300, MDAC 86034
        protected DBDataPermission() : this(PermissionState.None)
        { // V1.0.3300
        }

        protected DBDataPermission(PermissionState state)
        { // V1.0.3300
            if (state == PermissionState.Unrestricted)
            {
                _isUnrestricted = true;
            }
            else if (state == PermissionState.None)
            {
                _isUnrestricted = false;
            }
            else
            {
                throw ADP.InvalidPermissionState(state);
            }
        }

        [Obsolete("DBDataPermission(PermissionState state,Boolean allowBlankPassword) has been deprecated.  Use the DBDataPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)] // V1.2.3300, MDAC 86034
        protected DBDataPermission(PermissionState state, bool allowBlankPassword) : this(state)
        { // V1.0.3300,  MDAC 84281
            AllowBlankPassword = allowBlankPassword;
        }

        protected DBDataPermission(DBDataPermission permission)
        { // V1.0.5000,  for Copy
            if (null == permission)
            {
                throw ADP.ArgumentNull("permissionAttribute");
            }
            CopyFrom(permission);
        }

        protected DBDataPermission(DBDataPermissionAttribute permissionAttribute)
        { // V1.0.5000, for CreatePermission
            if (null == permissionAttribute)
            {
                throw ADP.ArgumentNull("permissionAttribute");
            }
            _isUnrestricted = permissionAttribute.Unrestricted;
            if (!_isUnrestricted)
            {
                _allowBlankPassword = permissionAttribute.AllowBlankPassword;
                if (permissionAttribute.ShouldSerializeConnectionString() || permissionAttribute.ShouldSerializeKeyRestrictions())
                { // MDAC 86773
                    Add(permissionAttribute.ConnectionString, permissionAttribute.KeyRestrictions, permissionAttribute.KeyRestrictionBehavior);
                }
            }
        }

        // how connectionString security is used
        // parsetable (all string) is shared with connection
        internal DBDataPermission(DbConnectionOptions connectionOptions)
        { // v2.0
            if (null != connectionOptions)
            {
                _allowBlankPassword = connectionOptions.HasBlankPassword; // MDAC 84563
                AddPermissionEntry(new DBConnectionString(connectionOptions));
            }
        }

        public bool AllowBlankPassword
        { // V1.0.3300
            get
            {
                return _allowBlankPassword;
            }
            set
            { // MDAC 61263
                // for behavioral backward compatability with V1.1
                // set_AllowBlankPassword does not _isUnrestricted=false
                _allowBlankPassword = value;
            }
        }

        public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
        { // V1.0.5000
            DBConnectionString constr = new DBConnectionString(connectionString, restrictions, behavior, null, false);
            AddPermissionEntry(constr);
        }

        internal void AddPermissionEntry(DBConnectionString entry)
        {
            if (null == _keyvaluetree)
            {
                _keyvaluetree = new NameValuePermission();
            }
            if (null == _keyvalues)
            {
                _keyvalues = new ArrayList();
            }
            NameValuePermission.AddEntry(_keyvaluetree, _keyvalues, entry);
            _isUnrestricted = false; // MDAC 84639
        }

        protected void Clear()
        { // V1.2.3300, MDAC 83105
            _keyvaluetree = null;
            _keyvalues = null;
        }

        // IPermission interface methods
        // [ObsoleteAttribute("override Copy instead of using default implementation")] // not inherited
        public override IPermission Copy()
        {
            DBDataPermission copy = CreateInstance();
            copy.CopyFrom(this);
            return copy;
        }

        private void CopyFrom(DBDataPermission permission)
        {
            _isUnrestricted = permission.IsUnrestricted();
            if (!_isUnrestricted)
            {
                _allowBlankPassword = permission.AllowBlankPassword;

                if (null != permission._keyvalues)
                {
                    _keyvalues = (ArrayList)permission._keyvalues.Clone();

                    if (null != permission._keyvaluetree)
                    {
                        _keyvaluetree = permission._keyvaluetree.CopyNameValue();
                    }
                }
            }
        }

        // [ Obsolete("use DBDataPermission(DBDataPermission) ctor") ]
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")] // V1.0.5000, MDAC 82936
        protected virtual DBDataPermission CreateInstance()
        {
            // derived class should override with a different implementation avoiding reflection to allow semi-trusted scenarios
            return (Activator.CreateInstance(GetType(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, CultureInfo.InvariantCulture, null) as DBDataPermission);
        }

        public override IPermission Intersect(IPermission target)
        { // used during Deny actions
            if (null == target)
            {
                return null;
            }
            if (target.GetType() != this.GetType())
            {
                throw ADP.PermissionTypeMismatch();
            }
            if (this.IsUnrestricted())
            { // MDAC 84803, NDPWhidbey 29121
                return target.Copy();
            }

            DBDataPermission operand = (DBDataPermission)target;
            if (operand.IsUnrestricted())
            { // NDPWhidbey 29121
                return this.Copy();
            }

            DBDataPermission newPermission = (DBDataPermission)operand.Copy();
            newPermission._allowBlankPassword &= AllowBlankPassword;

            if ((null != _keyvalues) && (null != newPermission._keyvalues))
            {
                newPermission._keyvalues.Clear();

                newPermission._keyvaluetree.Intersect(newPermission._keyvalues, _keyvaluetree);
            }
            else
            {
                // either target.Add or this.Add have not been called
                // return a non-null object so IsSubset calls will fail
                newPermission._keyvalues = null;
                newPermission._keyvaluetree = null;
            }

            if (newPermission.IsEmpty())
            { // no intersection, MDAC 86773
                newPermission = null;
            }
            return newPermission;
        }

        private bool IsEmpty()
        { // MDAC 84804
            ArrayList keyvalues = _keyvalues;
            bool flag = (!IsUnrestricted() && !AllowBlankPassword && ((null == keyvalues) || (0 == keyvalues.Count)));
            return flag;
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (null == target)
            {
                return IsEmpty();
            }
            if (target.GetType() != this.GetType())
            {
                throw ADP.PermissionTypeMismatch();
            }

            DBDataPermission superset = (target as DBDataPermission);

            bool subset = superset.IsUnrestricted();
            if (!subset)
            {
                if (!IsUnrestricted() &&
                    (!AllowBlankPassword || superset.AllowBlankPassword) &&
                    ((null == _keyvalues) || (null != superset._keyvaluetree)))
                {
                    subset = true;
                    if (null != _keyvalues)
                    {
                        foreach (DBConnectionString kventry in _keyvalues)
                        {
                            if (!superset._keyvaluetree.CheckValueForKeyPermit(kventry))
                            {
                                subset = false;
                                break;
                            }
                        }
                    }
                }
            }
            return subset;
        }

        // IUnrestrictedPermission interface methods
        public bool IsUnrestricted()
        {
            return _isUnrestricted;
        }

        public override IPermission Union(IPermission target)
        {
            if (null == target)
            {
                return this.Copy();
            }
            if (target.GetType() != this.GetType())
            {
                throw ADP.PermissionTypeMismatch();
            }
            if (IsUnrestricted())
            { // MDAC 84803
                return this.Copy();
            }

            DBDataPermission newPermission = (DBDataPermission)target.Copy();
            if (!newPermission.IsUnrestricted())
            {
                newPermission._allowBlankPassword |= AllowBlankPassword;

                if (null != _keyvalues)
                {
                    foreach (DBConnectionString entry in _keyvalues)
                    {
                        newPermission.AddPermissionEntry(entry);
                    }
                }
            }
            return (newPermission.IsEmpty() ? null : newPermission);
        }

        private string DecodeXmlValue(string value)
        {
            if ((null != value) && (0 < value.Length))
            {
                value = value.Replace("&quot;", "\"");
                value = value.Replace("&apos;", "\'");
                value = value.Replace("&lt;", "<");
                value = value.Replace("&gt;", ">");
                value = value.Replace("&amp;", "&");
            }
            return value;
        }

        private string EncodeXmlValue(string value)
        {
            if ((null != value) && (0 < value.Length))
            {
                value = value.Replace('\0', ' '); // assumption that '\0' will only be at end of string
                value = value.Trim();
                value = value.Replace("&", "&amp;");
                value = value.Replace(">", "&gt;");
                value = value.Replace("<", "&lt;");
                value = value.Replace("\'", "&apos;");
                value = value.Replace("\"", "&quot;");
            }
            return value;
        }

        // <IPermission class="...Permission" version="1" AllowBlankPassword=false>
        //     <add ConnectionString="provider=x;data source=y;" KeyRestrictions="address=;server=" KeyRestrictionBehavior=PreventUsage/>
        // </IPermission>
        public override void FromXml(SecurityElement securityElement)
        {
            // code derived from CodeAccessPermission.ValidateElement
            if (null == securityElement)
            {
                throw ADP.ArgumentNull("securityElement");
            }
            string tag = securityElement.Tag;
            if (!tag.Equals(XmlStr._Permission) && !tag.Equals(XmlStr._IPermission))
            {
                throw ADP.NotAPermissionElement();
            }
            String version = securityElement.Attribute(XmlStr._Version);
            if ((null != version) && !version.Equals(XmlStr._VersionNumber))
            {
                throw ADP.InvalidXMLBadVersion();
            }

            string unrestrictedValue = securityElement.Attribute(XmlStr._Unrestricted);
            _isUnrestricted = (null != unrestrictedValue) && Boolean.Parse(unrestrictedValue);

            Clear(); // MDAC 83105
            if (!_isUnrestricted)
            {
                string allowNull = securityElement.Attribute(XmlStr._AllowBlankPassword);
                _allowBlankPassword = (null != allowNull) && Boolean.Parse(allowNull);

                ArrayList children = securityElement.Children;
                if (null != children)
                {
                    foreach (SecurityElement keyElement in children)
                    {
                        tag = keyElement.Tag;
                        if ((XmlStr._add == tag) || ((null != tag) && (XmlStr._add == tag.ToLower(CultureInfo.InvariantCulture))))
                        {
                            string constr = keyElement.Attribute(XmlStr._ConnectionString);
                            string restrt = keyElement.Attribute(XmlStr._KeyRestrictions);
                            string behavr = keyElement.Attribute(XmlStr._KeyRestrictionBehavior);

                            KeyRestrictionBehavior behavior = KeyRestrictionBehavior.AllowOnly;
                            if (null != behavr)
                            {
                                behavior = (KeyRestrictionBehavior)Enum.Parse(typeof(KeyRestrictionBehavior), behavr, true);
                            }
                            constr = DecodeXmlValue(constr);
                            restrt = DecodeXmlValue(restrt);
                            Add(constr, restrt, behavior);
                        }
                    }
                }
            }
            else
            {
                _allowBlankPassword = false;
            }
        }

        // <IPermission class="...Permission" version="1" AllowBlankPassword=false>
        //     <add ConnectionString="provider=x;data source=y;"/>
        //     <add ConnectionString="provider=x;data source=y;" KeyRestrictions="user id=;password=;" KeyRestrictionBehavior=AllowOnly/>
        //     <add ConnectionString="provider=x;data source=y;" KeyRestrictions="address=;server=" KeyRestrictionBehavior=PreventUsage/>
        // </IPermission>
        public override SecurityElement ToXml()
        {
            Type type = this.GetType();
            SecurityElement root = new SecurityElement(XmlStr._IPermission);
            root.AddAttribute(XmlStr._class, type.AssemblyQualifiedName.Replace('\"', '\''));
            root.AddAttribute(XmlStr._Version, XmlStr._VersionNumber);

            if (IsUnrestricted())
            {
                root.AddAttribute(XmlStr._Unrestricted, XmlStr._true);
            }
            else
            {
                root.AddAttribute(XmlStr._AllowBlankPassword, _allowBlankPassword.ToString(CultureInfo.InvariantCulture));

                if (null != _keyvalues)
                {
                    foreach (DBConnectionString value in _keyvalues)
                    {
                        SecurityElement valueElement = new SecurityElement(XmlStr._add);
                        string tmp;

                        tmp = value.ConnectionString; // WebData 97375
                        tmp = EncodeXmlValue(tmp);
                        if (!string.IsNullOrEmpty(tmp))
                        {
                            valueElement.AddAttribute(XmlStr._ConnectionString, tmp);
                        }
                        tmp = value.Restrictions;
                        tmp = EncodeXmlValue(tmp);
                        if (null == tmp) { tmp = ADP.StrEmpty; }
                        valueElement.AddAttribute(XmlStr._KeyRestrictions, tmp);

                        tmp = value.Behavior.ToString();
                        valueElement.AddAttribute(XmlStr._KeyRestrictionBehavior, tmp);

                        root.AddChild(valueElement);
                    }
                }
            }
            return root;
        }

        private static class XmlStr
        {
            internal const string _class = "class";
            internal const string _IPermission = "IPermission";
            internal const string _Permission = "Permission";
            internal const string _Unrestricted = "Unrestricted";
            internal const string _AllowBlankPassword = "AllowBlankPassword";
            internal const string _true = "true";
            internal const string _Version = "version";
            internal const string _VersionNumber = "1";

            internal const string _add = "add";

            internal const string _ConnectionString = "ConnectionString";
            internal const string _KeyRestrictions = "KeyRestrictions";
            internal const string _KeyRestrictionBehavior = "KeyRestrictionBehavior";
        }
    }
}