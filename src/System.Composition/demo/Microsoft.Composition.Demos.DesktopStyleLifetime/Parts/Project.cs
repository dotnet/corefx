// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

namespace DesktopStyleLifetimeDemo.Parts
{
    [Shared(Project.SharingBoundaryName)]
    public class Project
    {
        public const string SharingBoundaryName = "Project";

        private readonly ExportFactory<Document> _documentFactory;
        private readonly IDictionary<string, Export<Document>> _documents = new Dictionary<string, Export<Document>>();

        public Project(
            [SharingBoundary(Document.SharingBoundaryName)]
            ExportFactory<Document> documentFactory)
        {
            _documentFactory = documentFactory;
        }

        public string Name { get; set; }

        [Import]
        public Application Application { get; set; }

        public Document OpenDocument(string name)
        {
            var document = _documentFactory.CreateExport();
            document.Value.Name = name;
            _documents[name] = document;
            return document.Value;
        }

        public void CloseDocument(Document document)
        {
            var elc = _documents[document.Name];
            elc.Dispose();
            _documents.Remove(document.Name);
        }

        public void Dump()
        {
            Console.WriteLine("Project: {0}, Application = {1}", Name, Application.Name);
            foreach (var document in _documents)
            {
                document.Value.Value.Dump();
            }
        }
    }
}
