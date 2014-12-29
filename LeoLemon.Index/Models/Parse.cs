using System.Collections.Generic;
using LeoLemon.Index.Models.Interfaces;
using LeoLemon.Index.Structures;
using LeoLemon.Index.Service;

namespace LeoLemon.Index.Models
{
    /// <summary>
    /// Enum  TokenType
    /// Goal: Token classification
    /// Each enum represents token needed to classify
    /// </summary>
    public enum TokenType
    {
        DAYth, DAY, MONTH, YEAR4, YEAR2,
        NUMBER, FRACTION, NUMBER_PLUS,
        PHRASE, EXPRESSION, NAME,
        CURRENCY, WORD,
        UNKNOWN, THROWAWAY
    }

    /// <summary>
    /// Parser ihnerites from Interface IParse
    /// Goal : Cut each Text File into sepreat Terms
    /// </summary>
    public class Parse : IParse
    {

        /// <summary>
        ///  _ remover - privet field for the StopWords File
        ///  _Formatter - Private field for a helper class
        /// </summary>
        private StopWordRemover _Remover;
        private IFormat _Formatter;
        private HashSet<string> months;


        /// <value>
        /// The formatter.
        /// </value>
        public IFormat Formatter
        {
            get { return _Formatter; }
            set { _Formatter = value; }
        }

        /// <summary>
        /// Initializes a new instance of the Parse class.
        /// </summary>
        /// <param name="remover">The remover.</param>
        public Parse(StopWordRemover remover, bool toStem)
        {
            _Remover = remover;
            _Formatter = new LeoFormatter(toStem);
            months = new HashSet<string>();
            #region months  #region months
            months.Add("jan"); months.Add("january");
            months.Add("feb"); months.Add("february");
            months.Add("mar"); months.Add("march");
            months.Add("apr"); months.Add("april");
            months.Add("may");
            months.Add("jun"); months.Add("june");
            months.Add("jul"); months.Add("july");
            months.Add("aug"); months.Add("august");
            months.Add("sep"); months.Add("september");
            months.Add("oct"); months.Add("october");
            months.Add("nov"); months.Add("november");
            months.Add("dec"); months.Add("december");
             #endregion months
        }


        /// <summary>
        /// Executes the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        public void Execute(Doc document, ref Dictionary<string, List<int>> result)
        {
            if (document == null)
                return;

            Dictionary<string, List<int>> header = ParseText(document.Header, true);
            Dictionary<string, List<int>> text = ParseText(document.Text);

            foreach (string key in text.Keys)
            {
                if (header.ContainsKey(key))
                    header[key].AddRange(text[key]);
                else
                    header[key] = text[key];
            }

            result = header;
        }
        public void Execute(List<string> lst, ref Dictionary<string, List<int>> result)
        {
            result = ParseText(lst);
        }

        /// <summary>
        /// Classifies the specified string to a term.
        /// </summary>
        /// <param name="str">string -  examined word</param>
        /// <param name="helper"> helper - the word next to the examined</param>
        /// <returns>enum TokenType </returns>
        public TokenType Classify(string str, string helper)
        {
            // THROWAWAY - empty token
            if (string.IsNullOrWhiteSpace(str))
                return TokenType.THROWAWAY;

            // CURRENCY - exp : USDollars 43.m
            if (helper != "" && Identifier.Currency(str)
                && (Identifier.AdditionalNumber(helper) || Identifier.Number(helper)))
                return TokenType.CURRENCY;

            // Date - exp: 3th MAY 
            if (Identifier.DayTH(str) && months.Contains(helper)) return TokenType.DAYth;
            if (helper != "" && months.Contains(str.ToLower())) return TokenType.MONTH;

            if (Identifier.FractionNumber(str)) return TokenType.FRACTION;
            if (Identifier.Number(str) || Identifier.FloatNumber(str))
                return TokenType.NUMBER;

            // Determine if token  EXPRESSION exm: TNT
            if (Identifier.Expression(str)) return TokenType.EXPRESSION;
            // Determine if token  PHRASE exm: Air-Port
            if (Identifier.Phrase(str)) return TokenType.PHRASE;
            // Determine if token  NAME exm: Leonid Rise
            if (Identifier.Name(str)) return TokenType.NAME;
            // Determine if token  Word exm: surprise
            if (Identifier.Word(str)) return TokenType.WORD;
            // determine if token  NAME exm: Leonid Rise

            // determine if token  DelimiterText exm: 50-50
            if (Identifier.Throwaway(str) || Identifier.DelimiterText(str)) return TokenType.THROWAWAY;

            // determine if token  NAME exm: Leonid Rise
            if (Identifier.Year(str))
                return TokenType.YEAR4;

            // Use the next word to determine TokenType
            if (helper == "")
            {
            if (Identifier.Number(str)) return TokenType.NUMBER;
                return TokenType.UNKNOWN;
            }

            if (Identifier.Day(str))
            {
             if (months.Contains(helper)) return TokenType.DAY;
                            return TokenType.NUMBER;
            }

            //determine If the string is an AdditionalNumber exm: Frr 426bn
            if(Identifier.AdditionalNumber(str))
                return TokenType.NUMBER_PLUS;

            //The term is non of the above UNKNOWN 
            return TokenType.UNKNOWN;
        }



