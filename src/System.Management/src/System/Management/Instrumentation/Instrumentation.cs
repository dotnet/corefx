namespace System.Management.Instrumentation
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Threading;
    using System.Runtime.InteropServices;
    using Microsoft.CSharp;
    using System.CodeDom.Compiler;
    using System.Management;
    using System.Security;
	using System.Security.Permissions;
    using System.Globalization;
    using System.Runtime.Versioning;
    using WbemClient_v1;

	internal delegate void ProvisionFunction(Object o);

	sealed class SecurityHelper
	{
		// BUG#112640
		// This is a global definition of an unmanaged code security permission to be used whereever we
		// want to do a full stack walk demand for the 'unmanaged code permission'
		internal static readonly SecurityPermission UnmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
	}

	/// <summary>
	///    <para> Provides helper functions for exposing events and data for management.
	///       There is a single instance of this class per application domain.</para>
	/// </summary>
	public class Instrumentation
    {
        [ResourceExposure( ResourceScope.Process),DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        static extern int GetCurrentProcessId();

        // The processIdentity field and ProcessIdentity property provide
        // a string that should be globally unique accross on this machine.
        // NOTE: The 'ProcessIdentity' must be all lower case.  During object
        // retrival, we convert the keys to lower case and compare against
        // the ProcessIdentity.
        // ALTERNATIVE computation, but doesn't work for multi appdomains
        // - String.Format("{0}_{1:x16}", (uint)GetCurrentProcessId(), DateTime.Now.ToFileTime());
        static string processIdentity = null;
        static internal string ProcessIdentity
        {
            get
            {
                // Avoid double checked locking falacy
                lock(typeof(Instrumentation))
                {
                    if(null == processIdentity)
                        processIdentity = Guid.NewGuid().ToString().ToLower(CultureInfo.InvariantCulture);
                }
                return processIdentity;
            }
        }

        #region Public Members of Instrumentation class

        /// <summary>
        ///     <para>Registers the management instance or event classes in the specified assembly with WMI.  This ensures that the instrumentation schema is accessible to System.Management client applications.</para>
        /// </summary>
        /// <param name="assemblyToRegister"><para>The assembly containing instrumentation instance or event types.</para></param>
        public static void RegisterAssembly(Assembly assemblyToRegister)
        {
            // Check for valid argument
            if(null == assemblyToRegister)
                throw new ArgumentNullException("assemblyToRegister");

            // Force the schema to be registered if necessary
            GetInstrumentedAssembly(assemblyToRegister);
        }

        /// <summary>
        ///     <para>Determines if the instrumentation schema of the specified assembly has already been correctly registered with WMI.</para>
        /// </summary>
        /// <param name="assemblyToRegister"><para>The assembly containing instrumentation instance or event types.</para></param>
        /// <returns>
        ///     <para>true if the instrumentation schema in the specified assembly is registered with WMI; otherwise, false.</para>
        /// </returns>
        public static bool IsAssemblyRegistered(Assembly assemblyToRegister)
        {
            // Check for valid argument
            if(null == assemblyToRegister)
                throw new ArgumentNullException("assemblyToRegister");

            // See if we have already loaded this assembly in the current app domain
            lock(instrumentedAssemblies)
            {
                if(instrumentedAssemblies.ContainsKey(assemblyToRegister))
                    return true;
            }

            // See if the assembly is registered with WMI.  This will not force the
            // assembly to be registered, and will not load the dynamically generated
            // code if it is registered.
            SchemaNaming naming = SchemaNaming.GetSchemaNaming(assemblyToRegister);
            if(naming == null)
                return false;

            return naming.IsAssemblyRegistered();
        }
        
        /// <summary>
        ///    <para>Raises a management event.</para>
        /// </summary>
        /// <param name='eventData'>The object that determines the class, properties, and values of the event.</param>
        public static void Fire(Object eventData)
        {
            IEvent evt = eventData as IEvent;
            if(evt != null)
                evt.Fire();
            else
                GetFireFunction(eventData.GetType())(eventData);
        }

        /// <summary>
        ///    <para>Makes an instance visible through management instrumentation.</para>
        /// </summary>
        /// <param name='instanceData'>The instance that is to be visible through management instrumentation.</param>
        public static void Publish(Object instanceData)
        {
            Type t = instanceData as Type;
            Assembly assembly = instanceData as Assembly;
            IInstance instance = instanceData as IInstance;
            if(t != null)
            {
                // We were passed a 'type' to publish.  This is our cue to try to intall
                // the schema for the assembly that t belongs to
                GetInstrumentedAssembly(t.Assembly);
            }
            else if(assembly != null)
            {
                // We were passed an 'assembly' to publish.  This is our cue to try to intall
                // the schema for the assembly that t belongs to
                GetInstrumentedAssembly(assembly);
            }
            else if(instance != null)
            {
                instance.Published = true;
            }
            else
                GetPublishFunction(instanceData.GetType())(instanceData);
        }

        /// <summary>
        /// <para>Makes an instance that was previously published through the <see cref='System.Management.Instrumentation.Instrumentation.Publish'/>
        /// method no longer visible through management instrumentation.</para>
        /// </summary>
        /// <param name='instanceData'>The object to remove from visibility for management instrumentation.</param>
        public static void Revoke(Object instanceData)
        {
            IInstance instance = instanceData as IInstance;
            if(instance != null)
                instance.Published = false;
            else
                GetRevokeFunction(instanceData.GetType())(instanceData);
        }
	
		/// <summary>
		/// Specifies the maximum number of objects of the specified type to be provided at a time.
		/// </summary>
		/// <param name="instrumentationClass">The class for which the batch size is being set.</param>
		/// <param name="batchSize">The maximum number of objects to be provided at a time.</param>
        public static void SetBatchSize(Type instrumentationClass, int batchSize)
        {
            GetInstrumentedAssembly(instrumentationClass.Assembly).SetBatchSize(instrumentationClass, batchSize);
        }

        #endregion

        #region Non-Public Members of Instrumentation class

        internal static ProvisionFunction GetFireFunction(Type type)
        {
            return new ProvisionFunction(GetInstrumentedAssembly(type.Assembly).Fire);
        }

        internal static ProvisionFunction GetPublishFunction(Type type)
        {
            return new ProvisionFunction(GetInstrumentedAssembly(type.Assembly).Publish);
        }

        internal static ProvisionFunction GetRevokeFunction(Type type)
        {
            return new ProvisionFunction(GetInstrumentedAssembly(type.Assembly).Revoke);
        }

        private static Hashtable instrumentedAssemblies = new Hashtable();

        private static void Initialize(Assembly assembly)
        {
            lock(instrumentedAssemblies)
            {
                if(instrumentedAssemblies.ContainsKey(assembly))
                    return;

                SchemaNaming naming = SchemaNaming.GetSchemaNaming(assembly);
                if(naming == null)
                    return;

                if(false == naming.IsAssemblyRegistered())
                {
                    // If we are not an administrator, don't try to JIT install the schema
                    if(!WMICapabilities.IsUserAdmin())
                        throw new Exception(RC.GetString("ASSEMBLY_NOT_REGISTERED"));

			//
			// We always use the full version number for Whidbey.
			//
			naming.DecoupledProviderInstanceName = AssemblyNameUtility.UniqueToAssemblyFullVersion(assembly);

                    naming.RegisterNonAssemblySpecificSchema(null);
                    naming.RegisterAssemblySpecificSchema();
                }

                InstrumentedAssembly instrumentedAssembly = new InstrumentedAssembly(assembly, naming);
                instrumentedAssemblies.Add(assembly, instrumentedAssembly);
            }
        }

		private static InstrumentedAssembly GetInstrumentedAssembly(Assembly assembly)
		{
            InstrumentedAssembly instrumentedAssembly;
            lock(instrumentedAssemblies)
            {
                if(false == instrumentedAssemblies.ContainsKey(assembly))
                    Initialize(assembly);
                instrumentedAssembly = (InstrumentedAssembly)instrumentedAssemblies[assembly];
            }
            return instrumentedAssembly;
		}

#if SUPPORTS_WMI_DEFAULT_VAULES
        internal static ProvisionFunction GetInitializeInstanceFunction(Type type)
		{
			return new ProvisionFunction(InitializeInstance);
		}

		private static void InitializeInstance(Object o)
		{
			Type type = o.GetType();
			string className = ManagedNameAttribute.GetClassName(type);
			SchemaNaming naming = InstrumentedAttribute.GetSchemaNaming(type.Assembly);
			ManagementClass theClass = new ManagementClass(naming.NamespaceName + ":" + className);
			foreach(FieldInfo field in type.GetFields())
			{
				Object val = theClass.Properties[ManagedNameAttribute.GetFieldName(field)].Value;
				if(null != val)
				{
					field.SetValue(o, val);
				}
			}
		}
#endif
        #endregion
    }

    delegate void ConvertToWMI(object obj);

    class InstrumentedAssembly
    {
        SchemaNaming naming;

        public EventSource source;

        private void InitEventSource(object param)
        {
        	InstrumentedAssembly threadParam = (InstrumentedAssembly) param ;
        	threadParam.source = new EventSource(threadParam.naming.NamespaceName, threadParam.naming.DecoupledProviderInstanceName, this);
        }

        public Hashtable mapTypeToConverter;

       [ResourceExposure(ResourceScope.Machine),ResourceConsumption(ResourceScope.Machine)]
        public void FindReferences(Type type, CompilerParameters parameters)
        {
            //Don't add references twice (VSQFE#2469)
            if (!parameters.ReferencedAssemblies.Contains(type.Assembly.Location))
            {
                parameters.ReferencedAssemblies.Add(type.Assembly.Location);
            }

            // Add references for the base type
            if(type.BaseType != null) // && type.BaseType.Assembly != type.Assembly) //maybe one of the ancestors is in different assembly
                FindReferences(type.BaseType, parameters);

            // Add references for implemented interfaces
            foreach(Type typeInterface in type.GetInterfaces())
            {
                if(typeInterface.Assembly != type.Assembly)
                    FindReferences(typeInterface, parameters);
            }
        }

        public bool IsInstrumentedType(Type type)
        {
            if (null != type.GetInterface("System.Management.Instrumentation.IEvent", false) ||
                null != type.GetInterface("System.Management.Instrumentation.IInstance", false))
            {
                return true;
            }
            else 
            {
                object[] attributes = type.GetCustomAttributes(typeof(System.Management.Instrumentation.InstrumentationClassAttribute), true);
                return (attributes != null && attributes.Length != 0) ? true : false;
            }
        }

       [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
       public InstrumentedAssembly(Assembly assembly, SchemaNaming naming)
        {
            SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code
            this.naming = naming;

            Assembly compiledAssembly = naming.PrecompiledAssembly;
            if(null == compiledAssembly)
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
//              ICodeCompiler compiler = provider.CreateCompiler();
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true;
                parameters.ReferencedAssemblies.Add(assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(BaseEvent).Assembly.Location);
                parameters.ReferencedAssemblies.Add(typeof(System.ComponentModel.Component).Assembly.Location);

                // Must reference any base types in 'assembly'
                // TODO: Make this more restrictive.  Only look at instrumented types.
                foreach(Type type in assembly.GetTypes())
                {
                    // Only interested in instrumented types (VSQFE#2469)
                    if (IsInstrumentedType(type))    
                        FindReferences(type, parameters);
                }

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, naming.Code);
                foreach(CompilerError err in results.Errors)
                {
                    Console.WriteLine(err.ToString());
                }
                if (results.Errors.HasErrors)
                {
                    // we failed to compile the generated code - the compile error shows up on console but we need to throw
                    throw new Exception(RC.GetString("FAILED_TO_BUILD_GENERATED_ASSEMBLY"));
                }
                compiledAssembly = results.CompiledAssembly;
            }

            Type dynType = compiledAssembly.GetType("WMINET_Converter");
            mapTypeToConverter = (Hashtable)dynType.GetField("mapTypeToConverter").GetValue(null);

            // TODO: Is STA/MTA all we have to worry about?
            if(!MTAHelper.IsNoContextMTA())  // Bug#110141 - Checking for MTA is not enough.  We need to make sure we are not in a COM+ Context
            {
                ThreadDispatch disp = new ThreadDispatch ( new ThreadDispatch.ThreadWorkerMethodWithParam ( InitEventSource ) ) ;
                disp.Parameter = this ;
                disp.Start ( ) ;
			
		 // We are on an STA thread.  Create the event source on an MTA
//                          Thread thread = new Thread(new ThreadStart(InitEventSource));
//                          thread.ApartmentState = ApartmentState.MTA;
//                          thread.Start();
//                          thread.Join();			
             }
                 else
             {
               InitEventSource( this ) ;
              }

        }

		public void Fire(Object o)
		{
			SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code
			Fire(o.GetType(), o);
		}



        public static ReaderWriterLock readerWriterLock = new ReaderWriterLock();
        public static Hashtable mapIDToPublishedObject = new Hashtable();
        static Hashtable mapPublishedObjectToID = new Hashtable();
        static int upcountId = 0x0EFF;
        public void Publish(Object o)
        {
			SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code
			try
            {
                readerWriterLock.AcquireWriterLock(-1);
                if(mapPublishedObjectToID.ContainsKey(o))
                    return;// Bug#102932 - to make the same as IInstance, we do not throw new ArgumentException();
                mapIDToPublishedObject.Add(upcountId.ToString((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int32))), o);
                mapPublishedObjectToID.Add(o, upcountId);
                upcountId++;
            }
            finally
            {
                readerWriterLock.ReleaseWriterLock();
            }
        }

        public void Revoke(Object o)
        {
			SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code
			try
            {
                readerWriterLock.AcquireWriterLock(-1);
                Object idObject = mapPublishedObjectToID[o];
                if(idObject == null)
                    return;// Bug#102932 - to make the same as IInstance, we do not throw new ArgumentException();
                int id = (int)idObject;
                mapPublishedObjectToID.Remove(o);
                mapIDToPublishedObject.Remove(id.ToString((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int32))));
            }
            finally
            {
                readerWriterLock.ReleaseWriterLock();
            }
        }

        class TypeInfo
        {
			//
			// Reuters VSQFE#: 750	[marioh]
			// We store a reference to a fieldInfo that we use when updating the wbemojects pointer array.
			// Simply for efficiency reasons.
			//
			FieldInfo fieldInfo = null ;


            // Make ThreadLocal
            int batchSize = 64;//20;
            bool batchEvents = true;

            ConvertToWMI[] convertFunctionsBatch;
            ConvertToWMI convertFunctionNoBatch;
            IntPtr[] wbemObjects;
            Type converterType;


            int currentIndex = 0;

			//
			// [marioh, RAID: 123543]
			// Removed this member since no longer needed.
			// 
			//public object o;

            public EventSource source;

			//
			// [marioh, RAID: 123543]
			// Changed signature to account for sync problems.
			//
			public void Fire(object o)
			{
                if(source.Any())
                    return;

                if(!batchEvents)
                {
					// BUG#119210 - we need to lock this because we only have one
					// copy of wbemObjects[0] which is filled in by convertFunctionNoBatch
					lock(this)
					{
						convertFunctionNoBatch(o);
						//
						// Reuters VSQFE#: 750	[marioh]
						// At this point, the ToWMI method has been called and a new instance created.
						// We have to make sure the wbemObjects array (which is the array of IWbemClassObjects to be indicated)
						// is updated to point to the newly created instances.
						//
						wbemObjects[0] = (IntPtr) fieldInfo.GetValue ( convertFunctionNoBatch.Target ) ;
						source.IndicateEvents(1, wbemObjects);
					}
                }
                else
                {
                    lock(this)
                    {
                        convertFunctionsBatch[currentIndex++](o);
						//
						// Reuters VSQFE#: 750	[marioh]
						// At this point, the ToWMI method has been called and a new instance created.
						// We have to make sure the wbemObjects array (which is the array of IWbemClassObjects to be indicated)
						// is updated to point to the newly created instances.
						//
						wbemObjects[currentIndex-1] = (IntPtr) fieldInfo.GetValue ( convertFunctionsBatch[currentIndex-1].Target ) ;

                        if(cleanupThread == null)
                        {
                            int tickCount = Environment.TickCount;
                            if(tickCount-lastFire<1000)
                            {
                                lastFire = Environment.TickCount ;
                                cleanupThread = new Thread(new ThreadStart(Cleanup));
                                cleanupThread.SetApartmentState(ApartmentState.MTA);
                                cleanupThread.Start();
                            }
                            else
                            {
                                source.IndicateEvents(currentIndex, wbemObjects);
                                currentIndex = 0;
                                lastFire = tickCount;
                            }
                        }
                        else if(currentIndex==batchSize)
                        {
                            source.IndicateEvents(currentIndex, wbemObjects);
                            currentIndex = 0;
                            lastFire = Environment.TickCount;
                        }
                    }
                }
            }

            public int lastFire = 0;

            public void SetBatchSize(int batchSize)
            {
				//
				// [RAID: 125526, marioh]
				// Check parameter validity before continuing.
				// Assumption: batchSize >= 1
				// Throws ArgumentOutOfRangeException if batchSize <= 0
				//
				if ( batchSize <= 0 )
				{
					throw new ArgumentOutOfRangeException ("batchSize") ;
				}
                if(!WMICapabilities.MultiIndicateSupported)
                    batchSize = 1;
                lock(this)
                {
                    if(currentIndex > 0)
                    {
                        source.IndicateEvents(currentIndex, wbemObjects);
                        currentIndex = 0;
                        lastFire = Environment.TickCount;
                    }
                    wbemObjects = new IntPtr[batchSize];
                    if(batchSize > 1)
                    {
                        batchEvents = true;
                        this.batchSize = batchSize;

                        convertFunctionsBatch = new ConvertToWMI[batchSize];
                        for(int i=0;i<batchSize;i++)
                        {
                            Object converter = Activator.CreateInstance(converterType);
                            convertFunctionsBatch[i] = (ConvertToWMI)Delegate.CreateDelegate(typeof(ConvertToWMI), converter, "ToWMI");
                            wbemObjects[i] = ExtractIntPtr(converter);
                        }
						// Reuters VSQFE#: 750	[marioh] 
						// Initialize the FieldInfo used when refreshing instance pointers
						fieldInfo = convertFunctionsBatch[0].Target.GetType().GetField ("instWbemObjectAccessIP");
                    }
                    else
                    {
						// Reuters VSQFE#: 750	[marioh] 
						// Initialize the FieldInfo used when refreshing instance pointers
						fieldInfo = convertFunctionNoBatch.Target.GetType().GetField ("instWbemObjectAccessIP");
                        wbemObjects[0] = ExtractIntPtr(convertFunctionNoBatch.Target);
                        batchEvents = false;
                    }
                }
            }

            public IntPtr ExtractIntPtr(object o)
            {
                // return (IntPtr)o;
                return (IntPtr)o.GetType().GetField("instWbemObjectAccessIP").GetValue(o);
            }

            public void Cleanup()
            {
                int idleCount = 0;
                while(idleCount<20)
                {
                    Thread.Sleep(100);
                    if(0==currentIndex)
                    {
                        idleCount++;
                        continue;
                    }
                    idleCount = 0;
                    if((Environment.TickCount - lastFire)<100)
                        continue;
                    lock(this)
                    {
                        if(currentIndex>0)
                        {
                            source.IndicateEvents(currentIndex, wbemObjects);
                            currentIndex = 0;
                            lastFire = Environment.TickCount;
                        }
                    }
                }
                cleanupThread = null;
            }
            public Thread cleanupThread = null;

            public TypeInfo(EventSource source, SchemaNaming naming, Type converterType)
            {
                this.converterType = converterType;
                this.source = source;

                Object converter = Activator.CreateInstance(converterType);
                convertFunctionNoBatch = (ConvertToWMI)Delegate.CreateDelegate(typeof(ConvertToWMI), converter, "ToWMI");

                // NOTE: wbemObjects[0] will get initialized in SetBatchSize if batchSize == 1

                SetBatchSize(batchSize);
            }
        }

        Hashtable mapTypeToTypeInfo = new Hashtable();

        public void SetBatchSize(Type t, int batchSize)
        {
            GetTypeInfo(t).SetBatchSize(batchSize);
        }

        TypeInfo lastTypeInfo = null;
        Type lastType = null;
        TypeInfo GetTypeInfo(Type t)
        {
            lock(mapTypeToTypeInfo)
            {
                if(lastType==t)
                    return lastTypeInfo;

                lastType = t;
                TypeInfo typeInfo = (TypeInfo)mapTypeToTypeInfo[t];
                if(null==typeInfo)
                {
                    typeInfo = new TypeInfo(source, naming, (Type)mapTypeToConverter[t]);
                    mapTypeToTypeInfo.Add(t, typeInfo);
                }
                lastTypeInfo = typeInfo;
                return typeInfo;
            }
        }

        public void Fire(Type t, Object o)
        {
            TypeInfo typeInfo = GetTypeInfo(t);

			//
			// [marioh, RAID: 123543]
			// Avoid race condition whereby the class member o is changed by multiple threads and can cause 
			// event corruption.
			//
			//typeInfo.o = o;
			typeInfo.Fire(o);		
		}
	}


    /// <summary>
    ///    <para>Specifies a source of a management instrumentation event. 
    ///       Objects that implement this interface are known to be sources of management
    ///       instrumentation events. Classes that do not derive from <see cref='System.Management.Instrumentation.BaseEvent'/> should implement
    ///       this interface instead.</para>
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///    <para> 
        ///       Raises a management event.</para>
        /// </summary>
        void Fire();
    }

    /// <summary>
    ///    <para>Represents classes derived 
    ///       from <see cref='System.Management.Instrumentation.BaseEvent'/> that are known to be
    ///    management event classes. These derived classes inherit an implementation
    ///    of <see cref='System.Management.Instrumentation.IEvent'/> that allows events to be
    ///    fired through the <see cref='System.Management.Instrumentation.IEvent.Fire'/>
    ///    method.</para>
    /// </summary>
    /// <example>
    ///    <code lang='C#'>using System;
    /// using System.Management;
    /// using System.Configuration.Install;
    /// using System.Management.Instrumentation;
    /// 
    /// // This example demonstrates how to create a Management Event class by deriving
    /// // from BaseEvent class and to fire a Management Event from managed code.
    /// 
    /// // Specify which namespace the Manaegment Event class is created in
    /// [assembly:Instrumented("Root/Default")]
    /// 
    /// // Let the system know you will run InstallUtil.exe utility against
    /// // this assembly
    /// [System.ComponentModel.RunInstaller(true)]
    /// public class MyInstaller : DefaultManagementProjectInstaller {}
    /// 
    /// // Create a Management Instrumentation Event class
    /// public class MyEvent : BaseEvent
    /// {
    ///     public string EventName;
    /// }
    /// 
    /// public class Sample_EventProvider
    /// {
    ///     public static int Main(string[] args) {
    ///        MyEvent e = new MyEvent();
    ///        e.EventName = "Hello";
    ///        
    ///        // Fire the Management Event
    ///        e.Fire();
    ///        
    ///        return 0;
    ///     }
    /// }
    ///    </code>
    ///    <code lang='VB'>Imports System
    /// Imports System.Management
    /// Imports System.Configuration.Install
    /// Imports System.Management.Instrumentation
    /// 
    /// ' This sample demonstrates how to create a Management Event class by deriving
    /// ' from BaseEvent class and to fire a Management Event from managed code.
    /// 
    /// ' Specify which namespace the Manaegment Event class is created in
    /// &lt;assembly: Instrumented("Root/Default")&gt;
    /// 
    /// ' Let the system know InstallUtil.exe utility will be run against
    /// ' this assembly
    /// &lt;System.ComponentModel.RunInstaller(True)&gt; _
    /// Public Class MyInstaller
    ///     Inherits DefaultManagementProjectInstaller
    /// End Class
    /// 
    /// ' Create a Management Instrumentation Event class
    /// &lt;InstrumentationClass(InstrumentationType.Event)&gt; _ 
    /// Public Class MyEvent
    ///     Inherits BaseEvent
    ///     Public EventName As String
    /// End Class
    /// 
    /// Public Class Sample_EventProvider
    ///     Public Shared Function Main(args() As String) As Integer
    ///         Dim e As New MyEvent()
    ///         e.EventName = "Hello"
    /// 
    ///         ' Fire the Management Event
    ///         e.Fire()
    /// 
    ///         Return 0
    ///     End Function
    /// End Class
    ///    </code>
    /// </example>
    [InstrumentationClass(InstrumentationType.Event)]
    public abstract class BaseEvent : IEvent
	{
		private ProvisionFunction fireFunction = null;
		private ProvisionFunction FireFunction
		{
			get
			{
				if(null == fireFunction)
				{
					fireFunction = Instrumentation.GetFireFunction(this.GetType());
				}
				return fireFunction;
			}
		}

        /// <summary>
        ///    <para>Raises a management event.</para>
        /// </summary>
        public void Fire()
		{
			FireFunction(this);
		}
	}

    /// <summary>
    ///    <para>Specifies a source of a management instrumentation 
    ///       instance. Objects that implement this interface are known to be sources of
    ///       management instrumentation instances. Classes that do not derive from <see cref='System.Management.Instrumentation.Instance'/> should implement
    ///       this interface instead.</para>
    /// </summary>
    public interface IInstance
    {
        /// <summary>
        ///    Gets or sets a value indicating whether instances of
        ///    classes that implement this interface are visible through management
        ///    instrumentation.
        /// </summary>
        /// <value>
        /// <para><see langword='true'/>, if the 
        ///    instance is visible through management instrumentation; otherwise,
        /// <see langword='false'/>.</para>
        /// </value>
        bool Published
        {
            get;
            set;
        }
    }

    /// <summary>
    ///    <para> Represents derived classes known to be management 
    ///       instrumentation instance classes. These derived classes inherit an
    ///       implementation of <see cref='System.Management.Instrumentation.IInstance'/> that allows instances to be
    ///       published through the <see cref='System.Management.Instrumentation.IInstance.Published'/>
    ///       property.</para>
    /// </summary>
    /// <example>
    ///    <code lang='C#'>using System;
    /// using System.Management;
    /// using System.Configuration.Install;
    /// using System.Management.Instrumentation;
    /// 
    /// // This sample demonstrates how to create a Management Instrumentation Instance
    /// // class and how to publish an instance of this class to WMI.
    /// 
    /// // Specify which namespace the Instance class is created in
    /// [assembly:Instrumented("Root/Default")]
    /// 
    /// // Let the system know InstallUtil.exe utility will be run against
    /// // this assembly
    /// [System.ComponentModel.RunInstaller(true)]
    /// public class MyInstaller : DefaultManagementProjectInstaller {}
    /// 
    /// // Create a Management Instrumentation Instance class
    /// [InstrumentationClass(InstrumentationType.Instance)]
    /// public class InstanceClass : Instance
    /// {
    ///     public string SampleName;
    ///     public int SampleNumber;
    /// }
    /// 
    /// public class Sample_InstanceProvider
    /// {
    ///     public static int Main(string[] args) {
    ///         InstanceClass instClass = new InstanceClass();
    ///         instClass.SampleName = "Hello";
    ///         instClass.SampleNumber = 888;
    ///    
    ///         // Publish this instance to WMI
    ///         instClass.Published = true;
    ///    
    ///         return 0;
    ///     }
    /// }
    ///    </code>
    ///    <code lang='VB'>Imports System
    /// Imports System.Management
    /// Imports System.Configuration.Install
    /// Imports System.Management.Instrumentation
    /// 
    /// ' This sample demonstrate how to create a Management Instrumentation Instance
    /// ' class and how to publish an instance of this class to WMI.
    /// ' Specify which namespace the Instance class is created in
    /// &lt;assembly: Instrumented("Root/Default")&gt;
    /// 
    /// ' Let the system know InstallUtil.exe utility will be run against
    /// ' this assembly
    /// &lt;System.ComponentModel.RunInstaller(True)&gt;  _
    /// Public Class MyInstaller
    ///     Inherits DefaultManagementProjectInstaller
    /// End Class
    /// 
    /// ' Create a Management Instrumentation Instance class
    /// &lt;InstrumentationClass(InstrumentationType.Instance)&gt;  _
    /// Public Class InstanceClass
    ///     Inherits Instance
    ///     Public SampleName As String
    ///     Public SampleNumber As Integer
    /// End Class
    /// 
    /// Public Class Sample_InstanceProvider
    ///     Overloads Public Shared Function Main(args() As String) As Integer
    ///         Dim instClass As New InstanceClass()
    ///         instClass.SampleName = "Hello"
    ///         instClass.SampleNumber = 888
    /// 
    ///         ' Publish this instance to WMI
    ///         instClass.Published = True
    /// 
    ///         Return 0
    ///     End Function
    /// End Class
    ///    </code>
    /// </example>
    [InstrumentationClass(InstrumentationType.Instance)]
    public abstract class Instance : IInstance
    {
        private ProvisionFunction publishFunction = null;
        private ProvisionFunction revokeFunction = null;
        private ProvisionFunction PublishFunction
        {
            get
            {
                if(null == publishFunction)
                {
                    publishFunction = Instrumentation.GetPublishFunction(this.GetType());
                }
                return publishFunction;
            }
        }
        private ProvisionFunction RevokeFunction
        {
            get
            {
                if(null == revokeFunction)
                {
                    revokeFunction = Instrumentation.GetRevokeFunction(this.GetType());
                }
                return revokeFunction;
            }
        }
        private bool published = false;

		/// <summary>
		///    <para>Gets or sets a value indicating whether instances of classes that implement this interface are visible through management instrumentation.</para>
		/// </summary>
		/// <value>
		/// <para><see langword='true'/>, if the 
		///    instance is visible through management instrumentation; otherwise,
		/// <see langword='false'/>.</para>
		/// </value>
        [IgnoreMember]
		public bool Published
        {
            get
            {
                return published;
            }
            set
            {
                if(published && false==value)
                {
                    // We ARE published, and the caller is setting published to FALSE
                    RevokeFunction(this);
                    published = false;
                }
                else if(!published && true==value)
                {
                    // We ARE NOT published, and the caller is setting published to TRUE
                    PublishFunction(this);
                    published = true;
                }
            }
        }
    }
}

