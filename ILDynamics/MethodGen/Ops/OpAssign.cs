using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Assigns a value to a variable.
    /// </summary>
    public class OpAssign : ILOp
    {
        public ILOp Assignee;
        public ILOp Assigned;

        /// <summary>
        /// Creates an assignment from <paramref name="b"/> to <paramref name="a"/>.
        /// </summary>
        public OpAssign(ILOp a, ILOp b)
        {
            this.Assignee = a;
            this.Assigned = b;
            // b is assigned to a
            // a is the assignee
        }

        /// <summary>
        /// Loads the value and stores it into the assignee.
        /// </summary>
        public override void Load(Method m)
        {
            Assigned.Load(m);
            Assignee.Store(m);
        }

        /// <summary>
        /// Assignment cannot itself be assigned to another value.
        /// </summary>
        public override void Store(Method m)
        {
            throw new NotImplementedException();
        }
    }
}
