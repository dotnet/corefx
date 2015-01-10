//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;

    internal class SyndicationSerializer<TContent>
    {
        private static readonly DataContractSerializer contentSerializer = new DataContractSerializer(typeof(TContent));

        public void SerializeFeed(SyndicationFeed<TContent> feed, XmlWriter writer)
        {
            writer.WriteStartElement("feed", "http://www.w3.org/2005/Atom");

            this.SerializeProperties(feed, writer);

            foreach (var item in feed.Items)
            {
                this.SerializeItem(item, writer);
            }

            writer.WriteEndElement();
        }

        public void SerializeItem(SyndicationItem<TContent> item, XmlWriter writer)
        {
            writer.WriteStartElement("entry", "http://www.w3.org/2005/Atom");

            this.SerializeProperties(item, writer);

            if (item.Content != null)
            {
                writer.WriteStartElement("content");

                if (!string.IsNullOrEmpty(item.ContentType))
                {
                    writer.WriteAttributeString("type", item.ContentType);
                }

                contentSerializer.WriteObject(writer, item.Content);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public SyndicationFeed<TContent> DeserializeFeed(XmlReader reader)
        {
            var result = new SyndicationFeed<TContent>();

            reader.ReadStartElement("feed", "http://www.w3.org/2005/Atom");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    this.DeserializeProperty(reader, result);
                }
            }

            return result;
        }

        public SyndicationItem<TContent> DeserializeItem(XmlReader reader)
        {
            var result = new SyndicationItem<TContent>();

            reader.ReadStartElement("entry", "http://www.w3.org/2005/Atom");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    this.DeserializeProperty(reader, result);
                }
            }

            return result;
        }

        private void SerializeProperties(SyndicationItemBase item, XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(item.Id))
            {
                writer.WriteElementString("id", item.Id);
            }

            if (!string.IsNullOrEmpty(item.Id))
            {
                writer.WriteStartElement("title");
                writer.WriteAttributeString("type", "text");
                writer.WriteValue(item.Title);
                writer.WriteEndElement();
            }

            if (item.PublishDate.HasValue)
            {
                writer.WriteElementString("published", item.PublishDate.Value.ToString("u"));
            }

            if (item.LastUpdatedTime.HasValue)
            {
                writer.WriteElementString("updated", item.LastUpdatedTime.Value.ToString("u"));
            }

            if (item.SelfLink != null)
            {
                writer.WriteStartElement("link");
                writer.WriteAttributeString("rel", "self");
                writer.WriteAttributeString("href", item.SelfLink.ToString());
                writer.WriteEndElement();
            }
        }

        private void DeserializeProperty(XmlReader reader, SyndicationFeed<TContent> feed)
        {
            if (reader.Name == "entry")
            {
                var item = this.DeserializeItem(reader.ReadSubtree());
                feed.Items.Add(item);
            }
            else
            {
                this.DeserializeProperty(reader, (SyndicationItemBase)feed);
            }
        }

        private void DeserializeProperty(XmlReader reader, SyndicationItem<TContent> item)
        {
            if (reader.Name == "content")
            {
                item.ContentType = reader.GetAttribute("type");

                reader.ReadStartElement();
                item.Content = (TContent)contentSerializer.ReadObject(reader);
            }
            else
            {
                this.DeserializeProperty(reader, (SyndicationItemBase)item);
            }
        }

        private void DeserializeProperty(XmlReader reader, SyndicationItemBase item)
        {
            switch (reader.Name)
            {
                case "id":
                    item.Id = reader.ReadElementContentAsString();
                    break;

                case "title":
                    item.Title = reader.ReadElementContentAsString();
                    break;

                case "published":
                    item.PublishDate = XmlConvert.ToDateTimeOffset(reader.ReadElementContentAsString());
                    break;

                case "updated":
                    item.LastUpdatedTime = XmlConvert.ToDateTimeOffset(reader.ReadElementContentAsString());
                    break;

                case "author":
                    reader.ReadToDescendant("name");
                    item.Author = reader.ReadElementContentAsString();
                    break;

                case "link":
                    var rel = reader.GetAttribute("rel");
                    if (rel == "self")
                    {
                        var href = reader.GetAttribute("href");

                        Uri linkUri = null;
                        var isAbsoluteUri = Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out linkUri);
                        if (!isAbsoluteUri)
                        {
                            linkUri = new Uri(href, UriKind.Relative);
                        }

                        item.SelfLink = linkUri;
                    }

                    break;
            }
        }
    }
}
