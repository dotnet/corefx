// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define USETLBIMP
//#define USEIWOS

using System;
using WbemClient_v1;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
#if USETLBIMP
using WMISECLib;
#endif

namespace System.Management
{

#if USETLBIMP
internal class WmiEventSink : WMISECLib.IWmiEventSource
#elif USEIWOS
internal class WmiEventSink : IWbemObjectSink
#else
internal class WmiEventSink : IWmiEventSource
#endif
{
    private static int                      s_hash = 0;
    private int                             hash;
    private ManagementOperationObserver     watcher;
    private object                          context;
    private ManagementScope                 scope;
    private object                          stub;           // The secured IWbemObjectSink

    // Used for Put's only
    internal event InternalObjectPutEventHandler  InternalObjectPut;
    private ManagementPath                  path;           
    private string                          className;
    private bool                            isLocal;


    static ManagementOperationObserver watcherParameter;
    static object contextParameter; 
    static ManagementScope scopeParameter;
    static string pathParameter;
    static string classNameParameter;
    static WmiEventSink wmiEventSinkNew;

    internal static WmiEventSink GetWmiEventSink(
        ManagementOperationObserver watcher,
        object context, 
        ManagementScope scope,
        string path,
        string className)
    {
        if(MTAHelper.IsNoContextMTA()) // Bug#110141 - Checking for MTA is not enough.  We need to make sure we are not in a COM+ Context
            return new WmiEventSink(watcher, context, scope, path, className);

        watcherParameter = watcher;
        contextParameter = context;
        scopeParameter = scope;
        pathParameter = path;
        classNameParameter = className;

        //
        // [marioh, RAID: 111108]
        // Ensure we are able to trap exceptions from worker thread.
        //
        ThreadDispatch disp = new ThreadDispatch ( new ThreadDispatch.ThreadWorkerMethod ( HackToCreateWmiEventSink ) ) ;
        disp.Start ( ) ;

//        Thread thread = new Thread(new ThreadStart(HackToCreateWmiEventSink));
//        thread.Start(); // TODO: What if this throws an exception
//        thread.Join();
        return wmiEventSinkNew;
    }

    static void HackToCreateWmiEventSink()
    {
        wmiEventSinkNew = new WmiEventSink(watcherParameter, contextParameter, scopeParameter, pathParameter, classNameParameter);
    }

    protected WmiEventSink (ManagementOperationObserver watcher,
                         object context, 
                         ManagementScope scope,
                         string path,
                         string className)
    {
        try {
            this.context = context;
            this.watcher = watcher;
            this.className = className;
            this.isLocal = false;

            if (null != path)
            {
                this.path = new ManagementPath (path);
                if((0==String.Compare(this.path.Server, ".", StringComparison.OrdinalIgnoreCase)) ||
                    (0==String.Compare(this.path.Server, System.Environment.MachineName, StringComparison.OrdinalIgnoreCase)))
                {
                            this.isLocal = true;
                }
            }

            if (null != scope)
            {
                this.scope = (ManagementScope) scope.Clone ();
                if (null == path) // use scope to see if sink is local
                {
                    if((0==String.Compare(this.scope.Path.Server, ".", StringComparison.OrdinalIgnoreCase)) ||
                        (0==String.Compare(this.scope.Path.Server, System.Environment.MachineName, StringComparison.OrdinalIgnoreCase)))
                    {
                                this.isLocal = true;
                    }
                }
            }
#if USETLBIMP
            WmiNetUtilsHelper.GetDemultiplexedStub_f (this, this.isLocal, ref m_stub);
#elif USEIWOS 
            IUnsecuredApartment unsecApp = new UnsecuredApartment ();
            unsecApp.CreateObjectStub (this, ref m_stub);
#else
        WmiNetUtilsHelper.GetDemultiplexedStub_f (this, this.isLocal, out stub);
#endif
            hash = Threading.Interlocked.Increment(ref s_hash);
        } catch {}
    }

    public override int GetHashCode () {
        return hash;
    }

    public IWbemObjectSink Stub { 
        get {           
            try {
                return (null != stub) ? (IWbemObjectSink) stub : null; 
            } catch {
                return null;
            }
        }
    }

#if USEIWOS
    public virtual void Indicate (long lNumObjects, IWbemClassObject [] objArray)
    {
        try {
            for (long i = 0; i < lNumObjects; i++) {
                ObjectReadyEventArgs args = new ObjectReadyEventArgs (m_context, 
                    new WmiObject(m_services, objArray[i]));
                watcher.FireObjectReady (args);
            }
        } catch {}
    }
#else
    public virtual void Indicate (IntPtr pIWbemClassObject)
    {
        Marshal.AddRef(pIWbemClassObject);
        IWbemClassObjectFreeThreaded obj = new IWbemClassObjectFreeThreaded(pIWbemClassObject);
        try {
            ObjectReadyEventArgs args = new ObjectReadyEventArgs (context, 
                                        ManagementBaseObject.GetBaseObject (obj, scope));
            watcher.FireObjectReady (args); 
        } catch {}
    }
#endif

    public void SetStatus (
#if USEIWOS
                    long flags,
#else
                    int flags, 
#endif
                    int hResult, 
                    String message, 
                    IntPtr pErrorObj)
    {
        IWbemClassObjectFreeThreaded errObj = null;
        if(pErrorObj != IntPtr.Zero)
        {
            Marshal.AddRef(pErrorObj);
            errObj = new IWbemClassObjectFreeThreaded(pErrorObj);
        }

        try {
            if (flags == (int) tag_WBEM_STATUS_TYPE.WBEM_STATUS_COMPLETE)
            {
                // Is this a Put? If so fire the ObjectPut event
                if (null != path)
                {
                    if (null == className)
                        path.RelativePath = message;
                    else
                        path.RelativePath = className;

                    // Fire the internal event (if anyone is interested)
                    if (null != InternalObjectPut)
                    {
                        try {
                            InternalObjectPutEventArgs iargs = new InternalObjectPutEventArgs (path);
                            InternalObjectPut (this, iargs);
                        } catch {}
                    }

                    ObjectPutEventArgs args = new ObjectPutEventArgs (context, path);
                    watcher.FireObjectPut(args);
                }

                // Fire Completed event
                CompletedEventArgs args2 = null ;
                if ( errObj != null )
                    {
                        args2 = new CompletedEventArgs (context, hResult, 
                                                new ManagementBaseObject (errObj)
                                                );
                    }
                else
                    {
                        args2 = new CompletedEventArgs (context, hResult, 
                                                null
                                                );
                    }
                watcher.FireCompleted (args2);
                
                // Unhook and tidy up
                watcher.RemoveSink (this);
            }
            else if (0 != (flags & (int) tag_WBEM_STATUS_TYPE.WBEM_STATUS_PROGRESS))
            {
                // Fire Progress event
                ProgressEventArgs args = new ProgressEventArgs (context, 
                    (int) (((uint)hResult & 0xFFFF0000) >> 16), hResult & 0xFFFF, message);

                watcher.FireProgress (args);
            }
        } catch {}
    }

    internal void Cancel () 
    {
        // BUGBUG : Throw exception on failure?
        try {
            scope.GetIWbemServices().CancelAsyncCall_((IWbemObjectSink) stub);
        } catch {}      
    }

    internal void ReleaseStub ()
    {
        try {
            /*
             * We force a release of the stub here so as to allow
             * unsecapp.exe to die as soon as possible.
             */
            if (null != stub)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(stub);
                stub = null;
            }
        } catch {}
    }

}

