Build guidelines
================
###Goal

It is difficult for devs, who are new to the corefx repo, to reason about the build and understand how to implement new features, or otherwise update the repo's build.  The primary goal is to provide a repo which is easier to reason about.  A secondary goal, would be to help the dev implement their changes to the build (without getting in their way) such that they are confident they are following our guidelines.

	- Keep it simple
	- Keep it clean
	- Make it maintainable (secondary)

### Keep it simple

We don't want multiple ways to do the same thing.  There is a support cost and mind-share cost associated with having multiple ways to achieve something.  It's ok to have insider tricks, things for power-users who understand the default experience but want to optimize it for their development patterns.
	* Support cost
		* Discovery becomes difficult
		* Maintenance is difficult
		* Devs will grow accustomed to whatever pattern they first learn and not utilize the other, so why have more than one?
		* Multiple ways to accomplish the same goal make it difficult to ensure that what a dev is doing is the same thing that an automated system is doing (or close to it)
	* If the second way to do something is literally an alias to the first way, then that's probably ok, but we should have those things in a discoverable place, not cluttering our root.
		* The run tool can highlight an alternative command, ie. Build-managed.cmd, build-tests.cmd, etc…  The alternative commands (or wrapper scripts) would be moved out of the root and into a subdirectory of the builds folder 

### Keep it clean

It is difficult to understand our build because our build files are interspersed throughout our repo, or simply dropped at the root where they are lost amongst the myriad of other files in the root of the repo.  It is preferable if there is a single place to go, to reason about our build system.  
	* The root of the repo should only have 
		* Folders
		* Config files
		* The minimum set of entry points to discover how to interact with the repo (a Windows and an Unix entry point)
			* Additionally an sln permitted if the repo is loadable / buildable from VS
		* One MD documentation file, that documentation file can, and should, point to additional documentation files. To be consistent, the documentation file should probably be named README.md
		* Patent / license / notice files / etc, I'm assuming these are a legal requirement, but if not, let's move them!
	* Separate build specific files into their own folder to ease determining what is a build file, ie what do you need to look at to figure out how the build works. 
	* Dir.props/targets files should act like a ToC to other props/targets files respectively
		* Those additional props files and targets files should be in a separate directory so that it is clear where the ToC for the folder is
		* Should dir.props be responsible for all imports, ie, refactor so sub property files don't import anything, anything that's imported occurs in dir.props?
			* Should we add a depends on section to comments so that we know what other props files this props files depends on (may make navigating simpler).
				* Repo rooted paths
		* If dir.targets has an initial target, leave it in dir.targets, all other targets in separate targets files
		* Avoid global properties / items in targets, move them to a props file?
	* Builds files and proj files are intrinsic parts of a project and remain where they are

### Make it maintainable

Let's codify the "MSBuild coding style" Jeremy Kuhne has outlined - https://github.com/dotnet/buildtools/pull/946
Creating a tool that will analyze our build project system and enforce the coding style rules we have will go a long ways towards ensuring our code is readable and consistent without requiring a huge investment from dev mind space during code reviews.  All of the rules listed in "coding style" are easily codified, though "best practices" would be more difficult to implement. 
