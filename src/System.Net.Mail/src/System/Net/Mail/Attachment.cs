// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
    public abstract class AttachmentBase : IDisposable
    {
        internal bool disposed = false;
        private MimePart _part = new MimePart();
        private static readonly char[] s_contentCIDInvalidChars = new char[] { '<', '>' };

        internal AttachmentBase()
        {
        }

        protected AttachmentBase(string fileName)
        {
            SetContentFromFile(fileName, string.Empty);
        }

        protected AttachmentBase(string fileName, string mediaType)
        {
            SetContentFromFile(fileName, mediaType);
        }

        protected AttachmentBase(string fileName, ContentType contentType)
        {
            SetContentFromFile(fileName, contentType);
        }

        protected AttachmentBase(Stream contentStream)
        {
            _part.SetContent(contentStream);
        }

        protected AttachmentBase(Stream contentStream, string mediaType)
        {
            _part.SetContent(contentStream, null, mediaType);
        }

        internal AttachmentBase(Stream contentStream, string name, string mediaType)
        {
            _part.SetContent(contentStream, name, mediaType);
        }

        protected AttachmentBase(Stream contentStream, ContentType contentType)
        {
            _part.SetContent(contentStream, contentType);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                _part.Dispose();
            }
        }

        internal void SetContentFromFile(string fileName, ContentType contentType)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName == string.Empty)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, nameof(fileName)), nameof(fileName));
            }

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _part.SetContent(stream, contentType);
        }

        internal void SetContentFromFile(string fileName, string mediaType)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName == string.Empty)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, nameof(fileName)), nameof(fileName));
            }

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _part.SetContent(stream, null, mediaType);
        }

        internal void SetContentFromString(string content, ContentType contentType)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (_part.Stream != null)
            {
                _part.Stream.Close();
            }

            Encoding encoding;

            if (contentType != null && contentType.CharSet != null)
            {
                encoding = Text.Encoding.GetEncoding(contentType.CharSet);
            }
            else
            {
                if (MimeBasePart.IsAscii(content, false))
                {
                    encoding = Text.Encoding.ASCII;
                }
                else
                {
                    encoding = Text.Encoding.GetEncoding(MimeBasePart.DefaultCharSet);
                }
            }
            byte[] buffer = encoding.GetBytes(content);
            _part.SetContent(new MemoryStream(buffer), contentType);

            if (MimeBasePart.ShouldUseBase64Encoding(encoding))
            {
                _part.TransferEncoding = TransferEncoding.Base64;
            }
            else
            {
                _part.TransferEncoding = TransferEncoding.QuotedPrintable;
            }
        }

        internal void SetContentFromString(string content, Encoding encoding, string mediaType)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (_part.Stream != null)
            {
                _part.Stream.Close();
            }

            if (mediaType == null || mediaType == string.Empty)
            {
                mediaType = MediaTypeNames.Text.Plain;
            }

            //validate the mediaType
            int offset = 0;
            try
            {
                string value = MailBnfHelper.ReadToken(mediaType, ref offset, null);
                if (value.Length == 0 || offset >= mediaType.Length || mediaType[offset++] != '/')
                    throw new ArgumentException(SR.MediaTypeInvalid, nameof(mediaType));
                value = MailBnfHelper.ReadToken(mediaType, ref offset, null);
                if (value.Length == 0 || offset < mediaType.Length)
                {
                    throw new ArgumentException(SR.MediaTypeInvalid, nameof(mediaType));
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException(SR.MediaTypeInvalid, nameof(mediaType));
            }


            ContentType contentType = new ContentType(mediaType);

            if (encoding == null)
            {
                if (MimeBasePart.IsAscii(content, false))
                {
                    encoding = Encoding.ASCII;
                }
                else
                {
                    encoding = Encoding.GetEncoding(MimeBasePart.DefaultCharSet);
                }
            }

            contentType.CharSet = encoding.BodyName;
            byte[] buffer = encoding.GetBytes(content);
            _part.SetContent(new MemoryStream(buffer), contentType);

            if (MimeBasePart.ShouldUseBase64Encoding(encoding))
            {
                _part.TransferEncoding = TransferEncoding.Base64;
            }
            else
            {
                _part.TransferEncoding = TransferEncoding.QuotedPrintable;
            }
        }


        internal virtual void PrepareForSending(bool allowUnicode)
        {
            _part.ResetStream();
        }

        public Stream ContentStream
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                return _part.Stream;
            }
        }

        public string ContentId
        {
            get
            {
                string cid = _part.ContentID;
                if (string.IsNullOrEmpty(cid))
                {
                    cid = Guid.NewGuid().ToString();
                    ContentId = cid;
                    return cid;
                }
                if (cid.Length >= 2 && cid[0] == '<' && cid[cid.Length - 1] == '>')
                {
                    return cid.Substring(1, cid.Length - 2);
                }
                return cid;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _part.ContentID = null;
                }
                else
                {
                    if (value.IndexOfAny(s_contentCIDInvalidChars) != -1)
                    {
                        throw new ArgumentException(SR.MailHeaderInvalidCID, nameof(value));
                    }

                    _part.ContentID = "<" + value + ">";
                }
            }
        }

        public ContentType ContentType
        {
            get
            {
                return _part.ContentType;
            }

            set
            {
                _part.ContentType = value;
            }
        }

        public TransferEncoding TransferEncoding
        {
            get
            {
                return _part.TransferEncoding;
            }
            set
            {
                _part.TransferEncoding = value;
            }
        }

        internal Uri ContentLocation
        {
            get
            {
                Uri uri;
                if (!Uri.TryCreate(_part.ContentLocation, UriKind.RelativeOrAbsolute, out uri))
                {
                    return null;
                }
                return uri;
            }

            set
            {
                _part.ContentLocation = value == null ? null : value.IsAbsoluteUri ? value.AbsoluteUri : value.OriginalString;
            }
        }

        internal MimePart MimePart
        {
            get
            {
                return _part;
            }
        }
    }

    public class Attachment : AttachmentBase
    {
        private string _name;
        private Encoding _nameEncoding;

        internal Attachment()
        {
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(string fileName) : base(fileName)
        {
            Name = Path.GetFileName(fileName);
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(string fileName, string mediaType) :
            base(fileName, mediaType)
        {
            Name = Path.GetFileName(fileName);
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(string fileName, ContentType contentType) :
            base(fileName, contentType)
        {
            if (contentType.Name == null || contentType.Name == string.Empty)
            {
                Name = Path.GetFileName(fileName);
            }
            else
            {
                Name = contentType.Name;
            }
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(Stream contentStream, string name) :
            base(contentStream, null, null)
        {
            Name = name;
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(Stream contentStream, string name, string mediaType) :
            base(contentStream, null, mediaType)
        {
            Name = name;
            MimePart.ContentDisposition = new ContentDisposition();
        }

        public Attachment(Stream contentStream, ContentType contentType) :
            base(contentStream, contentType)
        {
            Name = contentType.Name;
            MimePart.ContentDisposition = new ContentDisposition();
        }

        internal void SetContentTypeName(bool allowUnicode)
        {
            if (!allowUnicode && _name != null && _name.Length != 0 && !MimeBasePart.IsAscii(_name, false))
            {
                Encoding encoding = NameEncoding;
                if (encoding == null)
                {
                    encoding = Encoding.GetEncoding(MimeBasePart.DefaultCharSet);
                }
                MimePart.ContentType.Name = MimeBasePart.EncodeHeaderValue(_name, encoding, MimeBasePart.ShouldUseBase64Encoding(encoding));
            }
            else
            {
                MimePart.ContentType.Name = _name;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Encoding nameEncoding = MimeBasePart.DecodeEncoding(value);
                if (nameEncoding != null)
                {
                    _nameEncoding = nameEncoding;
                    _name = MimeBasePart.DecodeHeaderValue(value);
                    MimePart.ContentType.Name = value;
                }
                else
                {
                    _name = value;
                    SetContentTypeName(true);
                    // This keeps ContentType.Name up to date for user viewability, but isn't necessary.
                    // SetContentTypeName is called again by PrepareForSending()
                }
            }
        }


        public Encoding NameEncoding
        {
            get
            {
                return _nameEncoding;
            }
            set
            {
                _nameEncoding = value;
                if (_name != null && _name != string.Empty)
                {
                    SetContentTypeName(true);
                }
            }
        }

        public ContentDisposition ContentDisposition
        {
            get
            {
                return MimePart.ContentDisposition;
            }
        }

        internal override void PrepareForSending(bool allowUnicode)
        {
            if (_name != null && _name != string.Empty)
            {
                SetContentTypeName(allowUnicode);
            }
            base.PrepareForSending(allowUnicode);
        }

        public static Attachment CreateAttachmentFromString(string content, string name)
        {
            Attachment a = new Attachment();
            a.SetContentFromString(content, null, string.Empty);
            a.Name = name;
            return a;
        }

        public static Attachment CreateAttachmentFromString(string content, string name, Encoding contentEncoding, string mediaType)
        {
            Attachment a = new Attachment();
            a.SetContentFromString(content, contentEncoding, mediaType);
            a.Name = name;
            return a;
        }

        public static Attachment CreateAttachmentFromString(string content, ContentType contentType)
        {
            Attachment a = new Attachment();
            a.SetContentFromString(content, contentType);
            a.Name = contentType.Name;
            return a;
        }
    }
}
