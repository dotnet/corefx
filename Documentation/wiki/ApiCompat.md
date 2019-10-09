### List of behavioral changes/compat breaks and deprecated/legacy APIs

**Consider adding deprecations entries directly into https://github.com/dotnet/platform-compat/pull/24**

When editing this file, please do not delete pre-existing content. If you don't like it, make a comment, but do NOT DELETE it.
It is honor system. There are no PRs, anyone on GitHub can edit our wiki docs.

The content will serve as database for platform-compat tool

#### FAQ

Q: Why not use **Obsolete** instead?

A: Obsolete attributes causes warnings. Warnings are often treated as Errors. Errors in compiler/framework update are viewed as breaking changes by large number of our customers. Roslyn team tried that in the past and fell back to do "warning waves" you have to opt-into.
Tooling on the side has 2 pros:
1. We can be agile and change/add things as we find them with fast shipping cycle.
2. We can annotate already-shipped versions (.NET Framework 4.6.x/4.7/older, .NET Core 1.0/1.1)

# Behavioral changes across platforms (compat breaks)

### System

- Secondary AppDomains are not supported on .NET Core. API such as AppDomain.CreateDomain will throw PNSE. Consider alternative isolation mechanisms such as separate processes. For controlling assembly load, consider AssemblyLoadContext.
- Remoting is not supported on .NET Core. Consider alternative IPC mechanisms such as memory mapped files, or pipes.
- Code Access Security and SecurityTransparency are not supported on .NET Core.

There is some more information at https://docs.microsoft.com/en-us/dotnet/core/porting/libraries

---

### System.Diagnostics.Process

- ```Process.Start``` with identity is not supported on Unix platforms.
- ```Process.UseShellExecute``` may not work on all Windows versions. Unlike Desktop, it defaults to false.
- Remote processes are not supported on Unix platforms.

#### Unsupported members in .NET Core

Key:

- PNSE : PlatformNotSupportedException
- Nop : No operation

| Member | Platform | Action |
| ------ | -------------------- | ------------- |
| Process.set_MaxWorkingSet() | Linux | PNSE |
| Process.set_MinWorkingSet() | Linux | PNSE |
| Process.get_MaxWorkingSet() | OSX | PNSE for other processes |
| Process.set_MaxWorkingSet() | OSX | PNSE for other processes; for current process value cannot be decreased |
| Process.get_ProcessorAffinity() | OSX | PNSE |
| Process.set_ProcessorAffinity() | OSX | PNSE |
| Process.get_MainWindowHandle() | Unix | PNSE |
| ProcessStartInfo.get_UserName() | Unix | PNSE |
| ProcessStartInfo.set_UserName() | Unix | PNSE |
| ProcessStartInfo.get_PasswordInClearText() | Unix | PNSE |
| ProcessStartInfo.set_PasswordInClearText() | Unix | PNSE |
| ProcessStartInfo.get_Domain() | Unix | PNSE |
| ProcessStartInfo.set_Domain() | Unix | PNSE |
| ProcessStartInfo.get_LoadUserProfile() | Unix | PNSE |
| ProcessStartInfo.set_LoadUserProfile() | Unix | PNSE |
| ProcessThread.get_BasePriority() | OSX | PNSE |
| ProcessThread.set_BasePriority() | Unix | PNSE |
| ProcessThread.set_ProcessorAffinity() | Unix | PNSE |
| ProcessThread.set_IdealProcessor() | Unix | Nop |
| ProcessThread.ResetIdealProcessor() | Unix | Nop |
| ProcessThread.get_PriorityBoostEnabled() | Unix | false |
| ProcessThread.set_PriorityBoostEnabled() | Unix | Nop |

---

### System.Diagnostics.EventSource

Channel support APIs (e.g. EventData.Channel) are present (they will compile on .NET Core and .NET Native)
however they do not do anything on a non-windows system.  Channel support on windows has not been tested
on .NET Core or .NET Native platforms.  

---

### System.Codedom

