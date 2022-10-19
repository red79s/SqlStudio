namespace SqlExecute
{
    enum SymbolConstants : int
    {
        /// <c> (EOF) </c>
        SYMBOL_EOF = 0,
        /// <c> (Error) </c>
        SYMBOL_ERROR = 1,
        /// <c> (Whitespace) </c>
        SYMBOL_WHITESPACE = 2,
        /// <c> (Comment End) </c>
        SYMBOL_COMMENTEND = 3,
        /// <c> (Comment Line) </c>
        SYMBOL_COMMENTLINE = 4,
        /// <c> (Comment Start) </c>
        SYMBOL_COMMENTSTART = 5,
        /// <c> - </c>
        SYMBOL_MINUS = 6,
        /// <c> != </c>
        SYMBOL_EXCLAMEQ = 7,
        /// <c> ( </c>
        SYMBOL_LPARAN = 8,
        /// <c> ) </c>
        SYMBOL_RPARAN = 9,
        /// <c> * </c>
        SYMBOL_TIMES = 10,
        /// <c> , </c>
        SYMBOL_COMMA = 11,
        /// <c> / </c>
        SYMBOL_DIV = 12,
        /// <c> + </c>
        SYMBOL_PLUS = 13,
        /// <c> &lt; </c>
        SYMBOL_LT = 14,
        /// <c> &lt;= </c>
        SYMBOL_LTEQ = 15,
        /// <c> &lt;&gt; </c>
        SYMBOL_LTGT = 16,
        /// <c> = </c>
        SYMBOL_EQ = 17,
        /// <c> &gt; </c>
        SYMBOL_GT = 18,
        /// <c> &gt;= </c>
        SYMBOL_GTEQ = 19,
        /// <c> ADD </c>
        SYMBOL_ADD = 20,
        /// <c> ALL </c>
        SYMBOL_ALL = 21,
        /// <c> ALTER </c>
        SYMBOL_ALTER = 22,
        /// <c> AND </c>
        SYMBOL_AND = 23,
        /// <c> AS </c>
        SYMBOL_AS = 24,
        /// <c> ASC </c>
        SYMBOL_ASC = 25,
        /// <c> Avg </c>
        SYMBOL_AVG = 26,
        /// <c> BETWEEN </c>
        SYMBOL_BETWEEN = 27,
        /// <c> BIT </c>
        SYMBOL_BIT = 28,
        /// <c> BY </c>
        SYMBOL_BY = 29,
        /// <c> CHARACTER </c>
        SYMBOL_CHARACTER = 30,
        /// <c> COLUMN </c>
        SYMBOL_COLUMN = 31,
        /// <c> CONSTRAINT </c>
        SYMBOL_CONSTRAINT = 32,
        /// <c> Count </c>
        SYMBOL_COUNT = 33,
        /// <c> CREATE </c>
        SYMBOL_CREATE = 34,
        /// <c> DATE </c>
        SYMBOL_DATE = 35,
        /// <c> DECIMAL </c>
        SYMBOL_DECIMAL = 36,
        /// <c> DELETE </c>
        SYMBOL_DELETE = 37,
        /// <c> DESC </c>
        SYMBOL_DESC = 38,
        /// <c> DISALLOW </c>
        SYMBOL_DISALLOW = 39,
        /// <c> DISTINCT </c>
        SYMBOL_DISTINCT = 40,
        /// <c> DROP </c>
        SYMBOL_DROP = 41,
        /// <c> FLOAT </c>
        SYMBOL_FLOAT = 42,
        /// <c> FOREIGN </c>
        SYMBOL_FOREIGN = 43,
        /// <c> FROM </c>
        SYMBOL_FROM = 44,
        /// <c> GROUP </c>
        SYMBOL_GROUP = 45,
        /// <c> HAVING </c>
        SYMBOL_HAVING = 46,
        /// <c> Id </c>
        SYMBOL_ID = 47,
        /// <c> IGNORE </c>
        SYMBOL_IGNORE = 48,
        /// <c> IN </c>
        SYMBOL_IN = 49,
        /// <c> INDEX </c>
        SYMBOL_INDEX = 50,
        /// <c> INNER </c>
        SYMBOL_INNER = 51,
        /// <c> INSERT </c>
        SYMBOL_INSERT = 52,
        /// <c> INTEGER </c>
        SYMBOL_INTEGER = 53,
        /// <c> IntegerLiteral </c>
        SYMBOL_INTEGERLITERAL = 54,
        /// <c> INTERVAL </c>
        SYMBOL_INTERVAL = 55,
        /// <c> INTO </c>
        SYMBOL_INTO = 56,
        /// <c> IS </c>
        SYMBOL_IS = 57,
        /// <c> JOIN </c>
        SYMBOL_JOIN = 58,
        /// <c> KEY </c>
        SYMBOL_KEY = 59,
        /// <c> LEFT </c>
        SYMBOL_LEFT = 60,
        /// <c> LIKE </c>
        SYMBOL_LIKE = 61,
        /// <c> Max </c>
        SYMBOL_MAX = 62,
        /// <c> Min </c>
        SYMBOL_MIN = 63,
        /// <c> NOT </c>
        SYMBOL_NOT = 64,
        /// <c> NULL </c>
        SYMBOL_NULL = 65,
        /// <c> ON </c>
        SYMBOL_ON = 66,
        /// <c> OR </c>
        SYMBOL_OR = 67,
        /// <c> ORDER </c>
        SYMBOL_ORDER = 68,
        /// <c> PRIMARY </c>
        SYMBOL_PRIMARY = 69,
        /// <c> REAL </c>
        SYMBOL_REAL = 70,
        /// <c> RealLiteral </c>
        SYMBOL_REALLITERAL = 71,
        /// <c> REFERENCES </c>
        SYMBOL_REFERENCES = 72,
        /// <c> RIGHT </c>
        SYMBOL_RIGHT = 73,
        /// <c> SELECT </c>
        SYMBOL_SELECT = 74,
        /// <c> SET </c>
        SYMBOL_SET = 75,
        /// <c> SMALLINT </c>
        SYMBOL_SMALLINT = 76,
        /// <c> StDev </c>
        SYMBOL_STDEV = 77,
        /// <c> StDevP </c>
        SYMBOL_STDEVP = 78,
        /// <c> StringLiteral </c>
        SYMBOL_STRINGLITERAL = 79,
        /// <c> Sum </c>
        SYMBOL_SUM = 80,
        /// <c> TABLE </c>
        SYMBOL_TABLE = 81,
        /// <c> TIME </c>
        SYMBOL_TIME = 82,
        /// <c> TIMESTAMP </c>
        SYMBOL_TIMESTAMP = 83,
        /// <c> UNIQUE </c>
        SYMBOL_UNIQUE = 84,
        /// <c> UPDATE </c>
        SYMBOL_UPDATE = 85,
        /// <c> VALUES </c>
        SYMBOL_VALUES = 86,
        /// <c> Var </c>
        SYMBOL_VAR = 87,
        /// <c> VarP </c>
        SYMBOL_VARP = 88,
        /// <c> WHERE </c>
        SYMBOL_WHERE = 89,
        /// <c> WITH </c>
        SYMBOL_WITH = 90,
        /// <c> &lt;Add Exp&gt; </c>
        SYMBOL_ADDEXP = 91,
        /// <c> &lt;Aggregate&gt; </c>
        SYMBOL_AGGREGATE = 92,
        /// <c> &lt;Alter Stm&gt; </c>
        SYMBOL_ALTERSTM = 93,
        /// <c> &lt;And Exp&gt; </c>
        SYMBOL_ANDEXP = 94,
        /// <c> &lt;Assign List&gt; </c>
        SYMBOL_ASSIGNLIST = 95,
        /// <c> &lt;Column Item&gt; </c>
        SYMBOL_COLUMNITEM = 96,
        /// <c> &lt;Column List&gt; </c>
        SYMBOL_COLUMNLIST = 97,
        /// <c> &lt;Column Source&gt; </c>
        SYMBOL_COLUMNSOURCE = 98,
        /// <c> &lt;Columns&gt; </c>
        SYMBOL_COLUMNS = 99,
        /// <c> &lt;Constraint&gt; </c>
        SYMBOL_CONSTRAINT2 = 100,
        /// <c> &lt;Constraint Opt&gt; </c>
        SYMBOL_CONSTRAINTOPT = 101,
        /// <c> &lt;Constraint Type&gt; </c>
        SYMBOL_CONSTRAINTTYPE = 102,
        /// <c> &lt;Create Stm&gt; </c>
        SYMBOL_CREATESTM = 103,
        /// <c> &lt;Delete Stm&gt; </c>
        SYMBOL_DELETESTM = 104,
        /// <c> &lt;Drop Stm&gt; </c>
        SYMBOL_DROPSTM = 105,
        /// <c> &lt;Expr List&gt; </c>
        SYMBOL_EXPRLIST = 106,
        /// <c> &lt;Expression&gt; </c>
        SYMBOL_EXPRESSION = 107,
        /// <c> &lt;Field Constraint&gt; </c>
        SYMBOL_FIELDCONSTRAINT = 108,
        /// <c> &lt;Field Def&gt; </c>
        SYMBOL_FIELDDEF = 109,
        /// <c> &lt;Field Def List&gt; </c>
        SYMBOL_FIELDDEFLIST = 110,
        /// <c> &lt;From Clause&gt; </c>
        SYMBOL_FROMCLAUSE = 111,
        /// <c> &lt;Group Clause&gt; </c>
        SYMBOL_GROUPCLAUSE = 112,
        /// <c> &lt;Having Clause&gt; </c>
        SYMBOL_HAVINGCLAUSE = 113,
        /// <c> &lt;Id List&gt; </c>
        SYMBOL_IDLIST = 114,
        /// <c> &lt;Id Member&gt; </c>
        SYMBOL_IDMEMBER = 115,
        /// <c> &lt;Insert Stm&gt; </c>
        SYMBOL_INSERTSTM = 116,
        /// <c> &lt;Into Clause&gt; </c>
        SYMBOL_INTOCLAUSE = 117,
        /// <c> &lt;Join&gt; </c>
        SYMBOL_JOIN2 = 118,
        /// <c> &lt;Join Chain&gt; </c>
        SYMBOL_JOINCHAIN = 119,
        /// <c> &lt;Mult Exp&gt; </c>
        SYMBOL_MULTEXP = 120,
        /// <c> &lt;Negate Exp&gt; </c>
        SYMBOL_NEGATEEXP = 121,
        /// <c> &lt;Not Exp&gt; </c>
        SYMBOL_NOTEXP = 122,
        /// <c> &lt;Order Clause&gt; </c>
        SYMBOL_ORDERCLAUSE = 123,
        /// <c> &lt;Order List&gt; </c>
        SYMBOL_ORDERLIST = 124,
        /// <c> &lt;Order Type&gt; </c>
        SYMBOL_ORDERTYPE = 125,
        /// <c> &lt;Pred Exp&gt; </c>
        SYMBOL_PREDEXP = 126,
        /// <c> &lt;Query&gt; </c>
        SYMBOL_QUERY = 127,
        /// <c> &lt;Restriction&gt; </c>
        SYMBOL_RESTRICTION = 128,
        /// <c> &lt;Select Stm&gt; </c>
        SYMBOL_SELECTSTM = 129,
        /// <c> &lt;Tuple&gt; </c>
        SYMBOL_TUPLE = 130,
        /// <c> &lt;Type&gt; </c>
        SYMBOL_TYPE = 131,
        /// <c> &lt;Unique&gt; </c>
        SYMBOL_UNIQUE2 = 132,
        /// <c> &lt;Update Stm&gt; </c>
        SYMBOL_UPDATESTM = 133,
        /// <c> &lt;Value&gt; </c>
        SYMBOL_VALUE = 134,
        /// <c> &lt;Where Clause&gt; </c>
        SYMBOL_WHERECLAUSE = 135,
        /// <c> &lt;With Clause&gt; </c>
        SYMBOL_WITHCLAUSE = 136
    };

    enum RuleConstants : int
    {
        /// <c> &lt;Query&gt; ::= &lt;Alter Stm&gt; </c>
        RULE_QUERY = 0,
        /// <c> &lt;Query&gt; ::= &lt;Create Stm&gt; </c>
        RULE_QUERY2 = 1,
        /// <c> &lt;Query&gt; ::= &lt;Delete Stm&gt; </c>
        RULE_QUERY3 = 2,
        /// <c> &lt;Query&gt; ::= &lt;Drop Stm&gt; </c>
        RULE_QUERY4 = 3,
        /// <c> &lt;Query&gt; ::= &lt;Insert Stm&gt; </c>
        RULE_QUERY5 = 4,
        /// <c> &lt;Query&gt; ::= &lt;Select Stm&gt; </c>
        RULE_QUERY6 = 5,
        /// <c> &lt;Query&gt; ::= &lt;Update Stm&gt; </c>
        RULE_QUERY7 = 6,
        /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id ADD COLUMN &lt;Field Def List&gt; &lt;Constraint Opt&gt; </c>
        RULE_ALTERSTM_ALTER_TABLE_ID_ADD_COLUMN = 7,
        /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id ADD &lt;Constraint&gt; </c>
        RULE_ALTERSTM_ALTER_TABLE_ID_ADD = 8,
        /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id DROP COLUMN Id </c>
        RULE_ALTERSTM_ALTER_TABLE_ID_DROP_COLUMN_ID = 9,
        /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id DROP CONSTRAINT Id </c>
        RULE_ALTERSTM_ALTER_TABLE_ID_DROP_CONSTRAINT_ID = 10,
        /// <c> &lt;Create Stm&gt; ::= CREATE &lt;Unique&gt; INDEX IntegerLiteral ON Id ( &lt;Order List&gt; ) &lt;With Clause&gt; </c>
        RULE_CREATESTM_CREATE_INDEX_INTEGERLITERAL_ON_ID_LPARAN_RPARAN = 11,
        /// <c> &lt;Create Stm&gt; ::= CREATE TABLE Id ( &lt;Field Def List&gt; ) &lt;Constraint Opt&gt; </c>
        RULE_CREATESTM_CREATE_TABLE_ID_LPARAN_RPARAN = 12,
        /// <c> &lt;Unique&gt; ::= UNIQUE </c>
        RULE_UNIQUE_UNIQUE = 13,
        /// <c> &lt;Unique&gt; ::=  </c>
        RULE_UNIQUE = 14,
        /// <c> &lt;With Clause&gt; ::= WITH PRIMARY </c>
        RULE_WITHCLAUSE_WITH_PRIMARY = 15,
        /// <c> &lt;With Clause&gt; ::= WITH DISALLOW NULL </c>
        RULE_WITHCLAUSE_WITH_DISALLOW_NULL = 16,
        /// <c> &lt;With Clause&gt; ::= WITH IGNORE NULL </c>
        RULE_WITHCLAUSE_WITH_IGNORE_NULL = 17,
        /// <c> &lt;With Clause&gt; ::=  </c>
        RULE_WITHCLAUSE = 18,
        /// <c> &lt;Field Def&gt; ::= Id &lt;Type&gt; &lt;Field Constraint&gt; </c>
        RULE_FIELDDEF_ID = 19,
        /// <c> &lt;Field Def&gt; ::= Id &lt;Type&gt; </c>
        RULE_FIELDDEF_ID2 = 20,
        /// <c> &lt;Field Def List&gt; ::= &lt;Field Def&gt; , &lt;Field Def List&gt; </c>
        RULE_FIELDDEFLIST_COMMA = 21,
        /// <c> &lt;Field Def List&gt; ::= &lt;Field Def&gt; </c>
        RULE_FIELDDEFLIST = 22,
        /// <c> &lt;Type&gt; ::= BIT </c>
        RULE_TYPE_BIT = 23,
        /// <c> &lt;Type&gt; ::= DATE </c>
        RULE_TYPE_DATE = 24,
        /// <c> &lt;Type&gt; ::= TIME </c>
        RULE_TYPE_TIME = 25,
        /// <c> &lt;Type&gt; ::= TIMESTAMP </c>
        RULE_TYPE_TIMESTAMP = 26,
        /// <c> &lt;Type&gt; ::= DECIMAL </c>
        RULE_TYPE_DECIMAL = 27,
        /// <c> &lt;Type&gt; ::= REAL </c>
        RULE_TYPE_REAL = 28,
        /// <c> &lt;Type&gt; ::= FLOAT </c>
        RULE_TYPE_FLOAT = 29,
        /// <c> &lt;Type&gt; ::= SMALLINT </c>
        RULE_TYPE_SMALLINT = 30,
        /// <c> &lt;Type&gt; ::= INTEGER </c>
        RULE_TYPE_INTEGER = 31,
        /// <c> &lt;Type&gt; ::= INTERVAL </c>
        RULE_TYPE_INTERVAL = 32,
        /// <c> &lt;Type&gt; ::= CHARACTER </c>
        RULE_TYPE_CHARACTER = 33,
        /// <c> &lt;Field Constraint&gt; ::= PRIMARY KEY </c>
        RULE_FIELDCONSTRAINT_PRIMARY_KEY = 34,
        /// <c> &lt;Field Constraint&gt; ::= NOT NULL </c>
        RULE_FIELDCONSTRAINT_NOT_NULL = 35,
        /// <c> &lt;Constraint Opt&gt; ::= &lt;Constraint&gt; </c>
        RULE_CONSTRAINTOPT = 36,
        /// <c> &lt;Constraint Opt&gt; ::=  </c>
        RULE_CONSTRAINTOPT2 = 37,
        /// <c> &lt;Constraint&gt; ::= CONSTRAINT Id &lt;Constraint Type&gt; </c>
        RULE_CONSTRAINT_CONSTRAINT_ID = 38,
        /// <c> &lt;Constraint&gt; ::= CONSTRAINT Id </c>
        RULE_CONSTRAINT_CONSTRAINT_ID2 = 39,
        /// <c> &lt;Constraint Type&gt; ::= PRIMARY KEY ( &lt;Id List&gt; ) </c>
        RULE_CONSTRAINTTYPE_PRIMARY_KEY_LPARAN_RPARAN = 40,
        /// <c> &lt;Constraint Type&gt; ::= UNIQUE ( &lt;Id List&gt; ) </c>
        RULE_CONSTRAINTTYPE_UNIQUE_LPARAN_RPARAN = 41,
        /// <c> &lt;Constraint Type&gt; ::= NOT NULL ( &lt;Id List&gt; ) </c>
        RULE_CONSTRAINTTYPE_NOT_NULL_LPARAN_RPARAN = 42,
        /// <c> &lt;Constraint Type&gt; ::= FOREIGN KEY ( &lt;Id List&gt; ) REFERENCES Id ( &lt;Id List&gt; ) </c>
        RULE_CONSTRAINTTYPE_FOREIGN_KEY_LPARAN_RPARAN_REFERENCES_ID_LPARAN_RPARAN = 43,
        /// <c> &lt;Drop Stm&gt; ::= DROP TABLE Id </c>
        RULE_DROPSTM_DROP_TABLE_ID = 44,
        /// <c> &lt;Drop Stm&gt; ::= DROP INDEX Id ON Id </c>
        RULE_DROPSTM_DROP_INDEX_ID_ON_ID = 45,
        /// <c> &lt;Insert Stm&gt; ::= INSERT INTO Id ( &lt;Id List&gt; ) &lt;Select Stm&gt; </c>
        RULE_INSERTSTM_INSERT_INTO_ID_LPARAN_RPARAN = 46,
        /// <c> &lt;Insert Stm&gt; ::= INSERT INTO Id ( &lt;Id List&gt; ) VALUES ( &lt;Expr List&gt; ) </c>
        RULE_INSERTSTM_INSERT_INTO_ID_LPARAN_RPARAN_VALUES_LPARAN_RPARAN = 47,
        /// <c> &lt;Update Stm&gt; ::= UPDATE Id SET &lt;Assign List&gt; &lt;Where Clause&gt; </c>
        RULE_UPDATESTM_UPDATE_ID_SET = 48,
        /// <c> &lt;Assign List&gt; ::= Id = &lt;Expression&gt; , &lt;Assign List&gt; </c>
        RULE_ASSIGNLIST_ID_EQ_COMMA = 49,
        /// <c> &lt;Assign List&gt; ::= Id = &lt;Expression&gt; </c>
        RULE_ASSIGNLIST_ID_EQ = 50,
        /// <c> &lt;Delete Stm&gt; ::= DELETE FROM Id &lt;Where Clause&gt; </c>
        RULE_DELETESTM_DELETE_FROM_ID = 51,
        /// <c> &lt;Select Stm&gt; ::= SELECT &lt;Columns&gt; &lt;Into Clause&gt; &lt;From Clause&gt; &lt;Where Clause&gt; &lt;Group Clause&gt; &lt;Having Clause&gt; &lt;Order Clause&gt; </c>
        RULE_SELECTSTM_SELECT = 52,
        /// <c> &lt;Columns&gt; ::= &lt;Restriction&gt; * </c>
        RULE_COLUMNS_TIMES = 53,
        /// <c> &lt;Columns&gt; ::= &lt;Restriction&gt; &lt;Column List&gt; </c>
        RULE_COLUMNS = 54,
        /// <c> &lt;Column List&gt; ::= &lt;Column Item&gt; , &lt;Column List&gt; </c>
        RULE_COLUMNLIST_COMMA = 55,
        /// <c> &lt;Column List&gt; ::= &lt;Column Item&gt; , * </c>
        RULE_COLUMNLIST_COMMA_TIMES = 56,
        /// <c> &lt;Column List&gt; ::= &lt;Column Item&gt; </c>
        RULE_COLUMNLIST = 57,
        /// <c> &lt;Column Item&gt; ::= &lt;Column Source&gt; </c>
        RULE_COLUMNITEM = 58,
        /// <c> &lt;Column Item&gt; ::= &lt;Column Source&gt; Id </c>
        RULE_COLUMNITEM_ID = 59,
        /// <c> &lt;Column Item&gt; ::= &lt;Column Source&gt; AS StringLiteral </c>
        RULE_COLUMNITEM_AS_STRINGLITERAL = 60,
        /// <c> &lt;Column Source&gt; ::= &lt;Aggregate&gt; </c>
        RULE_COLUMNSOURCE = 61,
        /// <c> &lt;Column Source&gt; ::= Id </c>
        RULE_COLUMNSOURCE_ID = 62,
        /// <c> &lt;Restriction&gt; ::= ALL </c>
        RULE_RESTRICTION_ALL = 63,
        /// <c> &lt;Restriction&gt; ::= DISTINCT </c>
        RULE_RESTRICTION_DISTINCT = 64,
        /// <c> &lt;Restriction&gt; ::=  </c>
        RULE_RESTRICTION = 65,
        /// <c> &lt;Aggregate&gt; ::= Count ( * ) </c>
        RULE_AGGREGATE_COUNT_LPARAN_TIMES_RPARAN = 66,
        /// <c> &lt;Aggregate&gt; ::= Count ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_COUNT_LPARAN_RPARAN = 67,
        /// <c> &lt;Aggregate&gt; ::= Avg ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_AVG_LPARAN_RPARAN = 68,
        /// <c> &lt;Aggregate&gt; ::= Min ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_MIN_LPARAN_RPARAN = 69,
        /// <c> &lt;Aggregate&gt; ::= Max ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_MAX_LPARAN_RPARAN = 70,
        /// <c> &lt;Aggregate&gt; ::= StDev ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_STDEV_LPARAN_RPARAN = 71,
        /// <c> &lt;Aggregate&gt; ::= StDevP ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_STDEVP_LPARAN_RPARAN = 72,
        /// <c> &lt;Aggregate&gt; ::= Sum ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_SUM_LPARAN_RPARAN = 73,
        /// <c> &lt;Aggregate&gt; ::= Var ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_VAR_LPARAN_RPARAN = 74,
        /// <c> &lt;Aggregate&gt; ::= VarP ( &lt;Expression&gt; ) </c>
        RULE_AGGREGATE_VARP_LPARAN_RPARAN = 75,
        /// <c> &lt;Into Clause&gt; ::= INTO Id </c>
        RULE_INTOCLAUSE_INTO_ID = 76,
        /// <c> &lt;Into Clause&gt; ::=  </c>
        RULE_INTOCLAUSE = 77,
        /// <c> &lt;From Clause&gt; ::= FROM &lt;Id List&gt; &lt;Join Chain&gt; </c>
        RULE_FROMCLAUSE_FROM = 78,
        /// <c> &lt;Join Chain&gt; ::= &lt;Join&gt; &lt;Join Chain&gt; </c>
        RULE_JOINCHAIN = 79,
        /// <c> &lt;Join Chain&gt; ::=  </c>
        RULE_JOINCHAIN2 = 80,
        /// <c> &lt;Join&gt; ::= INNER JOIN &lt;Id List&gt; ON Id = Id </c>
        RULE_JOIN_INNER_JOIN_ON_ID_EQ_ID = 81,
        /// <c> &lt;Join&gt; ::= LEFT JOIN &lt;Id List&gt; ON Id = Id </c>
        RULE_JOIN_LEFT_JOIN_ON_ID_EQ_ID = 82,
        /// <c> &lt;Join&gt; ::= RIGHT JOIN &lt;Id List&gt; ON Id = Id </c>
        RULE_JOIN_RIGHT_JOIN_ON_ID_EQ_ID = 83,
        /// <c> &lt;Join&gt; ::= JOIN &lt;Id List&gt; ON Id = Id </c>
        RULE_JOIN_JOIN_ON_ID_EQ_ID = 84,
        /// <c> &lt;Where Clause&gt; ::= WHERE &lt;Expression&gt; </c>
        RULE_WHERECLAUSE_WHERE = 85,
        /// <c> &lt;Where Clause&gt; ::=  </c>
        RULE_WHERECLAUSE = 86,
        /// <c> &lt;Group Clause&gt; ::= GROUP BY &lt;Id List&gt; </c>
        RULE_GROUPCLAUSE_GROUP_BY = 87,
        /// <c> &lt;Group Clause&gt; ::=  </c>
        RULE_GROUPCLAUSE = 88,
        /// <c> &lt;Order Clause&gt; ::= ORDER BY &lt;Order List&gt; </c>
        RULE_ORDERCLAUSE_ORDER_BY = 89,
        /// <c> &lt;Order Clause&gt; ::=  </c>
        RULE_ORDERCLAUSE = 90,
        /// <c> &lt;Order List&gt; ::= Id &lt;Order Type&gt; , &lt;Order List&gt; </c>
        RULE_ORDERLIST_ID_COMMA = 91,
        /// <c> &lt;Order List&gt; ::= Id &lt;Order Type&gt; </c>
        RULE_ORDERLIST_ID = 92,
        /// <c> &lt;Order Type&gt; ::= ASC </c>
        RULE_ORDERTYPE_ASC = 93,
        /// <c> &lt;Order Type&gt; ::= DESC </c>
        RULE_ORDERTYPE_DESC = 94,
        /// <c> &lt;Order Type&gt; ::=  </c>
        RULE_ORDERTYPE = 95,
        /// <c> &lt;Having Clause&gt; ::= HAVING &lt;Expression&gt; </c>
        RULE_HAVINGCLAUSE_HAVING = 96,
        /// <c> &lt;Having Clause&gt; ::=  </c>
        RULE_HAVINGCLAUSE = 97,
        /// <c> &lt;Expression&gt; ::= &lt;And Exp&gt; OR &lt;Expression&gt; </c>
        RULE_EXPRESSION_OR = 98,
        /// <c> &lt;Expression&gt; ::= &lt;And Exp&gt; </c>
        RULE_EXPRESSION = 99,
        /// <c> &lt;And Exp&gt; ::= &lt;Not Exp&gt; AND &lt;And Exp&gt; </c>
        RULE_ANDEXP_AND = 100,
        /// <c> &lt;And Exp&gt; ::= &lt;Not Exp&gt; </c>
        RULE_ANDEXP = 101,
        /// <c> &lt;Not Exp&gt; ::= NOT &lt;Pred Exp&gt; </c>
        RULE_NOTEXP_NOT = 102,
        /// <c> &lt;Not Exp&gt; ::= &lt;Pred Exp&gt; </c>
        RULE_NOTEXP = 103,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; BETWEEN &lt;Add Exp&gt; AND &lt;Add Exp&gt; </c>
        RULE_PREDEXP_BETWEEN_AND = 104,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; NOT BETWEEN &lt;Add Exp&gt; AND &lt;Add Exp&gt; </c>
        RULE_PREDEXP_NOT_BETWEEN_AND = 105,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Value&gt; IS NOT NULL </c>
        RULE_PREDEXP_IS_NOT_NULL = 106,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Value&gt; IS NULL </c>
        RULE_PREDEXP_IS_NULL = 107,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; LIKE StringLiteral </c>
        RULE_PREDEXP_LIKE_STRINGLITERAL = 108,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; IN &lt;Tuple&gt; </c>
        RULE_PREDEXP_IN = 109,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; = &lt;Add Exp&gt; </c>
        RULE_PREDEXP_EQ = 110,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt;&gt; &lt;Add Exp&gt; </c>
        RULE_PREDEXP_LTGT = 111,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; != &lt;Add Exp&gt; </c>
        RULE_PREDEXP_EXCLAMEQ = 112,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &gt; &lt;Add Exp&gt; </c>
        RULE_PREDEXP_GT = 113,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &gt;= &lt;Add Exp&gt; </c>
        RULE_PREDEXP_GTEQ = 114,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt; &lt;Add Exp&gt; </c>
        RULE_PREDEXP_LT = 115,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt;= &lt;Add Exp&gt; </c>
        RULE_PREDEXP_LTEQ = 116,
        /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; </c>
        RULE_PREDEXP = 117,
        /// <c> &lt;Add Exp&gt; ::= &lt;Add Exp&gt; + &lt;Mult Exp&gt; </c>
        RULE_ADDEXP_PLUS = 118,
        /// <c> &lt;Add Exp&gt; ::= &lt;Add Exp&gt; - &lt;Mult Exp&gt; </c>
        RULE_ADDEXP_MINUS = 119,
        /// <c> &lt;Add Exp&gt; ::= &lt;Mult Exp&gt; </c>
        RULE_ADDEXP = 120,
        /// <c> &lt;Mult Exp&gt; ::= &lt;Mult Exp&gt; * &lt;Negate Exp&gt; </c>
        RULE_MULTEXP_TIMES = 121,
        /// <c> &lt;Mult Exp&gt; ::= &lt;Mult Exp&gt; / &lt;Negate Exp&gt; </c>
        RULE_MULTEXP_DIV = 122,
        /// <c> &lt;Mult Exp&gt; ::= &lt;Negate Exp&gt; </c>
        RULE_MULTEXP = 123,
        /// <c> &lt;Negate Exp&gt; ::= - &lt;Value&gt; </c>
        RULE_NEGATEEXP_MINUS = 124,
        /// <c> &lt;Negate Exp&gt; ::= &lt;Value&gt; </c>
        RULE_NEGATEEXP = 125,
        /// <c> &lt;Value&gt; ::= &lt;Tuple&gt; </c>
        RULE_VALUE = 126,
        /// <c> &lt;Value&gt; ::= Id </c>
        RULE_VALUE_ID = 127,
        /// <c> &lt;Value&gt; ::= IntegerLiteral </c>
        RULE_VALUE_INTEGERLITERAL = 128,
        /// <c> &lt;Value&gt; ::= RealLiteral </c>
        RULE_VALUE_REALLITERAL = 129,
        /// <c> &lt;Value&gt; ::= StringLiteral </c>
        RULE_VALUE_STRINGLITERAL = 130,
        /// <c> &lt;Value&gt; ::= NULL </c>
        RULE_VALUE_NULL = 131,
        /// <c> &lt;Tuple&gt; ::= ( &lt;Select Stm&gt; ) </c>
        RULE_TUPLE_LPARAN_RPARAN = 132,
        /// <c> &lt;Tuple&gt; ::= ( &lt;Expr List&gt; ) </c>
        RULE_TUPLE_LPARAN_RPARAN2 = 133,
        /// <c> &lt;Expr List&gt; ::= &lt;Expression&gt; , &lt;Expr List&gt; </c>
        RULE_EXPRLIST_COMMA = 134,
        /// <c> &lt;Expr List&gt; ::= &lt;Expression&gt; </c>
        RULE_EXPRLIST = 135,
        /// <c> &lt;Id List&gt; ::= &lt;Id Member&gt; , &lt;Id List&gt; </c>
        RULE_IDLIST_COMMA = 136,
        /// <c> &lt;Id List&gt; ::= &lt;Id Member&gt; </c>
        RULE_IDLIST = 137,
        /// <c> &lt;Id Member&gt; ::= Id </c>
        RULE_IDMEMBER_ID = 138,
        /// <c> &lt;Id Member&gt; ::= Id Id </c>
        RULE_IDMEMBER_ID_ID = 139
    };
}