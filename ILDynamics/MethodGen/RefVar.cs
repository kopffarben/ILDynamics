using ILDynamics.MethodGen.Ops;
using ILDynamics.MethodGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Reference variable wrapper.
    /// </summary>
    public class RefInitJob
    {
        public RefableObject V;

        /// <summary>
        /// Container used when the referenced value is not yet initialized.
        /// </summary>
        /// <param name="v">Object providing the reference.</param>
        public RefInitJob(RefableObject v)
        {
            this.V = v;
        }
    }

    public class RefVar : Var
    {
        public Type VarType;
        private RefInitJob Job;

        /// <summary>
        /// Constructs a reference variable from another refable object.
        /// </summary>
        public RefVar(RefableObject v) : base(PointerOf(v.Type))
        {
            this.VarType = v.Type;
            this.Job = new RefInitJob(v);
        }

        /// <summary>
        /// Constructs a reference variable for the specified type.
        /// </summary>
        public RefVar(Type t) : base(PointerOf(t))
        {
            this.VarType = t;
        }

        /// <summary>
        /// Creates an assignment that writes <paramref name="val"/> via reference.
        /// </summary>
        public OpRefAssign RefAssign(ILOp val)
        {
            return new OpRefAssign(this, val);
        }

        /// <summary>
        /// Initializes the variable and sets its initial value if provided.
        /// </summary>
        public override void Init(Method Method)
        {
            base.Init(Method);
            if (this.Job != null)
            {
                RefableObject v = this.Job.V;
                this.Job = null;
                v.LoadAddress(Method);
                this.Store(Method);
            }
        }

        /// <summary>
        /// Converts a type to its by-ref representation.
        /// </summary>
        public static Type PointerOf(Type t)
        {
            return t.MakeByRefType();
        }
    }
}
