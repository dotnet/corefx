// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;

using Microsoft.Win32.SafeHandles;
using Xunit;

public class WindowsIdentityTestsImpersonate : IClassFixture<WindowsIdentityImpersonateFixture>
{
    private const string authenticationType = "WindowsAuthentication";
    private WindowsIdentityImpersonateFixture _fixture;

    public WindowsIdentityTestsImpersonate(WindowsIdentityImpersonateFixture windowsIdentityImpersonateFixture)
    {
        _fixture = windowsIdentityImpersonateFixture;

        Assert.False(_fixture.TestAccount1.AccountTokenHandle.IsInvalid);
        Assert.False(string.IsNullOrEmpty(_fixture.TestAccount1.AccountName));

        Assert.False(_fixture.TestAccount2.AccountTokenHandle.IsInvalid);
        Assert.False(string.IsNullOrEmpty(_fixture.TestAccount2.AccountName));
    }

    // Test to do in a domain without SeTcbPrivilege privilege
    // [Fact]
    public static void UPNCtor_NoSeTcbPrivilege()
    {
        string domainUser = "userName@domain";
        string expectedName = "domain\\userName";
        var windowsIdentity = new WindowsIdentity(domainUser, authenticationType);
        Assert.True(windowsIdentity.IsAuthenticated);
        Assert.Equal(expectedName, windowsIdentity.Name);
        using (windowsIdentity.Impersonate())
        {
            Assert.Equal(expectedName, WindowsIdentity.GetCurrent().Name);
            Assert.Equal(TokenImpersonationLevel.Identification, WindowsIdentity.GetCurrent().ImpersonationLevel);
        }
    }

    // Test to do in a domain with SeTcbPrivilege privilege
    // [Fact]
    public static void UPNCtor_SeTcbPrivilege()
    {
        string domainUser = "userName@domain";
        string expectedName = "domain\\userName";
        var windowsIdentity = new WindowsIdentity(domainUser, authenticationType);
        Assert.True(windowsIdentity.IsAuthenticated);
        Assert.Equal(expectedName, windowsIdentity.Name);
        using (windowsIdentity.Impersonate())
        {
            Assert.Equal(expectedName, WindowsIdentity.GetCurrent().Name);
            Assert.Equal(TokenImpersonationLevel.Impersonation, WindowsIdentity.GetCurrent().ImpersonationLevel);
        }
    }

    [Fact]
    public void Impersonate_WindowsIdentityObject()
    {
        Impersonate(() => new WindowsIdentity(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()).Impersonate(), _fixture.TestAccount1);
    }

