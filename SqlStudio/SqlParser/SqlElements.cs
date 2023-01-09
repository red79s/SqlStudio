using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStudio.SqlParser
{
    public enum SqlElementType { 
        KEYWORD, 
        LITERAL_SINGLE, 
        LITERAL_DOUBLE ,
        UNKNOWN, 
        SELECT, 
        UPDATE, 
        INSERT, 
        DELETE,
        FROM, 
        WHERE,
        WHERE_CLAUSE,
        SUB_QUERY, 
        JOIN, 
        VALUES, 
        SELECT_COLUMN, 
        SELECT_COLUMN_LIST, 
        FROM_TABLE, 
        TABLE_ALIAS, 
        AS, 
        ORDER, 
        BY
    };



    public class SqlElements
    {
        private SqlElementType _type = SqlElementType.UNKNOWN;
        private int _startIndex = 0;
        private int _length = 0;
        private string _value = null;
        private List<SqlElements> _subElements = null;
        private string _ref = null;
        private string _alias = null;

        public SqlElements(SqlElementType type)
        {
            _type = type;
            _subElements = new List<SqlElements>();
        }

        public SqlElements(SqlElementType type, int start, int length, string value)
        {
            _type = type;
            _startIndex = start;
            _length = length;
            _value = value;
            _subElements = new List<SqlElements>();
        }

        public List<SqlElements> SubParts
        {
            get { return _subElements; }
            set { _subElements = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int Index
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public SqlElementType ElementType
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Reference
        {
            get { return _ref; }
            set { _ref = value; }
        }

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public string AsString()
        {
            string ret = "";
            if (_type == SqlElementType.FROM || _type == SqlElementType.WHERE || _type == SqlElementType.SUB_QUERY)
                ret += Environment.NewLine;

            ret += string.Format("([{0}][{1}], index={2}, len={3})", _type.ToString(), _value, _startIndex, _length);
            if (_alias != null)
                ret += string.Format("(alias={0})", _alias);
            if (_ref != null)
                ret += string.Format("(ref={0})", _ref);

            

            return ret;
        }
    }
}
