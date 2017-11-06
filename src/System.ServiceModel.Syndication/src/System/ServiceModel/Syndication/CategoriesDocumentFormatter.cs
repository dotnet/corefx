// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Syndication
{
    [DataContract]
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("documentToWrite");
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
        public abstract void ReadFrom(XmlReader reader);
        public abstract void WriteTo(XmlWriter writer);

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
