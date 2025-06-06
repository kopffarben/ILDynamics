// Diese Datei ist Teil des ILDynamics-Projekts und liegt unter ILDynamics/Resolver/Filters.
// Sie implementiert einen Filter, der Capturing-Lambdas (in DisplayClass/Closure-Klassen)
// in Non-Capturing-Static-Methoden umwandelt, indem er das erfasste Feld als zusätzlichen Parameter injiziert
// und Delegate-Erzeugungen entsprechend modifiziert.

using System;
using System.Buffers.Binary;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ILDynamics.MethodGen.IL;
using ILDynamics.Resolver;
using ILDynamics.Resolver.Filters;

namespace ILDynamics.Resolver.Filters
{
    /// <summary>
    /// Ein Filter, der eine Methode aus einer Closure-Klasse (capturing Lambda-Instanzmethode)
    /// in eine Non-Capturing-Static-Methode transformiert. Dabei wird das erfasste Feld als
    /// zusätzlicher letzten Parameter angenommen und alle Ldfld/Zugriffe auf das Feld durch Ldarg ersetzt.
    /// </summary>
    public class CaptureToStaticTransformer : Filter
    {
        private FieldInfo _capturedField;
        private int _extraParamIndex;

        public CaptureToStaticTransformer() { }

        public CaptureToStaticTransformer(MethodInfo info, ILGenerator il)
        {
            this.Initialize(info, il);
        }

        public override void Initialize(MethodInfo info, ILGenerator il)
        {
            base.Initialize(info, il);
            // Info.DeclaringType ist die DisplayClass (Closure)
            var closureType = Info.DeclaringType;
            if (closureType == null || !closureType.Name.Contains("DisplayClass"))
                throw new InvalidOperationException("Die Methode gehört nicht zu einer Closure DisplayClass.");

            var fields = closureType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fields.Length != 1)
                throw new NotSupportedException("Unterstützt nur genau ein erfasstes Feld in der Closure.");
            _capturedField = fields[0];

            // In der dynamischen Methode wird erwartet, dass der letzte Parameter der capturedField-Typ ist.
            // Info.GetParameters() enthält nur Parameter der Lambda-Logik (ohne this und ohne capturedField).
            _extraParamIndex = Info.GetParameters().Length; // null-basierter Index
        }

