using LeoLemon.Index.Models;
using System;
using System.Collections.Generic;


namespace LeoLemon.Index.Service
{

    /// <summary>
    /// LeoFormatter is a class that has multiple methods
    /// to transform a token to a term in our index
    /// Base : IFormat
    /// </summary>
    public class LeoFormatter : IFormat
    {

        /// <summary>
        /// Uses the Stemmer provided by the user
        /// And a Dictionary of months;
        /// </summary>
        private Stemmer _stem = new Stemmer();
        private Dictionary<string, int> months;
        public bool IsStemming { get; set; }


        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        public LeoFormatter(bool toStem = true)
        {
            IsStemming = toStem;
            months = new Dictionary<string, int>();

            #region months
            // Add the list of months - num val to the Dictionary.
            // Will preforme lower case before examination
            months.Add("jan", 1); months.Add("january", 1); 
            months.Add("feb", 2); months.Add("february", 2);
            months.Add("mar", 3); months.Add("march", 3);
            months.Add("apr", 4); months.Add("april", 4);
            months.Add("may", 5);
            months.Add("jun", 6); months.Add("june", 6);
            months.Add("jul", 7); months.Add("july", 7);
            months.Add("aug", 8); months.Add("august", 8);
            months.Add("sep", 9); months.Add("september", 9);
            months.Add("oct", 10); months.Add("october", 10);
            months.Add("nov", 11); months.Add("november", 11);
            months.Add("dec", 12); months.Add("december", 12);
            #endregion
        }


        /// <summary>
        /// Formats the word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public string FormatWord(string word)
        {
            // return word after Stemming - lower case
            if (IsStemming)
                return _stem.stemTerm(word).ToLower();
            return word.ToLower();
        }


        /// <summary>
        /// Formats the date.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns>~DDMMYYYY</returns>
        public string FormatDate(string day, string month, string year)
        {
            //Convert Year
            int yearNum;
            try
            { yearNum = 
                yearNum = Convert.ToInt32(year);
                if (yearNum < 100) yearNum += 1900;}
            catch
            {yearNum = 0; }

            //Convert Month
            // Months to lower case , needed for Dictonary
            month = month.ToLower();
            int monthNum = 0;
            if (months.ContainsKey(month))
                monthNum = months[month];

            // Convert Day 
            int dayNum = 0;
            try
            {yearNum = Convert.ToInt32(day);}
            catch
            {yearNum = 0;}

            //Format of day ~DDMMYYY
            return String.Format("~{0}{1}{2}", dayNum.ToString("D2"), monthNum.ToString("D2"), yearNum.ToString("D4"));
        }


        /// <summary>
        /// Formats the number with\ without fraction.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns> Number Format : #ANYVAL:FIRST 5 DIGITS #0.000</returns>
        public string FormatNumber(string number, string fraction = "")
        {
            double result = 0;
            double fracNum = 0;

            if (number == "")
                number = "0";

            // find the value of double number 0.0 OR thoushands 
            if (number[number.Length - 1] == '.' || number[number.Length - 1] == ',')
                number = number.Substring(0, number.Length - 1);

            double num = Convert.ToDouble(number);

            if (fraction != "")
                // Cast fraction to double function
                fracNum = FractionToDouble(fraction);
            // sum numer + fraction;
            result = num + fracNum;

            //Number Format : #ANYVAL:FIRST 5 DIGITS #0.000
            return String.Format("#{0:#0.000}", result);
        }


        /// <summary>
        /// Formats the phrase exmple: air-condition \ Prime-minister
        /// </summary>
        /// <param name="item">The phrase.</param>
        /// <returns>phrase</returns>
        public string FormatPhrase(string[] item)
        {
            //Cleanup of uneeded signs
            string result = "";// "!";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                if (item[i] != "-")
                    if (IsStemming)
                        result += _stem.stemTerm(item[i]);
                    else
                    {
                        result += item[i];
                    }
                if (i + 1 < item.Length)
                    result += " ";
            }

            return result;
        }


        /// <summary>
        /// Formats the names. exm: Binyamin Netanyahu
        /// </summary>
        /// <param name="item">The names.</param>
        /// <returns>names</returns>
        public string FormatNames(string[] item)
        {
            //clean
            string result = ""; // "^";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                if (IsStemming)
                    result += _stem.stemTerm(item[i]);
                else
                    result += item[i];

                if (i + 1 < item.Length)
                    result += " ";
            }

            return result;
        }


        /// <summary>
        /// Formats the expression. ex:THE PICTURE
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>expression</returns>
        public string FormatExpression(string[] item)
        {
            string result = ""; // "&";
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = item[i].ToLower();
                if (IsStemming)
                    result += _stem.stemTerm(item[i]);
                else
                    result += item[i];
                if (i+1 < item.Length)
                    result += " ";
            }
            return result;
        }



        /// <summary>
        /// Fractions to double - Helper Function.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        /// <returns></returns>
        private double FractionToDouble(string fraction)
        {
            string[] split = fraction.Split('/');

            int a, b;
            int.TryParse(split[0], out a);
            int.TryParse(split[1], out b);

            return (double)a / b;
        }


        /// <summary>
        /// Formats the currency. exm: USDollers 3m
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="value">The value.</param>
        /// <param name="fraction">The fraction.</param>
        /// <param name="addition">The addition.</param>
        /// <returns>Format $currency value  addition</returns>
        public string FormatCurrency(string currency, string value, string fraction = "", string addition = "")
        {
            return string.Format("${0} {1} {2}{3}", currency, value, fraction, addition);
        }



        /// <summary>
        /// Formats the unknown.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>Clean word without signs</returns>
        public string FormatUnknown(string word)
        {
            word = word.Replace(",", "");
            word = word.Replace(".", "");
            word = word.Replace(":", "");
            word = word.Replace("'", "");
            return word.ToLower();
        }


        /// <summary>
        /// Formats the number plus. exm: Frr45m
        /// </summary>
        /// <returns>Format &Word Number Addition</returns>
        public string FormatNumberPlus(string left, double center, string right)
        {
            return String.Format("#{0} {1:#0.000}{2}", left, center, right);
        }
        

     
    }
}
