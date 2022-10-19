
using System.IO;

namespace SqlStudio.ScriptExecuter
{
    public class ScriptReaderInternalFormat : IScriptReader
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

        public string NextStatement()
        {
            if (_reader == null)
                Open();

            string line = null;
            while ((line = _reader.ReadLine()) == "")
            {
                _currentLineNumber++;
            }

            _currentLineNumber++;
            return line;
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
