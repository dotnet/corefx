# git commands and workflow for beginners

Goal: Keep it simple, no fancy stuff - just easiest way to get things done (and to understand them)

## Main text

TODO:
* We need reference table and also basic workflow (1 computer, 2 computers - move work, PR updates / rebase).
* The workflow will naturally create branches for each fix (new git concept)
* Picture with remote, origin and current will be worth thousands words ...

## Quick reference
The table below describes the Git flow for creating and updating a pull request.

| Task             | Git command             | Details     |
|------------------|-------------------------|-------------|
| Fork repository  | Fork the repository from the project's github page | See [Checking out the code repository](https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository) |
| Clone repository | `git clone https://github.com/YOUR-USERNAME/corefx.git` | See [Checking out the code repository](https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository) |
| Configure remote repository | `git remote add upstream https://github.com/dotnet/corefx.git` | See [Checking out the code repository](https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository) |
| Sync repository | `git fetch upstream`| Retrieve the latest code from the original fork. |
| Sync repository | `git checkout master` | Checkout your fork's master branch. |
| Sync repository | `git merge upstream/master` | Merge the latest code from the original to your fork's master branch. |
| Create a branch | `git branch < Name of the branch you want to create >` | Creates a new branch on which you will add your code.|
| Checkout a branch | `git checkout < Name of the branch created above >` | Sets your newly created branch as your current working branch | 
| Add changes to the queue | `git add --all` | Stages all the files you have added and edited for commit| 
| Commit changes with message | `git commit -m < message >` | Commits the changes to your local branch and includes the message as a description |
| Push the changes to your remote branch | `git push` | Pushes the changes you committed earlier to your local branch to your remote branch| 
| Raise a pull request| See details | See [Raising a pull request](https://github.com/dotnet/corefx/wiki/Creating-a-Pull-Request) |

## Ideas / Notes

Very useful git blog posts - [Sara Ford's Blog](https://saraford.net/):
* https://saraford.net/2017/05/02/how-to-fetch-down-updates-from-a-git-remote-repository-122/

Useful starter: [Git and GitHub.com](https://guides.github.com/activities/hello-world/)

[MS internal] GitHub usage help: http://aka.ms/GitHelp

### Logical workflow

Idea: Describe this logical workflow and annotate it with git commands (in a table?)

* Enlist / clone = get sources on your computer
* Work on a fix/feature = create branch
    * Tip: What if you forget and realize you have pending files in master, or if you committed to master
* Update (sync) with latest master branch changes
* File operations - edit, delete, rename (how)
    * Note: Rename is auto-detected by file content, no 'rename' records in git
* Publish (commit) changes locally
    * Note: Don't worry about history too much, it can be cleaned up before final checkin
    * ProTip: Don't mix unrelated changes, keep each bug fix / logical work separate - esp. separate larger code cleanup / refactoring / formatting changes (unlikely to introduce regressions) from real functional changes (which may potentially introduce regressions, etc.)
* Backup changes on GitHub
* Share changes between 2 computers (e.g. Laptop & main machine)
* Create PR
    * Cleanup your history prior to submitting PR
    * //Link to Dos & Don't in PRs - TODO: Is this the right place?
* Update PR to incorporate feedback
    * Note: Do not merge master during PR work! Do rebase instead if history is too messy or if there are conflicting changes in master (which is rare!) - if unsure, do it only if you're asked. If you must merge, then do it only after all feedback is incorporated (to avoid restarting the code review from scratch). Note that rebase can be done automatically (by repo maintainers) upon merging your PR -- that should be the 95% case.

### karelz's reference

**DISCLAIMER:** These are my old newbie zero-experience someone-told-me notes, they need to be validated.

| Task             | Git command             | TFS command |
|------------------|-------------------------|-------------|
| **Enlistment**   |                         |             |
| Enlist           |[one-time per repo] Create new repo or your repo fork (GitHub UI)<ul><li>Creates https://github.com/<your_username>/<repo_name> (on the server)</li></ul>**git clone <repo_name>**<ul><li>Just shortcut for "git clone https://github.com/<your_username>/<repo_name>"</li><li>Creates subdir repo_name (incl. 'origin' info - run "git remote -v")</li></ul>Note: The directory is movable between directories, disks, machines | tf workspace /new |
| Enlistment info | **git remote -v** | tf workspace |
| **Modify**   |                         |             |
| Edit file | Just edit the file <ul><li>To make it part of your to-be-committed changes: git add &lt;file&gt;</li></ul> | tf edit &lt;file&gt; |
| Add file | **git add &lt;file&gt;** | tf add &lt;file&gt; |
| Rename file | Just rename file on disk + **git add <new_file>**, git will automatically detect renames | tf rename &lt;file&gt; |
| Delete file | Just delete file on disk + **git add <old_file>**, git will automatically detect deletes | tf delete &lt;file&gt; |
| Undo (local) change | **git checkout -- &lt;file&gt;**<ul><li>Note: "git status" tells you the instructions</li></ul> | tf undo &lt;file&gt; |
| **Sync / checkin / history** | | |
| List pending & local-only changes | **git log**<ul><li>Lists changes which are not committed locally and which are not pushed to server yet</li></ul> | tf status |
| Sync | **git pull -r**<ul><li>fetch + rebase</li></ul> | tf get |
| Resolve conflicts | **tgit resolve** | tf resolve |
| Diff | **tgit diff** | tf diff |
| Checkin | <ul><li>Checkin locally (only into local history all edited files): **git commit -a**</li><li>Adjust local checkins before publishing them to server (e.g. squash=merge them together): **git rebase -i**<ul><li>Note: i=interactive mode (pops txt file editor)</li><li>Read the commands - use p/s/f as needed</li></ul></li><li>Push changes to server:<ul><li>**git push <repo_name> <branch_name>**</li><li>**git push origin my-feature**</li>Note: Never push into upstream</li><li>Review changes: **git compare**</li></ul> | tf checkin<br/>tf submit |
| Revert change | **TODO** | tf revert |
| History view | **git log**<br/>UI: **gitk** | tf history |
| **Branches** | | |
| Create branch / named set of changes ("shelveset") | **git checkout -b <branch_name>**<ul><li>Note: Branches are super-cheap, more like shelvesets</li><li>Beginner hint: Create a new branch before you try anything dangerous with git (pull / squash)</li></ul> | tf shelve |
| Switch between branches ("shelvesets") | **git checkout <branch_name>**<br/>Hint: Use **master** as branch name | tf unshelve |
| List active branch | **git status**<ul><li>Starts with active branch "On branch <branch_name>"</li></ul>Also in: **git branch** | -- |
| List branches | <ol><li>**git branch -a**</li><li>GitHub UI on your repo clone</li></ol> | tf shelvesets |
| Merge branches | **git checkout <target_branch_name>**<br/>**git merge <source_branch_name>** | tf integrate |
| Delete branch | <ol><li>Locally: **git branch -d <branch_name>**</li><li>Remotely: **git push origin --delete <branch_name>**</li></ol> | tf shelveset /delete |
| **Other** | | |
| Create stash / shelveset backup | <ul><li>Create: **git stash**</li><li>List all: **git stash list**</li><li>Restore: **git stash apply <stash_name>**</li><li>Delete: **git stash drop**</li></ul> | -- |
| Rewriting history | http://sethrobertson.github.io/GitFixUm/fixup.html (incl. dangerous rewrite published history) | -- |
