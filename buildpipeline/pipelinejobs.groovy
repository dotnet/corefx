// Import the utility functionality.

import jobs.generation.JobReport;
import jobs.generation.Utilities;
import org.dotnet.ci.pipelines.Pipeline

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName

// **************************
// Define innerloop testing.  These jobs run on every merge and a subset of them run on every PR, the ones
// that don't run per PR can be requested via a magic phrase.
// **************************
def linuxPipeline = Pipeline.createPipelineForGithub(this, project, branch, 'buildpipeline/portable-linux.groovy')

['netcoreapp'].each { targetGroup ->
	['Debug', 'Release'].each { configurationGroup ->
		['Linux x64'].each { osName ->
            // One for just innerloop
            linuxPipeline.triggerPipelineOnGithubPRComment("Portable ${osName} ${configurationGroup} Build", "(?i).*test\\W+portable\\W+linux\\W+${configurationGroup}\\W+pipeline.*",
                ['Config':configurationGroup, 'OuterLoop':false])
            // Add one for outerloop
            linuxPipeline.triggerPipelineOnGithubPRComment("Portable ${osName} ${configurationGroup} Build", "(?i).*test\\W+outerloop\\W+portable\\W+linux\\W+${configurationGroup}\\W+pipeline.*",
                ['Config':configurationGroup, 'OuterLoop':true])
		}
	}
}

// Create a pipeline for portable windows
def windowsPipeline = Pipeline.createPipelineForGithub(this, project, branch, 'buildpipeline/portable-windows.groovy')
['netcoreapp'].each { targetGroup ->
	['Debug', 'Release'].each { configurationGroup ->
		['Windows x64'].each { osName ->
            // One for just innerloop
            windowsPipeline.triggerPipelineOnGithubPRComment("Portable ${osName} ${configurationGroup} Build", "(?i).*test\\W+outerloop\\W+portable\\W+windows\\W+${configurationGroup}\\W+pipeline.*",
                ['Config':configurationGroup, 'OuterLoop':false])
            // Add one for outerloop
            windowsPipeline.triggerPipelineOnGithubPRComment("Portable ${osName} ${configurationGroup} Build", "(?i).*test\\W+portable\\W+windows\\W+${configurationGroup}\\W+pipeline.*",
                ['Config':configurationGroup, 'OuterLoop':true])
		}
	}
}

JobReport.Report.generateJobReport(out)

// Make the call to generate the help job
Utilities.createHelperJob(this, project, branch,
    "Welcome to the ${project} Repository",  // This is prepended to the help message
    "Have a nice day!")  // This is appended to the help message.  You might put known issues here.