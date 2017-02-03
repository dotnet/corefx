// Import the utility functionality.

import jobs.generation.Utilities;
import jobs.generation.JobReport;

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName
// Folder that the project jobs reside in (project/branch)
def projectFolder = Utilities.getFolderName(project) + '/' + Utilities.getFolderName(branch)

// Globals

// Map of osName -> osGroup.
def osGroupMap = ['Ubuntu14.04':'Linux',
                  'Ubuntu16.04':'Linux',
                  'Ubuntu16.10':'Linux',
                  'Debian8.4':'Linux',
                  'Fedora23':'Linux',
                  'Fedora24':'Linux',
                  'OSX':'OSX',
                  'Windows_NT':'Windows_NT',
                  'CentOS7.1': 'Linux',
                  'OpenSUSE13.2': 'Linux',
                  'OpenSUSE42.1': 'Linux',
                  'RHEL7.2': 'Linux',
                  'LinuxARMEmulator': 'Linux',
                  'PortableLinux': 'Linux']

def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Windows Nano 2016' : 'winnano16',
                   'Ubuntu16.04' : 'ubuntu16.04',
                   'Ubuntu16.10' : 'ubuntu16.10',
                   'CentOS7.1' : 'centos7.1',
                   'Debian8.4' : 'debian8.4',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'OpenSUSE42.1' : 'opensuse42.1',
                   'Fedora23' : 'fedora23',
                   'Fedora24' : 'fedora24',
                   'RHEL7.2' : 'rhel7.2',
                   'PortableLinux' : 'portablelinux']

def buildArchConfiguration = ['Debug': 'x86',
                              'Release': 'x64']

// **************************
// Define code coverage build
// **************************

[true, false].each { isPR ->
    ['local', 'nonlocal'].each { localType ->
        def isLocal = (localType == 'local')

        def newJobName = 'code_coverage_windows'
        def batchCommand = 'call build.cmd && call build-tests.cmd -coverage -outerloop -- /p:WithoutCategories=IgnoreForCI'
        if (isLocal) {
            newJobName = "${newJobName}_local"
            batchCommand = "${batchCommand}"
        }
        def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
            steps {
                batchFile(batchCommand)
            }
        }

        // Set up standard options
        Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
        // Set the machine affinity to windows machines
        Utilities.setMachineAffinity(newJob, 'Windows_NT', 'latest-or-auto')
        // Publish reports
        Utilities.addHtmlPublisher(newJob, 'bin/tests/coverage', 'Code Coverage Report', 'index.htm')
        // Archive results.
        Utilities.addArchival(newJob, '**/coverage/*,msbuild.log')
        // Timeout. Code coverage runs take longer, so we set the timeout to be longer.
        Utilities.setJobTimeout(newJob, 180)
        // Set triggers
        if (isPR) {
            if (!isLocal) {
                // Set PR trigger
                Utilities.addGithubPRTriggerForBranch(newJob, branch, 'Code Coverage Windows Debug', '(?i).*test\\W+code\\W+coverage.*')
            }
        }
        else {
            // Set a periodic trigger
            Utilities.addPeriodicTrigger(newJob, '@daily')
        }
    }
}

// **************************
// Define code formatter check build
// **************************

[true, false].each { isPR ->
    def newJob = job(Utilities.getFullJobName(project, 'native_code_format_check', isPR)) {
        steps {
            shell('python src/Native/Unix/format-code.py checkonly')
        }
    }

    // Set up standard options.
    Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
    // Set the machine affinity to Ubuntu14.04 machines
    Utilities.setMachineAffinity(newJob, 'Ubuntu14.04', 'latest-or-auto')
    if (isPR) {
        // Set PR trigger.  Only trigger when the phrase is said.
        Utilities.addGithubPRTriggerForBranch(newJob, branch, 'Code Formatter Check', '(?i).*test\\W+code\\W+formatter\\W+check.*', true)
    }
    else {
        // Set a push trigger
        Utilities.addGithubPushTrigger(newJob)
    }
}

