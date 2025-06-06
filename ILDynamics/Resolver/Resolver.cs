using ILDynamics.MethodGen.IL;
using ILDynamics.Resolver.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.Resolver
{
    /// <summary>
    /// Utilities for resolving and copying IL methods.
    /// </summary>
    public static class Resolver
    {
        /// <summary>
        /// Copies IL instructions from <paramref name="info"/> applying provided filters.
        /// </summary>
        /// <param name="info">Source method.</param>
        /// <param name="il">Target IL generator.</param>
        /// <param name="filters">Filters to transform IL.</param>
        public static void CopyMethodBody(MethodInfo info, ILGenerator il, IEnumerable<Filter> filters)
        {
            var methodbody = info.GetMethodBody();

            if (il != null)
                foreach (var item in methodbody.LocalVariables)
                    il.DeclareLocal(item.LocalType, item.IsPinned);

            byte[] arr = methodbody.GetILAsByteArray();

            for (int i = 0; i < arr.Length;)
            {
                var code = ILHelper.GetOpCode(arr.AsSpan(i), ref i);
                int size = code.GetOperandSize(arr.AsSpan(i));

                bool applied = false;
                foreach (var filter in filters)
                {
                    applied = applied || filter.Apply(code, size, arr.AsSpan(i));
                    if (applied)
                        break;
                }

                if (!applied)
                    throw new Exception("No filter is applied!");

                i += size;
            }
        }
        /// <summary>
        /// Creates a new method copying the body of <paramref name="m"/>.
        /// </summary>
        /// <param name="m">Method to copy.</param>
        /// <param name="filters">Optional filters.</param>
        public static MethodInfo CopyMethod(MethodInfo m, params Filter[] filters)
        {
            return CopyMethod(m, (IEnumerable<Filter>)filters);
        }


        /// <summary>
        /// Core implementation that defines and returns the copied method.
        /// </summary>
        /// <param name="m">Method to copy.</param>
        /// <param name="filters">Transform filters.</param>
        public static MethodInfo CopyMethod(MethodInfo m, IEnumerable<Filter> filters = null)
        {
            AssemblyName asmName = new AssemblyName();
            asmName.Name = "DynamicILAssembly";

            var demoAssembly = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);

            ModuleBuilder demoModule = demoAssembly.DefineDynamicModule(asmName.Name);

            TypeBuilder demoType = demoModule.DefineType("DynamicType", TypeAttributes.Public);

            var argsofm = m.GetParameters();
            Type[] args = new Type[argsofm.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = argsofm[i].ParameterType;

            if (filters != null)
            {
                foreach (var f in filters)
                {
                    if (f is Filters.CaptureToStaticTransformer)
                    {
                        var extras = Filters.CaptureToStaticTransformer.GetCapturedFieldTypes(m);
                        if (extras.Length > 0)
                            args = args.Concat(extras).ToArray();
                    }
                }
            }

            MethodBuilder factory = demoType.DefineMethod("DynamicMethod",
                MethodAttributes.Public | MethodAttributes.Static,
                m.ReturnType,
                args);

            ILGenerator il = factory.GetILGenerator();

            if (filters == null)
                filters = new List<Filter>() { new NoFilter(m, il) };

            foreach (var item in filters)
            {
                if (!item.Initialized)
                    item.Initialize(m, il);
            }

            CopyMethodBody(m, il, filters);

            Type dt = demoType.CreateType();
            return dt.GetMethod("DynamicMethod");
        }


    }
}
