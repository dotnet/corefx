This page describes the necessary steps for setting up your development environment on MacOS in order to be able to make contributions to the repository.

## Required Software

1. **macOS 10.12** "Sierra", or higher
    * Note: macOS 10.11 is not sufficient, because it lacks crypto support which we depend on.
2. **XCode** must be installed (needed for llvm compiler, headers and dependencies).
    * Go to "App Store" and install XCode from there.
3. **OpenSSL** must be installed: see detailed steps via [Homebrew ("brew")](https://brew.sh/) package manager below.
4. **CMake** must be installed: `brew install cmake`.

### OpenSSL

First install [Homebrew ("brew")](https://brew.sh/) package manager.
After installing *brew*, install *OpenSSL* and *pkg-config* by executing the following commands at a Terminal (command) prompt:

```Terminal
brew install pkg-config
brew install openssl

ln -s /usr/local/opt/openssl/lib/libcrypto.1.0.0.dylib /usr/local/lib/
ln -s /usr/local/opt/openssl/lib/libssl.1.0.0.dylib /usr/local/lib/

ln -s /usr/local/opt/openssl/lib/pkgconfig/libcrypto.pc /usr/local/lib/pkgconfig/
ln -s /usr/local/opt/openssl/lib/pkgconfig/libssl.pc /usr/local/lib/pkgconfig/
ln -s /usr/local/opt/openssl/lib/pkgconfig/openssl.pc /usr/local/lib/pkgconfig/
```

### CMake

Easiest way to get cmake is also via [Homebrew ("brew")](https://brew.sh/) package manager: `brew install cmake`

Alternatively you can install it directly from the [CMake download page](https://cmake.org/download/#latest), in which case you have to add *CMake* manually to your path by executing the following on a terminal:

```Terminal
sudo mkdir -p /usr/local/bin
sudo /Applications/CMake.app/Contents/bin/cmake-gui --install=/usr/local/bin
```

## Optional Software

You can use [Visual Studio for Mac](https://www.visualstudio.com/vs/visual-studio-mac/) or [Visual Studio Code](https://code.visualstudio.com/) as your development IDE.
