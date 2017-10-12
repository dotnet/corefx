//------------------------------------------------------------------------------
// <copyright from='1997' to='2001' company='Microsoft Corporation'>           
//    Copyright (c) Microsoft Corporation. All Rights Reserved.                
//    Information Contained Herein is Proprietary and Confidential.            
// </copyright>                                                                
//------------------------------------------------------------------------------

// TODO: Better logging to context of InstallUtil
namespace System.Management.Instrumentation
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration.Install;
    using System.Management;
    using Microsoft.Win32;
    using System.IO;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Runtime.Versioning;
	
    class SchemaNaming
    {

        class InstallLogWrapper
        {
            InstallContext context = null;
            public InstallLogWrapper(InstallContext context)
            {
                this.context = context;
            }
            public void LogMessage(string str)
            {
                if(null != context)
                    context.LogMessage(str);
            }
        }

        public static SchemaNaming GetSchemaNaming(Assembly assembly)
        {
            InstrumentedAttribute attr = InstrumentedAttribute.GetAttribute(assembly);

            // See if this assembly provides instrumentation
            if(null == attr)
                return null;

            return new SchemaNaming(attr.NamespaceName, attr.SecurityDescriptor, assembly);
        }

        Assembly assembly;
        SchemaNaming(string namespaceName, string securityDescriptor, Assembly assembly)
        {
            this.assembly = assembly;
            assemblyInfo = new AssemblySpecificNaming(namespaceName, securityDescriptor, assembly);

			if ( DoesInstanceExist(RegistrationPath) == false )
			{
				assemblyInfo.DecoupledProviderInstanceName = AssemblyNameUtility.UniqueToAssemblyMinorVersion(assembly);
			}
        }

        ///////////////////////////////////////////
        // string constants
        const string Win32ProviderClassName = "__Win32Provider";
        const string EventProviderRegistrationClassName = "__EventProviderRegistration";
        const string InstanceProviderRegistrationClassName = "__InstanceProviderRegistration";
        const string DecoupledProviderClassName = "MSFT_DecoupledProvider";
        const string ProviderClassName = "WMINET_ManagedAssemblyProvider";
        const string InstrumentationClassName = "WMINET_Instrumentation";
        const string InstrumentedAssembliesClassName = "WMINET_InstrumentedAssembly";
        const string DecoupledProviderCLSID = "{54D8502C-527D-43f7-A506-A9DA075E229C}";
        const string GlobalWmiNetNamespace = @"root\MicrosoftWmiNet";
        const string InstrumentedNamespacesClassName = "WMINET_InstrumentedNamespaces";
        const string NamingClassName = "WMINET_Naming";


        ///////////////////////////////////////////
        // class that holds read only naming info
        // specific to an assembly
        private class AssemblySpecificNaming
        {
           [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
           public AssemblySpecificNaming(string namespaceName, string securityDescriptor, Assembly assembly)
            {
                this.namespaceName = namespaceName;
                this.securityDescriptor = securityDescriptor;
                this.decoupledProviderInstanceName = AssemblyNameUtility.UniqueToAssemblyFullVersion(assembly);
                this.assemblyUniqueIdentifier = AssemblyNameUtility.UniqueToAssemblyBuild(assembly);
                this.assemblyName = assembly.FullName;
                this.assemblyPath = assembly.Location;
            }

            string namespaceName;
            string securityDescriptor;
            string decoupledProviderInstanceName;
            string assemblyUniqueIdentifier;
            string assemblyName;
            string assemblyPath;

            public string NamespaceName {get {return namespaceName;} }
            public string SecurityDescriptor {get {return securityDescriptor;} }
            public string DecoupledProviderInstanceName {get {return decoupledProviderInstanceName;}  set { decoupledProviderInstanceName=value;}}
            public string AssemblyUniqueIdentifier {get {return assemblyUniqueIdentifier;} }
            public string AssemblyName {get {return assemblyName;} }
            public string AssemblyPath {get {return assemblyPath;} }
        }

        ///////////////////////////////////////////
        // Accessors for name information
        // After these methods, there should be no
        // use of the lower case names
        AssemblySpecificNaming assemblyInfo;

        public string NamespaceName {get {return assemblyInfo.NamespaceName;} }
        public string SecurityDescriptor {get {return assemblyInfo.SecurityDescriptor;} }
		public string DecoupledProviderInstanceName {get {return assemblyInfo.DecoupledProviderInstanceName;} set {assemblyInfo.DecoupledProviderInstanceName=value;}}
        string AssemblyUniqueIdentifier {get {return assemblyInfo.AssemblyUniqueIdentifier;} }
        string AssemblyName {get {return assemblyInfo.AssemblyName;} }
        string AssemblyPath {get {return assemblyInfo.AssemblyPath;} }

        string Win32ProviderClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, Win32ProviderClassName);} }
        string DecoupledProviderClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, DecoupledProviderClassName);} }
        string InstrumentationClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, InstrumentationClassName);} }
        string EventProviderRegistrationClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, EventProviderRegistrationClassName);} }
        string EventProviderRegistrationPath {get {return AppendProperty(EventProviderRegistrationClassPath, "provider", @"\\\\.\\"+ProviderPath.Replace(@"\", @"\\").Replace(@"""", @"\"""));} }
        string InstanceProviderRegistrationClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, InstanceProviderRegistrationClassName);} }
        string InstanceProviderRegistrationPath {get {return AppendProperty(InstanceProviderRegistrationClassPath, "provider", @"\\\\.\\"+ProviderPath.Replace(@"\", @"\\").Replace(@"""", @"\"""));} }

        string ProviderClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, ProviderClassName);} }
        string ProviderPath {get {return AppendProperty(ProviderClassPath, "Name", assemblyInfo.DecoupledProviderInstanceName);} }
        string RegistrationClassPath {get {return MakeClassPath(assemblyInfo.NamespaceName, InstrumentedAssembliesClassName);} }
        string RegistrationPath {get {return AppendProperty(RegistrationClassPath, "Name", assemblyInfo.DecoupledProviderInstanceName);} }
        string GlobalRegistrationNamespace {get {return GlobalWmiNetNamespace;} }
        string GlobalInstrumentationClassPath {get {return MakeClassPath(GlobalWmiNetNamespace, InstrumentationClassName);} }
        string GlobalRegistrationClassPath {get {return MakeClassPath(GlobalWmiNetNamespace, InstrumentedNamespacesClassName);} }
        string GlobalRegistrationPath {get {return AppendProperty(GlobalRegistrationClassPath, "NamespaceName", assemblyInfo.NamespaceName.Replace(@"\", @"\\"));} }
        string GlobalNamingClassPath {get {return MakeClassPath(GlobalWmiNetNamespace, NamingClassName);} }

        string DataDirectory {get {return Path.Combine(WMICapabilities.FrameworkDirectory, NamespaceName);} }
        string MofPath {get {return Path.Combine(DataDirectory, DecoupledProviderInstanceName + ".mof");} }
        string CodePath {get {return Path.Combine(DataDirectory, DecoupledProviderInstanceName + ".cs");} }
        string PrecompiledAssemblyPath {get {return Path.Combine(DataDirectory, DecoupledProviderInstanceName + ".dll");} }

        static string MakeClassPath(string namespaceName, string className)
        {
            return namespaceName + ":" + className;
        }

        static string AppendProperty(string classPath, string propertyName, string propertyValue)
        {
            return classPath+'.'+propertyName+"=\""+propertyValue+'\"';
        }

        public bool IsAssemblyRegistered()
        {
            if(DoesInstanceExist(RegistrationPath) )
            {
                ManagementObject inst = new ManagementObject(RegistrationPath);
//                return (0==AssemblyUniqueIdentifier.ToLower(CultureInfo.InvariantCulture).CompareTo(inst["RegisteredBuild"].ToString().ToLower()));
                return (0==String.Compare(AssemblyUniqueIdentifier,inst["RegisteredBuild"].ToString(),StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

		// Schema has to be compared only if both the below conditions are met
		// 1. Intance of WMI_InstrumentedAssembly is already present for the given name
		// 2. If the registeredBuild - which includes unique value for the provider assembly is different
		private bool IsSchemaToBeCompared()
		{
			bool bRet = false;

			if (DoesInstanceExist(RegistrationPath))
			{
				ManagementObject obj = new ManagementObject(RegistrationPath);

				//                return (0==AssemblyUniqueIdentifier.ToLower(CultureInfo.InvariantCulture).CompareTo(inst["RegisteredBuild"].ToString().ToLower()));
				bRet = (0 != String.Compare(AssemblyUniqueIdentifier, obj["RegisteredBuild"].ToString(), StringComparison.OrdinalIgnoreCase));
			}

			return bRet;
		}
        ManagementObject registrationInstance = null;
        ManagementObject RegistrationInstance
        {
            get
            {
                if(null == registrationInstance)
                    registrationInstance = new ManagementObject(RegistrationPath);
                return registrationInstance;
            }
        }

        public string Code
        {
            [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
            get
            {
                using(StreamReader reader = new StreamReader(CodePath))
                {
                	return reader.ReadToEnd();
                }
            	}
        }

		public string Mof
		{
                    [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
			get
			{
				using(StreamReader reader = new StreamReader(MofPath))
				{
					return reader.ReadToEnd();
				}
			}
		}

        public Assembly PrecompiledAssembly
        {
            [ResourceExposure(ResourceScope.Machine),ResourceConsumption(ResourceScope.Machine)]
            get
            {
                if(File.Exists(PrecompiledAssemblyPath))
                    return Assembly.LoadFrom(PrecompiledAssemblyPath);
                return null;
            }
        }

		// function to check if the class to be added to MOF is already present in repository
		// [ramrao] VSUQFE#2248 (VSWhidbey 231885)
		bool IsClassAlreadyPresentInRepository(ManagementObject obj)
		{
			bool bRet = false;
			string ClassPathInRepository = MakeClassPath(NamespaceName, (string)obj.SystemProperties["__CLASS"].Value);

			if (DoesClassExist(ClassPathInRepository))
			{
				ManagementObject inst = new ManagementClass(ClassPathInRepository);
				bRet = inst.CompareTo(obj, ComparisonSettings.IgnoreCase | ComparisonSettings.IgnoreObjectSource);
			}

			return bRet;
		}
        string GenerateMof(string [] mofs)
        {
            return String.Concat(
                "//**************************************************************************\r\n",
                String.Format("//* {0}\r\n", DecoupledProviderInstanceName),
                String.Format("//* {0}\r\n", AssemblyUniqueIdentifier),
                "//**************************************************************************\r\n",
                "#pragma autorecover\r\n",
                EnsureNamespaceInMof(GlobalRegistrationNamespace),
                EnsureNamespaceInMof(NamespaceName),

                PragmaNamespace(GlobalRegistrationNamespace),
                GetMofFormat(new ManagementClass(GlobalInstrumentationClassPath)),
                GetMofFormat(new ManagementClass(GlobalRegistrationClassPath)),
                GetMofFormat(new ManagementClass(GlobalNamingClassPath)),
                GetMofFormat(new ManagementObject(GlobalRegistrationPath)),

                PragmaNamespace(NamespaceName),
                GetMofFormat(new ManagementClass(InstrumentationClassPath)),
                GetMofFormat(new ManagementClass(RegistrationClassPath)),
                GetMofFormat(new ManagementClass(DecoupledProviderClassPath)),
                GetMofFormat(new ManagementClass(ProviderClassPath)),

                GetMofFormat(new ManagementObject(ProviderPath)),
//                events.Count>0?GetMofFormat(new ManagementObject(EventProviderRegistrationPath)):"",
                GetMofFormat(new ManagementObject(EventProviderRegistrationPath)),
                GetMofFormat(new ManagementObject(InstanceProviderRegistrationPath)),

                String.Concat(mofs),

                GetMofFormat(new ManagementObject(RegistrationPath)) );
        }

        public void RegisterNonAssemblySpecificSchema(InstallContext installContext)
        {
            SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code
            
            // Make sure the 'Client' key has the correct permissions
            WmiNetUtilsHelper.VerifyClientKey_f();

            InstallLogWrapper context = new InstallLogWrapper(installContext);

            EnsureNamespace(context, GlobalRegistrationNamespace);

            EnsureClassExists(context, GlobalInstrumentationClassPath, new ClassMaker(MakeGlobalInstrumentationClass));

            EnsureClassExists(context, GlobalRegistrationClassPath, new ClassMaker(MakeNamespaceRegistrationClass));

            EnsureClassExists(context, GlobalNamingClassPath, new ClassMaker(MakeNamingClass));

            EnsureNamespace(context, NamespaceName);

            EnsureClassExists(context, InstrumentationClassPath, new ClassMaker(MakeInstrumentationClass));

            EnsureClassExists(context, RegistrationClassPath, new ClassMaker(MakeRegistrationClass));

            // Make sure Hosting model is set correctly by default.  If not, we blow away the class definition
            try
            {
                ManagementClass cls = new ManagementClass(DecoupledProviderClassPath);
                if(cls["HostingModel"].ToString() != "Decoupled:Com")
                {
                    cls.Delete();
                }
            }
            catch(ManagementException e)
            {
                if(e.ErrorCode != ManagementStatus.NotFound)
                    throw e;
            }

            EnsureClassExists(context, DecoupledProviderClassPath, new ClassMaker(MakeDecoupledProviderClass));

            EnsureClassExists(context, ProviderClassPath, new ClassMaker(MakeProviderClass));

            if(!DoesInstanceExist(GlobalRegistrationPath))
                RegisterNamespaceAsInstrumented();
        }

              [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
		public void RegisterAssemblySpecificSchema()
		{
			SecurityHelper.UnmanagedCode.Demand(); // Bug#112640 - Close off any potential use from anything but fully trusted code

			Type[] types = InstrumentedAttribute.GetInstrumentedTypes(assembly);
			StringCollection events = new StringCollection();
			StringCollection instances = new StringCollection();
			StringCollection abstracts = new StringCollection();
			string[] mofs = new string[types.Length];
			CodeWriter code = new CodeWriter();
			ReferencesCollection references = new ReferencesCollection();

			// Add the node with all the 'using' statements at the top
			code.AddChild(references.UsingCode);
			references.Add(typeof(Object));
			references.Add(typeof(ManagementClass));
			references.Add(typeof(Marshal));
			references.Add(typeof(System.Security.SuppressUnmanagedCodeSecurityAttribute));
			references.Add(typeof(System.Reflection.FieldInfo));
			references.Add(typeof(Hashtable));

			// Add a blank line
			code.Line();

			// Add master converter class
			CodeWriter codeWMIConverter = code.AddChild("public class WMINET_Converter");
            
			// Add master map of types to converters
			codeWMIConverter.Line("public static Hashtable mapTypeToConverter = new Hashtable();");
          
			// Add master CCTOR
			CodeWriter codeCCTOR = codeWMIConverter.AddChild("static WMINET_Converter()");

			// Make mapping of types to converter class names
			Hashtable mapTypeToConverterClassName = new Hashtable();

			for(int i=0;i<types.Length;i++)
				mapTypeToConverterClassName[types[i]] = "ConvertClass_" + i;

			// [ramrao] VSUQFE#2248 (VSWhidbey 231885)
			bool bSchemaToBeCompared = IsSchemaToBeCompared();
			bool bNewClassToCompile = false;

			// Mof compilation is dictated by, whether the assembly is registered as instrumented or not
			if (bSchemaToBeCompared == false)
			{
				bNewClassToCompile = !IsAssemblyRegistered();
			}

			for(int i=0;i<types.Length;i++)
			{
				SchemaMapping mapping = new SchemaMapping(types[i], this, mapTypeToConverterClassName);

				codeCCTOR.Line(String.Format("mapTypeToConverter[typeof({0})] = typeof({1});", mapping.ClassType.FullName.Replace('+', '.'), mapping.CodeClassName));  // bug#92918 - watch for nested classes
				// [ramrao] VSUQFE#2248 (VSWhidbey 231885)
				if (bSchemaToBeCompared == true && IsClassAlreadyPresentInRepository(mapping.NewClass) == false)
				{
					bNewClassToCompile = true;
				}

				ReplaceClassIfNecessary(mapping.ClassPath, mapping.NewClass);

				mofs[i] = GetMofFormat(mapping.NewClass);
				code.AddChild(mapping.Code);
				switch(mapping.InstrumentationType)
				{
					case InstrumentationType.Event:
						events.Add(mapping.ClassName);
						break;

					case InstrumentationType.Instance:
						instances.Add(mapping.ClassName);
						break;

					case InstrumentationType.Abstract:
						abstracts.Add(mapping.ClassName);
						break;

					default:
						break;
				}
			}

			RegisterAssemblySpecificDecoupledProviderInstance();
			RegisterProviderAsEventProvider(events);
			RegisterProviderAsInstanceProvider();
			RegisterAssemblyAsInstrumented();
			Directory.CreateDirectory(DataDirectory);
			using(StreamWriter log = new StreamWriter(CodePath, false, Encoding.Unicode))
			{
				log.WriteLine(code);
				log.WriteLine(iwoaDef+"new ManagementPath(@\""+this.NamespaceName+"\")"+iwoaDefEnd);
			}

			// Always generate the MOF in unicode
			using(StreamWriter log = new StreamWriter(MofPath, false, Encoding.Unicode))
			{
				log.WriteLine(GenerateMof(mofs));
			}

			// [ramrao] VSUQFE#2248 (VSWhidbey 231885)
			// Write the mof to a file and compile it only if there are any new classes , apart from
			// what is there in the repository
			if (bNewClassToCompile == true)
			{
			RegisterSchemaUsingMofcomp ( MofPath ) ;
			//WMICapabilities.AddAutorecoverMof(MofPath);
		}
		}
        
		const string iwoaDef =
@"class IWOA
{
protected const string DllName = ""wminet_utils.dll"";
protected const string EntryPointName = ""UFunc"";
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""GetPropertyHandle"")] public static extern int GetPropertyHandle_f27(int vFunc, IntPtr pWbemClassObject, [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszPropertyName, [Out] out Int32 pType, [Out] out Int32 plHandle);
//[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref Byte aData);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""ReadPropertyValue"")] public static extern int ReadPropertyValue_f29(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lBufferSize, [Out] out Int32 plNumBytes, [Out] out Byte aData);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""ReadDWORD"")] public static extern int ReadDWORD_f30(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [Out] out UInt32 pdw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteDWORD"")] public static extern int WriteDWORD_f31(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] UInt32 dw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""ReadQWORD"")] public static extern int ReadQWORD_f32(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [Out] out UInt64 pqw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteQWORD"")] public static extern int WriteQWORD_f33(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] UInt64 pw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""GetPropertyInfoByHandle"")] public static extern int GetPropertyInfoByHandle_f34(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [Out][MarshalAs(UnmanagedType.BStr)]  out string   pstrName, [Out] out Int32 pType);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""Lock"")] public static extern int Lock_f35(int vFunc, IntPtr pWbemClassObject, [In] Int32 lFlags);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""Unlock"")] public static extern int Unlock_f36(int vFunc, IntPtr pWbemClassObject, [In] Int32 lFlags);

[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""Put"")] public static extern int Put_f5(int vFunc, IntPtr pWbemClassObject, [In][MarshalAs(UnmanagedType.LPWStr)]  string   wszName, [In] Int32 lFlags, [In] ref object pVal, [In] Int32 Type);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In][MarshalAs(UnmanagedType.LPWStr)] string str);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref Byte n);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref SByte n);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref Int16 n);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"")] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref UInt16 n);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WritePropertyValue"", CharSet=CharSet.Unicode)] public static extern int WritePropertyValue_f28(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 lNumBytes, [In] ref Char c);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteDWORD"")] public static extern int WriteDWORD_f31(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int32 dw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteSingle"")] public static extern int WriteDWORD_f31(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Single dw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteQWORD"")] public static extern int WriteQWORD_f33(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Int64 pw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""WriteDouble"")] public static extern int WriteQWORD_f33(int vFunc, IntPtr pWbemClassObject, [In] Int32 lHandle, [In] Double pw);
[SuppressUnmanagedCodeSecurity, DllImport(DllName, EntryPoint=""Clone"")] public static extern int Clone_f(int vFunc, IntPtr pWbemClassObject, [Out] out IntPtr ppCopy);
}
interface IWmiConverter
{
    void ToWMI(object obj);
    ManagementObject GetInstance();
}
class SafeAssign
{
    public static UInt16 boolTrue = 0xffff;
    public static UInt16 boolFalse = 0;
    static Hashtable validTypes = new Hashtable();
    static SafeAssign()
    {
        validTypes.Add(typeof(SByte), null);
        validTypes.Add(typeof(Byte), null);
        validTypes.Add(typeof(Int16), null);
        validTypes.Add(typeof(UInt16), null);
        validTypes.Add(typeof(Int32), null);
        validTypes.Add(typeof(UInt32), null);
        validTypes.Add(typeof(Int64), null);
        validTypes.Add(typeof(UInt64), null);
        validTypes.Add(typeof(Single), null);
        validTypes.Add(typeof(Double), null);
        validTypes.Add(typeof(Boolean), null);
        validTypes.Add(typeof(String), null);
        validTypes.Add(typeof(Char), null);
        validTypes.Add(typeof(DateTime), null);
        validTypes.Add(typeof(TimeSpan), null);
        validTypes.Add(typeof(ManagementObject), null);
        nullClass.SystemProperties [""__CLASS""].Value = ""nullInstance"";
    }
    public static object GetInstance(object o)
    {
        if(o is ManagementObject)
            return o;
        return null;
    }
    static ManagementClass nullClass = new ManagementClass(" ;
		
const string iwoaDefEnd =
			@");
    
    public static ManagementObject GetManagementObject(object o)
    {
        if(o != null && o is ManagementObject)
            return o as ManagementObject;
        // Must return empty instance
        return nullClass.CreateInstance();
    }
    public static object GetValue(object o)
    {
        Type t = o.GetType();
        if(t.IsArray)
            t = t.GetElementType();
        if(validTypes.Contains(t))
            return o;
        return null;
    }
    public static string WMITimeToString(DateTime dt)
    {
        TimeSpan ts = dt.Subtract(dt.ToUniversalTime());
        int diffUTC = (ts.Minutes + ts.Hours * 60);
        if(diffUTC >= 0)
            return String.Format(""{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.{6:D3}000+{7:D3}"", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, diffUTC);
        return String.Format(""{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.{6:D3}000-{7:D3}"", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, -diffUTC);
    }
    public static string WMITimeToString(TimeSpan ts)
    {
        return String.Format(""{0:D8}{1:D2}{2:D2}{3:D2}.{4:D3}000:000"", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
    }
    public static string[] WMITimeArrayToStringArray(DateTime[] dates)
    {
        string[] strings = new string[dates.Length];
        for(int i=0;i<dates.Length;i++)
            strings[i] = WMITimeToString(dates[i]);
        return strings;
    }
    public static string[] WMITimeArrayToStringArray(TimeSpan[] timeSpans)
    {
        string[] strings = new string[timeSpans.Length];
        for(int i=0;i<timeSpans.Length;i++)
            strings[i] = WMITimeToString(timeSpans[i]);
        return strings;
    }
}
";


        ///////////////////////////////////////////
        // Functions that create instances for the
        // registration of various objects
        void RegisterNamespaceAsInstrumented()
        {
            ManagementClass registrationClass = new ManagementClass(GlobalRegistrationClassPath);
            ManagementObject inst = registrationClass.CreateInstance();
            inst["NamespaceName"] = NamespaceName;
            inst.Put();
        }

        void RegisterAssemblyAsInstrumented()
        {
            ManagementClass registrationClass = new ManagementClass(RegistrationClassPath);
            ManagementObject inst = registrationClass.CreateInstance();
            inst["Name"] = DecoupledProviderInstanceName;
            inst["RegisteredBuild"] = AssemblyUniqueIdentifier;
            inst["FullName"] = AssemblyName;
            inst["PathToAssembly"] = AssemblyPath;
            inst["Code"] = "";
            inst["Mof"] = "";
            inst.Put();
        }

        void RegisterAssemblySpecificDecoupledProviderInstance()
        {
            ManagementClass providerClass = new ManagementClass(ProviderClassPath);
            ManagementObject inst = providerClass.CreateInstance();
            inst["Name"] = DecoupledProviderInstanceName;
            inst["HostingModel"] = "Decoupled:Com"; // TODO : SHOULD NOT NEED
            if(null != SecurityDescriptor)
                inst["SecurityDescriptor"] = SecurityDescriptor;
            inst.Put();
        }

        string RegisterProviderAsEventProvider(StringCollection events)
        {
            // TODO: Hanlde no events with MOF generation
//            if(events.Count == 0)
//                return null;

            ManagementClass providerRegistrationClass = new ManagementClass(EventProviderRegistrationClassPath);
            ManagementObject inst = providerRegistrationClass.CreateInstance();
            inst["provider"] = "\\\\.\\"+ProviderPath;
            string [] queries = new string[events.Count];
            int iCur = 0;
            foreach(string eventName in events)
                queries[iCur++] = "select * from "+eventName;

            inst["EventQueryList"] = queries;
            return inst.Put().Path;
        }

        string RegisterProviderAsInstanceProvider()
        {
            ManagementClass providerRegistrationClass = new ManagementClass(InstanceProviderRegistrationClassPath);
            ManagementObject inst = providerRegistrationClass.CreateInstance();
            inst["provider"] = "\\\\.\\"+ProviderPath;
            inst["SupportsGet"] = true;
            inst["SupportsEnumeration"] = true;
            return inst.Put().Path;
        }

        ///////////////////////////////////////////
        // Functions that create Class prototypes

        delegate ManagementClass ClassMaker();

        ManagementClass MakeNamingClass()
        {
            ManagementClass baseClass = new ManagementClass(GlobalInstrumentationClassPath);
            ManagementClass newClass = baseClass.Derive(NamingClassName);
            newClass.Qualifiers.Add("abstract", true);
            PropertyDataCollection props = newClass.Properties;
            props.Add("InstrumentedAssembliesClassName", InstrumentedAssembliesClassName, CimType.String);
            return newClass;
        }
        
        ManagementClass MakeInstrumentationClass()
        {
            ManagementClass newClass = new ManagementClass(NamespaceName, "", null);
            newClass.SystemProperties ["__CLASS"].Value = InstrumentationClassName;
            newClass.Qualifiers.Add("abstract", true);
            return newClass;
        }

        ManagementClass MakeGlobalInstrumentationClass()
        {
            ManagementClass newClass = new ManagementClass(GlobalWmiNetNamespace, "", null);
            newClass.SystemProperties ["__CLASS"].Value = InstrumentationClassName;
            newClass.Qualifiers.Add("abstract", true);
            return newClass;
        }

        ManagementClass MakeRegistrationClass()
        {
            ManagementClass baseClass = new ManagementClass(InstrumentationClassPath);
            ManagementClass newClass = baseClass.Derive(InstrumentedAssembliesClassName);
            PropertyDataCollection props = newClass.Properties;
            props.Add("Name", CimType.String, false);
            PropertyData prop = props["Name"];
            prop.Qualifiers.Add("key", true);
            props.Add("RegisteredBuild", CimType.String, false);
            props.Add("FullName", CimType.String, false);
            props.Add("PathToAssembly", CimType.String, false);
            props.Add("Code", CimType.String, false);
            props.Add("Mof", CimType.String, false);
            return newClass;
        }

        ManagementClass MakeNamespaceRegistrationClass()
        {
            ManagementClass baseClass = new ManagementClass(GlobalInstrumentationClassPath);
            ManagementClass newClass = baseClass.Derive(InstrumentedNamespacesClassName);
            PropertyDataCollection props = newClass.Properties;
            props.Add("NamespaceName", CimType.String, false);
            PropertyData prop = props["NamespaceName"];
            prop.Qualifiers.Add("key", true);
            return newClass;
        }

        ManagementClass MakeDecoupledProviderClass()
        {
            ManagementClass baseClass = new ManagementClass(Win32ProviderClassPath);
            ManagementClass newClass = baseClass.Derive(DecoupledProviderClassName);
            PropertyDataCollection props = newClass.Properties;
            props.Add("HostingModel", "Decoupled:Com", CimType.String);
            props.Add("SecurityDescriptor", CimType.String, false);
            props.Add("Version", 1, CimType.UInt32);
            props["CLSID"].Value = DecoupledProviderCLSID;
            return newClass;
        }

        ManagementClass MakeProviderClass()
        {
            ManagementClass baseClass = new ManagementClass(DecoupledProviderClassPath);
            ManagementClass newClass = baseClass.Derive(ProviderClassName);
            PropertyDataCollection props = newClass.Properties;
            props.Add("Assembly", CimType.String, false);
            return newClass;
        }

        ///////////////////////////////////////////
        // WMI Helper Functions

		//
		// [RAID#143683, marioh] This method was added to register the instrumentation schema by forking 
		// mofcomp.exe rather than simply writing it directly to the repository. The reason for this is
		// a change in .NET server that requires MOF files to be compiled using mofcomp in order to be
		// autorecoverable. It is no longer possible to do by directly accessing the autorecover key in the
		// registry (due to security considerations).
		//
		// Note: Due to code design, we are using mofcomp IN ADDITION to writing directly to the repository.
		//
              [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
		static void RegisterSchemaUsingMofcomp ( string mofPath )
		{
			System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo ( ) ;
			processInfo.Arguments = mofPath ;
			processInfo.FileName = WMICapabilities.InstallationDirectory+"\\mofcomp.exe" ;
			processInfo.UseShellExecute = false ;
			processInfo.RedirectStandardOutput = true ;

			//
			// [PS#170134, marioh] Make sure no command shell window is displayed when invoking mofcomp
			//
			processInfo.CreateNoWindow = true ;				

			System.Diagnostics.Process proc = System.Diagnostics.Process.Start ( processInfo ) ;
			proc.WaitForExit ( ) ;
		}

        static void EnsureNamespace(string baseNamespace, string childNamespaceName)
        {
            if(!DoesInstanceExist(baseNamespace + ":__NAMESPACE.Name=\""+childNamespaceName+"\""))
            {
                ManagementClass namespaceClass = new ManagementClass(baseNamespace + ":__NAMESPACE");
                ManagementObject inst = namespaceClass.CreateInstance();
                inst["Name"] = childNamespaceName;
                inst.Put();
            }
        }

        static void EnsureNamespace(InstallLogWrapper context, string namespaceName)
        {
            context.LogMessage(RC.GetString("NAMESPACE_ENSURE")+ " " +namespaceName);

            string fullNamespace = null;
            foreach(string name in namespaceName.Split(new char[] {'\\'}))
            {
                if(fullNamespace == null)
                {
                    fullNamespace = name;
                    continue;
                }
                EnsureNamespace(fullNamespace, name);
                fullNamespace += "\\" + name;
            }
        }

        static void EnsureClassExists(InstallLogWrapper context, string classPath, ClassMaker classMakerFunction)
        {
            try
            {
                context.LogMessage(RC.GetString("CLASS_ENSURE") + " " +classPath);
                ManagementClass theClass = new ManagementClass(classPath);
                theClass.Get();
            }
            catch(ManagementException e)
            {
                if(e.ErrorCode == ManagementStatus.NotFound)
                {
                    // The class does not exist.  Create it.
                    context.LogMessage(RC.GetString("CLASS_ENSURECREATE")+ " " +classPath);
                    ManagementClass theClass = classMakerFunction();
                    theClass.Put();
                }
                else
                    throw e;
            }
        }

        static bool DoesInstanceExist(string objectPath)
        {
            bool exists = false;
            try
            {
                ManagementObject inst = new ManagementObject(objectPath);
                inst.Get();
                exists = true;
            }
            catch(ManagementException e)
            {
                if(     ManagementStatus.InvalidNamespace != e.ErrorCode
                    &&  ManagementStatus.InvalidClass != e.ErrorCode
                    &&  ManagementStatus.NotFound != e.ErrorCode
                    )
                    throw e;
            }
            return exists;
        }

		static bool DoesClassExist(string objectPath)
		{
			bool exists = false;

			try
			{
				ManagementObject cls = new ManagementClass(objectPath);

				cls.Get();
				exists = true;
			}
			catch (ManagementException e)
			{
				if (ManagementStatus.InvalidNamespace != e.ErrorCode && ManagementStatus.InvalidClass != e.ErrorCode && ManagementStatus.NotFound != e.ErrorCode)
                    throw e;
            }
            return exists;
        }

        /// <summary>
        /// Given a class path, this function will return the ManagementClass
        /// if it exists, or null if it does not.
        /// </summary>
        /// <param name="classPath">WMI path to Class</param>
        /// <returns></returns>
        static ManagementClass SafeGetClass(string classPath)
        {
            ManagementClass theClass = null;
            try
            {
                ManagementClass existingClass = new ManagementClass(classPath);
                existingClass.Get();
                theClass = existingClass;
            }
            catch(ManagementException e)
            {
                if(e.ErrorCode != ManagementStatus.NotFound)
                    throw e;
            }
            return theClass;
        }

        /// <summary>
        /// Given a class path, and a ManagementClass class definition, this
        /// function will create the class if it does not exist, replace the
        /// class if it exists but is different, or do nothing if the class
        /// exists and is identical.  This is useful for performance reasons
        /// since it can be expensive to delete an existing class and replace
        /// it.
        /// </summary>
        /// <param name="classPath">WMI path to class</param>
        /// <param name="newClass">Class to create or replace</param>
        static void ReplaceClassIfNecessary(string classPath, ManagementClass newClass)
        {
            try
            {
                ManagementClass oldClass = SafeGetClass(classPath);
                if(null == oldClass)
                    newClass.Put();
                else
                {
                    // TODO: Figure Out Why CompareTo does not work!!!
                    //                if(false == newClass.CompareTo(newClass, ComparisonSettings.IgnoreCase | ComparisonSettings.IgnoreObjectSource))
                    if(newClass.GetText(TextFormat.Mof) != oldClass.GetText(TextFormat.Mof))
                    {
                        // TODO: Log to context?
                        oldClass.Delete();
                        newClass.Put();
                    }
                }
            }
            catch(ManagementException e)
            {
				string strformat = RC.GetString("CLASS_NOTREPLACED_EXCEPT") + "\r\n{0}\r\n{1}";
                throw new ArgumentException(String.Format(strformat, classPath, newClass.GetText(TextFormat.Mof)), e);
            }
        }

        static string GetMofFormat(ManagementObject obj)
        {
            return obj.GetText(TextFormat.Mof).Replace("\n", "\r\n") + "\r\n";
        }

        static string PragmaNamespace(string namespaceName)
        {
            return String.Format("#pragma namespace(\"\\\\\\\\.\\\\{0}\")\r\n\r\n", namespaceName.Replace("\\", "\\\\"));
        }

        static string EnsureNamespaceInMof(string baseNamespace, string childNamespaceName)
        {
            return String.Format("{0}instance of __Namespace\r\n{{\r\n  Name = \"{1}\";\r\n}};\r\n\r\n", PragmaNamespace(baseNamespace), childNamespaceName);
        }

        static string EnsureNamespaceInMof(string namespaceName)
        {
            string mof="";
            string fullNamespace = null;
            foreach(string name in namespaceName.Split(new char[] {'\\'}))
            {
                if(fullNamespace == null)
                {
                    fullNamespace = name;
                    continue;
                }
                mof+=EnsureNamespaceInMof(fullNamespace, name);
                fullNamespace += "\\" + name;
            }
            return mof;
        }
    }
}
