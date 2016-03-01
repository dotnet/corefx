// Import the utility functionality.

import jobs.generation.Utilities;

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName

// Globals

// Map of os -> osGroup.
def osGroupMap = ['Ubuntu':'Linux',
                  'Ubuntu15.10':'Linux',
                  'Debian8.2':'Linux',
                  'OSX':'OSX',
                  'Windows_NT':'Windows_NT',
                  'FreeBSD':'FreeBSD',
                  'CentOS7.1': 'Linux',
                  'OpenSUSE13.2': 'Linux',
                  'RHEL7.2': 'Linux']
// Map of os -> nuget runtime
def targetNugetRuntimeMap = ['OSX' : 'osx.10.10-x64',
                             'Ubuntu' : 'ubuntu.14.04-x64',
                             'Ubuntu15.10' : 'ubuntu.14.04-x64',
                             'Debian8.2' : 'ubuntu.14.04-x64',
                             'FreeBSD' : 'ubuntu.14.04-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'OpenSUSE13.2' : 'ubuntu.14.04-x64',
                             'RHEL7.2': 'rhel.7-x64']

def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Windows Nano' : 'winnano',
                   'Ubuntu15.10' : 'ubuntu15.10',
                   'CentOS7.1' : 'centos7.1',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'RHEL7.2' : 'rhel7.2']

// **************************
// Define code coverage build
// **************************

[true, false].each { isPR ->
    def newJob = job(Utilities.getFullJobName(project, 'code_coverage_windows', isPR)) {
        steps {
            batchFile('call "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\Tools\\VsDevCmd.bat" && build.cmd /p:Coverage=true')
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
    // Set triggers
    if (isPR) {
        // Set branch specific PR trigger
        Utilities.addGithubPRTriggerForBranch(newJob, branch, 'Code Coverage Windows Debug', '(?i).*test\\W+code\\W+coverage.*')
    }
    else {
        // Set a periodic trigger
        Utilities.addPeriodicTrigger(newJob, '@daily')
    }
}

// **************************
// Define code formatter check build
// **************************

[true, false].each { isPR ->
    def newJob = job(Utilities.getFullJobName(project, 'native_code_format_check', isPR))) {
        steps {
            shell('python src/Native/format-code.py checkonly')
        }
    }
    
    // Set up standard options.
    Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
    // Set the machine affinity to Ubuntu machines
    Utilities.setMachineAffinity(newJob, 'Ubuntu', 'latest-or-auto')
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
    ['Windows Nano'].each { os ->
        ['Debug', 'Release'].each { configurationGroup ->

            def isPR = (branchName == 'pr')  
            def newJobName = "outerloop_${osShortName[os]}_${configurationGroup.toLowerCase()}"
            
			def newBuildJobName = "${osShortName[os]}_${configurationGroup.toLowerCase()}_bld"

			def newBuildJob = job(Utilities.getFullJobName(project, newBuildJobName, isPR)) {
        		steps {
            		batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd /p:ConfigurationGroup=${configurationGroup} /p:SkipTests=true")
            		// Package up the results.
            		batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
        		}
			}

            // Set the affinity.  All of these run on Windows currently.
            Utilities.setMachineAffinity(newBuildJob, 'Windows_NT', 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newBuildJob, project, isPR, "*/${branch}")
            // Archive the results
            Utilities.addArchival(newBuildJob, "bin/build.pack,run-test.cmd,bin/osGroup.AnyCPU.${configurationGroup}/**,bin/ref/**,bin/packages/**,msbuild.log")
            
            def fullCoreFXBuildJobName = Utilities.getFolderName(project) + '/' + newBuildJob.name
            def newTestJobName =  "${osShortName[os]}_${configurationGroup.toLowerCase()}_tst"
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
	                batchFile("C:\\Packer\\UnPacker.exe .\\bin\\build.pack .\\bin")
	                // Run the tests
	                batchFile("run-test.cmd .\\bin\\tests\\Windows_NT.AnyCPU.${configurationGroup}")
            	}

            	parameters {
            		stringParam('COREFX_BUILD', '', 'Build number to use for copying binaries for nano server bld.')
            	}
            }

            // Set the affinity.  All of these run on Windows Nano currently.
            Utilities.setMachineAffinity(newTestJob, os)
            // Set up standard options.
            Utilities.addStandardOptions(newTestJob, isPR)

            def fullCoreFXTestJobName = Utilities.getFolderName(project) + '/' + newTestJob.name
            def newJob = buildFlowJob(Utilities.getFullJobName(project, newJobName, isPR)) {
                buildFlow("""
                    b = build(params, '${fullCoreFXBuildJobName}')
                    build(params +
                    [COREFX_BUILD: b.build.number], '${fullCoreFXTestJobName}')
                    """)
            }

            // Set the machine affinity.
            Utilities.setMachineAffinity(newJob, os)
            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')

            // Set up appropriate triggers.  PR on demand, otherwise nightly
            if (isPR) {
                // Set PR trigger.
                // TODO: More elaborate regex trigger?
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+outerloop\\W+${os}\\W+${configurationGroup}.*")
            }
            else {
                // Set a periodic trigger
                Utilities.addPeriodicTrigger(newJob, '@daily')
            }
        }
    }
}

