// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Net.Sockets
{
    public enum IOControlCode : long
    {
        AbsorbRouterAlert = (long)2550136837,
        AddMulticastGroupOnInterface = (long)2550136842,
        AddressListChange = (long)671088663,
        AddressListQuery = (long)1207959574,
        AddressListSort = (long)3355443225,
        AssociateHandle = (long)2281701377,
        AsyncIO = (long)2147772029,
        BindToInterface = (long)2550136840,
        DataToRead = (long)1074030207,
        DeleteMulticastGroupFromInterface = (long)2550136843,
        EnableCircularQueuing = (long)671088642,
        Flush = (long)671088644,
        GetBroadcastAddress = (long)1207959557,
        GetExtensionFunctionPointer = (long)3355443206,
        GetGroupQos = (long)3355443208,
        GetQos = (long)3355443207,
        KeepAliveValues = (long)2550136836,
        LimitBroadcasts = (long)2550136839,
        MulticastInterface = (long)2550136841,
        MulticastScope = (long)2281701386,
        MultipointLoopback = (long)2281701385,
        NamespaceChange = (long)2281701401,
        NonBlockingIO = (long)2147772030,
        OobDataRead = (long)1074033415,
        QueryTargetPnpHandle = (long)1207959576,
        ReceiveAll = (long)2550136833,
        ReceiveAllIgmpMulticast = (long)2550136835,
        ReceiveAllMulticast = (long)2550136834,
        RoutingInterfaceChange = (long)2281701397,
        RoutingInterfaceQuery = (long)3355443220,
        SetGroupQos = (long)2281701388,
        SetQos = (long)2281701387,
        TranslateHandle = (long)3355443213,
        UnicastInterface = (long)2550136838,
    }
    public partial struct IPPacketInformation
    {
        private object _dummy;
        public System.Net.IPAddress Address { get { throw null; } }
        public int Interface { get { throw null; } }
        public override bool Equals(object comparand) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(System.Net.Sockets.IPPacketInformation packetInformation1, System.Net.Sockets.IPPacketInformation packetInformation2) { throw null; }
        public static bool operator !=(System.Net.Sockets.IPPacketInformation packetInformation1, System.Net.Sockets.IPPacketInformation packetInformation2) { throw null; }
    }
    public enum IPProtectionLevel
    {
        EdgeRestricted = 20,
        Restricted = 30,
        Unrestricted = 10,
        Unspecified = -1,
    }
    public partial class IPv6MulticastOption
    {
        public IPv6MulticastOption(System.Net.IPAddress group) { }
        public IPv6MulticastOption(System.Net.IPAddress group, long ifindex) { }
        public System.Net.IPAddress Group { get { throw null; } set { } }
        public long InterfaceIndex { get { throw null; } set { } }
    }
    public partial class LingerOption
    {
        public LingerOption(bool enable, int seconds) { }
        public bool Enabled { get { throw null; } set { } }
        public int LingerTime { get { throw null; } set { } }
    }
    public partial class MulticastOption
    {
        public MulticastOption(System.Net.IPAddress group) { }
        public MulticastOption(System.Net.IPAddress group, int interfaceIndex) { }
        public MulticastOption(System.Net.IPAddress group, System.Net.IPAddress mcint) { }
        public System.Net.IPAddress Group { get { throw null; } set { } }
        public int InterfaceIndex { get { throw null; } set { } }
        public System.Net.IPAddress LocalAddress { get { throw null; } set { } }
    }
    public partial class NetworkStream : System.IO.Stream
    {
        public NetworkStream(System.Net.Sockets.Socket socket) { }
        public NetworkStream(System.Net.Sockets.Socket socket, bool ownsSocket) { }
        public NetworkStream(System.Net.Sockets.Socket socket, System.IO.FileAccess access) { }
        public NetworkStream(System.Net.Sockets.Socket socket, System.IO.FileAccess access, bool ownsSocket) { }
        public override bool CanRead { get { throw null; } }
        public override bool CanSeek { get { throw null; } }
        public override bool CanTimeout { get { throw null; } }
        public override bool CanWrite { get { throw null; } }
        public virtual bool DataAvailable { get { throw null; } }
        public override long Length { get { throw null; } }
        public override long Position { get { throw null; } set { } }
        protected bool Readable { get { throw null; } set { } }
        public override int ReadTimeout { get { throw null; } set { } }
        protected System.Net.Sockets.Socket Socket { get { throw null; } }
        protected bool Writeable { get { throw null; } set { } }
        public override int WriteTimeout { get { throw null; } set { } }
        public override System.IAsyncResult BeginRead(byte[] buffer, int offset, int size, System.AsyncCallback callback, object state) { throw null; }
        public override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int size, System.AsyncCallback callback, object state) { throw null; }
        public void Close(int timeout) { }
        protected override void Dispose(bool disposing) { }
        public override int EndRead(System.IAsyncResult asyncResult) { throw null; }
        public override void EndWrite(System.IAsyncResult asyncResult) { }
        ~NetworkStream() { }
        public override void Flush() { }
        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { throw null; }
        public override int Read(byte[] buffer, int offset, int size) { throw null; }
        public override int Read(System.Span<byte> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int size, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override int ReadByte() { throw null; }
        public override long Seek(long offset, System.IO.SeekOrigin origin) { throw null; }
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int size) { }
        public override void Write(System.ReadOnlySpan<byte> buffer) { }
        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int size, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override void WriteByte(byte value) { }
    }
    public enum ProtocolFamily
    {
        AppleTalk = 16,
        Atm = 22,
        Banyan = 21,
        Ccitt = 10,
        Chaos = 5,
        Cluster = 24,
        DataKit = 9,
        DataLink = 13,
        DecNet = 12,
        Ecma = 8,
        FireFox = 19,
        HyperChannel = 15,
        Ieee12844 = 25,
        ImpLink = 3,
        InterNetwork = 2,
        InterNetworkV6 = 23,
        Ipx = 6,
        Irda = 26,
        Iso = 7,
        Lat = 14,
        Max = 29,
        NetBios = 17,
        NetworkDesigners = 28,
        NS = 6,
        Osi = 7,
        Pup = 4,
        Sna = 11,
        Unix = 1,
        Unknown = -1,
        Unspecified = 0,
        VoiceView = 18,
    }
    public enum ProtocolType
    {
        Ggp = 3,
        Icmp = 1,
        IcmpV6 = 58,
        Idp = 22,
        Igmp = 2,
        IP = 0,
        IPSecAuthenticationHeader = 51,
        IPSecEncapsulatingSecurityPayload = 50,
        IPv4 = 4,
        IPv6 = 41,
        IPv6DestinationOptions = 60,
        IPv6FragmentHeader = 44,
        IPv6HopByHopOptions = 0,
        IPv6NoNextHeader = 59,
        IPv6RoutingHeader = 43,
        Ipx = 1000,
        ND = 77,
        Pup = 12,
        Raw = 255,
        Spx = 1256,
        SpxII = 1257,
        Tcp = 6,
        Udp = 17,
        Unknown = -1,
        Unspecified = 0,
    }
    public sealed partial class SafeSocketHandle : Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid
    {
        public SafeSocketHandle(System.IntPtr preexistingHandle, bool ownsHandle) : base (default(bool)) { }
        protected override bool ReleaseHandle() { throw null; }
    }
    public enum SelectMode
    {
        SelectError = 2,
        SelectRead = 0,
        SelectWrite = 1,
    }
    public partial class SendPacketsElement
    {
        public SendPacketsElement(byte[] buffer) { }
        public SendPacketsElement(byte[] buffer, int offset, int count) { }
        public SendPacketsElement(byte[] buffer, int offset, int count, bool endOfPacket) { }
        public SendPacketsElement(System.IO.FileStream fileStream) { }
        public SendPacketsElement(System.IO.FileStream fileStream, long offset, int count) { }
        public SendPacketsElement(System.IO.FileStream fileStream, long offset, int count, bool endOfPacket) { }
        public SendPacketsElement(string filepath) { }
        public SendPacketsElement(string filepath, int offset, int count) { }
        public SendPacketsElement(string filepath, int offset, int count, bool endOfPacket) { }
        public SendPacketsElement(string filepath, long offset, int count) { }
        public SendPacketsElement(string filepath, long offset, int count, bool endOfPacket) { }
        public byte[] Buffer { get { throw null; } }
        public int Count { get { throw null; } }
        public bool EndOfPacket { get { throw null; } }
        public string FilePath { get { throw null; } }
        public System.IO.FileStream FileStream { get { throw null; } }
        public int Offset { get { throw null; } }
        public long OffsetLong { get { throw null; } }
    }
    public partial class Socket : System.IDisposable
    {
        public Socket(System.Net.Sockets.AddressFamily addressFamily, System.Net.Sockets.SocketType socketType, System.Net.Sockets.ProtocolType protocolType) { }
        public Socket(System.Net.Sockets.SocketInformation socketInformation) { }
        public Socket(System.Net.Sockets.SocketType socketType, System.Net.Sockets.ProtocolType protocolType) { }
        public System.Net.Sockets.AddressFamily AddressFamily { get { throw null; } }
        public int Available { get { throw null; } }
        public bool Blocking { get { throw null; } set { } }
        public bool Connected { get { throw null; } }
        public bool DontFragment { get { throw null; } set { } }
        public bool DualMode { get { throw null; } set { } }
        public bool EnableBroadcast { get { throw null; } set { } }
        public bool ExclusiveAddressUse { get { throw null; } set { } }
        public System.IntPtr Handle { get { throw null; } }
        public bool IsBound { get { throw null; } }
        public System.Net.Sockets.LingerOption LingerState { get { throw null; } set { } }
        public System.Net.EndPoint LocalEndPoint { get { throw null; } }
        public bool MulticastLoopback { get { throw null; } set { } }
        public bool NoDelay { get { throw null; } set { } }
        public static bool OSSupportsIPv4 { get { throw null; } }
        public static bool OSSupportsIPv6 { get { throw null; } }
        public System.Net.Sockets.ProtocolType ProtocolType { get { throw null; } }
        public int ReceiveBufferSize { get { throw null; } set { } }
        public int ReceiveTimeout { get { throw null; } set { } }
        public System.Net.EndPoint RemoteEndPoint { get { throw null; } }
        public System.Net.Sockets.SafeSocketHandle SafeHandle { get { throw null; } }
        public int SendBufferSize { get { throw null; } set { } }
        public int SendTimeout { get { throw null; } set { } }
        public System.Net.Sockets.SocketType SocketType { get { throw null; } }
        [System.ObsoleteAttribute("SupportsIPv4 is obsoleted for this type, please use OSSupportsIPv4 instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        public static bool SupportsIPv4 { get { throw null; } }
        [System.ObsoleteAttribute("SupportsIPv6 is obsoleted for this type, please use OSSupportsIPv6 instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        public static bool SupportsIPv6 { get { throw null; } }
        public short Ttl { get { throw null; } set { } }
        public bool UseOnlyOverlappedIO { get { throw null; } set { } }
        public System.Net.Sockets.Socket Accept() { throw null; }
        public bool AcceptAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public System.IAsyncResult BeginAccept(System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginAccept(int receiveSize, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginAccept(System.Net.Sockets.Socket acceptSocket, int receiveSize, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(System.Net.EndPoint remoteEP, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(System.Net.IPAddress address, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(System.Net.IPAddress[] addresses, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(string host, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginDisconnect(bool reuseSocket, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceive(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceive(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceive(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceive(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSend(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSend(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSend(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSend(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSendFile(string fileName, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, System.Net.Sockets.TransmitFileOptions flags, System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEP, System.AsyncCallback callback, object state) { throw null; }
        public void Bind(System.Net.EndPoint localEP) { }
        public static void CancelConnectAsync(System.Net.Sockets.SocketAsyncEventArgs e) { }
        public void Close() { }
        public void Close(int timeout) { }
        public void Connect(System.Net.EndPoint remoteEP) { }
        public void Connect(System.Net.IPAddress address, int port) { }
        public void Connect(System.Net.IPAddress[] addresses, int port) { }
        public void Connect(string host, int port) { }
        public bool ConnectAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public static bool ConnectAsync(System.Net.Sockets.SocketType socketType, System.Net.Sockets.ProtocolType protocolType, System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public void Disconnect(bool reuseSocket) { }
        public bool DisconnectAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public System.Net.Sockets.SocketInformation DuplicateAndClose(int targetProcessId) { throw null; }
        public System.Net.Sockets.Socket EndAccept(out byte[] buffer, System.IAsyncResult asyncResult) { throw null; }
        public System.Net.Sockets.Socket EndAccept(out byte[] buffer, out int bytesTransferred, System.IAsyncResult asyncResult) { throw null; }
        public System.Net.Sockets.Socket EndAccept(System.IAsyncResult asyncResult) { throw null; }
        public void EndConnect(System.IAsyncResult asyncResult) { }
        public void EndDisconnect(System.IAsyncResult asyncResult) { }
        public int EndReceive(System.IAsyncResult asyncResult) { throw null; }
        public int EndReceive(System.IAsyncResult asyncResult, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public int EndReceiveFrom(System.IAsyncResult asyncResult, ref System.Net.EndPoint endPoint) { throw null; }
        public int EndReceiveMessageFrom(System.IAsyncResult asyncResult, ref System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint endPoint, out System.Net.Sockets.IPPacketInformation ipPacketInformation) { throw null; }
        public int EndSend(System.IAsyncResult asyncResult) { throw null; }
        public int EndSend(System.IAsyncResult asyncResult, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public void EndSendFile(System.IAsyncResult asyncResult) { }
        public int EndSendTo(System.IAsyncResult asyncResult) { throw null; }
        ~Socket() { }
        public object GetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName) { throw null; }
        public void GetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, byte[] optionValue) { }
        public byte[] GetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, int optionLength) { throw null; }
        public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue) { throw null; }
        public int IOControl(System.Net.Sockets.IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue) { throw null; }
        public void Listen(int backlog) { }
        public bool Poll(int microSeconds, System.Net.Sockets.SelectMode mode) { throw null; }
        public int Receive(byte[] buffer) { throw null; }
        public int Receive(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Receive(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public int Receive(byte[] buffer, int size, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Receive(byte[] buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Receive(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers) { throw null; }
        public int Receive(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Receive(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public int Receive(System.Span<byte> buffer) { throw null; }
        public int Receive(System.Span<byte> buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Receive(System.Span<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public bool ReceiveAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public int ReceiveFrom(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP) { throw null; }
        public int ReceiveFrom(byte[] buffer, int size, System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP) { throw null; }
        public int ReceiveFrom(byte[] buffer, ref System.Net.EndPoint remoteEP) { throw null; }
        public int ReceiveFrom(byte[] buffer, System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP) { throw null; }
        public bool ReceiveFromAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref System.Net.Sockets.SocketFlags socketFlags, ref System.Net.EndPoint remoteEP, out System.Net.Sockets.IPPacketInformation ipPacketInformation) { throw null; }
        public bool ReceiveMessageFromAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public static void Select(System.Collections.IList checkRead, System.Collections.IList checkWrite, System.Collections.IList checkError, int microSeconds) { }
        public int Send(byte[] buffer) { throw null; }
        public int Send(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Send(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public int Send(byte[] buffer, int size, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Send(byte[] buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Send(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers) { throw null; }
        public int Send(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Send(System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public int Send(System.ReadOnlySpan<byte> buffer) { throw null; }
        public int Send(System.ReadOnlySpan<byte> buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public int Send(System.ReadOnlySpan<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, out System.Net.Sockets.SocketError errorCode) { throw null; }
        public bool SendAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public void SendFile(string fileName) { }
        public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, System.Net.Sockets.TransmitFileOptions flags) { }
        public bool SendPacketsAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public int SendTo(byte[] buffer, int offset, int size, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEP) { throw null; }
        public int SendTo(byte[] buffer, int size, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEP) { throw null; }
        public int SendTo(byte[] buffer, System.Net.EndPoint remoteEP) { throw null; }
        public int SendTo(byte[] buffer, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEP) { throw null; }
        public bool SendToAsync(System.Net.Sockets.SocketAsyncEventArgs e) { throw null; }
        public void SetIPProtectionLevel(System.Net.Sockets.IPProtectionLevel level) { }
        public void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, bool optionValue) { }
        public void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, byte[] optionValue) { }
        public void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, int optionValue) { }
        public void SetSocketOption(System.Net.Sockets.SocketOptionLevel optionLevel, System.Net.Sockets.SocketOptionName optionName, object optionValue) { }
        public void Shutdown(System.Net.Sockets.SocketShutdown how) { }
    }
    public partial class SocketAsyncEventArgs : System.EventArgs, System.IDisposable
    {
        public SocketAsyncEventArgs() { }
        public System.Net.Sockets.Socket AcceptSocket { get { throw null; } set { } }
        public byte[] Buffer { get { throw null; } }
        public System.Collections.Generic.IList<System.ArraySegment<byte>> BufferList { get { throw null; } set { } }
        public int BytesTransferred { get { throw null; } }
        public System.Exception ConnectByNameError { get { throw null; } }
        public System.Net.Sockets.Socket ConnectSocket { get { throw null; } }
        public int Count { get { throw null; } }
        public bool DisconnectReuseSocket { get { throw null; } set { } }
        public System.Net.Sockets.SocketAsyncOperation LastOperation { get { throw null; } }
        public System.Memory<byte> MemoryBuffer { get { throw null; } }
        public int Offset { get { throw null; } }
        public System.Net.Sockets.IPPacketInformation ReceiveMessageFromPacketInfo { get { throw null; } }
        public System.Net.EndPoint RemoteEndPoint { get { throw null; } set { } }
        public System.Net.Sockets.SendPacketsElement[] SendPacketsElements { get { throw null; } set { } }
        public System.Net.Sockets.TransmitFileOptions SendPacketsFlags { get { throw null; } set { } }
        public int SendPacketsSendSize { get { throw null; } set { } }
        public System.Net.Sockets.SocketError SocketError { get { throw null; } set { } }
        public System.Net.Sockets.SocketFlags SocketFlags { get { throw null; } set { } }
        public object UserToken { get { throw null; } set { } }
        public event System.EventHandler<System.Net.Sockets.SocketAsyncEventArgs> Completed { add { } remove { } }
        public void Dispose() { }
        ~SocketAsyncEventArgs() { }
        protected virtual void OnCompleted(System.Net.Sockets.SocketAsyncEventArgs e) { }
        public void SetBuffer(byte[] buffer, int offset, int count) { }
        public void SetBuffer(int offset, int count) { }
        public void SetBuffer(System.Memory<byte> buffer) { }
    }
    public enum SocketAsyncOperation
    {
        Accept = 1,
        Connect = 2,
        Disconnect = 3,
        None = 0,
        Receive = 4,
        ReceiveFrom = 5,
        ReceiveMessageFrom = 6,
        Send = 7,
        SendPackets = 8,
        SendTo = 9,
    }
    [System.FlagsAttribute]
    public enum SocketFlags
    {
        Broadcast = 1024,
        ControlDataTruncated = 512,
        DontRoute = 4,
        Multicast = 2048,
        None = 0,
        OutOfBand = 1,
        Partial = 32768,
        Peek = 2,
        Truncated = 256,
    }
    public partial struct SocketInformation
    {
        private object _dummy;
        public System.Net.Sockets.SocketInformationOptions Options { get { throw null; } set { } }
        public byte[] ProtocolInformation { get { throw null; } set { } }
    }
    [System.FlagsAttribute]
    public enum SocketInformationOptions
    {
        Connected = 2,
        Listening = 4,
        NonBlocking = 1,
        UseOnlyOverlappedIO = 8,
    }
    public enum SocketOptionLevel
    {
        IP = 0,
        IPv6 = 41,
        Socket = 65535,
        Tcp = 6,
        Udp = 17,
    }
    public enum SocketOptionName
    {
        AcceptConnection = 2,
        AddMembership = 12,
        AddSourceMembership = 15,
        BlockSource = 17,
        Broadcast = 32,
        BsdUrgent = 2,
        ChecksumCoverage = 20,
        Debug = 1,
        DontFragment = 14,
        DontLinger = -129,
        DontRoute = 16,
        DropMembership = 13,
        DropSourceMembership = 16,
        Error = 4103,
        ExclusiveAddressUse = -5,
        Expedited = 2,
        HeaderIncluded = 2,
        HopLimit = 21,
        IPOptions = 1,
        IPProtectionLevel = 23,
        IpTimeToLive = 4,
        IPv6Only = 27,
        KeepAlive = 8,
        Linger = 128,
        MaxConnections = 2147483647,
        MulticastInterface = 9,
        MulticastLoopback = 11,
        MulticastTimeToLive = 10,
        NoChecksum = 1,
        NoDelay = 1,
        OutOfBandInline = 256,
        PacketInformation = 19,
        ReceiveBuffer = 4098,
        ReceiveLowWater = 4100,
        ReceiveTimeout = 4102,
        ReuseAddress = 4,
        ReuseUnicastPort = 12295,
        SendBuffer = 4097,
        SendLowWater = 4099,
        SendTimeout = 4101,
        TcpKeepAliveInterval = 17,
        TcpKeepAliveRetryCount = 16,
        TcpKeepAliveTime = 3,
        Type = 4104,
        TypeOfService = 3,
        UnblockSource = 18,
        UpdateAcceptContext = 28683,
        UpdateConnectContext = 28688,
        UseLoopback = 64,
    }
    public partial struct SocketReceiveFromResult
    {
        public int ReceivedBytes;
        public System.Net.EndPoint RemoteEndPoint;
    }
    public partial struct SocketReceiveMessageFromResult
    {
        public System.Net.Sockets.IPPacketInformation PacketInformation;
        public int ReceivedBytes;
        public System.Net.EndPoint RemoteEndPoint;
        public System.Net.Sockets.SocketFlags SocketFlags;
    }
    public enum SocketShutdown
    {
        Both = 2,
        Receive = 0,
        Send = 1,
    }
    public static partial class SocketTaskExtensions
    {
        public static System.Threading.Tasks.Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Sockets.Socket> AcceptAsync(this System.Net.Sockets.Socket socket, System.Net.Sockets.Socket acceptSocket) { throw null; }
        public static System.Threading.Tasks.Task ConnectAsync(this System.Net.Sockets.Socket socket, System.Net.EndPoint remoteEP) { throw null; }
        public static System.Threading.Tasks.Task ConnectAsync(this System.Net.Sockets.Socket socket, System.Net.IPAddress address, int port) { throw null; }
        public static System.Threading.Tasks.Task ConnectAsync(this System.Net.Sockets.Socket socket, System.Net.IPAddress[] addresses, int port) { throw null; }
        public static System.Threading.Tasks.Task ConnectAsync(this System.Net.Sockets.Socket socket, string host, int port) { throw null; }
        public static System.Threading.Tasks.Task<int> ReceiveAsync(this System.Net.Sockets.Socket socket, System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public static System.Threading.Tasks.Task<int> ReceiveAsync(this System.Net.Sockets.Socket socket, System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public static System.Threading.Tasks.ValueTask<int> ReceiveAsync(this System.Net.Sockets.Socket socket, System.Memory<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Sockets.SocketReceiveFromResult> ReceiveFromAsync(this System.Net.Sockets.Socket socket, System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEndPoint) { throw null; }
        public static System.Threading.Tasks.Task<System.Net.Sockets.SocketReceiveMessageFromResult> ReceiveMessageFromAsync(this System.Net.Sockets.Socket socket, System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEndPoint) { throw null; }
        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket, System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public static System.Threading.Tasks.Task<int> SendAsync(this System.Net.Sockets.Socket socket, System.Collections.Generic.IList<System.ArraySegment<byte>> buffers, System.Net.Sockets.SocketFlags socketFlags) { throw null; }
        public static System.Threading.Tasks.ValueTask<int> SendAsync(this System.Net.Sockets.Socket socket, System.ReadOnlyMemory<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public static System.Threading.Tasks.Task<int> SendToAsync(this System.Net.Sockets.Socket socket, System.ArraySegment<byte> buffer, System.Net.Sockets.SocketFlags socketFlags, System.Net.EndPoint remoteEP) { throw null; }
    }
    public enum SocketType
    {
        Dgram = 2,
        Raw = 3,
        Rdm = 4,
        Seqpacket = 5,
        Stream = 1,
        Unknown = -1,
    }
    public partial class TcpClient : System.IDisposable
    {
        public TcpClient() { }
        public TcpClient(System.Net.IPEndPoint localEP) { }
        public TcpClient(System.Net.Sockets.AddressFamily family) { }
        public TcpClient(string hostname, int port) { }
        protected bool Active { get { throw null; } set { } }
        public int Available { get { throw null; } }
        public System.Net.Sockets.Socket Client { get { throw null; } set { } }
        public bool Connected { get { throw null; } }
        public bool ExclusiveAddressUse { get { throw null; } set { } }
        public System.Net.Sockets.LingerOption LingerState { get { throw null; } set { } }
        public bool NoDelay { get { throw null; } set { } }
        public int ReceiveBufferSize { get { throw null; } set { } }
        public int ReceiveTimeout { get { throw null; } set { } }
        public int SendBufferSize { get { throw null; } set { } }
        public int SendTimeout { get { throw null; } set { } }
        public System.IAsyncResult BeginConnect(System.Net.IPAddress address, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(System.Net.IPAddress[] addresses, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginConnect(string host, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public void Close() { }
        public void Connect(System.Net.IPAddress address, int port) { }
        public void Connect(System.Net.IPAddress[] ipAddresses, int port) { }
        public void Connect(System.Net.IPEndPoint remoteEP) { }
        public void Connect(string hostname, int port) { }
        public System.Threading.Tasks.Task ConnectAsync(System.Net.IPAddress address, int port) { throw null; }
        public System.Threading.Tasks.Task ConnectAsync(System.Net.IPAddress[] addresses, int port) { throw null; }
        public System.Threading.Tasks.Task ConnectAsync(string host, int port) { throw null; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public void EndConnect(System.IAsyncResult asyncResult) { }
        ~TcpClient() { }
        public System.Net.Sockets.NetworkStream GetStream() { throw null; }
    }
    public partial class TcpListener
    {
        [System.ObsoleteAttribute("This method has been deprecated. Please use TcpListener(IPAddress localaddr, int port) instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        public TcpListener(int port) { }
        public TcpListener(System.Net.IPAddress localaddr, int port) { }
        public TcpListener(System.Net.IPEndPoint localEP) { }
        protected bool Active { get { throw null; } }
        public bool ExclusiveAddressUse { get { throw null; } set { } }
        public System.Net.EndPoint LocalEndpoint { get { throw null; } }
        public System.Net.Sockets.Socket Server { get { throw null; } }
        public System.Net.Sockets.Socket AcceptSocket() { throw null; }
        public System.Threading.Tasks.Task<System.Net.Sockets.Socket> AcceptSocketAsync() { throw null; }
        public System.Net.Sockets.TcpClient AcceptTcpClient() { throw null; }
        public System.Threading.Tasks.Task<System.Net.Sockets.TcpClient> AcceptTcpClientAsync() { throw null; }
        public void AllowNatTraversal(bool allowed) { }
        public System.IAsyncResult BeginAcceptSocket(System.AsyncCallback callback, object state) { throw null; }
        public System.IAsyncResult BeginAcceptTcpClient(System.AsyncCallback callback, object state) { throw null; }
        public static System.Net.Sockets.TcpListener Create(int port) { throw null; }
        public System.Net.Sockets.Socket EndAcceptSocket(System.IAsyncResult asyncResult) { throw null; }
        public System.Net.Sockets.TcpClient EndAcceptTcpClient(System.IAsyncResult asyncResult) { throw null; }
        public bool Pending() { throw null; }
        public void Start() { }
        public void Start(int backlog) { }
        public void Stop() { }
    }
    [System.FlagsAttribute]
    public enum TransmitFileOptions
    {
        Disconnect = 1,
        ReuseSocket = 2,
        UseDefaultWorkerThread = 0,
        UseKernelApc = 32,
        UseSystemThread = 16,
        WriteBehind = 4,
    }
    public partial class UdpClient : System.IDisposable
    {
        public UdpClient() { }
        public UdpClient(int port) { }
        public UdpClient(int port, System.Net.Sockets.AddressFamily family) { }
        public UdpClient(System.Net.IPEndPoint localEP) { }
        public UdpClient(System.Net.Sockets.AddressFamily family) { }
        public UdpClient(string hostname, int port) { }
        protected bool Active { get { throw null; } set { } }
        public int Available { get { throw null; } }
        public System.Net.Sockets.Socket Client { get { throw null; } set { } }
        public bool DontFragment { get { throw null; } set { } }
        public bool EnableBroadcast { get { throw null; } set { } }
        public bool ExclusiveAddressUse { get { throw null; } set { } }
        public bool MulticastLoopback { get { throw null; } set { } }
        public short Ttl { get { throw null; } set { } }
        public void AllowNatTraversal(bool allowed) { }
        public System.IAsyncResult BeginReceive(System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginSend(byte[] datagram, int bytes, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginSend(byte[] datagram, int bytes, System.Net.IPEndPoint endPoint, System.AsyncCallback requestCallback, object state) { throw null; }
        public System.IAsyncResult BeginSend(byte[] datagram, int bytes, string hostname, int port, System.AsyncCallback requestCallback, object state) { throw null; }
        public void Close() { }
        public void Connect(System.Net.IPAddress addr, int port) { }
        public void Connect(System.Net.IPEndPoint endPoint) { }
        public void Connect(string hostname, int port) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public void DropMulticastGroup(System.Net.IPAddress multicastAddr) { }
        public void DropMulticastGroup(System.Net.IPAddress multicastAddr, int ifindex) { }
        public byte[] EndReceive(System.IAsyncResult asyncResult, ref System.Net.IPEndPoint remoteEP) { throw null; }
        public int EndSend(System.IAsyncResult asyncResult) { throw null; }
        public void JoinMulticastGroup(int ifindex, System.Net.IPAddress multicastAddr) { }
        public void JoinMulticastGroup(System.Net.IPAddress multicastAddr) { }
        public void JoinMulticastGroup(System.Net.IPAddress multicastAddr, int timeToLive) { }
        public void JoinMulticastGroup(System.Net.IPAddress multicastAddr, System.Net.IPAddress localAddress) { }
        public byte[] Receive(ref System.Net.IPEndPoint remoteEP) { throw null; }
        public System.Threading.Tasks.Task<System.Net.Sockets.UdpReceiveResult> ReceiveAsync() { throw null; }
        public int Send(byte[] dgram, int bytes) { throw null; }
        public int Send(byte[] dgram, int bytes, System.Net.IPEndPoint endPoint) { throw null; }
        public int Send(byte[] dgram, int bytes, string hostname, int port) { throw null; }
        public System.Threading.Tasks.Task<int> SendAsync(byte[] datagram, int bytes) { throw null; }
        public System.Threading.Tasks.Task<int> SendAsync(byte[] datagram, int bytes, System.Net.IPEndPoint endPoint) { throw null; }
        public System.Threading.Tasks.Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port) { throw null; }
    }
    public partial struct UdpReceiveResult : System.IEquatable<System.Net.Sockets.UdpReceiveResult>
    {
        private object _dummy;
        public UdpReceiveResult(byte[] buffer, System.Net.IPEndPoint remoteEndPoint) { throw null; }
        public byte[] Buffer { get { throw null; } }
        public System.Net.IPEndPoint RemoteEndPoint { get { throw null; } }
        public bool Equals(System.Net.Sockets.UdpReceiveResult other) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(System.Net.Sockets.UdpReceiveResult left, System.Net.Sockets.UdpReceiveResult right) { throw null; }
        public static bool operator !=(System.Net.Sockets.UdpReceiveResult left, System.Net.Sockets.UdpReceiveResult right) { throw null; }
    }
    public sealed partial class UnixDomainSocketEndPoint : System.Net.EndPoint
    {
        public UnixDomainSocketEndPoint(string path) { }
    }
}
