// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Code adapted from https://blogs.msdn.microsoft.com/haibo_luo/2010/04/19/ilvisualizer-2010-solution

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Linq.Expressions.Tests
{
    public interface IILStringCollector
    {
        void Process(ILInstruction ilInstruction, string operandString, Dictionary<int, int> targetLabels);
    }

    public class ReadableILStringToTextWriter : IILStringCollector
    {
        protected readonly TextWriter _writer;

        protected ReadableILStringToTextWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public virtual void Process(ILInstruction ilInstruction, string operandString, Dictionary<int, int> targetLabels)
        {
            if (targetLabels.TryGetValue(ilInstruction.Offset, out int label))
            {
                _writer.WriteLine(
                    "Label_{0:x2}: {1,-10} {2}", label, ilInstruction.OpCode.Name, operandString);
            }
            else
            {
                _writer.WriteLine(
                    "{0,-10} {1}", ilInstruction.OpCode.Name, operandString);
            }
        }
    }

    public class RichILStringToTextWriter : ReadableILStringToTextWriter
    {
        private readonly Dictionary<int, int> _startCounts = new Dictionary<int, int>();
        private readonly Dictionary<int, Type> _startCatch = new Dictionary<int, Type>();
        private readonly Dictionary<int, int> _endCounts = new Dictionary<int, int>();
        private readonly HashSet<int> _startFinally = new HashSet<int>();
        private readonly HashSet<int> _startFault = new HashSet<int>();
        private readonly HashSet<int> _startFilter = new HashSet<int>();
        private string _indent = "";

        public RichILStringToTextWriter(TextWriter writer, ExceptionInfo[] exceptions)
            : base(writer)
        {
            foreach (var e in exceptions)
            {
                if (!_startCounts.TryGetValue(e.StartAddress, out int startCount))
                {
                    _startCounts.Add(e.StartAddress, startCount);
                }

                _startCounts[e.StartAddress] += e.Handlers.Length;

                foreach (var c in e.Handlers)
                {
                    if (c.Kind == HandlerKind.Finally)
                    {
                        _startFinally.Add(c.StartAddress);
                    }
                    else if (c.Kind == HandlerKind.Fault)
                    {
                        _startFault.Add(c.StartAddress);
                    }
                    else if (c.Kind == HandlerKind.Filter)
                    {
                        _startFilter.Add(c.StartAddress);
                    }
                    else
                    {
                        _startCatch.Add(c.StartAddress, c.Type);
                    }

                    if (!_endCounts.TryGetValue(c.EndAddress, out int endCount))
                    {
                        _endCounts.Add(c.EndAddress, endCount);
                    }

                    _endCounts[c.EndAddress]++;
                }
            }
        }

        public override void Process(ILInstruction instruction, string operandString, Dictionary<int, int> targetLabels)
        {
            if (_endCounts.TryGetValue(instruction.Offset, out int endCount))
            {
                for (var i = 0; i < endCount; i++)
                {
                    Dedent();
                    _writer.WriteLine(_indent + "}");
                }
            }

            if (_startCounts.TryGetValue(instruction.Offset, out int startCount))
            {
                for (var i = 0; i < startCount; i++)
                {
                    _writer.WriteLine(_indent + ".try");
                    _writer.WriteLine(_indent + "{");
                    Indent();
                }
            }

            if (_startCatch.TryGetValue(instruction.Offset, out Type t))
            {
                Dedent();
                _writer.WriteLine(_indent + "}");
                _writer.WriteLine(_indent + $"catch ({t.ToIL()})");
                _writer.WriteLine(_indent + "{");
                Indent();
            }

            if (_startFilter.Contains(instruction.Offset))
            {
                Dedent();
                _writer.WriteLine(_indent + "}");
                _writer.WriteLine(_indent + "filter");
                _writer.WriteLine(_indent + "{");
                Indent();
            }

            if (_startFinally.Contains(instruction.Offset))
            {
                Dedent();
                _writer.WriteLine(_indent + "}");
                _writer.WriteLine(_indent + "finally");
                _writer.WriteLine(_indent + "{");
                Indent();
            }

            if (_startFault.Contains(instruction.Offset))
            {
                Dedent();
                _writer.WriteLine(_indent + "}");
                _writer.WriteLine(_indent + "fault");
                _writer.WriteLine(_indent + "{");
                Indent();
            }

            _writer.WriteLine(
                targetLabels.TryGetValue(instruction.Offset, out int label)
                    ? $"{_indent}Label_{label:x2}: {instruction.OpCode.Name,-10} {operandString}"
                    : $"{_indent}{instruction.OpCode.Name,-10} {operandString}");
        }

        public void Indent()
        {
            _indent = new string(' ', _indent.Length + 2);
        }

        public void Dedent()
        {
            _indent = new string(' ', _indent.Length - 2);
        }
    }

    public abstract class ILInstructionVisitor
    {
        public virtual void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineIInstruction(InlineIInstruction inlineIInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineRInstruction(InlineRInstruction inlineRInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction, Dictionary<int, int> targetLabels) { }
        public virtual void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction, Dictionary<int, int> targetLabels) { }
    }

    public class ReadableILStringVisitor : ILInstructionVisitor
    {
        protected readonly IFormatProvider formatProvider;
        protected readonly IILStringCollector collector;

        public ReadableILStringVisitor(IILStringCollector collector)
            : this(collector, DefaultFormatProvider.Instance)
        {
        }

        private ReadableILStringVisitor(IILStringCollector collector, IFormatProvider formatProvider)
        {
            this.formatProvider = formatProvider;
            this.collector = collector;
        }

        public override void VisitInlineBrTargetInstruction(InlineBrTargetInstruction inlineBrTargetInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineBrTargetInstruction, formatProvider.Label(inlineBrTargetInstruction.TargetOffset, targetLabels), targetLabels);
        }

        public override void VisitInlineFieldInstruction(InlineFieldInstruction inlineFieldInstruction, Dictionary<int, int> targetLabels)
        {
            string field;
            try
            {
                field = inlineFieldInstruction.Field.ToIL();
            }
            catch (Exception ex)
            {
                field = "!" + ex.Message + "!";
            }
            collector.Process(inlineFieldInstruction, field, targetLabels);
        }

        public override void VisitInlineIInstruction(InlineIInstruction inlineIInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineIInstruction, inlineIInstruction.Value.ToString(), targetLabels);
        }

        public override void VisitInlineI8Instruction(InlineI8Instruction inlineI8Instruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineI8Instruction, inlineI8Instruction.Value.ToString(), targetLabels);
        }

        public override void VisitInlineMethodInstruction(InlineMethodInstruction inlineMethodInstruction, Dictionary<int, int> targetLabels)
        {
            string method;
            try
            {
                method = inlineMethodInstruction.Method.ToIL();
            }
            catch (Exception ex)
            {
                method = "!" + ex.Message + "!";
            }
            collector.Process(inlineMethodInstruction, method, targetLabels);
        }

        public override void VisitInlineNoneInstruction(InlineNoneInstruction inlineNoneInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineNoneInstruction, string.Empty, targetLabels);
        }

        public override void VisitInlineRInstruction(InlineRInstruction inlineRInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineRInstruction, inlineRInstruction.Value.ToString(CultureInfo.InvariantCulture), targetLabels);
        }

        public override void VisitInlineSigInstruction(InlineSigInstruction inlineSigInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineSigInstruction, formatProvider.SigByteArrayToString(inlineSigInstruction.Signature), targetLabels);
        }

        public override void VisitInlineStringInstruction(InlineStringInstruction inlineStringInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineStringInstruction, formatProvider.EscapedString(inlineStringInstruction.String), targetLabels);
        }

        public override void VisitInlineSwitchInstruction(InlineSwitchInstruction inlineSwitchInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineSwitchInstruction, formatProvider.MultipleLabels(inlineSwitchInstruction.TargetOffsets, targetLabels), targetLabels);
        }

        public override void VisitInlineTokInstruction(InlineTokInstruction inlineTokInstruction, Dictionary<int, int> targetLabels)
        {
            string member;
            try
            {
                string prefix = "";
                string token;
                switch (inlineTokInstruction.Member.MemberType)
                {
                    case MemberTypes.Method:
                    case MemberTypes.Constructor:
                        prefix = "method ";
                        token = ((MethodBase)inlineTokInstruction.Member).ToIL();
                        break;
                    case MemberTypes.Field:
                        prefix = "field ";
                        token = ((FieldInfo)inlineTokInstruction.Member).ToIL();
                        break;
                    default:
                        token = ((TypeInfo)inlineTokInstruction.Member).ToIL();
                        break;
                }

                member = prefix + token;
            }
            catch (Exception ex)
            {
                member = "!" + ex.Message + "!";
            }
            collector.Process(inlineTokInstruction, member, targetLabels);
        }

        public override void VisitInlineTypeInstruction(InlineTypeInstruction inlineTypeInstruction, Dictionary<int, int> targetLabels)
        {
            string type;
            try
            {
                type = inlineTypeInstruction.Type.ToIL();
            }
            catch (Exception ex)
            {
                type = "!" + ex.Message + "!";
            }
            collector.Process(inlineTypeInstruction, type, targetLabels);
        }

        public override void VisitInlineVarInstruction(InlineVarInstruction inlineVarInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(inlineVarInstruction, formatProvider.Argument(inlineVarInstruction.Ordinal), targetLabels);
        }

        public override void VisitShortInlineBrTargetInstruction(ShortInlineBrTargetInstruction shortInlineBrTargetInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(shortInlineBrTargetInstruction, formatProvider.Label(shortInlineBrTargetInstruction.TargetOffset, targetLabels), targetLabels);
        }

        public override void VisitShortInlineIInstruction(ShortInlineIInstruction shortInlineIInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(shortInlineIInstruction, shortInlineIInstruction.Value.ToString(), targetLabels);
        }

        public override void VisitShortInlineRInstruction(ShortInlineRInstruction shortInlineRInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(shortInlineRInstruction, shortInlineRInstruction.Value.ToString(CultureInfo.InvariantCulture), targetLabels);
        }

        public override void VisitShortInlineVarInstruction(ShortInlineVarInstruction shortInlineVarInstruction, Dictionary<int, int> targetLabels)
        {
            collector.Process(shortInlineVarInstruction, formatProvider.Argument(shortInlineVarInstruction.Ordinal), targetLabels);
        }
    }

    internal static class ILHelpers
    {
        public static string ToIL(this Type type) => ToIL(type?.GetTypeInfo());

        public static string ToIL(this TypeInfo type)
        {
            if (type == null)
            {
                return "";
            }

            if (type.IsArray)
            {
                if (type.GetElementType().MakeArrayType().GetTypeInfo() == type)
                {
                    return ToIL(type.GetElementType()) + "[]";
                }
                else
                {
                    string bounds = string.Join(",", Enumerable.Repeat("...", type.GetArrayRank()));
                    return ToIL(type.GetElementType()) + "[" + bounds + "]";
                }
            }
            else if (type.IsGenericType && !type.IsGenericTypeDefinition && !type.IsGenericParameter /* TODO */)
            {
                string args = string.Join(",", type.GetGenericArguments().Select(ToIL));
                string def = ToIL(type.GetGenericTypeDefinition());
                return def + "<" + args + ">";
            }
            else if (type.IsByRef)
            {
                return ToIL(type.GetElementType()) + "&";
            }
            else if (type.IsPointer)
            {
                return ToIL(type.GetElementType()) + "*";
            }
            else
            {
                if (!s_primitives.TryGetValue(type, out string res))
                {
                    res = "[" + type.Assembly.GetName().Name + "]" + type.FullName;

                    if (type.IsValueType)
                    {
                        res = "valuetype " + res;
                    }
                    else
                    {
                        res = "class " + res;
                    }
                }

                return res;
            }
        }

        public static string ToIL(this MethodBase method)
        {
            if (method == null)
            {
                return "";
            }

            string res = "";

            if (!method.IsStatic)
            {
                res = "instance ";
            }

            var mtd = method as MethodInfo;
            Type ret = mtd?.ReturnType ?? typeof(void);

            res += ret.ToIL() + " ";
            res += method.DeclaringType.ToIL();
            res += "::";
            res += method.Name;

            if (method.IsGenericMethod)
            {
                res += "<" + string.Join(",", method.GetGenericArguments().Select(ToIL)) + ">";
            }

            res += "(" + string.Join(",", method.GetParameters().Select(p => ToIL(p.ParameterType))) + ")";

            return res;
        }

        public static string ToIL(this FieldInfo field)
        {
            return field.DeclaringType.ToIL() + "::" + field.Name;
        }

        private static readonly Dictionary<TypeInfo, string> s_primitives = new Dictionary<Type, string>
        {
            { typeof(object), "object" },
            { typeof(void), "void" },
            { typeof(IntPtr), "native int" },
            { typeof(UIntPtr), "native uint" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(float), "float32" },
            { typeof(double), "float64" },
            { typeof(sbyte), "int8" },
            { typeof(short), "int16" },
            { typeof(int), "int32" },
            { typeof(long), "int64" },
            { typeof(byte), "uint8" },
            { typeof(ushort), "uint16" },
            { typeof(uint), "uint32" },
            { typeof(ulong), "uint64" },
            //{ typeof(TypedReference), "typedref" },
        }.ToDictionary(kv => kv.Key.GetTypeInfo(), kv => kv.Value);
    }
}
