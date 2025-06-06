using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Subtracts one value from another.
    /// </summary>
    public class OpSub : ILOp
    {

        public ILOp Val1, Val2;

        /// <summary>
        /// Initializes a subtraction operation of two operands.
        /// </summary>
        public OpSub(ILOp val1, ILOp val2)
        {
            this.Val1 = val1;
            this.Val2 = val2;
        }

        /// <summary>
        /// Emits IL that subtracts the second operand from the first.
        /// </summary>
        public override void Load(Method Method)
        {
            Val1.Load(Method);
            Val2.Load(Method);
            Method.OpCodes.Emit(OpCodes.Sub);
        }

        /// <summary>
        /// Subtraction results cannot be stored directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }

}
