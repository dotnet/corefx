## Motivation, Goals, Rules
We want to create better new-contributor docs, to encourage new contributors in CoreFX repo and to make it easier for existing contributors to find the right information.

To iterate really fast, CoreFX Wiki is now **writable for everyone**. It allows fast collaboration without blocking on PR code reviews, etc. Feel free to improve the docs with your notes, gotchas, ideas, etc. For even easier collaboration: [![Gitter](https://badges.gitter.im/dotnet/corefx-contrib-docs.svg)](https://gitter.im/dotnet/corefx-contrib-docs?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

When the docs are finalized, we will move them back to CoreFX repo under PR model.

Feel free to create sub-pages as necessary, please try to link/make them reachable from this 'main' page, so that we can eventually just "copy-paste" the content into CoreFX [README.md](https://github.com/dotnet/corefx/blob/master/README.md), without larger restructuring/redesign of the navigation layout. It is fine to leave TODOs and intermittent notes/ideas in all pages for now - just follow the structure you see.

Below is a skeleton to start filling out content.

# NEW CONTRIBUTOR INFORMATION

## Main page section content

This text will replace [Issue Guide](https://github.com/dotnet/corefx/blob/master/README.md#issue-guide) and [Contributing Guide](https://github.com/dotnet/corefx/blob/master/README.md#contributing-guide) sections on main page:

### Issue Guide

Note: Targeted at people just filing bugs - what to expect - e.g. API process, where to file, how to file, etc.

[Old docs](https://github.com/dotnet/corefx/wiki/Issues#existing-docs)

TODO: See ideas in [[Issues]]

### Contributing Guide

Basics:
* Per-machine setup: [Machine setup](https://github.com/dotnet/corefx/wiki/Setting-up-the-development-environment) and [Fork and clone repo](https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository)
* [Build and run tests](https://github.com/dotnet/corefx/wiki/Build-and-run-tests)
* [git commands and workflow](https://github.com/dotnet/corefx/wiki/git-reference) - for newbies
* [Pick issue](https://github.com/dotnet/corefx/wiki/Pick-issue)
* [Coding guidelines](https://github.com/dotnet/corefx/tree/master/Documentation#coding-guidelines)
* Testing change - TODO
* [Creating a Pull Request](https://github.com/dotnet/corefx/wiki/Creating-a-Pull-Request)

Advanced:
* TODO - see ideas in [[Contributions]]

# Raw Notes and TODOs

## Random useful stuff

  * How to debug (VS, VS Code, windbg)
    * Mixed mode
    * Linux - [using SOS](https://blogs.msdn.microsoft.com/premier_developer/2017/05/02/debugging-net-core-with-sos-everywhere/)
    * Mac
  * How to investigate perf
    * Production level profiling (e.g. needed in [#13837](https://github.com/dotnet/corefx/issues/13837))
    * Memory analysis (GC happens too often) (e.g. needed in [#14008](https://github.com/dotnet/corefx/issues/14008))
    * https://github.com/dotnet/BenchmarkDotNet
    * Perf Analysis on .NET Core on Windows - [Vance's blog post](https://blogs.msdn.microsoft.com/vancem/2016/11/02/performance-analysis-on-net-core-applications-with-perfview-on-windows/)
    * Collecting perf data on .NET Core on Linux - [Vance's blog post](https://blogs.msdn.microsoft.com/vancem/2016/10/07/collecting-performance-data-on-linux-using-perfcollect-and-perfview/)
  * How trace Runtime event
    * On Windows - PerfView, on [Linux](http://blogs.microsoft.co.il/sasha/2017/03/30/tracing-runtime-events-in-net-core-on-linux/)
  * How/when to run with GCStress (set environment variable [COMPlus_GCStress](https://github.com/dotnet/coreclr/blob/master/src/vm/eeconfig.h#L654)=3 on **retail build**)
  * How to change .NET Core docs
    * Tutorials, samples - https://github.com/dotnet/docs
    * API reference - https://github.com/dotnet/dotnet-api-docs
    * MS Only: .NET Core website - https://github.com/dotnet/dotnet-core-website
  * SharpLab: https://sharplab.io/
  * How to port .NET Framework app to .NET Core:
    * https://docs.microsoft.com/dotnet/core/porting/
    * http://developerblog.redhat.com/2016/12/08/observations-porting-from-net-framework-to-net-core/
  * Developing .NET Core apps: https://blogs.msdn.microsoft.com/mvpawardprogram/2016/07/19/key-steps-in-developing-net-core-applications/
  * Using reference source from here is OK: https://github.com/Microsoft/referencesource (same license)
    * Richard's explanation: https://github.com/dotnet/coreclr/issues/9301#issuecomment-277534312
  * Useful tips and tricks, tools, education material - https://github.com/quozd/awesome-dotnet (we might bring some links to CoreFX docs)
