using ILDynamics.MethodGen.Ops;
using ILDynamics.MethodGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Represents an object used in IL generation.
    /// </summary>
    public abstract class ILOp
    {

        /// <summary>
        /// Base constructor for IL operations.
        /// </summary>
        public ILOp()
        {

        }

        /// <summary>
        /// Emits IL to load this operand.
        /// </summary>
        public abstract void Load(Method m);

        /// <summary>
        /// Emits IL to store this operand.
        /// </summary>
        public abstract void Store(Method m);

        /// <summary>
        /// Returns an operation assigning <paramref name="v"/> to this operand.
        /// </summary>
        /// <param name="v">Value to assign.</param>
        public OpAssign Assign(ILOp v)
        {
            return new OpAssign(this, v);
            //v.Load();
            //this.Store();
        }
    }
}
