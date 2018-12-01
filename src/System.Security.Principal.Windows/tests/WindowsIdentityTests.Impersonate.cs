// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
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
            Assert.Equal(expectedName, InteropHelper.GetCurrentUserName());
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
            Assert.Equal(expectedName, InteropHelper.GetCurrentUserName());
            Assert.Equal(TokenImpersonationLevel.Impersonation, WindowsIdentity.GetCurrent().ImpersonationLevel);
        }
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_WindowsIdentityObject()
    {
        Impersonate(() => new WindowsIdentity(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()).Impersonate(), _fixture.TestAccount1);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_UserTokenObject()
    {
        Impersonate(() => WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()), _fixture.TestAccount1);
    }

    private void Impersonate(Func<WindowsImpersonationContext> ctxFactory, WindowsTestAccount impersonatedAccount)
    {
        // Users on same machine could return different case for machine/domain name for WindowsIdentity.Name

        string previous = InteropHelper.GetCurrentUserName();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Equals(impersonatedAccount.AccountName, StringComparison.InvariantCultureIgnoreCase));

        // test with explicit Undo() call
        WindowsImpersonationContext ctx = ctxFactory();
        try
        {
            Assert.Equal(impersonatedAccount.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
        }
        finally
        {
            ctx.Undo();
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);

        // test with Dispose pattern
        using (ctx = ctxFactory())
        {
            Assert.Equal(impersonatedAccount.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_WindowsIdentityObject_InvalidToken()
    {
        Assert.Throws<ArgumentException>(() => new WindowsIdentity(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()).Impersonate());
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_UserTokenObject_ZeroToken_NOP()
    {
        // Users on same machine could return different case for machine/domain name

        string previous = InteropHelper.GetCurrentUserName();

        // impersonating a zero token means clear the token on the thread in this case NOP
        using (WindowsIdentity.Impersonate(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()))
        {
            Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_IsImpersonating_UserTokenObject_ZeroToken_ClearThreadToken()
    {
        // Users on same machine could return different case for machine/domain name

        string previous = InteropHelper.GetCurrentUserName();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Equals(_fixture.TestAccount1.AccountName, StringComparison.InvariantCultureIgnoreCase));

        using (WindowsImpersonationContext ctx = WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
        {
            Assert.Equal(InteropHelper.GetCurrentUserName(), _fixture.TestAccount1.AccountName, ignoreCase: true);

            // impersonating a zero token means clear the token on the thread
            using (WindowsIdentity.Impersonate(SafeAccessTokenHandle.InvalidHandle.DangerousGetHandle()))
            {
                Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);
            }
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName(), ignoreCase: true);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_IsImpersonating_WindowsIdentityObject()
    {
        Impersonate_IsImpersonating(
            () => (_fixture.TestAccount1, () => new WindowsIdentity(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()).Impersonate()),
            () => (_fixture.TestAccount2, () => new WindowsIdentity(_fixture.TestAccount2.AccountTokenHandle.DangerousGetHandle()).Impersonate()));
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public void Impersonate_IsImpersonating_UserTokenObject()
    {
        Impersonate_IsImpersonating(
            () => (_fixture.TestAccount1, () => WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle())),
            () => (_fixture.TestAccount2, () => WindowsIdentity.Impersonate(_fixture.TestAccount2.AccountTokenHandle.DangerousGetHandle())));
    }

    public void Impersonate_IsImpersonating(Func<(WindowsTestAccount, Func<WindowsImpersonationContext>)> user1Data, Func<(WindowsTestAccount, Func<WindowsImpersonationContext>)> user2Data)
    {
        // Users on same machine could return different case for machine/domain name

        string previous = InteropHelper.GetCurrentUserName();

        // test with explicit Undo() call
        (WindowsTestAccount TestAccountUser1, Func<WindowsImpersonationContext> CtxFactoryUser1) = user1Data();

        // Assert.NotEqual() lacks ignoreCase overload
        Assert.False(previous.Equals(TestAccountUser1.AccountName, StringComparison.InvariantCultureIgnoreCase));

        WindowsImpersonationContext user1ctx = CtxFactoryUser1();
        try
        {
            Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser1.AccountName, ignoreCase: true);

            (WindowsTestAccount TestAccountUser2, Func<WindowsImpersonationContext> CtxFactoryUser2) = user2Data();
            WindowsImpersonationContext user2ctx = CtxFactoryUser2();
            try
            {
                Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser2.AccountName, ignoreCase: true);
            }
            finally
            {
                user2ctx.Undo();
            }

            Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser1.AccountName, ignoreCase: true);
        }
        finally
        {
            user1ctx.Undo();
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName());

        // test with Dispose pattern
        using (WindowsImpersonationContext ctxUser1 = CtxFactoryUser1())
        {
            Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser1.AccountName, ignoreCase: true);

            (WindowsTestAccount TestAccountUser2, Func<WindowsImpersonationContext> CtxFactoryUser2) = user2Data();
            using (WindowsImpersonationContext ctxUser2 = CtxFactoryUser2())
            {
                Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser2.AccountName, ignoreCase: true);
            }

            Assert.Equal(InteropHelper.GetCurrentUserName(), TestAccountUser1.AccountName, ignoreCase: true);
        }

        Assert.Equal(previous, InteropHelper.GetCurrentUserName());
    }

    // Impersonate doesn't behave like RunImpersonated, 
    // RunImpersonated reverts ExecutionContext
    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    // On full framework 'RunImpersonate' doesn't capture/revert Execution context
    [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
    public void Impersonate_ExcutionContext_NotReverted()
    {
        AsyncLocal<string> impersonatedContextValue = new AsyncLocal<string>
        {
            Value = ""
        };

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

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public async Task Impersonate_FlowExecutionContext_RunImpersonate()
    {
        // To be sure to not use ThreadPool thread
        CustomTaskScheduler customTaskScheduler = new CustomTaskScheduler();
        TaskFactory customFactory = new TaskFactory(customTaskScheduler);
        List<bool> checkResult = new List<bool>();
        string current = null;

        await customFactory.StartNew(async () =>
        {
            current = InteropHelper.GetCurrentUserName();

            await WindowsIdentity.RunImpersonated(_fixture.TestAccount1.AccountTokenHandle, async () =>
            {
                await Impersonate_FlowExecutionContext_Body(checkResult, customFactory, _fixture.TestAccount1);
            });
        });

        Impersonate_FlowExecutionContext_Asserts(customTaskScheduler, checkResult, current);
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public async Task Impersonate_FlowExecutionContext_WindowsImpersonationContext()
    {
        // To be sure to not use ThreadPool thread
        CustomTaskScheduler customTaskScheduler = new CustomTaskScheduler();
        TaskFactory customFactory = new TaskFactory(customTaskScheduler);
        List<bool> checkResult = new List<bool>();
        string current = null;

        await customFactory.StartNew(async () =>
        {
            current = InteropHelper.GetCurrentUserName();

            using (WindowsImpersonationContext windowsImpersonationContext = WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
            {
                await Impersonate_FlowExecutionContext_Body(checkResult, customFactory, _fixture.TestAccount1);
            }
        });

        Impersonate_FlowExecutionContext_Asserts(customTaskScheduler, checkResult, current);
    }

    private async Task Impersonate_FlowExecutionContext_Body(List<bool> checkResult, TaskFactory customFactory, WindowsTestAccount testAccount)
    {
        checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));

        await customFactory.StartNew(async () =>
        {
            checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));

            await customFactory.StartNew(async () =>
            {
                checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));

                await customFactory.StartNew(() =>
                {
                    checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));
                });

                checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));
            });

            checkResult.Add(testAccount.AccountName.Equals(InteropHelper.GetCurrentUserName(), StringComparison.InvariantCultureIgnoreCase));
        });
    }

    private void Impersonate_FlowExecutionContext_Asserts(CustomTaskScheduler customTaskScheduler, List<bool> checkResult, string current)
    {
        customTaskScheduler.WaitOutstandingThread();

        Assert.Equal(current, InteropHelper.GetCurrentUserName());
        Assert.False(checkResult.Exists(check => !check), $"At least one context doesn't flow impersonation, {checkResult.Count(check => !check)} of {checkResult.Count}");

        Assert.Equal(customTaskScheduler.ThreadsData.Count, customTaskScheduler.ThreadsData.Select(td => td.Item2).Distinct().Count());

        // Verify if all thread are clean
        foreach ((string afterInvocationThreadIdentity, int threadId) afterInvocationThreadIdentity in customTaskScheduler.ThreadsData)
        {
            // Temporary disabled because could be meaningless
            // Thread could return to the pool with dirty token on netcoreapp?Or ThreadPool cleanup token before reuse?
            // Assert.Equal(current, afterInvocationThreadIdentity);
        }
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public async Task Impersonate_FlowExecutionContext_WindowsImpersonationContext_Nested()
    {
        using (WindowsImpersonationContext windowsImpersonationContext = WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
        {
            Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
            await Task.Run(async () =>
            {
                Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);

                using (WindowsImpersonationContext windowsImpersonationContext2 = WindowsIdentity.Impersonate(_fixture.TestAccount2.AccountTokenHandle.DangerousGetHandle()))
                {
                    Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);

                    await Task.Run(() =>
                    {
                        Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
                    });

                    Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
                }

                Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
            });
        }
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
    public async Task Impersonate_FlowExecutionContext_WindowsImpersonationContextRunImpersonate_Nested()
    {
        using (WindowsImpersonationContext windowsImpersonationContext = WindowsIdentity.Impersonate(_fixture.TestAccount1.AccountTokenHandle.DangerousGetHandle()))
        {
            Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
            await Task.Run(async () =>
            {
                Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);

                await WindowsIdentity.RunImpersonated(_fixture.TestAccount2.AccountTokenHandle, async () =>
                {
                    Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);

                    await Task.Run(() =>
                    {
                        Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
                    });

                    Assert.Equal(_fixture.TestAccount2.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
                });

                Assert.Equal(_fixture.TestAccount1.AccountName, InteropHelper.GetCurrentUserName(), ignoreCase: true);
            });
        }
    }
}

