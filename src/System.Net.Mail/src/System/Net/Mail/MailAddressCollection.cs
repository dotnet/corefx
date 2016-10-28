// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
    public class MailAddressCollection : Collection<MailAddress>
    {
        public MailAddressCollection()
        {
        }

        public void Add(string addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }
            if (addresses == string.Empty)
            {
                throw new ArgumentException(SR.Format(SR.net_emptystringcall, nameof(addresses)), nameof(addresses));
            }

            ParseValue(addresses);
        }

        protected override void SetItem(int index, MailAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            base.SetItem(index, item);
        }

        protected override void InsertItem(int index, MailAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            base.InsertItem(index, item);
        }

        internal void ParseValue(string addresses)
        {
            IList<MailAddress> result = MailAddressParser.ParseMultipleAddresses(addresses);

            for (int i = 0; i < result.Count; i++)
            {
                Add(result[i]);
            }
        }

        public override string ToString()
        {
            bool first = true;
            StringBuilder builder = new StringBuilder();

            foreach (MailAddress address in this)
            {
                if (!first)
                {
                    builder.Append(", ");
                }

                builder.Append(address.ToString());
                first = false;
            }

            return builder.ToString();
        }

        internal string Encode(int charsConsumed, bool allowUnicode)
        {
            string encodedAddresses = string.Empty;

            //encode each address individually (except the first), fold and separate with a comma
            foreach (MailAddress address in this)
            {
                if (string.IsNullOrEmpty(encodedAddresses))
                {
                    //no need to append a comma to the first one because it may be the only one.
                    encodedAddresses = address.Encode(charsConsumed, allowUnicode);
                }
                else
                {
                    //appending another one, append a comma to separate and then fold and add the encoded address
                    //the charsConsumed will be 1 because only the first line needs to account for the header itself for 
                    //line length; subsequent lines have a single whitespace character because they are folded here
                    encodedAddresses += ", " + address.Encode(1, allowUnicode);
                }
            }
            return encodedAddresses;
        }
    }
}
