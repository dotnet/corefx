// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Security.Cryptography;

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.IPermission))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.ISecurityEncodable))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.SecurityElement))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.Permissions.CodeAccessSecurityAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.Permissions.SecurityAction))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.Permissions.SecurityAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.Permissions.SecurityPermissionAttribute))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Security.Permissions.SecurityPermissionFlag))]

namespace System
{
    public sealed partial class ApplicationIdentity : System.Runtime.Serialization.ISerializable
    {
        public ApplicationIdentity(string applicationIdentityFullName) { }
        public string CodeBase { get { throw null; } }
        public string FullName { get { throw null; } }
        [System.Security.SecurityCriticalAttribute]
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() { throw null; }
    }
}

namespace System.Data.Common
{
    public abstract partial class DBDataPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        protected DBDataPermission() { }
        protected DBDataPermission(System.Data.Common.DBDataPermission permission) { }
        protected DBDataPermission(System.Data.Common.DBDataPermissionAttribute permissionAttribute) { }
        protected DBDataPermission(System.Security.Permissions.PermissionState state) { }
        protected DBDataPermission(System.Security.Permissions.PermissionState state, bool allowBlankPassword) { }
        public bool AllowBlankPassword { get; set; }
        public virtual void Add(string connectionString, string restrictions, System.Data.KeyRestrictionBehavior behavior) { }
        protected void Clear() { }
        public override System.Security.IPermission Copy() { throw null; }
        protected virtual System.Data.Common.DBDataPermission CreateInstance() { throw null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = true, Inherited = false)]
    public abstract partial class DBDataPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        protected DBDataPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public bool AllowBlankPassword { get { throw null; } set { } }
        public string ConnectionString { get { throw null; } set { } }
        public System.Data.KeyRestrictionBehavior KeyRestrictionBehavior { get { throw null; } set { } }
        public string KeyRestrictions { get { throw null; } set { } }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeConnectionString() { throw null; }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeKeyRestrictions() { throw null; }
    }
}

