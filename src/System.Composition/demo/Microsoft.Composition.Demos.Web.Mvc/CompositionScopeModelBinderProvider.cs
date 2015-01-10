// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.Composition.Demos.Web.Mvc
{
    class CompositionScopeModelBinderProvider : IModelBinderProvider
    {
        private const string ModelBinderContractNameSuffix = "++ModelBinder";

        public static string GetModelBinderContractName(Type modelType)
        {
            return modelType.AssemblyQualifiedName + ModelBinderContractNameSuffix;
        }

        public IModelBinder GetBinder(Type modelType)
        {
            IModelBinder export;
            if (!CompositionProvider.Current.TryGetExport(GetModelBinderContractName(modelType), out export))
                return null;

            return export;
        }
    }
}
