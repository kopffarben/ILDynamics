using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Variable wrapper for IL generation.
    /// </summary>
    public class Var<T> : Var
    {
        /// <summary>
        /// Constructs a typed variable of <typeparamref name="T"/>.
        /// </summary>
        public Var() : base(typeof(T))
        {

        }
    }

    public class Var : RefableObject
    {
        public Dictionary<Method, bool> Initialized;
        public Dictionary<Method, int> Index;

        /// <summary>
        /// Constructs a variable for the specified type.
        /// </summary>
        /// <param name="type">CLR type of the variable.</param>
        public Var(Type type)
        {
            this.Initialized = new Dictionary<Method, bool>();
            this.Index = new Dictionary<Method, int>();
            this.Type = type;
        }

        /// <summary>
        /// Ensures the variable is declared within the given method.
        /// </summary>
        public virtual void Init(Method Method)
        {
            if (Initialized.ContainsKey(Method) && Initialized[Method] == true)
                return;
            else
            {
                Initialized[Method] = true;
                int index = Method.NewVar(this);
                Index[Method] = index;
            }
        }

        /// <summary>
        /// Loads the variable's address onto the evaluation stack.
        /// </summary>
        public override void LoadAddress(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Ldloca_S, Index[Method]);
        }

        /// <summary>
        /// Loads the variable's value onto the stack.
        /// </summary>
        public override void Load(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Ldloc_S, Index[Method]);
        }

        /// <summary>
        /// Stores the value from the stack into the variable.
        /// </summary>
        public override void Store(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Stloc_S, Index[Method]);
        }

    }
}
