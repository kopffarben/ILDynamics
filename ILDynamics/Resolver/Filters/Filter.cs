using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.Resolver.Filters
{
    /// <summary>
    /// Base class for IL filters.
    /// </summary>
    public abstract class Filter
    {
        public MethodInfo Info;
        public ILGenerator IL;
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Initializes the filter base type.
        /// </summary>
        public Filter()
        {
            Initialized = false;
        }

        /// <summary>
        /// Initializes the filter with method context.
        /// </summary>
        /// <param name="info">Method being processed.</param>
        /// <param name="il">IL generator for output.</param>
        public virtual void Initialize(MethodInfo info, ILGenerator il)
        {
            this.Info = info;
            this.IL = il;
            Initialized = true;
        }

        /// <summary>
        /// Applies the filter to a single opcode and returns whether it handled the instruction.
        /// </summary>
        public abstract bool Apply(OpCode opcode, int opsize, Span<byte> operands);
    }
}
