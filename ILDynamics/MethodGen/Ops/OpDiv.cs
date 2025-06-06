using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Performs division in IL.
    /// </summary>
    public class OpDiv : ILOp
    {
        public ILOp Val1, Val2;

        /// <summary>
        /// Initializes a division operation of <paramref name="val1"/> by <paramref name="val2"/>.
        /// </summary>
        public OpDiv(ILOp val1, ILOp val2)
        {
            this.Val1 = val1;
            this.Val2 = val2;
        }

        /// <summary>
        /// Emits division of the two operands.
        /// </summary>
        public override void Load(Method Method)
        {
            Val1.Load(Method);
            Val2.Load(Method);
            Method.OpCodes.Emit(OpCodes.Div);
        }

        /// <summary>
        /// Division results cannot be stored directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
