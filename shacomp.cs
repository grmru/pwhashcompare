using System;
using System.Collections.Generic;
using System.IO;

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
            if (args.Length >= 2)
            {
                DoSearch(args[0], new List<string>());
            }
            else
            {
                Console.WriteLine("Need 2 arguments!");
            }

            return 1;
        }

        public static void DoSearch(string sourceFilePath, List<string> linesToSearch)
        {
            System.IO.FileInfo fi = new FileInfo(sourceFilePath);
            if (!fi.Exists)
            {
                return;
            }

            using (StreamReader sr = new StreamReader(sourceFilePath))
            {             
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    if (linesToSearch.Contains(line))
                    {
                        Console.WriteLine("found");
                    }

                    DrawTextProgressBar(sr.BaseStream.Position, fi.Length); //Функция отображения консольного прогресс бара
                }                     
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("");
            Console.WriteLine("SHA1 compare program usage:");
            Console.WriteLine("shacomp [mask for files where to search] [path to file with list of searching values]");
            Console.WriteLine("");
        }

        public static void DrawTextProgressBar(long progress, long total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;
 
            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }
 
            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }
 
            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + " (" + (((double)progress * 100) / (double)total).ToString() + "%)"); //blanks at the end remove any excess
        }
    }
}