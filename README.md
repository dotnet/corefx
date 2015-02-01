# .NET Core Libraries (CoreFX)

[![Build status][build-status-image]][build-status]  [![Issue Stats][pull-requests-image]][pull-requests]  [![Issue Stats][issues-closed-image]][issues-closed]

[build-status-image]: http://dotnet-ci.cloudapp.net/job/dotnet_corefx/badge/icon
[build-status]: http://dotnet-ci.cloudapp.net/job/dotnet_corefx/
[pull-requests-image]: http://www.issuestats.com/github/dotnet/corefx/badge/pr
[pull-requests]: http://www.issuestats.com/github/dotnet/corefx
[issues-closed-image]: http://www.issuestats.com/github/dotnet/corefx/badge/issue
[issues-closed]: http://www.issuestats.com/github/dotnet/corefx

The corefx repo contains the library implementation (called "CoreFX") for [.NET Core](http://github.com/dotnet/core). It includes System.Collections, System.IO, System.Xml and many other components. It builds and runs on Windows. You can 'watch' the repo to see Linux and Mac support being added over the next few months.

.NET Core is part of ASP.NET 5 and is a subset of the .NET Framework. You can learn more about .NET Core and how and where you can use it in the [.NET Core is open source][.NET Core oss] blog post. 

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations. 

Want to get more familiar with what's going on in the code?
* [Pull requests](https://github.com/dotnet/corefx/pulls): [Open](https://github.com/dotnet/corefx/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/corefx/pulls?q=is%3Apr+is%3Aclosed)

Looking for something to work on? The list of [up-for-grabs issues](https://github.com/dotnet/corefx/issues?q=is%3Aopen+is%3Aissue+label%3Aup-for-grabs) is a great place to start.

* [Contributing Guide][Contributing Guide]
* [Developer Guide]

You are also encouraged to start a discussion by filing an issue or creating a
gist. See the [contributing guides][Contributing Guide] for more details. 

[Contributing Guide]: https://github.com/dotnet/corefx/wiki/Contributing
[Developer Guide]: https://github.com/dotnet/corefx/wiki/Developer-Guide

You can discuss .NET OSS more generally in the [.NET Foundation forums].

[.NET Foundation forums]: http://forums.dotnetfoundation.org/

## .NET Core Library Components

The repo contains the following components. More libraries are coming soon. 'Watch' the repo, to be notified.

* **Microsoft.Win32.Primitives**. Provides common types supporting the implementation of Win32-based libraries.

* **Microsoft.Win32.Registry**. Provides support for accessing and modifying the Windows Registry.

* **System.Collections.Concurrent**.  Provides a set of thread-safe collection types, instances of which may be used 
  concurrently from multiple threads.

* **System.Collections.Immutable**. Provides a set of immutable collection types that make it easy to keep
  mutable state under control without sacrificing performance or memory
  footprint. You can read more about them on [MSDN][immutable-msdn].

* **System.Collections.NonGeneric**.  Provides classes that define various collections of objects, such as ArrayList, 
  Hashtable, SortedList, Stack, and Queue. These collections exist in .NET Core primarily for backwards compatibility
  and generally should be avoided when writing new code.

* **System.Collections.Specialized**. Provides classes that define specialized collections of objects, for example, 
  a linked list dictionary, a bit vector, and collections that contain only strings. These collections exist in 
  .NET Core primarily for backwards compatibility and generally should be avoided when writing new code.

* **System.Console**. Provides the Console class, which enables access to the standard input, 
  output, and error streams for console-based applications.

* **System.Diagnostics.FileVersionInfo**. Provides useful functionality for querying
  and examining the version information of physical files on disk.

* **System.Diagnostics.Process**. Provides access to local and remote processes, and enables the starting and
  stopping of local system processes.

* **System.Globalization.Extensions**. Provides classes for performing unicode string normalization, culture-specific string comparisons and support the use of non-ASCII characters for Internet domain names.

* **System.IO.FileSystem**. Provides access to the file system, including support for enumerating and manipulating 
  file system objects and for reading and writing files via streams.

* **System.IO.FileSystem.DriveInfo**. Provides the System.IO.DriveInfo class, which enables developers to query local drive    information.

* **System.IO.Pipes**. Provides types that enable a means for interprocess communication through anonymous 
  and/or named pipes.

* **System.IO.UnmanagedMemoryStream**. Provides a stream for accessing unmanaged memory as represented by a pointer, 
  as well as an accessor for reading and writing primitive types from unmanaged memory.

* **System.Linq.Parallel**.  Provides a parallelized implementation of LINQ to Objects. "Parallel LINQ" (PLINQ) 
  implements the full set of LINQ standard query operators as well as additional operators specific to parallel operations.

* **System.Numerics.Vectors**. Provides a set of basic vector types that leverage single instruction, 
  multiple data (SIMD) CPU instructions. See our [recent][simd-post-1] [announcements][simd-post-2] for more details.

* **System.Reflection.Metadata**. Provides a highly-tuned, low-level ECMA-335 metadata reader.  This is the same
  reader used by "[Roslyn]" C# and Visual Basic compilers to parse assemblies.

* **System.Text.RegularExpressions**. Provides a regular expression engine. The types in this library provide useful 
  functionality for running common operations using regular expressions.

* **System.Threading.Tasks.Dataflow**.  Provides a set of types that support actor/agent-oriented designs through 
  primitives for in-process message passing, dataflow, and pipelining. "TPL Dataflow" builds 
  upon the APIs and scheduling infrastructure provided by the Task Parallel Library
  (TPL), and integrates with the language support for asynchrony provided by C#, Visual Basic, and F#.

* **System.Xml**. Provides DOM APIs such as the `XDocument` and `XmlDocument`
  types, XLinq, and the corresponding XPath extension methods.

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

[.NET Core oss]: http://blogs.msdn.com/b/dotnet/archive/2014/11/12/net-core-is-open-source.aspx
[Introducing .NET Core]: http://blogs.msdn.com/b/dotnet/archive/2014/12/04/introducing-net-core.aspx
[corefx]: http://github.com/dotnet/corefx

[roslyn]: https://roslyn.codeplex.com/
[immutable-msdn]: http://msdn.microsoft.com/en-us/library/dn385366(v=vs.110).aspx
[simd-post-1]: http://blogs.msdn.com/b/dotnet/archive/2014/04/07/the-jit-finally-proposed-jit-and-simd-are-getting-married.aspx
[simd-post-2]: http://blogs.msdn.com/b/dotnet/archive/2014/05/13/update-to-simd-support.aspx


