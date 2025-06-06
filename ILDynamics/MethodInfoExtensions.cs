// summary: Provides MethodInfo extension methods.
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler;

namespace ILDynamics
{
    /// <summary>
    /// Helper extensions for MethodInfo reflection.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Returns a readable method signature string.
        /// </summary>
        public static string GetSignature(this MethodInfo method)
        {
            var parameters = method.GetParameters();
            var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            return $"{method.ReturnType.Name} {method.Name}({paramList})";
        }

        /// <summary>
        /// Returns the method signature using Action/Func notation.
        /// </summary>
        public static string GetDelegateSignature(this MethodInfo method)
        {
            var parameters = method.GetParameters()
                .Select(p => p.ParameterType.Name)
                .ToList();

            string delegateType;

            if (method.ReturnType == typeof(void))
            {
                delegateType = "Action";
                if (parameters.Count > 0)
                {
                    delegateType += $"<{string.Join(", ", parameters)}>";
                }
            }
            else
            {
                delegateType = "Func";
                parameters.Add(method.ReturnType.Name);
                delegateType += $"<{string.Join(", ", parameters)}>";
            }

            return $"{delegateType} {method.Name}";
        }

        /// <summary>
        /// Decompiles the method body into a C# string using ILSpy's decompiler.
        /// </summary>
        public static string DecompileToString(this MethodInfo method)
        {
            if (string.IsNullOrEmpty(method.Module.FullyQualifiedName))
            {
                throw new ArgumentException("Method must belong to a physical module.");
            }

            var decompiler = new CSharpDecompiler(method.Module.FullyQualifiedName, new DecompilerSettings());
            var handle = MetadataTokens.MethodDefinitionHandle(method.MetadataToken);
            EntityHandle entity = handle;
            return decompiler.DecompileAsString(new[] { entity });
        }
    }
}
