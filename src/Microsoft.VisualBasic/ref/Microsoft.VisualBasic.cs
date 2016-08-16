// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.VisualBasic
{
    /// <summary>
    /// Indicates the type of procedure being invoked when calling the CallByName function.
    /// </summary>
    public enum CallType
    {
        /// <summary>
        /// A property value is being retrieved.  This member is equivalent to the Visual Basic constant
        /// vbGet.
        /// </summary>
        Get = 2,
        /// <summary>
        /// An Object property value is being determined. This member is equivalent to the Visual Basic
        /// constant vbLet.
        /// </summary>
        Let = 4,
        /// <summary>
        /// A method is being invoked.  This member is equivalent to the Visual Basic constant vbMethod.
        /// </summary>
        Method = 1,
        /// <summary>
        /// A property value is being determined.  This member is equivalent to the Visual Basic constant
        /// vbSet.
        /// </summary>
        Set = 8,
    }
    /// <summary>
    /// The Constants module contains miscellaneous constants. These constants can be used anywhere
    /// in your code.
    /// </summary>
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Constants
    {
        internal Constants() { }
        /// <summary>
        /// Represents a backspace character for print and display functions.
        /// </summary>
        public const string vbBack = "\b";
        /// <summary>
        /// Represents a carriage-return character for print and display functions.
        /// </summary>
        public const string vbCr = "\r";
        /// <summary>
        /// Represents a carriage-return character combined with a linefeed character for print and display
        /// functions.
        /// </summary>
        public const string vbCrLf = "\r\n";
        /// <summary>
        /// Represents a form-feed character for print functions.
        /// </summary>
        public const string vbFormFeed = "\f";
        /// <summary>
        /// Represents a linefeed character for print and display functions.
        /// </summary>
        public const string vbLf = "\n";
        /// <summary>
        /// Represents a newline character for print and display functions.
        /// </summary>
        [System.Obsolete("For a carriage return and line feed, use vbCrLf.  For the current platform's newline, use System.Environment.NewLine.")]
        public const string vbNewLine = "\r\n";
        /// <summary>
        /// Represents a null character for print and display functions.
        /// </summary>
        public const string vbNullChar = "\0";
        /// <summary>
        /// Represents a zero-length string for print and display functions, and for calling external procedures.
        /// </summary>
        public const string vbNullString = null;
        /// <summary>
        /// Represents a tab character for print and display functions.
        /// </summary>
        public const string vbTab = "\t";
        /// <summary>
        /// Represents a carriage-return character for print functions.
        /// </summary>
        public const string vbVerticalTab = "\v";
    }
    /// <summary>
    /// The HideModuleNameAttribute attribute, when applied to a module, allows the module members
    /// to be accessed using only the qualification needed for the module.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), AllowMultiple = false, Inherited = false)]
    public sealed partial class HideModuleNameAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HideModuleNameAttribute" />
        /// attribute.
        /// </summary>
        public HideModuleNameAttribute() { }
    }
    /// <summary>
    /// The Strings module contains procedures used to perform string operations.
    /// </summary>
    [Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute]
    public sealed partial class Strings
    {
        internal Strings() { }
        /// <summary>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </summary>
        /// <param name="String">
        /// Required. Any valid Char or String expression. If <paramref name="String" /> is a String expression,
        /// only the first character of the string is used for input. If <paramref name="String" /> is
        /// Nothing or contains no characters, an <see cref="System.ArgumentException" /> error occurs.
        /// </param>
        /// <returns>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </returns>
        public static int AscW(char String) { return default(int); }
        /// <summary>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </summary>
        /// <param name="String">
        /// Required. Any valid Char or String expression. If <paramref name="String" /> is a String expression,
        /// only the first character of the string is used for input. If <paramref name="String" /> is
        /// Nothing or contains no characters, an <see cref="System.ArgumentException" /> error occurs.
        /// </param>
        /// <returns>
        /// Returns an Integer value representing the character code corresponding to a character.
        /// </returns>
        public static int AscW(string String) { return default(int); }
        /// <summary>
        /// Returns the character associated with the specified character code.
        /// </summary>
        /// <param name="CharCode">
        /// Required. An Integer expression representing the <paramref name="code point" />, or character
        /// code, for the character.
        /// </param>
        /// <returns>
        /// Returns the character associated with the specified character code.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="CharCode" /> &lt; -32768 or &gt; 65535 for ChrW.
        /// </exception>
        public static char ChrW(int CharCode) { return default(char); }
    }
}
namespace Microsoft.VisualBasic.CompilerServices
{
    /// <summary>
    /// Provides methods that perform various type conversions.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class Conversions
    {
        internal Conversions() { }
        /// <summary>
        /// Converts an object to the specified type.
        /// </summary>
        /// <param name="Expression">The object to convert.</param>
        /// <param name="TargetType">The type to which to convert the object.</param>
        /// <returns>
        /// An object of the specified target type.
        /// </returns>
        public static object ChangeType(object Expression, System.Type TargetType) { return default(object); }
        /// <summary>
        /// Converts an object to a <see cref="System.Boolean" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// A Boolean value. Returns False if the object is null; otherwise, True.
        /// </returns>
        public static bool ToBoolean(object Value) { return default(bool); }
        /// <summary>
        /// Converts a string to a <see cref="System.Boolean" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// A Boolean value. Returns False if the string is null; otherwise, True.
        /// </returns>
        public static bool ToBoolean(string Value) { return default(bool); }
        /// <summary>
        /// Converts an object to a <see cref="System.Byte" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Byte value of the object.
        /// </returns>
        public static byte ToByte(object Value) { return default(byte); }
        /// <summary>
        /// Converts a string to a <see cref="System.Byte" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Byte value of the string.
        /// </returns>
        public static byte ToByte(string Value) { return default(byte); }
        /// <summary>
        /// Converts an object to a <see cref="System.Char" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Char value of the object.
        /// </returns>
        public static char ToChar(object Value) { return default(char); }
        /// <summary>
        /// Converts a string to a <see cref="System.Char" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Char value of the string.
        /// </returns>
        public static char ToChar(string Value) { return default(char); }
        /// <summary>
        /// Converts an object to a one-dimensional <see cref="System.Char" /> array.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// A one-dimensional Char array.
        /// </returns>
        public static char[] ToCharArrayRankOne(object Value) { return default(char[]); }
        /// <summary>
        /// Converts a string to a one-dimensional <see cref="System.Char" /> array.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// A one-dimensional Char array.
        /// </returns>
        public static char[] ToCharArrayRankOne(string Value) { return default(char[]); }
        /// <summary>
        /// Converts an object to a <see cref="System.DateTime" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The DateTime value of the object.
        /// </returns>
        public static System.DateTime ToDate(object Value) { return default(System.DateTime); }
        /// <summary>
        /// Converts a string to a <see cref="System.DateTime" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The DateTime value of the string.
        /// </returns>
        public static System.DateTime ToDate(string Value) { return default(System.DateTime); }
        /// <summary>
        /// Converts a <see cref="System.Boolean" /> value to a <see cref="System.Decimal" /> value.
        /// </summary>
        /// <param name="Value">A Boolean value to convert.</param>
        /// <returns>
        /// The Decimal value of the Boolean value.
        /// </returns>
        public static decimal ToDecimal(bool Value) { return default(decimal); }
        /// <summary>
        /// Converts an object to a <see cref="System.Decimal" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Decimal value of the object.
        /// </returns>
        public static decimal ToDecimal(object Value) { return default(decimal); }
        /// <summary>
        /// Converts a string to a <see cref="System.Decimal" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Decimal value of the string.
        /// </returns>
        public static decimal ToDecimal(string Value) { return default(decimal); }
        /// <summary>
        /// Converts an object to a <see cref="System.Double" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Double value of the object.
        /// </returns>
        public static double ToDouble(object Value) { return default(double); }
        /// <summary>
        /// Converts a string to a <see cref="System.Double" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Double value of the string.
        /// </returns>
        public static double ToDouble(string Value) { return default(double); }
        /// <summary>
        /// Converts an object to a generic type <paramref name="T" />.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <typeparam name="T">The type to convert <paramref name="Value" /> to.</typeparam>
        /// <returns>
        /// A structure or object of generic type <paramref name="T" />.
        /// </returns>
        public static T ToGenericParameter<T>(object Value) { return default(T); }
        /// <summary>
        /// Converts an object to an integer value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The int value of the object.
        /// </returns>
        public static int ToInteger(object Value) { return default(int); }
        /// <summary>
        /// Converts a string to an integer value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The int value of the string.
        /// </returns>
        public static int ToInteger(string Value) { return default(int); }
        /// <summary>
        /// Converts an object to a Long value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Long value of the object.
        /// </returns>
        public static long ToLong(object Value) { return default(long); }
        /// <summary>
        /// Converts a string to a Long value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Long value of the string.
        /// </returns>
        public static long ToLong(string Value) { return default(long); }
        /// <summary>
        /// Converts an object to an <see cref="System.SByte" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The SByte value of the object.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static sbyte ToSByte(object Value) { return default(sbyte); }
        /// <summary>
        /// Converts a string to an <see cref="System.SByte" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The SByte value of the string.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static sbyte ToSByte(string Value) { return default(sbyte); }
        /// <summary>
        /// Converts an object to a Short value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Short value of the object.
        /// </returns>
        public static short ToShort(object Value) { return default(short); }
        /// <summary>
        /// Converts a string to a Short value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Short value of the string.
        /// </returns>
        public static short ToShort(string Value) { return default(short); }
        /// <summary>
        /// Converts an object to a <see cref="System.Single" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Single value of the object.
        /// </returns>
        public static float ToSingle(object Value) { return default(float); }
        /// <summary>
        /// Converts a <see cref="System.String" /> to a <see cref="System.Single" /> value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Single value of the string.
        /// </returns>
        public static float ToSingle(string Value) { return default(float); }
        /// <summary>
        /// Converts a <see cref="System.Boolean" /> value to a <see cref="System.String" />.
        /// </summary>
        /// <param name="Value">The Boolean value to convert.</param>
        /// <returns>
        /// The String representation of the Boolean value.
        /// </returns>
        public static string ToString(bool Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.Byte" /> value to a <see cref="System.String" />.
        /// </summary>
        /// <param name="Value">The Byte value to convert.</param>
        /// <returns>
        /// The String representation of the Byte value.
        /// </returns>
        public static string ToString(byte Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.Char" /> value to a <see cref="System.String" />.
        /// </summary>
        /// <param name="Value">The Char value to convert.</param>
        /// <returns>
        /// The String representation of the Char value.
        /// </returns>
        public static string ToString(char Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.DateTime" /> value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The DateTime value to convert.</param>
        /// <returns>
        /// The String representation of the DateTime value.
        /// </returns>
        public static string ToString(System.DateTime Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.Decimal" /> value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Decimal value to convert.</param>
        /// <returns>
        /// The String representation of the Decimal value.
        /// </returns>
        public static string ToString(decimal Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.Double" /> value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Double value to convert.</param>
        /// <returns>
        /// The String representation of the Double value.
        /// </returns>
        public static string ToString(double Value) { return default(string); }
        /// <summary>
        /// Converts a Short value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Short value to convert.</param>
        /// <returns>
        /// The String representation of the Short value.
        /// </returns>
        public static string ToString(short Value) { return default(string); }
        /// <summary>
        /// Converts an integer value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The int value to convert.</param>
        /// <returns>
        /// The String representation of the int value.
        /// </returns>
        public static string ToString(int Value) { return default(string); }
        /// <summary>
        /// Converts a Long value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Long value to convert.</param>
        /// <returns>
        /// The String representation of the Long value.
        /// </returns>
        public static string ToString(long Value) { return default(string); }
        /// <summary>
        /// Converts an object to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The String representation of the object.
        /// </returns>
        public static string ToString(object Value) { return default(string); }
        /// <summary>
        /// Converts a <see cref="System.Single" /> value (a single-precision floating point number)
        /// to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Single value to convert.</param>
        /// <returns>
        /// The String representation of the Single value.
        /// </returns>
        public static string ToString(float Value) { return default(string); }
        /// <summary>
        /// Converts a uint value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Uint value to convert.</param>
        /// <returns>
        /// The String representation of the Uint value.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static string ToString(uint Value) { return default(string); }
        /// <summary>
        /// Converts a Ulong value to a <see cref="System.String" /> value.
        /// </summary>
        /// <param name="Value">The Ulong value to convert.</param>
        /// <returns>
        /// The String representation of the Ulong value.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static string ToString(ulong Value) { return default(string); }
        /// <summary>
        /// Converts an object to a Uint value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Uint value of the object.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static uint ToUInteger(object Value) { return default(uint); }
        /// <summary>
        /// Converts a string to a Uint value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Uint value of the string.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static uint ToUInteger(string Value) { return default(uint); }
        /// <summary>
        /// Converts an object to a Ulong value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Ulong value of the object.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static ulong ToULong(object Value) { return default(ulong); }
        /// <summary>
        /// Converts a string to a Ulong value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Ulong value of the string.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static ulong ToULong(string Value) { return default(ulong); }
        /// <summary>
        /// Converts an object to a Ushort value.
        /// </summary>
        /// <param name="Value">The object to convert.</param>
        /// <returns>
        /// The Ushort value of the object.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static ushort ToUShort(object Value) { return default(ushort); }
        /// <summary>
        /// Converts a string to a Ushort value.
        /// </summary>
        /// <param name="Value">The string to convert.</param>
        /// <returns>
        /// The Ushort value of the string.
        /// </returns>
        [System.CLSCompliantAttribute(false)]
        public static ushort ToUShort(string Value) { return default(ushort); }
    }
    /// <summary>
    /// When applied to a class, the compiler implicitly calls a component-initializing method from
    /// the default synthetic constructor.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), AllowMultiple = false, Inherited = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class DesignerGeneratedAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DesignerGeneratedAttribute" /> attribute.
        /// </summary>
        public DesignerGeneratedAttribute() { }
    }
    /// <summary>
    /// The Visual Basic compiler uses this class during static local initialization; it is not meant
    /// to be called directly from your code. An exception of this type is thrown if a static local variable
    /// fails to initialize.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class IncompleteInitialization : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="IncompleteInitialization" /> class.
        /// </summary>
        public IncompleteInitialization() { }
    }
    /// <summary>
    /// This class provides helpers that the Visual Basic compiler uses for late binding calls; it
    /// is not meant to be called directly from your code.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class NewLateBinding
    {
        internal NewLateBinding() { }
        /// <summary>
        /// Executes a late-bound method or function call. This helper method is not meant to be called
        /// directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Type">The type of the call object.</param>
        /// <param name="MemberName">The name of the property or method on the call object.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
        /// <param name="CopyBack">
        /// An array of Boolean values that the late binder uses to communicate back to the call site which
        /// arguments match ByRef parameters. Each True value indicates that the arguments matched and should
        /// be copied out after the call to LateCall is complete.
        /// </param>
        /// <param name="IgnoreReturn">A Boolean value indicating whether or not the return value can be ignored.</param>
        /// <returns>
        /// An instance of the call object.
        /// </returns>
        public static object LateCall(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool[] CopyBack, bool IgnoreReturn) { return default(object); }
        /// <summary>
        /// Executes a late-bound property get or field access call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Type">The type of the call object.</param>
        /// <param name="MemberName">The name of the property or method on the call object.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
        /// <param name="CopyBack">
        /// An array of Boolean values that the late binder uses to communicate back to the call site which
        /// arguments match ByRef parameters. Each True value indicates that the arguments matched and should
        /// be copied out after the call to LateCall is complete.
        /// </param>
        /// <returns>
        /// An instance of the call object.
        /// </returns>
        public static object LateGet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool[] CopyBack) { return default(object); }
        /// <summary>
        /// Executes a late-bound property get or field access call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <returns>
        /// An instance of the call object.
        /// </returns>
        public static object LateIndexGet(object Instance, object[] Arguments, string[] ArgumentNames) { return default(object); }
        /// <summary>
        /// Executes a late-bound property set or field write call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        public static void LateIndexSet(object Instance, object[] Arguments, string[] ArgumentNames) { }
        /// <summary>
        /// Executes a late-bound property set or field write call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="OptimisticSet">
        /// A Boolean value used to determine whether the set operation will work. Set to True when you
        /// believe that an intermediate value has been set in the property or field; otherwise False.
        /// </param>
        /// <param name="RValueBase">
        /// A Boolean value that specifies when the base reference of the late reference is an RValue.
        /// Set to True when the base reference of the late reference is an RValue; this allows you to generate
        /// a run-time exception for late assignments to fields of RValues of value types. Otherwise, set to False.
        /// </param>
        public static void LateIndexSetComplex(object Instance, object[] Arguments, string[] ArgumentNames, bool OptimisticSet, bool RValueBase) { }
        /// <summary>
        /// Executes a late-bound property set or field write call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Type">The type of the call object.</param>
        /// <param name="MemberName">The name of the property or method on the call object.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
        public static void LateSet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments) { }
        /// <summary>
        /// Executes a late-bound property set or field write call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Type">The type of the call object.</param>
        /// <param name="MemberName">The name of the property or method on the call object.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
        /// <param name="OptimisticSet">
        /// A Boolean value used to determine whether the set operation will work. Set to True when you
        /// believe that an intermediate value has been set in the property or field; otherwise False.
        /// </param>
        /// <param name="RValueBase">
        /// A Boolean value that specifies when the base reference of the late reference is an RValue.
        /// Set to True when the base reference of the late reference is an RValue; this allows you to generate
        /// a run-time exception for late assignments to fields of RValues of value types. Otherwise, set to False.
        /// </param>
        /// <param name="CallType">
        /// An enumeration member of type <see cref="CallType" /> representing
        /// the type of procedure being called. The value of CallType can be Method, Get, or Set. Only
        /// Set is used.
        /// </param>
        public static void LateSet(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool OptimisticSet, bool RValueBase, Microsoft.VisualBasic.CallType CallType) { }
        /// <summary>
        /// Executes a late-bound property set or field write call. This helper method is not meant to
        /// be called directly from your code.
        /// </summary>
        /// <param name="Instance">An instance of the call object exposing the property or method.</param>
        /// <param name="Type">The type of the call object.</param>
        /// <param name="MemberName">The name of the property or method on the call object.</param>
        /// <param name="Arguments">
        /// An array containing the arguments to be passed to the property or method being called.
        /// </param>
        /// <param name="ArgumentNames">An array of argument names.</param>
        /// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
        /// <param name="OptimisticSet">
        /// A Boolean value used to determine whether the set operation will work. Set to True when you
        /// believe that an intermediate value has been set in the property or field; otherwise False.
        /// </param>
        /// <param name="RValueBase">
        /// A Boolean value that specifies when the base reference of the late reference is an RValue.
        /// Set to True when the base reference of the late reference is an RValue; this allows you to generate
        /// a run-time exception for late assignments to fields of RValues of value types. Otherwise, set to False.
        /// </param>
        public static void LateSetComplex(object Instance, System.Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, System.Type[] TypeArguments, bool OptimisticSet, bool RValueBase) { }
    }
    /// <summary>
    /// The Visual Basic compiler uses this class for object flow control; it is not meant to be called
    /// directly from your code.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class ObjectFlowControl
    {
        internal ObjectFlowControl() { }
        /// <summary>
        /// Checks for a synchronization lock on the specified type.
        /// </summary>
        /// <param name="Expression">The data type for which to check for synchronization lock.</param>
        public static void CheckForSyncLockOnValueType(object Expression) { }
        /// <summary>
        /// Provides services to the Visual Basic compiler for compiling For...Next loops.
        /// </summary>
        [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
        public sealed partial class ForLoopControl
        {
            internal ForLoopControl() { }
            /// <summary>
            /// Initializes a For...Next loop.
            /// </summary>
            /// <param name="Counter">The loop counter variable.</param>
            /// <param name="Start">The initial value of the loop counter.</param>
            /// <param name="Limit">The value of the To option.</param>
            /// <param name="StepValue">The value of the Step option.</param>
            /// <param name="LoopForResult">An object that contains verified values for loop values.</param>
            /// <param name="CounterResult">The counter value for the next loop iteration.</param>
            /// <returns>
            /// False if the loop has terminated; otherwise, True.
            /// </returns>
            public static bool ForLoopInitObj(object Counter, object Start, object Limit, object StepValue, ref object LoopForResult, ref object CounterResult) { return default(bool); }
            /// <summary>
            /// Checks for valid values for the loop counter, Step, and To values.
            /// </summary>
            /// <param name="count">
            /// Required. A Decimal value that represents the initial value passed for the loop counter variable.
            /// </param>
            /// <param name="limit">Required. A Decimal value that represents the value passed by using the To keyword.</param>
            /// <param name="StepValue">Required. A Decimal value that represents the value passed by using the Step keyword.</param>
            /// <returns>
            /// True if <paramref name="StepValue" /> is greater than zero and <paramref name="count" /> is
            /// less than or equal to <paramref name="limit" /> or <paramref name="StepValue" /> is less than
            /// or equal to zero and <paramref name="count" /> is greater than or equal to <paramref name="limit" />
            /// ; otherwise, False.
            /// </returns>
            public static bool ForNextCheckDec(decimal count, decimal limit, decimal StepValue) { return default(bool); }
            /// <summary>
            /// Increments a For...Next loop.
            /// </summary>
            /// <param name="Counter">The loop counter variable.</param>
            /// <param name="LoopObj">An object that contains verified values for loop values.</param>
            /// <param name="CounterResult">The counter value for the next loop iteration.</param>
            /// <returns>
            /// False if the loop has terminated; otherwise, True.
            /// </returns>
            public static bool ForNextCheckObj(object Counter, object LoopObj, ref object CounterResult) { return default(bool); }
            /// <summary>
            /// Checks for valid values for the loop counter, Step, and To values.
            /// </summary>
            /// <param name="count">
            /// Required. A Single value that represents the initial value passed for the loop counter variable.
            /// </param>
            /// <param name="limit">Required. A Single value that represents the value passed by using the To keyword.</param>
            /// <param name="StepValue">Required. A Single value that represents the value passed by using the Step keyword.</param>
            /// <returns>
            /// True if <paramref name="StepValue" /> is greater than zero and <paramref name="count" /> is
            /// less than or equal to <paramref name="limit" />, or if <paramref name="StepValue" /> is less
            /// than or equal to zero and <paramref name="count" /> is greater than or equal to <paramref name="limit" />
            /// ; otherwise, False.
            /// </returns>
            public static bool ForNextCheckR4(float count, float limit, float StepValue) { return default(bool); }
            /// <summary>
            /// Checks for valid values for the loop counter, Step, and To values.
            /// </summary>
            /// <param name="count">
            /// Required. A Double value that represents the initial value passed for the loop counter variable.
            /// </param>
            /// <param name="limit">Required. A Double value that represents the value passed by using the To keyword.</param>
            /// <param name="StepValue">Required. A Double value that represents the value passed by using the Step keyword.</param>
            /// <returns>
            /// True if <paramref name="StepValue" /> is greater than zero and <paramref name="count" /> is
            /// less than or equal to <paramref name="limit" />, or if <paramref name="StepValue" /> is less
            /// than or equal to zero and <paramref name="count" /> is greater than or equal to <paramref name="limit" />
            /// ; otherwise, False.
            /// </returns>
            public static bool ForNextCheckR8(double count, double limit, double StepValue) { return default(bool); }
        }
    }
    /// <summary>
    /// Provides late-bound math operators, such as
    /// <see cref="AddObject(System.Object,System.Object)" /> and
    /// <see cref="Operators.CompareObject(System.Object,System.Object,System.Boolean)" />, which the Visual Basic compiler uses internally.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class Operators
    {
        internal Operators() { }
        /// <summary>
        /// Represents the Visual Basic addition (+) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The sum of <paramref name="Left" /> and <paramref name="Right" />.
        /// </returns>
        public static object AddObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic And operator.
        /// </summary>
        /// <param name="Left">Required. Any Boolean or numeric expression.</param>
        /// <param name="Right">Required. Any Boolean or numeric expression.</param>
        /// <returns>
        /// For Boolean operations, True if both <paramref name="Left" /> and <paramref name="Right" />
        /// evaluate to True; otherwise, False. For bitwise operations, 1 if both <paramref name="Left" />
        /// and <paramref name="Right" /> evaluate to 1; otherwise, 0.
        /// </returns>
        public static object AndObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic equal (=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> and <paramref name="Right" /> are equal; otherwise, False.
        /// </returns>
        public static object CompareObjectEqual(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic greater-than (&gt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> is greater than <paramref name="Right" />; otherwise, False.
        /// </returns>
        public static object CompareObjectGreater(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic greater-than or equal-to (&gt;=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> is greater than or equal to <paramref name="Right" />; otherwise,
        /// False.
        /// </returns>
        public static object CompareObjectGreaterEqual(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic less-than (&lt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> is less than <paramref name="Right" />; otherwise, False.
        /// </returns>
        public static object CompareObjectLess(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic less-than or equal-to (&lt;=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> is less than or equal to <paramref name="Right" />; otherwise,
        /// False.
        /// </returns>
        public static object CompareObjectLessEqual(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic not-equal (&lt;&gt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// True if <paramref name="Left" /> is not equal to <paramref name="Right" />; otherwise, False.
        /// </returns>
        public static object CompareObjectNotEqual(object Left, object Right, bool TextCompare) { return default(object); }
        /// <summary>
        /// Performs binary or text string comparison when given two strings.
        /// </summary>
        /// <param name="Left">Required. Any String expression.</param>
        /// <param name="Right">Required. Any String expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// Value Condition -1 <paramref name="Left" /> is less than <paramref name="Right" />. 0<paramref name="Left" />
        /// is equal to <paramref name="Right" />. 1 <paramref name="Left" /> is greater
        /// than <paramref name="Right" />.
        /// </returns>
        public static int CompareString(string Left, string Right, bool TextCompare) { return default(int); }
        /// <summary>
        /// Represents the Visual Basic concatenation (&amp;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <returns>
        /// A string representing the concatenation of <paramref name="Left" /> and <paramref name="Right" />.
        /// </returns>
        public static object ConcatenateObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the overloaded Visual Basic equals (=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded equals operator. False if operator overloading is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectEqual(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the overloaded Visual Basic greater-than (&gt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded greater-than operator. False if operator overloading is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectGreater(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the overloaded Visual Basic greater-than or equal-to (&gt;=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded greater-than or equal-to operator. False if operator overloading
        /// is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectGreaterEqual(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the overloaded Visual Basic less-than (&lt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded less-than operator. False if operator overloading is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectLess(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the overloaded Visual Basic less-than or equal-to (&lt;=) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded less-than or equal-to operator. False if operator overloading
        /// is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectLessEqual(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the overloaded Visual Basic not-equal (&lt;&gt;) operator.
        /// </summary>
        /// <param name="Left">Required. Any expression.</param>
        /// <param name="Right">Required. Any expression.</param>
        /// <param name="TextCompare">Required. True to perform a case-insensitive string comparison; otherwise, False.</param>
        /// <returns>
        /// The result of the overloaded not-equal operator. False if operator overloading is not supported.
        /// </returns>
        public static bool ConditionalCompareObjectNotEqual(object Left, object Right, bool TextCompare) { return default(bool); }
        /// <summary>
        /// Represents the Visual Basic division (/) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The full quotient of <paramref name="Left" /> divided by <paramref name="Right" />, including
        /// any remainder.
        /// </returns>
        public static object DivideObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic exponent (^) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The result of <paramref name="Left" /> raised to the power of <paramref name="Right" />.
        /// </returns>
        public static object ExponentObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic integer division (\) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The integer quotient of <paramref name="Left" /> divided by <paramref name="Right" />, which
        /// discards any remainder and retains only the integer portion.
        /// </returns>
        public static object IntDivideObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic arithmetic left shift (&lt;&lt;) operator.
        /// </summary>
        /// <param name="Operand">
        /// Required. Integral numeric expression. The bit pattern to be shifted. The data type must be
        /// an integral type (SByte, Byte, Short, UShort, Integer, UInteger, Long, or ULong).
        /// </param>
        /// <param name="Amount">
        /// Required. Numeric expression. The number of bits to shift the bit pattern. The data type must
        /// be Integer or widen to Integer.
        /// </param>
        /// <returns>
        /// An integral numeric value. The result of shifting the bit pattern. The data type is the same
        /// as that of <paramref name="Operand" />.
        /// </returns>
        public static object LeftShiftObject(object Operand, object Amount) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic Mod operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The remainder after <paramref name="Left" /> is divided by <paramref name="Right" />.
        /// </returns>
        public static object ModObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic multiply (*) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The product of <paramref name="Left" /> and <paramref name="Right" />.
        /// </returns>
        public static object MultiplyObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic unary minus (–) operator.
        /// </summary>
        /// <param name="Operand">Required. Any numeric expression.</param>
        /// <returns>
        /// The negative value of <paramref name="Operand" />.
        /// </returns>
        public static object NegateObject(object Operand) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic Not operator.
        /// </summary>
        /// <param name="Operand">Required. Any Boolean or numeric expression.</param>
        /// <returns>
        /// For Boolean operations, False if <paramref name="Operand" /> is True; otherwise, True. For
        /// bitwise operations, 1 if <paramref name="Operand" /> is 0; otherwise, 0.
        /// </returns>
        public static object NotObject(object Operand) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic Or operator.
        /// </summary>
        /// <param name="Left">Required. Any Boolean or numeric expression.</param>
        /// <param name="Right">Required. Any Boolean or numeric expression.</param>
        /// <returns>
        /// For Boolean operations, False if both <paramref name="Left" /> and <paramref name="Right" />
        /// evaluate to False; otherwise, True. For bitwise operations, 0 if both <paramref name="Left" />
        /// and <paramref name="Right" /> evaluate to 0; otherwise, 1.
        /// </returns>
        public static object OrObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic unary plus (+) operator.
        /// </summary>
        /// <param name="Operand">Required. Any numeric expression.</param>
        /// <returns>
        /// The value of <paramref name="Operand" />. (The sign of the <paramref name="Operand" /> is
        /// unchanged.)
        /// </returns>
        public static object PlusObject(object Operand) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic arithmetic right shift (&gt;&gt;) operator.
        /// </summary>
        /// <param name="Operand">
        /// Required. Integral numeric expression. The bit pattern to be shifted. The data type must be
        /// an integral type (SByte, Byte, Short, UShort, Integer, UInteger, Long, or ULong).
        /// </param>
        /// <param name="Amount">
        /// Required. Numeric expression. The number of bits to shift the bit pattern. The data type must
        /// be Integer or widen to Integer.
        /// </param>
        /// <returns>
        /// An integral numeric value. The result of shifting the bit pattern. The data type is the same
        /// as that of <paramref name="Operand" />.
        /// </returns>
        public static object RightShiftObject(object Operand, object Amount) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic subtraction (–) operator.
        /// </summary>
        /// <param name="Left">Required. Any numeric expression.</param>
        /// <param name="Right">Required. Any numeric expression.</param>
        /// <returns>
        /// The difference between <paramref name="Left" /> and <paramref name="Right" />.
        /// </returns>
        public static object SubtractObject(object Left, object Right) { return default(object); }
        /// <summary>
        /// Represents the Visual Basic Xor operator.
        /// </summary>
        /// <param name="Left">Required. Any Boolean or numeric expression.</param>
        /// <param name="Right">Required. Any Boolean or numeric expression.</param>
        /// <returns>
        /// A Boolean or numeric value. For a Boolean comparison, the return value is the logical exclusion
        /// (exclusive logical disjunction) of two Boolean values. For bitwise (numeric) operations, the return
        /// value is a numeric value that represents the bitwise exclusion (exclusive bitwise disjunction)
        /// of two numeric bit patterns. For more information, see Xor Operator (Visual Basic).
        /// </returns>
        public static object XorObject(object Left, object Right) { return default(object); }
    }
    /// <summary>
    /// Specifies that the current Option Compare setting should be passed as the default value for
    /// an argument.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2048), Inherited = false, AllowMultiple = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class OptionCompareAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="OptionCompareAttribute" /> class.
        /// </summary>
        public OptionCompareAttribute() { }
    }
    /// <summary>
    /// The Visual Basic compiler emits this helper class to indicate (for Visual Basic debugging)
    /// which comparison option, binary or text, is being used
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), Inherited = false, AllowMultiple = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class OptionTextAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionTextAttribute" />
        /// class. This is a helper method.
        /// </summary>
        public OptionTextAttribute() { }
    }
    /// <summary>
    /// Provides helpers for the Visual Basic Err object.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class ProjectData
    {
        internal ProjectData() { }
        /// <summary>
        /// Performs the work for the Clear method of the Err object. A helper method.
        /// </summary>
        [System.Security.SecuritySafeCriticalAttribute]
        public static void ClearProjectError() { }
        /// <summary>
        /// The Visual Basic compiler uses this helper method to capture exceptions in the Err object.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception" /> object to be caught.</param>
        [System.Security.SecuritySafeCriticalAttribute]
        public static void SetProjectError(System.Exception ex) { }
        /// <summary>
        /// The Visual Basic compiler uses this helper method to capture exceptions in the Err object.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception" /> object to be caught.</param>
        /// <param name="lErl">The line number of the exception.</param>
        [System.Security.SecuritySafeCriticalAttribute]
        public static void SetProjectError(System.Exception ex, int lErl) { }
    }
    /// <summary>
    /// This class provides attributes that are applied to the standard module construct when it is
    /// emitted to Intermediate Language (IL). It is not intended to be called directly from your code.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), Inherited = false, AllowMultiple = false)]
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class StandardModuleAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="StandardModuleAttribute" /> class.
        /// </summary>
        public StandardModuleAttribute() { }
    }
    /// <summary>
    /// The Visual Basic compiler uses this class internally when initializing static local members;
    /// it is not meant to be called directly from your code.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class StaticLocalInitFlag
    {
        /// <summary>
        /// Returns the state of the static local member's initialization flag (initialized or not).
        /// </summary>
        public short State;
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticLocalInitFlag" />
        /// class.
        /// </summary>
        public StaticLocalInitFlag() { }
    }
    /// <summary>
    /// Contains utilities that the Visual Basic compiler uses.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class Utils
    {
        internal Utils() { }
        /// <summary>
        /// Used by the Visual Basic compiler as a helper for Redim.
        /// </summary>
        /// <param name="arySrc">The array to be copied.</param>
        /// <param name="aryDest">The destination array.</param>
        /// <returns>
        /// The copied array.
        /// </returns>
        public static System.Array CopyArray(System.Array arySrc, System.Array aryDest) { return default(System.Array); }
    }
}
