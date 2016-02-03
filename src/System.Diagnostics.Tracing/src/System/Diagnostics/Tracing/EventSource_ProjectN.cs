// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Reflection;
using Microsoft.Win32;

namespace System.Diagnostics.Tracing
{
    public partial class EventSource : IDisposable
    {
        // ActivityID support (see also WriteEventWithRelatedActivityIdCore)
        /// <summary>
        /// When a thread starts work that is on behalf of 'something else' (typically another 
        /// thread or network request) it should mark the thread as working on that other work.
        /// This API marks the current thread as working on activity 'activityID'. This API
        /// should be used when the caller knows the thread's current activity (the one being
        /// overwritten) has completed. Otherwise, callers should prefer the overload that
        /// return the oldActivityThatWillContinue (below).
        /// 
        /// All events created with the EventSource on this thread are also tagged with the 
        /// activity ID of the thread. 
        /// 
        /// It is common, and good practice after setting the thread to an activity to log an event
        /// with a 'start' opcode to indicate that precise time/thread where the new activity 
        /// started.
        /// </summary>
        /// <param name="activityId">A Guid that represents the new activity with which to mark 
        /// the current thread</param>
        [System.Security.SecuritySafeCritical]
        public static void SetCurrentThreadActivityId(Guid activityId)
        {
            throw new NotSupportedException("Old style activities are not supported!");
        }

        /// <summary>
        /// When a thread starts work that is on behalf of 'something else' (typically another 
        /// thread or network request) it should mark the thread as working on that other work.
        /// This API marks the current thread as working on activity 'activityID'. It returns 
        /// whatever activity the thread was previously marked with. There is a convention that
        /// callers can assume that callees restore this activity mark before the callee returns. 
        /// To encourage this this API returns the old activity, so that it can be restored later.
        /// 
        /// All events created with the EventSource on this thread are also tagged with the 
        /// activity ID of the thread. 
        /// 
        /// It is common, and good practice after setting the thread to an activity to log an event
        /// with a 'start' opcode to indicate that precise time/thread where the new activity 
        /// started.
        /// </summary>
        /// <param name="activityId">A Guid that represents the new activity with which to mark 
        /// the current thread</param>
        /// <param name="oldActivityThatWillContinue">The Guid that represents the current activity  
        /// which will continue at some point in the future, on the current thread</param>
        [System.Security.SecuritySafeCritical]
        public static void SetCurrentThreadActivityId(Guid activityId, out Guid oldActivityThatWillContinue)
        {
            throw new NotSupportedException("Old style activities are not supported!");
        }

        /// <summary>
        /// Retrieves the ETW activity ID associated with the current thread.
        /// </summary>
        public static Guid CurrentThreadActivityId
        {
            [System.Security.SecuritySafeCritical]
            get
            {
                throw new NotSupportedException("Old style activities are not supported!");
            }
        }

        private int GetParameterCount(EventMetadata eventData)
        {
            int paramCount;
            if(eventData.Parameters == null)
            {
                paramCount = eventData.ParameterTypes.Length;
            }
            else
            {
                paramCount = eventData.Parameters.Length;
            }
            
            return paramCount;
        }

        private Type GetDataType(EventMetadata eventData, int parameterId)
        {
            Type dataType;
            if(eventData.Parameters == null)
            {
                dataType = EventTypeToType(eventData.ParameterTypes[parameterId]);
            }
            else
            {
                dataType = eventData.Parameters[parameterId].ParameterType;
            }
            
            return dataType;
        }

        private static readonly bool m_EventSourcePreventRecursion = true; 

        internal partial struct EventMetadata
        {
            public EventMetadata(EventDescriptor descriptor,
                EventTags tags,
                bool enabledForAnyListener,
                bool enabledForETW,
                string name,
                string message,
                EventParameterType[] parameterTypes)
            {
                this.Descriptor = descriptor;
                this.Tags = tags;
                this.EnabledForAnyListener = enabledForAnyListener;
                this.EnabledForETW = enabledForETW;
                this.TriggersActivityTracking = 0;
                this.Name = name;
                this.Message = message;
                this.Parameters = null;
                this.TraceLoggingEventTypes = null;
                this.ActivityOptions = EventActivityOptions.None;
                this.ParameterTypes = parameterTypes;
                this.HasRelatedActivityID = false;
            }
        }
        
        public enum EventParameterType
        {
            Boolean,
            Byte,
            SByte,
            Char,
            Int16,
            UInt16,
            Int32,
            UInt32,
            Int64,
            UInt64,
            IntPtr,
            Single,
            Double,
            Decimal,
            Guid,
            String
        }

