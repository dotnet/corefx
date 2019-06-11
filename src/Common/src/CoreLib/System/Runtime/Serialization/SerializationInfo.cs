// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Runtime.Serialization
{
    /// <summary>The structure for holding all of the data needed for object serialization and deserialization.</summary>
    public sealed class SerializationInfo
    {
        private const int DefaultSize = 4;

        // Even though we have a dictionary, we're still keeping all the arrays around for back-compat. 
        // Otherwise we may run into potentially breaking behaviors like GetEnumerator() not returning entries in the same order they were added.
        private string[] _names;
        private object?[] _values;
        private Type[] _types;
        private int _count;
        private Dictionary<string, int> _nameToIndex;
        private IFormatterConverter _converter;
        private string _rootTypeName;
        private string _rootTypeAssemblyName;
        private Type _rootType;
        
        internal static AsyncLocal<bool> AsyncDeserializationInProgress { get; } = new AsyncLocal<bool>();        

#if !CORECLR
        // On AoT, assume private members are reflection blocked, so there's no further protection required
        // for the thread's DeserializationTracker
        [ThreadStatic]
        private static DeserializationTracker t_deserializationTracker;

        private static DeserializationTracker GetThreadDeserializationTracker()
        {
            if (t_deserializationTracker == null)
            {
                t_deserializationTracker = new DeserializationTracker();
            }

            return t_deserializationTracker;
        }
#endif // !CORECLR

        // Returns true if deserialization is currently in progress
        public static bool DeserializationInProgress
        {
#if CORECLR
            [DynamicSecurityMethod] // Methods containing StackCrawlMark local var must be marked DynamicSecurityMethod
#endif
            get
            {
                if (AsyncDeserializationInProgress.Value)
                {
                    return true;
                }

#if CORECLR
                StackCrawlMark stackMark = StackCrawlMark.LookForMe;
                DeserializationTracker tracker = Thread.GetThreadDeserializationTracker(ref stackMark);
#else
                DeserializationTracker tracker = GetThreadDeserializationTracker();
#endif
                bool result = tracker.DeserializationInProgress;
                return result;
            }
        }

        // Throws a DeserializationBlockedException if dangerous deserialization is currently
        // in progress
        public static void ThrowIfDeserializationInProgress()
        {
            if (DeserializationInProgress)
            {
                throw new DeserializationBlockedException();
            }
        }

        // Throws a DeserializationBlockedException if dangerous deserialization is currently
        // in progress and the AppContext switch Switch.System.Runtime.Serialization.SerializationGuard.{switchSuffix}
        // is not true. The value of the switch is cached in cachedValue to avoid repeated lookups:
        // 0: No value cached
        // 1: The switch is true
        // -1: The switch is false
        public static void ThrowIfDeserializationInProgress(string switchSuffix, ref int cachedValue)
        {
            const string SwitchPrefix = "Switch.System.Runtime.Serialization.SerializationGuard.";
            if (switchSuffix == null)
            {
                throw new ArgumentNullException(nameof(switchSuffix));
            }
            if (String.IsNullOrWhiteSpace(switchSuffix))
            {
                throw new ArgumentException(SR.Argument_EmptyName, nameof(switchSuffix));
            }

            if (cachedValue == 0)
            {
                bool isEnabled = false;
                if (AppContext.TryGetSwitch(SwitchPrefix + switchSuffix, out isEnabled) && isEnabled)
                {
                    cachedValue = 1;
                }
                else
                {
                    cachedValue = -1;
                }
            }

            if (cachedValue == 1)
            {
                return;
            }
            else if (cachedValue == -1)
            {
                if (DeserializationInProgress)
                {
                    throw new DeserializationBlockedException(SR.Format(SR.Serialization_DangerousDeserialization_Switch, SwitchPrefix + switchSuffix));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(cachedValue));
            }
        }

        // Declares that the current thread and async context have begun deserialization.
        // In this state, if the SerializationGuard or other related AppContext switches are set,
        // actions likely to be dangerous during deserialization, such as starting a process will be blocked.
        // Returns a DeserializationToken that must be disposed to remove the deserialization state.
#if CORECLR
        [DynamicSecurityMethod] // Methods containing StackCrawlMark local var must be marked DynamicSecurityMethod
#endif
        public static DeserializationToken StartDeserialization()
        {
            if (LocalAppContextSwitches.SerializationGuard)
            {
#if CORECLR
                StackCrawlMark stackMark = StackCrawlMark.LookForMe;
                DeserializationTracker tracker = Thread.GetThreadDeserializationTracker(ref stackMark);
#else
                DeserializationTracker tracker = GetThreadDeserializationTracker();
#endif
                if  (!tracker.DeserializationInProgress)
                {
                    lock (tracker)
                    {
                        if (!tracker.DeserializationInProgress)
                        {
                            AsyncDeserializationInProgress.Value = true;
                            tracker.DeserializationInProgress = true;
                            return new DeserializationToken(tracker);
                        }
                    }
                }
            }
            
            return new DeserializationToken(null);
        }

        [CLSCompliant(false)]
        public SerializationInfo(Type type, IFormatterConverter converter) 
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            _rootType = type;
            _rootTypeName = type.FullName!;
            _rootTypeAssemblyName = type.Module.Assembly.FullName!;

            _names = new string[DefaultSize];
            _values = new object[DefaultSize];
            _types = new Type[DefaultSize];

            _nameToIndex = new Dictionary<string, int>();

            _converter = converter;
        }

        [CLSCompliant(false)]
        public SerializationInfo(Type type, IFormatterConverter converter, bool requireSameTokenInPartialTrust)
            : this(type, converter)
        {
            // requireSameTokenInPartialTrust is a vacuous parameter in a platform that does not support partial trust.
        }

        public string FullTypeName
        {
            get { return _rootTypeName; }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _rootTypeName = value;
                IsFullTypeNameSetExplicit = true;
            }
        }

        public string AssemblyName
        {
            get { return _rootTypeAssemblyName; }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _rootTypeAssemblyName = value;
                IsAssemblyNameSetExplicit = true;
            }
        }

        public bool IsFullTypeNameSetExplicit { get; private set; }

        public bool IsAssemblyNameSetExplicit { get; private set; }

        public void SetType(Type type)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!ReferenceEquals(_rootType, type))
            {
                _rootType = type;
                _rootTypeName = type.FullName!;
                _rootTypeAssemblyName = type.Module.Assembly.FullName!;
                IsFullTypeNameSetExplicit = false;
                IsAssemblyNameSetExplicit = false;
            }
        }

        public int MemberCount => _count;

        public Type ObjectType => _rootType;

        public SerializationInfoEnumerator GetEnumerator() => new SerializationInfoEnumerator(_names, _values, _types, _count);

        private void ExpandArrays()
        {
            int newSize;
            Debug.Assert(_names.Length == _count, "[SerializationInfo.ExpandArrays]_names.Length == _count");

            newSize = (_count * 2);

            // In the pathological case, we may wrap
            if (newSize < _count)
            {
                if (int.MaxValue > _count)
                {
                    newSize = int.MaxValue;
                }
            }

            // Allocate more space and copy the data
            string[] newMembers = new string[newSize];
            object[] newData = new object[newSize];
            Type[] newTypes = new Type[newSize];

            Array.Copy(_names, 0, newMembers, 0, _count);
            Array.Copy(_values, 0, newData, 0, _count);
            Array.Copy(_types, 0, newTypes, 0, _count);

            // Assign the new arrays back to the member vars.
            _names = newMembers;
            _values = newData;
            _types = newTypes;
        }

        public void AddValue(string name, object? value, Type type)
        {
            if (null == name)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if ((object)type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            AddValueInternal(name, value, type);
        }

        public void AddValue(string name, object? value)
        {
            if (null == value)
            {
                AddValue(name, value, typeof(object));
            }
            else
            {
                AddValue(name, value, value.GetType());
            }
        }

        public void AddValue(string name, bool value)
        {
            AddValue(name, (object)value, typeof(bool));
        }

        public void AddValue(string name, char value)
        {
            AddValue(name, (object)value, typeof(char));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, sbyte value)
        {
            AddValue(name, (object)value, typeof(sbyte));
        }

        public void AddValue(string name, byte value)
        {
            AddValue(name, (object)value, typeof(byte));
        }

        public void AddValue(string name, short value)
        {
            AddValue(name, (object)value, typeof(short));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, ushort value)
        {
            AddValue(name, (object)value, typeof(ushort));
        }

        public void AddValue(string name, int value)
        {
            AddValue(name, (object)value, typeof(int));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, uint value)
        {
            AddValue(name, (object)value, typeof(uint));
        }

        public void AddValue(string name, long value)
        {
            AddValue(name, (object)value, typeof(long));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, ulong value)
        {
            AddValue(name, (object)value, typeof(ulong));
        }

        public void AddValue(string name, float value)
        {
            AddValue(name, (object)value, typeof(float));
        }

        public void AddValue(string name, double value)
        {
            AddValue(name, (object)value, typeof(double));
        }

        public void AddValue(string name, decimal value)
        {
            AddValue(name, (object)value, typeof(decimal));
        }

        public void AddValue(string name, DateTime value)
        {
            AddValue(name, (object)value, typeof(DateTime));
        }

        internal void AddValueInternal(string name, object? value, Type type)
        {
            if (_nameToIndex.ContainsKey(name))
            {
                throw new SerializationException(SR.Serialization_SameNameTwice);
            }
            _nameToIndex.Add(name, _count);

            // If we need to expand the arrays, do so.
            if (_count >= _names.Length)
            {
                ExpandArrays();
            }

            // Add the data and then advance the counter.
            _names[_count] = name;
            _values[_count] = value;
            _types[_count] = type;
            _count++;
        }

        /// <summary>
        /// Finds the value if it exists in the current data. If it does, we replace
        /// the values, if not, we append it to the end. This is useful to the 
        /// ObjectManager when it's performing fixups.
        /// 
        /// All error checking is done with asserts. Although public in coreclr,
        /// it's not exposed in a contract and is only meant to be used by corefx.
        ///
        /// This isn't a public API, but it gets invoked dynamically by 
        /// BinaryFormatter
        ///
        /// This should not be used by clients: exposing out this functionality would allow children
        /// to overwrite their parent's values. It is public in order to give corefx access to it for
        /// its ObjectManager implementation, but it should not be exposed out of a contract.
        /// </summary>
        /// <param name="name"> The name of the data to be updated.</param>
        /// <param name="value"> The new value.</param>
        /// <param name="type"> The type of the data being added.</param>
        public void UpdateValue(string name, object value, Type type)
        {
            Debug.Assert(null != name, "[SerializationInfo.UpdateValue]name!=null");
            Debug.Assert(null != value, "[SerializationInfo.UpdateValue]value!=null");
            Debug.Assert(null != (object)type, "[SerializationInfo.UpdateValue]type!=null");

            int index = FindElement(name);
            if (index < 0)
            {
                AddValueInternal(name, value, type);
            }
            else
            {
                _values[index] = value;
                _types[index] = type;
            }
        }

        private int FindElement(string name)
        {
            if (null == name)
            {
                throw new ArgumentNullException(nameof(name));
            }
            int index;
            if (_nameToIndex.TryGetValue(name, out index))
            {
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Gets the location of a particular member and then returns
        /// the value of the element at that location.  The type of the member is
        /// returned in the foundType field.
        /// </summary>
        /// <param name="name"> The name of the element to find.</param>
        /// <param name="foundType"> The type of the element associated with the given name.</param>
        /// <returns>The value of the element at the position associated with name.</returns>
        private object? GetElement(string name, out Type foundType)
        {
            int index = FindElement(name);
            if (index == -1)
            {
                throw new SerializationException(SR.Format(SR.Serialization_NotFound, name));
            }

            Debug.Assert(index < _values.Length, "[SerializationInfo.GetElement]index<_values.Length");
            Debug.Assert(index < _types.Length, "[SerializationInfo.GetElement]index<_types.Length");

            foundType = _types[index];
            Debug.Assert((object)foundType != null, "[SerializationInfo.GetElement]foundType!=null");
            return _values[index];
        }

        private object? GetElementNoThrow(string name, out Type? foundType)
        {
            int index = FindElement(name);
            if (index == -1)
            {
                foundType = null;
                return null;
            }

            Debug.Assert(index < _values.Length, "[SerializationInfo.GetElement]index<_values.Length");
            Debug.Assert(index < _types.Length, "[SerializationInfo.GetElement]index<_types.Length");

            foundType = _types[index];
            Debug.Assert((object)foundType != null, "[SerializationInfo.GetElement]foundType!=null");
            return _values[index];
        }

        public object? GetValue(string name, Type type)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsRuntimeImplemented())
                throw new ArgumentException(SR.Argument_MustBeRuntimeType);

            Type foundType;
            object? value = GetElement(name, out foundType);

            if (ReferenceEquals(foundType, type) || type.IsAssignableFrom(foundType) || value == null)
            {
                return value;
            }

            Debug.Assert(_converter != null, "[SerializationInfo.GetValue]_converter!=null");
            return _converter.Convert(value, type);
        }

        internal object? GetValueNoThrow(string name, Type type)
        {
            Debug.Assert((object)type != null, "[SerializationInfo.GetValue]type ==null");
            Debug.Assert(type.IsRuntimeImplemented(), "[SerializationInfo.GetValue]type is not a runtime type");

            Type? foundType;
            object? value = GetElementNoThrow(name, out foundType);
            if (value == null)
                return null;

            if (ReferenceEquals(foundType, type) || type.IsAssignableFrom(foundType) || value == null)
            {
                return value;
            }

            Debug.Assert(_converter != null, "[SerializationInfo.GetValue]_converter!=null");

            return _converter.Convert(value, type);
        }

        public bool GetBoolean(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(bool)) ? (bool)value! : _converter.ToBoolean(value!); // if value is null To* method will either deal with it or throw
        }

        public char GetChar(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(char)) ? (char)value! : _converter.ToChar(value!);
        }

        [CLSCompliant(false)]
        public sbyte GetSByte(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(sbyte)) ? (sbyte)value! : _converter.ToSByte(value!);
        }

        public byte GetByte(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(byte)) ? (byte)value! : _converter.ToByte(value!);
        }

        public short GetInt16(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(short)) ? (short)value! : _converter.ToInt16(value!);
        }

        [CLSCompliant(false)]
        public ushort GetUInt16(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(ushort)) ? (ushort)value! : _converter.ToUInt16(value!);
        }

        public int GetInt32(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(int)) ? (int)value! : _converter.ToInt32(value!);
        }

        [CLSCompliant(false)]
        public uint GetUInt32(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(uint)) ? (uint)value! : _converter.ToUInt32(value!);
        }

        public long GetInt64(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(long)) ? (long)value! : _converter.ToInt64(value!);
        }

        [CLSCompliant(false)]
        public ulong GetUInt64(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(ulong)) ? (ulong)value! : _converter.ToUInt64(value!);
        }

        public float GetSingle(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(float)) ? (float)value! : _converter.ToSingle(value!);
        }


        public double GetDouble(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(double)) ? (double)value! : _converter.ToDouble(value!);
        }

        public decimal GetDecimal(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(decimal)) ? (decimal)value! : _converter.ToDecimal(value!);
        }

        public DateTime GetDateTime(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(DateTime)) ? (DateTime)value! : _converter.ToDateTime(value!);
        }

        public string? GetString(string name)
        {
            Type foundType;
            object? value = GetElement(name, out foundType);
            return ReferenceEquals(foundType, typeof(string)) || value == null ? (string?)value : _converter.ToString(value);
        }
    }
}
