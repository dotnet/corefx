// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    // Parses an assembly-qualified or full type name.
    // 
    // The type name grammer below is written in a custom form of BNF (Backus-Naur Form), the key is as follows:
    //
    //      Symbol:     <name>
    //      Optional:   [<name>]
    //      Literal:    ","
    //      Sequence:   (0...65535)
    //      Or:         <pointer>
    //                  <array>
    //
    //  <format> ::=
    //      <assemblyQualifiedName>
    //      <assemblyQualifiedNameWithinGenericTypeArgument>
    //      <fullName>
    //
    //  <assemblyQualifiedName> ::=
    //      <fullName> "," <assemblyName>
    //
    //  <assemblyQualifiedNameWithinGenericTypeArgument> ::=
    //      <fullName> "," <assemblyNameWithinGenericTypeArgument>
    //
    //  <fullName> ::= 
    //      <declaringTypeName>[<nestedTypeNames>][<genericTypeArguments>][<pointerOrArray>][<byReference>]
    // 
    //  <assemblyName> ::=
    //      (see AssemblyNameParser)
    //
    //  <assemblyNameWithinGenericTypeArgument> ::=
    //      (see AssemblyNameParser)
    //
    //  <declaringTypeName> ::=
    //      <simpleTypeName>
    //
    //  <nestedTypeNames> ::=
    //      [<nestedTypeNames>] "+" <nestedTypeName>
    //
    //  <nestedTypeName> ::=
    //      <simpleTypeName>
    //
    //  <simpleTypeName> ::=
    //      [<whitespace>] <identifier>
    //
    //  <genericTypeArguments> ::=
    //      "[" <genericTypeArgumentsList> "]"
    //
    //  <genericTypeArgumentsList> ::=
    //      [<genericTypeArgumentsList> ","] <genericTypeArgument>
    //
    //  <genericTypeArgument> ::=
    //      <genericTypeArgumentFullName>
    //      <genericTypeArgumentAssemblyQualifiedName>
    // 
    //  <genericTypeArgumentFullName> ::=
    //      <fullName>
    //
    //  <genericTypeArgumentAssemblyQualifiedName> ::=
    //      "[" <assemblyQualifiedNameWithinGenericTypeArgument> "]"
    //
    //  <pointerOrArray> ::=
    //      [<pointerOrArray>]<pointer>
    //      [<pointerOrArray>]<array>
    //
    //  <byReference> ::= 
    //      "&"
    //
    //  <pointer> ::= 
    //      "*"
    //
    //  <array> ::=
    //      <szArray>
    //      <singleDimensionalArray>
    //      <multiDimensionalArray> 
    //
    //  <szArray> ::=
    //      "[]"
    //
    //  <singleDimensionalArray> ::=
    //      "[*]"
    //
    //  <multiDimensionalArray> ::=
    //      "[" <arrayDimensionSeparator> "]"
    //
    //  <arrayDimensionSeparator> ::= 
    //      [<arrayDimensionSeparator>] ","
    //
    //  <identifier> ::=
    //      [<identifier>]<identifierChar>
    //      [<identifier>]<escapedChar>
    //
    //  <identifierChar> ::=
    //      any unicode character except <delimiter>
    //
    //  <escapedChar> ::=
    //      "\" <delimiter>
    //
    //  <whitespace> ::=
    //      [<whitespace>] " "
    //
    //  <delimiter> ::=
    //      "*"
    //      "["
    //      "]"
    //      "," 
    //      "\"
    //      "&"
    //      "+"
    //
    internal partial class TypeNameParser<TType>
    {
        private readonly TypeBuilder _builder = new TypeBuilder();
        private readonly ITypeNameParserTypeProvider<TType> _typeProvider;
        private Tokenizer _tokenizer;
        private readonly ParseMode _mode;

        private TypeNameParser(ITypeNameParserTypeProvider<TType> typeProvider, string typeName, int startIndex, ParseMode mode)
        {
            Debug.Assert(typeProvider != null);

            _typeProvider = typeProvider;
            _tokenizer = new Tokenizer(typeName, startIndex, Delimiters.TypeName);
            _mode = mode;
        }

        public static TType ParseType(string typeName, ITypeNameParserTypeProvider<TType> typeProvider)
        {
            TypeNameParser<TType> parser = new TypeNameParser<TType>(typeProvider, typeName, 0, ParseMode.AssemblyQualifiedName);
            TType type = parser.Parse();

            // Make sure there's no trailing chars
            parser.Close();

            return type;
        }

        public int Position
        {
            get { return _tokenizer.Position; }
        }
        
        private void Close()
        {
            _tokenizer.Close();
        }

        // <format>
        private TType Parse()
        {
            ParseDeclaringTypeFullName();

            ParseNestedTypeNamesIfAny();

            ParseGenericTypeArgumentsIfAny();

            ParsePointerOrArrayIfAny();

            ParseByReferenceIfAny();

            ParseAssemblyNameIfAny();

            return _builder.BuildType(_typeProvider);
        }

        // 'System', 'System.Int32'
        // 
        // <declaringTypeName>
        private void ParseDeclaringTypeFullName()
        {
            _builder.DeclaringTypeFullName = ParseSimpleTypeName();
        }

        // '+System', '+System+Foo', '+SystemFoo+Bar'
        //
        // <nestedTypeNames>
        private void ParseNestedTypeNamesIfAny()
        {
            // Read the nested type names
            while (_tokenizer.SkipIf(TokenType.Plus))
            {
                ParseNestedTypeName();
            }
        }

        // <nestedTypeName>
        private void ParseNestedTypeName()
        {
            _builder.AddNestedTypeName(ParseSimpleTypeName());
        }

        // <simpleTypeName>
        private string ParseSimpleTypeName()
        {
            // The parsed type name does not contain leading spaces to remain consistent with Reflection
            return _tokenizer.ReadId(IdentifierOptions.TrimStart | IdentifierOptions.Required);
        }

        // Parses '[T]', '[T1, T2]', and '[[T1, AssemblyName], T2]'  
        //
        // <genericTypeArguments>
        private void ParseGenericTypeArgumentsIfAny()
        {
            if (!HasGenericTypeArguments())
                return;

            // Consume '['
            _tokenizer.Skip(TokenType.LeftBracket);

            ParseGenericTypeArgumentsList();

            // Consume ']'
            _tokenizer.Skip(TokenType.RightBracket);
        }

        // Parses 'T', 'T1, T2', and '[T1, AssemblyName]'
        // 
        // <genericTypeArgumentList>
        private void ParseGenericTypeArgumentsList()
        {
            do
            {
                ParseGenericTypeArgument();
            } while (_tokenizer.SkipIf(TokenType.Comma));
        }

        // We need to sniff to determine if a left bracket
        // is a generic argument list, or an array. 
        // []  [,]  [*] are arrays
        // [T] [T1, T2] [[T1, AssemblyName], T2] are generic arguments
        private bool HasGenericTypeArguments()
        {
            if (_tokenizer.Peek() != TokenType.LeftBracket)
                return false;

            // NOTE: Peek skips white space, so a space between brackets,
            // such as in 'System.Int32[ ]', will not trip us up
            TokenType token = _tokenizer.PeekNext();
            return (token != TokenType.EndOfInput &&
                    token != TokenType.RightBracket &&
                    token != TokenType.Comma &&
                    token != TokenType.Star);
        }

        // Parses 'T', 'System.Int32', and '[System.Int32, mscorlib]'
        //
        // <genericTypeArgument>
        private void ParseGenericTypeArgument()
        {
            if (TryParseGenericTypeArgumentAssemblyQualifiedName())
                return;

            ParseGenericTypeArgumentFullName();
        }


        // Parses '[System.Int32, mscorlib]' in 'System.Func`1[[System.Int32, mscorlib]]'
        // 
        // <genericTypeArgumentAssemblyQualifiedName>
        private bool TryParseGenericTypeArgumentAssemblyQualifiedName()
        {
            if (!_tokenizer.SkipIf(TokenType.LeftBracket))
                return false;

            ParseGenericTypeArgumentName(ParseMode.AssemblyQualifiedNameWithinGenericTypeArgument);

            // Consume the extra ']'
            _tokenizer.Skip(TokenType.RightBracket);

            return true;
        }

        // Parses 'System.Int32' in 'System.Func`1[System.Int32]'
        //
        // <genericTypeArgumentFullName>
        private void ParseGenericTypeArgumentFullName()
        {
            ParseGenericTypeArgumentName(ParseMode.FullName);
        }

        private void ParseGenericTypeArgumentName(ParseMode mode)
        {
            TypeNameParser<TType> parser = new TypeNameParser<TType>(_typeProvider, _tokenizer.Input, _tokenizer.Position, mode);

            TType type = parser.Parse();
            _tokenizer.Position = parser.Position;

            _builder.AddGenericTypeArgument(type);
        }

        // Parses '*', '&', '[]', or '[,]'
        //
        // <pointerOrArray>
        private void ParsePointerOrArrayIfAny()
        {
            // Loop through to catch multiple modifiers. 
            while (TryParsePointerIfAny() || TryParseArrayIfAny())
            {
            }
        }

        // Parses '*' in 'System.Int32*'
        //
        // <pointer>
        private bool TryParsePointerIfAny()
        {
            if (!_tokenizer.SkipIf(TokenType.Star))
                return false;

            _builder.AddTypeModifier(TypeModifier.Pointer);
            return true;
        }

        // Parses '[]' or '[,]' in 'System.Int32[]' and 'System.Int32[,]'
        //
        // <array>
        private bool TryParseArrayIfAny()
        {
            if (_tokenizer.SkipIf(TokenType.LeftBracket))
            {
                ParseArray();

                _tokenizer.Skip(TokenType.RightBracket);
                return true;
            }

            return false;
        }

        private void ParseArray()
        {
            if (TryParseSingleDimensionalArray())
                return;

            ParseSZArrayOrMultiDimensionalArray();
        }

        // Parses '*' in 'System.Int32[*]
        //
        // <singleDimensionalArray>
        private bool TryParseSingleDimensionalArray()
        {
            if (_tokenizer.SkipIf(TokenType.Star))
            {
                _builder.AddTypeModifier(TypeModifier.Array(1));
                return true;
            }

            return false;
        }

        // Parses '' or ',' in 'System.Int32[]' and 'System.Int32[,]
        //
        // <szArray> or <multiDimensionalArray>
        private void ParseSZArrayOrMultiDimensionalArray()
        {
            // Make note that Reflection treats arrays created with Type.MakeArrayType(1)
            // differently from SZArrays created via Type.MakeArrayType(), so do not be
            // tempted to generalized this into a multi-dimensional array with a rank of 1.
            int rank = ParseArrayRank();
            if (rank == 1)
            {
                // System.Int32[]
                _builder.AddTypeModifier(TypeModifier.SZArray);
            }
            else
            {
                // System.Int32[,]
                _builder.AddTypeModifier(TypeModifier.Array(rank));
            }
        }

        // <arrayDimensionSeparator>
        private int ParseArrayRank()
        {
            int rank = 1;

            while (_tokenizer.SkipIf(TokenType.Comma))
            {
                rank++;
            }

            return rank;
        }

        // Parses '&' in 'System.Int32&'
        //
        // <byReference>
        private void ParseByReferenceIfAny()
        {
            if (!_tokenizer.SkipIf(TokenType.Ampersand))
                return;

            _builder.AddTypeModifier(TypeModifier.Reference);
        }

        // Parses ', mscorlib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
        //
        // <assemblyName> and <assemblyNameWithinGenericTypeArgument>
        private void ParseAssemblyNameIfAny()
        {
            if (_mode == ParseMode.FullName)
                return;

            if (!_tokenizer.SkipIf(TokenType.Comma))
                return;

            int position = _tokenizer.Position;
            _builder.AssemblyName = AssemblyNameParser.Parse(_tokenizer.Input, ref position, withinGenericTypeArgument: _mode == ParseMode.AssemblyQualifiedNameWithinGenericTypeArgument);
            _tokenizer.Position = position;
        }
    }
}
