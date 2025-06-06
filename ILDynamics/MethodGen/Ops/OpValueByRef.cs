using ILDynamics.MethodGen.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.Ops
{
    /// <summary>
    /// Loads a value via reference.
    /// </summary>
    public class OpValueByRef : ILOp
    {
        public ILOp Object;
        public Type ObjectType;

        /// <summary>
        /// Initializes a load-by-reference operation.
        /// </summary>
        /// <param name="obj">Object whose value will be loaded.</param>
        /// <param name="t">Type of the referenced value.</param>
        public OpValueByRef(ILOp obj, Type t)
        {
            this.Object = obj;
            this.ObjectType = t;
        }

        /// <summary>
        /// Initializes a load-by-reference using the object's type.
        /// </summary>
        public OpValueByRef(RefableObject obj)
        {
            this.Object = obj;
            this.ObjectType = obj.Type;
        }

        /// <summary>
        /// Emits IL to load the referenced value.
        /// </summary>
        public override void Load(Method Method)
        {
            Object.Load(Method);
            ILHelper.LoadValueByRef(Method.OpCodes, this.ObjectType);
        }

        /// <summary>
        /// Values loaded by reference cannot be stored via this op.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }

    }
}
