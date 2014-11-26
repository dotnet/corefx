// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

namespace DesktopStyleLifetimeDemo.Parts
{
    [Shared(Document.SharingBoundaryName)]
    public class Document
    {
        public const string SharingBoundaryName = "Document";

        public string Name { get; set; }

        [Import]
        public Application Application { get; set; }

        [Import]
        public Project Project { get; set; }

        public void Dump()
        {
            Console.WriteLine("Document: {0}, Application = {1}, Project = {2}", Name, Application.Name, Project.Name);
        }
    }
}
