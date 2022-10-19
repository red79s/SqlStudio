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
            this._type = type;
            this._subElements = new List<SqlElements>();
        }

        public SqlElements(SqlElementType type, int start, int length, string value)
        {
            this._type = type;
            this._startIndex = start;
            this._length = length;
            this._value = value;
            this._subElements = new List<SqlElements>();
        }

        public List<SqlElements> SubParts
        {
            get { return this._subElements; }
            set { this._subElements = value; }
        }

        public string Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public int Index
        {
            get { return this._startIndex; }
            set { this._startIndex = value; }
        }

        public int Length
        {
            get { return this._length; }
            set { this._length = value; }
        }

        public SqlElementType ElementType
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public string Reference
        {
            get { return this._ref; }
            set { this._ref = value; }
        }

        public string Alias
        {
            get { return this._alias; }
            set { this._alias = value; }
        }

        public string AsString()
        {
            string ret = "";
            if (this._type == SqlElementType.FROM || this._type == SqlElementType.WHERE || this._type == SqlElementType.SUB_QUERY)
                ret += Environment.NewLine;

            ret += string.Format("([{0}][{1}], index={2}, len={3})", this._type.ToString(), this._value, this._startIndex, this._length);
            if (this._alias != null)
                ret += string.Format("(alias={0})", this._alias);
            if (this._ref != null)
                ret += string.Format("(ref={0})", this._ref);

            

            return ret;
        }
    }
}
