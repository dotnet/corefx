// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Define the symbol below to enable automatic generation of strongly typed
//  overloads for 'Bid.Trace' and 'Bid.ScopeEnter'.
//
//#define BID_AUTOSIG
//#define BID_USE_SCOPEAUTO
//#define BID_USE_EXTENSIONS
//#define BID_USE_IDENT
//#define BID_USE_INSTANCE_TRACKING
//#define BID_USE_CONTROL
//#define BID_USE_PUTSTRLINE
//#define BID_USE_ALL_APIGROUP

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

#if BID_AUTOSIG
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif


//
//  The C# interface for the BID (Built-In Diagnostics) infrastructure consistis of two files that
//  implement the wrapper class Bid:
//
//      internal sealed partial class Bid
//
//  The main part is implemented in BidPrivateBase.cs and is supposed to be considered
//  as invariant part of the interface.
//
//  The second part is implemented in assembly (module) specific file, created from
//  AssemblyTemplate_BID.cs and usually renamed to <AssemblyName>_BID.cs. It is supposed to contain
//  overloaded methods Trace and ScopeEnter with exact signatures used in the given assembly.
//
//  SignatureGenerator (available in development cycle when BID_AUTOSIG symbol is defined)
//  can be used to help generate assembly specific, strongly typed overloads.
//
//  NOTE:
//      The current technique with two "include" files most likely will be changed,
//      so don't make strong assumptions regarding implementation details.
//
//      However, the intention is to keep used APIs unchanged, so the upcoming update(s) of the
//      BID infrastructure should not enforce any changes in already instrumented product code.
//

[ComVisible(false)]
internal static partial class Bid
{
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                         //
    //                                      INTERFACE                                          //
    //                                                                                         //
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  ApiGroup control flags are accessible from attached diagnostic subsystem via corresponding
    //  delegate, so the output can be enabled/disabled on the fly.
    //
    internal enum ApiGroup : uint
    {
        Off         = 0x00000000,

        Default     = 0x00000001,   // Bid.TraceEx (Always ON)
        Trace       = 0x00000002,   // Bid.Trace, Bid.PutStr
        Scope       = 0x00000004,   // Bid.Scope{Enter|Leave|Auto}
        Perf        = 0x00000008,   // TBD..
        Resource    = 0x00000010,   // TBD..
        Memory      = 0x00000020,   // TBD..
        StatusOk    = 0x00000040,   // S_OK, STATUS_SUCCESS, etc.
        Advanced    = 0x00000080,   // Bid.TraceEx

        Pooling     = 0x00001000,
        Dependency  = 0x00002000,
        StateDump   = 0x00004000,
        Correlation = 0x00040000,

        MaskBid     = 0x00000FFF,
        MaskUser    = 0xFFFFF000,
        MaskAll     = 0xFFFFFFFF
    }

    //
    //  These wrappers simplify coding when/if we want direct access
    //  to the ApiGroup Control Bits.
    //
#if BID_USE_ALL_APIGROUP
    internal static bool DefaultOn {
        get { return (modFlags & ApiGroup.Default) != 0; }
    }
#endif
    internal static bool TraceOn {
        [BidMethod(Enabled = false)] // Ignore this method in FXCopBid rule
        get { return (modFlags & ApiGroup.Trace) != 0; }
    }
    internal static bool ScopeOn {
        get { return (modFlags & ApiGroup.Scope) != 0; }
    }
#if BID_USE_ALL_APIGROUP
    internal static bool PerfOn {
        get { return (modFlags & ApiGroup.Perf) != 0; }
    }
    internal static bool ResourceOn {
        get { return (modFlags & ApiGroup.Resource) != 0; }
    }
    internal static bool MemoryOn {
        get { return (modFlags & ApiGroup.Memory) != 0; }
    }
    internal static bool StatusOkOn {
        get { return (modFlags & ApiGroup.StatusOk) != 0; }
    }
#endif
    internal static bool AdvancedOn {
        get { return (modFlags & ApiGroup.Advanced) != 0; }
    }

    internal static bool IsOn(ApiGroup flag) {
        return (modFlags & flag) != 0;
    }

#if BID_USE_ALL_APIGROUP
    internal static bool AreOn(ApiGroup flags) {
        return (modFlags & flags) == flags;
    }
#endif


    private static IntPtr __noData;

    internal static IntPtr NoData {
        get { return __noData; }
    }

    internal static IntPtr ID {
        get { return modID; }
    }

    internal static bool IsInitialized {
        get { return modID != NoData; }
    }

    //=//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  ModeFlags to be used with 'Ex' flavor of API functions (argument "uint flags")
    //
    internal struct ModeFlags
    {
        internal const uint
            Default     = 0x00,
            SmartNewLine= 0x01,
            NewLine     = 0x02,

            Enabled     = 0x04,
          /*DemandSrc   = 0x08,*/

            Blob        = 0x10,
            BlobCopy    = 0x12,
            BlobBinMode = 0x14;
    }

    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  PLAIN STRING OUTPUT
    //
    internal static void PutStr (string str) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.Default, str);
    }

#if BID_USE_PUTSTRLINE
    internal static void PutStrLine (string str) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, str);
    }

    internal static void PutNewLine() {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.NewLine, string.Empty);
    }

    internal static void PutStrEx (uint flags, string str) {
        if (modID != NoData)
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)flags, str);
    }

    internal static void PutSmartNewLine() {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, string.Empty);
    }

    //
    //  for( i = 0; i < strArray.Length; i++ ){
    //      Bid.PutStrEx( Bid.NewLineEx((i % 10) == 0), strArray[idx] );
    //  }
    //  Bid.PutSmartNewLine();
    //
    internal static uint NewLineEx(bool addNewLine) {
        return addNewLine ? ModeFlags.SmartNewLine : ModeFlags.Default;
    }
