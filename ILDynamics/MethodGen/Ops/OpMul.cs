using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Multiplies values.
    /// </summary>
    public class OpMul : ILOp
    {
        public ILOp[] Values;
        /// <summary>
        /// Initializes a multiplication operation for the given operands.
        /// </summary>
        /// <param name="values">Values to multiply.</param>
        public OpMul(params ILOp[] values)
        {
            this.Values = values;
        }

        /// <summary>
        /// Emits IL that multiplies all operands together.
        /// </summary>
        public override void Load(Method Method)
        {
            Values[0].Load(Method);

            for (int i = 1; i < Values.Length; i++)
            {
                var item = Values[i];
                item.Load(Method);
                Method.OpCodes.Emit(OpCodes.Mul);
            }
        }

        /// <summary>
        /// Multiplication results cannot be stored directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
