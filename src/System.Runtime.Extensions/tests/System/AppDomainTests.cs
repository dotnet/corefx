// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.ExceptionServices;
using Xunit;

namespace System.Tests
{
    public partial class AppDomainTests : RemoteExecutorTestBase
    {
        [Fact]
        public void CurrentDomain_Not_Null()
        {
            Assert.NotNull(AppDomain.CurrentDomain);
        }

        [Fact]
        public void CurrentDomain_Idempotent()
        {
            Assert.Equal(AppDomain.CurrentDomain, AppDomain.CurrentDomain);
        }

        [Fact]
        public void BaseDirectory_Same_As_AppContext()
        {
            Assert.Equal(AppDomain.CurrentDomain.BaseDirectory, AppContext.BaseDirectory);
        } 

        [Fact]
        public void RelativeSearchPath_Is_Null()
        {
            Assert.Null(AppDomain.CurrentDomain.RelativeSearchPath);
        } 

        [Fact]
        public void TargetFrameworkTest()
        {
            // On Uap we use the Microsoft.DotNet.XUnitRunnerUap instead of the RemoteExecutorConsoleApp
            string targetFrameworkName = PlatformDetection.IsUap ? ".NETCore,Version=v5.0" : "DUMMY-TFA";
            RemoteInvoke((_targetFrameworkName) => {
                Assert.Contains(_targetFrameworkName, AppContext.TargetFrameworkName);
            }, targetFrameworkName).Dispose();
        }

