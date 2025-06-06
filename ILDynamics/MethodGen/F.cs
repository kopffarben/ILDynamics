using ILDynamics.MethodGen;
using ILDynamics.MethodGen.Ops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Factory helpers to construct IL operations.
    /// </summary>
    public static class F
    {
        /// <summary>
        /// Creates an op that returns without a value.
        /// </summary>
        public static OpReturn Return()
        {
            return new OpReturn();
        }

        /// <summary>
        /// Creates an op that returns the given value.
        /// </summary>
        /// <param name="v">Value to return.</param>
        public static OpReturn Return(ILOp v)
        {
            return new OpReturn(v);
        }

        /// <summary>
        /// Wraps a constant value in an IL operation.
        /// </summary>
        /// <param name="v">The constant value.</param>
        public static ILOp Constant<T>(T v)
        {
            return new Constant<T>(v);
        }

        /// <summary>
        /// Emits addition of all supplied values.
        /// </summary>
        /// <param name="objs">Values to add.</param>
        public static ILOp Add(params ILOp[] objs)
        {
            return new OpAdd(objs);
        }

        /// <summary>
        /// Emits subtraction of <paramref name="val2"/> from <paramref name="val1"/>.
        /// </summary>
        public static ILOp Sub(ILOp val1, ILOp val2)
        {
            return new OpSub(val1, val2);
        }

        /// <summary>
        /// Emits multiplication for all given values.
        /// </summary>
        /// <param name="objs">Values to multiply.</param>
        public static ILOp Mul(params ILOp[] objs)
        {
            return new OpMul(objs);
        }

        /// <summary>
        /// Emits division of <paramref name="val1"/> by <paramref name="val2"/>.
        /// </summary>
        public static ILOp Div(ILOp val1, ILOp val2)
        {
            return new OpDiv(val1, val2);
        }

        /// <summary>
        /// Loads the value referenced by <paramref name="obj"/>.
        /// </summary>
        public static ILOp GetValueByRef(RefableObject obj)
        {
            return new OpValueByRef(obj);
        }

        /// <summary>
        /// Loads a reference to the given object variable.
        /// </summary>
        public static ILOp GetRefByVar(RefableObject obj)
        {
            return new OpRefByVar(obj);
        }

        /// <summary>
        /// Emits a call to a static method.
        /// </summary>
        /// <param name="objm">Target method.</param>
        /// <param name="parameters">Arguments for the call.</param>
        public static OpCall StaticCall(MethodInfo objm, params ILOp[] parameters)
        {
            return new OpCall(null, objm, parameters);
        }
    }
}
