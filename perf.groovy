// Import the utility functionality.

import jobs.generation.*;

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName
// Folder that the project jobs reside in (project/branch)
def projectFolder = Utilities.getFolderName(project) + '/' + Utilities.getFolderName(branch)

// Globals

// Map of os -> osGroup.
def osGroupMap = ['Ubuntu14.04':'Linux',
                  'Ubuntu16.04':'Linux',
                  'Debian8.4':'Linux',
                  'Fedora23':'Linux',
                  'OSX':'OSX',
                  'Windows_NT':'Windows_NT',
                  'CentOS7.1': 'Linux',
                  'OpenSUSE13.2': 'Linux',
                  'RHEL7.2': 'Linux',
                  'LinuxARMEmulator': 'Linux']

// Map of os -> nuget runtime
def targetNugetRuntimeMap = ['OSX' : 'osx.10.10-x64',
                             'Ubuntu14.04' : 'ubuntu.14.04-x64',
                             'Ubuntu16.04' : 'ubuntu.16.04-x64',
                             'Fedora23' : 'fedora.23-x64',
                             'Debian8.4' : 'debian.8-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'OpenSUSE13.2' : 'opensuse.13.2-x64',
                             'RHEL7.2': 'rhel.7-x64']

def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Windows Nano 2016' : 'winnano16',
                   'Ubuntu16.04' : 'ubuntu16.04',
                   'CentOS7.1' : 'centos7.1',
                   'Debian8.4' : 'debian8.4',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'Fedora23' : 'fedora23',
                   'RHEL7.2' : 'rhel7.2']


// **************************
// Define perf testing.  These tests should be run daily, and will run on Jenkins not in Helix
// **************************
[true, false].each { isPR ->
	['Release'].each { configurationGroup ->
		['Windows_NT'].each { os ->
			def osGroup = osGroupMap[os]
			def newJobName = "perf_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
			//Pulled from list here https://wiki.jenkins-ci.org/display/JENKINS/GitHub+pull+request+builder+plugin
			def runType = isPR ? "private" : "rolling"
			//def commitName = "${ghprbPullTitle}"
			//def user = "${ghprbActualCommitAuthor}"

			def newJob = job(Utilities.getFullJobName(project, newJobName, isPR)) {
				// On Windows we use the packer to put together everything. On *nix we use tar
				steps {
					batchFile("SET")
					// Grab the latest version of the Benchview tools for uploading results
					batchFile("C:\\Tools\\nuget.exe install Microsoft.BenchView.JSONFormat -Source http://benchviewtestfeed.azurewebsites.net/nuget -OutputDirectory C:\\tools -Prerelease -version 0.1.0-pre015")
					batchFile("python C:\\tools\\Microsoft.BenchView.JSONFormat.0.1.0-pre015\\tools\\submission-metadata.py --name " + "\"Test commit name\"" + " --user " + "\"anscoggi@microsoft.com\"" + " -o C:\\submission-metadata.json")
					batchFile("python C:\\tools\\Microsoft.BenchView.JSONFormat.0.1.0-pre015\\tools\\build.py git --type " + runType + " -o C:\\build.json")
					batchFile("python C:\\tools\\Microsoft.BenchView.JSONFormat.0.1.0-pre015\\tools\\machinedata.py -o C:\\machinedata.json")
					//We need to specify the max cpu count to be one as we do not want to be executing performance tests in parallel
					batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroup} /p:FuncTestsDisabled=true /p:Performance=true /p:TargetOS=${osGroup} /p:LogToBenchview=true /p:BenchviewRuntype=" + runType + " /maxcpucount:1")
					batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
				}
			}

			// Set the label.
			newJob.with {
				label('windows_clr_perf')
			}
			// Set up standard options.
			Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
			// Add the unit test results
			Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
			def archiveContents = "msbuild.log"
			// Packer.exe is a .NET Framework application. When we can use it from the tool-runtime, we can archive the ".pack" file here.
			archiveContents += ",bin/build.pack"
			
			// Add archival for the built data.
			Utilities.addArchival(newJob, archiveContents)
			// Set up triggers
			if (isPR) {
				TriggerBuilder builder = TriggerBuilder.triggerOnPullRequest()
				builder.setGithubContext("${os} Perf Tests")
				builder.triggerOnlyOnComment()
				builder.setCustomTriggerPhrase("(?i).*test\\W+${os}\\W+perf.*")
				builder.triggerForBranch(branch)
				builder.emitTrigger(newJob)
			}
			else {
				// Set a push trigger
				TriggerBuilder builder = TriggerBuilder.triggerOnCommit()
				builder.emitTrigger(newJob)
			}
		}
	}
}