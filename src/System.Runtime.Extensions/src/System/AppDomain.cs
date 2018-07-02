// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS0067 // events are declared but not used

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.IO;
using System.Runtime.Remoting;
using System.Security.Principal;

namespace System
{
    public partial class AppDomain : MarshalByRefObject
    {
        private static readonly AppDomain s_domain = new AppDomain();
        private readonly object _forLock = new object();
        private IPrincipal _defaultPrincipal;

        private AppDomain() { }

        public static AppDomain CurrentDomain => s_domain;

        public string BaseDirectory => AppContext.BaseDirectory;

        public string RelativeSearchPath => null;

        public event UnhandledExceptionEventHandler UnhandledException
        {
            add { AppContext.UnhandledException += value; }
            remove { AppContext.UnhandledException -= value; }
        }

        public string DynamicDirectory => null;

        [ObsoleteAttribute("AppDomain.SetDynamicBase has been deprecated. Please investigate the use of AppDomainSetup.DynamicBase instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void SetDynamicBase(string path) { }

        public string FriendlyName
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                return assembly != null ? assembly.GetName().Name : "DefaultDomain";
            }
        }

        public int Id => 1;

        public bool IsFullyTrusted => true;

        public bool IsHomogenous => true;

        public event EventHandler DomainUnload;

        public event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException
        {
            add 
            { 
#if uapaot
                AppContext.SetAppDomain(this);
#endif
                AppContext.FirstChanceException += value; 
            }
            remove { AppContext.FirstChanceException -= value; }
        }

        public event EventHandler ProcessExit
        {
            add { AppContext.ProcessExit += value; }
            remove { AppContext.ProcessExit -= value; }
        }

        public string ApplyPolicy(string assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }
            if (assemblyName.Length == 0 || assemblyName[0] == '\0')
            {
                throw new ArgumentException(SR.ZeroLengthString);
            }

            return assemblyName;
        }

