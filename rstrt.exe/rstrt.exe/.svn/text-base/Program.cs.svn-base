using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rstrt.exe
{
    class Program
    {
        public static void Remove(string file1, string file2)
        {
            bool removed = false;
            byte trycount = 0;

            if (File.Exists(file1))
            {
                while(!removed && trycount < 3)
                {
                    trycount++;

                    try
                    {
                        //Установка аттрибутов файла если файл существует
                        if (File.Exists(file1))
                        {
                            try
                            {
                                File.SetAttributes(file1, FileAttributes.Normal);
                            }
                            catch
                            {
                                Console.WriteLine("Cannot set file attributes for source file!");
                            }
                        }

                        if (File.Exists(file2))
                        {
                            try
                            {
                                File.SetAttributes(file2, FileAttributes.Normal);
                            }
                            catch
                            {
                                Console.WriteLine("Cannot set file attributes for destination file!");
                            }
                        }

                        Console.Write("Copying " + file1 + " with " + file2 + " ...");
                        File.Copy(file1, file2, true);
                        System.Threading.Thread.Sleep(3000);
                        Console.WriteLine("OK");
                        Console.Write("Deleting " + file1 + " ...");
                        File.Delete(file1);
                        System.Threading.Thread.Sleep(3000);
                        Console.WriteLine("OK");
                        removed = true;
                    }
                    catch (Exception ex) { Console.WriteLine("Error! " + ex.Message); removed = false; System.Threading.Thread.Sleep(3000); }
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Обновление...");

            bool p = false;
            try
            {
                System.Diagnostics.Process[] n = System.Diagnostics.Process.GetProcessesByName("zeus");
                for (int i = 0; i < n.Length; i++)
                {
                    n[i].Kill();
                    n[i].WaitForExit();
                    p = true;
                }
            }
            catch
            { 

            }
            System.Threading.Thread.Sleep(1000);
            string dirs = System.Reflection.Assembly.GetExecutingAssembly().Location;

            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(dirs));
            
            foreach (FileInfo file in dir.GetFiles("*.update"))
            {
               Remove(file.Name, file.Name.ToLower().Replace(".update", ""));
            }                   
            
            // Зачем?
            if (p)
            {
                if (args.Length == 0)
                    System.Diagnostics.Process.Start("zeus.exe");
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string str in args)
                    {
                        sb.Append(str + " ");
                    }
                    System.Diagnostics.Process.Start("zeus.exe", sb.ToString().TrimEnd());
                }
            }
        }
    }
}
