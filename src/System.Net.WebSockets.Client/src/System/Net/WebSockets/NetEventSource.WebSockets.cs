// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-WebSockets-Client")]
    // Unblock reflection in ILC mode by declaring types and methods as public.
    // An alternative would be to unblock reflection via rd.xml/csproj changes, but that would
    // result in a size hit for all of the metadata and we would 
#if uap
    public
#else
    internal
#endif
    sealed partial class NetEventSource { }
}
