// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.ServiceModel.Channels;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Syndication
{
    [XmlRoot(ElementName = App10Constants.Categories, Namespace = App10Constants.Namespace)]
    public class AtomPub10CategoriesDocumentFormatter : CategoriesDocumentFormatter, IXmlSerializable
    {
        private Type _inlineDocumentType;
        private int _maxExtensionSize;
        private bool _preserveAttributeExtensions;
        private bool _preserveElementExtensions;
        private Type _referencedDocumentType;

        public AtomPub10CategoriesDocumentFormatter()
            : this(typeof(InlineCategoriesDocument), typeof(ReferencedCategoriesDocument))
        {
        }

        public AtomPub10CategoriesDocumentFormatter(Type inlineDocumentType, Type referencedDocumentType)
            : base()
        {
            if (inlineDocumentType == null)
            {
                throw new ArgumentNullException(nameof(inlineDocumentType));
            }
            if (!typeof(InlineCategoriesDocument).IsAssignableFrom(inlineDocumentType))
            {
                throw new ArgumentException(SR.Format(SR.InvalidObjectTypePassed, nameof(inlineDocumentType), nameof(InlineCategoriesDocument)), nameof(inlineDocumentType));
            }
            if (referencedDocumentType == null)
            {
                throw new ArgumentNullException(nameof(referencedDocumentType));
            }
            if (!typeof(ReferencedCategoriesDocument).IsAssignableFrom(referencedDocumentType))
            {
                throw new ArgumentException(SR.Format(SR.InvalidObjectTypePassed, nameof(referencedDocumentType), nameof(ReferencedCategoriesDocument)), nameof(referencedDocumentType));
            }

            _maxExtensionSize = int.MaxValue;
            _preserveAttributeExtensions = true;
            _preserveElementExtensions = true;
            _inlineDocumentType = inlineDocumentType;
            _referencedDocumentType = referencedDocumentType;
        }

        public AtomPub10CategoriesDocumentFormatter(CategoriesDocument documentToWrite)
            : base(documentToWrite)
        {
            // No need to check that the parameter passed is valid - it is checked by the c'tor of the base class
            _maxExtensionSize = int.MaxValue;
            _preserveAttributeExtensions = true;
            _preserveElementExtensions = true;
            if (documentToWrite.IsInline)
            {
                _inlineDocumentType = documentToWrite.GetType();
                _referencedDocumentType = typeof(ReferencedCategoriesDocument);
            }
            else
            {
                _referencedDocumentType = documentToWrite.GetType();
                _inlineDocumentType = typeof(InlineCategoriesDocument);
            }
        }

        public override string Version
        {
            get { return App10Constants.Namespace; }
        }

        public override bool CanRead(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsStartElement(App10Constants.Categories, App10Constants.Namespace);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            
            ReadDocument(reader);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (this.Document == null)
            {
                throw new InvalidOperationException(SR.DocumentFormatterDoesNotHaveDocument);
            }

            WriteDocument(writer);
        }

        public override void ReadFrom(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (!CanRead(reader))
            {
                throw new XmlException(SR.Format(SR.UnknownDocumentXml, reader.LocalName, reader.NamespaceURI));
            }

            ReadDocument(reader);
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (this.Document == null)
            {
                throw new InvalidOperationException(SR.DocumentFormatterDoesNotHaveDocument);
            }
            
            writer.WriteStartElement(App10Constants.Prefix, App10Constants.Categories, App10Constants.Namespace);
            WriteDocument(writer);
            writer.WriteEndElement();
        }

        protected override InlineCategoriesDocument CreateInlineCategoriesDocument()
        {
            if (_inlineDocumentType == typeof(InlineCategoriesDocument))
            {
                return new InlineCategoriesDocument();
            }
            else
            {
                return (InlineCategoriesDocument)Activator.CreateInstance(_inlineDocumentType);
            }
        }

        protected override ReferencedCategoriesDocument CreateReferencedCategoriesDocument()
        {
            if (_referencedDocumentType == typeof(ReferencedCategoriesDocument))
            {
                return new ReferencedCategoriesDocument();
            }
            else
            {
                return (ReferencedCategoriesDocument)Activator.CreateInstance(_referencedDocumentType);
            }
        }

        private void ReadDocument(XmlReader reader)
        {
            try
            {
                SyndicationFeedFormatter.MoveToStartElement(reader);
                SetDocument(AtomPub10ServiceDocumentFormatter.ReadCategories(reader, null,
                    delegate ()
                    {
                        return this.CreateInlineCategoriesDocument();
                    },

                    delegate ()
                    {
                        return this.CreateReferencedCategoriesDocument();
                    },
                    this.Version,
                    _preserveElementExtensions,
                    _preserveAttributeExtensions,
                    _maxExtensionSize));
            }
            catch (FormatException e)
            {
                throw new XmlException(FeedUtils.AddLineInfo(reader, SR.ErrorParsingDocument), e);
            }
            catch (ArgumentException e)
            {
                throw new XmlException(FeedUtils.AddLineInfo(reader, SR.ErrorParsingDocument), e);
            }
        }

        private void WriteDocument(XmlWriter writer)
        {
            // declare the atom10 namespace upfront for compactness
            writer.WriteAttributeString(Atom10Constants.Atom10Prefix, Atom10FeedFormatter.XmlNsNs, Atom10Constants.Atom10Namespace);
            AtomPub10ServiceDocumentFormatter.WriteCategoriesInnerXml(writer, this.Document, null, this.Version);
        }
    }
}
