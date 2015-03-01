// This is auto generated file. Please don’t modify manually.
// The file is generated as part of the build through the ResourceGenerator tool 
// which takes the project resx resource file and generated this source code file.
// By default the tool will use Resources\Strings.resx but projects can customize
// that by overriding the StringResourcesPath property group.
namespace System
{
    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "System.Diagnostics.Process.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string PriorityClassNotSupported {
              get { return SR.GetResourceString("PriorityClassNotSupported", null); }
        }
        internal static string NoAssociatedProcess {
              get { return SR.GetResourceString("NoAssociatedProcess", null); }
        }
        internal static string ProcessIdRequired {
              get { return SR.GetResourceString("ProcessIdRequired", null); }
        }
        internal static string NotSupportedRemote {
              get { return SR.GetResourceString("NotSupportedRemote", null); }
        }
        internal static string NoProcessInfo {
              get { return SR.GetResourceString("NoProcessInfo", null); }
        }
        internal static string WaitTillExit {
              get { return SR.GetResourceString("WaitTillExit", null); }
        }
        internal static string NoProcessHandle {
              get { return SR.GetResourceString("NoProcessHandle", null); }
        }
        internal static string MissingProccess {
              get { return SR.GetResourceString("MissingProccess", null); }
        }
        internal static string BadMinWorkset {
              get { return SR.GetResourceString("BadMinWorkset", null); }
        }
        internal static string BadMaxWorkset {
              get { return SR.GetResourceString("BadMaxWorkset", null); }
        }
        internal static string ProcessHasExited {
              get { return SR.GetResourceString("ProcessHasExited", null); }
        }
        internal static string ProcessHasExitedNoId {
              get { return SR.GetResourceString("ProcessHasExitedNoId", null); }
        }
        internal static string ThreadExited {
              get { return SR.GetResourceString("ThreadExited", null); }
        }
        internal static string ProcessNotFound {
              get { return SR.GetResourceString("ProcessNotFound", null); }
        }
        internal static string CantGetProcessId {
              get { return SR.GetResourceString("CantGetProcessId", null); }
        }
        internal static string ProcessDisabled {
              get { return SR.GetResourceString("ProcessDisabled", null); }
        }
        internal static string WaitReasonUnavailable {
              get { return SR.GetResourceString("WaitReasonUnavailable", null); }
        }
        internal static string NotSupportedRemoteThread {
              get { return SR.GetResourceString("NotSupportedRemoteThread", null); }
        }
        internal static string CouldntConnectToRemoteMachine {
              get { return SR.GetResourceString("CouldntConnectToRemoteMachine", null); }
        }
        internal static string CouldntGetProcessInfos {
              get { return SR.GetResourceString("CouldntGetProcessInfos", null); }
        }
        internal static string InputIdleUnkownError {
              get { return SR.GetResourceString("InputIdleUnkownError", null); }
        }
        internal static string FileNameMissing {
              get { return SR.GetResourceString("FileNameMissing", null); }
        }
        internal static string EnumProcessModuleFailed {
              get { return SR.GetResourceString("EnumProcessModuleFailed", null); }
        }
        internal static string EnumProcessModuleFailedDueToWow {
              get { return SR.GetResourceString("EnumProcessModuleFailedDueToWow", null); }
        }
        internal static string NoAsyncOperation {
              get { return SR.GetResourceString("NoAsyncOperation", null); }
        }
        internal static string InvalidApplication {
              get { return SR.GetResourceString("InvalidApplication", null); }
        }
        internal static string StandardOutputEncodingNotAllowed {
              get { return SR.GetResourceString("StandardOutputEncodingNotAllowed", null); }
        }
        internal static string StandardErrorEncodingNotAllowed {
              get { return SR.GetResourceString("StandardErrorEncodingNotAllowed", null); }
        }
        internal static string CantGetStandardOut {
              get { return SR.GetResourceString("CantGetStandardOut", null); }
        }
        internal static string CantGetStandardIn {
              get { return SR.GetResourceString("CantGetStandardIn", null); }
        }
        internal static string CantGetStandardError {
              get { return SR.GetResourceString("CantGetStandardError", null); }
        }
        internal static string CantMixSyncAsyncOperation {
              get { return SR.GetResourceString("CantMixSyncAsyncOperation", null); }
        }
        internal static string CantRedirectStreams {
              get { return SR.GetResourceString("CantRedirectStreams", null); }
        }
        internal static string CantUseEnvVars {
              get { return SR.GetResourceString("CantUseEnvVars", null); }
        }
        internal static string EnvironmentBlockTooLong {
              get { return SR.GetResourceString("EnvironmentBlockTooLong", null); }
        }
        internal static string PendingAsyncOperation {
              get { return SR.GetResourceString("PendingAsyncOperation", null); }
        }
        internal static string UseShellExecute {
              get { return SR.GetResourceString("UseShellExecute", null); }
        }
        internal static string InvalidParameter {
              get { return SR.GetResourceString("InvalidParameter", null); }
        }
        internal static string InvalidEnumArgument {
              get { return SR.GetResourceString("InvalidEnumArgument", null); }
        }
        internal static string CategoryHelpCorrupt {
              get { return SR.GetResourceString("CategoryHelpCorrupt", null); }
        }
        internal static string CounterNameCorrupt {
              get { return SR.GetResourceString("CounterNameCorrupt", null); }
        }
        internal static string CounterDataCorrupt {
              get { return SR.GetResourceString("CounterDataCorrupt", null); }
        }
