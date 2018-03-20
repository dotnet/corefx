@Library('dotnet-ci') _

// Incoming parameters.  Access with "params.<param name>".
// Note that the parameters will be set as env variables so we cannot use names that conflict
// with the engineering system parameter names.
// CGroup - Build configuration.
// TestOuter - If true, runs outerloop, if false runs just innerloop

def submittedHelixJson = null

simpleDockerNode('microsoft/dotnet-buildtools-prereqs:ubuntu-16.04-cross-arm64-a3ae44b-20180315221921') {
    stage ('Checkout source') {
        checkoutRepo()
    }

    def logFolder = getLogFolder()

    stage ('Initialize tools') {
        // Init tools
        sh './init-tools.sh'
    }
    stage ('Generate version assets') {
        // Generate the version assets.  Do we need to even do this for non-official builds?
        sh "./build-managed.sh -- /t:GenerateVersionSourceFile /p:GenerateVersionSourceFile=true"
    }

    // TODO: Sync when runtime is available

    // TODO: Build product instead of just native once dependencies are avaliable

    stage ('Build Native') {
        sh """
            export ROOTFS_DIR=/crossrootfs/arm64
            ./build-native.sh -buildArch=arm64 -${params.CGroup} -- cross
        """
    }

    // TODO: Build Tests for arm64 when possible

    // TODO: Add submission for Helix testing once we have queue for arm64 Linux working
}

// TODO: Add "Execute tests" stage once we have queue for arm64 Linux working
