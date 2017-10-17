// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.DirectoryServices.Tests;
using System.Linq;
using Xunit;

namespace System.DirectoryServices.AccountManagement.Tests
{
    public class AccountManagementTests
    {
        internal static bool IsLdapConfigurationExist => LdapConfiguration.Configuration != null;
        internal static bool IsActiveDirectoryServer => IsLdapConfigurationExist && LdapConfiguration.Configuration.IsActiveDirectoryServer;

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestCurrentUse()
        {
            using (PrincipalContext context = DomainContext)
            using (UserPrincipal p = FindUser(LdapConfiguration.Configuration.UserNameWithNoDomain, context))
            {
                Assert.NotNull(p);
                Assert.Equal(LdapConfiguration.Configuration.UserNameWithNoDomain, p.Name);

                using (UserPrincipal cu = UserPrincipal.Current)
                {
                    try
                    {
                        // UserPrincipal.Current get the context from the current thread.
                        PrincipalContext pc = cu.Context;
                        Assert.NotEqual(pc.Name, p.Context.Name);
                    }
                    catch
                    {
                        // ignore the exception here as UserPrincipal.Current.Context can throw for
                        // non joined domain machines
                    }
                }
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestCurrentUserUsingSearchFilter()
        {
            using (PrincipalContext context = DomainContext)
            using (UserPrincipal p = FindUserUsingFilter(LdapConfiguration.Configuration.UserNameWithNoDomain, context))
            {
                Assert.NotNull(p);
                Assert.Equal(LdapConfiguration.Configuration.UserNameWithNoDomain, p.Name);
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestGuestsGroup()
        {
            using (PrincipalContext context = DomainContext)
            using (GroupPrincipal p = FindGroup("Guests", context))
            {
                Assert.NotNull(p);
                Assert.Equal("Guests", p.Name);
                Assert.Equal("Guests", p.SamAccountName);
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestAddingUser()
        {
            DeleteUser("corefxtest1");
            DeleteUser("corefxtest2");
            DeleteUser("corefxtest3");

            try
            {
                using (PrincipalContext context = DomainContext)
                using (UserPrincipal p1 = CreateUser(context, "corefxtest1", "netcore#1pass", "corefx1", "test1", "CoreFx Test User 1"))
                using (UserPrincipal p2 = CreateUser(context, "corefxtest2", "netcore#2pass", "corefx2", "test2", "CoreFx Test User 2"))
                using (UserPrincipal p3 = CreateUser(context, "corefxtest3", "netcore#3pass", "corefx3", "test3", "CoreFx Test User 3"))
                {
                    Assert.NotNull(p1);
                    Assert.NotNull(p2);
                    Assert.NotNull(p3);

                    ValidateRecentAddedUser(context, "corefxtest1", "corefx1", "test1", "CoreFx Test User 1");
                    ValidateRecentAddedUser(context, "corefxtest2", "corefx2", "test2", "CoreFx Test User 2");
                    ValidateRecentAddedUser(context, "corefxtest3", "corefx3", "test3", "CoreFx Test User 3");

                    ValidateUserUsingPrincipal(context, p1);
                    ValidateUserUsingPrincipal(context, p2);
                    ValidateUserUsingPrincipal(context, p3);
                }
            }
            finally
            {
                DeleteUser("corefxtest1");
                DeleteUser("corefxtest2");
                DeleteUser("corefxtest3");
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestAddingGroup()
        {
            DeleteGroup("corefxgroup1");
            DeleteGroup("corefxgroup2");
            DeleteGroup("corefxgroup3");

            try
            {
                using (PrincipalContext context = DomainContext)
                using (GroupPrincipal p1 = CreateGroup(context, "corefxgroup1", "CoreFX Test Group 1", "CoreFX Group 1"))
                using (GroupPrincipal p2 = CreateGroup(context, "corefxgroup2", "CoreFX Test Group 2", "CoreFX Group 2"))
                using (GroupPrincipal p3 = CreateGroup(context, "corefxgroup3", "CoreFX Test Group 3", "CoreFX Group 3"))
                {
                    Assert.NotNull(p1);
                    Assert.NotNull(p2);
                    Assert.NotNull(p3);

                    ValidateRecentAddedGroup(context, "corefxgroup1", "CoreFX Test Group 1", "CoreFX Group 1");
                    ValidateRecentAddedGroup(context, "corefxgroup2", "CoreFX Test Group 2", "CoreFX Group 2");
                    ValidateRecentAddedGroup(context, "corefxgroup3", "CoreFX Test Group 3", "CoreFX Group 3");

                    ValidateGroupUsingPrincipal(context, p1);
                    ValidateGroupUsingPrincipal(context, p2);
                    ValidateGroupUsingPrincipal(context, p3);
                }
            }
            finally
            {
                DeleteGroup("corefxgroup1");
                DeleteGroup("corefxgroup2");
                DeleteGroup("corefxgroup3");
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestAddingUserToAGroup()
        {
            DeleteUser("corefxchilduser1");
            DeleteGroup("corefxgroupcontainer1");

            try
            {
                using (PrincipalContext context = DomainContext)
                using (UserPrincipal user = CreateUser(context, "corefxchilduser1", "netcore#7pass", "corefxchilduser1", "testchild1", "CoreFx Test Child User 1"))
                using (GroupPrincipal group = CreateGroup(context, "corefxgroupcontainer1", "CoreFX Group Container 1", "CoreFX Test Group Container 1"))
                {
                    Assert.Equal("corefxchilduser1", user.Name);
                    Assert.Equal("corefxgroupcontainer1", group.Name);

                    // First, check the user is not in the group

                    Assert.False(user.GetGroups().Contains(group));
                    Assert.False(user.IsMemberOf(group));
                    Assert.False(group.Members.Contains(user));

                    // second, add user and validate it is member of the group

                    group.Members.Add(context, IdentityType.Name, user.Name);
                    group.Save();

                    Assert.True(user.GetGroups().Contains(group));
                    Assert.True(user.IsMemberOf(group));
                    Assert.True(group.Members.Contains(user));

                    // Third, remove the user from the group and check again
                    group.Members.Remove(context, IdentityType.Name, user.Name);
                    group.Save();
                    Assert.False(user.GetGroups().Contains(group));
                    Assert.False(user.IsMemberOf(group));
                    Assert.False(group.Members.Contains(user));
                }
            }
            finally
            {
                DeleteUser("corefxchilduser1");
                DeleteGroup("corefxgroupcontainer1");
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestDeleteUserAndGroup()
        {
            DeleteUser("corefxchilduser2");
            DeleteGroup("corefxgroupcontainer2");

            try
            {
                using (PrincipalContext context = DomainContext)
                {
                    using (UserPrincipal up = FindUser("corefxchilduser2", context)) { Assert.Null(up); }
                    using (GroupPrincipal gp = FindGroup("corefxgroupcontainer2", context)) { Assert.Null(gp); }

                    using (UserPrincipal user = CreateUser(context, "corefxchilduser2", "netcore#8pass", "corefxchilduser2", "testchild2", "CoreFx Test Child User 2"))
                    using (GroupPrincipal group = CreateGroup(context, "corefxgroupcontainer2", "CoreFX Group Container 2", "CoreFX Test Group Container 2"))
                    {
                        using (UserPrincipal up = FindUser("corefxchilduser2", context))
                        {
                            Assert.NotNull(up);
                            up.Delete();
                        }
                        using (GroupPrincipal gp = FindGroup("corefxgroupcontainer2", context))
                        {
                            Assert.NotNull(gp);
                            gp.Delete();
                        }
                    }

                    using (UserPrincipal up = FindUser("corefxchilduser2", context)) { Assert.Null(up); }
                    using (GroupPrincipal gp = FindGroup("corefxgroupcontainer2", context)) { Assert.Null(gp); }
                }
            }
            finally
            {
                DeleteUser("corefxchilduser2");
                DeleteGroup("corefxgroupcontainer2");
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestNegativeCases()
        {
            DeleteUser("corefxchilduser3");
            DeleteGroup("corefxgroupcontainer3");

            try
            {
                Assert.Throws<InvalidEnumArgumentException>(() => new PrincipalContext((ContextType) 768, null, null, null));
                Assert.Throws<PrincipalServerDownException>(() => new PrincipalContext(ContextType.Domain, "InvalidDomainName", null, null));
                Assert.Throws<ArgumentException>(() => new PrincipalContext(ContextType.Domain, LdapConfiguration.Configuration.ServerName, "InvalidTestUserName", null));
                Assert.Throws<ArgumentException>(() => new PrincipalContext(ContextType.Domain, LdapConfiguration.Configuration.ServerName, LdapConfiguration.Configuration.UserName, null));
                Assert.Throws<ArgumentException>(() => new UserPrincipal(null));
                Assert.Throws<ArgumentException>(() => new GroupPrincipal(null));

                using (PrincipalContext context = DomainContext)
                {
                    using (UserPrincipal user = CreateUser(context, "corefxchilduser3", "netcore#9pass", "corefxchilduser3", "testchild3", "CoreFx Test Child User 3"))
                    using (GroupPrincipal group = CreateGroup(context, "corefxgroupcontainer3", "CoreFX Group Container 3", "CoreFX Test Group Container 3"))
                    {
                        Assert.Throws<PrincipalExistsException>(() => CreateUser(context, "corefxchilduser3", "netcore#9pass", "corefxchilduser3", "testchild3", "CoreFx Test Child User 3"));
                        Assert.Throws<PrincipalExistsException>(() => CreateGroup(context, "corefxgroupcontainer3", "CoreFX Group Container 3", "CoreFX Test Group Container 3"));

                        group.Members.Add(context, IdentityType.Name, user.Name);
                        group.Save();
                        Assert.Throws<PrincipalExistsException>(() => group.Members.Add(context, IdentityType.Name, user.Name));
                        group.Members.Remove(context, IdentityType.Name, user.Name);
                        group.Save();

                        user.Delete();
                        Assert.Throws<InvalidOperationException>(() => user.Delete());
                        Assert.Throws<InvalidOperationException>(() => user.Save());

                        group.Delete();
                        Assert.Throws<InvalidOperationException>(() => group.Delete());
                        Assert.Throws<InvalidOperationException>(() => group.Save());
                    }
                }
            }
            finally
            {
                DeleteUser("corefxchilduser3");
                DeleteGroup("corefxgroupcontainer3");
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestComputerContext()
        {
            using (PrincipalContext context = DomainContext)
            {
                using (ComputerPrincipal cp = new ComputerPrincipal(context))
                {
                    cp.Name = "*";
                    PrincipalSearcher ps = new PrincipalSearcher();
                    ps.QueryFilter = cp;
                    using (ComputerPrincipal r1 = ps.FindOne() as ComputerPrincipal)
                    using (ComputerPrincipal r2 = ComputerPrincipal.FindByIdentity(context, r1.Name))
                    {
                        Assert.Equal(r2.AccountExpirationDate, r1.AccountExpirationDate);
                        Assert.Equal(r2.Description, r1.Description);
                        Assert.Equal(r2.DisplayName, r1.DisplayName);
                        Assert.Equal(r2.DistinguishedName, r1.DistinguishedName);
                        Assert.Equal(r2.Guid, r1.Guid);
                        Assert.Equal(r2.HomeDirectory, r1.HomeDirectory);
                        Assert.Equal(r2.HomeDrive, r1.HomeDrive);
                        Assert.Equal(r2.SamAccountName, r1.SamAccountName);
                        Assert.Equal(r2.Sid, r1.Sid);
                        Assert.Equal(r2.UserPrincipalName, r1.UserPrincipalName);
                    }
                }
            }
        }

        [ConditionalFact(nameof(IsActiveDirectoryServer))]
        public void TestUpdateUserAndGroupData()
        {
            DeleteUser("corefxchilduser4");
            DeleteGroup("corefxgroupcontainer4");

            try
            {
                using (PrincipalContext context = DomainContext)
                using (UserPrincipal user = CreateUser(context, "corefxchilduser4", "netcore#5pass", "corefxchilduser4", "testchild4", "CoreFx Test Child User 4"))
                using (GroupPrincipal group = CreateGroup(context, "corefxgroupcontainer4", "CoreFX Group Container 4", "CoreFX Test Group Container 4"))
                {
                    using (UserPrincipal up = FindUser("corefxchilduser4", context)) { Assert.Equal(user.DisplayName, up.DisplayName); }
                    using (GroupPrincipal gp = FindGroup("corefxgroupcontainer4", context)) { Assert.Equal(group.DisplayName, gp.DisplayName); }

                    user.DisplayName = "Updated CoreFx Test Child User 4";
                    user.Save();
                    group.DisplayName = "Updated CoreFX Test Group Container 4";
                    group.Save();

                    using (UserPrincipal up = FindUser("corefxchilduser4", context)) { Assert.Equal("Updated CoreFx Test Child User 4", up.DisplayName); }
                    using (GroupPrincipal gp = FindGroup("corefxgroupcontainer4", context)) { Assert.Equal("Updated CoreFX Test Group Container 4", gp.DisplayName); }
                }
            }
            finally
            {
                DeleteUser("corefxchilduser4");
                DeleteGroup("corefxgroupcontainer4");
            }
        }

        private void ValidateRecentAddedUser(PrincipalContext context, string userName, string firstName, string lastName, string displayName)
        {
            using (UserPrincipal p = FindUser(userName, context))
            {
                Assert.NotNull(p);
                Assert.Equal(userName, p.Name);
                Assert.Equal(firstName, p.GivenName);
                Assert.Equal(lastName, p.Surname);
                Assert.Equal(displayName, p.DisplayName);
                Assert.True(p.DistinguishedName.IndexOf(userName, StringComparison.OrdinalIgnoreCase) >= 0);
                Assert.Equal(userName, p.SamAccountName);
            }
        }

        private void ValidateRecentAddedGroup(PrincipalContext context, string groupName, string description, string displayName)
        {
            using (GroupPrincipal p = FindGroup(groupName, context))
            {
                Assert.NotNull(p);
                Assert.Equal(groupName, p.Name);
                Assert.Equal(description, p.Description);
                Assert.Equal(displayName, p.DisplayName);
                Assert.Equal(groupName, p.SamAccountName);
                Assert.True(p.DistinguishedName.IndexOf(groupName, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void ValidateUserUsingPrincipal(PrincipalContext context, UserPrincipal user)
        {
            using (UserPrincipal p = FindUser(user.Name, context))
            {
                Assert.NotNull(p);
                Assert.Equal(user.Name, p.Name);
                Assert.Equal(user.GivenName, p.GivenName);
                Assert.Equal(user.Surname, p.Surname);
                Assert.Equal(user.DisplayName, p.DisplayName);
                Assert.Equal(user.SamAccountName, p.SamAccountName);

                Assert.Equal(user.Guid, p.Guid);
                Assert.Equal(user.Sid, p.Sid);
                Assert.Equal(user.UserPrincipalName, p.UserPrincipalName);
                Assert.Equal(user.UserCannotChangePassword, p.UserCannotChangePassword);
                Assert.Equal(user.Enabled, p.Enabled);
                Assert.Equal(user.AccountExpirationDate, p.AccountExpirationDate);
            }
        }

        private void ValidateGroupUsingPrincipal(PrincipalContext context, GroupPrincipal group)
        {
            using (GroupPrincipal p = FindGroup(group.Name, context))
            {
                Assert.NotNull(p);
                Assert.Equal(group.Name, p.Name);
                Assert.Equal(group.DisplayName, p.DisplayName);
                Assert.Equal(group.SamAccountName, p.SamAccountName);

                Assert.Equal(group.Guid, p.Guid);
                Assert.Equal(group.Sid, p.Sid);
                Assert.Equal(group.UserPrincipalName, p.UserPrincipalName);
                Assert.Equal(group.DistinguishedName, p.DistinguishedName);
            }
        }

        private UserPrincipal CreateUser(PrincipalContext context, string userName, string password, string firstName, string lastName, string displayName)
        {
            UserPrincipal user = new UserPrincipal(context, userName, password, true);

            // assign some properties to the user principal
            user.GivenName = firstName;
            user.Surname = lastName;
            user.DisplayName = displayName;
            user.Save();
            return user;
        }

        private GroupPrincipal CreateGroup(PrincipalContext context, string groupName, string description, string displayName)
        {
            GroupPrincipal group = new GroupPrincipal(context, groupName);
            group.Description = description;
            group.DisplayName = displayName;
            group.Save();
            return group;
        }

        private void DeleteGroup(string groupName)
        {
            try
            {
                using (PrincipalContext context = DomainContext)
                using (GroupPrincipal p = FindGroup(groupName, context))
                {
                    if (p != null)
                        p.Delete();
                }
            }
            catch
            {
                // ignore the failure as we use this method to ensure clean up even if the group not exist
            }
        }

        private void DeleteUser(string userName)
        {
            try
            {
                using (PrincipalContext context = DomainContext)
                using (UserPrincipal p = FindUser(userName, context))
                {
                    if (p != null)
                        p.Delete();
                }
            }
            catch
            {
                // ignore the failure as we use this method to ensure clean up even if the user not exist
            }
        }

        private GroupPrincipal FindGroup(string groupName, PrincipalContext context)
        {
            return GroupPrincipal.FindByIdentity(context, IdentityType.Name, groupName);
        }

        private UserPrincipal FindUser(string userName, PrincipalContext context)
        {
            return UserPrincipal.FindByIdentity(context, IdentityType.Name, userName);
        }

        private UserPrincipal FindUserUsingFilter(string userName, PrincipalContext context)
        {
            CustomUserPrincipal userPrincipal = new CustomUserPrincipal(context);
            userPrincipal.SetUserNameFilter(userName);
            PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal);
            return searcher.FindOne() as UserPrincipal;
        }

        private PrincipalContext DomainContext => new PrincipalContext(
                                                        ContextType.Domain,
                                                        LdapConfiguration.Configuration.ServerName,
                                                        LdapConfiguration.Configuration.UserName,
                                                        LdapConfiguration.Configuration.Password);

        private PrincipalContext MachineContext => new PrincipalContext(
                                                        ContextType.Machine,
                                                        LdapConfiguration.Configuration.ServerName,
                                                        LdapConfiguration.Configuration.UserName,
                                                        LdapConfiguration.Configuration.Password);
    }

    [DirectoryObjectClass("user")]
    public class CustomUserPrincipal : UserPrincipal
    {
        private CustomFilter _customFilter;

        public CustomUserPrincipal(PrincipalContext context) : base(context) { }

        public void SetUserNameFilter(string name)
        {
            ((CustomFilter) AdvancedSearchFilter).SetFilter(name);
        }

        public override AdvancedFilters AdvancedSearchFilter
        {
            get
            {
                if (_customFilter == null)
                {
                    _customFilter = new CustomFilter(this);
                }

                return _customFilter;
            }
        }
    }

    public class CustomFilter : AdvancedFilters
    {
        public CustomFilter(Principal p) : base(p) { }

        public void SetFilter(string userName)
        {
            this.AdvancedFilterSet("cn", userName, typeof(string), MatchType.Equals);
        }
    }
}
