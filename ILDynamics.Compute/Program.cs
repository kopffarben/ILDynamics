﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDynamics.Compute
{
    /// <summary>
    /// Example entry program for compute library.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Shape s = new Shape(5, 2, 3);
            Console.WriteLine(s);
        }
    }
}
