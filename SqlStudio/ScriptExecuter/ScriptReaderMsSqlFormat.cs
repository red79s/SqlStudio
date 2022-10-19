using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.ScriptExecuter
{
    public class ScriptReaderMsSqlFormat : IScriptReader
    {
        private string _fileName;
        private Stream _stream;
        private int _currentLineNumber = 0;

        public void SetFile(string fileName)
        {
            _fileName = fileName;
        }

        public void SetStream(Stream stream)
        {
            _stream = stream;
        }

        private bool _goNext = false;

        public string NextStatement()
        {
            if (_reader == null)
                Open();

            if (_goNext)
            {
                _goNext = false;
                return "GO";
            }

            string cmd = "";

            while (true)
            {
                var line = _reader.ReadLine();
                _currentLineNumber++;

                if (line == null)
                {
                    return null;
                }
                if (line.Trim() == "")
                {
                    continue;
                }
                if (line.StartsWith("/*"))
                {
                    continue;
                }
                if (line == "GO")
                {
                    if (cmd != "")
                    {
                        _goNext = true;
                        return cmd;
                    }

                    return line;
                }
                
                if (cmd != "")
                {
                    cmd += Environment.NewLine;
                }
                cmd += line;
            }
        }

        public int GetCurrentLine()
        {
            return _currentLineNumber;
        }

        private StreamReader _reader = null;
        private void Open()
        {
            if (_stream != null)
            {
                _reader = new StreamReader(_stream);
            }
            else
            {
                _reader = new StreamReader(_fileName);
            }
        }

        private void Close()
        {
            if (_reader != null)
                _reader.Close();
            _reader = null;
        }

        public void Dispose()
        {
            if (_reader != null)
                Close();
        }
    }
}
