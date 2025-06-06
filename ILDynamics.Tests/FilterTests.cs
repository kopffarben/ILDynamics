using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using ILDynamics.Resolver.Filters;
using RESOLVER = ILDynamics.Resolver.Resolver;

namespace ILDynamics.Tests
{
    /// <summary>
    /// Helper methods used within filter tests.
    /// </summary>
    public static class FilterTestHelper
    {
        public static int MultiplyBy2(int x) => x * 2;
        public static string Foo() => "foo";
        public static string Bar() => "bar";
        public static string CallFoo() => Foo();
        public static int IgnoreFirst(int x, int y) => y * 2;
        public static int UseFirst(int x, int y) => x + y;
    }

    /// <summary>
    /// Tests for basic IL filters.
    /// </summary>
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        /// <summary>
        /// NoFilter should copy a method without changes.
        /// </summary>
        public void Test_NoFilter_Copy()
        {
            MethodInfo original = typeof(FilterTestHelper).GetMethod(nameof(FilterTestHelper.MultiplyBy2));

            MethodInfo copy = RESOLVER.CopyMethod(original, new NoFilter());
            object result = copy.Invoke(null, new object[] { 3 });

            Assert.AreEqual(6, (int)result);
            Assert.AreEqual(original.GetParameters().Length, copy.GetParameters().Length);
        }

        [TestMethod]
        /// <summary>
        /// MethodCallSwapper should replace the target method invocation.
        /// </summary>
        public void Test_MethodCallSwapper_Swap()
        {
            MethodInfo callFoo = typeof(FilterTestHelper).GetMethod(nameof(FilterTestHelper.CallFoo));
            var swapper = new MethodCallSwapper();
            swapper.AddSwap(
                typeof(FilterTestHelper).GetMethod(nameof(FilterTestHelper.Foo)),
                typeof(FilterTestHelper).GetMethod(nameof(FilterTestHelper.Bar)));

            MethodInfo swapped = RESOLVER.CopyMethod(callFoo, new Filter[] { swapper, new NoFilter() });
            object result = swapped.Invoke(null, null);

            Assert.AreEqual("bar", (string)result);
        }

        [TestMethod]
        /// <summary>
        /// Validates the GetIndex logic of the ParameterRemover.
        /// </summary>
        public void Test_ParameterRemover_GetIndex()
        {
            var remover = new ParameterRemover(0);

            Assert.ThrowsException<Exception>(() => remover.GetIndex(0));
            Assert.AreEqual(0, remover.GetIndex(1));
        }
    }
}
