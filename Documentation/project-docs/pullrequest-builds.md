# Pull Request Builds

As part of our Pull Requests we have some validation builds where we build the product and run tests on different Operative Systems and Build Configurations.

### Build Matrix

| Build | Framework | Builds Tests | Runs Tests |
|:---|:---:|:---:|:---:|
| Windows x86_Release | netcoreapp | X | X |
| Windows x64_Debug | netcoreapp | X | X |
| Windows NETFX_x86_Release | netfx | X | X |
| Windows UWP_CoreCLR_x64_Debug | uap | X | X |
| Windows UWP_NETNative_x86_Release | uapaot | X |   |
| Linux x64_Release | netcoreapp | X | X |
| Linux arm64_Release | netcoreapp | X | X |
| Linux arm_Release | netcoreapp | X |   |
| Linux musl_x64_Debug | netcoreapp | X |   |
| MacOS x64_Debug | netcoreapp | X |   |
| Packaging All Configurations x64_Debug | all | X | X |

Our build definitions are defined by some .yml files with the following structure:

- Entrypoint `.azure-pipelines.yml`
- Shared build steps `eng/pipelines/corefx-base.yml`
- Helix step `eng/pipelines/helix.yml`
- Windows Matrix `eng/pipelines/windows.yml`
- Linux Matrix `eng/pipelines/linux.yml`
- MacOS Matrix `eng/pipelines/macos.yml`

## Available builds

- corefx-ci (Runs by default on every commit)
- corefx-outerloop-linux
- corefx-outerloop-osx
- corefx-outerloop-windows

### Azure pipelines commands

```
/azp help (displays help message)
/azp run (runs all builds listed above)
/azp run <build name> (/azp run corefx-outerloop-linux)
```

`/azp` is a shortcut for `/AzurePipelines` which can be used as well.

In order to use comment triggers, at the moment you need to be a `Contributor` or `Member` of the repo.

## How to look at a build failure

First, you need to click in the `details` button on the right hand side of the build you're interested in looking at. That will redirect you to the `Checks` tab within that PR, which will have a summary of that build.

To get to the actual build, you will need an extra step here. You have two options:
   1. Click on `View more details on Azure Pipelines`:
   2. Click on `errors / warnings` summary hyperlink.

Once in the build UI you can look at a specific job, or step log, by clicking on them directly.

## How to look at a test failure

Currently, our test results are exposed by https://mc.dot.net/ as we wait for new features by Azure DevOps to use their text explorer. In order to get to the test results, you need to click on the `Send to Helix` step on a job, and within its logs, there will be a text containing the test results URL, which you can `CTRL+Click` to open. It will look like the following:
```
Results will be available from https://mc.dot.net/#/user/dotnet-bot/pr~2Fdotnet~2Fcorefx~2Frefs~2Fpull~2F35667~2Fmerge/test~2Ffunctional~2Fcli~2F/20190228.23
```

Then on Mission Control, you can just navigate through the results and look at stack traces, and detailed logs.

## How to rerun builds

In order to rerun a build, it can be done through comments, the checks tab through the `rerun` button, or in the build UI on the right top corner, under the `...` submenu you will find a `retry` button.

Unfortunately, `rerun` button on checks tab and `retry` button in build's UI are only available for Microsoft people.

## Access a build binlog

We publish binlogs as part of our PR builds to make weird build errors easier to diagnose. In order to find these, you can go to the build UI and find them pretty easily. 

  1. If the build is still running, you can click on the `...` submenu and there will be an `Artifacts` entry.
  2. If the build is done, on the top right corner, there will be a big blue button called `Artifacts`.

Once you click under the `Artifacts` section, a popup will show, with multiple directories, one for each build leg. Under this directories, you will find every binlog we produce as part of our builds.

# Known Issues

  1. Multiline comments are not supported, so if you want to trigger multiple builds, you have to do it on separate comments.
  2. Feature request: trigger multiple builds with one `/azp` call. I.e `/azp run corefx-ci, corefx-outerloop-osx`.
  3. Duplicated jobs are shown on PR badges when a build is `rerun`.
  4. Feature request: `/azp help` doesn't list available builds for the repo.
  5. Too much clicks to get to the build UI.
  6. Mobile experience is really bad.

All of this issues have been raised to Azure DevOps teams, expect @safern, to update the docs through PR to widely communicate new features coming.