        public override bool Apply(OpCode opcode, int operandSize, Span<byte> operands)
        {
            // 1. Überspringe ldarg.0 (Instance-Zugriff auf closure 'this')
            if (opcode.Equals(OpCodes.Ldarg_0) && operandSize == 0)
            {
                // Keinen Emit durchführen, einfach überspringen.
                return true;
            }

            // 2. Ersetze ldfld capturedField durch ldarg <extraParamIndex>
            if (opcode.Equals(OpCodes.Ldfld) && operandSize == 4)
            {
                int token = BinaryPrimitives.ReadInt32LittleEndian(operands);
                var field = Info.Module.ResolveField(token);
                if (field != null && field.MetadataToken == _capturedField.MetadataToken)
                {
                    // Emit: Ldarg für den letzten Parameter (_extraParamIndex)
                    EmitLoadArg(_extraParamIndex);
                    return true;
                }
            }

            // 3. Ersetze Delegate-Erzeugung (newobj DisplayClass::.ctor -> ldloc DisplayClassInstanz -> ldftn -> newobj Func<>)
            //    Wir erkennen ldftn auf die Closure-Instanzmethode und modifizieren sequenziell.
            if ((opcode.Equals(OpCodes.Ldftn) || opcode.Equals(OpCodes.Ldvirtftn)) && operandSize == 4)
            {
                int token = BinaryPrimitives.ReadInt32LittleEndian(operands);
                var method = Info.Module.ResolveMethod(token) as MethodInfo;
                if (method != null && method.DeclaringType == _capturedField.DeclaringType)
                {
                    // Wir müssen die folgenden Instruktionen neu schreiben:
                    // (a) ldloc ClosureInstanz           -> wird beibehalten, um captured-Instanz zu laden,
                    // (b) ldftn InstanzMethode           -> ersetzen durch ldnull + ldftn NeueStaticMethode
                    // (c) newobj Func<>-Ctor              -> anpassen auf passenden Funktions-Ctor
                    // Da Filters limitiert sind (einzelne Instruktionen), können wir hier nur den Ldftn umsetzen:

                    // 3a. Emit ldnull als Target für static method delegate
                    IL.Emit(OpCodes.Ldnull);
                    // 3b. Emit ldftn auf die neue Static-Methode. Wir nehmen an, sie hat denselben Namen + "_Static".
                    var parentType = _capturedField.DeclaringType.DeclaringType;
                    var newMethodName = method.Name + "_Static";
                    var newMethod = parentType.GetMethod(newMethodName, BindingFlags.NonPublic | BindingFlags.Static);
                    if (newMethod == null)
                        throw new InvalidOperationException($"Die erwartete statische Methode '{newMethodName}' wurde nicht gefunden im Typ '{parentType.FullName}'.");
                    IL.Emit(OpCodes.Ldftn, newMethod);
                    return true;
                }
            }

            // 4. Weitergabe von Parametern: Standardmäßig werden alle anderen Instruktionen durchgereicht, falls arg-Operationen auf den Parameter-Index zutreffen
            //    Wir müssen ggf. Argumente umindexieren, wenn das Closure-Feld als letzter Parameter angehängt wurde.
            if (ILHelper.IsArgS(opcode) || ILHelper.IsArgNotS(opcode))
            {
                // Konvertiere und indexiere um
                int originalIndex;
                if (ILHelper.IsArgNotS(opcode))
                {
                    (OpCode code2, int val) = ILHelper.ConvertToS(opcode);
                    originalIndex = val;
                    opcode = code2;
                }
                else if (operandSize == 4)
                {
                    originalIndex = BinaryPrimitives.ReadInt32LittleEndian(operands);
                }
                else if (operandSize == 2)
                {
                    originalIndex = BinaryPrimitives.ReadInt16LittleEndian(operands);
                }
                else if (operandSize == 1)
                {
                    originalIndex = operands[0];
                }
                else
                {
                    return false;
                }

                // Wenn originalIndex < Anzahl der ursprünglichen Parameter, bleibt es gleich.
                // Wenn >, wird nicht unterstützt.
                if (originalIndex < _extraParamIndex)
                {
                    IL.Emit(opcode, (byte)originalIndex);
                    return true;
                }
                else if (originalIndex == _extraParamIndex)
                {
                    IL.Emit(opcode, (byte)originalIndex);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Ungültiger Parameterindex im Apply von CaptureToStaticTransformer.");
                }
            }

            // 5. Alle sonstigen IL-Instruktionen unverändert durchreichen, wenn nötig
            if (operandSize == 4 && (opcode.Equals(OpCodes.Call) || opcode.Equals(OpCodes.Callvirt) || opcode.Equals(OpCodes.Newobj)))
            {
                int val = BinaryPrimitives.ReadInt32LittleEndian(operands);
                var m2 = Info.Module.ResolveMethod(val) as MethodInfo;
                IL.Emit(opcode, m2);
                return true;
            }
            else if (operandSize == 2)
            {
                short v = BinaryPrimitives.ReadInt16LittleEndian(operands);
                IL.Emit(opcode, v);
                return true;
            }
            else if (operandSize == 1)
            {
                byte v = operands[0];
                IL.Emit(opcode, v);
                return true;
            }
            else if (operandSize == 0)
            {
                IL.Emit(opcode);
                return true;
            }

            return false;
        }

        private void EmitLoadArg(int index)
        {
            switch (index)
            {
                case 0: IL.Emit(OpCodes.Ldarg_0); break;
                case 1: IL.Emit(OpCodes.Ldarg_1); break;
                case 2: IL.Emit(OpCodes.Ldarg_2); break;
                case 3: IL.Emit(OpCodes.Ldarg_3); break;
                default: IL.Emit(OpCodes.Ldarg, (byte)index); break;
            }
        }
    }
}
