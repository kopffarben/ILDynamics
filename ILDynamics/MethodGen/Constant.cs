using ILDynamics.MethodGen.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// IL constant value representation.
    /// </summary>
    public class Constant<T> : ILOp
    {
        public Type Type { get; private set; }

        public readonly T Value;

        /// <summary>
        /// Initializes the constant with the provided value.
        /// </summary>
        /// <param name="val">Value stored in the constant.</param>
        public Constant(T val)
        {
            Type = typeof(T);
            Value = val;
        }

        /// <summary>
        /// Emits IL to load the constant value.
        /// </summary>
        public override void Load(Method Method)
        {
            ILHelper.LoadConstant<T>(Method.OpCodes, Value);
        }

        /// <summary>
        /// Storing a constant is not supported and throws.
        /// </summary>
        public override void Store(Method Method)
        {
            throw new NotImplementedException();
        }

    }
}
