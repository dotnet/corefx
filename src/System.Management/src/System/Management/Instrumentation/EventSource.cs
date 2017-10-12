// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
namespace System.Management.Instrumentation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Security;
    using System.Globalization;
    using System.Runtime.Versioning;

    class ComThreadingInfo
    {
        public enum APTTYPE
        {
            APTTYPE_CURRENT = -1,
            APTTYPE_STA = 0,
            APTTYPE_MTA = 1,
            APTTYPE_NA  = 2,
            APTTYPE_MAINSTA = 3
        }

        public enum THDTYPE
        {
            THDTYPE_BLOCKMESSAGES   = 0,
            THDTYPE_PROCESSMESSAGES = 1
        }

        [ComImport]
        [Guid("000001ce-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IComThreadingInfo 
        {
            APTTYPE GetCurrentApartmentType();
            THDTYPE GetCurrentThreadType();
            Guid GetCurrentLogicalThreadId();
            void SetCurrentLogicalThreadId([In] Guid rguid);
        }

        Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        ComThreadingInfo()
        {
            IComThreadingInfo info = (IComThreadingInfo)CoGetObjectContext(ref IID_IUnknown);

            apartmentType  = info.GetCurrentApartmentType();
            threadType = info.GetCurrentThreadType();
            logicalThreadId = info.GetCurrentLogicalThreadId();
        }

        public static ComThreadingInfo Current
        {
            get
            {
                return new ComThreadingInfo();
            }
        }

        public override string ToString()
        {
            return String.Format("{{{0}}} - {1} - {2}", LogicalThreadId, ApartmentType, ThreadType);
        }

        APTTYPE apartmentType;
        THDTYPE threadType;
        Guid logicalThreadId;

        public APTTYPE ApartmentType { get { return apartmentType; } 
        }
        public THDTYPE ThreadType { get { return threadType; } 
        }
        public Guid LogicalThreadId { get { return logicalThreadId; } 
        }

        [ResourceExposure( ResourceScope.None),DllImport("ole32.dll", PreserveSig = false)]
        [return:MarshalAs(UnmanagedType.IUnknown)]
        static extern object CoGetObjectContext([In] ref Guid riid);
    }

    sealed class EventSource : IWbemProviderInit, IWbemEventProvider, IWbemEventProviderQuerySink, IWbemEventProviderSecurity, IWbemServices_Old
    {
        IWbemDecoupledRegistrar registrar = (IWbemDecoupledRegistrar)new WbemDecoupledRegistrar();

        static ArrayList eventSources = new ArrayList();

        InstrumentedAssembly instrumentedAssembly;

        public EventSource(string namespaceName, string appName, InstrumentedAssembly instrumentedAssembly)
        {
            lock(eventSources)
            {
                // This is a pathalogical case where the process is shutting down, but
                // someone wants us to register us as a provider.  In this case, we ignore
                // the request.  This would be problematic if the eventSources list has already
                // been enumerated and all other eventSources unregistered.  If we were allowed
                // to register a new event source, it would never be unregistered
                if(shutdownInProgress != 0)
                    return;

                this.instrumentedAssembly = instrumentedAssembly;
             int hr = registrar.Register_(0, null, null, null, namespaceName, appName, this);

                if(hr != 0)
                    Marshal.ThrowExceptionForHR(hr, WmiNetUtilsHelper.GetErrorInfo_f());

                // If we've gotten here, we were successful with the Register call
                eventSources.Add(this);
            }
        }

#if OldShutdown
        ~EventSource()
        {
            // Kill worker thread
            if(lengthFromSTA != -1)
            {
                alive = false;
                SetEvent(hDoIndicate);
//                doIndicate.Set();
            }

            registrar.UnRegister_();
        }
#endif

        ~EventSource()
        {
            // BUGBUG:
            // Hopefully the event source will have already been unregistered,
            // but just in case, we'll make sure it is.
            // This can currently happen if we are loaded into the non-default
            // AppDomain, and no-one calls AppDomain.Unload().  In this case,
            // we won't get AppDomain.ProcessExit, or AppDomain.DomainUnload.
            // When this happens, this destructor will be called during the
            // 'finalize for process shutdown' routine.  If we have an existing
            // incoming call on IWbemServices, it might be terminated abnormally.
            // This can unfortunately cause WMI to wait indefinately for us.
            UnRegister();
        }

        void UnRegister()
        {
            lock(this)
            {
                // Only unregister one time
                if(registrar == null)
                    return;

                // Kill worker thread
                if(workerThreadInitialized == true)
                {
                    alive = false;
                    doIndicate.Set(); // SetEvent(hDoIndicate);
                    GC.KeepAlive(this);
                    workerThreadInitialized = false;
                }
                registrar.UnRegister_();
                registrar = null;
            }
        }

        static int shutdownInProgress = 0;
        static ReaderWriterLock preventShutdownLock = new ReaderWriterLock();
        static void ProcessExit(object o, EventArgs args)
        {
            // We can be called multiple times, but only one time will we
            // try to unregister all the event sources
            if(shutdownInProgress != 0)
                return;

            // Notify anyone interested that a shutdown as been initiated
            Interlocked.Increment(ref shutdownInProgress);

            try
            {
                // Wait for anyone currently in a DCOM callback to finish
                preventShutdownLock.AcquireWriterLock(-1);

                // Unregister all existing event sources
                lock(eventSources)
                {
                    foreach(EventSource eventSource in eventSources)
                        eventSource.UnRegister();
                }
            }
            finally
            {
                // Let any straglers call start their calls
                preventShutdownLock.ReleaseWriterLock();
                Thread.Sleep(50);

                // Wait for any stragler that started
                preventShutdownLock.AcquireWriterLock(-1);
                preventShutdownLock.ReleaseWriterLock();
            }
        }
        static EventSource()
        {
            // We'll register for both ProcessExit and DomainUnload.  It is OK
            // if both are called, but bad if neither are called.  See comments
            // in ~EventSource();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(ProcessExit);
        }

        IWbemServices pNamespaceNA = null;
        IWbemObjectSink pSinkNA = null;
        IWbemServices pNamespaceMTA = null;
        IWbemObjectSink pSinkMTA = null;

        private class MTARequest
        {
            public AutoResetEvent doneIndicate = new AutoResetEvent(false);
            public Exception exception = null;
            public int lengthFromSTA = -1;
            public IntPtr[] objectsFromSTA;
            public MTARequest(int length, IntPtr[] objects)
            {
                this.lengthFromSTA = length;
                this.objectsFromSTA = objects;
            }
        }

        ArrayList reqList = new ArrayList(3);
        object critSec = new object();
        
        AutoResetEvent doIndicate = new AutoResetEvent(false);
        

        bool workerThreadInitialized = false;
        /*
        [DllImport("kernel32")] static extern IntPtr CreateEvent(IntPtr pSecurity, int manual, int initial, IntPtr name);
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")] static extern int SetEvent(IntPtr handle);
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")] static extern int WaitForSingleObject(IntPtr handle, int timeout);
        IntPtr hDoIndicate = CreateEvent(IntPtr.Zero, 0, 0, IntPtr.Zero);
        IntPtr hDoneIndicate = CreateEvent(IntPtr.Zero, 0, 0, IntPtr.Zero);
        */

        // Worker thread for each EventSource

        

        //
        // Ensure we are able to trap exceptions from worker thread.
        // Called from IndicateEvents which in turn is protected from
        // concurrent access.
        //
        

        public void MTAWorkerThread2()
        {
            while(true)
            {
                doIndicate.WaitOne(); //WaitForSingleObject(hDoIndicate, -1);
                if(alive == false)
                {
                    break;
                }            
                 // get requests from the request queue. Since two Set within short time on evtGo can wake this thread only once
                // workerthread should check until we empty all the results. Even if we consume the request that is not set,
                // workerthread will wake up one more time unnecessarily and do nothing
                while(true)
                {
                    MTARequest reqToProcess = null;
                    lock (critSec)
                    {
                        if (reqList.Count > 0)
                        {
                            reqToProcess = (MTARequest)reqList[0];
                            reqList.RemoveAt(0);
                        }
                        else
                        {
                            break;  // break the inner while true
                        }
                    }
                    try
                    {
                        if (pSinkMTA != null)
                        {
                            int hresult = pSinkMTA.Indicate_(reqToProcess.lengthFromSTA, reqToProcess.objectsFromSTA);
                            if (hresult < 0)
                            {
                                if ((hresult & 0xfffff000) == 0x80041000)
                                    ManagementException.ThrowWithExtendedInfo((ManagementStatus)hresult);
                                else
                                    Marshal.ThrowExceptionForHR(hresult, WmiNetUtilsHelper.GetErrorInfo_f());
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        reqToProcess.exception = e;
                    }
                    finally
                    {
                        reqToProcess.doneIndicate.Set(); //SetEvent(hDoneIndicate);
                        GC.KeepAlive(this);
                    }
                }
            }
        }


        bool alive = true;

        public void IndicateEvents(int length, IntPtr[] objects)
        {
            if (pSinkMTA == null)
                return;
            //
            // Checking for MTA is not enough.  We need to make sure we are not in a COM+ Context
            //
            if (MTAHelper.IsNoContextMTA())
            {
                // Sink lives in MTA
                int hresult = pSinkMTA.Indicate_(length, objects);
                if (hresult < 0)
                {
                    if ((hresult & 0xfffff000) == 0x80041000)
                        ManagementException.ThrowWithExtendedInfo((ManagementStatus)hresult);
                    else
                        Marshal.ThrowExceptionForHR(hresult, WmiNetUtilsHelper.GetErrorInfo_f());
                }
            }
            else
            {
                MTARequest myReq = new MTARequest(length,objects);
                int ndx;
                lock(critSec)
                {
                    // Do it from worker thread
                    if (workerThreadInitialized == false)
                    {
                        // We've never created worker thread
                        Thread thread = new Thread(new ThreadStart(MTAWorkerThread2));
                        thread.IsBackground = true;
                        thread.SetApartmentState(ApartmentState.MTA);
                        thread.Start();
                        workerThreadInitialized = true;
                    }
                    ndx = reqList.Add(myReq);
                    if( doIndicate.Set() == false ) //SetEvent(hDoIndicate);
                    {
                        reqList.RemoveAt(ndx);
                        throw new ManagementException(RC.GetString("WORKER_THREAD_WAKEUP_FAILED"));
                    }
                }
                myReq.doneIndicate.WaitOne(); //WaitForSingleObject(hDoneIndicate, -1);
                if (myReq.exception != null)
                {
                    throw myReq.exception;
                }
            }
            GC.KeepAlive(this); 
        }

        void RelocateSinkRCWToMTA()
        {
            ThreadDispatch disp = new ThreadDispatch(new ThreadDispatch.ThreadWorkerMethodWithParam(RelocateSinkRCWToMTA_ThreadFuncion));
            disp.Parameter = this;
            disp.Start();
        }

        void RelocateSinkRCWToMTA_ThreadFuncion( object param )
        {
            EventSource threadParam = (EventSource) param;
            threadParam.pSinkMTA = (IWbemObjectSink)RelocateRCWToCurrentApartment(threadParam.pSinkNA);
            threadParam.pSinkNA = null;
        }

        void RelocateNamespaceRCWToMTA()
        {
            ThreadDispatch disp = new ThreadDispatch(new ThreadDispatch.ThreadWorkerMethodWithParam(RelocateNamespaceRCWToMTA_ThreadFuncion));
            disp.Parameter = this;
            disp.Start();
        }

        void RelocateNamespaceRCWToMTA_ThreadFuncion(object param)
        {
            EventSource threadParam = (EventSource) param ;
            threadParam.pNamespaceMTA = (IWbemServices)RelocateRCWToCurrentApartment(threadParam.pNamespaceNA);
            threadParam.pNamespaceNA = null;
        }

        static object RelocateRCWToCurrentApartment(object comObject)
        {
            if(null == comObject)
                return null;

            IntPtr pUnk = Marshal.GetIUnknownForObject(comObject);
            int references = Marshal.ReleaseComObject(comObject);
            if (references != 0)
                throw new Exception();

            comObject = Marshal.GetObjectForIUnknown(pUnk);
            Marshal.Release(pUnk);
            return comObject;
        }

        public bool Any()
        {
            return (null == pSinkMTA) || (mapQueryIdToQuery.Count == 0);
        }

        Hashtable mapQueryIdToQuery = new Hashtable();

        // IWbemProviderInit

        int IWbemProviderInit.Initialize_(
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszUser,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszNamespace,
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszLocale,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemServices   pNamespace,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemProviderInitSink   pInitSink)
        {
            this.pNamespaceNA = pNamespace;
            RelocateNamespaceRCWToMTA();

            this.pSinkNA = null;
            this.pSinkMTA = null;

            lock(mapQueryIdToQuery)
            {
                mapQueryIdToQuery.Clear();
            }

            pInitSink.SetStatus_((int)tag_WBEM_EXTRA_RETURN_CODES.WBEM_S_INITIALIZED, 0);
            Marshal.ReleaseComObject(pInitSink);

            return 0;
        }

        // IWbemEventProvider
        int IWbemEventProvider.ProvideEvents_(
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pSink,
            [In] Int32 lFlags)
        {
            this.pSinkNA = pSink;
            RelocateSinkRCWToMTA();

            // TODO: Why do we get NewQuery BEFORE ProvideEvents?
            //mapQueryIdToQuery.Clear();
            return 0;
        }

        // IWbemEventProviderQuerySink

        int IWbemEventProviderQuerySink.NewQuery_(
            [In] UInt32 dwId,
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQueryLanguage,
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQuery)
        {
            lock(mapQueryIdToQuery)
            {
                // HACK: for bug where CancelQuery is not called correctly
                // and we get a NewQuery where dwId is already in our hashtable
                if(mapQueryIdToQuery.ContainsKey(dwId))
                    mapQueryIdToQuery.Remove(dwId);
                mapQueryIdToQuery.Add(dwId, wszQuery);
            }
            return 0;
        }

        int IWbemEventProviderQuerySink.CancelQuery_([In] UInt32 dwId)
        {
            lock(mapQueryIdToQuery)
            {
                mapQueryIdToQuery.Remove(dwId);
            }
            return 0;
        }

        // IWbemEventProviderSecurity
        int IWbemEventProviderSecurity.AccessCheck_(
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQueryLanguage,
            [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQuery,
            [In] Int32 lSidLength,
            [In] ref Byte pSid)
        {
            return 0;
        }

        // IWbemServices
        int IWbemServices_Old.OpenNamespace_([In][MarshalAs(UnmanagedType.BStr)]  string   strNamespace,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][Out][MarshalAs(UnmanagedType.Interface)]  ref IWbemServices   ppWorkingNamespace,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.CancelAsyncCall_([In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pSink)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.QueryObjectSink_([In] Int32 lFlags,
            [Out][MarshalAs(UnmanagedType.Interface)]  out IWbemObjectSink   ppResponseHandler)
        {
            ppResponseHandler = null;
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.GetObject_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][Out][MarshalAs(UnmanagedType.Interface)]  ref IWbemClassObject_DoNotMarshal   ppObject,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.GetObjectAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
//            pResponseHandler.SetStatus(0, (int)tag_WBEMSTATUS.WBEM_E_NOT_FOUND, null, null);
//            Marshal.ReleaseComObject(pResponseHandler);
//            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_FOUND);

            Match match = Regex.Match(strObjectPath.ToLower(CultureInfo.InvariantCulture), "(.*?)\\.instanceid=\"(.*?)\",processid=\"(.*?)\"");
            if(match.Success==false)
            {
                pResponseHandler.SetStatus_(0, (int)tag_WBEMSTATUS.WBEM_E_NOT_FOUND, null, IntPtr.Zero);
                Marshal.ReleaseComObject(pResponseHandler);
                return (int)(tag_WBEMSTATUS.WBEM_E_NOT_FOUND);
            }

            string className = match.Groups[1].Value;
            string instanceId = match.Groups[2].Value;
            string processId = match.Groups[3].Value;


            if(Instrumentation.ProcessIdentity != processId)
            {
                pResponseHandler.SetStatus_(0, (int)tag_WBEMSTATUS.WBEM_E_NOT_FOUND, null, IntPtr.Zero);
                Marshal.ReleaseComObject(pResponseHandler);
                return (int)(tag_WBEMSTATUS.WBEM_E_NOT_FOUND);
            }

            int id = ((IConvertible)instanceId).ToInt32((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int32)));
            Object theObject = null;

            try
            {
                InstrumentedAssembly.readerWriterLock.AcquireReaderLock(-1);
                theObject = InstrumentedAssembly.mapIDToPublishedObject[id.ToString((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int32)))];
            }
            finally
            {
                InstrumentedAssembly.readerWriterLock.ReleaseReaderLock();
            }

            if(theObject != null)
            {
                Type converterType = (Type)instrumentedAssembly.mapTypeToConverter[theObject.GetType()];
                if(converterType != null)
                {
                    Object converter = Activator.CreateInstance(converterType);
                    ConvertToWMI func = (ConvertToWMI)Delegate.CreateDelegate(typeof(ConvertToWMI), converter, "ToWMI");

            //
            // Regression: Reuters VSQFE#: 750, PS#141144    [marioh]
            // GetObjectAsync was missed. Again, here we have to call ToWMI before retrieving the pointer to the object
            // since we clone a new one during the ToWMI call. The code below was simply moved from a location further down
            // 
            lock(theObject)
                func(theObject);
            //
            // END: Regression: Reuters VSQFE#: 750, PS#141144    [marioh]

                    
                    IntPtr[] objs = new IntPtr[] {(IntPtr)converter.GetType().GetField("instWbemObjectAccessIP").GetValue(converter)};
                    Marshal.AddRef(objs[0]);
                    IWbemClassObjectFreeThreaded inst = new IWbemClassObjectFreeThreaded(objs[0]);

                    Object o = id;
                    inst.Put_("InstanceId", 0, ref o, 0);
                    o = Instrumentation.ProcessIdentity;
                    inst.Put_("ProcessId", 0, ref o, 0);
                    //               ConvertFuncToWMI func = (ConvertFuncToWMI)InstrumentedAssembly.mapTypeToToWMIFunc[h.Target.GetType()];
            //
            // Reuters VSQFE#: 750, PS#141144    [marioh]
            // The commented out code was moved up before accessing the object pointer in order to get the
            // newly cloned one.
            // 
//                    lock(theObject)
//                        func(theObject);
                    pResponseHandler.Indicate_(1, objs);

                    pResponseHandler.SetStatus_(0, 0, null, IntPtr.Zero);
                    Marshal.ReleaseComObject(pResponseHandler);
                    return 0;
                }
            }
            pResponseHandler.SetStatus_(0, (int)tag_WBEMSTATUS.WBEM_E_NOT_FOUND, null, IntPtr.Zero);
            Marshal.ReleaseComObject(pResponseHandler);
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_FOUND);
#if xxx
            IWbemClassObject classObj = null;
            IWbemCallResult result = null;
            pNamespace.GetObject("TestInstance", 0, pCtx, ref classObj, ref result);
            IWbemClassObject inst;
            classObj.SpawnInstance(0, out inst);

            TestInstance testInstance = (TestInstance)mapNameToTestInstance[match.Groups[1].Value];

            Object o = (object)testInstance.name;
            inst.Put("name", 0, ref o, 0);
            o = (object)testInstance.value;
            inst.Put("value", 0, ref o, 0);

            pResponseHandler.Indicate(1, new IWbemClassObject[] {inst});
            pResponseHandler.SetStatus_(0, 0, IntPtr.Zero, IntPtr.Zero);
            Marshal.ReleaseComObject(pResponseHandler);
#endif
        }

        int IWbemServices_Old.PutClass_([In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pObject,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.PutClassAsync_([In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pObject,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.DeleteClass_([In][MarshalAs(UnmanagedType.BStr)]  string   strClass,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.DeleteClassAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strClass,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.CreateClassEnum_([In][MarshalAs(UnmanagedType.BStr)]  string   strSuperclass,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [Out][MarshalAs(UnmanagedType.Interface)]  out IEnumWbemClassObject   ppEnum)
        {
            ppEnum = null;
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.CreateClassEnumAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strSuperclass,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.PutInstance_([In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pInst,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.PutInstanceAsync_([In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pInst,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.DeleteInstance_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.DeleteInstanceAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.CreateInstanceEnum_([In][MarshalAs(UnmanagedType.BStr)]  string   strFilter,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [Out][MarshalAs(UnmanagedType.Interface)]  out IEnumWbemClassObject   ppEnum)
        {
            ppEnum = null;
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.CreateInstanceEnumAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strFilter,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            try
            {
                // Make sure we do not let a shutdown kill this thread
                preventShutdownLock.AcquireReaderLock(-1);

                // If we are already shutting down, don't do anything
                if(shutdownInProgress != 0)
                    return 0;

                // If batching takes longer than 1/10 of a second, we want to stop
                // and flush the batch
                uint timeLimitForBatchFlush = (uint)Environment.TickCount + 100;

                // Find the managed type that is being requested by this call
                Type managedType = null;
                foreach(Type type in instrumentedAssembly.mapTypeToConverter.Keys)
                {
                    if(0==String.Compare(ManagedNameAttribute.GetMemberName(type), strFilter, StringComparison.Ordinal))
                    {
                        managedType = type;
                        break;
                    }
                }

                // If we do not support the requested type, just exit
                if(null == managedType)
                    return 0;

                // size for batching
                int batchSize = 64;

                // Array of IWbemClassObject IntPtrs for batching
                IntPtr[] objs = new IntPtr[batchSize];
                IntPtr[] objsClone = new IntPtr[batchSize];

                // Array of converter methods for batching
                ConvertToWMI[] funcs = new ConvertToWMI[batchSize];

                // Array of IWbemClassObjectFreeThreaded instances for batching
                IWbemClassObjectFreeThreaded[] insts = new IWbemClassObjectFreeThreaded[batchSize];

                // IWbemObjectAccess handle for 'InstanceId'
                int handleInstanceId = 0;

                // Current number of objects in batch
                int count = 0;

                // The process identity string
                Object processIdentity = Instrumentation.ProcessIdentity;

                // Walk all published instances
                try
                {
                    // Don't let anyone add entries to the dictionary while we are enumerating it.
                    // Other people can read from the dictionary
                    InstrumentedAssembly.readerWriterLock.AcquireReaderLock(-1);
                    foreach(DictionaryEntry entry in InstrumentedAssembly.mapIDToPublishedObject)
                    {
                        // If the process is going away, stop indicating more instances
                        if(shutdownInProgress != 0)
                            return 0;

                        // Is this object the type requested by this query?
                        if(managedType != entry.Value.GetType())
                            continue;

                        // Initialize this batch entry if necessary.  Each element of the batch arrays
                        // are initialized 'just in time'.  If we have less than batchSize entries total
                        // (or it takes too long to batch), we do not unnecessarily allocate the batch entry
                        if(funcs[count] == null)
                        {
                            Object converter = Activator.CreateInstance((Type)instrumentedAssembly.mapTypeToConverter[managedType]);
                            funcs[count] = (ConvertToWMI)Delegate.CreateDelegate(typeof(ConvertToWMI), converter, "ToWMI");

                            //
                            // Reuters VSQFE#: 750    [marioh]
                            // In order for the instance data batching logic to work properly, we HAVE TO convert the .NET
                            // objects to WMI objects before we update the batching pointers since the ToWMI method in the generated
                            // code will Spawn new instances.
                            // 
                            lock(entry.Value)
                                funcs[count](entry.Value);

                            objs[count] = (IntPtr)converter.GetType().GetField("instWbemObjectAccessIP").GetValue(converter);
                            Marshal.AddRef(objs[count]);
                            insts[count] = new IWbemClassObjectFreeThreaded(objs[count]);
                            insts[count].Put_("ProcessId", 0, ref processIdentity, 0);

                            int cimType;
                            if(count==0)
                                WmiNetUtilsHelper.GetPropertyHandle_f27(27, insts[count], "InstanceId", out cimType, out handleInstanceId);
                        }
                        else
                        {
                            //
                            // Reuters VSQFE#: 750    [marioh]
                            // If we end up re-using an existing delegate from the batch array, we still have to convert
                            // the .NET to WMI objects and update the instance pointers.
                            //

                            // Copy the managed instance information into the IWbemClassObject'
                            // We're using a batch, therefore we have to convert them.
                            lock(entry.Value)
                                funcs[count](entry.Value);

                            objs[count] = (IntPtr) funcs[count].Target.GetType().GetField("instWbemObjectAccessIP").GetValue(funcs[count].Target);

                            //
                            // We have to AddRef the interface pointer due to the IWbemClassObjectFreeThreaded not addreffing
                            // but releasing in the destructor. Great huh?
                            //
                            Marshal.AddRef(objs[count]);
                            insts[count] = new IWbemClassObjectFreeThreaded(objs[count]);
                            insts[count].Put_("ProcessId", 0, ref processIdentity, 0);

                            int cimType;
                            if(count==0)
                                WmiNetUtilsHelper.GetPropertyHandle_f27(27, insts[count], "InstanceId", out cimType, out handleInstanceId);
                        }

                        // We have an instance to publish.  Store the instance ID in 'InstanceId'
                        string instanceId = (string)entry.Key;
                        WmiNetUtilsHelper.WritePropertyValue_f28(28, insts[count], handleInstanceId, (instanceId.Length+1)*2, instanceId);

                        //                        // Copy the managed instance information into the IWbemClassObject'
                        //                        lock(entry.Value)
                        //                            funcs[count](entry.Value);

                        // Increment the batch counter
                        count++;

                        // If we've reached batchSize, or if we've gone longer than 1/10th second since
                        // an Indicate, flush the batch
                        if(count == batchSize || ((uint)Environment.TickCount) >= timeLimitForBatchFlush)
                        {
                            // Do the Indicate to WMI
                            // NOTE: On WinXP, we cannot control whether the implementation of
                            // Indicate will immediately send the objects to WMI, or if it will
                            // batch them.  If it batches them, we cannot reuse them in a future
                            // call to Indicate, or change them in any way after the call to
                            // Indicate.  Because of this, we will always 'Clone' the objects
                            // just before calling Indicate.  The performance is negligable, even
                            // on Windows 2000, which can handle about 200,000 Clones per second
                            // on a 1 GHz machine.
                            for(int i=0;i<count;i++)
                                WmiNetUtilsHelper.Clone_f(12, objs[i], out objsClone[i]);
                            int hr = pResponseHandler.Indicate_(count, objsClone);
                            for(int i=0;i<count;i++)
                                Marshal.Release(objsClone[i]);

                            // If hr is not S_OK, we stop the enumeration.  This can happen if the
                            // client cancels the call.
                            if(hr != 0 )
                                return 0;

                            // Reset the batch counter
                            count = 0;

                            // Reset the time limit for another 1/10th second in the future
                            timeLimitForBatchFlush = (uint)Environment.TickCount + 100;
                        }
                    }
                }
                finally
                {
                    InstrumentedAssembly.readerWriterLock.ReleaseReaderLock();
                }
                if(count > 0)
                {
                    // NOTE: On WinXP, we cannot control whether the implementation of
                    // Indicate will immediately send the objects to WMI, or if it will
                    // batch them.  If it batches them, we cannot reuse them in a future
                    // call to Indicate, or change them in any way after the call to
                    // Indicate.  Because of this, we will always 'Clone' the objects
                    // just before calling Indicate.  The performance is negligable, even
                    // on Windows 2000, which can handle about 200,000 Clones per second
                    // on a 1 GHz machine.
                    for(int i=0;i<count;i++)
                    {
                         WmiNetUtilsHelper.Clone_f(12, objs[i], out objsClone[i]);
                    }
                    pResponseHandler.Indicate_(count, objsClone);
                    for(int i=0;i<count;i++)
                        Marshal.Release(objsClone[i]);
                }
            }
            finally
            {
                pResponseHandler.SetStatus_(0, 0, null, IntPtr.Zero);
                Marshal.ReleaseComObject(pResponseHandler);
                preventShutdownLock.ReleaseReaderLock();
            }
            return 0;
        }

        int IWbemServices_Old.ExecQuery_([In][MarshalAs(UnmanagedType.BStr)]  string   strQueryLanguage,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strQuery,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [Out][MarshalAs(UnmanagedType.Interface)]  out IEnumWbemClassObject   ppEnum)
        {
            ppEnum = null;
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.ExecQueryAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strQueryLanguage,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strQuery,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.ExecNotificationQuery_([In][MarshalAs(UnmanagedType.BStr)]  string   strQueryLanguage,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strQuery,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [Out][MarshalAs(UnmanagedType.Interface)]  out IEnumWbemClassObject   ppEnum)
        {
            ppEnum = null;
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.ExecNotificationQueryAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strQueryLanguage,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strQuery,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.ExecMethod_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strMethodName,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pInParams,
            [In][Out][MarshalAs(UnmanagedType.Interface)]  ref IWbemClassObject_DoNotMarshal   ppOutParams,
            [In] IntPtr ppCallResult)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }

        int IWbemServices_Old.ExecMethodAsync_([In][MarshalAs(UnmanagedType.BStr)]  string   strObjectPath,
            [In][MarshalAs(UnmanagedType.BStr)]  string   strMethodName,
            [In] Int32 lFlags,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pCtx,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject_DoNotMarshal   pInParams,
            [In][MarshalAs(UnmanagedType.Interface)]  IWbemObjectSink   pResponseHandler)
        {
            return (int)(tag_WBEMSTATUS.WBEM_E_NOT_SUPPORTED);
        }
    }
}


#if UNUSED_CODE
    class HResultException : Exception
    {
        public HResultException(int hr)
        {
            base.HResult = hr;
        }
    }

    class WbemStatusException : HResultException
    {
        public WbemStatusException(tag_WBEMSTATUS status) : base((int)status) {}
    }

    /*********************************************
     * Wbem Internal
     *********************************************/
    [TypeLibTypeAttribute(0x0200)]
    [InterfaceTypeAttribute(0x0001)]
    [GuidAttribute("6C19BE32-7500-11D1-AD94-00C04FD8FDFF")]
    [ComImport]
    interface IWbemMetaData
    {
        void GetClass([In][MarshalAs(UnmanagedType.LPWStr)]  string   wszClassName, [In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext, [Out][MarshalAs(UnmanagedType.Interface)]  out IWbemClassObject   ppClass);
    } // end of IWbemMetaData

    [GuidAttribute("755F9DA6-7508-11D1-AD94-00C04FD8FDFF")]
    [TypeLibTypeAttribute(0x0200)]
    [InterfaceTypeAttribute(0x0001)]
    [ComImport]
    interface IWbemMultiTarget
    {
        void DeliverEvent([In] UInt32 dwNumEvents, [In][MarshalAs(UnmanagedType.Interface)]  ref IWbemClassObject   aEvents, [In] ref tag_WBEM_REM_TARGETS aTargets);
        void DeliverStatus([In] Int32 lFlags, [In][MarshalAs(UnmanagedType.Error)]  Int32   hresStatus, [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszStatus, [In][MarshalAs(UnmanagedType.Interface)]  IWbemClassObject   pErrorObj, [In] ref tag_WBEM_REM_TARGETS pTargets);
    } // end of IWbemMultiTarget

    struct tag_WBEM_REM_TARGETS
    {
        public Int32 m_lNumTargets;

        //        [ComConversionLossAttribute]
        //        public IntPtr m_aTargets;
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] UInt32[]  m_aTargets;
    } // end of tag_WBEM_REM_TARGETS
  
    [GuidAttribute("60E512D4-C47B-11D2-B338-00105A1F4AAF")]
    [TypeLibTypeAttribute(0x0200)]
    [InterfaceTypeAttribute(0x0001)]
    [ComImport]
    interface IWbemFilterProxy
    {
        void Initialize([In][MarshalAs(UnmanagedType.Interface)]  IWbemMetaData   pMetaData, [In][MarshalAs(UnmanagedType.Interface)]  IWbemMultiTarget   pMultiTarget);
        void Lock();
        void Unlock();
        void AddFilter([In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext, [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQuery, [In] UInt32 Id);
        void RemoveFilter([In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext, [In] UInt32 Id);
        void RemoveAllFilters([In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext);
        void AddDefinitionQuery([In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext, [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszQuery);
        void RemoveAllDefinitionQueries([In][MarshalAs(UnmanagedType.Interface)]  IWbemContext   pContext);
        void Disconnect();
    } // end of IWbemFilterProxy

    [TypeLibTypeAttribute(0x0202)]
    [GuidAttribute("6C19BE35-7500-11D1-AD94-00C04FD8FDFF")]
    [ClassInterfaceAttribute((short)0x0000)]
    [ComImport]
    class WbemFilterProxy  /*: IWbemFilterProxy, IWbemObjectSink*/
    {
    } // end of WbemFilterProxy
#endif

