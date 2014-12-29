using LeoLemon.Index.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Engine.Interfaces
{
    public interface INormalize 
    {
         double Normalize(int value, DocumentRecord record);
    }
}
