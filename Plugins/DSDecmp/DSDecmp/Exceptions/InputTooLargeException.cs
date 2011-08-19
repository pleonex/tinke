using System;
using System.Collections.Generic;
using System.Text;

namespace DSDecmp
{
    public class InputTooLargeException : Exception
    {
        public InputTooLargeException()
            : base(Main.Get_Traduction("S0E")) { }
    }
}
