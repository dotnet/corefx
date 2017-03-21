// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// FEATURE_COMPILE is not directly required, 
// but this functionality relies on private reflection and that would not work with AOT
#if FEATURE_INTERPRET && FEATURE_COMPILE

using System.IO;
using System.Linq.Expressions.Interpreter;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class InterpreterTests
    {
        private static readonly PropertyInfo s_debugView = typeof(LightLambda).GetPropertyAssert("DebugView");

        private static readonly Regex InstructionNumberStrip = new Regex(@"IP_\d{4}: ", RegexOptions.Compiled);

        [Fact]
        public static void VerifyInstructions_Simple()
        {
            // Using an unchecked multiplication to ensure that a mul instruction is emitted (and not mul.ovf)
            Expression<Func<string, bool>> f = s => s != null && unchecked(s.Substring(1).Length * 2) > 0;

            f.VerifyInstructions(
                @"object lambda_method(object[])
                  {
                    .locals 1
                    .maxstack 2
                    .maxcontinuation 0

                    InitParameter(0)
                    LoadLocal(0)
                    LoadObject(null)
                    Call(Boolean op_Inequality(System.String, System.String))
                    BranchFalse(10) -> 14
                    LoadLocal(0)
                    LoadObject(1)
                    Call(System.String Substring(Int32))
                    Call(Int32 get_Length())
                    LoadObject(2)
                    Mul()
                    LoadObject(0)
                    GreaterThan()
                    Branch(2) -> 15
                    LoadObject(False)
                  }");
        }

        [Fact]
        public static void VerifyInstructions_Exceptions()
        {
            ParameterExpression x = Expression.Parameter(typeof(int), "x");
            Expression<Func<int, int>> f =
                Expression.Lambda<Func<int, int>>(
                    Expression.TryCatchFinally(
                        Expression.Call(
                            typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(int) }),
                            Expression.Divide(
                                Expression.Constant(42),
                                x
                            )
                        ),
                        Expression.Empty(),
                        Expression.Catch(
                            typeof(DivideByZeroException),
                            Expression.Constant(-1)
                        )
                    ),
                    x
                );

            f.VerifyInstructions(
                @"object lambda_method(object[])
                  {
                    .locals 2
                    .maxstack 3
                    .maxcontinuation 1

                    InitParameter(0)
                    .try
                    {
                      EnterTryFinally[0] -> 11
                      LoadObject(42)
                      LoadLocal(0)
                      Div()
                      Call(Int32 Abs(Int32))
                      Goto[1] -> 13
                    }
                    catch(DivideByZeroException) [8->11]
                    {
                      EnterExceptionHandler()
                      StoreLocal(1)
                      LoadObject(-1)
                      LeaveExceptionHandler[3] -> 6
                    }
                    finally
                    {
                      EnterFinally[0] -> 11
                      LeaveFinally()
                    }
                  }");
        }

        [Fact]
        public static void ConstructorThrows_StackTrace()
        {
            Expression<Func<Thrower>> e = () => new Thrower(true);
            Func<Thrower> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(), "Thrower..ctor");
        }

        [Fact]
        public static void PropertyGetterThrows_StackTrace()
        {
            Expression<Func<Thrower, int>> e = t => t.Bar;
            Func<Thrower, int> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(new Thrower(error: false)), "Thrower.get_Bar");
        }

        [Fact]
        public static void PropertySetterThrows_StackTrace()
        {
            ParameterExpression t = Expression.Parameter(typeof(Thrower), "t");
            Expression<Action<Thrower>> e = Expression.Lambda<Action<Thrower>>(Expression.Assign(Expression.Property(t, nameof(Thrower.Bar)), Expression.Constant(0)), t);
            Action<Thrower> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(new Thrower(error: false)), "Thrower.set_Bar");
        }

        [Fact]
        public static void IndexerGetterThrows_StackTrace()
        {
            ParameterExpression t = Expression.Parameter(typeof(Thrower), "t");
            Expression<Func<Thrower, int>> e = Expression.Lambda<Func<Thrower, int>>(Expression.MakeIndex(t, typeof(Thrower).GetProperty("Item"), new[] { Expression.Constant(0) }), t);
            Func<Thrower, int> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(new Thrower(error: false)), "Thrower.get_Item");
        }

        [Fact]
        public static void IndexerSetterThrows_StackTrace()
        {
            ParameterExpression t = Expression.Parameter(typeof(Thrower), "t");
            Expression<Action<Thrower>> e = Expression.Lambda<Action<Thrower>>(Expression.Assign(Expression.MakeIndex(t, typeof(Thrower).GetProperty("Item"), new[] { Expression.Constant(0) }), Expression.Constant(0)), t);
            Action<Thrower> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(new Thrower(error: false)), "Thrower.set_Item");
        }

        [Fact]
        public static void MethodThrows_StackTrace()
        {
            Expression<Action<Thrower>> e = t => t.Foo();
            Action<Thrower> f = e.Compile(preferInterpretation: true);
            AssertStackTrace(() => f(new Thrower(error: false)), "Thrower.Foo");
        }

        public static void VerifyInstructions(this LambdaExpression expression, string expected)
        {
            string actual = expression.GetInstructions();

            string nExpected = Normalize(expected);
            string nActual = Normalize(actual);

            Assert.Equal(nExpected, nActual);
        }

        private static string Normalize(string s)
        {
            Collections.Generic.IEnumerable<string> lines =
                s
                .Replace("\r\n", "\n")
                .Split(new[] { '\n' })
                .Select(line => line.Trim())
                .Where(line => line != "" && !line.StartsWith("//"));

            return string.Join("\n", lines);
        }

        private static string StripInstructionNumbers(string instructionDump)
        {
            StringWriter output = new StringWriter();
            StringReader input = new StringReader(instructionDump);
            for (string line = input.ReadLine(); line != null; line = input.ReadLine())
            {
                output.WriteLine(InstructionNumberStrip.Replace(line, "", 1));
            }

            return output.ToString();
        }

        private static string GetInstructions(this LambdaExpression expression)
        {
            Delegate d = expression.Compile(true);
            var thunk = (Func<object[], object>)d.Target;
            var lambda = (LightLambda)thunk.Target;
            return StripInstructionNumbers((string)s_debugView.GetValue(lambda));
        }

        private static void AssertStackTrace(Action a, string searchTerm)
        {
            bool hasThrown = false;
            try
            {
                a();
            }
            catch (Exception ex)
            {
                AssertStackTrace(ex, searchTerm);
                hasThrown = true;
            }

            Assert.True(hasThrown);
        }

        private static void AssertStackTrace(Exception ex, string searchTerm)
        {
            Assert.True(ex.StackTrace.Contains(searchTerm));
        }

        private sealed class Thrower
        {
            public Thrower(bool error)
            {
                if (error)
                    throw new Exception();
            }

            public int this[int x]
            {
                get { throw new Exception(); }
                set { throw new Exception(); }
            }

            public int Bar
            {
                get { throw new Exception(); }
                set { throw new Exception(); }
            }

            public void Foo()
            {
                throw new Exception();
            }
        }
    }
}

#endif
