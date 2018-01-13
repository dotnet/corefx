// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Globalization
{
    public class CultureNotFoundException : ArgumentException
    {
        private string _invalidCultureName; // unrecognized culture name
        private int? _invalidCultureId;     // unrecognized culture Lcid

        public CultureNotFoundException()
            : base(DefaultMessage)
        {
        }

        public CultureNotFoundException(String message)
            : base(message)
        {
        }

        public CultureNotFoundException(String paramName, String message)
            : base(message, paramName)
        {
        }

        public CultureNotFoundException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CultureNotFoundException(String paramName, string invalidCultureName, String message)
            : base(message, paramName)
        {
            _invalidCultureName = invalidCultureName;
        }

        public CultureNotFoundException(String message, string invalidCultureName, Exception innerException)
            : base(message, innerException)
        {
            _invalidCultureName = invalidCultureName;
        }

        public CultureNotFoundException(string message, int invalidCultureId, Exception innerException)
            : base(message, innerException)
        {
            _invalidCultureId = invalidCultureId;
        }

        public CultureNotFoundException(string paramName, int invalidCultureId, string message)
            : base(message, paramName)
        {
            _invalidCultureId = invalidCultureId;
        }

        protected CultureNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public virtual Nullable<int> InvalidCultureId
        {
            get { return _invalidCultureId; }
        }

        public virtual string InvalidCultureName
        {
            get { return _invalidCultureName; }
        }

        private static String DefaultMessage
        {
            get
            {
                return SR.Argument_CultureNotSupported;
            }
        }

        private String FormatedInvalidCultureId
        {
            get
            {
                return InvalidCultureId != null ?
                    String.Format(CultureInfo.InvariantCulture, "{0} (0x{0:x4})", (int)InvalidCultureId) :
                    InvalidCultureName;
            }
        }

        public override String Message
        {
            get
            {
                String s = base.Message;
                if (_invalidCultureId != null || _invalidCultureName != null)
                {
                    String valueMessage = SR.Format(SR.Argument_CultureInvalidIdentifier, FormatedInvalidCultureId);
                    if (s == null)
                    {
                        return valueMessage;
                    }

                    return s + Environment.NewLine + valueMessage;
                }
                return s;
            }
        }
    }
}