#endif

    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Main Tracing Facility (More overloads to be provided in assembly-specific file)
    //
   #if BID_AUTOSIG
    internal static void Trace(string fmtPrintfW, params object[] args) {
        SignatureGenerator.Trace (fmtPrintfW, args);
    }
    internal static void TraceEx(uint flags, string fmtPrintfW, params object[] args) {
        SignatureGenerator.TraceEx (flags, fmtPrintfW, args);
    }
   #endif

    [BidMethod]
    internal static void Trace(string strConst) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, strConst);
    }

    [BidMethod]
    internal static void TraceEx(uint flags, string strConst) {
        if (modID != NoData)
            NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr)flags, strConst);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, string a1) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
    }

    [BidMethod]
    internal static void TraceEx(uint flags, string fmtPrintfW, string a1) {
        if (modID != NoData)
            NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW, a1);
    }

    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Scope Tracking
    //
    internal static void ScopeLeave(ref IntPtr hScp) {
        if ((modFlags & ApiGroup.Scope) != 0  && modID != NoData) {
            if (hScp != NoData) NativeMethods.ScopeLeave(modID, UIntPtr.Zero, UIntPtr.Zero, ref hScp);
        } else {
            hScp = NoData;  // NOTE: This assignment is necessary, even it may look useless
        }
    }

    //
    //  (More overloads to be provided in assembly-specific file)
    //
   #if BID_AUTOSIG
    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, params object[] args) {
        SignatureGenerator.ScopeEnter (out hScp, fmtPrintfW, args);
    }
#endif
    
    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string strConst) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, strConst);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        } else {
            hScp = NoData;
        }
    }

   #if BID_USE_SCOPEAUTO
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Automatic Scope Tracking
    //  NOTEs:
    //  - This is 'struct', so there are NO HEAP operations and associated perf. penalty;
    //  - Though use it for significant methods, so relative overhead will be inconsiderable;
    //  - Use 'short' syntax of 'using' expression (no local variable needed):
    //      void Foo() {
    //          using(new Bid.ScopeAuto("<MyClass.Foo>")) {
    //              // method's body...
    //          }
    //      }
    //
    internal struct ScopeAuto : IDisposable
    {
        internal ScopeAuto (string strScopeName) {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out _hscp, strScopeName);
            } else {
                _hscp = NoData;
            }
        }
        [BidMethod]
        internal ScopeAuto (string fmtPrintfW, string arg) {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out _hscp, fmtPrintfW, arg);
            } else {
                _hscp = NoData;
            }
        }
        [BidMethod]
        internal ScopeAuto (string fmtPrintfW, IntPtr arg) {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out _hscp, fmtPrintfW, arg);
            } else {
                _hscp = NoData;
            }
        }
        [BidMethod]
        internal ScopeAuto (string fmtPrintfW, int arg) {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out _hscp, fmtPrintfW, arg);
            } else {
                _hscp = NoData;
            }
        }
        [BidMethod]
        internal ScopeAuto (string fmtPrintfW, int a1, int a2) {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out _hscp, fmtPrintfW, a1, a2);
            } else {
                _hscp = NoData;
            }
        }

        public void Dispose() {
            if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData  &&  _hscp != NoData) {
                NativeMethods.ScopeLeave(modID, UIntPtr.Zero, UIntPtr.Zero, ref _hscp);
            }
            //  NOTE: In contrast with standalone ScopeLeave,
            //  there is no need to assign "NoData" to _hscp.
        }

        private IntPtr _hscp;

    } // ScopeAuto

#endif

#if BID_USE_CONTROL
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Output Control
    //
    internal static bool Enabled(string traceControlString) {
        return ((modFlags & ApiGroup.Trace) == 0  ||  modID == NoData)
                ? false
                : NativeMethods.Enabled(modID, UIntPtr.Zero, UIntPtr.Zero, traceControlString);
    }
#endif

#if BID_USE_IDENT
    //
    //  Indentation
    //
    internal struct Indent : IDisposable
    {
        internal Indent(int oneLevel){
            DASSERT(oneLevel == 1); // We need fake argument (struct can't have ctor with no args)
            In();
        }
        public void Dispose(){
            Out();
        }

        internal static void In(){
            if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
                NativeMethods.Indent(modID, indentIn);
        }
        internal static void Out(){
            if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
                NativeMethods.Indent(modID, indentOut);
        }

    }

    private const int   indentIn  = -1,
                        indentOut = -3;
#endif

    //=//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Binary output
    //

    // FXCopBid does not support validation of buffer versus length at this stage, disable testing
    // of this method by this rule
    [BidMethod(Enabled = false)] 
    internal static void TraceBin(string constStrHeader, byte[] buff, UInt16 length) {
        if (modID != NoData) {
            if (constStrHeader != null && constStrHeader.Length > 0) {
                NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, constStrHeader);
            }
            if( (UInt16)buff.Length < length ){
                length = (UInt16)buff.Length;
            }
            NativeMethods.TraceBin( modID, UIntPtr.Zero, (UIntPtr)Bid.ModeFlags.Blob,
                                    "<Trace|BLOB> %p %u\n", buff, length );
        }
    }

    // FXCopBid does not support validation of buffer versus length at this stage, disable testing
    // of this method by this rule
    [BidMethod(Enabled = false)] // do not validate calls to this method in FXCopBid
    internal static void TraceBinEx(byte[] buff, UInt16 length) {
        if (modID != NoData) {
            if( (UInt16)buff.Length < length ){
                length = (UInt16)buff.Length;
            }
            NativeMethods.TraceBin( modID, UIntPtr.Zero, (UIntPtr)Bid.ModeFlags.Blob,
                                    "<Trace|BLOB> %p %u\n", buff, length );
        }
    }

   #if BID_USE_EXTENSIONS
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  STRUCTURED EXTENSION
    //
    internal delegate void ExtDelegate(IntPtr modID, IntPtr objRef, int attr, IntPtr data);

    internal static ExtDelegate AddExtension(string extName, ExtDelegate extProc) {
        AddExtension(extName, extProc, IntPtr.Zero);
        return extProc;
    }

    internal static ExtDelegate AddExtension(string extName, ExtDelegate extProc, IntPtr extData) {
        if( modID != NoData ){
            NativeMethods.AddExtension( modID, DefaultCmdSpace, CtlCmd.AddExtension,
                                        extData, extName, extProc );
        }
        return extProc;
    }

    internal struct Details
    {
        internal const int
            Min         = 1,
            Std         = 2,
            Max         = 7,
            LevelMask   = 0x07,
            ModeDisco   = 0x08,
            ModeBinary  = 0x10;
    }

    internal static int LevelOfDetailsEx(int attr) {
        return (attr & Details.LevelMask);
    }
    internal static bool InBinaryModeEx(int attr) {
        return ((attr & Details.ModeBinary) != 0);
    }
    internal static bool InDiscoveryModeEx(int attr) {
        return ((attr & Details.ModeDisco) != 0);
    }

    //
    //  WriteEx to be used in BidExtensions
    //  (More overloads to be provided in assembly-specific file)
    //
   #if BID_AUTOSIG
    internal static void WriteEx(IntPtr hCtx, uint flags, string fmtPrintfW, params object[] args) {
        SignatureGenerator.WriteEx (hCtx, flags, fmtPrintfW, args);
    }
   #endif

    internal static void WriteEx(IntPtr hCtx, uint flags, string strConst) {
        NativeMethods.Trace(hCtx, UIntPtr.Zero, (UIntPtr)flags, strConst);
    }

    internal static void WriteEx(IntPtr hCtx, uint flags, string fmtPrintfW, string a1) {
        NativeMethods.Trace(hCtx, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW, a1);
    }

    internal static void WriteBinEx(IntPtr hCtx, byte[] buff, UInt16 length) {
        if (hCtx != NoData) {
            if( (UInt16)buff.Length < length ) {
                length = (UInt16)buff.Length;
            }
            NativeMethods.TraceBin( hCtx, UIntPtr.Zero, (UIntPtr)Bid.ModeFlags.Blob,
                                    "<Trace|BLOB> %p %u\n", buff, length );
        }
    }


    //
    //  Indentation to be used in BidExtensions
    //
    internal struct WriteIndentEx
    {
        private WriteIndentEx(int noData){ } // no instances, only static methods

        internal static void In(IntPtr hCtx){
            NativeMethods.Indent(hCtx, indentIn);
        }
        internal static void Out(IntPtr hCtx){
            NativeMethods.Indent(hCtx, indentOut);
        }
    }


    //
    //  Small helpers wrap all the work with GCHandle that we have to do in order to avoid
    //  object marshalling in P/Invoke.
    //
    //  NOTE:   Make sure that MakeRef/DelRef are perfectly balanced in order to not leak
    //          GCHandles. DelRef must be called in 'Dispose|Finalize' or 'finally' block.
    //
    internal static IntPtr MakeRef(object obj) {
        return (IntPtr)GCHandle.Alloc(obj, GCHandleType.Normal);
    }
    internal static object GetObj(IntPtr objRef) {
        return ((GCHandle)objRef).Target;
    }
    internal static void DelRef(IntPtr objRef) {
        ((GCHandle)objRef).Free();
    }


   #endif // BID_USE_EXTENSIONS
    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SERVICES
    //

    //
    //  MODULE-WIDE APIGROUP BITS
    //

