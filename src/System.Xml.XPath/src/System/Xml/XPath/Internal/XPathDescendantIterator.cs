﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    internal class XPathDescendantIterator : XPathAxisIterator
    {
        private int _level = 0;

        public XPathDescendantIterator(XPathNavigator nav, XPathNodeType type, bool matchSelf) : base(nav, type, matchSelf) { }
        public XPathDescendantIterator(XPathNavigator nav, string name, string namespaceURI, bool matchSelf) : base(nav, name, namespaceURI, matchSelf) { }

        public XPathDescendantIterator(XPathDescendantIterator it) : base(it)
        {
            this._level = it._level;
        }

        public override XPathNodeIterator Clone()
        {
            return new XPathDescendantIterator(this);
        }

        public override bool MoveNext()
        {
            if (first)
            {
                first = false;
                if (matchSelf && Matches)
                {
                    position = 1;
                    return true;
                }
            }

            while (true)
            {
                if (nav.MoveToFirstChild())
                {
                    _level++;
                }
                else
                {
                    while (true)
                    {
                        if (_level == 0)
                        {
                            return false;
                        }
                        if (nav.MoveToNext())
                        {
                            break;
                        }
                        nav.MoveToParent();
                        _level--;
                    }
                }

                if (Matches)
                {
                    position++;
                    return true;
                }
            }
        }
    }
}
