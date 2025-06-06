using ILGPU;
using ILGPU.Runtime;
using System;

namespace ILDynamics.Compute
{
    /// <summary>
    /// Generic tensor wrapper around a <see cref="MemoryBuffer{T}"/> with
    /// additional shape information.
    /// </summary>
    public class Tensor<T> where T : unmanaged
    {
        /// <summary>
        /// Underlying GPU buffer that stores the tensor elements.
        /// </summary>
        public MemoryBuffer<T> Buffer { get; private set; }

        /// <summary>
        /// Shape of the tensor. Can be reassigned only inside this class.
        /// </summary>
        public Shape Shape { get; private set; }

        /// <summary>
        /// Creates a tensor from an existing buffer and optional shape. When no
        /// shape is provided the shape is inferred from the buffer length.
        /// </summary>
        /// <param name="buffer">Buffer that backs this tensor.</param>
        /// <param name="shape">Optional explicit shape.</param>
        public Tensor(MemoryBuffer<T> buffer, Shape shape = null)
        {
            Buffer = buffer;
            Shape = shape ?? new Shape(buffer.Length);
        }

        /// <summary>
        /// Creates a tensor with a shape derived from the buffer length.
        /// </summary>
        /// <param name="buffer">Buffer that backs this tensor.</param>
        public Tensor(MemoryBuffer<T> buffer) : this(buffer, null)
        {
        }

        /// <summary>
        /// Initializes an empty tensor instance that can be configured later.
        /// </summary>
        public Tensor()
        {
        }


    }
}
