In CoreFx we agreed on the following policies before a Pull Request (PR) can be merged:

1. Every PR needs to be **approved** by a team member with write access to the repository.
2. Unless changes aren't protected by **Continuous Integration** (CI), e.g. Markdown edits, CI must have run recently, within 7 days unless no commits have been merged. In cases where other PRs have been merged into the same library since CI was run, you may wish to rerun in order to get fresh results and avoid build breaks.
3. The **corefx-ci badge** which indicates that all legs run successfully must be **green**, except when [unrelated test failure](#unrelated-test-failure) happened.

These rules apply to both community and team members.

## Unrelated test failure
In case Continuous Integration indicates test failures which are **highly unlikely** to be caused by changes in the Pull Request, the following actions should be taken:
1. An existing issue in the repository should be searched for. Usually the test method's or the test assembly's name (in case of a crash) are good parameters.
2. If there's an existing issue, a comment should be placed that includes a) the link to the build, b) the affected configuration (ie `netcoreapp-Windows_NT-Release-x64-Windows.81.Amd64.Open`) and c) the Error message and Stack trace. This is necessary as retention policies are in place that recycle _old_ builds. In case the issue is already closed, it should be reopened and labels should be updated to reflect the current failure state. 
3. If there's none, an issue should be created with the information mentioned above.
4. In a follow-up Pull Request the failing test should be disabled with the corresponding issue number, e.g. `[ActiveIssue(x)]` and the tracking issue should be labedeled as `disabled-test`.
5. A comment should be placed in the original Pull Request that links to the created or updated issues.

There are plenty of bugs, e.g. race conditions, where a failure might highlight a real problem and it won't manifest again on a retry. Therefore these steps should be followed for every iteration of the PR build, e.g. before retrying/rebuilding.

In the event that an **infrastructure issue** affects the PR build, @dnceng and/or team members should be contacted. Examples of infrastructure issues are **CI leg timeouts**, repository clone errors, permission denied errors, etc. In addition, the leg should be retried, unless retrying multiple times encounters the same infrastructure issue each time.
