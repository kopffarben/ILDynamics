using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using ILDynamics.Resolver.Filters;
using RESOLVER = ILDynamics.Resolver.Resolver;

namespace ILDynamics.Tests
{
    /// <summary>
    /// Tests for capturing-to-static transformation.
    /// </summary>
    public static class TestHelper
    {
        public static int Value;
    }

    [TestClass]
    public class CaptureToStaticTransformerTests
    {
        [TestInitialize]
        /// <summary>
        /// Resets shared state before each test.
        /// </summary>
        public void TestInitialize()
        {
            TestHelper.Value = 0;
        }

        [TestMethod]
        /// <summary>
        /// Verifies captured parameters are turned into explicit arguments.
        /// </summary>
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
        /// <summary>
        /// Tests a capturing lambda with no original parameters.
        /// </summary>
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
        /// <summary>
        /// Ensures returning a captured value still works.
        /// </summary>
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
        /// <summary>
        /// Confirms delegates built from transformed methods behave identically.
        /// </summary>
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
        /// <summary>
        /// Verifies captured values are forwarded for an Action with parameters.
        /// </summary>
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
        /// <summary>
        /// Tests Actions without parameters that capture a value.
        /// </summary>
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

        [TestMethod]
        /// <summary>
        /// Non-capturing lambdas should be unaffected by the transformer.
        /// </summary>
        public void Test_NonCapturingLambda_Passthrough()
        {
            // Arrange: create a non capturing lambda x => x * 2
            Func<int, int> lambda = x => x * 2;
            MethodInfo originalMethod = lambda.Method;

            // Act
            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());
            object result = transformed.Invoke(null, new object[] { 4 });

            // Assert
            Assert.AreEqual(8, (int)result);
            Assert.AreEqual(originalMethod.GetParameters().Length, transformed.GetParameters().Length);
        }

        [TestMethod]
        /// <summary>
        /// Normal methods that are not delegates should remain unchanged.
        /// </summary>
        public void Test_NonDelegateMethod_Passthrough()
        {
            // Arrange: get a normal method that is not a delegate
            MethodInfo normalMethod = typeof(Math).GetMethod(nameof(Math.Abs), new[] { typeof(int) });

            // Act
            MethodInfo transformed = RESOLVER.CopyMethod(normalMethod, new CaptureToStaticTransformer());
            object result = transformed.Invoke(null, new object[] { -7 });

            // Assert
            Assert.AreEqual(7, (int)result);
            Assert.AreEqual(normalMethod.GetParameters().Length, transformed.GetParameters().Length);
        }

        [TestMethod]
        /// <summary>
        /// Handles lambdas capturing multiple values.
        /// </summary>
        public void Test_CapturingLambda_MultipleCapturedValues()
        {
            // Arrange: lambda capturing two variables
            int a = 1;
            int b = 2;
            Func<int> lambda = () => a + b;
            MethodInfo originalMethod = lambda.Method;

            MethodInfo transformed = RESOLVER.CopyMethod(originalMethod, new CaptureToStaticTransformer());
            object result = transformed.Invoke(null, new object[] { a, b });

            // Assert
            Assert.AreEqual(3, (int)result);
        }

        [TestMethod]
        /// <summary>
        /// Ensures lambda with two parameters and a capture works.
        /// </summary>
        public void Test_CapturingLambda_TwoParameters()
        {
            // Arrange: lambda (x, y) => x + y + captured
            int captured = 5;
            Func<int, int, int> lambda = (x, y) => x + y + captured;
            MethodInfo transformed = RESOLVER.CopyMethod(lambda.Method, new CaptureToStaticTransformer());

            // Act
            object result = transformed.Invoke(null, new object[] { 2, 3, captured });

            // Assert
            Assert.AreEqual(10, (int)result);
        }

        [TestMethod]
        /// <summary>
        /// Tests actions with two parameters and a capture.
        /// </summary>
        public void Test_CapturingAction_TwoParameters()
        {
            // Arrange: Action with two parameters capturing a value
            int add = 2;
            Action<int, int> lambda = (x, y) => TestHelper.Value = x + y + add;
            MethodInfo transformed = RESOLVER.CopyMethod(lambda.Method, new CaptureToStaticTransformer());

            // Act
            transformed.Invoke(null, new object[] { 3, 4, add });

            // Assert
            Assert.AreEqual(9, TestHelper.Value);
        }
    }
}
