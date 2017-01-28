using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.ControlSimpleMenu
{
    class Helper
    {
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }
    }
}
