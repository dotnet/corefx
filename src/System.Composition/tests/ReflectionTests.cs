using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace System.Composition.UnitTests
{
    /// <summary>
    /// Tests dealing with reflection usage in System.Composition
    /// </summary>
    public class ReflectionTests : ContainerTests
    {
        public static bool HasMultiplerProcessors { get; } = Environment.ProcessorCount > 1;

        /// <summary>
        /// Regression test for issue #6857 System.Composition: Bug in Resolving Activators Due to the .NET Type System
        /// Non-deterministic
        /// </summary>
        /// <remarks>
        /// Because MethodInfo.GetParameters() lazy intializes it's return value, 
        /// concurrent calls can create different ParameterInfo instances.
        /// This can cause arguments to an export's constructor to incorrectly
        /// revert to a default value during GetExport.
        /// </remarks>
        [ConditionalFact(nameof(HasMultiplerProcessors))]
        public void MultiThreadedGetExportsWorkWithImportingConstuctor()
        {
            Parallel.For(0, Environment.ProcessorCount, _ => GetExport());
        }

        private static void GetExport()
        {
            CompositionContext cc = CreateContainer(typeof(A), typeof(B), typeof(C), typeof(D));
            D d = cc.GetExport<D>();
            Assert.IsAssignableFrom(typeof(A), d.A);
            Assert.IsAssignableFrom(typeof(B), d.B);
            Assert.IsAssignableFrom(typeof(A), d.B.A);
            Assert.IsAssignableFrom(typeof(C), d.C);
            Assert.IsAssignableFrom(typeof(A), d.C.A);
            Assert.IsAssignableFrom(typeof(B), d.C.B);
        }

        public interface IA { }

        [Export(typeof(IA))]
        public class A : IA { }

        public interface IB
        {
            IA A { get; }
        }

        [Export(typeof(IB))]
        public class B : IB
        {
            public IA A { get; }

            [ImportingConstructor]
            public B(IA ia)
            {
                A = ia;
            }
        }

        public interface IC
        {
            IA A { get; }
            IB B { get; }
        }

        [Export(typeof(IC))]
        public class C : IC
        {
            public IA A { get; }
            public IB B { get; }

            [ImportingConstructor]
            public C(IA ia, IB ib)
            {
                A = ia;
                B = ib;
            }
        }

        [Export]
        public class D
        {
            public IA A { get; }
            public IB B { get; }
            public IC C { get; }

            [ImportingConstructor]
            public D(IA ia, IB ib, IC ic)
            {
                A = ia;
                B = ib;
                C = ic;
            }
        }
    }
}
