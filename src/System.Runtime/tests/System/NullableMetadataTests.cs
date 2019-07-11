
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Runtime.Tests
{
    public class NullableMetadataTests
    {
        private const string NullableAttributeFullName = "System.Diagnostics.CompilerServices.NullableAttribute";
        private const string NullableContextAttributeFullName = "System.Diagnostics.CompilerServices.NullableContextAttribute";
        private const string NullablePublicOnlyAttributeFullName = "System.Diagnostics.CompilerServices.NullablePublicOnlyAttribute";

        [Fact]
        public static void NullableAttributesOnPublicApiOnly()
        {
            MemberInfo[] internalMembers = typeof(string).GetMembers(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (MemberInfo internalMember in internalMembers)
            {
                Assert.Empty(internalMember.GetCustomAttributes().Where(a => NullableAttributesFilter(a)));
            }
        }

        [Fact]
        public static void ShimsHaveOnlyTypeForwards()
        {
            Assembly assembly = Assembly.Load("mscorlib");

            Assert.Empty(assembly.GetTypes());
            Assert.NotEmpty(assembly.GetForwardedTypes());
        }

        [Fact]
        public static void ShimsDontHaveNullablePublicOnlyAttribute()
        {
            Assembly assembly = Assembly.Load("mscorlib");
            Module module = assembly.Modules.First();
            Assert.Empty(module.CustomAttributes.Where(a => a.GetType().FullName.Equals(NullablePublicOnlyAttributeFullName)));
        }

        private static bool NullableAttributesFilter(Attribute attribute)
        {
            return attribute.GetType().FullName.Equals(NullableAttributeFullName) || attribute.GetType().FullName.Equals(NullableContextAttributeFullName);
        }
    }
}