#else
        internal static string PriorityClassNotSupported {
              get { return SR.GetResourceString("PriorityClassNotSupported", @"The AboveNormal and BelowNormal priority classes are not available on this platform."); }
        }
        internal static string NoAssociatedProcess {
              get { return SR.GetResourceString("NoAssociatedProcess", @"No process is associated with this object."); }
        }
        internal static string ProcessIdRequired {
              get { return SR.GetResourceString("ProcessIdRequired", @"Feature requires a process identifier."); }
        }
        internal static string NotSupportedRemote {
              get { return SR.GetResourceString("NotSupportedRemote", @"Feature is not supported for remote machines."); }
        }
        internal static string NoProcessInfo {
              get { return SR.GetResourceString("NoProcessInfo", @"Process has exited, so the requested information is not available."); }
        }
        internal static string WaitTillExit {
              get { return SR.GetResourceString("WaitTillExit", @"Process must exit before requested information can be determined."); }
        }
        internal static string NoProcessHandle {
              get { return SR.GetResourceString("NoProcessHandle", @"Process was not started by this object, so requested information cannot be determined."); }
        }
        internal static string MissingProccess {
              get { return SR.GetResourceString("MissingProccess", @"Process with an Id of {0} is not running."); }
        }
        internal static string BadMinWorkset {
              get { return SR.GetResourceString("BadMinWorkset", @"Minimum working set size is invalid. It must be less than or equal to the maximum working set size."); }
        }
        internal static string BadMaxWorkset {
              get { return SR.GetResourceString("BadMaxWorkset", @"Maximum working set size is invalid. It must be greater than or equal to the minimum working set size."); }
        }
        internal static string ProcessHasExited {
              get { return SR.GetResourceString("ProcessHasExited", @"Cannot process request because the process ({0}) has exited."); }
        }
        internal static string ProcessHasExitedNoId {
              get { return SR.GetResourceString("ProcessHasExitedNoId", @"Cannot process request because the process has exited."); }
        }
        internal static string ThreadExited {
              get { return SR.GetResourceString("ThreadExited", @"The request cannot be processed because the thread ({0}) has exited."); }
        }
        internal static string ProcessNotFound {
              get { return SR.GetResourceString("ProcessNotFound", @"Thread {0} found, but no process {1} found."); }
        }
        internal static string CantGetProcessId {
              get { return SR.GetResourceString("CantGetProcessId", @"Cannot retrieve process identifier from the process handle."); }
        }
        internal static string ProcessDisabled {
              get { return SR.GetResourceString("ProcessDisabled", @"Process performance counter is disabled, so the requested operation cannot be performed."); }
        }
        internal static string WaitReasonUnavailable {
              get { return SR.GetResourceString("WaitReasonUnavailable", @"WaitReason is only available if the ThreadState is Wait."); }
        }
        internal static string NotSupportedRemoteThread {
              get { return SR.GetResourceString("NotSupportedRemoteThread", @"Feature is not supported for threads on remote computers."); }
        }
        internal static string CouldntConnectToRemoteMachine {
              get { return SR.GetResourceString("CouldntConnectToRemoteMachine", @"Couldn't connect to remote machine."); }
        }
        internal static string CouldntGetProcessInfos {
              get { return SR.GetResourceString("CouldntGetProcessInfos", @"Couldn't get process information from performance counter."); }
        }
        internal static string InputIdleUnkownError {
              get { return SR.GetResourceString("InputIdleUnkownError", @"WaitForInputIdle failed.  This could be because the process does not have a graphical interface."); }
        }
        internal static string FileNameMissing {
              get { return SR.GetResourceString("FileNameMissing", @"Cannot start process because a file name has not been provided."); }
        }
        internal static string EnumProcessModuleFailed {
              get { return SR.GetResourceString("EnumProcessModuleFailed", @"Unable to enumerate the process modules."); }
        }
        internal static string EnumProcessModuleFailedDueToWow {
              get { return SR.GetResourceString("EnumProcessModuleFailedDueToWow", @"A 32 bit processes cannot access modules of a 64 bit process."); }
        }
        internal static string NoAsyncOperation {
              get { return SR.GetResourceString("NoAsyncOperation", @"No async read operation is in progress on the stream."); }
        }
        internal static string InvalidApplication {
              get { return SR.GetResourceString("InvalidApplication", @"The specified executable is not a valid application for this OS platform."); }
        }
        internal static string StandardOutputEncodingNotAllowed {
              get { return SR.GetResourceString("StandardOutputEncodingNotAllowed", @"StandardOutputEncoding is only supported when standard output is redirected."); }
        }
        internal static string StandardErrorEncodingNotAllowed {
              get { return SR.GetResourceString("StandardErrorEncodingNotAllowed", @"StandardErrorEncoding is only supported when standard error is redirected."); }
        }
        internal static string CantGetStandardOut {
              get { return SR.GetResourceString("CantGetStandardOut", @"StandardOut has not been redirected or the process hasn't started yet."); }
        }
        internal static string CantGetStandardIn {
              get { return SR.GetResourceString("CantGetStandardIn", @"StandardIn has not been redirected."); }
        }
        internal static string CantGetStandardError {
              get { return SR.GetResourceString("CantGetStandardError", @"StandardError has not been redirected."); }
        }
        internal static string CantMixSyncAsyncOperation {
              get { return SR.GetResourceString("CantMixSyncAsyncOperation", @" Cannot mix synchronous and asynchronous operation on process stream."); }
        }
        internal static string CantRedirectStreams {
              get { return SR.GetResourceString("CantRedirectStreams", @"The Process object must have the UseShellExecute property set to false in order to redirect IO streams."); }
        }
        internal static string CantUseEnvVars {
              get { return SR.GetResourceString("CantUseEnvVars", @"The Process object must have the UseShellExecute property set to false in order to use environment variables."); }
        }
        internal static string EnvironmentBlockTooLong {
              get { return SR.GetResourceString("EnvironmentBlockTooLong", @"The environment block used to start a process cannot be longer than 65535 bytes.  Your environment block is {0} bytes long.  Remove some environment variables and try again."); }
        }
        internal static string PendingAsyncOperation {
              get { return SR.GetResourceString("PendingAsyncOperation", @"An async read operation has already been started on the stream."); }
        }
        internal static string UseShellExecute {
              get { return SR.GetResourceString("UseShellExecute", @" UseShellExecute must always be set to false."); }
        }
        internal static string InvalidParameter {
              get { return SR.GetResourceString("InvalidParameter", @"Invalid value '{1}' for parameter '{0}'."); }
        }
        internal static string InvalidEnumArgument {
              get { return SR.GetResourceString("InvalidEnumArgument", @"The value of argument '{0}' ({1}) is invalid for Enum type '{2}'."); }
        }
        internal static string CategoryHelpCorrupt {
              get { return SR.GetResourceString("CategoryHelpCorrupt", @"Cannot load Category Help data because an invalid index '{0}' was read from the registry."); }
        }
        internal static string CounterNameCorrupt {
              get { return SR.GetResourceString("CounterNameCorrupt", @"Cannot load Counter Name data because an invalid index '{0}' was read from the registry."); }
        }
        internal static string CounterDataCorrupt {
              get { return SR.GetResourceString("CounterDataCorrupt", @"Cannot load Performance Counter data because an unexpected registry key value type was read from '{0}'."); }
        }

#endif
    }
}