        [Fact]
        public void UnhandledException_Add_Remove()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
                AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(MyHandler);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void UnhandledException_NotCalled_When_Handled()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(NotExpectedToBeCalledHandler);
                try
                {
                    throw new Exception();
                }
                catch
                {
                }
                AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(NotExpectedToBeCalledHandler);
                return SuccessExitCode;
            }).Dispose();
        }

        [ActiveIssue(12716)]
        [PlatformSpecific(~TestPlatforms.OSX)] // Unhandled exception on a separate process causes xunit to crash on osx
        [Fact]
        public void UnhandledException_Called()
        {
            System.IO.File.Delete("success.txt");
            RemoteInvokeOptions options = new RemoteInvokeOptions();
            options.CheckExitCode = false;
            RemoteInvoke(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
                throw new Exception("****This Unhandled Exception is Expected****");
#pragma warning disable 0162
                    return SuccessExitCode;
#pragma warning restore 0162
            }, options).Dispose();

            Assert.True(System.IO.File.Exists("success.txt"));
        }

        static void NotExpectedToBeCalledHandler(object sender, UnhandledExceptionEventArgs args) 
        {
            Assert.True(false, "UnhandledException handler not expected to be called");
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args) 
        {
            File.Create("success.txt");
        }

        [Fact]
        public void DynamicDirectory_Null()
        {
            Assert.Null(AppDomain.CurrentDomain.DynamicDirectory);
        }

        [Fact]
        public void FriendlyName()
        {
            string s = AppDomain.CurrentDomain.FriendlyName;
            Assert.NotNull(s);
            string expected = Assembly.GetEntryAssembly()?.GetName()?.Name;

            // GetEntryAssembly may be null (i.e. desktop)
            if (expected == null || PlatformDetection.IsFullFramework)
                expected = Assembly.GetExecutingAssembly().GetName().Name;

            Assert.Equal(expected, s);
        }

        [Fact]
        public void Id()
        {
            // if running directly on some platforms Xunit may be Id = 1
            RemoteInvoke(() => {
                Assert.Equal(1, AppDomain.CurrentDomain.Id);
                return SuccessExitCode;
            }).Dispose();
        }        

        [Fact]
        public void IsFullyTrusted()
        {
            Assert.True(AppDomain.CurrentDomain.IsFullyTrusted);
        }        

        [Fact]
        public void IsHomogenous()
        {
            Assert.True(AppDomain.CurrentDomain.IsHomogenous);
        }        

        [Fact]
        public void FirstChanceException_Add_Remove()
        {
            RemoteInvoke(() => {
                EventHandler<FirstChanceExceptionEventArgs> handler = (sender, e) =>
                {
                };
                AppDomain.CurrentDomain.FirstChanceException += handler;
                AppDomain.CurrentDomain.FirstChanceException -= handler;
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void FirstChanceException_Called()
        {
            RemoteInvoke(() => {
                bool flag = false;
                EventHandler<FirstChanceExceptionEventArgs> handler = (sender, e) =>
                {
                    Exception ex = e.Exception;
                    if (ex is FirstChanceTestException)
                    {
                        flag = !flag;
                    }
                };
                AppDomain.CurrentDomain.FirstChanceException += handler;
                try
                {
                    throw new FirstChanceTestException("testing");
                }
                catch
                {
                }
                AppDomain.CurrentDomain.FirstChanceException -= handler;
                Assert.True(flag, "FirstChanceHandler not called");
                return SuccessExitCode;
            }).Dispose();
        }

        class FirstChanceTestException : Exception
        {
            public FirstChanceTestException(string message) : base(message) 
            { }
        }

        [Fact]
        public void ProcessExit_Add_Remove()
        {
            RemoteInvoke(() => {
                EventHandler handler = (sender, e) =>
                {
                };
                AppDomain.CurrentDomain.ProcessExit += handler;
                AppDomain.CurrentDomain.ProcessExit -= handler;
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/21410", TargetFrameworkMonikers.Uap)]
        public void ProcessExit_Called()
        {
            string path = GetTestFilePath();
            RemoteInvoke((pathToFile) =>
            {
                EventHandler handler = (sender, e) =>
                {
                    File.Create(pathToFile);
                };

                AppDomain.CurrentDomain.ProcessExit += handler;
                return SuccessExitCode;
            }, path).Dispose();

            Assert.True(File.Exists(path));
        }

        [Fact]
        public void ApplyPolicy()
        {
            AssertExtensions.Throws<ArgumentNullException>("assemblyName", () => { AppDomain.CurrentDomain.ApplyPolicy(null); });
            AssertExtensions.Throws<ArgumentException>(null, () => { AppDomain.CurrentDomain.ApplyPolicy(""); });
            string entryAssembly = Assembly.GetEntryAssembly()?.FullName ?? Assembly.GetExecutingAssembly().FullName;
            Assert.Equal(AppDomain.CurrentDomain.ApplyPolicy(entryAssembly), entryAssembly);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void CreateDomainNonNetfx()
        {
            AssertExtensions.Throws<ArgumentNullException>("friendlyName", () => { AppDomain.CreateDomain(null); });
            Assert.Throws<PlatformNotSupportedException>(() => { AppDomain.CreateDomain("test"); });
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework)]
        public void CreateDomainNetfx()
        {
            Assert.Throws<ArgumentNullException>(() => { AppDomain.CreateDomain(null); });
            AppDomain.CreateDomain("test");
        }

        [Fact]
        [ActiveIssue(21680, TargetFrameworkMonikers.UapAot)]
        public void ExecuteAssemblyByName()
        {
            RemoteInvoke(() => {
                string name = "TestApp";
                var assembly = Assembly.Load(name);
                Assert.Equal(5, AppDomain.CurrentDomain.ExecuteAssemblyByName(assembly.FullName));
                Assert.Equal(10, AppDomain.CurrentDomain.ExecuteAssemblyByName(assembly.FullName, new string[2] { "2", "3" }));
                Assert.Throws<FormatException>(() => AppDomain.CurrentDomain.ExecuteAssemblyByName(assembly.FullName, new string[1] { "a" }));
                AssemblyName assemblyName = assembly.GetName();
                assemblyName.CodeBase = null;
                Assert.Equal(105, AppDomain.CurrentDomain.ExecuteAssemblyByName(assemblyName, new string[3] { "50", "25", "25" }));
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/18718", TargetFrameworkMonikers.Uap)] // Need to copy files out of execution directory
        public void ExecuteAssembly()
        {
            CopyTestAssemblies();

            string name = Path.Combine(Environment.CurrentDirectory, "TestAppOutsideOfTPA.exe");
            AssertExtensions.Throws<ArgumentNullException>("assemblyFile", () => AppDomain.CurrentDomain.ExecuteAssembly(null));
            Assert.Throws<FileNotFoundException>(() => AppDomain.CurrentDomain.ExecuteAssembly("NonExistentFile.exe"));

            Func<int> executeAssembly = () => AppDomain.CurrentDomain.ExecuteAssembly(name, new string[2] { "2", "3" }, null, Configuration.Assemblies.AssemblyHashAlgorithm.SHA1);

            if (PlatformDetection.IsFullFramework)
                Assert.Equal(10, executeAssembly());
            else
                Assert.Throws<PlatformNotSupportedException>(() => executeAssembly());

            Assert.Equal(5, AppDomain.CurrentDomain.ExecuteAssembly(name));
            Assert.Equal(10, AppDomain.CurrentDomain.ExecuteAssembly(name, new string[2] { "2", "3" }));
        }        

        [Fact]
        public void GetData_SetData()
        {
            RemoteInvoke(() => {
                AssertExtensions.Throws<ArgumentNullException>("name", () => { AppDomain.CurrentDomain.SetData(null, null); });
                AppDomain.CurrentDomain.SetData("", null);
                Assert.Null(AppDomain.CurrentDomain.GetData(""));
                AppDomain.CurrentDomain.SetData("randomkey", 4);
                Assert.Equal(4, AppDomain.CurrentDomain.GetData("randomkey"));
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void SetData_SameKeyMultipleTimes_ReplacesOldValue()
        {
            RemoteInvoke(() => {
                string key = Guid.NewGuid().ToString("N");
                for (int i = 0; i < 3; i++)
                {
                    AppDomain.CurrentDomain.SetData(key, i.ToString());
                    Assert.Equal(i.ToString(), AppDomain.CurrentDomain.GetData(key));
                }
                AppDomain.CurrentDomain.SetData(key, null);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Netfx is more permissive and does not throw")]
        public void IsCompatibilitySwitchSet()
        {
            Assert.Throws<ArgumentNullException>(() => { AppDomain.CurrentDomain.IsCompatibilitySwitchSet(null); });
            AssertExtensions.Throws<ArgumentException>("switchName", () => { AppDomain.CurrentDomain.IsCompatibilitySwitchSet("");});
            Assert.Null(AppDomain.CurrentDomain.IsCompatibilitySwitchSet("randomSwitch"));
        }

        [Fact]
        public void IsDefaultAppDomain()
        {
            // Xunit may be default app domain if run directly
            RemoteInvoke(() =>
            {
                Assert.True(AppDomain.CurrentDomain.IsDefaultAppDomain());
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void IsFinalizingForUnload()
        {
            Assert.False(AppDomain.CurrentDomain.IsFinalizingForUnload());
        }

        [Fact]
        public void toString()
        {
            // Workaround issue: UWP culture is process wide
            RemoteInvoke(() =>
            {
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

                string actual = AppDomain.CurrentDomain.ToString();

                // NetFx has additional line endings
                if (PlatformDetection.IsFullFramework)
                    actual = actual.Trim();

                string expected = "Name:" + AppDomain.CurrentDomain.FriendlyName + Environment.NewLine + "There are no context policies.";
                Assert.Equal(expected, actual);

            }).Dispose();
        }

        [Fact]
        public void Unload()
        {
            RemoteInvoke(() => {
                AssertExtensions.Throws<ArgumentNullException>("domain", () => { AppDomain.Unload(null); });
                Assert.Throws<CannotUnloadAppDomainException>(() => { AppDomain.Unload(AppDomain.CurrentDomain); });
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void Load()
        {
            AssemblyName assemblyName = typeof(AppDomainTests).Assembly.GetName();
            assemblyName.CodeBase = null;
            Assert.NotNull(AppDomain.CurrentDomain.Load(assemblyName));
            Assert.NotNull(AppDomain.CurrentDomain.Load(typeof(AppDomainTests).Assembly.FullName));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Does not support Assembly.Load(byte[])")] 
        public void LoadBytes()
        {
            Assembly assembly = typeof(AppDomainTests).Assembly;
            byte[] aBytes = System.IO.File.ReadAllBytes(assembly.Location);
            Assert.NotNull(AppDomain.CurrentDomain.Load(aBytes));
        }

        [Fact]
        public void ReflectionOnlyGetAssemblies()
        {
            Assert.Equal(0, AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().Length);
        }

        [Fact]
        public void MonitoringIsEnabled()
        {
            RemoteInvoke(() => {
                Assert.False(AppDomain.MonitoringIsEnabled);
                Assert.Throws<ArgumentException>(() => { AppDomain.MonitoringIsEnabled = false; });

                if (PlatformDetection.IsFullFramework)
                {
                    AppDomain.MonitoringIsEnabled = true;
                    Assert.True(AppDomain.MonitoringIsEnabled);
                }
                else
                {
                    Assert.Throws<PlatformNotSupportedException>(() => { AppDomain.MonitoringIsEnabled = true; });
                }
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void MonitoringSurvivedMemorySize()
        {
            Assert.Throws<InvalidOperationException>(() => { var t = AppDomain.CurrentDomain.MonitoringSurvivedMemorySize; });
        }

        [Fact]
        public void MonitoringSurvivedProcessMemorySize()
        {
            Assert.Throws<InvalidOperationException>(() => { var t = AppDomain.MonitoringSurvivedProcessMemorySize; });
        }

        [Fact]
        public void MonitoringTotalAllocatedMemorySize()
        {
            Assert.Throws<InvalidOperationException>(() => {
                var t = AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
            });
        }

        [Fact]
        public void MonitoringTotalProcessorTime()
        {
            Assert.Throws<InvalidOperationException>(() => {
                var t = AppDomain.CurrentDomain.MonitoringTotalProcessorTime;
            });
        }

#pragma warning disable 618
        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void GetCurrentThreadId()
        {
            Assert.Equal(AppDomain.GetCurrentThreadId(), Environment.CurrentManagedThreadId);
        }

        [Fact]
        public void ShadowCopyFiles()
        {
            Assert.False(AppDomain.CurrentDomain.ShadowCopyFiles);
        }

        [Fact]
        public void AppendPrivatePath()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.AppendPrivatePath("test");
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void ClearPrivatePath()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.ClearPrivatePath();
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void ClearShadowCopyPath()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.ClearShadowCopyPath();
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void SetCachePath()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.SetCachePath("test");
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void SetShadowCopyFiles()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.SetShadowCopyFiles();
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void SetShadowCopyPath()
        {
            RemoteInvoke(() => {
                AppDomain.CurrentDomain.SetShadowCopyPath("test");
                return SuccessExitCode;
            }).Dispose();
        }

#pragma warning restore 618
        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Does not support Assembly.LoadFile")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void GetAssemblies()
        {
            RemoteInvoke(() => {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assert.NotNull(assemblies);
                Assert.True(assemblies.Length > 0, "There must be assemblies already loaded in the process");
                AppDomain.CurrentDomain.Load(typeof(AppDomainTests).Assembly.GetName().FullName);
                Assembly[] assemblies1 = AppDomain.CurrentDomain.GetAssemblies();
                // Another thread could have loaded an assembly hence not checking for equality
                Assert.True(assemblies1.Length >= assemblies.Length, "Assembly.Load of an already loaded assembly should not cause another load");
                Assembly.LoadFile(typeof(AppDomain).Assembly.Location);
                Assembly[] assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
                Assert.True(assemblies2.Length > assemblies.Length, "Assembly.LoadFile should cause an increase in GetAssemblies list");
                int ctr = 0;
                foreach (var a in assemblies2)
                {
                    // Dynamic assemblies do not support Location property.
                    if (!a.IsDynamic)
                    {
                        if (a.Location == typeof(AppDomain).Assembly.Location)
                            ctr++;
                    }
                }
                foreach (var a in assemblies)
                {
                    if (!a.IsDynamic)
                    {
                        if (a.Location == typeof(AppDomain).Assembly.Location)
                            ctr--;
                    }
                }
                Assert.True(ctr > 0, "Assembly.LoadFile should cause file to be loaded again");
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Does not support Assembly.LoadFile")]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void AssemblyLoad()
        {
            RemoteInvoke(() => {
                bool AssemblyLoadFlag = false;
                AssemblyLoadEventHandler handler = (sender, args) =>
                {
                    if (args.LoadedAssembly.FullName.Equals(typeof(AppDomainTests).Assembly.FullName))
                    {
                        AssemblyLoadFlag = !AssemblyLoadFlag;
                    }
                };

                AppDomain.CurrentDomain.AssemblyLoad += handler;

                try
                {
                    Assembly.LoadFile(typeof(AppDomainTests).Assembly.Location);
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyLoad -= handler;
                }
                Assert.True(AssemblyLoadFlag);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/18718", TargetFrameworkMonikers.Uap)] // Need to copy files out of execution directory'
        public void AssemblyResolve()
        {
            CopyTestAssemblies();

            RemoteInvoke(() => {
                ResolveEventHandler handler = (sender, e) =>
                {
                    return Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "AssemblyResolveTests.exe"));
                };

                AppDomain.CurrentDomain.AssemblyResolve += handler;

                Type t = Type.GetType("AssemblyResolveTests.Class1, AssemblyResolveTests", true);
                Assert.NotNull(t);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue("https://github.com/dotnet/corefx/issues/18718", TargetFrameworkMonikers.Uap)] // Need to copy files out of execution directory
        public void AssemblyResolve_RequestingAssembly()
        {
            CopyTestAssemblies();

            RemoteInvoke(() => {
                Assembly a = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "TestAppOutsideOfTPA.exe"));

                ResolveEventHandler handler = (sender, e) =>
                {
                    Assert.Equal(e.RequestingAssembly, a);
                    return Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "AssemblyResolveTests.exe"));
                };

                AppDomain.CurrentDomain.AssemblyResolve += handler;
                Type ptype = a.GetType("Program");
                MethodInfo myMethodInfo = ptype.GetMethod("foo");
                object ret = myMethodInfo.Invoke(null, null);
                Assert.NotNull(ret);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void AssemblyResolve_IsNotCalledForCoreLibResources()
        {
            RemoteInvoke(() =>
            {
                bool assemblyResolveHandlerCalled = false;
                AppDomain.CurrentDomain.AssemblyResolve +=
                    (sender, e) =>
                    {
                        // This implementation violates the contract. AssemblyResolve event handler is supposed to return an assembly
                        // that matches the requested identity and that is not the case here.
                        assemblyResolveHandlerCalled = true;
                        return typeof(AppDomainTests).Assembly;
                    };

                CultureInfo previousUICulture = CultureInfo.CurrentUICulture;
                CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
                try
                {
                    // The resource lookup for NullReferenceException (generally for CoreLib resources) should not raise the
                    // AssemblyResolve event because a misbehaving handler could cause an infinite recursion check and fail-fast to
                    // be triggered when the resource is not found, as the issue would repeat when reporting that error.
                    Assert.Throws<NullReferenceException>(() => ((string)null).Contains("a"));
                    Assert.False(assemblyResolveHandlerCalled);
                }
                finally
                {
                    CultureInfo.CurrentUICulture = previousUICulture;
                }

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue(21680, TargetFrameworkMonikers.UapAot)]
        public void TypeResolve()
        {
            RemoteInvoke(() => {
                Assert.Throws<TypeLoadException>(() => Type.GetType("Program", true));

                ResolveEventHandler handler = (sender, args) =>
                {
                    return Assembly.Load("TestApp");
                };

                AppDomain.CurrentDomain.TypeResolve += handler;

                Type t;
                try
                {
                    t = Type.GetType("Program", true);
                }
                finally
                {
                    AppDomain.CurrentDomain.TypeResolve -= handler;
                }
                Assert.NotNull(t);
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        [ActiveIssue(21680, TargetFrameworkMonikers.UapAot)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapNotUapAot, "In UWP the resources always exist in the resources.pri file even if the assembly is not loaded")]
        public void ResourceResolve()
        {
            RemoteInvoke(() => {
                ResourceManager res = new ResourceManager(typeof(FxResources.TestApp.SR));
                Assert.Throws<MissingManifestResourceException>(() => res.GetString("Message"));

                ResolveEventHandler handler = (sender, args) =>
                {
                    return Assembly.Load("TestApp");
                };

                AppDomain.CurrentDomain.ResourceResolve += handler;

                String s;
                try
                {
                    s = res.GetString("Message");
                }
                finally
                {
                    AppDomain.CurrentDomain.ResourceResolve -= handler;
                }
                Assert.Equal(s, "Happy Halloween");
                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]       
        public void SetThreadPrincipal()
        {
            RemoteInvoke(() => {
                Assert.Throws<ArgumentNullException>(() => { AppDomain.CurrentDomain.SetThreadPrincipal(null); });
                var identity = new System.Security.Principal.GenericIdentity("NewUser");
                var principal = new System.Security.Principal.GenericPrincipal(identity, null);
                AppDomain.CurrentDomain.SetThreadPrincipal(principal);
                return SuccessExitCode;
            }).Dispose();
        }

        private void CopyTestAssemblies()
        {
            string destTestAssemblyPath = Path.Combine(Environment.CurrentDirectory, "AssemblyResolveTests", "AssemblyResolveTests.dll");
            if (!File.Exists(destTestAssemblyPath) && File.Exists("AssemblyResolveTests.dll"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destTestAssemblyPath));
                File.Copy("AssemblyResolveTests.dll", destTestAssemblyPath, false);
            }

            destTestAssemblyPath = Path.Combine(Environment.CurrentDirectory, "TestAppOutsideOfTPA", "TestAppOutsideOfTPA.exe");
            if (!File.Exists(destTestAssemblyPath) && File.Exists("TestAppOutsideOfTPA.exe"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destTestAssemblyPath));
                File.Copy("TestAppOutsideOfTPA.exe", destTestAssemblyPath, false);
            }
        }        
    }
}

namespace FxResources.TestApp
{
    class SR { }
}
