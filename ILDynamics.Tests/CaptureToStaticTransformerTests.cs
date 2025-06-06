using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using ILDynamics.Resolver.Filters;
using RESOLVER = ILDynamics.Resolver.Resolver;

namespace ILDynamics.Tests
{
    public static class TestHelper
    {
        public static int Value;
    }

    [TestClass]
    public class CaptureToStaticTransformerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestHelper.Value = 0;
        }

        [TestMethod]
        public void Test_CapturingLambda_WithOneParameter()
        {
            // Arrange: Create a capturing lambda x => x + captured
            int capturedValue = 5;
            Func<int, int> lambda = x => x + capturedValue;
            MethodInfo originalMethod = lambda.Method;

            

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // The transformed method should be static with two parameters: (int x, int captured)
            object result = transformed.Invoke(null, new object[] { 7, capturedValue });

            // Assert: 7 + 5 = 12
            Assert.AreEqual(12, (int)result);
        }

        [TestMethod]
        public void Test_CapturingLambda_NoParameters()
        {
            // Arrange: Create a capturing lambda () => captured * 2
            int capturedValue = 4;
            Func<int> lambda = () => capturedValue * 2;
            MethodInfo originalMethod = lambda.Method;

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // The transformed method should be static with one parameter: (int captured)
            object result = transformed.Invoke(null, new object[] { capturedValue });

            // Assert: 4 * 2 = 8
            Assert.AreEqual(8, (int)result);
        }

        [TestMethod]
        public void Test_CapturingLambda_ReturnCapturedOnly()
        {
            // Arrange: Create a capturing lambda () => captured
            int capturedValue = 10;
            Func<int> lambda = () => capturedValue;
            MethodInfo originalMethod = lambda.Method;

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // The transformed method should be static with one parameter: (int captured)
            object result = transformed.Invoke(null, new object[] { capturedValue });

            // Assert: returned value equals captured value
            Assert.AreEqual(capturedValue, (int)result);
        }

        [TestMethod]
        public void Test_DelegateInvocation_PreservedLogic()
        {
            // Arrange: Create a capturing lambda (int x) => x * captured - 3
            int capturedValue = 6;
            Func<int, int> lambda = x => x * capturedValue - 3;
            MethodInfo originalMethod = lambda.Method;

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // Create a delegate from the transformed MethodInfo
            var delType = typeof(Func<int, int, int>);
            Delegate del = Delegate.CreateDelegate(delType, transformed);

            // Invoke delegate: (x, captured) => x * captured - 3
            object result = del.DynamicInvoke(5, capturedValue);

            // Assert: 5 * 6 - 3 = 27
            Assert.AreEqual(27, (int)result);
        }

        [TestMethod]
        public void Test_CapturingAction_WithOneParameter()
        {
            // Arrange: Create a capturing Action<int> x => TestHelper.Value = x + captured
            int capturedValue = 7;
            Action<int> lambda = x => TestHelper.Value = x + capturedValue;
            MethodInfo originalMethod = lambda.Method;

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // Invoke transformed: (x, captured)
            transformed.Invoke(null, new object[] { 5, capturedValue });

            // Assert: Value = 5 + 7 = 12
            Assert.AreEqual(12, TestHelper.Value);
        }

        [TestMethod]
        public void Test_CapturingAction_NoParameters()
        {
            // Arrange: Create a capturing Action () => TestHelper.Value = captured * 3
            int capturedValue = 3;
            Action lambda = () => TestHelper.Value = capturedValue * 3;
            MethodInfo originalMethod = lambda.Method;

            // Act: Copy the method with the transformer filter
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());

            // Invoke transformed: (captured)
            transformed.Invoke(null, new object[] { capturedValue });

            // Assert: Value = 3 * 3 = 9
            Assert.AreEqual(9, TestHelper.Value);
        }
    }
}
