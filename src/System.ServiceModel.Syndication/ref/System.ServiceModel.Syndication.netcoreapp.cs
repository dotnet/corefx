// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.ServiceModel.Syndication
{
    public partial class SyndicationFeed
    {
        public SyndicationLink Documentation { get{ throw null; } set{ } }
        public System.Collections.Generic.ICollection<string> SkipDays { get { throw null; } set { } }
        public System.Collections.Generic.ICollection<int> SkipHours { get { throw null; } set { } }
        public System.ServiceModel.Syndication.SyndicationTextInput TextInput { get { throw null; } set { } }
        public int TimeToLive { get { throw null; } set { } }
    }
    public abstract partial class SyndicationFeedFormatter
    {
        public System.Func<string, string, string, System.DateTimeOffset> DateTimeParser { get { throw null; } set { } }
        public System.Func<string, System.UriKind, string, string, System.Uri> UriParser { get { throw null; } set { } }
    }
    public partial class SyndicationTextInput
    {
        public string Description;
        public System.ServiceModel.Syndication.SyndicationLink Link;
        public string Name;
        public string Title;
        public SyndicationTextInput() { }
    }
}
