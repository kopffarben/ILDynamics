using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Represents a method call.
    /// </summary>
    public class OpCall : ILOp
    {
        public RefableObject Object;
        public MethodInfo MethodInfo;
        public ILOp[] Parameters;

        /// <summary>
        /// Initializes a call operation for the specified method.
        /// </summary>
        /// <param name="obj">Instance on which to call or <c>null</c> for static.</param>
        /// <param name="mi">Target method.</param>
        /// <param name="parameters">Arguments for the call.</param>
        public OpCall(RefableObject obj, MethodInfo mi, params ILOp[] parameters)
        {
            this.Object = obj;
            this.MethodInfo = mi;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Emits IL for invoking the method with all parameters.
        /// </summary>
        public override void Load(Method Method)
        {
            if (Object != null)
                Object.LoadAddress(Method);

            foreach (var item in Parameters)
                item.Load(Method);

            Method.OpCodes.Emit(OpCodes.Call, MethodInfo);
        }

        /// <summary>
        /// Method calls do not support storing a value directly.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }
    }
}
