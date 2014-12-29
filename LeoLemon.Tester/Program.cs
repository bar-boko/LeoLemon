using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeoLemon.Index.Models;
using LeoLemon.Engine;
using System.IO;

namespace LeoLemon.Tester
{
    class Program
    {
        static void Main(string[] args)
        {

            /// # SEARCHER


            //Searcher searcher = new Searcher(@"C:\Users\בר\Desktop\Index", true);
            //List<DocumentRecord> result = searcher.Search("blood-alcohol fatalities automobile accident");

            //Console.WriteLine("Done !");
            //Console.ReadKey();

            StreamReader pointers = new StreamReader("log.txt");
            string target = @"C:\Users\בר\Desktop\Index\LeoLemon1_Posting291214-1715.leo";

            StreamReader checkPosting = new StreamReader(target);
            StreamReader truePosting = new StreamReader(target);

            int right = 0, wrong = 0;

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;

            while(!pointers.EndOfStream)
            {
                
                string ptrStr = pointers.ReadLine();
                int ptr = int.Parse(ptrStr);

                string checkLine = File.ReadLines(target).Skip(ptr).Take(1).First();
                string trueLine = truePosting.ReadLine();

                if (checkLine == trueLine)
                {
                    right++;
                    Console.WriteLine("V");
                }
                else
                {
                    wrong++;
                    Console.WriteLine("X - {0} != {1}", checkLine, trueLine);

                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }

            double ratio = right / (right + wrong);

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Right : {0}, Wrong : {1} => Ratio = {2}", right, wrong, ratio);

            pointers.Close();
            pointers.Dispose();

            checkPosting.Close();
            checkPosting.Dispose();

            truePosting.Close();
            truePosting.Dispose();

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