#if BID_USE_ALL_APIGROUP
    internal static ApiGroup GetApiGroupBits (ApiGroup mask) {
        return modFlags & mask;
    }
#endif

    internal static ApiGroup SetApiGroupBits (ApiGroup mask, ApiGroup bits) {
        lock (_setBitsLock) {
            ApiGroup tmp = modFlags;
            if( mask != ApiGroup.Off ){
                modFlags ^= (bits ^ tmp) & mask;
            }
            return tmp;
        }
    }
    private static object _setBitsLock = new object();

#if BID_USE_INSTANCE_TRACKING
    //
    //  FAST COMMUNICATION WITH THE SUBSYSTEM
    //
    private
    struct TouchCode
    {
        internal const uint
            Reverse         = 1,
            Unicode         = 2,

            Extension       = 0 * 4,

            ObtainItemID    = 1 * 4 + Unicode,
            RecycleItemID   = 1 * 4 + Reverse + Unicode,
            UpdateItemID    = 2 * 4 + Unicode
        ;
    }

    //
    //  INSTANCE TRACKING IDs
    //
    internal static void ObtainItemID(out int itemID, string textID, IntPtr invariant){
        itemID = (modID != NoData)
            ? NativeMethods.Touch01(modID, textID, TouchCode.ObtainItemID, invariant, IntPtr.Zero)
            : 0;
    }
    internal static void ObtainItemID(out int itemID, string textID, uint invariant){
        itemID = (modID != NoData)
            ? NativeMethods.Touch01(modID, textID, TouchCode.ObtainItemID, (IntPtr)invariant, IntPtr.Zero)
            : 0;
    }

    internal static void ObtainItemID(out int itemID, string textID, int invariant){
        itemID = (modID != NoData)
            ? NativeMethods.Touch01(modID, textID, TouchCode.ObtainItemID, (IntPtr)invariant, IntPtr.Zero)
            : 0;
    }
    internal static void RecycleItemID(ref int itemID, string textID){
        if (modID != NoData  &&  itemID != 0) {
            NativeMethods.Touch01(modID, textID, TouchCode.RecycleItemID, (IntPtr)itemID, IntPtr.Zero);
            itemID = 0;
        }
    }

    internal static void UpdateItemID(ref int itemID, string textID, string associate){
        if (modID != NoData)
            NativeMethods.Touch02(modID, textID, TouchCode.UpdateItemID, ref itemID, associate);
    }

    internal static void UpdateItemID(ref int itemID, string textID, IntPtr associate){
        if (modID != NoData)
            NativeMethods.Touch03(modID, textID, TouchCode.UpdateItemID, ref itemID, associate);
    }

    internal static void UpdateItemID(ref int itemID, string textID, int associate){
        if (modID != NoData)
            NativeMethods.Touch03(modID, textID, TouchCode.UpdateItemID, ref itemID, (IntPtr)associate);
    }
    internal static void UpdateItemID(ref int itemID, string textID, uint associate){
        if (modID != NoData)
            NativeMethods.Touch03(modID, textID, TouchCode.UpdateItemID, ref itemID, (IntPtr)associate);
    }
#endif

    //
    //  BID-specific Text Metadata
    //
    internal static bool AddMetaText(string metaStr) {
        if( modID != NoData ){
            NativeMethods.AddMetaText(modID, DefaultCmdSpace, CtlCmd.AddMetaText, IntPtr.Zero, metaStr, IntPtr.Zero);
        }
        return true;
    }

#if BID_USE_CONTROL
    //
    //  Explicit shutdown of the diagnostic backend.
    //  Note that it's up to BID implementation how to handle this command; it can be just ignored.
    //
    internal static void Shutdown(int arg) {
        if( modID != NoData ){
            NativeMethods.DllBidCtlProc( modID, DefaultCmdSpace, CtlCmd.Shutdown,
                                        (IntPtr)arg, IntPtr.Zero, IntPtr.Zero );
        }
    }
