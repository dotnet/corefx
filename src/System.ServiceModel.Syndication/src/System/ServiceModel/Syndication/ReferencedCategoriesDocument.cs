//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Syndication
{
    using System.Xml;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using System.Runtime.CompilerServices;

    public class ReferencedCategoriesDocument : CategoriesDocument
    {
        Uri link;

        public ReferencedCategoriesDocument()
        {
        }

        public ReferencedCategoriesDocument(Uri link)
            : base()
        {
            if (link == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            this.link = link;
        }

        public Uri Link
        {
            get { return this.link; }
            set { this.link = value; }
        }

        internal override bool IsInline
        {
            get { return false; }
        }
    }
}
