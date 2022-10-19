using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.CvsImport
{
    public class CvsParser
    {
        private bool _isInQuote = false;
        private bool _endOfRow = false;
        private string _input;
        private int _index = 0;
        private char _columnSeperator = ',';

        public CvsParser(string input)
        {
            _input = input;
        }

        public bool Eof
        {
            get { return _index >= _input.Length; }
        }

        public bool EndOfRow
        {
            get 
            { 
                return _endOfRow || Eof; 
            }
        }

        public bool IsSeperator
        {
            get
            {
                if (Eof)
                {
                    return false;
                }

                return _input[_index] == _columnSeperator && !_isInQuote;
            }
        }

        public void MoveNext()
        {
            if (_index >= (_input.Length - 1))
            {
                _index = _input.Length;
                return;
            }

            _index++;

            if (_isInQuote)
            {
                if (_input[_index] == '"' && CanMoveNext && _input[_index + 1] != '"')
                {
                    _index++;
                    _isInQuote = false;
                }
                
                if (_input[_index] == '"' && !CanMoveNext)
                {
                    _index++;
                    _isInQuote = false;
                }

                if (_input[_index] == '"' && CanMoveNext && _input[_index + 1] == '"')
                {
                    _index++;
                }

            }
            else if (!_isInQuote && _input[_index] == '"')
            {
                _isInQuote = true;
                _index++;
            }

            if (LineEnd)
            {
                _endOfRow = true;
                _index += 2;
                return;
            }

            _endOfRow = false;
        }

        public bool LineEnd
        {
            get
            {
                if (Eof)
                    return true;

                if (_input[_index] == '\r' && CanMoveNext && _input[_index + 1] == '\n' && !_isInQuote)
                    return true;

                return false;
            }
        }

        private bool CanMoveNext
        {
            get { return _index < (_input.Length - 1); }
        }
        public char GetCurrent()
        {
            if (_index < _input.Length)
            {
                return _input[_index];
            }

            return (char)0;
        }

        public char GetNext()
        {
            if (_index < (_input.Length - 1))
            {
                return _input[_index + 1];
            }

            return (char)0;
        }
    }
}
