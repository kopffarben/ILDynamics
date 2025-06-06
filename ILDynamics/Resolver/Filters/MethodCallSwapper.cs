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
    /// Filter that swaps method call targets.
    /// </summary>
    public class MethodCallSwapper : Filter
    {
        public Dictionary<int, MethodInfo> SwapDict;
        private List<KeyValuePair<MethodInfo, MethodInfo>> TempList;

        /// <summary>
        /// Initializes the swapper and immediately associates it with a method.
        /// </summary>
        public MethodCallSwapper(MethodInfo info, ILGenerator il)
        {
            SwapDict = new Dictionary<int, MethodInfo>();
            TempList = new List<KeyValuePair<MethodInfo, MethodInfo>>();
            this.Initialize(info, il);
        }

        /// <summary>
        /// Default constructor for later initialization.
        /// </summary>
        public MethodCallSwapper()
        {
            SwapDict = new Dictionary<int, MethodInfo>();
            TempList = new List<KeyValuePair<MethodInfo, MethodInfo>>();
        }

        /// <summary>
        /// Prepares the swapper with the given method context.
        /// </summary>
        public override void Initialize(MethodInfo info, ILGenerator il)
        {
            base.Initialize(info, il);

            foreach (var item in TempList)
                AddSwap(item.Key, item.Value);

            TempList.Clear();
        }

        /// <summary>
        /// Registers a method call to be swapped with another target.
        /// </summary>
        public void AddSwap(MethodInfo a, MethodInfo b)
        {
            if (Initialized)
            {
                int token = Info.Module.ResolveMethod(a.MetadataToken).GetMetadataToken();

                if (SwapDict.ContainsKey(token))
                    throw new Exception("Function is already added to the filter!");

                SwapDict[token] = b;
            }
            else
            {
                TempList.Add(new KeyValuePair<MethodInfo, MethodInfo>(a, b));
            }
        }

        /// <summary>
        /// Replaces call instructions if they target a swapped method.
        /// </summary>
        public override bool Apply(OpCode code, int operandsize, Span<byte> operands)
        {
            if (operandsize == 4 && (code.Equals(OpCodes.Call) || code.Equals(OpCodes.Callvirt) || code.Equals(OpCodes.Newobj)))
            {
                int val = BinaryPrimitives.ReadInt32LittleEndian(operands);
                var currentmethod = Info.Module.ResolveMethod(val);
                int currentmethodtoken = currentmethod.GetMetadataToken();
                if (SwapDict.ContainsKey(currentmethodtoken))
                    IL.Emit(code, SwapDict[currentmethodtoken]);
                else
                    return false;
            }
            else
                return false;
            return true;
        }

    }
}
