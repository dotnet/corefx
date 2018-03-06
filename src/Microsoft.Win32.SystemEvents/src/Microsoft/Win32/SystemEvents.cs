// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Win32
{
    using System;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Text;
    using System.Threading;

    /// <devdoc>
    ///    <para> 
    ///       Provides a
    ///       set of global system events to callers. This
    ///       class cannot be inherited.</para>
    /// </devdoc>
    [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    public sealed class SystemEvents
    {
        // Almost all of our data is static.  We keep a single instance of
        // SystemEvents around so we can bind delegates to it.
        // Non-static methods in this class will only be called through
        // one of the delegates.
        private static readonly object s_eventLockObject = new object();
        private static readonly object s_procLockObject = new object();
        private static volatile SystemEvents s_systemEvents;
        private static volatile Thread s_windowThread;
        private static volatile ManualResetEvent s_eventWindowReady;
        private static Random s_randomTimerId = new Random();
        private static volatile bool s_registeredSessionNotification = false;
        private static volatile int s_domainQualifier;
        private static volatile Interop.User32.WNDCLASS s_staticwndclass;
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private static volatile IntPtr s_defWindowProc;

        private static volatile string s_className = null;

        // cross-thread marshaling
        private static volatile Queue<Delegate> s_threadCallbackList; // list of Delegates
        private static volatile int s_threadCallbackMessage = 0;
        private static volatile ManualResetEvent s_eventThreadTerminated;

        // Per-instance data that is isolated to the window thread.
        private volatile IntPtr _windowHandle;
        private Interop.User32.WndProc _windowProc;
        private Interop.Kernel32.ConsoleCtrlHandlerRoutine _consoleHandler;

        // The set of events we respond to.  
        private static readonly object s_onUserPreferenceChangingEvent = new object();
        private static readonly object s_onUserPreferenceChangedEvent = new object();
        private static readonly object s_onSessionEndingEvent = new object();
        private static readonly object s_onSessionEndedEvent = new object();
        private static readonly object s_onPowerModeChangedEvent = new object();
        private static readonly object s_onLowMemoryEvent = new object();
        private static readonly object s_onDisplaySettingsChangingEvent = new object();
        private static readonly object s_onDisplaySettingsChangedEvent = new object();
        private static readonly object s_onInstalledFontsChangedEvent = new object();
        private static readonly object s_onTimeChangedEvent = new object();
        private static readonly object s_onTimerElapsedEvent = new object();
        private static readonly object s_onPaletteChangedEvent = new object();
        private static readonly object s_onEventsThreadShutdownEvent = new object();
        private static readonly object s_onSessionSwitchEvent = new object();


        // Our list of handler information.  This is a lookup of the above keys and objects that
        // match a delegate with a SyncronizationContext so we can fire on the proper thread.
        private static Dictionary<object, List<SystemEventInvokeInfo>> s_handlers;


        /// <devdoc>
        ///     This class is static, there is no need to ever create it.
        /// </devdoc>
        private SystemEvents()
        {
        }


        // stole from SystemInformation... if we get SystemInformation moved
        // to somewhere that we can use it... rip this!
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private static volatile IntPtr s_processWinStation = IntPtr.Zero;
        private static volatile bool s_isUserInteractive = false;
        private static bool UserInteractive
        {
            get
            {
                if (Environment.OSVersion.Platform == System.PlatformID.Win32NT)
                {
                    IntPtr hwinsta = IntPtr.Zero;

                    hwinsta = Interop.User32.GetProcessWindowStation();
                    if (hwinsta != IntPtr.Zero && s_processWinStation != hwinsta)
                    {
                        s_isUserInteractive = true;

                        int lengthNeeded = 0;
                        Interop.User32.USEROBJECTFLAGS flags = new Interop.User32.USEROBJECTFLAGS();

                        if (Interop.User32.GetUserObjectInformationW(hwinsta, Interop.User32.UOI_FLAGS, flags, Marshal.SizeOf(flags), ref lengthNeeded))
                        {
                            if ((flags.dwFlags & Interop.User32.WSF_VISIBLE) == 0)
                            {
                                s_isUserInteractive = false;
                            }
                        }
                        s_processWinStation = hwinsta;
                    }
                }
                else
                {
                    s_isUserInteractive = true;
                }
                return s_isUserInteractive;
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the display settings are changing.</para>
        /// </devdoc>
        public static event EventHandler DisplaySettingsChanging
        {
            add
            {
                AddEventHandler(s_onDisplaySettingsChangingEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onDisplaySettingsChangingEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user changes the display settings.</para>
        /// </devdoc>
        public static event EventHandler DisplaySettingsChanged
        {
            add
            {
                AddEventHandler(s_onDisplaySettingsChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onDisplaySettingsChangedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs before the thread that listens for system events is terminated.
        ///           Delegates will be invoked on the events thread.</para>
        /// </devdoc>
        public static event EventHandler EventsThreadShutdown
        {
            // Really only here for GDI+ initialization and shut down
            add
            {
                AddEventHandler(s_onEventsThreadShutdownEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onEventsThreadShutdownEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user adds fonts to or removes fonts from the system.</para>
        /// </devdoc>
        public static event EventHandler InstalledFontsChanged
        {
            add
            {
                AddEventHandler(s_onInstalledFontsChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onInstalledFontsChangedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the system is running out of available RAM.</para>
        /// </devdoc>
        [Obsolete("This event has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler LowMemory
        {
            add
            {
                EnsureSystemEvents(true, true);
                AddEventHandler(s_onLowMemoryEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onLowMemoryEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user switches to an application that uses a different 
        ///       palette.</para>
        /// </devdoc>
        public static event EventHandler PaletteChanged
        {
            add
            {
                AddEventHandler(s_onPaletteChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onPaletteChangedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user suspends or resumes the system.</para>
        /// </devdoc>
        public static event PowerModeChangedEventHandler PowerModeChanged
        {
            add
            {
                EnsureSystemEvents(true, true);
                AddEventHandler(s_onPowerModeChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onPowerModeChangedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user is logging off or shutting down the system.</para>
        /// </devdoc>
        public static event SessionEndedEventHandler SessionEnded
        {
            add
            {
                EnsureSystemEvents(true, false);
                AddEventHandler(s_onSessionEndedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onSessionEndedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user is trying to log off or shutdown the system.</para>
        /// </devdoc>
        public static event SessionEndingEventHandler SessionEnding
        {
            add
            {
                EnsureSystemEvents(true, false);
                AddEventHandler(s_onSessionEndingEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onSessionEndingEvent, value);
            }
        }

        /// <devdoc>
        ///    <para>Occurs when a user session switches.</para>
        /// </devdoc>
        public static event SessionSwitchEventHandler SessionSwitch
        {
            add
            {
                EnsureSystemEvents(true, true);
                EnsureRegisteredSessionNotification();
                AddEventHandler(s_onSessionSwitchEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onSessionSwitchEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when the user changes the time on the system clock.</para>
        /// </devdoc>
        public static event EventHandler TimeChanged
        {
            add
            {
                EnsureSystemEvents(true, false);
                AddEventHandler(s_onTimeChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onTimeChangedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when a windows timer interval has expired.</para>
        /// </devdoc>
        public static event TimerElapsedEventHandler TimerElapsed
        {
            add
            {
                EnsureSystemEvents(true, false);
                AddEventHandler(s_onTimerElapsedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onTimerElapsedEvent, value);
            }
        }


        /// <devdoc>
        ///    <para>Occurs when a user preference has changed.</para>
        /// </devdoc>
        public static event UserPreferenceChangedEventHandler UserPreferenceChanged
        {
            add
            {
                AddEventHandler(s_onUserPreferenceChangedEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onUserPreferenceChangedEvent, value);
            }
        }

        /// <devdoc>
        ///    <para>Occurs when a user preference is changing.</para>
        /// </devdoc>
        public static event UserPreferenceChangingEventHandler UserPreferenceChanging
        {
            add
            {
                AddEventHandler(s_onUserPreferenceChangingEvent, value);
            }
            remove
            {
                RemoveEventHandler(s_onUserPreferenceChangingEvent, value);
            }
        }

        private static void AddEventHandler(object key, Delegate value)
        {
            lock (s_eventLockObject)
            {
                if (s_handlers == null)
                {
                    s_handlers = new Dictionary<object, List<SystemEventInvokeInfo>>();
                    EnsureSystemEvents(false, false);
                }

                List<SystemEventInvokeInfo> invokeItems;

                if (!s_handlers.TryGetValue(key, out invokeItems))
                {
                    invokeItems = new List<SystemEventInvokeInfo>();
                    s_handlers[key] = invokeItems;
                }
                else
                {
                    invokeItems = s_handlers[key];
                }

                invokeItems.Add(new SystemEventInvokeInfo(value));
            }
        }

        /// <devdoc>
        ///      Console handler we add in case we are a console application or a service.
        ///      Without this we will not get end session events.
        /// </devdoc>
        private bool ConsoleHandlerProc(int signalType)
        {
            switch (signalType)
            {
                case Interop.User32.CTRL_LOGOFF_EVENT:
                    OnSessionEnded((IntPtr)1, (IntPtr)Interop.User32.ENDSESSION_LOGOFF);
                    break;

                case Interop.User32.CTRL_SHUTDOWN_EVENT:
                    OnSessionEnded((IntPtr)1, (IntPtr)0);
                    break;
            }

            return false;
        }

        private Interop.User32.WNDCLASS WndClass
        {
            get
            {
                if (s_staticwndclass == null)
                {
                    const string classNameFormat = ".NET-BroadcastEventWindow.{0}.{1}";

                    IntPtr hInstance = Interop.Kernel32.GetModuleHandle(null);

                    s_className = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        classNameFormat,
                        Convert.ToString(AppDomain.CurrentDomain.GetHashCode(), 16),
                        s_domainQualifier);

                    Interop.User32.WNDCLASS tempwndclass = new Interop.User32.WNDCLASS();
                    tempwndclass.hbrBackground = (IntPtr)(Interop.User32.COLOR_WINDOW + 1);
                    tempwndclass.style = 0;

                    _windowProc = new Interop.User32.WndProc(this.WindowProc);
                    tempwndclass.lpszClassName = s_className;
                    tempwndclass.lpfnWndProc = _windowProc;
                    tempwndclass.hInstance = hInstance;
                    s_staticwndclass = tempwndclass;
                }
                return s_staticwndclass;
            }
        }

        private IntPtr DefWndProc
        {
            get
            {
                if (s_defWindowProc == IntPtr.Zero)
                {
                    s_defWindowProc = Interop.Kernel32.GetProcAddress(Interop.Kernel32.GetModuleHandle("user32.dll"), "DefWindowProcW");
                }
                return s_defWindowProc;
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "Only called on a single thread")]
        private void BumpQualifier()
        {
            s_staticwndclass = null;
            s_domainQualifier++;
        }

        /// <include file='doc\SystemEvents.uex' path='docs/doc[@for="SystemEvents.CreateBroadcastWindow"]/*' />
        /// <devdoc>
        ///      Goes through the work to register and create a window.
        /// </devdoc>
        private IntPtr CreateBroadcastWindow()
        {
            // Register the window class.
            Interop.User32.WNDCLASS_I wndclassi = new Interop.User32.WNDCLASS_I();
            IntPtr hInstance = Interop.Kernel32.GetModuleHandle(null);

            if (!Interop.User32.GetClassInfoW(hInstance, WndClass.lpszClassName, wndclassi))
            {
                if (Interop.User32.RegisterClassW(WndClass) == 0)
                {
                    _windowProc = null;
                    Debug.WriteLine("Unable to register broadcast window class: {0}", Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
            }
            else
            {
                // lets double check the wndproc returned by getclassinfo for sentinel value defwndproc.
                if (wndclassi.lpfnWndProc == DefWndProc)
                {
                    // if we are in there, it means className belongs to an unloaded appdomain.
                    short atom = 0;

                    // try to unregister it.
                    if (0 != Interop.User32.UnregisterClassW(WndClass.lpszClassName, Interop.Kernel32.GetModuleHandle(null)))
                    {
                        atom = Interop.User32.RegisterClassW(WndClass);
                    }

                    if (atom == 0)
                    {
                        do
                        {
                            BumpQualifier();
                            atom = Interop.User32.RegisterClassW(WndClass);
                        } while (atom == 0 && Marshal.GetLastWin32Error() == Interop.Errors.ERROR_CLASS_ALREADY_EXISTS);
                    }
                }
            }

            // And create an instance of the window.
            IntPtr hwnd = Interop.User32.CreateWindowExW(
                                                            0,
                                                            WndClass.lpszClassName,
                                                            WndClass.lpszClassName,
                                                            Interop.User32.WS_POPUP,
                                                            0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero,
                                                            hInstance, IntPtr.Zero);
            return hwnd;
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Creates a new window timer asociated with the
        ///       system events window.</para>
        /// </devdoc>
        public static IntPtr CreateTimer(int interval)
        {
            if (interval <= 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidLowBoundArgument, "interval", interval.ToString(System.Threading.Thread.CurrentThread.CurrentCulture), "0"));
            }

            EnsureSystemEvents(true, true);
            IntPtr timerId = Interop.User32.SendMessageW(new HandleRef(s_systemEvents, s_systemEvents._windowHandle),
                                                        Interop.User32.WM_CREATETIMER, (IntPtr)interval, IntPtr.Zero);

            if (timerId == IntPtr.Zero)
            {
                throw new ExternalException(SR.ErrorCreateTimer);
            }
            return timerId;
        }

        private void Dispose()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                if (s_registeredSessionNotification)
                {
                    Interop.Wtsapi32.WTSUnRegisterSessionNotification(new HandleRef(s_systemEvents, s_systemEvents._windowHandle));
                }

                IntPtr handle = _windowHandle;
                _windowHandle = IntPtr.Zero;

                // we check IsWindow because Application may have rudely destroyed our broadcast window.
                // if this were true, we want to unregister the class.
                if (Interop.User32.IsWindow(handle) && DefWndProc != IntPtr.Zero)
                {
                    // set our sentinel value that we will look for upon initialization to indicate
                    // the window class belongs to an unloaded appdomain and therefore should not be used.
                    if (IntPtr.Size == 4)
                    {
                        // In a 32-bit process we must call the non-'ptr' version of these APIs
                        Interop.User32.SetWindowLongW(handle, Interop.User32.GWL_WNDPROC, DefWndProc);
                        Interop.User32.SetClassLongW(handle, Interop.User32.GCL_WNDPROC, DefWndProc);
                    }
                    else
                    {
                        Interop.User32.SetWindowLongPtrW(handle, Interop.User32.GWL_WNDPROC, DefWndProc);
                        Interop.User32.SetClassLongPtrW(handle, Interop.User32.GCL_WNDPROC, DefWndProc);
                    }
                }

                // If DestroyWindow failed, it is because we're being
                // shutdown from another thread.  In this case, locate the
                // DefWindowProc call in User32, set the window proc to call it,
                // and post a WM_CLOSE.  This will close the window from 
                // the correct thread without relying on managed code executing.
                if (Interop.User32.IsWindow(handle) && !Interop.User32.DestroyWindow(handle))
                {
                    Interop.User32.PostMessageW(handle, Interop.User32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    IntPtr hInstance = Interop.Kernel32.GetModuleHandle(null);
                    Interop.User32.UnregisterClassW(s_className, hInstance);
                }
            }

            if (_consoleHandler != null)
            {
                Interop.Kernel32.SetConsoleCtrlHandler(_consoleHandler, false);
                _consoleHandler = null;
            }
        }

        /// <devdoc>
        ///  Creates the static resources needed by 
        ///  system events.
        /// </devdoc>
        private static void EnsureSystemEvents(bool requireHandle, bool throwOnRefusal)
        {
            // The secondary check here is to detect asp.net.  Asp.net uses multiple
            // app domains to field requests and we do not want to gobble up an 
            // additional thread per domain.  So under this scenario SystemEvents
            // becomes a nop.
            if (s_systemEvents == null)
            {
                lock (s_procLockObject)
                {
                    if (s_systemEvents == null)
                    {
                        if (Thread.GetDomain().GetData(".appDomain") != null)
                        {
                            if (throwOnRefusal)
                            {
                                throw new InvalidOperationException(SR.ErrorSystemEventsNotSupported);
                            }
                            return;
                        }

                        // If we are creating system events on a thread declared as STA, then
                        // just share the thread.
                        if (!UserInteractive || Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                        {
                            SystemEvents systemEvents = new SystemEvents();
                            systemEvents.Initialize();

                            // ensure this is initialized last as that will force concurrent threads calling
                            // this method to block until after we've initialized.
                            s_systemEvents = systemEvents;
                        }
                        else
                        {
                            s_eventWindowReady = new ManualResetEvent(false);
                            SystemEvents systemEvents = new SystemEvents();
                            s_windowThread = new Thread(new ThreadStart(systemEvents.WindowThreadProc));
                            s_windowThread.IsBackground = true;
                            s_windowThread.Name = ".NET SystemEvents";
                            s_windowThread.Start();
                            s_eventWindowReady.WaitOne();

                            // ensure this is initialized last as that will force concurrent threads calling
                            // this method to block until after we've initialized.
                            s_systemEvents = systemEvents;
                        }

                        if (requireHandle && s_systemEvents._windowHandle == IntPtr.Zero)
                        {
                            // In theory, it's not the end of the world that
                            // we don't get system events.  Unfortunately, the main reason windowHandle == 0
                            // is CreateWindowEx failed for mysterious reasons, and when that happens,
                            // subsequent (and more important) CreateWindowEx calls also fail.
                            throw new ExternalException(SR.ErrorCreateSystemEvents);
                        }
                    }
                }
            }
        }

        private static void EnsureRegisteredSessionNotification()
        {
            if (!s_registeredSessionNotification)
            {
                IntPtr retval = Interop.Kernel32.LoadLibrary(Interop.Libraries.Wtsapi32);

                if (retval != IntPtr.Zero)
                {
                    Interop.Wtsapi32.WTSRegisterSessionNotification(new HandleRef(s_systemEvents, s_systemEvents._windowHandle), Interop.Wtsapi32.NOTIFY_FOR_THIS_SESSION);
                    s_registeredSessionNotification = true;
                    Interop.Kernel32.FreeLibrary(retval);
                }
            }
        }

        private UserPreferenceCategory GetUserPreferenceCategory(int msg, IntPtr wParam, IntPtr lParam)
        {
            UserPreferenceCategory pref = UserPreferenceCategory.General;

            if (msg == Interop.User32.WM_SETTINGCHANGE)
            {
                if (lParam != IntPtr.Zero && Marshal.PtrToStringUni(lParam).Equals("Policy"))
                {
                    pref = UserPreferenceCategory.Policy;
                }
                else if (lParam != IntPtr.Zero && Marshal.PtrToStringUni(lParam).Equals("intl"))
                {
                    pref = UserPreferenceCategory.Locale;
                }
                else
                {
                    switch ((int)wParam)
                    {
                        case Interop.User32.SPI_SETACCESSTIMEOUT:
                        case Interop.User32.SPI_SETFILTERKEYS:
                        case Interop.User32.SPI_SETHIGHCONTRAST:
                        case Interop.User32.SPI_SETMOUSEKEYS:
                        case Interop.User32.SPI_SETSCREENREADER:
                        case Interop.User32.SPI_SETSERIALKEYS:
                        case Interop.User32.SPI_SETSHOWSOUNDS:
                        case Interop.User32.SPI_SETSOUNDSENTRY:
                        case Interop.User32.SPI_SETSTICKYKEYS:
                        case Interop.User32.SPI_SETTOGGLEKEYS:
                            pref = UserPreferenceCategory.Accessibility;
                            break;

                        case Interop.User32.SPI_SETDESKWALLPAPER:
                        case Interop.User32.SPI_SETFONTSMOOTHING:
                        case Interop.User32.SPI_SETCURSORS:
                        case Interop.User32.SPI_SETDESKPATTERN:
                        case Interop.User32.SPI_SETGRIDGRANULARITY:
                        case Interop.User32.SPI_SETWORKAREA:
                            pref = UserPreferenceCategory.Desktop;
                            break;

                        case Interop.User32.SPI_ICONHORIZONTALSPACING:
                        case Interop.User32.SPI_ICONVERTICALSPACING:
                        case Interop.User32.SPI_SETICONMETRICS:
                        case Interop.User32.SPI_SETICONS:
                        case Interop.User32.SPI_SETICONTITLELOGFONT:
                        case Interop.User32.SPI_SETICONTITLEWRAP:
                            pref = UserPreferenceCategory.Icon;
                            break;

                        case Interop.User32.SPI_SETDOUBLECLICKTIME:
                        case Interop.User32.SPI_SETDOUBLECLKHEIGHT:
                        case Interop.User32.SPI_SETDOUBLECLKWIDTH:
                        case Interop.User32.SPI_SETMOUSE:
                        case Interop.User32.SPI_SETMOUSEBUTTONSWAP:
                        case Interop.User32.SPI_SETMOUSEHOVERHEIGHT:
                        case Interop.User32.SPI_SETMOUSEHOVERTIME:
                        case Interop.User32.SPI_SETMOUSESPEED:
                        case Interop.User32.SPI_SETMOUSETRAILS:
                        case Interop.User32.SPI_SETSNAPTODEFBUTTON:
                        case Interop.User32.SPI_SETWHEELSCROLLLINES:
                        case Interop.User32.SPI_SETCURSORSHADOW:
                        case Interop.User32.SPI_SETHOTTRACKING:
                        case Interop.User32.SPI_SETTOOLTIPANIMATION:
                        case Interop.User32.SPI_SETTOOLTIPFADE:
                            pref = UserPreferenceCategory.Mouse;
                            break;

                        case Interop.User32.SPI_SETKEYBOARDDELAY:
                        case Interop.User32.SPI_SETKEYBOARDPREF:
                        case Interop.User32.SPI_SETKEYBOARDSPEED:
                        case Interop.User32.SPI_SETLANGTOGGLE:
                            pref = UserPreferenceCategory.Keyboard;
                            break;

                        case Interop.User32.SPI_SETMENUDROPALIGNMENT:
                        case Interop.User32.SPI_SETMENUFADE:
                        case Interop.User32.SPI_SETMENUSHOWDELAY:
                        case Interop.User32.SPI_SETMENUANIMATION:
                        case Interop.User32.SPI_SETSELECTIONFADE:
                            pref = UserPreferenceCategory.Menu;
                            break;

                        case Interop.User32.SPI_SETLOWPOWERACTIVE:
                        case Interop.User32.SPI_SETLOWPOWERTIMEOUT:
                        case Interop.User32.SPI_SETPOWEROFFACTIVE:
                        case Interop.User32.SPI_SETPOWEROFFTIMEOUT:
                            pref = UserPreferenceCategory.Power;
                            break;

                        case Interop.User32.SPI_SETSCREENSAVEACTIVE:
                        case Interop.User32.SPI_SETSCREENSAVERRUNNING:
                        case Interop.User32.SPI_SETSCREENSAVETIMEOUT:
                            pref = UserPreferenceCategory.Screensaver;
                            break;

                        case Interop.User32.SPI_SETKEYBOARDCUES:
                        case Interop.User32.SPI_SETCOMBOBOXANIMATION:
                        case Interop.User32.SPI_SETLISTBOXSMOOTHSCROLLING:
                        case Interop.User32.SPI_SETGRADIENTCAPTIONS:
                        case Interop.User32.SPI_SETUIEFFECTS:
                        case Interop.User32.SPI_SETACTIVEWINDOWTRACKING:
                        case Interop.User32.SPI_SETACTIVEWNDTRKZORDER:
                        case Interop.User32.SPI_SETACTIVEWNDTRKTIMEOUT:
                        case Interop.User32.SPI_SETANIMATION:
                        case Interop.User32.SPI_SETBORDER:
                        case Interop.User32.SPI_SETCARETWIDTH:
                        case Interop.User32.SPI_SETDRAGFULLWINDOWS:
                        case Interop.User32.SPI_SETDRAGHEIGHT:
                        case Interop.User32.SPI_SETDRAGWIDTH:
                        case Interop.User32.SPI_SETFOREGROUNDFLASHCOUNT:
                        case Interop.User32.SPI_SETFOREGROUNDLOCKTIMEOUT:
                        case Interop.User32.SPI_SETMINIMIZEDMETRICS:
                        case Interop.User32.SPI_SETNONCLIENTMETRICS:
                        case Interop.User32.SPI_SETSHOWIMEUI:
                            pref = UserPreferenceCategory.Window;
                            break;
                    }
                }
            }
            else if (msg == Interop.User32.WM_SYSCOLORCHANGE)
            {
                pref = UserPreferenceCategory.Color;
            }
            else
            {
                Debug.Fail("Unrecognized message passed to UserPreferenceCategory");
            }

            return pref;
        }

        private void Initialize()
        {
            _consoleHandler = new Interop.Kernel32.ConsoleCtrlHandlerRoutine(ConsoleHandlerProc);
            if (!Interop.Kernel32.SetConsoleCtrlHandler(_consoleHandler, true))
            {
                Debug.Fail("Failed to install console handler.");
                _consoleHandler = null;
            }

            _windowHandle = CreateBroadcastWindow();
            Debug.WriteLineIf(_windowHandle == IntPtr.Zero, "CreateBroadcastWindow failed");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(SystemEvents.Shutdown);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(SystemEvents.Shutdown);
        }

        /// <devdoc>
        ///     Called on the control's owning thread to perform the actual callback.
        ///     This empties this control's callback queue, propagating any excpetions
        ///     back as needed.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void InvokeMarshaledCallbacks()
        {
            Debug.Assert(s_threadCallbackList != null, "Invoking marshaled callbacks before there are any");

            Delegate current = null;
            lock (s_threadCallbackList)
            {
                if (s_threadCallbackList.Count > 0)
                {
                    current = (Delegate)s_threadCallbackList.Dequeue();
                }
            }

            // Now invoke on all the queued items.
            while (current != null)
            {
                try
                {
                    // Optimize a common case of using EventHandler. This allows us to invoke
                    // early bound, which is a bit more efficient.
                    EventHandler c = current as EventHandler;
                    if (c != null)
                    {
                        c(null, EventArgs.Empty);
                    }
                    else
                    {
                        current.DynamicInvoke(new object[0]);
                    }
                }
                catch (Exception t)
                {
                    Debug.Fail("SystemEvents marshaled callback failed:" + t);
                }
                lock (s_threadCallbackList)
                {
                    if (s_threadCallbackList.Count > 0)
                    {
                        current = (Delegate)s_threadCallbackList.Dequeue();
                    }
                    else
                    {
                        current = null;
                    }
                }
            }
        }

        /// <devdoc>
        ///     Executes the given delegate asynchronously on the thread that listens for system events.  Similar to Control.BeginInvoke().
        /// </devdoc>
        public static void InvokeOnEventsThread(Delegate method)
        {
            // This method is really only here for GDI+ initialization/shutdown
            EnsureSystemEvents(true, true);

#if DEBUG
            int pid;
            int thread = Interop.User32.GetWindowThreadProcessId(new HandleRef(s_systemEvents, s_systemEvents._windowHandle), out pid);
            Debug.Assert(s_windowThread == null || thread != Interop.Kernel32.GetCurrentThreadId(), "Don't call MarshaledInvoke on the system events thread");
#endif

            if (s_threadCallbackList == null)
            {
                lock (s_eventLockObject)
                {
                    if (s_threadCallbackList == null)
                    {
                        s_threadCallbackMessage = Interop.User32.RegisterWindowMessageW("SystemEventsThreadCallbackMessage");
                        s_threadCallbackList = new Queue<Delegate>();
                    }
                }
            }

            Debug.Assert(s_threadCallbackMessage != 0, "threadCallbackList initialized but threadCallbackMessage not?");

            lock (s_threadCallbackList)
            {
                s_threadCallbackList.Enqueue(method);
            }

            Interop.User32.PostMessageW(new HandleRef(s_systemEvents, s_systemEvents._windowHandle), s_threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Kills the timer specified by the given id.</para>
        /// </devdoc>
        public static void KillTimer(IntPtr timerId)
        {
            EnsureSystemEvents(true, true);
            if (s_systemEvents._windowHandle != IntPtr.Zero)
            {
                int res = (int)Interop.User32.SendMessageW(new HandleRef(s_systemEvents, s_systemEvents._windowHandle),
                                                                Interop.User32.WM_KILLTIMER, timerId, IntPtr.Zero);

                if (res == 0)
                    throw new ExternalException(SR.ErrorKillTimer);
            }
        }

        /// <devdoc>
        ///      Callback that handles the create timer
        ///      user message.
        /// </devdoc>
        private IntPtr OnCreateTimer(IntPtr wParam)
        {
            IntPtr timerId = (IntPtr)s_randomTimerId.Next();
            IntPtr res = Interop.User32.SetTimer(_windowHandle, timerId, (int)wParam, IntPtr.Zero);
            return (res == IntPtr.Zero ? IntPtr.Zero : timerId);
        }

        /// <devdoc>
        ///      Handler that raises the DisplaySettings changing event
        /// </devdoc>
        private void OnDisplaySettingsChanging()
        {
            RaiseEvent(s_onDisplaySettingsChangingEvent, this, EventArgs.Empty);
        }

        /// <devdoc>
        ///      Handler that raises the DisplaySettings changed event
        /// </devdoc>
        private void OnDisplaySettingsChanged()
        {
            RaiseEvent(s_onDisplaySettingsChangedEvent, this, EventArgs.Empty);
        }

        /// <devdoc>
        ///      Handler for any event that fires a standard EventHandler delegate.
        /// </devdoc>
        private void OnGenericEvent(object eventKey)
        {
            RaiseEvent(eventKey, this, EventArgs.Empty);
        }

        private void OnShutdown(object eventKey)
        {
            RaiseEvent(false, eventKey, this, EventArgs.Empty);
        }

        /// <devdoc>
        ///      Callback that handles the KillTimer
        ///      user message.        
        /// </devdoc>
        private bool OnKillTimer(IntPtr wParam)
        {
            bool res = Interop.User32.KillTimer(_windowHandle, wParam);
            return res;
        }

        /// <devdoc>
        ///      Handler for WM_POWERBROADCAST.
        /// </devdoc>
        private void OnPowerModeChanged(IntPtr wParam)
        {
            PowerModes mode;

            switch ((int)wParam)
            {
                case Interop.User32.PBT_APMSUSPEND:
                case Interop.User32.PBT_APMSTANDBY:
                    mode = PowerModes.Suspend;
                    break;

                case Interop.User32.PBT_APMRESUMECRITICAL:
                case Interop.User32.PBT_APMRESUMESUSPEND:
                case Interop.User32.PBT_APMRESUMESTANDBY:
                    mode = PowerModes.Resume;
                    break;

                case Interop.User32.PBT_APMBATTERYLOW:
                case Interop.User32.PBT_APMPOWERSTATUSCHANGE:
                case Interop.User32.PBT_APMOEMEVENT:
                    mode = PowerModes.StatusChange;
                    break;

                default:
                    return;
            }

            RaiseEvent(s_onPowerModeChangedEvent, this, new PowerModeChangedEventArgs(mode));
        }

        /// <devdoc>
        ///      Handler for WM_ENDSESSION.
        /// </devdoc>
        private void OnSessionEnded(IntPtr wParam, IntPtr lParam)
        {
            // wParam will be nonzero if the session is actually ending.  If
            // it was canceled then we do not want to raise the event.
            if (wParam != (IntPtr)0)
            {
                SessionEndReasons reason = SessionEndReasons.SystemShutdown;

                if (((unchecked((int)(long)lParam)) & Interop.User32.ENDSESSION_LOGOFF) != 0)
                {
                    reason = SessionEndReasons.Logoff;
                }

                SessionEndedEventArgs endEvt = new SessionEndedEventArgs(reason);

                RaiseEvent(s_onSessionEndedEvent, this, endEvt);
            }
        }

        /// <devdoc>
        ///      Handler for WM_QUERYENDSESSION.
        /// </devdoc>
        private int OnSessionEnding(IntPtr lParam)
        {
            int endOk = 1;

            SessionEndReasons reason = SessionEndReasons.SystemShutdown;

            // Casting to (int) is bad if we're 64-bit; casting to (long) is ok whether we're 64- or 32-bit.
            if ((((long)lParam) & Interop.User32.ENDSESSION_LOGOFF) != 0)
            {
                reason = SessionEndReasons.Logoff;
            }

            SessionEndingEventArgs endEvt = new SessionEndingEventArgs(reason);

            RaiseEvent(s_onSessionEndingEvent, this, endEvt);
            endOk = (endEvt.Cancel ? 0 : 1);

            return endOk;
        }

        private void OnSessionSwitch(int wParam)
        {
            SessionSwitchEventArgs switchEventArgs = new SessionSwitchEventArgs((SessionSwitchReason)wParam);

            RaiseEvent(s_onSessionSwitchEvent, this, switchEventArgs);
        }

        /// <devdoc>
        ///      Handler for WM_THEMECHANGED
        ///      Whidbey note: Before Whidbey, we used to fire UserPreferenceChanged with category
        ///      set to Window. In Whidbey, we support visual styles and need a new category Theme 
        ///      since Window is too general. We fire UserPreferenceChanged with this category, but
        ///      for backward compat, we also fire it with category set to Window.
        /// </devdoc>
        private void OnThemeChanged()
        {
            // we need to fire a changing event handler for Themes.
            // note that it needs to be documented that accessing theme information during the changing event is forbidden.
            RaiseEvent(s_onUserPreferenceChangingEvent, this, new UserPreferenceChangingEventArgs(UserPreferenceCategory.VisualStyle));

            UserPreferenceCategory pref = UserPreferenceCategory.Window;

            RaiseEvent(s_onUserPreferenceChangedEvent, this, new UserPreferenceChangedEventArgs(pref));

            pref = UserPreferenceCategory.VisualStyle;

            RaiseEvent(s_onUserPreferenceChangedEvent, this, new UserPreferenceChangedEventArgs(pref));
        }

        /// <devdoc>
        ///      Handler for WM_SETTINGCHANGE and WM_SYSCOLORCHANGE.
        /// </devdoc>
        private void OnUserPreferenceChanged(int msg, IntPtr wParam, IntPtr lParam)
        {
            UserPreferenceCategory pref = GetUserPreferenceCategory(msg, wParam, lParam);

            RaiseEvent(s_onUserPreferenceChangedEvent, this, new UserPreferenceChangedEventArgs(pref));
        }

        private void OnUserPreferenceChanging(int msg, IntPtr wParam, IntPtr lParam)
        {
            UserPreferenceCategory pref = GetUserPreferenceCategory(msg, wParam, lParam);

            RaiseEvent(s_onUserPreferenceChangingEvent, this, new UserPreferenceChangingEventArgs(pref));
        }

        /// <devdoc>
        ///      Handler for WM_TIMER.
        /// </devdoc>
        private void OnTimerElapsed(IntPtr wParam)
        {
            RaiseEvent(s_onTimerElapsedEvent, this, new TimerElapsedEventArgs(wParam));
        }

        private static void RaiseEvent(object key, params object[] args)
        {
            RaiseEvent(true, key, args);
        }

        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void RaiseEvent(bool checkFinalization, object key, params object[] args)
        {
            // If the AppDomain's unloading, we shouldn't fire SystemEvents other than Shutdown.
            if (checkFinalization && AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                return;
            }

            SystemEventInvokeInfo[] invokeItemArray = null;

            lock (s_eventLockObject)
            {
                if (s_handlers != null && s_handlers.ContainsKey(key))
                {
                    List<SystemEventInvokeInfo> invokeItems = s_handlers[key];

                    // clone the list so we don't have this type locked and cause
                    // a deadlock if someone tries to modify handlers during an invoke.
                    if (invokeItems != null)
                    {
                        invokeItemArray = invokeItems.ToArray();
                    }
                }
            }

            if (invokeItemArray != null)
            {
                for (int i = 0; i < invokeItemArray.Length; i++)
                {
                    try
                    {
                        SystemEventInvokeInfo info = invokeItemArray[i];
                        info.Invoke(checkFinalization, args);
                        invokeItemArray[i] = null; // clear it if it's valid
                    }
                    catch (Exception)
                    {
                        // Eat exceptions (Everett compat)
                    }
                }

                // clean out any that are dead.
                lock (s_eventLockObject)
                {
                    List<SystemEventInvokeInfo> invokeItems = null;

                    for (int i = 0; i < invokeItemArray.Length; i++)
                    {
                        SystemEventInvokeInfo info = invokeItemArray[i];
                        if (info != null)
                        {
                            if (invokeItems == null)
                            {
                                if (!s_handlers.TryGetValue(key, out invokeItems))
                                {
                                    // weird.  just to be safe.
                                    return;
                                }
                            }

                            invokeItems.Remove(info);
                        }
                    }
                }
            }
        }

        private static void RemoveEventHandler(object key, Delegate value)
        {
            lock (s_eventLockObject)
            {
                if (s_handlers != null && s_handlers.ContainsKey(key))
                {
                    List<SystemEventInvokeInfo> invokeItems = (List<SystemEventInvokeInfo>)s_handlers[key];

                    invokeItems.Remove(new SystemEventInvokeInfo(value));
                }
            }
        }

        private static void Shutdown()
        {
            if (s_systemEvents != null && s_systemEvents._windowHandle != IntPtr.Zero)
            {
                lock (s_procLockObject)
                {
                    if (s_systemEvents != null)
                    {
                        // If we are using system events from another thread, request that it terminate
                        if (s_windowThread != null)
                        {
                            s_eventThreadTerminated = new ManualResetEvent(false);

#if DEBUG
                            int pid;
                            int thread = Interop.User32.GetWindowThreadProcessId(new HandleRef(s_systemEvents, s_systemEvents._windowHandle), out pid);
                            Debug.Assert(thread != Interop.Kernel32.GetCurrentThreadId(), "Don't call Shutdown on the system events thread");
#endif
                            Interop.User32.PostMessageW(new HandleRef(s_systemEvents, s_systemEvents._windowHandle), Interop.User32.WM_QUIT, IntPtr.Zero, IntPtr.Zero);

                            s_eventThreadTerminated.WaitOne();
                            s_windowThread.Join(); // avoids an AppDomainUnloaded exception on our background thread.
                        }
                        else
                        {
                            s_systemEvents.Dispose();
                            s_systemEvents = null;
                        }
                    }
                }
            }
        }

        [PrePrepareMethod]
        private static void Shutdown(object sender, EventArgs e)
        {
            Shutdown();
        }

        /// <devdoc>
        ///      A standard Win32 window proc for our broadcast window.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case Interop.User32.WM_SETTINGCHANGE:
                    string newString;
                    IntPtr newStringPtr = lParam;
                    if (lParam != IntPtr.Zero)
                    {
                        newString = Marshal.PtrToStringUni(lParam);
                        if (newString != null)
                        {
                            newStringPtr = Marshal.StringToHGlobalUni(newString);
                        }
                    }
                    Interop.User32.PostMessageW(_windowHandle, Interop.User32.WM_REFLECT + msg, wParam, newStringPtr);
                    break;
                case Interop.User32.WM_WTSSESSION_CHANGE:
                    OnSessionSwitch((int)wParam);
                    break;
                case Interop.User32.WM_SYSCOLORCHANGE:
                case Interop.User32.WM_COMPACTING:
                case Interop.User32.WM_DISPLAYCHANGE:
                case Interop.User32.WM_FONTCHANGE:
                case Interop.User32.WM_PALETTECHANGED:
                case Interop.User32.WM_TIMECHANGE:
                case Interop.User32.WM_TIMER:
                case Interop.User32.WM_THEMECHANGED:
                    Interop.User32.PostMessageW(_windowHandle, Interop.User32.WM_REFLECT + msg, wParam, lParam);
                    break;

                case Interop.User32.WM_CREATETIMER:
                    return OnCreateTimer(wParam);

                case Interop.User32.WM_KILLTIMER:
                    return (IntPtr)(OnKillTimer(wParam) ? 1 : 0);

                case Interop.User32.WM_REFLECT + Interop.User32.WM_SETTINGCHANGE:
                    try
                    {
                        OnUserPreferenceChanging(msg - Interop.User32.WM_REFLECT, wParam, lParam);
                        OnUserPreferenceChanged(msg - Interop.User32.WM_REFLECT, wParam, lParam);
                    }
                    finally
                    {
                        try
                        {
                            if (lParam != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(lParam);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Assert(false, "Exception occurred while freeing memory: " + e.ToString());
                        }
                    }
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_SYSCOLORCHANGE:
                    OnUserPreferenceChanging(msg - Interop.User32.WM_REFLECT, wParam, lParam);
                    OnUserPreferenceChanged(msg - Interop.User32.WM_REFLECT, wParam, lParam);
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_THEMECHANGED:
                    OnThemeChanged();
                    break;

                case Interop.User32.WM_QUERYENDSESSION:
                    return (IntPtr)OnSessionEnding(lParam);

                case Interop.User32.WM_ENDSESSION:
                    OnSessionEnded(wParam, lParam);
                    break;

                case Interop.User32.WM_POWERBROADCAST:
                    OnPowerModeChanged(wParam);
                    break;

                // WM_HIBERNATE on WinCE
                case Interop.User32.WM_REFLECT + Interop.User32.WM_COMPACTING:
                    OnGenericEvent(s_onLowMemoryEvent);
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_DISPLAYCHANGE:
                    OnDisplaySettingsChanging();
                    OnDisplaySettingsChanged();
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_FONTCHANGE:
                    OnGenericEvent(s_onInstalledFontsChangedEvent);
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_PALETTECHANGED:
                    OnGenericEvent(s_onPaletteChangedEvent);
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_TIMECHANGE:
                    OnGenericEvent(s_onTimeChangedEvent);
                    break;

                case Interop.User32.WM_REFLECT + Interop.User32.WM_TIMER:
                    OnTimerElapsed(wParam);
                    break;

                default:
                    // If we received a thread execute message, then execute it.
                    if (msg == s_threadCallbackMessage && msg != 0)
                    {
                        InvokeMarshaledCallbacks();
                        return IntPtr.Zero;
                    }
                    break;
            }

            return Interop.User32.DefWindowProcW(hWnd, msg, wParam, lParam);
        }

        /// <devdoc>
        ///      This is the method that runs our window thread.  This method
        ///      creates a window and spins up a message loop.  The window
        ///      is made visible with a size of 0, 0, so that it will trap
        ///      global broadcast messages.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2102:CatchNonClsCompliantExceptionsInGeneralHandlers")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void WindowThreadProc()
        {
            try
            {
                Initialize();
                s_eventWindowReady.Set();

                if (_windowHandle != IntPtr.Zero)
                {
                    Interop.User32.MSG msg = new Interop.User32.MSG();

                    bool keepRunning = true;

                    // Blocking on a GetMessage() call prevents the EE from being able to unwind
                    // this thread properly (e.g. during AppDomainUnload). So, we use PeekMessage()
                    // and sleep so we always block in managed code instead.
                    while (keepRunning)
                    {
                        int ret = Interop.User32.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, Interop.User32.QS_ALLINPUT, Interop.User32.MWMO_INPUTAVAILABLE);

                        if (ret == Interop.User32.WAIT_TIMEOUT)
                        {
                            Thread.Sleep(1);
                        }
                        else
                        {
                            while (Interop.User32.PeekMessageW(ref msg, IntPtr.Zero, 0, 0, Interop.User32.PM_REMOVE))
                            {
                                if (msg.message == Interop.User32.WM_QUIT)
                                {
                                    keepRunning = false;
                                    break;
                                }

                                Interop.User32.TranslateMessage(ref msg);
                                Interop.User32.DispatchMessageW(ref msg);
                            }
                        }
                    }
                }

                OnShutdown(s_onEventsThreadShutdownEvent);
            }
            catch (Exception e)
            {
                // In case something very very wrong happend during the creation action.
                // This will unblock the calling thread.
                s_eventWindowReady.Set();

                if (!((e is ThreadInterruptedException) || (e is ThreadAbortException)))
                {
                    Debug.Fail("Unexpected thread exception in system events window thread proc", e.ToString());
                }
            }

            Dispose();
            if (s_eventThreadTerminated != null)
            {
                s_eventThreadTerminated.Set();
            }
        }

        // A class that helps fire events on the right thread.
        private class SystemEventInvokeInfo
        {
            private SynchronizationContext _syncContext; // the context that we'll use to fire against.
            private Delegate _delegate;     // the delegate we'll fire.  This is a weak ref so we don't hold object in memory.
            public SystemEventInvokeInfo(Delegate d)
            {
                _delegate = d;
                _syncContext = AsyncOperationManager.SynchronizationContext;
            }

            // fire the given event with the given params.
            public void Invoke(bool checkFinalization, params object[] args)
            {
                try
                {
                    // If we didn't get call back invoke directly.
                    if (_syncContext == null)
                    {
                        InvokeCallback(args);
                    }
                    else
                    {
                        // otherwise tell the context to do it for us.
                        _syncContext.Send(new SendOrPostCallback(InvokeCallback), args);
                    }
                }
                catch (InvalidAsynchronousStateException)
                {
                    // if the synch context is invalid -- do the invoke directly for app compat.
                    // If the app's shutting down, don't fire the event (unless it's shutdown).
                    if (!checkFinalization || !AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        InvokeCallback(args);
                    }
                }
            }

            // our delegate method that the SyncContext will call on.
            private void InvokeCallback(object arg)
            {
                _delegate.DynamicInvoke((object[])arg);
            }

            public override bool Equals(object other)
            {
                SystemEventInvokeInfo otherInvoke = other as SystemEventInvokeInfo;

                if (otherInvoke == null)
                {
                    return false;
                }
                return otherInvoke._delegate.Equals(_delegate);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
