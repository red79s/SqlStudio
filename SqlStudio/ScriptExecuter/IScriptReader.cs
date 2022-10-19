using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.ScriptExecuter
{
    public interface IScriptReader : IDisposable
    {
        void SetFile(string fileName);
        void SetStream(Stream stream);
        string NextStatement();
        int GetCurrentLine();
    }
}
