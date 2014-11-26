//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class Entity<TDescription>
    {
        private static readonly SyndicationSerializer<TDescription> serializer = new SyndicationSerializer<TDescription>();

        private readonly SyndicationItem<TDescription> syndicationItem;

        internal Entity(string name, TDescription description)
            : this(new SyndicationItem<TDescription>())
        {
            this.SyndicationItem.Title = name;
            this.SyndicationItem.Content = description;
        }

        internal Entity(SyndicationItem<TDescription> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.syndicationItem = item;
        }

        public Uri Uri
        {
            get { return this.syndicationItem.SelfLink; }
        }

        public string Name
        {
            get { return this.syndicationItem.Title; }
        }

        public TDescription Description
        {
            get { return this.syndicationItem.Content; }
            set { this.syndicationItem.Content = value; }
        }

        internal static SyndicationSerializer<TDescription> Serializer
        {
            get { return serializer; }
        }

        internal SyndicationItem<TDescription> SyndicationItem
        {
            get { return this.syndicationItem; }
        }

        protected internal virtual string Path
        {
            get { return this.Name; }
        }

        public static Entity<TDescription> Create(Stream stream)
        {
            Entity<TDescription> result;

            using (var reader = XmlReader.Create(stream))
            {
                var syndicationItem = serializer.DeserializeItem(reader);
                result = new Entity<TDescription>(syndicationItem);
            }

            return result;
        }

        /// <summary>
        /// Returns a the Atom Xml that represents this instance.
        /// </summary>
        public override string ToString()
        {
            var xmlWriterSettings = new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stream = new MemoryStream())
            {
                this.WriteTo(stream);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public void WriteTo(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                serializer.SerializeItem(this.syndicationItem, writer);
                writer.Flush();
            }
        }
    }
}
