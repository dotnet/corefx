﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
    internal class Axis : AstNode
    {
        private AxisType _axisType;
        private AstNode _input;
        private string _prefix;
        private string _name;
        private XPathNodeType _nodeType;
        protected bool abbrAxis;

        public enum AxisType
        {
            Ancestor,
            AncestorOrSelf,
            Attribute,
            Child,
            Descendant,
            DescendantOrSelf,
            Following,
            FollowingSibling,
            Namespace,
            Parent,
            Preceding,
            PrecedingSibling,
            Self,
            None
        };

        // constructor
        public Axis(AxisType axisType, AstNode input, string prefix, string name, XPathNodeType nodetype)
        {
            Debug.Assert(prefix != null);
            Debug.Assert(name != null);
            this._axisType = axisType;
            this._input = input;
            this._prefix = prefix;
            this._name = name;
            this._nodeType = nodetype;
        }

        // constructor
        public Axis(AxisType axisType, AstNode input)
            : this(axisType, input, string.Empty, string.Empty, XPathNodeType.All)
        {
            this.abbrAxis = true;
        }

        public override AstType Type { get { return AstType.Axis; } }

        public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }

        public AstNode Input
        {
            get { return _input; }
        }

        public string Prefix { get { return _prefix; } }
        public string Name { get { return _name; } }
        public XPathNodeType NodeType { get { return _nodeType; } }
        public AxisType TypeOfAxis { get { return _axisType; } }
        public bool AbbrAxis { get { return abbrAxis; } }
    }
}
