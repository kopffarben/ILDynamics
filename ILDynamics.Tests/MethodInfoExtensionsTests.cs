using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace ILDynamics.Tests
{
    /// <summary>
    /// Tests for MethodInfo extension helpers.
    /// </summary>
    [TestClass]
    public class MethodInfoExtensionsTests
    {
        public static int Add(int x, int y) => x + y;
        public static void DoIt(int x) { }
        public static int NoParams() => 42;

        [TestMethod]
        public void Test_GetSignature()
        {
            MethodInfo mi = typeof(MethodInfoExtensionsTests).GetMethod(nameof(Add));
            string sig = mi.GetSignature();
            Assert.AreEqual("Int32 Add(Int32 x, Int32 y)", sig);
        }

        [TestMethod]
        public void Test_GetDelegateSignature_Func()
        {
            MethodInfo mi = typeof(MethodInfoExtensionsTests).GetMethod(nameof(Add));
            string sig = mi.GetDelegateSignature();
            Assert.AreEqual("Func<Int32, Int32, Int32> Add", sig);
        }

        [TestMethod]
        public void Test_GetDelegateSignature_Action()
        {
            MethodInfo mi = typeof(MethodInfoExtensionsTests).GetMethod(nameof(DoIt));
            string sig = mi.GetDelegateSignature();
            Assert.AreEqual("Action<Int32> DoIt", sig);
        }

        [TestMethod]
        public void Test_GetDelegateSignature_NoParams()
        {
            MethodInfo mi = typeof(MethodInfoExtensionsTests).GetMethod(nameof(NoParams));
            string sig = mi.GetDelegateSignature();
            Assert.AreEqual("Func<Int32> NoParams", sig);
        }

        [TestMethod]
        public void Test_DecompileToString()
        {
            MethodInfo mi = typeof(MethodInfoExtensionsTests).GetMethod(nameof(Add));
            string code = mi.DecompileToString();
            Assert.IsTrue(code.Contains("Add"));
        }
    }
}
