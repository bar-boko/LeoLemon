using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeoLemon.Index.Models;

namespace LeoLemon.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "./Docs/";
            string stopWords = "";
            
            ReadFile _reader = new ReadFile(filePath);
            _reader.Execute();

        }
    }
}
