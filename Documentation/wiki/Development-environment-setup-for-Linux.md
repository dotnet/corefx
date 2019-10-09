This page describes the necessary steps for setting up your development environment on Linux in order to be able to make contributions to the repository.

## Required Software

1. **x86_64** Linux distribution
    * Note: 32bit distros are only community supported. ARM needs to cross-compile.
2. Packages for tools must be installed:
    * **clang/llvm** 3.5+ (3.9+ for ARM)
    * **cmake**
3. Packages for libraries must be installed in -development versions (with appropriate header files):
    * **openssl**
    * **curl**
    * **libunwind**
    * **libicu**

### Distro specific instructions

#### Fedora

* **Fedora 26**: `dnf install git tar libunwind compat-openssl10-devel libicu cmake clang zlib-devel libcurl-devel krb5-devel`
* **Fedora 25**: `sudo dnf install git tar libunwind openssl-devel libicu cmake clang zlib-devel libcurl-devel krb5-devel`

#### Ubuntu

* **Ubuntu 18.04**: `sudo apt install git clang-3.9 cmake make libc6-dev libssl-dev libkrb5-dev libcurl4-openssl-dev zlib1g-dev libicu60 libunwind8 curl`
* **Ubuntu 16.04**: `sudo apt install git clang-3.5 cmake make libc6-dev libssl-dev libkrb5-dev libcurl4-openssl-dev zlib1g-dev libicu55 libunwind8 curl`
* **Ubuntu 14.04**: `sudo apt-get install git clang-3.5 cmake make libc6-dev libssl-dev libkrb5-dev libcurl4-openssl-dev zlib1g-dev libunwind8 libicu52 curl`

## Optional Software

You can use [Visual Studio Code](https://code.visualstudio.com/) as your development IDE.
