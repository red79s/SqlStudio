using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityGenerator
{
    public class ColumnDef
    {
        public ColumnDef(string columnName, string dataType)
        {
            _columnName = columnName;
            _dataType = dataType;
        }

        private string _columnName = null;

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
        private string _dataType = null;

        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }
        private bool _isKey = false;

        public bool IsKey
        {
            get { return _isKey; }
            set { _isKey = value; }
        }
        private int _dataLength = 0;

        public int DataLength
        {
            get { return _dataLength; }
            set { _dataLength = value; }
        }
        private bool _isNullable = true;

        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

        private bool _isVirtual = false;
        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { _isVirtual = value; }
        }

        public int TitleNo
        {
            get;
            set;
        }

        public string TextCase
        {
            get;
            set;
        }

        public string AttributeId
        {
            get;
            set;
        }
    }
}