// For context flow tests we use custom task scheduler to be sure that 
// we're using "clean identity token thread". 
// If we do bug it's possible return impersonated thread to thread pool
// and pick up the same for continuation task generating false positive.
public class CustomTaskScheduler : TaskScheduler
{
    public ConcurrentBag<(string, int)> ThreadsData { get; } = new ConcurrentBag<(string, int)>();

    private volatile int _outstandingThread = 0;

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        throw new NotImplementedException();
    }

    protected override bool TryDequeue(Task task)
    {
        throw new NotImplementedException();
    }

    protected override void QueueTask(Task task)
    {
        Interlocked.Increment(ref _outstandingThread);
        var thread = new Thread(_ =>
        {
            TryExecuteTask(task);

            // we verify if thread is "clean"
            ThreadsData.Add((InteropHelper.GetCurrentUserName(), Thread.CurrentThread.ManagedThreadId));

            Interlocked.Decrement(ref _outstandingThread);
        });
        thread.Start();
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    public void WaitOutstandingThread()
    {
        while (_outstandingThread > 0)
        {
            Thread.Sleep(100);
        }
    }
}

// We need an helper to get current thread user identity name because WindowsIdentity.Name
// has side effects, it uses `RunImpersonate` that setup a static AsyncLocal state, this local state
// disturb assertions in case of context flow
public static class InteropHelper
{
    public static string GetCurrentUserName()
    {
        SafeAccessTokenHandle current = GetCurrentToken(TokenAccessLevels.MaximumAllowed, false, out bool isImpersonating, out int hr);
        SafeLocalAllocHandle tokenOwner = GetTokenInformation(current, TokenInformationClass.TokenOwner);
        var user = new SecurityIdentifier(tokenOwner.Read<IntPtr>(0));
        NTAccount ntAccount = user.Translate(typeof(NTAccount)) as NTAccount;
        return ntAccount.ToString();
    }