#endif

    //+//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  DEBUG-ONLY SERVICES
    //
    [System.Diagnostics.Conditional("DEBUG")]
    internal static void DTRACE(string strConst) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData) {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, strConst);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    internal static void DTRACE(string clrFormatString, params object[] args) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData) {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine,
                                String.Format(CultureInfo.CurrentCulture, clrFormatString, args));
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    internal static void DASSERT(bool condition) {
        if (!condition) {
           #if false
            if (0 == nativeAssert(sourceFileLineNumber)) {
                if (!Debugger.IsAttached) {
                    Debugger.Launch();
                }
                Debugger.Break();
            }
           #else
            System.Diagnostics.Trace.Assert(false);
           #endif
        }
    }

    //+//////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                         //
    //                               IMPLEMENTATION DETAILS                                    //
    //                                                                                         //
    //+//////////////////////////////////////////////////////////////////////////////////////////

    //
    //  modID and modFlags must be unique for each loadable entity (.exe, .dll, .netmodule)
    //  modID should be unique within the process (generated by DllBidEntryPoint), however modID may be recycled and reused
    //
    private
    static  IntPtr modID = internalInitialize();

    private
    static  ApiGroup modFlags;

    private static   string     modIdentity;

    private delegate ApiGroup   CtrlCB( ApiGroup mask, ApiGroup bits );
    private static   CtrlCB     ctrlCallback;

    //
    //  Binding Cookie
    //
    [StructLayout(LayoutKind.Sequential)]
    private class BindingCookie
    {
        internal IntPtr _data;
        internal BindingCookie()    { _data = (IntPtr)(-1); }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void Invalidate()  { _data = (IntPtr)(-1); }
    };

    private static BindingCookie cookieObject;
    private static GCHandle      hCookie;

    private static void deterministicStaticInit()
    {
        __noData          = (IntPtr)(-1);
        __defaultCmdSpace = (IntPtr)(-1);

        modFlags     = ApiGroup.Off;
        modIdentity  = string.Empty;
        ctrlCallback = new CtrlCB(SetApiGroupBits);

        cookieObject = new BindingCookie();
        hCookie      = GCHandle.Alloc(cookieObject, GCHandleType.Pinned);
    }

    //
    //  CONTROL CENTRE
    //

    private static IntPtr __defaultCmdSpace;

    internal static IntPtr DefaultCmdSpace {
        get { return __defaultCmdSpace; }
    }

    private
    enum CtlCmd : uint
    {
        //
        //  Standard modifiers for command codes.
        //
        Reverse = 1,
        Unicode = 2,

        //
        //  Predefined commands are in range [CtlCmd.DcsBase .. CtlCmd.DcsMax]
        //  'Dcs' stands for 'Default Command Space'
        //
        DcsBase = 268435456 * 4,    // 0x10000000 * 4
        DcsMax  = 402653183 * 4,    // 0x17FFFFFF * 4

        //
        //  Control Panel commands are in range [CtlCmd.CplBase .. CtlCmd.CplMax]
        //
        CplBase = 402653184 * 4,    // 0x18000000 * 4
        CplMax =  536870911 * 4,    // 0x1FFFFFFF * 4

        //
        //  Predefined commands (have wrapper functions)
        //
        CmdSpaceCount   =  0 * 4 + DcsBase,
        CmdSpaceEnum    =  1 * 4 + DcsBase,
        CmdSpaceQuery   =  2 * 4 + DcsBase,

        GetEventID      =  5 * 4 + DcsBase + Unicode,
        ParseString     =  6 * 4 + DcsBase + Unicode,
        AddExtension    =  7 * 4 + DcsBase + Unicode,
        AddMetaText     =  8 * 4 + DcsBase + Unicode,
        AddResHandle    =  9 * 4 + DcsBase + Unicode,
        Shutdown        = 10 * 4 + DcsBase + Unicode,

        LastItem

    } // CtlCmd

#if BID_USE_CONTROL
    internal static IntPtr GetCmdSpaceID (string textID) {
        return  (modID != NoData)
                ? NativeMethods.GetCmdSpaceID(modID, DefaultCmdSpace, CtlCmd.CmdSpaceQuery, 0, textID, IntPtr.Zero)
                : IntPtr.Zero;
    }
