
using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Tests.RealWorld
{
    public abstract class RealWorldTest
    {
        public abstract void Initialize();

        public void Verify()
        {
            VerifySerializer();
            VerifyDocument();
            VerifyReader();
            VerifyWriter();
        }

        public string PayloadAsString = "";

        public byte[] PayloadAsUtf8Bytes => Encoding.UTF8.GetBytes(PayloadAsString);

        public abstract void VerifySerializer();

        public abstract void VerifyDocument();

        public abstract void VerifyReader();

        public abstract void VerifyWriter();
    }

    public static class RealWorldTests
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] { new BlogPost.Test() };
                //yield return new object[] { new Profile.Test() };
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public static void TestRealWorldUseCases(RealWorldTest test)
        {
            test.Initialize();
            test.Verify();
        }
    }
}
