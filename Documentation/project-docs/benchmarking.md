# Using BenchmarkDotNet
In this example we are using BenchmarkDotNet (https://github.com/dotnet/BenchmarkDotNet) for our testing. Make sure to use the latest version which allows to specify a custom SKD path.

```
<ItemGroup>
   <PackageReference Include="BenchmarkDotNet" Version="0.10.10" />
</ItemGroup>
```

# Benchmarking .NET Core 2.0 applications
For benchmarking .NET Core 2.0 applications you only need the .NET Core 2.0 SDK installed: https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-win-x64.exe. Make sure that your `TargetFramework` property in your csproj is set to `netcoreapp2.0` and follow the official BenchmarkDotNet instructions: https://github.com/dotnet/BenchmarkDotNet/blob/master/docs/guide/Configs/Toolchains.md

# Benchmarking .NET Core 2.1 applications
Make sure to download the .NET Core 2.1 SDK zip archive (https://github.com/dotnet/core-setup#daily-builds) and extract it somewhere locally, e.g.: `C:\Program Files\dotnet-nightly\`.

In this tutorial we won't modify the `PATH` variable and instead always explicitely call the `dotnet.exe` from the downloaded SDK folder.

Check which version of the shared framework is bundled with the downloaded SDK and remember it for later. At the time writing this documentation it was `2.1.0-preview1-25919-02`.
> C:\Program Files\dotnet-nightly\shared\Microsoft.NETCore.App\2.1.0-preview1-25919-02

You can either decide to use your local self-compiled shared framework package or use the one from the .NET Core 2.1 SDK.

## Using the shared framework from the .NET Core 2.1 SDK
Follow the instructions described here https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/dogfooding.md#advanced-scenario---using-a-nightly-build-of-microsoftnetcoreapp and skip the last part which calls the `dotnet.exe`.

### Configuring BenchmarkDotNet to use the .NET Core 2.1 SDK
Now we are going to create a BenchmarkDotNet configuration file to set the path to the .NET Core 2.1 SDK

```
public class MainConfig : ManualConfig
{
    public MainConfig()
    {
        Add(Job.Default
            .With(Runtime.Core)
            .With(CsProjCoreToolchain.From(new NetCoreAppSettings(
                targetFrameworkMoniker: "netcoreapp2.1",
                runtimeFrameworkVersion: "2.1.0-preview1-25919-02", // <-- Adjust version here
                customDotNetCliPath: @"C:\Program Files\dotnet-nightly\dotnet.exe", // <-- Adjust path here
                name: "Core 2.1.0-preview")))
            .WithLaunchCount(1)
            .WithWarmupCount(1)
            .WithInvocationCount(10)
            .WithUnrollFactor(1)
            .WithTargetCount(3));

        // Add whatever jobs you need
        Add(DefaultColumnProviders.Instance);
        Add(MarkdownExporter.GitHub);
        Add(new ConsoleLogger());
        Add(new HtmlExporter());
        Add(MemoryDiagnoser.Default);
    }
}
```

Pass the configuration to the BenchmarkRunner.

### Executing

As mentioned before, instead of calling the dotnet in the `PATH` we call the `dotnet.exe` explicitely from the downloaded SDK folder:

```
> cd "path/to/your/benchmark/project"
> "C:\Program Files\dotnet-nightly\dotnet.exe" restore
> "C:\Program Files\dotnet-nightly\dotnet.exe" build -c Release
> "C:\Program Files\dotnet-nightly\dotnet.exe" run -c Release
```

## Using your self-compiled shared framework
Follow the instructions described here https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/dogfooding.md#more-advanced-scenario---using-your-local-corefx-build and skip the last part which calls the `dotnet.exe`.
Make sure to build your local corefx repository in RELEASE mode `.\build -release`! You currently need to have a self-contained application to inject your local shared framework package.

### Configuring BenchmarkDotNet to use the .NET Core 2.1 SDK
Now we are going to create a BenchmarkDotNet configuration file. Currently there is no easy way to run your BenchmarkDotNet application in a dedicated process, therefore we are using the InProcess switch:

```
public class MainConfig : ManualConfig
{
    public MainConfig()
    {
        Add(Job.Default
            .With(InProcessToolchain.Instance) <-- To run against your local built shared framework (in corefx folder)
            .WithLaunchCount(1)
            .WithWarmupCount(1)
            .WithInvocationCount(1)
            .WithUnrollFactor(1)
            .WithTargetCount(3));

        // Add whatever jobs you need
        Add(DefaultColumnProviders.Instance);
        Add(MarkdownExporter.GitHub);
        Add(new ConsoleLogger());
        Add(new HtmlExporter());
        Add(MemoryDiagnoser.Default);
    }
}
```

### Executing

As mentioned before, instead of calling the dotnet in the `PATH` we call the `dotnet.exe` explicitely from the downloaded SDK folder:

```
> cd "path/to/your/benchmark/project"
> "C:\Program Files\dotnet-nightly\dotnet.exe" restore
> "C:\Program Files\dotnet-nightly\dotnet.exe" build -c Release
> "C:\Program Files\dotnet-nightly\dotnet.exe" run -c Release
```
