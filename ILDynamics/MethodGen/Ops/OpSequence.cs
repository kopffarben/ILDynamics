using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Sequence of IL operations executed in order.
    /// </summary>
    public class OpSequence : ILOp
    {
        public ILOp[] Operations { get; private set; }

        /// <summary>
        /// Creates a sequence that executes the provided operations.
        /// </summary>
        /// <param name="ops">Operations to execute sequentially.</param>
        public OpSequence(params ILOp[] ops)
        {
            Operations = ops;
        }

        /// <summary>
        /// Emits IL for each contained operation in order.
        /// </summary>
        public override void Load(Method m)
        {
            for (int i = 0; i < Operations.Length; i++)
                Operations[i].Load(m);
        }

        /// <summary>
        /// Sequences do not support storing their result directly.
        /// </summary>
        public override void Store(Method m)
        {
            throw new NotImplementedException();
        }
    }
}
