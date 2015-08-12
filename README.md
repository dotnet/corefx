# .NET Core Libraries (CoreFX)

|   |Linux|Windows|OS X|
|:-:|:-:|:-:|:-:|
|**Debug**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_linux_debug_tst/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_linux_debug_tst/)|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_windows_debug/)|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_mac_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_mac_debug/)|
|**Release**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_linux_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_linux_release/)|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_windows_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_windows_release/)|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_mac_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_mac_release/)|
|**Coverage Report**||[![Coverage Status](https://img.shields.io/badge/corefx-code_coverage-blue.svg)](http://dotnet-ci.cloudapp.net/job/dotnet_corefx_coverage_windows/lastStableBuild/Code_Coverage_Report/)||

The corefx repo contains the library implementation (called "CoreFX") for [.NET Core](http://github.com/dotnet/core). It includes System.Collections, System.IO, System.Xml and many other components. It builds and runs on Windows. You can ['watch'](https://github.com/dotnet/corefx/subscription) the repo to see Linux and Mac support being added over the next few months.

You can also see more information in the [Documentation README](Documentation/README.md). 

.NET Core is a modular implementation of .NET that can be used as the base stack for a wide variety of scenarios, today scaling from console utilities to web apps in the cloud.  You can learn more about .NET Core and how and where you can use it in the [.NET Core is open source][.NET Core oss] and [Introducing .NET Core][Introducing .NET Core] blog posts.

The [.NET Core Runtime repo](https://github.com/dotnet/coreclr) contains the  runtime implementation (called "CoreCLR") for .NET Core. It includes RyuJIT, the .NET GC, native interop and many other components.

Runtime-specific library code - namely [mscorlib][mscorlib] - lives in the CoreCLR repo. It needs to be built and versioned in tandem with the runtime. The rest of CoreFX is agnostic of runtime-implementation and can be run on any compatible .NET runtime. These characteristics were the primary motivation for the 2-repo structure.

[.NET Core oss]: http://blogs.msdn.com/b/dotnet/archive/2014/11/12/net-core-is-open-source.aspx
[Introducing .NET Core]: http://blogs.msdn.com/b/dotnet/archive/2014/12/04/introducing-net-core.aspx
[mscorlib]: https://github.com/dotnet/coreclr/tree/master/src/mscorlib

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations.

Want to get more familiar with what's going on in the code?
* [Pull requests](https://github.com/dotnet/corefx/pulls): [Open](https://github.com/dotnet/corefx/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/corefx/pulls?q=is%3Apr+is%3Aclosed)
* [![Backlog](https://cloud.githubusercontent.com/assets/1302850/6260412/38987b1e-b793-11e4-9ade-d3fef4c6bf48.png)](https://github.com/dotnet/corefx/issues?q=is%3Aopen+is%3Aissue+label%3A%220+-+Backlog%22), [![Up Next](https://cloud.githubusercontent.com/assets/1302850/6260418/4c2c7a54-b793-11e4-8ce1-a27ff5378d08.png)](https://github.com/dotnet/corefx/issues?q=is%3Aopen+is%3Aissue+label%3A%221+-+Up+Next%22) and [![In Progress](https://cloud.githubusercontent.com/assets/1302850/6260414/41b0fc30-b793-11e4-9d50-d09563cd138a.png)](https://github.com/dotnet/corefx/issues?q=is%3Aopen+is%3Aissue+label%3A%222+-+In+Progress%22) changes

Looking for something to work on? The list of [up-for-grabs issues](https://github.com/dotnet/corefx/labels/up%20for%20grabs) is a great place to start or for larger items see the list of [feature approved](https://github.com/dotnet/corefx/labels/feature%20approved). See some of our guides for more details:

* [Contributing Guide](Documentation/project-docs/contributing.md)
* [Developer Guide](Documentation/project-docs/developer-guide.md)
* [Issue Guide](Documentation/project-docs/issue-guide.md)

You are also encouraged to start a discussion by filing an issue or creating a
gist.

You can discuss .NET OSS more generally in the [.NET Foundation forums].

Want to chat with other members of the CoreFX community?

[![Join the chat at https://gitter.im/dotnet/corefx](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/corefx?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[.NET Foundation forums]: http://forums.dotnetfoundation.org/

## .NET Core Library Components

The repo currently contains the source for the following components.
More libraries are coming soon (the overall list of items we currently plan to move onto GitHub is [here][typelist]).
['Watch'](https://github.com/dotnet/corefx/subscription) the repo to be notified.

|Component|Description|
|:--------|:----------|
|<sub>**System.Collections**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Collections.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that define generic collections, which allow developers to create strongly-typed collections.</sub>|
|<sub>**System.Collections.Concurrent**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Collections.Concurrent.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a set of thread-safe collection types, instances of which may be used concurrently from multiple threads.</sub>|
|<sub>**System.Collections.Immutable**<br/>[![MyGet Package](https://img.shields.io/nuget/vpre/System.Collections.Immutable.svg)](http://www.nuget.org/packages/System.Collections.Immutable/)</sub>|<sub>Provides a set of immutable collection types that are safe to use concurrently.</sub>|
|<sub>**System.Collections.NonGeneric**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Collections.NonGeneric.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that define various collections of objects, such as ArrayList and Hashtable. _These collections exist in .NET Core primarily for backwards compatibility and generally should be avoided when writing new code_.</sub>|
|<sub>**System.Collections.Specialized**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Collections.Specialized.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that define specialized collections of objects, for example, a linked list dictionary and collections that contain only strings. _These collections exist in .NET Core primarily for backwards compatibility and generally should be avoided when writing new code_.</sub>|
|<sub>**System.ComponentModel**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ComponentModel.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides interfaces for the editing and change tracking of objects used as data sources.</sub>|
|<sub>**System.ComponentModel.Annotations**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ComponentModel.Annotations.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides attributes that are used to define metadata for objects used as data sources.</sub>|
|<sub>**System.ComponentModel.EventBasedAsync**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ComponentModel.EventBasedAsync.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides support classes and delegates for the event-based asynchronous pattern. _This pattern and these supporting types exist in .NET Core primarily for backwards compatibility and generally should be avoided when writing new code_.</sub>|
|<sub>**System.ComponentModel.Primitives**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ComponentModel.Primitives.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides interfaces that are used to implement the run-time and design-time behavior of components.</sub>|
|<sub>**System.ComponentModel.TypeConverter**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ComponentModel.TypeConverter.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.ComponentModel.TypeConverter class, which represents a unified way of converting types of values to other types.</sub>|
|<sub>**System.Console**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Console.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the Console class, which enables access to the standard input, output, and error streams for console-based applications.</sub>|
|<sub>**System.Data.Common**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Data.Common.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the base abstract classes, including System.Data.DbConnection and System.Data.DbCommand, for data providers.</sub>|
|<sub>**System.Diagnostics.Contracts**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.Contracts.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types and methods for representing program contracts such as preconditions, postconditions, and invariants.</sub>|
|<sub>**System.Diagnostics.Debug**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.Debug.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a class to interact with the debugger as well as methods for performing runtime assertions.</sub>|
|<sub>**System.Diagnostics.FileVersionInfo**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.FileVersionInfo.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides useful functionality for querying and examining the version information of physical files on disk.</sub>|
|<sub>**System.Diagnostics.Process**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.Process.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides access to local and remote processes, and enables the starting and stopping of local system processes.</sub>|
|<sub>**System.Diagnostics.TextWriterTraceListener**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.TextWriterTraceListener.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides trace listeners for directing tracing output to a text writer, such as System.IO.StreamWriter.</sub>|
|<sub>**System.Diagnostics.Tools**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.Tools.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides attributes, such as GeneratedCodeAttribute, that are emitted or consumed by analysis tools.</sub>|
|<sub>**System.Diagnostics.TraceSource**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Diagnostics.TraceSource.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that help you trace the execution of your code.</sub>|
|<sub>**System.Dynamic.Runtime**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Dynamic.Runtime.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes and interfaces that support the Dynamic Language Runtime (DLR).</sub>|
|<sub>**System.Globalization**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Globalization.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that define culture-related information, including language, country/region, calendars, format patterns, and sort orders.</sub>|
|<sub>**System.Globalization.Calendars**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Globalization.Calendars.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for performing date calculations using specific calendars, including the Gregorian, Julian, Hijri and Korean calendars.</sub>|
|<sub>**System.Globalization.Extensions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Globalization.Extensions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for performing unicode string normalization, culture-specific string comparisons and support the use of non-ASCII characters for Internet domain names.</sub>|
|<sub>**System.IO**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides base input and output (I/O) types that enable reading and writing data streams.</sub>|
|<sub>**System.IO.Compression**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.Compression.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that support the compression and decompression of streams.</sub>|
|<sub>**System.IO.Compression.ZipFile**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.Compression.ZipFile.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides static methods for creating and using Zip files.</sub>|
|<sub>**System.IO.FileSystem**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.FileSystem.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides access to the file system, including support for enumerating and manipulating file system objects and for reading and writing files via streams.</sub>|
|<sub>**System.IO.FileSystem.DriveInfo**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.FileSystem.DriveInfo.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.IO.DriveInfo class, which enables developers to query local drive information.</sub>|
|<sub>**System.IO.FileSystem.Primitives**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.FileSystem.Primitives.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides common enumerations and exceptions for path-based I/O libraries.</sub>|
|<sub>**System.IO.FileSystem.Watcher**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.FileSystem.Watcher.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.IO.Watcher class, which listens to the system directory change notifications and raises events when a directory or file within a directory changes.</sub>|
|<sub>**System.IO.MemoryMappedFiles**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.MemoryMappedFiles.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides access to memory-mapped files, enabling code to read and write files by reading and writing memory.</sub>|
|<sub>**System.IO.Packaging**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.Packaging.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes that support storage of multiple data objects in a single container.</sub>|
|<sub>**System.IO.Pipes**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.Pipes.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that enable a means for interprocess communication through anonymous and/or named pipes.</sub>|
|<sub>**System.IO.UnmanagedMemoryStream**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.IO.UnmanagedMemoryStream.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a stream for accessing unmanaged memory as represented by a pointer, as well as an accessor for reading and writing primitive types from unmanaged memory.</sub>|
|<sub>**System.Linq**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Linq.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the foundation of Language-Integrated Query (LINQ), including LINQ standard query operators that operate on objects that implement ```IEnumerable<T>```.</sub>|
|<sub>**System.Linq.Expressions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Linq.Expressions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes, interfaces, and enumerations that enable language-level code expressions to be represented as objects in the form of expression trees.</sub>|
|<sub>**System.Linq.Parallel**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Linq.Parallel.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub> Provides a parallelized implementation of LINQ to Objects. "Parallel LINQ" (PLINQ) implements the full set of LINQ standard query operators as well as additional operators specific to parallel operations.</sub>|
|<sub>**System.Linq.Queryable**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Linq.Queryable.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides LINQ standard query operators that operate on objects that implement ```IQueryable<T>```.</sub>|
|<sub>**System.Net.Http**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.Http.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a programming interface for modern HTTP applications, including HTTP client components that allow applications to consume web services over HTTP and HTTP components that can be used by both clients and servers for parsing HTTP headers.</sub>|
|<sub>**System.Net.Http.WinHttpHandler**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.Http.WinHttpHandler.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a message handler for HttpClient based on the WinHTTP interface of Windows. While similar to HttpClientHandler, it provides developers more granular control over the application's HTTP communication than the HttpClientHandler.</sub>|
|<sub>**System.Net.Primitives**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.Primitives.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides common types for network-based libraries, including System.Net.IPAddress, System.Net.IPEndPoint, and System.Net.CookieContainer.</sub>|
|<sub>**System.Net.Requests**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.Requests.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides older classes (such as HttpWebRequest and HttpWebResponse) for sending HTTP requests and receiving HTTP responses from a resource identified by a URI. _This library is available primarily for compatibility; developers should prefer the classes in the System.Net.Http package._</sub>|
|<sub>**System.Net.WebHeaderCollection**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.WebHeaderCollection.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Contains types that represent HTTP request and response headers. This library is used with classes such as System.Net.HttpWebRequest and System.Net.HttpWebResponse and allows developers to query/edit header names/values.</sub>|
|<sub>**System.Net.WebSockets**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.WebSockets.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.Net.WebSockets.WebSocket abstract class and related types to allow developers to implement the WebSocket protocol (RFC 6455). WebSockets provide full-duplex communication over a single TCP connection.</sub>|
|<sub>**System.Net.WebSockets.Client**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Net.WebSockets.Client.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.Net.WebSockets.ClientWebSocket class, which implements the client role of the WebSockets protocol (RFC 6455).</sub>|
|<sub>**System.Numerics.Vectors**<br/>[![MyGet Package](https://img.shields.io/nuget/vpre/System.Numerics.Vectors.svg)](http://www.nuget.org/packages/System.Numerics.Vectors/)</sub>|<sub>Provides a set of basic vector types that leverage single instruction, multiple data (SIMD) CPU instructions.</sub>|
|<sub>**System.Numerics.Vectors.WindowsRuntime**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Numerics.Vectors.WindowsRuntime.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides extension methods for converting between types in System.Numerics.Vectors and types in the Windows Runtime (WinRT).</sub>|
|<sub>**System.ObjectModel**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ObjectModel.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types and interfaces that enable the creation of observable types that provide notifications to clients when changes are made.</sub>|
|<sub>**System.Reflection**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that retrieve information about assemblies, modules, members, parameters, and other entities in managed code by examining their metadata.</sub>|
|<sub>**System.Reflection.DispatchProxy**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.DispatchProxy.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a mechanism for dynamically creating proxy types that implement a specified interface and derive from a specified DispatchProxy type.</sub>|
|<sub>**System.Reflection.Emit**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.Emit.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that allow a compiler or other tool to emit metadata and generate PE files on disk.</sub>|
|<sub>**System.Reflection.Emit.ILGeneration**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.Emit.ILGeneration.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that allow a compiler or other tool to emit Microsoft intermediate language (MSIL).</sub>|
|<sub>**System.Reflection.Emit.Lightweight**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.Emit.Lightweight.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.Reflection.Emit.DynamicMethod class, which represents a dynamic method that can be compiled, executed, and discarded.</sub>|
|<sub>**System.Reflection.Extensions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.Extensions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides custom attribute extension methods for System.Reflection types.</sub>|
|<sub>**System.Reflection.Metadata**<br/>[![MyGet Package](https://img.shields.io/nuget/vpre/System.Reflection.Metadata.svg)](http://www.nuget.org/packages/System.Reflection.Metadata/)</sub>|<sub>Provides a highly-tuned, low-level ECMA-335 metadata reader.  This is the same reader used by "[Roslyn]" C# and Visual Basic compilers to parse assemblies.</sub>|
|<sub>**System.Reflection.TypeExtensions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Reflection.TypeExtensions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides extension methods for types in the System.Reflection namespace. _These extensions are designed to be source-compatible with older reflection-based APIs_.</sub>|
|<sub>**System.Resources.ReaderWriter**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Resources.ReaderWriter.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for reading and writing resources in the system-default format.</sub>|
|<sub>**System.Runtime**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the fundamental primitives, classes, and base classes that define commonly-used value and reference data types, events and event handlers, interfaces, attributes, and exceptions.</sub>|
|<sub>**System.Runtime.Extensions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Extensions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides commonly-used classes for performing mathematical functions, conversions, string comparisons, and querying environment information.</sub>|
|<sub>**System.Runtime.Handles**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Handles.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides base classes, including CriticalHandle and SafeHandle, for types that represent operating system handles.</sub>|
|<sub>**System.Runtime.InteropServices**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.InteropServices.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that support platform invoke (P/Invoke) and COM interop.</sub>|
|<sub>**System.Runtime.InteropServices.RuntimeInformation**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.InteropServices.RuntimeInformation.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that expose information about the runtime and operating system environment in which code is executing.</sub>|
|<sub>**System.Runtime.Numerics**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Numerics.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides two useful numeric structures, BigInteger and Complex.</sub>|
|<sub>**System.Runtime.Serialization.Json**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Serialization.Json.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for serializing objects to the JavaScript Object Notation (JSON) and for deserializing JSON data to objects.</sub>|
|<sub>**System.Runtime.Serialization.Primitives**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Serialization.Primitives.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides common types, including System.Runtime.Serialization.DataContractAttribute, for libraries that support data contract serialization.</sub>|
|<sub>**System.Runtime.Serialization.Xml**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Runtime.Serialization.Xml.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for serializing objects to the Extensible Markup Language (XML) and deserializing XML data to objects.</sub>|
|<sub>**System.Security.Cryptography.DeriveBytes**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.DeriveBytes.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.Security.Cryptography.Rfc2898DeriveBytes class, which implements password-based key derivation functionality per [RFC 2898](http://www.ietf.org/rfc/rfc2898.txt).</sub>|
|<sub>**System.Security.Cryptography.Encoding**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.Encoding.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types for representing Abstract Syntax Notation One (ASN.1)-encoded data.</sub>|
|<sub>**System.Security.Cryptography.Encryption**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.Encryption.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides base types for symmetric and asymmetric cryptographic algorithms.</sub>|
|<sub>**System.Security.Cryptography.Encryption.Aes**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.Encryption.Aes.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types which perform symmetric encryption and decryption using the Advanced Encryption Standard (AES) algorithm.</sub>|
|<sub>**System.Security.Cryptography.Hashing**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.Hashing.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides base types for cryptographic hashing and hash-based message authentication code (HMAC).</sub>|
|<sub>**System.Security.Cryptography.Hashing.Algorithms**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.Hashing.Algorithms.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides concrete implementations of cryptographic hashing and hash-based message authentication code (HMAC), including MD5, SHA-1, and SHA-2.</sub>|
|<sub>**System.Security.Cryptography.RandomNumberGenerator**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.RandomNumberGenerator.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the System.Security.Cryptography.RandomNumberGenerator class, which generates cryptographically secure random numbers.</sub>|
|<sub>**System.Security.Cryptography.RSA**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.RSA.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types which perform asymmetric encryption and decryption using the RSA algorithm.</sub>|
|<sub>**System.Security.Cryptography.X509Certificates**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Cryptography.X509Certificates.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types for reading, exporting and verifying Authenticode X.509 v3 certificates.</sub>|
|<sub>**System.Security.Principal**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Principal.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the base interfaces for principal and identity objects that represent the security context under which code is running.</sub>|
|<sub>**System.Security.Principal.Windows**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.Principal.Windows.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for retrieving the current Windows user and for interacting with Windows users and groups.</sub>|
|<sub>**System.Security.SecureString**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Security.SecureString.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides support for accessing and modifying text that should be kept confidential.</sub>|
|<sub>**System.ServiceProcess.ServiceController**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.ServiceProcess.ServiceController.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the ServiceController class that represents a Windows service and allows you to connect to a running or stopped service, manipulate it, or get information about it.</sub>|
|<sub>**System.Text.Encoding**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Text.Encoding.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides base abstract encoding classes for converting blocks of characters to and from blocks of bytes.</sub>|
|<sub>**System.Text.Encoding.CodePages**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Text.Encoding.CodePages.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides the ability to access existing encoding types for string manipulation across common cultural standards, as well as support to create custom Encoding Providers.</sub>|
|<sub>**System.Text.Encoding.Extensions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Text.Encoding.Extensions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides support for specific encodings, including ASCII, UTF-7, UTF-8, UTF-16, and UTF-32.</sub>|
|<sub>**System.Text.Encodings.Web**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Text.Encodings.Web.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub> Provides support for encodings related to HTML, JavaScript, and URLs.</sub>|
|<sub>**System.Text.RegularExpressions**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Text.RegularExpressions.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides a regular expression engine. The types in this library provide useful functionality for running common operations using regular expressions.</sub>|
|<sub>**System.Threading**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Threading.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides synchronization primitives used when writing multi-threaded and asynchronous code.</sub>|
|<sub>**System.Threading.Overlapped**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Threading.Overlapped.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides common types for interacting with asynchronous (or overlapped) input and output (I/O) on Windows.</sub>|
|<sub>**System.Threading.Tasks**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Threading.Tasks.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that simplify the work of writing concurrent and asynchronous code.</sub>|
|<sub>**System.Threading.Tasks.Dataflow**<br/>[![MyGet Package](https://img.shields.io/nuget/vpre/Microsoft.Tpl.Dataflow.svg)](http://www.nuget.org/packages/Microsoft.Tpl.Dataflow/)</sub>|<sub> Provides a set of types that support actor/agent-oriented designs through primitives for in-process message passing, dataflow, and pipelining.</sub>|
|<sub>**System.Threading.Tasks.Parallel**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Threading.Tasks.Parallel.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub> Provides library-based data parallel replacements for common operations such as for loops, for each loops, and execution of a set of statements.</sub>|
|<sub>**System.Xml.ReaderWriter**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.ReaderWriter.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types for reading and writing streams of XML.</sub>|
|<sub>**System.Xml.XDocument**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XDocument.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides XML-related types for querying XML documents using LINQ.</sub>|
|<sub>**System.Xml.XmlDocument**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XmlDocument.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub> Provides types for manipulating an XML Document Object Model (DOM).</sub>|
|<sub>**System.Xml.XmlSerializer**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XmlSerializer.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides classes for serializing objects to XML and for deserializing XML data to objects.</sub>|
|<sub>**System.Xml.XPath**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XPath.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub> Provides classes that define a cursor model for navigating and editing XML information items as instances of the XQuery 1.0 and XPath 2.0 Data Model.</sub>|
|<sub>**System.Xml.XPath.XDocument**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XPath.XDocument.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides extension methods that add System.Xml.XPath support to the System.Xml.XDocument package.</sub>|
|<sub>**System.Xml.XPath.XmlDocument**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/System.Xml.XPath.XmlDocument.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides extension methods that add System.Xml.XPath support to the System.Xml.XmlDocument package.</sub>|
|<sub>**Microsoft.CSharp**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/Microsoft.CSharp.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides support for compilation and code generation, including dynamic, using the C# language.</sub>|
|<sub>**Microsoft.VisualBasic**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/Microsoft.VisualBasic.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides types that support the Visual Basic runtime.</sub>|
|<sub>**Microsoft.Win32.Primitives**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/Microsoft.Win32.Primitives.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides common types supporting the implementation of Win32-based libraries.</sub>|
|<sub>**Microsoft.Win32.Registry**<br/>[![MyGet Package](https://img.shields.io/myget/dotnet-core/v/Microsoft.Win32.Registry.svg)](https://www.myget.org/gallery/dotnet-core)</sub>|<sub>Provides support for accessing and modifying the Windows Registry.</sub>|

[roslyn]: https://github.com/dotnet/roslyn
[typelist]: https://github.com/dotnet/corefx-progress/blob/master/src-diff/README.md

## License

.NET Core (including the corefx repo) is licensed under the [MIT license](LICENSE).

## .NET Foundation

.NET Core is a [.NET Foundation](http://www.dotnetfoundation.org/projects) project.

## Related Projects
There are many .NET related projects on GitHub.

- The
[.NET home repo](https://github.com/Microsoft/dotnet) links to 100s of .NET projects, from Microsoft and the community.
- The [.NET Core repo](https://github.com/dotnet/core) links to .NET Core related projects from Microsoft.
- The [ASP.NET home repo](https://github.com/aspnet/home) is the best place to start learning about ASP.NET 5.
- [dotnet.github.io](http://dotnet.github.io) is a good place to discover .NET Foundation projects.
