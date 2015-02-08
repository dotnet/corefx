// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Xunit;

namespace System.Reflection.Metadata.Tests
{
    public class HandleTests
    {
        [Fact]
        public void HandleConversionGivesCorrectKind()
        {
            var expectedKinds = new SortedSet<HandleKind>((HandleKind[])Enum.GetValues(typeof(HandleKind)));

            Action<Handle, HandleKind> assert = (handle, expectedKind) =>
            {
                Assert.Equal(expectedKind, handle.Kind);
                expectedKinds.Remove(expectedKind);
            };

            assert(default(ModuleDefinitionHandle), HandleKind.ModuleDefinition);
            assert(default(AssemblyDefinitionHandle), HandleKind.AssemblyDefinition);
            assert(default(InterfaceImplementationHandle), HandleKind.InterfaceImplementation);
            assert(default(MethodDefinitionHandle), HandleKind.MethodDefinition);
            assert(default(MethodSpecificationHandle), HandleKind.MethodSpecification);
            assert(default(TypeDefinitionHandle), HandleKind.TypeDefinition);
            assert(default(ExportedTypeHandle), HandleKind.ExportedType);
            assert(default(TypeReferenceHandle), HandleKind.TypeReference);
            assert(default(TypeSpecificationHandle), HandleKind.TypeSpecification);
            assert(default(MemberReferenceHandle), HandleKind.MemberReference);
            assert(default(FieldDefinitionHandle), HandleKind.FieldDefinition);
            assert(default(EventDefinitionHandle), HandleKind.EventDefinition);
            assert(default(PropertyDefinitionHandle), HandleKind.PropertyDefinition);
            assert(default(StandaloneSignatureHandle), HandleKind.StandaloneSignature);
            assert(default(MemberReferenceHandle), HandleKind.MemberReference);
            assert(default(FieldDefinitionHandle), HandleKind.FieldDefinition);
            assert(default(EventDefinitionHandle), HandleKind.EventDefinition);
            assert(default(PropertyDefinitionHandle), HandleKind.PropertyDefinition);
            assert(default(StandaloneSignatureHandle), HandleKind.StandaloneSignature);
            assert(default(ParameterHandle), HandleKind.Parameter);
            assert(default(GenericParameterHandle), HandleKind.GenericParameter);
            assert(default(GenericParameterConstraintHandle), HandleKind.GenericParameterConstraint);
            assert(default(ModuleReferenceHandle), HandleKind.ModuleReference);
            assert(default(CustomAttributeHandle), HandleKind.CustomAttribute);
            assert(default(DeclarativeSecurityAttributeHandle), HandleKind.DeclarativeSecurityAttribute);
            assert(default(ManifestResourceHandle), HandleKind.ManifestResource);
            assert(default(ConstantHandle), HandleKind.Constant);
            assert(default(ManifestResourceHandle), HandleKind.ManifestResource);
            assert(default(AssemblyFileHandle), HandleKind.AssemblyFile);
            assert(default(MethodImplementationHandle), HandleKind.MethodImplementation);
            assert(default(AssemblyFileHandle), HandleKind.AssemblyFile);
            assert(default(StringHandle), HandleKind.String);
            assert(default(AssemblyReferenceHandle), HandleKind.AssemblyReference);
            assert(default(UserStringHandle), HandleKind.UserString);
            assert(default(GuidHandle), HandleKind.Guid);
            assert(default(BlobHandle), HandleKind.Blob);
            assert(default(NamespaceDefinitionHandle), HandleKind.NamespaceDefinition);

            Assert.True(expectedKinds.Count == 0, "Some handles are missing from this test: " + String.Join(",\r\n", expectedKinds));
        }

        [Fact]
        public void HandleKindHidesSpecialStringAndNamespaces()
        {
            foreach (uint virtualBit in new[] { 0U, TokenTypeIds.VirtualTokenMask })
            {
                uint invalidStringTypeCount = 0;
                uint invalidNamespaceTypeCount = 0;

                for (uint i = 0; i <= sbyte.MaxValue; i++)
                {
                    Handle handle = new Handle(virtualBit | i << TokenTypeIds.RowIdBitCount);
                    Assert.True(handle.IsNil ^ handle.IsVirtual);
                    Assert.Equal(virtualBit != 0, handle.IsVirtual);
                    Assert.Equal(handle.TokenType, i << TokenTypeIds.RowIdBitCount);

                    switch (i)
                    {
                        // String and namespace have two extra bits to represent their kind that are hidden from the handle type
                        case (uint)HandleKind.String:
                        case (uint)HandleKind.String + 1:
                        case (uint)HandleKind.String + 2:
                        case (uint)HandleKind.String + 3:
                            Assert.Equal(HandleKind.String, handle.Kind);

                            StringKind stringType = (StringKind)((i - (int)HandleKind.String) << TokenTypeIds.RowIdBitCount);
                            StringHandle stringHandle;
                            try
                            {
                                stringHandle = (StringHandle)handle;
                            }
                            catch (InvalidCastException)
                            {
                                Assert.True(handle.TokenType > TokenTypeIds.MaxString);
                                invalidStringTypeCount++;
                                break;
                            }
                            Assert.Equal(stringType, stringHandle.StringKind);
                            break;

                        case (uint)HandleKind.NamespaceDefinition:
                        case (uint)HandleKind.NamespaceDefinition + 1:
                        case (uint)HandleKind.NamespaceDefinition + 2:
                        case (uint)HandleKind.NamespaceDefinition + 3:
                            Assert.Equal(HandleKind.NamespaceDefinition, handle.Kind);

                            NamespaceKind namespaceType = (NamespaceKind)((i - (int)HandleKind.NamespaceDefinition) << TokenTypeIds.RowIdBitCount);
                            NamespaceDefinitionHandle namespaceHandle;
                            try
                            {
                                namespaceHandle = (NamespaceDefinitionHandle)handle;
                            }
                            catch (InvalidCastException)
                            {
                                Assert.True(handle.TokenType > TokenTypeIds.MaxNamespace);
                                invalidNamespaceTypeCount++;
                                break;
                            }
                            Assert.Equal(namespaceType, namespaceHandle.NamespaceKind);
                            break;

                        // all other types surface token type directly.
                        default:
                            Assert.Equal((uint)handle.Kind, i);
                            break;
                    }
                }
                Assert.Equal((uint)((TokenTypeIds.String | TokenTypeIds.StringOrNamespaceKindMask) - TokenTypeIds.MaxString) >> TokenTypeIds.RowIdBitCount, invalidStringTypeCount);
                Assert.Equal((uint)((TokenTypeIds.Namespace | TokenTypeIds.StringOrNamespaceKindMask) - TokenTypeIds.MaxNamespace) >> TokenTypeIds.RowIdBitCount, invalidNamespaceTypeCount);
            }
        }
    }
}