// **************************
// Define outerloop windows Nano testing.  Run locally on each machine.
// **************************
[true, false].each { isPR ->
    ['Windows Nano 2016'].each { osName ->
        ['Debug', 'Release'].each { configurationGroup ->

            def newJobName = "outerloop_${osShortName[osName]}_${configurationGroup.toLowerCase()}"

            def newBuildJobName = "outerloop_${osShortName[osName]}_${configurationGroup.toLowerCase()}_bld"

            def newBuildJob = job(Utilities.getFullJobName(project, newBuildJobName, isPR)) {
                steps {
                    batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -os=Windows_NT -${configurationGroup} -skipTests -outerloop -- /p:WithoutCategories=IgnoreForCI")
                    // Package up the results.
                    batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack . bin packages")
                }
            }

            // Set the affinity.  All of these run on Windows currently.
            Utilities.setMachineAffinity(newBuildJob, 'Windows_NT', 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newBuildJob, project, isPR, "*/${branch}")
            // Archive the results
            Utilities.addArchival(newBuildJob, "bin/build.pack,run-test.cmd,msbuild.log")

            def fullCoreFXBuildJobName = projectFolder + '/' + newBuildJob.name
            def newTestJobName =  "outerloop_${osShortName[osName]}_${configurationGroup.toLowerCase()}_tst"
            def newTestJob = job(Utilities.getFullJobName(project, newTestJobName, isPR)) {
                steps {
                    // The tests/corefx components
                    copyArtifacts(fullCoreFXBuildJobName) {
                        includePatterns('bin/build.pack')
                        includePatterns('run-test.cmd')
                        buildSelector {
                            buildNumber('\${COREFX_BUILD}')
                        }
                    }

                    // Unpack the build data
                    batchFile("PowerShell -command \"\"C:\\Packer\\unpacker.ps1 .\\bin\\build.pack . > .\\bin\\unpacker.log\"\"")
                    // Run the tests
                    batchFile("run-test.cmd .\\bin\\tests\\Windows_NT.AnyCPU.${configurationGroup} %WORKSPACE%\\packages")
                    // Run the tests
                    batchFile("run-test.cmd .\\bin\\tests\\AnyOS.AnyCPU.${configurationGroup} %WORKSPACE%\\packages")
                }

                parameters {
                    stringParam('COREFX_BUILD', '', 'Build number to use for copying binaries for nano server bld.')
                }
            }

            // Set the affinity.  All of these run on Windows Nano currently.
            Utilities.setMachineAffinity(newTestJob, osName)
            // Set up standard options.
            Utilities.addStandardOptions(newTestJob, isPR)
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newTestJob, 'bin/**/testResults.xml')

            def fullCoreFXTestJobName = projectFolder + '/' + newTestJob.name
            def newJob = buildFlowJob(Utilities.getFullJobName(project, newJobName, isPR)) {
                buildFlow("""
                    b = build(params, '${fullCoreFXBuildJobName}')
                    build(params +
                    [COREFX_BUILD: b.build.number], '${fullCoreFXTestJobName}')
                    """)
            }

            // Set the machine affinity to windows_nt, since git fails on Nano.
            Utilities.setMachineAffinity(newJob, 'Windows_NT', 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")

            // Set up appropriate triggers.  PR on demand, otherwise nightly
            if (isPR) {
                // Set PR trigger.
                // TODO: More elaborate regex trigger?
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${osName} ${configurationGroup}", "(?i).*test\\W+outerloop\\W+${osName}\\W+${configurationGroup}.*")
            }
            else {
                // Set a periodic trigger
                Utilities.addPeriodicTrigger(newJob, '@daily')
            }
        }
    }
}

