// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Data.OleDb {

    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;


    // TOOD move to System.Security.Permissions
    //
    //[Serializable] 
    //public sealed class OleDbPermission :  DBDataPermission {

    //    private String[] _providerRestriction; // should never be string[0]
    //    private String _providers;
    //    public OleDbPermission() : this(PermissionState.None) {
    //    }

    //    public OleDbPermission(PermissionState state) : base(state) {
    //    }
    //    public OleDbPermission(PermissionState state, bool allowBlankPassword) : this(state) {
    //        AllowBlankPassword = allowBlankPassword;
    //    }

    //    private OleDbPermission(OleDbPermission permission) : base(permission) { // for Copy
    //    }

    //    internal OleDbPermission(OleDbPermissionAttribute permissionAttribute) : base(permissionAttribute) { // for CreatePermission
    //    }

    //    internal OleDbPermission(OleDbConnectionString constr) : base(constr) { // for Open
    //        if ((null == constr) || constr.IsEmpty) {
    //            base.Add(ADP.StrEmpty, ADP.StrEmpty, KeyRestrictionBehavior.AllowOnly);
    //        }
    //    }

    //    [System.ComponentModel.Browsable(false)]
    //    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    //    [Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
    //    public string Provider {
    //        get {
    //            string providers = _providers; // MDAC 83103
    //            if (null == providers) {
    //                string[] restrictions = _providerRestriction;
    //                if (null != restrictions) {
    //                    if (0 < restrictions.Length) {
    //                        providers = restrictions[0];
    //                        for (int i = 1; i < restrictions.Length; ++i) {
    //                            providers += ";" + restrictions[i];
    //                        }
    //                    }
    //                }
    //            }
    //            return ((null != providers) ? providers : ADP.StrEmpty);
    //        }
    //        set { // MDAC 61263
    //            string[] restrictions = null;
    //            if (!ADP.IsEmpty(value)) {
    //                restrictions = value.Split(new char[1] { ';' });
    //                restrictions = DBConnectionString.RemoveDuplicates(restrictions);
    //            }
    //            _providerRestriction = restrictions;
    //            _providers = value;
    //        }
    //    }

    //    override public IPermission Copy () {
    //        return new OleDbPermission(this);
    //    }
    //}

    //TODO - move to System.Security.Permissions
    //
    //
    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false )]
    //[Serializable] 
    //public sealed class OleDbPermissionAttribute : DBDataPermissionAttribute {

    //    private String _providers;

    //    public OleDbPermissionAttribute(SecurityAction action) : base(action) {
    //    }

    //    [System.ComponentModel.Browsable(false)]
    //    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    //    [Obsolete("Provider property has been deprecated.  Use the Add method.  http://go.microsoft.com/fwlink/?linkid=14202")]
    //    public String Provider {
    //        get {
    //            string providers = _providers;
    //            return ((null != providers) ? providers : ADP.StrEmpty);
    //        }
    //        set {
    //            _providers = value;
    //        }
    //    }

    //    override public IPermission CreatePermission() {
    //        return new OleDbPermission(this);
    //    }
    //}
}
