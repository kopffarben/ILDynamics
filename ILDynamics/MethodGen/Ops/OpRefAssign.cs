using ILDynamics.MethodGen.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// IL operation that assigns a value by reference.
    /// </summary>
    public class OpRefAssign : ILOp
    {
        public RefVar Assignee;
        public ILOp Assigned;

        /// <summary>
        /// Creates an assignment that stores <paramref name="b"/> into <paramref name="a"/> by reference.
        /// </summary>
        public OpRefAssign(RefVar a, ILOp b)
        {
            this.Assignee = a;
            this.Assigned = b;
            // b is assigned to a
            // a is the assignee
        }

        /// <summary>
        /// Emits IL to assign the referenced value.
        /// </summary>
        public override void Load(Method Method)
        {
            Assignee.Load(Method);
            Assigned.Load(Method);
            ILHelper.StoreValueByRef(Method.OpCodes, Assignee.VarType);
        }

        /// <summary>
        /// Reference assignments cannot be stored directly.
        /// </summary>
        public override void Store(Method m)
        {
            throw new NotImplementedException();
        }
    }
}
