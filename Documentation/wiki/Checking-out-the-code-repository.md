This page describes the required steps for checking out the code repository.

### Quick Reference

#### Typical workflow

1. [MS Internal] `\\fxcore\fxkit\install.cmd [<install_dir>]` - Downloads git.exe and friends.
    * TODO: Figure out public alternative, or publish the tooling repo.
2. `git clone corefx` - Creates CoreFX repo copy in local dir 'corefx' (350 MB).
    * WARNING: Do not use directory with space nor '#' (until [#19883](https://github.com/dotnet/corefx/issues/19883) and [#22775](https://github.com/dotnet/corefx/issues/22775) are fixed).
3. `git remote add upstream dotnet` - Creates connection with https://github.com/dotnet/corefx. Powers `git get` custom command in FxKit.

#### Tips & tricks

* Git cloned directory can be moved around (between disks/machines) without any harm.
* [MS Internal] FxKit is a git repo (it can be moved around).



## Fork the repository

Once you have setup your environment it is time to check out the code.
From the project [home page](https://github.com/dotnet/corefx) create a fork.

Note: It is one-time operation **per GitHub user**, no need to repeat it per computer.
 
![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/1-Fork.png)

This will create a fork of the repository in your personal Github account.



## Clone the repository

Navigate to the repository under your GitHub account, select Clone or download and copy the repository location.

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/2-Clone.png)

Open the command prompt, navigate to the location that you wish to clone the directory to and type the following command:
```
git clone <repository_url_copied_from_above>
```

WARNING: Do not use directory with space nor `#` until [#19883](https://github.com/dotnet/corefx/issues/19883) and [#22775](https://github.com/dotnet/corefx/issues/22775) are fixed.

It will look something like this:

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/3-Clone.png)

It will download ~150MB (10s-3min). In the end you should see a success message like this:

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/4-Clone.png)



## Configure remote repository for syncing

In order to keep your fork up to date with the original repository you have to configure the remote repository.
To list your current remote configured remote type the following command:
```
git remote -v
```

You should see your personal repository configured for fetch and push, it will look something like this:
  * origin https://github.com/YourUsername/corefx.git (fetch) 
  * origin https://github.com/YourUsername/corefx.git (push)

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/7-RemoteList.png)

Now configure the remote by typing the following command: 
```
git remote add upstream https://github.com/dotnet/corefx.git
```

To verify that the remote has been setup correctly type again: 
```
git remote -v
```

You should now see your personal repository and the remote repository configured for fetch and push, it will look something like this:
  * origin https://github.com/YourUsername/corefx.git (fetch)
  * origin https://github.com/YourUsername/corefx.git (push)
  * upstream https://github.com/dotnet/corefx.git (fetch)
  * upstream https://github.com/dotnet/corefx.git (push)

![](https://raw.githubusercontent.com/haralabidis/FirstTimeContributors/master/Assets/9-RemoteList2.png)

#### Syncing with the remote repository
To sync your local master branch with the remote upstream repository simply run:
```
git checkout master
git pull upstream master
```

Once you have successfully cloned the repository, verify that you can [build and run tests](https://github.com/dotnet/corefx/wiki/Build-and-run-tests).