// **************************
// Define outerloop testing for OSes that can build and run.  Run locally on each machine.
// **************************
[true, false].each { isPR ->
    ['Windows 10', 'Windows 7', 'Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Ubuntu16.10', 'CentOS7.1', 'OpenSUSE13.2', 'OpenSUSE42.1', 'RHEL7.2', 'Fedora23', 'Fedora24', 'Debian8.4', 'OSX', 'PortableLinux'].each { osName ->
        ['Debug', 'Release'].each { configurationGroup ->

            def osForMachineAffinity = osName
            if (osForMachineAffinity == 'PortableLinux') {
                // Portable Linux builds happen on RHEL7.2
                osForMachineAffinity = "RHEL7.2"
            }

            def newJobName = "outerloop_${osShortName[osName]}_${configurationGroup.toLowerCase()}"

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                steps {
                    if (osName == 'Windows 10' || osName == 'Windows 7' || osName == 'Windows_NT') {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -${configurationGroup}")
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build-tests.cmd -${configurationGroup} -outerloop -- /p:WithoutCategories=IgnoreForCI")
                    }
                    else if (osName == 'OSX') {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()}")
                        shell("HOME=\$WORKSPACE/tempHome ./build-tests.sh -${configurationGroup.toLowerCase()} -outerloop -- /p:WithoutCategories=IgnoreForCI")
                    }
                    else {
                        def portableLinux = (osName == 'PortableLinux') ? '-portableLinux' : ''
                        shell("sudo HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} ${portableLinux}")
                        shell("sudo HOME=\$WORKSPACE/tempHome ./build-tests.sh -${configurationGroup.toLowerCase()} -outerloop -- /p:WithoutCategories=IgnoreForCI")
                    }
                }
            }

            // Set the affinity.  OS name matches the machine affinity.
            if (osName == 'Windows_NT' || osName == 'OSX') {
                Utilities.setMachineAffinity(newJob, osForMachineAffinity, "latest-or-auto-elevated")
            }
            else if (osGroupMap[osName] == 'Linux') {
                Utilities.setMachineAffinity(newJob, osForMachineAffinity, 'outer-latest-or-auto')
            } else {
                Utilities.setMachineAffinity(newJob, osForMachineAffinity, 'latest-or-auto');
            }

            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/**/testResults.xml')
            // Add archival for the built data.
            Utilities.addArchival(newJob, "msbuild.log", '', doNotFailIfNothingArchived=true, archiveOnlyIfSuccessful=false)
            // Set up appropriate triggers.  PR on demand, otherwise nightly
            if (isPR) {
                // Set PR trigger.
                // TODO: More elaborate regex trigger?
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${osName} ${configurationGroup}", "(?i).*test\\W+outerloop\\W+${osName}\\W+${configurationGroup}.*")
            }
            else {
                // Set a periodic trigger
                Utilities.addPeriodicTrigger(newJob, '@daily')
            }
        }
    }
}

// **************************
// Define perf testing.  Built locally and submitted to Helix.
// **************************

// builds with secrets should never be available for pull requests.
// right now perf tests are only run on Win10 (but can be built on any Windows)
['Windows 10'].each { osName ->
    ['Debug', 'Release'].each { configurationGroup ->

        def newJobName = "perf_${osShortName[osName]}_${configurationGroup.toLowerCase()}"

        def newJob = job(Utilities.getFullJobName(project, newJobName, /* isPR */ false)) {
            steps {
                helix("Build.cmd -- /p:Creator=dotnet-bot /p:ArchiveTests=true /p:ConfigurationGroup=${configurationGroup} /p:Configuration=Windows_${configurationGroup} /p:TestDisabled=true /p:EnableCloudTest=true /p:BuildMoniker={uniqueId} /p:TargetQueue=Windows.10.Amd64 /p:TestProduct=CoreFx /p:Branch=master /p:OSGroup=Windows_NT /p:CloudDropAccountName=dotnetbuilddrops /p:CloudResultsAccountName=dotnetjobresults /p:CloudDropAccessToken={CloudDropAccessToken} /p:CloudResultsAccessToken={CloudResultsAccessToken} /p:BuildCompleteConnection={BuildCompleteConnection} /p:BuildIsOfficialConnection={BuildIsOfficialConnection} /p:DocumentDbKey={DocumentDbKey} /p:DocumentDbUri=https://hms.documents.azure.com:443/ /p:FuncTestsDisabled=true /p:Performance=true")
            }
        }

        Utilities.setMachineAffinity(newJob, 'Windows_NT', 'latest-or-auto')

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, /* isPR */ false, "*/${branch}")

        // Set a periodic trigger
        Utilities.addPeriodicTrigger(newJob, '@daily')

        Utilities.addPrivatePermissions(newJob)
    }
}

