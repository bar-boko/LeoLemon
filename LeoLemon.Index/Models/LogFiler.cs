using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeoLemon.Index.Models
{
    class LogFiler
    {
        private StreamWriter _sw;

        public LogFiler(string path)
        {
            _sw = new StreamWriter(path);
        }

        public void WriteToLog(string str)
        {
            _sw.Write(string.Format("{0} >> {1}\n", DateTime.Now, str));
        }

        public void Close()
        { _sw.Close(); }
    }
}
