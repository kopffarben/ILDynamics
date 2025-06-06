﻿using ILDynamics.MethodGen;
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
    /// Utility methods for working with IL opcodes.
    /// </summary>
    public static partial class ILHelper
    {
        /// <summary>
        /// Retrieves the <see cref="OpCode"/> with the specified numeric value.
        /// </summary>
        /// <param name="val">Opcode value.</param>
        public static OpCode GetOpCodeByValue(short val)
        {
            FieldInfo[] fields = typeof(OpCodes).GetFields();
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(OpCode))
                {
                    if (f.GetValue(null) is OpCode code)
                        if (code.Value == val)
                            return code;
                }
            }
            throw new Exception("Not Found!");
        }

        /// <summary>
        /// Parses an opcode from a byte span and updates the offset.
        /// </summary>
        /// <param name="arr">Byte sequence containing IL.</param>
        /// <param name="newoffset">Offset advanced past the opcode.</param>
        public static OpCode GetOpCode(Span<byte> arr, ref int newoffset)
        {
            if (arr[0] >= 248)
            {
                if (arr.Length <= 1)
                    throw new Exception("Damaged IL Code!");

                newoffset += 2;
                short num = BinaryPrimitives.ReadInt16BigEndian(arr);
                return GetOpCodeByValue(num);
            }
            else
            {
                newoffset += 1;
                return GetOpCodeByValue(arr[0]);
            }
        }

        //todo need all primitives
        /// <summary>
        /// Emits IL to load a value by reference of the given type.
        /// </summary>
        /// <param name="codes">Opcode stream.</param>
        /// <param name="t">Referenced type.</param>
        public static void LoadValueByRef(ILOpCodes codes, Type t)
        {
            if (t == typeof(short))
                codes.Emit(OpCodes.Ldind_I2);
            else if (t == typeof(ushort))
                codes.Emit(OpCodes.Ldind_U2);
            else if (t == typeof(int))
                codes.Emit(OpCodes.Ldind_I4);
            else if (t == typeof(uint))
                codes.Emit(OpCodes.Ldind_U4);
            else if (t == typeof(float))
                codes.Emit(OpCodes.Ldind_R4);
            else if (t == typeof(double))
                codes.Emit(OpCodes.Ldind_R8);
            else
                codes.Emit(OpCodes.Ldind_Ref);
        }

        //todo need all primitives
        /// <summary>
        /// Emits IL to store a value by reference of the given type.
        /// </summary>
        /// <param name="codes">Opcode stream.</param>
        /// <param name="t">Referenced type.</param>
        public static void StoreValueByRef(ILOpCodes codes, Type t)
        {
            if (t == typeof(short))
                codes.Emit(OpCodes.Stind_I2);
            //else if (t == typeof(ushort))
            //    codes.Emit(OpCodes.);
            else if (t == typeof(int))
                codes.Emit(OpCodes.Stind_I4);
            //else if (t == typeof(uint))
            //    codes.Emit(OpCodes.);
            else if (t == typeof(float))
                codes.Emit(OpCodes.Stind_R4);
            else if (t == typeof(double))
                codes.Emit(OpCodes.Stind_R8);
            else
                codes.Emit(OpCodes.Stind_Ref);
        }

        //todo need all primitives
        /// <summary>
        /// Emits IL to load a constant value.
        /// </summary>
        /// <typeparam name="T">Constant type.</typeparam>
        /// <param name="codes">Opcode stream.</param>
        /// <param name="val">Constant value.</param>
        public static void LoadConstant<T>(ILOpCodes codes, T val)
        {
            //if (typeof(T) == typeof(short))
            //    codes.Emit(OpCodes.Ldc_, (int)(object)val);
            //else if (typeof(T) == typeof(ushort))
            //    codes.Emit(OpCodes.Ldc_U2, (int)(object)val);
            if (typeof(T) == typeof(int))
                codes.Emit(OpCodes.Ldc_I4, (int)(object)val);
            //else if (typeof(T) == typeof(uint))
            //    codes.Emit(OpCodes.Ldc_U4, (int)(object)val);
            else if (typeof(T) == typeof(float))
                codes.Emit(OpCodes.Ldc_R4, (float)(object)val);
            else if (typeof(T) == typeof(double))
                codes.Emit(OpCodes.Ldc_R8, (float)(object)val);
            else
                throw new NotImplementedException();
        }

    }
}
