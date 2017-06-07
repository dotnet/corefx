@Library('dotnet-ci') _
import jobs.generation.SummaryBuilder;
// Incoming parameters.  Access with "params.<param name>".
// Config - Build configuration. Note that we don't using 'Configuration' since it's used
//          in the build scripts and this can cause problems.
// OuterLoop - If true, runs outerloop, if false runs just innerloop


stage ('Generate Link') {
    def SummaryBuilder builder = new SummaryBuilder()
    def linkList = [ "www.bing.com" ]
    builder.addSummaryLinks('Test Execution Summary', linkList)
}