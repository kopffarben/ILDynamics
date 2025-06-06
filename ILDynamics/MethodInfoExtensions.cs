// summary: Provides MethodInfo extension methods.
using System;
using System.Linq;
using System.Reflection;

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
    }
}
