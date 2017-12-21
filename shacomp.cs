using System;
using System.Collections.Generic;

namespace grmru.shacomp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintUsage();
            }

            return 1;
        }

        public static void PrintUsage()
        {
            Console.WriteLine("SHA1 compare program usage:");
            Console.WriteLine("shacomp [mask for files where to search] [path to file with list of searching values]");
        }
    }
}