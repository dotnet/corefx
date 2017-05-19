﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Configuration
{
    public class SettingsPropertyIsReadOnlyException : Exception
    {
        public SettingsPropertyIsReadOnlyException(String message)
             : base(message)
        {
        }

        public SettingsPropertyIsReadOnlyException(String message, Exception innerException)
             : base(message, innerException)
        {
        }

        protected SettingsPropertyIsReadOnlyException(SerializationInfo info, StreamingContext context)
             : base(info, context)
        {
        }

        public SettingsPropertyIsReadOnlyException()
        { }
    }
}
