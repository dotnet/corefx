// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Xunit;

namespace System.Reflection.Metadata.Decoding
{
    public partial class TypeNameParserTests
    {
        [Fact]
        public void Parse_NullAsTypeName_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => TypeNameParser.Parse((string)null, new StringBasedTypeProvider()));
        }

        [Fact]
        public void Parse_EmptyAsTypeName_ThrowsArgument()
        {
            Assert.Throws<ArgumentException>(() => TypeNameParser.Parse("", new StringBasedTypeProvider()));
        }

        [Fact]
        public void Parse_NullAsTypeProvider_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => TypeNameParser.Parse("T", (ITypeNameParserTypeProvider<string>)null));
        }

        [Fact]
        public void IdExpected_EncountedOnlyWhiteSpace()
        {
            ParseInvalidType(" ",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("  ",                                                                                                   2, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T, ",                                                                                                  3, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T+ ",                                                                                                  3, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T,  ",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[], ",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[],  ",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T, mscorlib, ",                                                                                        13, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing ', '
            ParseInvalidType("T, mscorlib,  ",                                                                                       14, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing ',  '
            ParseInvalidType("T, mscorlib, Version= ",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '= '
            ParseInvalidType("T, mscorlib, Version=  ",                                                                              23, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '=  '
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture= ",                                                              39, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '= '
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture=  ",                                                             40, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '=  '
            ParseInvalidType("T[], mscorlib, ",                                                                                      15, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing ', '
            ParseInvalidType("T[], mscorlib,  ",                                                                                     16, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing ',  '
            ParseInvalidType("T[], mscorlib, Version= ",                                                                             24, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '= '
            ParseInvalidType("T[], mscorlib, Version=  ",                                                                            25, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '=  '
            ParseInvalidType("T[], mscorlib, Version=1.0.0.0, Culture= ",                                                            41, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '= '
            ParseInvalidType("T[], mscorlib, Version=1.0.0.0, Culture=  ",                                                           42, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace, reflectionBug:true);       // BUG: Reflection ignores trailing '=  '
            ParseInvalidType("T[A, ",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[A,  ",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[A, B, ",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[A, B,  ",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[ ",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[  ",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, ",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A,  ",                                                                                              7, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib, ",                                                                                     16, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib,  ",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib]], ",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib]],  ",                                                                                  19, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib], ",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib],  ",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib], [ ",                                                                                  19, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib], [  ",                                                                                 20, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib, Version= ",                                                                            25, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib, Version=  ",                                                                           26, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture= ",                                                           42, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture=  ",                                                          43, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, ",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B,  ",                                                                                           10, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, ",                                                                                  19, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib,  ",                                                                                 20, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib]], ",                                                                                21, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib]],  ",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib], [ ",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib], [  ",                                                                              23, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version= ",                                                                         28, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=  ",                                                                        29, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0], ",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],  ",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0], [ ",                                                              39, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0], [  ",                                                             40, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture= ",                                                        45, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=  ",                                                       46, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral], ",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],  ",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral], [ ",                                             56, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral], [  ",                                            57, TypeNameFormatErrorId.IdExpected_EncounteredOnlyWhiteSpace);
        }

        [Fact]
        public void IdExpected_EncountedEndOfString()
        {
            ParseInvalidType("T,",                                                                                                  2, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[],",                                                                                                4, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T, \"",                                                                                               4, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T, mscorlib,",                                                                                        12, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing ','
            ParseInvalidType("T, mscorlib, Version=",                                                                               21, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing '='
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture=",                                                              38, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing '='
            ParseInvalidType("T[], mscorlib,",                                                                                      14, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing ','
            ParseInvalidType("T[], mscorlib, Version=",                                                                             23, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing '='
            ParseInvalidType("T[], mscorlib, Version=1.0.0.0, Culture=",                                                            40, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString, reflectionBug:true);   // BUG: Reflection ignores trailing '='
            ParseInvalidType("T[A,",                                                                                                4, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[A, B,",                                                                                             7, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[",                                                                                                 3, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A,",                                                                                               5, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, \"",                                                                                            7, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib,",                                                                                     15, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib]],",                                                                                   17, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib],",                                                                                    16, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib], [",                                                                                  18, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib, Version=",                                                                            24, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture=",                                                           41, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B,",                                                                                            8, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib,",                                                                                  18, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib]],",                                                                                20, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib], [",                                                                               21, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=",                                                                         27, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],",                                                                36, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0], [",                                                              38, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=",                                                        44, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],",                                               53, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral], [",                                             55, TypeNameFormatErrorId.IdExpected_EncounteredEndOfString);
        }

        [Fact]
        public void IdExpected_EncounteredDelimiter()
        {
            ParseInvalidType("+",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType(",",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("*",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("&",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("[",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("]",                                                                                                    1, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T,=",                                                                                                  3, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T,,",                                                                                                  3, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T,\"\"",                                                                                               4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[],=",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[],,",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[],\"\"",                                                                                             6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T, mscorlib,=",                                                                                        13, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,=
            ParseInvalidType("T, mscorlib,,",                                                                                        13, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,,
            ParseInvalidType("T, mscorlib,\"\"",                                                                                     14, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing quotes
            ParseInvalidType("T, mscorlib, Version==",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing =
            ParseInvalidType("T, mscorlib, Version=,",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture==",                                                              39, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing =
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture=,",                                                              39, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,
            ParseInvalidType("T[], mscorlib,=",                                                                                      15, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing =
            ParseInvalidType("T[], mscorlib,,",                                                                                      15, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,
            ParseInvalidType("T[], mscorlib,\"\"",                                                                                   16, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing quotes
            ParseInvalidType("T[], mscorlib, Version==",                                                                             24, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing =
            ParseInvalidType("T[], mscorlib, Version=,",                                                                             24, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,
            ParseInvalidType("T[], mscorlib, Version=1.0.0.0, Culture==",                                                            41, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing =
            ParseInvalidType("T[], mscorlib, Version=1.0.0.0, Culture=,",                                                            41, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter, reflectionBug:true);  // BUG: Reflection ignores the trailing ,
            ParseInvalidType("T[A,+",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,,",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,*",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,&",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,]",                                                                                                5, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[+",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[,",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[*",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[&",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[]",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A,[[",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,+",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,,",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,*",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,&",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,]",                                                                                             8, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[+",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[,",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[*",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[&",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[[",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[A, B,[]",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[+",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[,",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[*",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[&",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[[",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[]",                                                                                                 4, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A,=",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A,,",                                                                                               6, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A,\"\"",                                                                                            7, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib,=",                                                                                     16, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib,,",                                                                                     16, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib,\"\"",                                                                                  17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib]],=",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib]],,",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib]],\"\"",                                                                                19, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],+",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],,",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],*",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],&",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],]",                                                                                    17, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[+",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[,",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[*",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[&",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[[",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib],[]",                                                                                   18, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib, Version==",                                                                            25, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib, Version=,",                                                                            25, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture==",                                                           42, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture=,",                                                           42, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B,=",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B,,",                                                                                            9, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B,\"\"",                                                                                         10, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib,=",                                                                                  19, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib,,",                                                                                  19, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib,\"\"",                                                                               20, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib]],=",                                                                                21, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib]],,",                                                                                21, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib]],\"\"",                                                                             22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], [+",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], [,",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], [*",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], [&",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], [[",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib], []",                                                                               22, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version==",                                                                         28, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=,",                                                                         28, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],+",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],,",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],*",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],&",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],]",                                                                37, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[+",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[,",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[&",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[*",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[[",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0],[]",                                                               38, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture==",                                                        45, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=,",                                                        45, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],+",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],,",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],*",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],&",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],]",                                               54, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[+",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[,",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[*",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[&",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[[",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral],[]",                                              55, TypeNameFormatErrorId.IdExpected_EncounteredDelimiter);
        }

        [Fact]
        public void EscapedDelimiterExpected()
        {
            ParseInvalidType(@"\A",                                                                                                  2, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"\ ",                                                                                                  2, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"\\\ ",                                                                                                4, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"\\\ A",                                                                                               4, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"\=",                                                                                                  2, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType("\\\"",                                                                                                 2, TypeNameFormatErrorId.EscapedDelimiterExpected, reflectionBug:true);                        // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"T, mscorlib\A",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\ ",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\+",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\+",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\&",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\*",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\[",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\]",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T, mscorlib\]",                                                                                       13, TypeNameFormatErrorId.EscapedDelimiterExpected);
            ParseInvalidType(@"T[[A, mscorlib\[]]",                                                                                  16, TypeNameFormatErrorId.EscapedDelimiterExpected);
        }

        [Fact]
        public void EscapedLiteralExpected_EncounteredEndOfString()
        {
            ParseInvalidType(@"\",                                                                                                   1, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString, reflectionBug:true);  // BUG: Reflection treats this as an empty type
            ParseInvalidType(@" \",                                                                                                  2, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"  \",                                                                                                 3, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"\\\",                                                                                                 3, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString, reflectionBug:true);  // BUG: Reflection treats this as an empty type
            ParseInvalidType(@"T\",                                                                                                  2, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString, reflectionBug:true);  // BUG: Reflection ignores trailing '\';
            ParseInvalidType(@"T, mscorlib\",                                                                                        12, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"T[\",                                                                                                 3, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"T[[\",                                                                                                4, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"T[[A,\",                                                                                              6, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
            ParseInvalidType(@"T[[A, mscorlib\",                                                                                     15, TypeNameFormatErrorId.EscapedDelimiterExpected_EncounteredEndOfString);
        }

         [Fact]
        public void EndOfStringExpected_EncounteredExtraCharacters()
        {
            ParseInvalidType("T&A",                                                                                                 3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T&[]",                                                                                                3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T&[,]",                                                                                               3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T&[A]",                                                                                               3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T&[[A, mscorlib]]",                                                                                   3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T*A",                                                                                                 3, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType(@"T*\\",                                                                                               4, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T[]A",                                                                                                4, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T,mscorlib\"",                                                                                        11, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T,mscorlib=",                                                                                         11, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType("T]",                                                                                                  2, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType(@"T\[]",                                                                                               4, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
            ParseInvalidType(@"T\, mscorlib, Culture=",                                                                             22, TypeNameFormatErrorId.EndOfStringExpected_EncounteredExtraCharacters);
        }

         [Fact]
        public void DelimiterExpected_EncounteredEndOfString()
        {
            ParseInvalidType("T, \"mscorlib",                                                                                       12, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString, reflectionBug:true);    // BUG: Reflection ignores the missing quote
            ParseInvalidType("T, mscorlib, Version",                                                                                20, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString, reflectionBug:true);    // BUG: Reflection ignores the trailing version
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture",                                                               37, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString, reflectionBug:true);    // BUG: Reflection ignores the trailing version
            ParseInvalidType("T[A",                                                                                                 3, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[A, B",                                                                                              6, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[",                                                                                                  2, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[, ",                                                                                                4, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A",                                                                                                4, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib",                                                                                      14, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, \"mscorlib",                                                                                    15, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib]",                                                                                     15, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib], [A]",                                                                                20, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib, Version",                                                                             23, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A, mscorlib, Version=1.0.0.0, Culture",                                                            40, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B",                                                                                              7, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib",                                                                                   17, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib]]",                                                                                 19, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib], [A",                                                                              22, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version",                                                                          26, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0",                                                                  34, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0], [A",                                                             39, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture",                                                         43, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral",                                                 51, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
            ParseInvalidType("T[[A[[B, mscorlib, Version=1.0.0.0, Culture=neutral], [A",                                            56, TypeNameFormatErrorId.DelimiterExpected_EncounteredEndOfString);
        }

        [Fact]
        public void DelimiterExpected()
        {
            ParseInvalidType("T, mscorlib, Version,",                                                                               21, TypeNameFormatErrorId.DelimiterExpected, reflectionBug:true);   // BUG: Reflection ignores the trailing Version + ,
            ParseInvalidType("T, mscorlib, Version=1.0.0.0, Culture,",                                                              38, TypeNameFormatErrorId.DelimiterExpected, reflectionBug:true);   // BUG: Reflection ignores the trailing Culture + ,
            ParseInvalidType("T[, [",                                                                                               5, TypeNameFormatErrorId.DelimiterExpected);
            ParseInvalidType("T[[A[,[",                                                                                             7, TypeNameFormatErrorId.DelimiterExpected);            
            ParseInvalidType("T[][A]",                                                                                              5, TypeNameFormatErrorId.DelimiterExpected);
            ParseInvalidType("T[,][A]",                                                                                             6, TypeNameFormatErrorId.DelimiterExpected);
            ParseInvalidType("T[,][[A, mscorlib]]",                                                                                 6, TypeNameFormatErrorId.DelimiterExpected);
        }

        [Fact]
        public void DuplicateAssemblyComponent()
        {
            ParseInvalidType("T, mscorlib, version=1.0.0.0, version=1.0.0.0",                                                       45, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, version=1.0.0.0, Version=2.0.0.0",                                                       45, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, version=1.0.0.0, Version=1.0.0.0",                                                       45, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, version=1.0.0.0, VERSION=1.0.0.0",                                                       45, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, \"version\"=1.0.0.0, VERSION=1.0.0.0",                                                   47, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, version=1.0.0.0, PublicKeyToken=ABC, Version=1.0.0.0",                                   65, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate version
            ParseInvalidType("T, mscorlib, Culture=\"\", culture=en-US",                                                            38, TypeNameFormatErrorId.DuplicateAssemblyComponent, reflectionBug:true);  // BUG: Reflection ignores the duplicate culture
        }

        [Fact]
        public void AssemblyNames()
        {
            ParseType("T, mscorlib",                                                                                                "|mscorlib(name)|T(simple)");
            ParseType("T,\"mscorlib\"",                                                                                             "|mscorlib(name)|T(simple)");
            ParseType("T,\" mscorlib\"",                                                                                            "| mscorlib(name)|T(simple)");
            ParseType("T,\" mscorlib \"",                                                                                           "| mscorlib (name)|T(simple)");
            ParseType("T, mscorlib, Version=1.0.0.0",                                                                               "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, \"Version\"=1.0.0.0",                                                                           "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, \"Ver sion\"=1.0.0.0",                                                                          "|mscorlib(name)Ver sion(componentName)1.0.0.0(componentValue)|T(simple)",                      "|mscorlib(name)|T(simple)");       // Reflection throws away unknown recognized components
            ParseType("T, mscorlib, Version=\"1.0.0.0\"",                                                                           "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, Culture=en-US",                                                                                 "|mscorlib(name)Culture(componentName)en-US(componentValue)|T(simple)");
            ParseType("T, mscorlib, Culture=neutral",                                                                               "|mscorlib(name)Culture(componentName)neutral(componentValue)|T(simple)");
            ParseType("T, mscorlib, PublicKeyToken=null",                                                                           "|mscorlib(name)PublicKeyToken(componentName)null(componentValue)|T(simple)");
            ParseType("T, mscorlib, PublicKeyToken=\"\"",                                                                           "|mscorlib(name)PublicKeyToken(componentName)(componentValue)|T(simple)",                       "|mscorlib(name)PublicKeyToken(componentName)null(componentValue)|T(simple)");
            ParseType("T, mscorlib, PublicKeyToken=\" \"",                                                                          "|mscorlib(name)PublicKeyToken(componentName) (componentValue)|T(simple)",                      "|mscorlib(name)|T(simple)");       // BUG: Reflection throws away the PublicKeyToken in this case
            ParseType("T, mscorlib, Version=1.0.0.0, Culture=en-US",                                                                "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)Culture(componentName)en-US(componentValue)|T(simple)");
            ParseType("T, mscorlib, Version=1.0.0.0, PublicKeyToken=b03f5f7f11d50a3a, Culture=en-US",                               "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)PublicKeyToken(componentName)b03f5f7f11d50a3a(componentValue)Culture(componentName)en-US(componentValue)|T(simple)");
            ParseType("T, mscorlib, Version=1.0.0.0, PublicKeyToken=b03f5f7f11d50a3a, Culture=en-US, Retargetable=Yes",             "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)PublicKeyToken(componentName)b03f5f7f11d50a3a(componentValue)Culture(componentName)en-US(componentValue)Retargetable(componentName)Yes(componentValue)|T(simple)");
        }

        [Fact]
        public void EscapeSequence()
        {
            ParseType(@"\\",                                                                                                        @"\(simple)");
            ParseType(@"T\\",                                                                                                       @"T\(simple)");
            ParseType(@"T\+B",                                                                                                      "T+B(simple)");
            ParseType(@"T\[",                                                                                                       "T[(simple)");
            ParseType(@"T\]",                                                                                                       "T](simple)");
            ParseType(@"T\[\]",                                                                                                     "T[](simple)");
            ParseType(@"T[\*]",                                                                                                     "T(simple)<*(simple)>");
            ParseType(@"T\&",                                                                                                       "T&(simple)");
            ParseType(@"T\*",                                                                                                       "T*(simple)");
            ParseType(@"T\,",                                                                                                       "T,(simple)");
            ParseType(@"Dog\\Cat",                                                                                                  @"Dog\Cat(simple)");
            ParseType(@"T, mscor\,lib",                                                                                             "|mscor,lib(name)|T(simple)");
            ParseType(@"T, mscorlib\,",                                                                                             "|mscorlib,(name)|T(simple)");
            ParseType(@"T\, mscorlib",                                                                                              "T, mscorlib(simple)");
            ParseType(@"T\, mscorlib\, Culture=",                                                                                   "T, mscorlib, Culture=(simple)");
            ParseType(@"T, mscorlib\, Culture\=",                                                                                   "|mscorlib, Culture=(name)|T(simple)");
            ParseType("T, \\\"mscorlib",                                                                                            "|\"mscorlib(name)|T(simple)");
            ParseType("T, mscorlib, Custom=\\\"",                                                                                   "|mscorlib(name)Custom(componentName)\"(componentValue)|T(simple)",           "|mscorlib(name)|T(simple)");
            ParseType(@"T[\,]",                                                                                                     "T(simple)<,(simple)>");
            ParseType(@"T[A\,]",                                                                                                    "T(simple)<A,(simple)>");
            ParseType(@"T[[A\,, mscorlib]]",                                                                                        "T(simple)<|mscorlib(name)|A,(simple)>");
            ParseType(@"T[[A, mscorlib\]]]",                                                                                        "T(simple)<|mscorlib](name)|A(simple)>");
            ParseType(@"T[[A, mscorlib\]]]",                                                                                        "T(simple)<|mscorlib](name)|A(simple)>");
            ParseType(@"T[[A, mscor\,lib]]",                                                                                        "T(simple)<|mscor,lib(name)|A(simple)>");
        }

        [Fact]
        public void SignificantWhiteSpace()
        {   
            ParseType("T ",                                                                                                         "T (simple)");
            ParseType("T  ",                                                                                                        "T  (simple)");
            ParseType("T &",                                                                                                        "T (simple)(reference)");
            ParseType("T *",                                                                                                        "T (simple)(pointer)");
            ParseType("T []",                                                                                                       "T (simple){}");
            ParseType("T  []",                                                                                                       "T  (simple){}");
            ParseType("T [*]",                                                                                                      "T (simple){1}");
            ParseType("T [,]",                                                                                                      "T (simple){2}");
            ParseType("T [A]",                                                                                                      "T (simple)<A(simple)>");
            ParseType("T [A ]",                                                                                                     "T (simple)<A (simple)>");
            ParseType("T [[A , mscorlib]]",                                                                                         "T (simple)<|mscorlib(name)|A (simple)>");
            ParseType("T`1 ",                                                                                                       "T`1 (simple)");
            ParseType("Int32 ",                                                                                                     "Int32 (simple)");
            ParseType("System Foo Int32 ",                                                                                          "System Foo Int32 (simple)");
            ParseType("Int32`1 ",                                                                                                   "Int32`1 (simple)");
            ParseType("System. Int32",                                                                                              "System. Int32(simple)");
            ParseType("System.Int32`1",                                                                                             "System.Int32`1(simple)");
            ParseType("System. Windows.Forms.Control",                                                                              "System. Windows.Forms.Control(simple)");
            ParseType("System. Collections.List`1",                                                                                 "System. Collections.List`1(simple)");
            ParseType("T, msc orlib",                                                                                               "|msc orlib(name)|T(simple)");
            ParseType("T, mscorlib, Ver sion=1.0.0.0",                                                                              "|mscorlib(name)Ver sion(componentName)1.0.0.0(componentValue)|T(simple)",      "|mscorlib(name)|T(simple)");                                                                           // Reflection throws away unrecognized elements
            ParseType("T, mscorlib, Version=1 .0.0.0",                                                                              "|mscorlib(name)Version(componentName)1 .0.0.0(componentValue)|T(simple)",      "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");                              // BUG: Reflection ignores spaces within the version component
            ParseType("T, mscorlib, Custom=en -US",                                                                                 "|mscorlib(name)Custom(componentName)en -US(componentValue)|T(simple)",          "|mscorlib(name)|T(simple)");                                                                          // Reflection throws away unrecognized elements
            ParseType("T[[A, msc orlib]]",                                                                                          "T(simple)<|msc orlib(name)|A(simple)>");
            ParseType("T[[A, mscorlib, Ver sion=1.0.0.0]]",                                                                         "T(simple)<|mscorlib(name)Ver sion(componentName)1.0.0.0(componentValue)|A(simple)>",      "T(simple)<|mscorlib(name)|A(simple)>");                                                     // Reflection throws away unrecognized elements
            ParseType("T[[A, mscorlib, Version=1 .0.0.0]]",                                                                         "T(simple)<|mscorlib(name)Version(componentName)1 .0.0.0(componentValue)|A(simple)>",      "T(simple)<|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|A(simple)>");        // BUG: Reflection ignores spaces within the version component
            ParseType("T[[A, mscorlib, Custom=en -US]]",                                                                            "T(simple)<|mscorlib(name)Custom(componentName)en -US(componentValue)|A(simple)>",         "T(simple)<|mscorlib(name)|A(simple)>");                                                     // Reflection throws away unrecognized elements
        }

        [Fact]
        public void InsignificantWhiteSpace()
        {
            ParseType(" T",                                                                                                         "T(simple)");
            ParseType("   T",                                                                                                       "T(simple)");
            ParseType("    T",                                                                                                      "T(simple)");
            ParseType("T* ",                                                                                                        "T(simple)(pointer)");
            ParseType("T& ",                                                                                                        "T(simple)(reference)");
            ParseType("T* *",                                                                                                       "T(simple)(pointer)(pointer)");
            ParseType("T* &",                                                                                                       "T(simple)(pointer)(reference)");
            ParseType("T[ ]",                                                                                                       "T(simple){}");
            ParseType("T[  * ]",                                                                                                    "T(simple){1}");
            ParseType("T[  * ] []",                                                                                                 "T(simple){1}{}");
            ParseType("T[ [A, mscorlib] ] []",                                                                                      "T(simple)<|mscorlib(name)|A(simple)>{}");
            ParseType("T[ , ]",                                                                                                     "T(simple){2}");
            ParseType("T[ , , ]",                                                                                                   "T(simple){3}");
            ParseType("T[ , , ][]",                                                                                                 "T(simple){3}{}");
            ParseType("T[ A]",                                                                                                      "T(simple)<A(simple)>");
            ParseType("T[  A]",                                                                                                     "T(simple)<A(simple)>");
            ParseType("T[   A]",                                                                                                    "T(simple)<A(simple)>");
            ParseType("T[   A][]",                                                                                                  "T(simple)<A(simple)>{}");
            ParseType("T[A] []",                                                                                                    "T(simple)<A(simple)>{}");
            ParseType("T[[A, mscorlib ]]",                                                                                          "T(simple)<|mscorlib(name)|A(simple)>");
            ParseType("T, mscorlib",                                                                                                "|mscorlib(name)|T(simple)");
            ParseType("T,  mscorlib",                                                                                               "|mscorlib(name)|T(simple)");
            ParseType("T,   mscorlib",                                                                                              "|mscorlib(name)|T(simple)");
            ParseType("T, mscorlib ",                                                                                               "|mscorlib(name)|T(simple)");
            ParseType("T, mscorlib  ",                                                                                              "|mscorlib(name)|T(simple)");
            ParseType("T, mscorlib   ",                                                                                             "|mscorlib(name)|T(simple)");
            ParseType("T, mscorlib   , Version=1.0.0.0",                                                                            "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, Version=1.0.0.0",                                                                               "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib,  Version=1.0.0.0",                                                                              "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, Version= 1.0.0.0",                                                                              "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
            ParseType("T, mscorlib, Version=1.0.0.0 ",                                                                              "|mscorlib(name)Version(componentName)1.0.0.0(componentValue)|T(simple)");
        }

        [Fact]
        public void SimpleTypeName()
        {
            ParseType("T",                                                                                                          "T(simple)");
            ParseType("\"T\"",                                                                                                      "\"T\"(simple)");
            ParseType("\"T",                                                                                                        "\"T(simple)");
            ParseType("T\"",                                                                                                        "T\"(simple)");
            ParseType("T`1",                                                                                                        "T`1(simple)");
            ParseType("Int32",                                                                                                      "Int32(simple)");
            ParseType("Int32`1",                                                                                                    "Int32`1(simple)");
            ParseType("System.Int32",                                                                                               "System.Int32(simple)");
            ParseType("System.Int32`1",                                                                                             "System.Int32`1(simple)");
            ParseType("System.Windows.Forms.Control",                                                                               "System.Windows.Forms.Control(simple)");
            ParseType("System.Collections.List`1",                                                                                  "System.Collections.List`1(simple)");
        }

        [Fact]
        public void NestedTypeName()
        {
            ParseType("A+B",                                                                                                        "A(simple)-B");
            ParseType("A`1+B",                                                                                                      "A`1(simple)-B");
            ParseType("A+B`1",                                                                                                      "A(simple)-B`1");
            ParseType("A+B`1",                                                                                                      "A(simple)-B`1");
            ParseType("A+B+C",                                                                                                      "A(simple)-B-C");
            ParseType("A+B+C+D+E+F+G+H+I+J+K+L+M+N+O+P+Q+R+S+T+U+V+W+X+Y",                                                          "A(simple)-B-C-D-E-F-G-H-I-J-K-L-M-N-O-P-Q-R-S-T-U-V-W-X-Y");
            ParseType("System.Collections.List`1+Enumerator",                                                                       "System.Collections.List`1(simple)-Enumerator");
            ParseType("System.Collections.List+Enumerator`1+Nested",                                                                "System.Collections.List(simple)-Enumerator`1-Nested");
        }

        [Fact]
        public void GenericTypeArgumentFullName()
        {
            ParseType("T[T1]",                                                                                                      "T(simple)<T1(simple)>");
            ParseType("T[T1,T2]",                                                                                                   "T(simple)<T1(simple),T2(simple)>");
            ParseType("T[T1,T2,T3]",                                                                                                "T(simple)<T1(simple),T2(simple),T3(simple)>");
            ParseType("T[T1,T2,T3,T4]",                                                                                             "T(simple)<T1(simple),T2(simple),T3(simple),T4(simple)>");
            ParseType("T+N[T1]",                                                                                                    "T(simple)-N<T1(simple)>");
            ParseType("T+N[T1+T3,T2]",                                                                                              "T(simple)-N<T1(simple)-T3,T2(simple)>");
            ParseType("T[T1,T2+T4,T3]",                                                                                             "T(simple)<T1(simple),T2(simple)-T4,T3(simple)>");
            ParseType("T[T1,T2,T3,T4]",                                                                                             "T(simple)<T1(simple),T2(simple),T3(simple),T4(simple)>");
            ParseType("Type[T1,T2,T3,T4]",                                                                                          "Type(simple)<T1(simple),T2(simple),T3(simple),T4(simple)>");
            ParseType("Type[T1[T2]]",                                                                                               "Type(simple)<T1(simple)<T2(simple)>>");
            ParseType("Type[T1[T2],T2]",                                                                                            "Type(simple)<T1(simple)<T2(simple)>,T2(simple)>");
            ParseType("Type[T1[T2[T3]]]",                                                                                           "Type(simple)<T1(simple)<T2(simple)<T3(simple)>>>");
            ParseType("Type[T1[T2[T3],T3]]",                                                                                        "Type(simple)<T1(simple)<T2(simple)<T3(simple)>,T3(simple)>>");
            ParseType("Type[T[T[T],T,T]]",                                                                                          "Type(simple)<T(simple)<T(simple)<T(simple)>,T(simple),T(simple)>>");
            ParseType("Type[T[T[T+N],T,T]]",                                                                                        "Type(simple)<T(simple)<T(simple)<T(simple)-N>,T(simple),T(simple)>>");
        }

        [Fact]
        public void ByReference()
        {
            ParseType("T&",                                                                                                         "T(simple)(reference)");
            ParseType("T+N&",                                                                                                       "T(simple)-N(reference)");
        }

        [Fact]
        public void Pointer()
        {
            ParseType("T*",                                                                                                         "T(simple)(pointer)");
            ParseType("T+N*",                                                                                                       "T(simple)-N(pointer)");
            ParseType("T+N**",                                                                                                      "T(simple)-N(pointer)(pointer)");
        }

        [Fact]
        public void Arrays()
        {
            ParseType("T[]",                                                                                                        "T(simple){}");
            ParseType("T[][]",                                                                                                      "T(simple){}{}");
            ParseType("T[][][]",                                                                                                    "T(simple){}{}{}");
            ParseType("T[*]",                                                                                                       "T(simple){1}");
            ParseType("T[*][*]",                                                                                                    "T(simple){1}{1}");
            ParseType("T[*][*][*]",                                                                                                 "T(simple){1}{1}{1}");
            ParseType("T[,]",                                                                                                       "T(simple){2}");
            ParseType("T[,][,,]",                                                                                                   "T(simple){2}{3}");
            ParseType("T[,][,,][,,,]",                                                                                              "T(simple){2}{3}{4}");
            ParseType("T[][*][,]",                                                                                                  "T(simple){}{1}{2}");
            ParseType("T[*][][,]",                                                                                                  "T(simple){1}{}{2}");
            ParseType("T[*][][,][]",                                                                                                "T(simple){1}{}{2}{}");
        }


        private void ParseType(string typeName, string expected, string reflectionExpected = null)
        {
            string actual = TypeNameParser.Parse<string>(typeName, new StringBasedTypeProvider());

            Assert.Equal(expected, actual);

            ParseTypeWithDesktopReflection(typeName, reflectionExpected ?? expected);
        }

        private void ParseInvalidType(string typeName, int position, TypeNameFormatErrorId errorId, bool reflectionBug = false)
        {
            var ex = Assert.Throws<TypeNameFormatException>(() => TypeNameParser.Parse(typeName, new StringBasedTypeProvider()));
            Assert.Equal(position, ex.Position);
            Assert.Equal(errorId, ex.ErrorId);

            if (!reflectionBug)
            {
                ParseInvalidTypeWithDesktopReflection(typeName);
            }
        }

        partial void ParseTypeWithDesktopReflection(string typeName, string expected);
        partial void ParseInvalidTypeWithDesktopReflection(string typeName);
    }
}
