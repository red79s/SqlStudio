using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlStudio.SqlParser
{
    public class SqlParser
    {
        private SqlElements _query = null;
        public SqlParser()
        {
        }

        public void Parse(string query)
        {
            List<SqlElements> queryElm = this.GetElements(query);
            if (queryElm.Count < 1)
                return;

            switch (queryElm[0].ElementType)
            {
                case SqlElementType.SELECT:
                    this._query = this.ParseSelect(queryElm);
                    break;
                case SqlElementType.UPDATE:
                    this._query = this.ParseUpdate(queryElm);
                    break;
            }
        }

        public SqlElements Query
        {
            get { return this._query; }
        }

        public string GetTreeString(SqlElements startElement)
        {
            if (startElement == null)
                startElement = this._query;

            if (startElement == null)
                return "";

            string ret = startElement.AsString();
            foreach (SqlElements elm in startElement.SubParts)
                ret += this.GetTreeString(elm);

            return ret;
        }

        public List<SqlElements> GetElements(string commandLine)
        {
            List<SqlElements> stack = new List<SqlElements>();
            string regexExpression = "(\"([^\"]*)\")|('([^']*)')|([^ ;,.\\[\\]\\(\\)]+)|(;)|(,)|(.)|(\\[)|(\\])|(\\()|(\\))";

            MatchCollection matches = Regex.Matches(commandLine, regexExpression, RegexOptions.Singleline);
            foreach (Match m in matches)
            {
                string value = m.Value;
                SqlElementType type = SqlElementType.UNKNOWN;
                if (value.Length > 2)
                {
                    if (value[0] == '\'' && value[value.Length - 1] == '\'')
                    {
                        type = SqlElementType.LITERAL_SINGLE;
                        value = value.Substring(1, value.Length - 2);
                    }
                    else if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        type = SqlElementType.LITERAL_SINGLE;
                        value = value.Substring(1, value.Length - 2);
                    }
                }
                if (type == SqlElementType.UNKNOWN)
                    type = this.GetElementType(value);

                SqlElements sqlElm = new SqlElements(type, m.Index, m.Length, value);
                stack.Add(sqlElm);
            }

            return stack;
        }

        private SqlElementType GetElementType(string value)
        {
            value = value.ToLower();
            switch (value)
            {
                case "select": return SqlElementType.SELECT;
                case "update": return SqlElementType.UPDATE;
                case "insert": return SqlElementType.INSERT;
                case "delete": return SqlElementType.DELETE;
                case "from": return SqlElementType.FROM;
                case "where": return SqlElementType.WHERE;
                case "join": return SqlElementType.JOIN;
                case "values": return SqlElementType.VALUES;
                case "as": return SqlElementType.AS;
                case "order": return SqlElementType.ORDER;
                case "by": return SqlElementType.BY;
                default: return SqlElementType.UNKNOWN;
            }
        }

        private string GetSubQueryString(List<SqlElements> query, int start, int length)
        {
            string ret = "";

            for (int i = start; i < (start + length); i++)
            {
                ret += query[i].Value + " ";
            }

            if (ret.Length > 0)
                ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        public SqlElements ParseSelect(List<SqlElements> query)
        {
            query.RemoveAt(0);

            SqlElements select = new SqlElements(SqlElementType.SELECT);
            if (query.Count < 1)
                return select;

            select.Value = this.GetSubQueryString(query, 0, query.Count);
            select.Index = query[0].Index;
            select.Length = (query[query.Count - 1].Index - query[0].Index) + query[query.Count - 1].Length;

            SqlElements columns = this.ParseColumnList(query);
            if (columns != null)
                select.SubParts.Add(columns);

            SqlElements from = this.ParseFrom(query);
            if (from != null)
                select.SubParts.Add(from);

            SqlElements where = this.ParseWhere(query);
            if (where != null)
                select.SubParts.Add(where);

            return select;
        }

        public SqlElements ParseUpdate(List<SqlElements> query)
        {
            SqlElements update = query[0];
            query.RemoveAt(0);

            return update;
        }

        public SqlElements ParseColumnList(List<SqlElements> query)
        {
            int endIndex = -1;
            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].ElementType == SqlElementType.FROM)
                {
                    endIndex = i - 1;
                    break;
                }
            }

            if (endIndex < 0)
                return null;


            SqlElements columns = new SqlElements(SqlElementType.SELECT_COLUMN_LIST, query[0].Index, (query[endIndex].Index - query[0].Index) + query[endIndex].Length, this.GetSubQueryString(query, 0, endIndex + 1));

            for (int i = 0; i <= endIndex; i++)
            {
                if (query[i].Value == ",")
                    continue;

                SqlElements col = new SqlElements(SqlElementType.SELECT_COLUMN);
                col.Index = query[i].Index;
                col.Length = query[i].Length;
                col.Value = query[i].Value;

                if (i < (endIndex - 1) && query[i + 1].ElementType == SqlElementType.AS)
                {
                    col.Alias = query[i + 2].Value;
                    i += 2;
                }

                columns.SubParts.Add(col);
            }

            query.RemoveRange(0, endIndex + 1);
            
            return columns;
        }

        private SqlElements ParseFrom(List<SqlElements> query)
        {
            if (query.Count < 2 || query[0].ElementType != SqlElementType.FROM)
                return null;
            query.RemoveAt(0);

            int endIndex = -1;
            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].ElementType == SqlElementType.WHERE || query[i].ElementType == SqlElementType.ORDER)
                {
                    endIndex = i - 1;
                    break;
                }
                endIndex = i;
            }

            if (endIndex < 0)
                return null;

            SqlElements from = new SqlElements(SqlElementType.FROM);
            from.Value = this.GetSubQueryString(query, 0, endIndex + 1);
            from.Index = query[0].Index;
            from.Length = (query[endIndex].Index - query[0].Index) + query[endIndex].Length;

            for (int i = 0; i <= endIndex; i++)
            {
                if (query[i].Value == ",")
                    continue;

                SqlElements fromTable = new SqlElements(SqlElementType.FROM_TABLE);
                fromTable.Index = query[i].Index;
                fromTable.Length = query[i].Length;
                fromTable.Value = query[i].Value;

                if (i < (endIndex) && query[i + 1].Value != ",")
                {
                    fromTable.Reference = query[i + 1].Value;
                    i++;
                }

                from.SubParts.Add(fromTable);
            }

            query.RemoveRange(0, endIndex + 1);

            return from;
        }

        private SqlElements ParseWhere(List<SqlElements> query)
        {
            if (query.Count < 2 || query[0].ElementType != SqlElementType.WHERE)
                return null;
            query.RemoveAt(0);

            int endIndex = -1;
            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].ElementType == SqlElementType.ORDER)
                {
                    endIndex = i - 1;
                    break;
                }
                endIndex = i;
            }

            if (endIndex < 0)
                return null;

            SqlElements where = new SqlElements(SqlElementType.WHERE);
            where.Value = this.GetSubQueryString(query, 0, endIndex + 1);
            where.Index = query[0].Index;
            where.Length = (query[endIndex].Index - query[0].Index) + query[endIndex].Length;

            for (int i = 0; i <= endIndex; i++)
            {
                if (query[i].Value == ",")
                    continue;

                SqlElements whereClause = new SqlElements(SqlElementType.WHERE_CLAUSE);
                whereClause.Index = query[i].Index;
                whereClause.Length = query[i].Length;
                whereClause.Value = query[i].Value;



                where.SubParts.Add(whereClause);
            }

            query.RemoveRange(0, endIndex + 1);

            return where;
        }
    }
}
