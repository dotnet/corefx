# Disclaimer

This prototype is __highly experimental__ and is not fully implemented or tested. The API shape is subject to change, and there's no guarantee that these APIs will ever be released at all. No warranty or servicing agreement is provided with these builds. We strongly advise you not to run these builds in production.

# What is the feature/utf8string branch and the `Utf8String` feature?

This is a branch where we're putting the `System.Utf8String` type and related APIs. The `System.Utf8String` type is similar to `System.String`, but the backing data is stored as UTF-8 instead of UTF-16. Many of the APIs on `System.String` have analogs in `System.Utf8String`. For example:

```cs
string theString = ...;
ReadOnlySpan<char> theStringSpan = theString.AsSpan();

Utf8String theUtf8String = ...;
ReadOnlySpan<Utf8Char> theUtf8StringSpan = theUtf8String.AsSpan(); // as a span of Utf8Char
ReadOnlySpan<byte> theUtf8StringSpan = theUtf8String.AsBytes(); // as a span of byte
```

Importantly, `Utf8String` is __not__ intended as an equal to `String`. Our goal is not to bifurcate the framework by introducing two equivalent string types. Rather, `String` will remain the foremost text exchange type in the framework, is intended for general use, and will enjoy the widest possible API support.

`Utf8String` is instead intended for scenarios where an i/o-heavy or web-first application is receiving data which is already in UTF-8 format and where the application wants to avoid the overhead of changing the string representation back and forth several times while operating on the data. A typical web application flow might involve data coming in as UTF-8 bytes, transcoding that data to `String`, performing some comparisons / slicing / etc. of that data, and transcoding the result to send it back across the network. The multiple transcoding steps and increased memory usage due to expanding UTF-8 data to UTF-16 internal representation may be undesirable for certain applications. Those applications are the target for these APIs.

Like `String`, `Utf8String` is:

