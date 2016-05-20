Package and File Versioning
===========================

We have now added package versioning and assembly file versioning when building the repo. This versioning works in both Windows and non-Windows platforms. Also, this versioning applies for both the managed and native libraries we produce. We had a few constrains to take into account, for example:
- Version needed to be higher than the versions that we have already shipped.
- There needs to be an ability to have multiple versions per day.
- Versions need to be always increasing.
- Version needs to be lower than [SHRT_MAX](https://www.google.com/?ion=1&espv=2#q=max%20short%20int) since the version is used as assembly file version which has that constrain.
- (Nice to have) Version needs to be reproducible.
- (Nice to have) We shouldn't have the need to check in a file containing the version or revision number.

Taking into account all these constrains we came up with the new versioning. The version is calculated per build by [a targets file that lives in buildtools](https://github.com/dotnet/buildtools/blob/master/src/Microsoft.DotNet.Build.Tasks/PackageFiles/versioning.targets), and depending on what we are building(native/managed, windows/non-windows) it will generate versioning assets that will be used during the build to be able to produce build output with the right versions.

How is the version calculated
=============================

The version is composed by two parts; the build number major and the build number minor. An example looks something like 00001.01, where the first five digits represent the build number major, and the two last digits after the dot represent the build number minor.

Calculating BuildNumberMajor
----------------------------

The BuildNumberMajor is represented by 5 digits, and is determined by calculating the time that has happened between the latest commit's date and a SeedDate that gets passed in. The first portion of the BuildNumberMajor (first 3 digits) represent the number of month(s) since the SeedDate. The second portion of the BuildNumberMajor (last 2 digits) represent the day of the month of the latest commit's date. This part of the version is reproducible. 

Calculating BuildNumberMinor
----------------------------

The BuildNumberMinor represent the build # of the day this version is. This number increases by 1, and it starts in 01. Given that there is no way for a dev machine to know how many builds have happened that day, this number is calculated by our VSO official builds and needs to be passed in. Since dev builds usually don't have a BuildNumberMinor (unless it's passed in manually), they will always have a BuildNumberMinor=0. For example, a Official build version would look something like `00001.01` while a dev build version will look like `00001.0` 

Getting the version of a native binary in non-windows platforms
========================================================

When trying to get a version of a native binary in non-windows, there are two ways to do it:
- Using the Linux tool `what(1)` by installing the tool (if it doesn't come by default with the OS) and then running something like `what myNativeNonWindowsBinary.so`
- The other way to do it if you don't want to install additional tools, is by using `strings+grep`, and the way to do it is by running `strings myNativeNonWindowsBinary.so | grep "@(#)"`

How to force a dev build to produce a specific version
======================================================

If you need to manually specify the version you want to produce your build output with, you can accomplish this by running the following from the root of the repo: 
- `build.cmd /p:BuildNumberMajor=00001 /p:BuildNumberMinor=01` in Windows
- `build.sh /p:BuildNumberMajor=00001 /p:BuildNumberMinor=01` in non-Windows