    private static SafeLocalAllocHandle GetTokenInformation(SafeAccessTokenHandle tokenHandle, TokenInformationClass tokenInformationClass, bool nullOnInvalidParam = false)
    {
        SafeLocalAllocHandle safeLocalAllocHandle = SafeLocalAllocHandle.InvalidHandle;
        uint dwLength = sizeof(uint);
        bool result = Interop.Advapi32.GetTokenInformation(tokenHandle,
                                                      (uint)tokenInformationClass,
                                                      safeLocalAllocHandle,
                                                      0,
                                                      out dwLength);
        int dwErrorCode = Marshal.GetLastWin32Error();
        switch (dwErrorCode)
        {
            case Interop.Errors.ERROR_BAD_LENGTH:
            // special case for TokenSessionId. Falling through
            case Interop.Errors.ERROR_INSUFFICIENT_BUFFER:
                // ptrLength is an [In] param to LocalAlloc 
                UIntPtr ptrLength = new UIntPtr(dwLength);
                safeLocalAllocHandle.Dispose();
                safeLocalAllocHandle = Interop.Kernel32.LocalAlloc(0, ptrLength);
                if (safeLocalAllocHandle == null || safeLocalAllocHandle.IsInvalid)
                {
                    throw new OutOfMemoryException();
                }

                safeLocalAllocHandle.Initialize(dwLength);

                result = Interop.Advapi32.GetTokenInformation(tokenHandle,
                                                         (uint)tokenInformationClass,
                                                         safeLocalAllocHandle,
                                                         dwLength,
                                                         out dwLength);
                if (!result)
                {
                    throw new SecurityException(new Win32Exception().Message);
                }

                break;
            case Interop.Errors.ERROR_INVALID_HANDLE:
                throw new ArgumentException("Invalid token for impersonation - it cannot be duplicated.");
            case Interop.Errors.ERROR_INVALID_PARAMETER:
                if (nullOnInvalidParam)
                {
                    safeLocalAllocHandle.Dispose();
                    return null;
                }

                // Throw the exception.
                goto default;
            default:
                throw new SecurityException(new Win32Exception(dwErrorCode).Message);
        }
        return safeLocalAllocHandle;
    }

