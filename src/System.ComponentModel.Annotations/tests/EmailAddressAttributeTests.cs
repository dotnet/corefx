// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public class EmailAddressAttributeTests : ValidationAttributeTestBase
    {
        public override IEnumerable<Test> ValidValues()
        {
            yield return new Test(new EmailAddressAttribute(), null);
            yield return new Test(new EmailAddressAttribute(), "someName@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "1234@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "firstName.lastName@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "\u00A0@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "!#$%&'*+-/=?^_`|~@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "\"firstName.lastName\"@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "someName@someDomain.com");
            yield return new Test(new EmailAddressAttribute(), "someName@some~domain.com");
            yield return new Test(new EmailAddressAttribute(), "someName@some_domain.com");
            yield return new Test(new EmailAddressAttribute(), "someName@1234.com");
            yield return new Test(new EmailAddressAttribute(), "someName@someDomain\uFFEF.com");
        }

        public override IEnumerable<Test> InvalidValues()
        {
            yield return new Tests.Test(new EmailAddressAttribute(), 0);
            yield return new Tests.Test(new EmailAddressAttribute(), "");
            yield return new Tests.Test(new EmailAddressAttribute(), " \r \t \n" );
            yield return new Tests.Test(new EmailAddressAttribute(), "@someDomain.com");
            yield return new Tests.Test(new EmailAddressAttribute(), "@someDomain@abc.com");
            yield return new Tests.Test(new EmailAddressAttribute(), "someName");
            yield return new Tests.Test(new EmailAddressAttribute(), "someName@");
            yield return new Tests.Test(new EmailAddressAttribute(), "someName@a@b.com");
        }

        [Fact]
        public static void DataType_CustomDataType_ReturnExpected()
        {
            var attribute = new EmailAddressAttribute();
            Assert.Equal(DataType.EmailAddress, attribute.DataType);
            Assert.Null(attribute.CustomDataType);
        }
    }
}
