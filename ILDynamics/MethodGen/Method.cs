using ILDynamics.MethodGen.Ops;
using ILDynamics.MethodGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ILDynamics.MethodGen.IL;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Generates dynamic methods.
    /// </summary>
    public class Method<T> : Method
    {
        /// <summary>
        /// Initializes a new generic method with the specified return type.
        /// </summary>
        public Method() : base(typeof(T))
        {

        }

        /// <summary>
        /// Invokes the created method with the supplied arguments.
        /// </summary>
        public new T this[params object[] objs]
        {
            get
            {
                return (T)base[objs];
            }
        }

    }

    public class Method
    {
        public Dictionary<Param, int> ParameterIndex { get; }
        public List<Type> ParameterTypes { get; }
        public Dictionary<Var, int> VariableIndex { get; }
        public List<Type> VariableTypes { get; }

        internal ILOpCodes OpCodes;

        public Type ReturnType;

        private MethodInfo methodInfo;

        /// <summary>
        /// Constructs a method builder for the given return type.
        /// </summary>
        /// <param name="returntype">Return type of the dynamic method.</param>
        public Method(Type returntype)
        {
            ParameterTypes = new List<Type>();
            ParameterIndex = new Dictionary<Param, int>();

            VariableIndex = new Dictionary<Var, int>();
            VariableTypes = new List<Type>();
            OpCodes = new ILOpCodes();

            this.ReturnType = returntype;
        }

        /// <summary>
        /// Invokes the dynamic method and returns its value.
        /// </summary>
        public object this[params object[] objs]
        {
            get
            {
                return methodInfo.Invoke(null, objs);
            }
        }

        /// <summary>
        /// Generates the dynamic method and returns the <see cref="MethodInfo"/>.
        /// </summary>
        public MethodInfo Create()
        {
            if (methodInfo != null)
                throw new Exception("The static method is already created!");

            AssemblyName asmName = new AssemblyName();
            asmName.Name = "DynamicILAssembly";

            var demoAssembly = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);

            ModuleBuilder demoModule = demoAssembly.DefineDynamicModule(asmName.Name);

            TypeBuilder demoType = demoModule.DefineType("DynamicType", TypeAttributes.Public);

            System.Reflection.Emit.MethodBuilder factory = demoType.DefineMethod("DynamicMethod",
                MethodAttributes.Public | MethodAttributes.Static,
                ReturnType,
                ParameterTypes.ToArray());

            ILGenerator il = factory.GetILGenerator();

            OpCodes.Generate(il);

            Type dt = demoType.CreateType();
            return methodInfo = dt.GetMethod("DynamicMethod");
        }

        /// <summary>
        /// Adds a new parameter to the method and returns its index.
        /// </summary>
        /// <param name="ilParameter">Parameter to add.</param>
        public virtual int NewParam(Param ilParameter)
        {
            if (ParameterIndex.ContainsKey(ilParameter))
                throw new Exception("You can't add the same parameter twice!");

            ParameterTypes.Add(ilParameter.Type);
            return ParameterIndex[ilParameter] = ParameterIndex.Count;
        }

        /// <summary>
        /// Declares a new local variable and returns its index.
        /// </summary>
        /// <param name="iLVariable">Variable to declare.</param>
        public virtual int NewVar(Var iLVariable)
        {
            if (VariableIndex.ContainsKey(iLVariable))
                throw new Exception("You can't add the same variable twice!");

            OpCodes.DeclareVariable(iLVariable.Type);
            VariableTypes.Add(iLVariable.Type);
            return VariableIndex[iLVariable] = VariableIndex.Count;
        }
    }
}