namespace System.Data.Odbc
{
    public sealed partial class OdbcPermission : System.Data.Common.DBDataPermission
    {
        public OdbcPermission() : base(default(System.Security.Permissions.PermissionState)) { }
        public OdbcPermission(System.Security.Permissions.PermissionState state) : base(default(System.Security.Permissions.PermissionState)) { }
        public OdbcPermission(System.Security.Permissions.PermissionState state, bool allowBlankPassword) : base(default(System.Security.Permissions.PermissionState)) { }
        public override void Add(string connectionString, string restrictions, System.Data.KeyRestrictionBehavior behavior) { }
        public override System.Security.IPermission Copy() { return null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method,
    AllowMultiple = true, Inherited = false)]
    public sealed partial class OdbcPermissionAttribute : System.Data.Common.DBDataPermissionAttribute
    {
        public OdbcPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Data.OleDb
{
    public sealed partial class OleDbPermission : System.Data.Common.DBDataPermission
    {
        public OleDbPermission() : base(default(System.Security.Permissions.PermissionState)) { }
        public OleDbPermission(System.Security.Permissions.PermissionState state) : base(default(System.Security.Permissions.PermissionState)) { }
        public OleDbPermission(System.Security.Permissions.PermissionState state, bool allowBlankPassword) : base(default(System.Security.Permissions.PermissionState)) { }
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public string Provider { get { return null; } set { } }
        public override System.Security.IPermission Copy() { return null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed partial class OleDbPermissionAttribute : System.Data.Common.DBDataPermissionAttribute
    {
        public OleDbPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public string Provider { get { return null; } set { } }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Data.OracleClient
{
    public sealed partial class OraclePermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public OraclePermission(System.Security.Permissions.PermissionState state) { }
        public bool AllowBlankPassword { get; set; }
        public void Add(string connectionString, string restrictions, System.Data.KeyRestrictionBehavior behavior) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed partial class OraclePermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public OraclePermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public bool AllowBlankPassword { get; set; }
        public string ConnectionString { get { return null; } set { } }
        public System.Data.KeyRestrictionBehavior KeyRestrictionBehavior { get; set; }
        public string KeyRestrictions { get { return null; } set { } }
        public override System.Security.IPermission CreatePermission() { return null; }
        [System.ComponentModel.EditorBrowsableAttribute(ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeConnectionString() => false;
        [System.ComponentModel.EditorBrowsableAttribute(ComponentModel.EditorBrowsableState.Never)]
        public bool ShouldSerializeKeyRestrictions() => false;
    }
}

namespace System.Drawing.Printing
{
    public sealed partial class PrintingPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public PrintingPermission(System.Drawing.Printing.PrintingPermissionLevel printingLevel) { }
        public PrintingPermission(System.Security.Permissions.PermissionState state) { }
        public System.Drawing.Printing.PrintingPermissionLevel Level { get { throw null; } set { } }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement element) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed partial class PrintingPermissionAttribute : Security.Permissions.CodeAccessSecurityAttribute
    {
        public PrintingPermissionAttribute(Security.Permissions.SecurityAction action) : base(action) { }
        public PrintingPermissionLevel Level { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public enum PrintingPermissionLevel
    {
        AllPrinting = 3,
        DefaultPrinting = 2,
        NoPrinting = 0,
        SafePrinting = 1,
    }
}
namespace System.Data.SqlClient
{
    public sealed partial class SqlClientPermission : System.Data.Common.DBDataPermission
    {
        public SqlClientPermission() : base(default(System.Security.Permissions.PermissionState)) { }
        public SqlClientPermission(System.Security.Permissions.PermissionState state) : base(default(System.Security.Permissions.PermissionState)) { }
        public SqlClientPermission(System.Security.Permissions.PermissionState state, bool allowBlankPassword) : base(default(System.Security.Permissions.PermissionState)) { }
        public override void Add(string connectionString, string restrictions, System.Data.KeyRestrictionBehavior behavior) { }
        public override System.Security.IPermission Copy() { return null; }
    }
    [System.AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = true, Inherited = false)]
    public sealed partial class SqlClientPermissionAttribute : System.Data.Common.DBDataPermissionAttribute
    {
        public SqlClientPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Diagnostics
{
    public sealed partial class EventLogPermission : System.Security.Permissions.ResourcePermissionBase
    {
        public EventLogPermission() { }
        public EventLogPermission(System.Diagnostics.EventLogPermissionAccess permissionAccess, string machineName) { }
        public EventLogPermission(System.Diagnostics.EventLogPermissionEntry[] permissionAccessEntries) { }
        public EventLogPermission(System.Security.Permissions.PermissionState state) { }
        public System.Diagnostics.EventLogPermissionEntryCollection PermissionEntries { get { return null; } }
    }
    [System.Flags]
    public enum EventLogPermissionAccess
    {
        Administer = 48,
        Audit = 10,
        Browse = 2,
        Instrument = 6,
        None = 0,
        Write = 16,
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct
        | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true, Inherited = false)]
    public partial class EventLogPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public EventLogPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string MachineName { get { return null; } set { } }
        public System.Diagnostics.EventLogPermissionAccess PermissionAccess { get; set; }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
    public partial class EventLogPermissionEntry
    {
        public EventLogPermissionEntry(System.Diagnostics.EventLogPermissionAccess permissionAccess, string machineName) { }
        public string MachineName { get { return null; } }
        public System.Diagnostics.EventLogPermissionAccess PermissionAccess { get; }
    }
    public partial class EventLogPermissionEntryCollection : System.Collections.CollectionBase
    {
        internal EventLogPermissionEntryCollection() { }
        public System.Diagnostics.EventLogPermissionEntry this[int index] { get { return null; } set { } }
        public int Add(System.Diagnostics.EventLogPermissionEntry value) { return 0; }
        public void AddRange(System.Diagnostics.EventLogPermissionEntryCollection value) { }
        public void AddRange(System.Diagnostics.EventLogPermissionEntry[] value) { }
        public bool Contains(System.Diagnostics.EventLogPermissionEntry value) { return false; }
        public void CopyTo(System.Diagnostics.EventLogPermissionEntry[] array, int index) { }
        public int IndexOf(System.Diagnostics.EventLogPermissionEntry value) { return 0; }
        public void Insert(int index, System.Diagnostics.EventLogPermissionEntry value) { }
        protected override void OnClear() { }
        protected override void OnInsert(int index, object value) { }
        protected override void OnRemove(int index, object value) { }
        protected override void OnSet(int index, object oldValue, object newValue) { }
        public void Remove(System.Diagnostics.EventLogPermissionEntry value) { }
    }
    public sealed partial class PerformanceCounterPermission : System.Security.Permissions.ResourcePermissionBase
    {
        public PerformanceCounterPermission() { }
        public PerformanceCounterPermission(System.Diagnostics.PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName) { }
        public PerformanceCounterPermission(System.Diagnostics.PerformanceCounterPermissionEntry[] permissionAccessEntries) { }
        public PerformanceCounterPermission(System.Security.Permissions.PermissionState state) { }
        public System.Diagnostics.PerformanceCounterPermissionEntryCollection PermissionEntries { get { return null; } }
    }
    [System.Flags]
    public enum PerformanceCounterPermissionAccess
    {
        Administer = 7,
        Browse = 1,
        Instrument = 3,
        None = 0,
        Read = 1,
        Write = 2,
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor
        | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true, Inherited = false)]
    public partial class PerformanceCounterPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PerformanceCounterPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string CategoryName { get { return null; } set { } }
        public string MachineName { get { return null; } set { } }
        public System.Diagnostics.PerformanceCounterPermissionAccess PermissionAccess { get; set; }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
    public partial class PerformanceCounterPermissionEntry
    {
        public PerformanceCounterPermissionEntry(System.Diagnostics.PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName) { }
        public string CategoryName { get { return null; } }
        public string MachineName { get { return null; } }
        public System.Diagnostics.PerformanceCounterPermissionAccess PermissionAccess { get; }
    }
    public partial class PerformanceCounterPermissionEntryCollection : System.Collections.CollectionBase
    {
        internal PerformanceCounterPermissionEntryCollection() { }
        public System.Diagnostics.PerformanceCounterPermissionEntry this[int index] { get { return null; } set { } }
        public int Add(System.Diagnostics.PerformanceCounterPermissionEntry value) { return 0; }
        public void AddRange(System.Diagnostics.PerformanceCounterPermissionEntryCollection value) { }
        public void AddRange(System.Diagnostics.PerformanceCounterPermissionEntry[] value) { }
        public bool Contains(System.Diagnostics.PerformanceCounterPermissionEntry value) { return false; }
        public void CopyTo(System.Diagnostics.PerformanceCounterPermissionEntry[] array, int index) { }
        public int IndexOf(System.Diagnostics.PerformanceCounterPermissionEntry value) { return 0; }
        public void Insert(int index, System.Diagnostics.PerformanceCounterPermissionEntry value) { }
        protected override void OnClear() { }
        protected override void OnInsert(int index, object value) { }
        protected override void OnRemove(int index, object value) { }
        protected override void OnSet(int index, object oldValue, object newValue) { }
        public void Remove(System.Diagnostics.PerformanceCounterPermissionEntry value) { }
    }
}
namespace System.Net
{
    public sealed partial class DnsPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public DnsPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class |
        AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed partial class DnsPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public DnsPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
    public partial class EndpointPermission
    {
        internal EndpointPermission() { }
        public string Hostname { get { return null; } }
        public int Port { get { return 0; } }
        public TransportType Transport { get; }
        public override bool Equals(object obj) { return false; }
        public override int GetHashCode() { return 0; }
    }

    [System.Flags]
    public enum NetworkAccess
    {
        Accept = 128,
        Connect = 64,
    }
    public sealed partial class SocketPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public const int AllPorts = -1;
        public SocketPermission(System.Net.NetworkAccess access, System.Net.TransportType transport, string hostName, int portNumber) { }
        public SocketPermission(System.Security.Permissions.PermissionState state) { }
        public System.Collections.IEnumerator AcceptList { get { return null; } }
        public System.Collections.IEnumerator ConnectList { get { return null; } }
        public void AddPermission(System.Net.NetworkAccess access, System.Net.TransportType transport, string hostName, int portNumber) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class |
        AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed partial class SocketPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public SocketPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string Access { get { return null; } set { } }
        public string Host { get { return null; } set { } }
        public string Port { get { return null; } set { } }
        public string Transport { get { return null; } set { } }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
    public enum TransportType
    {
        All = 3,
        Connectionless = 1,
        ConnectionOriented = 2,
        Tcp = 2,
        Udp = 1,
    }
    public sealed partial class WebPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public WebPermission() { }
        public WebPermission(System.Net.NetworkAccess access, string uriString) { }
        public WebPermission(System.Net.NetworkAccess access, System.Text.RegularExpressions.Regex uriRegex) { }
        public WebPermission(System.Security.Permissions.PermissionState state) { }
        public System.Collections.IEnumerator AcceptList { get { return null; } }
        public System.Collections.IEnumerator ConnectList { get { return null; } }
        public void AddPermission(System.Net.NetworkAccess access, string uriString) { }
        public void AddPermission(System.Net.NetworkAccess access, System.Text.RegularExpressions.Regex uriRegex) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class |
        AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed partial class WebPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public WebPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string Accept { get { return null; } set { } }
        public string AcceptPattern { get { return null; } set { } }
        public string Connect { get { return null; } set { } }
        public string ConnectPattern { get { return null; } set { } }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Net.Mail
{
    public enum SmtpAccess
    {
        Connect = 1,
        ConnectToUnrestrictedPort = 2,
        None = 0,
    }
    public sealed partial class SmtpPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public SmtpPermission(bool unrestricted) { }
        public SmtpPermission(System.Net.Mail.SmtpAccess access) { }
        public SmtpPermission(System.Security.Permissions.PermissionState state) { }
        public System.Net.Mail.SmtpAccess Access { get; }
        public void AddPermission(System.Net.Mail.SmtpAccess access) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class |
        AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed partial class SmtpPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public SmtpPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string Access { get { return null; } set { } }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Net.NetworkInformation
{
    [Flags]
    public enum NetworkInformationAccess
    {
        None = 0,
        Read = 1,
        Ping = 4
    };
    public sealed class NetworkInformationPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public NetworkInformationPermission(System.Security.Permissions.PermissionState state) { }
        public NetworkInformationPermission(NetworkInformationAccess access) { }
        public NetworkInformationAccess Access { get { throw null; } }
        public void AddPermission(NetworkInformationAccess access) { }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.IPermission Copy() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.SecurityElement ToXml() { throw null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class NetworkInformationPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public NetworkInformationPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public string Access { get { throw null; } set { } }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
}
namespace System.Net.PeerToPeer
{
    public sealed partial class PnrpPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public PnrpPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement e) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed partial class PnrpPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PnrpPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
    public enum PnrpScope
    {
        All = 0,
        Global = 1,
        LinkLocal = 3,
        SiteLocal = 2,
    }
}
namespace System.Net.PeerToPeer.Collaboration
{
    public sealed partial class PeerCollaborationPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public PeerCollaborationPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement e) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct |
       AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed partial class PeerCollaborationPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PeerCollaborationPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
namespace System.Security
{
    public abstract partial class CodeAccessPermission : System.Security.IPermission, System.Security.ISecurityEncodable, System.Security.IStackWalk
    {
        protected CodeAccessPermission() { }
        public void Assert() { }
        public abstract System.Security.IPermission Copy();
        public void Demand() { }
        [System.ObsoleteAttribute]
        public void Deny() { }
        public override bool Equals(object obj) { throw null; }
        public abstract void FromXml(System.Security.SecurityElement elem);
        public override int GetHashCode() { throw null; }
        public abstract System.Security.IPermission Intersect(System.Security.IPermission target);
        public abstract bool IsSubsetOf(System.Security.IPermission target);
        public void PermitOnly() { }
        public static void RevertAll() { }
        public static void RevertAssert() { }
        [System.ObsoleteAttribute]
        public static void RevertDeny() { }
        public static void RevertPermitOnly() { }
        public override string ToString() { throw null; }
        public abstract System.Security.SecurityElement ToXml();
        public virtual System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    public partial class HostProtectionException : System.SystemException
    {
        public HostProtectionException() { }
        protected HostProtectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public HostProtectionException(string message) : base(message) { }
        public HostProtectionException(string message, System.Exception e) : base(message, e) { }
        public HostProtectionException(string message, System.Security.Permissions.HostProtectionResource protectedResources, System.Security.Permissions.HostProtectionResource demandedResources) { }
        public System.Security.Permissions.HostProtectionResource DemandedResources { get { throw null; } }
        public System.Security.Permissions.HostProtectionResource ProtectedResources { get { throw null; } }
        public override string ToString() => base.ToString();
    }
    public partial class HostSecurityManager
    {
        public HostSecurityManager() { }
        public virtual System.Security.Policy.PolicyLevel DomainPolicy { get { throw null; } }
        public virtual System.Security.HostSecurityManagerOptions Flags { get { throw null; } }
        public virtual System.Security.Policy.ApplicationTrust DetermineApplicationTrust(System.Security.Policy.Evidence applicationEvidence, System.Security.Policy.Evidence activatorEvidence, System.Security.Policy.TrustManagerContext context) { throw null; }
        public virtual System.Security.Policy.EvidenceBase GenerateAppDomainEvidence(System.Type evidenceType) { throw null; }
        public virtual System.Security.Policy.EvidenceBase GenerateAssemblyEvidence(System.Type evidenceType, System.Reflection.Assembly assembly) { throw null; }
        public virtual System.Type[] GetHostSuppliedAppDomainEvidenceTypes() { throw null; }
        public virtual System.Type[] GetHostSuppliedAssemblyEvidenceTypes(System.Reflection.Assembly assembly) { throw null; }
        public virtual System.Security.Policy.Evidence ProvideAppDomainEvidence(System.Security.Policy.Evidence inputEvidence) { throw null; }
        public virtual System.Security.Policy.Evidence ProvideAssemblyEvidence(System.Reflection.Assembly loadedAssembly, System.Security.Policy.Evidence inputEvidence) { throw null; }
        [System.ObsoleteAttribute]
        public virtual System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence) { throw null; }
    }
    [System.FlagsAttribute]
    public enum HostSecurityManagerOptions
    {
        AllFlags = 31,
        HostAppDomainEvidence = 1,
        HostAssemblyEvidence = 4,
        HostDetermineApplicationTrust = 8,
        HostPolicyLevel = 2,
        HostResolvePolicy = 16,
        None = 0,
    }
    public partial interface IEvidenceFactory
    {
        System.Security.Policy.Evidence Evidence { get; }
    }
    public partial interface ISecurityPolicyEncodable
    {
        void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level);
        System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level);
    }
    public partial interface IStackWalk
    {
        void Assert();
        void Demand();
        void Deny();
        void PermitOnly();
    }
    public sealed partial class NamedPermissionSet : System.Security.PermissionSet
    {
        public NamedPermissionSet(System.Security.NamedPermissionSet permSet) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name, System.Security.Permissions.PermissionState state) : base(default(System.Security.Permissions.PermissionState)) { }
        public NamedPermissionSet(string name, System.Security.PermissionSet permSet) : base(default(System.Security.Permissions.PermissionState)) { }
        public string Description { get; set; }
        public string Name { get; set; }
        public override System.Security.PermissionSet Copy() { throw null; }
        public System.Security.NamedPermissionSet Copy(string name) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override void FromXml(System.Security.SecurityElement et) { }
        public override int GetHashCode() => base.GetHashCode();
        public override System.Security.SecurityElement ToXml() { throw null; }
    }
    public partial class PermissionSet : System.Collections.ICollection, System.Collections.IEnumerable, System.Runtime.Serialization.IDeserializationCallback, System.Security.ISecurityEncodable, System.Security.IStackWalk
    {
        public PermissionSet(System.Security.Permissions.PermissionState state) { }
        public PermissionSet(System.Security.PermissionSet permSet) { }
        public virtual int Count { get { throw null; } }
        public virtual bool IsReadOnly { get { throw null; } }
        public virtual bool IsSynchronized { get { throw null; } }
        public virtual object SyncRoot { get { throw null; } }
        public System.Security.IPermission AddPermission(System.Security.IPermission perm) { throw null; }
        protected virtual System.Security.IPermission AddPermissionImpl(System.Security.IPermission perm) { throw null; }
        public void Assert() { }
        public bool ContainsNonCodeAccessPermissions() { throw null; }
        [System.ObsoleteAttribute]
        public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat) { throw null; }
        public virtual System.Security.PermissionSet Copy() { throw null; }
        public virtual void CopyTo(System.Array array, int index) { }
        public void Demand() { }
        [System.ObsoleteAttribute]
        public void Deny() { }
        public override bool Equals(object o) { throw null; }
        public virtual void FromXml(System.Security.SecurityElement et) { }
        public System.Collections.IEnumerator GetEnumerator() { throw null; }
        protected virtual System.Collections.IEnumerator GetEnumeratorImpl() { throw null; }
        public override int GetHashCode() { throw null; }
        public System.Security.IPermission GetPermission(System.Type permClass) { throw null; }
        protected virtual System.Security.IPermission GetPermissionImpl(System.Type permClass) { throw null; }
        public System.Security.PermissionSet Intersect(System.Security.PermissionSet other) { throw null; }
        public bool IsEmpty() { throw null; }
        public bool IsSubsetOf(System.Security.PermissionSet target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public void PermitOnly() { }
        public System.Security.IPermission RemovePermission(System.Type permClass) { throw null; }
        protected virtual System.Security.IPermission RemovePermissionImpl(System.Type permClass) { throw null; }
        public static void RevertAssert() { }
        public System.Security.IPermission SetPermission(System.Security.IPermission perm) { throw null; }
        protected virtual System.Security.IPermission SetPermissionImpl(System.Security.IPermission perm) { throw null; }
        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
        public override string ToString() { throw null; }
        public virtual System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.PermissionSet Union(System.Security.PermissionSet other) { throw null; }
    }
    public enum PolicyLevelType
    {
        AppDomain = 3,
        Enterprise = 2,
        Machine = 1,
        User = 0,
    }
    public sealed partial class SecurityContext : System.IDisposable
    {
        internal SecurityContext() { }
        public static System.Security.SecurityContext Capture() { throw null; }
        public System.Security.SecurityContext CreateCopy() { throw null; }
        public void Dispose() { }
        public static bool IsFlowSuppressed() { throw null; }
        public static bool IsWindowsIdentityFlowSuppressed() { throw null; }
        public static void RestoreFlow() { }
        public static void Run(System.Security.SecurityContext securityContext, System.Threading.ContextCallback callback, object state) { }
        public static System.Threading.AsyncFlowControl SuppressFlow() { throw null; }
        public static System.Threading.AsyncFlowControl SuppressFlowWindowsIdentity() { throw null; }
    }
    public enum SecurityContextSource
    {
        CurrentAppDomain = 0,
        CurrentAssembly = 1,
    }
    public static partial class SecurityManager
    {
        [System.ObsoleteAttribute]
        public static bool CheckExecutionRights { get { throw null; } set { } }
        [System.ObsoleteAttribute]
        public static bool SecurityEnabled { get { throw null; } set { } }
        [SecurityCritical]
        public static bool CurrentThreadRequiresSecurityContextCapture() { throw null; }
        public static System.Security.PermissionSet GetStandardSandbox(System.Security.Policy.Evidence evidence) { throw null; }
        public static void GetZoneAndOrigin(out System.Collections.ArrayList zone, out System.Collections.ArrayList origin) { zone = default(System.Collections.ArrayList); origin = default(System.Collections.ArrayList); }
        [System.ObsoleteAttribute]
        public static bool IsGranted(System.Security.IPermission perm) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.Policy.PolicyLevel LoadPolicyLevelFromFile(string path, System.Security.PolicyLevelType type) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.Policy.PolicyLevel LoadPolicyLevelFromString(string str, System.Security.PolicyLevelType type) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Collections.IEnumerator PolicyHierarchy() { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence evidence, System.Security.PermissionSet reqdPset, System.Security.PermissionSet optPset, System.Security.PermissionSet denyPset, out System.Security.PermissionSet denied) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolvePolicy(System.Security.Policy.Evidence[] evidences) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Collections.IEnumerator ResolvePolicyGroups(System.Security.Policy.Evidence evidence) { throw null; }
        [System.ObsoleteAttribute]
        public static System.Security.PermissionSet ResolveSystemPolicy(System.Security.Policy.Evidence evidence) { throw null; }
        [System.ObsoleteAttribute]
        public static void SavePolicy() { }
        [System.ObsoleteAttribute]
        public static void SavePolicyLevel(System.Security.Policy.PolicyLevel level) { }
    }
    public abstract partial class SecurityState
    {
        protected SecurityState() { }
        public abstract void EnsureState();
        public bool IsStateAvailable() { throw null; }
    }
    public enum SecurityZone
    {
        Internet = 3,
        Intranet = 1,
        MyComputer = 0,
        NoZone = -1,
        Trusted = 2,
        Untrusted = 4,
    }
    public sealed partial class XmlSyntaxException : System.SystemException
    {
        public XmlSyntaxException() { }
        public XmlSyntaxException(int lineNumber) { }
        public XmlSyntaxException(int lineNumber, string message) { }
        public XmlSyntaxException(string message) : base(message) { }
        public XmlSyntaxException(string message, System.Exception inner) : base(message, inner) { }
    }
}

namespace System.Security.Permissions
{
    public sealed partial class EnvironmentPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public EnvironmentPermission(System.Security.Permissions.EnvironmentPermissionAccess flag, string pathList) { }
        public EnvironmentPermission(System.Security.Permissions.PermissionState state) { }
        public void AddPathList(System.Security.Permissions.EnvironmentPermissionAccess flag, string pathList) { }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public string GetPathList(System.Security.Permissions.EnvironmentPermissionAccess flag) { throw null; }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public void SetPathList(System.Security.Permissions.EnvironmentPermissionAccess flag, string pathList) { }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    [System.FlagsAttribute]
    public enum EnvironmentPermissionAccess
    {
        AllAccess = 3,
        NoAccess = 0,
        Read = 1,
        Write = 2,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class EnvironmentPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public EnvironmentPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string All { get; set; }
        public string Read { get; set; }
        public string Write { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class FileDialogPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public FileDialogPermission(System.Security.Permissions.FileDialogPermissionAccess access) { }
        public FileDialogPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Permissions.FileDialogPermissionAccess Access { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.FlagsAttribute]
    public enum FileDialogPermissionAccess
    {
        None = 0,
        Open = 1,
        OpenSave = 3,
        Save = 2,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class FileDialogPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public FileDialogPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool Open { get; set; }
        public bool Save { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class FileIOPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, System.Security.AccessControl.AccessControlActions actions, string path) { }
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, System.Security.AccessControl.AccessControlActions actions, string[] pathList) { }
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public FileIOPermission(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        public FileIOPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Permissions.FileIOPermissionAccess AllFiles { get { throw null; } set { } }
        public System.Security.Permissions.FileIOPermissionAccess AllLocalFiles { get { throw null; } set { } }
        public void AddPathList(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public void AddPathList(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        public override System.Security.IPermission Copy() { throw null; }
        public override bool Equals(object o) { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override int GetHashCode() { throw null; }
        public string[] GetPathList(System.Security.Permissions.FileIOPermissionAccess access) { throw null; }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public void SetPathList(System.Security.Permissions.FileIOPermissionAccess access, string path) { }
        public void SetPathList(System.Security.Permissions.FileIOPermissionAccess access, string[] pathList) { }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    [System.FlagsAttribute]
    public enum FileIOPermissionAccess
    {
        AllAccess = 15,
        Append = 4,
        NoAccess = 0,
        PathDiscovery = 8,
        Read = 1,
        Write = 2,
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class FileIOPermissionAttribute : CodeAccessSecurityAttribute
    {
        public FileIOPermissionAttribute(SecurityAction action) : base(action) { }
        public string Read { get; set; }
        public string Write { get; set; }
        public string Append { get; set; }
        public string PathDiscovery { get; set; }
        public string ViewAccessControl { get; set; }
        public string ChangeAccessControl { get; set; }
        [Obsolete]
        public string All { get; set; }
        public string ViewAndModify { get; set; }
        public FileIOPermissionAccess AllFiles { get; set; }
        public FileIOPermissionAccess AllLocalFiles { get; set; }
        public override IPermission CreatePermission() { return null; }
    }
    public sealed partial class GacIdentityPermission : System.Security.CodeAccessPermission
    {
        public GacIdentityPermission() { }
        public GacIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class GacIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public GacIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)4205, AllowMultiple = true, Inherited = false)]
    public sealed partial class HostProtectionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public HostProtectionAttribute() : base(default(System.Security.Permissions.SecurityAction)) { }
        public HostProtectionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool ExternalProcessMgmt { get; set; }
        public bool ExternalThreading { get; set; }
        public bool MayLeakOnAbort { get; set; }
        public System.Security.Permissions.HostProtectionResource Resources { get; set; }
        public bool SecurityInfrastructure { get; set; }
        public bool SelfAffectingProcessMgmt { get; set; }
        public bool SelfAffectingThreading { get; set; }
        public bool SharedState { get; set; }
        public bool Synchronization { get; set; }
        public bool UI { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    [System.FlagsAttribute]
    public enum HostProtectionResource
    {
        All = 511,
        ExternalProcessMgmt = 4,
        ExternalThreading = 16,
        MayLeakOnAbort = 256,
        None = 0,
        SecurityInfrastructure = 64,
        SelfAffectingProcessMgmt = 8,
        SelfAffectingThreading = 32,
        SharedState = 2,
        Synchronization = 1,
        UI = 128,
    }
    public enum IsolatedStorageContainment
    {
        None = 0x00,
        DomainIsolationByUser = 0x10,
        ApplicationIsolationByUser = 0x15,
        AssemblyIsolationByUser = 0x20,
        DomainIsolationByMachine = 0x30,
        AssemblyIsolationByMachine = 0x40,
        ApplicationIsolationByMachine = 0x45,
        DomainIsolationByRoamingUser = 0x50,
        AssemblyIsolationByRoamingUser = 0x60,
        ApplicationIsolationByRoamingUser = 0x65,
        AdministerIsolatedStorageByUser = 0x70,
        UnrestrictedIsolatedStorage = 0xF0
    }
    public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission
    {
        public IsolatedStorageFilePermission(PermissionState state) : base(state) { }
        public override IPermission Union(IPermission target) { return null; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public override IPermission Intersect(IPermission target) { return null; }
        public override IPermission Copy() { return null; }
        public override SecurityElement ToXml() { return null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor
     | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
    AllowMultiple = true, Inherited = false)]
    sealed public class IsolatedStorageFilePermissionAttribute : IsolatedStoragePermissionAttribute
    {
        public IsolatedStorageFilePermissionAttribute(SecurityAction action) : base(action) { }
        public override IPermission CreatePermission() { return null; }
    }
    [SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
    public abstract class IsolatedStoragePermission : CodeAccessPermission, IUnrestrictedPermission
    {
        protected IsolatedStoragePermission(PermissionState state) { }
        public long UserQuota { get; set; }
        public IsolatedStorageContainment UsageAllowed { get; set; }
        public bool IsUnrestricted() { return false; }
        public override SecurityElement ToXml() { return default(SecurityElement); }
        public override void FromXml(SecurityElement esd) { }
    }
    public abstract class IsolatedStoragePermissionAttribute : CodeAccessSecurityAttribute
    {
        protected IsolatedStoragePermissionAttribute(SecurityAction action) : base(action) { }
        public long UserQuota { get; set; }
        public IsolatedStorageContainment UsageAllowed { get; set; }
    }
    public partial interface IUnrestrictedPermission
    {
        bool IsUnrestricted();
    }
    public sealed class KeyContainerPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public KeyContainerPermission(PermissionState state) { }
        public KeyContainerPermission(KeyContainerPermissionFlags flags) { }
        public KeyContainerPermission(KeyContainerPermissionFlags flags, KeyContainerPermissionAccessEntry[] accessList) { }
        public KeyContainerPermissionFlags Flags { get; }
        public KeyContainerPermissionAccessEntryCollection AccessEntries { get; }
        public bool IsUnrestricted() { return false; }
        private bool IsEmpty() { return false; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public override IPermission Intersect(IPermission target) { return null; }
        public override IPermission Union(IPermission target) { return null; }
        public override IPermission Copy() { return null; }
        public override SecurityElement ToXml() { return null; }
        public override void FromXml(SecurityElement securityElement) { }
    }
    public sealed class KeyContainerPermissionAccessEntry
    {
        public KeyContainerPermissionAccessEntry(string keyContainerName, KeyContainerPermissionFlags flags) { }
        public KeyContainerPermissionAccessEntry(CspParameters parameters, KeyContainerPermissionFlags flags) { }
        public KeyContainerPermissionAccessEntry(string keyStore, string providerName, int providerType,
                        string keyContainerName, int keySpec, KeyContainerPermissionFlags flags)
        { }
        public string KeyStore { get; set; }
        public string ProviderName { get; set; }
        public int ProviderType { get; set; }
        public string KeyContainerName { get; set; }
        public int KeySpec { get; set; }
        public KeyContainerPermissionFlags Flags { get; set; }
        public override bool Equals(object o) { return false; }
        public override int GetHashCode() { return 0; }
    }
    public sealed class KeyContainerPermissionAccessEntryCollection : ICollection
    {
        public KeyContainerPermissionAccessEntry this[int index] { get { return null; } }
        public int Count { get; }
        public int Add(KeyContainerPermissionAccessEntry accessEntry) { return 0; }
        public void Clear() { }
        public int IndexOf(KeyContainerPermissionAccessEntry accessEntry) { return 0; }
        public void Remove(KeyContainerPermissionAccessEntry accessEntry) { }
        public KeyContainerPermissionAccessEntryEnumerator GetEnumerator() { return null; }
        public void CopyTo(KeyContainerPermissionAccessEntry[] array, int index) { }
        public void CopyTo(Array array, int index) { throw new NotImplementedException(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }
    }
    public sealed class KeyContainerPermissionAccessEntryEnumerator : System.Collections.IEnumerator
    {
        public KeyContainerPermissionAccessEntry Current { get; }
        object System.Collections.IEnumerator.Current { get; }
        public bool MoveNext() { return false; }
        public void Reset() { }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class KeyContainerPermissionAttribute : CodeAccessSecurityAttribute
    {
        public KeyContainerPermissionAttribute(SecurityAction action) : base(action) { }
        public string KeyStore { get; set; }
        public string ProviderName { get; set; }
        public int ProviderType { get; set; }
        public string KeyContainerName { get; set; }
        public int KeySpec { get; set; }
        public KeyContainerPermissionFlags Flags { get; set; }
        public override IPermission CreatePermission() { return null; }
    }
    public enum KeyContainerPermissionFlags
    {
        NoFlags = 0x0000,
        Create = 0x0001,
        Open = 0x0002,
        Delete = 0x0004,
        Import = 0x0010,
        Export = 0x0020,
        Sign = 0x0100,
        Decrypt = 0x0200,
        ViewAcl = 0x1000,
        ChangeAcl = 0x2000,
        AllFlags = 0x3337
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class PermissionSetAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PermissionSetAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string File { get; set; }
        public string Hex { get; set; }
        public string Name { get; set; }
        public bool UnicodeEncoded { get; set; }
        public string XML { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
        public System.Security.PermissionSet CreatePermissionSet() { throw null; }
    }
    public enum PermissionState
    {
        None = 0,
        Unrestricted = 1,
    }
    public sealed partial class PrincipalPermission : System.Security.IPermission, System.Security.ISecurityEncodable, System.Security.Permissions.IUnrestrictedPermission
    {
        public PrincipalPermission(System.Security.Permissions.PermissionState state) { }
        public PrincipalPermission(string name, string role) { }
        public PrincipalPermission(string name, string role, bool isAuthenticated) { }
        public System.Security.IPermission Copy() { throw null; }
        public void Demand() { }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement elem) { }
        public override int GetHashCode() => base.GetHashCode();
        public System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)68, AllowMultiple = true, Inherited = false)]
    public sealed partial class PrincipalPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PrincipalPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public bool Authenticated { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class PublisherIdentityPermission : System.Security.CodeAccessPermission
    {
        public PublisherIdentityPermission(System.Security.Cryptography.X509Certificates.X509Certificate certificate) { }
        public PublisherIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate Certificate { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class PublisherIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public PublisherIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string CertFile { get; set; }
        public string SignedFile { get; set; }
        public string X509Certificate { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class ReflectionPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public ReflectionPermission(System.Security.Permissions.PermissionState state) { }
        public ReflectionPermission(System.Security.Permissions.ReflectionPermissionFlag flag) { }
        public System.Security.Permissions.ReflectionPermissionFlag Flags { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class ReflectionPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public ReflectionPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.Permissions.ReflectionPermissionFlag Flags { get; set; }
        public bool MemberAccess { get; set; }
        [System.ObsoleteAttribute]
        public bool ReflectionEmit { get; set; }
        public bool RestrictedMemberAccess { get; set; }
        [System.ObsoleteAttribute]
        public bool TypeInformation { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    [System.FlagsAttribute]
    public enum ReflectionPermissionFlag
    {
        [System.ObsoleteAttribute]
        AllFlags = 7,
        MemberAccess = 2,
        NoFlags = 0,
        [System.ObsoleteAttribute]
        ReflectionEmit = 4,
        RestrictedMemberAccess = 8,
        [System.ObsoleteAttribute]
        TypeInformation = 1,
    }
    public sealed partial class RegistryPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public RegistryPermission(System.Security.Permissions.PermissionState state) { }
        public RegistryPermission(System.Security.Permissions.RegistryPermissionAccess access, System.Security.AccessControl.AccessControlActions control, string pathList) { }
        public RegistryPermission(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        public void AddPathList(System.Security.Permissions.RegistryPermissionAccess access, System.Security.AccessControl.AccessControlActions actions, string pathList) { }
        public void AddPathList(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement elem) { }
        public string GetPathList(System.Security.Permissions.RegistryPermissionAccess access) { throw null; }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public void SetPathList(System.Security.Permissions.RegistryPermissionAccess access, string pathList) { }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission other) { throw null; }
    }
    [System.FlagsAttribute]
    public enum RegistryPermissionAccess
    {
        AllAccess = 7,
        Create = 4,
        NoAccess = 0,
        Read = 1,
        Write = 2,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class RegistryPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public RegistryPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        [System.ObsoleteAttribute]
        public string All { get; set; }
        public string ChangeAccessControl { get; set; }
        public string Create { get; set; }
        public string Read { get; set; }
        public string ViewAccessControl { get; set; }
        public string ViewAndModify { get; set; }
        public string Write { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    [SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
    public abstract class ResourcePermissionBase : CodeAccessPermission, IUnrestrictedPermission
    {
        public const string Any = "*";
        public const string Local = ".";
        protected ResourcePermissionBase() { }
        protected ResourcePermissionBase(PermissionState state) { }
        private static Hashtable CreateHashtable() { return null; }
        private string ComputerName { get; set; }
        private bool IsEmpty { get; }
        protected Type PermissionAccessType { get; set; }
        protected string[] TagNames { get; set; }
        protected void AddPermissionAccess(ResourcePermissionBaseEntry entry) { }
        protected void Clear() { }
        public override IPermission Copy() { return null; }
        protected ResourcePermissionBaseEntry[] GetPermissionEntries() { return null; }
        public override void FromXml(SecurityElement securityElement) { }
        public override IPermission Intersect(IPermission target) { return null; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public bool IsUnrestricted() { return false; }
        protected void RemovePermissionAccess(ResourcePermissionBaseEntry entry) { }
        public override SecurityElement ToXml() { return null; }
        public override IPermission Union(IPermission target) { return null; }
    }
    public class ResourcePermissionBaseEntry
    {
        public ResourcePermissionBaseEntry() { }
        public ResourcePermissionBaseEntry(int permissionAccess, string[] permissionAccessPath) { }
        public int PermissionAccess { get; }
        public string[] PermissionAccessPath { get; }
    }
    public sealed partial class SecurityPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public SecurityPermission(System.Security.Permissions.PermissionState state) { }
        public SecurityPermission(System.Security.Permissions.SecurityPermissionFlag flag) { }
        public System.Security.Permissions.SecurityPermissionFlag Flags { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    public sealed partial class SiteIdentityPermission : System.Security.CodeAccessPermission
    {
        public SiteIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public SiteIdentityPermission(string site) { }
        public string Site { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class SiteIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public SiteIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string Site { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed class StorePermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public StorePermission(PermissionState state) { }
        public StorePermission(StorePermissionFlags flag) { }
        public StorePermissionFlags Flags { get; set; }
        public bool IsUnrestricted() { return false; }
        public override IPermission Union(IPermission target) { return null; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public override IPermission Intersect(IPermission target) { return null; }
        public override IPermission Copy() { return null; }
        public override SecurityElement ToXml() { return null; }
        public override void FromXml(SecurityElement securityElement) { }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class StorePermissionAttribute : CodeAccessSecurityAttribute
    {
        public StorePermissionAttribute(SecurityAction action) : base(action) { }
        public StorePermissionFlags Flags { get; set; }
        public bool CreateStore { get; set; }
        public bool DeleteStore { get; set; }
        public bool EnumerateStores { get; set; }
        public bool OpenStore { get; set; }
        public bool AddToStore { get; set; }
        public bool RemoveFromStore { get; set; }
        public bool EnumerateCertificates { get; set; }
        public override IPermission CreatePermission() { return null; }
    }
    [Flags]
    public enum StorePermissionFlags
    {
        NoFlags = 0x00,

        CreateStore = 0x01,
        DeleteStore = 0x02,
        EnumerateStores = 0x04,

        OpenStore = 0x10,
        AddToStore = 0x20,
        RemoveFromStore = 0x40,
        EnumerateCertificates = 0x80,

        AllFlags = 0xF7
    }

    public sealed partial class StrongNameIdentityPermission : System.Security.CodeAccessPermission
    {
        public StrongNameIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public StrongNameIdentityPermission(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get; set; }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get; set; }
        public System.Version Version { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement e) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class StrongNameIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public StrongNameIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string Version { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class StrongNamePublicKeyBlob
    {
        public StrongNamePublicKeyBlob(byte[] publicKey) { }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class TypeDescriptorPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public TypeDescriptorPermission(System.Security.Permissions.PermissionState state) { }
        public TypeDescriptorPermission(System.Security.Permissions.TypeDescriptorPermissionFlags flag) { }
        public System.Security.Permissions.TypeDescriptorPermissionFlags Flags { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class TypeDescriptorPermissionAttribute : CodeAccessSecurityAttribute
    {
        public TypeDescriptorPermissionAttribute(SecurityAction action) : base(action) { }
        public TypeDescriptorPermissionFlags Flags { get; set; }
        public bool RestrictedRegistrationAccess { get; set; }
        public override IPermission CreatePermission() { return null; }
    }
    [System.FlagsAttribute]
    public enum TypeDescriptorPermissionFlags
    {
        NoFlags = 0,
        RestrictedRegistrationAccess = 1,
    }
    public sealed partial class UIPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public UIPermission(System.Security.Permissions.PermissionState state) { }
        public UIPermission(System.Security.Permissions.UIPermissionClipboard clipboardFlag) { }
        public UIPermission(System.Security.Permissions.UIPermissionWindow windowFlag) { }
        public UIPermission(System.Security.Permissions.UIPermissionWindow windowFlag, System.Security.Permissions.UIPermissionClipboard clipboardFlag) { }
        public System.Security.Permissions.UIPermissionClipboard Clipboard { get; set; }
        public System.Security.Permissions.UIPermissionWindow Window { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public bool IsUnrestricted() { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class UIPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public UIPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.Permissions.UIPermissionClipboard Clipboard { get; set; }
        public System.Security.Permissions.UIPermissionWindow Window { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public enum UIPermissionClipboard
    {
        AllClipboard = 2,
        NoClipboard = 0,
        OwnClipboard = 1,
    }
    public enum UIPermissionWindow
    {
        AllWindows = 3,
        NoWindows = 0,
        SafeSubWindows = 1,
        SafeTopLevelWindows = 2,
    }
    public sealed partial class UrlIdentityPermission : System.Security.CodeAccessPermission
    {
        public UrlIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public UrlIdentityPermission(string site) { }
        public string Url { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class UrlIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public UrlIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public string Url { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
    public sealed partial class ZoneIdentityPermission : System.Security.CodeAccessPermission
    {
        public ZoneIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public ZoneIdentityPermission(System.Security.SecurityZone zone) { }
        public System.Security.SecurityZone SecurityZone { get; set; }
        public override System.Security.IPermission Copy() { throw null; }
        public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { throw null; }
        public override bool IsSubsetOf(System.Security.IPermission target) { throw null; }
        public override System.Security.SecurityElement ToXml() { throw null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { throw null; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)109, AllowMultiple = true, Inherited = false)]
    public sealed partial class ZoneIdentityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public ZoneIdentityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(default(System.Security.Permissions.SecurityAction)) { }
        public System.Security.SecurityZone Zone { get; set; }
        public override System.Security.IPermission CreatePermission() { throw null; }
    }
}
namespace System.Security.Policy
{
    public sealed partial class AllMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public AllMembershipCondition() { }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class ApplicationDirectory : System.Security.Policy.EvidenceBase
    {
        public ApplicationDirectory(string name) { }
        public string Directory { get { throw null; } }
        public object Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class ApplicationDirectoryMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public ApplicationDirectoryMembershipCondition() { }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class ApplicationTrust : System.Security.Policy.EvidenceBase, System.Security.ISecurityEncodable
    {
        public ApplicationTrust() { }
        public ApplicationTrust(System.ApplicationIdentity identity) { }
        public ApplicationTrust(System.Security.PermissionSet defaultGrantSet, System.Collections.Generic.IEnumerable<System.Security.Policy.StrongName> fullTrustAssemblies) { }
        public System.ApplicationIdentity ApplicationIdentity { get { throw null; } set { } }
        public System.Security.Policy.PolicyStatement DefaultGrantSet { get { throw null; } set { } }
        public object ExtraInfo { get { throw null; } set { } }
        public System.Collections.Generic.IList<System.Security.Policy.StrongName> FullTrustAssemblies { get { throw null; } }
        public bool IsApplicationTrustedToRun { get { throw null; } set { } }
        public bool Persist { get { throw null; } set { } }
        public void FromXml(System.Security.SecurityElement element) { }
        public System.Security.SecurityElement ToXml() { throw null; }
    }
    public sealed partial class ApplicationTrustCollection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        internal ApplicationTrustCollection() { }
        public int Count { get { throw null; } }
        public bool IsSynchronized { get { throw null; } }
        public System.Security.Policy.ApplicationTrust this[int index] { get { throw null; } }
        public System.Security.Policy.ApplicationTrust this[string appFullName] { get { throw null; } }
        public object SyncRoot { get { throw null; } }
        public int Add(System.Security.Policy.ApplicationTrust trust) { throw null; }
        public void AddRange(System.Security.Policy.ApplicationTrust[] trusts) { }
        public void AddRange(System.Security.Policy.ApplicationTrustCollection trusts) { }
        public void Clear() { }
        public void CopyTo(System.Security.Policy.ApplicationTrust[] array, int index) { }
        [System.Security.SecurityCritical]
        public System.Security.Policy.ApplicationTrustCollection Find(System.ApplicationIdentity applicationIdentity, System.Security.Policy.ApplicationVersionMatch versionMatch) { throw null; }
        public System.Security.Policy.ApplicationTrustEnumerator GetEnumerator() { throw null; }
        public void Remove(System.Security.Policy.ApplicationTrust trust) { }
        [System.Security.SecurityCritical]
        public void Remove(System.ApplicationIdentity applicationIdentity, System.Security.Policy.ApplicationVersionMatch versionMatch) { }
        public void RemoveRange(System.Security.Policy.ApplicationTrust[] trusts) { }
        public void RemoveRange(System.Security.Policy.ApplicationTrustCollection trusts) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
    }
    public sealed partial class ApplicationTrustEnumerator : System.Collections.IEnumerator
    {
        internal ApplicationTrustEnumerator() { }
        public System.Security.Policy.ApplicationTrust Current { get { throw null; } }
        object System.Collections.IEnumerator.Current { get { throw null; } }
        public bool MoveNext() { throw null; }
        public void Reset() { }
    }
    public enum ApplicationVersionMatch
    {
        MatchAllVersions = 1,
        MatchExactVersion = 0,
    }
    public partial class CodeConnectAccess
    {
        public static readonly string AnyScheme;
        public static readonly int DefaultPort;
        public static readonly int OriginPort;
        public static readonly string OriginScheme;
        public CodeConnectAccess(string allowScheme, int allowPort) { }
        public int Port { get { throw null; } }
        public string Scheme { get { throw null; } }
        public static System.Security.Policy.CodeConnectAccess CreateAnySchemeAccess(int allowPort) { throw null; }
        public static System.Security.Policy.CodeConnectAccess CreateOriginSchemeAccess(int allowPort) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
    }
    public abstract partial class CodeGroup
    {
        protected CodeGroup(System.Security.Policy.IMembershipCondition membershipCondition, System.Security.Policy.PolicyStatement policy) { }
        public virtual string AttributeString { get { throw null; } }
        public System.Collections.IList Children { get; set; }
        public string Description { get; set; }
        public System.Security.Policy.IMembershipCondition MembershipCondition { get; set; }
        public abstract string MergeLogic { get; }
        public string Name { get; set; }
        public virtual string PermissionSetName { get { throw null; } }
        public System.Security.Policy.PolicyStatement PolicyStatement { get; set; }
        public void AddChild(System.Security.Policy.CodeGroup group) { }
        public abstract System.Security.Policy.CodeGroup Copy();
        protected virtual void CreateXml(System.Security.SecurityElement element, System.Security.Policy.PolicyLevel level) { }
        public override bool Equals(object o) => base.Equals(o);
        public bool Equals(System.Security.Policy.CodeGroup cg, bool compareChildren) { throw null; }
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        protected virtual void ParseXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public void RemoveChild(System.Security.Policy.CodeGroup group) { }
        public abstract System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence);
        public abstract System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence);
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class Evidence : System.Collections.ICollection, System.Collections.IEnumerable
    {
        public Evidence() { }
        [System.ObsoleteAttribute]
        public Evidence(object[] hostEvidence, object[] assemblyEvidence) { }
        public Evidence(System.Security.Policy.Evidence evidence) { }
        public Evidence(System.Security.Policy.EvidenceBase[] hostEvidence, System.Security.Policy.EvidenceBase[] assemblyEvidence) { }
        [System.ObsoleteAttribute]
        public int Count { get { throw null; } }
        public bool IsReadOnly { get { throw null; } }
        public bool IsSynchronized { get { throw null; } }
        public bool Locked { get { throw null; } set { } }
        public object SyncRoot { get { throw null; } }
        [System.ObsoleteAttribute]
        public void AddAssembly(object id) { }
        public void AddAssemblyEvidence<T>(T evidence) where T : System.Security.Policy.EvidenceBase { }
        [System.ObsoleteAttribute]
        public void AddHost(object id) { }
        public void AddHostEvidence<T>(T evidence) where T : System.Security.Policy.EvidenceBase { }
        public void Clear() { }
        public System.Security.Policy.Evidence Clone() { throw null; }
        [System.ObsoleteAttribute]
        public void CopyTo(System.Array array, int index) { }
        public System.Collections.IEnumerator GetAssemblyEnumerator() { throw null; }
        public T GetAssemblyEvidence<T>() where T : System.Security.Policy.EvidenceBase { throw null; }
        [System.ObsoleteAttribute]
        public System.Collections.IEnumerator GetEnumerator() { throw null; }
        public System.Collections.IEnumerator GetHostEnumerator() { throw null; }
        public T GetHostEvidence<T>() where T : System.Security.Policy.EvidenceBase { throw null; }
        public void Merge(System.Security.Policy.Evidence evidence) { }
        public void RemoveType(System.Type t) { }
    }
    public abstract partial class EvidenceBase
    {
        protected EvidenceBase() { }
        public virtual System.Security.Policy.EvidenceBase Clone() { throw null; }
    }
    public sealed partial class FileCodeGroup : System.Security.Policy.CodeGroup
    {
        public FileCodeGroup(System.Security.Policy.IMembershipCondition membershipCondition, System.Security.Permissions.FileIOPermissionAccess access) : base(default(System.Security.Policy.IMembershipCondition), default(System.Security.Policy.PolicyStatement)) { }
        public override string AttributeString { get { throw null; } }
        public override string MergeLogic { get { throw null; } }
        public override string PermissionSetName { get { throw null; } }
        public override System.Security.Policy.CodeGroup Copy() { throw null; }
        protected override void CreateXml(System.Security.SecurityElement element, System.Security.Policy.PolicyLevel level) { }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        protected override void ParseXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { throw null; }
        public override System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { throw null; }
    }
    public sealed partial class FirstMatchCodeGroup : System.Security.Policy.CodeGroup
    {
        public FirstMatchCodeGroup(System.Security.Policy.IMembershipCondition membershipCondition, System.Security.Policy.PolicyStatement policy) : base(default(System.Security.Policy.IMembershipCondition), default(System.Security.Policy.PolicyStatement)) { }
        public override string MergeLogic { get { throw null; } }
        public override System.Security.Policy.CodeGroup Copy() { throw null; }
        public override System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { throw null; }
        public override System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { throw null; }
    }
    public sealed partial class GacInstalled : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public GacInstalled() { }
        public object Copy() { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class GacMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public GacMembershipCondition() { }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class Hash : System.Security.Policy.EvidenceBase, System.Runtime.Serialization.ISerializable
    {
        public Hash(System.Reflection.Assembly assembly) { }
        public byte[] MD5 { get { throw null; } }
        public byte[] SHA1 { get { throw null; } }
        public byte[] SHA256 { get { throw null; } }
        public static System.Security.Policy.Hash CreateMD5(byte[] md5) { throw null; }
        public static System.Security.Policy.Hash CreateSHA1(byte[] sha1) { throw null; }
        public static System.Security.Policy.Hash CreateSHA256(byte[] sha256) { throw null; }
        public byte[] GenerateHash(System.Security.Cryptography.HashAlgorithm hashAlg) { throw null; }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() { throw null; }
    }
    public sealed partial class HashMembershipCondition : System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable, System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public HashMembershipCondition(System.Security.Cryptography.HashAlgorithm hashAlg, byte[] value) { }
        public System.Security.Cryptography.HashAlgorithm HashAlgorithm { get; set; }
        public byte[] HashValue { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public partial interface IIdentityPermissionFactory
    {
        System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence);
    }
    public partial interface IMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable
    {
        bool Check(System.Security.Policy.Evidence evidence);
        System.Security.Policy.IMembershipCondition Copy();
        bool Equals(object obj);
        string ToString();
    }
    public sealed partial class NetCodeGroup : System.Security.Policy.CodeGroup
    {
        public static readonly string AbsentOriginScheme;
        public static readonly string AnyOtherOriginScheme;
        public NetCodeGroup(System.Security.Policy.IMembershipCondition membershipCondition) : base(default(System.Security.Policy.IMembershipCondition), default(System.Security.Policy.PolicyStatement)) { }
        public override string AttributeString { get { throw null; } }
        public override string MergeLogic { get { throw null; } }
        public override string PermissionSetName { get { throw null; } }
        public void AddConnectAccess(string originScheme, System.Security.Policy.CodeConnectAccess connectAccess) { }
        public override System.Security.Policy.CodeGroup Copy() { throw null; }
        protected override void CreateXml(System.Security.SecurityElement element, System.Security.Policy.PolicyLevel level) { }
        public override bool Equals(object o) => base.Equals(o);
        public System.Collections.DictionaryEntry[] GetConnectAccessRules() { throw null; }
        public override int GetHashCode() => base.GetHashCode();
        protected override void ParseXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public void ResetConnectAccess() { }
        public override System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { throw null; }
        public override System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { throw null; }
    }
    public sealed partial class PermissionRequestEvidence : System.Security.Policy.EvidenceBase
    {
        public PermissionRequestEvidence(System.Security.PermissionSet request, System.Security.PermissionSet optional, System.Security.PermissionSet denied) { }
        public System.Security.PermissionSet DeniedPermissions { get { throw null; } }
        public System.Security.PermissionSet OptionalPermissions { get { throw null; } }
        public System.Security.PermissionSet RequestedPermissions { get { throw null; } }
        public System.Security.Policy.PermissionRequestEvidence Copy() { throw null; }
        public override string ToString() => base.ToString();
    }
    public partial class PolicyException : System.SystemException
    {
        public PolicyException() { }
        protected PolicyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public PolicyException(string message) : base(message) { }
        public PolicyException(string message, System.Exception exception) : base(message, exception) { }
    }
    public sealed partial class PolicyLevel
    {
        internal PolicyLevel() { }
        [System.ObsoleteAttribute]
        public System.Collections.IList FullTrustAssemblies { get { throw null; } }
        public string Label { get { throw null; } }
        public System.Collections.IList NamedPermissionSets { get { throw null; } }
        public System.Security.Policy.CodeGroup RootCodeGroup { get; set; }
        public string StoreLocation { get { throw null; } }
        public System.Security.PolicyLevelType Type { get { throw null; } }
        [System.ObsoleteAttribute]
        public void AddFullTrustAssembly(System.Security.Policy.StrongName sn) { }
        [System.ObsoleteAttribute]
        public void AddFullTrustAssembly(System.Security.Policy.StrongNameMembershipCondition snMC) { }
        public void AddNamedPermissionSet(System.Security.NamedPermissionSet permSet) { }
        public System.Security.NamedPermissionSet ChangeNamedPermissionSet(string name, System.Security.PermissionSet pSet) { throw null; }
        public static System.Security.Policy.PolicyLevel CreateAppDomainLevel() { throw null; }
        public void FromXml(System.Security.SecurityElement e) { }
        public System.Security.NamedPermissionSet GetNamedPermissionSet(string name) { throw null; }
        public void Recover() { }
        [System.ObsoleteAttribute]
        public void RemoveFullTrustAssembly(System.Security.Policy.StrongName sn) { }
        [System.ObsoleteAttribute]
        public void RemoveFullTrustAssembly(System.Security.Policy.StrongNameMembershipCondition snMC) { }
        public System.Security.NamedPermissionSet RemoveNamedPermissionSet(System.Security.NamedPermissionSet permSet) { throw null; }
        public System.Security.NamedPermissionSet RemoveNamedPermissionSet(string name) { throw null; }
        public void Reset() { }
        public System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.SecurityElement ToXml() { throw null; }
    }
    public sealed partial class PolicyStatement : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable
    {
        public PolicyStatement(System.Security.PermissionSet permSet) { }
        public PolicyStatement(System.Security.PermissionSet permSet, System.Security.Policy.PolicyStatementAttribute attributes) { }
        public System.Security.Policy.PolicyStatementAttribute Attributes { get; set; }
        public string AttributeString { get { throw null; } }
        public System.Security.PermissionSet PermissionSet { get; set; }
        public System.Security.Policy.PolicyStatement Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement et) { }
        public void FromXml(System.Security.SecurityElement et, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    [System.FlagsAttribute]
    public enum PolicyStatementAttribute
    {
        All = 3,
        Exclusive = 1,
        LevelFinal = 2,
        Nothing = 0,
    }
    public sealed partial class Publisher : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Publisher(System.Security.Cryptography.X509Certificates.X509Certificate cert) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate Certificate { get { throw null; } }
        public object Copy() { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class PublisherMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public PublisherMembershipCondition(System.Security.Cryptography.X509Certificates.X509Certificate certificate) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate Certificate { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class Site : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Site(string name) { }
        public string Name { get { throw null; } }
        public object Copy() { throw null; }
        public static System.Security.Policy.Site CreateFromUrl(string url) { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class SiteMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public SiteMembershipCondition(string site) { }
        public string Site { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class StrongName : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public StrongName(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get { throw null; } }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get { throw null; } }
        public System.Version Version { get { throw null; } }
        public object Copy() { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class StrongNameMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public StrongNameMembershipCondition(System.Security.Permissions.StrongNamePublicKeyBlob blob, string name, System.Version version) { }
        public string Name { get; set; }
        public System.Security.Permissions.StrongNamePublicKeyBlob PublicKey { get; set; }
        public System.Version Version { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public partial class TrustManagerContext
    {
        public TrustManagerContext() { }
        public TrustManagerContext(System.Security.Policy.TrustManagerUIContext uiContext) { }
        public virtual bool IgnorePersistedDecision { get { throw null; } set { } }
        public virtual bool KeepAlive { get { throw null; } set { } }
        public virtual bool NoPrompt { get { throw null; } set { } }
        public virtual bool Persist { get { throw null; } set { } }
        public virtual System.ApplicationIdentity PreviousApplicationIdentity { get { throw null; } set { } }
        public virtual System.Security.Policy.TrustManagerUIContext UIContext { get { throw null; } set { } }
    }
    public enum TrustManagerUIContext
    {
        Install = 0,
        Run = 2,
        Upgrade = 1,
    }
    public sealed partial class UnionCodeGroup : System.Security.Policy.CodeGroup
    {
        public UnionCodeGroup(System.Security.Policy.IMembershipCondition membershipCondition, System.Security.Policy.PolicyStatement policy) : base(default(System.Security.Policy.IMembershipCondition), default(System.Security.Policy.PolicyStatement)) { }
        public override string MergeLogic { get { throw null; } }
        public override System.Security.Policy.CodeGroup Copy() { throw null; }
        public override System.Security.Policy.PolicyStatement Resolve(System.Security.Policy.Evidence evidence) { throw null; }
        public override System.Security.Policy.CodeGroup ResolveMatchingCodeGroups(System.Security.Policy.Evidence evidence) { throw null; }
    }
    public sealed partial class Url : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Url(string name) { }
        public string Value { get { throw null; } }
        public object Copy() { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class UrlMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public UrlMembershipCondition(string url) { }
        public string Url { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
    public sealed partial class Zone : System.Security.Policy.EvidenceBase, System.Security.Policy.IIdentityPermissionFactory
    {
        public Zone(System.Security.SecurityZone zone) { }
        public System.Security.SecurityZone SecurityZone { get { throw null; } }
        public object Copy() { throw null; }
        public static System.Security.Policy.Zone CreateFromUrl(string url) { throw null; }
        public System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence) { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
    }
    public sealed partial class ZoneMembershipCondition : System.Security.ISecurityEncodable, System.Security.ISecurityPolicyEncodable, System.Security.Policy.IMembershipCondition
    {
        public ZoneMembershipCondition(System.Security.SecurityZone zone) { }
        public System.Security.SecurityZone SecurityZone { get; set; }
        public bool Check(System.Security.Policy.Evidence evidence) { throw null; }
        public System.Security.Policy.IMembershipCondition Copy() { throw null; }
        public override bool Equals(object o) => base.Equals(o);
        public void FromXml(System.Security.SecurityElement e) { }
        public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level) { }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => base.ToString();
        public System.Security.SecurityElement ToXml() { throw null; }
        public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level) { throw null; }
    }
}
namespace System.Transactions
{
    public sealed partial class DistributedTransactionPermission : System.Security.CodeAccessPermission, System.Security.Permissions.IUnrestrictedPermission
    {
        public DistributedTransactionPermission(System.Security.Permissions.PermissionState state) { }
        public override System.Security.IPermission Copy() { return null; }
        public override void FromXml(System.Security.SecurityElement securityElement) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return null; }
        public override bool IsSubsetOf(System.Security.IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override System.Security.SecurityElement ToXml() { return null; }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return null; }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed partial class DistributedTransactionPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public DistributedTransactionPermissionAttribute(System.Security.Permissions.SecurityAction action) : base(action) { }
        public new bool Unrestricted { get; set; }
        public override System.Security.IPermission CreatePermission() { return null; }
    }
}