// **************************
// Define outerloop testing.  Run locally on each machine.
// **************************
def linuxOSes = ['Ubuntu15.10', 'CentOS7.1', 'OpenSUSE13.2', 'RHEL7.2', 'Ubuntu14.04']
[true, false].each { isPR ->
    ['Windows 10', 'Windows 7', 'Windows_NT', 'Ubuntu14.04', 'OSX', 'Ubuntu15.10', 'CentOS7.1', 'OpenSUSE13.2', 'RHEL7.2'].each { os ->
        ['Debug', 'Release'].each { configurationGroup ->

            def newJobName = "outerloop_${osShortName[os]}_${configurationGroup.toLowerCase()}"

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                steps {
                    if (os == 'Windows 10' || os == 'Windows 7' || os == 'Windows_NT') {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && Build.cmd /p:ConfigurationGroup=${configurationGroup} /p:WithCategories=\"InnerLoop;OuterLoop\" /p:TestWithLocalLibraries=true")
                    }
                    else if (os == 'OSX') {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh /p:ConfigurationGroup=${configurationGroup} /p:WithCategories=\"\\\"InnerLoop;OuterLoop\\\"\" /p:TestWithLocalLibraries=true")
                    }
                    else {
                        shell("sudo HOME=\$WORKSPACE/tempHome ./build.sh /p:ConfigurationGroup=${configurationGroup} /p:WithCategories=\"\\\"InnerLoop;OuterLoop\\\"\" /p:TestWithLocalLibraries=true")    
                    }
                }
            }

            // Set the affinity.  OS name matches the machine affinity.
            if (linuxOSes.contains(os)) {
                Utilities.setMachineAffinity(newJob, os, "outer-latest-or-auto")    
            }
            else {
                Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            }

            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')

            // Unix runs take more than 2 hours to run, so we set the timeout to be longer.
            if (linuxOSes.contains(os) || os == 'OSX') {
                Utilities.setJobTimeout(newJob, 240)
            }

            // Set up appropriate triggers.  PR on demand, otherwise nightly
            if (isPR) {
                // Set PR trigger.
                // TODO: More elaborate regex trigger?
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+outerloop\\W+${os}\\W+${configurationGroup}.*")
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
['Windows 10'].each { os ->
    ['Debug', 'Release'].each { configurationGroup ->

        def newJobName = "perf_${osShortName[os]}_${configurationGroup.toLowerCase()}"

        def newJob = job(Utilities.getFullJobName(project, newJobName, /* isPR */ false)) {
            steps {
                helix("Build.cmd /p:Creator=dotnet-bot /p:ArchiveTests=true /p:ConfigurationGroup=${configurationGroup} /p:Configuration=Windows_${configurationGroup} /p:TestDisabled=true /p:EnableCloudTest=true /p:BuildMoniker={uniqueId} /p:TargetQueue=Windows.10.Amd64 /p:TestProduct=CoreFx /p:Branch=master /p:OSGroup=Windows_NT /p:CloudDropAccountName=dotnetbuilddrops /p:CloudResultsAccountName=dotnetjobresults /p:CloudDropAccessToken={CloudDropAccessToken} /p:CloudResultsAccessToken={CloudResultsAccessToken} /p:BuildCompleteConnection={BuildCompleteConnection} /p:BuildIsOfficialConnection={BuildIsOfficialConnection} /p:DocumentDbKey={DocumentDbKey} /p:DocumentDbUri=https://hms.documents.azure.com:443/ /p:FuncTestsDisabled=true /p:Performance=true")
            }
            // perf tests can be built on any Windows
            label("windows10 || windows7 || windows")
        }

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, /* isPR */ false, "*/${branch}")
        
        // Set a periodic trigger
        Utilities.addPeriodicTrigger(newJob, '@daily')
    }
}

