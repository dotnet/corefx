// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlConfigurationDemo.Extension
{
    public static class ContainerConfigurationExtensions
    {
        public static ContainerConfiguration WithPartsFromXml(
            this ContainerConfiguration configuration,
            string configurationSectionName = Constants.DefaultConfigurationSectionName)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (configurationSectionName == null) throw new ArgumentNullException("configurationSectionName");

            var section = (ConfigurationSection)ConfigurationManager.GetSection(configurationSectionName);

            var types = new List<Type>();
            var registrationBulider = new ConventionBuilder();

            foreach (var partConfiguration in section.Parts)
            {
                var type = Type.GetType(partConfiguration.Type);
                types.Add(type);

                var partBuilder = registrationBulider.ForType(type);
                var exported = false;

                foreach (var export in partConfiguration.Exports)
                {
                    exported = true;
                    var contractType = Type.GetType(export.ContractType);
                    partBuilder.Export(eb => eb.AsContractType(contractType));
                }

                if (!exported)
                    partBuilder.Export();
            }

            configuration.WithParts(types, registrationBulider);

            return configuration;
        }
    }
}
