using System;
using System.Collections.Generic;


namespace LeoLemon.Index.Structures
{

    /// <summary>
    /// Class Doc is a structure for the Documents
    /// extracted from the files 
    /// </summary>
    public class Doc
    {

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// DocId - Serial Number of the Doc
        /// Date 
        /// Header
        /// Text 
        /// </value>
        public string DocId { get; set; }
        public DateTime Date { get; set; }
        public List<string> Header { get; set; }
        public List<string> Text { get; set; }


        /// <summary>
        /// Initializes a new instance of the class Doc.
        /// </summary>
        public Doc()  { }


        /// <summary>
        /// Initializes a new instance of the DOC class.
        /// </summary>
        /// <param name="docID">The document identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="header">The header.</param>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException">docID</exception>
        public Doc(string docID, string date, string[] header, string[] text) 
        {
            if (docID == null) throw new ArgumentNullException("docID");
            Text = new List<string>(text);
            DocId = docID;
            Header = new List<string>(header);
            Date = ParseDate(date);
        }


        /// <summary>
        /// Parses the date string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
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
