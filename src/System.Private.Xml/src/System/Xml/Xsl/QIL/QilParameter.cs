// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Xml.Xsl.Qil
{
    /// <summary>
    /// View over a Qil parameter node.
    /// </summary>
    internal class QilParameter : QilIterator
    {
        private QilNode _name;

        //-----------------------------------------------
        // Constructor
        //-----------------------------------------------

        /// <summary>
        /// Construct a parameter
        /// </summary>
        public QilParameter(QilNodeType nodeType, QilNode defaultValue, QilNode name, XmlQueryType xmlType) : base(nodeType, defaultValue)
        {
            _name = name;
            this.xmlType = xmlType;
        }


        //-----------------------------------------------
        // IList<QilNode> methods -- override
        //-----------------------------------------------

        public override int Count
        {
            get { return 2; }
        }

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Binding;
                    case 1: return _name;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Binding = value; break;
                    case 1: _name = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }


        //-----------------------------------------------
        // QilParameter methods
        //-----------------------------------------------

        /// <summary>
        /// Default value expression of this parameter (may be null).
        /// </summary>
        public QilNode DefaultValue
        {
            get { return Binding; }
            set { Binding = value; }
        }

        /// <summary>
        /// Name of this parameter (may be null).
        /// </summary>
        public QilName Name
        {
            get { return (QilName)_name; }
            set { _name = value; }
        }
    }
}