#endif

    //-//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  EntryPoint
    //
    private const int BidVer = 9210;

    [StructLayout(LayoutKind.Sequential)]
    private struct BIDEXTINFO
    {
        IntPtr  hModule;
        [MarshalAs(UnmanagedType.LPWStr)]
        string  DomainName;
        int     Reserved2;
        int     Reserved;
        [MarshalAs(UnmanagedType.LPWStr)]
        string  ModulePath;
        IntPtr  ModulePathA;
        IntPtr  pBindCookie;

        internal BIDEXTINFO(IntPtr hMod, string modPath, string friendlyName, IntPtr cookiePtr)
        {
            hModule     = hMod;
            DomainName  = friendlyName;
            Reserved2   = 0;
            Reserved    = 0;
            ModulePath  = modPath;
            ModulePathA = IntPtr.Zero;
            pBindCookie = cookiePtr;
        }
    }; // BIDEXTINFO

    private static string getIdentity(Module mod)
    {
        string idStr;
        object[] attrColl = mod.GetCustomAttributes(typeof(BidIdentityAttribute), true);
        if( attrColl.Length == 0 ){
            idStr = mod.Name;
        } else {
            idStr = ((BidIdentityAttribute)attrColl[0]).IdentityString;
        }
        //Debug.Assert( attrColl.Length == 1 );
        return idStr;
    }

    private static string getAppDomainFriendlyName()
    {
        string name = AppDomain.CurrentDomain.FriendlyName;
        if( name == null || name.Length <= 0 ) {
            name = "AppDomain.H" + AppDomain.CurrentDomain.GetHashCode();
        }

        return VersioningHelper.MakeVersionSafeName(name, ResourceScope.Machine, ResourceScope.AppDomain);
    }

    private const uint configFlags = 0xD0000000; // ACTIVE_BID|CTLCALLBACK|MASK_PAGE

    [ResourceExposure(ResourceScope.Machine)]
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)] // Module.FullyQualifiedName
    private static string getModulePath(Module mod) {
        return mod.FullyQualifiedName;
    }

    [ResourceExposure(ResourceScope.None)] // info contained within call to DllBidEntryPoint
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)] // getModulePath
    private static void initEntryPoint()
    {
        NativeMethods.DllBidInitialize();

        //
        //  Multi-file assemblies are not supported by current model of the BID managed wrapper.
        //  The below Marshal.GetHINSTANCE(mod) will return HINSTANCE for the manifest module
        //  instead of actual module, which is Ok because it is the only module
        //  in the single-file assembly.
        //
        Module mod  = Assembly.GetExecutingAssembly().ManifestModule;
        modIdentity = getIdentity(mod);
        modID = NoData;

        string friendlyName = getAppDomainFriendlyName();
        BIDEXTINFO extInfo = new BIDEXTINFO(Marshal.GetHINSTANCE(mod),
                                            getModulePath(mod),
                                            friendlyName,
                                            hCookie.AddrOfPinnedObject());

        NativeMethods.DllBidEntryPoint( ref modID, BidVer, modIdentity,
                                        configFlags, ref modFlags, ctrlCallback,
                                        ref extInfo, IntPtr.Zero, IntPtr.Zero );

        if( modID != NoData )
        {
            object[] attrColl = mod.GetCustomAttributes(typeof(BidMetaTextAttribute), true);
            foreach (object obj in attrColl) {
                AddMetaText( ((BidMetaTextAttribute)obj).MetaText );
            }
            
            Bid.Trace("<ds.Bid|Info> VersionSafeName='%ls'\n", friendlyName);
        }
    } // initEntryPoint

    [ResourceExposure(ResourceScope.None)]
    [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private static void doneEntryPoint()
    {
        if (modID == NoData) {
            modFlags = ApiGroup.Off;
            return;
        }

        try {
            NativeMethods.DllBidEntryPoint( ref modID, 0, IntPtr.Zero,
                                            configFlags, ref modFlags,
                                            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero );
            NativeMethods.DllBidFinalize();
        }
        catch {
            //
            //  We do intentionally catch everything because no matter what happens
            //  we don't want any exception to escape when we're in context of a finalizer.
            //  Note that critical exceptions such as ThreadAbortException could be
            //  propagated anyway (CLR 2.0 and above).
            //
            modFlags = ApiGroup.Off;    // This is 'NoOp', just to not have empty catch block.
        }
        finally {
            cookieObject.Invalidate();
            modID = NoData;
            modFlags = ApiGroup.Off;
        }

    } // doneEntryPoint

    //
    //  Automatic Initialization/Finalization.
    //

    private sealed class AutoInit : SafeHandle
    {
        internal AutoInit() : base(IntPtr.Zero, true) {
            initEntryPoint();
            _bInitialized = true;
        }
        override protected bool ReleaseHandle() {
            _bInitialized = false;
            doneEntryPoint();
            return true;
        }
        public override bool IsInvalid {
            get { return !_bInitialized; }
        }
        private bool _bInitialized;
    }

    private static AutoInit ai;

    private static IntPtr internalInitialize()
    {
        deterministicStaticInit();
        ai = new AutoInit();
        return modID;
    }

    //=//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Interop calls to pluggable hooks
    //

    [SuppressUnmanagedCodeSecurity, ComVisible(false)]
    private static partial class NativeMethods
    {
        //
        //  Plain text
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall,
        EntryPoint="DllBidPutStrW")] extern
        internal static void PutStr(IntPtr hID, UIntPtr src, UIntPtr info, string str);

        //
        //  Trace
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidTraceCW")] extern
        internal static void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string strConst);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidTraceCW")] extern
        internal static void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1);

        //
        //  Scope
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, EntryPoint="DllBidScopeLeave")] extern
        internal static void ScopeLeave(IntPtr hID, UIntPtr src, UIntPtr info, ref IntPtr hScp);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string strConst);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter( IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp,
                                         string fmtPrintfW, int a1);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter( IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp,
                                         string fmtPrintfW, int a1, int a2);


        //
        //  Output control
        //
#if BID_USE_CONTROL
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidEnabledW")] extern
        internal static bool Enabled(IntPtr hID, UIntPtr src, UIntPtr info, string tcs);
#endif

#if BID_USE_IDENT
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidIndent")] extern
        internal static int Indent(IntPtr hID, int nIdx);
#endif

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl,
        EntryPoint="DllBidTraceCW")] extern
        internal static void TraceBin(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, byte[] buff, UInt32 len);

#if BID_USE_INSTANCE_TRACKING
        //
        //  Fast Communication API
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidTouch")] extern
        internal static int Touch01(IntPtr hID, string textID, uint code, IntPtr arg1, IntPtr arg2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidTouch")] extern
        internal static void Touch02(IntPtr hID, string textID, uint code, ref int itemID, string associate);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidTouch")] extern
        internal static void Touch03(IntPtr hID, string textID, uint code, ref int itemID, IntPtr associate);
#endif

        //
        //  Services
        //
#if BID_USE_CONTROL
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidCtlProc")] extern
        internal static void DllBidCtlProc( IntPtr hID, IntPtr cmdSpace, CtlCmd cmd,
                                            IntPtr arg1, IntPtr arg2, IntPtr arg3 );
#endif

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidCtlProc")] extern
        internal static void AddMetaText( IntPtr hID, IntPtr cmdSpace, CtlCmd cmd, IntPtr nop1,
                                          string txtID, IntPtr nop2);

       #if BID_USE_EXTENSIONS
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, EntryPoint="DllBidCtlProc")] extern
        internal static void AddExtension( IntPtr hID, IntPtr cmdSpaceID, CtlCmd cmd,
                                           IntPtr data, string txtID, ExtDelegate proc);
       #endif

#if BID_USE_CONTROL
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Ansi, BestFitMapping=false, EntryPoint="DllBidCtlProc")] extern
        internal static IntPtr GetCmdSpaceID( IntPtr hID, IntPtr cmdSpace, CtlCmd cmd, uint noOp,
                                              string txtID, IntPtr NoOp2 );
#endif

        //
        //  Initialization / finalization
        //
        [ResourceExposure(ResourceScope.Machine)]
        [DllImport(dllName, CharSet=CharSet.Ansi, BestFitMapping=false)] extern
        internal static void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, string sIdentity,
                                            uint propBits, ref ApiGroup pGblFlags, CtrlCB fAddr,
                                            ref BIDEXTINFO pExtInfo, IntPtr pHooks, IntPtr pHdr);

        [ResourceExposure(ResourceScope.Machine)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName)] extern
        internal static void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, IntPtr unused1,
                                            uint propBits, ref ApiGroup pGblFlags, IntPtr unused2,
                                            IntPtr unused3, IntPtr unused4, IntPtr unused5);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName)] extern
        internal static void DllBidInitialize();

        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName)] extern
        internal static void DllBidFinalize();

    } // NativeMethods

} // Bid{PrivateBase}

