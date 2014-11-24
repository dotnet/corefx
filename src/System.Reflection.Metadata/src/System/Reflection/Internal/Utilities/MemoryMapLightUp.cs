// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Internal
{
    internal static class MemoryMapLightUp
    {
        private static Type _lazyMemoryMappedFileType;
        private static Type _lazyMemoryMappedViewAccessorType;
        private static Type _lazyMemoryMappedFileAccessType;
        private static Type _lazyMemoryMappedFileSecurityType;
        private static Type _lazyHandleInheritabilityType;
        private static MethodInfo _lazyCreateFromFile;
        private static MethodInfo _lazyCreateViewAccessor;
        private static PropertyInfo _lazySafeMemoryMappedViewHandle;
        private static PropertyInfo _lazyPointerOffset;
        private static FieldInfo _lazyInternalViewField;
        private static PropertyInfo _lazyInternalPointerOffset;

        private static readonly object _MemoryMappedFileAccess_Read = 1;
        private static readonly object _HandleInheritability_None = 0;
        private static readonly object _LongZero = (long)0;
        private static readonly object _True = true;

        // test only:
        internal static bool Test450Compat;

        private static bool? _lazyIsAvailable;

        internal static bool IsAvailable
        {
            get
            {
                if (!_lazyIsAvailable.HasValue)
                {
                    _lazyIsAvailable = TryLoadTypes();
                }

                return _lazyIsAvailable.Value;
            }
        }

        private static bool TryLoadType(string assemblyQualifiedName, out Type type)
        {
            try
            {
                type = Type.GetType(assemblyQualifiedName, throwOnError: false);
            }
            catch
            {
                type = null;
            }

            return type != null;
        }

        private static bool TryLoadTypes()
        {
            const string SystemCoreRef = "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

            return FileStreamReadLightUp.FileStreamType.Value != null
                && TryLoadType("System.IO.MemoryMappedFiles.MemoryMappedFile, " + SystemCoreRef, out _lazyMemoryMappedFileType)
                && TryLoadType("System.IO.MemoryMappedFiles.MemoryMappedViewAccessor, " + SystemCoreRef, out _lazyMemoryMappedViewAccessorType)
                && TryLoadType("System.IO.MemoryMappedFiles.MemoryMappedFileAccess, " + SystemCoreRef, out _lazyMemoryMappedFileAccessType)
                && TryLoadType("System.IO.MemoryMappedFiles.MemoryMappedFileSecurity, " + SystemCoreRef, out _lazyMemoryMappedFileSecurityType)
                && TryLoadType("System.IO.HandleInheritability, " + SystemCoreRef, out _lazyHandleInheritabilityType)
                && TryLoadMembers();
        }

        private static bool TryLoadMembers()
        {
            _lazyCreateFromFile =
                (from m in _lazyMemoryMappedFileType.GetTypeInfo().GetDeclaredMethods("CreateFromFile")
                 let ps = m.GetParameters()
                 where ps.Length == 7 &&
                     ps[0].ParameterType == FileStreamReadLightUp.FileStreamType.Value &&
                     ps[1].ParameterType == typeof(string) &&
                     ps[2].ParameterType == typeof(long) &&
                     ps[3].ParameterType == _lazyMemoryMappedFileAccessType &&
                     ps[4].ParameterType == _lazyMemoryMappedFileSecurityType &&
                     ps[5].ParameterType == _lazyHandleInheritabilityType &&
                     ps[6].ParameterType == typeof(bool)
                 select m).SingleOrDefault();

            if (_lazyCreateFromFile == null)
            {
                return false;
            }

            _lazyCreateViewAccessor =
                (from m in _lazyMemoryMappedFileType.GetTypeInfo().GetDeclaredMethods("CreateViewAccessor")
                 let ps = m.GetParameters()
                 where ps.Length == 3 &&
                     ps[0].ParameterType == typeof(long) &&
                     ps[1].ParameterType == typeof(long) &&
                     ps[2].ParameterType == _lazyMemoryMappedFileAccessType
                 select m).SingleOrDefault();

            if (_lazyCreateViewAccessor == null)
            {
                return false;
            }

            _lazySafeMemoryMappedViewHandle = _lazyMemoryMappedViewAccessorType.GetTypeInfo().GetDeclaredProperty("SafeMemoryMappedViewHandle");
            if (_lazySafeMemoryMappedViewHandle == null)
            {
                return false;
            }

            // Available on FW >= 4.5.1:
            _lazyPointerOffset = Test450Compat ? null : _lazyMemoryMappedViewAccessorType.GetTypeInfo().GetDeclaredProperty("PointerOffset");
            if (_lazyPointerOffset == null)
            {
                // FW < 4.5.1
                _lazyInternalViewField = _lazyMemoryMappedViewAccessorType.GetTypeInfo().GetDeclaredField("m_view");
                if (_lazyInternalViewField == null)
                {
                    return false;
                }

                _lazyInternalPointerOffset = _lazyInternalViewField.FieldType.GetTypeInfo().GetDeclaredProperty("PointerOffset");
                if (_lazyInternalPointerOffset == null)
                {
                    return false;
                }
            }

            return true;
        }

        internal static IDisposable CreateMemoryMap(Stream stream)
        {
            Debug.Assert(_lazyIsAvailable.GetValueOrDefault());

            try
            {
                return (IDisposable)_lazyCreateFromFile.Invoke(null, new object[7]
                {
                    stream,                      // fileStream
                    null,                        // mapName
                    _LongZero,                    // capacity
                    _MemoryMappedFileAccess_Read, // access
                    null,                        // memoryMappedFileSecurity
                    _HandleInheritability_None,   // inheritability
                    _True,                        // leaveOpen
                });
            }
            catch (MemberAccessException)
            {
                _lazyIsAvailable = false;
                return null;
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        internal static IDisposable CreateViewAccessor(object memoryMap, long start, int size)
        {
            Debug.Assert(_lazyIsAvailable.GetValueOrDefault());
            try
            {
                return (IDisposable)_lazyCreateViewAccessor.Invoke(memoryMap, new object[3]
                {
                    start,                       // start
                    (long)size,                  // size
                    _MemoryMappedFileAccess_Read, // access
                });
            }
            catch (MemberAccessException)
            {
                _lazyIsAvailable = false;
                return null;
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        internal unsafe static byte* AcquirePointer(object accessor, out SafeBuffer safeBuffer)
        {
            Debug.Assert(_lazyIsAvailable.GetValueOrDefault());

            safeBuffer = (SafeBuffer)_lazySafeMemoryMappedViewHandle.GetValue(accessor);

            byte* ptr = null;
            safeBuffer.AcquirePointer(ref ptr);

            long offset;
            if (_lazyPointerOffset != null)
            {
                offset = (long)_lazyPointerOffset.GetValue(accessor);
            }
            else
            {
                object internalView = _lazyInternalViewField.GetValue(accessor);
                offset = (long)_lazyInternalPointerOffset.GetValue(internalView);
            }

            return ptr + offset;
        }
    }
}