        /// <summary>
        /// Adds to result Dictionary.
        /// </summary>
        /// <param name="lst">The LST. contains the term and the num of appreances </param>
        /// <param name="term">The term.</param>
        /// <param name="idx">The index.</param>
        /// <param name="isHeader">if set to <c>true</c> [is header].</param>
        private void AddToResult(ref Dictionary<string, List<int>> lst, string term, ref int idx, bool isHeader)
        {
            term = term.Replace("  ", " ");
            //term = term.Replace("##", "#");
            term = term.TrimStart(".,/\\' ".ToCharArray());
            term = term.TrimEnd(".,/\\' -".ToCharArray());

            if (string.IsNullOrWhiteSpace(term) || _Remover.InStopWords(term))
                return;

            if(!lst.ContainsKey(term))
                lst[term] = new List<int>();
            if (isHeader)
                lst[term].Add(-1 * idx);
            else
                lst[term].Add(idx);
            idx++;
        }


        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="isHeader">if set to <c>true</c> [is header].</param>
        /// <returns></returns>
        private Dictionary<string, List<int>> ParseText(List<string> tokens, bool isHeader = false)
        {
            if (tokens == null)
                return null;

            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            TokenType tokenT = TokenType.UNKNOWN;
            int i = 0; // The current word


            int j = 0; // The helper Word
            int idx = 0;


            while (i < tokens.Count)
            {
                string temp = tokens[i], helper = "";
                if (i + 1 < tokens.Count)
                    helper = tokens[i + 1];

                // Use the Classify function to find the term enum

                tokenT = Classify(temp, helper);

                //Switch - Case Based on each word Classify
                switch (tokenT)
                {
                    #region UNKNOWN
                     // A simple term without special classification
                    case TokenType.UNKNOWN:
                        AddToResult(ref result, _Formatter.FormatUnknown(temp), ref idx, isHeader);
                        i++;
                        break;
                    #endregion

                    #region THROWAWAY
                  //Unneeded Symbols 
                    case TokenType.THROWAWAY:
                        i++;
                        break;
                    #endregion

                    #region YEAR4
                    //Year in 4 digits ex: 1989
                    case TokenType.YEAR4:
                        temp = _Formatter.FormatDate("", "", temp);
                        AddToResult(ref result, temp, ref idx, isHeader);
                        i++;
                        break;
                    #endregion

                    #region WORD
                    // term that represents a word
                    case TokenType.WORD:
                        temp = _Formatter.FormatWord(temp);
                        AddToResult(ref result, temp, ref idx, isHeader);
                        i++;
                        break;
                    #endregion

                    #region PHRASE
                    case TokenType.PHRASE:
                        {
                            List<string> phrases = new List<string>();
                            phrases.Add(temp);

                            j = i + 1;

                            if (i + 1 < tokens.Count)
                                if (i + 2 < tokens.Count)
                                    tokenT = Classify(tokens[i + 1], tokens[i + 2]);
                                else
                                    tokenT = Classify(tokens[i + 1], "");


                            while (j < tokens.Count && tokenT == TokenType.PHRASE)
                            {
                                phrases.Add(tokens[j]);
                                j++;

                                if (j + 1 < tokens.Count)
                                    if (j + 2 < tokens.Count)
                                        tokenT = Classify(tokens[j + 1], tokens[j + 2]);
                                    else
                                        tokenT = Classify(tokens[j + 1], "");
                            }

                            i = j;


                            for (int index = 0; index < phrases.Count; index++)
                                if (_Remover.InStopWords(phrases[index]))
                                    phrases.Remove(phrases[index]);

                            AddToResult(ref result, _Formatter.FormatPhrase(phrases.ToArray()), ref idx, isHeader);
                        }
                        break;
                    #endregion

                    #region NAME
                    case TokenType.NAME:
                        {
                            List<string> names = new List<string>();
                            names.Add(temp);

                            j = i + 1;
                            if (i + 1 < tokens.Count)
                                if (i + 2 < tokens.Count)
                                    tokenT = Classify(tokens[i + 1], tokens[i + 2]);
                                else
                                    tokenT = Classify(tokens[i + 1], "");

                            while (j < tokens.Count && tokenT == TokenType.NAME)
                            {
                                names.Add(tokens[j]);
                                j++;

                                if (j + 1 < tokens.Count)
                                    if (j + 2 < tokens.Count)
                                        tokenT = Classify(tokens[j + 1], tokens[j + 2]);
                                    else
                                        tokenT = Classify(tokens[j + 1], "");
                            }
                            i = j;

                            string tempStr = "";
                            foreach (string s in names)
                                tempStr += s + " ";

                            if(!_Remover.InStopWords(tempStr.Trim()))
                                AddToResult(ref result, _Formatter.FormatNames(names.ToArray()), ref idx, isHeader);
                        }
                        break;
                    #endregion

                    #region EXPRESSION
                    case TokenType.EXPRESSION:
                        string[] exps = temp.Split('-');
                        AddToResult(ref result, _Formatter.FormatExpression(exps), ref idx, isHeader);
                        i++;
                        break;
                    #endregion

                    #region NUMBER
                    case TokenType.NUMBER:
                        string value = temp;
                        if (i + 1 < tokens.Count)
                        {
                            string fraction = "";
                            if (Identifier.FractionNumber(tokens[i + 1]))
                            {
                                fraction = tokens[i + 1];
                                i += 2;
                            }
                            else
                                i++;
                            AddToResult(ref result, _Formatter.FormatNumber(value, fraction), ref idx, isHeader);
                        }
                        else
                            i++;
                        break;
                    #endregion

                    #region CURRENCY
                    case TokenType.CURRENCY:

                        string currency = temp;
                        string val = tokens[i + 1];

                        if (Identifier.Number(tokens[i + 1]))
                        {
                            if (i + 2 < tokens.Count)
                            {
                                if (Identifier.FractionNumber(tokens[i + 2]))
                                {
                                    AddToResult(ref result, _Formatter.FormatCurrency(currency, val, tokens[i + 2]), ref idx, isHeader);
                                    i += 3;
                                }
                                else
                                {
                                    AddToResult(ref result, _Formatter.FormatCurrency(currency, val), ref idx, isHeader);
                                    i += 2;
                                }
                            }
                            else
                            {
                                AddToResult(ref result, _Formatter.FormatCurrency(currency, val), ref idx, isHeader);
                                i += 2;
                            }
                        }
                        else
                        {
                            string addition = Identifier.ReturnCurrencyAddition(tokens[i+1]);
                                val = val.Substring(0, val.Length - addition.Length);
                                AddToResult(ref result, _Formatter.FormatCurrency(currency, val, "", addition), ref idx, isHeader);

                                i += 2;
                        }
                        break;
                    #endregion

                    #region DAYth
                    case TokenType.DAYth:
                        {
                            string day = temp.Substring(0, 2);
                            string month = tokens[i + 1];
                            string year = tokens[i + 2];

                            AddToResult(ref result, _Formatter.FormatDate(day, month, year), ref idx, isHeader);
                            i += 3;
                        }
                        break;
                    #endregion

                    #region MONTH
                    case TokenType.MONTH:
                        {
                            string month = temp;
                            string str = tokens[i + 1];
                            str = str.Replace(",", "");

                            if (Identifier.Day(str))
                            {
                                if (i + 2 < tokens.Count)
                                {
                                    string str2 = tokens[i + 2].Replace(",", "");
                                    if (Identifier.Year(str2))
                                    {
                                        AddToResult(ref result, _Formatter.FormatDate(str, month, str2), ref idx, isHeader);
                                        i += 3;
                                    }
                                    else
                                    {
                                        if (i + 3 < tokens.Count)
                                        {
                                            AddToResult(ref result, _Formatter.FormatDate(str, month, tokens[i + 3]), ref idx, isHeader);
                                            i += 4;
                                        }
                                        else
                                        {
                                            AddToResult(ref result, _Formatter.FormatDate(str, month, ""), ref idx, isHeader);
                                            i += 3;
                                        }
                                    }
                                }
                                else
                                {
                                    AddToResult(ref result, _Formatter.FormatDate(str, month, ""), ref idx, isHeader);
                                    i += 2;
                                }
                            }
                            else
                            {
                                AddToResult(ref result, _Formatter.FormatDate("", month, str), ref idx, isHeader);
                                i += 2;
                            }
                        }
                        break;
                    #endregion

                    #region DAY
                    case TokenType.DAY:
                        {
                            string day = temp;
                            string month = tokens[i + 1];
                            string year = "";
                            if (i + 2 < tokens.Count)
                                year = tokens[i + 2];

                            AddToResult(ref result, _Formatter.FormatDate(day, month, year), ref idx, isHeader);
                            if (year != "")
                                i += 3;
                            else
                                i += 2;
                        }
                        break;
                    #endregion

                    #region FRACTION
                    case TokenType.FRACTION:
                        {
                            AddToResult(ref result, _Formatter.FormatNumber("", temp), ref idx, isHeader);
                            i++;
                        }
                        break;
                    #endregion

                    // Our special case which represtns the form: Frr 342.5m
                    #region NUMBER PLUS
                    case TokenType.NUMBER_PLUS:
                        {
                            string left  = "", right = "";
                            double center = 0;

                            Identifier.AdditionalNumber(temp, ref left, ref center, ref right);
                            if(!string.IsNullOrEmpty(left))
                                AddToResult(ref result, _Formatter.FormatNumberPlus(left, center, right), ref idx, isHeader);
                            AddToResult(ref result, string.Format("#{0:#0.000}{1}", center, right), ref idx, isHeader);
                            i++;

                        }
                        break;
                    #endregion
                }

                
            }
            return result;
        }
    }
}