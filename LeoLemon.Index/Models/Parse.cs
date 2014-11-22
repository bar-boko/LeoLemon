using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeoLemon.Index.Models.Interfaces;
using LeoLemon.Index.Structures;
using System.Text.RegularExpressions;
using LeoLemon.Index.Service;

namespace LeoLemon.Index.Models
{
    public enum TokenType
    {
        DAYth, DAY, MONTH, YEAR4, YEAR2,
        NUMBER, FRACTION,
        PHRASE, EXPRESSION, NAME,
        CURRENCY, PRECENTAGE, WORD,
        UNKNOWN, THROWAWAY
    }
    class Parse : IParse
    {
        private StopWordRemover _Remover;
        private IFormat _Formatter;

        public IFormat Formatter
        {
            get { return _Formatter; }
            set { _Formatter = value; }
        }

        #region REGEXes
        private Regex _Regex_Number = new Regex(@"^(\d)$");
        private Regex _Regex_NumberFloat = new Regex(@"^(\d.\d)$");
        private Regex _Regex_NumberFraction = new Regex(@"^(\d/\d)$");
        private Regex _Regex_NumberFraction2 = new Regex(@"^(\d\\\d)$");
        private Regex _Regex_NumberThousands = new Regex(@"^(d{1,3}((,)d{3})*)$");
        private Regex _Regex_Year4 = new Regex(@"^\d{4}$");
        private Regex _Regex_Year2 = new Regex(@"^\d{2}$");
        private Regex _Regex_YearComma = new Regex(@"^(,)\d{4}$");
        private Regex _Regex_Month = new Regex(@"^(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC|jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|JANUARY|FEBRUARY|MARCH|APRIL|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|january|february|march|april|june|july|august|september|october|november|december|January|February|March|April|June|July|August|September|October|November|December)$");
        private Regex _Regex_Month_Year = new Regex(@"^\d{2}$");
        private Regex _Regex_Day = new Regex(@"^\d{1,2}$");
        private Regex _Regex_DayTH = new Regex(@"^\d{1,2}(th|rd|nd|st)$");
        private Regex _Regex_NumbersGarbage = new Regex(@"^(^(\d)|(\d.\d)|(d{1,3}((,)d{3})*)|(\d/\d)|(\d\\\d))[.,:;]$");
        private Regex _Regex_Precentage = new Regex(@"^(\d+|\d+.\d+)(%)$");
        private Regex _Regex_PrecentagePlus = new Regex(@"^(%|Precentage|precentages|prcenetage|PRECENTAGE|PRECENTAGES|Precentages)$");
        private Regex _Regex_Phrase = new Regex(@"^(([A-Z]+ |[A-Z]+))+$");
        private Regex _Regex_Name = new Regex(@"^(([A-Z][a-z]+ |[A-Z][a-z]*))+$");
        private Regex _Regex_Expression = new Regex(@"^([A-Z](a-z)+)*(-)([A-Z](a-z)+)$");
        private Regex _Regex_Word = new Regex(@"^[a-z]+$");
        private Regex _Regex_ThrowAway = new Regex(@"^[/\,.]$");
        private Regex _Regex_Currency = new Regex(@"^([A-Z]*)(Dollars|Pounds)$");
        private Regex _Regex_CurrenctAddition = new Regex(@"(m|bn)$");
        #endregion

        public Parse(StopWordRemover remover)
        {
            _Remover = remover;
            _Formatter = new LeoFormatter();
        }

        public void Execute(Doc document)
        {
            if (document == null)
                return;

            document.Header = ParseText(document.Header);
            document.Text = ParseText(document.Text);
        }

