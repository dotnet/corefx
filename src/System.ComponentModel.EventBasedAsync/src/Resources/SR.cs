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
        private const string s_resourcesName = "System.ComponentModel.EventBasedAsync.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string Async_NullDelegate {
              get { return SR.GetResourceString("Async_NullDelegate", null); }
        }
        internal static string Async_OperationAlreadyCompleted {
              get { return SR.GetResourceString("Async_OperationAlreadyCompleted", null); }
        }
        internal static string Async_OperationCancelled {
              get { return SR.GetResourceString("Async_OperationCancelled", null); }
        }
        internal static string BackgroundWorker_WorkerAlreadyRunning {
              get { return SR.GetResourceString("BackgroundWorker_WorkerAlreadyRunning", null); }
        }
        internal static string BackgroundWorker_WorkerDoesntReportProgress {
              get { return SR.GetResourceString("BackgroundWorker_WorkerDoesntReportProgress", null); }
        }
        internal static string BackgroundWorker_WorkerDoesntSupportCancellation {
              get { return SR.GetResourceString("BackgroundWorker_WorkerDoesntSupportCancellation", null); }
        }
#else
        internal static string Async_NullDelegate {
              get { return SR.GetResourceString("Async_NullDelegate", @"A non-null SendOrPostCallback must be supplied."); }
        }
        internal static string Async_OperationAlreadyCompleted {
              get { return SR.GetResourceString("Async_OperationAlreadyCompleted", @"This operation has already had OperationCompleted called on it and further calls are illegal."); }
        }
        internal static string Async_OperationCancelled {
              get { return SR.GetResourceString("Async_OperationCancelled", @"Operation has been cancelled."); }
        }
        internal static string BackgroundWorker_WorkerAlreadyRunning {
              get { return SR.GetResourceString("BackgroundWorker_WorkerAlreadyRunning", @"This BackgroundWorker is currently busy and cannot run multiple tasks concurrently."); }
        }
        internal static string BackgroundWorker_WorkerDoesntReportProgress {
              get { return SR.GetResourceString("BackgroundWorker_WorkerDoesntReportProgress", @"This BackgroundWorker states that it doesn't report progress. Modify WorkerReportsProgress to state that it does report progress."); }
        }
        internal static string BackgroundWorker_WorkerDoesntSupportCancellation {
              get { return SR.GetResourceString("BackgroundWorker_WorkerDoesntSupportCancellation", @"This BackgroundWorker states that it doesn't support cancellation. Modify WorkerSupportsCancellation to state that it does support cancellation."); }
        }

#endif
    }
}
