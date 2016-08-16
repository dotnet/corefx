// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the MethodNameCompleted event.
    /// </summary>
    public partial class AsyncCompletedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCompletedEventArgs" />
        /// class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled">A value indicating whether the asynchronous operation was canceled.</param>
        /// <param name="userState">
        /// The optional user-supplied state object passed to the
        /// <see cref="BackgroundWorker.RunWorkerAsync(Object)" /> method.
        /// </param>
        public AsyncCompletedEventArgs(System.Exception error, bool cancelled, object userState) { }
        /// <summary>
        /// Gets a value indicating whether an asynchronous operation has been canceled.
        /// </summary>
        /// <returns>
        /// true if the background operation has been canceled; otherwise false. The default is false.
        /// </returns>
        public bool Cancelled { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating which error occurred during an asynchronous operation.
        /// </summary>
        /// <returns>
        /// An <see cref="Exception" /> instance, if an error occurred during an asynchronous
        /// operation; otherwise null.
        /// </returns>
        public System.Exception Error { get { return default(System.Exception); } }
        /// <summary>
        /// Gets the unique identifier for the asynchronous task.
        /// </summary>
        /// <returns>
        /// An object reference that uniquely identifies the asynchronous task; otherwise, null if no value
        /// has been set.
        /// </returns>
        public object UserState { get { return default(object); } }
        /// <summary>
        /// Raises a user-supplied exception if an asynchronous operation failed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Cancelled" /> property is true.
        /// </exception>
        /// <exception cref="Reflection.TargetInvocationException">
        /// The <see cref="Error" /> property has been
        /// set by the asynchronous operation. The <see cref="Exception.InnerException" /> property
        /// holds a reference to <see cref="Error" />.
        /// </exception>
        protected void RaiseExceptionIfNecessary() { }
    }
    /// <summary>
    /// Represents the method that will handle the MethodNameCompleted event of an asynchronous operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    /// An <see cref="AsyncCompletedEventArgs" /> that contains the event
    /// data.
    /// </param>
    public delegate void AsyncCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    /// <summary>
    /// Tracks the lifetime of an asynchronous operation.
    /// </summary>
    public sealed partial class AsyncOperation
    {
        internal AsyncOperation() { }
        /// <summary>
        /// Gets the <see cref="Threading.SynchronizationContext" /> object that was passed to
        /// the constructor.
        /// </summary>
        /// <returns>
        /// The <see cref="Threading.SynchronizationContext" /> object that was passed to the
        /// constructor.
        /// </returns>
        public System.Threading.SynchronizationContext SynchronizationContext { get { return default(System.Threading.SynchronizationContext); } }
        /// <summary>
        /// Gets or sets an object used to uniquely identify an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The state object passed to the asynchronous method invocation.
        /// </returns>
        public object UserSuppliedState { get { return default(object); } }
        /// <summary>
        /// Finalizes the asynchronous operation.
        /// </summary>
        ~AsyncOperation() { }
        /// <summary>
        /// Ends the lifetime of an asynchronous operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="OperationCompleted" /> has been called previously
        /// for this task.
        /// </exception>
        public void OperationCompleted() { }
        /// <summary>
        /// Invokes a delegate on the thread or context appropriate for the application model.
        /// </summary>
        /// <param name="d">
        /// A <see cref="Threading.SendOrPostCallback" /> object that wraps the delegate to be
        /// called when the operation ends.
        /// </param>
        /// <param name="arg">An argument for the delegate contained in the <paramref name="d" /> parameter.</param>
        /// <exception cref="InvalidOperationException">
        /// The
        /// <see cref="PostOperationCompleted(Threading.SendOrPostCallback,Object)" /> method has been called previously for this task.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        public void Post(System.Threading.SendOrPostCallback d, object arg) { }
        /// <summary>
        /// Ends the lifetime of an asynchronous operation.
        /// </summary>
        /// <param name="d">
        /// A <see cref="Threading.SendOrPostCallback" /> object that wraps the delegate to be
        /// called when the operation ends.
        /// </param>
        /// <param name="arg">An argument for the delegate contained in the <paramref name="d" /> parameter.</param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="OperationCompleted" /> has been called previously
        /// for this task.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        public void PostOperationCompleted(System.Threading.SendOrPostCallback d, object arg) { }
    }
    /// <summary>
    /// Provides concurrency management for classes that support asynchronous method calls. This class
    /// cannot be inherited.
    /// </summary>
    public static partial class AsyncOperationManager
    {
        /// <summary>
        /// Gets or sets the synchronization context for the asynchronous operation.
        /// </summary>
        /// <returns>
        /// The synchronization context for the asynchronous operation.
        /// </returns>
        [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(2))]
        public static System.Threading.SynchronizationContext SynchronizationContext { get { return default(System.Threading.SynchronizationContext); } set { } }
        /// <summary>
        /// Returns an <see cref="AsyncOperation" /> for tracking the duration
        /// of a particular asynchronous operation.
        /// </summary>
        /// <param name="userSuppliedState">
        /// An object used to associate a piece of client state, such as a task ID, with a particular asynchronous
        /// operation.
        /// </param>
        /// <returns>
        /// An <see cref="AsyncOperation" /> that you can use to track the duration
        /// of an asynchronous method invocation.
        /// </returns>
        public static System.ComponentModel.AsyncOperation CreateOperation(object userSuppliedState) { return default(System.ComponentModel.AsyncOperation); }
    }
    /// <summary>
    /// Executes an operation on a separate thread.
    /// </summary>
    public partial class BackgroundWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorker" />
        /// class.
        /// </summary>
        public BackgroundWorker() { }
        /// <summary>
        /// Gets a value indicating whether the application has requested cancellation of a background
        /// operation.
        /// </summary>
        /// <returns>
        /// true if the application has requested cancellation of a background operation; otherwise, false.
        /// The default is false.
        /// </returns>
        public bool CancellationPending { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="BackgroundWorker" />
        /// is running an asynchronous operation.
        /// </summary>
        /// <returns>
        /// true, if the <see cref="BackgroundWorker" /> is running an asynchronous
        /// operation; otherwise, false.
        /// </returns>
        public bool IsBusy { get { return default(bool); } }
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BackgroundWorker" />
        /// can report progress updates.
        /// </summary>
        /// <returns>
        /// true if the <see cref="BackgroundWorker" /> supports progress updates;
        /// otherwise false. The default is false.
        /// </returns>
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool WorkerReportsProgress { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BackgroundWorker" />
        /// supports asynchronous cancellation.
        /// </summary>
        /// <returns>
        /// true if the <see cref="BackgroundWorker" /> supports cancellation;
        /// otherwise false. The default is false.
        /// </returns>
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool WorkerSupportsCancellation { get { return default(bool); } set { } }
        /// <summary>
        /// Occurs when <see cref="BackgroundWorker.RunWorkerAsync" /> is called.
        /// </summary>
        public event System.ComponentModel.DoWorkEventHandler DoWork { add { } remove { } }
        /// <summary>
        /// Occurs when <see cref="ReportProgress(Int32)" />
        /// is called.
        /// </summary>
        public event System.ComponentModel.ProgressChangedEventHandler ProgressChanged { add { } remove { } }
        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        public event System.ComponentModel.RunWorkerCompletedEventHandler RunWorkerCompleted { add { } remove { } }
        /// <summary>
        /// Requests cancellation of a pending background operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="WorkerSupportsCancellation" /> is false.
        /// </exception>
        public void CancelAsync() { }
        /// <summary>
        /// Raises the <see cref="DoWork" /> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDoWork(System.ComponentModel.DoWorkEventArgs e) { }
        /// <summary>
        /// Raises the <see cref="ProgressChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnProgressChanged(System.ComponentModel.ProgressChangedEventArgs e) { }
        /// <summary>
        /// Raises the <see cref="RunWorkerCompleted" /> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnRunWorkerCompleted(System.ComponentModel.RunWorkerCompletedEventArgs e) { }
        /// <summary>
        /// Raises the <see cref="ProgressChanged" /> event.
        /// </summary>
        /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="WorkerReportsProgress" /> property
        /// is set to false.
        /// </exception>
        public void ReportProgress(int percentProgress) { }
        /// <summary>
        /// Raises the <see cref="ProgressChanged" /> event.
        /// </summary>
        /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
        /// <param name="userState">
        /// The state object passed to
        /// <see cref="RunWorkerAsync(Object)" />.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="WorkerReportsProgress" /> property
        /// is set to false.
        /// </exception>
        public void ReportProgress(int percentProgress, object userState) { }
        /// <summary>
        /// Starts execution of a background operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsBusy" /> is true.
        /// </exception>
        public void RunWorkerAsync() { }
        /// <summary>
        /// Starts execution of a background operation.
        /// </summary>
        /// <param name="argument">
        /// A parameter for use by the background operation to be executed in the
        /// <see cref="DoWork" /> event handler.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsBusy" /> is true.
        /// </exception>
        public void RunWorkerAsync(object argument) { }
    }
    /// <summary>
    /// Provides data for the <see cref="BackgroundWorker.DoWork" /> event
    /// handler.
    /// </summary>
    public partial class DoWorkEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoWorkEventArgs" /> class.
        /// </summary>
        /// <param name="argument">Specifies an argument for an asynchronous operation.</param>
        public DoWorkEventArgs(object argument) { }
        /// <summary>
        /// Gets a value that represents the argument of an asynchronous operation.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> representing the argument of an asynchronous operation.
        /// </returns>
        public object Argument { get { return default(object); } }
        /// <summary>
        /// Gets or sets a value that represents the result of an asynchronous operation.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> representing the result of an asynchronous operation.
        /// </returns>
        public object Result { get { return default(object); } set { } }
    }
    /// <summary>
    /// Represents the method that will handle the <see cref="BackgroundWorker.DoWork" />
    /// event. This class cannot be inherited.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="DoWorkEventArgs" />    that contains the event data.</param>
    public delegate void DoWorkEventHandler(object sender, System.ComponentModel.DoWorkEventArgs e);
    /// <summary>
    /// Provides data for the <see cref="BackgroundWorker.ProgressChanged" />
    /// event.
    /// </summary>
    public partial class ProgressChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressChangedEventArgs" />
        /// class.
        /// </summary>
        /// <param name="progressPercentage">The percentage of an asynchronous task that has been completed.</param>
        /// <param name="userState">A unique user state.</param>
        public ProgressChangedEventArgs(int progressPercentage, object userState) { }
        /// <summary>
        /// Gets the asynchronous task progress percentage.
        /// </summary>
        /// <returns>
        /// A percentage value indicating the asynchronous task progress.
        /// </returns>
        public int ProgressPercentage { get { return default(int); } }
        /// <summary>
        /// Gets a unique user state.
        /// </summary>
        /// <returns>
        /// A unique <see cref="Object" /> indicating the user state.
        /// </returns>
        public object UserState { get { return default(object); } }
    }
    /// <summary>
    /// Represents the method that will handle the
    /// <see cref="BackgroundWorker.ProgressChanged" /> event of the <see cref="BackgroundWorker" /> class. This class
    /// cannot be inherited.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    /// A <see cref="ProgressChangedEventArgs" />   that contains the event
    /// data.
    /// </param>
    public delegate void ProgressChangedEventHandler(object sender, System.ComponentModel.ProgressChangedEventArgs e);
    /// <summary>
    /// Provides data for the MethodNameCompleted event.
    /// </summary>
    public partial class RunWorkerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunWorkerCompletedEventArgs" />
        /// class.
        /// </summary>
        /// <param name="result">The result of an asynchronous operation.</param>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled">A value indicating whether the asynchronous operation was canceled.</param>
        public RunWorkerCompletedEventArgs(object result, System.Exception error, bool cancelled) : base(default(System.Exception), default(bool), default(object)) { }
        /// <summary>
        /// Gets a value that represents the result of an asynchronous operation.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> representing the result of an asynchronous operation.
        /// </returns>
        /// <exception cref="Reflection.TargetInvocationException">
        /// <see cref="AsyncCompletedEventArgs.Error" /> is not null. The
        /// <see cref="Exception.InnerException" /> property holds a reference to
        /// <see cref="AsyncCompletedEventArgs.Error" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="AsyncCompletedEventArgs.Cancelled" /> is true.
        /// </exception>
        public object Result { get { return default(object); } }
        /// <summary>
        /// Gets a value that represents the user state.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> representing the user state.
        /// </returns>
        [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
        public new object UserState { get { return default(object); } }
    }
    /// <summary>
    /// Represents the method that will handle the
    /// <see cref="BackgroundWorker.RunWorkerCompleted" /> event of a <see cref="BackgroundWorker" /> class.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    /// A <see cref="RunWorkerCompletedEventArgs" />   that contains the event
    /// data.
    /// </param>
    public delegate void RunWorkerCompletedEventHandler(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e);
}
