// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal enum RegexParseError
    {
        TooManyAlternates,
        IllegalCondition,
        IncompleteSlashP,
        MalformedSlashP,
        UnrecognizedEscape,
        UnrecognizedControl,
        MissingControl,
        TooFewHex,
        CaptureGroupOutOfRange,
        UndefinedNameRef,
        UndefinedBackref,
        MalformedNameRef,
        IllegalEndEscape,
        UnterminatedComment,
        UnrecognizedGrouping,
        AlternationCantCapture,
        AlternationCantHaveComment,
        MalformedReference,
        UndefinedReference,
        InvalidGroupName,
        CapnumNotZero,
        UnterminatedBracket,
        SubtractionMustBeLast,
        ReversedCharRange,
        BadClassInCharRange,
        NotEnoughParens,
        IllegalRange,
        InternalError,
        NestedQuantify,
        QuantifyAfterNothing,
        TooManyParens,
        UnknownProperty // Unicode block, \p{Property}
    }

    internal class RegexParseException : ArgumentException
    {
        public RegexParseError Error { get; }

        public RegexParseException(RegexParseError error, string message) : base(message)
        {
            Error = error;
        }

        public RegexParseException() : base()
        {
        }

        public RegexParseException(string message) : base(message)
        {
        }

        public RegexParseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
