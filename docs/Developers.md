# Developer Guide

## Building, Testing, and Running

You can build .NET Core either via the command line or by using Visual Studio.
We currently only support building and running on Windows. Other platforms will
come later.

The command line build is invoked in  
[Visual Studio Command Prompt](http://msdn.microsoft.com/en-us/library/ms229859(v=vs.110).aspx)
via

```
build.cmd
```

In order to build successfully, you must have Visual Studio 2013 or higher
installed.

### Running Tests

We use the OSS testing framework [xUnit.net][xunit].

Running the tests from the command line is as simple as invoking `build.cmd`.

You can also run the tests from within Visual Studio. See [this page][xunit-runner]
for more details on how to install the required test runner and how you can
invoke the tests using Test Explorer.

[xunit]: http://xunit.github.io/
[xunit-runner]: https://xunit.codeplex.com/wikipage?title=HowToUseVs2012

## Strong Name Signing

All .NET Core binaries are strong named. In order for us to enable you to build
binaries that have a matching identity we leverage a mechanism called
*OSS signing*.

OSS signing is essentially just [delay signing][delay-signing] the assembly
except that the resulting assembly is marked as fully signed. This allows you to
load the assembly in most contexts, or more precisely in any context that
doesn't require validating the strong name identity. This means that you can't
install OSS signed assemblies into the GAC nor can you use it in partial trust.

OSS signing allows you to produce binaries that match the strong name identity
of the official binaries as released by Microsoft without having to add skip
verification entries to your machine.

When running on the .NET Framework (desktop) we only support using OSS signed
binaries for debugging and testing purposes. In other words, we don't guarantee
that you can successfully load OSS signed assemblies in all scenarios that are
required for production use.

However, in the context of ASP.NET Core and .NET Native on Windows we support
loading OSS signed binaries in production. But keep in mind that Microsoft
doesn't support the binaries themselves -- we only support the ability to load
them.

[delay-signing]: http://msdn.microsoft.com/en-us/library/t07a3dye.aspx
