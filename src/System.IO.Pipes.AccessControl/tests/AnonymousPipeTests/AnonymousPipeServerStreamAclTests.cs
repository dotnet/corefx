using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Xunit;

namespace System.IO.Pipes.Tests
{
    public class AnonymousPipeServerStreamAclTests : PipeServerStreamAclTestBase
    {
        [Fact]
        public void Create_NullSecurity()
        {
            CreateAndVerifyAnonymousPipe(PipeDirection.In, HandleInheritability.None, 0, null).Dispose();
        }

        [Theory]
        [InlineData(PipeDirection.InOut)]
        [InlineData((PipeDirection)(int.MinValue))]
        [InlineData((PipeDirection)0)]
        [InlineData((PipeDirection)4)]
        [InlineData((PipeDirection)(int.MaxValue))]
        public void Create_InvalidPipeDirection(PipeDirection direction)
        {
            if (direction == PipeDirection.InOut)
            {
                Assert.Throws<NotSupportedException>(() =>
                {
                    CreateAndVerifyAnonymousPipe(direction, HandleInheritability.None, 0, GetBasicPipeSecurity()).Dispose();
                });
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    CreateAndVerifyAnonymousPipe(direction, HandleInheritability.None, 0, GetBasicPipeSecurity()).Dispose();
                });
            }
        }

        [Theory]
        [InlineData((HandleInheritability)(int.MinValue))]
        [InlineData((HandleInheritability)(-1))]
        [InlineData((HandleInheritability)2)]
        [InlineData((HandleInheritability)(int.MaxValue))]
        public void Create_InvalidInheritability(HandleInheritability inheritability)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                CreateAndVerifyAnonymousPipe(PipeDirection.In, inheritability, 0, GetBasicPipeSecurity()).Dispose();
            });
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void Create_InvalidBufferSize(int bufferSize)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                CreateAndVerifyAnonymousPipe(PipeDirection.In, HandleInheritability.None, bufferSize, GetBasicPipeSecurity()).Dispose();
            });
        }

        [Theory]
        [InlineData(PipeDirection.In,  HandleInheritability.Inheritable, 0)]
        [InlineData(PipeDirection.In,  HandleInheritability.Inheritable, 1)]
        [InlineData(PipeDirection.In,  HandleInheritability.None, 0)]
        [InlineData(PipeDirection.In,  HandleInheritability.None, 1)]
        [InlineData(PipeDirection.Out, HandleInheritability.Inheritable, 0)]
        [InlineData(PipeDirection.Out, HandleInheritability.Inheritable, 1)]
        [InlineData(PipeDirection.Out, HandleInheritability.None, 0)]
        [InlineData(PipeDirection.Out, HandleInheritability.None, 1)]
        public void Create_ValidParameters(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
        {
            CreateAndVerifyAnonymousPipe(direction, inheritability, bufferSize, GetBasicPipeSecurity()).Dispose();
        }

        public static IEnumerable<object[]> Create_CombineRightsAndAccessControl_MemberData() =>
            from rights in Enum.GetValues(typeof(PipeAccessRights)).Cast<PipeAccessRights>()
            from accessControl in new[] { AccessControlType.Allow, AccessControlType.Deny }
            select new object[] { rights, accessControl };

        // These tests match NetFX behavior
        [Theory]
        [MemberData(nameof(Create_CombineRightsAndAccessControl_MemberData))]
        public void Create_CombineRightsAndAccessControl(PipeAccessRights rights, AccessControlType accessControl)
        {
            if (rights == PipeAccessRights.Synchronize && accessControl == AccessControlType.Deny)
            {
                Assert.Throws<ArgumentException>("accessMask", () =>
                {
                    PipeSecurity security = GetPipeSecurity(WellKnownSidType.BuiltinUsersSid, PipeAccessRights.Synchronize, AccessControlType.Deny);
                });
            }
            else if ((rights == PipeAccessRights.FullControl || rights == PipeAccessRights.ReadWrite) && accessControl == AccessControlType.Allow)
            {
                VerifyValidSecurity(rights, accessControl);
            }
            else
            {
                PipeSecurity security = GetPipeSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);
                Assert.Throws<UnauthorizedAccessException>(() =>
                {
                    AnonymousPipeServerStreamAcl.Create(PipeDirection.In, HandleInheritability.None, 0, security).Dispose();
                });
            }
        }

        [Theory]
        [InlineData(PipeAccessRights.ReadWrite | PipeAccessRights.Synchronize, AccessControlType.Allow)]
        public void Create_ValidBitwiseRightsSecurity(PipeAccessRights rights, AccessControlType accessControl)
        {
            VerifyValidSecurity(rights, accessControl);
        }

        private void VerifyValidSecurity(PipeAccessRights rights, AccessControlType accessControl)
        {
            PipeSecurity security = GetPipeSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);
            CreateAndVerifyAnonymousPipe(PipeDirection.In, HandleInheritability.None, 0, security).Dispose();
        }

        private AnonymousPipeServerStream CreateAndVerifyAnonymousPipe(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity expectedSecurity)
        {
            AnonymousPipeServerStream pipe = AnonymousPipeServerStreamAcl.Create(direction, inheritability, bufferSize, expectedSecurity);
            Assert.NotNull(pipe);

            if (expectedSecurity != null)
            {
                PipeSecurity actualSecurity = pipe.GetAccessControl();
                VerifyPipeSecurity(expectedSecurity, actualSecurity);
            }

            return pipe;
        }

    }
}
