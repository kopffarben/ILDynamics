using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Loads a variable's address.
    /// </summary>
    public class OpRefByVar : ILOp
    {
        public RefableObject Var;

        /// <summary>
        /// Initializes a reference load from the specified variable.
        /// </summary>
        /// <param name="v">Variable to reference.</param>
        public OpRefByVar(RefableObject v)
        {
            this.Var = v;
        }

        /// <summary>
        /// Emits IL to load the address of the variable.
        /// </summary>
        public override void Load(Method Method)
        {
            this.Var.LoadAddress(Method);
        }

        /// <summary>
        /// References cannot be directly stored.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
