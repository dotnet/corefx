// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Decoding;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class SignatureDecoderTests
    {
        [Fact]
        public unsafe void VerifyMultipleOptionalModifiers()
        {
            // Type 1: int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) modopt([mscorlib]System.Runtime.CompilerServices.CallConvCdecl)
            // Type 2: char* 
            // Type 3: uint32 
            // Type 4: char modopt([mscorlib]System.Runtime.CompilerServices.IsConst)*
            var testSignature = new byte[] { 0x20, 0x45, 0x20, 0x69, 0x08, 0x0F, 0x03, 0x09, 0x0F, 0x20, 0x55, 0x03 };
            var types = new string[]
            {
                "11 1A Int32",  // This has multiple modifiers, which exercises different code paths than single modifiers
                "Char*",
                "UInt32",
                "15 Char*"
            };

            fixed (byte* testSignaturePtr = &testSignature[0])
            {
                var signatureBlob = new BlobReader(testSignaturePtr, testSignature.Length);
                var sigTypeProvider = new TestSignatureTypeProvider();

                foreach (string typeString in types)
                {
                    // Verify that each type is decoded as expected
                    Assert.Equal(typeString, SignatureDecoder.DecodeType(ref signatureBlob, sigTypeProvider));
                }
                // And that nothing is left over to decode
                Assert.Throws<BadImageFormatException>(() => SignatureDecoder.DecodeType(ref signatureBlob, sigTypeProvider));
            }
        }

        [Fact]
        public unsafe void DecodeValidMethodSpecificationSignature()
        {
            // See Ecma 335 Section II.23.2.15 MethodSpec
            var testSignature = new byte[] 
            {
                0x0A, // IMAGE_CEE_CS_CALLCONV_GENERICINST
                2, // type parameter count
                (byte)SignatureTypeCode.Int32,
                (byte)SignatureTypeCode.String,
            };

            fixed (byte* testSignaturePtr = &testSignature[0])
            {
                var signatureBlob = new BlobReader(testSignaturePtr, testSignature.Length);
                IEnumerable<string> expectedTypes = new[] { "Int32", "String" };
                IEnumerable<string> actualTypes = SignatureDecoder.DecodeMethodSpecificationSignature(ref signatureBlob, new TestSignatureTypeProvider());
                Assert.Equal(expectedTypes, actualTypes);
            }
        }

        [Fact]
        public unsafe void DecodeInvalidMethodSpecificationSignature()
        {
            var testSignature = new byte[] { 0x00 };

            fixed (byte* testSignaturePtr = &testSignature[0])
            {
                var signatureBlob = new BlobReader(testSignaturePtr, testSignature.Length);
                Assert.Throws<BadImageFormatException>(() => SignatureDecoder.DecodeMethodSpecificationSignature(ref signatureBlob, new TestSignatureTypeProvider()));
            }
        }

        // Simple test implementation of ISignatureTypeProvider to check that the right types are decoded
        private class TestSignatureTypeProvider : ISignatureTypeProvider<string>
        {
            public string GetModifiedType(string unmodifiedType, ImmutableArray<CustomModifier<string>> customModifiers)
            {
                return string.Join(" ", customModifiers.Select(c => c.Type)) + " " + unmodifiedType;
            }

            public string GetPrimitiveType(PrimitiveTypeCode typeCode) { return typeCode.ToString(); }
            public string GetTypeFromDefinition(TypeDefinitionHandle handle, bool? isValueType) { return handle.RowId.ToString("X"); }
            public string GetTypeFromReference(TypeReferenceHandle handle, bool? isValueType) { return handle.RowId.ToString("X"); }
            public string GetByReferenceType(string elemenstring) { return elemenstring + "&"; }
            public string GetSZArrayType(string elemenstring) { return elemenstring + "[]"; }
            public string GetPointerType(string elemenstring) { return elemenstring + "*"; }

            public MetadataReader Reader { get { return null; } } // Not needed, currently
            public string GetFunctionPointerType(MethodSignature<string> signature) { return null; } // Not needed, currently
            public string GetGenericMethodParameter(int index) { return null; } // Not needed, currently
            public string GetGenericTypeParameter(int index) { return null; } // Not needed, currently
            public string GetGenericInstance(string genericType, ImmutableArray<string> typeArguments) { return null; } // Not needed, currently
            public string GetArrayType(string elemenstring, ArrayShape shape) { return null; } // Not needed, currently
            public string GetPinnedType(string elemenstring) { return null; } // Not needed, currently
        }
    }
}
