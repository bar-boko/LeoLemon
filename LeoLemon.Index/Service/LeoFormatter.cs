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
            return "00/00/0000";
        }

        public string FormatNumber(string number, string fraction = "")
        {
            return "0.000";
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
            return "Dollars 12.50";
        }

        public string FormatPrecentages(string number)
        {
            return "0%";
        }
    }
}
