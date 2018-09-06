// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
** Purpose:
** This public class defines the methods / properties for the
** individual Data Values of an EventRecord.  Instances of this
** class are obtained from EventRecord.
**
============================================================*/

namespace System.Diagnostics.Eventing.Reader
{
    public sealed class EventProperty
    {
        private object _value;

        internal EventProperty(object value)
        {
            _value = value;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }
    }
}
