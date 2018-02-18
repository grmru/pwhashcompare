using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace grmru.pwhashcompare
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintUsage();
            }

            if (args.Length >= 4)
            {
                DirectoryInfo d = new DirectoryInfo(args[0]);
                if (d == null || !d.Exists)
                {
                    Console.WriteLine("Directory access error!");
                    Console.WriteLine("");
                    return -1;
                }

                FileInfo[] src_files = d.GetFiles(args[1]);
                if (src_files == null || src_files.Length <= 0)
                {
                    Console.WriteLine("No source files was found!");
                    Console.WriteLine("");
                    return -10;
                }

                for (int f = 0; f < src_files.Length; f++)
                {
                    FileInfo fi = new FileInfo(args[2]);
                    if (fi == null || !fi.Exists)
                    {
                        Console.WriteLine("Error access to values file!");
                        Console.WriteLine("");
                        return -20;
                    }

                    List<string> plain_values = ReadLinesFromFile(fi.FullName);
                    List<string> hash_values = new List<string>();

                    if (args[3].Trim().ToLower() == "sha256")
                    {
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            hash_values.Add(GetSHA256Hash(plain_values[l]));
                        }
                        
                        Console.WriteLine("Your hashed values: ");
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            Console.WriteLine(hash_values[l]);
                        }

                        DoSearch(src_files[f].FullName, hash_values);
                    }
                    else if(args[3].Trim().ToLower() == "sha1")
                    {
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            hash_values.Add(GetSHA1Hash(plain_values[l]));
                        }

                        Console.WriteLine("Your hashed values: ");
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            Console.WriteLine(hash_values[l]);
                        }

                        DoSearch(src_files[f].FullName, hash_values);
                    }
                    else if(args[3].Trim().ToLower() == "md5")
                    {
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            hash_values.Add(GetMD5Hash(plain_values[l]));
                        }

                        Console.WriteLine("Your hashed values: ");
                        for (int l = 0; l < plain_values.Count; l++)
                        {
                            Console.WriteLine(hash_values[l]);
                        }

                        DoSearch(src_files[f].FullName, hash_values);
                    }
                    else
                    {
                        DoSearch(src_files[f].FullName, plain_values);
                    }
                }
            }
            else
            {
                Console.WriteLine("Need 4 arguments!");
                Console.WriteLine("");
            }

            return 1;
        }

        public static List<string> ReadLinesFromFile(string filepath)
        {
            List<string> ret = new List<string>();
            string line = string.Empty;  

            System.IO.StreamReader f = new System.IO.StreamReader(filepath);  
            while((line = f.ReadLine()) != null)  
            {  
                if (line.Trim() != string.Empty)
                {
                    ret.Add(line);
                }
            }
            f.Close();  
            
            return ret;
        }

        public static string GetSHA256Hash(string value) 
        {
            string ret = string.Empty;

            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                sb.Append(b.ToString("X2"));
            }
            ret = sb.ToString();
            return ret;
        }

        public static string GetSHA1Hash(string value) 
        {
            string ret = string.Empty;

            StringBuilder sb = new StringBuilder();

            using (SHA1 hash = SHA1Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                sb.Append(b.ToString("X2"));
            }
            ret = sb.ToString();
            return ret;
        }

        public static string GetMD5Hash(string value) 
        {
            string ret = string.Empty;

            StringBuilder sb = new StringBuilder();

            using (MD5 hash = MD5.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                sb.Append(b.ToString("X2"));
            }
            ret = sb.ToString();
            return ret;
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
                Console.WriteLine(sourceFilePath + " [Started]");                     
                string line = string.Empty;
                int posRefresh = 0;
                int lineCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    lineCount++;
                    if (linesToSearch.Contains(line))
                    {
                        for (int l = 0; l < linesToSearch.Count; l++)
                        {
                            if (line == linesToSearch[l])
                            {
                                Console.WriteLine("");
                                Console.WriteLine("[!! FOUND !!] at line: " + lineCount + " " + line + " is a value at line " + (l+1));
                            }
                        }
                    }
                    posRefresh++;
                    if (posRefresh > 1000000)
                    {
                        DrawTextProgressBar(sr.BaseStream.Position, fi.Length);
                        posRefresh = 0;
                    }
                }    
                DrawTextProgressBar(sr.BaseStream.Position, fi.Length);
                Console.WriteLine("");
                Console.WriteLine(sourceFilePath + " [Done]");
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("");
            Console.WriteLine("SHA1 compare program usage:");
            Console.WriteLine("    shacomp [directory for files where to search] [mask] [path to file with list of searching values] [sha1|sha256|md5|plain]");
            Console.WriteLine("");
        }

        public static void DrawTextProgressBar(long progress, long total)
        {
            Console.CursorLeft = 0;
            Console.Write("[");
            Console.CursorLeft = 32;
            Console.Write("]");
            Console.CursorLeft = 1;
            float chunk = 30.0f / total;
 
            int pos = 1;
            for (int i = 0; i < chunk * progress; i++)
            {
                Console.CursorLeft = pos++;
                Console.Write("#");
            }
 
            for (int i = pos; i <= 31; i++)
            {
                Console.CursorLeft = pos++;
                Console.Write(" ");
            }
 
            Console.CursorLeft = 35;
            Console.Write(progress.ToString() + " of " + total.ToString() + " (" + (((double)progress * 100) / (double)total).ToString() + "%)");
        }
    }
}