// **************************
// Define ARM64 testing.  Built locally and submitted to lab machines
// **************************
['Windows_NT'].each { os ->
    ['Debug', 'Release'].each { configurationGroup ->
        def newJobName = "arm64_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
        def arm64Users = ['ianhays', 'kyulee1', 'gkhanna79', 'weshaggard', 'stephentoub', 'rahku', 'ramarag']
        def newJob = job(Utilities.getFullJobName(project, newJobName, /* isPR */ false)) {
            steps {
                // build the world, but don't run the tests
                batchFile("sync.cmd -p")
                batchFile("build-native.cmd  -${configurationGroup} -buildArch=arm64 -- toolsetDir=C:\\ats2")
                batchFile("build-managed.cmd -${configurationGroup} -SkipTests -FilterToOSGroup=${os} -- /p:ArchiveTests=true")
                batchFile("build-tests.cmd   -${configurationGroup} -SkipTests -FilterToOSGroup=${os} -- /p:ArchiveTests=true /p:Platform=ARM64 /p:TestNugetRuntimeId=win10-arm64")
            }
            label("arm64")
            
            // Kick off the test run
            publishers {
                postBuildScripts {
                    steps {
                        // Run the tests
                        batchFile("python arm64_post_build.py -repo_root %WORKSPACE% -arch arm64 -build_type ${configurationGroup.toLowerCase()} -scenario CoreFX -key_location C:\\tools\\key.txt")
                    }
					onlyIfBuildSucceeds(true)
                    onlyIfBuildFails(false)
                }
            }
        }

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, /* isPR */ false, "*/${branch}")
        
        // Set a daily trigger
        Utilities.addPeriodicTrigger(newJob, '@daily')
        
        // Set up a PR trigger that is only triggerable by certain members
        Utilities.addPrivateGithubPRTriggerForBranch(newJob, branch, "Windows_NT ARM64 ${configurationGroup} Build and Test", "(?i).*test\\W+ARM64\\W+${os}\\W+${configurationGroup}", null, arm64Users)

        // Set up a per-push trigger
        Utilities.addGithubPushTrigger(newJob)

        // Get results
        //Utilities.addXUnitDotNETResults(newJob, 'bin/tests/testresults/**/testResults.xml')
    }
}

// **************************
// Define innerloop testing.  These jobs run on every merge and a subset of them run on every PR, the ones
// that don't run per PR can be requested via a magic phrase.
// **************************
[true, false].each { isPR ->
    ['netcoreapp'].each { targetGroup ->
        ['Debug', 'Release'].each { configurationGroup ->
            ['Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Ubuntu16.10', 'Debian8.4', 'CentOS7.1', 'OpenSUSE13.2', 'OpenSUSE42.1', 'Fedora23', 'Fedora24', 'RHEL7.2', 'OSX', 'PortableLinux'].each { osName ->
                def osGroup = osGroupMap[osName]
                def osForMachineAffinity = osName
                
                if (osForMachineAffinity == 'PortableLinux') {
                    // Portable Linux builds happen on RHEL7.2
                    osForMachineAffinity = "RHEL7.2"
                }

                def newJobName = "${osName.toLowerCase()}_${configurationGroup.toLowerCase()}"

                def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                    // On Windows we use the packer to put together everything. On *nix we use tar
                    steps {
                        if (osName == 'Windows 10' || osName == 'Windows 7' || osName == 'Windows_NT') {
                            batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -${configurationGroup} -os:${osGroup} -buildArch:${buildArchConfiguration[configurationGroup]} -framework:${targetGroup}")
                            batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build-tests.cmd -${configurationGroup} -os:${osGroup} -buildArch:${buildArchConfiguration[configurationGroup]} -framework:${targetGroup} -- /p:WithoutCategories=IgnoreForCI")
                            batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                        }
                        else {
                            // Use Server GC for Ubuntu/OSX Debug PR build & test
                            def useServerGC = (configurationGroup == 'Release' && isPR) ? 'useServerGC' : ''
                            def portableLinux = (osName == 'PortableLinux') ? '-portableLinux' : ''
                            shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} -framework:${targetGroup} -os:${osGroup} ${portableLinux}")
                            shell("HOME=\$WORKSPACE/tempHome ./build-tests.sh -${configurationGroup.toLowerCase()} -framework:${targetGroup} -os:${osGroup} -- ${useServerGC} /p:WithoutCategories=IgnoreForCI")
                            // Tar up the appropriate bits.
                            shell("tar -czf bin/build.tar.gz --directory=\"bin/runtime/${targetGroup}-${osGroup}-${configurationGroup}-x64\" .")
                        }
                    }
                }

                // Set the affinity.
                Utilities.setMachineAffinity(newJob, osForMachineAffinity, 'latest-or-auto')
                // Set up standard options.
                Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
                // Add the unit test results
                Utilities.addXUnitDotNETResults(newJob, 'bin/**/testResults.xml')
                def archiveContents = "msbuild.log"
                if (osName.contains('Windows')) {
                    // Packer.exe is a .NET Framework application. When we can use it from the tool-runtime, we can archive the ".pack" file here.
                    archiveContents += ",bin/build.pack"
                }
                else {
                    archiveContents += ",bin/build.tar.gz"
                }
                // Add archival for the built data.
                Utilities.addArchival(newJob, archiveContents, '', doNotFailIfNothingArchived=true, archiveOnlyIfSuccessful=false)
                // Set up triggers
                if (isPR) {
                    // Set PR trigger, we run Windows_NT, Ubuntu 14.04, CentOS 7.1, PortableLinux and OSX on every PR.
                    if ( osName == 'Windows_NT' || osName == 'Ubuntu14.04' || osName == 'CentOS7.1' || osName == 'OSX' || osName== 'PortableLinux') {
                        Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop ${osName} ${configurationGroup} Build and Test")
                    }
                    else {
                        Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop ${osName} ${configurationGroup} Build and Test", "(?i).*test\\W+innerloop\\W+${osName}\\W+${configurationGroup}.*")
                    }
                }
                else {
                    // Set a push trigger
                    Utilities.addGithubPushTrigger(newJob)
                }
            }
        }
    }
}

