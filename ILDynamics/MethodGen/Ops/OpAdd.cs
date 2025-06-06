using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Adds multiple values in IL.
    /// </summary>
    public class OpAdd : ILOp
    {
        public ILOp[] Values;
        /// <summary>
        /// Initializes an addition operation for the provided operands.
        /// </summary>
        /// <param name="values">Operands to add.</param>
        public OpAdd(params ILOp[] values)
        {
            this.Values = values;
        }

        /// <summary>
        /// Emits IL that adds all operands together.
        /// </summary>
        public override void Load(Method Method)
        {
            Values[0].Load(Method);

            for (int i = 1; i < Values.Length; i++)
            {
                var item = Values[i];
                item.Load(Method);
                Method.OpCodes.Emit(OpCodes.Add);
            }
        }

        /// <summary>
        /// Addition results cannot be stored directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
