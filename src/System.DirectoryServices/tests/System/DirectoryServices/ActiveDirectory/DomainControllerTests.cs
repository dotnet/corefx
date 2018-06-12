// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.DirectoryServices.ActiveDirectory.Tests
{
    public class DomainControllerTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetDomainController_NullContext_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.GetDomainController(null));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(DirectoryContextType.ApplicationPartition)]
        [InlineData(DirectoryContextType.ConfigurationSet)]
        [InlineData(DirectoryContextType.Domain)]
        [InlineData(DirectoryContextType.Forest)]
        public void GetDomainController_InvalidContextType_ThrowsArgumentException(DirectoryContextType contextType)
        {
            var context = new DirectoryContext(contextType, "name");
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.GetDomainController(context));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [OuterLoop("Takes too long on domain joined machines")]
        [InlineData("\0")]
        [InlineData("[")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Access to path is denied when in App container")]
        public void GetDomainController_InvalidName(string name)
        {
            var context = new DirectoryContext(DirectoryContextType.DirectoryServer, name);
            Exception exception = Record.Exception(() => DomainController.GetDomainController(context));
            Assert.NotNull(exception);
            Assert.True(exception is ActiveDirectoryObjectNotFoundException ||
                        exception is ActiveDirectoryOperationException,
                        $"We got unrecognized exception {exception}");
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Access to path is denied when in App container")]
        public void GetDomainController_InvalidIPV6()
        {
            var context = new DirectoryContext(DirectoryContextType.DirectoryServer, "[::1]:port");
            Exception exception = Record.Exception(() => DomainController.GetDomainController(context));
            Assert.NotNull(exception);
            Assert.True(exception is ActiveDirectoryObjectNotFoundException ||
                        exception is ActiveDirectoryOperationException,
                        $"We got unrecognized exception {exception}");
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindOne_NullContext_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindOne(null));
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindOne(null, "siteName"));
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindOne(null, LocatorOptions.AvoidSelf));
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindOne(null, "siteName", LocatorOptions.AvoidSelf));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(DirectoryContextType.ApplicationPartition)]
        [InlineData(DirectoryContextType.ConfigurationSet)]
        [InlineData(DirectoryContextType.DirectoryServer)]
        [InlineData(DirectoryContextType.Forest)]
        public void FindOne_InvalidContextType_ThrowsArgumentException(DirectoryContextType contextType)
        {
            var context = new DirectoryContext(contextType, "name");
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindOne(context));
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindOne(context, "siteName"));
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindOne(context, LocatorOptions.AvoidSelf));
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindOne(context, "siteName", LocatorOptions.AvoidSelf));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindOne_NullSiteName_ThrowsArgumentNullException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            AssertExtensions.Throws<ArgumentNullException>("siteName", () => DomainController.FindOne(context, null));
            AssertExtensions.Throws<ArgumentNullException>("siteName", () => DomainController.FindOne(context, null, LocatorOptions.AvoidSelf));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindOne_EmptySiteName_ThrowsArgumentException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            AssertExtensions.Throws<ArgumentException>("siteName", () => DomainController.FindOne(context, string.Empty));
            AssertExtensions.Throws<ArgumentException>("siteName", () => DomainController.FindOne(context, string.Empty, LocatorOptions.AvoidSelf));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData((LocatorOptions)(-1))]
        [InlineData((LocatorOptions)int.MaxValue)]
        public void FindOne_InvalidFlag_ThrowsArgumentException(LocatorOptions flag)
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            AssertExtensions.Throws<ArgumentException>("flag", () => DomainController.FindOne(context, flag));
            AssertExtensions.Throws<ArgumentException>("flag", () => DomainController.FindOne(context, "siteName", flag));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData("\0", typeof(ActiveDirectoryObjectNotFoundException))]
        [InlineData("server:port", typeof(ActiveDirectoryOperationException))]
        public void FindOne_InvalidName_ThrowsException(string name, Type exceptionType)
        {
            var context = new DirectoryContext(DirectoryContextType.Domain, name);
            Assert.Throws(exceptionType, () => DomainController.FindOne(context, "siteName"));
            Assert.Throws(exceptionType, () => DomainController.FindOne(context, "siteName", LocatorOptions.AvoidSelf));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Not approved COM object for app")]
        public void FindAll_NoSuchName_ReturnsEmpty()
        {
            // Domain joined machines can have entries in the DomainController.
            if (PlatformDetection.IsDomainJoinedMachine)
            {
                var context = new DirectoryContext(DirectoryContextType.Domain, "\0");
                Assert.NotNull(DomainController.FindAll(context));
                Assert.NotNull(DomainController.FindAll(context, "siteName"));
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [OuterLoop]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Getting information about domain is denied inside App")]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/21553", TargetFrameworkMonikers.UapAot)]
        public void FindAll_NullName_ThrowsActiveDirectoryOperationException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            if (!PlatformDetection.IsDomainJoinedMachine)
            {
                Assert.Throws<ActiveDirectoryOperationException>(() => DomainController.FindAll(context));
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindAll_NullContext_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindAll(null));
            AssertExtensions.Throws<ArgumentNullException>("context", () => DomainController.FindAll(null, "siteName"));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(DirectoryContextType.ApplicationPartition)]
        [InlineData(DirectoryContextType.ConfigurationSet)]
        [InlineData(DirectoryContextType.DirectoryServer)]
        [InlineData(DirectoryContextType.Forest)]
        public void FindAll_InvalidContextType_ThrowsArgumentException(DirectoryContextType contextType)
        {
            var context = new DirectoryContext(contextType, "name");
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindAll(context));
            AssertExtensions.Throws<ArgumentException>("context", () => DomainController.FindAll(context, "siteName"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindAll_NullSiteName_ThrowsArgumentNullException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            AssertExtensions.Throws<ArgumentNullException>("siteName", () => DomainController.FindAll(context, null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindAll_EmptySiteName_ThrowsArgumentException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain);
            AssertExtensions.Throws<ArgumentException>("siteName", () => DomainController.FindAll(context, string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void FindAll_InvalidName_ThrowsActiveDirectoryOperationException()
        {
            var context = new DirectoryContext(DirectoryContextType.Domain, "server:port");
            Assert.Throws<ActiveDirectoryOperationException>(() => DomainController.FindAll(context, "siteName"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckReplicationConsistency_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.CheckReplicationConsistency());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckReplicationConsistency_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.CheckReplicationConsistency());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CurrentTime_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.CurrentTime);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Domain_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.Domain);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Forest_GetWithNoContext_ThrowsActiveDirectoryObjectNotFoundException()
        {
            var controller = new SubController();
            Assert.Throws<ActiveDirectoryObjectNotFoundException>(() => controller.Forest);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void EnableGlobalCatalog_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.EnableGlobalCatalog());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void EnableGlobalCatalog_NoContext_ThrowsActiveDirectoryObjectNotFoundException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.EnableGlobalCatalog());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetAllReplicationNeighbors_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetAllReplicationNeighbors());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetAllReplicationNeighbors_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetAllReplicationNeighbors());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetDirectoryEntry_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetDirectoryEntry());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetDirectoryEntry_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetDirectoryEntry());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetDirectorySearcher_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetDirectorySearcher());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationConnectionFailures_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetReplicationConnectionFailures());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationConnectionFailures_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetReplicationConnectionFailures());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationCursors_NullPartition_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("partition", () => controller.GetReplicationCursors(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationCursors_EmptyPartition_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("partition", () => controller.GetReplicationCursors(string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationCursors_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetReplicationCursors("partition"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationCursors_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetReplicationCursors("partition"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationMetadata_NullObjectPath_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("objectPath", () => controller.GetReplicationMetadata(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationMetadata_EmptyObjectPath_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("objectPath", () => controller.GetReplicationMetadata(string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationMetadata_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetReplicationMetadata("objectPath"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationMetadata_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetReplicationMetadata("objectPath"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationNeighbors_NullPartition_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("partition", () => controller.GetReplicationNeighbors(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationNeighbors_EmptyPartition_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("partition", () => controller.GetReplicationNeighbors(string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationNeighbors_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetReplicationNeighbors("partition"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationNeighbors_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetReplicationNeighbors("partition"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationOperationInformation_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.GetReplicationOperationInformation());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void GetReplicationOperationInformation_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.GetReplicationOperationInformation());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void HighestCommittedUsn_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.HighestCommittedUsn);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void InboundConnections_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.InboundConnections);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void InboundConnections_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.InboundConnections);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void IPAddress_GetWithNoContext_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("hostNameOrAddress", () => controller.IPAddress);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void IsGlobalCatalog_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.IsGlobalCatalog());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void IsGlobalCatalog_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.IsGlobalCatalog());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void MoveToAnotherSite_NullSiteName_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("siteName", () => controller.MoveToAnotherSite(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void MoveToAnotherSite_EmptySiteName_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("siteName", () => controller.MoveToAnotherSite(string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void MoveToAnotherSite_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.MoveToAnotherSite("siteName"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void MoveToAnotherSite_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.MoveToAnotherSite("siteName"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Name_GetWithNoContext_ReturnsNull()
        {
            var controller = new SubController();
            Assert.Null(controller.Name);
            Assert.Null(controller.ToString());
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void OSVersion_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.OSVersion);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void OSVersion_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.OSVersion);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void OutboundConnections_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.OutboundConnections);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void OutboundConnections_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.OutboundConnections);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Partitions_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.Partitions);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Roles_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.Roles);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Roles_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.Roles);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SeizeRoleOwnership_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.SeizeRoleOwnership(ActiveDirectoryRole.InfrastructureRole));
            Assert.Throws<NullReferenceException>(() => controller.SeizeRoleOwnership(ActiveDirectoryRole.NamingRole));
            Assert.Throws<NullReferenceException>(() => controller.SeizeRoleOwnership(ActiveDirectoryRole.PdcRole));
            Assert.Throws<NullReferenceException>(() => controller.SeizeRoleOwnership(ActiveDirectoryRole.RidRole));
            Assert.Throws<NullReferenceException>(() => controller.SeizeRoleOwnership(ActiveDirectoryRole.SchemaRole));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(ActiveDirectoryRole.SchemaRole - 1)]
        [InlineData(ActiveDirectoryRole.InfrastructureRole + 1)]
        public void SeizeRoleOwnership_InvalidRole_ThrowsInvalidEnumArgumentException(ActiveDirectoryRole role)
        {
            var controller = new SubController();
            AssertExtensions.Throws<InvalidEnumArgumentException>("role", () => controller.SeizeRoleOwnership(role));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SiteName_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.SiteName);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SiteName_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.SiteName);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncFromAllServersCallback_Set_GetReturnsExpected()
        {
            SyncUpdateCallback callback = SyncUpdateCallback;
            var controller = new SubController { SyncFromAllServersCallback = callback };
            Assert.Equal(callback, controller.SyncFromAllServersCallback);
        }

        private bool SyncUpdateCallback(SyncFromAllServersEvent eventType, string targetServer, string sourceServer, SyncFromAllServersOperationException exception)
        {
            return true;
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncFromAllServersCallback_GetSetWhenDisposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.SyncFromAllServersCallback);
            Assert.Throws<ObjectDisposedException>(() => controller.SyncFromAllServersCallback = null);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncFromAllServersCallback_GetWithNoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Null(controller.SyncFromAllServersCallback);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_NullPartition_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("partition", () => controller.SyncReplicaFromServer(null, "sourceServer"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_EmptyPartition_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("partition", () => controller.SyncReplicaFromServer(string.Empty, "sourceServer"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_NullSourceServer_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("sourceServer", () => controller.SyncReplicaFromServer("partition", null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_EmptySourceServer_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("sourceServer", () => controller.SyncReplicaFromServer("partition", string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.SyncReplicaFromServer("partition", "sourceServer"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromServer_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.SyncReplicaFromServer("partition", "sourceServer"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromAllServers_NullPartition_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("partition", () => controller.SyncReplicaFromAllServers(null, SyncFromAllServersOptions.AbortIfServerUnavailable));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromAllServers_EmptyPartition_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("partition", () => controller.SyncReplicaFromAllServers(string.Empty, SyncFromAllServersOptions.AbortIfServerUnavailable));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromAllServers_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.SyncReplicaFromAllServers("partition", SyncFromAllServersOptions.AbortIfServerUnavailable));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void SyncReplicaFromAllServers_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.SyncReplicaFromAllServers("partition", SyncFromAllServersOptions.AbortIfServerUnavailable));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TransferRoleOwnership_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.TransferRoleOwnership(ActiveDirectoryRole.NamingRole));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(ActiveDirectoryRole.SchemaRole - 1)]
        [InlineData(ActiveDirectoryRole.InfrastructureRole + 1)]
        public void TransferRoleOwnership_InvalidRole_ThrowsInvalidEnumArgumentException(ActiveDirectoryRole role)
        {
            var controller = new SubController();
            AssertExtensions.Throws<InvalidEnumArgumentException>("role", () => controller.TransferRoleOwnership(role));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TriggerSyncReplicaFromNeighbors_NullPartition_ThrowsArgumentNullException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentNullException>("partition", () => controller.TriggerSyncReplicaFromNeighbors(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TriggerSyncReplicaFromNeighbors_EmptyPartition_ThrowsArgumentException()
        {
            var controller = new SubController();
            AssertExtensions.Throws<ArgumentException>("partition", () => controller.TriggerSyncReplicaFromNeighbors(string.Empty));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TriggerSyncReplicaFromNeighbors_Disposed_ThrowsObjectDisposedException()
        {
            var controller = new SubController();
            controller.Dispose();

            Assert.Throws<ObjectDisposedException>(() => controller.TriggerSyncReplicaFromNeighbors("partition"));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TriggerSyncReplicaFromNeighbors_NoContext_ThrowsNullReferenceException()
        {
            var controller = new SubController();
            Assert.Throws<NullReferenceException>(() => controller.TriggerSyncReplicaFromNeighbors("partition"));
        }

        private class SubController : DomainController
        {
        }
    }
}
