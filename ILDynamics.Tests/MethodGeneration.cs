using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ILDynamics.MethodGen.Ops;
using ILDynamics.MethodGen;
using static ILDynamics.MethodGen.F;

namespace ILDynamics.Tests
{
    /// <summary>
    /// Tests for dynamic method generation.
    /// </summary>
    [TestClass]
    public class MethodGeneration
    {
        [TestMethod]
        /// <summary>
        /// Generates a simple addition method and verifies the result.
        /// </summary>
        public void TestAdd1()
        {
            Method<int> f = new Method<int>();
            var p = new Param<int>();
            var v = new Var<int>();

            v.Assign(Add(p, Constant(2), Constant(3))).Load(f);

            Return(Add(v, p)).Load(f);

            f.Create();

            int val = f[10]; // execute the method!
            Assert.AreEqual(val, 25);
        }

        [TestMethod]
        /// <summary>
        /// Tests addition when all values are constants.
        /// </summary>
        public void TestAdd2()
        {
            Method<int> f = new Method<int>();
            var v = new Var<int>();

            v.Assign(Add(Constant(2), Constant(3))).Load(f);

            var p = new Param<int>();

            Return(Add(v, p)).Load(f);

            f.Create();

            int val = f[10];
            Assert.AreEqual(val, 15);
        }

        [TestMethod]
        /// <summary>
        /// Verifies subtraction of constants.
        /// </summary>
        public void TestSub1()
        {
            Method<int> f = new Method<int>();
            var v = new Var<int>();
            v.Assign(Sub(Constant(2), Constant(3))).Load(f);
            Return(v).Load(f);
            f.Create();
            int val = f[null]; // execute the method!
            Assert.AreEqual(val, -1);
        }

        [TestMethod]
        /// <summary>
        /// Tests nested subtraction operations.
        /// </summary>
        public void TestSub2()
        {
            Method<int> f = new Method<int>();
            var v = new Var<int>();
            v.Assign(Sub(Constant(2), Sub(Constant(5), Constant(3)))).Load(f);
            Return(v).Load(f);
            f.Create();
            int val = f[null]; // execute the method!
            Assert.AreEqual(val, 0);
        }

        [TestMethod]
        /// <summary>
        /// Checks passing references to a variable.
        /// </summary>
        public void TestRef()
        {
            Method<int> f = new Method<int>();
            Var a = new Var<int>();
            a.Assign(Constant(5)).Load(f);

            RefVar b = new RefVar(a);
            b.RefAssign(Constant(3)).Load(f);
            Return(a).Load(f);

            f.Create();

            int val = f[null];
            Assert.AreEqual(val, 3);
        }

        [TestMethod]
        /// <summary>
        /// Tests reference operations with additional variables.
        /// </summary>
        public void TestRefOp()
        {
            Method<int> f = new Method<int>();
            Var a = new Var<int>();
            a.Assign(Constant(5)).Load(f);

            RefVar b = new RefVar(a);
            b.RefAssign(Constant(3)).Load(f);

            Var c = new Var<int>();
            c.Assign(Constant(15)).Load(f);

            b.Assign(GetRefByVar(c)).Load(f);
            b.RefAssign(Constant(5)).Load(f);

            Return(Add(a, c)).Load(f);

            f.Create();

            int val = f[null];
            Assert.AreEqual(val, 8);
        }

        [TestMethod]
        /// <summary>
        /// Ensures method call operations work correctly.
        /// </summary>
        public void TestOpCall1()
        {
            Method<string> f = new Method<string>();
            var a = new Param<int>();
            var tostr = typeof(int).GetMethod("ToString", Array.Empty<Type>());
            Return(a.Call(tostr)).Load(f);
            f.Create();
            string val = f[5];
            Assert.AreEqual(val, "5");
        }

        public static int experiment1 = 5;
        /// <summary>
        /// Helper method used for dynamic call tests.
        /// </summary>
        public static void Method1(int x)
        {
            Console.WriteLine(x);
            experiment1 += x;
        }

        [TestMethod]
        /// <summary>
        /// Verifies calling a dynamic method without return value.
        /// </summary>
        public void TestOpCall2()
        {
            Method f = new Method(null);
            var p = new Param<int>();
            StaticCall(typeof(MethodGeneration).GetMethod("Method1"), p).Load(f);
            Return().Load(f);
            f.Create();
            _ = f[3];
            Assert.AreEqual(8, experiment1);
        }

    }
}
