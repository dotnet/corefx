// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
            ProviderMetadata providerMetadata = new ProviderMetadata("");
            string name = providerMetadata.Name;
            Guid id = providerMetadata.Id;
            string messageFilePath = providerMetadata.MessageFilePath;
            string resourceFilePath = providerMetadata.ResourceFilePath;
            string parameterFilePath = providerMetadata.ParameterFilePath;
            Uri helpLink = providerMetadata.HelpLink;
            string displayName = providerMetadata.DisplayName;
            IList<EventLogLink> logLinks = providerMetadata.LogLinks;
            IList<EventLevel> levels = providerMetadata.Levels;
            IList<EventOpcode> ppcodes = providerMetadata.Opcodes;
            IList<EventKeyword> keywords = providerMetadata.Keywords;
            IEnumerable<EventMetadata> events = providerMetadata.Events;
            IList<EventTask> tasks = providerMetadata.Tasks;
        }
    }
}
