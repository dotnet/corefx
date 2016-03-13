// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System
{
    //
    // The class is used as a base for custom uri parsing and derived Uri factoring.
    // A set of protected .ctors allows to hookup on the builtin parser behaviors.
    //
    // A developer must implement at least internal default .ctor to participate in the Uri extensibility game.
    //
    internal abstract partial class UriParser
    {
        internal string SchemeName
        {
            get
            {
                return _scheme;
            }
        }
        internal int DefaultPort
        {
            get
            {
                return _port;
            }
        }

        private const UriSyntaxFlags SchemeOnlyFlags = UriSyntaxFlags.MayHavePath;
        // This is a "scheme-only" base parser, everything after the scheme is
        // returned as the path component.
        // The user parser will need to do the majority of the work itself.
        //
        // However when the ctor is called from OnCreateUri context the calling parser
        // settings will later override the result on the base class
        //
        protected UriParser() : this(SchemeOnlyFlags) { }

        //
        // Is called on each Uri ctor for every non-simple parser i.e. the one that does have
        // user code.
        //
        protected virtual UriParser OnNewUri()
        {
            return this;
        }

        //
        // Is called whenever a parser gets registered with some scheme
        // The base implementaion is a nop.
        //
        protected virtual void OnRegister(string schemeName, int defaultPort)
        {
        }

        //
        // Parses and validates a Uri object, is called at the Uri ctor time.
        //
        // This method returns a non null parsingError if Uri being created is invalid:
        //
        protected virtual void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
        {
            parsingError = uri.ParseMinimal();
        }

        //
        // Resolves a relative Uri object into new AbsoluteUri.
        //
        //  baseUri         - The baseUri used to resolve this Uri.
        //  relativeuri     - A relative Uri string passed by the application.
        //
        // This method returns:
        // The result Uri value used to represent a new Uri
        //
        protected virtual string Resolve(Uri baseUri, Uri relativeUri, out UriFormatException parsingError)
        {
            if (baseUri.UserDrivenParsing)
                throw new InvalidOperationException(SR.Format(SR.net_uri_UserDrivenParsing, this.GetType().ToString()));

            if (!baseUri.IsAbsoluteUri)
                throw new InvalidOperationException(SR.net_uri_NotAbsolute);


            string newUriString = null;
            bool userEscaped = false;
            Uri result = Uri.ResolveHelper(baseUri, relativeUri, ref newUriString, ref userEscaped, out parsingError);

            if (parsingError != null)
                return null;

            if (result != null)
                return result.OriginalString;

            return newUriString;
        }

        protected virtual bool IsBaseOf(Uri baseUri, Uri relativeUri)
        {
            return baseUri.IsBaseOfHelper(relativeUri);
        }

        //
        // This method is invoked to allow a cutsom parser to override the
        // internal parser when serving application with Uri componenet strings.
        // The output format depends on the "format" parameter
        //
        // Parameters:
        //  uriComponents   - Which components are to be retrieved.
        //  uriFormat       - The requested output format.
        //
        // This method returns:
        // The final result. The base impementaion could be invoked to get a suggested value
        //
        protected virtual string GetComponents(Uri uri, UriComponents components, UriFormat format)
        {
            if (((components & UriComponents.SerializationInfoString) != 0) && components != UriComponents.SerializationInfoString)
                throw new ArgumentOutOfRangeException(nameof(components), components, SR.net_uri_NotJustSerialization);

            if ((format & ~UriFormat.SafeUnescaped) != 0)
                throw new ArgumentOutOfRangeException(nameof(format));

            if (uri.UserDrivenParsing)
                throw new InvalidOperationException(SR.Format(SR.net_uri_UserDrivenParsing, this.GetType().ToString()));

            if (!uri.IsAbsoluteUri)
                throw new InvalidOperationException(SR.net_uri_NotAbsolute);

            return uri.GetComponentsHelper(components, format);
        }

        protected virtual bool IsWellFormedOriginalString(Uri uri)
        {
            return uri.InternalIsWellFormedOriginalString();
        }

        //
        // Is a Uri scheme known to System.Uri?
        //
        public static bool IsKnownScheme(string schemeName)
        {
            if (schemeName == null)
                throw new ArgumentNullException(nameof(schemeName));

            if (!Uri.CheckSchemeName(schemeName))
                throw new ArgumentOutOfRangeException(nameof(schemeName));

            UriParser syntax = UriParser.GetSyntax(schemeName.ToLowerInvariant());
            return syntax != null && syntax.NotAny(UriSyntaxFlags.V1_UnknownUri);
        }
    }
}
