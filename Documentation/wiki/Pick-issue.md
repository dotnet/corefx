## How to pick an issue to work on

### Quick Reference

* [first-timers-only issues] - Query TODO - issues designed for first-time contributors, anyone else should stay away (motivation: http://www.firsttimersonly.com/)
* [Easy issues](https://github.com/dotnet/corefx/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aopen%20label%3Aeasy%20no%3Aassignee) - Good for starting contributors.
* [up-for-grabs issues](https://github.com/dotnet/corefx/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aopen%20label%3Aup-for-grabs%20no%3Aassignee%20-label%3Aapi-needs-work) - Issues where we welcome help. Typically there is comment with hint where to start and how complex the issue likely is. Feel free to ping [area owners](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/issue-guide.md#areas) if it is not clear.
* Most impactful issues: (for seasoned contributors)
  * [wishlist](https://github.com/dotnet/corefx/issues?q=is%3Aissue+is%3Aopen+label%3Awishlist) - Issues on top of our backlog we won't likely get to. Warning: Might not be easy.
  * [3.0](https://github.com/dotnet/corefx/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aopen%20milestone%3A3.0.0) - Issues in our 3.0 (next release) backlog. We think we will get to them. Help is welcome, but might be non-trivial.
* **Get help** if you're stuck: Ping [area owners](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/issue-guide.md#areas) or [@karelz](https://github.com/karelz) or [@danmosemsft](https://github.com/danmosemsft)

## Details

You can discover available issues by executing a query on the [issues section](https://github.com/dotnet/corefx/issues) of the project.

Below is a list of the labels that you can use in the query:

* easy - This should be an easy fix and is probable the best starting point for new contributors.
* up-for-grabs - The item is available and can be selected.
  * Recommended:
    * test-enhancement - Tests are missing or need to be updated - typically great entry into new code base.
  * NOT Recommended:
    * api-approved - Adding APIs is a bit more involved (TODO link). We recommend new contributors to start with easier ones.
    * api-needs-work - APIs which need to be designed first. Some won't be accepted even if designed. Stay away from them, unless you are seasoned API designer, and are willing to accept long [API review process](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/api-review-process.md) (up to months), with the possibility that the API will be rejected as not best fit for CoreFX repo.

See below an example query for finding issues that are open, up for grabs, api needs work with no assignee. 

QUERY [is:issue is:open label:up-for-grabs no:assignee -label:api-needs-work](https://github.com/dotnet/corefx/issues?utf8=%E2%9C%93&q=is%3Aopen%20label%3Aup-for-grabs%20-label%3Aapi-needs-work%20no%3Aassignee)

If you don't feel comfortable with constructing and typing the query in the issues search box you can use GitHub's GUI.
Click on the labels dropdown in the issues page and select from the available labels.

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/7-IssueLabels.png) 