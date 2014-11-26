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
        private Dictionary<string, int> months;

        public LeoFormatter()
        {
            IsStemming = true;
            months = new Dictionary<string, int>();
            #region months
            months.Add("JAN", 1); months.Add("Jan", 1); months.Add("jan", 1); months.Add("JANUARY", 1); months.Add("january", 1); months.Add("January", 1);
            months.Add("FEB", 2); months.Add("Feb", 2); months.Add("feb", 2); months.Add("FEBRUARY", 2); months.Add("february", 2); months.Add("February", 2);
            months.Add("MAR", 3); months.Add("Mar", 3); months.Add("mar", 3); months.Add("MARCH", 3); months.Add("march", 3); months.Add("March", 3);
            months.Add("APR", 4); months.Add("Apr", 4); months.Add("apr", 4); months.Add("APRIL", 4); months.Add("april", 4); months.Add("April", 4);
            months.Add("MAY", 5); months.Add("May", 5); months.Add("may", 5);
            months.Add("JUN", 6); months.Add("Jun", 6); months.Add("jun", 6); months.Add("JULN", 6); months.Add("june", 6); months.Add("June", 6);
            months.Add("JUL", 7); months.Add("Jul", 7); months.Add("jul", 7); months.Add("JULY", 7); months.Add("july", 7); months.Add("July", 7);
            months.Add("AUG", 8); months.Add("Aug", 8); months.Add("aug", 8); months.Add("AUGUST", 8); months.Add("august", 8); months.Add("August", 8);
            months.Add("SEP", 9); months.Add("Sep", 9); months.Add("sep", 9); months.Add("SEPTEMBER", 9); months.Add("september", 9); months.Add("September", 9);
            months.Add("OCT", 10); months.Add("Oct", 10); months.Add("oct", 10); months.Add("OCTOBER", 10); months.Add("october", 10); months.Add("October", 10);
            months.Add("NOV", 11); months.Add("Nov", 11); months.Add("nov", 11); months.Add("NOVEMBER", 11); months.Add("november", 11); months.Add("November", 11);
            months.Add("DEC", 12); months.Add("Dec", 12); months.Add("dec", 12); months.Add("DECEMBER", 12); months.Add("december", 12); months.Add("December", 12);
            #endregion
        }

        public string FormatWord(string word)
        {
            return _stem.stemTerm(word);
        }
        public string FormatDate(string day, string month, string year)
        {
            int yearNum = 0;
            try
            { 
            yearNum = Convert.ToInt32(year);
            if (yearNum < 100) yearNum = yearNum + 1900;
            }
            catch
            {
                yearNum = 0;
            }

            int monthNum = 0;
            if (months.ContainsKey(month))
                monthNum = months[month];

            int dayNum = 0;
            try
            {
                yearNum = Convert.ToInt32(day);
            }
            catch
            {
                yearNum = 0;
            }

            return String.Format("~{0}{1}{2}", dayNum.ToString("D2"), monthNum.ToString("D2"), yearNum.ToString("D4"));
        }

        public string FormatNumber(string number, string fraction = "")
        {
            double result = 0, fracNum = 0;

            if (number == "")
                number = "0";

            if (number[number.Length - 1] == '.' || number[number.Length - 1] == ',')
                number = number.Substring(0, number.Length - 1);

            double num = Convert.ToDouble(number);
            if (fraction != "")
                fracNum = FractionToDouble(fraction);
            result = num + fracNum;

            return String.Format("#" + "{0:#.000}", result);
        }

        public string FormatPhrase(string[] item)
        {
            string result = "!";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                if (item[i] != "-")
                    if (IsStemming)
                        result += _stem.stemTerm(item[i]);
            }

            return result;
        }

        public string FormatNames(string[] item)
        {
            string result = "^";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                result += _stem.stemTerm(item[i]);
            }

            return result;
        }

        public string FormatExpression(string[] item)
        {
            string result = "&";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                if (IsStemming)
                    result += _stem.stemTerm(item[i]);
                if (i+1 < item.Length)
                    result += " ";
            }

            return result;
        }


        private double FractionToDouble(string fraction)
        {
            string[] split = fraction.Split('/');

            int a, b;
            int.TryParse(split[0], out a);
            int.TryParse(split[1], out b);

            return (double)a / b;
        }


        public string FormatCurrency(string currency, string value, string fraction = "", string addition = "")
        {
            return string.Format("${0}[{1} {2}{3}]", currency, value, fraction, addition);
        }

        public string FormatPrecentages(string number)
        {
            return String.Format("%" + "{0:#.000}", number);
        }

        public string FormatUnknown(string word)
        {
            word = word.Replace(",", "");
            word = word.Replace(".", "");
            return word;
        }

        public bool IsStemming
        {
            get;
            private set;
        }
    }
}