- Compilation support not enabled in Codedom, throws ```PlatformNotSupportedException``` in .NET Core. See [#12180](https://github.com/dotnet/corefx/issues/12180)

---

### System.Collections.Concurrent

#### ConcurrentDictionary<T>

##### ctor(...)
* Affected platforms: Desktop
* Description: APIs on Desktop throw `NullReferenceException` - TODO better details = see [#18441](https://github.com/dotnet/corefx/pull/18441#issuecomment-294672821)

### System.Collections.NonGeneric
##### Modified Exceptions
| Member | netcore2.0 | netfx | Details |
| ------ | --------- | --------- | ---- |
| SortedList.GetValueList.CopyTo | ANE("destinationArray") | ANE("dest") | Array is null |
| SortedList.GetValueList.CopyTo | AE("array") | AE(null) | Array is multidimensional |
| SortedList.GetValueList.CopyTo | AOoRE("destinationIndex") | AOoRE("dstIndex") | negative index |
| SortedList.GetValueList.CopyTo | AE("destinationArray") | AE("") | Index + list.Count > array.Count |
| SortedList.GetKeyList.CopyTo | ANE("destinationArray") | ANE("dest") | Array is null |
| SortedList.GetKeyList.CopyTo | AE("array") | AE(null) | Array is multidimensional |
| SortedList.GetKeyList.CopyTo | AOoRE("destinationIndex") | AOoRE("dstIndex") | negative index |
| SortedList.GetKeyList.CopyTo | AE("destinationArray") | AE("") | Index + list.Count > array.Count |
| CollectionBase.CopyTo | AOoRE("destinationIndex") | AOoRE("dstIndex") | Index < 0 |
| CollectionBase.CopyTo | AE("destinationArray") | AE("") | Index + array.Length > collection.Count |
| ReadOnlyCollectionBase.CopyTo | ANE("destinationArray") | ANE("dest") | Array is null |
| ReadOnlyCollectionBase.CopyTo | AOoRE("destinationIndex") | AOoRE("dstIndex") | Index < 0 |
| ReadOnlyCollectionBase.CopyTo | AE("destinationArray") | AE("") | Index + array.Length > collection.Count |

---

### System.Collections.Immutable
##### Modified Exceptions
| Member | netcore2.0 | netfx | Details |
| ------ | --------- | --------- | ---- |
| ImmutableArray.CopyTo | ANE("destinationArray") | ANE("dest") | destinationArray is null |
| ImmutableArray.CopyTo | AOoRE("sourceIndex") | AOoRE("srcIndex") | negative sourceIndex |
| ImmutableArray.CopyTo | AOoRE("destinationIndex") | AOoRE("dstIndex") | negative destinationIndex |
| ImmutableArray.CopyTo | AE("sourceArray") | AE("") | Index + list.Count > sourceArray.Count |
| ImmutableArray.CopyTo | AE("destinationArray") | AE("") | Index + list.Count > destinationArray.Count |

---

### System.Collections.Generic
##### Modified Exceptions
| Member | netcore2.0 | netfx | Details |
| ------ | --------- | --------- | ---- |
| SortedSet<T>.GetViewBetween | AE("lowerValue") | AE(null) | lowerValue > upperValue |
| SortedSet<T>.IntersectWith | No Exception | throws IOE | Collection was modified after the Enumerator was instantiated |

---

### System.Console
##### Unsupported Members
| Member | Platform | Action |
| ------ | -------------------- | ------------- |
MoveBufferArea | AnyUnix | throws PNSE
set_BufferHeight | AnyUnix | throws PNSE
set_BufferWidth | AnyUnix | throws PNSE
set_WindowWidth | AnyUnix | throws PNSE
set_WindowHeight | AnyUnix | throws PNSE
set_WindowLeft | AnyUnix | throws PNSE
set_WindowTop | AnyUnix | throws PNSE
get_CursorVisible| AnyUnix | throws PNSE
get_Title| AnyUnix | throws PNSE
Beep| AnyUnix | throws PNSE
set_CursorSize | AnyUnix | throws PNSE
SetWindowPosition | AnyUnix | throws PNSE
SetWindowSize | AnyUnix | throws PNSE

---

### System.Globalization

#### RegionInfo

##### Name
* Affected platforms: .Net Core
* Description: When creating a RegionInfo object using a culture name (e.g. "en-US") and then requesting the region name using RegionInfo.Name, on the Desktop the returned name will be the culture name (e.g. "en-US") while on .Net Core the returned name will be the region name only (e.g. "US")

#### CultureInfo

##### GetCultures(CultureTypes types)
* Affected platforms: .Net Core on Linux and OSX
* Description: When enumerating the cultures on Linux/OSX using CultureInfo.GetCultures and passing the CultureTypes enums, only the values CultureTypes.NeutralCultures and CultureTypes.SpecificCultures will be honored and any other value will be ignored. The reason is the other CultureTypes values are Windows specific and not applicable to Linux systems.

---

### System.ServiceProcess.ServiceController

- ```System.ServiceProcess.TimeoutException``` is not a ```Serializable``` type in .NET Core. Using ```TimeoutException(SerializationInfo, StreamingContext)``` ctor() will result in ```PlatformNotSupportedException```.

---

### System.Text

#### Encoding

##### GetEncoding(int codepage), GetEncoding(String name)
* Affected platforms: .Net Core
* Description: .Net Core by default doesn't support all encodings which is supported on the Desktop. It only supports a limited set of the following encodings: Unicode, UTF8, UTF32, UTF7, ASCII, and ISO-8859-1. For the applications which need to use other encodings, the app will need to opt-in registering CodePagesEncodingProvider or other custom encoding provider which support the desired encoding.
The Remarks section in the page https://msdn.microsoft.com/en-us/library/system.text.encodingprovider(v=vs.110).aspx has more information how this can be done.

#### Encoding

##### Default
* Affected platforms: .Net Core
* Description: On the Desktop when requesting the default encoding using Encoding.Default, we read the value of the active code page on the system and then we return the matching encoding object. On .Net Core we always return UTF8 as the default encoding. The reason is, UTF8 should be the default encoding because it covers all possible code points. Also .Net Core doesn't support all encodings by default so returning encoding matches the active code page on the system will not be possible. The apps can do the following to get the same value we used to get from Encoding.Default on the desktop:
- Register the CodePagesEncodingProvider. Look at the Remarks section in the page https://msdn.microsoft.com/en-us/library/system.text.encodingprovider(v=vs.110).aspx to know how to do that.
- Issue the call Encoding.GetEncoding(0)

Note, on Linux systems we always return UTF8 even if registered the encoding provider.

---

### System.IO.Pipes

#### Unsupported Members
| Member | Unsupported on |
| ------ | -------------------- |
| System.IO.Pipes.PipeAccessRights | non-Windows |
| System.IO.Pipes.PipeAccessRule | non-Windows |
| System.IO.Pipes.PipeAuditRule | non-Windows |
| System.IO.Pipes.PipesAclExtensions | non-Windows |
| System.IO.Pipes.PipeSecurity | non-Windows |

#### Platform Differences
There are a large number of differences in behavior between Pipes on Windows and on Unix. Where possible we matched behavior but due to functional differences inherent in the platforms, some differences had to remain. Some of these manifest in the form of PNSEs while others result in functional differences. See https://github.com/dotnet/corefx/pull/1056 for details.

### System.IO

#### System.IO.UnmanagedMemoryStream.set_Position
- NetFX allows a negative Position following some PositionPointer overflowing inputs. See dotnet/coreclr#11376.On Netcoreapp we throw an AOoRE.
#### System.IO.MemoryMappedFile.CreateView/CreateFromFile
- ##### Unix/Windows differences
   - On Unix, permission errors come from CreateView*
   - On Windows, permission errors come from CreateFromFile
   - On Unix, map names are not supported
   - FileShare is limited on Unix, with None == exclusive, everything else == concurrent
   - Unix model for executable MemoryMappedFileAccess differs from Windows
- ##### Netfx/Netcoreapp differences
   - On netfx, the MMF.CreateFromFile default FileShare is None. We intentionally changed it in netcoreapp in #6092.
   - CreateFromFile in desktop uses FileStream's ctor that takes a FileSystemRights in order to specify execute privileges. This means that it can use MemoryMappedFileAccess.*Execute, whereas netcoreapp throws UnauthorizedAccessExceptions.
#### System.IO.RenamedEventArgs
- On netfx, creating a RenamedEventArgs with a given OldName or OldFullPath leads to a demand for FileIOPermissions and validity checking for that path. On netcoreapp we allow any path and do not check it or assert permissions against it.
#### System.IO.FileSystemWatcher.EnableRaisingEvents
- Desktop FSW disallows modification of the watched folder. On netcoreapp it is allowed and events against it will be properly caught
- There are a number of differences in the thrown events between Windows and Unix. This is mostly due to large differences in how the Linux, OSX, and Windows filesystem watchers interact. For example, Unix often throws a paired Created+Deleted event instead of a Renamed event. 
#### System.IO.BinaryReader.Read
- Difference in behavior that added extra checks to BinaryReader/Writer buffers on .Net Core.
#### System.IO.StreamWriter.WriteLineAsync 
- On netfx, calling WriteLineAsync with an empty string would throw a NullReferenceException. On netcoreapp we changed it to instead write a new line.
#### System.IO.IsolatedStorage
- Enumerating `IsolatedStorageFile`'s (via `IsolatedStorageFile.GetEnumerator(IsolatedStorageScope)`) does not yield any results. See #10936.
- `IsolatedStorageFile.GetUserStoreForSite()` throws `NotSupportedException` as NetFX does as it applies only to Silverlight.
- `IsolatedStorageFile.GetStore()` methods throw `PlatformNotSupportedException` when specifying identity or evidence types. `GetUserStore*` and `GetMachineStore*` methods should be used.
- Machine wide scope is not supported on UAP.
#### Modified Exceptions
| Member | netcore2.0 | netfx | Details |
| ------ | --------- | --------- | ---- |
| System.IO.MemoryMappedFile.CreateFromFile | AE("mode") | ANE(null) | FileMode.Truncate disallowed |
| System.IO.FileSystemWatcher.ctor | AE("path") | null | empty or invalid path |
#### Unsupported Members
| Member | Unsupported on |
| ------ | -------------------- |
| System.IO.FileSystemAclExtensions | non-Windows |
| System.Security.AccessControl.DirectoryObjectSecurity | non-Windows |
| System.Security.AccessControl.DirectorySecurity | non-Windows |
| System.Security.AccessControl.FileSecurity | non-Windows |
| System.Security.AccessControl.FileSystemAccessRule | non-Windows |
| System.Security.AccessControl.FileSystemAuditRule | non-Windows |
| System.Security.AccessControl.FileSystemSecurity | non-Windows |
| System.IO.IsolatedStorage.IsolatedStorage.GetPermission(System.Security.PermissionSet) | all |
| System.IO.IsolatedStorage.IsolatedStorageFile.GetPermission(System.Security.PermissionSet) | all |
| System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(System.IO.IsolatedStorage.IsolatedStorageScope, System.Security.Policy.Evidence, System.Type, System.Security.Policy.Evidence, System.Type) | all |

---

### System.Net

#### HttpWebRequest

##### Sending a POST request
* Affected platforms: UWP
* Description: HttpWebRequest has historically allowed any request header to be sent. However, in CoreFx and the UWP platform, it is built on top of System.Net.Http. And System.Net.Http has a stricter object model where it requires headers to be added only to specific collections, either regular request headers or entity-body (content) related headers (such as 'Content-Type', 'Content-Length'). Prior releases of the UWP platform prevented content-related request headers from being added to a request if the request (i.e. POST) had no actual request body. This was a different behavior than .NET Framework. The current release of the UWP platform fixes that matching the .NET Framework behavior. This means that any content-related request header can be sent even if there is no actual request body content.

Details: [issue #5565](https://github.com/dotnet/corefx/issues/5565)

---

### System.Net.Http

#### HttpClientHandler

##### AllowAutoRedirects
* Affected platforms: All
* Description: When AllowAutoRedirects is true, the implementation on the desktop will automatically follow redirects, even when redirecting from an https URI to an http URI.  For security reasons, the implementation in .NET Core does not follow such redirects, instead either failing the request or simply not following the redirection as if AllowAutoRedirects was false.

Details: [issue #19098](https://github.com/dotnet/corefx/issues/19098)

##### MaxAutomaticRedirections
* Affected platforms: .NET Core (Windows, Linux)
* Description: On .NET Framework and UWP platforms, sending a request that exceeds the maximum automatic redirections setting will result in a final 3xx HTTP response status code.  On .NET Core (Windows, Linux), exceeding the maximum will result in an HttpRequestException being thrown.

Details: [Issue #8947](https://github.com/dotnet/corefx/issues/8947)

##### MaxRequestContentBufferSize
* Affected platforms: .NET Core including UWP
* Description: This property is not supported. In the .NET Framework it was only used when the handler needed to automatically buffer the request content. That only happened if neither 'Content-Length' nor 'Transfer-Encoding: chunked' request headers were specified. So, the handler thus needed to buffer in the request content to determine its length and then would choose 'Content-Length' semantics when POST'ing. In .NET Core and UWP platforms, the handler will resolve the ambiguity by always choosing 'Transfer-Encoding: chunked'. The handler will never automatically buffer in the request content. The property will always return a value of zero in the getter. The setter is ignored.

Details: [PR #22201](https://github.com/dotnet/corefx/pull/22201)

##### MaxResponseHeadersLength
* Affected platforms: UWP
* Description: On the UWP platform, the default value for MaxResponseHeadersLength is -1. This setting means that there is no limit. Setting the property to any other value is ignored. On UWP, HttpClientHandler is built on the WinRT APIs which use a hard-code value (default value of WinInet) for MaxResponseHeadersLength.

Details: [PR #21511](https://github.com/dotnet/corefx/pull/21511)

##### PreAuthenticate

* Affected platforms: UWP
* Description: When PreAuthenticate is true, the HTTP stack will try to reuse any cached HTTP credentials (Authorization request header) for subsequent requests. The default is false. On the UWP platform, however, the default for this property is true. Setting the property to false is ignored. On UWP, HttpClientHandler is built on the WinRT
APIs which in turn are built on WinInet. And WinInet always uses authentication caching internally.

Details: [PR #21403](https://github.com/dotnet/corefx/pull/21403)

##### SendAsync (using the 'TRACE' HttpMethod)

* Affected platforms: UWP
* Description: Sending an HttpRequestMessage with the 'TRACE' method is not supported on UWP platform. Doing so will return an HttpRequestException. Please see related article: https://www.rapid7.com/db/vulnerabilities/http-trace-method-enabled

Details: [Issue #22161](https://github.com/dotnet/corefx/issues/22161)

##### UseDefaultCredentials

* Affected platforms: UWP
* Description: This property will default to false (same as .NET Framework). However, it cannot be
set to true. The setter is a no-op. The UWP platform implementation of System.Net.Http uses a different set of rules for sending default credentials. It is based on whether the app has Enterprise Authentication capability as well as if the
endpoint is specified in an intranet zone. This design is part of the base native WinInet and WinHTTP stacks. It was a design choice to provide optimum security as well as convenience for developers writing apps for enterprise scenarios.

Details: [PR #21672](https://github.com/dotnet/corefx/pull/21672)

---

### System.Net.HttpListener

#### HttpListener

* Affected platforms: UWP, Linux
* Description: HTTPS connection support for HttpListener on the UWP or Linux platforms is not currently supported. Implementing this will require new APIs/behavior to configure TLS/SSL certificates in a cross-platform manner. This
work be scheduled for a future release.

Details: [issue #14691](https://github.com/dotnet/corefx/issues/14691)

---

### System.Net.NetworkInformation

#### Ping

##### Send/SendAsync/SendPingAsync
* Affected platforms: UWP
* Description: The UWP implementation of Ping.Send* APIs throws `PlatformNotSupportedException` due to missing Windows OS support for ICMP from AppContainer processes.

Details: [issue #19583](https://github.com/dotnet/corefx/issues/19583)

---

### System.Net.Sockets

#### Socket

##### IOControlCode.OobDataRead
* Affected platforms: All
* OobDataRead wraps the operating system implementation of the IO control code SIOCATMARK, which varies significantly between Windows, Linux, and OSX. The specifics of each system are beyond the scope of this document, but are investigated in the issue linked below.

Details: [issue #25639](https://github.com/dotnet/corefx/issues/25639)

---

### System.Net.WebSockets.Client

#### ClientWebSocket

##### SendAsync
* Affected platforms: UWP
* Description: The UWP implementation of ClientWebSocket buffers the entire message before it can pass it to the underlying WinRT MessageWebSocket. This is due to missing support for sending partial messages in WinRT. This is a behavior difference from .NET Framework and .NET Core.

Details: [issue #22053](https://github.com/dotnet/corefx/issues/22053)

#### ClientWebSocketOptions

|Platform|.Net Core|Member|Supported|
|---|---|---|---|
|Windows|2.0|ClientCertificates|NO|
|Windows|2.0|Proxy|NO|
|UWP|2.0|ClientCertificates|YES|
|UWP|2.0|Proxy|NO (Default proxy always used)|
|Linux/OSX|1.1|ClientCertificates|YES|
|Linux/OSX|2.0|Proxy|NO|

Details: [issue #5120](https://github.com/dotnet/corefx/issues/5120)

---

### System.Numerics

#### Complex

##### Sqrt

* A new algorithm has been adopted for Complex.Sqrt, and several other related Complex functions. The new algorithm is more efficient, more accurate, and has better handling for numerical edge cases. This will affect the return value of these functions. In all cases, the result will be at least as accurate as the old algorithm.
  * Affected functions: Sqrt, Abs, Asin
  * Most notably, Complex.Sqrt(-1) now correctly returns Complex.ImaginaryOne, as expected.
  * In many other cases, operations previously returning NaN or Infinity will now return valid, accurate, and finite results.

Details: [PR #15338](https://github.com/dotnet/corefx/pull/15338)

##### Sin, Cos, Tan

* A new algorithm has been adopted for the Complex trigonometric functions, Sin, Cos, and Tan. The new algorithm has different behavior for numerical edge cases, but is at least as accurate as before. This change will primarily result in different return values for trig functions with extremely large input values.

Details: [PR #15835](https://github.com/dotnet/corefx/pull/15835)

---

### System.Security
#### Unsupported Members
| Member | Unsupported on |
| ------ | -------------------- |
| System.Security.AccessControl.**** | non-Windows |
| System.Security.Principal.IdentityNotMappedException | non-Windows |
| System.Security.Principal.IdentityReference | non-Windows |
| System.Security.Principal.IdentityReferenceCollection | non-Windows |
| System.Security.Principal.NTAccount | non-Windows |
| System.Security.Principal.SecurityIdentifier | non-Windows |
| System.Security.Principal.TokenAccessLevels | non-Windows |
| System.Security.Principal.WellKnownSidType | non-Windows |
| System.Security.Principal.WindowsBuiltInRole | non-Windows |
| System.Security.Principal.WindowsIdentity | non-Windows |
| System.Security.Principal.WindowsPrincipal | non-Windows |
| Microsoft.Win32.SafeHandles.SafeAccessTokenHandle | non-Windows |

---

### System.Security.Cryptography

#### HMAC

The HMAC base class in .NET Core does not provide the HMAC algorithm to derived types, as all HMAC operations in .NET Core are performed by system libraries.  Initialize, HashCore, and HashFinal all throw PlatformNotSupportedException.

HMAC.HashName can only be set to the current value in .NET Core, otherwise a PlatformNotSupportedException is thrown.

#### SignedXml is missing constants defining SHA2 related algorithms
* Affected platforms: all except Desktop >= 4.6.2 and nuget packages released before mentioned change.
* Workaround: Use hard-coded specification strings
* Motivation: Members were removed in order to make System.Security.Cryptography.Xml work on netstandard. Constants must be backported on Desktop in order to add them back.
* Related changes: https://github.com/dotnet/corefx/pull/19189

#### SignedXml will not be able to find public key for DSA certificates
* Affected platforms: all except Desktop
* Workaround: Find key for DSA certificate manually and use key directly
* Motivation: Members were removed in order to make System.Security.Crytography.Xml work on netstandard.
* Affected APIs:
    - `bool SignedXml.CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)`
    - (protected) `virtual SignedXml.AsymmetricAlgorithm GetPublicKey()`
    - `bool SignedXml.CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)`
* Related changes: https://github.com/dotnet/corefx/pull/19189

#### `ProtectedData`
* Affected platforms: Non-Windows
* Description: Throws `PlatformNotSupportedException` if not on Windows (https://github.com/dotnet/corefx/issues/6746)

#### `X509Certificates` on macOS
* In .NET Core 1.x certificates were backed by OpenSSL, now they are backed by Security.framework
  * X509Certificate2.Handle is now a SecIdentityRef or SecCertificateRef value, as appropriate.
  * new X509Certificate2(IntPtr) will only work for a SecIdentityRef or SecCertificateRef value, not an OpenSSL X509\*.
* X509Store is now driven by Keychain services
  * Data previously added to an X509Store will not be discovered X509Store any longer.
  * Custom stores can not be created, because Keychain creation requires data not available in the API surface.
* X509Certificate2.PrivateKey
  * On .NET Framework this returns instances of RSACryptoServiceProvider or DSACryptoServiceProvider, and no other types.
  * On .NET Core this returns the value of cert.GetRSAPrivateKey() or cert.GetDSAPrivateKey(), which are not tightly constrained in their specific return type.

#### `AsymmetricAlgorithm.FromXmlString` (or `ToXmlString`)
* In .NET Core these methods are not implemented due to a cycle in dependencies between XML, HTTP, and cryptography.

#### `[Algorithm].Create()`
* In .NET Framework the `Create()` factories for cryptographic algorithms (`Aes.Create()`, `RSA.Create()`, et cetera) produce instances of public types (e.g. `RSACryptoServiceProvider`), and the returned type can be controlled via CryptoConfig.
* In .NET Core the factory methods produce instances of private types, and the returned type cannot be controlled.

#### `[Algorithm].Create(string)`
* `AsymmetricAlgorithm`, `HashAlgorithm`, `HMAC`, `KeyedHashAlgorithm`, `SymmetricAlgorithm` all have Create(string) methods that throw `PlatformNotSupportedException`s on netcoreapp.
* To work around the PNSEs, replace `[Algorithm].Create(string)` with `([Algorithm])CryptoConfig.CreateFromName(string)`
* See https://github.com/dotnet/corefx/issues/22626

#### Cryptographic algorithm instances with "Managed" in their name
* Types such as SHA1Managed are written in managed code in .NET Framework. In .NET Core the types exist only for API compatibility, and they use the same system library that the `[Algorithm].Create()` instance would use.
  * Since SHA1Managed (et al) wraps an instance of `SHA1.Create()` (et al) it is marginally slower than using the factory.
  * Performance characteristics may differ between .NET Framework and .NET Core.

#### Cryptographic algorithm instances with "CryptoServiceProvider" in their name
* The "CryptoServiceProvider" suffix indicates types that are backed by the legacy Windows Cryptographic API (CAPI).
* On Windows these types behave as their .NET Framework equivalent types behave.
* On non-Windows OSes the types exist for compatibility, but as wrappers for (e.g.) `RSA.Create()`.
  * Members which are specific to Windows CAPI, such as `ICspSymmetricAlgorithm.get_CspKeyContainerInfo()`, will throw a `PlatformNotSupportedException`.

#### `Oid.FromFriendlyName(string)`, `new Oid(string).FriendlyName`, `Oid.FromValue(string).FriendlyName`
* "Friendly Name" values for OIDs are provided by the system cryptographic libraries, with the exception of a small set of hard-coded lookups within .NET Core.
  * In general FriendlyName should only be used to display data, and never as a programmatic decision.
  * .NET Core on Windows will behave as .NET Framework behaved.
    * Caution: Many OID Friendly Name values are localized on Windows.
  * .NET Core on Linux uses the OpenSSL "long name" string for a given object ID.
  * .NET Core on macOS only has the hard-coded lookup table, resulting in significantly more exceptions from `Oid.FromFriendlyName(string)`.

#### `System.Security.Cryptography.Rijndael`, `System.Security.Cryptography.RijndaelManaged`
* In .NET Framework the `Rijndael` class represents the Rijndael algorithm, which permits `BlockSize` values other than 128-bit.
* In .NET Core the `Rijndael` class exists only for compatibility. The `RijndaelManaged` class uses the subset of the Rijndael algorithm which is standardized as AES.

#### `System.Security.Cryptography.RSAOAEPKeyExchangeFormatter`, `System.Security.Cryptography.RSAOAEPKeyExchangeDeformatter`
* The .NET Framework version of these classes has a legacy fallback which may present pre-padded data to the `RSA.EncryptValue(byte[])` method, permitting interoperability to custom RSA types implemented before .NET 4.6 added the `RSA.Encrypt(byte[], RSAEncryptionPadding)` method.
* .NET Core only uses the `RSA.Encrypt(byte[], RSAEncryptionPadding)` method.

---

### System.Threading

#### Unsupported members

| Member | Runtimes | Platforms |
| - | - | - |
| Thread.Abort() | .NET Core | All |
| Thread.Abort(object) | .NET Core | All |
| Thread.ResetAbort() | .NET Core | All |
| Thread.Resume | .NET Core | All |
| Thread.Suspend | .NET Core | All |

#### ExecutionContext

##### .NET Framework -> .NET Core

###### `SynchronizationContext` is not captured or applied
* `SynchronizationContext` is no longer part of the `ExecutionContext`. `Run` does not apply the synchronization context that was current at the point of the corresponding `ExecutionContext.Capture`.
* See https://blogs.msdn.microsoft.com/pfxteam/2012/06/15/executioncontext-vs-synchronizationcontext/ under the section titled "Isn’t SynchronizationContext part of ExecutionContext?" for some more information and some of the reasoning behind this change.

###### Changing an `AsyncLocal` instance's value changes the current `ExecutionContext`
* .NET Framework keeps `ExecutionContext` instances mutable, and generally tries to create a new instance on Capture, such that captured instances don’t change. .NET Core on the other hand, keeps EC instances immutable and does not need to copy on Capture. So as a result, multiple Captures may share the same EC instance.
* This transfers the negative performance impact from allocation from the frequent operations `Capture` and `Run`, to less frequent operations, including changing an `AsyncLocal`'s value among

###### `Run` does not throw in some cases based on the execution context
* .NET Framework does not allow `Run` for an execution context acquired from `Thread.CurrentThread.ExecutionContext`. It is recommended to use an instance provided by `ExecutionContext.Capture` instead.

###### `Run` can be called multiple times with the same execution context
* .NET Framework only allows one call to `Run` for an instance of `ExecutionContext` obtained from `ExecutionContext.Capture`. .NET Core allows multiple calls.

###### `CreateCopy` does not throw in some cases based on the execution context
* .NET Framework does not allow `CreateCopy` on an execution context that has been passed into `Run`, or for an execution context obtained from `Thread.CurrentThread.ExecutionContext`. .NET Core allows these.

###### `AsyncFlowControl.Undo` allows restoring flow on a different `ExecutionContext` instance
* `AsyncFlowControl.Undo` allows restoring flow on a different `ExecutionContext` instance than the one that was current at the time of the corresponding `ExecutionContext.SuppressFlow`. `AsyncFlowControl` is disposable and may be used with a `using` block in C#.

###### `AsyncFlowControl` equality does not behave equivalently in all scenarios
* The contents of `AsyncFlowControl` have changed, and two instances may not compare equal similarly to how they compared before. `AsyncFlowControl` is a struct for performance reasons and its contents are implementation-defined.

#### `ReaderWriterLock`

##### .NET Framework -> .NET Core

###### `Int32` timeout parameter values cannot be less than `-1`
* .NET Framework treats the `Int32` timeout parameter values as unsigned, where `-1` is considered an infinite timeout. .NET Core throws `ArgumentOutOfRangeException` for values less than `-1`.

#### `CancellationTokenSource`

##### .NET Framework -> .NET Core

###### The `AppContext` switch "Switch.System.Threading.ThrowExceptionIfDisposedCancellationTokenSource" has no effect
* .NET Core behaves as though that switch is set to false, and there is no way to override this behavior. As one example, `CancellationToken.Register` does not throw after the token source is disposed.

#### `Thread`

##### .NET Framework -> .NET Core

###### `CurrentCulture`, `CurrentUICulture` sometimes throw `InvalidOperationException`
* .NET Core requires that these properties are accessed from the thread corresponding to the thread instance. If a thread tries to read or write these properties on a thread instance corresponding to a different thread, `InvalidOperationException` is thrown. Consider using `CultureInfo.CurrentCulture` and `CultureInfo.CurrentUICulture` instead where appropriate.

###### `Interrupt` may interrupt a wait in a `finally` block
* .NET Framework does not interrupt a thread while it is inside a `finally` block, .NET Core does.

---

### System.Transactions

#### Distributed transactions are not supported in .NET Core
* Affected platforms: .NET Core
* Description: Only local transaction is supported on .NET Core for now.
Details: issue #16755

---

### System.Xml

#### XmlAttributeCollection

##### InsertAfter
* Affected platforms: .Net Core
* Description: When the reference node is the last in the collection and it's a duplicate of the new node, InsertAfter is unable to handle the case on Desktop and throws `System.ArgumentOutOfRangeException` further in List<T>.Insert() due to an incorrectly calculated insert position. To be consistent with what InsertBefore does, the duplicate node (attribute) needed to be removed before inserting. This change was made in Core.

##### SetNamedItem
* Affected platforms: .Net Core
* Description: Desktop does not check for null argument and throws `System.NullReferenceException`. Core returns null instead.

#### XmlNamedNodeMap

##### SetNamedItem
* Affected platforms: .Net Core
* Description: Desktop does not check for null argument and throws `System.NullReferenceException`. Core returns null instead.

---

### System.Xml.Schema

#### XmlSchemaSet

##### Add
* Affected platforms: .NET Core
* Description: If the schema being added imports another schema through external URI, Core does not allow resolving that URI by default while Desktop does. To allow the resolution on Core, the following needs to be called before adding the schema:
`AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);`

---

### System.Xml.Xsl

#### XslCompiledTransform

##### Load
* Affected platforms: .Net Core
* Description: Due to unavailability of some System.CodeDom APIs XslCompiledTransform.Load throws `PlatformNotSupportedException` if `msxsl:script` is used in the stylesheet on .NET Core.

#### XslTransform

##### Load
* Affected platforms: .Net Core
* Description: Due to unavailability of some System.CodeDom APIs XslTransform.Load throws `PlatformNotSupportedException` if `msxsl:script` is used in the stylesheet on .NET Core.

# Obsolete/Deprecated APIs

Want to add something new? Please copy and paste this section, or follow steps in https://github.com/dotnet/platform-compat/pull/24:

```Markdown
### <Description of the API>

#### Motivation

*Placeholder: Why should the API be deprecated?*

#### Replacement

*Placeholder: What are the placements?*
```

### System.Xml.Serialization

#### Issue with Soap Encoded Message Serialization (https://github.com/dotnet/corefx/issues/18964)

If an application is built on .Net Standard2.0 and uses Soap encoded serialization, the application needs to be built against .Net Framework before it can run on .Net Framework because of https://github.com/dotnet/corefx/issues/18964.

### System.Runtime.Serialization

#### DataContractSerializer/DataContractJsonSerializer serializes Exceptions differently between .Net Core 1.1 and .Net Core 2.0

Exception types implements ISerializable interface on .Net Framework. As ISerializable was not available in .Net Core 1.1, DataContractSerializer dealt with Exception types specially to support Exception serialization. ISerializable is now available in .Net Core 2.0. As a result, DataContractSerializer serializes Exceptions as normal ISerializable types. The serialization payloads of Exceptions are different between .Net Core 1.1 and .Net Core 2.0.

### System.Net.WebSockets - RegisterPrefixes and CreateClientWebSocket

#### Motivation

The APIs are documented already on MSDN as internal, not for external/direct consumption -- [`WebSocket.RegisterPrefixes`](https://msdn.microsoft.com/en-us/library/system.net.websockets.websocket.registerprefixes(v=vs.110).aspx) and [`WebSocket.CreateClientWebSocket`](https://msdn.microsoft.com/en-us/library/system.net.websockets.websocket.createclientwebsocket(v=vs.110).aspx).

Details: [issue #18905](https://github.com/dotnet/corefx/issues/18905)

#### Replacement

N/A

### `System.Security.Cryptography.AsymmetricAlgorithm.Create()`

#### Motivation

Callers may expect this method to provide them with a good choice of asymmetric algorithm, but it always returns a default-keysize `RSA` object.  While RSA can be used for both asymmetric encryption and asymmetric digital signature, it cannot be used for asymmetric key agreement (no supported algorithm supports all three).  The resulting `AsymmetricAlgorithm` object would have to be cast to a specific algorithm type before being used.

#### Replacement

Choose an algorithm based upon your needs, and call that algorithm-specific `Create()` method:

* Encryption: `System.Security.Cryptography.RSA.Create()`
* Digital Signature: `System.Security.Cryptography.RSA.Create()`, `System.Security.Cryptography.ECDsa.Create()`, or `System.Security.Cryptography.DSA.Create()`
* Key Agreement: `System.Security.Cryptography.ECDiffieHellman.Create()` (.NET Framework only)

### `System.Security.Cryptography.AsymmetricAlgorithm.Create(string)`

#### Motivation

The AsymmetricAlgorithm class does not provide sufficient members to perform any asymmetric operations, so callers must always cast the return type anyways.

On .NET Core this method throws `PlatformNotSupportedException`.

#### Replacement

The most direct replacement is `(AsymmetricAlgorithm)CryptoConfig.CreateFromName(algName)`, but that has the same problems.

The recommended replacement is to identify what algorithm you are expecting and make use of the type-specific methods:

* RSA: `System.Security.Cryptography.RSA.Create()` or `System.Security.Cryptography.RSA.Create(string)`
* ECDSA: `System.Security.Cryptography.ECDsa.Create()`, or `System.Security.Cryptography.ECDsa.Create(string)`
* DSA: `System.Security.Cryptography.DSA.Create()` or `System.Security.Cryptography.DSA.Create(string)`
* EC Diffie-Hellman: `System.Security.Cryptography.ECDiffieHellman.Create()` (.NET Framework only) or `System.Security.Cryptography.ECDiffieHellman.Create(string)` (.NET Framework only)

### `System.Security.Cryptography.AsymmetricAlgorithm.KeyExchangeAlgorithm`

#### Motivation

This `string` value has inconsistent behavior across the algorithm implementations.

#### Replacement

* `KeyExchangeAlgorithm` only makes sense for RSA, so one easy replacement is the [pattern matching expression](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7#pattern-matching) `if (key is RSA rsa)`.
* For an RSA algorithm instance the KeyExchangeAlgorithm was a coarse proxy for testing if the key had encryption enabled. The correct replacement is a `try { encrypt/decrypt } catch (CryptographicException) { }` statement.

### `System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm`

#### Motivation

This `string` value has inconsistent behavior across the algorithm implementations.

#### Replacement

A type check (`is`, `as`, or pattern-is-assignment).

### `System.Security.Cryptography.HashAlgorithm.Create()`

#### Motivation

Callers may expect this method to provide them with a good choice of hash/digest algorithm, but it always returns SHA-1 (for compatibility reasons).

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and call that algorithm-specific `Create()` method. For example, `System.Security.Cryptography.SHA256.Create()`.

### `System.Security.Cryptography.HashAlgorithm.Create(string)`

#### Motivation

The algorithm chosen may require options to be set using members not available on the `KeyedHashAlgorithm` class, making it possible that casting would be required before safe utilization of the cryptographic object.  For example, `KeyedHashAlgorithm` derives from `HashAlgorithm`, so all HMAC types can be returned from this method.

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `System.Security.Cryptography.SHA256.Create()`.

### `System.Security.Cryptography.KeyedHashAlgorithm.Create()`

#### Motivation

Callers may expect this method to provide them with a good choice of a keyed hash/digest algorithm, but it always returns HMAC-SHA-1 (for compatibility reasons).  Even if that were not the case, the specific chosen algorithm could have options not available at the base class level, making it possible that casting would be required before safe utilization of the cryptographic object.

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `new System.Security.Cryptography.HMACSHA256()` or `new System.Security.Cryptography.HMACSHA256(byte[])`.

### `System.Security.Cryptography.KeyedHashAlgorithm.Create(string)`

#### Motivation

The algorithm chosen may require options to be set using members not available on the `KeyedHashAlgorithm` class, making it possible that casting would be required before safe utilization of the cryptographic object.

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `new System.Security.Cryptography.HMACSHA256()` or `new System.Security.Cryptography.HMACSHA256(byte[])`.

### `System.Security.Cryptography.HMAC.Create()`

#### Motivation

Callers may expect this method to provide them with a good choice of a HMAC algorithm, but it always returns HMAC-SHA-1 (for compatibility reasons).

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `new System.Security.Cryptography.HMACSHA256()` or `new System.Security.Cryptography.HMACSHA256(byte[])`.

### `System.Security.Cryptography.HMAC.Create(string)`

#### Motivation

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `new System.Security.Cryptography.HMACSHA256()` or `new System.Security.Cryptography.HMACSHA256(byte[])`.

### `System.Security.Cryptography.SymmetricAlgorithm.Create()`

#### Motivation

Callers may expect this method to provide them with a good choice of a keyed hash/digest algorithm, but it always returns Rijndael (for compatibility reasons).  Even if that were not the case, the specific chosen algorithm could have options not available at the base class level, making it possible that casting would be required before safe utilization of the cryptographic object.

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `System.Security.Cryptography.Aes.Create()`.

### `System.Security.Cryptography.SymmetricAlgorithm.Create(string)`

#### Motivation

The algorithm chosen may require options to be set using members not available on the `SymmetricAlgorithm` class, making it possible that casting would be required before safe utilization of the cryptographic object.

This method throws `PlatformNotSupportedException` in .NET Core.

#### Replacement

Choose an algorithm based upon your needs, and instantiate the algorithm appropriately.  For example, `System.Security.Cryptography.Aes.Create()`.

### `System.Security.Cryptography.AesManaged`

#### Motivation

* The AesManaged implementation on .NET Core is not implemented in managed code, it defers to Aes.Create().
* On .NET Framework the AesManaged class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`Aes.Create()`

### System.Security.Cryptography.DES

#### Motivation

DES (Data Encryption Standard) is a weak algorithm due to its small keyspace (56 bits) and small block size (64 bits). DES should not be used for directly encrypting data, but may be appropriate in specialized circumstances:

* Decrypting data already encrypted with DES.
* Used where required as part of a specification
* Used as a primitive in the construction of a larger algorithm, such as DES-EDE, aka 3DES, aka TripleDES.

#### Replacement

System.Security.Cryptography.Aes is a representation of the AES (Advanced Encryption Standard) algorithm, the replacement recommended by [NIST SP 800-57, Part 1, Revision 4](http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-57pt1r4.pdf).

### `System.Security.Cryptography.RC2`

#### Motivation

RC2 is an old algorithm which should not be used for encrypting new data.

#### Replacement

System.Security.Cryptography.Aes is a representation of the AES (Advanced Encryption Standard) algorithm, the replacement recommended by [NIST SP 800-57, Part 1, Revision 4](http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-57pt1r4.pdf).

### `System.Security.Cryptography.Rijndael`

#### Motivation

The AES algorithm is the Rijndael algorithm with a BlockSize value fixed at 128-bit. Few platforms support Rijndael, but most support AES.

The .NET Core implementation of Rijndael is functionally limited to the functionality of AES.

#### Replacement

`System.Security.Cryptography.Aes`

### `System.Security.Cryptography.RijndaelManaged`

#### Motivation

* The RijndaelManaged implementation on .NET Core is not implemented in managed code, it defers to Rijndael.Create().
* The RijndaelManaged implementation on .NET Core is functionally limited to the AES behaviors.
* On .NET Framework the RijndaelManaged class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`System.Security.Cryptography.Aes.Create()`.  (On .NET Framework, if non-AES Rijndael is required, `System.Security.Cryptography.Rijndael.Create()`)

### `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(byte[], byte[], int)`

#### Motivation

This constructor overload chooses SHA-1 as the basis for the HMAC, hiding the algorithm choice.

#### Replacement

.NET Core users should change to `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(byte[], byte[], int, HashAlgorithmName)`, using HashAlgorithmName.SHA1 when needed for compatibility.

The new overload is not available in .NET Framework at this time.

### `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, byte[])`

#### Motivation

This constructor overload chooses SHA-1 as the basis for the HMAC, hiding the algorithm choice; it also chooses an iteration count of 1000, which is considered too low by modern standards.

#### Replacement

.NET Core users should change to `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, byte[], int, HashAlgorithmName)`, using HashAlgorithmName.SHA1 and 1000 iterations when needed for compatibility.

While the HashAlgorithmName overload is not available to current .NET Framework versions, the `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, byte[], int)` overload should be preferred.

### `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, int)`

#### Motivation

This constructor overload chooses SHA-1 as the basis for the HMAC, hiding the algorithm choice.  It also chooses 1000 for the iteration count, a value considered too low by modern standards.

#### Replacement

.NET Core users should change to `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, int, int, HashAlgorithmName)`, using HashAlgorithmName.SHA1 and 1000 iterations when needed for compatibility.

While the HashAlgorithmName overload is not available to current .NET Framework versions, the `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, int, int)` overload should be preferred.

### `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, int, int)`

#### Motivation

This constructor overload chooses SHA-1 as the basis for the HMAC, hiding the algorithm choice.

#### Replacement

.NET Core users should change to `System.Security.Cryptography.Rfc2898DeriveBytes..ctor(string, int, int, HashAlgorithmName)`, using HashAlgorithmName.SHA1 when needed for compatibility.

The new overload is not available in .NET Framework at this time.

### `System.Security.Cryptography.RSA.DecryptValue(byte[])`

#### Motivation

This method has never been implemented by a subclass of RSA which was part of .NET Core or the .NET Framework, and it has no officially defined behavior.

#### Replacement

`System.Security.Cryptography.RSA.Decrypt(byte[], RSAEncryptionPadding)`

### `System.Security.Cryptography.RSA.EncryptValue(byte[])`

#### Motivation

This method has never been implemented by a subclass of RSA which was part of .NET Core or the .NET Framework, and it has no officially defined behavior.

#### Replacement

`System.Security.Cryptography.RSA.Encrypt(byte[], RSAEncryptionPadding)`

### `System.Security.Cryptography.SHA1Managed`

#### Motivation

* The SHA1Managed implementation on .NET Core is not implemented in managed code, it defers to SHA1.Create().
* On .NET Framework the SHA1Managed class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`System.Security.Cryptography.SHA1.Create()`

### `System.Security.Cryptography.SHA256Managed`

#### Motivation

* The SHA256Managed implementation on .NET Core is not implemented in managed code, it defers to SHA256.Create().
* On .NET Framework the SHA256Managed class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`System.Security.Cryptography.SHA256.Create()`

### `System.Security.Cryptography.SHA384Managed`

#### Motivation

* The SHA384Managed implementation on .NET Core is not implemented in managed code, it defers to SHA384.Create().
* On .NET Framework the SHA384Managed class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`System.Security.Cryptography.SHA384.Create()`

### `System.Security.Cryptography.SHA512Managed`

#### Motivation

* The SHA512Managed implementation on .NET Core is not implemented in managed code, it defers to SHA512.Create().
* On .NET Framework the SHA512Managed class cannot be instantiated on computers with FIPS policy enforcement enabled.

#### Replacement

`System.Security.Cryptography.SHA512.Create()`

### `System.Security.Cryptography.AesCryptoServiceProvider`

#### Motivation

* On .NET Framework AesCryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than AesCng.
* On .NET Core AesCryptoServiceProvider is written as a wrapper over Aes.Create(), making this type marginally slower than calling Aes.Create() directly.

#### Replacement

`System.Security.Cryptography.Aes.Create()`

### `System.Security.Cryptography.DESCryptoServiceProvider`

#### Motivation

* DES (Data Encryption Standard) is a weak algorithm.
* On .NET Core DESCryptoServiceProvider is written as a wrapper over DES.Create(), making this type marginally slower than calling DES.Create() directly.

#### Replacement

When DES is required, `System.Security.Cryptography.DES.Create()`.

Otherwise, `System.Security.Cryptography.Aes` is a representation of the AES (Advanced Encryption Standard) algorithm, the replacement recommended by [NIST SP 800-57, Part 1, Revision 4](http://nvlpubs.nist.gov/nistpubs/SpecialPublications/NIST.SP.800-57pt1r4.pdf). 

### `System.Security.Cryptography.DSACryptoServiceProvider`

#### Motivation

* DSACryptoServiceProvider only supports FIPS 186-2 DSA, which has a keysize limitation of 1024-bit and SHA-1 as the only choice for digest algorithm.
* On .NET Core on non-Windows systems DSACryptoServiceProvider is written as a wrapper over DSA.Create(), making this type marginally slower than calling DSA.Create() directly, and CAPI-specific members will throw exceptions.

#### Replacement

* If direct interop is required to Windows CAPI, no replacement exists.
* If DSA is required, use `System.Security.Cryptography.DSA.Create()`.
* If the algorithm choice is flexible, consider changing to RSA (`System.Security.Cryptography.RSA.Create()`) or Elliptic-Curve DSA (ECDSA) (`System.Security.Cryptography.ECDsa.Create()`).

### `System.Security.Cryptography.MD5CryptoServiceProvider`

#### Motivation

On .NET Core MD5CryptoServiceProvider is written as a wrapper over MD5.Create(), making this type marginally slower than calling MD5.Create() directly.

#### Replacement

`System.Security.Cryptography.MD5.Create()`

### `System.Security.Cryptography.RC2CryptoServiceProvider`

#### Motivation

On .NET Core RC2CryptoServiceProvider is written as a wrapper over RC2.Create(), making this type marginally slower than calling RC2.Create() directly.

#### Replacement

`System.Security.Cryptography.RC2.Create()`

### `System.Security.Cryptography.RNGCryptoServiceProvider`

#### Motivation

* On .NET Framework RNGCryptoServiceProvider is the implementation returned by RandomNumberGenerator.Create()
* On .NET Core RNGCryptoServiceProvider is written as a wrapper over RandomNumberGenerator.Create(), making this type marginally slower than calling RandomNumberGenerator.Create() directly.

#### Replacement

`System.Security.Cryptography.RandomNumberGenerator.Create()`

### `System.Security.Cryptography.RSACryptoServiceProvider`

#### Motivation

* RSACryptoServiceProvider does not have support for OAEP encryption or decryption with SHA-2 (SHA256, SHA384, SHA512) as the choice of hash algorithm.
* RSACryptoServiceProvider does not have support for RSA-PSS signature generation or validation.
* On .NET Core on non-Windows systems RSACryptoServiceProvider is written as a wrapper over RSA.Create(), making this type marginally slower than calling RSA.Create() directly, and CAPI-specific members will throw exceptions.

#### Replacement

* If direct interop is required to Windows CAPI, no replacement exists.
* Otherwise, `System.Security.Cryptography.RSA.Create()`
  * On .NET Framework `RSA.Create()` returns an instance of RSACryptoServiceProvider. This behavior can be changed via CryptoConfig, or callers may need to cross-compile and instantiate `System.Security.Cryptography.RSACng` directly.

### `System.Security.Cryptography.SHA1CryptoServiceProvider`

#### Motivation

* On .NET Framework SHA1CryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than SHA1Cng.
* On .NET Core SHA1CryptoServiceProvider is written as a wrapper over SHA1.Create(), making this type marginally slower than calling SHA1.Create() directly.

#### Replacement

`System.Security.Cryptography.SHA1.Create()`

### `System.Security.Cryptography.SHA256CryptoServiceProvider`

#### Motivation

* On .NET Framework SHA256CryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than SHA256Cng.
* On .NET Core SHA256CryptoServiceProvider is written as a wrapper over SHA256.Create(), making this type marginally slower than calling SHA256.Create() directly.

#### Replacement

`System.Security.Cryptography.SHA256.Create()` (as of .NET Framework 4.6.2, SHA256.Create() no longer throws when FIPS policy enforcement is enabled)

### `System.Security.Cryptography.SHA384CryptoServiceProvider`

#### Motivation

* On .NET Framework SHA384CryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than SHA384Cng.
* On .NET Core SHA384CryptoServiceProvider is written as a wrapper over SHA384.Create(), making this type marginally slower than calling SHA384.Create() directly.

#### Replacement

`System.Security.Cryptography.SHA384.Create()`  (as of .NET Framework 4.6.2, SHA384.Create() no longer throws when FIPS policy enforcement is enabled)

### `System.Security.Cryptography.SHA512CryptoServiceProvider`

#### Motivation

* On .NET Framework SHA512CryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than SHA512Cng.
* On .NET Core SHA512CryptoServiceProvider is written as a wrapper over SHA512.Create(), making this type marginally slower than calling SHA512.Create() directly.

#### Replacement

`System.Security.Cryptography.SHA512.Create()`  (as of .NET Framework 4.6.2, SHA512.Create() no longer throws when FIPS policy enforcement is enabled)

### `System.Security.Cryptography.TripleDESCryptoServiceProvider`

#### Motivation

* On .NET Framework TripleDESCryptoServiceProvider uses the legacy Windows Cryptographic API (API). In Windows 8.1 and newer the CAPI implementation calls the CNG implementation, making this type automatically marginally slower than TripleDESCng.
* On .NET Core TripleDESCryptoServiceProvider is written as a wrapper over TripleDES.Create(), making this type marginally slower than calling TripleDES.Create() directly.

#### Replacement

`System.Security.Cryptography.TripleDES.Create()`

### `System.Security.Cryptography.X509Certificates.PublicKey.Key`

#### Motivation

* This property returns the same object on each call.  Since the returned value is `IDisposable`, but the containing class is not, it is difficult to properly manage the object lifetime.
* This property is limited to the RSA and DSA algorithms, it has no support for ECDSA.
* On .NET Framework this property returns instances of RSACryptoServiceProvider and DSACryptoServiceProvider, which are no longer recommended for use.
* On .NET Core the returned objects may not be of the same type as .NET Framework, leading to exceptions when the caller performs a hard-cast.

#### Replacement

Usually this property is only invoked in the context of a certificate, e.g. `cert.PublicKey.Key`. In those cases:

* RSA: `cert.GetRSAPublicKey()`
* DSA: `cert.GetDSAPublicKey()`
* ECDSA: `cert.GetECDsaPublicKey()`

### `System.Security.Cryptography.X509Certificates.X509Certificate..ctor` (all overloads)

#### Motivation

The X509Certificate2 class provides more functionality than the X509Certificate class and has essentially no extra cost to instantiation.

#### Replacement

The equivalent signature overload of the `System.Security.Cryptography.X509Certificate2` constructor.

### `System.Security.Cryptography.X509Certificates.X509Certificate.Import` (all overloads)

#### Motivation

* Most users of X509Certificate and X509Certificate2 objects assume that the object is immutable except for the `Reset()`/`Dispose()` methods, the use of `Import` violates this assumption.
* These methods are not implemented in .NET Core.

#### Replacement

The equivalent signature overload of the `System.Security.Cryptography.X509Certificate2` constructor.

### `System.Security.Cryptography.X509Certificates.X509Certificate2.set_FriendlyName`

#### Motivation

This method is only available on Windows (other operating systems result in a `PlatformNotSupportedException`). When used on an X509Certificate2 instance whose underlying PCCERT_CONTEXT belongs to a persisted certificate store, assigning this property changes the value persisted within the store, affecting future copies.

#### Replacement

If `FriendlyName` is being used as a means of temporarily identifying a certificate instance, use an application-local construction, such as a `Dictionary<string, X509Certificate2>`.

If `FriendlyName` is being used for the purpose of a side effect to the persisted Windows certificate store, continue using it, but be aware of the platform-specific limitations.

### `System.Security.Cryptography.X509Certificates.get_PrivateKey`

#### Motivation

* This property returns the same object on each call.  Since the returned value is `IDisposable`, but the containing class is not, it is difficult to properly manage the object lifetime.
* This property is limited to the RSA and DSA algorithms, it has no support for ECDSA.
* On .NET Framework this property returns instances of RSACryptoServiceProvider and DSACryptoServiceProvider, which are no longer recommended for use.
  * The returned RSACryptoServiceProvider will use whatever CSP it had at the time of import (or creation). Frequently this is PROV_RSA_FULL, which is not capable of SHA-2 based signature generation or verification.
  * RSACryptoServiceProvider and DSACryptoServiceProvider are unable to access keys stored within a CNG Key Storage Provider.
* On .NET Core the returned objects may not be of the same type as .NET Framework, leading to exceptions when the caller performs a hard-cast.

#### Replacement

* RSA: `cert.GetRSAPrivateKey()`
* DSA: `cert.GetDSAPrivateKey()`
* ECDSA: `cert.GetECDsaPrivateKey()`

### `System.Security.Cryptography.X509Certificates.X509Certificate2.set_FriendlyName`

#### Motivation

This property setter is not available in .NET Core (invoking it results in a `PlatformNotSupportedException`).

On .NET Framework, when used on an X509Certificate2 instance whose underlying PCCERT_CONTEXT belongs to a persisted certificate store, assigning this property changes the value persisted within the store, affecting future copies.

Utilizing the property setter may violate assumptions other users of this object may have about object immutability.

#### Replacement

The new methods `cert.CopyWithPrivateKey(RSA)`, `cert.CopyWithPrivateKey(DSA)`, and `cert.CopyWithPrivateKey(ECDsa)` will produce a new X509Certificate2 object with the private key associated.

If the persisted store side effect of this property is desired:
* Ensure that the private key reference is a persisted key
  * `if (key is ICspAsymmetricAlgorithm capiKey) check that capiKey.CspKeyContainerInfo.KeyContainerName is not null or empty`
  * `if (key is RSACng rsaCng) check that rsaCng.Key.KeyName is not null or empty`
    * (similar for DSACng and ECDsaCng)
* Use the appropriate `CopyWithPrivateKey` (extension) method.
* Open the appropriate `X509Store` with `OpenFlags.ReadWrite`
* Add the new certificate to the store, this will replace the existing store certificate with the new one.
  * Other data such as FriendlyName, Archived, and other certificate properties may be lost.

Alternatively, P/Invoke `CertSetCertificateContextProperty` appropriately, using `cert.Handle` as the value for `pCertContext`.

### `System.Security.Cryptography.X509Certificate2.Verify()`

#### Motivation

The `Verify` method simply returns the result of `X509Chain.Build(cert)` (revocation-enabled, with DateTime.Now as the effective time).  If `Verify` returns `false` many callers call X509Chain.Build to determine the source of the error, which leads to duplicated work.

#### Replacement

```c#
using (var chain = new X509Chain())
{
    bool verified = chain.Build(cert);

    if (!verified)
    {
       // custom logic as needed
    }
}
```

Or, if only the validity period was intended as a check, use the `cert.NotBefore` and `cert.NotAfter` values.

### `System.Security.Cryptography.X509Store.AddRange`

#### Motivation

When an Exception is thrown it is possible that the store is in a modified state.

X509Store.Add has cases where Add has no visible effect, but during attempted cleanup from an Exception being thrown `AddRange` will call `Remove` on every element which was already added.  This may result in a net deletion of certificates from an X509Store.

#### Replacement

Use `X509Store.Add` and application-specific logic for handling rollback (or not) in the event of an Exception.

### `System.Security.Cryptography.X509Store.RemoveRange`

#### Motivation

When an Exception is thrown it is possible that the store is in a modified state.

X509Store.Remove succeeds when invoked with a certificate which is not present in the store.  If an Exception is thrown on a later element `RemoveRange` will try to restore state by calling `Add` on all certificates which have been "removed". In this state it is possible that a net addition of certificate has happened to the X509Store.

#### Replacement

Use `X509Store.Remove` and application-specific logic for handling rollback (or not) in the event of an Exception.


### `System.Globalization.CultureTypes.WindowsOnlyCultures`
### `System.Globalization.CultureTypes.FrameworkCultures`

#### Motivation

After the framework design changed to read all culture data from the underlying OS, the enum values WindowsOnlyCultures and FrameworkCultures became none sense. This applies to all .Net platforms. The applications can continue use these enum values but will get a compilation warning and will not have any noticable effect during the runtime 


### Incorporated
* System.Collections.\* (non-generic) - https://github.com/dotnet/platform-compat/blob/master/docs/DE0006.md
* System.Security.SecureString - https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md
* System.Security.Permissions - https://github.com/dotnet/platform-compat/blob/master/docs/DE0002.md
* System.Net.\*WebRequest - https://github.com/dotnet/platform-compat/blob/master/docs/DE0003.md
* System.Net.WebClient - https://github.com/dotnet/platform-compat/blob/master/docs/DE0004.md
* System.Net.Mail.SmtpClient - https://github.com/dotnet/platform-compat/blob/master/docs/DE0005.md [**Note** the destination topic says "SmtpClient shouldn't be used". SmtpClient is in fact not obsolete in the .NET Framework, although it has a limited implementation in .NET Core.]
