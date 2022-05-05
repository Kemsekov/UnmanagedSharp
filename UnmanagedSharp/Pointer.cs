using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnmanagedSharp
{
    /// <summary>
    /// Wrapper for unmanaged pointers.
    /// </summary>
    public unsafe struct Pointer<T>
    where T : unmanaged
    {
        public Pointer(T* pointer)
        {
            Value = pointer;
        }
        public T* Value;
        public T Dereference()=>*Value;
        public bool IsNull()=>Value==null;
    }
}