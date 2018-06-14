// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public class AssociatedMetadataTypeTypeDescriptionProviderTests
    {
        [Fact]
        public static void Constructor_NullType_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("type", () => new AssociatedMetadataTypeTypeDescriptionProvider(null));
        }

        [Fact]
        public static void Constructor_NullAssociatedMetadataType_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("associatedMetadataType", () => new AssociatedMetadataTypeTypeDescriptionProvider(typeof(string), null));
        }
    }
}
