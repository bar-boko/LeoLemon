using LeoLemon.Index.Structures;
using System.Collections.Generic;

namespace LeoLemon.Index.Models.Interfaces
{

    /// <summary>
    /// IParse interface 
    /// </summary>
    public interface IParse
    {

        /// <summary>
        /// Executes the LeoLemon Parse.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="result">The result.</param>
        void Execute(Doc document, ref Dictionary<string, List<int>> result);
    }
}