* an immutable reference type,
* pinnable (you'll get back a `byte*` rather than a `char*`), and
* null-terminated.

There are some restrictions on `Utf8String` due to its early preview state.

* `Utf8String` cannot be used across p/invoke boundaries. (Workaround: use `fixed (byte* pBytes = theUtf8String) { /* ... */ }` and marshal the data manually).
* Debugging tools such as sos do not know how to show these values properly.
* There is no indexer on `Utf8String`. Instead, call the `AsSpan()` or `AsBytes()` extension method and call the indexer on the returned span.
* Many APIs are missing or incomplete. Additionally, the names and shapes of APIs are fluid. Some APIs have placeholder names so that we can expose the API behavior early to testers and receive feedback quickly without worrying about polishing the prototype.

# Getting started with Utf8String builds

If you're working in the IDE, you'll need Visual Studio 2017 (any edition w/ latest updates). If you're working on the command line, you'll need the dotnet CLI, which comes [with the SDK](https://www.microsoft.com/net/learn/dotnet/hello-world-tutorial).

## Configuring package sources

First, make sure that you have the .NET nightly MyGet feed specified in your project. This is the feed where all of the UTF-8 related packages will be located.

To configure the feed using Visual Studio:

1. Select Tools -> NuGet Package Manager -> Package Manager Settings.
2. Select _Package sources_ from the left-hand listbox.
3. In the _Available package sources_ window on the right side of the screen, add a package source named "corefx myget" with URL "https://dotnet.myget.org/F/dotnet-core/api/v3/index.json", and move this source to the top of the priority list.

To instead configure the feed using a custom config file, drop a file named `NuGet.config` into the same directory as your `.csproj`, and give the file the below contents.

```xml
<configuration>
  <packageSources>
    <clear/>
    <add key="corefx myget" value="https://dotnet.myget.org/F/dotnet-core/api/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

## Configuring your `.csproj`

You'll need to modify your `.csproj` file manually to reference the new runtime version. Merge the contents of the snippet below with those of your `.csproj` file, replacing the `RuntimeFrameworkVersion` element as appropriate.

When you copy over the snippet, be sure to replace the `RuntimeFrameworkVersion` element with the latest version listed at https://github.com/dotnet/corefx/blob/feature/utf8string/Documentation/utf8string/version_history.md. This will ensure you're using an up-to-date build of the `Utf8String` feature work.

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>netcoreapp3.0</TargetFramework>
  <!--
      Change the below version to reference the exact version you're looking for.
      Check https://github.com/dotnet/corefx/tree/feature/utf8string/Documentation/utf8string/version_history.md for latest version information and version history.
  -->
  <RuntimeFrameworkVersion>3.0.0-alphautf8string-REPLACE-ME</RuntimeFrameworkVersion>
  <NETCoreAppMaximumVersion>3.0</NETCoreAppMaximumVersion>
  <LangVersion>latest</LangVersion>
  <!-- Include the below element if you intend on using "dotnet publish" -->
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

If you already have the project open in Visual Studio at the time you make the `.csproj` changes, you may need to close and reopen VS to get it to pick up the changes.

# Useful APIs and ancillary types to get started

## Literal support

There is no C# keyword to create `Utf8String` literals. Instead, use the below syntax.

```cs
const string stringLiteral = "This is literal text.";
Utf8String utf8StringLiteral = Utf8String.Literal(stringLiteral);
```

The JIT will special-case the call to `Utf8String.Literal` and will produce literal-like codegen.

## A sample program

```cs
using System;
class Program
{
    static void Main(string[] args)
    {
        Utf8String s1 = Utf8String.Literal("Hello world!");
        Console.WriteLine(s1);

        char[] chars = new char[] { 'T', 'E', 'X', 'T' }; // "TEXT" as UTF-16
        Utf8String s2 = new Utf8String(chars); // create from UTF-16 text
        Console.WriteLine(s2);

        byte[] bytes = new byte[] { 0x74, 0x65, 0x78, 0x74 }; // "text" as UTF-8
        Utf8String s3 = new Utf8String(bytes); // create from UTF-8 text
        Console.WriteLine(s3);

        Console.WriteLine(s2 == s3); // ordinal by default, prints False
        Console.WriteLine(Utf8String.Equals(s2, s3, StringComparison.OrdinalIgnoreCase)); // prints True
    }
}
```

## Creating a `Utf8String` instance

There are several constructors on the `Utf8String type:

* `.ctor(ReadOnlySpan<byte>)` and `.ctor(ReadOnlySpan<char>)` create an instance from existing UTF-8 or UTF-16 text, respectively.
* `.ctor(byte*)` and `.ctor(char*)` create an instance from existing _null-terminated_ UTF-8 or UTF-16 text, respectively.
* The `Create` static factory method creates an instance from existing UTF-8 text and allows the developer to specify the behavior that should take place when invalid data is seen.

By default, all of the `Utf8String` constructors check their input for invalid UTF-8 data (overlong, surrogate, or out-of-range sequences). When an invalid UTF-8 sequence is seen, it is replaced by the Unicode replacement character sequence `[ EF BF BD ] (U+FFFD)`. (See also `System.Text.UnicodeScalar.ReplacementChar`.)

The static `Create` method allows the developer to specify a `System.Text.InvalidSequenceBehavior`, which controls how the factory behaves. The three options are `Fail` (the factory throws an exception), `ReplaceInvalidSequence` (invalid sequences replaced with `U+FFFD` as described above), and `LeaveUnchanged` (all validation is skipped).

## Working with spans

There is a plan to provide APIs that allow developers to work with UTF-8 data spans directly (e.g., copy UTF-8 data from a source span to a destination span, performing a `ToUpper` transform during copy) rather than force all accesses to go through the `Utf8String` type. These APIs are not yet exposed through the reference assemblies.

## Miscellaneous APIs

There are some helper APIs such as `System.IO.File.ReadAllTextUtf8` and `System.IO.StreamWriter.Write(Utf8String)` that work directly with `Utf8String` instances. More APIs will be added as the feature comes online.

# Feedback wanted!

We know there's a ton of missing APIs and functionality here. We're working through the backlog, but we need to hear from our developer audience what features / behaviors are important to you so that they can be properly prioritized.

Feature requests can be put directly in the _corefx_ repo by visiting https://github.com/dotnet/corefx and filing a new issue.

Alternatively, email the developer at levib@microsoft.com or message him on Twitter at https://twitter.com/levibroderick.
