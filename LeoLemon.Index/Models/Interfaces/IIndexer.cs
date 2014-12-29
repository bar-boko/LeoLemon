using LeoLemon.Index.Structures;


namespace LeoLemon.Index.Models.Interfaces
{

    /// <summary>
    /// IIndexer interface 
    /// </summary>
    public interface IIndexer
    {
        /// <summary>
        /// Indexes the and post.
        /// </summary>
        /// <param name="document">The document.</param>
        void IndexAndPost(Doc document);
    }
}
