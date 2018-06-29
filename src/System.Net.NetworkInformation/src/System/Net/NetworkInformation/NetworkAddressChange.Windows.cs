// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace System.Net.NetworkInformation
{
    public partial class NetworkChange
    {
        private static readonly object s_globalLock = new object();

        public static event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
        {
            add
            {
                AvailabilityChangeListener.Start(value);
            }
            remove
            {
                AvailabilityChangeListener.Stop(value);
            }
        }

        public static event NetworkAddressChangedEventHandler NetworkAddressChanged
        {
            add
            {
                AddressChangeListener.Start(value);
            }
            remove
            {
                AddressChangeListener.Stop(value);
            }
        }

        internal static class AvailabilityChangeListener
        {
            private static readonly NetworkAddressChangedEventHandler s_addressChange = ChangedAddress;
            private static volatile bool s_isAvailable = false;

            private static void ChangedAddress(object sender, EventArgs eventArgs)
            {
                Dictionary<NetworkAvailabilityChangedEventHandler, ExecutionContext> availabilityChangedSubscribers = null;

                lock (s_globalLock)
                {
                    bool isAvailableNow = SystemNetworkInterface.InternalGetIsNetworkAvailable();

                    // If there is an Availability Change, need to execute user callbacks.
                    if (isAvailableNow != s_isAvailable)
                    {
                        s_isAvailable = isAvailableNow;

                        if (s_availabilityChangedSubscribers.Count > 0)
                        {
                            availabilityChangedSubscribers = new Dictionary<NetworkAvailabilityChangedEventHandler, ExecutionContext>(s_availabilityChangedSubscribers);
                        }
                    }
                }

                // Executing user callbacks if Availability Change event occured.
                if (availabilityChangedSubscribers != null)
                {
                    bool isAvailable = s_isAvailable;
                    NetworkAvailabilityEventArgs args = isAvailable ? s_availableEventArgs : s_notAvailableEventArgs;
                    ContextCallback callbackContext = isAvailable ? s_runHandlerAvailable : s_runHandlerNotAvailable;

                    foreach (KeyValuePair<NetworkAvailabilityChangedEventHandler, ExecutionContext> 
                        subscriber in availabilityChangedSubscribers)
                    {
                        NetworkAvailabilityChangedEventHandler handler = subscriber.Key;
                        ExecutionContext ec = subscriber.Value;

                        if (ec == null) // Flow supressed
                        {
                            handler(null, args);
                        }
                        else
                        {
                            ExecutionContext.Run(ec, callbackContext, handler);
                        }
                    }
                }
            }

            internal static void Start(NetworkAvailabilityChangedEventHandler caller)
            {
                if (caller != null)
                {
                    lock (s_globalLock)
                    {
                        if (s_availabilityChangedSubscribers.Count == 0)
                        {
                            s_isAvailable = NetworkInterface.GetIsNetworkAvailable();
                            AddressChangeListener.UnsafeStart(s_addressChange);
                        }

                        s_availabilityChangedSubscribers.TryAdd(caller, ExecutionContext.Capture());
                    }
                }
            }

            internal static void Stop(NetworkAvailabilityChangedEventHandler caller)
            {
                if (caller != null)
                {
                    lock (s_globalLock)
                    {
                        s_availabilityChangedSubscribers.Remove(caller);
                        if (s_availabilityChangedSubscribers.Count == 0)
                        {
                            AddressChangeListener.Stop(s_addressChange);
                        }
                    }
                }
            }
        }

        // Helper class for detecting address change events.
        internal static unsafe class AddressChangeListener
        {
            private static RegisteredWaitHandle s_registeredWait;

            // Need to keep the reference so it isn't GC'd before the native call executes.
            private static bool s_isListening = false;
            private static bool s_isPending = false;
            private static SafeCloseSocketAndEvent s_ipv4Socket = null;
            private static SafeCloseSocketAndEvent s_ipv6Socket = null;
            private static WaitHandle s_ipv4WaitHandle = null;
            private static WaitHandle s_ipv6WaitHandle = null;

            // Callback fired when an address change occurs.
            private static void AddressChangedCallback(object stateObject, bool signaled)
            {
                Dictionary<NetworkAddressChangedEventHandler, ExecutionContext> addressChangedSubscribers = null;

                lock (s_globalLock)
                {
                    // The listener was canceled, which would only happen if we aren't listening for more events.
                    s_isPending = false;

                    if (!s_isListening)
                    {
                        return;
                    }

                    s_isListening = false;

                    // Need to copy the array so the callback can call start and stop
                    if (s_addressChangedSubscribers.Count > 0)
                    {
                        addressChangedSubscribers = new Dictionary<NetworkAddressChangedEventHandler, ExecutionContext>(s_addressChangedSubscribers);
                    }

                    try
                    {
                        //wait for the next address change
                        StartHelper(null, false, (StartIPOptions)stateObject);
                    }
                    catch (NetworkInformationException nie)
                    {
                        if (NetEventSource.IsEnabled) NetEventSource.Error(null, nie);
                    }
                }

                // Release the lock before calling into user callback.
                if (addressChangedSubscribers != null)
                {
                    foreach (KeyValuePair<NetworkAddressChangedEventHandler, ExecutionContext>
                        subscriber in addressChangedSubscribers)
                    {
                        NetworkAddressChangedEventHandler handler = subscriber.Key;
                        ExecutionContext ec = subscriber.Value;

                        if (ec == null) // Flow supressed
                        {
                            handler(null, EventArgs.Empty);
                        }
                        else
                        {
                            ExecutionContext.Run(ec, s_runAddressChangedHandler, handler);
                        }
                    }
                }
            }

            internal static void Start(NetworkAddressChangedEventHandler caller)
            {
                StartHelper(caller, true, StartIPOptions.Both);
            }

            internal static void UnsafeStart(NetworkAddressChangedEventHandler caller)
            {
                StartHelper(caller, false, StartIPOptions.Both);
            }

            private static void StartHelper(NetworkAddressChangedEventHandler caller, bool captureContext, StartIPOptions startIPOptions)
            {
                if (caller != null)
                {
                    lock (s_globalLock)
                    {
                        // Setup changedEvent and native overlapped struct.
                        if (s_ipv4Socket == null)
                        {
                            int blocking;

                            // Sockets will be initialized by the call to OSSupportsIP*.
                            if (Socket.OSSupportsIPv4)
                            {
                                blocking = -1;
                                s_ipv4Socket = SafeCloseSocketAndEvent.CreateWSASocketWithEvent(AddressFamily.InterNetwork, SocketType.Dgram, (ProtocolType)0, true, false);
                                Interop.Winsock.ioctlsocket(s_ipv4Socket, Interop.Winsock.IoctlSocketConstants.FIONBIO, ref blocking);
                                s_ipv4WaitHandle = s_ipv4Socket.GetEventHandle();
                            }

                            if (Socket.OSSupportsIPv6)
                            {
                                blocking = -1;
                                s_ipv6Socket = SafeCloseSocketAndEvent.CreateWSASocketWithEvent(AddressFamily.InterNetworkV6, SocketType.Dgram, (ProtocolType)0, true, false);
                                Interop.Winsock.ioctlsocket(s_ipv6Socket, Interop.Winsock.IoctlSocketConstants.FIONBIO, ref blocking);
                                s_ipv6WaitHandle = s_ipv6Socket.GetEventHandle();
                            }
                        }

                        s_addressChangedSubscribers.TryAdd(caller, captureContext ? ExecutionContext.Capture() : null);

                        if (s_isListening || s_addressChangedSubscribers.Count == 0)
                        {
                            return;
                        }

                        if (!s_isPending)
                        {
                            int length;
                            SocketError errorCode;

                            if (Socket.OSSupportsIPv4 && (startIPOptions & StartIPOptions.StartIPv4) != 0)
                            {
                                s_registeredWait = ThreadPool.RegisterWaitForSingleObject(
                                    s_ipv4WaitHandle,
                                    new WaitOrTimerCallback(AddressChangedCallback),
                                    StartIPOptions.StartIPv4,
                                    -1,
                                    true);

                                errorCode = Interop.Winsock.WSAIoctl_Blocking(
                                    s_ipv4Socket.DangerousGetHandle(),
                                    (int)IOControlCode.AddressListChange,
                                    null, 0, null, 0,
                                    out length,
                                    IntPtr.Zero, IntPtr.Zero);

                                if (errorCode != SocketError.Success)
                                {
                                    NetworkInformationException exception = new NetworkInformationException();
                                    if (exception.ErrorCode != (uint)SocketError.WouldBlock)
                                    {
                                        throw exception;
                                    }
                                }

                                SafeWaitHandle s_ipv4SocketGetEventHandleSafeWaitHandle =
                                    s_ipv4Socket.GetEventHandle().GetSafeWaitHandle();

                                errorCode = Interop.Winsock.WSAEventSelect(
                                    s_ipv4Socket,
                                    s_ipv4SocketGetEventHandleSafeWaitHandle,
                                    Interop.Winsock.AsyncEventBits.FdAddressListChange);

                                if (errorCode != SocketError.Success)
                                {
                                    throw new NetworkInformationException();
                                }
                            }

                            if (Socket.OSSupportsIPv6 && (startIPOptions & StartIPOptions.StartIPv6) != 0)
                            {
                                s_registeredWait = ThreadPool.RegisterWaitForSingleObject(
                                    s_ipv6WaitHandle,
                                    new WaitOrTimerCallback(AddressChangedCallback),
                                    StartIPOptions.StartIPv6,
                                    -1,
                                    true);

                                errorCode = Interop.Winsock.WSAIoctl_Blocking(
                                    s_ipv6Socket.DangerousGetHandle(),
                                    (int)IOControlCode.AddressListChange,
                                    null, 0, null, 0,
                                    out length,
                                    IntPtr.Zero, IntPtr.Zero);

                                if (errorCode != SocketError.Success)
                                {
                                    NetworkInformationException exception = new NetworkInformationException();
                                    if (exception.ErrorCode != (uint)SocketError.WouldBlock)
                                    {
                                        throw exception;
                                    }
                                }

                                SafeWaitHandle s_ipv6SocketGetEventHandleSafeWaitHandle =
                                    s_ipv6Socket.GetEventHandle().GetSafeWaitHandle();

                                errorCode = Interop.Winsock.WSAEventSelect(
                                    s_ipv6Socket,
                                    s_ipv6SocketGetEventHandleSafeWaitHandle,
                                    Interop.Winsock.AsyncEventBits.FdAddressListChange);

                                if (errorCode != SocketError.Success)
                                {
                                    throw new NetworkInformationException();
                                }
                            }
                        }

                        s_isListening = true;
                        s_isPending = true;
                    }
                }
            }

            internal static void Stop(NetworkAddressChangedEventHandler caller)
            {
                if (caller != null)
                {
                    lock (s_globalLock)
                    {
                        s_addressChangedSubscribers.Remove(caller);
                        if (s_addressChangedSubscribers.Count == 0 && s_isListening)
                        {
                            s_isListening = false;
                        }
                    }
                }
            }
        }
    }
}
