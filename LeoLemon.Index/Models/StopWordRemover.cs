using LeoLemon.Index.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeoLemon.Index.Structures;

namespace LeoLemon.Index.Models
{
    class StopWordRemover : IStopWordRemover
    {
        private StreamReader _Reader;
        public HashSet<string> _stopWord;

        public StopWordRemover(string path)
        {
            _stopWord = new HashSet<string>();
            _Reader = new StreamReader(path);

            {
                List<string> lst = new List<string>();

                while (!_Reader.EndOfStream)
                    lst.Add(_Reader.ReadLine());

                foreach (string str in lst)
                {
                    string[] splitted = str.Split(' ');
                    foreach (string s in splitted)
                        _stopWord.Add(s);
                }
            }

        }

        public void RemoveStopWords(Doc doc, bool isUpper = false)
        {
            List<string> clearList = new List<string>();
            foreach(string item in doc.Text)
            {
                string str = item;
                if (isUpper)
                    str = item.ToLower();
                    

                if (!_stopWord.Contains(str))
                    clearList.Add(item);

            }
            doc.Text = clearList.ToArray(); 
        }
    }
}
