using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.MethodGen
{
    /// <summary>
    /// Represents a parameter in IL generation.
    /// </summary>
    public class Param<T> : Param
    {
        /// <summary>
        /// Initializes a parameter of type <typeparamref name="T"/>.
        /// </summary>
        public Param() : base(typeof(T))
        {
        }
    }

    public class Param : RefableObject
    {
        public Dictionary<Method, bool> Initialized;
        public Dictionary<Method, int> Index;

        /// <summary>
        /// Constructs a parameter for the specified type.
        /// </summary>
        /// <param name="type">CLR type of the parameter.</param>
        public Param(Type type)
        {
            this.Initialized = new Dictionary<Method, bool>();
            this.Index = new Dictionary<Method, int>();
            this.Type = type;
        }

        /// <summary>
        /// Ensures the parameter has an index within the method.
        /// </summary>
        public virtual void Init(Method Method)
        {
            if (Initialized.ContainsKey(Method) && Initialized[Method] == true)
                return;
            else
            {
                Initialized[Method] = true;
                int index = Method.NewParam(this);
                Index[Method] = index;
            }
        }

        /// <summary>
        /// Loads the parameter's address onto the stack.
        /// </summary>
        public override void LoadAddress(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Ldarga_S, Index[Method]);
        }

        /// <summary>
        /// Loads the parameter value onto the stack.
        /// </summary>
        public override void Load(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Ldarg_S, Index[Method]);
        }

        /// <summary>
        /// Stores the value on the stack into the parameter.
        /// </summary>
        public override void Store(Method Method)
        {
            Init(Method);
            Method.OpCodes.Emit(OpCodes.Starg_S, Index[Method]);
        }

    }

}

