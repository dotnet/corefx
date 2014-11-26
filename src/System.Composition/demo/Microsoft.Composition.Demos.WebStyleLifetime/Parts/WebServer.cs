// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStyleLifetimeDemo.Extension;

namespace WebStyleLifetimeDemo.Parts
{
    [Export]
    public class WebServer
    {
        private ExportFactory<CompositionContext> _requestScopeFactory;

        [ImportingConstructor]
        public WebServer(
            [SharingBoundary(Boundaries.DataConsistency, Boundaries.UserIdentity, Boundaries.HttpRequest)]
            ExportFactory<CompositionContext> requestScopeFactory)
        {
            _requestScopeFactory = requestScopeFactory;
        }

        public void Get(string path)
        {
            var controllerTypeNameSuffix = path[0].ToString().ToUpper() + path.Substring(1) + "Controller";
            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).First(t => t.Name.EndsWith(controllerTypeNameSuffix));

            using (var requestScope = _requestScopeFactory.CreateExport())
            {
                var controller = (IController)requestScope.Value.GetExport(type);
                controller.Get();
            }
        }
    }
}
