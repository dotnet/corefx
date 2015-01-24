// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for a cancelable event.
    /// </summary>
    public class CancelEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether we should cancel the operation or not
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initializes a new instance of the CancelEventArgs class with the Cancel property set to false.
        /// </summary>
        public CancelEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CancelEventArgs class with the Cancel property set to the given value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false.</param>
        public CancelEventArgs(bool cancel)
        {
            this.Cancel = cancel;
        }
    }
}
