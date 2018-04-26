// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Reflection.Context
{
    public class CustomReflectionContextTests
    {
        [Fact]
        public void CustomContext()
        {
            var customReflectionContext = new MyCRC();
            Type type = typeof(string);

            //A representation of the type in the custom reflection context.
            TypeInfo customTypeInfo = customReflectionContext.MapType(type.GetTypeInfo());
            
            //The "ToString" member as represented in the custom reflection context.
            MemberInfo customMemberInfo = customTypeInfo.GetDeclaredMethods("ToString").First();
            
            IEnumerable<Attribute> results = customMemberInfo.GetCustomAttributes();
            Assert.Single(results);
            Assert.IsType<CustomAttribute>(results.First());
        }

        internal class CustomAttribute : Attribute
        {
        }

        internal class MyCRC : CustomReflectionContext
        {
            protected override IEnumerable<object> GetCustomAttributes(MemberInfo member, IEnumerable<object> declaredAttributes)
            {
                if (member.Name.StartsWith("To"))
                {
                    yield return new CustomAttribute();
                }
            }
        }
    }
}
