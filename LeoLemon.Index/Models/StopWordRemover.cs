using LeoLemon.Index.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            char[] seperator = new char[2];
            seperator[0] = ' ';
            seperator[1] = '\n';
              string[] stopList = (_Reader.ToString().Split(seperator, StringSplitOptions.RemoveEmptyEntries));

            foreach (string item in stopList)
                _stopWord.Add(item);
            
        }

        public void RemoveStopWords(Structures.ParsedDoc doc)
        {
            List<string> clearList = new List<string>();
            foreach(string item in doc.Text)
            {
                if (!_stopWord.Contains(item))
                {
                    clearList.Add(item);
                }

            }
            doc.Text = clearList.ToArray(); 
            
        }
    }
}
