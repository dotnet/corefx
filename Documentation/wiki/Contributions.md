Part of [[New contributor Docs]], read first **[Motivation, goals, rules](https://github.com/dotnet/corefx/wiki/New-contributor-Docs#motivation-goals-rules)**

### [Main page section content](https://github.com/dotnet/corefx/wiki/New-contributor-Docs#contributing-guide)

# Notes, ideas

## Old docs

May need updates or to be entirely replaced

* [Contributing Guide](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md)
* [Developer Guide](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md)

## New content ideas

Link [Contributing to .NET for Dummies](http://rion.io/2017/04/28/contributing-to-net-for-dummies/)

Use parts of first-time Roslyn contribution blog post - [part1](https://blog.decayingcode.com/post/contributing-to-open-source-my-first-roslyn-pull-request-getting-the-environment-ready/) and [part2](https://blog.decayingcode.com/post/contributing-to-open-source-my-first-roslyn-pull-request-fixing-the-bug/)

Use / link [dotnet/docs contributing guide](https://github.com/dotnet/docs/blob/master/CONTRIBUTING.md) -- might be a good starter for people to get used to GitHub contributions, PR workflow, etc.

### Enlist + GitHub usage + Prereqs
* uCRT: https://docs.microsoft.com/en-us/dotnet/articles/core/windows-prerequisites

### Source code structure
CoreFX source code structure intro (pre-existing text, should be updated & moved appropriately)
* The repo contains the source for each of the assemblies that comprises .NET Core.  Each ```Microsoft.*``` or ```System.*``` folder under [src](https://github.com/dotnet/corefx/tree/master/src) represents an individual library.  Each such folder may contain a ```ref``` folder, which contains the source representing the "contract" or "reference assembly" for that library.  It may also contain a ```src``` folder, which contains the source for some or all of the implementation for that library (some implementation may live in System.Private.Corelib in the [coreclr repo](https://github.com/dotnet/coreclr), with the build tooling generating type forwards from the library assembly to System.Private.Corelib.)
* It may also contain a ```test``` folder containing the tests associated with that library, whether the implementation source lives in corefx or in coreclr.

### Small change
* See [#20570](https://github.com/dotnet/corefx/issues/20570) discussion on the same topic
* What kind of changes - LINK
  * Add:
    * Decisions to take / reject changes: https://github.com/dotnet/corefx/issues/6351#issuecomment-267779077
    * Breaking changes: https://github.com/dotnet/coreclr/pull/8415#issuecomment-264267267
      * Note: Was tweeted: https://twitter.com/matthewwarren/status/805711973467955200
* How to build - build.cmd
* How to run tests
  * How to debug test failure
    * msbuild /t:test /p:testdebugger=devenv.exe
    * msbuild /t:test /p:testdebugger=c:\debuggers\windbg.exe
  * How to rerun specific test
  * How to run Outerloop tets (explain Inner vs. Outer loop tests)
  * It is ok to test on just 1 platform locally and use CI to test the other platforms. Whatever is more convenient for contributor.
* How to submit PR
  * General rules (titles, what kind of changes, don't mix changes, coding style)
    * Use [GitHub auto-closing](https://help.github.com/articles/closing-issues-via-commit-messages/) (e.g. "Fixes #12345" in PR text) - it creates nice links (see example: [#19732](https://github.com/dotnet/corefx/issues/19732))
  * @dotnet-bot docs - use command "@dotnet-bot help" in PRs, or [search some older one](https://github.com/dotnet/corefx/pulls?utf8=%E2%9C%93&q=is%3Apr%20%22%40dotnet-bot%20help%22%20)

### Add & expose API in CoreFX (.NET Core only)
Note: http://aka.ms/api-review LINK is used in file licenses
* Add it for netcore20 (latest) - see [notes](https://github.com/dotnet/corefx/pull/19885#issuecomment-302827302)
* How to add tests - see [notes](https://github.com/dotnet/corefx/pull/19885#issuecomment-302827302)
* Where to put tests (CoreFX)
    * All new APIs are initially .NET Core-Only, as such any tests covering a new API need to be in their own file (matching the *.netcoreapp.cs naming convention) which is conditioned to only be included in the .NET Core cross-compilation for the test project.
    * As an end-to-end (using the System.MathF API as an example):
        * Locate the [folder](https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/tests/) containing the relevant test project
        * Validate that the [Configuration.props](https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/tests/Configurations.props) contains the relevant `BuildConfiguration` entries (this is a single property containing a `;` delimited list). There should be an entry for both `netcoreapp-Windows_NT` and `netcoreapp-Unix`.
        * Validate that the [test project](https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/tests/System.Runtime.Extensions.Tests.csproj) has a `<DefineConstants Condition="'$(TargetGroup)'=='netcoreapp'">$(DefineConstants);netcoreapp</DefineConstants>` entry.
        * Validate that the test project contains the relevant build configuration entries (there should be a *-Debug and *-Release entry for each item in the `BuildConfiguration` list from the previous step). These should be `<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcoreapp-Unix-Debug|AnyCPU'" />`, `<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcoreapp-Unix-Release|AnyCPU'" />`, `<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcoreapp-Windows_NT-Debug|AnyCPU'" />`, and `<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'netcoreapp-Windows_NT-Release|AnyCPU'" />`.
        * In the test project, locate an existing `ItemGroup` (or add a new `ItemGroup`) with `Condition="'$(TargetGroup)'=='netcoreapp'"` (it should be the only condition on the item group) and add a new `Compile` entry for the relevant test file (such as `<Compile Include="System\MathTests.netcoreapp.cs" />`). 
    * Some other examples are [#19147](https://github.com/dotnet/corefx/pull/19147#issuecomment-301352244) or [#16996](https://github.com/dotnet/corefx/pull/16996) or [#19885](https://github.com/dotnet/corefx/pull/19885#issuecomment-303040875) (which creates new netcoreapp configuration)

### Expose API across CoreCLR/CoreFX
* How to run tests - https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md#testing-with-private-coreclr-bits
* Exposing API: [#16996](https://github.com/dotnet/corefx/pull/16996) (Reflection.Emit)
* Magic: (JanK) https://github.com/dotnet/corefx/pull/14396#issuecomment-267440477

### Advanced changes
* Expose API only on Windows/Linux -- use Registry example?
* Documentation changes
* Desktop tests
* Perf tests
  * https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/performance-tests.md
  * How to add them: https://github.com/dotnet/corefx/pull/7152

### Random
See also [Random useful stuff](https://github.com/dotnet/corefx/wiki/New-contributor-Docs#random-useful-stuff)