//+//////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                             //
//                                         Attributes                                          //
//                                                                                             //
//-//////////////////////////////////////////////////////////////////////////////////////////////
//
//  [module: BidIdentity("ModuleIdentityString")]
//
[AttributeUsage(AttributeTargets.Module, AllowMultiple=false)]
internal sealed class BidIdentityAttribute : Attribute
{
    internal BidIdentityAttribute( string idStr ){
        _identity = idStr;
    }
    internal string IdentityString {
        get { return _identity; }
    }
    string  _identity;
}

//
//  [module: BidMetaText("<Alias> ...")]
//  [module: BidMetaText("<ApiGroup> ...")]
//  ...etc...
//
[AttributeUsage(AttributeTargets.Module, AllowMultiple=true)]
internal sealed class BidMetaTextAttribute : Attribute
{
    internal BidMetaTextAttribute( string str ){
        _metaText = str;
    }
    internal string MetaText {
        get { return _metaText; }
    }
    string  _metaText;
}


/// <summary>
/// This attribute is used by FxCopBid rule to mark methods that accept format string and list of arguments that match it
/// FxCopBid rule uses this attribute to check if the method needs to be included in checks and to read type mappings
/// between the argument type to printf Type spec.
/// 
/// If you need to rename/remove the attribute or change its properties, make sure to update the FxCopBid rule!
/// </summary>
[System.Diagnostics.ConditionalAttribute("CODE_ANALYSIS")]
[System.AttributeUsage(AttributeTargets.Method)]
internal sealed class BidMethodAttribute : Attribute
{
    private bool m_enabled;

    /// <summary>
    /// enabled by default
    /// </summary>
    internal BidMethodAttribute()
    {
        m_enabled = true;
    }

    /// <summary>
    /// if Enabled is true, FxCopBid rule will validate all calls to this method and require that it will have string argument;
    /// otherwise, this method is ignored.
    /// </summary>
    public bool Enabled {
        get
        {
            return m_enabled;
        }
        set
        {
            m_enabled = value;
        }
    }
}

/// <summary>
/// This attribute is used by FxCopBid rule to tell FXCOP the 'real' type sent to the native trace call for this argument. For
/// example, if Bid.Trace accepts enumeration value, but marshals it as string to the native trace method, set this attribute
/// on the argument and set ArgumentType = typeof(string)
/// 
/// It can be applied on a parameter, to let FxCopBid rule know the format spec type used for the argument, or it can be applied on a method,
/// to insert additional format spec arguments at specific location.
/// 
/// If you need to rename/remove the attribute or change its properties, make sure to update the FxCopBid rule!
/// </summary>
[System.Diagnostics.ConditionalAttribute("CODE_ANALYSIS")]
[System.AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple=true)]
internal sealed class BidArgumentTypeAttribute : Attribute
{
    // this overload can be used on the argument itself
    internal BidArgumentTypeAttribute(Type bidArgumentType)
    {
        this.ArgumentType = bidArgumentType;
        this.Index = -1; // if this c-tor is used on methods, default index value is 'last'
    }

    // this overload can be used on the method to add hidden spec arguments
    // set index to -1 to add an argument to the end
    internal BidArgumentTypeAttribute(Type bidArgumentType, int index)
    {
        this.ArgumentType = bidArgumentType;
        this.Index = index;
    }

    public readonly Type ArgumentType;
    // should be used only if attribute is applied on the method
    public readonly int Index;
}

#if BID_AUTOSIG
//+//////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                             //
//                                     SignatureGenerator                                      //
//                                                                                             //
//-//////////////////////////////////////////////////////////////////////////////////////////////
//
//  NOTE:
//      SignatureGenerator is NOT part of product code. It can be only used in development
//      cycle in order to help generate strongly typed overloads for Bid.Trace and Bid.ScopeEnter
//
internal sealed class SignatureGenerator
{
    private sealed class FmtString
    {
        const string surrogateOutputMarker = "*****";

        string _str;
        bool   _newLine;

        internal FmtString(string fmt)
        {
            int len = fmt.Length;
            if( len <= 0 ){
                _newLine = false;
                _str = string.Empty;
            } else {
                _newLine = (fmt[len-1] == '\n');
                _str = surrogateOutputMarker + (_newLine ? fmt.Substring(0, len-1) : fmt);
            }
        }

        internal bool WantsNewLine {
            get { return _newLine; }
        }
        internal string Body {
            get { return _str; }
        }

    } // FmtString


    [Serializable]
    private sealed class UniqueSignatures
    {
        const string invalidTypeSuffix = ".Edit_TypeName_Here";

        StringCollection    _sigPool;
        ArrayList           _numArgsPool;
        StringBuilder       _buf;
        int                 _problemCount;

        internal UniqueSignatures()
        {
            _sigPool      = new StringCollection();
            _numArgsPool  = new ArrayList();
            _buf          = new StringBuilder(256);
            _problemCount = 0;
        }

        internal int Consider(params object[] args)
        {
            string signature = argList2Sig(args);
            int    idx1 = _sigPool.IndexOf(signature);
            if( idx1 < 0 ){
                idx1 = _sigPool.Add(signature);
                _numArgsPool.Add(args.Length);
            }
            Bid.DASSERT( NumOfArgs(idx1) == args.Length );
            return idx1;
        }

        internal int Count {
            get {
                int cnt = _sigPool.Count;
                Bid.DASSERT( cnt == _numArgsPool.Count );
                return cnt;
            }
        }

        internal string Signature(int idx) {
            return _sigPool [idx];
        }

        internal int NumOfArgs(int idx) {
            return (int)_numArgsPool [idx];
        }


        string argList2Sig(params object[] args)
        {
            Type    argType;
            int     idx = 0;

            _buf.Length = 0;    // cleanup StringBuilder

            foreach (object arg in args)
            {
                _buf.Append(", ");
                if( arg != null )
                {
                    argType = arg.GetType();
                    _buf.Append(argType.FullName);
                    if( !argType.IsPrimitive && argType != typeof(string) )
                    {
                        _buf.Append(invalidTypeSuffix);
                        _problemCount++;
                    }
                }
                else
                {
                    _buf.Append("System" + invalidTypeSuffix);
                    _problemCount++;
                }
                _buf.Append(" a");
                _buf.Append(++idx);
            }
            return _buf.ToString();
        }