#if JEFF_WARNING_REMOVAL_TEST 
namespace System.Management
{
    class DoNothing
    {
        static void SayNothing()
        {
            tag_SWbemRpnConst w;
            w.unionhack = 0;

            tag_CompileStatusInfo x;
            x.lPhaseError = 0;
            x.hRes = 0;
            x.ObjectNum = 0;
            x.FirstLine = 0;
            x.LastLine = 0;
            x.dwOutFlags = 0;

            tag_SWbemQueryQualifiedName y;
            y.m_uVersion = 0;
            y.m_uTokenType = 0;
            y.m_uNameListSize = 0;
            y.m_ppszNameList = IntPtr.Zero;
            y.m_bArraysUsed = 0;
            y.m_pbArrayElUsed = IntPtr.Zero;
            y.m_puArrayIndex = IntPtr.Zero;

            tag_SWbemRpnQueryToken z;
            z.m_uVersion = 0;
            z.m_uTokenType = 0;
            z.m_uSubexpressionShape = 0;
            z.m_uOperator = 0;
            z.m_pRightIdent = IntPtr.Zero;
            z.m_pLeftIdent = IntPtr.Zero;
            z.m_uConstApparentType = 0;
            z.m_Const = w;
            z.m_uConst2ApparentType = 0;
            z.m_Const2 = w;
            z.m_pszRightFunc = "";
            z.m_pszLeftFunc = "";

            tag_SWbemRpnTokenList a;
            a.m_uVersion = 0;
            a.m_uTokenType = 0;
            a.m_uNumTokens = 0;

            tag_SWbemRpnEncodedQuery b;
            b.m_uVersion = 0;
            b.m_uTokenType = 0;
            b.m_uParsedFeatureMask1 = 0;
            b.m_uParsedFeatureMask2 = 0;
            b.m_uDetectedArraySize = 0;
            b.m_puDetectedFeatures = IntPtr.Zero;
            b.m_uSelectListSize = 0;
            b.m_ppSelectList = IntPtr.Zero;
            b.m_uFromTargetType = 0;
            b.m_pszOptionalFromPath = "";
            b.m_uFromListSize = 0;
            b.m_ppszFromList = IntPtr.Zero;
            b.m_uWhereClauseSize = 0;
            b.m_ppRpnWhereClause = IntPtr.Zero;
            b.m_dblWithinPolling = 0;
            b.m_dblWithinWindow = 0;
            b.m_uOrderByListSize = 0;
            b.m_ppszOrderByList = IntPtr.Zero;
            b.m_uOrderDirectionEl = IntPtr.Zero;

            tag_SWbemAnalysisMatrix c;
            c.m_uVersion = 0;
            c.m_uMatrixType = 0;
            c.m_pszProperty = "";
            c.m_uPropertyType = 0;
            c.m_uEntries = 0;
            c.m_pValues = IntPtr.Zero;
            c.m_pbTruthTable = IntPtr.Zero;

            tag_SWbemAnalysisMatrixList d;
            d.m_uVersion = 0;
            d.m_uMatrixType = 0;
            d.m_uNumMatrices = 0;
            d.m_pMatrices = IntPtr.Zero;

            tag_SWbemAssocQueryInf e;
            e.m_uVersion = 0;
            e.m_uAnalysisType = 0;
            e.m_uFeatureMask = 0;
            e.m_pPath = null;
            e.m_pszPath = "";
            e.m_pszQueryText = "";
            e.m_pszResultClass = "";
            e.m_pszAssocClass = "";
            e.m_pszRole = "";
            e.m_pszResultRole = "";
            e.m_pszRequiredQualifier = "";
            e.m_pszRequiredAssocQualifier = "";
        }
    }
}
#endif

