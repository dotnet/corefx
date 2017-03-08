﻿namespace System.Diagnostics
{
    public abstract partial class DiagnosticSource
    {
        /// <summary>
        /// Starts an Activity and writes start event.
        /// 
        /// Activity describes logical operation, its context and parent relation; 
        /// Current activity flows through the operation processing.
        /// 
        /// This method starts given Activity (maintains global Current Activity 
        /// and Parent for the given activity) and notifies consumers  that new Activity 
        /// was started. Consumers could access <see cref="Activity.Current"/>
        /// to add context and/or augment telemetry.
        /// 
        /// Producers may pass additional details to the consumer in the payload.
        /// </summary>
        /// <param name="activity">Activity to be started</param>
        /// <param name="args">An object that represent the value being passed as a payload for the event.</param>
        /// <returns>Started Activity for convenient chaining</returns>
        /// <seealso cref="Activity"/>
        public Activity StartActivity(Activity activity, object args)
        {
            activity.Start();
            Write(activity.OperationName + ".Start", args);
            return activity;
        }

        /// <summary>
        /// Stops given Activity: maintains global Current Activity and notifies consumers 
        /// that Activity was stopped. Consumers could access <see cref="Activity.Current"/>
        /// to add context and/or augment telemetry.
        /// 
        /// Producers may pass additional details to the consumer in the payload.
        /// </summary>
        /// <param name="activity">Activity to be stopped</param>
        /// <param name="args">An object that represent the value being passed as a payload for the event.</param>
        /// <seealso cref="Activity"/>
        public void StopActivity(Activity activity, object args)
        {
            // Stop sets the end time if it was unset, but we want it set before we issue the write
            // so we do it now.   
            if (activity.Duration == TimeSpan.Zero)
                activity.SetEndTime(DateTime.UtcNow);
            Write(activity.OperationName + ".Stop", args);
            activity.Stop();    // Resets Activity.Current (we want this after the Write)
        }
    }
}