// **************************
// Define Linux ARM Emulator testing. This creates a per PR job which
// cross builds native binaries for the Emulator rootfs.
// NOTE: To add Ubuntu-ARM cross build jobs to this code, add the Ubuntu OS to the
// OS array, branch the steps to be performed by Ubuntu and the Linux ARM emulator
// based on the OS being handled, and handle the triggers accordingly
// (the machine affinity of the new job remains the same)
// **************************
[true, false].each { isPR ->
    ['Debug', 'Release'].each { configurationGroup ->
        ['LinuxARMEmulator'].each { osName ->
            ['HardFP', 'SoftFP'].each { abi ->
                def osGroup = osGroupMap[osName]
                def newJobName = "${osName.toLowerCase()}_${abi.toLowerCase()}_cross_${configurationGroup.toLowerCase()}"

                // Setup variables to hold emulator folder path and the rootfs mount path
                def armemul_path = '/opt/linux-arm-emulator'
                def armrootfs_mountpath = '/opt/linux-arm-emulator-root'

                def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                    steps {
                        // Call the arm32_ci_script.sh script to perform the cross build of native corefx
                        def script = "./scripts/arm32_ci_script.sh --emulatorPath=${armemul_path} --mountPath=${armrootfs_mountpath} --buildConfig=${configurationGroup.toLowerCase()} --verbose"
                        if (abi == "SoftFP") {
                            script += " --armel"
                        }
                        shell(script)

                        // Archive the native and managed binaries
                        shell("tar -czf bin/build.tar.gz bin/*.${configurationGroup} bin/ref --exclude=*.Tests")
                    }
                }

                // The cross build jobs run on Ubuntu. The arm-cross-latest version
                // contains the packages needed for cross building corefx
                Utilities.setMachineAffinity(newJob, 'Ubuntu14.04', 'arm-cross-latest')

                // Set up standard options.
                Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")

                // Add archival for the built binaries
                def archiveContents = "bin/build.tar.gz"
                Utilities.addArchival(newJob, archiveContents)

                // Set up triggers
                if (isPR) {
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop Linux ARM Emulator ${abi} ${configurationGroup} Cross Build", "(?i).*test\\W+innerloop\\W+linuxarmemulator\\W+${abi}\\W+${configurationGroup}.*")
                }
                else {
                    // Set a push trigger
                    Utilities.addGithubPushTrigger(newJob)
                }
            }
        }
    }
}

JobReport.Report.generateJobReport(out)

// Make the call to generate the help job
Utilities.createHelperJob(this, project, branch,
    "Welcome to the ${project} Repository",  // This is prepended to the help message
    "Have a nice day!")  // This is appended to the help message.  You might put known issues here.
