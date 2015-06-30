// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>Contains information for a socket's linger time, the amount of time it will
    ///       remain after closing if data remains to be sent.</para>
    /// </devdoc>
    public class LingerOption
    {
        private bool _enabled;
        private int _lingerTime;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='Sockets.LingerOption'/> class.
        ///    </para>
        /// </devdoc>
        public LingerOption(bool enable, int seconds)
        {
            Enabled = enable;
            LingerTime = seconds;
        }

        /// <devdoc>
        ///    <para>
        ///       Enables or disables lingering after
        ///       close.
        ///    </para>
        /// </devdoc>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The amount of time, in seconds, to remain connected after a close.
        ///    </para>
        /// </devdoc>
        public int LingerTime
        {
            get
            {
                return _lingerTime;
            }
            set
            {
                _lingerTime = value;
            }
        }
    } // class LingerOption
} // namespace System.Net.Sockets
