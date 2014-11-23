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
            months.Add("JAN", 01); months.Add("Jan", 01); months.Add("jan", 01); months.Add("JANUARY", 01); months.Add("january", 01); months.Add("January", 01);
            months.Add("FEB", 02); months.Add("Feb", 02); months.Add("feb", 02); months.Add("FEBRUARY", 02); months.Add("february", 02); months.Add("February", 02);
            months.Add("MAR", 03); months.Add("Mar", 03); months.Add("mar", 03); months.Add("MARCH", 03); months.Add("march", 03); months.Add("March", 03);
            months.Add("APR", 01); months.Add("Apr", 01); months.Add("apr", 04); months.Add("APRIL", 04); months.Add("april", 04); months.Add("April", 04);
            months.Add("MAY", 01); months.Add("May", 01); months.Add("may", 05);
            months.Add("JUN", 01); months.Add("Jun", 01); months.Add("jun", 06); months.Add("JULN", 06); months.Add("june", 06); months.Add("June", 06);
            months.Add("JUL", 01); months.Add("Jul", 01); months.Add("jul", 07); months.Add("JULY", 07); months.Add("july", 07); months.Add("July", 07);
            months.Add("AUG", 01); months.Add("Aug", 01); months.Add("aug", 08); months.Add("AUGUST", 08); months.Add("august", 08); months.Add("August", 08);
            months.Add("SEP", 01); months.Add("Sep", 01); months.Add("sep", 09); months.Add("SEPTEMBER", 09); months.Add("september", 09); months.Add("September", 09);
            months.Add("OCT", 01); months.Add("Oct", 01); months.Add("oct", 10); months.Add("OCTOBER", 10); months.Add("october", 10); months.Add("October", 10);
            months.Add("NOV", 01); months.Add("Nov", 01); months.Add("nov", 11); months.Add("NOVEMBER", 11); months.Add("november", 11); months.Add("November", 11);
            months.Add("DEC", 01); months.Add("Dec", 01); months.Add("dec", 12); months.Add("DECEMBER", 12); months.Add("december", 12); months.Add("December", 12);
            #endregion
        }

        public string FormatWord(string word)
        {
            return _stem.stemTerm(word);
        }
        public string FormatDate(string day, string month, string year)
        {
            int yearNum = Convert.ToInt32(year);
            if (yearNum < 100) yearNum = yearNum + 1900;

            int monthNum = 0;
            if (months.ContainsKey(month))
                monthNum = months[month];

            int dayNum = 0;
            if (day != "")
                dayNum = Convert.ToInt32(day);

            return String.Format("~{0}{1}{2}", dayNum.ToString("D2"), monthNum.ToString("D2"), yearNum.ToString("D4"));
        }

        public string FormatNumber(string number, string fraction = "")
        {
            double result = 0, fracNum = 0;

            if (number == "")
                number = "0";

            if (number.IndexOf('.') == number.Length - 1)
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
            return string.Format("{0} {1} {2} {3}");
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