    [Fact]
    public void Impersonate_UserTokenObject()
    {
        Impersonate(() => WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()), _fixture.TestAccount1);
    }

    private void Impersonate(Func<WindowsImpersonationContext> ctxFactory, WindowsTestAccount impersonatedAccount)
    {
        // Users on same machine could return different case for machine/domain name for WindowsIdentity.Name

        WindowsIdentity previous = WindowsIdentity.GetCurrent();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Name.Equals(impersonatedAccount.AccountName, StringComparison.InvariantCultureIgnoreCase));

        // test with explicit Undo() call
        WindowsImpersonationContext ctx = ctxFactory();
        try
        {
            Assert.Equal(impersonatedAccount.AccountName, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
        }
        finally
        {
            ctx.Undo();
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);

        // test with Dispose pattern
        using (ctx = ctxFactory())
        {
            Assert.Equal(impersonatedAccount.AccountName, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
    }

    [Fact]
    public void Impersonate_WindowsIdentityObject_InvalidToken()
    {
        Assert.Throws<ArgumentException>(() => new WindowsIdentity(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()).Impersonate());
    }

    [Fact]
    public void Impersonate_UserTokenObject_ZeroToken_NOP()
    {
        // Users on same machine could return different case for machine/domain name for WindowsIdentity.Name

        WindowsIdentity previous = WindowsIdentity.GetCurrent();

        // impersonating a zero token means clear the token on the thread in this case NOP
        using (WindowsIdentity.Impersonate(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()))
        {
            Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
    }

    [Fact]
    public void Impersonate_IsImpersonating_UserTokenObject_ZeroToken_ClearThreadToken()
    {
        // Users on same machine could return different case for machine/domain name for WindowsIdentity.Name

        WindowsIdentity previous = WindowsIdentity.GetCurrent();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Name.Equals(_fixture.TestAccount1.AccountName, StringComparison.InvariantCultureIgnoreCase));

        using (WindowsImpersonationContext ctx = WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
        {
            Assert.Equal(WindowsIdentity.GetCurrent().Name, _fixture.TestAccount1.AccountName, ignoreCase: true);

            // impersonating a zero token means clear the token on the thread
            using (WindowsIdentity.Impersonate(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()))
            {
                Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
            }
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name, ignoreCase: true);
    }

    [Fact]
    public void Impersonate_IsImpersonating_WindowsIdentityObject()
    {
        Impersonate_IsImpersonating(
            () => (_fixture.TestAccount1, () => new WindowsIdentity(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()).Impersonate()),
            () => (_fixture.TestAccount2, () => new WindowsIdentity(_fixture.TestAccount2.AccountTokenHandle.DangerousGetHandle()).Impersonate()));
    }

    [Fact]
    public void Impersonate_IsImpersonating_UserTokenObject()
    {
        Impersonate_IsImpersonating(
            () => (_fixture.TestAccount1, () => WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle())),
            () => (_fixture.TestAccount2, () => WindowsIdentity.Impersonate(_fixture.TestAccount2.AccountTokenHandle.DangerousGetHandle())));
    }

    public void Impersonate_IsImpersonating(Func<(WindowsTestAccount, Func<WindowsImpersonationContext>)> user1Data, Func<(WindowsTestAccount, Func<WindowsImpersonationContext>)> user2Data)
    {
        // Users on same machine could return different case for machine/domain name for WindowsIdentity.Name

        WindowsIdentity previous = WindowsIdentity.GetCurrent();

        // test with explicit Undo() call
        (WindowsTestAccount TestAccountUser1, Func<WindowsImpersonationContext> CtxFactoryUser1) = user1Data();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Name.Equals(TestAccountUser1.AccountName, StringComparison.InvariantCultureIgnoreCase));

        WindowsImpersonationContext user1ctx = CtxFactoryUser1();
        try
        {
            Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser1.AccountName, ignoreCase: true);

            (WindowsTestAccount TestAccountUser2, Func<WindowsImpersonationContext> CtxFactoryUser2) = user2Data();
            WindowsImpersonationContext user2ctx = CtxFactoryUser2();
            try
            {
                Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser2.AccountName, ignoreCase: true);
            }
            finally
            {
                user2ctx.Undo();
            }

            Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser1.AccountName, ignoreCase: true);
        }
        finally
        {
            user1ctx.Undo();
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name);

        // test with Dispose pattern
        using (WindowsImpersonationContext ctxUser1 = CtxFactoryUser1())
        {
            Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser1.AccountName, ignoreCase: true);

            (WindowsTestAccount TestAccountUser2, Func<WindowsImpersonationContext> CtxFactoryUser2) = user2Data();
            using (WindowsImpersonationContext ctxUser2 = CtxFactoryUser2())
            {
                Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser2.AccountName, ignoreCase: true);
            }

            Assert.Equal(WindowsIdentity.GetCurrent().Name, TestAccountUser1.AccountName, ignoreCase: true);
        }

        Assert.Equal(previous.Name, WindowsIdentity.GetCurrent().Name);
    }

    // Impersonate doesn't behave like RunImpersonated, 
    // RunImpersonated reverts ExecutionContext
    [Fact]
    public void Impersonate_ExcutionContext_NotReverted()
    {
        AsyncLocal<string> impersonatedContextValue = new AsyncLocal<string>();

        impersonatedContextValue.Value = "";

        using (WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
        {
            impersonatedContextValue.Value = "NewValue";
        }

        Assert.Equal("NewValue", impersonatedContextValue.Value);

        impersonatedContextValue.Value = "";

        WindowsIdentity.RunImpersonated(_fixture.TestAccount1.AccountTokenHandle, () => impersonatedContextValue.Value = "NewValue");

        Assert.Equal("", impersonatedContextValue.Value);
        Assert.NotEqual("NewValue", impersonatedContextValue.Value);
    }
}

