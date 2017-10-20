// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Syndication
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

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
                throw new ArgumentException(SR.Format(SR.InvalidObjectTypePassed, nameof(inlineDocumentType), nameof(InlineCategoriesDocument)));
            }

            if (referencedDocumentType == null)
            {
                throw new ArgumentNullException(nameof(referencedDocumentType));
            }

            if (!typeof(ReferencedCategoriesDocument).IsAssignableFrom(referencedDocumentType))
            {
                throw new ArgumentException(SR.Format(SR.InvalidObjectTypePassed, nameof(referencedDocumentType), nameof(ReferencedCategoriesDocument)));
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

            reader = XmlReaderWrapper.CreateFromReader(reader);
            return reader.IsStartElement(App10Constants.Categories, App10Constants.Namespace);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            ReadDocumentAsync(XmlReaderWrapper.CreateFromReader(reader)).GetAwaiter().GetResult();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (Document == null)
            {
                throw new InvalidOperationException(SR.DocumentFormatterDoesNotHaveDocument);
            }

            WriteDocumentAsync(XmlWriterWrapper.CreateFromWriter(writer)).GetAwaiter().GetResult();
        }

        public override void ReadFrom(XmlReader reader)
        {
            ReadFromAsync(reader).GetAwaiter().GetResult();
        }

        public override void WriteTo(XmlWriter writer)
        {
            WriteToAsync(writer).GetAwaiter().GetResult();
        }

        public override async Task ReadFromAsync(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!CanRead(reader))
            {
                throw new XmlException(SR.Format(SR.UnknownDocumentXml, reader.LocalName, reader.NamespaceURI));
            }

            await ReadDocumentAsync(XmlReaderWrapper.CreateFromReader(reader));
        }

        public override async Task WriteToAsync(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (Document == null)
            {
                throw new InvalidOperationException(SR.DocumentFormatterDoesNotHaveDocument);
            }

            writer.WriteStartElement(App10Constants.Prefix, App10Constants.Categories, App10Constants.Namespace);
            await WriteDocumentAsync(writer);
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

        private async Task ReadDocumentAsync(XmlReader reader)
        {
            try
            {
                await SyndicationFeedFormatter.MoveToStartElementAsync(reader);
                SetDocument(await AtomPub10ServiceDocumentFormatter.ReadCategories(reader, null,
                    delegate ()
                    {
                        return CreateInlineCategoriesDocument();
                    },

                    delegate ()
                    {
                        return CreateReferencedCategoriesDocument();
                    },
                    Version,
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

        private Task WriteDocumentAsync(XmlWriter writer)
        {
            // declare the atom10 namespace upfront for compactness
            writer.WriteAttributeString(Atom10Constants.Atom10Prefix, Atom10FeedFormatter.XmlNsNs, Atom10Constants.Atom10Namespace);
            return AtomPub10ServiceDocumentFormatter.WriteCategoriesInnerXml(writer, Document, null, Version);
        }
    }
}
