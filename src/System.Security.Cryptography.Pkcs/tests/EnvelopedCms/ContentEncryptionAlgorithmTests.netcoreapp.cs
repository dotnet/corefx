// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using Xunit;

using Test.Cryptography;
using System.Security.Cryptography.Pkcs.Tests;

namespace System.Security.Cryptography.Pkcs.EnvelopedCmsTests.Tests
{
    public static partial class ContentEncryptionAlgorithmTests
    {
        static partial void CheckParameters(AlgorithmIdentifier identifier, int? length)
        {
            if (length.HasValue)
            {
                Assert.NotNull(identifier.Parameters);
                Assert.Equal(length.Value, identifier.Parameters.Length);
            }
            else
            {
                Assert.Null(identifier.Parameters);
            }
        }     
    }
}