public class WindowsIdentityImpersonateFixture : IDisposable
{
    public WindowsTestAccount TestAccount1 { get; private set; }
    public WindowsTestAccount TestAccount2 { get; private set; }

    public WindowsIdentityImpersonateFixture()
    {
        TestAccount1 = new WindowsTestAccount("CorFxTstWiImp01kiu");
        TestAccount1.Create();
        TestAccount2 = new WindowsTestAccount("CorFxTstWiImp02lpu");
        TestAccount2.Create();
    }

    public void Dispose()
    {
        TestAccount1.Dispose();
        TestAccount2.Dispose();
    }
}

public sealed class WindowsTestAccount : IDisposable
{
    private readonly string _userName;
    private SafeAccessTokenHandle _accountTokenHandle;
    public SafeAccessTokenHandle AccountTokenHandle => _accountTokenHandle;
    public string AccountName
    {
        get
        {
            // We should not use System.Security.Principal.Windows classes that we'are testing.
            // To avoid too much pinvoke plumbing to get userName from OS for now we concat machine name.
            return Environment.MachineName + "\\" + _userName;
        }
    }
    public WindowsTestAccount(string userName) => _userName = userName;

    public void Create()
    {
        string testAccountPassword;
        using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[33];
            rng.GetBytes(randomBytes);

            // Add special chars to ensure it satisfies password requirements.
            testAccountPassword = Convert.ToBase64String(randomBytes) + "_-As@!%*(1)4#2";
            DateTime accountExpirationDate = DateTime.Now.AddMinutes(2);
            using (var principalCtx = new PrincipalContext(ContextType.Machine))
            {
                bool needToCreate = false;
                using (var foundUserPrincipal = UserPrincipal.FindByIdentity(principalCtx, _userName))
                {
                    if (foundUserPrincipal == null)
                    {
                        needToCreate = true;
                    }
                    else
                    {
                        // Somehow the account leaked from previous runs, however, password is lost, reset it.
                        foundUserPrincipal.SetPassword(testAccountPassword);
                        foundUserPrincipal.AccountExpirationDate = accountExpirationDate;
                        foundUserPrincipal.Save();
                    }
                }

                if (needToCreate)
                {
                    using (var userPrincipal = new UserPrincipal(principalCtx))
                    {
                        userPrincipal.SetPassword(testAccountPassword);
                        userPrincipal.AccountExpirationDate = accountExpirationDate;
                        userPrincipal.Name = _userName;
                        userPrincipal.DisplayName = _userName;
                        userPrincipal.Description = _userName;
                        userPrincipal.Save();
                    }
                }

                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                if (!LogonUser(_userName, ".", testAccountPassword, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out _accountTokenHandle))
                {
                    _accountTokenHandle = null;
                    throw new Exception($"Failed to get SafeAccessTokenHandle for test account {_userName}", new Win32Exception());
                }
            }
        }

    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string userName, string domain, string password, int logonType, int logonProvider, out SafeAccessTokenHandle safeAccessTokenHandle);

    public void Dispose()
    {
        if (_accountTokenHandle == null)
        {
            return;
        }

        _accountTokenHandle.Dispose();
        _accountTokenHandle = null;

        using (var principalCtx = new PrincipalContext(ContextType.Machine))
        using (var userPrincipal = UserPrincipal.FindByIdentity(principalCtx, AccountName))
        {
            if (userPrincipal == null)
            {
                throw new Exception($"Failed to get user principal to delete test account {AccountName}");
            }

            try
            {
                userPrincipal.Delete();
            }
            catch (InvalidOperationException)
            {
                // TODO: Investigate, it always throw this exception with "Can't delete object already deleted", but it actually deletes it.
            }
        }
    }
}
