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
    class CompositionScopeFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        public CompositionScopeFilterAttributeFilterProvider()
            : base(cacheAttributeInstances: false)
        { }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToArray();
            ComposeAttributes(attributes);
            return attributes;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToArray();
            ComposeAttributes(attributes);
            return attributes;
        }

        void ComposeAttributes(FilterAttribute[] attributes)
        {
            foreach (var attribute in attributes)
                CompositionProvider.Current.SatisfyImports(attribute);
        }
    }
}
