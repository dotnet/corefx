@Library('dotnet-ci') _
import jobs.generation.SummaryBuilder;
// Incoming parameters.  Access with "params.<param name>".
// Config - Build configuration. Note that we don't using 'Configuration' since it's used
//          in the build scripts and this can cause problems.
// OuterLoop - If true, runs outerloop, if false runs just innerloop


stage ('Generate Link') {
    def newSummary = manager.createSummary("terminal.gif")
    
    // Append the header
    newSummary.appendText("<b>Links to Read:</b><ul>", false)
    newSummary.appendText("<li><a href=\\\"www.bing.com\\\">www.bing.com</a></li>", false)
    newSummary.appendText("<li>None</li>", false)
}