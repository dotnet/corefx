// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProviderMetadataTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_DoNotThrow()
        {
            var providerMetadata = new ProviderMetadata("");
            var Name = providerMetadata.Name;
            var Id = providerMetadata.Id;
            var MessageFilePath = providerMetadata.MessageFilePath;
            var ResourceFilePath = providerMetadata.ResourceFilePath;
            var ParameterFilePath = providerMetadata.ParameterFilePath;
            var HelpLink = providerMetadata.HelpLink;
            var DisplayName = providerMetadata.DisplayName;
            var LogLinks = providerMetadata.LogLinks;
            var Levels = providerMetadata.Levels;
            var Opcodes = providerMetadata.Opcodes;
            var Keywords = providerMetadata.Keywords;
            var Events = providerMetadata.Events;
            var Tasks = providerMetadata.Tasks;
        }
    }
}
