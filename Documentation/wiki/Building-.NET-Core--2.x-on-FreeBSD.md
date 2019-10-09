<a name="home"/>

[Disclaimer](#disclaimer) | [Overview](#overview) | [Building CoreCLR](#building-coreclr) | [Building CoreFX](#building-corefx) | [Troubleshooting](#troubleshooting-and-diag) | [Tips and Tricks](#tips-and-tricks) | [Updates](#updates)

## Disclaimer

Support for FreeBSD is in early stages.
Instructions bellow may or may not give you what you want. 
Get ready for bumpy road.
You have been warned!
Tested only on basic FreeBSD 11.0 image.

## Overview

### Runtime vs. Software Development Kit (SDK)

In general, there are two products - Runtime and SDK. The Runtime (among other functions), converts the Intermediary Language code (IL) to runnable binaries that are executed on the target system. The SDK consists of the base components that are used by developers to write software. 

There are two parts to each product: Platform specific code, and "managed" dotnet components. The platform specific code is built using Autogen, CMake (and some Python) and compiled against standard C libraries with standard C/C++ compilers (GCC, Clang, musl). The managed dotnet components, however, are built using managed tools written in C# against DotNet. On Windows, this is supported by the .Net Framework and Visual Studio. On Linux, this is supported by DotNetCore 1.0. As FreeBSD does not (fully) support either, we have the following choices:
- Import the managed components in binary format from another platform
- Build the components with Mono or Linux DotNet

This instruction set focuses on the former. 

### Repositories

There is no single GitHub repository that represents the entire dotnet core build. It is very difficult at this point to get all the parts glued together. This cohesion will be needed at some point for adding package to ports collection, but is not currently in scope for these instructions. [Source-build](https://github.com/dotnet/source-build) project is trying to make build easier for OS vendors. 

### Building the .NET Repositories

In a simplified way, the build dependencies are cli->core-setup->corefx->coreclr. The Command Line Interface (cli) needs a number of components like MSBuild and the Roslyn compiler. We will attempt to build all the repos in reverse order, starting with the coreclr and the corefx. Most repositories build debug version by default. To change build configuration, append  -release or the release keyword to various build instructions.

### Notes About Cross Platform Builds

It is important to ensure that the repositories and build configurations are the same on FreeBSD and on the cross build platform (Windows or GNU/Linux) for the managed code. Put another way: 

- Both outputs must be of debug or release configuration. You cannot mix the configurations.
- Both outputs must be from the same git commit (roughly?).

If the platforms do not match, the coreclr tests will fail to run with an error of

```
coreclr_initialize failed - status: 0x80004005
```

See https://github.com/dotnet/coreclr/issues/1419 for more details

[Top](#home)

## Building coreclr

First of all, one needs the usual build dependencies:

`pkg install cmake git icu libunwind bash python2 krb5 lttng-ust`

The build system expects all the clang/llvm tools to exist with version number. The clang 3.8 should be sufficient but it was difficult to use with current build system. This instruction was built using clang 3.9.

To build the FreeBSD native parts of coreclr run:

`./build.sh -clang3.9`


### Building CoreLib

The second part of coreclr is corelib. The System.Private.CoreLib.dll executable is managed code and expects a functional SDK. This is normally solved by using an older version for bootstrapping, which does not expressly exist yet for FreeBSD.

* Note: It is **mandatory** that the managed part needs to be built from the same branch and/or commit hash.
Also debug/release flavors are not binary compatible. Managed and native part needs to either be both release or both debug. *

#### Building CoreLib on Windows 

System.Private.CoreLib.dll is to be built for FreeBSD on Windows 10 if the development environment is correctly configured for build dotnet. The instructions are outlined in the Windows build instructions here:

https://github.com/dotnet/coreclr/blob/master/Documentation/building/windows-instructions.md

To build mscorlib run:

``` build.cmd -freebsdmscorlib```

Building the CLR managed tests on Windows is still a work in progress.

#### Building Corelib on Linux

There are two caveats (aside from inconvenience) we need to overcome with Linux build. 

Linux (and other platform) assemblies are "crossgen"ed by default . That is optimization that generates native code for given platform. 

```
Unhandled Exception: System.BadImageFormatException: Could not load file or assembly 'System.Runtime, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'. An attempt was made to load a program with an incorrect format.```

You can use 'file' utility to get basic info about assembly:

```file foo.dll

System.Private.CoreLib.dll: PE32+ executable (DLL) (console) x86-64 Mono/.Net assembly, for MS Windows
```
vs
```
file foo.dll: PE32 executable (DLL) (console) Intel 80386 Mono/.Net assembly, for MS Windows
```
Current branches (2.1) has `-skipcrossgen` option.

Alternativaly one can use `export COMPlus_ZapDisable=1` and COMPlus_ReadyToRun=0 environmental variables to tell runtime to ignore native code in assemblies (currently does not work for corelib).


#### When doing "normal" build on Linux, corelib would get some features available only on Linux.
That may not matter for simple "Hello World" but it will cause troubles. You can see various options in `clr.coreclr.props`.
We need 
Long term goal would be to allow cross-platform build. There may be fragments in place but it will need more exploration and testing. Windows build.cmd has 'freebsdmscorlib' option and that can be ported to NIX branches. 


With that we can run Linux build:

```
./build.sh -clang3.9 -skipcrossgen -osgroup FreeBSD 
```

That will produce `bin/Product/Linux.x64.Debug/System.Private.CoreLib.dll`

Copy that to FreeBSD system to `bin/Product/FreeBSD.x64.Debug` directory.
To make debug easier copy also `bin/Product/Linux.x64.Debug/PDB/System.Private.CoreLib.pdb` to same location.
That will be handy for analyzing stack traces. 

#### Building CoreLib on FreeBSD (experimental)
As of 2.1 it is possible to run Linux SDK under Linux emulation. 

Getting ready:

```
$ sudo kldload linux64
$ sudo pkg install linux_base-c7 linux-c7-curl linux-c7-icu linux-c7-openssl-libs
```

add following entries to /etc/fstab

```
linprocfs   /compat/linux/proc  linprocfs   rw  0   0`
linsysfs    /compat/linux/sys   linsysfs    rw  0   0`
tmpfs    /compat/linux/dev/shm  tmpfs   rw,mode=1777    0   0`
```

and  run  `$sudo mount -a` (or reboot)

now it should just work. but build calls 'python' regardless or PYTHON variable.
By default, FreeBSD has python2.7. One can create ~/bin/python as 

```
#!/bin/sh
python2.7 "$@"
```

Now it should be possible to run:

```
export  SSL_CERT_FILE=/usr/local/share/certs/ca-root-nss.crt
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0


./init-tools.sh
```
If everything is OK, Linux tools and SDK should just work...
```
 Unsupported OS 'FreeBSD' detected. Downloading linux-x64 tools.`
 Installing dotnet cli...`
 Restoring BuildTools version 2.2.0-preview1-02810-02...`
 Using RID linux-x64 for BuildTools native tools`
 Initializing BuildTools...`
 Making all .sh files executable under Tools.`
 Unsupported OS FreeBSD detected. Skipping crossgen of the toolset.
 Done initializing tools.`
```

now, we need to fix run.sh and Tools/msbuild.sh to use #!/usr/local/bin/bash 
There are two problems on FreeBSD: 
- /bin/bash does not exit.
- Process running under Linux emulation is failing to execute FreeBSD code:
 (run /compat/linux/bin/bash and try ./run.sh)

(there may be some other solution (BSD gurus please chime in)) 

`./build.sh -skiptests -msbuildonunsupportedplatform  `

This should produce /mnt/coreclr-master/bin/Product/FreeBSD.x64.Debug/System.Private.CoreLib.dll
Note that build 'fails' even if the library is created. That will need some investigation. 
(note that console will show error about 'syscall sendfile not implemented'. There is fallback if the system call fails (for compatibility with Linux 2.2))
 

### Verification and Testing
If everything is OK, one should be able to run simple assembly on FreeSBD now.

To run PAL tests do from coreclr root:

```
./src/pal/tests/palsuite/runpaltests.sh $PWD/bin/obj/FreeBSD.x64.Debug $PWD/bin/paltestout
```

It is important to use absolute path for the parameters e.g. ./bin/XX does not work

To build managed tests we will again need OS capable of producing managed assemblies.
On Linux one can run

```
./build-test.sh
```

and that will create test assemblies. This step will take LONG time and it will needed resources. 
It was failing on 2 core systems with 2G of RAM. 4G seems ok. It should finish with something like:

```
Command execution succeeded.
Command successfully completed.
Test build successful.
Test binaries are available at /home/furt/git/wfurt-coreclr/bin/tests/Linux.x64.Debug

To run all tests use 'tests/runtests.sh' where:
    testRootDir      = /home/furt/git/wfurt-coreclr/bin/tests/Linux.x64.Debug
    coreClrBinDir    = /home/furt/git/wfurt-coreclr/bin/Product/Linux.x64.Debug
    coreFxBinDir     = /home/furt/git/wfurt-coreclr/bin/tests/Linux.x64.Debug/Tests/Core_Root
    testNativeBinDir = /home/furt/git/wfurt-coreclr/bin/obj/Linux.x64.Debug/tests
```

[Top](#home)

## Building CoreFX

CoreFX provides set of libraries providing compliance with .net standard - 2.0 in current state. 
That is essentially needed to run any decent application. The challenges here are similar to challenges to building coreCLR. However build of corefx has some preliminary support for cross-targeting platforms. 
Some managed code in CoreFX has OS specific implementations and we need to pick proper one to run properly on FreeBSD. 

### Building native components

To Build native parts on FreeBSD

```
./src/Native/build-native.sh -clang3.9
```

### Building managed components on Windows

To build the manage components in Windows please follow the instructions here:
Framework (corefx):
https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md

### Building managed components on GNU/Linux

To build managed parts on GNU/Linux
: 
```
./build.sh -OS=FreeBSD -buildtests -skiptests -SkipManagedPackageBuild [-release]
```
This will produce assemblies under `bin/*`. `-buildtests` can be omitted if there is no intention to execute test on FreeBSD. Copy everything from `bin/*` to your FreeBSD system.

Just to be sure, remove any possible Linux binaries. 
```
rm -rf bin/obj/FreeBSD.x64.Debug bin/FreeBSD.x64.Debug/native
```
Cmake may get confused is it sees old artifacts. 

### Testing the SDK

Steps bellow need changes from wfurt/corefx. Pull from there or get patches. 
For now, there are some unusual steps. The copy instructions assume core-setup, corefx and coreclr are in same diectory next to each other. 

[Top](#home)

##### core-setup

```
cd core-setup
mkdir -p  bin/obj
echo 'static char sccsid[] __attribute__((used)) = "stub";' > bin/obj/version.cpp
sed -I old  's@../../../version.cpp@bin/obj/version.cpp@' src/corehost/CMakeLists.txt
src/corehost/build.sh --arch x64 --hostver "2.0.0" --apphostver "2.0.0" --fxrver "2.0.0" --policyver "2.0.0" --commithash `git rev-parse HEAD`
```
This will produce native FreeBSD dotnet binary, libhostfxr.so and libhostpolicy

With that, go back to corefx tree:
```
cp ../core-setup/cli/exe/dotnet/dotnet bin/testhost/netcoreapp-FreeBSD-Debug-x64/dotnet
cp ../core-setup/cli/fxr/libhostfxr.so bin/testhost/netcoreapp-FreeBSD-Debug-x64/host/fxr/9.9.9/libhostfxr.so
cp ../core-setup/cli/dll/libhostpolicy.so bin/testhost/netcoreapp-FreeBSD-Debug-x64/shared/Microsoft.NETCore.App/9.9.9/libhostpolicy.so

cp bin/FreeBSD.x64.Debug/native/*so bin/testhost/netcoreapp-FreeBSD-Debug-x64/shared/Microsoft.NETCore.App/9.9.9/
cp ../coreclr/bin/Product/FreeBSD.x64.Debug/*so bin/testhost/netcoreapp-FreeBSD-Debug-x64/shared/Microsoft.NETCore.App/9.9.9
cp ../coreclr/bin/Product/FreeBSD.x64.Debug/System.Private.CoreLib.dll  bin/testhost/netcoreapp-FreeBSD-Debug-x64/shared/Microsoft.NETCore.App/9.9.9
```

Get `https://github.com/wfurt/blob/blob/master/Microsoft.NETCore.App.deps.json` and replace original version in `bin/testhost/netcoreapp-FreeBSD-Debug-x64/shared/Microsoft.NETCore.App/9.9.9`

Correct TestPlatform
```
find bin/ -name RunTests.sh | xargs -I@@ sed -I .old 's/nonlinuxtests/nonfreebsdtests/' @@
```

Now, you should be able to run `run-test.sh` or 'RunTests.sh' under `bin/AnyOS.AnyCPU.Debug` directory.
Second needs argument pointing at `bin/testhost/netcoreapp-FreeBSD-Debug-x64`.

[Top](#home)

## Troubleshooting and diag

### Useful variables
```
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true # skips creating NuGet package cache when running dotnet cli 
export DOTNET_CLI_TELEMETRY_OPTOUT=1          # disables telemetry - broken on FreeBSD
export COREHOST_TRACE=1                       # traces loading of assemblies and other useful info 
export COMPlus_ZapDisable=1                   # ignores native code in assemblies 
export BUILDTOOLS_SKIP_CROSSGEN=1             # attempt to NOT cross-gen - works only in some repos
```

[Top](#home)

## Tips and Tricks

### Running FreeBSD on Windows 10

A minimal FreeBSD image can be downloaded and installed and runs well under [Oracle Virtual Box](https://www.virtualbox.org/), but shared folders are not supported. To share data between the Windows 10 host and FreeBSD VM, mount any shared folder (or a Windows users' home directory) via [mount_smbfs(8)](https://www.freebsd.org/cgi/man.cgi?mount_smbfs). A password is required is there is no guest access to the folder. 

To enable sharing between the Host and the VM, Open the FreeBSD virtual machine Settings and add a second network adapter as a [host only adapter](https://www.virtualbox.org/manual/ch06.html#network_hostonly). Once this is set up, any share can be mounted from within FreeBSD. An example mount command to a users home directory:

```
sudo mount_smbfs -I 192.168.56.1 //russh@layne/Users/russh/ mnt/
```
This example will prompt for the *Windows users'* password. mount_smbfs(8) indicates that an RC file can be added to the FreeBSD users home directory for use with fstab, but has not yet been tested. 

### Building FreeBSD SDK
This is highly experimental set of hacks and it may or may not work at any given time. It assumes recent versions versions of the SDK as old code base does not have all the fixes to build and run the SDK.
It is based on boot-strap tool described here https://github.com/dotnet/source-build/blob/dev/release/2.0/Documentation/boostrap-new-os.md (https://github.com/dotnet/source-build/blob/dev/release/2.0/scripts/bootstrap/buildbootstrapcli.sh) 

This script allows to to take published .NET SDK and replace native parts of it so it can possibly run on other OS. This was primarily used to get .NET on new Linux distributions without compatible environment. It does not produce functional SDK but it creates very good start. This is transient and it will eventually be replaced by build-from-source project. 

The example below used FreeBSD 11 system, Ubuntu Server 16.04 and 2.2.0-preview1-007816 CLI as base line.  
(not that 16.10 Ubuntu must not be used because it will fail) 
1. get baseline Linux SDK - daily builds are published here: https://github.com/dotnet/cli
2. rebuild native FreeBSD parts (run command bellow on FreeBSD)
`./buildbootstrapcli.sh -release -os FreeBSD -rid freebsd.11-x64 -seedcli ~/dotnet-2.2.0-preview1-007816`
That should checkout code and rebuild native FreeBSD parts

now sdk should run (needs COMPlus_ZapDisable=1 COMPlus_ReadyToRun=0) but it will not function correctly.
This is because managed parts assume Linux.

`dotnet --info` should show information about runtime and sdk.

If you get arror about parsing JSON file, remove extra 'n' characters from `shared/Microsoft.NETCore.App/<version>/Microsoft.NETCore.App.deps.json` file.
The sed from script does not correctly process '\n'

3. rebuild mscorelib
* checkout coreclr to same submit hash as presented by bootstrap script
* ./build.sh -clang3.9 -skipcrossgen -osgroup FreeBSD -release
* copy corelib to FreeBSD:

`scp linux:coreclr/bin/obj/FreeBSD.x64.Debug/System.Private.CoreLib/System.Private.CoreLib.dll freebsd.11-x64/dotnetcli/shared/Microsoft.NETCore.App/2.1.0-preview1-26013-05/System.Private.CoreLib.dll `

4. rebuild corefx managed parts on Linux (same instructions as above) 

```
./build.sh -OS=FreeBSD -buildtests -skiptests -SkipManagedPackageBuild -release
rm -f bin/testhost/netcoreapp-FreeBSD-Release-x64/shared/Microsoft.NETCore.App/9.9.9/System.Private.CoreLib.*
scp bin/testhost/netcoreapp-FreeBSD-Release-x64/shared/Microsoft.NETCore.App/9.9.9/*.dll dll dotnetcli/shared/Microsoft.NETCore.App/2.1.0-preview1-26013-05
```

5. set directory on path
```
mkdir foo
cd foo
export COMPlus_ZapDisable=1
export COMPlus_ReadyToRun=0
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
dotnet new console 
dotnet restore
dotnet build
dotnet publish
dotnet --fx-version 2.1.0-preview1-26013-05  bin/Debug/netcoreapp2.0/foo.dll
Hello World!
```

Note that first two environmental variables are currently mandatory (see cross-gen notes above) 



[Top](#home)

## Updates

* 10/04/17: tw - merges done for coreclr and core-setp. Simplified instructions for coreclr, syntax change for osgroup
* 11/08/17: rh - re-org. Added Windows build links
* 12/21/17: tw - add notes how to create Frankenstein SDK
* 05/09/18: tw - FreeBSD build broken note.
* 05/22/18: tw - notes how to build managed code on FreeBSD
* 06/15/18: mrm - updates to the Building FreeBSD SDK section steps

[Top](#home)