#if xxxx

    /// <summary>
    ///    <para>[To be supplied.]</para>
    /// </summary>
    public static void SetField(Object inst, ISWbemProperty prop, FieldInfo field)
    {
        Object o = prop.get_Value();
        IConvertible i = (IConvertible)o;

        Type t2 = field.FieldType;

        if(t2 == typeof(SByte))
            o = i.ToSByte(null);
        else if(t2 == typeof(Byte))
            o = i.ToByte(null);
        else if(t2 == typeof(Int16))
            o = i.ToInt16(null);
        else if(t2 == typeof(UInt16))
            o = i.ToUInt16(null);
        else if(t2 == typeof(Int32))
            o = i.ToInt32(null);
        else if(t2 == typeof(UInt32))
            o = i.ToUInt32(null);
        else if(t2 == typeof(Int64))
            o = i.ToInt64(null);
        else if(t2 == typeof(UInt64))
            o = i.ToUInt64(null);
        else if(t2 == typeof(Single))
            o = i.ToSingle(null);
        else if(t2 == typeof(Double))
            o = i.ToDouble(null);
        else if(t2 == typeof(Boolean))
            o = i.ToBoolean(null);
        else if(t2 == typeof(String))
            o = i.ToString(null);
        else if(t2 == typeof(Char))
            o = i.ToChar(null);
        else if(t2 == typeof(DateTime))
//            o = i.ToDateTime(null);

        {/*Console.WriteLine(" NO CONVERSION TO DATETIME: "+o+" - "+o.GetType().Name);*/return;}
        else if(t2 == typeof(TimeSpan))
//            o = //i.To;
        {/*Console.WriteLine(" NO CONVERSION TO TIMESPAN: "+o+" - "+o.GetType().Name);*/return;}
        else if(t2 == typeof(Object))
            /*Nothing to do*/o = o;
        else
            throw new Exception("Unsupported type for default property - " + t2.Name);

        field.SetValue(inst, o);
    }

    /// <summary>
    ///    <para>[To be supplied.]</para>
    /// </summary>
    public static void SetProp(Object o, ISWbemProperty prop)
    {
        try
        {
            // TODO: FIX UP THIS MESS!!!!
            if(o == null)
                /*NOTHING TO DO*/o = o;
            else if(o.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)o;
                TimeSpan ts = dt.Subtract(dt.ToUniversalTime());
                int diffUTC = (ts.Minutes + ts.Hours * 60);
                if(diffUTC >= 0)
                    o = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.{6:D3}000+{7:D3}", new Object [] {dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, diffUTC});
                else
                    o = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.{6:D3}000-{7:D3}", new Object [] {dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, -diffUTC});
            }
            else if(o.GetType() == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)o;
                o = String.Format("{0:D8}{1:D2}{2:D2}{3:D2}.{4:D3}000:000", new Object [] {ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds});
            }
            else if(o.GetType() == typeof(char))
            {
                if(0 == (char)o)
                    o = (int)0;
                else
                  o=o.ToString();
            }

            prop.set_Value(ref o);
        }
        catch
        {
//            Console.WriteLine(prop.Name + " - "+o.GetType().Name + " - " + (o == null));
            o = o.ToString();
            prop.set_Value(ref o);
        }
    }

#endif
