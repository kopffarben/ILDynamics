using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Negates a value in IL.
    /// </summary>
    public class OpNeg : ILOp
    {
        public ILOp Val;

        /// <summary>
        /// Initializes a negation operation for the given value.
        /// </summary>
        /// <param name="val">Value to negate.</param>
        public OpNeg(ILOp val)
        {
            this.Val = val;
        }

        /// <summary>
        /// Emits IL that negates the operand.
        /// </summary>
        public override void Load(Method Method)
        {
            this.Val.Load(Method);
            Method.OpCodes.Emit(OpCodes.Neg);
        }

        /// <summary>
        /// Negation result cannot be stored directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
