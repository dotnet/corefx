// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopStyleLifetimeDemo.Parts
{
    [Shared]
    public class Application
    {
        private readonly ExportFactory<Project> _projectFactory;
        private readonly IDictionary<string, Export<Project>> _projects = new Dictionary<string, Export<Project>>();

        public Application(
            [SharingBoundary(Project.SharingBoundaryName)]
            ExportFactory<Project> projectFactory)
        {
            _projectFactory = projectFactory;
        }

        public string Name { get; set; }

        public void Run(string applicationName)
        {
            Name = applicationName;

            var tests = OpenProject("Tests.csproj");
            var miw = tests.OpenDocument("MakeItWork.cs");

            var web = OpenProject("Web.csproj");
            var ctrl = web.OpenDocument("Controller.cs");
            var mod = web.OpenDocument("Model.cs");

            Dump();

            tests.CloseDocument(miw);
            CloseProject(tests);

            web.CloseDocument(mod);
            web.CloseDocument(ctrl);
            CloseProject(web);
        }

        public Project OpenProject(string name)
        {
            var project = _projectFactory.CreateExport();
            project.Value.Name = name;
            _projects[name] = project;
            return project.Value;
        }

        public void CloseProject(Project project)
        {
            var elc = _projects[project.Name];
            elc.Dispose();
            _projects.Remove(project.Name);
        }

        public void Dump()
        {
            Console.WriteLine("Application: {0}", Name);
            Console.WriteLine("Projects:");
            foreach (var project in _projects)
            {
                project.Value.Value.Dump();
            }
        }
    }
}
