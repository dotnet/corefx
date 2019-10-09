****WORK IN PROGRESS****

# Creating a Pull Request

This page describes how to create a pull request to the [corefx](https://github.com/dotnet/corefx) project. The prerequisites for doing this 
are:

1. [Machine Setup](https://github.com/dotnet/corefx/wiki/Setting-up-the-development-environment)
2. [Fork and Clone Repository](https://github.com/dotnet/corefx/wiki/Checking-out-the-code-repository)
3. [Build and Run Tests](https://github.com/dotnet/corefx/wiki/Build-and-run-tests)

Once you have successfully done those things feel free to proceed here!

## Getting Started

1. Make sure you have the latest code
   ```
   git fetch upstream
   git checkout master
   git rebase upstream/master
   ```
2. [Pick your issue](https://github.com/dotnet/corefx/wiki/Pick-issue)
3. Create a branch
   ```
   git checkout -b MyNewFeature
   ```
4. Make your changes
5. Add the files
   ```
   git add --all .
   ```
6. Commit to your local repository
7. Push the changes remotely

   This is a little different than other development you may have done. We are going to do a pull request across repositories. From the branch on my fork, to master on CoreFX.
8. Open GitHub
9. See button to PR from your branch on your remote fork to master branch on CoreFX repository
10. Click button to create Pull Request
11. Write up the details of your pull request and submit!

## Pull Request Writing Guidelines
TODO - There was a link floating around to a great blog post on writing PRs. Insert that here.

## Questions and Answers

**Q: How do I make additional changes to my branch?**

A: Just like you normally would! If you commit to your local branch then push it will show up 
on the remote and run the tests again.

**Q: I am getting a lot of feedback, is this bad?**

A: Nope! Almost all of the tickets going into CoreFX have feedback for how they can be improved. The team appreciates your contributions and expects that people will need feedback. It's not a 
negative reflection on you or your skills. It takes everyone time to learn new coding standards and how everything works, especially when also contributing to a repository for the first time.

**Q: The steps in this document aren't working, how can I update them?**

A: All of these documents are in GitHub. Simply follow the steps above and do a Pull Request to make changes to this document.

**Q: How can I let a check run, again!?**

A: To be able to run a check again without having to do coding changes, the simplest and easiest way is to add a comment to your pull request.

The boilerplate for such a message is: `/azp run CHECK-PIPELINE`
So, for example, if you want to run the corefx-ci pipeline again, simply comment: `/azp run corefx-ci`.

The entire pipeline will now run again.