// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.Win32.SafeHandles {
  [System.Security.SecurityCriticalAttribute]
  public sealed partial class SafeFileHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid {
    public SafeFileHandle(System.IntPtr preexistingHandle, bool ownsHandle) : base (default(bool)) { }
    public override bool IsInvalid { [System.Security.SecurityCriticalAttribute]get { return default(bool); } }
    [System.Security.SecurityCriticalAttribute]
    protected override bool ReleaseHandle() { return default(bool); }
  }
  [System.Security.SecurityCriticalAttribute]
  public abstract partial class SafeHandleMinusOneIsInvalid : System.Runtime.InteropServices.SafeHandle {
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    protected SafeHandleMinusOneIsInvalid(bool ownsHandle) { }
    public override bool IsInvalid { [System.Security.SecurityCriticalAttribute]get { return default(bool); } }
  }
  [System.Security.SecurityCriticalAttribute]
  public abstract partial class SafeHandleZeroOrMinusOneIsInvalid : System.Runtime.InteropServices.SafeHandle {
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    protected SafeHandleZeroOrMinusOneIsInvalid(bool ownsHandle) { }
    public override bool IsInvalid { [System.Security.SecurityCriticalAttribute]get { return default(bool); } }
  }
  [System.Security.SecurityCriticalAttribute]
  public sealed partial class SafeWaitHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid {
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public SafeWaitHandle(System.IntPtr existingHandle, bool ownsHandle) : base (default(bool)) { }
    public override bool IsInvalid { [System.Security.SecurityCriticalAttribute]get { return default(bool); } }
    [System.Security.SecurityCriticalAttribute]
    protected override bool ReleaseHandle() { return default(bool); }
  }
}
namespace System {
  public partial class AccessViolationException : System.SystemException {
    public AccessViolationException() { }
    protected AccessViolationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public AccessViolationException(string message) { }
    public AccessViolationException(string message, System.Exception innerException) { }
  }
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate void Action();
  public delegate void Action<in T>(T obj);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
  public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
  public sealed partial class ActivationContext : System.IDisposable, System.Runtime.Serialization.ISerializable {
    internal ActivationContext() { }
    public System.ActivationContext.ContextForm Form { get { return default(System.ActivationContext.ContextForm); } }
    public System.ApplicationIdentity Identity { get { return default(System.ApplicationIdentity); } }
    public static System.ActivationContext CreatePartialActivationContext(System.ApplicationIdentity identity) { return default(System.ActivationContext); }
    public static System.ActivationContext CreatePartialActivationContext(System.ApplicationIdentity identity, string[] manifestPaths) { return default(System.ActivationContext); }
    public void Dispose() { }
    ~ActivationContext() { }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public enum ContextForm {
      Loose = 0,
      StoreBounded = 1,
    }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public static partial class Activator {
    public static object CreateInstance(System.Type type) { return default(object); }
    public static object CreateInstance(System.Type type, bool nonPublic) { return default(object); }
    public static object CreateInstance(System.Type type, params object[] args) { return default(object); }
    public static object CreateInstance(System.Type type, object[] args, object[] activationAttributes) { return default(object); }
    public static object CreateInstance(System.Type type, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, object[] args, System.Globalization.CultureInfo culture) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object CreateInstance(System.Type type, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes) { return default(object); }
    public static T CreateInstance<T>() { return default(T); }
    [System.Security.SecurityCriticalAttribute]
    public static object GetObject(System.Type type, string url) { return default(object); }
    [System.Security.SecurityCriticalAttribute]
    public static object GetObject(System.Type type, string url, object state) { return default(object); }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Count = {InnerExceptionCount}")]
  public partial class AggregateException : System.Exception {
    public AggregateException() { }
    public AggregateException(System.Collections.Generic.IEnumerable<System.Exception> innerExceptions) { }
    public AggregateException(params System.Exception[] innerExceptions) { }
    [System.Security.SecurityCriticalAttribute]
    protected AggregateException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public AggregateException(string message) { }
    public AggregateException(string message, System.Collections.Generic.IEnumerable<System.Exception> innerExceptions) { }
    public AggregateException(string message, System.Exception innerException) { }
    public AggregateException(string message, params System.Exception[] innerExceptions) { }
    public System.Collections.ObjectModel.ReadOnlyCollection<System.Exception> InnerExceptions { get { return default(System.Collections.ObjectModel.ReadOnlyCollection<System.Exception>); } }
    public System.AggregateException Flatten() { return default(System.AggregateException); }
    public override System.Exception GetBaseException() { return default(System.Exception); }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public void Handle(System.Func<System.Exception, bool> predicate) { }
    public override string ToString() { return default(string); }
  }
  public partial class ApplicationException : System.Exception {
    public ApplicationException() { }
    protected ApplicationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ApplicationException(string message) { }
    public ApplicationException(string message, System.Exception innerException) { }
  }
  public sealed partial class ApplicationId {
    public ApplicationId(byte[] publicKeyToken, string name, System.Version version, string processorArchitecture, string culture) { }
    public string Culture { get { return default(string); } }
    public string Name { get { return default(string); } }
    public string ProcessorArchitecture { get { return default(string); } }
    public byte[] PublicKeyToken { get { return default(byte[]); } }
    public System.Version Version { get { return default(System.Version); } }
    public System.ApplicationId Copy() { return default(System.ApplicationId); }
    public override bool Equals(object o) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public override string ToString() { return default(string); }
  }
  public sealed partial class ApplicationIdentity : System.Runtime.Serialization.ISerializable {
    public ApplicationIdentity(string applicationIdentityFullName) { }
    public string CodeBase { get { return default(string); } }
    public string FullName { get { return default(string); } }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  public partial class ArgumentException : System.SystemException, System.Runtime.Serialization.ISerializable {
    public ArgumentException() { }
    protected ArgumentException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ArgumentException(string message) { }
    public ArgumentException(string message, System.Exception innerException) { }
    public ArgumentException(string message, string paramName) { }
    public ArgumentException(string message, string paramName, System.Exception innerException) { }
    public override string Message { get { return default(string); } }
    public virtual string ParamName { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class ArgumentNullException : System.ArgumentException {
    public ArgumentNullException() { }
    [System.Security.SecurityCriticalAttribute]
    protected ArgumentNullException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ArgumentNullException(string paramName) { }
    public ArgumentNullException(string message, System.Exception innerException) { }
    public ArgumentNullException(string paramName, string message) { }
  }
  public partial class ArgumentOutOfRangeException : System.ArgumentException, System.Runtime.Serialization.ISerializable {
    public ArgumentOutOfRangeException() { }
    protected ArgumentOutOfRangeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ArgumentOutOfRangeException(string paramName) { }
    public ArgumentOutOfRangeException(string message, System.Exception innerException) { }
    public ArgumentOutOfRangeException(string paramName, object actualValue, string message) { }
    public ArgumentOutOfRangeException(string paramName, string message) { }
    public virtual object ActualValue { get { return default(object); } }
    public override string Message { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class ArithmeticException : System.SystemException {
    public ArithmeticException() { }
    protected ArithmeticException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ArithmeticException(string message) { }
    public ArithmeticException(string message, System.Exception innerException) { }
  }
  public abstract partial class Array : System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable {
    internal Array() { }
    public bool IsFixedSize { get { return default(bool); } }
    public bool IsReadOnly { get { return default(bool); } }
    public bool IsSynchronized { get { return default(bool); } }
    public int Length { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(int); } }
    public long LongLength { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(long); } }
    public int Rank { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(int); } }
    public object SyncRoot { get { return default(object); } }
    int System.Collections.ICollection.Count { get { return default(int); } }
    bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
    object System.Collections.ICollection.SyncRoot { get { return default(object); } }
    bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
    bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
    object System.Collections.IList.this[int index] { get { return default(object); } set { } }
    public static System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly<T>(T[] array) { return default(System.Collections.ObjectModel.ReadOnlyCollection<T>); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch(System.Array array, int index, int length, object value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch(System.Array array, int index, int length, object value, System.Collections.IComparer comparer) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch(System.Array array, object value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch(System.Array array, object value, System.Collections.IComparer comparer) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch<T>(T[] array, T value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch<T>(T[] array, T value, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch<T>(T[] array, int index, int length, T value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int BinarySearch<T>(T[] array, int index, int length, T value, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static void Clear(System.Array array, int index, int length) { }
    public object Clone() { return default(object); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static void ConstrainedCopy(System.Array sourceArray, int sourceIndex, System.Array destinationArray, int destinationIndex, int length) { }
    public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, System.Converter<TInput, TOutput> converter) { return default(TOutput[]); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Copy(System.Array sourceArray, System.Array destinationArray, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Copy(System.Array sourceArray, System.Array destinationArray, long length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Copy(System.Array sourceArray, int sourceIndex, System.Array destinationArray, int destinationIndex, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Copy(System.Array sourceArray, long sourceIndex, System.Array destinationArray, long destinationIndex, long length) { }
    public void CopyTo(System.Array array, int index) { }
    public void CopyTo(System.Array array, long index) { }
    public static System.Array CreateInstance(System.Type elementType, int length) { return default(System.Array); }
    public static System.Array CreateInstance(System.Type elementType, int length1, int length2) { return default(System.Array); }
    public static System.Array CreateInstance(System.Type elementType, int length1, int length2, int length3) { return default(System.Array); }
    public static System.Array CreateInstance(System.Type elementType, params int[] lengths) { return default(System.Array); }
    public static System.Array CreateInstance(System.Type elementType, int[] lengths, int[] lowerBounds) { return default(System.Array); }
    public static System.Array CreateInstance(System.Type elementType, params long[] lengths) { return default(System.Array); }
    public static T[] Empty<T>() { return default(T[]); }
    public static bool Exists<T>(T[] array, System.Predicate<T> match) { return default(bool); }
    public static T Find<T>(T[] array, System.Predicate<T> match) { return default(T); }
    public static T[] FindAll<T>(T[] array, System.Predicate<T> match) { return default(T[]); }
    public static int FindIndex<T>(T[] array, int startIndex, int count, System.Predicate<T> match) { return default(int); }
    public static int FindIndex<T>(T[] array, int startIndex, System.Predicate<T> match) { return default(int); }
    public static int FindIndex<T>(T[] array, System.Predicate<T> match) { return default(int); }
    public static T FindLast<T>(T[] array, System.Predicate<T> match) { return default(T); }
    public static int FindLastIndex<T>(T[] array, int startIndex, int count, System.Predicate<T> match) { return default(int); }
    public static int FindLastIndex<T>(T[] array, int startIndex, System.Predicate<T> match) { return default(int); }
    public static int FindLastIndex<T>(T[] array, System.Predicate<T> match) { return default(int); }
    public static void ForEach<T>(T[] array, System.Action<T> action) { }
    public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
    public int GetLength(int dimension) { return default(int); }
    public long GetLongLength(int dimension) { return default(long); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public int GetLowerBound(int dimension) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public int GetUpperBound(int dimension) { return default(int); }
    public object GetValue(int index) { return default(object); }
    public object GetValue(int index1, int index2) { return default(object); }
    public object GetValue(int index1, int index2, int index3) { return default(object); }
    public object GetValue(params int[] indices) { return default(object); }
    public object GetValue(long index) { return default(object); }
    public object GetValue(long index1, long index2) { return default(object); }
    public object GetValue(long index1, long index2, long index3) { return default(object); }
    public object GetValue(params long[] indices) { return default(object); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int IndexOf(System.Array array, object value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int IndexOf(System.Array array, object value, int startIndex) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int IndexOf(System.Array array, object value, int startIndex, int count) { return default(int); }
    public static int IndexOf<T>(T[] array, T value) { return default(int); }
    public static int IndexOf<T>(T[] array, T value, int startIndex) { return default(int); }
    public static int IndexOf<T>(T[] array, T value, int startIndex, int count) { return default(int); }
    public void Initialize() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int LastIndexOf(System.Array array, object value) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int LastIndexOf(System.Array array, object value, int startIndex) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int LastIndexOf(System.Array array, object value, int startIndex, int count) { return default(int); }
    public static int LastIndexOf<T>(T[] array, T value) { return default(int); }
    public static int LastIndexOf<T>(T[] array, T value, int startIndex) { return default(int); }
    public static int LastIndexOf<T>(T[] array, T value, int startIndex, int count) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Resize<T>(ref T[] array, int newSize) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Reverse(System.Array array) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Reverse(System.Array array, int index, int length) { }
    public void SetValue(object value, int index) { }
    public void SetValue(object value, int index1, int index2) { }
    public void SetValue(object value, int index1, int index2, int index3) { }
    public void SetValue(object value, params int[] indices) { }
    public void SetValue(object value, long index) { }
    public void SetValue(object value, long index1, long index2) { }
    public void SetValue(object value, long index1, long index2, long index3) { }
    public void SetValue(object value, params long[] indices) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array array) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array keys, System.Array items) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array keys, System.Array items, System.Collections.IComparer comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array keys, System.Array items, int index, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array keys, System.Array items, int index, int length, System.Collections.IComparer comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array array, System.Collections.IComparer comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array array, int index, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort(System.Array array, int index, int length, System.Collections.IComparer comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<T>(T[] array) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<T>(T[] array, System.Collections.Generic.IComparer<T> comparer) { }
    public static void Sort<T>(T[] array, System.Comparison<T> comparison) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<T>(T[] array, int index, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<T>(T[] array, int index, int length, System.Collections.Generic.IComparer<T> comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, System.Collections.Generic.IComparer<TKey> comparer) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length, System.Collections.Generic.IComparer<TKey> comparer) { }
    int System.Collections.IList.Add(object value) { return default(int); }
    void System.Collections.IList.Clear() { }
    bool System.Collections.IList.Contains(object value) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    int System.Collections.IList.IndexOf(object value) { return default(int); }
    void System.Collections.IList.Insert(int index, object value) { }
    void System.Collections.IList.Remove(object value) { }
    void System.Collections.IList.RemoveAt(int index) { }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    public static bool TrueForAll<T>(T[] array, System.Predicate<T> match) { return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct ArraySegment<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.IEnumerable {
    public ArraySegment(T[] array) { throw new System.NotImplementedException(); }
    public ArraySegment(T[] array, int offset, int count) { throw new System.NotImplementedException(); }
    public T[] Array { get { return default(T[]); } }
    public int Count { get { return default(int); } }
    public int Offset { get { return default(int); } }
    bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
    T System.Collections.Generic.IList<T>.this[int index] { get { return default(T); } set { } }
    T System.Collections.Generic.IReadOnlyList<T>.this[int index] { get { return default(T); } }
    public bool Equals(System.ArraySegment<T> obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.ArraySegment<T> a, System.ArraySegment<T> b) { return default(bool); }
    public static bool operator !=(System.ArraySegment<T> a, System.ArraySegment<T> b) { return default(bool); }
    void System.Collections.Generic.ICollection<T>.Add(T item) { }
    void System.Collections.Generic.ICollection<T>.Clear() { }
    bool System.Collections.Generic.ICollection<T>.Contains(T item) { return default(bool); }
    void System.Collections.Generic.ICollection<T>.CopyTo(T[] array, int arrayIndex) { }
    bool System.Collections.Generic.ICollection<T>.Remove(T item) { return default(bool); }
    System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
    int System.Collections.Generic.IList<T>.IndexOf(T item) { return default(int); }
    void System.Collections.Generic.IList<T>.Insert(int index, T item) { }
    void System.Collections.Generic.IList<T>.RemoveAt(int index) { }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
  }
  public partial class ArrayTypeMismatchException : System.SystemException {
    public ArrayTypeMismatchException() { }
    protected ArrayTypeMismatchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ArrayTypeMismatchException(string message) { }
    public ArrayTypeMismatchException(string message, System.Exception innerException) { }
  }
  public partial class AssemblyLoadEventArgs : System.EventArgs {
    public AssemblyLoadEventArgs(System.Reflection.Assembly loadedAssembly) { }
    public System.Reflection.Assembly LoadedAssembly { get { return default(System.Reflection.Assembly); } }
  }
  public delegate void AssemblyLoadEventHandler(object sender, System.AssemblyLoadEventArgs args);
  public delegate void AsyncCallback(System.IAsyncResult ar);
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767), Inherited=true, AllowMultiple=false)]
  public abstract partial class Attribute {
    protected Attribute() { }
    public virtual object TypeId { get { return default(object); } }
    [System.Security.SecuritySafeCriticalAttribute]
    public override bool Equals(object obj) { return default(bool); }
    public static System.Attribute GetCustomAttribute(System.Reflection.Assembly element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.Assembly element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.MemberInfo element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.Module element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.Module element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Assembly element) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Assembly element, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Assembly element, System.Type attributeType) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Assembly element, System.Type attributeType, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.MemberInfo element) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.MemberInfo element, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.MemberInfo element, System.Type type) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.MemberInfo element, System.Type type, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Module element) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Module element, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Module element, System.Type attributeType) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.Module element, System.Type attributeType, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.ParameterInfo element) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.ParameterInfo element, bool inherit) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Attribute[]); }
    public static System.Attribute[] GetCustomAttributes(System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    public virtual bool IsDefaultAttribute() { return default(bool); }
    public static bool IsDefined(System.Reflection.Assembly element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(System.Reflection.Assembly element, System.Type attributeType, bool inherit) { return default(bool); }
    public static bool IsDefined(System.Reflection.MemberInfo element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(bool); }
    public static bool IsDefined(System.Reflection.Module element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(System.Reflection.Module element, System.Type attributeType, bool inherit) { return default(bool); }
    public static bool IsDefined(System.Reflection.ParameterInfo element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(bool); }
    public virtual bool Match(object obj) { return default(bool); }
  }
  [System.FlagsAttribute]
  public enum AttributeTargets {
    All = 32767,
    Assembly = 1,
    Class = 4,
    Constructor = 32,
    Delegate = 4096,
    Enum = 16,
    Event = 512,
    Field = 256,
    GenericParameter = 16384,
    Interface = 1024,
    Method = 64,
    Module = 2,
    Parameter = 2048,
    Property = 128,
    ReturnValue = 8192,
    Struct = 8,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(4), Inherited=true)]
  public sealed partial class AttributeUsageAttribute : System.Attribute {
    public AttributeUsageAttribute(System.AttributeTargets validOn) { }
    public bool AllowMultiple { get { return default(bool); } set { } }
    public bool Inherited { get { return default(bool); } set { } }
    public System.AttributeTargets ValidOn { get { return default(System.AttributeTargets); } }
  }
  public partial class BadImageFormatException : System.SystemException {
    public BadImageFormatException() { }
    protected BadImageFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public BadImageFormatException(string message) { }
    public BadImageFormatException(string message, System.Exception inner) { }
    public BadImageFormatException(string message, string fileName) { }
    public BadImageFormatException(string message, string fileName, System.Exception inner) { }
    public string FileName { get { return default(string); } }
    public string FusionLog { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
    public override string Message { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  [System.FlagsAttribute]
  public enum Base64FormattingOptions {
    InsertLineBreaks = 1,
    None = 0,
  }
  public static partial class BitConverter {
    public static readonly bool IsLittleEndian;
    [System.Security.SecuritySafeCriticalAttribute]
    public static long DoubleToInt64Bits(double value) { return default(long); }
    public static byte[] GetBytes(bool value) { return default(byte[]); }
    public static byte[] GetBytes(char value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] GetBytes(double value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] GetBytes(short value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] GetBytes(int value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] GetBytes(long value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] GetBytes(float value) { return default(byte[]); }
    [System.CLSCompliantAttribute(false)]
    public static byte[] GetBytes(ushort value) { return default(byte[]); }
    [System.CLSCompliantAttribute(false)]
    public static byte[] GetBytes(uint value) { return default(byte[]); }
    [System.CLSCompliantAttribute(false)]
    public static byte[] GetBytes(ulong value) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static double Int64BitsToDouble(long value) { return default(double); }
    public static bool ToBoolean(byte[] value, int startIndex) { return default(bool); }
    public static char ToChar(byte[] value, int startIndex) { return default(char); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static double ToDouble(byte[] value, int startIndex) { return default(double); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static short ToInt16(byte[] value, int startIndex) { return default(short); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int ToInt32(byte[] value, int startIndex) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static long ToInt64(byte[] value, int startIndex) { return default(long); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static float ToSingle(byte[] value, int startIndex) { return default(float); }
    public static string ToString(byte[] value) { return default(string); }
    public static string ToString(byte[] value, int startIndex) { return default(string); }
    public static string ToString(byte[] value, int startIndex, int length) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(byte[] value, int startIndex) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(byte[] value, int startIndex) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(byte[] value, int startIndex) { return default(ulong); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Boolean : System.IComparable, System.IComparable<bool>, System.IConvertible, System.IEquatable<bool> {
    public static readonly string FalseString;
    public static readonly string TrueString;
    public int CompareTo(bool value) { return default(int); }
    public int CompareTo(object obj) { return default(int); }
    public bool Equals(bool obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static bool Parse(string value) { return default(bool); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    string System.IConvertible.ToString(System.IFormatProvider provider) { return default(string); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    public override string ToString() { return default(string); }
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string value, out bool result) { result = default(bool); return default(bool); }
  }
  public static partial class Buffer {
    public static void BlockCopy(System.Array src, int srcOffset, System.Array dst, int dstOffset, int count) { }
    public static int ByteLength(System.Array array) { return default(int); }
    public static byte GetByte(System.Array array, int index) { return default(byte); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe static void MemoryCopy(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy) { }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe static void MemoryCopy(void* source, void* destination, ulong destinationSizeInBytes, ulong sourceBytesToCopy) { }
    public static void SetByte(System.Array array, int index, byte value) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Byte : System.IComparable, System.IComparable<byte>, System.IConvertible, System.IEquatable<byte>, System.IFormattable {
    public const byte MaxValue = (byte)255;
    public const byte MinValue = (byte)0;
    public int CompareTo(byte value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public bool Equals(byte obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static byte Parse(string s) { return default(byte); }
    public static byte Parse(string s, System.Globalization.NumberStyles style) { return default(byte); }
    public static byte Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(byte); }
    public static byte Parse(string s, System.IFormatProvider provider) { return default(byte); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string s, out byte result) { result = default(byte); return default(bool); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out byte result) { result = default(byte); return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Char : System.IComparable, System.IComparable<char>, System.IConvertible, System.IEquatable<char> {
    public const char MaxValue = '\uFFFF';
    public const char MinValue = '\0';
    public int CompareTo(char value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public static string ConvertFromUtf32(int utf32) { return default(string); }
    public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) { return default(int); }
    public static int ConvertToUtf32(string s, int index) { return default(int); }
    public bool Equals(char obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static double GetNumericValue(char c) { return default(double); }
    public static double GetNumericValue(string s, int index) { return default(double); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static System.Globalization.UnicodeCategory GetUnicodeCategory(char c) { return default(System.Globalization.UnicodeCategory); }
    public static System.Globalization.UnicodeCategory GetUnicodeCategory(string s, int index) { return default(System.Globalization.UnicodeCategory); }
    public static bool IsControl(char c) { return default(bool); }
    public static bool IsControl(string s, int index) { return default(bool); }
    public static bool IsDigit(char c) { return default(bool); }
    public static bool IsDigit(string s, int index) { return default(bool); }
    public static bool IsHighSurrogate(char c) { return default(bool); }
    public static bool IsHighSurrogate(string s, int index) { return default(bool); }
    public static bool IsLetter(char c) { return default(bool); }
    public static bool IsLetter(string s, int index) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool IsLetterOrDigit(char c) { return default(bool); }
    public static bool IsLetterOrDigit(string s, int index) { return default(bool); }
    public static bool IsLower(char c) { return default(bool); }
    public static bool IsLower(string s, int index) { return default(bool); }
    public static bool IsLowSurrogate(char c) { return default(bool); }
    public static bool IsLowSurrogate(string s, int index) { return default(bool); }
    public static bool IsNumber(char c) { return default(bool); }
    public static bool IsNumber(string s, int index) { return default(bool); }
    public static bool IsPunctuation(char c) { return default(bool); }
    public static bool IsPunctuation(string s, int index) { return default(bool); }
    public static bool IsSeparator(char c) { return default(bool); }
    public static bool IsSeparator(string s, int index) { return default(bool); }
    public static bool IsSurrogate(char c) { return default(bool); }
    public static bool IsSurrogate(string s, int index) { return default(bool); }
    public static bool IsSurrogatePair(char highSurrogate, char lowSurrogate) { return default(bool); }
    public static bool IsSurrogatePair(string s, int index) { return default(bool); }
    public static bool IsSymbol(char c) { return default(bool); }
    public static bool IsSymbol(string s, int index) { return default(bool); }
    public static bool IsUpper(char c) { return default(bool); }
    public static bool IsUpper(string s, int index) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool IsWhiteSpace(char c) { return default(bool); }
    public static bool IsWhiteSpace(string s, int index) { return default(bool); }
    public static char Parse(string s) { return default(char); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    string System.IConvertible.ToString(System.IFormatProvider provider) { return default(string); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    public static char ToLower(char c) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static char ToLower(char c, System.Globalization.CultureInfo culture) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static char ToLowerInvariant(char c) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override string ToString() { return default(string); }
    public static string ToString(char c) { return default(string); }
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public static char ToUpper(char c) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static char ToUpper(char c, System.Globalization.CultureInfo culture) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static char ToUpperInvariant(char c) { return default(char); }
    public static bool TryParse(string s, out char result) { result = default(char); return default(bool); }
  }
  public sealed partial class CharEnumerator : System.Collections.Generic.IEnumerator<char>, System.Collections.IEnumerator, System.ICloneable, System.IDisposable {
    internal CharEnumerator() { }
    public char Current { get { return default(char); } }
    object System.Collections.IEnumerator.Current { get { return default(object); } }
    public object Clone() { return default(object); }
    public void Dispose() { }
    public bool MoveNext() { return default(bool); }
    public void Reset() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767), Inherited=true, AllowMultiple=false)]
  public sealed partial class CLSCompliantAttribute : System.Attribute {
    public CLSCompliantAttribute(bool isCompliant) { }
    public bool IsCompliant { get { return default(bool); } }
  }
  public delegate int Comparison<in T>(T x, T y);
  public partial class ContextMarshalException : System.SystemException {
    public ContextMarshalException() { }
    protected ContextMarshalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ContextMarshalException(string message) { }
    public ContextMarshalException(string message, System.Exception inner) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public partial class ContextStaticAttribute : System.Attribute {
    public ContextStaticAttribute() { }
  }
  public static partial class Convert {
    public static readonly object DBNull;
    public static object ChangeType(object value, System.Type conversionType) { return default(object); }
    public static object ChangeType(object value, System.Type conversionType, System.IFormatProvider provider) { return default(object); }
    public static object ChangeType(object value, System.TypeCode typeCode) { return default(object); }
    public static object ChangeType(object value, System.TypeCode typeCode, System.IFormatProvider provider) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] FromBase64CharArray(char[] inArray, int offset, int length) { return default(byte[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static byte[] FromBase64String(string s) { return default(byte[]); }
    public static System.TypeCode GetTypeCode(object value) { return default(System.TypeCode); }
    public static bool IsDBNull(object value) { return default(bool); }
    public static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut, System.Base64FormattingOptions options) { return default(int); }
    public static string ToBase64String(byte[] inArray) { return default(string); }
    public static string ToBase64String(byte[] inArray, System.Base64FormattingOptions options) { return default(string); }
    public static string ToBase64String(byte[] inArray, int offset, int length) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string ToBase64String(byte[] inArray, int offset, int length, System.Base64FormattingOptions options) { return default(string); }
    public static bool ToBoolean(bool value) { return default(bool); }
    public static bool ToBoolean(byte value) { return default(bool); }
    public static bool ToBoolean(char value) { return default(bool); }
    public static bool ToBoolean(System.DateTime value) { return default(bool); }
    public static bool ToBoolean(decimal value) { return default(bool); }
    public static bool ToBoolean(double value) { return default(bool); }
    public static bool ToBoolean(short value) { return default(bool); }
    public static bool ToBoolean(int value) { return default(bool); }
    public static bool ToBoolean(long value) { return default(bool); }
    public static bool ToBoolean(object value) { return default(bool); }
    public static bool ToBoolean(object value, System.IFormatProvider provider) { return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool ToBoolean(sbyte value) { return default(bool); }
    public static bool ToBoolean(float value) { return default(bool); }
    public static bool ToBoolean(string value) { return default(bool); }
    public static bool ToBoolean(string value, System.IFormatProvider provider) { return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool ToBoolean(ushort value) { return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool ToBoolean(uint value) { return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool ToBoolean(ulong value) { return default(bool); }
    public static byte ToByte(bool value) { return default(byte); }
    public static byte ToByte(byte value) { return default(byte); }
    public static byte ToByte(char value) { return default(byte); }
    public static byte ToByte(System.DateTime value) { return default(byte); }
    public static byte ToByte(decimal value) { return default(byte); }
    public static byte ToByte(double value) { return default(byte); }
    public static byte ToByte(short value) { return default(byte); }
    public static byte ToByte(int value) { return default(byte); }
    public static byte ToByte(long value) { return default(byte); }
    public static byte ToByte(object value) { return default(byte); }
    public static byte ToByte(object value, System.IFormatProvider provider) { return default(byte); }
    [System.CLSCompliantAttribute(false)]
    public static byte ToByte(sbyte value) { return default(byte); }
    public static byte ToByte(float value) { return default(byte); }
    public static byte ToByte(string value) { return default(byte); }
    public static byte ToByte(string value, System.IFormatProvider provider) { return default(byte); }
    public static byte ToByte(string value, int fromBase) { return default(byte); }
    [System.CLSCompliantAttribute(false)]
    public static byte ToByte(ushort value) { return default(byte); }
    [System.CLSCompliantAttribute(false)]
    public static byte ToByte(uint value) { return default(byte); }
    [System.CLSCompliantAttribute(false)]
    public static byte ToByte(ulong value) { return default(byte); }
    public static char ToChar(bool value) { return default(char); }
    public static char ToChar(byte value) { return default(char); }
    public static char ToChar(char value) { return default(char); }
    public static char ToChar(System.DateTime value) { return default(char); }
    public static char ToChar(decimal value) { return default(char); }
    public static char ToChar(double value) { return default(char); }
    public static char ToChar(short value) { return default(char); }
    public static char ToChar(int value) { return default(char); }
    public static char ToChar(long value) { return default(char); }
    public static char ToChar(object value) { return default(char); }
    public static char ToChar(object value, System.IFormatProvider provider) { return default(char); }
    [System.CLSCompliantAttribute(false)]
    public static char ToChar(sbyte value) { return default(char); }
    public static char ToChar(float value) { return default(char); }
    public static char ToChar(string value) { return default(char); }
    public static char ToChar(string value, System.IFormatProvider provider) { return default(char); }
    [System.CLSCompliantAttribute(false)]
    public static char ToChar(ushort value) { return default(char); }
    [System.CLSCompliantAttribute(false)]
    public static char ToChar(uint value) { return default(char); }
    [System.CLSCompliantAttribute(false)]
    public static char ToChar(ulong value) { return default(char); }
    public static System.DateTime ToDateTime(bool value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(byte value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(char value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(System.DateTime value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(decimal value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(double value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(short value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(int value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(long value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(object value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(object value, System.IFormatProvider provider) { return default(System.DateTime); }
    [System.CLSCompliantAttribute(false)]
    public static System.DateTime ToDateTime(sbyte value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(float value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(string value) { return default(System.DateTime); }
    public static System.DateTime ToDateTime(string value, System.IFormatProvider provider) { return default(System.DateTime); }
    [System.CLSCompliantAttribute(false)]
    public static System.DateTime ToDateTime(ushort value) { return default(System.DateTime); }
    [System.CLSCompliantAttribute(false)]
    public static System.DateTime ToDateTime(uint value) { return default(System.DateTime); }
    [System.CLSCompliantAttribute(false)]
    public static System.DateTime ToDateTime(ulong value) { return default(System.DateTime); }
    public static decimal ToDecimal(bool value) { return default(decimal); }
    public static decimal ToDecimal(byte value) { return default(decimal); }
    public static decimal ToDecimal(char value) { return default(decimal); }
    public static decimal ToDecimal(System.DateTime value) { return default(decimal); }
    public static decimal ToDecimal(decimal value) { return default(decimal); }
    public static decimal ToDecimal(double value) { return default(decimal); }
    public static decimal ToDecimal(short value) { return default(decimal); }
    public static decimal ToDecimal(int value) { return default(decimal); }
    public static decimal ToDecimal(long value) { return default(decimal); }
    public static decimal ToDecimal(object value) { return default(decimal); }
    public static decimal ToDecimal(object value, System.IFormatProvider provider) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static decimal ToDecimal(sbyte value) { return default(decimal); }
    public static decimal ToDecimal(float value) { return default(decimal); }
    public static decimal ToDecimal(string value) { return default(decimal); }
    public static decimal ToDecimal(string value, System.IFormatProvider provider) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static decimal ToDecimal(ushort value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static decimal ToDecimal(uint value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static decimal ToDecimal(ulong value) { return default(decimal); }
    public static double ToDouble(bool value) { return default(double); }
    public static double ToDouble(byte value) { return default(double); }
    public static double ToDouble(char value) { return default(double); }
    public static double ToDouble(System.DateTime value) { return default(double); }
    public static double ToDouble(decimal value) { return default(double); }
    public static double ToDouble(double value) { return default(double); }
    public static double ToDouble(short value) { return default(double); }
    public static double ToDouble(int value) { return default(double); }
    public static double ToDouble(long value) { return default(double); }
    public static double ToDouble(object value) { return default(double); }
    public static double ToDouble(object value, System.IFormatProvider provider) { return default(double); }
    [System.CLSCompliantAttribute(false)]
    public static double ToDouble(sbyte value) { return default(double); }
    public static double ToDouble(float value) { return default(double); }
    public static double ToDouble(string value) { return default(double); }
    public static double ToDouble(string value, System.IFormatProvider provider) { return default(double); }
    [System.CLSCompliantAttribute(false)]
    public static double ToDouble(ushort value) { return default(double); }
    [System.CLSCompliantAttribute(false)]
    public static double ToDouble(uint value) { return default(double); }
    [System.CLSCompliantAttribute(false)]
    public static double ToDouble(ulong value) { return default(double); }
    public static short ToInt16(bool value) { return default(short); }
    public static short ToInt16(byte value) { return default(short); }
    public static short ToInt16(char value) { return default(short); }
    public static short ToInt16(System.DateTime value) { return default(short); }
    public static short ToInt16(decimal value) { return default(short); }
    public static short ToInt16(double value) { return default(short); }
    public static short ToInt16(short value) { return default(short); }
    public static short ToInt16(int value) { return default(short); }
    public static short ToInt16(long value) { return default(short); }
    public static short ToInt16(object value) { return default(short); }
    public static short ToInt16(object value, System.IFormatProvider provider) { return default(short); }
    [System.CLSCompliantAttribute(false)]
    public static short ToInt16(sbyte value) { return default(short); }
    public static short ToInt16(float value) { return default(short); }
    public static short ToInt16(string value) { return default(short); }
    public static short ToInt16(string value, System.IFormatProvider provider) { return default(short); }
    public static short ToInt16(string value, int fromBase) { return default(short); }
    [System.CLSCompliantAttribute(false)]
    public static short ToInt16(ushort value) { return default(short); }
    [System.CLSCompliantAttribute(false)]
    public static short ToInt16(uint value) { return default(short); }
    [System.CLSCompliantAttribute(false)]
    public static short ToInt16(ulong value) { return default(short); }
    public static int ToInt32(bool value) { return default(int); }
    public static int ToInt32(byte value) { return default(int); }
    public static int ToInt32(char value) { return default(int); }
    public static int ToInt32(System.DateTime value) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int ToInt32(decimal value) { return default(int); }
    public static int ToInt32(double value) { return default(int); }
    public static int ToInt32(short value) { return default(int); }
    public static int ToInt32(int value) { return default(int); }
    public static int ToInt32(long value) { return default(int); }
    public static int ToInt32(object value) { return default(int); }
    public static int ToInt32(object value, System.IFormatProvider provider) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    public static int ToInt32(sbyte value) { return default(int); }
    public static int ToInt32(float value) { return default(int); }
    public static int ToInt32(string value) { return default(int); }
    public static int ToInt32(string value, System.IFormatProvider provider) { return default(int); }
    public static int ToInt32(string value, int fromBase) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    public static int ToInt32(ushort value) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    public static int ToInt32(uint value) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    public static int ToInt32(ulong value) { return default(int); }
    public static long ToInt64(bool value) { return default(long); }
    public static long ToInt64(byte value) { return default(long); }
    public static long ToInt64(char value) { return default(long); }
    public static long ToInt64(System.DateTime value) { return default(long); }
    public static long ToInt64(decimal value) { return default(long); }
    public static long ToInt64(double value) { return default(long); }
    public static long ToInt64(short value) { return default(long); }
    public static long ToInt64(int value) { return default(long); }
    public static long ToInt64(long value) { return default(long); }
    public static long ToInt64(object value) { return default(long); }
    public static long ToInt64(object value, System.IFormatProvider provider) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static long ToInt64(sbyte value) { return default(long); }
    public static long ToInt64(float value) { return default(long); }
    public static long ToInt64(string value) { return default(long); }
    public static long ToInt64(string value, System.IFormatProvider provider) { return default(long); }
    public static long ToInt64(string value, int fromBase) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static long ToInt64(ushort value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static long ToInt64(uint value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static long ToInt64(ulong value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(bool value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(byte value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(char value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(System.DateTime value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(decimal value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(double value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(short value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(int value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(long value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(object value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(object value, System.IFormatProvider provider) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(sbyte value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(float value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(string value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(string value, System.IFormatProvider provider) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(string value, int fromBase) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(ushort value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(uint value) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(ulong value) { return default(sbyte); }
    public static float ToSingle(bool value) { return default(float); }
    public static float ToSingle(byte value) { return default(float); }
    public static float ToSingle(char value) { return default(float); }
    public static float ToSingle(System.DateTime value) { return default(float); }
    public static float ToSingle(decimal value) { return default(float); }
    public static float ToSingle(double value) { return default(float); }
    public static float ToSingle(short value) { return default(float); }
    public static float ToSingle(int value) { return default(float); }
    public static float ToSingle(long value) { return default(float); }
    public static float ToSingle(object value) { return default(float); }
    public static float ToSingle(object value, System.IFormatProvider provider) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    public static float ToSingle(sbyte value) { return default(float); }
    public static float ToSingle(float value) { return default(float); }
    public static float ToSingle(string value) { return default(float); }
    public static float ToSingle(string value, System.IFormatProvider provider) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    public static float ToSingle(ushort value) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    public static float ToSingle(uint value) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    public static float ToSingle(ulong value) { return default(float); }
    public static string ToString(bool value) { return default(string); }
    public static string ToString(bool value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(byte value) { return default(string); }
    public static string ToString(byte value, System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string ToString(byte value, int toBase) { return default(string); }
    public static string ToString(char value) { return default(string); }
    public static string ToString(char value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(System.DateTime value) { return default(string); }
    public static string ToString(System.DateTime value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(decimal value) { return default(string); }
    public static string ToString(decimal value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(double value) { return default(string); }
    public static string ToString(double value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(short value) { return default(string); }
    public static string ToString(short value, System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string ToString(short value, int toBase) { return default(string); }
    public static string ToString(int value) { return default(string); }
    public static string ToString(int value, System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string ToString(int value, int toBase) { return default(string); }
    public static string ToString(long value) { return default(string); }
    public static string ToString(long value, System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string ToString(long value, int toBase) { return default(string); }
    public static string ToString(object value) { return default(string); }
    public static string ToString(object value, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(sbyte value) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(sbyte value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(float value) { return default(string); }
    public static string ToString(float value, System.IFormatProvider provider) { return default(string); }
    public static string ToString(string value) { return default(string); }
    public static string ToString(string value, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(ushort value) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(ushort value, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(uint value) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(uint value, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(ulong value) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string ToString(ulong value, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(bool value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(byte value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(char value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(System.DateTime value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(decimal value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(double value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(short value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(int value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(long value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(object value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(object value, System.IFormatProvider provider) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(sbyte value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(float value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(string value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(string value, System.IFormatProvider provider) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(string value, int fromBase) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(ushort value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(uint value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(ulong value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(bool value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(byte value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(char value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(System.DateTime value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(decimal value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(double value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(short value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(int value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(long value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(object value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(object value, System.IFormatProvider provider) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(sbyte value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(float value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(string value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(string value, System.IFormatProvider provider) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(string value, int fromBase) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(ushort value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(uint value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint ToUInt32(ulong value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(bool value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(byte value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(char value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(System.DateTime value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(decimal value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(double value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(short value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(int value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(long value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(object value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(object value, System.IFormatProvider provider) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(sbyte value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(float value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(string value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(string value, System.IFormatProvider provider) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(string value, int fromBase) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(ushort value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(uint value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong ToUInt64(ulong value) { return default(ulong); }
  }
  public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
  public sealed partial class DataMisalignedException : System.SystemException {
    public DataMisalignedException() { }
    public DataMisalignedException(string message) { }
    public DataMisalignedException(string message, System.Exception innerException) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
  public partial struct DateTime : System.IComparable, System.IComparable<System.DateTime>, System.IConvertible, System.IEquatable<System.DateTime>, System.IFormattable, System.Runtime.Serialization.ISerializable {
    public static readonly System.DateTime MaxValue;
    public static readonly System.DateTime MinValue;
    public DateTime(int year, int month, int day) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, System.Globalization.Calendar calendar) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, System.DateTimeKind kind) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, System.Globalization.Calendar calendar) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, System.DateTimeKind kind) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, System.Globalization.Calendar calendar) { throw new System.NotImplementedException(); }
    public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, System.Globalization.Calendar calendar, System.DateTimeKind kind) { throw new System.NotImplementedException(); }
    public DateTime(long ticks) { throw new System.NotImplementedException(); }
    public DateTime(long ticks, System.DateTimeKind kind) { throw new System.NotImplementedException(); }
    public System.DateTime Date { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.DateTime); } }
    public int Day { get { return default(int); } }
    public System.DayOfWeek DayOfWeek { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.DayOfWeek); } }
    public int DayOfYear { get { return default(int); } }
    public int Hour { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public System.DateTimeKind Kind { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.DateTimeKind); } }
    public int Millisecond { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public int Minute { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public int Month { get { return default(int); } }
    public static System.DateTime Now { get { return default(System.DateTime); } }
    public int Second { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public long Ticks { get { return default(long); } }
    public System.TimeSpan TimeOfDay { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.TimeSpan); } }
    public static System.DateTime Today { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.DateTime); } }
    public static System.DateTime UtcNow { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries"), System.Security.SecuritySafeCriticalAttribute]get { return default(System.DateTime); } }
    public int Year { get { return default(int); } }
    public System.DateTime Add(System.TimeSpan value) { return default(System.DateTime); }
    public System.DateTime AddDays(double value) { return default(System.DateTime); }
    public System.DateTime AddHours(double value) { return default(System.DateTime); }
    public System.DateTime AddMilliseconds(double value) { return default(System.DateTime); }
    public System.DateTime AddMinutes(double value) { return default(System.DateTime); }
    public System.DateTime AddMonths(int months) { return default(System.DateTime); }
    public System.DateTime AddSeconds(double value) { return default(System.DateTime); }
    public System.DateTime AddTicks(long value) { return default(System.DateTime); }
    public System.DateTime AddYears(int value) { return default(System.DateTime); }
    public static int Compare(System.DateTime t1, System.DateTime t2) { return default(int); }
    public int CompareTo(System.DateTime value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public static int DaysInMonth(int year, int month) { return default(int); }
    public bool Equals(System.DateTime value) { return default(bool); }
    public static bool Equals(System.DateTime t1, System.DateTime t2) { return default(bool); }
    public override bool Equals(object value) { return default(bool); }
    public static System.DateTime FromBinary(long dateData) { return default(System.DateTime); }
    public static System.DateTime FromFileTime(long fileTime) { return default(System.DateTime); }
    public static System.DateTime FromFileTimeUtc(long fileTime) { return default(System.DateTime); }
    public static System.DateTime FromOADate(double d) { return default(System.DateTime); }
    public string[] GetDateTimeFormats() { return default(string[]); }
    public string[] GetDateTimeFormats(char format) { return default(string[]); }
    public string[] GetDateTimeFormats(char format, System.IFormatProvider provider) { return default(string[]); }
    public string[] GetDateTimeFormats(System.IFormatProvider provider) { return default(string[]); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override int GetHashCode() { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public bool IsDaylightSavingTime() { return default(bool); }
    public static bool IsLeapYear(int year) { return default(bool); }
    public static System.DateTime operator +(System.DateTime d, System.TimeSpan t) { return default(System.DateTime); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator ==(System.DateTime d1, System.DateTime d2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator >(System.DateTime t1, System.DateTime t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator >=(System.DateTime t1, System.DateTime t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator !=(System.DateTime d1, System.DateTime d2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator <(System.DateTime t1, System.DateTime t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator <=(System.DateTime t1, System.DateTime t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static System.TimeSpan operator -(System.DateTime d1, System.DateTime d2) { return default(System.TimeSpan); }
    public static System.DateTime operator -(System.DateTime d, System.TimeSpan t) { return default(System.DateTime); }
    public static System.DateTime Parse(string s) { return default(System.DateTime); }
    public static System.DateTime Parse(string s, System.IFormatProvider provider) { return default(System.DateTime); }
    public static System.DateTime Parse(string s, System.IFormatProvider provider, System.Globalization.DateTimeStyles styles) { return default(System.DateTime); }
    public static System.DateTime ParseExact(string s, string format, System.IFormatProvider provider) { return default(System.DateTime); }
    public static System.DateTime ParseExact(string s, string format, System.IFormatProvider provider, System.Globalization.DateTimeStyles style) { return default(System.DateTime); }
    public static System.DateTime ParseExact(string s, string[] formats, System.IFormatProvider provider, System.Globalization.DateTimeStyles style) { return default(System.DateTime); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static System.DateTime SpecifyKind(System.DateTime value, System.DateTimeKind kind) { return default(System.DateTime); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.TimeSpan Subtract(System.DateTime value) { return default(System.TimeSpan); }
    public System.DateTime Subtract(System.TimeSpan value) { return default(System.DateTime); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecurityCriticalAttribute]
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public long ToBinary() { return default(long); }
    public long ToFileTime() { return default(long); }
    public long ToFileTimeUtc() { return default(long); }
    public System.DateTime ToLocalTime() { return default(System.DateTime); }
    public string ToLongDateString() { return default(string); }
    public string ToLongTimeString() { return default(string); }
    public double ToOADate() { return default(double); }
    public string ToShortDateString() { return default(string); }
    public string ToShortTimeString() { return default(string); }
    public override string ToString() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public System.DateTime ToUniversalTime() { return default(System.DateTime); }
    public static bool TryParse(string s, out System.DateTime result) { result = default(System.DateTime); return default(bool); }
    public static bool TryParse(string s, System.IFormatProvider provider, System.Globalization.DateTimeStyles styles, out System.DateTime result) { result = default(System.DateTime); return default(bool); }
    public static bool TryParseExact(string s, string format, System.IFormatProvider provider, System.Globalization.DateTimeStyles style, out System.DateTime result) { result = default(System.DateTime); return default(bool); }
    public static bool TryParseExact(string s, string[] formats, System.IFormatProvider provider, System.Globalization.DateTimeStyles style, out System.DateTime result) { result = default(System.DateTime); return default(bool); }
  }
  public enum DateTimeKind {
    Local = 2,
    Unspecified = 0,
    Utc = 1,
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
  public partial struct DateTimeOffset : System.IComparable, System.IComparable<System.DateTimeOffset>, System.IEquatable<System.DateTimeOffset>, System.IFormattable, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
    public static readonly System.DateTimeOffset MaxValue;
    public static readonly System.DateTimeOffset MinValue;
    public DateTimeOffset(System.DateTime dateTime) { throw new System.NotImplementedException(); }
    public DateTimeOffset(System.DateTime dateTime, System.TimeSpan offset) { throw new System.NotImplementedException(); }
    public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, System.Globalization.Calendar calendar, System.TimeSpan offset) { throw new System.NotImplementedException(); }
    public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, System.TimeSpan offset) { throw new System.NotImplementedException(); }
    public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, System.TimeSpan offset) { throw new System.NotImplementedException(); }
    public DateTimeOffset(long ticks, System.TimeSpan offset) { throw new System.NotImplementedException(); }
    public System.DateTime Date { get { return default(System.DateTime); } }
    public System.DateTime DateTime { get { return default(System.DateTime); } }
    public int Day { get { return default(int); } }
    public System.DayOfWeek DayOfWeek { get { return default(System.DayOfWeek); } }
    public int DayOfYear { get { return default(int); } }
    public int Hour { get { return default(int); } }
    public System.DateTime LocalDateTime { get { return default(System.DateTime); } }
    public int Millisecond { get { return default(int); } }
    public int Minute { get { return default(int); } }
    public int Month { get { return default(int); } }
    public static System.DateTimeOffset Now { get { return default(System.DateTimeOffset); } }
    public System.TimeSpan Offset { get { return default(System.TimeSpan); } }
    public int Second { get { return default(int); } }
    public long Ticks { get { return default(long); } }
    public System.TimeSpan TimeOfDay { get { return default(System.TimeSpan); } }
    public System.DateTime UtcDateTime { get { return default(System.DateTime); } }
    public static System.DateTimeOffset UtcNow { get { return default(System.DateTimeOffset); } }
    public long UtcTicks { get { return default(long); } }
    public int Year { get { return default(int); } }
    public System.DateTimeOffset Add(System.TimeSpan timeSpan) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddDays(double days) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddHours(double hours) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddMilliseconds(double milliseconds) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddMinutes(double minutes) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddMonths(int months) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddSeconds(double seconds) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddTicks(long ticks) { return default(System.DateTimeOffset); }
    public System.DateTimeOffset AddYears(int years) { return default(System.DateTimeOffset); }
    public static int Compare(System.DateTimeOffset first, System.DateTimeOffset second) { return default(int); }
    public int CompareTo(System.DateTimeOffset other) { return default(int); }
    public bool Equals(System.DateTimeOffset other) { return default(bool); }
    public static bool Equals(System.DateTimeOffset first, System.DateTimeOffset second) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public bool EqualsExact(System.DateTimeOffset other) { return default(bool); }
    public static System.DateTimeOffset FromFileTime(long fileTime) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset FromUnixTimeMilliseconds(long milliseconds) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset FromUnixTimeSeconds(long seconds) { return default(System.DateTimeOffset); }
    public override int GetHashCode() { return default(int); }
    public static System.DateTimeOffset operator +(System.DateTimeOffset dateTimeOffset, System.TimeSpan timeSpan) { return default(System.DateTimeOffset); }
    public static bool operator ==(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static bool operator >(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static bool operator >=(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static implicit operator System.DateTimeOffset (System.DateTime dateTime) { return default(System.DateTimeOffset); }
    public static bool operator !=(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static bool operator <(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static bool operator <=(System.DateTimeOffset left, System.DateTimeOffset right) { return default(bool); }
    public static System.TimeSpan operator -(System.DateTimeOffset left, System.DateTimeOffset right) { return default(System.TimeSpan); }
    public static System.DateTimeOffset operator -(System.DateTimeOffset dateTimeOffset, System.TimeSpan timeSpan) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset Parse(string input) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset Parse(string input, System.IFormatProvider formatProvider) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset Parse(string input, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset ParseExact(string input, string format, System.IFormatProvider formatProvider) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset ParseExact(string input, string format, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles) { return default(System.DateTimeOffset); }
    public static System.DateTimeOffset ParseExact(string input, string[] formats, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles) { return default(System.DateTimeOffset); }
    public System.TimeSpan Subtract(System.DateTimeOffset value) { return default(System.TimeSpan); }
    public System.DateTimeOffset Subtract(System.TimeSpan value) { return default(System.DateTimeOffset); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    [System.Security.SecurityCriticalAttribute]
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public long ToFileTime() { return default(long); }
    public System.DateTimeOffset ToLocalTime() { return default(System.DateTimeOffset); }
    public System.DateTimeOffset ToOffset(System.TimeSpan offset) { return default(System.DateTimeOffset); }
    public override string ToString() { return default(string); }
    public string ToString(System.IFormatProvider formatProvider) { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider formatProvider) { return default(string); }
    public System.DateTimeOffset ToUniversalTime() { return default(System.DateTimeOffset); }
    public long ToUnixTimeMilliseconds() { return default(long); }
    public long ToUnixTimeSeconds() { return default(long); }
    public static bool TryParse(string input, out System.DateTimeOffset result) { result = default(System.DateTimeOffset); return default(bool); }
    public static bool TryParse(string input, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles, out System.DateTimeOffset result) { result = default(System.DateTimeOffset); return default(bool); }
    public static bool TryParseExact(string input, string format, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles, out System.DateTimeOffset result) { result = default(System.DateTimeOffset); return default(bool); }
    public static bool TryParseExact(string input, string[] formats, System.IFormatProvider formatProvider, System.Globalization.DateTimeStyles styles, out System.DateTimeOffset result) { result = default(System.DateTimeOffset); return default(bool); }
  }
  public enum DayOfWeek {
    Friday = 5,
    Monday = 1,
    Saturday = 6,
    Sunday = 0,
    Thursday = 4,
    Tuesday = 2,
    Wednesday = 3,
  }
  public sealed partial class DBNull : System.IConvertible, System.Runtime.Serialization.ISerializable {
    internal DBNull() { }
    public static readonly System.DBNull Value;
    [System.Security.SecurityCriticalAttribute]
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    public override string ToString() { return default(string); }
    public string ToString(System.IFormatProvider provider) { return default(string); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Decimal : System.IComparable, System.IComparable<decimal>, System.IConvertible, System.IEquatable<decimal>, System.IFormattable, System.Runtime.Serialization.IDeserializationCallback {
    [System.Runtime.CompilerServices.DecimalConstantAttribute((byte)0, (byte)0, (uint)4294967295, (uint)4294967295, (uint)4294967295)]
    public static readonly decimal MaxValue;
    [System.Runtime.CompilerServices.DecimalConstantAttribute((byte)0, (byte)128, (uint)0, (uint)0, (uint)1)]
    public static readonly decimal MinusOne;
    [System.Runtime.CompilerServices.DecimalConstantAttribute((byte)0, (byte)128, (uint)4294967295, (uint)4294967295, (uint)4294967295)]
    public static readonly decimal MinValue;
    [System.Runtime.CompilerServices.DecimalConstantAttribute((byte)0, (byte)0, (uint)0, (uint)0, (uint)1)]
    public static readonly decimal One;
    [System.Runtime.CompilerServices.DecimalConstantAttribute((byte)0, (byte)0, (uint)0, (uint)0, (uint)0)]
    public static readonly decimal Zero;
    [System.Security.SecuritySafeCriticalAttribute]
    public Decimal(double value) { throw new System.NotImplementedException(); }
    public Decimal(int value) { throw new System.NotImplementedException(); }
    public Decimal(int lo, int mid, int hi, bool isNegative, byte scale) { throw new System.NotImplementedException(); }
    public Decimal(int[] bits) { throw new System.NotImplementedException(); }
    public Decimal(long value) { throw new System.NotImplementedException(); }
    [System.Security.SecuritySafeCriticalAttribute]
    public Decimal(float value) { throw new System.NotImplementedException(); }
    [System.CLSCompliantAttribute(false)]
    public Decimal(uint value) { throw new System.NotImplementedException(); }
    [System.CLSCompliantAttribute(false)]
    public Decimal(ulong value) { throw new System.NotImplementedException(); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Add(decimal d1, decimal d2) { return default(decimal); }
    public static decimal Ceiling(decimal d) { return default(decimal); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecuritySafeCriticalAttribute]
    public static int Compare(decimal d1, decimal d2) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public int CompareTo(decimal value) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public int CompareTo(object value) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Divide(decimal d1, decimal d2) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool Equals(decimal value) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool Equals(decimal d1, decimal d2) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override bool Equals(object value) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Floor(decimal d) { return default(decimal); }
    public static decimal FromOACurrency(long cy) { return default(decimal); }
    public static int[] GetBits(decimal d) { return default(int[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Multiply(decimal d1, decimal d2) { return default(decimal); }
    public static decimal Negate(decimal d) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal operator +(decimal d1, decimal d2) { return default(decimal); }
    public static decimal operator --(decimal d) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal operator /(decimal d1, decimal d2) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator ==(decimal d1, decimal d2) { return default(bool); }
    public static explicit operator byte (decimal value) { return default(byte); }
    public static explicit operator char (decimal value) { return default(char); }
    public static explicit operator double (decimal value) { return default(double); }
    public static explicit operator short (decimal value) { return default(short); }
    public static explicit operator int (decimal value) { return default(int); }
    public static explicit operator long (decimal value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static explicit operator sbyte (decimal value) { return default(sbyte); }
    public static explicit operator float (decimal value) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    public static explicit operator ushort (decimal value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static explicit operator uint (decimal value) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static explicit operator ulong (decimal value) { return default(ulong); }
    public static explicit operator decimal (double value) { return default(decimal); }
    public static explicit operator decimal (float value) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator >(decimal d1, decimal d2) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator >=(decimal d1, decimal d2) { return default(bool); }
    public static implicit operator decimal (byte value) { return default(decimal); }
    public static implicit operator decimal (char value) { return default(decimal); }
    public static implicit operator decimal (short value) { return default(decimal); }
    public static implicit operator decimal (int value) { return default(decimal); }
    public static implicit operator decimal (long value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static implicit operator decimal (sbyte value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static implicit operator decimal (ushort value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static implicit operator decimal (uint value) { return default(decimal); }
    [System.CLSCompliantAttribute(false)]
    public static implicit operator decimal (ulong value) { return default(decimal); }
    public static decimal operator ++(decimal d) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator !=(decimal d1, decimal d2) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator <(decimal d1, decimal d2) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool operator <=(decimal d1, decimal d2) { return default(bool); }
    public static decimal operator %(decimal d1, decimal d2) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal operator *(decimal d1, decimal d2) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal operator -(decimal d1, decimal d2) { return default(decimal); }
    public static decimal operator -(decimal d) { return default(decimal); }
    public static decimal operator +(decimal d) { return default(decimal); }
    public static decimal Parse(string s) { return default(decimal); }
    public static decimal Parse(string s, System.Globalization.NumberStyles style) { return default(decimal); }
    public static decimal Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(decimal); }
    public static decimal Parse(string s, System.IFormatProvider provider) { return default(decimal); }
    public static decimal Remainder(decimal d1, decimal d2) { return default(decimal); }
    public static decimal Round(decimal d) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Round(decimal d, int decimals) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Round(decimal d, int decimals, System.MidpointRounding mode) { return default(decimal); }
    public static decimal Round(decimal d, System.MidpointRounding mode) { return default(decimal); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Subtract(decimal d1, decimal d2) { return default(decimal); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    public static byte ToByte(decimal value) { return default(byte); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static double ToDouble(decimal d) { return default(double); }
    public static short ToInt16(decimal value) { return default(short); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int ToInt32(decimal d) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static long ToInt64(decimal d) { return default(long); }
    public static long ToOACurrency(decimal value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte ToSByte(decimal value) { return default(sbyte); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static float ToSingle(decimal d) { return default(float); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static ushort ToUInt16(decimal value) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static uint ToUInt32(decimal d) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static ulong ToUInt64(decimal d) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static decimal Truncate(decimal d) { return default(decimal); }
    public static bool TryParse(string s, out decimal result) { result = default(decimal); return default(bool); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out decimal result) { result = default(decimal); return default(bool); }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(2))]
  public abstract partial class Delegate : System.ICloneable, System.Runtime.Serialization.ISerializable {
    protected Delegate(object target, string method) { }
    protected Delegate(System.Type target, string method) { }
    public System.Reflection.MethodInfo Method { get { return default(System.Reflection.MethodInfo); } }
    public object Target { get { return default(object); } }
    public virtual object Clone() { return default(object); }
    public static System.Delegate Combine(System.Delegate a, System.Delegate b) { return default(System.Delegate); }
    public static System.Delegate Combine(params System.Delegate[] delegates) { return default(System.Delegate); }
    protected virtual System.Delegate CombineImpl(System.Delegate d) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, object firstArgument, System.Reflection.MethodInfo method) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, object firstArgument, System.Reflection.MethodInfo method, bool throwOnBindFailure) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, object target, string method) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, object target, string method, bool ignoreCase) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, System.Reflection.MethodInfo method) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, System.Reflection.MethodInfo method, bool throwOnBindFailure) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, System.Type target, string method) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, System.Type target, string method, bool ignoreCase) { return default(System.Delegate); }
    public static System.Delegate CreateDelegate(System.Type type, System.Type target, string method, bool ignoreCase, bool throwOnBindFailure) { return default(System.Delegate); }
    public object DynamicInvoke(params object[] args) { return default(object); }
    protected virtual object DynamicInvokeImpl(object[] args) { return default(object); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public virtual System.Delegate[] GetInvocationList() { return default(System.Delegate[]); }
    protected virtual System.Reflection.MethodInfo GetMethodImpl() { return default(System.Reflection.MethodInfo); }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public static bool operator ==(System.Delegate d1, System.Delegate d2) { return default(bool); }
    public static bool operator !=(System.Delegate d1, System.Delegate d2) { return default(bool); }
    public static System.Delegate Remove(System.Delegate source, System.Delegate value) { return default(System.Delegate); }
    public static System.Delegate RemoveAll(System.Delegate source, System.Delegate value) { return default(System.Delegate); }
    protected virtual System.Delegate RemoveImpl(System.Delegate d) { return default(System.Delegate); }
  }
  public partial class DivideByZeroException : System.ArithmeticException {
    public DivideByZeroException() { }
    protected DivideByZeroException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public DivideByZeroException(string message) { }
    public DivideByZeroException(string message, System.Exception innerException) { }
  }
  public partial class DllNotFoundException : System.TypeLoadException {
    public DllNotFoundException() { }
    protected DllNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public DllNotFoundException(string message) { }
    public DllNotFoundException(string message, System.Exception inner) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Double : System.IComparable, System.IComparable<double>, System.IConvertible, System.IEquatable<double>, System.IFormattable {
    public const double Epsilon = 4.94065645841247E-324;
    public const double MaxValue = 1.7976931348623157E+308;
    public const double MinValue = -1.7976931348623157E+308;
    public const double NaN = 0.0 / 0.0;
    public const double NegativeInfinity = -1.0 / 0.0;
    public const double PositiveInfinity = 1.0 / 0.0;
    public int CompareTo(double value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public bool Equals(double obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsInfinity(double d) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsNaN(double d) { return default(bool); }
    public static bool IsNegativeInfinity(double d) { return default(bool); }
    public static bool IsPositiveInfinity(double d) { return default(bool); }
    public static bool operator ==(double left, double right) { return default(bool); }
    public static bool operator >(double left, double right) { return default(bool); }
    public static bool operator >=(double left, double right) { return default(bool); }
    public static bool operator !=(double left, double right) { return default(bool); }
    public static bool operator <(double left, double right) { return default(bool); }
    public static bool operator <=(double left, double right) { return default(bool); }
    public static double Parse(string s) { return default(double); }
    public static double Parse(string s, System.Globalization.NumberStyles style) { return default(double); }
    public static double Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(double); }
    public static double Parse(string s, System.IFormatProvider provider) { return default(double); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string s, out double result) { result = default(double); return default(bool); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out double result) { result = default(double); return default(bool); }
  }
  public partial class DuplicateWaitObjectException : System.ArgumentException {
    public DuplicateWaitObjectException() { }
    protected DuplicateWaitObjectException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public DuplicateWaitObjectException(string parameterName) { }
    public DuplicateWaitObjectException(string message, System.Exception innerException) { }
    public DuplicateWaitObjectException(string parameterName, string message) { }
  }
  public partial class EntryPointNotFoundException : System.TypeLoadException {
    public EntryPointNotFoundException() { }
    protected EntryPointNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public EntryPointNotFoundException(string message) { }
    public EntryPointNotFoundException(string message, System.Exception inner) { }
  }
  public abstract partial class Enum : System.ValueType, System.IComparable, System.IConvertible, System.IFormattable {
    protected Enum() { }
    [System.Security.SecuritySafeCriticalAttribute]
    public int CompareTo(object target) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public static string Format(System.Type enumType, object value, string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    public static string GetName(System.Type enumType, object value) { return default(string); }
    public static string[] GetNames(System.Type enumType) { return default(string[]); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static System.Type GetUnderlyingType(System.Type enumType) { return default(System.Type); }
    public static System.Array GetValues(System.Type enumType) { return default(System.Array); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool HasFlag(System.Enum flag) { return default(bool); }
    public static bool IsDefined(System.Type enumType, object value) { return default(bool); }
    public static object Parse(System.Type enumType, string value) { return default(object); }
    public static object Parse(System.Type enumType, string value, bool ignoreCase) { return default(object); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    [System.ObsoleteAttribute("The provider argument is not used. Please use ToString().")]
    string System.IConvertible.ToString(System.IFormatProvider provider) { return default(string); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.ObsoleteAttribute("The provider argument is not used. Please use ToString(String).")]
    string System.IFormattable.ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, byte value) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, short value) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, int value) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, long value) { return default(object); }
    public static object ToObject(System.Type enumType, object value) { return default(object); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, sbyte value) { return default(object); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, ushort value) { return default(object); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, uint value) { return default(object); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.Type enumType, ulong value) { return default(object); }
    public override string ToString() { return default(string); }
    [System.ObsoleteAttribute("The provider argument is not used. Please use ToString().")]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public string ToString(string format) { return default(string); }
    [System.ObsoleteAttribute("The provider argument is not used. Please use ToString(String).")]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct { result = default(TEnum); return default(bool); }
    public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct { result = default(TEnum); return default(bool); }
  }
  public static partial class Environment {
    public static string CommandLine { get { return default(string); } }
    public static string CurrentDirectory { get { return default(string); } set { } }
    public static int CurrentManagedThreadId { get { return default(int); } }
    public static int ExitCode { get { return default(int); } set { } }
    public static bool HasShutdownStarted { get { return default(bool); } }
    public static bool Is64BitOperatingSystem { get { return default(bool); } }
    public static bool Is64BitProcess { get { return default(bool); } }
    public static string MachineName { get { return default(string); } }
    public static string NewLine { get { return default(string); } }
    public static System.OperatingSystem OSVersion { get { return default(System.OperatingSystem); } }
    public static int ProcessorCount { get { return default(int); } }
    public static string StackTrace { get { return default(string); } }
    public static int SystemPageSize { get { return default(int); } }
    public static int TickCount { get { return default(int); } }
    public static string UserDomainName { get { return default(string); } }
    public static bool UserInteractive { get { return default(bool); } }
    public static string UserName { get { return default(string); } }
    public static System.Version Version { get { return default(System.Version); } }
    public static long WorkingSet { get { return default(long); } }
    public static void Exit(int exitCode) { }
    public static string ExpandEnvironmentVariables(string name) { return default(string); }
    public static void FailFast(string message) { }
    [System.Security.SecurityCriticalAttribute]
    public static void FailFast(string message, System.Exception exception) { }
    public static string[] GetCommandLineArgs() { return default(string[]); }
    public static string GetEnvironmentVariable(string variable) { return default(string); }
    public static System.Collections.IDictionary GetEnvironmentVariables() { return default(System.Collections.IDictionary); }
    public static string GetFolderPath(System.Environment.SpecialFolder folder) { return default(string); }
    public static string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option) { return default(string); }
    public static string[] GetLogicalDrives() { return default(string[]); }
    public static void SetEnvironmentVariable(string variable, string value) { }
    public enum SpecialFolder {
      AdminTools = 48,
      ApplicationData = 26,
      CDBurning = 59,
      CommonAdminTools = 47,
      CommonApplicationData = 35,
      CommonDesktopDirectory = 25,
      CommonDocuments = 46,
      CommonMusic = 53,
      CommonOemLinks = 58,
      CommonPictures = 54,
      CommonProgramFiles = 43,
      CommonProgramFilesX86 = 44,
      CommonPrograms = 23,
      CommonStartMenu = 22,
      CommonStartup = 24,
      CommonTemplates = 45,
      CommonVideos = 55,
      Cookies = 33,
      Desktop = 0,
      DesktopDirectory = 16,
      Favorites = 6,
      Fonts = 20,
      History = 34,
      InternetCache = 32,
      LocalApplicationData = 28,
      LocalizedResources = 57,
      MyComputer = 17,
      MyDocuments = 5,
      MyMusic = 13,
      MyPictures = 39,
      MyVideos = 14,
      NetworkShortcuts = 19,
      Personal = 5,
      PrinterShortcuts = 27,
      ProgramFiles = 38,
      ProgramFilesX86 = 42,
      Programs = 2,
      Recent = 8,
      Resources = 56,
      SendTo = 9,
      StartMenu = 11,
      Startup = 7,
      System = 37,
      SystemX86 = 41,
      Templates = 21,
      UserProfile = 40,
      Windows = 36,
    }
    public enum SpecialFolderOption {
      Create = 32768,
      DoNotVerify = 16384,
      None = 0,
    }
  }
  public enum EnvironmentVariableTarget {
    Machine = 2,
    Process = 0,
    User = 1,
  }
  public partial class EventArgs {
    public static readonly System.EventArgs Empty;
    public EventArgs() { }
  }
  public delegate void EventHandler(object sender, System.EventArgs e);
  public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public partial class Exception : System.Runtime.Serialization.ISerializable {
    public Exception() { }
    protected Exception(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public Exception(string message) { }
    public Exception(string message, System.Exception innerException) { }
    public virtual System.Collections.IDictionary Data { get { return default(System.Collections.IDictionary); } }
    public virtual string HelpLink { get { return default(string); } set { } }
    public int HResult { get { return default(int); } protected set { } }
    public System.Exception InnerException { get { return default(System.Exception); } }
    public virtual string Message { get { return default(string); } }
    public virtual string Source { get { return default(string); } set { } }
    public virtual string StackTrace { get { return default(string); } }
    public System.Reflection.MethodBase TargetSite { get { return default(System.Reflection.MethodBase); } }
    protected event System.EventHandler<System.Runtime.Serialization.SafeSerializationEventArgs> SerializeObjectState { add { } remove { } }
    public virtual System.Exception GetBaseException() { return default(System.Exception); }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public new System.Type GetType() { return default(System.Type); }
    public override string ToString() { return default(string); }
  }
  [System.ObsoleteAttribute("This type previously indicated an unspecified fatal error in the runtime. The runtime no longer raises this exception so this type is obsolete.")]
  public sealed partial class ExecutionEngineException : System.SystemException {
    public ExecutionEngineException() { }
    public ExecutionEngineException(string message) { }
    public ExecutionEngineException(string message, System.Exception innerException) { }
  }
  public partial class FieldAccessException : System.MemberAccessException {
    public FieldAccessException() { }
    protected FieldAccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public FieldAccessException(string message) { }
    public FieldAccessException(string message, System.Exception inner) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(16), Inherited=false)]
  public partial class FlagsAttribute : System.Attribute {
    public FlagsAttribute() { }
  }
  public partial class FormatException : System.SystemException {
    public FormatException() { }
    protected FormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public FormatException(string message) { }
    public FormatException(string message, System.Exception innerException) { }
  }
  public abstract partial class FormattableString : System.IFormattable {
    protected FormattableString() { }
    public abstract int ArgumentCount { get; }
    public abstract string Format { get; }
    public abstract object GetArgument(int index);
    public abstract object[] GetArguments();
    public static string Invariant(System.FormattableString formattable) { return default(string); }
    string System.IFormattable.ToString(string ignored, System.IFormatProvider formatProvider) { return default(string); }
    public override string ToString() { return default(string); }
    public abstract string ToString(System.IFormatProvider formatProvider);
  }
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate TResult Func<out TResult>();
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate TResult Func<in T, out TResult>(T arg);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
  public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
  public static partial class GC {
    public static int MaxGeneration { get { return default(int); } }
    public static void AddMemoryPressure(long bytesAllocated) { }
    public static void CancelFullGCNotification() { }
    public static void Collect() { }
    public static void Collect(int generation) { }
    public static void Collect(int generation, System.GCCollectionMode mode) { }
    public static void Collect(int generation, System.GCCollectionMode mode, bool blocking) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static int CollectionCount(int generation) { return default(int); }
    public static int GetGeneration(object obj) { return default(int); }
    public static int GetGeneration(System.WeakReference wo) { return default(int); }
    public static long GetTotalMemory(bool forceFullCollection) { return default(long); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static void KeepAlive(object obj) { }
    public static void RegisterForFullGCNotification(int maxGenerationThreshold, int largeObjectHeapThreshold) { }
    public static void RemoveMemoryPressure(long bytesAllocated) { }
    public static void ReRegisterForFinalize(object obj) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static void SuppressFinalize(object obj) { }
    public static System.GCNotificationStatus WaitForFullGCApproach() { return default(System.GCNotificationStatus); }
    public static System.GCNotificationStatus WaitForFullGCApproach(int millisecondsTimeout) { return default(System.GCNotificationStatus); }
    public static System.GCNotificationStatus WaitForFullGCComplete() { return default(System.GCNotificationStatus); }
    public static System.GCNotificationStatus WaitForFullGCComplete(int millisecondsTimeout) { return default(System.GCNotificationStatus); }
    public static void WaitForPendingFinalizers() { }
  }
  public enum GCCollectionMode {
    Default = 0,
    Forced = 1,
    Optimized = 2,
  }
  public enum GCNotificationStatus {
    Canceled = 2,
    Failed = 1,
    NotApplicable = 4,
    Succeeded = 0,
    Timeout = 3,
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Guid : System.IComparable, System.IComparable<System.Guid>, System.IEquatable<System.Guid>, System.IFormattable {
    public static readonly System.Guid Empty;
    public Guid(byte[] b) { throw new System.NotImplementedException(); }
    public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) { throw new System.NotImplementedException(); }
    public Guid(int a, short b, short c, byte[] d) { throw new System.NotImplementedException(); }
    public Guid(string g) { throw new System.NotImplementedException(); }
    [System.CLSCompliantAttribute(false)]
    public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) { throw new System.NotImplementedException(); }
    public int CompareTo(System.Guid value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public bool Equals(System.Guid g) { return default(bool); }
    public override bool Equals(object o) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static System.Guid NewGuid() { return default(System.Guid); }
    public static bool operator ==(System.Guid a, System.Guid b) { return default(bool); }
    public static bool operator !=(System.Guid a, System.Guid b) { return default(bool); }
    public static System.Guid Parse(string input) { return default(System.Guid); }
    public static System.Guid ParseExact(string input, string format) { return default(System.Guid); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    string System.IFormattable.ToString(string format, System.IFormatProvider provider) { return default(string); }
    public byte[] ToByteArray() { return default(byte[]); }
    public override string ToString() { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string input, out System.Guid result) { result = default(System.Guid); return default(bool); }
    public static bool TryParseExact(string input, string format, out System.Guid result) { result = default(System.Guid); return default(bool); }
  }
  public partial interface IAsyncResult {
    object AsyncState { get; }
    System.Threading.WaitHandle AsyncWaitHandle { get; }
    bool CompletedSynchronously { get; }
    bool IsCompleted { get; }
  }
  public partial interface ICloneable {
    object Clone();
  }
  public partial interface IComparable {
    int CompareTo(object obj);
  }
  public partial interface IComparable<in T> {
    int CompareTo(T other);
  }
  [System.CLSCompliantAttribute(false)]
  public partial interface IConvertible {
    System.TypeCode GetTypeCode();
    bool ToBoolean(System.IFormatProvider provider);
    byte ToByte(System.IFormatProvider provider);
    char ToChar(System.IFormatProvider provider);
    System.DateTime ToDateTime(System.IFormatProvider provider);
    decimal ToDecimal(System.IFormatProvider provider);
    double ToDouble(System.IFormatProvider provider);
    short ToInt16(System.IFormatProvider provider);
    int ToInt32(System.IFormatProvider provider);
    long ToInt64(System.IFormatProvider provider);
    sbyte ToSByte(System.IFormatProvider provider);
    float ToSingle(System.IFormatProvider provider);
    string ToString(System.IFormatProvider provider);
    object ToType(System.Type conversionType, System.IFormatProvider provider);
    ushort ToUInt16(System.IFormatProvider provider);
    uint ToUInt32(System.IFormatProvider provider);
    ulong ToUInt64(System.IFormatProvider provider);
  }
  public partial interface ICustomFormatter {
    string Format(string format, object arg, System.IFormatProvider formatProvider);
  }
  public partial interface IDisposable {
    void Dispose();
  }
  public partial interface IEquatable<T> {
    bool Equals(T other);
  }
  public partial interface IFormatProvider {
    object GetFormat(System.Type formatType);
  }
  public partial interface IFormattable {
    string ToString(string format, System.IFormatProvider formatProvider);
  }
  public sealed partial class IndexOutOfRangeException : System.SystemException {
    public IndexOutOfRangeException() { }
    public IndexOutOfRangeException(string message) { }
    public IndexOutOfRangeException(string message, System.Exception innerException) { }
  }
  public sealed partial class InsufficientExecutionStackException : System.SystemException {
    public InsufficientExecutionStackException() { }
    public InsufficientExecutionStackException(string message) { }
    public InsufficientExecutionStackException(string message, System.Exception innerException) { }
  }
  public sealed partial class InsufficientMemoryException : System.OutOfMemoryException {
    public InsufficientMemoryException() { }
    public InsufficientMemoryException(string message) { }
    public InsufficientMemoryException(string message, System.Exception innerException) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Int16 : System.IComparable, System.IComparable<short>, System.IConvertible, System.IEquatable<short>, System.IFormattable {
    public const short MaxValue = (short)32767;
    public const short MinValue = (short)-32768;
    public int CompareTo(short value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public bool Equals(short obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static short Parse(string s) { return default(short); }
    public static short Parse(string s, System.Globalization.NumberStyles style) { return default(short); }
    public static short Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(short); }
    public static short Parse(string s, System.IFormatProvider provider) { return default(short); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out short result) { result = default(short); return default(bool); }
    public static bool TryParse(string s, out short result) { result = default(short); return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Int32 : System.IComparable, System.IComparable<int>, System.IConvertible, System.IEquatable<int>, System.IFormattable {
    public const int MaxValue = 2147483647;
    public const int MinValue = -2147483648;
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int CompareTo(int value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Equals(int obj) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override bool Equals(object obj) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override int GetHashCode() { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static int Parse(string s) { return default(int); }
    public static int Parse(string s, System.Globalization.NumberStyles style) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Parse(string s, System.IFormatProvider provider) { return default(int); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out int result) { result = default(int); return default(bool); }
    public static bool TryParse(string s, out int result) { result = default(int); return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Int64 : System.IComparable, System.IComparable<long>, System.IConvertible, System.IEquatable<long>, System.IFormattable {
    public const long MaxValue = (long)9223372036854775807;
    public const long MinValue = (long)-9223372036854775808;
    public int CompareTo(long value) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public bool Equals(long obj) { return default(bool); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    public static long Parse(string s) { return default(long); }
    public static long Parse(string s, System.Globalization.NumberStyles style) { return default(long); }
    public static long Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(long); }
    public static long Parse(string s, System.IFormatProvider provider) { return default(long); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out long result) { result = default(long); return default(bool); }
    public static bool TryParse(string s, out long result) { result = default(long); return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct IntPtr : System.Runtime.Serialization.ISerializable {
    public static readonly System.IntPtr Zero;
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public IntPtr(int value) { throw new System.NotImplementedException(); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public IntPtr(long value) { throw new System.NotImplementedException(); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public unsafe IntPtr(void* value) { throw new System.NotImplementedException(); }
    public static int Size { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(int); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static System.IntPtr Add(System.IntPtr pointer, int offset) { return default(System.IntPtr); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static System.IntPtr operator +(System.IntPtr pointer, int offset) { return default(System.IntPtr); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static bool operator ==(System.IntPtr value1, System.IntPtr value2) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static explicit operator System.IntPtr (int value) { return default(System.IntPtr); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static explicit operator System.IntPtr (long value) { return default(System.IntPtr); }
    public static explicit operator int (System.IntPtr value) { return default(int); }
    public static explicit operator long (System.IntPtr value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public unsafe static explicit operator void* (System.IntPtr value) { return default(void*); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public unsafe static explicit operator System.IntPtr (void* value) { return default(System.IntPtr); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static bool operator !=(System.IntPtr value1, System.IntPtr value2) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static System.IntPtr operator -(System.IntPtr pointer, int offset) { return default(System.IntPtr); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(2), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static System.IntPtr Subtract(System.IntPtr pointer, int offset) { return default(System.IntPtr); }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public int ToInt32() { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public long ToInt64() { return default(long); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public unsafe void* ToPointer() { return default(void*); }
    public override string ToString() { return default(string); }
    public string ToString(string format) { return default(string); }
  }
  public partial class InvalidCastException : System.SystemException {
    public InvalidCastException() { }
    protected InvalidCastException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public InvalidCastException(string message) { }
    public InvalidCastException(string message, System.Exception innerException) { }
    public InvalidCastException(string message, int errorCode) { }
  }
  public partial class InvalidOperationException : System.SystemException {
    public InvalidOperationException() { }
    protected InvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public InvalidOperationException(string message) { }
    public InvalidOperationException(string message, System.Exception innerException) { }
  }
  public sealed partial class InvalidProgramException : System.SystemException {
    public InvalidProgramException() { }
    public InvalidProgramException(string message) { }
    public InvalidProgramException(string message, System.Exception inner) { }
  }
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public partial class InvalidTimeZoneException : System.Exception {
    public InvalidTimeZoneException() { }
    protected InvalidTimeZoneException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public InvalidTimeZoneException(string message) { }
    public InvalidTimeZoneException(string message, System.Exception innerException) { }
  }
  public partial interface IObservable<out T> {
    System.IDisposable Subscribe(System.IObserver<T> observer);
  }
  public partial interface IObserver<in T> {
    void OnCompleted();
    void OnError(System.Exception error);
    void OnNext(T value);
  }
  public partial interface IProgress<in T> {
    void Report(T value);
  }
  public partial interface IServiceProvider {
    object GetService(System.Type serviceType);
  }
  [System.Diagnostics.DebuggerDisplayAttribute("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
  public partial class Lazy<T> {
    public Lazy() { }
    public Lazy(bool isThreadSafe) { }
    public Lazy(System.Func<T> valueFactory) { }
    public Lazy(System.Func<T> valueFactory, bool isThreadSafe) { }
    public Lazy(System.Func<T> valueFactory, System.Threading.LazyThreadSafetyMode mode) { }
    public Lazy(System.Threading.LazyThreadSafetyMode mode) { }
    public bool IsValueCreated { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    [System.Diagnostics.DebuggerBrowsableAttribute((System.Diagnostics.DebuggerBrowsableState)(0))]
    public T Value { get { return default(T); } }
    public override string ToString() { return default(string); }
  }
  public partial class Lazy<T, TMetadata> : System.Lazy<T> {
    public Lazy(TMetadata metadata) { }
    public Lazy(TMetadata metadata, bool isThreadSafe) { }
    public Lazy(TMetadata metadata, System.Threading.LazyThreadSafetyMode mode) { }
    public Lazy(System.Func<T> valueFactory, TMetadata metadata) { }
    public Lazy(System.Func<T> valueFactory, TMetadata metadata, bool isThreadSafe) { }
    public Lazy(System.Func<T> valueFactory, TMetadata metadata, System.Threading.LazyThreadSafetyMode mode) { }
    public TMetadata Metadata { get { return default(TMetadata); } }
  }
  public enum LoaderOptimization {
    [System.ObsoleteAttribute("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    DisallowBindings = 4,
    [System.ObsoleteAttribute("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    DomainMask = 3,
    MultiDomain = 2,
    MultiDomainHost = 3,
    NotSpecified = 0,
    SingleDomain = 1,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
  public sealed partial class LoaderOptimizationAttribute : System.Attribute {
    public LoaderOptimizationAttribute(byte value) { }
    public LoaderOptimizationAttribute(System.LoaderOptimization value) { }
    public System.LoaderOptimization Value { get { return default(System.LoaderOptimization); } }
  }
  public static partial class Math {
    public const double E = 2.7182818284590451;
    public const double PI = 3.1415926535897931;
    public static decimal Abs(decimal value) { return default(decimal); }
    public static double Abs(double value) { return default(double); }
    public static short Abs(short value) { return default(short); }
    public static int Abs(int value) { return default(int); }
    public static long Abs(long value) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte Abs(sbyte value) { return default(sbyte); }
    public static float Abs(float value) { return default(float); }
    public static double Acos(double d) { return default(double); }
    public static double Asin(double d) { return default(double); }
    public static double Atan(double d) { return default(double); }
    public static double Atan2(double y, double x) { return default(double); }
    public static long BigMul(int a, int b) { return default(long); }
    public static decimal Ceiling(decimal d) { return default(decimal); }
    public static double Ceiling(double a) { return default(double); }
    public static double Cos(double d) { return default(double); }
    public static double Cosh(double value) { return default(double); }
    public static int DivRem(int a, int b, out int result) { result = default(int); return default(int); }
    public static long DivRem(long a, long b, out long result) { result = default(long); return default(long); }
    public static double Exp(double d) { return default(double); }
    public static decimal Floor(decimal d) { return default(decimal); }
    public static double Floor(double d) { return default(double); }
    public static double IEEERemainder(double x, double y) { return default(double); }
    public static double Log(double d) { return default(double); }
    public static double Log(double a, double newBase) { return default(double); }
    public static double Log10(double d) { return default(double); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static byte Max(byte val1, byte val2) { return default(byte); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static decimal Max(decimal val1, decimal val2) { return default(decimal); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static double Max(double val1, double val2) { return default(double); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static short Max(short val1, short val2) { return default(short); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static int Max(int val1, int val2) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static long Max(long val1, long val2) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static sbyte Max(sbyte val1, sbyte val2) { return default(sbyte); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static float Max(float val1, float val2) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static ushort Max(ushort val1, ushort val2) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static uint Max(uint val1, uint val2) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static ulong Max(ulong val1, ulong val2) { return default(ulong); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static byte Min(byte val1, byte val2) { return default(byte); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static decimal Min(decimal val1, decimal val2) { return default(decimal); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static double Min(double val1, double val2) { return default(double); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static short Min(short val1, short val2) { return default(short); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static int Min(int val1, int val2) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static long Min(long val1, long val2) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static sbyte Min(sbyte val1, sbyte val2) { return default(sbyte); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static float Min(float val1, float val2) { return default(float); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static ushort Min(ushort val1, ushort val2) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static uint Min(uint val1, uint val2) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static ulong Min(ulong val1, ulong val2) { return default(ulong); }
    public static double Pow(double x, double y) { return default(double); }
    public static decimal Round(decimal d) { return default(decimal); }
    public static decimal Round(decimal d, int decimals) { return default(decimal); }
    public static decimal Round(decimal d, int decimals, System.MidpointRounding mode) { return default(decimal); }
    public static decimal Round(decimal d, System.MidpointRounding mode) { return default(decimal); }
    public static double Round(double a) { return default(double); }
    public static double Round(double value, int digits) { return default(double); }
    public static double Round(double value, int digits, System.MidpointRounding mode) { return default(double); }
    public static double Round(double value, System.MidpointRounding mode) { return default(double); }
    public static int Sign(decimal value) { return default(int); }
    public static int Sign(double value) { return default(int); }
    public static int Sign(short value) { return default(int); }
    public static int Sign(int value) { return default(int); }
    public static int Sign(long value) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    public static int Sign(sbyte value) { return default(int); }
    public static int Sign(float value) { return default(int); }
    public static double Sin(double a) { return default(double); }
    public static double Sinh(double value) { return default(double); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static double Sqrt(double d) { return default(double); }
    public static double Tan(double a) { return default(double); }
    public static double Tanh(double value) { return default(double); }
    public static decimal Truncate(decimal d) { return default(decimal); }
    public static double Truncate(double d) { return default(double); }
  }
  public partial class MemberAccessException : System.SystemException {
    public MemberAccessException() { }
    protected MemberAccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public MemberAccessException(string message) { }
    public MemberAccessException(string message, System.Exception inner) { }
  }
  public partial class MethodAccessException : System.MemberAccessException {
    public MethodAccessException() { }
    protected MethodAccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public MethodAccessException(string message) { }
    public MethodAccessException(string message, System.Exception inner) { }
  }
  public enum MidpointRounding {
    AwayFromZero = 1,
    ToEven = 0,
  }
  public partial class MissingFieldException : System.MissingMemberException, System.Runtime.Serialization.ISerializable {
    public MissingFieldException() { }
    protected MissingFieldException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public MissingFieldException(string message) { }
    public MissingFieldException(string message, System.Exception inner) { }
    public MissingFieldException(string className, string fieldName) { }
    public override string Message { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
  }
  public partial class MissingMemberException : System.MemberAccessException, System.Runtime.Serialization.ISerializable {
    protected string ClassName;
    protected string MemberName;
    protected byte[] Signature;
    public MissingMemberException() { }
    protected MissingMemberException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public MissingMemberException(string message) { }
    public MissingMemberException(string message, System.Exception inner) { }
    public MissingMemberException(string className, string memberName) { }
    public override string Message { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class MissingMethodException : System.MissingMemberException, System.Runtime.Serialization.ISerializable {
    public MissingMethodException() { }
    protected MissingMethodException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public MissingMethodException(string message) { }
    public MissingMethodException(string message, System.Exception inner) { }
    public MissingMethodException(string className, string methodName) { }
    public override string Message { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct ModuleHandle {
    public static readonly System.ModuleHandle EmptyHandle;
    public int MDStreamVersion { get { return default(int); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public bool Equals(System.ModuleHandle handle) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.RuntimeFieldHandle GetRuntimeFieldHandleFromMetadataToken(int fieldToken) { return default(System.RuntimeFieldHandle); }
    public System.RuntimeMethodHandle GetRuntimeMethodHandleFromMetadataToken(int methodToken) { return default(System.RuntimeMethodHandle); }
    public System.RuntimeTypeHandle GetRuntimeTypeHandleFromMetadataToken(int typeToken) { return default(System.RuntimeTypeHandle); }
    public static bool operator ==(System.ModuleHandle left, System.ModuleHandle right) { return default(bool); }
    public static bool operator !=(System.ModuleHandle left, System.ModuleHandle right) { return default(bool); }
    public System.RuntimeFieldHandle ResolveFieldHandle(int fieldToken) { return default(System.RuntimeFieldHandle); }
    public System.RuntimeFieldHandle ResolveFieldHandle(int fieldToken, System.RuntimeTypeHandle[] typeInstantiationContext, System.RuntimeTypeHandle[] methodInstantiationContext) { return default(System.RuntimeFieldHandle); }
    public System.RuntimeMethodHandle ResolveMethodHandle(int methodToken) { return default(System.RuntimeMethodHandle); }
    public System.RuntimeMethodHandle ResolveMethodHandle(int methodToken, System.RuntimeTypeHandle[] typeInstantiationContext, System.RuntimeTypeHandle[] methodInstantiationContext) { return default(System.RuntimeMethodHandle); }
    public System.RuntimeTypeHandle ResolveTypeHandle(int typeToken) { return default(System.RuntimeTypeHandle); }
    public System.RuntimeTypeHandle ResolveTypeHandle(int typeToken, System.RuntimeTypeHandle[] typeInstantiationContext, System.RuntimeTypeHandle[] methodInstantiationContext) { return default(System.RuntimeTypeHandle); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
  public sealed partial class MTAThreadAttribute : System.Attribute {
    public MTAThreadAttribute() { }
  }
  public abstract partial class MulticastDelegate : System.Delegate {
    protected MulticastDelegate(object target, string method) : base (default(object), default(string)) { }
    protected MulticastDelegate(System.Type target, string method) : base (default(object), default(string)) { }
    protected sealed override System.Delegate CombineImpl(System.Delegate follow) { return default(System.Delegate); }
    protected sealed override object DynamicInvokeImpl(object[] args) { return default(object); }
    public sealed override bool Equals(object obj) { return default(bool); }
    public sealed override int GetHashCode() { return default(int); }
    public sealed override System.Delegate[] GetInvocationList() { return default(System.Delegate[]); }
    protected override System.Reflection.MethodInfo GetMethodImpl() { return default(System.Reflection.MethodInfo); }
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public static bool operator ==(System.MulticastDelegate d1, System.MulticastDelegate d2) { return default(bool); }
    public static bool operator !=(System.MulticastDelegate d1, System.MulticastDelegate d2) { return default(bool); }
    protected sealed override System.Delegate RemoveImpl(System.Delegate value) { return default(System.Delegate); }
  }
  public sealed partial class MulticastNotSupportedException : System.SystemException {
    public MulticastNotSupportedException() { }
    public MulticastNotSupportedException(string message) { }
    public MulticastNotSupportedException(string message, System.Exception inner) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public sealed partial class NonSerializedAttribute : System.Attribute {
    public NonSerializedAttribute() { }
  }
  public partial class NotFiniteNumberException : System.ArithmeticException {
    public NotFiniteNumberException() { }
    public NotFiniteNumberException(double offendingNumber) { }
    protected NotFiniteNumberException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public NotFiniteNumberException(string message) { }
    public NotFiniteNumberException(string message, double offendingNumber) { }
    public NotFiniteNumberException(string message, double offendingNumber, System.Exception innerException) { }
    public NotFiniteNumberException(string message, System.Exception innerException) { }
    public double OffendingNumber { get { return default(double); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class NotImplementedException : System.SystemException {
    public NotImplementedException() { }
    protected NotImplementedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public NotImplementedException(string message) { }
    public NotImplementedException(string message, System.Exception inner) { }
  }
  public partial class NotSupportedException : System.SystemException {
    public NotSupportedException() { }
    protected NotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public NotSupportedException(string message) { }
    public NotSupportedException(string message, System.Exception innerException) { }
  }
  public static partial class Nullable {
    public static int Compare<T>(System.Nullable<T> n1, System.Nullable<T> n2) where T : struct { return default(int); }
    public static bool Equals<T>(System.Nullable<T> n1, System.Nullable<T> n2) where T : struct { return default(bool); }
    public static System.Type GetUnderlyingType(System.Type nullableType) { return default(System.Type); }
  }
  [System.Diagnostics.DebuggerStepThroughAttribute]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Nullable<T> where T : struct {
    public Nullable(T value) { throw new System.NotImplementedException(); }
    public bool HasValue { get { return default(bool); } }
    public T Value { get { return default(T); } }
    public override bool Equals(object other) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public T GetValueOrDefault() { return default(T); }
    public T GetValueOrDefault(T defaultValue) { return default(T); }
    public static explicit operator T (System.Nullable<T> value) { return default(T); }
    public static implicit operator System.Nullable<T> (T value) { return default(System.Nullable<T>); }
    public override string ToString() { return default(string); }
  }
  public partial class NullReferenceException : System.SystemException {
    public NullReferenceException() { }
    protected NullReferenceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public NullReferenceException(string message) { }
    public NullReferenceException(string message, System.Exception innerException) { }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(2))]
  public partial class Object {
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public Object() { }
    public virtual bool Equals(object obj) { return default(bool); }
    public static bool Equals(object objA, object objB) { return default(bool); }
    ~Object() { }
    public virtual int GetHashCode() { return default(int); }
    public System.Type GetType() { return default(System.Type); }
    protected object MemberwiseClone() { return default(object); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static bool ReferenceEquals(object objA, object objB) { return default(bool); }
    public virtual string ToString() { return default(string); }
  }
  public partial class ObjectDisposedException : System.InvalidOperationException {
    protected ObjectDisposedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ObjectDisposedException(string objectName) { }
    public ObjectDisposedException(string message, System.Exception innerException) { }
    public ObjectDisposedException(string objectName, string message) { }
    public override string Message { get { return default(string); } }
    public string ObjectName { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(6140), Inherited=false)]
  public sealed partial class ObsoleteAttribute : System.Attribute {
    public ObsoleteAttribute() { }
    public ObsoleteAttribute(string message) { }
    public ObsoleteAttribute(string message, bool error) { }
    public bool IsError { get { return default(bool); } }
    public string Message { get { return default(string); } }
  }
  public sealed partial class OperatingSystem : System.ICloneable, System.Runtime.Serialization.ISerializable {
    public OperatingSystem(System.PlatformID platform, System.Version version) { }
    public System.PlatformID Platform { get { return default(System.PlatformID); } }
    public string ServicePack { get { return default(string); } }
    public System.Version Version { get { return default(System.Version); } }
    public string VersionString { get { return default(string); } }
    public object Clone() { return default(object); }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  public partial class OperationCanceledException : System.SystemException {
    public OperationCanceledException() { }
    protected OperationCanceledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public OperationCanceledException(string message) { }
    public OperationCanceledException(string message, System.Exception innerException) { }
    public OperationCanceledException(string message, System.Exception innerException, System.Threading.CancellationToken token) { }
    public OperationCanceledException(string message, System.Threading.CancellationToken token) { }
    public OperationCanceledException(System.Threading.CancellationToken token) { }
    public System.Threading.CancellationToken CancellationToken { get { return default(System.Threading.CancellationToken); } }
  }
  public partial class OutOfMemoryException : System.SystemException {
    public OutOfMemoryException() { }
    protected OutOfMemoryException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public OutOfMemoryException(string message) { }
    public OutOfMemoryException(string message, System.Exception innerException) { }
  }
  public partial class OverflowException : System.ArithmeticException {
    public OverflowException() { }
    protected OverflowException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public OverflowException(string message) { }
    public OverflowException(string message, System.Exception innerException) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited=true, AllowMultiple=false)]
  public sealed partial class ParamArrayAttribute : System.Attribute {
    public ParamArrayAttribute() { }
  }
  public enum PlatformID {
    MacOSX = 6,
    Unix = 4,
    Win32NT = 2,
    Win32S = 0,
    Win32Windows = 1,
    WinCE = 3,
    Xbox = 5,
  }
  public partial class PlatformNotSupportedException : System.NotSupportedException {
    public PlatformNotSupportedException() { }
    protected PlatformNotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public PlatformNotSupportedException(string message) { }
    public PlatformNotSupportedException(string message, System.Exception inner) { }
  }
  public delegate bool Predicate<in T>(T obj);
  public partial class Progress<T> : System.IProgress<T> {
    public Progress() { }
    public Progress(System.Action<T> handler) { }
    public event System.EventHandler<T> ProgressChanged { add { } remove { } }
    protected virtual void OnReport(T value) { }
    void System.IProgress<T>.Report(T value) { }
  }
  public partial class Random {
    public Random() { }
    public Random(int Seed) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public virtual int Next() { return default(int); }
    public virtual int Next(int maxValue) { return default(int); }
    public virtual int Next(int minValue, int maxValue) { return default(int); }
    public virtual void NextBytes(byte[] buffer) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public virtual double NextDouble() { return default(double); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected virtual double Sample() { return default(double); }
  }
  public partial class RankException : System.SystemException {
    public RankException() { }
    protected RankException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public RankException(string message) { }
    public RankException(string message, System.Exception innerException) { }
  }
  public partial class ResolveEventArgs : System.EventArgs {
    public ResolveEventArgs(string name) { }
    public ResolveEventArgs(string name, System.Reflection.Assembly requestingAssembly) { }
    public string Name { get { return default(string); } }
    public System.Reflection.Assembly RequestingAssembly { get { return default(System.Reflection.Assembly); } }
  }
  public delegate System.Reflection.Assembly ResolveEventHandler(object sender, System.ResolveEventArgs args);
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct RuntimeArgumentHandle {
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct RuntimeFieldHandle : System.Runtime.Serialization.ISerializable {
    public System.IntPtr Value { get { return default(System.IntPtr); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public override bool Equals(object obj) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public bool Equals(System.RuntimeFieldHandle handle) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public static bool operator ==(System.RuntimeFieldHandle left, System.RuntimeFieldHandle right) { return default(bool); }
    public static bool operator !=(System.RuntimeFieldHandle left, System.RuntimeFieldHandle right) { return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct RuntimeMethodHandle : System.Runtime.Serialization.ISerializable {
    public System.IntPtr Value { get { return default(System.IntPtr); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public override bool Equals(object obj) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public bool Equals(System.RuntimeMethodHandle handle) { return default(bool); }
    public System.IntPtr GetFunctionPointer() { return default(System.IntPtr); }
    public override int GetHashCode() { return default(int); }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public static bool operator ==(System.RuntimeMethodHandle left, System.RuntimeMethodHandle right) { return default(bool); }
    public static bool operator !=(System.RuntimeMethodHandle left, System.RuntimeMethodHandle right) { return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct RuntimeTypeHandle : System.Runtime.Serialization.ISerializable {
    public System.IntPtr Value { get { return default(System.IntPtr); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public override bool Equals(object obj) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public bool Equals(System.RuntimeTypeHandle handle) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public System.ModuleHandle GetModuleHandle() { return default(System.ModuleHandle); }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public static bool operator ==(object left, System.RuntimeTypeHandle right) { return default(bool); }
    public static bool operator ==(System.RuntimeTypeHandle left, object right) { return default(bool); }
    public static bool operator !=(object left, System.RuntimeTypeHandle right) { return default(bool); }
    public static bool operator !=(System.RuntimeTypeHandle left, object right) { return default(bool); }
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct SByte : System.IComparable, System.IComparable<sbyte>, System.IConvertible, System.IEquatable<sbyte>, System.IFormattable {
    public const sbyte MaxValue = (sbyte)127;
    public const sbyte MinValue = (sbyte)-128;
    public int CompareTo(object obj) { return default(int); }
    public int CompareTo(sbyte value) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(sbyte obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte Parse(string s) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte Parse(string s, System.Globalization.NumberStyles style) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(sbyte); }
    [System.CLSCompliantAttribute(false)]
    public static sbyte Parse(string s, System.IFormatProvider provider) { return default(sbyte); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out sbyte result) { result = default(sbyte); return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, out sbyte result) { result = default(sbyte); return default(bool); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(4124), Inherited=false)]
  public sealed partial class SerializableAttribute : System.Attribute {
    public SerializableAttribute() { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct Single : System.IComparable, System.IComparable<float>, System.IConvertible, System.IEquatable<float>, System.IFormattable {
    public const float Epsilon = 1.401298E-45f;
    public const float MaxValue = 3.40282347E+38f;
    public const float MinValue = -3.40282347E+38f;
    public const float NaN = 0.0f / 0.0f;
    public const float NegativeInfinity = -1.0f / 0.0f;
    public const float PositiveInfinity = 1.0f / 0.0f;
    public int CompareTo(object value) { return default(int); }
    public int CompareTo(float value) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(float obj) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsInfinity(float f) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsNaN(float f) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsNegativeInfinity(float f) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsPositiveInfinity(float f) { return default(bool); }
    public static bool operator ==(float left, float right) { return default(bool); }
    public static bool operator >(float left, float right) { return default(bool); }
    public static bool operator >=(float left, float right) { return default(bool); }
    public static bool operator !=(float left, float right) { return default(bool); }
    public static bool operator <(float left, float right) { return default(bool); }
    public static bool operator <=(float left, float right) { return default(bool); }
    public static float Parse(string s) { return default(float); }
    public static float Parse(string s, System.Globalization.NumberStyles style) { return default(float); }
    public static float Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(float); }
    public static float Parse(string s, System.IFormatProvider provider) { return default(float); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out float result) { result = default(float); return default(bool); }
    public static bool TryParse(string s, out float result) { result = default(float); return default(bool); }
  }
  public sealed partial class StackOverflowException : System.SystemException {
    public StackOverflowException() { }
    public StackOverflowException(string message) { }
    public StackOverflowException(string message, System.Exception innerException) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
  public sealed partial class STAThreadAttribute : System.Attribute {
    public STAThreadAttribute() { }
  }
  public sealed partial class String : System.Collections.Generic.IEnumerable<char>, System.Collections.IEnumerable, System.ICloneable, System.IComparable, System.IComparable<string>, System.IConvertible, System.IEquatable<string> {
    public static readonly string Empty;
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe String(char* value) { }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe String(char* value, int startIndex, int length) { }
    [System.Security.SecuritySafeCriticalAttribute]
    public String(char c, int count) { }
    [System.Security.SecuritySafeCriticalAttribute]
    public String(char[] value) { }
    [System.Security.SecuritySafeCriticalAttribute]
    public String(char[] value, int startIndex, int length) { }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe String(sbyte* value) { }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe String(sbyte* value, int startIndex, int length) { }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe String(sbyte* value, int startIndex, int length, System.Text.Encoding enc) { }
    [System.Runtime.CompilerServices.IndexerName("Chars")]
    public char this[int index] { get { return default(char); } }
    public int Length { get { return default(int); } }
    public object Clone() { return default(object); }
    public static int Compare(string strA, int indexA, string strB, int indexB, int length) { return default(int); }
    public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase) { return default(int); }
    public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase, System.Globalization.CultureInfo culture) { return default(int); }
    public static int Compare(string strA, int indexA, string strB, int indexB, int length, System.Globalization.CultureInfo culture, System.Globalization.CompareOptions options) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int Compare(string strA, int indexA, string strB, int indexB, int length, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Compare(string strA, string strB) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Compare(string strA, string strB, bool ignoreCase) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Compare(string strA, string strB, bool ignoreCase, System.Globalization.CultureInfo culture) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int Compare(string strA, string strB, System.Globalization.CultureInfo culture, System.Globalization.CompareOptions options) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static int Compare(string strA, string strB, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public static int CompareOrdinal(string strA, int indexA, string strB, int indexB, int length) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static int CompareOrdinal(string strA, string strB) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int CompareTo(object value) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int CompareTo(string strB) { return default(int); }
    public static string Concat(System.Collections.Generic.IEnumerable<string> values) { return default(string); }
    public static string Concat(object arg0) { return default(string); }
    public static string Concat(object arg0, object arg1) { return default(string); }
    public static string Concat(object arg0, object arg1, object arg2) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static string Concat(object arg0, object arg1, object arg2, object arg3, __arglist) { return default(string); }
    public static string Concat(params object[] args) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Concat(string str0, string str1) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Concat(string str0, string str1, string str2) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Concat(string str0, string str1, string str2, string str3) { return default(string); }
    public static string Concat(params string[] values) { return default(string); }
    public static string Concat<T>(System.Collections.Generic.IEnumerable<T> values) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Contains(string value) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Copy(string str) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { }
    public bool EndsWith(string value) { return default(bool); }
    public bool EndsWith(string value, bool ignoreCase, System.Globalization.CultureInfo culture) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool EndsWith(string value, System.StringComparison comparisonType) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public override bool Equals(object obj) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Equals(string value) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool Equals(string a, string b) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool Equals(string a, string b, System.StringComparison comparisonType) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool Equals(string value, System.StringComparison comparisonType) { return default(bool); }
    public static string Format(System.IFormatProvider provider, string format, object arg0) { return default(string); }
    public static string Format(System.IFormatProvider provider, string format, object arg0, object arg1) { return default(string); }
    public static string Format(System.IFormatProvider provider, string format, object arg0, object arg1, object arg2) { return default(string); }
    public static string Format(System.IFormatProvider provider, string format, params object[] args) { return default(string); }
    public static string Format(string format, object arg0) { return default(string); }
    public static string Format(string format, object arg0, object arg1) { return default(string); }
    public static string Format(string format, object arg0, object arg1, object arg2) { return default(string); }
    public static string Format(string format, params object[] args) { return default(string); }
    public System.CharEnumerator GetEnumerator() { return default(System.CharEnumerator); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    [System.Security.SecuritySafeCriticalAttribute]
    public override int GetHashCode() { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(char value) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(char value, int startIndex) { return default(int); }
    public int IndexOf(char value, int startIndex, int count) { return default(int); }
    public int IndexOf(string value) { return default(int); }
    public int IndexOf(string value, int startIndex) { return default(int); }
    public int IndexOf(string value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public int IndexOf(string value, int startIndex, int count, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(string value, int startIndex, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(string value, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOfAny(char[] anyOf) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOfAny(char[] anyOf, int startIndex) { return default(int); }
    public int IndexOfAny(char[] anyOf, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string Insert(int startIndex, string value) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Intern(string str) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string IsInterned(string str) { return default(string); }
    public bool IsNormalized() { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool IsNormalized(System.Text.NormalizationForm normalizationForm) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool IsNullOrEmpty(string value) { return default(bool); }
    public static bool IsNullOrWhiteSpace(string value) { return default(bool); }
    public static string Join(string separator, System.Collections.Generic.IEnumerable<string> values) { return default(string); }
    public static string Join(string separator, params object[] values) { return default(string); }
    public static string Join(string separator, params string[] value) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static string Join(string separator, string[] value, int startIndex, int count) { return default(string); }
    public static string Join<T>(string separator, System.Collections.Generic.IEnumerable<T> values) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int LastIndexOf(char value) { return default(int); }
    public int LastIndexOf(char value, int startIndex) { return default(int); }
    public int LastIndexOf(char value, int startIndex, int count) { return default(int); }
    public int LastIndexOf(string value) { return default(int); }
    public int LastIndexOf(string value, int startIndex) { return default(int); }
    public int LastIndexOf(string value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public int LastIndexOf(string value, int startIndex, int count, System.StringComparison comparisonType) { return default(int); }
    public int LastIndexOf(string value, int startIndex, System.StringComparison comparisonType) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int LastIndexOf(string value, System.StringComparison comparisonType) { return default(int); }
    public int LastIndexOfAny(char[] anyOf) { return default(int); }
    public int LastIndexOfAny(char[] anyOf, int startIndex) { return default(int); }
    public int LastIndexOfAny(char[] anyOf, int startIndex, int count) { return default(int); }
    public string Normalize() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string Normalize(System.Text.NormalizationForm normalizationForm) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator ==(string a, string b) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator !=(string a, string b) { return default(bool); }
    public string PadLeft(int totalWidth) { return default(string); }
    public string PadLeft(int totalWidth, char paddingChar) { return default(string); }
    public string PadRight(int totalWidth) { return default(string); }
    public string PadRight(int totalWidth, char paddingChar) { return default(string); }
    public string Remove(int startIndex) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string Remove(int startIndex, int count) { return default(string); }
    public string Replace(char oldChar, char newChar) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string Replace(string oldValue, string newValue) { return default(string); }
    public string[] Split(params char[] separator) { return default(string[]); }
    public string[] Split(char[] separator, int count) { return default(string[]); }
    public string[] Split(char[] separator, int count, System.StringSplitOptions options) { return default(string[]); }
    public string[] Split(char[] separator, System.StringSplitOptions options) { return default(string[]); }
    public string[] Split(string[] separator, int count, System.StringSplitOptions options) { return default(string[]); }
    public string[] Split(string[] separator, System.StringSplitOptions options) { return default(string[]); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool StartsWith(string value) { return default(bool); }
    public bool StartsWith(string value, bool ignoreCase, System.Globalization.CultureInfo culture) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public bool StartsWith(string value, System.StringComparison comparisonType) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string Substring(int startIndex) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string Substring(int startIndex, int length) { return default(string); }
    System.Collections.Generic.IEnumerator<char> System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<char>); }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    string System.IConvertible.ToString(System.IFormatProvider provider) { return default(string); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public char[] ToCharArray() { return default(char[]); }
    [System.Security.SecuritySafeCriticalAttribute]
    public char[] ToCharArray(int startIndex, int length) { return default(char[]); }
    public string ToLower() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToLower(System.Globalization.CultureInfo culture) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToLowerInvariant() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override string ToString() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    public string ToUpper() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToUpper(System.Globalization.CultureInfo culture) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string ToUpperInvariant() { return default(string); }
    public string Trim() { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string Trim(params char[] trimChars) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string TrimEnd(params char[] trimChars) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public string TrimStart(params char[] trimChars) { return default(string); }
  }
  public abstract partial class StringComparer : System.Collections.Generic.IComparer<string>, System.Collections.Generic.IEqualityComparer<string>, System.Collections.IComparer, System.Collections.IEqualityComparer {
    protected StringComparer() { }
    public static System.StringComparer CurrentCulture { get { return default(System.StringComparer); } }
    public static System.StringComparer CurrentCultureIgnoreCase { get { return default(System.StringComparer); } }
    public static System.StringComparer InvariantCulture { get { return default(System.StringComparer); } }
    public static System.StringComparer InvariantCultureIgnoreCase { get { return default(System.StringComparer); } }
    public static System.StringComparer Ordinal { get { return default(System.StringComparer); } }
    public static System.StringComparer OrdinalIgnoreCase { get { return default(System.StringComparer); } }
    public int Compare(object x, object y) { return default(int); }
    public abstract int Compare(string x, string y);
    public static System.StringComparer Create(System.Globalization.CultureInfo culture, bool ignoreCase) { return default(System.StringComparer); }
    public new bool Equals(object x, object y) { return default(bool); }
    public abstract bool Equals(string x, string y);
    public int GetHashCode(object obj) { return default(int); }
    public abstract int GetHashCode(string obj);
    int System.Collections.IComparer.Compare(object x, object y) { return default(int); }
    bool System.Collections.IEqualityComparer.Equals(object x, object y) { return default(bool); }
    int System.Collections.IEqualityComparer.GetHashCode(object obj) { return default(int); }
  }
  public enum StringComparison {
    CurrentCulture = 0,
    CurrentCultureIgnoreCase = 1,
    InvariantCulture = 2,
    InvariantCultureIgnoreCase = 3,
    Ordinal = 4,
    OrdinalIgnoreCase = 5,
  }
  [System.FlagsAttribute]
  public enum StringSplitOptions {
    None = 0,
    RemoveEmptyEntries = 1,
  }
  public partial class SystemException : System.Exception {
    public SystemException() { }
    protected SystemException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public SystemException(string message) { }
    public SystemException(string message, System.Exception innerException) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public partial class ThreadStaticAttribute : System.Attribute {
    public ThreadStaticAttribute() { }
  }
  public partial class TimeoutException : System.SystemException {
    public TimeoutException() { }
    protected TimeoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TimeoutException(string message) { }
    public TimeoutException(string message, System.Exception innerException) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct TimeSpan : System.IComparable, System.IComparable<System.TimeSpan>, System.IEquatable<System.TimeSpan>, System.IFormattable {
    public static readonly System.TimeSpan MaxValue;
    public static readonly System.TimeSpan MinValue;
    public const long TicksPerDay = (long)864000000000;
    public const long TicksPerHour = (long)36000000000;
    public const long TicksPerMillisecond = (long)10000;
    public const long TicksPerMinute = (long)600000000;
    public const long TicksPerSecond = (long)10000000;
    public static readonly System.TimeSpan Zero;
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public TimeSpan(int hours, int minutes, int seconds) { throw new System.NotImplementedException(); }
    public TimeSpan(int days, int hours, int minutes, int seconds) { throw new System.NotImplementedException(); }
    public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds) { throw new System.NotImplementedException(); }
    public TimeSpan(long ticks) { throw new System.NotImplementedException(); }
    public int Days { get { return default(int); } }
    public int Hours { get { return default(int); } }
    public int Milliseconds { get { return default(int); } }
    public int Minutes { get { return default(int); } }
    public int Seconds { get { return default(int); } }
    public long Ticks { get { return default(long); } }
    public double TotalDays { get { return default(double); } }
    public double TotalHours { get { return default(double); } }
    public double TotalMilliseconds { get { return default(double); } }
    public double TotalMinutes { get { return default(double); } }
    public double TotalSeconds { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(double); } }
    public System.TimeSpan Add(System.TimeSpan ts) { return default(System.TimeSpan); }
    public static int Compare(System.TimeSpan t1, System.TimeSpan t2) { return default(int); }
    public int CompareTo(object value) { return default(int); }
    public int CompareTo(System.TimeSpan value) { return default(int); }
    public System.TimeSpan Duration() { return default(System.TimeSpan); }
    public override bool Equals(object value) { return default(bool); }
    public bool Equals(System.TimeSpan obj) { return default(bool); }
    public static bool Equals(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    public static System.TimeSpan FromDays(double value) { return default(System.TimeSpan); }
    public static System.TimeSpan FromHours(double value) { return default(System.TimeSpan); }
    public static System.TimeSpan FromMilliseconds(double value) { return default(System.TimeSpan); }
    public static System.TimeSpan FromMinutes(double value) { return default(System.TimeSpan); }
    public static System.TimeSpan FromSeconds(double value) { return default(System.TimeSpan); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static System.TimeSpan FromTicks(long value) { return default(System.TimeSpan); }
    public override int GetHashCode() { return default(int); }
    public System.TimeSpan Negate() { return default(System.TimeSpan); }
    public static System.TimeSpan operator +(System.TimeSpan t1, System.TimeSpan t2) { return default(System.TimeSpan); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator ==(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator >(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator >=(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator !=(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator <(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator <=(System.TimeSpan t1, System.TimeSpan t2) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static System.TimeSpan operator -(System.TimeSpan t1, System.TimeSpan t2) { return default(System.TimeSpan); }
    public static System.TimeSpan operator -(System.TimeSpan t) { return default(System.TimeSpan); }
    public static System.TimeSpan operator +(System.TimeSpan t) { return default(System.TimeSpan); }
    public static System.TimeSpan Parse(string s) { return default(System.TimeSpan); }
    public static System.TimeSpan Parse(string input, System.IFormatProvider formatProvider) { return default(System.TimeSpan); }
    public static System.TimeSpan ParseExact(string input, string format, System.IFormatProvider formatProvider) { return default(System.TimeSpan); }
    public static System.TimeSpan ParseExact(string input, string format, System.IFormatProvider formatProvider, System.Globalization.TimeSpanStyles styles) { return default(System.TimeSpan); }
    public static System.TimeSpan ParseExact(string input, string[] formats, System.IFormatProvider formatProvider) { return default(System.TimeSpan); }
    public static System.TimeSpan ParseExact(string input, string[] formats, System.IFormatProvider formatProvider, System.Globalization.TimeSpanStyles styles) { return default(System.TimeSpan); }
    public System.TimeSpan Subtract(System.TimeSpan ts) { return default(System.TimeSpan); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    public override string ToString() { return default(string); }
    public string ToString(string format) { return default(string); }
    public string ToString(string format, System.IFormatProvider formatProvider) { return default(string); }
    public static bool TryParse(string input, System.IFormatProvider formatProvider, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
    public static bool TryParse(string s, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
    public static bool TryParseExact(string input, string format, System.IFormatProvider formatProvider, System.Globalization.TimeSpanStyles styles, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
    public static bool TryParseExact(string input, string format, System.IFormatProvider formatProvider, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
    public static bool TryParseExact(string input, string[] formats, System.IFormatProvider formatProvider, System.Globalization.TimeSpanStyles styles, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
    public static bool TryParseExact(string input, string[] formats, System.IFormatProvider formatProvider, out System.TimeSpan result) { result = default(System.TimeSpan); return default(bool); }
  }
  [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
  public sealed partial class TimeZoneInfo : System.IEquatable<System.TimeZoneInfo>, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
    internal TimeZoneInfo() { }
    public System.TimeSpan BaseUtcOffset { get { return default(System.TimeSpan); } }
    public string DaylightName { get { return default(string); } }
    public string DisplayName { get { return default(string); } }
    public string Id { get { return default(string); } }
    public static System.TimeZoneInfo Local { get { return default(System.TimeZoneInfo); } }
    public string StandardName { get { return default(string); } }
    public bool SupportsDaylightSavingTime { get { return default(bool); } }
    public static System.TimeZoneInfo Utc { get { return default(System.TimeZoneInfo); } }
    public static void ClearCachedData() { }
    public static System.DateTime ConvertTime(System.DateTime dateTime, System.TimeZoneInfo destinationTimeZone) { return default(System.DateTime); }
    public static System.DateTime ConvertTime(System.DateTime dateTime, System.TimeZoneInfo sourceTimeZone, System.TimeZoneInfo destinationTimeZone) { return default(System.DateTime); }
    public static System.DateTimeOffset ConvertTime(System.DateTimeOffset dateTimeOffset, System.TimeZoneInfo destinationTimeZone) { return default(System.DateTimeOffset); }
    public static System.DateTime ConvertTimeBySystemTimeZoneId(System.DateTime dateTime, string destinationTimeZoneId) { return default(System.DateTime); }
    public static System.DateTime ConvertTimeBySystemTimeZoneId(System.DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId) { return default(System.DateTime); }
    public static System.DateTimeOffset ConvertTimeBySystemTimeZoneId(System.DateTimeOffset dateTimeOffset, string destinationTimeZoneId) { return default(System.DateTimeOffset); }
    public static System.DateTime ConvertTimeFromUtc(System.DateTime dateTime, System.TimeZoneInfo destinationTimeZone) { return default(System.DateTime); }
    public static System.DateTime ConvertTimeToUtc(System.DateTime dateTime) { return default(System.DateTime); }
    public static System.DateTime ConvertTimeToUtc(System.DateTime dateTime, System.TimeZoneInfo sourceTimeZone) { return default(System.DateTime); }
    public static System.TimeZoneInfo CreateCustomTimeZone(string id, System.TimeSpan baseUtcOffset, string displayName, string standardDisplayName) { return default(System.TimeZoneInfo); }
    public static System.TimeZoneInfo CreateCustomTimeZone(string id, System.TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, System.TimeZoneInfo.AdjustmentRule[] adjustmentRules) { return default(System.TimeZoneInfo); }
    public static System.TimeZoneInfo CreateCustomTimeZone(string id, System.TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, System.TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime) { return default(System.TimeZoneInfo); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(System.TimeZoneInfo other) { return default(bool); }
    public static System.TimeZoneInfo FindSystemTimeZoneById(string id) { return default(System.TimeZoneInfo); }
    public static System.TimeZoneInfo FromSerializedString(string source) { return default(System.TimeZoneInfo); }
    public System.TimeZoneInfo.AdjustmentRule[] GetAdjustmentRules() { return default(System.TimeZoneInfo.AdjustmentRule[]); }
    public System.TimeSpan[] GetAmbiguousTimeOffsets(System.DateTime dateTime) { return default(System.TimeSpan[]); }
    public System.TimeSpan[] GetAmbiguousTimeOffsets(System.DateTimeOffset dateTimeOffset) { return default(System.TimeSpan[]); }
    public override int GetHashCode() { return default(int); }
    public static System.Collections.ObjectModel.ReadOnlyCollection<System.TimeZoneInfo> GetSystemTimeZones() { return default(System.Collections.ObjectModel.ReadOnlyCollection<System.TimeZoneInfo>); }
    public System.TimeSpan GetUtcOffset(System.DateTime dateTime) { return default(System.TimeSpan); }
    public System.TimeSpan GetUtcOffset(System.DateTimeOffset dateTimeOffset) { return default(System.TimeSpan); }
    public bool HasSameRules(System.TimeZoneInfo other) { return default(bool); }
    public bool IsAmbiguousTime(System.DateTime dateTime) { return default(bool); }
    public bool IsAmbiguousTime(System.DateTimeOffset dateTimeOffset) { return default(bool); }
    public bool IsDaylightSavingTime(System.DateTime dateTime) { return default(bool); }
    public bool IsDaylightSavingTime(System.DateTimeOffset dateTimeOffset) { return default(bool); }
    public bool IsInvalidTime(System.DateTime dateTime) { return default(bool); }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public string ToSerializedString() { return default(string); }
    public override string ToString() { return default(string); }
    [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
    public sealed partial class AdjustmentRule : System.IEquatable<System.TimeZoneInfo.AdjustmentRule>, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
      internal AdjustmentRule() { }
      public System.DateTime DateEnd { get { return default(System.DateTime); } }
      public System.DateTime DateStart { get { return default(System.DateTime); } }
      public System.TimeSpan DaylightDelta { get { return default(System.TimeSpan); } }
      public System.TimeZoneInfo.TransitionTime DaylightTransitionEnd { get { return default(System.TimeZoneInfo.TransitionTime); } }
      public System.TimeZoneInfo.TransitionTime DaylightTransitionStart { get { return default(System.TimeZoneInfo.TransitionTime); } }
      public static System.TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(System.DateTime dateStart, System.DateTime dateEnd, System.TimeSpan daylightDelta, System.TimeZoneInfo.TransitionTime daylightTransitionStart, System.TimeZoneInfo.TransitionTime daylightTransitionEnd) { return default(System.TimeZoneInfo.AdjustmentRule); }
      public bool Equals(System.TimeZoneInfo.AdjustmentRule other) { return default(bool); }
      public override int GetHashCode() { return default(int); }
      void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
      [System.Security.SecurityCriticalAttribute]
      void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    [System.Runtime.CompilerServices.TypeForwardedFromAttribute("System.Core, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=7cec85d7bea7798e")]
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct TransitionTime : System.IEquatable<System.TimeZoneInfo.TransitionTime>, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
      public int Day { get { return default(int); } }
      public System.DayOfWeek DayOfWeek { get { return default(System.DayOfWeek); } }
      public bool IsFixedDateRule { get { return default(bool); } }
      public int Month { get { return default(int); } }
      public System.DateTime TimeOfDay { get { return default(System.DateTime); } }
      public int Week { get { return default(int); } }
      public static System.TimeZoneInfo.TransitionTime CreateFixedDateRule(System.DateTime timeOfDay, int month, int day) { return default(System.TimeZoneInfo.TransitionTime); }
      public static System.TimeZoneInfo.TransitionTime CreateFloatingDateRule(System.DateTime timeOfDay, int month, int week, System.DayOfWeek dayOfWeek) { return default(System.TimeZoneInfo.TransitionTime); }
      public override bool Equals(object obj) { return default(bool); }
      public bool Equals(System.TimeZoneInfo.TransitionTime other) { return default(bool); }
      public override int GetHashCode() { return default(int); }
      public static bool operator ==(System.TimeZoneInfo.TransitionTime t1, System.TimeZoneInfo.TransitionTime t2) { return default(bool); }
      public static bool operator !=(System.TimeZoneInfo.TransitionTime t1, System.TimeZoneInfo.TransitionTime t2) { return default(bool); }
      void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
      [System.Security.SecurityCriticalAttribute]
      void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
  }
  public static partial class Tuple {
    public static System.Tuple<T1> Create<T1>(T1 item1) { return default(System.Tuple<T1>); }
    public static System.Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return default(System.Tuple<T1, T2>); }
    public static System.Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return default(System.Tuple<T1, T2, T3>); }
    public static System.Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return default(System.Tuple<T1, T2, T3, T4>); }
    public static System.Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { return default(System.Tuple<T1, T2, T3, T4, T5>); }
    public static System.Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { return default(System.Tuple<T1, T2, T3, T4, T5, T6>); }
    public static System.Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { return default(System.Tuple<T1, T2, T3, T4, T5, T6, T7>); }
    public static System.Tuple<T1, T2, T3, T4, T5, T6, T7, System.Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) { return default(System.Tuple<T1, T2, T3, T4, T5, T6, T7, System.Tuple<T8>>); }
  }
  public partial class Tuple<T1> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1) { }
    public T1 Item1 { get { return default(T1); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3, T4> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public T4 Item4 { get { return default(T4); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3, T4, T5> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public T4 Item4 { get { return default(T4); } }
    public T5 Item5 { get { return default(T5); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3, T4, T5, T6> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public T4 Item4 { get { return default(T4); } }
    public T5 Item5 { get { return default(T5); } }
    public T6 Item6 { get { return default(T6); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3, T4, T5, T6, T7> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public T4 Item4 { get { return default(T4); } }
    public T5 Item5 { get { return default(T5); } }
    public T6 Item6 { get { return default(T6); } }
    public T7 Item7 { get { return default(T7); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.IComparable {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest) { }
    public T1 Item1 { get { return default(T1); } }
    public T2 Item2 { get { return default(T2); } }
    public T3 Item3 { get { return default(T3); } }
    public T4 Item4 { get { return default(T4); } }
    public T5 Item5 { get { return default(T5); } }
    public T6 Item6 { get { return default(T6); } }
    public T7 Item7 { get { return default(T7); } }
    public TRest Rest { get { return default(TRest); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    int System.Collections.IStructuralComparable.CompareTo(object other, System.Collections.IComparer comparer) { return default(int); }
    bool System.Collections.IStructuralEquatable.Equals(object other, System.Collections.IEqualityComparer comparer) { return default(bool); }
    int System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer) { return default(int); }
    int System.IComparable.CompareTo(object obj) { return default(int); }
    public override string ToString() { return default(string); }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class Type : System.Reflection.MemberInfo, System.Reflection.IReflect {
    public static readonly char Delimiter;
    public static readonly System.Type[] EmptyTypes;
    public static readonly System.Reflection.MemberFilter FilterAttribute;
    public static readonly System.Reflection.MemberFilter FilterName;
    public static readonly System.Reflection.MemberFilter FilterNameIgnoreCase;
    public static readonly object Missing;
    protected Type() { }
    public abstract System.Reflection.Assembly Assembly { get; }
    public abstract string AssemblyQualifiedName { get; }
    public System.Reflection.TypeAttributes Attributes { get { return default(System.Reflection.TypeAttributes); } }
    public abstract System.Type BaseType { get; }
    public virtual bool ContainsGenericParameters { get { return default(bool); } }
    public virtual System.Reflection.MethodBase DeclaringMethod { get { return default(System.Reflection.MethodBase); } }
    public override System.Type DeclaringType { get { return default(System.Type); } }
    public static System.Reflection.Binder DefaultBinder { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Reflection.Binder); } }
    public abstract string FullName { get; }
    public virtual System.Reflection.GenericParameterAttributes GenericParameterAttributes { get { return default(System.Reflection.GenericParameterAttributes); } }
    public virtual int GenericParameterPosition { get { return default(int); } }
    public virtual System.Type[] GenericTypeArguments { get { return default(System.Type[]); } }
    public abstract System.Guid GUID { get; }
    public bool HasElementType { get { return default(bool); } }
    public bool IsAbstract { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsAnsiClass { get { return default(bool); } }
    public bool IsArray { get { return default(bool); } }
    public bool IsAutoClass { get { return default(bool); } }
    public bool IsAutoLayout { get { return default(bool); } }
    public bool IsByRef { get { return default(bool); } }
    public bool IsClass { get { return default(bool); } }
    public bool IsCOMObject { get { return default(bool); } }
    public virtual bool IsConstructedGenericType { get { return default(bool); } }
    public bool IsContextful { get { return default(bool); } }
    public virtual bool IsEnum { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsExplicitLayout { get { return default(bool); } }
    public virtual bool IsGenericParameter { get { return default(bool); } }
    public virtual bool IsGenericType { get { return default(bool); } }
    public virtual bool IsGenericTypeDefinition { get { return default(bool); } }
    public bool IsImport { get { return default(bool); } }
    public bool IsInterface { [System.Security.SecuritySafeCriticalAttribute]get { return default(bool); } }
    public bool IsLayoutSequential { get { return default(bool); } }
    public bool IsMarshalByRef { get { return default(bool); } }
    public bool IsNested { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsNestedAssembly { get { return default(bool); } }
    public bool IsNestedFamANDAssem { get { return default(bool); } }
    public bool IsNestedFamily { get { return default(bool); } }
    public bool IsNestedFamORAssem { get { return default(bool); } }
    public bool IsNestedPrivate { get { return default(bool); } }
    public bool IsNestedPublic { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsNotPublic { get { return default(bool); } }
    public bool IsPointer { get { return default(bool); } }
    public bool IsPrimitive { get { return default(bool); } }
    public bool IsPublic { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsSealed { get { return default(bool); } }
    public virtual bool IsSecurityCritical { get { return default(bool); } }
    public virtual bool IsSecuritySafeCritical { get { return default(bool); } }
    public virtual bool IsSecurityTransparent { get { return default(bool); } }
    public virtual bool IsSerializable { get { return default(bool); } }
    public bool IsSpecialName { get { return default(bool); } }
    public bool IsUnicodeClass { get { return default(bool); } }
    public bool IsValueType { get { return default(bool); } }
    public bool IsVisible { get { return default(bool); } }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public abstract override System.Reflection.Module Module { get; }
    public override string Name { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(string); } }
    public abstract string Namespace { get; }
    public override System.Type ReflectedType { get { return default(System.Type); } }
    public virtual System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute { get { return default(System.Runtime.InteropServices.StructLayoutAttribute); } }
    public virtual System.RuntimeTypeHandle TypeHandle { get { return default(System.RuntimeTypeHandle); } }
    public System.Reflection.ConstructorInfo TypeInitializer { get { return default(System.Reflection.ConstructorInfo); } }
    public abstract System.Type UnderlyingSystemType { get; }
    public override bool Equals(object o) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public virtual bool Equals(System.Type o) { return default(bool); }
    public virtual System.Type[] FindInterfaces(System.Reflection.TypeFilter filter, object filterCriteria) { return default(System.Type[]); }
    public virtual System.Reflection.MemberInfo[] FindMembers(System.Reflection.MemberTypes memberType, System.Reflection.BindingFlags bindingAttr, System.Reflection.MemberFilter filter, object filterCriteria) { return default(System.Reflection.MemberInfo[]); }
    public virtual int GetArrayRank() { return default(int); }
    protected abstract System.Reflection.TypeAttributes GetAttributeFlagsImpl();
    public System.Reflection.ConstructorInfo GetConstructor(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.ConstructorInfo); }
    public System.Reflection.ConstructorInfo GetConstructor(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.ConstructorInfo); }
    public System.Reflection.ConstructorInfo GetConstructor(System.Type[] types) { return default(System.Reflection.ConstructorInfo); }
    protected abstract System.Reflection.ConstructorInfo GetConstructorImpl(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    public System.Reflection.ConstructorInfo[] GetConstructors() { return default(System.Reflection.ConstructorInfo[]); }
    public abstract System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr);
    public virtual System.Reflection.MemberInfo[] GetDefaultMembers() { return default(System.Reflection.MemberInfo[]); }
    public abstract System.Type GetElementType();
    public virtual string GetEnumName(object value) { return default(string); }
    public virtual string[] GetEnumNames() { return default(string[]); }
    public virtual System.Type GetEnumUnderlyingType() { return default(System.Type); }
    public virtual System.Array GetEnumValues() { return default(System.Array); }
    public System.Reflection.EventInfo GetEvent(string name) { return default(System.Reflection.EventInfo); }
    public abstract System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr);
    public virtual System.Reflection.EventInfo[] GetEvents() { return default(System.Reflection.EventInfo[]); }
    public abstract System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr);
    public System.Reflection.FieldInfo GetField(string name) { return default(System.Reflection.FieldInfo); }
    public abstract System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr);
    public System.Reflection.FieldInfo[] GetFields() { return default(System.Reflection.FieldInfo[]); }
    public abstract System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr);
    public virtual System.Type[] GetGenericArguments() { return default(System.Type[]); }
    public virtual System.Type[] GetGenericParameterConstraints() { return default(System.Type[]); }
    public virtual System.Type GetGenericTypeDefinition() { return default(System.Type); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override int GetHashCode() { return default(int); }
    public System.Type GetInterface(string name) { return default(System.Type); }
    public abstract System.Type GetInterface(string name, bool ignoreCase);
    public virtual System.Reflection.InterfaceMapping GetInterfaceMap(System.Type interfaceType) { return default(System.Reflection.InterfaceMapping); }
    public abstract System.Type[] GetInterfaces();
    public System.Reflection.MemberInfo[] GetMember(string name) { return default(System.Reflection.MemberInfo[]); }
    public virtual System.Reflection.MemberInfo[] GetMember(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MemberInfo[]); }
    public virtual System.Reflection.MemberInfo[] GetMember(string name, System.Reflection.MemberTypes type, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MemberInfo[]); }
    public System.Reflection.MemberInfo[] GetMembers() { return default(System.Reflection.MemberInfo[]); }
    public abstract System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr);
    public System.Reflection.MethodInfo GetMethod(string name) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Type[] types) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    protected abstract System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    public System.Reflection.MethodInfo[] GetMethods() { return default(System.Reflection.MethodInfo[]); }
    public abstract System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr);
    public System.Type GetNestedType(string name) { return default(System.Type); }
    public abstract System.Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr);
    public System.Type[] GetNestedTypes() { return default(System.Type[]); }
    public abstract System.Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr);
    public System.Reflection.PropertyInfo[] GetProperties() { return default(System.Reflection.PropertyInfo[]); }
    public abstract System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr);
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Reflection.PropertyInfo GetProperty(string name) { return default(System.Reflection.PropertyInfo); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Reflection.PropertyInfo GetProperty(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.PropertyInfo); }
    public System.Reflection.PropertyInfo GetProperty(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.PropertyInfo); }
    public System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType) { return default(System.Reflection.PropertyInfo); }
    public System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType, System.Type[] types) { return default(System.Reflection.PropertyInfo); }
    public System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.PropertyInfo); }
    public System.Reflection.PropertyInfo GetProperty(string name, System.Type[] types) { return default(System.Reflection.PropertyInfo); }
    protected abstract System.Reflection.PropertyInfo GetPropertyImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    public new System.Type GetType() { return default(System.Type); }
    public static System.Type GetType(string typeName) { return default(System.Type); }
    public static System.Type GetType(string typeName, bool throwOnError) { return default(System.Type); }
    public static System.Type GetType(string typeName, bool throwOnError, bool ignoreCase) { return default(System.Type); }
    public static System.Type GetType(string typeName, System.Func<System.Reflection.AssemblyName, System.Reflection.Assembly> assemblyResolver, System.Func<System.Reflection.Assembly, string, bool, System.Type> typeResolver) { return default(System.Type); }
    public static System.Type GetType(string typeName, System.Func<System.Reflection.AssemblyName, System.Reflection.Assembly> assemblyResolver, System.Func<System.Reflection.Assembly, string, bool, System.Type> typeResolver, bool throwOnError) { return default(System.Type); }
    public static System.Type GetType(string typeName, System.Func<System.Reflection.AssemblyName, System.Reflection.Assembly> assemblyResolver, System.Func<System.Reflection.Assembly, string, bool, System.Type> typeResolver, bool throwOnError, bool ignoreCase) { return default(System.Type); }
    public static System.Type[] GetTypeArray(object[] args) { return default(System.Type[]); }
    public static System.TypeCode GetTypeCode(System.Type type) { return default(System.TypeCode); }
    protected virtual System.TypeCode GetTypeCodeImpl() { return default(System.TypeCode); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static System.Type GetTypeFromCLSID(System.Guid clsid) { return default(System.Type); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static System.Type GetTypeFromCLSID(System.Guid clsid, bool throwOnError) { return default(System.Type); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static System.Type GetTypeFromCLSID(System.Guid clsid, string server) { return default(System.Type); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static System.Type GetTypeFromCLSID(System.Guid clsid, string server, bool throwOnError) { return default(System.Type); }
    public static System.Type GetTypeFromHandle(System.RuntimeTypeHandle handle) { return default(System.Type); }
    [System.Security.SecurityCriticalAttribute]
    public static System.Type GetTypeFromProgID(string progID) { return default(System.Type); }
    [System.Security.SecurityCriticalAttribute]
    public static System.Type GetTypeFromProgID(string progID, bool throwOnError) { return default(System.Type); }
    [System.Security.SecurityCriticalAttribute]
    public static System.Type GetTypeFromProgID(string progID, string server) { return default(System.Type); }
    [System.Security.SecurityCriticalAttribute]
    public static System.Type GetTypeFromProgID(string progID, string server, bool throwOnError) { return default(System.Type); }
    public static System.RuntimeTypeHandle GetTypeHandle(object o) { return default(System.RuntimeTypeHandle); }
    protected abstract bool HasElementTypeImpl();
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args) { return default(object); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Globalization.CultureInfo culture) { return default(object); }
    public abstract object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters);
    protected abstract bool IsArrayImpl();
    public virtual bool IsAssignableFrom(System.Type c) { return default(bool); }
    protected abstract bool IsByRefImpl();
    protected abstract bool IsCOMObjectImpl();
    protected virtual bool IsContextfulImpl() { return default(bool); }
    public virtual bool IsEnumDefined(object value) { return default(bool); }
    public virtual bool IsEquivalentTo(System.Type other) { return default(bool); }
    public virtual bool IsInstanceOfType(object o) { return default(bool); }
    protected virtual bool IsMarshalByRefImpl() { return default(bool); }
    protected abstract bool IsPointerImpl();
    protected abstract bool IsPrimitiveImpl();
    public virtual bool IsSubclassOf(System.Type c) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected virtual bool IsValueTypeImpl() { return default(bool); }
    public virtual System.Type MakeArrayType() { return default(System.Type); }
    public virtual System.Type MakeArrayType(int rank) { return default(System.Type); }
    public virtual System.Type MakeByRefType() { return default(System.Type); }
    public virtual System.Type MakeGenericType(params System.Type[] typeArguments) { return default(System.Type); }
    public virtual System.Type MakePointerType() { return default(System.Type); }
    public static bool operator ==(System.Type left, System.Type right) { return default(bool); }
    public static bool operator !=(System.Type left, System.Type right) { return default(bool); }
    public static System.Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase) { return default(System.Type); }
    public override string ToString() { return default(string); }
  }
  public partial class TypeAccessException : System.TypeLoadException {
    public TypeAccessException() { }
    protected TypeAccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TypeAccessException(string message) { }
    public TypeAccessException(string message, System.Exception inner) { }
  }
  public enum TypeCode {
    Boolean = 3,
    Byte = 6,
    Char = 4,
    DateTime = 16,
    DBNull = 2,
    Decimal = 15,
    Double = 14,
    Empty = 0,
    Int16 = 7,
    Int32 = 9,
    Int64 = 11,
    Object = 1,
    SByte = 5,
    Single = 13,
    String = 18,
    UInt16 = 8,
    UInt32 = 10,
    UInt64 = 12,
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct TypedReference {
    public override bool Equals(object o) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static System.Type GetTargetType(System.TypedReference value) { return default(System.Type); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public static System.TypedReference MakeTypedReference(object target, System.Reflection.FieldInfo[] flds) { return default(System.TypedReference); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecuritySafeCriticalAttribute]
    public static void SetTypedReference(System.TypedReference target, object value) { }
    public static System.RuntimeTypeHandle TargetTypeToken(System.TypedReference value) { return default(System.RuntimeTypeHandle); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static object ToObject(System.TypedReference value) { return default(object); }
  }
  public sealed partial class TypeInitializationException : System.SystemException {
    public TypeInitializationException(string fullTypeName, System.Exception innerException) { }
    public string TypeName { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class TypeLoadException : System.SystemException, System.Runtime.Serialization.ISerializable {
    public TypeLoadException() { }
    protected TypeLoadException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TypeLoadException(string message) { }
    public TypeLoadException(string message, System.Exception inner) { }
    public override string Message { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
    public string TypeName { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class TypeUnloadedException : System.SystemException {
    public TypeUnloadedException() { }
    protected TypeUnloadedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TypeUnloadedException(string message) { }
    public TypeUnloadedException(string message, System.Exception innerException) { }
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct UInt16 : System.IComparable, System.IComparable<ushort>, System.IConvertible, System.IEquatable<ushort>, System.IFormattable {
    public const ushort MaxValue = (ushort)65535;
    public const ushort MinValue = (ushort)0;
    public int CompareTo(object value) { return default(int); }
    public int CompareTo(ushort value) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(ushort obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.CLSCompliantAttribute(false)]
    public static ushort Parse(string s) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort Parse(string s, System.Globalization.NumberStyles style) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public static ushort Parse(string s, System.IFormatProvider provider) { return default(ushort); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out ushort result) { result = default(ushort); return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, out ushort result) { result = default(ushort); return default(bool); }
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct UInt32 : System.IComparable, System.IComparable<uint>, System.IConvertible, System.IEquatable<uint>, System.IFormattable {
    public const uint MaxValue = (uint)4294967295;
    public const uint MinValue = (uint)0;
    public int CompareTo(object value) { return default(int); }
    public int CompareTo(uint value) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(uint obj) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public override int GetHashCode() { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.CLSCompliantAttribute(false)]
    public static uint Parse(string s) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint Parse(string s, System.Globalization.NumberStyles style) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public static uint Parse(string s, System.IFormatProvider provider) { return default(uint); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out uint result) { result = default(uint); return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, out uint result) { result = default(uint); return default(bool); }
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct UInt64 : System.IComparable, System.IComparable<ulong>, System.IConvertible, System.IEquatable<ulong>, System.IFormattable {
    public const ulong MaxValue = (ulong)18446744073709551615;
    public const ulong MinValue = (ulong)0;
    public int CompareTo(object value) { return default(int); }
    public int CompareTo(ulong value) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(ulong obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public System.TypeCode GetTypeCode() { return default(System.TypeCode); }
    [System.CLSCompliantAttribute(false)]
    public static ulong Parse(string s) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong Parse(string s, System.Globalization.NumberStyles style) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong Parse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public static ulong Parse(string s, System.IFormatProvider provider) { return default(ulong); }
    int System.IComparable.CompareTo(object value) { return default(int); }
    System.TypeCode System.IConvertible.GetTypeCode() { return default(System.TypeCode); }
    bool System.IConvertible.ToBoolean(System.IFormatProvider provider) { return default(bool); }
    byte System.IConvertible.ToByte(System.IFormatProvider provider) { return default(byte); }
    char System.IConvertible.ToChar(System.IFormatProvider provider) { return default(char); }
    System.DateTime System.IConvertible.ToDateTime(System.IFormatProvider provider) { return default(System.DateTime); }
    decimal System.IConvertible.ToDecimal(System.IFormatProvider provider) { return default(decimal); }
    double System.IConvertible.ToDouble(System.IFormatProvider provider) { return default(double); }
    short System.IConvertible.ToInt16(System.IFormatProvider provider) { return default(short); }
    int System.IConvertible.ToInt32(System.IFormatProvider provider) { return default(int); }
    long System.IConvertible.ToInt64(System.IFormatProvider provider) { return default(long); }
    sbyte System.IConvertible.ToSByte(System.IFormatProvider provider) { return default(sbyte); }
    float System.IConvertible.ToSingle(System.IFormatProvider provider) { return default(float); }
    object System.IConvertible.ToType(System.Type type, System.IFormatProvider provider) { return default(object); }
    ushort System.IConvertible.ToUInt16(System.IFormatProvider provider) { return default(ushort); }
    uint System.IConvertible.ToUInt32(System.IFormatProvider provider) { return default(uint); }
    ulong System.IConvertible.ToUInt64(System.IFormatProvider provider) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(System.IFormatProvider provider) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format) { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(string format, System.IFormatProvider provider) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, System.Globalization.NumberStyles style, System.IFormatProvider provider, out ulong result) { result = default(ulong); return default(bool); }
    [System.CLSCompliantAttribute(false)]
    public static bool TryParse(string s, out ulong result) { result = default(ulong); return default(bool); }
  }
  [System.CLSCompliantAttribute(false)]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct UIntPtr : System.Runtime.Serialization.ISerializable {
    public static readonly System.UIntPtr Zero;
    public UIntPtr(uint value) { throw new System.NotImplementedException(); }
    public UIntPtr(ulong value) { throw new System.NotImplementedException(); }
    [System.CLSCompliantAttribute(false)]
    public unsafe UIntPtr(void* value) { throw new System.NotImplementedException(); }
    public static int Size { get { return default(int); } }
    public static System.UIntPtr Add(System.UIntPtr pointer, int offset) { return default(System.UIntPtr); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static System.UIntPtr operator +(System.UIntPtr pointer, int offset) { return default(System.UIntPtr); }
    public static bool operator ==(System.UIntPtr value1, System.UIntPtr value2) { return default(bool); }
    public static explicit operator System.UIntPtr (uint value) { return default(System.UIntPtr); }
    public static explicit operator System.UIntPtr (ulong value) { return default(System.UIntPtr); }
    public static explicit operator uint (System.UIntPtr value) { return default(uint); }
    public static explicit operator ulong (System.UIntPtr value) { return default(ulong); }
    [System.CLSCompliantAttribute(false)]
    public unsafe static explicit operator void* (System.UIntPtr value) { return default(void*); }
    [System.CLSCompliantAttribute(false)]
    public unsafe static explicit operator System.UIntPtr (void* value) { return default(System.UIntPtr); }
    public static bool operator !=(System.UIntPtr value1, System.UIntPtr value2) { return default(bool); }
    public static System.UIntPtr operator -(System.UIntPtr pointer, int offset) { return default(System.UIntPtr); }
    public static System.UIntPtr Subtract(System.UIntPtr pointer, int offset) { return default(System.UIntPtr); }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    [System.CLSCompliantAttribute(false)]
    public unsafe void* ToPointer() { return default(void*); }
    public override string ToString() { return default(string); }
    public uint ToUInt32() { return default(uint); }
    public ulong ToUInt64() { return default(ulong); }
  }
  public partial class UnauthorizedAccessException : System.SystemException {
    public UnauthorizedAccessException() { }
    protected UnauthorizedAccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public UnauthorizedAccessException(string message) { }
    public UnauthorizedAccessException(string message, System.Exception inner) { }
  }
  public partial class UnhandledExceptionEventArgs : System.EventArgs {
    public UnhandledExceptionEventArgs(object exception, bool isTerminating) { }
    public object ExceptionObject { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(object); } }
    public bool IsTerminating { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(bool); } }
  }
  public delegate void UnhandledExceptionEventHandler(object sender, System.UnhandledExceptionEventArgs e);
  public partial class Uri : System.Runtime.Serialization.ISerializable {
    public static readonly string SchemeDelimiter;
    public static readonly string UriSchemeFile;
    public static readonly string UriSchemeFtp;
    public static readonly string UriSchemeGopher;
    public static readonly string UriSchemeHttp;
    public static readonly string UriSchemeHttps;
    public static readonly string UriSchemeMailto;
    public static readonly string UriSchemeNetPipe;
    public static readonly string UriSchemeNetTcp;
    public static readonly string UriSchemeNews;
    public static readonly string UriSchemeNntp;
    protected Uri(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) { }
    public Uri(string uriString) { }
    [System.ObsoleteAttribute]
    public Uri(string uriString, bool dontEscape) { }
    public Uri(string uriString, System.UriKind uriKind) { }
    public Uri(System.Uri baseUri, string relativeUri) { }
    [System.ObsoleteAttribute("dontEscape is always false")]
    public Uri(System.Uri baseUri, string relativeUri, bool dontEscape) { }
    public Uri(System.Uri baseUri, System.Uri relativeUri) { }
    public string AbsolutePath { get { return default(string); } }
    public string AbsoluteUri { get { return default(string); } }
    public string Authority { get { return default(string); } }
    public string DnsSafeHost { get { return default(string); } }
    public string Fragment { get { return default(string); } }
    public string Host { get { return default(string); } }
    public System.UriHostNameType HostNameType { get { return default(System.UriHostNameType); } }
    public string IdnHost { get { return default(string); } }
    public bool IsAbsoluteUri { get { return default(bool); } }
    public bool IsDefaultPort { get { return default(bool); } }
    public bool IsFile { get { return default(bool); } }
    public bool IsLoopback { get { return default(bool); } }
    public bool IsUnc { get { return default(bool); } }
    public string LocalPath { get { return default(string); } }
    public string OriginalString { get { return default(string); } }
    public string PathAndQuery { get { return default(string); } }
    public int Port { get { return default(int); } }
    public string Query { get { return default(string); } }
    public string Scheme { get { return default(string); } }
    public string[] Segments { get { return default(string[]); } }
    public bool UserEscaped { get { return default(bool); } }
    public string UserInfo { get { return default(string); } }
    public static System.UriHostNameType CheckHostName(string name) { return default(System.UriHostNameType); }
    public static bool CheckSchemeName(string schemeName) { return default(bool); }
    public static int Compare(System.Uri uri1, System.Uri uri2, System.UriComponents partsToCompare, System.UriFormat compareFormat, System.StringComparison comparisonType) { return default(int); }
    public override bool Equals(object comparand) { return default(bool); }
    [System.ObsoleteAttribute]
    protected virtual void Escape() { }
    public static string EscapeDataString(string stringToEscape) { return default(string); }
    [System.ObsoleteAttribute]
    protected static string EscapeString(string str) { return default(string); }
    public static string EscapeUriString(string stringToEscape) { return default(string); }
    public static int FromHex(char digit) { return default(int); }
    public string GetComponents(System.UriComponents components, System.UriFormat format) { return default(string); }
    public override int GetHashCode() { return default(int); }
    public string GetLeftPart(System.UriPartial part) { return default(string); }
    protected void GetObjectData(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) { }
    public static string HexEscape(char character) { return default(string); }
    public static char HexUnescape(string pattern, ref int index) { return default(char); }
    [System.ObsoleteAttribute]
    protected virtual bool IsBadFileSystemCharacter(char character) { return default(bool); }
    public bool IsBaseOf(System.Uri uri) { return default(bool); }
    [System.ObsoleteAttribute]
    protected static bool IsExcludedCharacter(char character) { return default(bool); }
    public static bool IsHexDigit(char character) { return default(bool); }
    public static bool IsHexEncoding(string pattern, int index) { return default(bool); }
    [System.ObsoleteAttribute]
    protected virtual bool IsReservedCharacter(char character) { return default(bool); }
    public bool IsWellFormedOriginalString() { return default(bool); }
    public static bool IsWellFormedUriString(string uriString, System.UriKind uriKind) { return default(bool); }
    [System.ObsoleteAttribute("Use MakeRelativeUri(Uri uri) instead.")]
    public string MakeRelative(System.Uri toUri) { return default(string); }
    public System.Uri MakeRelativeUri(System.Uri uri) { return default(System.Uri); }
    public static bool operator ==(System.Uri uri1, System.Uri uri2) { return default(bool); }
    public static bool operator !=(System.Uri uri1, System.Uri uri2) { return default(bool); }
    [System.ObsoleteAttribute("The method has been deprecated. It is not used by the system.")]
    protected virtual void Parse() { }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
    public static bool TryCreate(string uriString, System.UriKind uriKind, out System.Uri result) { result = default(System.Uri); return default(bool); }
    public static bool TryCreate(System.Uri baseUri, string relativeUri, out System.Uri result) { result = default(System.Uri); return default(bool); }
    public static bool TryCreate(System.Uri baseUri, System.Uri relativeUri, out System.Uri result) { result = default(System.Uri); return default(bool); }
    [System.ObsoleteAttribute]
    protected virtual string Unescape(string path) { return default(string); }
    public static string UnescapeDataString(string stringToUnescape) { return default(string); }
  }
  [System.FlagsAttribute]
  public enum UriComponents {
    AbsoluteUri = 127,
    Fragment = 64,
    Host = 4,
    HostAndPort = 132,
    HttpRequestUrl = 61,
    KeepDelimiter = 1073741824,
    NormalizedHost = 256,
    Path = 16,
    PathAndQuery = 48,
    Port = 8,
    Query = 32,
    Scheme = 1,
    SchemeAndServer = 13,
    SerializationInfoString = -2147483648,
    StrongAuthority = 134,
    StrongPort = 128,
    UserInfo = 2,
  }
  public enum UriFormat {
    SafeUnescaped = 3,
    Unescaped = 2,
    UriEscaped = 1,
  }
  public partial class UriFormatException : System.FormatException, System.Runtime.Serialization.ISerializable {
    public UriFormatException() { }
    protected UriFormatException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) { }
    public UriFormatException(string textString) { }
    public UriFormatException(string textString, System.Exception e) { }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public enum UriHostNameType {
    Basic = 1,
    Dns = 2,
    IPv4 = 3,
    IPv6 = 4,
    Unknown = 0,
  }
  public enum UriKind {
    Absolute = 1,
    Relative = 2,
    RelativeOrAbsolute = 0,
  }
  public enum UriPartial {
    Authority = 1,
    Path = 2,
    Query = 3,
    Scheme = 0,
  }
  public abstract partial class ValueType {
    protected ValueType() { }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public override string ToString() { return default(string); }
  }
  public sealed partial class Version : System.ICloneable, System.IComparable, System.IComparable<System.Version>, System.IEquatable<System.Version> {
    public Version() { }
    public Version(int major, int minor) { }
    public Version(int major, int minor, int build) { }
    public Version(int major, int minor, int build, int revision) { }
    public Version(string version) { }
    public int Build { get { return default(int); } }
    public int Major { get { return default(int); } }
    public short MajorRevision { get { return default(short); } }
    public int Minor { get { return default(int); } }
    public short MinorRevision { get { return default(short); } }
    public int Revision { get { return default(int); } }
    public object Clone() { return default(object); }
    public int CompareTo(object version) { return default(int); }
    public int CompareTo(System.Version value) { return default(int); }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(System.Version obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.Version v1, System.Version v2) { return default(bool); }
    public static bool operator >(System.Version v1, System.Version v2) { return default(bool); }
    public static bool operator >=(System.Version v1, System.Version v2) { return default(bool); }
    public static bool operator !=(System.Version v1, System.Version v2) { return default(bool); }
    public static bool operator <(System.Version v1, System.Version v2) { return default(bool); }
    public static bool operator <=(System.Version v1, System.Version v2) { return default(bool); }
    public static System.Version Parse(string input) { return default(System.Version); }
    int System.IComparable.CompareTo(object version) { return default(int); }
    public override string ToString() { return default(string); }
    public string ToString(int fieldCount) { return default(string); }
    public static bool TryParse(string input, out System.Version result) { result = default(System.Version); return default(bool); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
  public partial struct Void {
  }
  public partial class WeakReference : System.Runtime.Serialization.ISerializable {
    protected WeakReference() { }
    public WeakReference(object target) { }
    public WeakReference(object target, bool trackResurrection) { }
    protected WeakReference(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public virtual bool IsAlive { get { return default(bool); } }
    public virtual object Target { get { return default(object); } set { } }
    public virtual bool TrackResurrection { get { return default(bool); } }
    ~WeakReference() { }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public sealed partial class WeakReference<T> : System.Runtime.Serialization.ISerializable where T : class {
    public WeakReference(T target) { }
    public WeakReference(T target, bool trackResurrection) { }
    ~WeakReference() { }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public void SetTarget(T target) { }
    public bool TryGetTarget(out T target) { target = default(T); return default(bool); }
  }
}
namespace System.CodeDom.Compiler {
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767), Inherited=false, AllowMultiple=false)]
  public sealed partial class GeneratedCodeAttribute : System.Attribute {
    public GeneratedCodeAttribute(string tool, string version) { }
    public string Tool { get { return default(string); } }
    public string Version { get { return default(string); } }
  }
}
namespace System.Collections {
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct DictionaryEntry {
    public DictionaryEntry(object key, object value) { throw new System.NotImplementedException(); }
    public object Key { get { return default(object); } set { } }
    public object Value { get { return default(object); } set { } }
  }
  public partial interface ICollection : System.Collections.IEnumerable {
    int Count { get; }
    bool IsSynchronized { get; }
    object SyncRoot { get; }
    void CopyTo(System.Array array, int index);
  }
  public partial interface IComparer {
    int Compare(object x, object y);
  }
  public partial interface IDictionary : System.Collections.ICollection, System.Collections.IEnumerable {
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }
    object this[object key] { get; set; }
    System.Collections.ICollection Keys { get; }
    System.Collections.ICollection Values { get; }
    void Add(object key, object value);
    void Clear();
    bool Contains(object key);
    new System.Collections.IDictionaryEnumerator GetEnumerator();
    void Remove(object key);
  }
  public partial interface IDictionaryEnumerator : System.Collections.IEnumerator {
    System.Collections.DictionaryEntry Entry { get; }
    object Key { get; }
    object Value { get; }
  }
  [System.Runtime.InteropServices.GuidAttribute("496B0ABE-CDEE-11d3-88E8-00902754C43A")]
  public partial interface IEnumerable {
    [System.Runtime.InteropServices.DispIdAttribute(-4)]
    System.Collections.IEnumerator GetEnumerator();
  }
  [System.Runtime.InteropServices.GuidAttribute("496B0ABF-CDEE-11d3-88E8-00902754C43A")]
  public partial interface IEnumerator {
    object Current { get; }
    bool MoveNext();
    void Reset();
  }
  public partial interface IEqualityComparer {
    bool Equals(object x, object y);
    int GetHashCode(object obj);
  }
  [System.ObsoleteAttribute("Please use IEqualityComparer instead.")]
  public partial interface IHashCodeProvider {
    int GetHashCode(object obj);
  }
  public partial interface IList : System.Collections.ICollection, System.Collections.IEnumerable {
    bool IsFixedSize { get; }
    bool IsReadOnly { get; }
    object this[int index] { get; set; }
    int Add(object value);
    void Clear();
    bool Contains(object value);
    int IndexOf(object value);
    void Insert(int index, object value);
    void Remove(object value);
    void RemoveAt(int index);
  }
  public partial interface IStructuralComparable {
    int CompareTo(object other, System.Collections.IComparer comparer);
  }
  public partial interface IStructuralEquatable {
    bool Equals(object other, System.Collections.IEqualityComparer comparer);
    int GetHashCode(System.Collections.IEqualityComparer comparer);
  }
}
namespace System.Collections.Generic {
  public abstract partial class Comparer<T> : System.Collections.Generic.IComparer<T>, System.Collections.IComparer {
    protected Comparer() { }
    public static System.Collections.Generic.Comparer<T> Default { get { return default(System.Collections.Generic.Comparer<T>); } }
    public abstract int Compare(T x, T y);
    public static System.Collections.Generic.Comparer<T> Create(System.Comparison<T> comparison) { return default(System.Collections.Generic.Comparer<T>); }
    int System.Collections.IComparer.Compare(object x, object y) { return default(int); }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
  public partial class Dictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
    public Dictionary() { }
    public Dictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { }
    public Dictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
    public Dictionary(System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
    public Dictionary(int capacity) { }
    public Dictionary(int capacity, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
    protected Dictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public System.Collections.Generic.IEqualityComparer<TKey> Comparer { get { return default(System.Collections.Generic.IEqualityComparer<TKey>); } }
    public int Count { get { return default(int); } }
    public TValue this[TKey key] { get { return default(TValue); } set { } }
    public System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection Keys { get { return default(System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection); } }
    bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.IsReadOnly { get { return default(bool); } }
    System.Collections.Generic.ICollection<TKey> System.Collections.Generic.IDictionary<TKey,TValue>.Keys { get { return default(System.Collections.Generic.ICollection<TKey>); } }
    System.Collections.Generic.ICollection<TValue> System.Collections.Generic.IDictionary<TKey,TValue>.Values { get { return default(System.Collections.Generic.ICollection<TValue>); } }
    System.Collections.Generic.IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey,TValue>.Keys { get { return default(System.Collections.Generic.IEnumerable<TKey>); } }
    System.Collections.Generic.IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey,TValue>.Values { get { return default(System.Collections.Generic.IEnumerable<TValue>); } }
    bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
    object System.Collections.ICollection.SyncRoot { get { return default(object); } }
    bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
    bool System.Collections.IDictionary.IsReadOnly { get { return default(bool); } }
    object System.Collections.IDictionary.this[object key] { get { return default(object); } set { } }
    System.Collections.ICollection System.Collections.IDictionary.Keys { get { return default(System.Collections.ICollection); } }
    System.Collections.ICollection System.Collections.IDictionary.Values { get { return default(System.Collections.ICollection); } }
    public System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection Values { get { return default(System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection); } }
    public void Add(TKey key, TValue value) { }
    public void Clear() { }
    public bool ContainsKey(TKey key) { return default(bool); }
    public bool ContainsValue(TValue value) { return default(bool); }
    public System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator); }
    [System.Security.SecurityCriticalAttribute]
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public virtual void OnDeserialization(object sender) { }
    public bool Remove(TKey key) { return default(bool); }
    void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Add(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { }
    bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
    void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index) { }
    bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
    System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey,TValue>>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
    void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
    void System.Collections.IDictionary.Add(object key, object value) { }
    bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
    System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
    void System.Collections.IDictionary.Remove(object key) { }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator, System.Collections.IEnumerator, System.IDisposable {
      public System.Collections.Generic.KeyValuePair<TKey, TValue> Current { get { return default(System.Collections.Generic.KeyValuePair<TKey, TValue>); } }
      System.Collections.DictionaryEntry System.Collections.IDictionaryEnumerator.Entry { get { return default(System.Collections.DictionaryEntry); } }
      object System.Collections.IDictionaryEnumerator.Key { get { return default(object); } }
      object System.Collections.IDictionaryEnumerator.Value { get { return default(object); } }
      object System.Collections.IEnumerator.Current { get { return default(object); } }
      public void Dispose() { }
      public bool MoveNext() { return default(bool); }
      void System.Collections.IEnumerator.Reset() { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
    public sealed partial class KeyCollection : System.Collections.Generic.ICollection<TKey>, System.Collections.Generic.IEnumerable<TKey>, System.Collections.ICollection, System.Collections.IEnumerable {
      public KeyCollection(System.Collections.Generic.Dictionary<TKey, TValue> dictionary) { }
      public int Count { get { return default(int); } }
      bool System.Collections.Generic.ICollection<TKey>.IsReadOnly { get { return default(bool); } }
      bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
      object System.Collections.ICollection.SyncRoot { get { return default(object); } }
      public void CopyTo(TKey[] array, int index) { }
      public System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator); }
      void System.Collections.Generic.ICollection<TKey>.Add(TKey item) { }
      void System.Collections.Generic.ICollection<TKey>.Clear() { }
      bool System.Collections.Generic.ICollection<TKey>.Contains(TKey item) { return default(bool); }
      bool System.Collections.Generic.ICollection<TKey>.Remove(TKey item) { return default(bool); }
      System.Collections.Generic.IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TKey>); }
      void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
      [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
      public partial struct Enumerator : System.Collections.Generic.IEnumerator<TKey>, System.Collections.IEnumerator, System.IDisposable {
        public TKey Current { get { return default(TKey); } }
        object System.Collections.IEnumerator.Current { get { return default(object); } }
        public void Dispose() { }
        public bool MoveNext() { return default(bool); }
        void System.Collections.IEnumerator.Reset() { }
      }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
    public sealed partial class ValueCollection : System.Collections.Generic.ICollection<TValue>, System.Collections.Generic.IEnumerable<TValue>, System.Collections.ICollection, System.Collections.IEnumerable {
      public ValueCollection(System.Collections.Generic.Dictionary<TKey, TValue> dictionary) { }
      public int Count { get { return default(int); } }
      bool System.Collections.Generic.ICollection<TValue>.IsReadOnly { get { return default(bool); } }
      bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
      object System.Collections.ICollection.SyncRoot { get { return default(object); } }
      public void CopyTo(TValue[] array, int index) { }
      public System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator); }
      void System.Collections.Generic.ICollection<TValue>.Add(TValue item) { }
      void System.Collections.Generic.ICollection<TValue>.Clear() { }
      bool System.Collections.Generic.ICollection<TValue>.Contains(TValue item) { return default(bool); }
      bool System.Collections.Generic.ICollection<TValue>.Remove(TValue item) { return default(bool); }
      System.Collections.Generic.IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TValue>); }
      void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
      [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
      public partial struct Enumerator : System.Collections.Generic.IEnumerator<TValue>, System.Collections.IEnumerator, System.IDisposable {
        public TValue Current { get { return default(TValue); } }
        object System.Collections.IEnumerator.Current { get { return default(object); } }
        public void Dispose() { }
        public bool MoveNext() { return default(bool); }
        void System.Collections.IEnumerator.Reset() { }
      }
    }
  }
  public abstract partial class EqualityComparer<T> : System.Collections.Generic.IEqualityComparer<T>, System.Collections.IEqualityComparer {
    protected EqualityComparer() { }
    public static System.Collections.Generic.EqualityComparer<T> Default { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Collections.Generic.EqualityComparer<T>); } }
    public abstract bool Equals(T x, T y);
    public abstract int GetHashCode(T obj);
    bool System.Collections.IEqualityComparer.Equals(object x, object y) { return default(bool); }
    int System.Collections.IEqualityComparer.GetHashCode(object obj) { return default(int); }
  }
  public partial interface ICollection<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable {
    int Count { get; }
    bool IsReadOnly { get; }
    void Add(T item);
    void Clear();
    bool Contains(T item);
    void CopyTo(T[] array, int arrayIndex);
    bool Remove(T item);
  }
  public partial interface IComparer<in T> {
    int Compare(T x, T y);
  }
  public partial interface IDictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IEnumerable {
    TValue this[TKey key] { get; set; }
    System.Collections.Generic.ICollection<TKey> Keys { get; }
    System.Collections.Generic.ICollection<TValue> Values { get; }
    void Add(TKey key, TValue value);
    bool ContainsKey(TKey key);
    bool Remove(TKey key);
    bool TryGetValue(TKey key, out TValue value);
  }
  public partial interface IEnumerable<out T> : System.Collections.IEnumerable {
    new System.Collections.Generic.IEnumerator<T> GetEnumerator();
  }
  public partial interface IEnumerator<out T> : System.Collections.IEnumerator, System.IDisposable {
    new T Current { get; }
  }
  public partial interface IEqualityComparer<in T> {
    bool Equals(T x, T y);
    int GetHashCode(T obj);
  }
  public partial interface IList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable {
    T this[int index] { get; set; }
    int IndexOf(T item);
    void Insert(int index, T item);
    void RemoveAt(int index);
  }
  public partial interface IReadOnlyCollection<out T> : System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable {
    int Count { get; }
  }
  public partial interface IReadOnlyDictionary<TKey, TValue> : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IEnumerable {
    TValue this[TKey key] { get; }
    System.Collections.Generic.IEnumerable<TKey> Keys { get; }
    System.Collections.Generic.IEnumerable<TValue> Values { get; }
    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, out TValue value);
  }
  public partial interface IReadOnlyList<out T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.IEnumerable {
    T this[int index] { get; }
  }
  public partial interface ISet<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.IEnumerable {
    new bool Add(T item);
    void ExceptWith(System.Collections.Generic.IEnumerable<T> other);
    void IntersectWith(System.Collections.Generic.IEnumerable<T> other);
    bool IsProperSubsetOf(System.Collections.Generic.IEnumerable<T> other);
    bool IsProperSupersetOf(System.Collections.Generic.IEnumerable<T> other);
    bool IsSubsetOf(System.Collections.Generic.IEnumerable<T> other);
    bool IsSupersetOf(System.Collections.Generic.IEnumerable<T> other);
    bool Overlaps(System.Collections.Generic.IEnumerable<T> other);
    bool SetEquals(System.Collections.Generic.IEnumerable<T> other);
    void SymmetricExceptWith(System.Collections.Generic.IEnumerable<T> other);
    void UnionWith(System.Collections.Generic.IEnumerable<T> other);
  }
  public partial class KeyNotFoundException : System.SystemException, System.Runtime.Serialization.ISerializable {
    public KeyNotFoundException() { }
    protected KeyNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public KeyNotFoundException(string message) { }
    public KeyNotFoundException(string message, System.Exception innerException) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct KeyValuePair<TKey, TValue> {
    public KeyValuePair(TKey key, TValue value) { throw new System.NotImplementedException(); }
    public TKey Key { get { return default(TKey); } }
    public TValue Value { get { return default(TValue); } }
    public override string ToString() { return default(string); }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
  public partial class List<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public List() { }
    public List(System.Collections.Generic.IEnumerable<T> collection) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public List(int capacity) { }
    public int Capacity { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } set { } }
    public int Count { get { return default(int); } }
    public T this[int index] { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(T); } [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]set { } }
    bool System.Collections.Generic.ICollection<T>.IsReadOnly { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
    object System.Collections.ICollection.SyncRoot { get { return default(object); } }
    bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
    bool System.Collections.IList.IsReadOnly { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    object System.Collections.IList.this[int index] { get { return default(object); } set { } }
    public void Add(T item) { }
    public void AddRange(System.Collections.Generic.IEnumerable<T> collection) { }
    public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly() { return default(System.Collections.ObjectModel.ReadOnlyCollection<T>); }
    public int BinarySearch(T item) { return default(int); }
    public int BinarySearch(T item, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
    public int BinarySearch(int index, int count, T item, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
    public void Clear() { }
    public bool Contains(T item) { return default(bool); }
    public System.Collections.Generic.List<TOutput> ConvertAll<TOutput>(System.Converter<T, TOutput> converter) { return default(System.Collections.Generic.List<TOutput>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public void CopyTo(T[] array) { }
    public void CopyTo(T[] array, int arrayIndex) { }
    public void CopyTo(int index, T[] array, int arrayIndex, int count) { }
    public bool Exists(System.Predicate<T> match) { return default(bool); }
    public T Find(System.Predicate<T> match) { return default(T); }
    public System.Collections.Generic.List<T> FindAll(System.Predicate<T> match) { return default(System.Collections.Generic.List<T>); }
    public int FindIndex(int startIndex, int count, System.Predicate<T> match) { return default(int); }
    public int FindIndex(int startIndex, System.Predicate<T> match) { return default(int); }
    public int FindIndex(System.Predicate<T> match) { return default(int); }
    public T FindLast(System.Predicate<T> match) { return default(T); }
    public int FindLastIndex(int startIndex, int count, System.Predicate<T> match) { return default(int); }
    public int FindLastIndex(int startIndex, System.Predicate<T> match) { return default(int); }
    public int FindLastIndex(System.Predicate<T> match) { return default(int); }
    public void ForEach(System.Action<T> action) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Collections.Generic.List<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.List<T>.Enumerator); }
    public System.Collections.Generic.List<T> GetRange(int index, int count) { return default(System.Collections.Generic.List<T>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(T item) { return default(int); }
    public int IndexOf(T item, int index) { return default(int); }
    public int IndexOf(T item, int index, int count) { return default(int); }
    public void Insert(int index, T item) { }
    public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection) { }
    public int LastIndexOf(T item) { return default(int); }
    public int LastIndexOf(T item, int index) { return default(int); }
    public int LastIndexOf(T item, int index, int count) { return default(int); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Remove(T item) { return default(bool); }
    public int RemoveAll(System.Predicate<T> match) { return default(int); }
    public void RemoveAt(int index) { }
    public void RemoveRange(int index, int count) { }
    public void Reverse() { }
    public void Reverse(int index, int count) { }
    public void Sort() { }
    public void Sort(System.Collections.Generic.IComparer<T> comparer) { }
    public void Sort(System.Comparison<T> comparison) { }
    public void Sort(int index, int count, System.Collections.Generic.IComparer<T> comparer) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    int System.Collections.IList.Add(object item) { return default(int); }
    bool System.Collections.IList.Contains(object item) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    int System.Collections.IList.IndexOf(object item) { return default(int); }
    void System.Collections.IList.Insert(int index, object item) { }
    void System.Collections.IList.Remove(object item) { }
    public T[] ToArray() { return default(T[]); }
    public void TrimExcess() { }
    public bool TrueForAll(System.Predicate<T> match) { return default(bool); }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable {
      public T Current { get { return default(T); } }
      object System.Collections.IEnumerator.Current { get { return default(object); } }
      [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
      public void Dispose() { }
      public bool MoveNext() { return default(bool); }
      void System.Collections.IEnumerator.Reset() { }
    }
  }
}
namespace System.Collections.ObjectModel {
  [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
  public partial class Collection<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public Collection() { }
    public Collection(System.Collections.Generic.IList<T> list) { }
    public int Count { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public T this[int index] { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(T); } set { } }
    protected System.Collections.Generic.IList<T> Items { get { return default(System.Collections.Generic.IList<T>); } }
    bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
    bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
    object System.Collections.ICollection.SyncRoot { get { return default(object); } }
    bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
    bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
    object System.Collections.IList.this[int index] { get { return default(object); } set { } }
    public void Add(T item) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public void Clear() { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected virtual void ClearItems() { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Contains(T item) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public void CopyTo(T[] array, int index) { }
    public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public int IndexOf(T item) { return default(int); }
    public void Insert(int index, T item) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected virtual void InsertItem(int index, T item) { }
    public bool Remove(T item) { return default(bool); }
    public void RemoveAt(int index) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected virtual void RemoveItem(int index) { }
    protected virtual void SetItem(int index, T item) { }
    void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    int System.Collections.IList.Add(object value) { return default(int); }
    bool System.Collections.IList.Contains(object value) { return default(bool); }
    int System.Collections.IList.IndexOf(object value) { return default(int); }
    void System.Collections.IList.Insert(int index, object value) { }
    void System.Collections.IList.Remove(object value) { }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}")]
  public partial class ReadOnlyCollection<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList {
    public ReadOnlyCollection(System.Collections.Generic.IList<T> list) { }
    public int Count { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } }
    public T this[int index] { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(T); } }
    protected System.Collections.Generic.IList<T> Items { get { return default(System.Collections.Generic.IList<T>); } }
    bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
    T System.Collections.Generic.IList<T>.this[int index] { get { return default(T); } set { } }
    bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
    object System.Collections.ICollection.SyncRoot { get { return default(object); } }
    bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
    bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
    object System.Collections.IList.this[int index] { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(object); } set { } }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public bool Contains(T value) { return default(bool); }
    public void CopyTo(T[] array, int index) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
    public int IndexOf(T value) { return default(int); }
    void System.Collections.Generic.ICollection<T>.Add(T value) { }
    void System.Collections.Generic.ICollection<T>.Clear() { }
    bool System.Collections.Generic.ICollection<T>.Remove(T value) { return default(bool); }
    void System.Collections.Generic.IList<T>.Insert(int index, T value) { }
    void System.Collections.Generic.IList<T>.RemoveAt(int index) { }
    void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    int System.Collections.IList.Add(object value) { return default(int); }
    void System.Collections.IList.Clear() { }
    bool System.Collections.IList.Contains(object value) { return default(bool); }
    int System.Collections.IList.IndexOf(object value) { return default(int); }
    void System.Collections.IList.Insert(int index, object value) { }
    void System.Collections.IList.Remove(object value) { }
    void System.Collections.IList.RemoveAt(int index) { }
  }
}
namespace System.ComponentModel {
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767))]
  public partial class DefaultValueAttribute : System.Attribute {
    public DefaultValueAttribute(bool value) { }
    public DefaultValueAttribute(byte value) { }
    public DefaultValueAttribute(char value) { }
    public DefaultValueAttribute(double value) { }
    public DefaultValueAttribute(short value) { }
    public DefaultValueAttribute(int value) { }
    public DefaultValueAttribute(long value) { }
    public DefaultValueAttribute(object value) { }
    public DefaultValueAttribute(float value) { }
    public DefaultValueAttribute(string value) { }
    public DefaultValueAttribute(System.Type type, string value) { }
    public virtual object Value { get { return default(object); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    protected void SetValue(object value) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(6140))]
  public sealed partial class EditorBrowsableAttribute : System.Attribute {
    public EditorBrowsableAttribute() { }
    public EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState state) { }
    public System.ComponentModel.EditorBrowsableState State { get { return default(System.ComponentModel.EditorBrowsableState); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
  }
  public enum EditorBrowsableState {
    Advanced = 2,
    Always = 0,
    Never = 1,
  }
  public partial class Win32Exception : System.Runtime.InteropServices.ExternalException, System.Runtime.Serialization.ISerializable {
    public Win32Exception() { }
    public Win32Exception(int error) { }
    public Win32Exception(int error, string message) { }
    protected Win32Exception(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public Win32Exception(string message) { }
    public Win32Exception(string message, System.Exception innerException) { }
    public int NativeErrorCode { get { return default(int); } }
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
}
namespace System.Configuration.Assemblies {
  [System.ObsoleteAttribute]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct AssemblyHash : System.ICloneable {
    [System.ObsoleteAttribute]
    public static readonly System.Configuration.Assemblies.AssemblyHash Empty;
    [System.ObsoleteAttribute]
    public AssemblyHash(byte[] value) { throw new System.NotImplementedException(); }
    [System.ObsoleteAttribute]
    public AssemblyHash(System.Configuration.Assemblies.AssemblyHashAlgorithm algorithm, byte[] value) { throw new System.NotImplementedException(); }
    [System.ObsoleteAttribute]
    public System.Configuration.Assemblies.AssemblyHashAlgorithm Algorithm { get { return default(System.Configuration.Assemblies.AssemblyHashAlgorithm); } set { } }
    [System.ObsoleteAttribute]
    public object Clone() { return default(object); }
    [System.ObsoleteAttribute]
    public byte[] GetValue() { return default(byte[]); }
    [System.ObsoleteAttribute]
    public void SetValue(byte[] value) { }
  }
  public enum AssemblyHashAlgorithm {
    MD5 = 32771,
    None = 0,
    SHA1 = 32772,
    SHA256 = 32780,
    SHA384 = 32781,
    SHA512 = 32782,
  }
  public enum AssemblyVersionCompatibility {
    SameDomain = 3,
    SameMachine = 1,
    SameProcess = 2,
  }
}
namespace System.Diagnostics {
  [System.AttributeUsageAttribute((System.AttributeTargets)(68), AllowMultiple=true)]
  public sealed partial class ConditionalAttribute : System.Attribute {
    public ConditionalAttribute(string conditionString) { }
    public string ConditionString { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(3), AllowMultiple=false)]
  public sealed partial class DebuggableAttribute : System.Attribute {
    public DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled) { }
    public DebuggableAttribute(System.Diagnostics.DebuggableAttribute.DebuggingModes modes) { }
    public System.Diagnostics.DebuggableAttribute.DebuggingModes DebuggingFlags { get { return default(System.Diagnostics.DebuggableAttribute.DebuggingModes); } }
    public bool IsJITOptimizerDisabled { get { return default(bool); } }
    public bool IsJITTrackingEnabled { get { return default(bool); } }
    [System.FlagsAttribute]
    public enum DebuggingModes {
      Default = 1,
      DisableOptimizations = 256,
      EnableEditAndContinue = 4,
      IgnoreSymbolStoreSequencePoints = 2,
      None = 0,
    }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple=false)]
  public sealed partial class DebuggerBrowsableAttribute : System.Attribute {
    public DebuggerBrowsableAttribute(System.Diagnostics.DebuggerBrowsableState state) { }
    public System.Diagnostics.DebuggerBrowsableState State { get { return default(System.Diagnostics.DebuggerBrowsableState); } }
  }
  public enum DebuggerBrowsableState {
    Collapsed = 2,
    Never = 0,
    RootHidden = 3,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(4509), AllowMultiple=true)]
  public sealed partial class DebuggerDisplayAttribute : System.Attribute {
    public DebuggerDisplayAttribute(string value) { }
    public string Name { get { return default(string); } set { } }
    public System.Type Target { get { return default(System.Type); } set { } }
    public string TargetTypeName { get { return default(string); } set { } }
    public string Type { get { return default(string); } set { } }
    public string Value { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(224), Inherited=false)]
  public sealed partial class DebuggerHiddenAttribute : System.Attribute {
    public DebuggerHiddenAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(236), Inherited=false)]
  public sealed partial class DebuggerNonUserCodeAttribute : System.Attribute {
    public DebuggerNonUserCodeAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(108), Inherited=false)]
  public sealed partial class DebuggerStepThroughAttribute : System.Attribute {
    public DebuggerStepThroughAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(13), AllowMultiple=true)]
  public sealed partial class DebuggerTypeProxyAttribute : System.Attribute {
    public DebuggerTypeProxyAttribute(string typeName) { }
    public DebuggerTypeProxyAttribute(System.Type type) { }
    public string ProxyTypeName { get { return default(string); } }
    public System.Type Target { get { return default(System.Type); } set { } }
    public string TargetTypeName { get { return default(string); } set { } }
  }
  public partial class Stopwatch {
    public static readonly long Frequency;
    public static readonly bool IsHighResolution;
    public Stopwatch() { }
    public System.TimeSpan Elapsed { get { return default(System.TimeSpan); } }
    public long ElapsedMilliseconds { get { return default(long); } }
    public long ElapsedTicks { get { return default(long); } }
    public bool IsRunning { get { return default(bool); } }
    public static long GetTimestamp() { return default(long); }
    public void Reset() { }
    public void Restart() { }
    public void Start() { }
    public static System.Diagnostics.Stopwatch StartNew() { return default(System.Diagnostics.Stopwatch); }
    public void Stop() { }
  }
}
namespace System.Diagnostics.CodeAnalysis {
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767), Inherited=false, AllowMultiple=true)]
  [System.Diagnostics.ConditionalAttribute("CODE_ANALYSIS")]
  public sealed partial class SuppressMessageAttribute : System.Attribute {
    public SuppressMessageAttribute(string category, string checkId) { }
    public string Category { get { return default(string); } }
    public string CheckId { get { return default(string); } }
    public string Justification { get { return default(string); } set { } }
    public string MessageId { get { return default(string); } set { } }
    public string Scope { get { return default(string); } set { } }
    public string Target { get { return default(string); } set { } }
  }
}
namespace System.Globalization {
  public abstract partial class Calendar : System.ICloneable {
    public const int CurrentEra = 0;
    protected Calendar() { }
    public virtual System.Globalization.CalendarAlgorithmType AlgorithmType { get { return default(System.Globalization.CalendarAlgorithmType); } }
    protected virtual int DaysInYearBeforeMinSupportedYear { get { return default(int); } }
    public abstract int[] Eras { get; }
    public bool IsReadOnly { get { return default(bool); } }
    public virtual System.DateTime MaxSupportedDateTime { get { return default(System.DateTime); } }
    public virtual System.DateTime MinSupportedDateTime { get { return default(System.DateTime); } }
    public virtual int TwoDigitYearMax { get { return default(int); } set { } }
    public virtual System.DateTime AddDays(System.DateTime time, int days) { return default(System.DateTime); }
    public virtual System.DateTime AddHours(System.DateTime time, int hours) { return default(System.DateTime); }
    public virtual System.DateTime AddMilliseconds(System.DateTime time, double milliseconds) { return default(System.DateTime); }
    public virtual System.DateTime AddMinutes(System.DateTime time, int minutes) { return default(System.DateTime); }
    public abstract System.DateTime AddMonths(System.DateTime time, int months);
    public virtual System.DateTime AddSeconds(System.DateTime time, int seconds) { return default(System.DateTime); }
    public virtual System.DateTime AddWeeks(System.DateTime time, int weeks) { return default(System.DateTime); }
    public abstract System.DateTime AddYears(System.DateTime time, int years);
    public virtual object Clone() { return default(object); }
    public abstract int GetDayOfMonth(System.DateTime time);
    public abstract System.DayOfWeek GetDayOfWeek(System.DateTime time);
    public abstract int GetDayOfYear(System.DateTime time);
    public virtual int GetDaysInMonth(int year, int month) { return default(int); }
    public abstract int GetDaysInMonth(int year, int month, int era);
    public virtual int GetDaysInYear(int year) { return default(int); }
    public abstract int GetDaysInYear(int year, int era);
    public abstract int GetEra(System.DateTime time);
    public virtual int GetHour(System.DateTime time) { return default(int); }
    public virtual int GetLeapMonth(int year) { return default(int); }
    public virtual int GetLeapMonth(int year, int era) { return default(int); }
    public virtual double GetMilliseconds(System.DateTime time) { return default(double); }
    public virtual int GetMinute(System.DateTime time) { return default(int); }
    public abstract int GetMonth(System.DateTime time);
    public virtual int GetMonthsInYear(int year) { return default(int); }
    public abstract int GetMonthsInYear(int year, int era);
    public virtual int GetSecond(System.DateTime time) { return default(int); }
    public virtual int GetWeekOfYear(System.DateTime time, System.Globalization.CalendarWeekRule rule, System.DayOfWeek firstDayOfWeek) { return default(int); }
    public abstract int GetYear(System.DateTime time);
    public virtual bool IsLeapDay(int year, int month, int day) { return default(bool); }
    public abstract bool IsLeapDay(int year, int month, int day, int era);
    public virtual bool IsLeapMonth(int year, int month) { return default(bool); }
    public abstract bool IsLeapMonth(int year, int month, int era);
    public virtual bool IsLeapYear(int year) { return default(bool); }
    public abstract bool IsLeapYear(int year, int era);
    public static System.Globalization.Calendar ReadOnly(System.Globalization.Calendar calendar) { return default(System.Globalization.Calendar); }
    public virtual System.DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond) { return default(System.DateTime); }
    public abstract System.DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era);
    public virtual int ToFourDigitYear(int year) { return default(int); }
  }
  public enum CalendarAlgorithmType {
    LunarCalendar = 2,
    LunisolarCalendar = 3,
    SolarCalendar = 1,
    Unknown = 0,
  }
  public enum CalendarWeekRule {
    FirstDay = 0,
    FirstFourDayWeek = 2,
    FirstFullWeek = 1,
  }
  public partial class CompareInfo : System.Runtime.Serialization.IDeserializationCallback {
    internal CompareInfo() { }
    public int LCID { get { return default(int); } }
    public virtual string Name { get { return default(string); } }
    public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int Compare(string string1, int offset1, string string2, int offset2) { return default(int); }
    public virtual int Compare(string string1, int offset1, string string2, int offset2, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int Compare(string string1, string string2) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int Compare(string string1, string string2, System.Globalization.CompareOptions options) { return default(int); }
    public override bool Equals(object value) { return default(bool); }
    public static System.Globalization.CompareInfo GetCompareInfo(int culture) { return default(System.Globalization.CompareInfo); }
    public static System.Globalization.CompareInfo GetCompareInfo(int culture, System.Reflection.Assembly assembly) { return default(System.Globalization.CompareInfo); }
    public static System.Globalization.CompareInfo GetCompareInfo(string name) { return default(System.Globalization.CompareInfo); }
    public static System.Globalization.CompareInfo GetCompareInfo(string name, System.Reflection.Assembly assembly) { return default(System.Globalization.CompareInfo); }
    public override int GetHashCode() { return default(int); }
    public virtual int GetHashCode(string source, System.Globalization.CompareOptions options) { return default(int); }
    public virtual System.Globalization.SortKey GetSortKey(string source) { return default(System.Globalization.SortKey); }
    public virtual System.Globalization.SortKey GetSortKey(string source, System.Globalization.CompareOptions options) { return default(System.Globalization.SortKey); }
    public virtual int IndexOf(string source, char value) { return default(int); }
    public virtual int IndexOf(string source, char value, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int IndexOf(string source, char value, int startIndex) { return default(int); }
    public virtual int IndexOf(string source, char value, int startIndex, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int IndexOf(string source, char value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int IndexOf(string source, char value, int startIndex, int count, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int IndexOf(string source, string value) { return default(int); }
    public virtual int IndexOf(string source, string value, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int IndexOf(string source, string value, int startIndex) { return default(int); }
    public virtual int IndexOf(string source, string value, int startIndex, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int IndexOf(string source, string value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int IndexOf(string source, string value, int startIndex, int count, System.Globalization.CompareOptions options) { return default(int); }
    public virtual bool IsPrefix(string source, string prefix) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual bool IsPrefix(string source, string prefix, System.Globalization.CompareOptions options) { return default(bool); }
    public static bool IsSortable(char ch) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static bool IsSortable(string text) { return default(bool); }
    public virtual bool IsSuffix(string source, string suffix) { return default(bool); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual bool IsSuffix(string source, string suffix, System.Globalization.CompareOptions options) { return default(bool); }
    public virtual int LastIndexOf(string source, char value) { return default(int); }
    public virtual int LastIndexOf(string source, char value, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int LastIndexOf(string source, char value, int startIndex) { return default(int); }
    public virtual int LastIndexOf(string source, char value, int startIndex, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int LastIndexOf(string source, char value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int LastIndexOf(string source, char value, int startIndex, int count, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int LastIndexOf(string source, string value) { return default(int); }
    public virtual int LastIndexOf(string source, string value, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int LastIndexOf(string source, string value, int startIndex) { return default(int); }
    public virtual int LastIndexOf(string source, string value, int startIndex, System.Globalization.CompareOptions options) { return default(int); }
    public virtual int LastIndexOf(string source, string value, int startIndex, int count) { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual int LastIndexOf(string source, string value, int startIndex, int count, System.Globalization.CompareOptions options) { return default(int); }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    public override string ToString() { return default(string); }
  }
  [System.FlagsAttribute]
  public enum CompareOptions {
    IgnoreCase = 1,
    IgnoreKanaType = 8,
    IgnoreNonSpace = 2,
    IgnoreSymbols = 4,
    IgnoreWidth = 16,
    None = 0,
    Ordinal = 1073741824,
    OrdinalIgnoreCase = 268435456,
    StringSort = 536870912,
  }
  public partial class CultureInfo : System.ICloneable, System.IFormatProvider {
    public CultureInfo(int culture) { }
    public CultureInfo(int culture, bool useUserOverride) { }
    public CultureInfo(string name) { }
    public CultureInfo(string name, bool useUserOverride) { }
    public virtual System.Globalization.Calendar Calendar { get { return default(System.Globalization.Calendar); } }
    public virtual System.Globalization.CompareInfo CompareInfo { get { return default(System.Globalization.CompareInfo); } }
    public static System.Globalization.CultureInfo CurrentCulture { get { return default(System.Globalization.CultureInfo); } }
    public static System.Globalization.CultureInfo CurrentUICulture { get { return default(System.Globalization.CultureInfo); } }
    public virtual System.Globalization.DateTimeFormatInfo DateTimeFormat { get { return default(System.Globalization.DateTimeFormatInfo); } set { } }
    public static System.Globalization.CultureInfo DefaultThreadCurrentCulture { get { return default(System.Globalization.CultureInfo); } set { } }
    public static System.Globalization.CultureInfo DefaultThreadCurrentUICulture { get { return default(System.Globalization.CultureInfo); } set { } }
    public virtual string DisplayName { get { return default(string); } }
    public virtual string EnglishName { get { return default(string); } }
    public static System.Globalization.CultureInfo InstalledUICulture { get { return default(System.Globalization.CultureInfo); } }
    public static System.Globalization.CultureInfo InvariantCulture { get { return default(System.Globalization.CultureInfo); } }
    public virtual bool IsNeutralCulture { get { return default(bool); } }
    public bool IsReadOnly { get { return default(bool); } }
    public virtual int LCID { get { return default(int); } }
    public virtual string Name { get { return default(string); } }
    public virtual string NativeName { get { return default(string); } }
    public virtual System.Globalization.NumberFormatInfo NumberFormat { get { return default(System.Globalization.NumberFormatInfo); } set { } }
    public virtual System.Globalization.Calendar[] OptionalCalendars { get { return default(System.Globalization.Calendar[]); } }
    public virtual System.Globalization.CultureInfo Parent { get { return default(System.Globalization.CultureInfo); } }
    public virtual System.Globalization.TextInfo TextInfo { get { return default(System.Globalization.TextInfo); } }
    public virtual string ThreeLetterISOLanguageName { get { return default(string); } }
    public virtual string ThreeLetterWindowsLanguageName { get { return default(string); } }
    public virtual string TwoLetterISOLanguageName { get { return default(string); } }
    public bool UseUserOverride { get { return default(bool); } }
    public void ClearCachedData() { }
    public virtual object Clone() { return default(object); }
    public static System.Globalization.CultureInfo CreateSpecificCulture(string name) { return default(System.Globalization.CultureInfo); }
    public override bool Equals(object value) { return default(bool); }
    public static System.Globalization.CultureInfo GetCultureInfo(int culture) { return default(System.Globalization.CultureInfo); }
    public static System.Globalization.CultureInfo GetCultureInfo(string name) { return default(System.Globalization.CultureInfo); }
    public static System.Globalization.CultureInfo GetCultureInfo(string name, string altName) { return default(System.Globalization.CultureInfo); }
    public static System.Globalization.CultureInfo GetCultureInfoByIetfLanguageTag(string name) { return default(System.Globalization.CultureInfo); }
    public static System.Globalization.CultureInfo[] GetCultures(System.Globalization.CultureTypes types) { return default(System.Globalization.CultureInfo[]); }
    public virtual object GetFormat(System.Type formatType) { return default(object); }
    public override int GetHashCode() { return default(int); }
    public static System.Globalization.CultureInfo ReadOnly(System.Globalization.CultureInfo ci) { return default(System.Globalization.CultureInfo); }
    public override string ToString() { return default(string); }
  }
  public partial class CultureNotFoundException : System.ArgumentException, System.Runtime.Serialization.ISerializable {
    public CultureNotFoundException() { }
    protected CultureNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public CultureNotFoundException(string message) { }
    public CultureNotFoundException(string message, System.Exception innerException) { }
    public CultureNotFoundException(string message, int invalidCultureId, System.Exception innerException) { }
    public CultureNotFoundException(string paramName, int invalidCultureId, string message) { }
    public CultureNotFoundException(string paramName, string message) { }
    public CultureNotFoundException(string message, string invalidCultureName, System.Exception innerException) { }
    public CultureNotFoundException(string paramName, string invalidCultureName, string message) { }
    public virtual System.Nullable<int> InvalidCultureId { get { return default(System.Nullable<int>); } }
    public virtual string InvalidCultureName { get { return default(string); } }
    public override string Message { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  [System.FlagsAttribute]
  public enum CultureTypes {
    AllCultures = 7,
    [System.ObsoleteAttribute("This value has been deprecated.  Please use other values in CultureTypes.")]
    FrameworkCultures = 64,
    InstalledWin32Cultures = 4,
    NeutralCultures = 1,
    ReplacementCultures = 16,
    SpecificCultures = 2,
    UserCustomCulture = 8,
    [System.ObsoleteAttribute("This value has been deprecated.  Please use other values in CultureTypes.")]
    WindowsOnlyCultures = 32,
  }
  public sealed partial class DateTimeFormatInfo : System.ICloneable, System.IFormatProvider {
    public DateTimeFormatInfo() { }
    public string[] AbbreviatedDayNames { get { return default(string[]); } set { } }
    public string[] AbbreviatedMonthGenitiveNames { get { return default(string[]); } set { } }
    public string[] AbbreviatedMonthNames { get { return default(string[]); } set { } }
    public string AMDesignator { get { return default(string); } set { } }
    public System.Globalization.Calendar Calendar { get { return default(System.Globalization.Calendar); } set { } }
    public System.Globalization.CalendarWeekRule CalendarWeekRule { get { return default(System.Globalization.CalendarWeekRule); } set { } }
    public static System.Globalization.DateTimeFormatInfo CurrentInfo { get { return default(System.Globalization.DateTimeFormatInfo); } }
    public string DateSeparator { get { return default(string); } set { } }
    public string[] DayNames { get { return default(string[]); } set { } }
    public System.DayOfWeek FirstDayOfWeek { get { return default(System.DayOfWeek); } set { } }
    public string FullDateTimePattern { get { return default(string); } set { } }
    public static System.Globalization.DateTimeFormatInfo InvariantInfo { get { return default(System.Globalization.DateTimeFormatInfo); } }
    public bool IsReadOnly { get { return default(bool); } }
    public string LongDatePattern { get { return default(string); } set { } }
    public string LongTimePattern { get { return default(string); } set { } }
    public string MonthDayPattern { get { return default(string); } set { } }
    public string[] MonthGenitiveNames { get { return default(string[]); } set { } }
    public string[] MonthNames { get { return default(string[]); } set { } }
    public string NativeCalendarName { get { return default(string); } }
    public string PMDesignator { get { return default(string); } set { } }
    public string RFC1123Pattern { get { return default(string); } }
    public string ShortDatePattern { get { return default(string); } set { } }
    public string[] ShortestDayNames { get { return default(string[]); } set { } }
    public string ShortTimePattern { get { return default(string); } set { } }
    public string SortableDateTimePattern { get { return default(string); } }
    public string TimeSeparator { get { return default(string); } set { } }
    public string UniversalSortableDateTimePattern { get { return default(string); } }
    public string YearMonthPattern { get { return default(string); } set { } }
    public object Clone() { return default(object); }
    public string GetAbbreviatedDayName(System.DayOfWeek dayofweek) { return default(string); }
    public string GetAbbreviatedEraName(int era) { return default(string); }
    public string GetAbbreviatedMonthName(int month) { return default(string); }
    public string[] GetAllDateTimePatterns() { return default(string[]); }
    public string[] GetAllDateTimePatterns(char format) { return default(string[]); }
    public string GetDayName(System.DayOfWeek dayofweek) { return default(string); }
    public int GetEra(string eraName) { return default(int); }
    public string GetEraName(int era) { return default(string); }
    public object GetFormat(System.Type formatType) { return default(object); }
    public static System.Globalization.DateTimeFormatInfo GetInstance(System.IFormatProvider provider) { return default(System.Globalization.DateTimeFormatInfo); }
    public string GetMonthName(int month) { return default(string); }
    public string GetShortestDayName(System.DayOfWeek dayOfWeek) { return default(string); }
    public static System.Globalization.DateTimeFormatInfo ReadOnly(System.Globalization.DateTimeFormatInfo dtfi) { return default(System.Globalization.DateTimeFormatInfo); }
    public void SetAllDateTimePatterns(string[] patterns, char format) { }
  }
  [System.FlagsAttribute]
  public enum DateTimeStyles {
    AdjustToUniversal = 16,
    AllowInnerWhite = 4,
    AllowLeadingWhite = 1,
    AllowTrailingWhite = 2,
    AllowWhiteSpaces = 7,
    AssumeLocal = 32,
    AssumeUniversal = 64,
    NoCurrentDateDefault = 8,
    None = 0,
    RoundtripKind = 128,
  }
  public enum DigitShapes {
    Context = 0,
    NativeNational = 2,
    None = 1,
  }
  public sealed partial class NumberFormatInfo : System.ICloneable, System.IFormatProvider {
    public NumberFormatInfo() { }
    public int CurrencyDecimalDigits { get { return default(int); } set { } }
    public string CurrencyDecimalSeparator { get { return default(string); } set { } }
    public string CurrencyGroupSeparator { get { return default(string); } set { } }
    public int[] CurrencyGroupSizes { get { return default(int[]); } set { } }
    public int CurrencyNegativePattern { get { return default(int); } set { } }
    public int CurrencyPositivePattern { get { return default(int); } set { } }
    public string CurrencySymbol { get { return default(string); } set { } }
    public static System.Globalization.NumberFormatInfo CurrentInfo { get { return default(System.Globalization.NumberFormatInfo); } }
    public System.Globalization.DigitShapes DigitSubstitution { get { return default(System.Globalization.DigitShapes); } set { } }
    public static System.Globalization.NumberFormatInfo InvariantInfo { get { return default(System.Globalization.NumberFormatInfo); } }
    public bool IsReadOnly { get { return default(bool); } }
    public string NaNSymbol { get { return default(string); } set { } }
    public string[] NativeDigits { get { return default(string[]); } set { } }
    public string NegativeInfinitySymbol { get { return default(string); } set { } }
    public string NegativeSign { get { return default(string); } set { } }
    public int NumberDecimalDigits { get { return default(int); } set { } }
    public string NumberDecimalSeparator { get { return default(string); } set { } }
    public string NumberGroupSeparator { get { return default(string); } set { } }
    public int[] NumberGroupSizes { get { return default(int[]); } set { } }
    public int NumberNegativePattern { get { return default(int); } set { } }
    public int PercentDecimalDigits { get { return default(int); } set { } }
    public string PercentDecimalSeparator { get { return default(string); } set { } }
    public string PercentGroupSeparator { get { return default(string); } set { } }
    public int[] PercentGroupSizes { get { return default(int[]); } set { } }
    public int PercentNegativePattern { get { return default(int); } set { } }
    public int PercentPositivePattern { get { return default(int); } set { } }
    public string PercentSymbol { get { return default(string); } set { } }
    public string PerMilleSymbol { get { return default(string); } set { } }
    public string PositiveInfinitySymbol { get { return default(string); } set { } }
    public string PositiveSign { get { return default(string); } set { } }
    public object Clone() { return default(object); }
    public object GetFormat(System.Type formatType) { return default(object); }
    public static System.Globalization.NumberFormatInfo GetInstance(System.IFormatProvider formatProvider) { return default(System.Globalization.NumberFormatInfo); }
    public static System.Globalization.NumberFormatInfo ReadOnly(System.Globalization.NumberFormatInfo nfi) { return default(System.Globalization.NumberFormatInfo); }
  }
  [System.FlagsAttribute]
  public enum NumberStyles {
    AllowCurrencySymbol = 256,
    AllowDecimalPoint = 32,
    AllowExponent = 128,
    AllowHexSpecifier = 512,
    AllowLeadingSign = 4,
    AllowLeadingWhite = 1,
    AllowParentheses = 16,
    AllowThousands = 64,
    AllowTrailingSign = 8,
    AllowTrailingWhite = 2,
    Any = 511,
    Currency = 383,
    Float = 167,
    HexNumber = 515,
    Integer = 7,
    None = 0,
    Number = 111,
  }
  public partial class SortKey {
    internal SortKey() { }
    public virtual byte[] KeyData { get { return default(byte[]); } }
    public virtual string OriginalString { get { return default(string); } }
    public static int Compare(System.Globalization.SortKey sortkey1, System.Globalization.SortKey sortkey2) { return default(int); }
    public override bool Equals(object value) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public override string ToString() { return default(string); }
  }
  public partial class TextInfo : System.ICloneable, System.Runtime.Serialization.IDeserializationCallback {
    internal TextInfo() { }
    public virtual int ANSICodePage { get { return default(int); } }
    public string CultureName { get { return default(string); } }
    public virtual int EBCDICCodePage { get { return default(int); } }
    public bool IsReadOnly { get { return default(bool); } }
    public bool IsRightToLeft { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public int LCID { get { return default(int); } }
    public virtual string ListSeparator { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } set { } }
    public virtual int MacCodePage { get { return default(int); } }
    public virtual int OEMCodePage { get { return default(int); } }
    public virtual object Clone() { return default(object); }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static System.Globalization.TextInfo ReadOnly(System.Globalization.TextInfo textInfo) { return default(System.Globalization.TextInfo); }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual char ToLower(char c) { return default(char); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual string ToLower(string str) { return default(string); }
    public override string ToString() { return default(string); }
    public string ToTitleCase(string str) { return default(string); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual char ToUpper(char c) { return default(char); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual string ToUpper(string str) { return default(string); }
  }
  [System.FlagsAttribute]
  public enum TimeSpanStyles {
    AssumeNegative = 1,
    None = 0,
  }
  public enum UnicodeCategory {
    ClosePunctuation = 21,
    ConnectorPunctuation = 18,
    Control = 14,
    CurrencySymbol = 26,
    DashPunctuation = 19,
    DecimalDigitNumber = 8,
    EnclosingMark = 7,
    FinalQuotePunctuation = 23,
    Format = 15,
    InitialQuotePunctuation = 22,
    LetterNumber = 9,
    LineSeparator = 12,
    LowercaseLetter = 1,
    MathSymbol = 25,
    ModifierLetter = 3,
    ModifierSymbol = 27,
    NonSpacingMark = 5,
    OpenPunctuation = 20,
    OtherLetter = 4,
    OtherNotAssigned = 29,
    OtherNumber = 10,
    OtherPunctuation = 24,
    OtherSymbol = 28,
    ParagraphSeparator = 13,
    PrivateUse = 17,
    SpaceSeparator = 11,
    SpacingCombiningMark = 6,
    Surrogate = 16,
    TitlecaseLetter = 2,
    UppercaseLetter = 0,
  }
}
namespace System.IO {
  public partial class DirectoryNotFoundException : System.IO.IOException {
    public DirectoryNotFoundException() { }
    protected DirectoryNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public DirectoryNotFoundException(string message) { }
    public DirectoryNotFoundException(string message, System.Exception innerException) { }
  }
  public partial class EndOfStreamException : System.IO.IOException {
    public EndOfStreamException() { }
    protected EndOfStreamException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public EndOfStreamException(string message) { }
    public EndOfStreamException(string message, System.Exception innerException) { }
  }
  [System.FlagsAttribute]
  public enum FileAccess {
    Read = 1,
    ReadWrite = 3,
    Write = 2,
  }
  public partial class FileLoadException : System.IO.IOException {
    public FileLoadException() { }
    protected FileLoadException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public FileLoadException(string message) { }
    public FileLoadException(string message, System.Exception inner) { }
    public FileLoadException(string message, string fileName) { }
    public FileLoadException(string message, string fileName, System.Exception inner) { }
    public string FileName { get { return default(string); } }
    public string FusionLog { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
    public override string Message { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  public enum FileMode {
    Append = 6,
    Create = 2,
    CreateNew = 1,
    Open = 3,
    OpenOrCreate = 4,
    Truncate = 5,
  }
  public partial class FileNotFoundException : System.IO.IOException {
    public FileNotFoundException() { }
    protected FileNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public FileNotFoundException(string message) { }
    public FileNotFoundException(string message, System.Exception innerException) { }
    public FileNotFoundException(string message, string fileName) { }
    public FileNotFoundException(string message, string fileName, System.Exception innerException) { }
    public string FileName { get { return default(string); } }
    public string FusionLog { [System.Security.SecuritySafeCriticalAttribute]get { return default(string); } }
    public override string Message { get { return default(string); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  [System.FlagsAttribute]
  public enum FileOptions {
    Asynchronous = 1073741824,
    DeleteOnClose = 67108864,
    Encrypted = 16384,
    None = 0,
    RandomAccess = 268435456,
    SequentialScan = 134217728,
    WriteThrough = -2147483648,
  }
  [System.FlagsAttribute]
  public enum FileShare {
    Delete = 4,
    Inheritable = 16,
    None = 0,
    Read = 1,
    ReadWrite = 3,
    Write = 2,
  }
  public partial class FileStream : System.IO.Stream {
    public FileStream(Microsoft.Win32.SafeHandles.SafeFileHandle handle, System.IO.FileAccess access) { }
    public FileStream(Microsoft.Win32.SafeHandles.SafeFileHandle handle, System.IO.FileAccess access, int bufferSize) { }
    public FileStream(Microsoft.Win32.SafeHandles.SafeFileHandle handle, System.IO.FileAccess access, int bufferSize, bool isAsync) { }
    [System.ObsoleteAttribute("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
    public FileStream(System.IntPtr handle, System.IO.FileAccess access) { }
    [System.ObsoleteAttribute("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
    public FileStream(System.IntPtr handle, System.IO.FileAccess access, bool ownsHandle) { }
    [System.ObsoleteAttribute("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead")]
    public FileStream(System.IntPtr handle, System.IO.FileAccess access, bool ownsHandle, int bufferSize) { }
    [System.ObsoleteAttribute("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead")]
    public FileStream(System.IntPtr handle, System.IO.FileAccess access, bool ownsHandle, int bufferSize, bool isAsync) { }
    public FileStream(string path, System.IO.FileMode mode) { }
    public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access) { }
    public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share) { }
    public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, int bufferSize) { }
    public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, int bufferSize, bool useAsync) { }
    public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, int bufferSize, System.IO.FileOptions options) { }
    public override bool CanRead { get { return default(bool); } }
    public override bool CanSeek { get { return default(bool); } }
    public override bool CanWrite { get { return default(bool); } }
    [System.ObsoleteAttribute("Use SafeFileHandle instead")]
    public virtual System.IntPtr Handle { get { return default(System.IntPtr); } }
    public virtual bool IsAsync { get { return default(bool); } }
    public override long Length { get { return default(long); } }
    public string Name { get { return default(string); } }
    public override long Position { get { return default(long); } set { } }
    public virtual Microsoft.Win32.SafeHandles.SafeFileHandle SafeFileHandle { get { return default(Microsoft.Win32.SafeHandles.SafeFileHandle); } }
    public override System.IAsyncResult BeginRead(byte[] array, int offset, int numBytes, System.AsyncCallback userCallback, object stateObject) { return default(System.IAsyncResult); }
    public override System.IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, System.AsyncCallback userCallback, object stateObject) { return default(System.IAsyncResult); }
    protected override void Dispose(bool disposing) { }
    public override int EndRead(System.IAsyncResult asyncResult) { return default(int); }
    public override void EndWrite(System.IAsyncResult asyncResult) { }
    ~FileStream() { }
    public override void Flush() { }
    public virtual void Flush(bool flushToDisk) { }
    public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public virtual void Lock(long position, long length) { }
    public override int Read(byte[] array, int offset, int count) { return default(int); }
    public override System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<int>); }
    public override int ReadByte() { return default(int); }
    public override long Seek(long offset, System.IO.SeekOrigin origin) { return default(long); }
    public override void SetLength(long value) { }
    public virtual void Unlock(long position, long length) { }
    public override void Write(byte[] array, int offset, int count) { }
    public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public override void WriteByte(byte value) { }
  }
  public partial class IOException : System.SystemException {
    public IOException() { }
    protected IOException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public IOException(string message) { }
    public IOException(string message, System.Exception innerException) { }
    public IOException(string message, int hresult) { }
  }
  public static partial class Path {
    public static readonly char AltDirectorySeparatorChar;
    public static readonly char DirectorySeparatorChar;
    [System.ObsoleteAttribute("see GetInvalidPathChars and GetInvalidFileNameChars methods.")]
    public static readonly char[] InvalidPathChars;
    public static readonly char PathSeparator;
    public static readonly char VolumeSeparatorChar;
    public static string ChangeExtension(string path, string extension) { return default(string); }
    public static string Combine(string path1, string path2) { return default(string); }
    public static string Combine(string path1, string path2, string path3) { return default(string); }
    public static string Combine(string path1, string path2, string path3, string path4) { return default(string); }
    public static string Combine(params string[] paths) { return default(string); }
    public static string GetDirectoryName(string path) { return default(string); }
    public static string GetExtension(string path) { return default(string); }
    public static string GetFileName(string path) { return default(string); }
    public static string GetFileNameWithoutExtension(string path) { return default(string); }
    public static string GetFullPath(string path) { return default(string); }
    public static char[] GetInvalidFileNameChars() { return default(char[]); }
    public static char[] GetInvalidPathChars() { return default(char[]); }
    public static string GetPathRoot(string path) { return default(string); }
    public static string GetRandomFileName() { return default(string); }
    public static string GetTempFileName() { return default(string); }
    public static string GetTempPath() { return default(string); }
    public static bool HasExtension(string path) { return default(bool); }
    public static bool IsPathRooted(string path) { return default(bool); }
  }
  public partial class PathTooLongException : System.IO.IOException {
    public PathTooLongException() { }
    protected PathTooLongException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public PathTooLongException(string message) { }
    public PathTooLongException(string message, System.Exception innerException) { }
  }
  public enum SeekOrigin {
    Begin = 0,
    Current = 1,
    End = 2,
  }
  public abstract partial class Stream : System.IDisposable {
    public static readonly System.IO.Stream Null;
    protected Stream() { }
    public abstract bool CanRead { get; }
    public abstract bool CanSeek { get; }
    public virtual bool CanTimeout { get { return default(bool); } }
    public abstract bool CanWrite { get; }
    public abstract long Length { get; }
    public abstract long Position { get; set; }
    public virtual int ReadTimeout { get { return default(int); } set { } }
    public virtual int WriteTimeout { get { return default(int); } set { } }
    public virtual System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
    public virtual System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
    public virtual void Close() { }
    public void CopyTo(System.IO.Stream destination) { }
    public void CopyTo(System.IO.Stream destination, int bufferSize) { }
    public System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination, int bufferSize) { return default(System.Threading.Tasks.Task); }
    public virtual System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    [System.ObsoleteAttribute("CreateWaitHandle will be removed eventually.  Please use \"new ManualResetEvent(false)\" instead.")]
    protected virtual System.Threading.WaitHandle CreateWaitHandle() { return default(System.Threading.WaitHandle); }
    public void Dispose() { }
    protected virtual void Dispose(bool disposing) { }
    public virtual int EndRead(System.IAsyncResult asyncResult) { return default(int); }
    public virtual void EndWrite(System.IAsyncResult asyncResult) { }
    public abstract void Flush();
    public System.Threading.Tasks.Task FlushAsync() { return default(System.Threading.Tasks.Task); }
    public virtual System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    [System.ObsoleteAttribute("Do not call or override this method.")]
    protected virtual void ObjectInvariant() { }
    public abstract int Read(byte[] buffer, int offset, int count);
    public System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count) { return default(System.Threading.Tasks.Task<int>); }
    public virtual System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<int>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public virtual int ReadByte() { return default(int); }
    public abstract long Seek(long offset, System.IO.SeekOrigin origin);
    public abstract void SetLength(long value);
    public static System.IO.Stream Synchronized(System.IO.Stream stream) { return default(System.IO.Stream); }
    public abstract void Write(byte[] buffer, int offset, int count);
    public System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count) { return default(System.Threading.Tasks.Task); }
    public virtual System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public virtual void WriteByte(byte value) { }
  }
}
namespace System.Reflection {
  public sealed partial class AmbiguousMatchException : System.SystemException {
    public AmbiguousMatchException() { }
    public AmbiguousMatchException(string message) { }
    public AmbiguousMatchException(string message, System.Exception inner) { }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public partial class Assembly : System.Reflection.ICustomAttributeProvider {
    protected Assembly() { }
    public virtual string CodeBase { get { return default(string); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DefinedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo>); } }
    public virtual System.Reflection.MethodInfo EntryPoint { get { return default(System.Reflection.MethodInfo); } }
    public virtual string EscapedCodeBase { get { return default(string); } }
    public virtual System.Security.Policy.Evidence Evidence { get { return default(System.Security.Policy.Evidence); } }
    public virtual System.Collections.Generic.IEnumerable<System.Type> ExportedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
    public virtual string FullName { get { return default(string); } }
    public virtual bool GlobalAssemblyCache { get { return default(bool); } }
    public virtual long HostContext { get { return default(long); } }
    public virtual string ImageRuntimeVersion { get { return default(string); } }
    public virtual bool IsDynamic { get { return default(bool); } }
    public bool IsFullyTrusted { get { return default(bool); } }
    public virtual string Location { get { return default(string); } }
    public virtual System.Reflection.Module ManifestModule { get { return default(System.Reflection.Module); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.Module> Modules { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.Module>); } }
    public virtual bool ReflectionOnly { get { return default(bool); } }
    public event System.Reflection.ModuleResolveEventHandler ModuleResolve { add { } remove { } }
    public object CreateInstance(string typeName) { return default(object); }
    public object CreateInstance(string typeName, bool ignoreCase) { return default(object); }
    public virtual object CreateInstance(string typeName, bool ignoreCase, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes) { return default(object); }
    public static string CreateQualifiedName(string assemblyName, string typeName) { return default(string); }
    public override bool Equals(object o) { return default(bool); }
    public static System.Reflection.Assembly GetAssembly(System.Type type) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly GetCallingAssembly() { return default(System.Reflection.Assembly); }
    public virtual object[] GetCustomAttributes(bool inherit) { return default(object[]); }
    public virtual object[] GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public static System.Reflection.Assembly GetEntryAssembly() { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly GetExecutingAssembly() { return default(System.Reflection.Assembly); }
    public virtual System.Type[] GetExportedTypes() { return default(System.Type[]); }
    public virtual System.IO.FileStream GetFile(string name) { return default(System.IO.FileStream); }
    public virtual System.IO.FileStream[] GetFiles() { return default(System.IO.FileStream[]); }
    public virtual System.IO.FileStream[] GetFiles(bool getResourceModules) { return default(System.IO.FileStream[]); }
    public override int GetHashCode() { return default(int); }
    public System.Reflection.Module[] GetLoadedModules() { return default(System.Reflection.Module[]); }
    public virtual System.Reflection.Module[] GetLoadedModules(bool getResourceModules) { return default(System.Reflection.Module[]); }
    public virtual System.Reflection.ManifestResourceInfo GetManifestResourceInfo(string resourceName) { return default(System.Reflection.ManifestResourceInfo); }
    public virtual string[] GetManifestResourceNames() { return default(string[]); }
    public virtual System.IO.Stream GetManifestResourceStream(string name) { return default(System.IO.Stream); }
    public virtual System.IO.Stream GetManifestResourceStream(System.Type type, string name) { return default(System.IO.Stream); }
    public virtual System.Reflection.Module GetModule(string name) { return default(System.Reflection.Module); }
    public System.Reflection.Module[] GetModules() { return default(System.Reflection.Module[]); }
    public virtual System.Reflection.Module[] GetModules(bool getResourceModules) { return default(System.Reflection.Module[]); }
    public virtual System.Reflection.AssemblyName GetName() { return default(System.Reflection.AssemblyName); }
    public virtual System.Reflection.AssemblyName GetName(bool copiedName) { return default(System.Reflection.AssemblyName); }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public virtual System.Reflection.AssemblyName[] GetReferencedAssemblies() { return default(System.Reflection.AssemblyName[]); }
    public virtual System.Reflection.Assembly GetSatelliteAssembly(System.Globalization.CultureInfo culture) { return default(System.Reflection.Assembly); }
    public virtual System.Reflection.Assembly GetSatelliteAssembly(System.Globalization.CultureInfo culture, System.Version version) { return default(System.Reflection.Assembly); }
    public virtual System.Type GetType(string name) { return default(System.Type); }
    public virtual System.Type GetType(string name, bool throwOnError) { return default(System.Type); }
    public virtual System.Type GetType(string name, bool throwOnError, bool ignoreCase) { return default(System.Type); }
    public virtual System.Type[] GetTypes() { return default(System.Type[]); }
    public virtual bool IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    public static System.Reflection.Assembly Load(byte[] rawAssembly) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, System.Security.Policy.Evidence securityEvidence) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyRef) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyRef, System.Security.Policy.Evidence assemblySecurity) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly Load(string assemblyString) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly Load(string assemblyString, System.Security.Policy.Evidence assemblySecurity) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly LoadFile(string path) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly LoadFile(string path, System.Security.Policy.Evidence securityEvidence) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly LoadFrom(string assemblyFile) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly LoadFrom(string assemblyFile, byte[] hashValue, System.Configuration.Assemblies.AssemblyHashAlgorithm hashAlgorithm) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly LoadFrom(string assemblyFile, System.Security.Policy.Evidence securityEvidence) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly LoadFrom(string assemblyFile, System.Security.Policy.Evidence securityEvidence, byte[] hashValue, System.Configuration.Assemblies.AssemblyHashAlgorithm hashAlgorithm) { return default(System.Reflection.Assembly); }
    public System.Reflection.Module LoadModule(string moduleName, byte[] rawModule) { return default(System.Reflection.Module); }
    public virtual System.Reflection.Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore) { return default(System.Reflection.Module); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly LoadWithPartialName(string partialName) { return default(System.Reflection.Assembly); }
    [System.ObsoleteAttribute]
    public static System.Reflection.Assembly LoadWithPartialName(string partialName, System.Security.Policy.Evidence securityEvidence) { return default(System.Reflection.Assembly); }
    public static bool operator ==(System.Reflection.Assembly left, System.Reflection.Assembly right) { return default(bool); }
    public static bool operator !=(System.Reflection.Assembly left, System.Reflection.Assembly right) { return default(bool); }
    public static System.Reflection.Assembly ReflectionOnlyLoad(byte[] rawAssembly) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly ReflectionOnlyLoad(string assemblyString) { return default(System.Reflection.Assembly); }
    public static System.Reflection.Assembly ReflectionOnlyLoadFrom(string assemblyFile) { return default(System.Reflection.Assembly); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    bool System.Reflection.ICustomAttributeProvider.IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    public override string ToString() { return default(string); }
    public static System.Reflection.Assembly UnsafeLoadFrom(string assemblyFile) { return default(System.Reflection.Assembly); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyAlgorithmIdAttribute : System.Attribute {
    public AssemblyAlgorithmIdAttribute(System.Configuration.Assemblies.AssemblyHashAlgorithm algorithmId) { }
    [System.CLSCompliantAttribute(false)]
    public AssemblyAlgorithmIdAttribute(uint algorithmId) { }
    [System.CLSCompliantAttribute(false)]
    public uint AlgorithmId { get { return default(uint); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyCompanyAttribute : System.Attribute {
    public AssemblyCompanyAttribute(string company) { }
    public string Company { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyConfigurationAttribute : System.Attribute {
    public AssemblyConfigurationAttribute(string configuration) { }
    public string Configuration { get { return default(string); } }
  }
  public enum AssemblyContentType {
    Default = 0,
    WindowsRuntime = 1,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyCopyrightAttribute : System.Attribute {
    public AssemblyCopyrightAttribute(string copyright) { }
    public string Copyright { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyCultureAttribute : System.Attribute {
    public AssemblyCultureAttribute(string culture) { }
    public string Culture { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyDefaultAliasAttribute : System.Attribute {
    public AssemblyDefaultAliasAttribute(string defaultAlias) { }
    public string DefaultAlias { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyDelaySignAttribute : System.Attribute {
    public AssemblyDelaySignAttribute(bool delaySign) { }
    public bool DelaySign { get { return default(bool); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyDescriptionAttribute : System.Attribute {
    public AssemblyDescriptionAttribute(string description) { }
    public string Description { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyFileVersionAttribute : System.Attribute {
    public AssemblyFileVersionAttribute(string version) { }
    public string Version { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyFlagsAttribute : System.Attribute {
    [System.ObsoleteAttribute("This constructor has been deprecated. Please use AssemblyFlagsAttribute(AssemblyNameFlags) instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    public AssemblyFlagsAttribute(int assemblyFlags) { }
    public AssemblyFlagsAttribute(System.Reflection.AssemblyNameFlags assemblyFlags) { }
    [System.CLSCompliantAttribute(false)]
    [System.ObsoleteAttribute("This constructor has been deprecated. Please use AssemblyFlagsAttribute(AssemblyNameFlags) instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    public AssemblyFlagsAttribute(uint flags) { }
    public int AssemblyFlags { get { return default(int); } }
    [System.CLSCompliantAttribute(false)]
    [System.ObsoleteAttribute("This property has been deprecated. Please use AssemblyFlags instead. http://go.microsoft.com/fwlink/?linkid=14202")]
    public uint Flags { get { return default(uint); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyInformationalVersionAttribute : System.Attribute {
    public AssemblyInformationalVersionAttribute(string informationalVersion) { }
    public string InformationalVersion { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyKeyFileAttribute : System.Attribute {
    public AssemblyKeyFileAttribute(string keyFile) { }
    public string KeyFile { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyKeyNameAttribute : System.Attribute {
    public AssemblyKeyNameAttribute(string keyName) { }
    public string KeyName { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=true, Inherited=false)]
  public sealed partial class AssemblyMetadataAttribute : System.Attribute {
    public AssemblyMetadataAttribute(string key, string value) { }
    public string Key { get { return default(string); } }
    public string Value { get { return default(string); } }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public sealed partial class AssemblyName : System.ICloneable, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
    public AssemblyName() { }
    public AssemblyName(string assemblyName) { }
    public string CodeBase { get { return default(string); } set { } }
    public System.Reflection.AssemblyContentType ContentType { get { return default(System.Reflection.AssemblyContentType); } set { } }
    public System.Globalization.CultureInfo CultureInfo { get { return default(System.Globalization.CultureInfo); } set { } }
    public string CultureName { get { return default(string); } }
    public string EscapedCodeBase { get { return default(string); } }
    public System.Reflection.AssemblyNameFlags Flags { get { return default(System.Reflection.AssemblyNameFlags); } set { } }
    public string FullName { get { return default(string); } }
    public System.Configuration.Assemblies.AssemblyHashAlgorithm HashAlgorithm { get { return default(System.Configuration.Assemblies.AssemblyHashAlgorithm); } set { } }
    public System.Reflection.StrongNameKeyPair KeyPair { get { return default(System.Reflection.StrongNameKeyPair); } set { } }
    public string Name { get { return default(string); } set { } }
    public System.Reflection.ProcessorArchitecture ProcessorArchitecture { get { return default(System.Reflection.ProcessorArchitecture); } set { } }
    public System.Version Version { get { return default(System.Version); } set { } }
    public System.Configuration.Assemblies.AssemblyVersionCompatibility VersionCompatibility { get { return default(System.Configuration.Assemblies.AssemblyVersionCompatibility); } set { } }
    public object Clone() { return default(object); }
    public static System.Reflection.AssemblyName GetAssemblyName(string assemblyFile) { return default(System.Reflection.AssemblyName); }
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public byte[] GetPublicKey() { return default(byte[]); }
    public byte[] GetPublicKeyToken() { return default(byte[]); }
    public void OnDeserialization(object sender) { }
    public static bool ReferenceMatchesDefinition(System.Reflection.AssemblyName reference, System.Reflection.AssemblyName definition) { return default(bool); }
    public void SetPublicKey(byte[] publicKey) { }
    public void SetPublicKeyToken(byte[] publicKeyToken) { }
    public override string ToString() { return default(string); }
  }
  [System.FlagsAttribute]
  public enum AssemblyNameFlags {
    EnableJITcompileOptimizer = 16384,
    EnableJITcompileTracking = 32768,
    None = 0,
    PublicKey = 1,
    Retargetable = 256,
  }
  public partial class AssemblyNameProxy {
    public AssemblyNameProxy() { }
    public System.Reflection.AssemblyName GetAssemblyName(string assemblyFile) { return default(System.Reflection.AssemblyName); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyProductAttribute : System.Attribute {
    public AssemblyProductAttribute(string product) { }
    public string Product { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false, AllowMultiple=false)]
  public sealed partial class AssemblySignatureKeyAttribute : System.Attribute {
    public AssemblySignatureKeyAttribute(string publicKey, string countersignature) { }
    public string Countersignature { get { return default(string); } }
    public string PublicKey { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyTitleAttribute : System.Attribute {
    public AssemblyTitleAttribute(string title) { }
    public string Title { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyTrademarkAttribute : System.Attribute {
    public AssemblyTrademarkAttribute(string trademark) { }
    public string Trademark { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyVersionAttribute : System.Attribute {
    public AssemblyVersionAttribute(string version) { }
    public string Version { get { return default(string); } }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(2))]
  public abstract partial class Binder {
    protected Binder() { }
    public abstract System.Reflection.FieldInfo BindToField(System.Reflection.BindingFlags bindingAttr, System.Reflection.FieldInfo[] match, object value, System.Globalization.CultureInfo culture);
    public abstract System.Reflection.MethodBase BindToMethod(System.Reflection.BindingFlags bindingAttr, System.Reflection.MethodBase[] match, ref object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] names, out object state);
    public virtual bool CanChangeType(object value, System.Type type, System.Globalization.CultureInfo culture) { return default(bool); }
    public abstract object ChangeType(object value, System.Type type, System.Globalization.CultureInfo culture);
    public abstract void ReorderArgumentArray(ref object[] args, object state);
    public abstract System.Reflection.MethodBase SelectMethod(System.Reflection.BindingFlags bindingAttr, System.Reflection.MethodBase[] match, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    public abstract System.Reflection.PropertyInfo SelectProperty(System.Reflection.BindingFlags bindingAttr, System.Reflection.PropertyInfo[] match, System.Type returnType, System.Type[] indexes, System.Reflection.ParameterModifier[] modifiers);
  }
  [System.FlagsAttribute]
  public enum BindingFlags {
    CreateInstance = 512,
    DeclaredOnly = 2,
    Default = 0,
    ExactBinding = 65536,
    FlattenHierarchy = 64,
    GetField = 1024,
    GetProperty = 4096,
    IgnoreCase = 1,
    IgnoreReturn = 16777216,
    Instance = 4,
    InvokeMethod = 256,
    NonPublic = 32,
    OptionalParamBinding = 262144,
    Public = 16,
    PutDispProperty = 16384,
    PutRefDispProperty = 32768,
    SetField = 2048,
    SetProperty = 8192,
    Static = 8,
    SuppressChangeType = 131072,
  }
  [System.FlagsAttribute]
  public enum CallingConventions {
    Any = 3,
    ExplicitThis = 64,
    HasThis = 32,
    Standard = 1,
    VarArgs = 2,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class ConstructorInfo : System.Reflection.MethodBase {
    public static readonly string ConstructorName;
    public static readonly string TypeConstructorName;
    protected ConstructorInfo() { }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public object Invoke(object[] parameters) { return default(object); }
    public abstract object Invoke(System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] parameters, System.Globalization.CultureInfo culture);
    public static bool operator ==(System.Reflection.ConstructorInfo left, System.Reflection.ConstructorInfo right) { return default(bool); }
    public static bool operator !=(System.Reflection.ConstructorInfo left, System.Reflection.ConstructorInfo right) { return default(bool); }
  }
  public partial class CustomAttributeData {
    protected CustomAttributeData() { }
    public System.Type AttributeType { get { return default(System.Type); } }
    public virtual System.Reflection.ConstructorInfo Constructor { get { return default(System.Reflection.ConstructorInfo); } }
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument> ConstructorArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument>); } }
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument> NamedArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument>); } }
    public override bool Equals(object obj) { return default(bool); }
    public static System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributes(System.Reflection.Assembly target) { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public static System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributes(System.Reflection.MemberInfo target) { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public static System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributes(System.Reflection.Module target) { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public static System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributes(System.Reflection.ParameterInfo target) { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public override int GetHashCode() { return default(int); }
    public override string ToString() { return default(string); }
  }
  public static partial class CustomAttributeExtensions {
    public static System.Attribute GetCustomAttribute(this System.Reflection.Assembly element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(this System.Reflection.Module element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Attribute); }
    public static System.Attribute GetCustomAttribute(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Attribute); }
    public static T GetCustomAttribute<T>(this System.Reflection.Assembly element) where T : System.Attribute { return default(T); }
    public static T GetCustomAttribute<T>(this System.Reflection.MemberInfo element) where T : System.Attribute { return default(T); }
    public static T GetCustomAttribute<T>(this System.Reflection.MemberInfo element, bool inherit) where T : System.Attribute { return default(T); }
    public static T GetCustomAttribute<T>(this System.Reflection.Module element) where T : System.Attribute { return default(T); }
    public static T GetCustomAttribute<T>(this System.Reflection.ParameterInfo element) where T : System.Attribute { return default(T); }
    public static T GetCustomAttribute<T>(this System.Reflection.ParameterInfo element, bool inherit) where T : System.Attribute { return default(T); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Assembly element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Assembly element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Module element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.Module element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<System.Attribute> GetCustomAttributes(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(System.Collections.Generic.IEnumerable<System.Attribute>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.Assembly element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.MemberInfo element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.MemberInfo element, bool inherit) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.Module element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.ParameterInfo element) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static System.Collections.Generic.IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.ParameterInfo element, bool inherit) where T : System.Attribute { return default(System.Collections.Generic.IEnumerable<T>); }
    public static bool IsDefined(this System.Reflection.Assembly element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(this System.Reflection.MemberInfo element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(this System.Reflection.MemberInfo element, System.Type attributeType, bool inherit) { return default(bool); }
    public static bool IsDefined(this System.Reflection.Module element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(this System.Reflection.ParameterInfo element, System.Type attributeType) { return default(bool); }
    public static bool IsDefined(this System.Reflection.ParameterInfo element, System.Type attributeType, bool inherit) { return default(bool); }
  }
  public partial class CustomAttributeFormatException : System.FormatException {
    public CustomAttributeFormatException() { }
    protected CustomAttributeFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public CustomAttributeFormatException(string message) { }
    public CustomAttributeFormatException(string message, System.Exception inner) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct CustomAttributeNamedArgument {
    public CustomAttributeNamedArgument(System.Reflection.MemberInfo memberInfo, object value) { throw new System.NotImplementedException(); }
    public CustomAttributeNamedArgument(System.Reflection.MemberInfo memberInfo, System.Reflection.CustomAttributeTypedArgument typedArgument) { throw new System.NotImplementedException(); }
    public bool IsField { get { return default(bool); } }
    public System.Reflection.MemberInfo MemberInfo { get { return default(System.Reflection.MemberInfo); } }
    public string MemberName { get { return default(string); } }
    public System.Reflection.CustomAttributeTypedArgument TypedValue { get { return default(System.Reflection.CustomAttributeTypedArgument); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.Reflection.CustomAttributeNamedArgument left, System.Reflection.CustomAttributeNamedArgument right) { return default(bool); }
    public static bool operator !=(System.Reflection.CustomAttributeNamedArgument left, System.Reflection.CustomAttributeNamedArgument right) { return default(bool); }
    public override string ToString() { return default(string); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct CustomAttributeTypedArgument {
    public CustomAttributeTypedArgument(object value) { throw new System.NotImplementedException(); }
    public CustomAttributeTypedArgument(System.Type argumentType, object value) { throw new System.NotImplementedException(); }
    public System.Type ArgumentType { get { return default(System.Type); } }
    public object Value { get { return default(object); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.Reflection.CustomAttributeTypedArgument left, System.Reflection.CustomAttributeTypedArgument right) { return default(bool); }
    public static bool operator !=(System.Reflection.CustomAttributeTypedArgument left, System.Reflection.CustomAttributeTypedArgument right) { return default(bool); }
    public override string ToString() { return default(string); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1036))]
  public sealed partial class DefaultMemberAttribute : System.Attribute {
    public DefaultMemberAttribute(string memberName) { }
    public string MemberName { get { return default(string); } }
  }
  [System.FlagsAttribute]
  public enum EventAttributes {
    None = 0,
    ReservedMask = 1024,
    RTSpecialName = 1024,
    SpecialName = 512,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class EventInfo : System.Reflection.MemberInfo {
    protected EventInfo() { }
    public virtual System.Reflection.MethodInfo AddMethod { get { return default(System.Reflection.MethodInfo); } }
    public abstract System.Reflection.EventAttributes Attributes { get; }
    public virtual System.Type EventHandlerType { get { return default(System.Type); } }
    public virtual bool IsMulticast { get { return default(bool); } }
    public bool IsSpecialName { get { return default(bool); } }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public virtual System.Reflection.MethodInfo RaiseMethod { get { return default(System.Reflection.MethodInfo); } }
    public virtual System.Reflection.MethodInfo RemoveMethod { get { return default(System.Reflection.MethodInfo); } }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public virtual void AddEventHandler(object target, System.Delegate handler) { }
    public override bool Equals(object obj) { return default(bool); }
    public System.Reflection.MethodInfo GetAddMethod() { return default(System.Reflection.MethodInfo); }
    public abstract System.Reflection.MethodInfo GetAddMethod(bool nonPublic);
    public override int GetHashCode() { return default(int); }
    public System.Reflection.MethodInfo[] GetOtherMethods() { return default(System.Reflection.MethodInfo[]); }
    public virtual System.Reflection.MethodInfo[] GetOtherMethods(bool nonPublic) { return default(System.Reflection.MethodInfo[]); }
    public System.Reflection.MethodInfo GetRaiseMethod() { return default(System.Reflection.MethodInfo); }
    public abstract System.Reflection.MethodInfo GetRaiseMethod(bool nonPublic);
    public System.Reflection.MethodInfo GetRemoveMethod() { return default(System.Reflection.MethodInfo); }
    public abstract System.Reflection.MethodInfo GetRemoveMethod(bool nonPublic);
    public static bool operator ==(System.Reflection.EventInfo left, System.Reflection.EventInfo right) { return default(bool); }
    public static bool operator !=(System.Reflection.EventInfo left, System.Reflection.EventInfo right) { return default(bool); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public virtual void RemoveEventHandler(object target, System.Delegate handler) { }
  }
  public partial class ExceptionHandlingClause {
    protected ExceptionHandlingClause() { }
    public virtual System.Type CatchType { get { return default(System.Type); } }
    public virtual int FilterOffset { get { return default(int); } }
    public virtual System.Reflection.ExceptionHandlingClauseOptions Flags { get { return default(System.Reflection.ExceptionHandlingClauseOptions); } }
    public virtual int HandlerLength { get { return default(int); } }
    public virtual int HandlerOffset { get { return default(int); } }
    public virtual int TryLength { get { return default(int); } }
    public virtual int TryOffset { get { return default(int); } }
    public override string ToString() { return default(string); }
  }
  [System.FlagsAttribute]
  public enum ExceptionHandlingClauseOptions {
    Clause = 0,
    Fault = 4,
    Filter = 1,
    Finally = 2,
  }
  [System.FlagsAttribute]
  public enum FieldAttributes {
    Assembly = 3,
    FamANDAssem = 2,
    Family = 4,
    FamORAssem = 5,
    FieldAccessMask = 7,
    HasDefault = 32768,
    HasFieldMarshal = 4096,
    HasFieldRVA = 256,
    InitOnly = 32,
    Literal = 64,
    NotSerialized = 128,
    PinvokeImpl = 8192,
    Private = 1,
    PrivateScope = 0,
    Public = 6,
    ReservedMask = 38144,
    RTSpecialName = 1024,
    SpecialName = 512,
    Static = 16,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class FieldInfo : System.Reflection.MemberInfo {
    protected FieldInfo() { }
    public abstract System.Reflection.FieldAttributes Attributes { get; }
    public abstract System.RuntimeFieldHandle FieldHandle { get; }
    public abstract System.Type FieldType { get; }
    public bool IsAssembly { get { return default(bool); } }
    public bool IsFamily { get { return default(bool); } }
    public bool IsFamilyAndAssembly { get { return default(bool); } }
    public bool IsFamilyOrAssembly { get { return default(bool); } }
    public bool IsInitOnly { get { return default(bool); } }
    public bool IsLiteral { get { return default(bool); } }
    public bool IsNotSerialized { get { return default(bool); } }
    public bool IsPinvokeImpl { get { return default(bool); } }
    public bool IsPrivate { get { return default(bool); } }
    public bool IsPublic { get { return default(bool); } }
    public virtual bool IsSecurityCritical { get { return default(bool); } }
    public virtual bool IsSecuritySafeCritical { get { return default(bool); } }
    public virtual bool IsSecurityTransparent { get { return default(bool); } }
    public bool IsSpecialName { get { return default(bool); } }
    public bool IsStatic { get { return default(bool); } }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public override bool Equals(object obj) { return default(bool); }
    public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle) { return default(System.Reflection.FieldInfo); }
    public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.FieldInfo); }
    public override int GetHashCode() { return default(int); }
    public virtual System.Type[] GetOptionalCustomModifiers() { return default(System.Type[]); }
    public virtual object GetRawConstantValue() { return default(object); }
    public virtual System.Type[] GetRequiredCustomModifiers() { return default(System.Type[]); }
    public abstract object GetValue(object obj);
    [System.CLSCompliantAttribute(false)]
    public virtual object GetValueDirect(System.TypedReference obj) { return default(object); }
    public static bool operator ==(System.Reflection.FieldInfo left, System.Reflection.FieldInfo right) { return default(bool); }
    public static bool operator !=(System.Reflection.FieldInfo left, System.Reflection.FieldInfo right) { return default(bool); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public void SetValue(object obj, object value) { }
    public abstract void SetValue(object obj, object value, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Globalization.CultureInfo culture);
    [System.CLSCompliantAttribute(false)]
    public virtual void SetValueDirect(System.TypedReference obj, object value) { }
  }
  [System.FlagsAttribute]
  public enum GenericParameterAttributes {
    Contravariant = 2,
    Covariant = 1,
    DefaultConstructorConstraint = 16,
    None = 0,
    NotNullableValueTypeConstraint = 8,
    ReferenceTypeConstraint = 4,
    SpecialConstraintMask = 28,
    VarianceMask = 3,
  }
  public partial interface ICustomAttributeProvider {
    object[] GetCustomAttributes(bool inherit);
    object[] GetCustomAttributes(System.Type attributeType, bool inherit);
    bool IsDefined(System.Type attributeType, bool inherit);
  }
  public enum ImageFileMachine {
    AMD64 = 34404,
    ARM = 452,
    I386 = 332,
    IA64 = 512,
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct InterfaceMapping {
    public System.Reflection.MethodInfo[] InterfaceMethods;
    public System.Type InterfaceType;
    public System.Reflection.MethodInfo[] TargetMethods;
    public System.Type TargetType;
  }
  public static partial class IntrospectionExtensions {
    public static System.Reflection.TypeInfo GetTypeInfo(this System.Type type) { return default(System.Reflection.TypeInfo); }
  }
  public partial class InvalidFilterCriteriaException : System.ApplicationException {
    public InvalidFilterCriteriaException() { }
    protected InvalidFilterCriteriaException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public InvalidFilterCriteriaException(string message) { }
    public InvalidFilterCriteriaException(string message, System.Exception inner) { }
  }
  [System.Runtime.InteropServices.GuidAttribute("AFBF15E5-C37C-11d2-B88E-00A0C9B471B8")]
  public partial interface IReflect {
    System.Type UnderlyingSystemType { get; }
    System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr);
    System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr);
    System.Reflection.MemberInfo[] GetMember(string name, System.Reflection.BindingFlags bindingAttr);
    System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr);
    System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr);
    System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr);
    System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr);
    System.Reflection.PropertyInfo GetProperty(string name, System.Reflection.BindingFlags bindingAttr);
    System.Reflection.PropertyInfo GetProperty(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
    object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters);
  }
  public partial interface IReflectableType {
    System.Reflection.TypeInfo GetTypeInfo();
  }
  public partial class LocalVariableInfo {
    protected LocalVariableInfo() { }
    public virtual bool IsPinned { get { return default(bool); } }
    public virtual int LocalIndex { get { return default(int); } }
    public virtual System.Type LocalType { get { return default(System.Type); } }
    public override string ToString() { return default(string); }
  }
  public partial class ManifestResourceInfo {
    public ManifestResourceInfo(System.Reflection.Assembly containingAssembly, string containingFileName, System.Reflection.ResourceLocation resourceLocation) { }
    public virtual string FileName { get { return default(string); } }
    public virtual System.Reflection.Assembly ReferencedAssembly { get { return default(System.Reflection.Assembly); } }
    public virtual System.Reflection.ResourceLocation ResourceLocation { get { return default(System.Reflection.ResourceLocation); } }
  }
  public delegate bool MemberFilter(System.Reflection.MemberInfo m, object filterCriteria);
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class MemberInfo : System.Reflection.ICustomAttributeProvider {
    protected MemberInfo() { }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
    public abstract System.Type DeclaringType { get; }
    public abstract System.Reflection.MemberTypes MemberType { get; }
    public virtual int MetadataToken { get { return default(int); } }
    public virtual System.Reflection.Module Module { get { return default(System.Reflection.Module); } }
    public abstract string Name { get; }
    public abstract System.Type ReflectedType { get; }
    public override bool Equals(object obj) { return default(bool); }
    public abstract object[] GetCustomAttributes(bool inherit);
    public abstract object[] GetCustomAttributes(System.Type attributeType, bool inherit);
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public override int GetHashCode() { return default(int); }
    public abstract bool IsDefined(System.Type attributeType, bool inherit);
    public static bool operator ==(System.Reflection.MemberInfo left, System.Reflection.MemberInfo right) { return default(bool); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public static bool operator !=(System.Reflection.MemberInfo left, System.Reflection.MemberInfo right) { return default(bool); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    bool System.Reflection.ICustomAttributeProvider.IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
  }
  [System.FlagsAttribute]
  public enum MemberTypes {
    All = 191,
    Constructor = 1,
    Custom = 64,
    Event = 2,
    Field = 4,
    Method = 8,
    NestedType = 128,
    Property = 16,
    TypeInfo = 32,
  }
  [System.FlagsAttribute]
  public enum MethodAttributes {
    Abstract = 1024,
    Assembly = 3,
    CheckAccessOnOverride = 512,
    FamANDAssem = 2,
    Family = 4,
    FamORAssem = 5,
    Final = 32,
    HasSecurity = 16384,
    HideBySig = 128,
    MemberAccessMask = 7,
    NewSlot = 256,
    PinvokeImpl = 8192,
    Private = 1,
    PrivateScope = 0,
    Public = 6,
    RequireSecObject = 32768,
    ReservedMask = 53248,
    ReuseSlot = 0,
    RTSpecialName = 4096,
    SpecialName = 2048,
    Static = 16,
    UnmanagedExport = 8,
    Virtual = 64,
    VtableLayoutMask = 256,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class MethodBase : System.Reflection.MemberInfo {
    protected MethodBase() { }
    public abstract System.Reflection.MethodAttributes Attributes { get; }
    public virtual System.Reflection.CallingConventions CallingConvention { get { return default(System.Reflection.CallingConventions); } }
    public virtual bool ContainsGenericParameters { get { return default(bool); } }
    public bool IsAbstract { get { return default(bool); } }
    public bool IsAssembly { get { return default(bool); } }
    public bool IsConstructor { get { return default(bool); } }
    public bool IsFamily { get { return default(bool); } }
    public bool IsFamilyAndAssembly { get { return default(bool); } }
    public bool IsFamilyOrAssembly { get { return default(bool); } }
    public bool IsFinal { get { return default(bool); } }
    public virtual bool IsGenericMethod { get { return default(bool); } }
    public virtual bool IsGenericMethodDefinition { get { return default(bool); } }
    public bool IsHideBySig { get { return default(bool); } }
    public bool IsPrivate { get { return default(bool); } }
    public bool IsPublic { get { return default(bool); } }
    public virtual bool IsSecurityCritical { get { return default(bool); } }
    public virtual bool IsSecuritySafeCritical { get { return default(bool); } }
    public virtual bool IsSecurityTransparent { get { return default(bool); } }
    public bool IsSpecialName { get { return default(bool); } }
    public bool IsStatic { get { return default(bool); } }
    public bool IsVirtual { get { return default(bool); } }
    public abstract System.RuntimeMethodHandle MethodHandle { get; }
    public virtual System.Reflection.MethodImplAttributes MethodImplementationFlags { get { return default(System.Reflection.MethodImplAttributes); } }
    public override bool Equals(object obj) { return default(bool); }
    public static System.Reflection.MethodBase GetCurrentMethod() { return default(System.Reflection.MethodBase); }
    public virtual System.Type[] GetGenericArguments() { return default(System.Type[]); }
    public override int GetHashCode() { return default(int); }
    [System.Security.SecuritySafeCriticalAttribute]
    public virtual System.Reflection.MethodBody GetMethodBody() { return default(System.Reflection.MethodBody); }
    public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle) { return default(System.Reflection.MethodBase); }
    public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.MethodBase); }
    public abstract System.Reflection.MethodImplAttributes GetMethodImplementationFlags();
    public abstract System.Reflection.ParameterInfo[] GetParameters();
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public object Invoke(object obj, object[] parameters) { return default(object); }
    public abstract object Invoke(object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] parameters, System.Globalization.CultureInfo culture);
    public static bool operator ==(System.Reflection.MethodBase left, System.Reflection.MethodBase right) { return default(bool); }
    public static bool operator !=(System.Reflection.MethodBase left, System.Reflection.MethodBase right) { return default(bool); }
  }
  public partial class MethodBody {
    protected MethodBody() { }
    public virtual System.Collections.Generic.IList<System.Reflection.ExceptionHandlingClause> ExceptionHandlingClauses { get { return default(System.Collections.Generic.IList<System.Reflection.ExceptionHandlingClause>); } }
    public virtual bool InitLocals { get { return default(bool); } }
    public virtual int LocalSignatureMetadataToken { get { return default(int); } }
    public virtual System.Collections.Generic.IList<System.Reflection.LocalVariableInfo> LocalVariables { get { return default(System.Collections.Generic.IList<System.Reflection.LocalVariableInfo>); } }
    public virtual int MaxStackSize { get { return default(int); } }
    public virtual byte[] GetILAsByteArray() { return default(byte[]); }
  }
  public enum MethodImplAttributes {
    AggressiveInlining = 256,
    CodeTypeMask = 3,
    ForwardRef = 16,
    IL = 0,
    InternalCall = 4096,
    Managed = 0,
    ManagedMask = 4,
    MaxMethodImplVal = 65535,
    Native = 1,
    NoInlining = 8,
    NoOptimization = 64,
    OPTIL = 2,
    PreserveSig = 128,
    Runtime = 3,
    Synchronized = 32,
    Unmanaged = 4,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class MethodInfo : System.Reflection.MethodBase {
    protected MethodInfo() { }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public virtual System.Reflection.ParameterInfo ReturnParameter { get { return default(System.Reflection.ParameterInfo); } }
    public virtual System.Type ReturnType { get { return default(System.Type); } }
    public abstract System.Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes { get; }
    public virtual System.Delegate CreateDelegate(System.Type delegateType) { return default(System.Delegate); }
    public virtual System.Delegate CreateDelegate(System.Type delegateType, object target) { return default(System.Delegate); }
    public override bool Equals(object obj) { return default(bool); }
    public abstract System.Reflection.MethodInfo GetBaseDefinition();
    public override System.Type[] GetGenericArguments() { return default(System.Type[]); }
    public virtual System.Reflection.MethodInfo GetGenericMethodDefinition() { return default(System.Reflection.MethodInfo); }
    public override int GetHashCode() { return default(int); }
    public virtual System.Reflection.MethodInfo MakeGenericMethod(params System.Type[] typeArguments) { return default(System.Reflection.MethodInfo); }
    public static bool operator ==(System.Reflection.MethodInfo left, System.Reflection.MethodInfo right) { return default(bool); }
    public static bool operator !=(System.Reflection.MethodInfo left, System.Reflection.MethodInfo right) { return default(bool); }
  }
  public sealed partial class Missing : System.Runtime.Serialization.ISerializable {
    internal Missing() { }
    public static readonly System.Reflection.Missing Value;
    [System.Security.SecurityCriticalAttribute]
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class Module : System.Reflection.ICustomAttributeProvider, System.Runtime.Serialization.ISerializable {
    public static readonly System.Reflection.TypeFilter FilterTypeName;
    public static readonly System.Reflection.TypeFilter FilterTypeNameIgnoreCase;
    protected Module() { }
    public virtual System.Reflection.Assembly Assembly { get { return default(System.Reflection.Assembly); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
    public virtual string FullyQualifiedName { get { return default(string); } }
    public virtual int MDStreamVersion { get { return default(int); } }
    public virtual int MetadataToken { get { return default(int); } }
    public System.ModuleHandle ModuleHandle { get { return default(System.ModuleHandle); } }
    public virtual System.Guid ModuleVersionId { get { return default(System.Guid); } }
    public virtual string Name { get { return default(string); } }
    public virtual string ScopeName { get { return default(string); } }
    public override bool Equals(object o) { return default(bool); }
    public virtual System.Type[] FindTypes(System.Reflection.TypeFilter filter, object filterCriteria) { return default(System.Type[]); }
    public virtual object[] GetCustomAttributes(bool inherit) { return default(object[]); }
    public virtual object[] GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public System.Reflection.FieldInfo GetField(string name) { return default(System.Reflection.FieldInfo); }
    public virtual System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.FieldInfo); }
    public System.Reflection.FieldInfo[] GetFields() { return default(System.Reflection.FieldInfo[]); }
    public virtual System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingFlags) { return default(System.Reflection.FieldInfo[]); }
    public override int GetHashCode() { return default(int); }
    public System.Reflection.MethodInfo GetMethod(string name) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo GetMethod(string name, System.Type[] types) { return default(System.Reflection.MethodInfo); }
    protected virtual System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    public System.Reflection.MethodInfo[] GetMethods() { return default(System.Reflection.MethodInfo[]); }
    public virtual System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingFlags) { return default(System.Reflection.MethodInfo[]); }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public virtual void GetPEKind(out System.Reflection.PortableExecutableKinds peKind, out System.Reflection.ImageFileMachine machine) { peKind = default(System.Reflection.PortableExecutableKinds); machine = default(System.Reflection.ImageFileMachine); }
    public virtual System.Type GetType(string className) { return default(System.Type); }
    public virtual System.Type GetType(string className, bool ignoreCase) { return default(System.Type); }
    public virtual System.Type GetType(string className, bool throwOnError, bool ignoreCase) { return default(System.Type); }
    public virtual System.Type[] GetTypes() { return default(System.Type[]); }
    public virtual bool IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    public virtual bool IsResource() { return default(bool); }
    public static bool operator ==(System.Reflection.Module left, System.Reflection.Module right) { return default(bool); }
    public static bool operator !=(System.Reflection.Module left, System.Reflection.Module right) { return default(bool); }
    public System.Reflection.FieldInfo ResolveField(int metadataToken) { return default(System.Reflection.FieldInfo); }
    public virtual System.Reflection.FieldInfo ResolveField(int metadataToken, System.Type[] genericTypeArguments, System.Type[] genericMethodArguments) { return default(System.Reflection.FieldInfo); }
    public System.Reflection.MemberInfo ResolveMember(int metadataToken) { return default(System.Reflection.MemberInfo); }
    public virtual System.Reflection.MemberInfo ResolveMember(int metadataToken, System.Type[] genericTypeArguments, System.Type[] genericMethodArguments) { return default(System.Reflection.MemberInfo); }
    public System.Reflection.MethodBase ResolveMethod(int metadataToken) { return default(System.Reflection.MethodBase); }
    public virtual System.Reflection.MethodBase ResolveMethod(int metadataToken, System.Type[] genericTypeArguments, System.Type[] genericMethodArguments) { return default(System.Reflection.MethodBase); }
    public virtual byte[] ResolveSignature(int metadataToken) { return default(byte[]); }
    public virtual string ResolveString(int metadataToken) { return default(string); }
    public System.Type ResolveType(int metadataToken) { return default(System.Type); }
    public virtual System.Type ResolveType(int metadataToken, System.Type[] genericTypeArguments, System.Type[] genericMethodArguments) { return default(System.Type); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    bool System.Reflection.ICustomAttributeProvider.IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    public override string ToString() { return default(string); }
  }
  public delegate System.Reflection.Module ModuleResolveEventHandler(object sender, System.ResolveEventArgs e);
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false, Inherited=false)]
  public sealed partial class ObfuscateAssemblyAttribute : System.Attribute {
    public ObfuscateAssemblyAttribute(bool assemblyIsPrivate) { }
    public bool AssemblyIsPrivate { get { return default(bool); } }
    public bool StripAfterObfuscation { get { return default(bool); } set { } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(8157), AllowMultiple=true, Inherited=false)]
  public sealed partial class ObfuscationAttribute : System.Attribute {
    public ObfuscationAttribute() { }
    public bool ApplyToMembers { get { return default(bool); } set { } }
    public bool Exclude { get { return default(bool); } set { } }
    public string Feature { get { return default(string); } set { } }
    public bool StripAfterObfuscation { get { return default(bool); } set { } }
  }
  [System.FlagsAttribute]
  public enum ParameterAttributes {
    HasDefault = 4096,
    HasFieldMarshal = 8192,
    In = 1,
    Lcid = 4,
    None = 0,
    Optional = 16,
    Out = 2,
    Reserved3 = 16384,
    Reserved4 = 32768,
    ReservedMask = 61440,
    Retval = 8,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public partial class ParameterInfo : System.Reflection.ICustomAttributeProvider, System.Runtime.Serialization.IObjectReference {
    protected System.Reflection.ParameterAttributes AttrsImpl;
    protected System.Type ClassImpl;
    protected object DefaultValueImpl;
    protected System.Reflection.MemberInfo MemberImpl;
    protected string NameImpl;
    protected int PositionImpl;
    protected ParameterInfo() { }
    public virtual System.Reflection.ParameterAttributes Attributes { get { return default(System.Reflection.ParameterAttributes); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
    public virtual object DefaultValue { get { return default(object); } }
    public virtual bool HasDefaultValue { get { return default(bool); } }
    public bool IsIn { get { return default(bool); } }
    public bool IsLcid { get { return default(bool); } }
    public bool IsOptional { get { return default(bool); } }
    public bool IsOut { get { return default(bool); } }
    public bool IsRetval { get { return default(bool); } }
    public virtual System.Reflection.MemberInfo Member { get { return default(System.Reflection.MemberInfo); } }
    public virtual int MetadataToken { get { return default(int); } }
    public virtual string Name { get { return default(string); } }
    public virtual System.Type ParameterType { get { return default(System.Type); } }
    public virtual int Position { get { return default(int); } }
    public virtual object RawDefaultValue { get { return default(object); } }
    public virtual object[] GetCustomAttributes(bool inherit) { return default(object[]); }
    public virtual object[] GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeData>); }
    public virtual System.Type[] GetOptionalCustomModifiers() { return default(System.Type[]); }
    public object GetRealObject(System.Runtime.Serialization.StreamingContext context) { return default(object); }
    public virtual System.Type[] GetRequiredCustomModifiers() { return default(System.Type[]); }
    public virtual bool IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
    object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(System.Type attributeType, bool inherit) { return default(object[]); }
    bool System.Reflection.ICustomAttributeProvider.IsDefined(System.Type attributeType, bool inherit) { return default(bool); }
    public override string ToString() { return default(string); }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct ParameterModifier {
    public ParameterModifier(int parameterCount) { throw new System.NotImplementedException(); }
    public bool this[int index] { get { return default(bool); } set { } }
  }
  [System.CLSCompliantAttribute(false)]
  public sealed partial class Pointer : System.Runtime.Serialization.ISerializable {
    internal Pointer() { }
    [System.Security.SecurityCriticalAttribute]
    public unsafe static object Box(void* ptr, System.Type type) { return default(object); }
    [System.Security.SecurityCriticalAttribute]
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    [System.Security.SecurityCriticalAttribute]
    public unsafe static void* Unbox(object ptr) { return default(void*); }
  }
  [System.FlagsAttribute]
  public enum PortableExecutableKinds {
    ILOnly = 1,
    NotAPortableExecutableImage = 0,
    PE32Plus = 4,
    Preferred32Bit = 16,
    Required32Bit = 2,
    Unmanaged32Bit = 8,
  }
  public enum ProcessorArchitecture {
    Amd64 = 4,
    Arm = 5,
    IA64 = 3,
    MSIL = 1,
    None = 0,
    X86 = 2,
  }
  [System.FlagsAttribute]
  public enum PropertyAttributes {
    HasDefault = 4096,
    None = 0,
    Reserved2 = 8192,
    Reserved3 = 16384,
    Reserved4 = 32768,
    ReservedMask = 62464,
    RTSpecialName = 1024,
    SpecialName = 512,
  }
  [System.Runtime.InteropServices.ClassInterfaceAttribute((System.Runtime.InteropServices.ClassInterfaceType)(0))]
  public abstract partial class PropertyInfo : System.Reflection.MemberInfo {
    protected PropertyInfo() { }
    public abstract System.Reflection.PropertyAttributes Attributes { get; }
    public abstract bool CanRead { get; }
    public abstract bool CanWrite { get; }
    public virtual System.Reflection.MethodInfo GetMethod { get { return default(System.Reflection.MethodInfo); } }
    public bool IsSpecialName { get { return default(bool); } }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public abstract System.Type PropertyType { get; }
    public virtual System.Reflection.MethodInfo SetMethod { get { return default(System.Reflection.MethodInfo); } }
    public override bool Equals(object obj) { return default(bool); }
    public System.Reflection.MethodInfo[] GetAccessors() { return default(System.Reflection.MethodInfo[]); }
    public abstract System.Reflection.MethodInfo[] GetAccessors(bool nonPublic);
    public virtual object GetConstantValue() { return default(object); }
    public System.Reflection.MethodInfo GetGetMethod() { return default(System.Reflection.MethodInfo); }
    public abstract System.Reflection.MethodInfo GetGetMethod(bool nonPublic);
    public override int GetHashCode() { return default(int); }
    public abstract System.Reflection.ParameterInfo[] GetIndexParameters();
    public virtual System.Type[] GetOptionalCustomModifiers() { return default(System.Type[]); }
    public virtual object GetRawConstantValue() { return default(object); }
    public virtual System.Type[] GetRequiredCustomModifiers() { return default(System.Type[]); }
    public System.Reflection.MethodInfo GetSetMethod() { return default(System.Reflection.MethodInfo); }
    public abstract System.Reflection.MethodInfo GetSetMethod(bool nonPublic);
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public object GetValue(object obj) { return default(object); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public virtual object GetValue(object obj, object[] index) { return default(object); }
    public abstract object GetValue(object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture);
    public static bool operator ==(System.Reflection.PropertyInfo left, System.Reflection.PropertyInfo right) { return default(bool); }
    public static bool operator !=(System.Reflection.PropertyInfo left, System.Reflection.PropertyInfo right) { return default(bool); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public void SetValue(object obj, object value) { }
    [System.Diagnostics.DebuggerHiddenAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public virtual void SetValue(object obj, object value, object[] index) { }
    public abstract void SetValue(object obj, object value, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture);
  }
  public abstract partial class ReflectionContext {
    protected ReflectionContext() { }
    public virtual System.Reflection.TypeInfo GetTypeForObject(object value) { return default(System.Reflection.TypeInfo); }
    public abstract System.Reflection.Assembly MapAssembly(System.Reflection.Assembly assembly);
    public abstract System.Reflection.TypeInfo MapType(System.Reflection.TypeInfo type);
  }
  public sealed partial class ReflectionTypeLoadException : System.SystemException {
    public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions) { }
    public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions, string message) { }
    public System.Exception[] LoaderExceptions { get { return default(System.Exception[]); } }
    public System.Type[] Types { get { return default(System.Type[]); } }
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  [System.FlagsAttribute]
  public enum ResourceAttributes {
    Private = 2,
    Public = 1,
  }
  [System.FlagsAttribute]
  public enum ResourceLocation {
    ContainedInAnotherAssembly = 2,
    ContainedInManifestFile = 4,
    Embedded = 1,
  }
  public static partial class RuntimeReflectionExtensions {
    public static System.Reflection.MethodInfo GetMethodInfo(this System.Delegate del) { return default(System.Reflection.MethodInfo); }
    public static System.Reflection.MethodInfo GetRuntimeBaseDefinition(this System.Reflection.MethodInfo method) { return default(System.Reflection.MethodInfo); }
    public static System.Reflection.EventInfo GetRuntimeEvent(this System.Type type, string name) { return default(System.Reflection.EventInfo); }
    public static System.Collections.Generic.IEnumerable<System.Reflection.EventInfo> GetRuntimeEvents(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.EventInfo>); }
    public static System.Reflection.FieldInfo GetRuntimeField(this System.Type type, string name) { return default(System.Reflection.FieldInfo); }
    public static System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo> GetRuntimeFields(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo>); }
    public static System.Reflection.InterfaceMapping GetRuntimeInterfaceMap(this System.Reflection.TypeInfo typeInfo, System.Type interfaceType) { return default(System.Reflection.InterfaceMapping); }
    public static System.Reflection.MethodInfo GetRuntimeMethod(this System.Type type, string name, System.Type[] parameters) { return default(System.Reflection.MethodInfo); }
    public static System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> GetRuntimeMethods(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); }
    public static System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> GetRuntimeProperties(this System.Type type) { return default(System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo>); }
    public static System.Reflection.PropertyInfo GetRuntimeProperty(this System.Type type, string name) { return default(System.Reflection.PropertyInfo); }
  }
  public partial class StrongNameKeyPair : System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable {
    public StrongNameKeyPair(byte[] keyPairArray) { }
    public StrongNameKeyPair(System.IO.FileStream keyPairFile) { }
    protected StrongNameKeyPair(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public StrongNameKeyPair(string keyPairContainer) { }
    public byte[] PublicKey { get { return default(byte[]); } }
    void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) { }
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  public partial class TargetException : System.Exception {
    public TargetException() { }
    protected TargetException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TargetException(string message) { }
    public TargetException(string message, System.Exception inner) { }
  }
  public sealed partial class TargetInvocationException : System.Exception {
    public TargetInvocationException(System.Exception inner) { }
    public TargetInvocationException(string message, System.Exception inner) { }
  }
  public sealed partial class TargetParameterCountException : System.Exception {
    public TargetParameterCountException() { }
    public TargetParameterCountException(string message) { }
    public TargetParameterCountException(string message, System.Exception inner) { }
  }
  [System.FlagsAttribute]
  public enum TypeAttributes {
    Abstract = 128,
    AnsiClass = 0,
    AutoClass = 131072,
    AutoLayout = 0,
    BeforeFieldInit = 1048576,
    Class = 0,
    ClassSemanticsMask = 32,
    CustomFormatClass = 196608,
    CustomFormatMask = 12582912,
    ExplicitLayout = 16,
    HasSecurity = 262144,
    Import = 4096,
    Interface = 32,
    LayoutMask = 24,
    NestedAssembly = 5,
    NestedFamANDAssem = 6,
    NestedFamily = 4,
    NestedFamORAssem = 7,
    NestedPrivate = 3,
    NestedPublic = 2,
    NotPublic = 0,
    Public = 1,
    ReservedMask = 264192,
    RTSpecialName = 2048,
    Sealed = 256,
    SequentialLayout = 8,
    Serializable = 8192,
    SpecialName = 1024,
    StringFormatMask = 196608,
    UnicodeClass = 65536,
    VisibilityMask = 7,
    WindowsRuntime = 16384,
  }
  public delegate bool TypeFilter(System.Type m, object filterCriteria);
  public abstract partial class TypeInfo : System.Type, System.Reflection.IReflectableType {
    internal TypeInfo() { }
    public override System.Reflection.Assembly Assembly { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(System.Reflection.Assembly); } }
    public override string AssemblyQualifiedName { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(string); } }
    public abstract new System.Reflection.TypeAttributes Attributes { get; }
    public override System.Type BaseType { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(System.Type); } }
    public abstract new bool ContainsGenericParameters { get; }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo> DeclaredConstructors { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.EventInfo> DeclaredEvents { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.EventInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo> DeclaredFields { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo> DeclaredMembers { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> DeclaredMethods { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo>); } }
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> DeclaredProperties { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo>); } }
    public abstract new System.Reflection.MethodBase DeclaringMethod { get; }
    public override string FullName { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(string); } }
    public abstract new System.Reflection.GenericParameterAttributes GenericParameterAttributes { get; }
    public abstract new int GenericParameterPosition { get; }
    public abstract new System.Type[] GenericTypeArguments { get; }
    public virtual System.Type[] GenericTypeParameters { get { return default(System.Type[]); } }
    public override System.Guid GUID { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(System.Guid); } }
    public new bool HasElementType { get { return default(bool); } }
    public virtual System.Collections.Generic.IEnumerable<System.Type> ImplementedInterfaces { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
    public new bool IsAbstract { get { return default(bool); } }
    public new bool IsAnsiClass { get { return default(bool); } }
    public new bool IsArray { get { return default(bool); } }
    public new bool IsAutoClass { get { return default(bool); } }
    public new bool IsAutoLayout { get { return default(bool); } }
    public new bool IsByRef { get { return default(bool); } }
    public new bool IsClass { get { return default(bool); } }
    public virtual new bool IsCOMObject { get { return default(bool); } }
    public abstract new bool IsEnum { get; }
    public new bool IsExplicitLayout { get { return default(bool); } }
    public abstract new bool IsGenericParameter { get; }
    public abstract new bool IsGenericType { get; }
    public abstract new bool IsGenericTypeDefinition { get; }
    public new bool IsImport { get { return default(bool); } }
    public new bool IsInterface { get { return default(bool); } }
    public new bool IsLayoutSequential { get { return default(bool); } }
    public new bool IsMarshalByRef { get { return default(bool); } }
    public new bool IsNested { get { return default(bool); } }
    public new bool IsNestedAssembly { get { return default(bool); } }
    public new bool IsNestedFamANDAssem { get { return default(bool); } }
    public new bool IsNestedFamily { get { return default(bool); } }
    public new bool IsNestedFamORAssem { get { return default(bool); } }
    public new bool IsNestedPrivate { get { return default(bool); } }
    public new bool IsNestedPublic { get { return default(bool); } }
    public new bool IsNotPublic { get { return default(bool); } }
    public new bool IsPointer { get { return default(bool); } }
    public virtual new bool IsPrimitive { get { return default(bool); } }
    public new bool IsPublic { get { return default(bool); } }
    public new bool IsSealed { get { return default(bool); } }
    public abstract new bool IsSerializable { get; }
    public new bool IsSpecialName { get { return default(bool); } }
    public new bool IsUnicodeClass { get { return default(bool); } }
    public virtual new bool IsValueType { get { return default(bool); } }
    public new bool IsVisible { get { return default(bool); } }
    public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    public override string Namespace { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(string); } }
    public virtual new System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute { get { return default(System.Runtime.InteropServices.StructLayoutAttribute); } }
    public new System.Reflection.ConstructorInfo TypeInitializer { get { return default(System.Reflection.ConstructorInfo); } }
    public override System.Type UnderlyingSystemType { get { return default(System.Type); } }
    public virtual System.Type AsType() { return default(System.Type); }
    public virtual new System.Type[] FindInterfaces(System.Reflection.TypeFilter filter, object filterCriteria) { return default(System.Type[]); }
    public virtual new System.Reflection.MemberInfo[] FindMembers(System.Reflection.MemberTypes memberType, System.Reflection.BindingFlags bindingAttr, System.Reflection.MemberFilter filter, object filterCriteria) { return default(System.Reflection.MemberInfo[]); }
    public abstract new int GetArrayRank();
    public new System.Reflection.ConstructorInfo GetConstructor(System.Type[] types) { return default(System.Reflection.ConstructorInfo); }
    public new System.Reflection.ConstructorInfo[] GetConstructors() { return default(System.Reflection.ConstructorInfo[]); }
    public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.ConstructorInfo[]); }
    public virtual System.Reflection.EventInfo GetDeclaredEvent(string name) { return default(System.Reflection.EventInfo); }
    public virtual System.Reflection.FieldInfo GetDeclaredField(string name) { return default(System.Reflection.FieldInfo); }
    public virtual System.Reflection.MethodInfo GetDeclaredMethod(string name) { return default(System.Reflection.MethodInfo); }
    [System.Diagnostics.DebuggerHiddenAttribute]
    public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> GetDeclaredMethods(string name) { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); }
    public virtual System.Reflection.TypeInfo GetDeclaredNestedType(string name) { return default(System.Reflection.TypeInfo); }
    public virtual System.Reflection.PropertyInfo GetDeclaredProperty(string name) { return default(System.Reflection.PropertyInfo); }
    public virtual new System.Reflection.MemberInfo[] GetDefaultMembers() { return default(System.Reflection.MemberInfo[]); }
    public override System.Type GetElementType() { return default(System.Type); }
    public virtual new string GetEnumName(object value) { return default(string); }
    public virtual new string[] GetEnumNames() { return default(string[]); }
    public virtual new System.Type GetEnumUnderlyingType() { return default(System.Type); }
    public virtual new System.Array GetEnumValues() { return default(System.Array); }
    public new System.Reflection.EventInfo GetEvent(string name) { return default(System.Reflection.EventInfo); }
    public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.EventInfo); }
    public virtual new System.Reflection.EventInfo[] GetEvents() { return default(System.Reflection.EventInfo[]); }
    public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.EventInfo[]); }
    public new System.Reflection.FieldInfo GetField(string name) { return default(System.Reflection.FieldInfo); }
    public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.FieldInfo); }
    public new System.Reflection.FieldInfo[] GetFields() { return default(System.Reflection.FieldInfo[]); }
    public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.FieldInfo[]); }
    public virtual new System.Type[] GetGenericArguments() { return default(System.Type[]); }
    public abstract new System.Type[] GetGenericParameterConstraints();
    public abstract new System.Type GetGenericTypeDefinition();
    public new System.Type GetInterface(string name) { return default(System.Type); }
    public override System.Type GetInterface(string name, bool ignoreCase) { return default(System.Type); }
    public override System.Type[] GetInterfaces() { return default(System.Type[]); }
    public new System.Reflection.MemberInfo[] GetMember(string name) { return default(System.Reflection.MemberInfo[]); }
    public virtual new System.Reflection.MemberInfo[] GetMember(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MemberInfo[]); }
    public virtual new System.Reflection.MemberInfo[] GetMember(string name, System.Reflection.MemberTypes type, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MemberInfo[]); }
    public new System.Reflection.MemberInfo[] GetMembers() { return default(System.Reflection.MemberInfo[]); }
    public override System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MemberInfo[]); }
    public new System.Reflection.MethodInfo GetMethod(string name) { return default(System.Reflection.MethodInfo); }
    public new System.Reflection.MethodInfo GetMethod(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MethodInfo); }
    public new System.Reflection.MethodInfo GetMethod(string name, System.Type[] types) { return default(System.Reflection.MethodInfo); }
    public new System.Reflection.MethodInfo GetMethod(string name, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.MethodInfo); }
    public new System.Reflection.MethodInfo[] GetMethods() { return default(System.Reflection.MethodInfo[]); }
    public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.MethodInfo[]); }
    public new System.Type GetNestedType(string name) { return default(System.Type); }
    public override System.Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Type); }
    public new System.Type[] GetNestedTypes() { return default(System.Type[]); }
    public override System.Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr) { return default(System.Type[]); }
    public new System.Reflection.PropertyInfo[] GetProperties() { return default(System.Reflection.PropertyInfo[]); }
    public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.PropertyInfo[]); }
    public new System.Reflection.PropertyInfo GetProperty(string name) { return default(System.Reflection.PropertyInfo); }
    public new System.Reflection.PropertyInfo GetProperty(string name, System.Reflection.BindingFlags bindingAttr) { return default(System.Reflection.PropertyInfo); }
    public new System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType) { return default(System.Reflection.PropertyInfo); }
    public new System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType, System.Type[] types) { return default(System.Reflection.PropertyInfo); }
    public new System.Reflection.PropertyInfo GetProperty(string name, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) { return default(System.Reflection.PropertyInfo); }
    public new System.Reflection.PropertyInfo GetProperty(string name, System.Type[] types) { return default(System.Reflection.PropertyInfo); }
    public virtual bool IsAssignableFrom(System.Reflection.TypeInfo typeInfo) { return default(bool); }
    public virtual new bool IsAssignableFrom(System.Type c) { return default(bool); }
    public virtual new bool IsEnumDefined(object value) { return default(bool); }
    public virtual new bool IsEquivalentTo(System.Type other) { return default(bool); }
    public virtual new bool IsInstanceOfType(object o) { return default(bool); }
    public virtual new bool IsSubclassOf(System.Type c) { return default(bool); }
    public abstract new System.Type MakeArrayType();
    public abstract new System.Type MakeArrayType(int rank);
    public abstract new System.Type MakeByRefType();
    public abstract new System.Type MakeGenericType(params System.Type[] typeArguments);
    public abstract new System.Type MakePointerType();
    System.Reflection.TypeInfo System.Reflection.IReflectableType.GetTypeInfo() { return default(System.Reflection.TypeInfo); }
  }
}
namespace System.Runtime {
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class AssemblyTargetedPatchBandAttribute : System.Attribute {
    public AssemblyTargetedPatchBandAttribute(string targetedPatchBand) { }
    public string TargetedPatchBand { get { return default(string); } }
  }
  public enum GCLargeObjectHeapCompactionMode {
    CompactOnce = 2,
    Default = 1,
  }
  public enum GCLatencyMode {
    Batch = 0,
    Interactive = 1,
    LowLatency = 2,
    SustainedLowLatency = 3,
  }
  public static partial class GCSettings {
    public static bool IsServerGC { get { return default(bool); } }
    public static System.Runtime.GCLargeObjectHeapCompactionMode LargeObjectHeapCompactionMode { [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(System.Runtime.GCLargeObjectHeapCompactionMode); } [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]set { } }
    public static System.Runtime.GCLatencyMode LatencyMode { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(System.Runtime.GCLatencyMode); } [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]set { } }
  }
  public sealed partial class MemoryFailPoint : System.Runtime.ConstrainedExecution.CriticalFinalizerObject, System.IDisposable {
    public MemoryFailPoint(int sizeInMegabytes) { }
    public void Dispose() { }
    ~MemoryFailPoint() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(96), AllowMultiple=false, Inherited=false)]
  public sealed partial class TargetedPatchingOptOutAttribute : System.Attribute {
    public TargetedPatchingOptOutAttribute(string reason) { }
    public string Reason { get { return default(string); } }
  }
}
namespace System.Runtime.CompilerServices {
  [System.AttributeUsageAttribute((System.AttributeTargets)(256))]
  public sealed partial class AccessedThroughPropertyAttribute : System.Attribute {
    public AccessedThroughPropertyAttribute(string propertyName) { }
    public string PropertyName { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false, AllowMultiple=false)]
  public sealed partial class AsyncStateMachineAttribute : System.Runtime.CompilerServices.StateMachineAttribute {
    public AsyncStateMachineAttribute(System.Type stateMachineType) : base (default(System.Type)) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct AsyncTaskMethodBuilder {
    public System.Threading.Tasks.Task Task { get { return default(System.Threading.Tasks.Task); } }
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    public static System.Runtime.CompilerServices.AsyncTaskMethodBuilder Create() { return default(System.Runtime.CompilerServices.AsyncTaskMethodBuilder); }
    public void SetException(System.Exception exception) { }
    public void SetResult() { }
    public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) { }
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct AsyncTaskMethodBuilder<TResult> {
    public System.Threading.Tasks.Task<TResult> Task { get { return default(System.Threading.Tasks.Task<TResult>); } }
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    [System.Security.SecuritySafeCriticalAttribute]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    public static System.Runtime.CompilerServices.AsyncTaskMethodBuilder<TResult> Create() { return default(System.Runtime.CompilerServices.AsyncTaskMethodBuilder<TResult>); }
    public void SetException(System.Exception exception) { }
    public void SetResult(TResult result) { }
    public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) { }
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct AsyncVoidMethodBuilder {
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    [System.Security.SecuritySafeCriticalAttribute]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
    public static System.Runtime.CompilerServices.AsyncVoidMethodBuilder Create() { return default(System.Runtime.CompilerServices.AsyncVoidMethodBuilder); }
    public void SetException(System.Exception exception) { }
    public void SetResult() { }
    public void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) { }
    [System.Diagnostics.DebuggerStepThroughAttribute]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine { }
  }
  public partial class CallConvCdecl {
    public CallConvCdecl() { }
  }
  public partial class CallConvFastcall {
    public CallConvFastcall() { }
  }
  public partial class CallConvStdcall {
    public CallConvStdcall() { }
  }
  public partial class CallConvThiscall {
    public CallConvThiscall() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited=false)]
  public sealed partial class CallerFilePathAttribute : System.Attribute {
    public CallerFilePathAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited=false)]
  public sealed partial class CallerLineNumberAttribute : System.Attribute {
    public CallerLineNumberAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited=false)]
  public sealed partial class CallerMemberNameAttribute : System.Attribute {
    public CallerMemberNameAttribute() { }
  }
  [System.FlagsAttribute]
  public enum CompilationRelaxations {
    NoStringInterning = 8,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(71))]
  public partial class CompilationRelaxationsAttribute : System.Attribute {
    public CompilationRelaxationsAttribute(int relaxations) { }
    public CompilationRelaxationsAttribute(System.Runtime.CompilerServices.CompilationRelaxations relaxations) { }
    public int CompilationRelaxations { get { return default(int); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(32767), Inherited=true)]
  public sealed partial class CompilerGeneratedAttribute : System.Attribute {
    public CompilerGeneratedAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(4))]
  public partial class CompilerGlobalScopeAttribute : System.Attribute {
    public CompilerGlobalScopeAttribute() { }
  }
  public static partial class CompilerMarshalOverride {
  }
  public sealed partial class ConditionalWeakTable<TKey, TValue> where TKey : class where TValue : class {
    public ConditionalWeakTable() { }
    public void Add(TKey key, TValue value) { }
    ~ConditionalWeakTable() { }
    public TValue GetOrCreateValue(TKey key) { return default(TValue); }
    public TValue GetValue(TKey key, System.Runtime.CompilerServices.ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback) { return default(TValue); }
    public bool Remove(TKey key) { return default(bool); }
    public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
    public delegate TValue CreateValueCallback(TKey key);
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct ConfiguredTaskAwaitable {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter() { return default(System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter); }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct ConfiguredTaskAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion {
      public bool IsCompleted { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
      [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
      public void GetResult() { }
      [System.Security.SecuritySafeCriticalAttribute]
      public void OnCompleted(System.Action continuation) { }
      [System.Security.SecurityCriticalAttribute]
      public void UnsafeOnCompleted(System.Action continuation) { }
    }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct ConfiguredTaskAwaitable<TResult> {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Runtime.CompilerServices.ConfiguredTaskAwaitable<TResult>.ConfiguredTaskAwaiter GetAwaiter() { return default(System.Runtime.CompilerServices.ConfiguredTaskAwaitable<TResult>.ConfiguredTaskAwaiter); }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct ConfiguredTaskAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion {
      public bool IsCompleted { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
      [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
      public TResult GetResult() { return default(TResult); }
      [System.Security.SecuritySafeCriticalAttribute]
      public void OnCompleted(System.Action continuation) { }
      [System.Security.SecurityCriticalAttribute]
      public void UnsafeOnCompleted(System.Action continuation) { }
    }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2304), Inherited=false)]
  public abstract partial class CustomConstantAttribute : System.Attribute {
    protected CustomConstantAttribute() { }
    public abstract object Value { get; }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2304), Inherited=false)]
  public sealed partial class DateTimeConstantAttribute : System.Runtime.CompilerServices.CustomConstantAttribute {
    public DateTimeConstantAttribute(long ticks) { }
    public override object Value { get { return default(object); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2304), Inherited=false)]
  public sealed partial class DecimalConstantAttribute : System.Attribute {
    public DecimalConstantAttribute(byte scale, byte sign, int hi, int mid, int low) { }
    [System.CLSCompliantAttribute(false)]
    public DecimalConstantAttribute(byte scale, byte sign, uint hi, uint mid, uint low) { }
    public decimal Value { get { return default(decimal); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1))]
  public sealed partial class DefaultDependencyAttribute : System.Attribute {
    public DefaultDependencyAttribute(System.Runtime.CompilerServices.LoadHint loadHintArgument) { }
    public System.Runtime.CompilerServices.LoadHint LoadHint { get { return default(System.Runtime.CompilerServices.LoadHint); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=true)]
  public sealed partial class DependencyAttribute : System.Attribute {
    public DependencyAttribute(string dependentAssemblyArgument, System.Runtime.CompilerServices.LoadHint loadHintArgument) { }
    public string DependentAssembly { get { return default(string); } }
    public System.Runtime.CompilerServices.LoadHint LoadHint { get { return default(System.Runtime.CompilerServices.LoadHint); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false, Inherited=false)]
  public sealed partial class DisablePrivateReflectionAttribute : System.Attribute {
    public DisablePrivateReflectionAttribute() { }
  }
  public partial class DiscardableAttribute : System.Attribute {
    public DiscardableAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(69))]
  public sealed partial class ExtensionAttribute : System.Attribute {
    public ExtensionAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256))]
  public sealed partial class FixedAddressValueTypeAttribute : System.Attribute {
    public FixedAddressValueTypeAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public sealed partial class FixedBufferAttribute : System.Attribute {
    public FixedBufferAttribute(System.Type elementType, int length) { }
    public System.Type ElementType { get { return default(System.Type); } }
    public int Length { get { return default(int); } }
  }
  public static partial class FormattableStringFactory {
    public static System.FormattableString Create(string format, params object[] arguments) { return default(System.FormattableString); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(8))]
  public sealed partial class HasCopySemanticsAttribute : System.Attribute {
    public HasCopySemanticsAttribute() { }
  }
  public partial interface IAsyncStateMachine {
    void MoveNext();
    void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine);
  }
  public partial interface ICriticalNotifyCompletion : System.Runtime.CompilerServices.INotifyCompletion {
    [System.Security.SecurityCriticalAttribute]
    void UnsafeOnCompleted(System.Action continuation);
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(128), Inherited=true)]
  public sealed partial class IndexerNameAttribute : System.Attribute {
    public IndexerNameAttribute(string indexerName) { }
  }
  public partial interface INotifyCompletion {
    void OnCompleted(System.Action continuation);
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=true, Inherited=false)]
  public sealed partial class InternalsVisibleToAttribute : System.Attribute {
    public InternalsVisibleToAttribute(string assemblyName) { }
    public bool AllInternalsVisible { get { return default(bool); } set { } }
    public string AssemblyName { get { return default(string); } }
  }
  public static partial class IsBoxed {
  }
  public static partial class IsByValue {
  }
  public static partial class IsConst {
  }
  public static partial class IsCopyConstructed {
  }
  public static partial class IsExplicitlyDereferenced {
  }
  public static partial class IsImplicitlyDereferenced {
  }
  public static partial class IsJitIntrinsic {
  }
  public static partial class IsLong {
  }
  public static partial class IsPinned {
  }
  public static partial class IsSignUnspecifiedByte {
  }
  public partial interface IStrongBox {
    object Value { get; set; }
  }
  public static partial class IsUdtReturn {
  }
  public static partial class IsVolatile {
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false, AllowMultiple=false)]
  public sealed partial class IteratorStateMachineAttribute : System.Runtime.CompilerServices.StateMachineAttribute {
    public IteratorStateMachineAttribute(System.Type stateMachineType) : base (default(System.Type)) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2304), Inherited=false)]
  public sealed partial class IUnknownConstantAttribute : System.Runtime.CompilerServices.CustomConstantAttribute {
    public IUnknownConstantAttribute() { }
    public override object Value { get { return default(object); } }
  }
  public enum LoadHint {
    Always = 1,
    Default = 0,
    Sometimes = 2,
  }
  public enum MethodCodeType {
    IL = 0,
    Native = 1,
    OPTIL = 2,
    Runtime = 3,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(96), Inherited=false)]
  public sealed partial class MethodImplAttribute : System.Attribute {
    public System.Runtime.CompilerServices.MethodCodeType MethodCodeType;
    public MethodImplAttribute() { }
    public MethodImplAttribute(short value) { }
    public MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions methodImplOptions) { }
    public System.Runtime.CompilerServices.MethodImplOptions Value { get { return default(System.Runtime.CompilerServices.MethodImplOptions); } }
  }
  [System.FlagsAttribute]
  public enum MethodImplOptions {
    AggressiveInlining = 256,
    ForwardRef = 16,
    InternalCall = 4096,
    NoInlining = 8,
    NoOptimization = 64,
    PreserveSig = 128,
    Synchronized = 32,
    Unmanaged = 4,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(8), Inherited=true)]
  public sealed partial class NativeCppClassAttribute : System.Attribute {
    public NativeCppClassAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false)]
  public sealed partial class ReferenceAssemblyAttribute : System.Attribute {
    public ReferenceAssemblyAttribute() { }
    public ReferenceAssemblyAttribute(string description) { }
    public string Description { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1052), AllowMultiple=true, Inherited=false)]
  public sealed partial class RequiredAttributeAttribute : System.Attribute {
    public RequiredAttributeAttribute(System.Type requiredContract) { }
    public System.Type RequiredContract { get { return default(System.Type); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false, AllowMultiple=false)]
  public sealed partial class RuntimeCompatibilityAttribute : System.Attribute {
    public RuntimeCompatibilityAttribute() { }
    public bool WrapNonExceptionThrows { get { return default(bool); } set { } }
  }
  public static partial class RuntimeHelpers {
    public static int OffsetToStringData { get { return default(int); } }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public static void EnsureSufficientExecutionStack() { }
    public static new bool Equals(object o1, object o2) { return default(bool); }
    public static void ExecuteCodeWithGuaranteedCleanup(System.Runtime.CompilerServices.RuntimeHelpers.TryCode code, System.Runtime.CompilerServices.RuntimeHelpers.CleanupCode backoutCode, object userData) { }
    public static int GetHashCode(object o) { return default(int); }
    public static object GetObjectValue(object obj) { return default(object); }
    public static void InitializeArray(System.Array array, System.RuntimeFieldHandle fldHandle) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void PrepareConstrainedRegions() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void PrepareConstrainedRegionsNoOP() { }
    [System.Security.SecurityCriticalAttribute]
    public static void PrepareContractedDelegate(System.Delegate d) { }
    [System.Security.SecurityCriticalAttribute]
    public static void PrepareDelegate(System.Delegate d) { }
    public static void PrepareMethod(System.RuntimeMethodHandle method) { }
    public static void PrepareMethod(System.RuntimeMethodHandle method, System.RuntimeTypeHandle[] instantiation) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static void ProbeForSufficientStack() { }
    public static void RunClassConstructor(System.RuntimeTypeHandle type) { }
    public static void RunModuleConstructor(System.ModuleHandle module) { }
    public delegate void CleanupCode(object userData, bool exceptionThrown);
    public delegate void TryCode(object userData);
  }
  public sealed partial class RuntimeWrappedException : System.Exception {
    internal RuntimeWrappedException() { }
    public object WrappedException { get { return default(object); } }
    [System.Security.SecurityCriticalAttribute]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(16))]
  public sealed partial class ScopelessEnumAttribute : System.Attribute {
    public ScopelessEnumAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(972))]
  public sealed partial class SpecialNameAttribute : System.Attribute {
    public SpecialNameAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false, AllowMultiple=false)]
  public partial class StateMachineAttribute : System.Attribute {
    public StateMachineAttribute(System.Type stateMachineType) { }
    public System.Type StateMachineType { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { return default(System.Type); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), Inherited=false)]
  public sealed partial class StringFreezingAttribute : System.Attribute {
    public StringFreezingAttribute() { }
  }
  public partial class StrongBox<T> : System.Runtime.CompilerServices.IStrongBox {
    public T Value;
    public StrongBox() { }
    public StrongBox(T value) { }
    object System.Runtime.CompilerServices.IStrongBox.Value { get { return default(object); } set { } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(3))]
  public sealed partial class SuppressIldasmAttribute : System.Attribute {
    public SuppressIldasmAttribute() { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct TaskAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion {
    public bool IsCompleted { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public void GetResult() { }
    [System.Security.SecuritySafeCriticalAttribute]
    public void OnCompleted(System.Action continuation) { }
    [System.Security.SecurityCriticalAttribute]
    public void UnsafeOnCompleted(System.Action continuation) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct TaskAwaiter<TResult> : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion {
    public bool IsCompleted { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public TResult GetResult() { return default(TResult); }
    [System.Security.SecuritySafeCriticalAttribute]
    public void OnCompleted(System.Action continuation) { }
    [System.Security.SecurityCriticalAttribute]
    public void UnsafeOnCompleted(System.Action continuation) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5148), Inherited=false, AllowMultiple=false)]
  public sealed partial class TypeForwardedFromAttribute : System.Attribute {
    public TypeForwardedFromAttribute(string assemblyFullName) { }
    public string AssemblyFullName { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=true, Inherited=false)]
  public sealed partial class TypeForwardedToAttribute : System.Attribute {
    public TypeForwardedToAttribute(System.Type destination) { }
    public System.Type Destination { get { return default(System.Type); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(8))]
  public sealed partial class UnsafeValueTypeAttribute : System.Attribute {
    public UnsafeValueTypeAttribute() { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
  public partial struct YieldAwaitable {
    public System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter GetAwaiter() { return default(System.Runtime.CompilerServices.YieldAwaitable.YieldAwaiter); }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Size=1)]
    public partial struct YieldAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion {
      public bool IsCompleted { get { return default(bool); } }
      public void GetResult() { }
      [System.Security.SecuritySafeCriticalAttribute]
      public void OnCompleted(System.Action continuation) { }
      [System.Security.SecurityCriticalAttribute]
      public void UnsafeOnCompleted(System.Action continuation) { }
    }
  }
}
namespace System.Runtime.ConstrainedExecution {
  public enum Cer {
    MayFail = 1,
    None = 0,
    Success = 2,
  }
  public enum Consistency {
    MayCorruptAppDomain = 1,
    MayCorruptInstance = 2,
    MayCorruptProcess = 0,
    WillNotCorruptState = 3,
  }
  public abstract partial class CriticalFinalizerObject {
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    protected CriticalFinalizerObject() { }
    ~CriticalFinalizerObject() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(96), Inherited=false)]
  public sealed partial class PrePrepareMethodAttribute : System.Attribute {
    public PrePrepareMethodAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1133), Inherited=false)]
  public sealed partial class ReliabilityContractAttribute : System.Attribute {
    public ReliabilityContractAttribute(System.Runtime.ConstrainedExecution.Consistency consistencyGuarantee, System.Runtime.ConstrainedExecution.Cer cer) { }
    public System.Runtime.ConstrainedExecution.Cer Cer { get { return default(System.Runtime.ConstrainedExecution.Cer); } }
    public System.Runtime.ConstrainedExecution.Consistency ConsistencyGuarantee { get { return default(System.Runtime.ConstrainedExecution.Consistency); } }
  }
}
namespace System.Runtime.ExceptionServices {
  public sealed partial class ExceptionDispatchInfo {
    internal ExceptionDispatchInfo() { }
    public System.Exception SourceException { get { return default(System.Exception); } }
    public static System.Runtime.ExceptionServices.ExceptionDispatchInfo Capture(System.Exception source) { return default(System.Runtime.ExceptionServices.ExceptionDispatchInfo); }
    public void Throw() { }
  }
  public partial class FirstChanceExceptionEventArgs : System.EventArgs {
    public FirstChanceExceptionEventArgs(System.Exception exception) { }
    public System.Exception Exception { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get { return default(System.Exception); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple=false, Inherited=false)]
  public sealed partial class HandleProcessCorruptedStateExceptionsAttribute : System.Attribute {
    public HandleProcessCorruptedStateExceptionsAttribute() { }
  }
}
namespace System.Runtime.InteropServices {
  public enum CharSet {
    Ansi = 2,
    Auto = 4,
    None = 1,
    Unicode = 3,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5), Inherited=false)]
  public sealed partial class ClassInterfaceAttribute : System.Attribute {
    public ClassInterfaceAttribute(short classInterfaceType) { }
    public ClassInterfaceAttribute(System.Runtime.InteropServices.ClassInterfaceType classInterfaceType) { }
    public System.Runtime.InteropServices.ClassInterfaceType Value { get { return default(System.Runtime.InteropServices.ClassInterfaceType); } }
  }
  public enum ClassInterfaceType {
    AutoDispatch = 1,
    AutoDual = 2,
    None = 0,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(4), Inherited=false)]
  public sealed partial class ComDefaultInterfaceAttribute : System.Attribute {
    public ComDefaultInterfaceAttribute(System.Type defaultInterface) { }
    public System.Type Value { get { return default(System.Type); } }
  }
  public enum ComInterfaceType {
    InterfaceIsDual = 0,
    InterfaceIsIDispatch = 2,
    InterfaceIsIUnknown = 1,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5597), Inherited=false)]
  public sealed partial class ComVisibleAttribute : System.Attribute {
    public ComVisibleAttribute(bool visibility) { }
    public bool Value { get { return default(bool); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(960), Inherited=false)]
  public sealed partial class DispIdAttribute : System.Attribute {
    public DispIdAttribute(int dispId) { }
    public int Value { get { return default(int); } }
  }
  public partial class ExternalException : System.SystemException {
    public ExternalException() { }
    protected ExternalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public ExternalException(string message) { }
    public ExternalException(string message, System.Exception inner) { }
    public ExternalException(string message, int errorCode) { }
    public virtual int ErrorCode { get { return default(int); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public sealed partial class FieldOffsetAttribute : System.Attribute {
    public FieldOffsetAttribute(int offset) { }
    public int Value { get { return default(int); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5149), Inherited=false)]
  public sealed partial class GuidAttribute : System.Attribute {
    public GuidAttribute(string guid) { }
    public string Value { get { return default(string); } }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1024), Inherited=false)]
  public sealed partial class InterfaceTypeAttribute : System.Attribute {
    public InterfaceTypeAttribute(short interfaceType) { }
    public InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType interfaceType) { }
    public System.Runtime.InteropServices.ComInterfaceType Value { get { return default(System.Runtime.InteropServices.ComInterfaceType); } }
  }
  public enum LayoutKind {
    Auto = 3,
    Explicit = 2,
    Sequential = 0,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited=false)]
  public sealed partial class OutAttribute : System.Attribute {
    public OutAttribute() { }
  }
  public abstract partial class SafeBuffer : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid, System.IDisposable {
    protected SafeBuffer(bool ownsHandle) : base (default(bool)) { }
    [System.CLSCompliantAttribute(false)]
    public ulong ByteLength { get { return default(ulong); } }
    public override bool IsInvalid { get { return default(bool); } }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public unsafe void AcquirePointer(ref byte* pointer) { }
    [System.CLSCompliantAttribute(false)]
    public void Initialize(uint numElements, uint sizeOfEachElement) { }
    [System.CLSCompliantAttribute(false)]
    public void Initialize(ulong numBytes) { }
    [System.CLSCompliantAttribute(false)]
    public void Initialize<T>(uint numElements) where T : struct { }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public T Read<T>(ulong byteOffset) where T : struct { return default(T); }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public void ReadArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public void ReleasePointer() { }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public void Write<T>(ulong byteOffset, T value) where T : struct { }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public void WriteArray<T>(ulong byteOffset, T[] array, int index, int count) where T : struct { }
  }
  [System.Security.SecurityCriticalAttribute]
  public abstract partial class SafeHandle : System.Runtime.ConstrainedExecution.CriticalFinalizerObject, System.IDisposable {
    protected System.IntPtr handle;
    protected SafeHandle() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    protected SafeHandle(System.IntPtr invalidHandleValue, bool ownsHandle) { }
    public bool IsClosed { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2)), System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public abstract bool IsInvalid { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]get; }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecurityCriticalAttribute]
    public void Close() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public void DangerousAddRef(ref bool success) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.IntPtr DangerousGetHandle() { return default(System.IntPtr); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public void DangerousRelease() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecuritySafeCriticalAttribute]
    public void Dispose() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Security.SecurityCriticalAttribute]
    protected virtual void Dispose(bool disposing) { }
    ~SafeHandle() { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    protected abstract bool ReleaseHandle();
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    protected void SetHandle(System.IntPtr handle) { }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]
    public void SetHandleAsInvalid() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(12), Inherited=false)]
  public sealed partial class StructLayoutAttribute : System.Attribute {
    public System.Runtime.InteropServices.CharSet CharSet;
    public int Pack;
    public int Size;
    public StructLayoutAttribute(short layoutKind) { }
    public StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind layoutKind) { }
    public System.Runtime.InteropServices.LayoutKind Value { get { return default(System.Runtime.InteropServices.LayoutKind); } }
  }
}
namespace System.Runtime.Serialization {
  public partial interface IDeserializationCallback {
    void OnDeserialization(object sender);
  }
  [System.CLSCompliantAttribute(false)]
  public partial interface IFormatterConverter {
    object Convert(object value, System.Type type);
    object Convert(object value, System.TypeCode typeCode);
    bool ToBoolean(object value);
    byte ToByte(object value);
    char ToChar(object value);
    System.DateTime ToDateTime(object value);
    decimal ToDecimal(object value);
    double ToDouble(object value);
    short ToInt16(object value);
    int ToInt32(object value);
    long ToInt64(object value);
    sbyte ToSByte(object value);
    float ToSingle(object value);
    string ToString(object value);
    ushort ToUInt16(object value);
    uint ToUInt32(object value);
    ulong ToUInt64(object value);
  }
  public partial interface IObjectReference {
    [System.Security.SecurityCriticalAttribute]
    object GetRealObject(System.Runtime.Serialization.StreamingContext context);
  }
  public partial interface ISafeSerializationData {
    void CompleteDeserialization(object deserialized);
  }
  public partial interface ISerializable {
    [System.Security.SecurityCriticalAttribute]
    void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false)]
  public sealed partial class OnDeserializedAttribute : System.Attribute {
    public OnDeserializedAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false)]
  public sealed partial class OnDeserializingAttribute : System.Attribute {
    public OnDeserializingAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false)]
  public sealed partial class OnSerializedAttribute : System.Attribute {
    public OnSerializedAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(64), Inherited=false)]
  public sealed partial class OnSerializingAttribute : System.Attribute {
    public OnSerializingAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(256), Inherited=false)]
  public sealed partial class OptionalFieldAttribute : System.Attribute {
    public OptionalFieldAttribute() { }
    public int VersionAdded { get { return default(int); } set { } }
  }
  public sealed partial class SafeSerializationEventArgs : System.EventArgs {
    internal SafeSerializationEventArgs() { }
    public System.Runtime.Serialization.StreamingContext StreamingContext { get { return default(System.Runtime.Serialization.StreamingContext); } }
    public void AddSerializedState(System.Runtime.Serialization.ISafeSerializationData serializedState) { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct SerializationEntry {
    public string Name { get { return default(string); } }
    public System.Type ObjectType { get { return default(System.Type); } }
    public object Value { get { return default(object); } }
  }
  public sealed partial class SerializationInfo {
    [System.CLSCompliantAttribute(false)]
    public SerializationInfo(System.Type type, System.Runtime.Serialization.IFormatterConverter converter) { }
    [System.CLSCompliantAttribute(false)]
    public SerializationInfo(System.Type type, System.Runtime.Serialization.IFormatterConverter converter, bool requireSameTokenInPartialTrust) { }
    public string AssemblyName { get { return default(string); } [System.Security.SecuritySafeCriticalAttribute]set { } }
    public string FullTypeName { get { return default(string); } set { } }
    public bool IsAssemblyNameSetExplicit { get { return default(bool); } }
    public bool IsFullTypeNameSetExplicit { get { return default(bool); } }
    public int MemberCount { get { return default(int); } }
    public System.Type ObjectType { get { return default(System.Type); } }
    public void AddValue(string name, bool value) { }
    public void AddValue(string name, byte value) { }
    public void AddValue(string name, char value) { }
    public void AddValue(string name, System.DateTime value) { }
    public void AddValue(string name, decimal value) { }
    public void AddValue(string name, double value) { }
    public void AddValue(string name, short value) { }
    public void AddValue(string name, int value) { }
    public void AddValue(string name, long value) { }
    public void AddValue(string name, object value) { }
    public void AddValue(string name, object value, System.Type type) { }
    [System.CLSCompliantAttribute(false)]
    public void AddValue(string name, sbyte value) { }
    public void AddValue(string name, float value) { }
    [System.CLSCompliantAttribute(false)]
    public void AddValue(string name, ushort value) { }
    [System.CLSCompliantAttribute(false)]
    public void AddValue(string name, uint value) { }
    [System.CLSCompliantAttribute(false)]
    public void AddValue(string name, ulong value) { }
    public bool GetBoolean(string name) { return default(bool); }
    public byte GetByte(string name) { return default(byte); }
    public char GetChar(string name) { return default(char); }
    public System.DateTime GetDateTime(string name) { return default(System.DateTime); }
    public decimal GetDecimal(string name) { return default(decimal); }
    public double GetDouble(string name) { return default(double); }
    public System.Runtime.Serialization.SerializationInfoEnumerator GetEnumerator() { return default(System.Runtime.Serialization.SerializationInfoEnumerator); }
    public short GetInt16(string name) { return default(short); }
    public int GetInt32(string name) { return default(int); }
    public long GetInt64(string name) { return default(long); }
    [System.CLSCompliantAttribute(false)]
    public sbyte GetSByte(string name) { return default(sbyte); }
    public float GetSingle(string name) { return default(float); }
    public string GetString(string name) { return default(string); }
    [System.CLSCompliantAttribute(false)]
    public ushort GetUInt16(string name) { return default(ushort); }
    [System.CLSCompliantAttribute(false)]
    public uint GetUInt32(string name) { return default(uint); }
    [System.CLSCompliantAttribute(false)]
    public ulong GetUInt64(string name) { return default(ulong); }
    [System.Security.SecuritySafeCriticalAttribute]
    public object GetValue(string name, System.Type type) { return default(object); }
    [System.Security.SecuritySafeCriticalAttribute]
    public void SetType(System.Type type) { }
  }
  public sealed partial class SerializationInfoEnumerator : System.Collections.IEnumerator {
    internal SerializationInfoEnumerator() { }
    public System.Runtime.Serialization.SerializationEntry Current { get { return default(System.Runtime.Serialization.SerializationEntry); } }
    public string Name { get { return default(string); } }
    public System.Type ObjectType { get { return default(System.Type); } }
    object System.Collections.IEnumerator.Current { get { return default(object); } }
    public object Value { get { return default(object); } }
    public bool MoveNext() { return default(bool); }
    public void Reset() { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct StreamingContext {
    public StreamingContext(System.Runtime.Serialization.StreamingContextStates state) { throw new System.NotImplementedException(); }
    public StreamingContext(System.Runtime.Serialization.StreamingContextStates state, object additional) { throw new System.NotImplementedException(); }
    public object Context { get { return default(object); } }
    public System.Runtime.Serialization.StreamingContextStates State { get { return default(System.Runtime.Serialization.StreamingContextStates); } }
    public override bool Equals(object obj) { return default(bool); }
    public override int GetHashCode() { return default(int); }
  }
  [System.FlagsAttribute]
  public enum StreamingContextStates {
    All = 255,
    Clone = 64,
    CrossAppDomain = 128,
    CrossMachine = 2,
    CrossProcess = 1,
    File = 4,
    Other = 32,
    Persistence = 8,
    Remoting = 16,
  }
}
namespace System.Runtime.Versioning {
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false, Inherited=false)]
  public sealed partial class TargetFrameworkAttribute : System.Attribute {
    public TargetFrameworkAttribute(string frameworkName) { }
    public string FrameworkDisplayName { get { return default(string); } set { } }
    public string FrameworkName { get { return default(string); } }
  }
}
namespace System.Security {
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false, Inherited=false)]
  public sealed partial class AllowPartiallyTrustedCallersAttribute : System.Attribute {
    public AllowPartiallyTrustedCallersAttribute() { }
    public System.Security.PartialTrustVisibilityLevel PartialTrustVisibilityLevel { get { return default(System.Security.PartialTrustVisibilityLevel); } set { } }
  }
  public enum PartialTrustVisibilityLevel {
    NotVisibleByDefault = 1,
    VisibleToAllHosts = 0,
  }
  public sealed partial class SecureString : System.IDisposable {
    public SecureString() { }
    [System.CLSCompliantAttribute(false)]
    public unsafe SecureString(char* value, int length) { }
    public int Length { get { return default(int); } }
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute]
    public void AppendChar(char c) { }
    public void Clear() { }
    public System.Security.SecureString Copy() { return default(System.Security.SecureString); }
    public void Dispose() { }
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute]
    public void InsertAt(int index, char c) { }
    public bool IsReadOnly() { return default(bool); }
    public void MakeReadOnly() { }
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute]
    public void RemoveAt(int index) { }
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute]
    public void SetAt(int index, char c) { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5501), AllowMultiple=false, Inherited=false)]
  public sealed partial class SecurityCriticalAttribute : System.Attribute {
    public SecurityCriticalAttribute() { }
  }
  public partial class SecurityException : System.SystemException {
    public SecurityException() { }
    protected SecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public SecurityException(string message) { }
    public SecurityException(string message, System.Exception inner) { }
    public SecurityException(string message, System.Type type) { }
    public SecurityException(string message, System.Type type, string state) { }
    public object Demanded { get { return default(object); } set { } }
    public object DenySetInstance { get { return default(object); } set { } }
    public System.Reflection.AssemblyName FailedAssemblyInfo { get { return default(System.Reflection.AssemblyName); } set { } }
    public string GrantedSet { get { return default(string); } set { } }
    public System.Reflection.MethodInfo Method { get { return default(System.Reflection.MethodInfo); } set { } }
    public string PermissionState { get { return default(string); } set { } }
    public System.Type PermissionType { get { return default(System.Type); } set { } }
    public object PermitOnlySetInstance { get { return default(object); } set { } }
    public string RefusedSet { get { return default(string); } set { } }
    public string Url { get { return default(string); } set { } }
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public override string ToString() { return default(string); }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false)]
  public sealed partial class SecurityRulesAttribute : System.Attribute {
    public SecurityRulesAttribute(System.Security.SecurityRuleSet ruleSet) { }
    public System.Security.SecurityRuleSet RuleSet { get { return default(System.Security.SecurityRuleSet); } }
    public bool SkipVerificationInFullTrust { get { return default(bool); } set { } }
  }
  public enum SecurityRuleSet : byte {
    Level1 = (byte)1,
    Level2 = (byte)2,
    None = (byte)0,
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5500), AllowMultiple=false, Inherited=false)]
  public sealed partial class SecuritySafeCriticalAttribute : System.Attribute {
    public SecuritySafeCriticalAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(1), AllowMultiple=false, Inherited=false)]
  public sealed partial class SecurityTransparentAttribute : System.Attribute {
    public SecurityTransparentAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(5501), AllowMultiple=false, Inherited=false)]
  [System.ObsoleteAttribute("SecurityTreatAsSafe is only used for .NET 2.0 transparency compatibility.  Please use the SecuritySafeCriticalAttribute instead.")]
  public sealed partial class SecurityTreatAsSafeAttribute : System.Attribute {
    public SecurityTreatAsSafeAttribute() { }
  }
  [System.AttributeUsageAttribute((System.AttributeTargets)(2), AllowMultiple=true, Inherited=false)]
  public sealed partial class UnverifiableCodeAttribute : System.Attribute {
    public UnverifiableCodeAttribute() { }
  }
  public partial class VerificationException : System.SystemException {
    public VerificationException() { }
    protected VerificationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public VerificationException(string message) { }
    public VerificationException(string message, System.Exception innerException) { }
  }
}
namespace System.Security.Policy {
  public sealed partial class Evidence : System.Collections.ICollection, System.Collections.IEnumerable {
    public Evidence() { }
    [System.ObsoleteAttribute]
    public Evidence(object[] hostEvidence, object[] assemblyEvidence) { }
    public Evidence(System.Security.Policy.Evidence evidence) { }
    [System.ObsoleteAttribute]
    public int Count { get { return default(int); } }
    public bool IsReadOnly { get { return default(bool); } }
    public bool IsSynchronized { get { return default(bool); } }
    public bool Locked { get { return default(bool); } set { } }
    public object SyncRoot { get { return default(object); } }
    [System.ObsoleteAttribute]
    public void AddAssembly(object id) { }
    [System.ObsoleteAttribute]
    public void AddHost(object id) { }
    public void Clear() { }
    public System.Security.Policy.Evidence Clone() { return default(System.Security.Policy.Evidence); }
    [System.ObsoleteAttribute]
    public void CopyTo(System.Array array, int index) { }
    public System.Collections.IEnumerator GetAssemblyEnumerator() { return default(System.Collections.IEnumerator); }
    [System.ObsoleteAttribute]
    public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
    public System.Collections.IEnumerator GetHostEnumerator() { return default(System.Collections.IEnumerator); }
    public void Merge(System.Security.Policy.Evidence evidence) { }
    public void RemoveType(System.Type t) { }
  }
  public abstract partial class EvidenceBase {
    protected EvidenceBase() { }
    public virtual System.Security.Policy.EvidenceBase Clone() { return default(System.Security.Policy.EvidenceBase); }
  }
}
namespace System.Text {
  public abstract partial class Decoder {
    protected Decoder() { }
    public System.Text.DecoderFallback Fallback { get { return default(System.Text.DecoderFallback); } set { } }
    public System.Text.DecoderFallbackBuffer FallbackBuffer { get { return default(System.Text.DecoderFallbackBuffer); } }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual void Convert(byte* bytes, int byteCount, char* chars, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed) { bytesUsed = default(int); charsUsed = default(int); completed = default(bool); }
    public virtual void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed) { bytesUsed = default(int); charsUsed = default(int); completed = default(bool); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetCharCount(byte* bytes, int count, bool flush) { return default(int); }
    public abstract int GetCharCount(byte[] bytes, int index, int count);
    public virtual int GetCharCount(byte[] bytes, int index, int count, bool flush) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetChars(byte* bytes, int byteCount, char* chars, int charCount, bool flush) { return default(int); }
    public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);
    public virtual int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush) { return default(int); }
    public virtual void Reset() { }
  }
  public abstract partial class DecoderFallback {
    protected DecoderFallback() { }
    public static System.Text.DecoderFallback ExceptionFallback { get { return default(System.Text.DecoderFallback); } }
    public abstract int MaxCharCount { get; }
    public static System.Text.DecoderFallback ReplacementFallback { get { return default(System.Text.DecoderFallback); } }
    public abstract System.Text.DecoderFallbackBuffer CreateFallbackBuffer();
  }
  public abstract partial class DecoderFallbackBuffer {
    protected DecoderFallbackBuffer() { }
    public abstract int Remaining { get; }
    public abstract bool Fallback(byte[] bytesUnknown, int index);
    public abstract char GetNextChar();
    public abstract bool MovePrevious();
    public virtual void Reset() { }
  }
  public abstract partial class Encoder {
    protected Encoder() { }
    public System.Text.EncoderFallback Fallback { get { return default(System.Text.EncoderFallback); } set { } }
    public System.Text.EncoderFallbackBuffer FallbackBuffer { get { return default(System.Text.EncoderFallbackBuffer); } }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual void Convert(char* chars, int charCount, byte* bytes, int byteCount, bool flush, out int charsUsed, out int bytesUsed, out bool completed) { charsUsed = default(int); bytesUsed = default(int); completed = default(bool); }
    public virtual void Convert(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, int byteCount, bool flush, out int charsUsed, out int bytesUsed, out bool completed) { charsUsed = default(int); bytesUsed = default(int); completed = default(bool); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetByteCount(char* chars, int count, bool flush) { return default(int); }
    public abstract int GetByteCount(char[] chars, int index, int count, bool flush);
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush) { return default(int); }
    public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush);
    public virtual void Reset() { }
  }
  public abstract partial class EncoderFallback {
    protected EncoderFallback() { }
    public static System.Text.EncoderFallback ExceptionFallback { get { return default(System.Text.EncoderFallback); } }
    public abstract int MaxCharCount { get; }
    public static System.Text.EncoderFallback ReplacementFallback { get { return default(System.Text.EncoderFallback); } }
    public abstract System.Text.EncoderFallbackBuffer CreateFallbackBuffer();
  }
  public abstract partial class EncoderFallbackBuffer {
    protected EncoderFallbackBuffer() { }
    public abstract int Remaining { get; }
    public abstract bool Fallback(char charUnknownHigh, char charUnknownLow, int index);
    public abstract bool Fallback(char charUnknown, int index);
    public abstract char GetNextChar();
    public abstract bool MovePrevious();
    public virtual void Reset() { }
  }
  public abstract partial class Encoding : System.ICloneable {
    protected Encoding() { }
    protected Encoding(int codePage) { }
    protected Encoding(int codePage, System.Text.EncoderFallback encoderFallback, System.Text.DecoderFallback decoderFallback) { }
    public static System.Text.Encoding ASCII { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Text.Encoding); } }
    public static System.Text.Encoding BigEndianUnicode { get { return default(System.Text.Encoding); } }
    public virtual string BodyName { get { return default(string); } }
    public virtual int CodePage { get { return default(int); } }
    public System.Text.DecoderFallback DecoderFallback { get { return default(System.Text.DecoderFallback); } set { } }
    public static System.Text.Encoding Default { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries"), System.Security.SecuritySafeCriticalAttribute]get { return default(System.Text.Encoding); } }
    public System.Text.EncoderFallback EncoderFallback { get { return default(System.Text.EncoderFallback); } set { } }
    public virtual string EncodingName { get { return default(string); } }
    public virtual string HeaderName { get { return default(string); } }
    public virtual bool IsBrowserDisplay { get { return default(bool); } }
    public virtual bool IsBrowserSave { get { return default(bool); } }
    public virtual bool IsMailNewsDisplay { get { return default(bool); } }
    public virtual bool IsMailNewsSave { get { return default(bool); } }
    public bool IsReadOnly { get { return default(bool); } }
    public virtual bool IsSingleByte { get { return default(bool); } }
    public static System.Text.Encoding Unicode { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Text.Encoding); } }
    public static System.Text.Encoding UTF32 { get { return default(System.Text.Encoding); } }
    public static System.Text.Encoding UTF7 { get { return default(System.Text.Encoding); } }
    public static System.Text.Encoding UTF8 { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Text.Encoding); } }
    public virtual string WebName { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(string); } }
    public virtual int WindowsCodePage { get { return default(int); } }
    public virtual object Clone() { return default(object); }
    public static byte[] Convert(System.Text.Encoding srcEncoding, System.Text.Encoding dstEncoding, byte[] bytes) { return default(byte[]); }
    public static byte[] Convert(System.Text.Encoding srcEncoding, System.Text.Encoding dstEncoding, byte[] bytes, int index, int count) { return default(byte[]); }
    public override bool Equals(object value) { return default(bool); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetByteCount(char* chars, int count) { return default(int); }
    public virtual int GetByteCount(char[] chars) { return default(int); }
    public abstract int GetByteCount(char[] chars, int index, int count);
    public virtual int GetByteCount(string s) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount) { return default(int); }
    public virtual byte[] GetBytes(char[] chars) { return default(byte[]); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public virtual byte[] GetBytes(char[] chars, int index, int count) { return default(byte[]); }
    public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex);
    public virtual byte[] GetBytes(string s) { return default(byte[]); }
    public virtual int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) { return default(int); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetCharCount(byte* bytes, int count) { return default(int); }
    public virtual int GetCharCount(byte[] bytes) { return default(int); }
    public abstract int GetCharCount(byte[] bytes, int index, int count);
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe virtual int GetChars(byte* bytes, int byteCount, char* chars, int charCount) { return default(int); }
    public virtual char[] GetChars(byte[] bytes) { return default(char[]); }
    public virtual char[] GetChars(byte[] bytes, int index, int count) { return default(char[]); }
    public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);
    public virtual System.Text.Decoder GetDecoder() { return default(System.Text.Decoder); }
    public virtual System.Text.Encoder GetEncoder() { return default(System.Text.Encoder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public static System.Text.Encoding GetEncoding(int codepage) { return default(System.Text.Encoding); }
    public static System.Text.Encoding GetEncoding(int codepage, System.Text.EncoderFallback encoderFallback, System.Text.DecoderFallback decoderFallback) { return default(System.Text.Encoding); }
    public static System.Text.Encoding GetEncoding(string name) { return default(System.Text.Encoding); }
    public static System.Text.Encoding GetEncoding(string name, System.Text.EncoderFallback encoderFallback, System.Text.DecoderFallback decoderFallback) { return default(System.Text.Encoding); }
    public static System.Text.EncodingInfo[] GetEncodings() { return default(System.Text.EncodingInfo[]); }
    public override int GetHashCode() { return default(int); }
    public abstract int GetMaxByteCount(int charCount);
    public abstract int GetMaxCharCount(int byteCount);
    public virtual byte[] GetPreamble() { return default(byte[]); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe string GetString(byte* bytes, int byteCount) { return default(string); }
    public virtual string GetString(byte[] bytes) { return default(string); }
    public virtual string GetString(byte[] bytes, int index, int count) { return default(string); }
    public bool IsAlwaysNormalized() { return default(bool); }
    public virtual bool IsAlwaysNormalized(System.Text.NormalizationForm form) { return default(bool); }
    [System.Security.SecurityCriticalAttribute]
    public static void RegisterProvider(System.Text.EncodingProvider provider) { }
  }
  public sealed partial class EncodingInfo {
    internal EncodingInfo() { }
    public int CodePage { get { return default(int); } }
    public string DisplayName { get { return default(string); } }
    public string Name { get { return default(string); } }
    public override bool Equals(object value) { return default(bool); }
    public System.Text.Encoding GetEncoding() { return default(System.Text.Encoding); }
    public override int GetHashCode() { return default(int); }
  }
  public abstract partial class EncodingProvider {
    public EncodingProvider() { }
    public abstract System.Text.Encoding GetEncoding(int codepage);
    public virtual System.Text.Encoding GetEncoding(int codepage, System.Text.EncoderFallback encoderFallback, System.Text.DecoderFallback decoderFallback) { return default(System.Text.Encoding); }
    public abstract System.Text.Encoding GetEncoding(string name);
    public virtual System.Text.Encoding GetEncoding(string name, System.Text.EncoderFallback encoderFallback, System.Text.DecoderFallback decoderFallback) { return default(System.Text.Encoding); }
  }
  public enum NormalizationForm {
    FormC = 1,
    FormD = 2,
    FormKC = 5,
    FormKD = 6,
  }
  public sealed partial class StringBuilder : System.Runtime.Serialization.ISerializable {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public StringBuilder() { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public StringBuilder(int capacity) { }
    public StringBuilder(int capacity, int maxCapacity) { }
    public StringBuilder(string value) { }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public StringBuilder(string value, int capacity) { }
    [System.Security.SecuritySafeCriticalAttribute]
    public StringBuilder(string value, int startIndex, int length, int capacity) { }
    public int Capacity { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } set { } }
    [System.Runtime.CompilerServices.IndexerName("Chars")]
    public char this[int index] { get { return default(char); } set { } }
    public int Length { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(int); } set { } }
    public int MaxCapacity { get { return default(int); } }
    public System.Text.StringBuilder Append(bool value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(byte value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(char value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    [System.Security.SecurityCriticalAttribute]
    public unsafe System.Text.StringBuilder Append(char* value, int valueCount) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(char value, int repeatCount) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Append(char[] value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Append(char[] value, int startIndex, int charCount) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(decimal value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(double value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(short value) { return default(System.Text.StringBuilder); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Text.StringBuilder Append(int value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(long value) { return default(System.Text.StringBuilder); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Text.StringBuilder Append(object value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Append(sbyte value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Append(float value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Append(string value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Append(string value, int startIndex, int count) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Append(ushort value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Append(uint value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Append(ulong value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(System.IFormatProvider provider, string format, object arg0) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(System.IFormatProvider provider, string format, object arg0, object arg1) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(System.IFormatProvider provider, string format, object arg0, object arg1, object arg2) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(System.IFormatProvider provider, string format, params object[] args) { return default(System.Text.StringBuilder); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Text.StringBuilder AppendFormat(string format, object arg0) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(string format, object arg0, object arg1) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendFormat(string format, params object[] args) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendLine() { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder AppendLine(string value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Clear() { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { }
    public int EnsureCapacity(int capacity) { return default(int); }
    public bool Equals(System.Text.StringBuilder sb) { return default(bool); }
    public System.Text.StringBuilder Insert(int index, bool value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, byte value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Insert(int index, char value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, char[] value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Insert(int index, char[] value, int startIndex, int charCount) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, decimal value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, double value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, short value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, int value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, long value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, object value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Insert(int index, sbyte value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Insert(int index, float value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Insert(int index, string value) { return default(System.Text.StringBuilder); }
    [System.Security.SecuritySafeCriticalAttribute]
    public System.Text.StringBuilder Insert(int index, string value, int count) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Insert(int index, ushort value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Insert(int index, uint value) { return default(System.Text.StringBuilder); }
    [System.CLSCompliantAttribute(false)]
    public System.Text.StringBuilder Insert(int index, ulong value) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Remove(int startIndex, int length) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Replace(char oldChar, char newChar) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Replace(char oldChar, char newChar, int startIndex, int count) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Replace(string oldValue, string newValue) { return default(System.Text.StringBuilder); }
    public System.Text.StringBuilder Replace(string oldValue, string newValue, int startIndex, int count) { return default(System.Text.StringBuilder); }
    [System.Security.SecurityCriticalAttribute]
    void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    [System.Security.SecuritySafeCriticalAttribute]
    public override string ToString() { return default(string); }
    [System.Security.SecuritySafeCriticalAttribute]
    public string ToString(int startIndex, int length) { return default(string); }
  }
}
namespace System.Threading {
  [System.Diagnostics.DebuggerDisplayAttribute("IsCancellationRequested = {IsCancellationRequested}")]
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct CancellationToken {
    public CancellationToken(bool canceled) { throw new System.NotImplementedException(); }
    public bool CanBeCanceled { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsCancellationRequested { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public static System.Threading.CancellationToken None { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Threading.CancellationToken); } }
    public System.Threading.WaitHandle WaitHandle { get { return default(System.Threading.WaitHandle); } }
    public override bool Equals(object other) { return default(bool); }
    public bool Equals(System.Threading.CancellationToken other) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.Threading.CancellationToken left, System.Threading.CancellationToken right) { return default(bool); }
    public static bool operator !=(System.Threading.CancellationToken left, System.Threading.CancellationToken right) { return default(bool); }
    public System.Threading.CancellationTokenRegistration Register(System.Action callback) { return default(System.Threading.CancellationTokenRegistration); }
    public System.Threading.CancellationTokenRegistration Register(System.Action callback, bool useSynchronizationContext) { return default(System.Threading.CancellationTokenRegistration); }
    public System.Threading.CancellationTokenRegistration Register(System.Action<object> callback, object state) { return default(System.Threading.CancellationTokenRegistration); }
    public System.Threading.CancellationTokenRegistration Register(System.Action<object> callback, object state, bool useSynchronizationContext) { return default(System.Threading.CancellationTokenRegistration); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public void ThrowIfCancellationRequested() { }
  }
  [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public partial struct CancellationTokenRegistration : System.IDisposable, System.IEquatable<System.Threading.CancellationTokenRegistration> {
    public void Dispose() { }
    public override bool Equals(object obj) { return default(bool); }
    public bool Equals(System.Threading.CancellationTokenRegistration other) { return default(bool); }
    public override int GetHashCode() { return default(int); }
    public static bool operator ==(System.Threading.CancellationTokenRegistration left, System.Threading.CancellationTokenRegistration right) { return default(bool); }
    public static bool operator !=(System.Threading.CancellationTokenRegistration left, System.Threading.CancellationTokenRegistration right) { return default(bool); }
  }
  public partial class CancellationTokenSource : System.IDisposable {
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public CancellationTokenSource() { }
    public CancellationTokenSource(int millisecondsDelay) { }
    public CancellationTokenSource(System.TimeSpan delay) { }
    public bool IsCancellationRequested { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public System.Threading.CancellationToken Token { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(System.Threading.CancellationToken); } }
    public void Cancel() { }
    public void Cancel(bool throwOnFirstException) { }
    public void CancelAfter(int millisecondsDelay) { }
    public void CancelAfter(System.TimeSpan delay) { }
    public static System.Threading.CancellationTokenSource CreateLinkedTokenSource(System.Threading.CancellationToken token1, System.Threading.CancellationToken token2) { return default(System.Threading.CancellationTokenSource); }
    public static System.Threading.CancellationTokenSource CreateLinkedTokenSource(params System.Threading.CancellationToken[] tokens) { return default(System.Threading.CancellationTokenSource); }
    public void Dispose() { }
    protected virtual void Dispose(bool disposing) { }
  }
  public delegate void ContextCallback(object state);
  public partial class HostExecutionContext : System.IDisposable {
    public HostExecutionContext() { }
    public HostExecutionContext(object state) { }
    protected internal object State { get { return default(object); } set { } }
    public virtual System.Threading.HostExecutionContext CreateCopy() { return default(System.Threading.HostExecutionContext); }
    public void Dispose() { }
    public virtual void Dispose(bool disposing) { }
  }
  public partial class HostExecutionContextManager {
    public HostExecutionContextManager() { }
    public virtual System.Threading.HostExecutionContext Capture() { return default(System.Threading.HostExecutionContext); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public virtual void Revert(object previousState) { }
    public virtual object SetHostExecutionContext(System.Threading.HostExecutionContext hostExecutionContext) { return default(object); }
  }
  public enum LazyThreadSafetyMode {
    ExecutionAndPublication = 2,
    None = 0,
    PublicationOnly = 1,
  }
  public static partial class Timeout {
    public const int Infinite = -1;
    public static readonly System.TimeSpan InfiniteTimeSpan;
  }
  public abstract partial class WaitHandle : System.IDisposable {
    protected static readonly System.IntPtr InvalidHandle;
    public const int WaitTimeout = 258;
    protected WaitHandle() { }
    [System.ObsoleteAttribute("In the profiles > 2.x, use SafeHandle instead of Handle")]
    public virtual System.IntPtr Handle { get { return default(System.IntPtr); } set { } }
    public Microsoft.Win32.SafeHandles.SafeWaitHandle SafeWaitHandle { [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]get { return default(Microsoft.Win32.SafeHandles.SafeWaitHandle); } [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(2))]set { } }
    public virtual void Close() { }
    public void Dispose() { }
    protected virtual void Dispose(bool explicitDisposing) { }
    public static bool SignalAndWait(System.Threading.WaitHandle toSignal, System.Threading.WaitHandle toWaitOn) { return default(bool); }
    public static bool SignalAndWait(System.Threading.WaitHandle toSignal, System.Threading.WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext) { return default(bool); }
    public static bool SignalAndWait(System.Threading.WaitHandle toSignal, System.Threading.WaitHandle toWaitOn, System.TimeSpan timeout, bool exitContext) { return default(bool); }
    public static bool WaitAll(System.Threading.WaitHandle[] waitHandles) { return default(bool); }
    public static bool WaitAll(System.Threading.WaitHandle[] waitHandles, int millisecondsTimeout) { return default(bool); }
    public static bool WaitAll(System.Threading.WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext) { return default(bool); }
    public static bool WaitAll(System.Threading.WaitHandle[] waitHandles, System.TimeSpan timeout) { return default(bool); }
    public static bool WaitAll(System.Threading.WaitHandle[] waitHandles, System.TimeSpan timeout, bool exitContext) { return default(bool); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int WaitAny(System.Threading.WaitHandle[] waitHandles) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int WaitAny(System.Threading.WaitHandle[] waitHandles, int millisecondsTimeout) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int WaitAny(System.Threading.WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int WaitAny(System.Threading.WaitHandle[] waitHandles, System.TimeSpan timeout) { return default(int); }
    [System.Runtime.ConstrainedExecution.ReliabilityContractAttribute((System.Runtime.ConstrainedExecution.Consistency)(3), (System.Runtime.ConstrainedExecution.Cer)(1))]
    public static int WaitAny(System.Threading.WaitHandle[] waitHandles, System.TimeSpan timeout, bool exitContext) { return default(int); }
    public virtual bool WaitOne() { return default(bool); }
    public virtual bool WaitOne(int millisecondsTimeout) { return default(bool); }
    public virtual bool WaitOne(int millisecondsTimeout, bool exitContext) { return default(bool); }
    public virtual bool WaitOne(System.TimeSpan timeout) { return default(bool); }
    public virtual bool WaitOne(System.TimeSpan timeout, bool exitContext) { return default(bool); }
  }
}
namespace System.Threading.Tasks {
  [System.Diagnostics.DebuggerDisplayAttribute("Concurrent={ConcurrentTaskCountForDebugger}, Exclusive={ExclusiveTaskCountForDebugger}, Mode={ModeForDebugger}")]
  public partial class ConcurrentExclusiveSchedulerPair {
    public ConcurrentExclusiveSchedulerPair() { }
    public ConcurrentExclusiveSchedulerPair(System.Threading.Tasks.TaskScheduler taskScheduler) { }
    public ConcurrentExclusiveSchedulerPair(System.Threading.Tasks.TaskScheduler taskScheduler, int maxConcurrencyLevel) { }
    public ConcurrentExclusiveSchedulerPair(System.Threading.Tasks.TaskScheduler taskScheduler, int maxConcurrencyLevel, int maxItemsPerTask) { }
    public System.Threading.Tasks.Task Completion { get { return default(System.Threading.Tasks.Task); } }
    public System.Threading.Tasks.TaskScheduler ConcurrentScheduler { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public System.Threading.Tasks.TaskScheduler ExclusiveScheduler { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public void Complete() { }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Id = {Id}, Status = {Status}, Method = {DebuggerDisplayMethodDescription}")]
  public partial class Task : System.IAsyncResult, System.IDisposable {
    public Task(System.Action action) { }
    public Task(System.Action action, System.Threading.CancellationToken cancellationToken) { }
    public Task(System.Action action, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public Task(System.Action action, System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public Task(System.Action<object> action, object state) { }
    public Task(System.Action<object> action, object state, System.Threading.CancellationToken cancellationToken) { }
    public Task(System.Action<object> action, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public Task(System.Action<object> action, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public object AsyncState { get { return default(object); } }
    public static System.Threading.Tasks.Task CompletedTask { get { return default(System.Threading.Tasks.Task); } }
    public System.Threading.Tasks.TaskCreationOptions CreationOptions { get { return default(System.Threading.Tasks.TaskCreationOptions); } }
    public static System.Nullable<int> CurrentId { get { return default(System.Nullable<int>); } }
    public System.AggregateException Exception { get { return default(System.AggregateException); } }
    public static System.Threading.Tasks.TaskFactory Factory { get { return default(System.Threading.Tasks.TaskFactory); } }
    public int Id { get { return default(int); } }
    public bool IsCanceled { get { return default(bool); } }
    public bool IsCompleted { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(bool); } }
    public bool IsFaulted { get { return default(bool); } }
    public System.Threading.Tasks.TaskStatus Status { get { return default(System.Threading.Tasks.TaskStatus); } }
    System.Threading.WaitHandle System.IAsyncResult.AsyncWaitHandle { get { return default(System.Threading.WaitHandle); } }
    bool System.IAsyncResult.CompletedSynchronously { get { return default(bool); } }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Runtime.CompilerServices.ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) { return default(System.Runtime.CompilerServices.ConfiguredTaskAwaitable); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task, object> continuationAction, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task, object> continuationAction, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task, object> continuationAction, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task, object> continuationAction, object state, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task, object> continuationAction, object state, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, object, TResult> continuationFunction, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, object, TResult> continuationFunction, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, object, TResult> continuationFunction, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, object, TResult> continuationFunction, object state, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWith<TResult>(System.Func<System.Threading.Tasks.Task, object, TResult> continuationFunction, object state, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task Delay(int millisecondsDelay) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Delay(int millisecondsDelay, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Delay(System.TimeSpan delay) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Delay(System.TimeSpan delay, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public void Dispose() { }
    protected virtual void Dispose(bool disposing) { }
    public static System.Threading.Tasks.Task FromCanceled(System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task<TResult> FromCanceled<TResult>(System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task FromException(System.Exception exception) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task<TResult> FromException<TResult>(System.Exception exception) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task<TResult> FromResult<TResult>(TResult result) { return default(System.Threading.Tasks.Task<TResult>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public System.Runtime.CompilerServices.TaskAwaiter GetAwaiter() { return default(System.Runtime.CompilerServices.TaskAwaiter); }
    public static System.Threading.Tasks.Task Run(System.Action action) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Run(System.Action action, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Run(System.Func<System.Threading.Tasks.Task> function) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task Run(System.Func<System.Threading.Tasks.Task> function, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<TResult> function) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<System.Threading.Tasks.Task<TResult>> function) { return default(System.Threading.Tasks.Task<TResult>); }
    public static System.Threading.Tasks.Task<TResult> Run<TResult>(System.Func<System.Threading.Tasks.Task<TResult>> function, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public void RunSynchronously() { }
    public void RunSynchronously(System.Threading.Tasks.TaskScheduler scheduler) { }
    public void Start() { }
    public void Start(System.Threading.Tasks.TaskScheduler scheduler) { }
    public void Wait() { }
    public bool Wait(int millisecondsTimeout) { return default(bool); }
    public bool Wait(int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { return default(bool); }
    public void Wait(System.Threading.CancellationToken cancellationToken) { }
    public bool Wait(System.TimeSpan timeout) { return default(bool); }
    public static void WaitAll(params System.Threading.Tasks.Task[] tasks) { }
    public static bool WaitAll(System.Threading.Tasks.Task[] tasks, int millisecondsTimeout) { return default(bool); }
    public static bool WaitAll(System.Threading.Tasks.Task[] tasks, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { return default(bool); }
    public static void WaitAll(System.Threading.Tasks.Task[] tasks, System.Threading.CancellationToken cancellationToken) { }
    public static bool WaitAll(System.Threading.Tasks.Task[] tasks, System.TimeSpan timeout) { return default(bool); }
    public static int WaitAny(params System.Threading.Tasks.Task[] tasks) { return default(int); }
    public static int WaitAny(System.Threading.Tasks.Task[] tasks, int millisecondsTimeout) { return default(int); }
    public static int WaitAny(System.Threading.Tasks.Task[] tasks, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { return default(int); }
    public static int WaitAny(System.Threading.Tasks.Task[] tasks, System.Threading.CancellationToken cancellationToken) { return default(int); }
    public static int WaitAny(System.Threading.Tasks.Task[] tasks, System.TimeSpan timeout) { return default(int); }
    public static System.Threading.Tasks.Task WhenAll(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> tasks) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task WhenAll(params System.Threading.Tasks.Task[] tasks) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task<TResult[]> WhenAll<TResult>(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<TResult>> tasks) { return default(System.Threading.Tasks.Task<TResult[]>); }
    public static System.Threading.Tasks.Task<TResult[]> WhenAll<TResult>(params System.Threading.Tasks.Task<TResult>[] tasks) { return default(System.Threading.Tasks.Task<TResult[]>); }
    public static System.Threading.Tasks.Task<System.Threading.Tasks.Task> WhenAny(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> tasks) { return default(System.Threading.Tasks.Task<System.Threading.Tasks.Task>); }
    public static System.Threading.Tasks.Task<System.Threading.Tasks.Task> WhenAny(params System.Threading.Tasks.Task[] tasks) { return default(System.Threading.Tasks.Task<System.Threading.Tasks.Task>); }
    public static System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>> WhenAny<TResult>(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<TResult>> tasks) { return default(System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>>); }
    public static System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>> WhenAny<TResult>(params System.Threading.Tasks.Task<TResult>[] tasks) { return default(System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>>); }
    public static System.Runtime.CompilerServices.YieldAwaitable Yield() { return default(System.Runtime.CompilerServices.YieldAwaitable); }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Id = {Id}, Status = {Status}, Method = {DebuggerDisplayMethodDescription}, Result = {DebuggerDisplayResultDescription}")]
  public partial class Task<TResult> : System.Threading.Tasks.Task {
    public Task(System.Func<TResult> function) : base (default(System.Action)) { }
    public Task(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken) : base (default(System.Action)) { }
    public Task(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions) : base (default(System.Action)) { }
    public Task(System.Func<TResult> function, System.Threading.Tasks.TaskCreationOptions creationOptions) : base (default(System.Action)) { }
    public Task(System.Func<object, TResult> function, object state) : base (default(System.Action)) { }
    public Task(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken) : base (default(System.Action)) { }
    public Task(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions) : base (default(System.Action)) { }
    public Task(System.Func<object, TResult> function, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) : base (default(System.Action)) { }
    public static new System.Threading.Tasks.TaskFactory<TResult> Factory { get { return default(System.Threading.Tasks.TaskFactory<TResult>); } }
    [System.Diagnostics.DebuggerBrowsableAttribute((System.Diagnostics.DebuggerBrowsableState)(0))]
    public TResult Result { [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]get { return default(TResult); } }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public new System.Runtime.CompilerServices.ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) { return default(System.Runtime.CompilerServices.ConfiguredTaskAwaitable<TResult>); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>, object> continuationAction, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>, object> continuationAction, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>, object> continuationAction, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>, object> continuationAction, object state, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>, object> continuationAction, object state, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWith(System.Action<System.Threading.Tasks.Task<TResult>> continuationAction, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, TNewResult> continuationFunction) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, TNewResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, TNewResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, TNewResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, TNewResult> continuationFunction, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, object, TNewResult> continuationFunction, object state) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, object, TNewResult> continuationFunction, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, object, TNewResult> continuationFunction, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, object, TNewResult> continuationFunction, object state, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TNewResult>); }
    public System.Threading.Tasks.Task<TNewResult> ContinueWith<TNewResult>(System.Func<System.Threading.Tasks.Task<TResult>, object, TNewResult> continuationFunction, object state, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TNewResult>); }
    [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
    public new System.Runtime.CompilerServices.TaskAwaiter<TResult> GetAwaiter() { return default(System.Runtime.CompilerServices.TaskAwaiter<TResult>); }
  }
  public partial class TaskCanceledException : System.OperationCanceledException {
    public TaskCanceledException() { }
    protected TaskCanceledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TaskCanceledException(string message) { }
    public TaskCanceledException(string message, System.Exception innerException) { }
    public TaskCanceledException(System.Threading.Tasks.Task task) { }
    public System.Threading.Tasks.Task Task { get { return default(System.Threading.Tasks.Task); } }
  }
  public partial class TaskCompletionSource<TResult> {
    public TaskCompletionSource() { }
    public TaskCompletionSource(object state) { }
    public TaskCompletionSource(object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public TaskCompletionSource(System.Threading.Tasks.TaskCreationOptions creationOptions) { }
    public System.Threading.Tasks.Task<TResult> Task { get { return default(System.Threading.Tasks.Task<TResult>); } }
    public void SetCanceled() { }
    public void SetException(System.Collections.Generic.IEnumerable<System.Exception> exceptions) { }
    public void SetException(System.Exception exception) { }
    public void SetResult(TResult result) { }
    public bool TrySetCanceled() { return default(bool); }
    public bool TrySetCanceled(System.Threading.CancellationToken cancellationToken) { return default(bool); }
    public bool TrySetException(System.Collections.Generic.IEnumerable<System.Exception> exceptions) { return default(bool); }
    public bool TrySetException(System.Exception exception) { return default(bool); }
    public bool TrySetResult(TResult result) { return default(bool); }
  }
  [System.FlagsAttribute]
  public enum TaskContinuationOptions {
    AttachedToParent = 4,
    DenyChildAttach = 8,
    ExecuteSynchronously = 524288,
    HideScheduler = 16,
    LazyCancellation = 32,
    LongRunning = 2,
    None = 0,
    NotOnCanceled = 262144,
    NotOnFaulted = 131072,
    NotOnRanToCompletion = 65536,
    OnlyOnCanceled = 196608,
    OnlyOnFaulted = 327680,
    OnlyOnRanToCompletion = 393216,
    PreferFairness = 1,
  }
  [System.FlagsAttribute]
  public enum TaskCreationOptions {
    AttachedToParent = 4,
    DenyChildAttach = 8,
    HideScheduler = 16,
    LongRunning = 2,
    None = 0,
    PreferFairness = 1,
  }
  public static partial class TaskExtensions {
    public static System.Threading.Tasks.Task Unwrap(this System.Threading.Tasks.Task<System.Threading.Tasks.Task> task) { return default(System.Threading.Tasks.Task); }
    public static System.Threading.Tasks.Task<TResult> Unwrap<TResult>(this System.Threading.Tasks.Task<System.Threading.Tasks.Task<TResult>> task) { return default(System.Threading.Tasks.Task<TResult>); }
  }
  public partial class TaskFactory {
    public TaskFactory() { }
    public TaskFactory(System.Threading.CancellationToken cancellationToken) { }
    public TaskFactory(System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { }
    public TaskFactory(System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { }
    public TaskFactory(System.Threading.Tasks.TaskScheduler scheduler) { }
    public System.Threading.CancellationToken CancellationToken { get { return default(System.Threading.CancellationToken); } }
    public System.Threading.Tasks.TaskContinuationOptions ContinuationOptions { get { return default(System.Threading.Tasks.TaskContinuationOptions); } }
    public System.Threading.Tasks.TaskCreationOptions CreationOptions { get { return default(System.Threading.Tasks.TaskCreationOptions); } }
    public System.Threading.Tasks.TaskScheduler Scheduler { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public System.Threading.Tasks.Task ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task[]> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task[]> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task[]> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task[]> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>[]> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>[]> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>[]> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>[]> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Action<System.Threading.Tasks.Task> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TResult>(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>> continuationAction) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>> continuationAction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>> continuationAction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Action<System.Threading.Tasks.Task<TAntecedentResult>> continuationAction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task FromAsync(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync(System.IAsyncResult asyncResult, System.Action<System.IAsyncResult> endMethod) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync(System.IAsyncResult asyncResult, System.Action<System.IAsyncResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync(System.IAsyncResult asyncResult, System.Action<System.IAsyncResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync<TArg1>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync<TArg1>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TResult>(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TResult>(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TResult>(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TResult>(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TResult>(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task FromAsync<TArg1, TArg2>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync<TArg1, TArg2>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TResult>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TResult>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task FromAsync<TArg1, TArg2, TArg3>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task FromAsync<TArg1, TArg2, TArg3>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Action<System.IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TResult>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TResult>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task StartNew(System.Action action) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action action, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action action, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action action, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action<object> action, object state) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action<object> action, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action<object> action, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task StartNew(System.Action<object> action, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<TResult> function) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<TResult> function, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<object, TResult> function, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew<TResult>(System.Func<object, TResult> function, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
  }
  public partial class TaskFactory<TResult> {
    public TaskFactory() { }
    public TaskFactory(System.Threading.CancellationToken cancellationToken) { }
    public TaskFactory(System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { }
    public TaskFactory(System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { }
    public TaskFactory(System.Threading.Tasks.TaskScheduler scheduler) { }
    public System.Threading.CancellationToken CancellationToken { get { return default(System.Threading.CancellationToken); } }
    public System.Threading.Tasks.TaskContinuationOptions ContinuationOptions { get { return default(System.Threading.Tasks.TaskContinuationOptions); } }
    public System.Threading.Tasks.TaskCreationOptions CreationOptions { get { return default(System.Threading.Tasks.TaskCreationOptions); } }
    public System.Threading.Tasks.TaskScheduler Scheduler { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task[], TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAll<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>[], TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny(System.Threading.Tasks.Task[] tasks, System.Func<System.Threading.Tasks.Task, TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskContinuationOptions continuationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> ContinueWhenAny<TAntecedentResult>(System.Threading.Tasks.Task<TAntecedentResult>[] tasks, System.Func<System.Threading.Tasks.Task<TAntecedentResult>, TResult> continuationFunction, System.Threading.Tasks.TaskContinuationOptions continuationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync(System.Func<System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync(System.IAsyncResult asyncResult, System.Func<System.IAsyncResult, TResult> endMethod, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1>(System.Func<TArg1, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2>(System.Func<TArg1, TArg2, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TArg3>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> FromAsync<TArg1, TArg2, TArg3>(System.Func<TArg1, TArg2, TArg3, System.AsyncCallback, object, System.IAsyncResult> beginMethod, System.Func<System.IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<TResult> function) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<TResult> function, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<TResult> function, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<object, TResult> function, object state) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<object, TResult> function, object state, System.Threading.CancellationToken cancellationToken, System.Threading.Tasks.TaskCreationOptions creationOptions, System.Threading.Tasks.TaskScheduler scheduler) { return default(System.Threading.Tasks.Task<TResult>); }
    public System.Threading.Tasks.Task<TResult> StartNew(System.Func<object, TResult> function, object state, System.Threading.Tasks.TaskCreationOptions creationOptions) { return default(System.Threading.Tasks.Task<TResult>); }
  }
  [System.Diagnostics.DebuggerDisplayAttribute("Id={Id}")]
  public abstract partial class TaskScheduler {
    protected TaskScheduler() { }
    public static System.Threading.Tasks.TaskScheduler Current { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public static System.Threading.Tasks.TaskScheduler Default { get { return default(System.Threading.Tasks.TaskScheduler); } }
    public int Id { get { return default(int); } }
    public virtual int MaximumConcurrencyLevel { get { return default(int); } }
    public static event System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs> UnobservedTaskException { add { } remove { } }
    public static System.Threading.Tasks.TaskScheduler FromCurrentSynchronizationContext() { return default(System.Threading.Tasks.TaskScheduler); }
    [System.Security.SecurityCriticalAttribute]
    protected abstract System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> GetScheduledTasks();
    [System.Security.SecurityCriticalAttribute]
    protected internal abstract void QueueTask(System.Threading.Tasks.Task task);
    [System.Security.SecurityCriticalAttribute]
    protected internal virtual bool TryDequeue(System.Threading.Tasks.Task task) { return default(bool); }
    [System.Security.SecurityCriticalAttribute]
    protected bool TryExecuteTask(System.Threading.Tasks.Task task) { return default(bool); }
    [System.Security.SecurityCriticalAttribute]
    protected abstract bool TryExecuteTaskInline(System.Threading.Tasks.Task task, bool taskWasPreviouslyQueued);
  }
  public partial class TaskSchedulerException : System.Exception {
    public TaskSchedulerException() { }
    public TaskSchedulerException(System.Exception innerException) { }
    protected TaskSchedulerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    public TaskSchedulerException(string message) { }
    public TaskSchedulerException(string message, System.Exception innerException) { }
  }
  public enum TaskStatus {
    Canceled = 6,
    Created = 0,
    Faulted = 7,
    RanToCompletion = 5,
    Running = 3,
    WaitingForActivation = 1,
    WaitingForChildrenToComplete = 4,
    WaitingToRun = 2,
  }
  public partial class UnobservedTaskExceptionEventArgs : System.EventArgs {
    public UnobservedTaskExceptionEventArgs(System.AggregateException exception) { }
    public System.AggregateException Exception { get { return default(System.AggregateException); } }
    public bool Observed { get { return default(bool); } }
    public void SetObserved() { }
  }
}
