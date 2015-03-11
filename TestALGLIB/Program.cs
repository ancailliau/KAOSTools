using System;

namespace TestALGLIB
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");

            Console.WriteLine ( alglib.invincompletebeta (1.0/5, 9.0/5, .2) );

        }
    }
}
