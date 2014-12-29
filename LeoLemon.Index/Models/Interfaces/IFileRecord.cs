using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Models.Interfaces
{
    public interface IFileRecord
    {
        void WriteToFile(StreamWriter sw);
    }
}