    private enum TokenInformationClass : int
    {
        TokenOwner = 4
    }

    private static SafeAccessTokenHandle GetCurrentToken(TokenAccessLevels desiredAccess, bool threadOnly, out bool isImpersonating, out int hr)
    {
        isImpersonating = true;
        hr = 0;
        bool success = Interop.Advapi32.OpenThreadToken(desiredAccess, WinSecurityContext.Both, out SafeAccessTokenHandle safeTokenHandle);
        if (!success)
        {
            hr = Marshal.GetHRForLastWin32Error();
        }

        if (!success && hr == GetHRForWin32Error(Interop.Errors.ERROR_NO_TOKEN))
        {
            // No impersonation
            isImpersonating = false;
            if (!threadOnly)
            {
                safeTokenHandle = GetCurrentProcessToken(desiredAccess, out hr);
            }
        }
        return safeTokenHandle;
    }

    private static SafeAccessTokenHandle GetCurrentProcessToken(TokenAccessLevels desiredAccess, out int hr)
    {
        hr = 0;
        if (!Interop.Advapi32.OpenProcessToken(Interop.Kernel32.GetCurrentProcess(), desiredAccess, out SafeAccessTokenHandle safeTokenHandle))
        {
            hr = GetHRForWin32Error(Marshal.GetLastWin32Error());
        }

        return safeTokenHandle;
    }

    private static int GetHRForWin32Error(int dwLastError)
    {
        if ((dwLastError & 0x80000000) == 0x80000000)
        {
            return dwLastError;
        }
        else
        {
            return (dwLastError & 0x0000FFFF) | unchecked((int)0x80070000);
        }
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

            USER_INFO_1 userInfo = new USER_INFO_1
            {
                usri1_name = _userName,
                usri1_password = testAccountPassword,
                usri1_priv = 1
            };

            // Create user and remove/create if already exists
            uint result = NetUserAdd(null, 1, ref userInfo, out uint param_err);

            // error codes https://docs.microsoft.com/en-us/windows/desktop/netmgmt/network-management-error-codes
            // 0 == NERR_Success
            if (result == 2224) // NERR_UserExists
            {
                result = NetUserDel(null, userInfo.usri1_name);
                if (result != 0)
                {
                    throw new Win32Exception((int)result);
                }
                result = NetUserAdd(null, 1, ref userInfo, out param_err);
                if (result != 0)
                {
                    throw new Win32Exception((int)result);
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

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string userName, string domain, string password, int logonType, int logonProvider, out SafeAccessTokenHandle safeAccessTokenHandle);

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern uint NetUserAdd([MarshalAs(UnmanagedType.LPWStr)]string servername, uint level, ref USER_INFO_1 buf, out uint parm_err);

    [DllImport("netapi32.dll")]
    internal static extern uint NetUserDel([MarshalAs(UnmanagedType.LPWStr)]string servername, [MarshalAs(UnmanagedType.LPWStr)]string username);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct USER_INFO_1
    {
        public string usri1_name;
        public string usri1_password;
        public uint usri1_password_age;
        public uint usri1_priv;
        public string usri1_home_dir;
        public string usri1_comment;
        public uint usri1_flags;
        public string usri1_script_path;
    }

    public void Dispose()
    {
        if (_accountTokenHandle == null)
        {
            return;
        }

        _accountTokenHandle.Dispose();
        _accountTokenHandle = null;

        uint result = NetUserDel(null, _userName);
        if (result != 0)
        {
            throw new Win32Exception((int)result);
        }
    }
}
