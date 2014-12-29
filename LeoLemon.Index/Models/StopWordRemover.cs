using LeoLemon.Index.Models.Interfaces;
using System.Collections.Generic;
using System.IO;
using LeoLemon.Index.Structures;

namespace LeoLemon.Index.Models
{

    /// <summary>
    /// StopWordRemover 
    /// base: IStopWordRemover
    /// </summary>
    public class StopWordRemover : IStopWordRemover
    {
        private StreamReader _Reader;
        public HashSet<string> _stopWord;


        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="path">The path of stopword file</param>
        public StopWordRemover(string path)
        {
            _stopWord = new HashSet<string>();
            _Reader = new StreamReader(path);


            // Read the stream of stop words 
            // and insert them to hashset _stopWord
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


        /// <summary>
        /// Removes the stop words from document.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="isUpper">if set to <c>true</c> [is upper].</param>
        public void RemoveStopWords(Doc doc, bool isUpper = false)
        {
            // Remove from Text
            RemoveStopWordsArray(doc.Text, isUpper);
            //Remove from Header
            RemoveStopWordsArray(doc.Header, isUpper);
        }


        /// <summary>
        /// Removes the stop words from array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="isUpper">if set to <c>true</c> [is upper].</param>
        private void RemoveStopWordsArray(List<string> array, bool isUpper = false)
        {
            if (array == null)
                return;

            int i;
            for (i = 0; i < array.Count; i++)
            {
                string str = array[i];

                if (isUpper)
                    str = str.ToLower();
                //Cleaning rules
               
                str = str.TrimStart(@""".,/\ ".ToCharArray());
                str = str.TrimEnd(@""".,/\ ".ToCharArray());
                

                if (string.IsNullOrEmpty(str))
                {
                    array.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!_stopWord.Contains(str))
                    array[i] = str;
                else
                {
                    array.RemoveAt(i);
                    i--;
                }
            }
        }



        /// <summary>
        /// determines if the word on the 
        /// stop words or not
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public bool InStopWords(string str)
        {
            str = str.ToLower();
            return _stopWord.Contains(str);
        }
    }
}
