// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildTimeCodeGeneration.Parts;

namespace BuildTimeCodeGeneration.Generated
{
    // This code is identical to what the container would generate at runtime.
    //
    class BuildTimeCodeGeneration_ExportDescriptorProvider : ExportDescriptorProvider
    {
        // The source will only be asked for parts once per exportKey, but it is the
        // source's responsibility to return the same part multiple times when that part
        // has more than one export (not shown here.)
        //
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (contract.Equals(new CompositionContract(typeof(RequestListener))))
                return new[] { RequestListenerPart(contract, definitionAccessor) };

            if (contract.Equals(new CompositionContract(typeof(ConsoleLog))))
                return new[] { ConsoleLogPart(contract, definitionAccessor) };

            return NoExportDescriptors;
        }

        // Console log is a disposable singleton (no boundaries)
        // that exports itself under its own concrete type.
        //
        ExportDescriptorPromise ConsoleLogPart(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
                contract,
                typeof(ConsoleLog).Name,
                true,
                NoDependencies,
                _ =>
                {
                    var sharingId = LifetimeContext.AllocateSharingId();

                    return ExportDescriptor.Create((c, o) =>
                    {
                        CompositeActivator activatorBody = (sc, so) =>
                        {
                            var result = new ConsoleLog();
                            c.AddBoundInstance(result);
                            return result;
                        };

                        var scope = c.FindContextWithin(null);
                        if (object.ReferenceEquals(scope, c))
                            return scope.GetOrCreate(sharingId, o, activatorBody);
                        else
                            return CompositionOperation.Run(scope, (c1, o1) => c1.GetOrCreate(sharingId, o1, activatorBody));
                    }, NoMetadata);
                });
        }

        // Non-shared part that exports itself and has a dependency on ConsoleLog.
        //
        ExportDescriptorPromise RequestListenerPart(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            return new ExportDescriptorPromise(
                contract,
                typeof(RequestListener).Name,
                false,
                () => new[] { definitionAccessor.ResolveRequiredDependency("log", new CompositionContract(typeof(Lazy<ConsoleLog>)), true) },
                dependencies =>
                {
                    var logActivator = dependencies.Single().Target.GetDescriptor().Activator;
                    return ExportDescriptor.Create((c, o) =>
                    {
                        var log = (Lazy<ConsoleLog>)logActivator(c, o);
                        return new RequestListener(log);
                    }, NoMetadata);
                });
        }
    }
}
