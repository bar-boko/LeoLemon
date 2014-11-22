using LeoLemon.Index.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Models.Interfaces
{
    public interface IIndexer
    {
        void IndexAndPost(Doc document);
    }
}
