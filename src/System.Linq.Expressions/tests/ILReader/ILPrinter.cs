﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Tests
{
    static class ILPrinter
    {
        private static CachedTypeFactory s_typeFactory = new CachedTypeFactory(typeof(IStrongBox), typeof(StrongBox<>));

        private static ITypeFactory GetTypeFactory(Expression expression)
        {
            s_typeFactory.AddTypesFrom(expression);
            return s_typeFactory;
        }

        public static string GetIL(this LambdaExpression expression)
        {
            var d = expression.Compile();

            var method = d.GetMethodInfo();

            var sw = new StringWriter();
            var reader = ILReaderFactory.Create(method);
            var exceptions = reader.ILProvider.GetExceptionInfos();
            var writer = new RichILStringToTextWriter(sw, exceptions);

            sw.WriteLine(".method " + method.ToIL());
            sw.WriteLine("{");
            sw.WriteLine("  .maxstack " + reader.ILProvider.MaxStackSize);

            var typeFactory = GetTypeFactory(expression);
            var sig = reader.ILProvider.GetLocalSignature();
            var lsp = new LocalsSignatureParser(reader.Resolver, typeFactory);
            var locals = default(Type[]);
            if (lsp.Parse(sig, out locals) && locals.Length > 0)
            {
                sw.WriteLine("  .locals init (");

                for (var i = 0; i < locals.Length; i++)
                {
                    sw.WriteLine($"    [{i}] {locals[i].ToIL()}{(i != locals.Length - 1 ? "," : "")}");
                }

                sw.WriteLine("  )");
            }

            sw.WriteLine();

            writer.Indent();
            reader.Accept(new ReadableILStringVisitor(writer));
            writer.Dedent();

            sw.WriteLine("}");

            return sw.ToString();
        }
    }

    class CachedTypeFactory : DefaultTypeFactory
    {
        private static readonly PropertyInfo s_RuntimeTypeHandle_Value = typeof(RuntimeTypeHandle).GetProperty("Value");

        private readonly Dictionary<IntPtr, Type> _cache = new Dictionary<IntPtr, Type>();

        public CachedTypeFactory(params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(type);
            }
        }

        public void AddTypesFrom(Expression expression) => new TypeFinder(this).Visit(expression);

        public void AddType(Type type)
        {
            var handle = (IntPtr)s_RuntimeTypeHandle_Value.GetValue(type.TypeHandle);

            lock (_cache)
            {
                if (!_cache.ContainsKey(handle))
                {
                    _cache.Add(handle, type);
                }
            }
        }

        public override Type FromHandle(IntPtr handle)
        {
            Type res;
            if (_cache.TryGetValue(handle, out res))
            {
                return res;
            }

            return base.FromHandle(handle);
        }

        class TypeFinder : ExpressionVisitor
        {
            private readonly CachedTypeFactory _parent;

            public TypeFinder(CachedTypeFactory factory)
            {
                _parent = factory;
            }

            public override Expression Visit(Expression node)
            {
                if (node != null)
                {
                    Visit(node.Type);
                }

                return base.Visit(node);
            }

            private void Visit(Type type)
            {
                var ti = type.GetTypeInfo();

                if (ti.IsArray || ti.IsPointer || ti.IsByRef)
                {
                    Visit(type.GetElementType());
                }
                else if (ti.IsGenericType && !ti.IsGenericTypeDefinition)
                {
                    Visit(type.GetGenericTypeDefinition());
                    foreach (var arg in type.GetGenericArguments())
                    {
                        Visit(arg);
                    }
                }
                else if (!ti.IsPrimitive)
                {
                    _parent.AddType(type);
                }
            }
        }
    }
}
