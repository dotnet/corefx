// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

namespace System.Diagnostics
{
    public class SourceFilter : TraceFilter
    {
        private string _src;

        public SourceFilter(string source)
        {
            Source = source;
        }

        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage,
                                         object[] args, object data1, object[] data)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return string.Equals(_src, source);
        }

        public string Source
        {
            get
            {
                return _src;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Source));
                _src = value;
            }
        }
    }
}

