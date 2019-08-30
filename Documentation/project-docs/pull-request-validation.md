In the .NET Platform repository we agreed on the following policies before a Pull Request can be merged:

1. Every PR needs to be **approved** by a team member with write access to the repository.
2. Unless changes aren't protected by **Continuous Integration** (CI), i.e. Markdown edits, CI must have run recently.
3. The **corefx-ci badge** which indicates that all legs run successfully must be **green**, except an [unrelated test failure](#unrelated-test-failure) happened.

These rules apply to both community and team members.

## Unrelated test failure
In case Continuous Integration indicates test failures which are **highly unlikely** (in doubt, hit retry) to be caused by changes in the Pull Request, the following actions should be taken:
1. Search for an existing issue in the repository. Usually the test method's name is good search parameter.
2. If there's an existing issue, a comment should be placed that includes a) the link to the build, b) the affected configuration (ie `netcoreapp-Windows_NT-Release-x64-Windows.81.Amd64.Open`) and c) the Error message and Stack trace. This is necessary as retention policies are in place that recycle _old_ builds. 
3. If there's none, an issue should be created with the information mentioned above.
4. In a follow-up Pull Request the failing test should be disabled with the corresponding issue number, i.e. `[ActiveIssue(x)]` and the tracking issue should be labedeled as `disabled-test`.

In the event that an infrastructure issue affects the PR build, i.e. a CI leg times out, @dnceng and/or team members should be contacted.
