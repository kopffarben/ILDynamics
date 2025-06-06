using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Return operation for IL.
    /// </summary>
    public class OpReturn : ILOp
    {
        public ILOp Val;

        /// <summary>
        /// Initializes a return operation optionally yielding a value.
        /// </summary>
        /// <param name="a">Value to return or null for void.</param>
        public OpReturn(ILOp a = null)
        {
            this.Val = a;
            // b is assigned to a
            // a is the assignee
        }

        /// <summary>
        /// Emits IL to return from the current method.
        /// </summary>
        public override void Load(Method Method)
        {
            if (Val != null)
                Val.Load(Method);
            Method.OpCodes.Emit(System.Reflection.Emit.OpCodes.Ret);
        }

        /// <summary>
        /// Return instructions cannot be stored to another value.
        /// </summary>
        public override void Store(Method m)
        {
            throw new NotImplementedException();
        }
    }
}
