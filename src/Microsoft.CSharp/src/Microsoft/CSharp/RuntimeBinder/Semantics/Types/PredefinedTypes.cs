// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class PredefinedTypes
    {
        private SymbolTable _runtimeBinderSymbolTable;
        private readonly BSYMMGR _pBSymmgr;
        private AggregateSymbol[] _predefSyms;    // array of predefined symbol types.

        public PredefinedTypes(BSYMMGR pBSymmgr)
        {
            _pBSymmgr = pBSymmgr;
            _runtimeBinderSymbolTable = null;
        }

        // We want to delay load the predef syms as needed.
        private AggregateSymbol DelayLoadPredefSym(PredefinedType pt)
        {
            CType type = _runtimeBinderSymbolTable.GetCTypeFromType(PredefinedTypeFacts.GetAssociatedSystemType(pt));
            AggregateSymbol sym = type.getAggregate();

            // If we failed to load this thing, we have problems.
            if (sym == null)
            {
                return null;
            }
            return InitializePredefinedType(sym, pt);
        }

        internal static AggregateSymbol InitializePredefinedType(AggregateSymbol sym, PredefinedType pt)
        {
            sym.SetPredefined(true);
            sym.SetPredefType(pt);
            sym.SetSkipUDOps(pt <= PredefinedType.PT_ENUM && pt != PredefinedType.PT_INTPTR && pt != PredefinedType.PT_UINTPTR && pt != PredefinedType.PT_TYPE);

            return sym;
        }

        public void Init(SymbolTable symtable)
        {
            _runtimeBinderSymbolTable = symtable;
            Debug.Assert(_pBSymmgr != null);
            Debug.Assert(_predefSyms == null);

            _predefSyms = new AggregateSymbol[(int)PredefinedType.PT_COUNT];
        }

        public void ReportMissingPredefTypeError(ErrorHandling errorContext, PredefinedType pt)
        {
            Debug.Assert(_pBSymmgr != null);
            Debug.Assert(_predefSyms != null);
            Debug.Assert((PredefinedType)0 <= pt && pt < PredefinedType.PT_COUNT && _predefSyms[(int)pt] == null);

            // We do not assert that !predefTypeInfo[pt].isRequired because if the user is defining
            // their own MSCorLib and is defining a required PredefType, they'll run into this error
            // and we need to allow it to go through.

            errorContext.Error(ErrorCode.ERR_PredefinedTypeNotFound, PredefinedTypeFacts.GetName(pt));
        }

        public AggregateSymbol GetReqPredefAgg(PredefinedType pt)
        {
            if (!PredefinedTypeFacts.IsRequired(pt)) throw Error.InternalCompilerError();
            if (_predefSyms[(int)pt] == null)
            {
                // Delay load this thing.
                _predefSyms[(int)pt] = DelayLoadPredefSym(pt);
            }
            return _predefSyms[(int)pt];
        }

        public AggregateSymbol GetOptPredefAgg(PredefinedType pt)
        {
            if (_predefSyms[(int)pt] == null)
            {
                // Delay load this thing.
                _predefSyms[(int)pt] = DelayLoadPredefSym(pt);
            }

            Debug.Assert(_predefSyms != null);
            return _predefSyms[(int)pt];
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Some of the predefined types have built-in names, like "int" or "string" or
        // "object". This return the nice name if one exists; otherwise null is 
        // returned.

        private static string GetNiceName(PredefinedType pt)
        {
            return PredefinedTypeFacts.GetNiceName(pt);
        }

        public static string GetNiceName(AggregateSymbol type)
        {
            if (type.IsPredefined())
                return GetNiceName(type.GetPredefType());
            else
                return null;
        }

        public static string GetFullName(PredefinedType pt)
        {
            return PredefinedTypeFacts.GetName(pt);
        }

        public static bool isRequired(PredefinedType pt)
        {
            return PredefinedTypeFacts.IsRequired(pt);
        }
    }

    internal static class PredefinedTypeFacts
    {
        internal static string GetName(PredefinedType type)
        {
            return s_pdTypes[(int)type].name;
        }

        internal static bool IsRequired(PredefinedType type)
        {
            return s_pdTypes[(int)type].required;
        }

        internal static FUNDTYPE GetFundType(PredefinedType type)
        {
            return s_pdTypes[(int)type].fundType;
        }

        internal static Type GetAssociatedSystemType(PredefinedType type)
        {
            return s_pdTypes[(int)type].AssociatedSystemType;
        }

        internal static bool IsSimpleType(PredefinedType type)
        {
            switch (type)
            {
                case PredefinedType.PT_BYTE:
                case PredefinedType.PT_SHORT:
                case PredefinedType.PT_INT:
                case PredefinedType.PT_LONG:
                case PredefinedType.PT_FLOAT:
                case PredefinedType.PT_DOUBLE:
                case PredefinedType.PT_DECIMAL:
                case PredefinedType.PT_CHAR:
                case PredefinedType.PT_BOOL:
                case PredefinedType.PT_SBYTE:
                case PredefinedType.PT_USHORT:
                case PredefinedType.PT_UINT:
                case PredefinedType.PT_ULONG:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsNumericType(PredefinedType type)
        {
            switch (type)
            {
                case PredefinedType.PT_BYTE:
                case PredefinedType.PT_SHORT:
                case PredefinedType.PT_INT:
                case PredefinedType.PT_LONG:
                case PredefinedType.PT_FLOAT:
                case PredefinedType.PT_DOUBLE:
                case PredefinedType.PT_DECIMAL:
                case PredefinedType.PT_SBYTE:
                case PredefinedType.PT_USHORT:
                case PredefinedType.PT_UINT:
                case PredefinedType.PT_ULONG:
                    return true;
                default:
                    return false;
            }
        }

        internal static string GetNiceName(PredefinedType type)
        {
            switch (type)
            {
                case PredefinedType.PT_BYTE:
                    return "byte";
                case PredefinedType.PT_SHORT:
                    return "short";
                case PredefinedType.PT_INT:
                    return "int";
                case PredefinedType.PT_LONG:
                    return "long";
                case PredefinedType.PT_FLOAT:
                    return "float";
                case PredefinedType.PT_DOUBLE:
                    return "double";
                case PredefinedType.PT_DECIMAL:
                    return "decimal";
                case PredefinedType.PT_CHAR:
                    return "char";
                case PredefinedType.PT_BOOL:
                    return "bool";
                case PredefinedType.PT_SBYTE:
                    return "sbyte";
                case PredefinedType.PT_USHORT:
                    return "ushort";
                case PredefinedType.PT_UINT:
                    return "uint";
                case PredefinedType.PT_ULONG:
                    return "ulong";
                case PredefinedType.PT_OBJECT:
                    return "object";
                case PredefinedType.PT_STRING:
                    return "string";
                default:
                    return null;
            }
        }

        internal static bool IsPredefinedType(string name)
        {
            return s_pdTypeNames.ContainsKey(name);
        }

        internal static PredefinedType GetPredefTypeIndex(string name)
        {
            return s_pdTypeNames[name];
        }

        private sealed class PredefinedTypeInfo
        {
            internal readonly PredefinedType type;
            internal readonly string name;
            internal readonly bool required;
            internal readonly FUNDTYPE fundType;
            internal readonly Type AssociatedSystemType;

            internal PredefinedTypeInfo(PredefinedType type, Type associatedSystemType, string name, bool required, FUNDTYPE fundType)
            {
                this.type = type;
                this.name = name;
                this.required = required;
                this.fundType = fundType;
                AssociatedSystemType = associatedSystemType;
            }

            internal PredefinedTypeInfo(PredefinedType type, Type associatedSystemType, string name, bool required)
                : this(type, associatedSystemType, name, required, FUNDTYPE.FT_REF)
            {
            }
        }

        private static readonly PredefinedTypeInfo[] s_pdTypes = new PredefinedTypeInfo[] {
            new PredefinedTypeInfo(PredefinedType.PT_BYTE,   typeof(byte), "System.Byte", true, FUNDTYPE.FT_U1),
            new PredefinedTypeInfo(PredefinedType.PT_SHORT,  typeof(short), "System.Int16", true, FUNDTYPE.FT_I2),
            new PredefinedTypeInfo(PredefinedType.PT_INT,    typeof(int), "System.Int32", true, FUNDTYPE.FT_I4),
            new PredefinedTypeInfo(PredefinedType.PT_LONG,   typeof(long), "System.Int64", true, FUNDTYPE.FT_I8),
            new PredefinedTypeInfo(PredefinedType.PT_FLOAT,  typeof(float), "System.Single", true, FUNDTYPE.FT_R4),
            new PredefinedTypeInfo(PredefinedType.PT_DOUBLE, typeof(double), "System.Double", true, FUNDTYPE.FT_R8),
            new PredefinedTypeInfo(PredefinedType.PT_DECIMAL, typeof(decimal), "System.Decimal", false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_CHAR,   typeof(char), "System.Char", true, FUNDTYPE.FT_U2),
            new PredefinedTypeInfo(PredefinedType.PT_BOOL,   typeof(bool), "System.Boolean", true, FUNDTYPE.FT_I1),
            new PredefinedTypeInfo(PredefinedType.PT_SBYTE,  typeof(sbyte), "System.SByte", true, FUNDTYPE.FT_I1),
            new PredefinedTypeInfo(PredefinedType.PT_USHORT, typeof(ushort), "System.UInt16", true, FUNDTYPE.FT_U2),
            new PredefinedTypeInfo(PredefinedType.PT_UINT,   typeof(uint), "System.UInt32", true, FUNDTYPE.FT_U4),
            new PredefinedTypeInfo(PredefinedType.PT_ULONG,  typeof(ulong), "System.UInt64", true, FUNDTYPE.FT_U8),
            new PredefinedTypeInfo(PredefinedType.PT_INTPTR,  typeof(IntPtr), "System.IntPtr", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_UINTPTR, typeof(UIntPtr), "System.UIntPtr", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_OBJECT, typeof(object), "System.Object", true),
            new PredefinedTypeInfo(PredefinedType.PT_STRING, typeof(string), "System.String", true),
            new PredefinedTypeInfo(PredefinedType.PT_DELEGATE, typeof(Delegate), "System.Delegate", true),
            new PredefinedTypeInfo(PredefinedType.PT_MULTIDEL, typeof(MulticastDelegate), "System.MulticastDelegate", true),
            new PredefinedTypeInfo(PredefinedType.PT_ARRAY,   typeof(Array), "System.Array", true),
            new PredefinedTypeInfo(PredefinedType.PT_EXCEPTION, typeof(Exception), "System.Exception", true),
            new PredefinedTypeInfo(PredefinedType.PT_TYPE, typeof(Type), "System.Type", true),
            new PredefinedTypeInfo(PredefinedType.PT_MONITOR, typeof(System.Threading.Monitor), "System.Threading.Monitor", true),
            new PredefinedTypeInfo(PredefinedType.PT_VALUE,   typeof(ValueType), "System.ValueType", true),
            new PredefinedTypeInfo(PredefinedType.PT_ENUM,    typeof(Enum), "System.Enum", true),
            new PredefinedTypeInfo(PredefinedType.PT_DATETIME,    typeof(DateTime), "System.DateTime", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGABLEATTRIBUTE, typeof(DebuggableAttribute), "System.Diagnostics.DebuggableAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGABLEATTRIBUTE_DEBUGGINGMODES, typeof(DebuggableAttribute.DebuggingModes), "System.Diagnostics.DebuggableAttribute.DebuggingModes", false),
            new PredefinedTypeInfo(PredefinedType.PT_IN,            typeof(System.Runtime.InteropServices.InAttribute), "System.Runtime.InteropServices.InAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_OUT,           typeof(System.Runtime.InteropServices.OutAttribute), "System.Runtime.InteropServices.OutAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ATTRIBUTE, typeof(Attribute), "System.Attribute", true),
            new PredefinedTypeInfo(PredefinedType.PT_ATTRIBUTEUSAGE, typeof(AttributeUsageAttribute), "System.AttributeUsageAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ATTRIBUTETARGETS, typeof(AttributeTargets), "System.AttributeTargets", false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_OBSOLETE, typeof(ObsoleteAttribute), "System.ObsoleteAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_CONDITIONAL, typeof(ConditionalAttribute), "System.Diagnostics.ConditionalAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_CLSCOMPLIANT, typeof(CLSCompliantAttribute), "System.CLSCompliantAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_GUID, typeof(System.Runtime.InteropServices.GuidAttribute), "System.Runtime.InteropServices.GuidAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEFAULTMEMBER, typeof(System.Reflection.DefaultMemberAttribute), "System.Reflection.DefaultMemberAttribute", true),
            new PredefinedTypeInfo(PredefinedType.PT_PARAMS, typeof(ParamArrayAttribute), "System.ParamArrayAttribute", true),
            new PredefinedTypeInfo(PredefinedType.PT_COMIMPORT, typeof(System.Runtime.InteropServices.ComImportAttribute), "System.Runtime.InteropServices.ComImportAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_FIELDOFFSET, typeof(System.Runtime.InteropServices.FieldOffsetAttribute), "System.Runtime.InteropServices.FieldOffsetAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_STRUCTLAYOUT, typeof(System.Runtime.InteropServices.StructLayoutAttribute), "System.Runtime.InteropServices.StructLayoutAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_LAYOUTKIND, typeof(System.Runtime.InteropServices.LayoutKind), "System.Runtime.InteropServices.LayoutKind", false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_MARSHALAS, typeof(System.Runtime.InteropServices.MarshalAsAttribute), "System.Runtime.InteropServices.MarshalAsAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DLLIMPORT, typeof(System.Runtime.InteropServices.DllImportAttribute), "System.Runtime.InteropServices.DllImportAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_INDEXERNAME, typeof(System.Runtime.CompilerServices.IndexerNameAttribute), "System.Runtime.CompilerServices.IndexerNameAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DECIMALCONSTANT, typeof(System.Runtime.CompilerServices.DecimalConstantAttribute), "System.Runtime.CompilerServices.DecimalConstantAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEFAULTVALUE, typeof(System.Runtime.InteropServices.DefaultParameterValueAttribute), "System.Runtime.InteropServices.DefaultParameterValueAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_UNMANAGEDFUNCTIONPOINTER, typeof(System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute), "System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_CALLINGCONVENTION, typeof(System.Runtime.InteropServices.CallingConvention), "System.Runtime.InteropServices.CallingConvention", false, FUNDTYPE.FT_I4),
            new PredefinedTypeInfo(PredefinedType.PT_CHARSET, typeof(System.Runtime.InteropServices.CharSet), "System.Runtime.InteropServices.CharSet", false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_TYPEHANDLE, typeof(RuntimeTypeHandle), "System.RuntimeTypeHandle", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_FIELDHANDLE, typeof(RuntimeFieldHandle), "System.RuntimeFieldHandle", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_METHODHANDLE, typeof(RuntimeMethodHandle), "System.RuntimeMethodHandle", false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_G_DICTIONARY, typeof(Dictionary<,>), "System.Collections.Generic.Dictionary`2", false),
            new PredefinedTypeInfo(PredefinedType.PT_IASYNCRESULT, typeof(IAsyncResult), "System.IAsyncResult", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_ASYNCCBDEL, typeof(AsyncCallback), "System.AsyncCallback",  false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_IDISPOSABLE, typeof(IDisposable), "System.IDisposable",   true, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_IENUMERABLE, typeof(System.Collections.IEnumerable), "System.Collections.IEnumerable", true, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_IENUMERATOR, typeof(System.Collections.IEnumerator), "System.Collections.IEnumerator", true, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_SYSTEMVOID, typeof(void), "System.Void", true, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_RUNTIMEHELPERS, typeof(System.Runtime.CompilerServices.RuntimeHelpers), "System.Runtime.CompilerServices.RuntimeHelpers", false),
            new PredefinedTypeInfo(PredefinedType.PT_VOLATILEMOD, typeof(System.Runtime.CompilerServices.IsVolatile), "System.Runtime.CompilerServices.IsVolatile", false),
            new PredefinedTypeInfo(PredefinedType.PT_COCLASS,    typeof(System.Runtime.InteropServices.CoClassAttribute), "System.Runtime.InteropServices.CoClassAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ACTIVATOR,  typeof(Activator), "System.Activator",  false),
            new PredefinedTypeInfo(PredefinedType.PT_G_IENUMERABLE, typeof(IEnumerable<>), "System.Collections.Generic.IEnumerable`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_G_IENUMERATOR, typeof(IEnumerator<>), "System.Collections.Generic.IEnumerator`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_G_OPTIONAL, typeof(Nullable<>), "System.Nullable`1",  false, FUNDTYPE.FT_STRUCT),
            new PredefinedTypeInfo(PredefinedType.PT_FIXEDBUFFER, typeof(System.Runtime.CompilerServices.FixedBufferAttribute), "System.Runtime.CompilerServices.FixedBufferAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEFAULTCHARSET, typeof(System.Runtime.InteropServices.DefaultCharSetAttribute), "System.Runtime.InteropServices.DefaultCharSetAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_COMPILATIONRELAXATIONS, typeof(System.Runtime.CompilerServices.CompilationRelaxationsAttribute), "System.Runtime.CompilerServices.CompilationRelaxationsAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_RUNTIMECOMPATIBILITY, typeof(System.Runtime.CompilerServices.RuntimeCompatibilityAttribute), "System.Runtime.CompilerServices.RuntimeCompatibilityAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_FRIENDASSEMBLY, typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute), "System.Runtime.CompilerServices.InternalsVisibleToAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGERHIDDEN, typeof(DebuggerHiddenAttribute), "System.Diagnostics.DebuggerHiddenAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_TYPEFORWARDER, typeof(System.Runtime.CompilerServices.TypeForwardedToAttribute), "System.Runtime.CompilerServices.TypeForwardedToAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_KEYFILE, typeof(System.Reflection.AssemblyKeyFileAttribute), "System.Reflection.AssemblyKeyFileAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_KEYNAME, typeof(System.Reflection.AssemblyKeyNameAttribute), "System.Reflection.AssemblyKeyNameAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DELAYSIGN, typeof(System.Reflection.AssemblyDelaySignAttribute), "System.Reflection.AssemblyDelaySignAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_NOTSUPPORTEDEXCEPTION, typeof(NotSupportedException), "System.NotSupportedException", false),
            new PredefinedTypeInfo(PredefinedType.PT_COMPILERGENERATED, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), "System.Runtime.CompilerServices.CompilerGeneratedAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_UNSAFEVALUETYPE, typeof(System.Runtime.CompilerServices.UnsafeValueTypeAttribute), "System.Runtime.CompilerServices.UnsafeValueTypeAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ASSEMBLYFLAGS, typeof(System.Reflection.AssemblyFlagsAttribute), "System.Reflection.AssemblyFlagsAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ASSEMBLYVERSION, typeof(System.Reflection.AssemblyVersionAttribute), "System.Reflection.AssemblyVersionAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_ASSEMBLYCULTURE, typeof(System.Reflection.AssemblyCultureAttribute), "System.Reflection.AssemblyCultureAttribute", false),
            // LINQ
            new PredefinedTypeInfo(PredefinedType.PT_G_IQUERYABLE, typeof(System.Linq.IQueryable<>), "System.Linq.IQueryable`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_IQUERYABLE, typeof(System.Linq.IQueryable), "System.Linq.IQueryable", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_STRINGBUILDER, typeof(System.Text.StringBuilder), "System.Text.StringBuilder", false),
            new PredefinedTypeInfo(PredefinedType.PT_G_ICOLLECTION, typeof(ICollection<>), "System.Collections.Generic.ICollection`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_G_ILIST, typeof(IList<>), "System.Collections.Generic.IList`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_EXTENSION, typeof(System.Runtime.CompilerServices.ExtensionAttribute), "System.Runtime.CompilerServices.ExtensionAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_G_EXPRESSION, typeof(System.Linq.Expressions.Expression<>), "System.Linq.Expressions.Expression`1", false),
            new PredefinedTypeInfo(PredefinedType.PT_EXPRESSION, typeof(System.Linq.Expressions.Expression), "System.Linq.Expressions.Expression", false),
            new PredefinedTypeInfo(PredefinedType.PT_LAMBDAEXPRESSION, typeof(System.Linq.Expressions.LambdaExpression), "System.Linq.Expressions.LambdaExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_BINARYEXPRESSION, typeof(System.Linq.Expressions.BinaryExpression), "System.Linq.Expressions.BinaryExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_UNARYEXPRESSION, typeof(System.Linq.Expressions.UnaryExpression), "System.Linq.Expressions.UnaryExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_CONDITIONALEXPRESSION, typeof(System.Linq.Expressions.ConditionalExpression), "System.Linq.Expressions.ConditionalExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_CONSTANTEXPRESSION, typeof(System.Linq.Expressions.ConstantExpression), "System.Linq.Expressions.ConstantExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_PARAMETEREXPRESSION, typeof(System.Linq.Expressions.ParameterExpression), "System.Linq.Expressions.ParameterExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBEREXPRESSION, typeof(System.Linq.Expressions.MemberExpression), "System.Linq.Expressions.MemberExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_METHODCALLEXPRESSION, typeof(System.Linq.Expressions.MethodCallExpression), "System.Linq.Expressions.MethodCallExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_NEWEXPRESSION, typeof(System.Linq.Expressions.NewExpression), "System.Linq.Expressions.NewExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_BINDING, typeof(System.Linq.Expressions.MemberBinding), "System.Linq.Expressions.MemberBinding", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBERINITEXPRESSION, typeof(System.Linq.Expressions.MemberInitExpression), "System.Linq.Expressions.MemberInitExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_LISTINITEXPRESSION, typeof(System.Linq.Expressions.ListInitExpression), "System.Linq.Expressions.ListInitExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_TYPEBINARYEXPRESSION, typeof(System.Linq.Expressions.TypeBinaryExpression), "System.Linq.Expressions.TypeBinaryExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_NEWARRAYEXPRESSION, typeof(System.Linq.Expressions.NewArrayExpression), "System.Linq.Expressions.NewArrayExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBERASSIGNMENT, typeof(System.Linq.Expressions.MemberAssignment), "System.Linq.Expressions.MemberAssignment", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBERLISTBINDING, typeof(System.Linq.Expressions.MemberListBinding), "System.Linq.Expressions.MemberListBinding", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBERMEMBERBINDING, typeof(System.Linq.Expressions.MemberMemberBinding), "System.Linq.Expressions.MemberMemberBinding", false),
            new PredefinedTypeInfo(PredefinedType.PT_INVOCATIONEXPRESSION, typeof(System.Linq.Expressions.InvocationExpression), "System.Linq.Expressions.InvocationExpression", false),
            new PredefinedTypeInfo(PredefinedType.PT_FIELDINFO, typeof(System.Reflection.FieldInfo), "System.Reflection.FieldInfo", false),
            new PredefinedTypeInfo(PredefinedType.PT_METHODINFO, typeof(System.Reflection.MethodInfo), "System.Reflection.MethodInfo", false),
            new PredefinedTypeInfo(PredefinedType.PT_CONSTRUCTORINFO, typeof(System.Reflection.ConstructorInfo), "System.Reflection.ConstructorInfo", false),
            new PredefinedTypeInfo(PredefinedType.PT_PROPERTYINFO, typeof(System.Reflection.PropertyInfo), "System.Reflection.PropertyInfo", false),
            new PredefinedTypeInfo(PredefinedType.PT_METHODBASE, typeof(System.Reflection.MethodBase), "System.Reflection.MethodBase", false),
            new PredefinedTypeInfo(PredefinedType.PT_MEMBERINFO, typeof(System.Reflection.MemberInfo), "System.Reflection.MemberInfo", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGERDISPLAY, typeof(DebuggerDisplayAttribute), "System.Diagnostics.DebuggerDisplayAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGERBROWSABLE, typeof(DebuggerBrowsableAttribute), "System.Diagnostics.DebuggerBrowsableAttribute", false),
            new PredefinedTypeInfo(PredefinedType.PT_DEBUGGERBROWSABLESTATE, typeof(DebuggerBrowsableState), "System.Diagnostics.DebuggerBrowsableState", false, FUNDTYPE.FT_I4),
            new PredefinedTypeInfo(PredefinedType.PT_G_EQUALITYCOMPARER, typeof(EqualityComparer<>), "System.Collections.Generic.EqualityComparer`1", false),
            new PredefinedTypeInfo(PredefinedType.PT_ELEMENTINITIALIZER, typeof(System.Linq.Expressions.ElementInit), "System.Linq.Expressions.ElementInit", false),

            new PredefinedTypeInfo(PredefinedType.PT_MISSING, typeof(System.Reflection.Missing), "System.Reflection.Missing", false),
            new PredefinedTypeInfo(PredefinedType.PT_G_IREADONLYLIST, typeof(IReadOnlyList<>), "System.Collections.Generic.IReadOnlyList`1", false, FUNDTYPE.FT_REF),
            new PredefinedTypeInfo(PredefinedType.PT_G_IREADONLYCOLLECTION, typeof(IReadOnlyCollection<>), "System.Collections.Generic.IReadOnlyCollection`1", false, FUNDTYPE.FT_REF),
        };

        private static readonly Dictionary<string, PredefinedType> s_pdTypeNames = CreatePredefinedTypeFacts();

        private static Dictionary<string, PredefinedType> CreatePredefinedTypeFacts()
        {
            var pdTypeNames = new Dictionary<string, PredefinedType>((int)PredefinedType.PT_COUNT);
#if DEBUG
            for (int i = 0; i < (int)PredefinedType.PT_COUNT; i++)
            {
                Debug.Assert(s_pdTypes[i].type == (PredefinedType)i);
            }
#endif
            for (int i = 0; i < (int)PredefinedType.PT_COUNT; i++)
            {
                pdTypeNames.Add(s_pdTypes[i].name, (PredefinedType)i);
            }
            return pdTypeNames;
        }
    }
}
