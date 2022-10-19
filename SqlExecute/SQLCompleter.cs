using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GoldParser;
using System.Data;

namespace SqlExecute
{
    public class SQLCompleter
    {
        public delegate void DebugMessageDelegate(object sender, string msg);
        public event DebugMessageDelegate DebugMessage;

        private Grammar _grammar = null;
        private string _loadingError = null;
        private DbSchemaCache.DbSchemaCache _dbCache = null;

        public SQLCompleter()
        {
            try
            {
                byte[] buffer = File.ReadAllBytes(System.Windows.Forms.Application.StartupPath +  @"\SQL-ANSI-89.cgt");
                MemoryStream ms = new MemoryStream(buffer);
                _grammar = new Grammar(new BinaryReader(ms));
            }
            catch (Exception ex)
            {
                _grammar = null;
                _loadingError = ex.Message;
            }
        }

        public DbSchemaCache.DbSchemaCache DBCache
        {
            get 
            {
                lock (this)
                {
                    return _dbCache;
                }
            }
            set 
            {
                lock (this)
                {
                    _dbCache = value;
                }
            }
        }

        public List<string> GetPosibleCompletions(string cmd, int index)
        {
            if (cmd == null || cmd.Length < 1)
                return new List<string>();
            if (index < 0 || index >= cmd.Length)
                return new List<string>();

            if (_grammar == null)
            {
                if (DebugMessage != null)
                    DebugMessage(this, _loadingError);

                return new List<string>();
            }

            List<TerminalNode> tokens = new List<TerminalNode>();
            List<NonTerminalNode> reductions = new List<NonTerminalNode>();
            List<string> ret = new List<string>();
            try
            {
                Parser parser = new Parser(new StringReader(cmd), _grammar);
                int reductionNumber = 0;
                bool bFinished = false;
                while (!bFinished)
                {
                    ParseMessage pMessage = parser.Parse();
                    switch (pMessage)
                    {
                        case ParseMessage.SyntaxError:
                            if (parser.TokenSymbol.Index == 0)
                            {
                                parser.PopInputToken();
                            }
                            return HandleSyntaxError(parser, reductions, tokens, index);
                        case ParseMessage.LexicalError:
                            return HandleSyntaxError(parser, reductions, tokens, index);
                        case ParseMessage.Accept:
                            return HandleAccept(parser, index);
                        case ParseMessage.TokenRead:
                            if (parser.TokenText == ";")
                                parser.PopInputToken();
                            else
                            {
                                TerminalNode terminal = new TerminalNode(
                                parser.TokenSymbol,
                                parser.TokenText,
                                parser.TokenString,
                                parser.TokenLineNumber,
                                parser.TokenLinePosition,
                                parser.TokenCharPosition);
                                parser.TokenSyntaxNode = terminal;
                                if (parser.TokenSymbol.Index != 0)
                                    tokens.Add(terminal);
                            }
                            break;
                        case ParseMessage.Reduction:
                            NonTerminalNode nonTerminal = new NonTerminalNode(parser.ReductionRule);
						    nonTerminal.ReductionNumber = ++reductionNumber;
						    parser.TokenSyntaxNode = nonTerminal;
                            reductions.Add(nonTerminal);
                            for (int i = 0; i < parser.ReductionCount; i++)
                            {
                                SyntaxNode node = parser.GetReductionSyntaxNode(i) as SyntaxNode;
                                nonTerminal.Add(node);
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                if (DebugMessage != null)
                    DebugMessage(this, ex.Message);
                return new List<string>();
            }

            return ret;
        }

        private List<string> HandleSyntaxError(Parser parser, List<NonTerminalNode> reductions, List<TerminalNode> tokens, int index)
        {
             List<string> completions = new List<string>();

            //cursor is at syntax error
            if ((parser.TokenCharPosition + parser.TokenLength - 1) == index)
            {
                string tokenText = parser.TokenText;
                //if (parser.TokenString == "(EOF)" && tokens.Count > 1)
                //{
                //    tokenText = tokens[tokens.Count - 2].Text;
                //}

                Symbol[] tokenArr = parser.GetExpectedTokens();
                if (tokenArr != null)
                {
                    foreach (Symbol token in tokenArr)
                    {
                        if (token.Index == (int)SymbolConstants.SYMBOL_ID)
                        {
                            //column or table?
                            SymbolConstants symbol = FindPrevSymbol(tokens);
                            if (symbol == SymbolConstants.SYMBOL_FROM || symbol == SymbolConstants.SYMBOL_UPDATE)
                            {
                                //return all tables
                                completions.AddRange(_dbCache.GetTables(""));
                            }
                        }
                        else if (token.Index == (int)SymbolConstants.SYMBOL_SET && FindPrevSymbol(tokens) == SymbolConstants.SYMBOL_UPDATE && tokens.Count == 2)
                        {
                            completions.AddRange(_dbCache.GetTables(tokens[tokens.Count - 1].Text));
                        }
                        else if (token.Name.Length >= tokenText.Length && CompareStringBegining(token.Name, tokenText, false))
                        {
                            if (token.Name == "LIKE")
                                completions.Add("LIKE '%%'");
                            else
                                completions.Add(token.Name);
                        }
                    }
                }
            }
            else
            {

            }

            return completions;
        }

        private SymbolConstants FindPrevSymbol(List<TerminalNode> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Symbol.Index == (int)SymbolConstants.SYMBOL_SELECT)
                    return SymbolConstants.SYMBOL_SELECT;
                else if (tokens[i].Symbol.Index == (int)SymbolConstants.SYMBOL_FROM)
                    return SymbolConstants.SYMBOL_FROM;
                else if (tokens[i].Symbol.Index == (int)SymbolConstants.SYMBOL_WHERE)
                    return SymbolConstants.SYMBOL_WHERE;
                else if (tokens[i].Symbol.Index == (int)SymbolConstants.SYMBOL_UPDATE)
                    return SymbolConstants.SYMBOL_UPDATE;
            }
            return SymbolConstants.SYMBOL_ID;
        }

        private bool CompareStringBegining(string str1, string str2, bool caseSensetive)
        {
            if (str1 == null || str1.Length < 1)
                return true;
            if (str2 == null || str2.Length < 1)
                return true;

            if (str1.Length > str2.Length)
            {
                if (caseSensetive)
                    return str1.Substring(0, str2.Length) == str2;
                else
                    return str1.Substring(0, str2.Length).ToUpper() == str2.ToUpper();
            }
            else
            {
                if (caseSensetive)
                    return str2.Substring(0, str1.Length) == str1;
                else
                    return str2.Substring(0, str1.Length).ToUpper() == str1.ToUpper();
            }
        }

        private List<string> HandleAccept(Parser parser, int index)
        {
            List<string> completions = new List<string>();

            NonTerminalNode node = (NonTerminalNode)parser.TokenSyntaxNode;
            AppendParentInfo(node);
            TerminalNode indexNode = GetIndexNode(node, index);

            if (indexNode != null)
            {
                if (IsNodePartOf(indexNode, RuleConstants.RULE_COLUMNSOURCE_ID) ||
                    IsNodePartOf(indexNode, RuleConstants.RULE_COLUMNS_TIMES) ||
                    IsNodePartOf(indexNode, RuleConstants.RULE_COLUMNLIST_COMMA_TIMES) ||
                    IsNodePartOf(indexNode, RuleConstants.RULE_WHERECLAUSE) ||
                    IsNodePartOf(indexNode, RuleConstants.RULE_WHERECLAUSE_WHERE))
                {
                    List<TableNames> tableNames = GetTables(node);
                    completions.AddRange(GetColumnsCompletions(indexNode.Text, tableNames));
                }
                else if (IsNodePartOf(indexNode, RuleConstants.RULE_IDMEMBER_ID_ID) &&
                    IsNodePartOf(indexNode, RuleConstants.RULE_FROMCLAUSE_FROM) &&
                    CompareStringBegining(indexNode.Text, "WHERE", false))
                {
                    completions.Add("WHERE");
                }
                else if (IsNodePartOf(indexNode, RuleConstants.RULE_FROMCLAUSE_FROM) &&
                    IsNodePartOf(indexNode, RuleConstants.RULE_IDMEMBER_ID))
                {
                    completions.AddRange(_dbCache.GetTables(indexNode.Text));
                }
                else if (IsNodePartOf(indexNode, RuleConstants.RULE_DELETESTM_DELETE_FROM_ID))
                {
                    completions.AddRange(_dbCache.GetTables(indexNode.Text));
                }
            }
            

            return completions;
        }

        private List<string> GetColumnsCompletions(string colName, List<TableNames> tables)
        {
            List<string> completions = new List<string>();
            
            if (tables != null && tables.Count > 0 && DBCache != null)
            {
                string sSearchTable = "";
                string sSearchTableAlias = "";
                string sSearchColumn = colName;
                string sWhereClause = "";
                
                if (colName == "*")
                {
                    sSearchColumn = "";
                }
                else if (colName.IndexOf(".") >= 0)
                {
                    int dotIndex = colName.IndexOf(".");
                    sSearchTable = colName.Substring(0, dotIndex);
                    sSearchColumn = colName.Substring(dotIndex + 1, colName.Length - (dotIndex + 1));

                    for (int i = 0; i < tables.Count; i++)
                    {
                        if (sSearchTable == tables[i].TableAlias)
                        {
                            sSearchTableAlias = sSearchTable;
                            sSearchTable = tables[i].TableName;
                        }
                    }

                    if (sSearchColumn == "x")
                        sSearchColumn = "";
                }

                for (int i = 0; i < tables.Count; i++)
                {
                    if (sSearchTable.Length < 1 || sSearchTable == tables[i].TableName)
                    {
                        if (sWhereClause.Length > 0)
                            sWhereClause += " OR ";
                        sWhereClause += string.Format("table_name = '{0}'", tables[i].TableName);
                    }
                }

                _dbCache.ColumnCache.Clear();
                _dbCache.ColumnCache.FillQuery(sWhereClause);
                DataRow[] rows = _dbCache.ColumnCache.Select();
                for (int r = 0; r < rows.Length; r++)
                {
                    if (CompareStringBegining(sSearchColumn, (string)rows[r]["column_name"], false))
                    {
                        if (sSearchTableAlias.Length > 0)
                            completions.Add(string.Format("{0}.{1}", sSearchTableAlias, (string)rows[r]["column_name"]));
                        else
                            completions.Add((string)rows[r]["column_name"]);
                    }
                }
            }

            return completions;
        }

        private class TableNames
        {
            public string TableName { get; set; }
            public string TableAlias { get; set; }
            public TableNames(string tableName, string tableAlias)
            {
                TableName = tableName;
                TableAlias = tableAlias;
            }
        }

        private List<TableNames> GetTables(SyntaxNode node)
        {
            if (node is NonTerminalNode)
            {
                NonTerminalNode ntNode = (NonTerminalNode)node;
                if (ntNode.Rule.Index == (int)RuleConstants.RULE_FROMCLAUSE_FROM)
                {
                    for (int i = 0; i < ntNode.Count; i++)
                    {
                        if (ntNode[i] is NonTerminalNode)
                        {
                            NonTerminalNode ntIdListNode = (NonTerminalNode)ntNode[i];
                            if (ntIdListNode.Rule.Index == (int)RuleConstants.RULE_IDLIST_COMMA ||
                                ntIdListNode.Rule.Index == (int)RuleConstants.RULE_IDLIST)
                            {
                                List<TableNames> ret = new List<TableNames>();
                                for (int k = 0; k < ntIdListNode.Count; k++)
                                {
                                    if (ntIdListNode[k] is NonTerminalNode)
                                    {
                                        NonTerminalNode ntId = (NonTerminalNode)ntIdListNode[k];
                                        if (ntId.Rule.Index == (int)RuleConstants.RULE_IDMEMBER_ID)
                                        {
                                            ret.Add(new TableNames(((TerminalNode)ntId[0]).Text, ""));
                                        }
                                        else if (ntId.Rule.Index == (int)RuleConstants.RULE_IDMEMBER_ID_ID)
                                        {
                                            ret.Add(new TableNames(((TerminalNode)ntId[0]).Text, ((TerminalNode)ntId[1]).Text));
                                        }
                                    }
                                }
                                return ret;
                            }
                            else if (ntIdListNode.Rule.Index == (int)RuleConstants.RULE_IDMEMBER_ID)
                            {
                                List<TableNames> ret = new List<TableNames>();
                                ret.Add(new TableNames(((TerminalNode)ntIdListNode[0]).Text, ""));
                                return ret;
                            }
                            else if (ntIdListNode.Rule.Index == (int)RuleConstants.RULE_IDMEMBER_ID_ID)
                            {
                                List<TableNames> ret = new List<TableNames>();
                                ret.Add(new TableNames(((TerminalNode)ntIdListNode[0]).Text, ((TerminalNode)ntIdListNode[1]).Text));
                                return ret;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < ntNode.Count; i++)
                    {
                        List<TableNames> ret = GetTables(ntNode[i]);
                        if (ret != null)
                            return ret;
                    }
                }
            }

            return null;
        }

        private bool IsNodePartOf(SyntaxNode node, RuleConstants rule)
        {
            if (node is NonTerminalNode)
            {
                NonTerminalNode ntNode = (NonTerminalNode)node;
                if (ntNode.Rule.Index == (int)rule)
                    return true;
                else if (ntNode.Parent != null)
                    return IsNodePartOf(ntNode.Parent, rule);
                return false;
            }
            else
            {
                TerminalNode tNode = (TerminalNode)node;
                if (tNode.Parent != null)
                    return IsNodePartOf(tNode.Parent, rule);
                return false;
            }
        }

        private TerminalNode GetIndexNode(NonTerminalNode topNode, int index)
        {
            if (topNode == null)
                return null;

            return SearchNode(topNode, index);
        }

        private TerminalNode SearchNode(SyntaxNode node, int index)
        {
            if (node is NonTerminalNode)
            {
                NonTerminalNode ntNode = (NonTerminalNode)node;
                for (int i = 0; i < ntNode.Count; i++)
                {
                    ntNode[i].Parent = ntNode;
                    TerminalNode res = SearchNode(ntNode[i], index);
                    if (res != null)
                        return res;
                }
            }
            else
            {
                TerminalNode tNode = (TerminalNode)node;
                if (tNode.CharIndex <= index && index < (tNode.CharIndex + tNode.Text.Length))
                    return tNode;
            }
            return null;
        }

        private void AppendParentInfo(SyntaxNode node)
        {
            if (node is NonTerminalNode)
            {
                NonTerminalNode ntNode = (NonTerminalNode)node;
                for (int i = 0; i < ntNode.Count; i++)
                {
                    ntNode[i].Parent = ntNode;
                    AppendParentInfo(ntNode[i]);
                }
            }
        }

        private string GetPartialCmdToken(string cmd, int index)
        {
            if (cmd[index] == ' ')
                return "";

            for (int i = index; i > 0; i--)
            {
                if (cmd[i - 1] == ' ')
                    return cmd.Substring(i, index - i);
            }

            return cmd.Substring(0, index);
        }
    }

    /// <summary>
    /// Summary description for SyntaxNode.
    /// </summary>
    public class SyntaxNode
    {
        private NonTerminalNode m_parent = null;
        public NonTerminalNode Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }
    }

    /// <summary>
    /// Summary description for TerminalNode.
    /// </summary>
    public class TerminalNode : SyntaxNode
    {
        private Symbol m_symbol;
        private string m_text;
        private string m_printText;
        private int m_lineNumber;
        private int m_charIndex;
        private int m_linePosition;

        public TerminalNode(Symbol symbol, string text, string printText,
            int lineNumber, int linePosition, int charIndex)
        {
            m_symbol = symbol;
            m_text = text;
            m_printText = printText;
            m_lineNumber = lineNumber;
            m_linePosition = linePosition;
            m_charIndex = charIndex;
        }

        public Symbol Symbol
        {
            get { return m_symbol; }
        }

        public string Text
        {
            get { return m_text; }
        }

        public override string ToString()
        {
            return m_printText;
        }

        public int LineNumber
        {
            get { return m_lineNumber; }
        }

        public int LinePosition
        {
            get { return m_linePosition; }
        }

        public int CharIndex
        {
            get { return m_charIndex; }
        }
    }

    public class NonTerminalNode : SyntaxNode
    {
        private int m_reductionNumber;
        private GoldParser.Rule m_rule;
        private List<SyntaxNode> m_array = new List<SyntaxNode>();

        public NonTerminalNode(GoldParser.Rule rule)
        {
            m_rule = rule;
        }

        public int ReductionNumber
        {
            get { return m_reductionNumber; }
            set { m_reductionNumber = value; }
        }

        public int Count
        {
            get { return m_array.Count; }
        }

        public SyntaxNode this[int index]
        {
            get { return (SyntaxNode)m_array[index]; }
        }

        public void Add(SyntaxNode node)
        {
            if (node == null)
            {
                return; //throw new ArgumentNullException("node");
            }
            m_array.Add(node);
        }

        public GoldParser.Rule Rule
        {
            get { return m_rule; }
        }

    }
}
