﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_COMPILE

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class StackSpillerTests
    {
        [Fact]
        public static void Spill_Binary()
        {
            Test((l, r) => Expression.Add(l, r), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Binary_IndexAssign()
        {
            var item = typeof(Dictionary<int, int>).GetProperty("Item");

            Test((d, i, v) =>
            {
                var index = Expression.Property(d, item, i);

                return
                    Expression.Block(
                        Expression.Assign(index, v),
                        index
                    );
            }, Expression.Constant(new Dictionary<int, int>()), Expression.Constant(0), Expression.Constant(1));
        }

        [Fact]
        public static void Spill_Binary_MemberAssign()
        {
            var baz = typeof(Bar).GetProperty(nameof(Bar.Baz));

            Test((l, r) =>
            {
                var prop = Expression.Property(l, baz);

                return
                    Expression.Block(
                        Expression.Assign(prop, r),
                        prop
                    );
            }, Expression.Constant(new Bar()), Expression.Constant(42));
        }

        [Fact]
        public static void Spill_TryFinally()
        {
            Test(a => Expression.TryFinally(a, Expression.Empty()), Expression.Constant(1));
        }

        [Fact]
        public static void Spill_TryCatch()
        {
            foreach (var div in new[] { 1, 0 })
            {
                Test((n, d, a) => Expression.TryCatch(Expression.Divide(n, d), Expression.Catch(typeof(DivideByZeroException), a)), Expression.Constant(1), Expression.Constant(div), Expression.Constant(42));
            }
        }

        [Fact]
        public static void Spill_Loop()
        {
            Test((a, b) =>
            {
                var @break = Expression.Label(typeof(int));
                return Expression.Add(a, Expression.Loop(Expression.Break(@break, b), @break));
            }, Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Member()
        {
            Test(s => Expression.Property(s, nameof(string.Length)), Expression.Constant("bar"));
        }

        [Fact]
        public static void Spill_TypeBinary()
        {
            Test(s => Expression.TypeIs(s, typeof(string)), Expression.Constant("bar", typeof(object)));
            Test(s => Expression.TypeEqual(s, typeof(string)), Expression.Constant("bar", typeof(object)));
        }

        [Fact]
        public static void Spill_Binary_Logical()
        {
            Test((l, r) => Expression.AndAlso(l, r), Expression.Constant(false), Expression.Constant(false));
            Test((l, r) => Expression.AndAlso(l, r), Expression.Constant(false), Expression.Constant(true));
            Test((l, r) => Expression.AndAlso(l, r), Expression.Constant(true), Expression.Constant(false));
            Test((l, r) => Expression.AndAlso(l, r), Expression.Constant(true), Expression.Constant(true));
        }

        [Fact]
        public static void Spill_Conditional()
        {
            Test((t, l, r) => Expression.Condition(t, l, r), Expression.Constant(false), Expression.Constant(1), Expression.Constant(2));
            Test((t, l, r) => Expression.Condition(t, l, r), Expression.Constant(true), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Index()
        {
            var item = typeof(Dictionary<int, int>).GetProperty("Item");

            Test((d, i) => Expression.MakeIndex(d, item, new[] { i }), Expression.Constant(new Dictionary<int, int> { { 1, 2 } }), Expression.Constant(1));
        }

        [Fact]
        public static void Spill_Call_Static()
        {
            var max = typeof(Math).GetMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) });

            Test((x, y) => Expression.Call(max, x, y), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Call_Instance()
        {
            var substring = typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

            Test((s, i, j) => Expression.Call(s, substring, i, j), Expression.Constant("foobar"), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Invocation()
        {
            Test((d, a, b) => Expression.Invoke(d, a, b), Expression.Constant(new Func<int, int, int>((x, y) => x + y)), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_Invocation_Inline()
        {
            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));

            Test((a, b) => Expression.Invoke(Expression.Lambda(Expression.Subtract(x, y), x, y), a, b), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_New()
        {
            var ctor = typeof(TimeSpan).GetConstructor(new[] { typeof(int), typeof(int), typeof(int) });

            Test((h, m, s) => Expression.New(ctor, h, m, s), Expression.Constant(1), Expression.Constant(2), Expression.Constant(3));
        }

        [Fact]
        public static void Spill_NewArrayInit()
        {
            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(int[]));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.NewArrayInit(typeof(int), a, b, c)
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.ArrayIndex(p, Expression.Constant(0)),
                                Expression.ArrayIndex(p, Expression.Constant(1))
                            ),
                            Expression.ArrayIndex(p, Expression.Constant(2))
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_NewArrayBounds()
        {
            var getUpperBound = typeof(Array).GetMethod(nameof(Array.GetUpperBound));

            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(int[,,]));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.NewArrayBounds(typeof(int), a, b, c)
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.Call(p, getUpperBound, Expression.Constant(0)),
                                Expression.Call(p, getUpperBound, Expression.Constant(1))
                            ),
                            Expression.Call(p, getUpperBound, Expression.Constant(2))
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_ListInit()
        {
            var item = typeof(List<int>).GetProperty("Item");

            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(List<int>));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.ListInit(Expression.New(typeof(List<int>)), a, b, c)
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.MakeIndex(p, item, new[] { Expression.Constant(0) }),
                                Expression.MakeIndex(p, item, new[] { Expression.Constant(1) })
                            ),
                            Expression.MakeIndex(p, item, new[] { Expression.Constant(2) })
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_MemberInit_Bind()
        {
            var baz = typeof(Bar).GetProperty(nameof(Bar.Baz));
            var foo = typeof(Bar).GetProperty(nameof(Bar.Foo));
            var qux = typeof(Bar).GetProperty(nameof(Bar.Qux));

            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(Bar));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.MemberInit(
                                Expression.New(typeof(Bar)),
                                Expression.Bind(baz, a),
                                Expression.Bind(foo, b),
                                Expression.Bind(qux, c)
                            )
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.Property(p, baz),
                                Expression.Property(p, foo)
                            ),
                            Expression.Property(p, qux)
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_MemberInit_MemberBind()
        {
            var bar = typeof(BarHolder).GetProperty(nameof(BarHolder.Bar));
            var baz = typeof(Bar).GetProperty(nameof(Bar.Baz));
            var foo = typeof(Bar).GetProperty(nameof(Bar.Foo));
            var qux = typeof(Bar).GetProperty(nameof(Bar.Qux));

            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(BarHolder));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.MemberInit(
                                Expression.New(typeof(BarHolder)),
                                Expression.MemberBind(
                                    bar, 
                                    Expression.Bind(baz, a),
                                    Expression.Bind(foo, b),
                                    Expression.Bind(qux, c)
                                )
                            )
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.Property(Expression.Property(p, bar), baz),
                                Expression.Property(Expression.Property(p, bar), foo)
                            ),
                            Expression.Property(Expression.Property(p, bar), qux)
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_MemberInit_ListBind()
        {
            var xs = typeof(ListHolder).GetProperty(nameof(ListHolder.Xs));
            var add = typeof(List<int>).GetMethod(nameof(List<int>.Add));
            var item = typeof(List<int>).GetProperty("Item");

            Test((a, b, c) =>
            {
                var p = Expression.Parameter(typeof(ListHolder));

                return
                    Expression.Block(
                        new[] { p },
                        Expression.Assign(
                            p,
                            Expression.MemberInit(
                                Expression.New(typeof(ListHolder)),
                                Expression.ListBind(
                                    xs,
                                    Expression.ElementInit(add, a),
                                    Expression.ElementInit(add, b),
                                    Expression.ElementInit(add, c)
                                )
                            )
                        ),
                        Expression.Multiply(
                            Expression.Add(
                                Expression.Property(Expression.Property(p, xs), item, Expression.Constant(0)),
                                Expression.Property(Expression.Property(p, xs), item, Expression.Constant(1))
                            ),
                            Expression.Property(Expression.Property(p, xs), item, Expression.Constant(2))
                        )
                    );
            }, Expression.Constant(2), Expression.Constant(3), Expression.Constant(5));
        }

        [Fact]
        public static void Spill_Switch()
        {
            Test((v, d, a) => Expression.Switch(v, d, Expression.SwitchCase(a, Expression.Constant(1))), Expression.Constant(1), Expression.Constant(2), Expression.Constant(3));
            Test((v, d, a) => Expression.Switch(v, d, Expression.SwitchCase(a, Expression.Constant(7))), Expression.Constant(1), Expression.Constant(2), Expression.Constant(3));
            Test((v, d, a) => Expression.Switch(v, d, Expression.SwitchCase(Expression.Constant(7), a)), Expression.Constant(1), Expression.Constant(2), Expression.Constant(3));
            Test((v, d, a) => Expression.Switch(v, d, Expression.SwitchCase(Expression.Constant(7), a)), Expression.Constant(1), Expression.Constant(2), Expression.Constant(1));
        }

        [Fact]
        public static void Spill_Block()
        {
            Test((a, b) => Expression.Block(a, b), Expression.Constant(1), Expression.Constant(2));
        }

        [Fact]
        public static void Spill_LabelGoto()
        {
            foreach (var t in new[] { true, false })
            {
                Test((c, a, b) =>
                {
                    var lbl = Expression.Label(typeof(int));

                    return
                        Expression.Block(
                            Expression.IfThen(c, Expression.Goto(lbl, a)),
                            Expression.Label(lbl, b)
                        );
                }, Expression.Constant(t), Expression.Constant(1), Expression.Constant(2));
            }
        }

        [Fact]
        public static void Spill_NotRefInstance_IndexAssignment()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var v = Expression.Constant(new ValueVector());
            var item = typeof(ValueVector).GetProperty("Item");
            var i = Expression.Constant(0);
            var x = Expression.Constant(42);

            NotSupported(Expression.Assign(Expression.Property(v, item, i), Spill(x)));
        }

        [Fact]
        public static void Spill_NotRefInstance_Index()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var v = Expression.Constant(new ValueVector());
            var item = typeof(ValueVector).GetProperty("Item");
            var i = Expression.Constant(0);
            
            NotSupported(Expression.Property(v, item, Spill(i)));
        }

        [Fact]
        public static void Spill_NotRefInstance_MemberAssignment()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var v = Expression.Constant(new ValueBar());
            var foo = typeof(ValueBar).GetProperty(nameof(ValueBar.Foo));
            var x = Expression.Constant(42);

            NotSupported(Expression.Assign(Expression.Property(v, foo), Spill(x)));
        }

        [Fact]
        public static void Spill_NotRefInstance_Call()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var v = Expression.Constant(new ValueBar());
            var qux = typeof(ValueBar).GetMethod(nameof(ValueBar.Qux));
            var x = Expression.Constant(42);

            NotSupported(Expression.Call(v, qux, Spill(x)));
        }

        [Fact]
        public static void Spill_RequireNoRefArgs_Call()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var assign = typeof(ByRefs).GetMethod(nameof(ByRefs.Assign));
            var x = Expression.Parameter(typeof(int));
            var v = Expression.Constant(42);
            var b = Expression.Block(new[] { x }, Expression.Call(assign, x, Spill(v)), x);

            NotSupported(b);
        }

        [Fact]
        public static void Spill_RequireNoRefArgs_New()
        {
            // See https://github.com/dotnet/corefx/issues/11740 for documented limitation

            var ctor = typeof(ByRefs).GetConstructors()[0];
            var x = Expression.Parameter(typeof(int));
            var v = Expression.Constant(42);
            var b = Expression.Block(new[] { x }, Expression.New(ctor, x, Spill(v)), x);

            NotSupported(b);
        }

        [Fact]
        public static void Spill_Optimizations_Constant()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Constant(0);
            var v = Spill(Expression.Constant(1));

            var e =
                Expression.Lambda<Action<int[]>>(
                    Expression.Assign(Expression.ArrayAccess(xs, i), v),
                    xs
                );
            
            e.VerifyIL(@"
                .method void ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure,int32[])
                {
                  .maxstack 4
                  .locals init (
                    [0] int32[],
                    [1] int32,
                    [2] int32
                  )

                  // Save instance (`xs`) into V_0
                  IL_0000: ldarg.1    
                  IL_0001: stloc.0    

                  // Save rhs (`try { 1 } finally {}`) into V_1
                  .try
                  {
                    IL_0002: ldc.i4.1   
                    IL_0003: stloc.2    
                    IL_0004: leave      IL_000a
                  }
                  finally
                  {
                    IL_0009: endfinally 
                  }
                  IL_000a: ldloc.2    
                  IL_000b: stloc.1    

                  // Load instance from V_0
                  IL_000c: ldloc.0    

                  // <OPTIMIZATION> Evaluate index (`0`) </OPTIMIZATION>
                  IL_000d: ldc.i4.0   

                  // Load rhs from V_1
                  IL_000e: ldloc.1    

                  // Evaluate `instance[index] = rhs` index assignment
                  IL_000f: stelem.i4  

                  IL_0010: ret        
                }"
            );
        }

        [Fact]
        public static void Spill_Optimizations_Default()
        {
            var xs = Expression.Parameter(typeof(int[]));
            var i = Expression.Default(typeof(int));
            var v = Spill(Expression.Constant(1));

            var e =
                Expression.Lambda<Action<int[]>>(
                    Expression.Assign(Expression.ArrayAccess(xs, i), v),
                    xs
                );

            e.VerifyIL(@"
                .method void ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure,int32[])
                {
                  .maxstack 4
                  .locals init (
                    [0] int32[],
                    [1] int32,
                    [2] int32
                  )

                  // Save instance (`xs`) into V_0
                  IL_0000: ldarg.1    
                  IL_0001: stloc.0    

                  // Save rhs (`try { 1 } finally {}`) into V_1
                  .try
                  {
                    IL_0002: ldc.i4.1   
                    IL_0003: stloc.2    
                    IL_0004: leave      IL_000a
                  }
                  finally
                  {
                    IL_0009: endfinally 
                  }
                  IL_000a: ldloc.2    
                  IL_000b: stloc.1  

                  // Load instance from V_0  
                  IL_000c: ldloc.0    

                  // <OPTIMIZATION> Evaluate index (`0`) </OPTIMIZATION>
                  IL_000d: ldc.i4.0   

                  // Load rhs from V_1
                  IL_000e: ldloc.1    

                  // Evaluate `instance[index] = rhs` index assignment
                  IL_000f: stelem.i4  

                  IL_0010: ret        
                }"
            );
        }

        [Fact]
        public static void Spill_Optimizations_LiteralField()
        {
            var e =
                Expression.Lambda<Func<double>>(
                    Expression.Add(
                        Expression.Field(null, typeof(Math).GetField(nameof(Math.PI))),
                        Spill(Expression.Constant(0.0))
                    )
                );

            e.VerifyIL(@"
                .method float64 ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure)
                {
                  .maxstack 3
                  .locals init (
                    [0] float64,
                    [1] float64
                  )

                  // Save rhs (`try { 0.0 } finally {}`) into V_0
                  .try
                  {
                    IL_0000: ldc.r8     0
                    IL_0009: stloc.1    
                    IL_000a: leave      IL_0010
                  }
                  finally
                  {
                    IL_000f: endfinally 
                  }
                  IL_0010: ldloc.1    
                  IL_0011: stloc.0    

                  // <OPTIMIZATION> Evaluate lhs (`Math.PI` gets inlined) </OPTIMIZATION>
                  IL_0012: ldc.r8     3.14159265358979

                  // Load rhs from V_0  
                  IL_001b: ldloc.0    

                  // Evaluate `lhs + rhs`
                  IL_001c: add        

                  IL_001d: ret        
                }"
            );
        }

        [Fact]
        public static void Spill_Optimizations_StaticReadOnlyField()
        {
            var e =
                Expression.Lambda<Func<string>>(
                    Expression.Call(
                        typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) }),
                        Expression.Field(null, typeof(bool).GetField(nameof(bool.TrueString))),
                        Spill(Expression.Constant("!"))
                    )
                );

            e.VerifyIL(@"
                .method string ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure)
                {
                  .maxstack 3
                  .locals init (
                    [0] string,
                    [1] string
                  )

                  // Save arg1 (`try { ""!"" } finally {}`) into V_0
                  .try
                  {
                    IL_0000: ldstr      ""!""
                    IL_0005: stloc.1
                    IL_0006: leave      IL_000c
                  }
                  finally
                  {
                    IL_000b: endfinally
                  }
                  IL_000c: ldloc.1    
                  IL_000d: stloc.0    

                  // <OPTIMIZATION> Evaluate arg0 (`bool::TrueString`) </OPTIMIZATION>
                  IL_000e: ldsfld     bool::TrueString

                  // Load arg1 from V_0
                  IL_0013: ldloc.0    

                  // Evaluate `string.Concat(arg0, arg1)` call
                  IL_0014: call       string string::Concat(string,string)

                  IL_0019: ret
                }"
            );
        }

        [Fact]
        public static void Spill_Optimizations_RuntimeVariables1()
        {
            var f = Expression.Parameter(typeof(Action<IRuntimeVariables, int>));
            var e =
                Expression.Lambda<Action<Action<IRuntimeVariables, int>>>(
                    Expression.Invoke(
                        f,
                        Expression.RuntimeVariables(),
                        Spill(Expression.Constant(2))
                    ),
                    f
                );

            e.VerifyIL(@"
                .method void ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure,class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>)
                {
                  .maxstack 4
                  .locals init (
                    [0] class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>,
                    [1] int32,
                    [2] int32
                  )

                  // Save target (`f`) into V_0
                  IL_0000: ldarg.1    
                  IL_0001: stloc.0    

                  // Save arg1 (`try { 2 } finally {}`) into V_1
                  .try
                  {
                    IL_0002: ldc.i4.2   
                    IL_0003: stloc.2    
                    IL_0004: leave      IL_000a
                  }
                  finally
                  {
                    IL_0009: endfinally 
                  }
                  IL_000a: ldloc.2    
                  IL_000b: stloc.1   

                  // Load target from V_0
                  IL_000c: ldloc.0    

                  // <OPTIMIZATION> Load arg0 (`RuntimeVariables`) by calling RuntimeOps.CreateRuntimeVariables </OPTIMIZATION>
                  IL_000d: call       class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables class [System.Linq.Expressions]System.Runtime.CompilerServices.RuntimeOps::CreateRuntimeVariables()

                  // Load arg1 from V_1
                  IL_0012: ldloc.1    

                  // Evaluate `target(arg0, arg1)` delegate invocation
                  IL_0013: callvirt   instance void class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>::Invoke(class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32)

                  IL_0018: ret        
                }"
            );
        }

        [Fact]
        public static void Spill_Optimizations_RuntimeVariables2()
        {
            var f = Expression.Parameter(typeof(Action<IRuntimeVariables, int>));
            var x = Expression.Parameter(typeof(int));
            var e =
                Expression.Lambda<Action<Action<IRuntimeVariables, int>, int>>(
                    Expression.Invoke(
                        f,
                        Expression.RuntimeVariables(x),
                        Spill(Expression.Constant(2))
                    ),
                    f,
                    x
                );

            e.VerifyIL(@"
                .method void ::lambda_method(class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure,class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>,int32)
                {
                  .maxstack 10
                  .locals init (
                    [0] object[],
                    [1] class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>,
                    [2] int32,
                    [3] int32
                  )

                  // Hoist `x` to a closure in V_0
                  IL_0000: ldc.i4.1   
                  IL_0001: newarr     object
                  IL_0006: dup        
                  IL_0007: ldc.i4.0   
                  IL_0008: ldarg.2    
                  IL_0009: newobj     instance void class [System.Runtime]System.Runtime.CompilerServices.StrongBox`1<int32>::.ctor(int32)
                  IL_000e: stelem.ref 
                  IL_000f: stloc.0    

                  // Save target (`f`) into V_1
                  IL_0010: ldarg.1    
                  IL_0011: stloc.1    

                  // Save arg1 (`try { 2 } finally {}`) into V_2
                  .try
                  {
                    IL_0012: ldc.i4.2   
                    IL_0013: stloc.3    
                    IL_0014: leave      IL_001a
                  }
                  finally
                  {
                    IL_0019: endfinally 
                  }
                  IL_001a: ldloc.3    
                  IL_001b: stloc.2   

                  // Load target from V_1 
                  IL_001c: ldloc.1    

                  // <OPTIMIZATION> Load arg0 (`RuntimeVariables`) by calling RuntimeOps.CreateRuntimeVariables </OPTIMIZATION>
                  IL_001d: ldloc.0    
                  IL_001e: ldarg.0    
                  IL_001f: ldfld      class [System.Linq.Expressions]System.Runtime.CompilerServices.Closure::Constants
                  IL_0024: ldc.i4.0   
                  IL_0025: ldelem.ref 
                  IL_0026: castclass  int64[]
                  IL_002b: call       class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables class [System.Linq.Expressions]System.Runtime.CompilerServices.RuntimeOps::CreateRuntimeVariables(object[],int64[])

                  // Load arg1 from V_2
                  IL_0030: ldloc.2    

                  // Evaluate `target(arg0, arg1)` delegate invocation
                  IL_0031: callvirt   instance void class [System.Private.CoreLib]System.Action`2<class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32>::Invoke(class [System.Linq.Expressions]System.Runtime.CompilerServices.IRuntimeVariables,int32)

                  IL_0036: ret        
                }"
            );
        }

        private static void NotSupported(Expression expression)
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object))).Compile();
            });
        }

        private static void Test(Func<Expression, Expression> factory, Expression arg1)
        {
            Test(args => factory(args[0]), new[] { arg1 });
        }

        private static void Test(Func<Expression, Expression, Expression> factory, Expression arg1, Expression arg2)
        {
            Test(args => factory(args[0], args[1]), new[] { arg1, arg2 });
        }

        private static void Test(Func<Expression, Expression, Expression, Expression> factory, Expression arg1, Expression arg2, Expression arg3)
        {
            Test(args => factory(args[0], args[1], args[2]), new[] { arg1, arg2, arg3 });
        }

        private static void Test(Func<Expression[], Expression> factory, Expression[] args)
        {
            var expected = Eval(factory(args));

            for (var i = 0; i < args.Length; i++)
            {
                var newArgs = args.Select((arg, j) => j == i ? Spill(arg) : arg).ToArray();
                Assert.Equal(expected, Eval(factory(newArgs)));
            }

            for (var i = 0; i < args.Length; i++)
            {
                var newArgs = args.Select((arg, j) => j == i ? new Extension(arg) : arg).ToArray();
                Assert.Equal(expected, Eval(factory(newArgs)));
            }
        }

        private static object Eval(Expression expression)
        {
            return Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object))).Compile()();
        }

        private static Expression Spill(Expression expression)
        {
            return Expression.TryFinally(expression, Expression.Empty());
        }

        class BarHolder
        {
            public Bar Bar { get; } = new Bar();
        }

        class ListHolder
        {
            public List<int> Xs { get; } = new List<int>();
        }

        class Bar
        {
            public int Baz { get; set; }
            public int Foo { get; set; }
            public int Qux { get; set; }
        }

        class Extension : Expression
        {
            private readonly Expression _reduced;

            public Extension(Expression reduced)
            {
                _reduced = reduced;
            }

            public override bool CanReduce => true;
            public override ExpressionType NodeType => ExpressionType.Extension;
            public override Type Type => _reduced.Type;
            public override Expression Reduce() => _reduced;
        }

        struct ValueVector
        {
            private int v0;

            public int this[int x]
            {
                get { return v0; }
                set { v0 = value; }
            }
        }

        struct ValueBar
        {
            public int Foo { get; set; }

            public int Qux(int x) => x + 1;
        }

        class ByRefs
        {
            public ByRefs(ref int x, int v)
            {
                x = v;
            }

            public static void Assign(ref int x, int v)
            {
                x = v;
            }
        }
    }
}

#endif
