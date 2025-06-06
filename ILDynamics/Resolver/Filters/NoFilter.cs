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
    /// Pass-through filter used when no transformation is needed.
    /// </summary>

    public class NoFilter : Filter
    {
        /// <summary>
        /// Creates the filter and immediately initializes it.
        /// </summary>
        public NoFilter(MethodInfo info, ILGenerator il)
        {
            this.Initialize(info, il);
        }

        /// <summary>
        /// Parameterless constructor for deferred initialization.
        /// </summary>
        public NoFilter()
        {
        }

        /// <summary>
        /// Emits the given opcode without modification.
        /// </summary>
        public override bool Apply(OpCode code, int operandsize, Span<byte> operands)
        {
            if (operandsize == 4)
            {
                int val = BinaryPrimitives.ReadInt32LittleEndian(operands);
                if (code.Equals(OpCodes.Call) || code.Equals(OpCodes.Callvirt) || code.Equals(OpCodes.Newobj))
                {
                    var b = Info.Module.ResolveMethod(val);
                    IL.Emit(code, b as MethodInfo);
                }
                else
                {
                    IL.Emit(code, val);
                }
            }
            else if (operandsize == 2)
            {
                short val = BinaryPrimitives.ReadInt16LittleEndian(operands);
                IL.Emit(code, val);
            }
            else if (operandsize == 1)
            {
                byte val = operands[0];
                IL.Emit(code, val);
            }
            else if (operandsize == 0)
            {
                IL.Emit(code);
            }
            else
                return false;
            return true;
        }

    }

}
