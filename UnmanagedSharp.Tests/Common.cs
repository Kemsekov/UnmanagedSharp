using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnmanagedSharp.Tests
{
    public static class Common
    {
        public static void FillCollection<T>(ICollection<T> collection, int count, Func<int,T> generator)
        {
            for (int i = 0; i < count; i++)
            {
                collection.Add(generator(i));
            }
        }
    }
}