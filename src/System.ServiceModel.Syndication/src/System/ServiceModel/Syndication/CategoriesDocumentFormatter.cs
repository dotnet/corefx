// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Syndication
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;

    public abstract class CategoriesDocumentFormatter
    {
        private CategoriesDocument _document;

        protected CategoriesDocumentFormatter()
        {
        }
        protected CategoriesDocumentFormatter(CategoriesDocument documentToWrite)
        {
            if (documentToWrite == null)
            {
                throw new ArgumentNullException(nameof(documentToWrite));
            }
            _document = documentToWrite;
        }

        public CategoriesDocument Document
        {
            get { return _document; }
        }

        public abstract string Version
        { get; }

        public abstract bool CanRead(XmlReader reader);
        public abstract Task ReadFromAsync(XmlReader reader);
        public abstract Task WriteTo(XmlWriter writer);

        protected virtual InlineCategoriesDocument CreateInlineCategoriesDocument()
        {
            return new InlineCategoriesDocument();
        }

        protected virtual ReferencedCategoriesDocument CreateReferencedCategoriesDocument()
        {
            return new ReferencedCategoriesDocument();
        }

        protected virtual void SetDocument(CategoriesDocument document)
        {
            _document = document;
        }
    }
}