        private Type EventTypeToType(EventParameterType type)
        {
            switch (type)
            {
                case EventParameterType.Boolean:
                    return typeof(bool);
                case EventParameterType.Byte:
                    return typeof(byte);
                case EventParameterType.SByte:
                    return typeof(sbyte);
                case EventParameterType.Char:
                    return typeof(char);
                case EventParameterType.Int16:
                    return typeof(short);
                case EventParameterType.UInt16:
                    return typeof(ushort);
                case EventParameterType.Int32:
                    return typeof(int);
                case EventParameterType.UInt32:
                    return typeof(uint);
                case EventParameterType.Int64:
                    return typeof(long);
                case EventParameterType.UInt64:
                    return typeof(ulong);
                case EventParameterType.IntPtr:
                    return typeof(IntPtr);
                case EventParameterType.Single:
                    return typeof(float);
                case EventParameterType.Double:
                    return typeof(double);
                case EventParameterType.Decimal:
                    return typeof(decimal);
                case EventParameterType.Guid:
                    return typeof(Guid);
                case EventParameterType.String:
                    return typeof(string);
                default:
                    // TODO: should I throw an exception here?
                    return null;
            }
        }
    }
    
    internal partial class ManifestBuilder
    {
        private string GetTypeNameHelper(Type type)
        {
            switch (type.GetTypeCode())
            {
                case (Microsoft.Reflection.TypeCode)TypeCode.Boolean:
                    return "win:Boolean";
                case (Microsoft.Reflection.TypeCode)TypeCode.Byte:
                    return "win:UInt8";
                case (Microsoft.Reflection.TypeCode)TypeCode.Char:
                case (Microsoft.Reflection.TypeCode)TypeCode.UInt16:
                    return "win:UInt16";
                case (Microsoft.Reflection.TypeCode)TypeCode.UInt32:
                    return "win:UInt32";
                case (Microsoft.Reflection.TypeCode)TypeCode.UInt64:
                    return "win:UInt64";
                case (Microsoft.Reflection.TypeCode)TypeCode.SByte:
                    return "win:Int8";
                case (Microsoft.Reflection.TypeCode)TypeCode.Int16:
                    return "win:Int16";
                case (Microsoft.Reflection.TypeCode)TypeCode.Int32:
                    return "win:Int32";
                case (Microsoft.Reflection.TypeCode)TypeCode.Int64:
                    return "win:Int64";
                case (Microsoft.Reflection.TypeCode)TypeCode.String:
                    return "win:UnicodeString";
                case (Microsoft.Reflection.TypeCode)TypeCode.Single:
                    return "win:Float";
                case (Microsoft.Reflection.TypeCode)TypeCode.Double:
                    return "win:Double";
                case (Microsoft.Reflection.TypeCode)TypeCode.DateTime:
                    return "win:FILETIME";
                default:
                    if (type == typeof(Guid))
                        return "win:GUID";
                    else if (type == typeof(IntPtr))
                        return "win:Pointer";
                    else if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
                        return "win:Binary";
                        
                    ManifestError(Resources.GetResourceString("EventSource_UnsupportedEventTypeInManifest", type.Name), true);
                    return string.Empty;
            }
        }
    }
    
    internal partial class EventProvider
    {
        [System.Security.SecurityCritical]
        internal unsafe int SetInformation(
            UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS eventInfoClass,
            IntPtr data,
            uint dataSize)
        {
            int status = UnsafeNativeMethods.ManifestEtw.ERROR_NOT_SUPPORTED;

            if (!m_setInformationMissing)
            {
                try
                {
                    status = UnsafeNativeMethods.ManifestEtw.EventSetInformation(
                        m_regHandle,
                        eventInfoClass,
                        (void*)data,
                        (int)dataSize);
                }
                catch (TypeLoadException)
                {
                    m_setInformationMissing = true;
                }
            }

            return status;
        }
    }
    
    internal static class Resources
    {
        internal static string GetResourceString(string key, params object[] args)
        {
            string fmt = SR.GetResourceString(key, null);
            if (fmt != null)
                return string.Format(fmt, args);

            string sargs = String.Empty;
            foreach(var arg in args)
            {
                if (sargs != String.Empty)
                    sargs += ", ";
                sargs += arg.ToString();
            }

            return key + " (" + sargs + ")";
        }
        
        public static string GetRuntimeResourceString(string key, params object[] args)
        {
            return GetResourceString(key, args);
        }
    }
}
