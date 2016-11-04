// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);

    public class DoWorkEventArgs : CancelEventArgs
    {
        private readonly object _argument;
        private object _result;

        public DoWorkEventArgs(object argument)
        {
            _argument = argument;
        }

        public object Argument => _argument;

        public object Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
