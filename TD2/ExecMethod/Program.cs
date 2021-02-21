using System;

namespace ExecMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 4)
                Console.WriteLine("<HTML> <BODY> Hello " + args[0] + " et " + args[1] + " et " + args[2] + " et " + args[3] + " Question 5 </BODY> </HTML>");
            else
                Console.WriteLine("ExeTest <string parameter>");
        }
    }
}