        internal void writeSignatures(TextWriter stm, string[] pattern)
        {
            for (int i = 0; i < this.Count; i++) {
                makeSignature(pattern, this.Signature(i), this.NumOfArgs(i), ref _buf);
                stm.WriteLine(_buf);
            }
            _buf.Length = 0;
        }

        private static void makeSignature(string[] pattern, string argList, int numOfArgs,
                                          ref StringBuilder buf)
        {
            buf.Length = 0;
            foreach (string patternChunk in pattern)
            {
                switch (patternChunk)
                {
                 case "ARGS":
                    buf.Append(argList);
                    break;
                 case "ARGUSE":
                    buf.Append(argListUse(numOfArgs));
                    break;
                 case "ARGNUM":
                    buf.Append(numOfArgs.ToString());
                    break;
                 default:
                    buf.Append(patternChunk);
                    break;
                }
            }
        }

        internal void PrepareForIntermediateStore()
        {
            _buf.Length = 0;
        }

    } // UniqueSignatures

    //=//////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SignatureGenerator methods ...
    //

    private static string argListUse(int numOfArgs)
    {
        StringBuilder buf = new StringBuilder();
        for (int idx = 1; idx <= numOfArgs; idx++) {
            buf.Append(",a");
            buf.Append(idx);
        }
        return buf.ToString();
    }

    private static string dumpArgList(params object[] args)
    {
        StringBuilder buf = new StringBuilder();
        foreach( object obj in args ){
            buf.Append(" ");
            if( obj != null ){
                buf.Append(obj.ToString());
            } else {
                buf.Append("(null)");
            }
        }
        return " [" + buf.ToString().Substring(1) + "]";
    }


    static void writeHeader(TextWriter stm, string modName)
    {
        int idx = modName.LastIndexOfAny(new char[]{'\\', '/'});
        if (idx > 0) {
            modName = modName.Substring(idx+1);
        }
        string[] pattern = Templates.Headers.File.Split('^');

        foreach (string patternChunk in pattern) {
            switch (patternChunk) {
             case "MODNAME":
                stm.Write(modName);
                break;
             default:
                stm.Write(patternChunk);
                break;
            }
        }
    }

    //=//////////////////////////////////////////////////////////////////////////////////////////

    private static void fakeOutput(uint flags, string fmt, params object[] args)
    {
        FmtString fmtStr = new FmtString(fmt);
        string buf = fmtStr.Body + dumpArgList(args);
        if( fmtStr.WantsNewLine || (flags & (Bid.ModeFlags.NewLine|Bid.ModeFlags.SmartNewLine)) != 0 ){
            buf += Environment.NewLine;
        }
        Bid.PutStr(buf);
    }

    internal static void Trace(string fmtPrintfW, params object[] args)
    {
        fakeOutput(0, fmtPrintfW, args);
        Buckets.Trace.Consider(args);
        Buckets.Native.Trace.Consider(args);
    }

    internal static void TraceEx(uint flags, string fmtPrintfW, params object[] args)
    {
        fakeOutput(flags, fmtPrintfW, args);
        Buckets.TraceEx.Consider(args);
        Buckets.Native.Trace.Consider(args);
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmt, params object[] args)
    {
        FmtString fmtStr = new FmtString(fmt);
        string buf = fmtStr.Body + dumpArgList(args);
        buf += Environment.NewLine;
        Bid.ScopeEnter(out hScp, "%s", buf);
        Buckets.ScopeEnter.Consider(args);
        Buckets.Native.ScopeEnter.Consider(args);
    }

   #if BID_USE_EXTENSIONS
    internal static void WriteEx(IntPtr hCtx, uint flags, string fmtPrintfW, params object[] args)
    {
        //
        //  WARNING:
        //  PutStr inside fakeOutput uses standard modID but should use hCtx in context of WriteEx.
        //  Shouldn't be a problem in case of generating signatures, because the default
        //  text-streaming BID implementation makes no difference between modID and hCtx.
        //  In fact, SignatureGenerator doesn't need pluggable implementation at all.
        //
        fakeOutput(flags, fmtPrintfW, args);
        Buckets.WriteEx.Consider(args);
        Buckets.Native.Trace.Consider(args);
    }
   #endif


    //
    //  Automatic Initialization / Finalization
    //
    private static Buckets  buckets = null;

    static SignatureGenerator() {
        buckets = new Buckets();
    }

    //=//////////////////////////////////////////////////////////////////////////////////////////

    private sealed class Buckets
    {
        internal static UniqueSignatures    Trace;
        internal static UniqueSignatures    TraceEx;
        internal static UniqueSignatures    ScopeEnter;
        internal static UniqueSignatures    WriteEx;

        internal struct Native {
           internal static UniqueSignatures Trace;
           internal static UniqueSignatures ScopeEnter;
        }

        private Stream          _tempStore;
        private BinaryFormatter _formatter;
        private string          _moduleName;


        internal Buckets()
        {
            _tempStore = null;
            _moduleName = getModuleName();
            initSignatures();
        }

        ~Buckets()
        {
            writeFiles();
        }

        private void writeFiles()
        {
            saveSignatures();

            TextWriter stm = null;
            try {
                stm = new StreamWriter(SignatureFileName);
                writeSignatureFile(stm, SignatureFileName);
            }
            finally {
                if( stm != null ){
                    stm.Close();
                    stm = null;
                }
            }
        }

        private string TempStoreFileName
        {
            get { return _moduleName + "_tempstore.tmp"; }
        }

        private string SignatureFileName
        {
            get { return _moduleName + "_BID.cs"; }
        }

        private string getModuleName()
        {
            string modName;  // AppDomain.CurrentDomain.BaseDirectory;

            modName  = Assembly.GetExecutingAssembly().ManifestModule.Name;

            int len = modName.LastIndexOf('.');
            Bid.DASSERT( len > 0 );
            if( len > 0 ){
                modName = modName.Substring(0, len);
            }
            return modName;
        }

        Stream tryOpenTempStore(bool bWrite)
        {
            Stream store = null;
            try {
                store = bWrite ? File.OpenWrite( TempStoreFileName )
                               : File.OpenRead( TempStoreFileName );
            }
            catch(FileNotFoundException){
                store = null;
            }
            return store;
        }

        private void initSignatures()
        {
            _tempStore = tryOpenTempStore(false);

            try {
                Trace               = newUniqueSignatures();
                TraceEx             = newUniqueSignatures();
                ScopeEnter          = newUniqueSignatures();
                WriteEx             = newUniqueSignatures();
                Native.Trace        = newUniqueSignatures();
                Native.ScopeEnter   = newUniqueSignatures();
            }
            finally {
                if( _tempStore != null ){
                    _tempStore.Close();
                    _tempStore = null;
                }
            }
        }

