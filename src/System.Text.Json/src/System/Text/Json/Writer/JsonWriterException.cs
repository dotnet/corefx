// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    //TODO: Mark as serializable
    public sealed class JsonWriterException : Exception
    {
        public JsonWriterException(string message) : base(message)
        {
        }
    }
}
