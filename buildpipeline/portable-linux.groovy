@Library('dotnet-ci') _

// Incoming parameters.  Access with "params.<param name>".
// Config - Build configuration. Note that we don't using 'Configuration' since it's used
//          in the build scripts and this can cause problems.
// OuterLoop - If true, runs outerloop, if false runs just innerloop

def submittedHelixJson = null

simpleDockerNode('microsoft/dotnet-buildtools-prereqs:rhel7_prereqs_2') {
    stage ('Checkout source') {
        checkout scm
    }

    def logFolder = getLogFolder()

}

stage ('Generate Link') {
    def SummaryBuilder builder = new SummaryBuilder()
    def linkList = [ "www.bing.com" ]
    builder.addSummaryLinks('Test Execution Summary', linkList)
}