// Special sink implementation for ManagementObject.Get
// Doesn't issue ObjectReady events
internal class WmiGetEventSink : WmiEventSink
{
    private ManagementObject    managementObject;

    static ManagementOperationObserver watcherParameter;
    static object contextParameter; 
    static ManagementScope scopeParameter;
    static ManagementObject managementObjectParameter;

    static WmiGetEventSink wmiGetEventSinkNew;

    internal static WmiGetEventSink GetWmiGetEventSink(
        ManagementOperationObserver watcher,
        object context, 
        ManagementScope scope,
        ManagementObject managementObject)
    {
        if(MTAHelper.IsNoContextMTA()) // Bug#110141 - Checking for MTA is not enough.  We need to make sure we are not in a COM+ Context
            return new WmiGetEventSink(watcher, context, scope, managementObject);

        watcherParameter = watcher;
        contextParameter = context;
        scopeParameter = scope;
        managementObjectParameter = managementObject;

        //
        // [marioh, RAID: 111108]
        // Ensure we are able to trap exceptions from worker thread.
        //
        ThreadDispatch disp = new ThreadDispatch ( new ThreadDispatch.ThreadWorkerMethod ( HackToCreateWmiGetEventSink ) ) ;
        disp.Start ( ) ;

//      Thread thread = new Thread(new ThreadStart(HackToCreateWmiGetEventSink));
//        thread.Start(); // TODO: What if this throws an exception
//        thread.Join();
        return wmiGetEventSinkNew;
    }

    static void HackToCreateWmiGetEventSink()
    {
        wmiGetEventSinkNew = new WmiGetEventSink(watcherParameter, contextParameter, scopeParameter, managementObjectParameter);
    }


    private WmiGetEventSink (ManagementOperationObserver watcher,
                         object context, 
                         ManagementScope scope,
                         ManagementObject managementObject) :
        base (watcher, context, scope, null, null)
    {
        this.managementObject = managementObject;
    }

#if USEIWOS
    public override void Indicate (long lNumObjects, IWbemClassObject [] objArray)
    {
        try {
            for (long i = 0; i < lNumObjects; i++) {
                if (null != managementObject)
                    managementObject.WmiObject = objArray[i];
            }
        } catch () {}
    }
#else
    public override void Indicate (IntPtr pIWbemClassObject)
    {
        Marshal.AddRef(pIWbemClassObject);
        IWbemClassObjectFreeThreaded obj = new IWbemClassObjectFreeThreaded(pIWbemClassObject);
        if (null != managementObject)
        {
            try {
                managementObject.wbemObject = obj;
            } catch {}
        }
    }
#endif

}



}
