using System;
using System.Collections.Generic;
using System.Text;

namespace FormatTextControl
{
    public struct TextPos
    {
        private int _line;
        private int _index;
        private int _bufferOffset;

        public TextPos(int line, int index)
        {
            _line = line;
            _index = index;
            _bufferOffset = 0;
        }

        public TextPos(int line, int index, int bufferOffset)
        {
            _line = line;
            _index = index;
            _bufferOffset = bufferOffset;
        }

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public int BufferOffset
        {
            get { return _bufferOffset; }
            set { _bufferOffset = value; }
        }

        public static bool operator <(TextPos tp1, TextPos tp2)
        {
            if (tp1.Line < tp2.Line)
                return true;
            else if (tp1.Line > tp2.Line)
                return false;
            else if (tp1.Index < tp2.Index)
                return true;
            return false;
        }

        public static bool operator >(TextPos tp1, TextPos tp2)
        {
            if (tp1.Line > tp2.Line)
                return true;
            else if (tp1.Line < tp2.Line)
                return false;
            else if (tp1.Index > tp2.Index)
                return true;
            return false;
        }

        public static bool operator >=(TextPos tp1, TextPos tp2)
        {
            return (tp1 > tp2) || (tp1 == tp2);
        }

        public static bool operator <=(TextPos tp1, TextPos tp2)
        {
            return (tp1 < tp2) || (tp1 == tp2);
        }

        public static bool operator ==(TextPos tp1, TextPos tp2)
        {
            if (tp1.Line == tp2.Line && tp1.Index == tp2.Index)
                return true;
            return false;
        }

        public static bool operator !=(TextPos tp1, TextPos tp2)
        {
            return !(tp1 == tp2);
        }

        public override bool Equals(object obj)
        {
            return this == (TextPos)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", _line, _index);
        }
    }
}
