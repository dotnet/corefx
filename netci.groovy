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
                  'LinuxARMEmulator': 'Linux']

// Map of osName -> nuget runtime
def targetNugetRuntimeMap = ['OSX' : 'osx.10.10-x64',
                             'Ubuntu14.04' : 'ubuntu.14.04-x64',
                             'Ubuntu16.04' : 'ubuntu.16.04-x64',
                             'Ubuntu16.10' : 'ubuntu.16.10-x64',
                             'Fedora23' : 'fedora.23-x64',
                             'Fedora24' : 'fedora.24-x64',
                             'Debian8.4' : 'debian.8-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'OpenSUSE13.2' : 'opensuse.13.2-x64',
                             'OpenSUSE42.1' : 'opensuse.42.1-x64',
                             'RHEL7.2': 'rhel.7-x64']

def osShortName = ['Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Ubuntu16.04' : 'ubuntu16.04',
                   'Ubuntu16.10' : 'ubuntu16.10',
                   'CentOS7.1' : 'centos7.1',
                   'Debian8.4' : 'debian8.4',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'OpenSUSE42.1' : 'opensuse42.1',
                   'Fedora23' : 'fedora23',
                   'Fedora24' : 'fedora42',
                   'RHEL7.2' : 'rhel7.2']

// **************************
// Define code coverage build
// **************************

[true, false].each { isPR ->
    ['local', 'nonlocal'].each { localType ->
        def isLocal = (localType == 'local')

        def newJobName = 'code_coverage_windows'
        def batchCommand = 'call build.cmd -coverage -outerloop -- /p:WithoutCategories=IgnoreForCI'
        if (isLocal) {
            newJobName = "${newJobName}_local"
            batchCommand = "${batchCommand} /p:TestWithLocalLibraries=true"
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
// Define outerloop testing for OSes that can build and run.  Run locally on each machine.
// **************************
[true, false].each { isPR ->
    ['Windows 7', 'Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Ubuntu16.10', 'CentOS7.1', 'OpenSUSE13.2', 'OpenSUSE42.1', 'RHEL7.2', 'Fedora23', 'Fedora24', 'Debian8.4', 'OSX'].each { osName ->
        ['Debug', 'Release'].each { configurationGroup ->

            def newJobName = "outerloop_${osShortName[osName]}_${configurationGroup.toLowerCase()}"

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                steps {
                    if (osName == 'Windows 7' || osName == 'Windows_NT') {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -${configurationGroup} -outerloop -- /p:WithoutCategories=IgnoreForCI")
                    }
                    else if (osName == 'OSX') {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} -outerloop -testWithLocalLibraries -- /p:WithoutCategories=IgnoreForCI")
                    }
                    else {
                        shell("sudo HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} -outerloop -testWithLocalLibraries -- /p:TestNugetRuntimeId=${targetNugetRuntimeMap[osName]} /p:WithoutCategories=IgnoreForCI")
                    }
                }
            }

            // Set the affinity.  OS name matches the machine affinity.
            if (osName == 'Windows_NT' || osName == 'OSX') {
                Utilities.setMachineAffinity(newJob, osName, "latest-or-auto-elevated")
            }
            else if (osGroupMap[osName] == 'Linux') {
                Utilities.setMachineAffinity(newJob, osName, 'outer-latest-or-auto')
            } else {
                Utilities.setMachineAffinity(newJob, osName, 'latest-or-auto');
            }

            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
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
// Define innerloop testing.  These jobs run on every merge and a subset of them run on every PR, the ones
// that don't run per PR can be requested via a magic phrase.
// **************************
[true, false].each { isPR ->
    ['Debug', 'Release'].each { configurationGroup ->
        ['Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Ubuntu16.10', 'Debian8.4', 'CentOS7.1', 'OpenSUSE13.2', 'OpenSUSE42.1', 'Fedora23', 'Fedora24', 'RHEL7.2', 'OSX'].each { osName ->
            def osGroup = osGroupMap[osName]
            def newJobName = "${osName.toLowerCase()}_${configurationGroup.toLowerCase()}"

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                // On Windows we use the packer to put together everything. On *nix we use tar
                steps {
                    if (osName == 'Windows 7' || osName == 'Windows_NT') {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -${configurationGroup} -os=${osGroup} -- /p:WithoutCategories=IgnoreForCI")
                        batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                    }
                    else {
                        // Use Server GC for Ubuntu/OSX Debug PR build & test
                        def useServerGC = (configurationGroup == 'Release' && isPR) ? 'useServerGC' : ''
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} -testWithLocalLibraries -- ${useServerGC} /p:TestNugetRuntimeId=${targetNugetRuntimeMap[osName]} /p:WithoutCategories=IgnoreForCI")
                        // Tar up the appropriate bits.  On OSX the tarring is a different syntax for exclusion.
                        if (osName == 'OSX') {
                            shell("tar -czf bin/build.tar.gz --exclude *.Tests bin/*.${configurationGroup} bin/ref bin/packages")
                        }
                        else {
                            shell("tar -czf bin/build.tar.gz bin/*.${configurationGroup} bin/ref bin/packages --exclude=*.Tests")
                        }
                    }
                }
            }

            // Set the affinity.
            Utilities.setMachineAffinity(newJob, osName, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
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
                // Set PR trigger, we run Windows_NT, Ubuntu 14.04, CentOS 7.1 and OSX on every PR.
                if ( osName == 'Windows_NT' || osName == 'Ubuntu14.04' || osName == 'CentOS7.1' || osName == 'OSX' ) {
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
            def osGroup = osGroupMap[osName]
            def newJobName = "${osName.toLowerCase()}_cross_${configurationGroup.toLowerCase()}"
            def arch = "arm-softfp"

        // Setup variables to hold emulator folder path and the rootfs mount path
        def armemul_path = '/opt/linux-arm-emulator'
        def armrootfs_mountpath = '/opt/linux-arm-emulator-root'

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                steps {
                    // Call the arm32_ci_script.sh script to perform the cross build of native corefx
                    shell("./scripts/arm32_ci_script.sh --emulatorPath=${armemul_path} --mountPath=${armrootfs_mountpath} --buildConfig=${configurationGroup.toLowerCase()} --verbose")

                    // Archive the native and managed binaries
                    shell("tar -czf bin/build.tar.gz bin/*.${configurationGroup} bin/ref bin/packages --exclude=*.Tests")
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
                if (osName == 'LinuxARMEmulator') {
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop Linux ARM Emulator ${configurationGroup} Cross Build")
                }
            }
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}

JobReport.Report.generateJobReport(out)

// Make the call to generate the help job
Utilities.createHelperJob(this, project, branch,
    "Welcome to the ${project} Repository",  // This is prepended to the help message
    "Have a nice day!")  // This is appended to the help message.  You might put known issues here.