        private void saveSignatures()
        {
            _tempStore = tryOpenTempStore(true);
            if( _tempStore != null ){
                try {
                    storeIntermediate(Trace);
                    storeIntermediate(TraceEx);
                    storeIntermediate(ScopeEnter);
                    storeIntermediate(WriteEx);
                    storeIntermediate(Native.Trace);
                    storeIntermediate(Native.ScopeEnter);
                }
                finally {
                    _tempStore.Close();
                    _tempStore = null;
                }
            }
        }

        private UniqueSignatures newUniqueSignatures()
        {
            if( _tempStore == null || _tempStore.Length == 0 ){
                return new UniqueSignatures();
            }
            if( _formatter == null ){
                _formatter = new BinaryFormatter();
            }
            return (UniqueSignatures)_formatter.Deserialize(_tempStore);
        }

        private void storeIntermediate(UniqueSignatures usig)
        {
            if( _formatter == null ){
                _formatter = new BinaryFormatter();
            }
            usig.PrepareForIntermediateStore();
            _formatter.Serialize(_tempStore, usig);
        }


        private void writeSignatureFile(TextWriter stm, string moduleName)
        {
            writeHeader(stm, moduleName);

            if( Trace.Count > 0 ) stm.Write(Templates.Headers.Trace);
            Trace.writeSignatures(stm, Patterns.Trace);

            if( TraceEx.Count > 0 ) stm.Write(Templates.Headers.TraceEx);
            TraceEx.writeSignatures(stm, Patterns.TraceEx);

            if( ScopeEnter.Count > 0 ) stm.Write(Templates.Headers.ScopeEnter);
            ScopeEnter.writeSignatures(stm, Patterns.ScopeEnter);

            if( WriteEx.Count > 0 ) stm.Write(Templates.Headers.WriteEx);
            WriteEx.writeSignatures(stm, Patterns.WriteEx);

            stm.Write(Templates.Headers.Native);

            Native.Trace.writeSignatures(stm, Patterns.Native.Trace);
            Native.ScopeEnter.writeSignatures(stm, Patterns.Native.ScopeEnter);


            stm.WriteLine();
            stm.WriteLine("    } // Native");
            stm.WriteLine();
            stm.WriteLine("} // Bid");
        }

    } // Buckets


    //=//////////////////////////////////////////////////////////////////////////////////////////

    struct Templates
    {
        internal struct Headers
        {
            internal const string File =
            "\r\n" +
            "using System;\r\n" +
            "using System.Text;\r\n" +
            "using System.Security;\r\n" +
            "using System.Reflection;\r\n" +
            "using System.Security.Permissions;\r\n" +
            "using System.Runtime.InteropServices;\r\n" +
            "\r\n" +
            "internal static partial class Bid\r\n" +
            "{\r\n" +
            "    //\r\n" +
            "    //  Loader Stub DLL. Can be the assembly itself (mixed mode).\r\n" +
            "    //\r\n" +
            "    private const string dllName = \"BidLdr.dll\";\r\n" +
            "\r\n" +
            "\r\n";

            internal const string Native =
            "    //\r\n" +
            "    // Interop calls to pluggable hooks [SuppressUnmanagedCodeSecurity] applied\r\n" +
            "    //\r\n" +
            "    private static partial class NativeMethods\r\n" +
            "    {\r\n" +
            "\r\n";

            internal const string Trace =
            "    //\r\n" +
            "    //  Trace overloads\r\n" +
            "    //\r\n";

            internal const string TraceEx =
            "    //\r\n" +
            "    //  TraceEx overloads\r\n" +
            "    //\r\n";

            internal const string ScopeEnter =
            "    //\r\n" +
            "    //  ScopeEnter overloads\r\n" +
            "    //\r\n";

            internal const string WriteEx =
            "    //\r\n" +
            "    //  WriteEx overloads\r\n" +
            "    //\r\n";


        } // Headers


        internal const string Trace =
        "    internal static void Trace(string fmtPrintfW^ARGS^) {\r\n" +
        "        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)\r\n" +
        "            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW^ARGUSE^);\r\n" +
        "    }\r\n";

        internal const string TraceEx =
        "    internal static void TraceEx(uint flags, string fmtPrintfW^ARGS^) {\r\n" +
        "        if (modID != NoData)\r\n" +
        "            NativeMethods.Trace (modID, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW^ARGUSE^);\r\n" +
        "    }\r\n";

        internal const string ScopeEnter =
        "    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW^ARGS^) {\r\n" +
        "        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {\r\n" +
        "            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW^ARGUSE^);\r\n" +
        "        } else {\r\n" +
        "            hScp = NoData;\r\n" +
        "        }\r\n" +
        "    }\r\n";

        internal const string WriteEx =
        "    internal static void WriteEx(IntPtr hCtx, uint flags, string fmtPrintfW^ARGS^) {\r\n" +
        "        NativeMethods.Trace (hCtx, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW^ARGUSE^);\r\n" +
        "    }\r\n";


        internal struct Native
        {
            internal const string Trace =
            "        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint=\"DllBidTraceCW\")] extern\r\n" +
            "        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW^ARGS^);\r\n";

            internal const string ScopeEnter =
            "        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint=\"DllBidScopeEnterCW\")] extern\r\n" +
            "        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW^ARGS^);\r\n";

        } // Native


    } // Templates


    private sealed class Patterns
    {
        internal static string[]    Trace;
        internal static string[]    TraceEx;
        internal static string[]    ScopeEnter;
        internal static string[]    WriteEx;

        internal struct Native {
           internal static string[] Trace;
           internal static string[] ScopeEnter;
        }

        static Patterns()
        {
            init(ref Trace,             Templates.Trace);
            init(ref TraceEx,           Templates.TraceEx);
            init(ref ScopeEnter,        Templates.ScopeEnter);
            init(ref WriteEx,           Templates.WriteEx);

            init(ref Native.Trace,      Templates.Native.Trace);
            init(ref Native.ScopeEnter, Templates.Native.ScopeEnter);
        }

        private static void init(ref string[] pattern, string templateString) {
            pattern = templateString.Split('^');
        }

    } // Patterns


} // SignatureGenerator


#endif  // BID_AUTOSIG
