using ILDynamics.MethodGen.Ops;
using ILDynamics.MethodGen;
using System;
using System.Reflection;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Base class for objects passed by reference.
    /// </summary>
    public abstract class RefableObject : ILOp
    {
        public Type Type { get; protected set; }

        /// <summary>
        /// Initializes a reference-capable object.
        /// </summary>
        public RefableObject()
        {

        }

        /// <summary>
        /// Creates a call operation targeting this instance.
        /// </summary>
        /// <param name="objm">Method to invoke.</param>
        /// <param name="parameters">Method arguments.</param>
        public ILOp Call(MethodInfo objm, params ILOp[] parameters)
        {
            return new OpCall(this, objm, parameters);
        }

        /// <summary>
        /// Emits IL to load the address of this object.
        /// </summary>
        public abstract void LoadAddress(Method Method);
    }
    //todo need RefObject
}