        public static AppDomain CreateDomain(string friendlyName)
        {
            if (friendlyName == null) throw new ArgumentNullException(nameof(friendlyName));
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_AppDomains);
        }

        public int ExecuteAssembly(string assemblyFile) => ExecuteAssembly(assemblyFile, null);

        public int ExecuteAssembly(string assemblyFile, string[] args)
        {
            if (assemblyFile == null)
            {
                throw new ArgumentNullException(nameof(assemblyFile));
            }
            string fullPath = Path.GetFullPath(assemblyFile);
            Assembly assembly = Assembly.LoadFile(fullPath);
            return ExecuteAssembly(assembly, args);
        }

        public int ExecuteAssembly(string assemblyFile, string[] args, byte[] hashValue, Configuration.Assemblies.AssemblyHashAlgorithm hashAlgorithm)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_CAS); // This api is only meaningful for very specific partial trust/CAS scenarios
        }

        private int ExecuteAssembly(Assembly assembly, string[] args)
        {
            MethodInfo entry = assembly.EntryPoint;
            if (entry == null)
            {
                throw new MissingMethodException(SR.EntryPointNotFound + assembly.FullName);
            }

            object result = null;
            try
            {
                result = entry.GetParameters().Length > 0 ?
                    entry.Invoke(null, new object[] { args }) :
                    entry.Invoke(null, null);
            }
            catch (TargetInvocationException targetInvocationException)
            {
                if (targetInvocationException.InnerException == null)
                {
                    throw;
                }
                
                // We are catching the TIE here and throws the inner exception only,
                // this is needed to have a consistent exception story with desktop clr
                ExceptionDispatchInfo.Throw(targetInvocationException.InnerException);
            }

            return result != null ? (int)result : 0;
        }

        public int ExecuteAssemblyByName(AssemblyName assemblyName, params string[] args) =>
            ExecuteAssembly(Assembly.Load(assemblyName), args);

        public int ExecuteAssemblyByName(string assemblyName) =>
            ExecuteAssemblyByName(assemblyName, null);

        public int ExecuteAssemblyByName(string assemblyName, params string[] args) =>
            ExecuteAssembly(Assembly.Load(assemblyName), args);

        public object GetData(string name) => AppContext.GetData(name);

        public void SetData(string name, object data) => AppContext.SetData(name, data);

        public bool? IsCompatibilitySwitchSet(string value)
        {
            bool result;
            return AppContext.TryGetSwitch(value, out result) ? result : default(bool?);
        }

        public bool IsDefaultAppDomain() => true;

        public bool IsFinalizingForUnload() => false;

        public override string ToString() =>
            SR.AppDomain_Name + FriendlyName + Environment.NewLine + SR.AppDomain_NoContextPolicies;

        public static void Unload(AppDomain domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }
            throw new CannotUnloadAppDomainException(SR.NotSupported_Platform);
        }

        public Assembly Load(byte[] rawAssembly) => Assembly.Load(rawAssembly);

        public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore) => Assembly.Load(rawAssembly, rawSymbolStore);

        public Assembly Load(AssemblyName assemblyRef) => Assembly.Load(assemblyRef);

        public Assembly Load(string assemblyString) => Assembly.Load(assemblyString);

        public Assembly[] ReflectionOnlyGetAssemblies() => Array.Empty<Assembly>();

        public static bool MonitoringIsEnabled
        {
            get { return false; }
            set
            {
                if (!value)
                {
                    throw new ArgumentException(SR.Arg_MustBeTrue);
                }
                throw new PlatformNotSupportedException(SR.PlatformNotSupported_AppDomain_ResMon);
            }
        }

        public long MonitoringSurvivedMemorySize { get { throw CreateResMonNotAvailException(); } }

        public static long MonitoringSurvivedProcessMemorySize { get { throw CreateResMonNotAvailException(); } }

        public long MonitoringTotalAllocatedMemorySize { get { throw CreateResMonNotAvailException(); } }

        public TimeSpan MonitoringTotalProcessorTime { get { throw CreateResMonNotAvailException(); } }

        private static Exception CreateResMonNotAvailException() => new InvalidOperationException(SR.PlatformNotSupported_AppDomain_ResMon);

        [ObsoleteAttribute("AppDomain.GetCurrentThreadId has been deprecated because it does not provide a stable Id when managed threads are running on fibers (aka lightweight threads). To get a stable identifier for a managed thread, use the ManagedThreadId property on Thread.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
        public static int GetCurrentThreadId() => Environment.CurrentManagedThreadId;

        public bool ShadowCopyFiles => false;

        [ObsoleteAttribute("AppDomain.AppendPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void AppendPrivatePath(string path) { }

        [ObsoleteAttribute("AppDomain.ClearPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void ClearPrivatePath() { }

        [ObsoleteAttribute("AppDomain.ClearShadowCopyPath has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyDirectories instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void ClearShadowCopyPath() { }

        [ObsoleteAttribute("AppDomain.SetCachePath has been deprecated. Please investigate the use of AppDomainSetup.CachePath instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void SetCachePath(string path) { }

        [ObsoleteAttribute("AppDomain.SetShadowCopyFiles has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyFiles instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void SetShadowCopyFiles() { }

        [ObsoleteAttribute("AppDomain.SetShadowCopyPath has been deprecated. Please investigate the use of AppDomainSetup.ShadowCopyDirectories instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void SetShadowCopyPath(string path) { }

        public Assembly[] GetAssemblies() => AssemblyLoadContext.GetLoadedAssemblies();

        public event AssemblyLoadEventHandler AssemblyLoad
        {
            add { AssemblyLoadContext.AssemblyLoad += value; }
            remove { AssemblyLoadContext.AssemblyLoad -= value; }
        }

        public event ResolveEventHandler AssemblyResolve
        {
            add { AssemblyLoadContext.AssemblyResolve += value; }
            remove { AssemblyLoadContext.AssemblyResolve -= value; }
        }

        public event ResolveEventHandler ReflectionOnlyAssemblyResolve;

        public event ResolveEventHandler TypeResolve
        {
            add { AssemblyLoadContext.TypeResolve += value; }
            remove { AssemblyLoadContext.TypeResolve -= value; }
        }

        public event ResolveEventHandler ResourceResolve
        {
            add { AssemblyLoadContext.ResourceResolve += value; }
            remove { AssemblyLoadContext.ResourceResolve -= value; }
        }

        public void SetPrincipalPolicy(PrincipalPolicy policy) { }

        public void SetThreadPrincipal(IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            lock (_forLock)
            {
                // Check that principal has not been set previously.
                if (_defaultPrincipal != null)
                {
                    throw new SystemException(SR.AppDomain_Policy_PrincipalTwice);
                }
                _defaultPrincipal = principal;
            }
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
 
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            Contract.EndContractBlock();

            return Activator.CreateInstance(assemblyName, typeName);
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
 
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            Contract.EndContractBlock();

            return Activator.CreateInstance(assemblyName,
                                            typeName,
                                            ignoreCase,
                                            bindingAttr,
                                            binder,
                                            args,
                                            culture,
                                            activationAttributes);
        }

        public ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
 
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            Contract.EndContractBlock();

            return Activator.CreateInstance(assemblyName, typeName, activationAttributes);
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName)
        {
            ObjectHandle oh = CreateInstance(assemblyName, typeName);
            if (oh == null)
                return null;
 
            return oh.Unwrap();
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes)
        {
            ObjectHandle oh = CreateInstance(assemblyName, 
                                             typeName, 
                                             ignoreCase, 
                                             bindingAttr,
                                             binder, 
                                             args, 
                                             culture, 
                                             activationAttributes);
 
            if (oh == null)
                return null; 
            
            return oh.Unwrap();
        }

        public object CreateInstanceAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
        {
            ObjectHandle oh = CreateInstance(assemblyName, typeName, activationAttributes);
            if (oh == null)
                return null; 
 
            return oh.Unwrap();
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
            Contract.EndContractBlock();

            return Activator.CreateInstanceFrom(assemblyFile, typeName);
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
            Contract.EndContractBlock();

            return Activator.CreateInstanceFrom(assemblyFile,
                                                typeName,
                                                ignoreCase,
                                                bindingAttr,
                                                binder,
                                                args,
                                                culture,
                                                activationAttributes);
        }

        public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
        {
            // jit does not check for that, so we should do it ...
            if (this == null)
                throw new NullReferenceException();
            Contract.EndContractBlock();

            return Activator.CreateInstanceFrom(assemblyFile, typeName, activationAttributes);
        }

        public object CreateInstanceFromAndUnwrap(string assemblyFile, string typeName)
        {
            ObjectHandle oh = CreateInstanceFrom(assemblyFile, typeName);
            if (oh == null)
                return null;  
 
            return oh.Unwrap(); 
        }

        public object CreateInstanceFromAndUnwrap(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes)
        {
            ObjectHandle oh = CreateInstanceFrom(assemblyFile, 
                                                 typeName, 
                                                 ignoreCase, 
                                                 bindingAttr,
                                                 binder, 
                                                 args, 
                                                 culture, 
                                                 activationAttributes);
 
            if (oh == null)
                return null; 
 
            return oh.Unwrap();
        }

        public object CreateInstanceFromAndUnwrap(string assemblyFile, string typeName, object[] activationAttributes)
        {
            ObjectHandle oh = CreateInstanceFrom(assemblyFile, typeName, activationAttributes);
            if (oh == null)
                return null; 
 
            return oh.Unwrap();
        }
    }
}
