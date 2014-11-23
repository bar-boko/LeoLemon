using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Service
{
    public interface IFormat
    {
        bool IsStemming { get; }
        string FormatDate(string day, string month, string year);
        string FormatNumber(string number, string fraction = "");
        string FormatPhrase(string[] item);
        string FormatNames(string[] item);
        string FormatExpression(string[] item);
        string FormatCurrency(string currency, string value, string fraction = "", string addition = "");
        string FormatPrecentages(string number);
        string FormatWord(string word);
        string FormatUnknown(string word);
    }
}
