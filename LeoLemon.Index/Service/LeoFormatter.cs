using LeoLemon.Index.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Service
{
    public class LeoFormatter : IFormat
    {
        private Stemmer _stem = new Stemmer();

        public string FormatWord(string word)
        {
            return _stem.stemTerm(word);
        }
        public string FormatDate(string day, string month, string year)
        {
            return string.Format("{0}/{1}/{2}", day, month, year);
        }

        public string FormatNumber(string number, string fraction = "")
        {
            return number + ", " + fraction;
        }

        public string FormatPhrase(string[] item)
        {
            return "the phrase";
        }

        public string FormatNames(string[] item)
        {
            return "bar bokovza";
        }

        public string FormatExpression(string[] item)
        {
            return "coca cola";
        }

        public string FormatCurrency(string currency, string value, string fraction = "", string addition = "")
        {
            return string.Format("{0} {1} {2} {3}");
        }

        public string FormatPrecentages(string number)
        {
            return number + "%";
        }
    }
}
