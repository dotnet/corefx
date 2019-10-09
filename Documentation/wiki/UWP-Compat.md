# Behavior change between .NET Core and UWP

This document contains list of not supported APIs on UWP but supported on .NET Core

## Notes
- This document is for preview only and was semi-generated, information may be incomplete or may contain errors
- This API list does not include APIs not supported by .NET Core. See https://github.com/dotnet/corefx/wiki/ApiCompat for reference

# Table of Content

<!-- MarkdownTOC depth=2 autolink=true bracket=round -->

- [ACL](#acl)
    - [Namespaces](#namespaces)
- [Registry](#registry)
    - [Types](#types)
- [Process](#process)
    - [Members](#members)
- [Xml / Xml Serialization](#xml--xml-serialization)
    - [Types](#types-1)
    - [Members](#members-1)
- [ServiceController](#servicecontroller)
    - [Types](#types-2)
- [System.Net](#systemnet)
    - [Types](#types-3)
- [System.Linq](#systemlinq)
    - [Members](#members-2)
- [System.IO](#systemio)
    - [Namespaces](#namespaces-1)
    - [Members](#members-3)
- [Crypto](#crypto)
    - [Members](#members-4)
- [SqlClient](#sqlclient)
    - [Members](#members-5)
- [DirectoryServices](#directoryservices)
- [System.Security.Cryptography.OpenSsl](#systemsecuritycryptographyopenssl)
- [System.Reflection.Context](#systemreflectioncontext)
- [StackFrame](#stackframe)
- [MemoryMappedFiles](#memorymappedfiles)
- [System.Threading](#systemthreading)
    - [Members](#members-6)
- [System.Runtime*](#systemruntime)
- [Serialization related APIs](#serialization-related-apis)
- [System.Reflection](#systemreflection)
    - [Other APIs throwing PlatformNotSupportedException](#other-apis-throwing-platformnotsupportedexception)
- [System.Runtime.InteropServices](#systemruntimeinteropservices)
    - [Other not supported APIs](#other-not-supported-apis)
- [System.Diagnostics.Debugger](#systemdiagnosticsdebugger)
- [System.Private.CoreLib](#systemprivatecorelib)
    - [Other not supported APIs](#other-not-supported-apis-1)

<!-- /MarkdownTOC -->

# ACL
Note: Note: This assembly is marked with UWPCompatible=false

## Namespaces

```
System.Security.AccessControl
```

# Registry
Note: This assembly is marked with UWPCompatible=false

## Types

```
Microsoft.Win32.Registry
Microsoft.Win32.RegistryKey
Microsoft.Win32.SafeHandles.SafeRegistryHandle
Microsoft.Win32.RegistryAclExtensions
```

# Process
In general retrieving information about processes is not supported
## Members

```
P:System.Diagnostics.Process.PagedSystemMemorySize64
P:System.Diagnostics.Process.MainModule
P:System.Diagnostics.Process.PagedMemorySize
P:System.Diagnostics.Process.PrivateMemorySize64
P:System.Diagnostics.Process.NonpagedSystemMemorySize64
P:System.Diagnostics.Process.ExitTime
P:System.Diagnostics.Process.ProcessName
P:System.Diagnostics.Process.HasExited
P:System.Diagnostics.Process.PeakVirtualMemorySize
P:System.Diagnostics.Process.VirtualMemorySize
M:System.Diagnostics.Process.Start(System.String)
P:System.Diagnostics.Process.PeakVirtualMemorySize64
P:System.Diagnostics.Process.PrivateMemorySize
M:System.Diagnostics.Process.Start(System.String,System.String)
P:System.Diagnostics.Process.BasePriority
M:System.Diagnostics.Process.GetProcessesByName(System.String)
M:System.Diagnostics.Process.GetProcessesByName(System.String,System.String)
P:System.Diagnostics.Process.PagedMemorySize64
P:System.Diagnostics.Process.VirtualMemorySize64
P:System.Diagnostics.Process.WorkingSet
P:System.Diagnostics.Process.SafeHandle
P:System.Diagnostics.Process.ExitCode
M:System.Diagnostics.Process.Start(System.String,System.String,System.String,System.Security.SecureString,System.String)
P:System.Diagnostics.Process.PagedSystemMemorySize
P:System.Diagnostics.Process.MachineName
M:System.Diagnostics.Process.GetProcesses
P:System.Diagnostics.Process.PeakPagedMemorySize
P:System.Diagnostics.Process.NonpagedSystemMemorySize
P:System.Diagnostics.Process.WorkingSet64
P:System.Diagnostics.Process.HandleCount
M:System.Diagnostics.Process.GetProcesses(System.String)
P:System.Diagnostics.Process.PeakPagedMemorySize64
P:System.Diagnostics.Process.Threads
P:System.Diagnostics.Process.PeakWorkingSet64
P:System.Diagnostics.Process.Id
M:System.Diagnostics.Process.Start(System.String,System.String,System.Security.SecureString,System.String)
P:System.Diagnostics.Process.SessionId
P:System.Diagnostics.Process.Modules
P:System.Diagnostics.Process.PeakWorkingSet
P:System.Diagnostics.ProcessStartInfo.UseShellExecute
```

# Xml / Xml Serialization
Anything depending on Ref.Emit is not supported
## Types

```
System.Xml.Xsl.XslCompiledTransform
```

## Members

```
M:System.Runtime.Serialization.XmlObjectSerializerReadContextComplex.ResolveDataContractFromTypeName  (only when code is using NetDataContractSerializer)
```

# ServiceController
## Types

```
System.ServiceProcess.ServiceBase
System.ServiceProcess.ServiceController
System.ServiceProcess.SessionChangeDescription
System.ServiceProcess.TimeoutException
```

# System.Net

Note: This data was collected from comments on UWP specific disabled tests
- UAP does not allow HTTP/1.0 requests
- UAP does not support custom proxies
- UAP doesn't allow revocation checking to be turned off
- SslProtocols not supported on UAP
- WinHttpHandler not supported on UAP
- WebSocket partial send is not supported on UAP

## Types

```
System.Net.NetworkInformation.Ping
System.Net.HttpListenerTimeoutManager
System.Net.Http.RtcRequestFactory
System.Net.Http.HttpClientHandler

[System.Net.WebSockets] System.Net.WebSockets.WebSocketValidate::ThrowPlatformNotSupportedException()
[System.Net.WebSockets.Client] System.Net.WebSockets.WinRTWebSocket.<ConnectAsync>d__31::MoveNext()
[System.Net.HttpListener] System.Net.HttpListenerTimeoutManager
[System.Net.Http] System.Net.Http.HttpClientHandler
```

# System.Linq
## Members

```
M:System.Linq.Expressions.Expression.GetDelegateType(System.Type[])
M:System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression,System.String,System.Boolean,System.Collections.Generic.IEnumerable{System.Linq.Expressions.ParameterExpression})
[System.Linq.Expressions] System.Linq.Expressions.Compiler.DelegateHelpers::MakeNewCustomDelegate(Type[])
```

# System.IO
Note: This data was collected from comments on UWP specific disabled tests
- Accessing drive format is not permitted inside an AppContainer
- GetDiskFreeSpaceEx blocked in AppContainer

## Namespaces

```
System.IO.FileSystemAclExtensions
```

## Members

```
M:System.IO.Ports.SerialPort.GetPortNames
[System.IO.Compression] System.IO.Compression.ZLibException::.ctor(SerializationInfo, StreamingContext)
[System.IO.FileSystem.DriveInfo] System.IO.DriveInfo::.ctor(SerializationInfo, StreamingContext)
[System.Collections] System.Collections.Generic.LinkedList`1.Enumerator::.ctor(SerializationInfo, StreamingContext)
```

# Crypto
## Members

```
[System.Security.Cryptography.Cng] System.Security.Cryptography.ECDsaCng::SpecialNistAlgorithmToCurveName(string)
S.R.Extensions
[System.Runtime.Extensions] System.Collections.Hashtable.SyncHashtable::.ctor(SerializationInfo, StreamingContext)
```

# SqlClient
Note: This assembly is marked with UWPCompatible=false

## Members
```
[System.Data.SqlClient] System.Data.LocalDBAPI::LoadProcAddress()
[System.Data.SqlClient] System.Data.SqlClient.SNI.LocalDB::GetLocalDBConnectionString(string)
```

# DirectoryServices
Note: This assembly is marked with UWPCompatible=false

Note: This data was collected from comments on UWP specific disabled tests
- Getting information about domain is denied inside AppContainer

# System.Security.Cryptography.OpenSsl
Note: This assembly is marked with UWPCompatible=false

# System.Reflection.Context
Note: This assembly is marked with UWPCompatible=false

# StackFrame
Note: This data was collected from comments on UWP specific disabled tests
- [uapaot] StackFrame is not supported

# MemoryMappedFiles
Note: This data was collected from comments on UWP specific disabled tests
- Windows API returns IO exception error code when viewAccess is ReadWriteExecute

# System.Threading
Note: This data was collected from comments on UWP specific disabled tests
- [uapaot] ThreadPool.SetMinThreads is not supported

## Members

```
[System.Private.CoreLib] System.Threading.ThreadPool::BindHandle(IntPtr)
[System.Private.CoreLib] System.Threading.ThreadPool::BindHandle(SafeHandle)
```

# System.Runtime*
Note: This data was collected from comments on UWP specific disabled tests
- [uapaot] Assembly.Load(byte[])")] not supported
- [uap, not uapaot] When getting resources from PRI file the casing doesn't matter, if it is there it will always find and return the resource
- [uapaot] Non-zero lower-bounded arrays not supported 
- [uapaot] Exception.TargetSite always returns null

# Serialization related APIs

```
[System.Private.CoreLib] System.MulticastDelegate::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.NotFiniteNumberException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.NotImplementedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.NotSupportedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.NullReferenceException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ObjectDisposedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.OperationCanceledException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.OutOfMemoryException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.OverflowException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.PlatformNotSupportedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.RankException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.AbandonedMutexException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.ExecutionContext::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.LockRecursionException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.SemaphoreFullException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.SynchronizationLockException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.Tasks.TaskCanceledException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.Tasks.TaskSchedulerException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.ThreadInterruptedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.ThreadStateException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Threading.WaitHandleCannotBeOpenedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.TimeoutException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.TimeZoneNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.AccessViolationException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ApplicationException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ArgumentException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ArgumentNullException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ArgumentOutOfRangeException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ArithmeticException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.ArrayTypeMismatchException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.BadImageFormatException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Collections.Generic.KeyNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.DivideByZeroException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.DllNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.DuplicateWaitObjectException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Empty::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.EntryPointNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Delegate::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.FieldAccessException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.FormatException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Globalization.CultureNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.InvalidCastException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.InvalidOperationException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.InvalidTimeZoneException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.DirectoryNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.EndOfStreamException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.FileLoadException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.FileNotFoundException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.IOException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.IO.PathTooLongException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.MemberAccessException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.MethodAccessException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.MissingFieldException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.MissingMemberException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.MissingMethodException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Runtime.InteropServices.ExternalException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Runtime.InteropServices.MarshalDirectiveException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Runtime.Serialization.SerializationException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Security.Cryptography.CryptographicException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Security.SecurityException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Security.VerificationException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.StrongNameKeyPair::System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object)
[System.Private.CoreLib] System.Reflection.StrongNameKeyPair::System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.AssemblyName::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.AssemblyName::OnDeserialization(object)
[System.Private.CoreLib] System.Reflection.CustomAttributeFormatException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.InvalidFilterCriteriaException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.Missing::System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.Pointer::System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.TargetException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Resources.MissingManifestResourceException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Resources.MissingSatelliteAssemblyException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Reflection.StrongNameKeyPair::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.RuntimeFieldHandle::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.RuntimeMethodHandle::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.RuntimeTypeHandle::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.SystemException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Text.Encoding.DefaultDecoder::GetRealObject(StreamingContext)
[System.Private.CoreLib] System.Text.Encoding.DefaultEncoder::GetRealObject(StreamingContext)
[System.Private.Interop] System.Runtime.InteropServices.COMException::.ctor(SerializationInfo, StreamingContext)
[System.Private.Interop] System.Runtime.InteropServices.InvalidComObjectException::.ctor(SerializationInfo, StreamingContext)
[System.Private.Interop] System.Runtime.InteropServices.InvalidOleVariantTypeException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.TypeAccessException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.TypeLoadException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.TypeUnloadedException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.UnauthorizedAccessException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Globalization.TextInfo::System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object)
[System.Private.Interop] System.Runtime.InteropServices.SafeArrayRankMismatchException::.ctor(SerializationInfo, StreamingContext)
[System.Private.Interop] System.Runtime.InteropServices.SafeArrayTypeMismatchException::.ctor(SerializationInfo, StreamingContext)
[System.Private.Interop] System.Runtime.InteropServices.SEHException::.ctor(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.DBNull::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.CoreLib] System.Exception::add_SerializeObjectState(EventHandler<SafeSerializationEventArgs>)
[System.Private.CoreLib] System.Exception::remove_SerializeObjectState(EventHandler<SafeSerializationEventArgs>)
```

# System.Reflection

- Type.InvokeMember will throw PlatformNotSupportedException when passed a wrapped COM object
- Passing a type not implemented by the runtime type to Type.MakeGenericType(Type[]) is not supported (on CoreCLR, you could do this to produce a RefEmit.TypeBuilder)
- Type.TypeHandle throws PlatformNotSupportedException on open types or types created by Type.GetTypeFromCLSID()
- Loading assembly from byte array or file paths is not supported
- Late-bound invoking delegate constructors is not supported
- Reflecting over IL bodies is not supported
- MethodBase.MethodHandle is not supported on array constructor
- Assembly::GetCallingAssembly() is not supported
- Multi-module assemblies are not supported
- Loading modules from paths not supported
- Satellite assemblies are not supported
- Loading StrongNameKeyPair from path is not supported
- Reflection support for global members is not supported
- System.Reflection.Pointer - while Pointer itself works, passing them as arguments to late-bound invokes will not. Workaround is to pass IntPtr instead. 

```
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveField(int, Type[], Type[])
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveMember(int, Type[], Type[])
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveMethod(int, Type[], Type[])
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveSignature(int)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveString(int)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ResolveType(int, Type[], Type[])
```

- ProgID is not supported:

```
[System.Private.CoreLib] System.Type::GetCLSIDFromProgID(string, Guid)
```

- `System.Reflection.AssemblyName::GetAssemblyName(string)` and `System.Reflection.AssemblyName::EscapeCodeBase(string)` are not supported


## Other APIs throwing PlatformNotSupportedException

```
[System.Private.Reflection.Execution] Internal.Reflection.Execution.ExecutionEnvironmentImplementation::GetManifestResourceInfo(Assembly, string)
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.IntPtrConstructorMethodInvoker::.ctor(MetadataReader, MethodHandle)
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.IntPtrConstructorMethodInvoker::LdFtnResult.get()
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.NullableInstanceMethodInvoker::LdFtnResult.get()
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.StringConstructorMethodInvoker::LdFtnResult.get()
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.SyntheticMethodInvoker::CreateDelegate(RuntimeTypeHandle, object, bool, bool, bool)
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.SyntheticMethodInvoker::LdFtnResult.get()
[System.Private.Reflection.Execution] Internal.Reflection.Execution.MethodInvokers.VirtualMethodInvoker::LdFtnResult.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeCLSIDNullaryConstructorInfo::MethodHandle.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeCLSIDNullaryConstructorInfo::UncachedMethodInvoker.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeConstructorInfo::GetMethodBody()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeConstructorInfo::Invoke(object, BindingFlags, Binder, object[], CultureInfo)
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeMethodInfo::GetMethodBody()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeSyntheticConstructorInfo::MethodHandle.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.MethodInfos.RuntimeSyntheticMethodInfo::MethodHandle.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetField(string, BindingFlags)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetFields(BindingFlags)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetMethodImpl(string, BindingFlags, Binder, CallingConventions, Type[], ParameterModifier[])
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetMethods(BindingFlags)
[System.Private.Reflection.Core] System.ActivatorImplementation::CreateInstance(Type, BindingFlags, Binder, object[], CultureInfo, object[])
[System.Private.CoreLib] System.Type::ReflectionOnlyGetType(string, bool, bool)
[System.Private.CoreLib] System.Reflection.Assembly::ReflectionOnlyLoad(byte[])
[System.Private.CoreLib] System.Reflection.Assembly::ReflectionOnlyLoad(string)
[System.Private.CoreLib] System.Reflection.Assembly::ReflectionOnlyLoadFrom(string)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::CodeBase.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetFile(string)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetFiles(bool)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetModule(string)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetReferencedAssemblies()
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetRuntimeAssembly(AssemblyBindResult)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetSatelliteAssembly(CultureInfo)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::GetSatelliteAssembly(CultureInfo, Version)
[System.Private.Reflection.Core] System.Reflection.Runtime.Assemblies.RuntimeAssembly::LoadModule(string, byte[], byte[])
[System.Private.Reflection.Core] System.Reflection.Runtime.EventInfos.RuntimeEventInfo::GetOtherMethods(bool)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetObjectData(SerializationInfo, StreamingContext)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::GetPEKind(PortableExecutableKinds, ImageFileMachine)
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::IsResource()
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::MDStreamVersion.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.Modules.RuntimeModule::ScopeName.get()
[System.Private.Reflection.Core] System.Reflection.Runtime.TypeInfos.RuntimeTypeInfo::GetInterfaceMap(Type)
[System.Private.CoreLib] System.Reflection.StrongNameKeyPair::.ctor(string)
[System.Private.CoreLib] System.Reflection.StrongNameKeyPair::PublicKey.get()
```

# System.Runtime.InteropServices

- PIA/IDispatch not supported

```
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::.ctor(Type, string)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::AddEventHandler(object, Delegate)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::Attributes.get()
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::DeclaringType.get()
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetAddMethod(bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetCustomAttributes(bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetCustomAttributes(Type, bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetCustomAttributesData()
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetRaiseMethod(bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::GetRemoveMethod(bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::IsDefined(Type, bool)
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::Name.get()
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::ReflectedType.get()
[System.Private.Interop] System.Runtime.InteropServices.ComAwareEventInfo::RemoveEventHandler(object, Delegate)
[System.Private.Interop] System.Runtime.InteropServices.ComEventsHelper::Combine(object, Guid, int, Delegate)
[System.Private.Interop] System.Runtime.InteropServices.ComEventsHelper::Remove(object, Guid, int, Delegate)
[System.Private.Interop] System.Runtime.InteropServices.DispatchWrapper::.ctor(object)
[System.Private.Interop] System.Runtime.InteropServices.DispatchWrapper::WrappedObject.get()
```

- binding to moniker is not supported: `System.Runtime.InteropServices.Marshal::BindToMoniker(string)` (not in WACK)
- Interop feature UnmanagedType.AsAny not supported
- Interop feature UnmanagedType.CustomMarshaler not supported

```
[System.Private.Interop] System.Runtime.InteropServices.Marshal::ReadByte(object, int)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::ReadInt16(object, int)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::ReadInt32(object, int)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::ReadInt64(object, int)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::WriteByte(object, int, byte)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::WriteInt16(object, int, short)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::WriteInt32(object, int, int)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::WriteInt64(object, int, long)
```

-  `System.Runtime.InteropServices.Marshal::GetExceptionForHR(int, IntPtr)` is not supported for non-null pointers
- `System.Runtime.InteropServices.ReversePInvokeCallInterceptor::.ctor(IntPtr, object, CallingConvention, LocalVariableType[], LocalVariableType[], BaseMarshaller[], MarshallerArgumentInfo[])` throws for not supported calling conventions

## Other not supported APIs

```
[System.Private.Interop] System.Runtime.InteropServices.Marshal::ChangeWrapperHandleStrength(object, bool)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::CreateWrapperOfType(object, Type)

[System.Private.Interop] System.Runtime.InteropServices.Marshal::AreComObjectsAvailableForCleanup()
[System.Private.Interop] System.Runtime.InteropServices.Marshal::CreateAggregatedObject(IntPtr, object)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::GetComInterfaceForObject(object, Type, CustomQueryInterfaceMode)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::GetStartComSlot(Type)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::GetTypeInfoName(ITypeInfo)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::GetUniqueObjectForIUnknown(IntPtr)
[System.Private.Interop] System.Runtime.InteropServices.Marshal::GetExceptionCode()
```

# System.Diagnostics.Debugger

- `System.Diagnostics.Debugger::Launch()` is not supported

# System.Private.CoreLib

- System.Environment.Exit(int) is not supported
- System.ArgIterator is not supported (removed from NetStandard 2.0)
- Remoting not supported

```
[System.Private.CoreLib] System.MarshalByRefObject::GetLifetimeService()
[System.Private.CoreLib] System.MarshalByRefObject::InitializeLifetimeService()
```

```
[System.Private.CoreLib] System.Runtime.Loader.AssemblyLoadContext::LoadFromAssemblyPath(string)
```

- Inspecting underlying PE is not supported. IL metadata is gone after native compilation and so are tokens

```
[System.Private.CoreLib] System.ModuleHandle::GetRuntimeFieldHandleFromMetadataToken(int)
[System.Private.CoreLib] System.ModuleHandle::GetRuntimeMethodHandleFromMetadataToken(int)
[System.Private.CoreLib] System.ModuleHandle::GetRuntimeTypeHandleFromMetadataToken(int)
[System.Private.CoreLib] System.ModuleHandle::ResolveFieldHandle(int)
[System.Private.CoreLib] System.ModuleHandle::ResolveFieldHandle(int, RuntimeTypeHandle[], RuntimeTypeHandle[])
[System.Private.CoreLib] System.ModuleHandle::ResolveMethodHandle(int)
[System.Private.CoreLib] System.ModuleHandle::ResolveMethodHandle(int, RuntimeTypeHandle[], RuntimeTypeHandle[])
[System.Private.CoreLib] System.ModuleHandle::ResolveTypeHandle(int)
[System.Private.CoreLib] System.ModuleHandle::ResolveTypeHandle(int, RuntimeTypeHandle[], RuntimeTypeHandle[])
```

## Other not supported APIs

```
[System.Private.CoreLib] System.Delegate::.ctor(object, string)
[System.Private.CoreLib] System.Delegate::.ctor(Type, string)
[System.Private.CoreLib] System.MulticastDelegate::.ctor(object, string)
[System.Private.CoreLib] System.MulticastDelegate::.ctor(Type, string)
[System.Private.CoreLib] System.ModuleHandle::MDStreamVersion.get()
[System.Private.CoreLib] System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(Array, RuntimeFieldHandle)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::AddByteOffset(T, IntPtr)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::AreSame(T, T)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::As(object)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::As(TFrom)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::AsPointer(T)
[System.Private.CoreLib] System.Runtime.CompilerServices.Unsafe::SizeOf()
[System.Private.Interop] System.Runtime.InteropServices.ForwardPInvokeCallInterceptor::.ctor(IntPtr, CallingConvention, LocalVariableType[], LocalVariableType[], BaseMarshaller[], MarshallerArgumentInfo[])
[System.Private.TypeLoader] Internal.Runtime.TypeLoader.Intrinsics::AddrOf(T)
[System.Private.TypeLoader] Internal.Runtime.TypeLoader.Intrinsics::Call(IntPtr, object)
[System.Private.TypeLoader] Internal.TypeSystem.NoMetadata.RuntimeMethodDesc::HasCustomAttribute(string, string)
[System.Private.TypeLoader] Internal.TypeSystem.TypeSystemContext::GetArrayTypesCache(bool, int)
```
