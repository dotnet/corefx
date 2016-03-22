// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Xml.Serialization.Emit;
using OpCode = System.Reflection.Emit.OpCode;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class ILGenerator
    {
        [Obsolete("TODO", error: false)]
        public virtual int ILOffset
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual void BeginCatchBlock(Type exceptionType)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void BeginExceptFilterBlock()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Label BeginExceptionBlock()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void BeginFaultBlock()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void BeginFinallyBlock()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void BeginScope()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual LocalBuilder DeclareLocal(Type localType)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Label DefineLabel()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, byte arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, double arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, short arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, int arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, long arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, ConstructorInfo con)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, Label label)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, Label[] labels)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, LocalBuilder local)
        {
            throw new InvalidOperationException();
        }

        //public virtual void Emit(OpCode opcode, SignatureHelper signature)
        //{
        //}

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, FieldInfo field)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, MethodInfo meth)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public void Emit(OpCode opcode, sbyte arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, float arg)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, string str)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void Emit(OpCode opcode, Type cls)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EmitWriteLine(LocalBuilder localBuilder)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EmitWriteLine(FieldInfo fld)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EmitWriteLine(string value)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EndExceptionBlock()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void EndScope()
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void MarkLabel(Label loc)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void ThrowException(Type excType)
        {
            throw new InvalidOperationException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void UsingNamespace(string usingNamespace)
        {
            throw new InvalidOperationException();
        }
    }
}
#endif