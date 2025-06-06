using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen.IL
{
    /// <summary>
    /// Additional IL utilities.
    /// </summary>
    public static partial class ILHelper
    {
        /// <summary>
        /// Determines the operand size for the given opcode.
        /// </summary>
        /// <param name="code">Opcode to inspect.</param>
        /// <param name="arr">IL bytes containing operand if needed.</param>
        public static int GetOperandSize(this OpCode code, Span<byte> arr)
        {
            return code.OperandType switch
            {
                OperandType.InlineBrTarget => 4,
                OperandType.InlineField => 4,
                OperandType.InlineI => 4,
                OperandType.InlineI8 => 8,
                OperandType.InlineMethod => 4,
                OperandType.InlineR => 8,
                OperandType.InlineString => 4,
                OperandType.InlineSwitch => BinaryPrimitives.ReadInt32LittleEndian(arr) * 4 + 4,
                OperandType.InlineTok => 4,
                OperandType.InlineType => 4,
                OperandType.InlineVar => 2,
                OperandType.ShortInlineBrTarget => 1,
                OperandType.ShortInlineI => 1,
                OperandType.ShortInlineR => 4,
                OperandType.ShortInlineVar => 1,
                OperandType.InlineNone => 0,
#pragma warning disable CS0618 // 'OperandType.InlinePhi' is obsolete: 'This API has been deprecated. https://go.microsoft.com/fwlink/?linkid=14202'
                OperandType.InlinePhi => throw new NotImplementedException(),
#pragma warning restore CS0618 // 'OperandType.InlinePhi' is obsolete: 'This API has been deprecated. https://go.microsoft.com/fwlink/?linkid=14202'
                OperandType.InlineSig => throw new NotImplementedException(),
                _ => throw new NotImplementedException("Unsupported Operand Type!"),
            };
        }

        /// <summary>
        /// Checks if the opcode uses the short argument form.
        /// </summary>
        public static bool IsArgS(OpCode code)
        {
            try
            {
                return code.Name.EndsWith("arg.s") && code.GetOperandSize(null) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the opcode is an argument load using the long form.
        /// </summary>
        public static bool IsArgNotS(OpCode code)
        {
            try
            {
                int l = code.Name.Length;
                int val = int.Parse(code.Name.Last().ToString());
                return code.Name.AsSpan(0, l - 1).EndsWith("arg.") && 0 <= val && val < 10 && code.GetOperandSize(null) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the opcode loads a local variable using the short form.
        /// </summary>
        public static bool IsLocS(OpCode code)
        {
            try
            {
                return code.Name.EndsWith("loc.s") && code.GetOperandSize(null) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the opcode loads a local variable using the long form.
        /// </summary>
        public static bool IsLocNotS(OpCode code)
        {
            try
            {
                int l = code.Name.Length;
                int val = int.Parse(code.Name.Last().ToString());
                return code.Name.AsSpan(0, l - 1).EndsWith("loc.") && 0 <= val && val < 10 && code.GetOperandSize(null) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the opcode ends with 's' and has an inline operand.
        /// </summary>
        public static bool IsS(OpCode code)
        {
            try
            {
                return code.Name.EndsWith("s") && code.GetOperandSize(null) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the opcode operates on method arguments.
        /// </summary>
        public static bool IsArg(OpCode code)
        {
            return code.Name.Contains("arg");
        }
        /// <summary>
        /// Returns true if the opcode operates on local variables.
        /// </summary>
        public static bool IsLoc(OpCode code)
        {
            return code.Name.Contains("loc");
        }

        /// <summary>
        /// Converts a numeric opcode to its short form counterpart.
        /// </summary>
        /// <param name="code">Opcode to convert.</param>
        public static (OpCode, int) ConvertToS(OpCode code)
        {
            string name = code.Name;
            string sub = name[0..^1];
            int arg = int.Parse(name.Last().ToString());

            FieldInfo[] fields = typeof(OpCodes).GetFields();
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(OpCode))
                {
                    if (f.GetValue(null) is OpCode c)
                        if (c.Name == sub + "s")
                            return (c, arg);
                }
            }
            throw new Exception("Not Found!");
        }

        /// <summary>
        /// Converts a short form opcode back to the numeric variant.
        /// </summary>
        /// <param name="code">Short form opcode.</param>
        /// <param name="arg">Numeric suffix.</param>
        public static OpCode ConvertFromS(OpCode code, int arg)
        {
            string name = code.Name;
            string sub = name[0..^1];

            FieldInfo[] fields = typeof(OpCodes).GetFields();
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(OpCode))
                {
                    if (f.GetValue(null) is OpCode c)
                        if (c.Name == sub + arg.ToString())
                            return c;
                }
            }
            throw new Exception("Not Found!");
        }
    }

}