// Here are the OS's that needs separate builds and tests.
// We create a build for the native compilation, a build for the build of corefx itself (on Windows)
// and then a build for the test of corefx on the target platform.  Then we link them with a build
// flow job.

def innerLoopNonWindowsOSs = ['Ubuntu', 'Ubuntu15.10', 'Debian8.2', 'OSX', 'FreeBSD', 'CentOS7.1', 'OpenSUSE13.2', 'RHEL7.2']
[true, false].each { isPR ->
    ['Debug', 'Release'].each { configurationGroup ->
        innerLoopNonWindowsOSs.each { os ->
            def osGroup = osGroupMap[os]
            
            //
            // First define the nativecomp build
            //
            
            def newNativeCompBuildJobName = "nativecomp_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            
            def newNativeCompJob = job(Utilities.getFullJobName(project, newNativeCompBuildJobName, isPR)) {
                steps {
                    shell("./build.sh native x64 ${configurationGroup.toLowerCase()}")
                }
            }
            
            // Set the affinity.  Use the 'latest or auto' version to pick up
            // new auto images.
            Utilities.setMachineAffinity(newNativeCompJob, os, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newNativeCompJob, project, isPR, "*/${branch}")
            // Add archival for the built data.
            Utilities.addArchival(newNativeCompJob, "bin/**")
            
            //
            // First we set up a build job that builds the corefx repo on Windows
            //
            
            def newBuildJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}_bld"

            def newBuildJob = job(Utilities.getFullJobName(project, newBuildJobName, isPR))) {
                steps {
                    batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroup} /p:SkipTests=true /p:TestNugetRuntimeId=${targetNugetRuntimeMap[os]}")
                    // Package up the results.
                    batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                }
            }

            // Set the affinity.  All of these run on Windows currently.
            Utilities.setMachineAffinity(newBuildJob, 'Windows_NT', 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newBuildJob, project, isPR, "*/${branch}")
            // Archive the results
            Utilities.addArchival(newBuildJob, "bin/build.pack,bin/osGroup.AnyCPU.${configurationGroup}/**,bin/ref/**,bin/packages/**,msbuild.log")

            // Use Server GC for Ubuntu/OSX Debug PR build & test
            def serverGCString = ''
                     
            if ((os == 'Ubuntu' || os == 'OSX') && configurationGroup == 'Release' && isPR){
                serverGCString = '--useServerGC'
            }
            
            //
            // Then we set up a job that runs the test on the target OS
            //
            
            def jobFolder = Utilities.getFolderName(project) + '/' + Utilities.getFolderName(branch)
            def fullNativeCompBuildJobName = jobFolder + '/' + newNativeCompJob.name
            def fullCoreFXBuildJobName = jobFolder + '/' + newBuildJob.name
            
            def newTestJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}_tst"
            
            def newTestJob = job(Utilities.getFullJobName(project, newTestJobName, isPR)) {
                steps {
                    // Copy data from other builds.  Copy from correspoding branch of coreclr build
                    // TODO: Add a new job or allow for copying coreclr from debug build
                    
                    def coreClrJobFolder = Utilities.getFolderName("dotnet/coreclr") + '/' + Utilities.getFolderName(branch)
                    // CoreCLR
                    copyArtifacts("${coreClrJobFolder}/release_${os.toLowerCase()}") {
                        excludePatterns('**/testResults.xml', '**/*.ni.dll')
                        buildSelector {
                            latestSuccessful(true)
                        }
                    }
                    
                    // MSCorlib
                    copyArtifacts("${coreClrJobFolder}/release_windows_nt") {
                        includePatterns("bin/Product/${osGroup}*/**")
                        excludePatterns('**/testResults.xml', '**/*.ni.dll')
                        buildSelector {
                            latestSuccessful(true)
                        }
                    }
                    
                    // Native components
                    copyArtifacts(fullNativeCompBuildJobName) {
                        includePatterns("bin/**")
                        buildSelector {
                            buildNumber('\${COREFX_NATIVECOMP_BUILD}')
                        }
                    }
                    
                    // The tests/corefx components
                    copyArtifacts(fullCoreFXBuildJobName) {
                        includePatterns('bin/build.pack')
                        buildSelector {
                            buildNumber('\${COREFX_BUILD}')
                        }
                    }
                    
                    // Unpack the build data
                    shell("unpacker ./bin/build.pack ./bin")
                    // Export the LTTNG environment variable and then run the tests
                    shell("""export LTTNG_HOME=/home/dotnet-bot
                    ./run-test.sh \\
                        --configurationGroup ${configurationGroup} \\
                        --os ${osGroup} \\
                        --corefx-tests \${WORKSPACE}/bin/tests/${osGroup}.AnyCPU.${configurationGroup} \\
                        --coreclr-bins \${WORKSPACE}/bin/Product/${osGroup}.x64.Release/ \\
                        --mscorlib-bins \${WORKSPACE}/bin/Product/${osGroup}.x64.Release/ \\
                        ${serverGCString}
                    """)
                }
                
                // Add parameters for the input jobs
                parameters {
                    stringParam('COREFX_BUILD', '', 'Build number to copy CoreFX test binaries from')
                    stringParam('COREFX_NATIVECOMP_BUILD', '', 'Build number to copy CoreFX native components from')
                }
            }
            
            // Set the affinity.  All of these run on the target
            Utilities.setMachineAffinity(newTestJob, os, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newTestJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newTestJob, '**/testResults.xml')
            
            //
            // Then we set up a flow job that runs the build and the nativecomp build in parallel and then executes.
            // the test job
            //
            
            def fullCoreFXTestJobName = jobFolder + '/' + newTestJob.name
            def flowJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            def newFlowJob = buildFlowJob(Utilities.getFullJobName(project, flowJobName, isPR)) {
                buildFlow("""
                    parallel (
                        { nativeCompBuild = build(params, '${fullNativeCompBuildJobName}') },
                        { coreFXBuild = build(params, '${fullCoreFXBuildJobName}') }
                    )
                    
                    // Then run the test job
                    build(params + 
                        [COREFX_BUILD: coreFXBuild.build.number,
                         COREFX_NATIVECOMP_BUILD : nativeCompBuild.build.number], '${fullCoreFXTestJobName}')
                """)
                
                // Needs a workspace
                configure {
                    def buildNeedsWorkspace = it / 'buildNeedsWorkspace'
                    buildNeedsWorkspace.setValue('true')
                }
            }
            
            // Set the affinity.  All of these run on the target
            Utilities.setMachineAffinity(newFlowJob, os, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newFlowJob, project, isPR, "*/${branch}")
            // Set up triggers
            if (isPR) {
                // Set PR trigger.
                // Set of OS's that work currently. 
                if (os in ['OSX', 'Ubuntu', 'OpenSUSE13.2', 'CentOS7.1']) {
                    // TODO #6070: Temporarily disabled due to failing globalization tests on OpenSUSE.
                    if (os != 'OpenSUSE13.2') {
                        Utilities.addGithubPRTriggerForBranch(newFlowJob, branch, "Innerloop ${os} ${configurationGroup} Build and Test")
                    }
                }
                else {
                    Utilities.addGithubPRTriggerForBranch(newFlowJob, branch, "Innerloop ${os} ${configurationGroup} Build and Test", "(?i).*test\\W+${os}.*")
                }
            }
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newFlowJob)
            }
        }
    }
}

// Generate the build and test versions for Windows_NT.  When full build/run is supported on a platform, those platforms
// could be removed from above and then added in below.
def supportedFullCyclePlatforms = ['Windows_NT']

[true, false].isPR { isPR ->
    ['Debug', 'Release'].each { configurationGroup ->
        supportedFullCyclePlatforms.each { osGroup ->
            def newJobName = "${osGroup.toLowerCase()}_${configurationGroup.toLowerCase()}"

            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
                steps {
                    batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroup}")
                    batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                }
            }

            // Set the affinity.  All of these run on Windows currently.
            Utilities.setMachineAffinity(newJob, osGroup, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
            // Add archival for the built data.
            Utilities.addArchival(newJob, "bin/build.pack,bin/${osGroup}.AnyCPU.Debug/**,bin/ref/**,bin/packages/**,msbuild.log")
            // Set up triggers
            if (isPR) {
                // Set PR trigger.
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop ${osGroup} ${configurationGroup} Build and Test")
            }
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}
