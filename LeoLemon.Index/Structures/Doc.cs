using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Structures
{
    public class Doc
    {
        public string DocID { get; set; }

        public DateTime Date { get; set; }

        public string[] Header { get; set; }

        public string[] Text { get; set; }


        public Doc()  { }
        public Doc(string docID, string date, string[] header, string[] text) 
        {
            Text = text;
            DocID = docID;
            Header = header;
            Date = ParseDate(date);
        }

        public DateTime ParseDate(string date)
        {
            string year = date.Substring(0, 2);
            string month = date.Substring(2, 2);
            string day = date.Substring(4, 2);

            int yearVal = int.Parse(year), monthVal = int.Parse(month), dayVal = int.Parse(day);

            if (yearVal < DateTime.Now.Year / 100 + 1)
                yearVal += 2000;
            else
                yearVal += 1900;

            return new DateTime(yearVal, monthVal, dayVal);
        }
    }
}