        public TokenType Classify(string str, string helper)
        {
            if (_Regex_NumbersGarbage.Match(str).Success)
            {
                return Classify(str.Substring(0, str.Length - 1), helper);
            }

            if (_Regex_Expression.Match(str).Success) return TokenType.EXPRESSION;
            if (_Regex_Phrase.Match(str).Success) return TokenType.PHRASE;
            if (_Regex_Name.Match(str).Success) return TokenType.NAME;
            if (_Regex_Precentage.Match(str).Success) return TokenType.PRECENTAGE;
            if (_Regex_Word.Match(str).Success) return TokenType.WORD;
            if (_Regex_ThrowAway.Match(str).Success) return TokenType.THROWAWAY;
            if (_Regex_Year4.Match(str).Success)
                return TokenType.YEAR4;

            if (helper == "")
            {

                if (_Regex_Number.Match(str).Success || _Regex_NumberThousands.Match(str).Success) return TokenType.NUMBER;
                return TokenType.UNKNOWN;
            }

            if (_Regex_DayTH.Match(str).Success && _Regex_Month.Match(helper).Success) return TokenType.DAYth;
            if (_Regex_Month.Match(str).Success) return TokenType.MONTH;
            if (_Regex_Currency.Match(str).Success) return TokenType.CURRENCY;

            if (_Regex_Day.Match(str).Success)
            {
                if (_Regex_Month.Match(helper).Success) return TokenType.DAY;
                if (_Regex_PrecentagePlus.Match(helper).Success) return TokenType.PRECENTAGE;
                return TokenType.NUMBER;
            }

            if (_Regex_Number.Match(str).Success || _Regex_NumberFloat.Match(str).Success || _Regex_NumberFraction.Match(str).Success || _Regex_NumberFraction2.Match(str).Success ||
                _Regex_NumberThousands.Match(str).Success)
            {
                if (_Regex_PrecentagePlus.Match(helper).Success) return TokenType.PRECENTAGE;
                return TokenType.NUMBER;
            }
            return TokenType.UNKNOWN;
        }
        private string[] ParseText(string[] tokens)
        {
            if (tokens == null)
                return null;

            List<string> result = new List<string>();
            TokenType tokenT = TokenType.UNKNOWN;
            int i = 0, j = 0;


            while (i < tokens.Length)
            {
                string temp = tokens[i], helper = "";
                if (i + 1 < tokens.Length)
                    helper = tokens[i + 1];

                tokenT = Classify(temp, helper);

                switch (tokenT)
                {
                    #region UNKNOWN
                    case TokenType.UNKNOWN:
                        result.Add(temp);
                        i++;
                        break;
                    #endregion

                    #region THROWAWAY
                    case TokenType.THROWAWAY:
                        i++;
                        break;
                    case TokenType.YEAR4:
                        temp = _Formatter.FormatDate("", "", temp);
                        result.Add(temp);
                        i++;
                        break;
                    #endregion

                    #region WORD
                    case TokenType.WORD:
                        temp = _Formatter.FormatWord(temp);
                        result.Add(temp);
                        i++;
                        break;
                    #endregion

                    #region PHRASE
                    case TokenType.PHRASE:
                        {
                            List<string> phrases = new List<string>();
                            phrases.Add(temp);

                            j = i + 1;

                            if (i + 1 < tokens.Length)
                                if (i + 2 < tokens.Length)
                                    tokenT = Classify(tokens[i + 1], tokens[i + 2]);
                                else
                                    tokenT = Classify(tokens[i + 1], "");


                            while (j < tokens.Length && tokenT == TokenType.PHRASE)
                            {
                                phrases.Add(tokens[j]);
                                j++;

                                if (j + 1 < tokens.Length)
                                    if (j + 2 < tokens.Length)
                                        tokenT = Classify(tokens[j + 1], tokens[j + 2]);
                                    else
                                        tokenT = Classify(tokens[j + 1], "");
                            }
                            i = j;

                            result.Add(_Formatter.FormatPhrase(phrases.ToArray()));
                        }
                        break;
                    #endregion

                    #region NAME
                    case TokenType.NAME:
                        {
                            List<string> names = new List<string>();
                            names.Add(temp);

                            j = i + 1;
                            if (i + 1 < tokens.Length)
                                if (i + 2 < tokens.Length)
                                    tokenT = Classify(tokens[i + 1], tokens[i + 2]);
                                else
                                    tokenT = Classify(tokens[i + 1], "");

                            while (j < tokens.Length && tokenT == TokenType.NAME)
                            {
                                names.Add(tokens[j]);
                                j++;

                                if (j + 1 < tokens.Length)
                                    if (j + 2 < tokens.Length)
                                        tokenT = Classify(tokens[j + 1], tokens[j + 2]);
                                    else
                                        tokenT = Classify(tokens[j + 1], "");
                            }
                            i = j;

                            result.Add(_Formatter.FormatPhrase(names.ToArray()));
                        }
                        break;
                    #endregion

                    #region EXPRESSION
                    case TokenType.EXPRESSION:
                        string[] exps = temp.Split('-');
                        result.Add(_Formatter.FormatExpression(exps));
                        i++;
                        break;
                    #endregion

                    #region NUMBER
                    case TokenType.NUMBER:
                        string value = temp;
                        if (i + 1 < tokens.Length)
                        {
                            string fraction = "";
                            if (_Regex_NumberFraction.Match(tokens[i + 1]).Success)
                            {
                                fraction = tokens[i + 1];
                                i += 2;
                            }
                            else
                                i++;
                            result.Add(_Formatter.FormatNumber(value, fraction));
                        }
                        else
                            i++;
                        break;
                    #endregion

                    #region PRECENTAGE
                    case TokenType.PRECENTAGE:

                        if (!_Regex_Number.Match(temp).Success)
                        {
                            temp = temp.Replace("%", "");
                            i++;
                        }
                        else
                            i += 2;
                        result.Add(_Formatter.FormatPrecentages(temp));

                        break;
                    #endregion

                    #region CURRENCY
                    case TokenType.CURRENCY:

                        string currency = temp;
                        string val = tokens[i + 1];

                        if (_Regex_Number.Match(tokens[i + 1]).Success || _Regex_NumberThousands.Match(tokens[i + 1]).Success)
                        {
                            if (i + 2 < tokens.Length)
                            {
                                if (_Regex_NumberFraction.Match(tokens[i + 2]).Success || _Regex_NumberFraction2.Match(tokens[i + 2]).Success)
                                {
                                    result.Add(_Formatter.FormatCurrency(currency, val, tokens[i + 2]));
                                    i += 3;
                                }
                                else
                                {
                                    result.Add(_Formatter.FormatCurrency(currency, val));
                                    i += 2;
                                }
                            }
                            else
                            {
                                result.Add(_Formatter.FormatCurrency(currency, val));
                                i += 2;
                            }
                        }
                        else
                        {
                            string addition = tokens[i + 1].Substring(tokens[i + 1].Length - 3, 2);
                            if (addition == "bn")
                            {
                                result.Add(_Formatter.FormatCurrency(currency, val, "", "bn"));
                            }
                            else
                            {
                                result.Add(_Formatter.FormatCurrency(currency, val, "", "m"));
                            }

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

                            result.Add(_Formatter.FormatDate(day, month, year));
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

                            if (_Regex_Day.Match(str).Success)
                            {
                                if (i + 2 < tokens.Length)
                                {
                                    string str2 = tokens[i + 2].Replace(",", "");
                                    if (_Regex_Year4.Match(str2).Success)
                                    {
                                        result.Add(_Formatter.FormatDate(str, month, str2));
                                        i += 3;
                                    }
                                    else
                                    {
                                        result.Add(_Formatter.FormatDate(str, month, tokens[i + 3]));
                                        i += 4;
                                    }
                                }
                                else
                                {
                                    result.Add(_Formatter.FormatDate(str, month, ""));
                                    i += 2;
                                }
                            }
                            else
                            {
                                result.Add(_Formatter.FormatDate("", month, str));
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
                            if (i + 2 < tokens.Length)
                                year = tokens[i + 2];

                            result.Add(_Formatter.FormatDate(day, month, year));
                            if (year != "")
                                i += 3;
                            else
                                i += 2;
                        }
                        break;
                    #endregion
                }

                
            }
            return result.ToArray();
        }
    }
}