enum SymbolConstants : int
{
   /// <c> (EOF) </c>
   SYMBOL_EOF            = 0,     
   /// <c> (Error) </c>
   SYMBOL_ERROR          = 1,     
   /// <c> (Whitespace) </c>
   SYMBOL_WHITESPACE     = 2,     
   /// <c> (Comment End) </c>
   SYMBOL_COMMENTEND     = 3,     
   /// <c> (Comment Line) </c>
   SYMBOL_COMMENTLINE    = 4,     
   /// <c> (Comment Start) </c>
   SYMBOL_COMMENTSTART   = 5,     
   /// <c> - </c>
   SYMBOL_MINUS          = 6,     
   /// <c> != </c>
   SYMBOL_EXCLAMEQ       = 7,     
   /// <c> ( </c>
   SYMBOL_LPARAN         = 8,     
   /// <c> ) </c>
   SYMBOL_RPARAN         = 9,     
   /// <c> * </c>
   SYMBOL_TIMES          = 10,     
   /// <c> , </c>
   SYMBOL_COMMA          = 11,     
   /// <c> / </c>
   SYMBOL_DIV            = 12,     
   /// <c> + </c>
   SYMBOL_PLUS           = 13,     
   /// <c> &lt; </c>
   SYMBOL_LT             = 14,     
   /// <c> &lt;= </c>
   SYMBOL_LTEQ           = 15,     
   /// <c> &lt;&gt; </c>
   SYMBOL_LTGT           = 16,     
   /// <c> = </c>
   SYMBOL_EQ             = 17,     
   /// <c> &gt; </c>
   SYMBOL_GT             = 18,     
   /// <c> &gt;= </c>
   SYMBOL_GTEQ           = 19,     
   /// <c> ADD </c>
   SYMBOL_ADD            = 20,     
   /// <c> ALL </c>
   SYMBOL_ALL            = 21,     
   /// <c> ALTER </c>
   SYMBOL_ALTER          = 22,     
   /// <c> AND </c>
   SYMBOL_AND            = 23,     
   /// <c> ASC </c>
   SYMBOL_ASC            = 24,     
   /// <c> Avg </c>
   SYMBOL_AVG            = 25,     
   /// <c> BETWEEN </c>
   SYMBOL_BETWEEN        = 26,     
   /// <c> BIT </c>
   SYMBOL_BIT            = 27,     
   /// <c> BY </c>
   SYMBOL_BY             = 28,     
   /// <c> CHARACTER </c>
   SYMBOL_CHARACTER      = 29,     
   /// <c> COLUMN </c>
   SYMBOL_COLUMN         = 30,     
   /// <c> CONSTRAINT </c>
   SYMBOL_CONSTRAINT     = 31,     
   /// <c> Count </c>
   SYMBOL_COUNT          = 32,     
   /// <c> CREATE </c>
   SYMBOL_CREATE         = 33,     
   /// <c> DATE </c>
   SYMBOL_DATE           = 34,     
   /// <c> DECIMAL </c>
   SYMBOL_DECIMAL        = 35,     
   /// <c> DELETE </c>
   SYMBOL_DELETE         = 36,     
   /// <c> DESC </c>
   SYMBOL_DESC           = 37,     
   /// <c> DISALLOW </c>
   SYMBOL_DISALLOW       = 38,     
   /// <c> DISTINCT </c>
   SYMBOL_DISTINCT       = 39,     
   /// <c> DROP </c>
   SYMBOL_DROP           = 40,     
   /// <c> FLOAT </c>
   SYMBOL_FLOAT          = 41,     
   /// <c> FOREIGN </c>
   SYMBOL_FOREIGN        = 42,     
   /// <c> FROM </c>
   SYMBOL_FROM           = 43,     
   /// <c> GROUP </c>
   SYMBOL_GROUP          = 44,     
   /// <c> HAVING </c>
   SYMBOL_HAVING         = 45,     
   /// <c> Id </c>
   SYMBOL_ID             = 46,     
   /// <c> IGNORE </c>
   SYMBOL_IGNORE         = 47,     
   /// <c> IN </c>
   SYMBOL_IN             = 48,     
   /// <c> INDEX </c>
   SYMBOL_INDEX          = 49,     
   /// <c> INNER </c>
   SYMBOL_INNER          = 50,     
   /// <c> INSERT </c>
   SYMBOL_INSERT         = 51,     
   /// <c> INTEGER </c>
   SYMBOL_INTEGER        = 52,     
   /// <c> IntegerLiteral </c>
   SYMBOL_INTEGERLITERAL = 53,     
   /// <c> INTERVAL </c>
   SYMBOL_INTERVAL       = 54,     
   /// <c> INTO </c>
   SYMBOL_INTO           = 55,     
   /// <c> IS </c>
   SYMBOL_IS             = 56,     
   /// <c> JOIN </c>
   SYMBOL_JOIN           = 57,     
   /// <c> KEY </c>
   SYMBOL_KEY            = 58,     
   /// <c> LEFT </c>
   SYMBOL_LEFT           = 59,     
   /// <c> LIKE </c>
   SYMBOL_LIKE           = 60,     
   /// <c> Max </c>
   SYMBOL_MAX            = 61,     
   /// <c> Min </c>
   SYMBOL_MIN            = 62,     
   /// <c> NOT </c>
   SYMBOL_NOT            = 63,     
   /// <c> NULL </c>
   SYMBOL_NULL           = 64,     
   /// <c> ON </c>
   SYMBOL_ON             = 65,     
   /// <c> OR </c>
   SYMBOL_OR             = 66,     
   /// <c> ORDER </c>
   SYMBOL_ORDER          = 67,     
   /// <c> PRIMARY </c>
   SYMBOL_PRIMARY        = 68,     
   /// <c> REAL </c>
   SYMBOL_REAL           = 69,     
   /// <c> RealLiteral </c>
   SYMBOL_REALLITERAL    = 70,     
   /// <c> REFERENCES </c>
   SYMBOL_REFERENCES     = 71,     
   /// <c> RIGHT </c>
   SYMBOL_RIGHT          = 72,     
   /// <c> SELECT </c>
   SYMBOL_SELECT         = 73,     
   /// <c> SET </c>
   SYMBOL_SET            = 74,     
   /// <c> SMALLINT </c>
   SYMBOL_SMALLINT       = 75,     
   /// <c> StDev </c>
   SYMBOL_STDEV          = 76,     
   /// <c> StDevP </c>
   SYMBOL_STDEVP         = 77,     
   /// <c> StringLiteral </c>
   SYMBOL_STRINGLITERAL  = 78,     
   /// <c> Sum </c>
   SYMBOL_SUM            = 79,     
   /// <c> TABLE </c>
   SYMBOL_TABLE          = 80,     
   /// <c> TIME </c>
   SYMBOL_TIME           = 81,     
   /// <c> TIMESTAMP </c>
   SYMBOL_TIMESTAMP      = 82,     
   /// <c> UNIQUE </c>
   SYMBOL_UNIQUE         = 83,     
   /// <c> UPDATE </c>
   SYMBOL_UPDATE         = 84,     
   /// <c> VALUES </c>
   SYMBOL_VALUES         = 85,     
   /// <c> Var </c>
   SYMBOL_VAR            = 86,     
   /// <c> VarP </c>
   SYMBOL_VARP           = 87,     
   /// <c> WHERE </c>
   SYMBOL_WHERE          = 88,     
   /// <c> WITH </c>
   SYMBOL_WITH           = 89,     
   /// <c> &lt;Add Exp&gt; </c>
   SYMBOL_ADDEXP         = 90,     
   /// <c> &lt;Aggregate&gt; </c>
   SYMBOL_AGGREGATE      = 91,     
   /// <c> &lt;Alter Stm&gt; </c>
   SYMBOL_ALTERSTM       = 92,     
   /// <c> &lt;And Exp&gt; </c>
   SYMBOL_ANDEXP         = 93,     
   /// <c> &lt;Assign List&gt; </c>
   SYMBOL_ASSIGNLIST     = 94,     
   /// <c> &lt;Column Item&gt; </c>
   SYMBOL_COLUMNITEM     = 95,     
   /// <c> &lt;Column List&gt; </c>
   SYMBOL_COLUMNLIST     = 96,     
   /// <c> &lt;Column Source&gt; </c>
   SYMBOL_COLUMNSOURCE   = 97,     
   /// <c> &lt;Columns&gt; </c>
   SYMBOL_COLUMNS        = 98,     
   /// <c> &lt;Constraint&gt; </c>
   SYMBOL_CONSTRAINT2    = 99,     
   /// <c> &lt;Constraint Opt&gt; </c>
   SYMBOL_CONSTRAINTOPT  = 100,     
   /// <c> &lt;Constraint Type&gt; </c>
   SYMBOL_CONSTRAINTTYPE = 101,     
   /// <c> &lt;Create Stm&gt; </c>
   SYMBOL_CREATESTM      = 102,     
   /// <c> &lt;Delete Stm&gt; </c>
   SYMBOL_DELETESTM      = 103,     
   /// <c> &lt;Drop Stm&gt; </c>
   SYMBOL_DROPSTM        = 104,     
   /// <c> &lt;Expr List&gt; </c>
   SYMBOL_EXPRLIST       = 105,     
   /// <c> &lt;Expression&gt; </c>
   SYMBOL_EXPRESSION     = 106,     
   /// <c> &lt;Field Def&gt; </c>
   SYMBOL_FIELDDEF       = 107,     
   /// <c> &lt;Field Def List&gt; </c>
   SYMBOL_FIELDDEFLIST   = 108,     
   /// <c> &lt;From Clause&gt; </c>
   SYMBOL_FROMCLAUSE     = 109,     
   /// <c> &lt;Group Clause&gt; </c>
   SYMBOL_GROUPCLAUSE    = 110,     
   /// <c> &lt;Having Clause&gt; </c>
   SYMBOL_HAVINGCLAUSE   = 111,     
   /// <c> &lt;ID List&gt; </c>
   SYMBOL_IDLIST         = 112,     
   /// <c> &lt;Id Member&gt; </c>
   SYMBOL_IDMEMBER       = 113,     
   /// <c> &lt;Insert Stm&gt; </c>
   SYMBOL_INSERTSTM      = 114,     
   /// <c> &lt;Into Clause&gt; </c>
   SYMBOL_INTOCLAUSE     = 115,     
   /// <c> &lt;Join&gt; </c>
   SYMBOL_JOIN2          = 116,     
   /// <c> &lt;Join Chain&gt; </c>
   SYMBOL_JOINCHAIN      = 117,     
   /// <c> &lt;Mult Exp&gt; </c>
   SYMBOL_MULTEXP        = 118,     
   /// <c> &lt;Negate Exp&gt; </c>
   SYMBOL_NEGATEEXP      = 119,     
   /// <c> &lt;Not Exp&gt; </c>
   SYMBOL_NOTEXP         = 120,     
   /// <c> &lt;Order Clause&gt; </c>
   SYMBOL_ORDERCLAUSE    = 121,     
   /// <c> &lt;Order List&gt; </c>
   SYMBOL_ORDERLIST      = 122,     
   /// <c> &lt;Order Type&gt; </c>
   SYMBOL_ORDERTYPE      = 123,     
   /// <c> &lt;Pred Exp&gt; </c>
   SYMBOL_PREDEXP        = 124,     
   /// <c> &lt;Query&gt; </c>
   SYMBOL_QUERY          = 125,     
   /// <c> &lt;Restriction&gt; </c>
   SYMBOL_RESTRICTION    = 126,     
   /// <c> &lt;Select Stm&gt; </c>
   SYMBOL_SELECTSTM      = 127,     
   /// <c> &lt;Tuple&gt; </c>
   SYMBOL_TUPLE          = 128,     
   /// <c> &lt;Type&gt; </c>
   SYMBOL_TYPE           = 129,     
   /// <c> &lt;Unique&gt; </c>
   SYMBOL_UNIQUE2        = 130,     
   /// <c> &lt;Update Stm&gt; </c>
   SYMBOL_UPDATESTM      = 131,     
   /// <c> &lt;Value&gt; </c>
   SYMBOL_VALUE          = 132,     
   /// <c> &lt;Where Clause&gt; </c>
   SYMBOL_WHERECLAUSE    = 133,     
   /// <c> &lt;With Clause&gt; </c>
   SYMBOL_WITHCLAUSE     = 134      
};

enum RuleConstants : int
{
   /// <c> &lt;Query&gt; ::= &lt;Alter Stm&gt; </c>
   RULE_QUERY                                                                = 0,    
   /// <c> &lt;Query&gt; ::= &lt;Create Stm&gt; </c>
   RULE_QUERY2                                                               = 1,    
   /// <c> &lt;Query&gt; ::= &lt;Delete Stm&gt; </c>
   RULE_QUERY3                                                               = 2,    
   /// <c> &lt;Query&gt; ::= &lt;Drop Stm&gt; </c>
   RULE_QUERY4                                                               = 3,    
   /// <c> &lt;Query&gt; ::= &lt;Insert Stm&gt; </c>
   RULE_QUERY5                                                               = 4,    
   /// <c> &lt;Query&gt; ::= &lt;Select Stm&gt; </c>
   RULE_QUERY6                                                               = 5,    
   /// <c> &lt;Query&gt; ::= &lt;Update Stm&gt; </c>
   RULE_QUERY7                                                               = 6,    
   /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id ADD COLUMN &lt;Field Def List&gt; &lt;Constraint Opt&gt; </c>
   RULE_ALTERSTM_ALTER_TABLE_ID_ADD_COLUMN                                   = 7,    
   /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id ADD &lt;Constraint&gt; </c>
   RULE_ALTERSTM_ALTER_TABLE_ID_ADD                                          = 8,    
   /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id DROP COLUMN Id </c>
   RULE_ALTERSTM_ALTER_TABLE_ID_DROP_COLUMN_ID                               = 9,    
   /// <c> &lt;Alter Stm&gt; ::= ALTER TABLE Id DROP CONSTRAINT Id </c>
   RULE_ALTERSTM_ALTER_TABLE_ID_DROP_CONSTRAINT_ID                           = 10,    
   /// <c> &lt;Create Stm&gt; ::= CREATE &lt;Unique&gt; INDEX IntegerLiteral ON Id ( &lt;Order List&gt; ) &lt;With Clause&gt; </c>
   RULE_CREATESTM_CREATE_INDEX_INTEGERLITERAL_ON_ID_LPARAN_RPARAN            = 11,    
   /// <c> &lt;Create Stm&gt; ::= CREATE TABLE Id ( &lt;ID List&gt; ) &lt;Constraint Opt&gt; </c>
   RULE_CREATESTM_CREATE_TABLE_ID_LPARAN_RPARAN                              = 12,    
   /// <c> &lt;Unique&gt; ::= UNIQUE </c>
   RULE_UNIQUE_UNIQUE                                                        = 13,    
   /// <c> &lt;Unique&gt; ::=  </c>
   RULE_UNIQUE                                                               = 14,    
   /// <c> &lt;With Clause&gt; ::= WITH PRIMARY </c>
   RULE_WITHCLAUSE_WITH_PRIMARY                                              = 15,    
   /// <c> &lt;With Clause&gt; ::= WITH DISALLOW NULL </c>
   RULE_WITHCLAUSE_WITH_DISALLOW_NULL                                        = 16,    
   /// <c> &lt;With Clause&gt; ::= WITH IGNORE NULL </c>
   RULE_WITHCLAUSE_WITH_IGNORE_NULL                                          = 17,    
   /// <c> &lt;With Clause&gt; ::=  </c>
   RULE_WITHCLAUSE                                                           = 18,    
   /// <c> &lt;Field Def&gt; ::= Id &lt;Type&gt; NOT NULL </c>
   RULE_FIELDDEF_ID_NOT_NULL                                                 = 19,    
   /// <c> &lt;Field Def&gt; ::= Id &lt;Type&gt; </c>
   RULE_FIELDDEF_ID                                                          = 20,    
   /// <c> &lt;Field Def List&gt; ::= &lt;Field Def&gt; , &lt;Field Def List&gt; </c>
   RULE_FIELDDEFLIST_COMMA                                                   = 21,    
   /// <c> &lt;Field Def List&gt; ::= &lt;Field Def&gt; </c>
   RULE_FIELDDEFLIST                                                         = 22,    
   /// <c> &lt;Type&gt; ::= BIT </c>
   RULE_TYPE_BIT                                                             = 23,    
   /// <c> &lt;Type&gt; ::= DATE </c>
   RULE_TYPE_DATE                                                            = 24,    
   /// <c> &lt;Type&gt; ::= TIME </c>
   RULE_TYPE_TIME                                                            = 25,    
   /// <c> &lt;Type&gt; ::= TIMESTAMP </c>
   RULE_TYPE_TIMESTAMP                                                       = 26,    
   /// <c> &lt;Type&gt; ::= DECIMAL </c>
   RULE_TYPE_DECIMAL                                                         = 27,    
   /// <c> &lt;Type&gt; ::= REAL </c>
   RULE_TYPE_REAL                                                            = 28,    
   /// <c> &lt;Type&gt; ::= FLOAT </c>
   RULE_TYPE_FLOAT                                                           = 29,    
   /// <c> &lt;Type&gt; ::= SMALLINT </c>
   RULE_TYPE_SMALLINT                                                        = 30,    
   /// <c> &lt;Type&gt; ::= INTEGER </c>
   RULE_TYPE_INTEGER                                                         = 31,    
   /// <c> &lt;Type&gt; ::= INTERVAL </c>
   RULE_TYPE_INTERVAL                                                        = 32,    
   /// <c> &lt;Type&gt; ::= CHARACTER </c>
   RULE_TYPE_CHARACTER                                                       = 33,    
   /// <c> &lt;Constraint Opt&gt; ::= &lt;Constraint&gt; </c>
   RULE_CONSTRAINTOPT                                                        = 34,    
   /// <c> &lt;Constraint Opt&gt; ::=  </c>
   RULE_CONSTRAINTOPT2                                                       = 35,    
   /// <c> &lt;Constraint&gt; ::= CONSTRAINT Id &lt;Constraint Type&gt; </c>
   RULE_CONSTRAINT_CONSTRAINT_ID                                             = 36,    
   /// <c> &lt;Constraint&gt; ::= CONSTRAINT Id </c>
   RULE_CONSTRAINT_CONSTRAINT_ID2                                            = 37,    
   /// <c> &lt;Constraint Type&gt; ::= PRIMARY KEY ( &lt;ID List&gt; ) </c>
   RULE_CONSTRAINTTYPE_PRIMARY_KEY_LPARAN_RPARAN                             = 38,    
   /// <c> &lt;Constraint Type&gt; ::= UNIQUE ( &lt;ID List&gt; ) </c>
   RULE_CONSTRAINTTYPE_UNIQUE_LPARAN_RPARAN                                  = 39,    
   /// <c> &lt;Constraint Type&gt; ::= NOT NULL ( &lt;ID List&gt; ) </c>
   RULE_CONSTRAINTTYPE_NOT_NULL_LPARAN_RPARAN                                = 40,    
   /// <c> &lt;Constraint Type&gt; ::= FOREIGN KEY ( &lt;ID List&gt; ) REFERENCES Id ( &lt;ID List&gt; ) </c>
   RULE_CONSTRAINTTYPE_FOREIGN_KEY_LPARAN_RPARAN_REFERENCES_ID_LPARAN_RPARAN = 41,    
   /// <c> &lt;Drop Stm&gt; ::= DROP TABLE Id </c>
   RULE_DROPSTM_DROP_TABLE_ID                                                = 42,    
   /// <c> &lt;Drop Stm&gt; ::= DROP INDEX Id ON Id </c>
   RULE_DROPSTM_DROP_INDEX_ID_ON_ID                                          = 43,    
   /// <c> &lt;Insert Stm&gt; ::= INSERT INTO Id ( &lt;ID List&gt; ) &lt;Select Stm&gt; </c>
   RULE_INSERTSTM_INSERT_INTO_ID_LPARAN_RPARAN                               = 44,    
   /// <c> &lt;Insert Stm&gt; ::= INSERT INTO Id ( &lt;ID List&gt; ) VALUES ( &lt;Expr List&gt; ) </c>
   RULE_INSERTSTM_INSERT_INTO_ID_LPARAN_RPARAN_VALUES_LPARAN_RPARAN          = 45,    
   /// <c> &lt;Update Stm&gt; ::= UPDATE Id SET &lt;Assign List&gt; &lt;Where Clause&gt; </c>
   RULE_UPDATESTM_UPDATE_ID_SET                                              = 46,    
   /// <c> &lt;Assign List&gt; ::= Id = &lt;Expression&gt; , &lt;Assign List&gt; </c>
   RULE_ASSIGNLIST_ID_EQ_COMMA                                               = 47,    
   /// <c> &lt;Assign List&gt; ::= Id = &lt;Expression&gt; </c>
   RULE_ASSIGNLIST_ID_EQ                                                     = 48,    
   /// <c> &lt;Delete Stm&gt; ::= DELETE FROM Id &lt;Where Clause&gt; </c>
   RULE_DELETESTM_DELETE_FROM_ID                                             = 49,    
   /// <c> &lt;Select Stm&gt; ::= SELECT &lt;Columns&gt; &lt;Into Clause&gt; &lt;From Clause&gt; &lt;Where Clause&gt; &lt;Group Clause&gt; &lt;Having Clause&gt; &lt;Order Clause&gt; </c>
   RULE_SELECTSTM_SELECT                                                     = 50,    
   /// <c> &lt;Columns&gt; ::= &lt;Restriction&gt; * </c>
   RULE_COLUMNS_TIMES                                                        = 51,    
   /// <c> &lt;Columns&gt; ::= &lt;Restriction&gt; &lt;Column List&gt; </c>
   RULE_COLUMNS                                                              = 52,    
   /// <c> &lt;Column List&gt; ::= &lt;Column Item&gt; , &lt;Column List&gt; </c>
   RULE_COLUMNLIST_COMMA                                                     = 53,    
   /// <c> &lt;Column List&gt; ::= &lt;Column Item&gt; </c>
   RULE_COLUMNLIST                                                           = 54,    
   /// <c> &lt;Column Item&gt; ::= &lt;Column Source&gt; </c>
   RULE_COLUMNITEM                                                           = 55,    
   /// <c> &lt;Column Item&gt; ::= &lt;Column Source&gt; Id </c>
   RULE_COLUMNITEM_ID                                                        = 56,    
   /// <c> &lt;Column Source&gt; ::= &lt;Aggregate&gt; </c>
   RULE_COLUMNSOURCE                                                         = 57,    
   /// <c> &lt;Column Source&gt; ::= Id </c>
   RULE_COLUMNSOURCE_ID                                                      = 58,    
   /// <c> &lt;Restriction&gt; ::= ALL </c>
   RULE_RESTRICTION_ALL                                                      = 59,    
   /// <c> &lt;Restriction&gt; ::= DISTINCT </c>
   RULE_RESTRICTION_DISTINCT                                                 = 60,    
   /// <c> &lt;Restriction&gt; ::=  </c>
   RULE_RESTRICTION                                                          = 61,    
   /// <c> &lt;Aggregate&gt; ::= Count ( * ) </c>
   RULE_AGGREGATE_COUNT_LPARAN_TIMES_RPARAN                                  = 62,    
   /// <c> &lt;Aggregate&gt; ::= Count ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_COUNT_LPARAN_RPARAN                                        = 63,    
   /// <c> &lt;Aggregate&gt; ::= Avg ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_AVG_LPARAN_RPARAN                                          = 64,    
   /// <c> &lt;Aggregate&gt; ::= Min ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_MIN_LPARAN_RPARAN                                          = 65,    
   /// <c> &lt;Aggregate&gt; ::= Max ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_MAX_LPARAN_RPARAN                                          = 66,    
   /// <c> &lt;Aggregate&gt; ::= StDev ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_STDEV_LPARAN_RPARAN                                        = 67,    
   /// <c> &lt;Aggregate&gt; ::= StDevP ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_STDEVP_LPARAN_RPARAN                                       = 68,    
   /// <c> &lt;Aggregate&gt; ::= Sum ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_SUM_LPARAN_RPARAN                                          = 69,    
   /// <c> &lt;Aggregate&gt; ::= Var ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_VAR_LPARAN_RPARAN                                          = 70,    
   /// <c> &lt;Aggregate&gt; ::= VarP ( &lt;Expression&gt; ) </c>
   RULE_AGGREGATE_VARP_LPARAN_RPARAN                                         = 71,    
   /// <c> &lt;Into Clause&gt; ::= INTO Id </c>
   RULE_INTOCLAUSE_INTO_ID                                                   = 72,    
   /// <c> &lt;Into Clause&gt; ::=  </c>
   RULE_INTOCLAUSE                                                           = 73,    
   /// <c> &lt;From Clause&gt; ::= FROM &lt;ID List&gt; &lt;Join Chain&gt; </c>
   RULE_FROMCLAUSE_FROM                                                      = 74,    
   /// <c> &lt;Join Chain&gt; ::= &lt;Join&gt; &lt;Join Chain&gt; </c>
   RULE_JOINCHAIN                                                            = 75,    
   /// <c> &lt;Join Chain&gt; ::=  </c>
   RULE_JOINCHAIN2                                                           = 76,    
   /// <c> &lt;Join&gt; ::= INNER JOIN &lt;ID List&gt; ON Id = Id </c>
   RULE_JOIN_INNER_JOIN_ON_ID_EQ_ID                                          = 77,    
   /// <c> &lt;Join&gt; ::= LEFT JOIN &lt;ID List&gt; ON Id = Id </c>
   RULE_JOIN_LEFT_JOIN_ON_ID_EQ_ID                                           = 78,    
   /// <c> &lt;Join&gt; ::= RIGHT JOIN &lt;ID List&gt; ON Id = Id </c>
   RULE_JOIN_RIGHT_JOIN_ON_ID_EQ_ID                                          = 79,    
   /// <c> &lt;Join&gt; ::= JOIN &lt;ID List&gt; ON Id = Id </c>
   RULE_JOIN_JOIN_ON_ID_EQ_ID                                                = 80,    
   /// <c> &lt;Where Clause&gt; ::= WHERE &lt;Expression&gt; </c>
   RULE_WHERECLAUSE_WHERE                                                    = 81,    
   /// <c> &lt;Where Clause&gt; ::=  </c>
   RULE_WHERECLAUSE                                                          = 82,    
   /// <c> &lt;Group Clause&gt; ::= GROUP BY &lt;ID List&gt; </c>
   RULE_GROUPCLAUSE_GROUP_BY                                                 = 83,    
   /// <c> &lt;Group Clause&gt; ::=  </c>
   RULE_GROUPCLAUSE                                                          = 84,    
   /// <c> &lt;Order Clause&gt; ::= ORDER BY &lt;Order List&gt; </c>
   RULE_ORDERCLAUSE_ORDER_BY                                                 = 85,    
   /// <c> &lt;Order Clause&gt; ::=  </c>
   RULE_ORDERCLAUSE                                                          = 86,    
   /// <c> &lt;Order List&gt; ::= Id &lt;Order Type&gt; , &lt;Order List&gt; </c>
   RULE_ORDERLIST_ID_COMMA                                                   = 87,    
   /// <c> &lt;Order List&gt; ::= Id &lt;Order Type&gt; </c>
   RULE_ORDERLIST_ID                                                         = 88,    
   /// <c> &lt;Order Type&gt; ::= ASC </c>
   RULE_ORDERTYPE_ASC                                                        = 89,    
   /// <c> &lt;Order Type&gt; ::= DESC </c>
   RULE_ORDERTYPE_DESC                                                       = 90,    
   /// <c> &lt;Order Type&gt; ::=  </c>
   RULE_ORDERTYPE                                                            = 91,    
   /// <c> &lt;Having Clause&gt; ::= HAVING &lt;Expression&gt; </c>
   RULE_HAVINGCLAUSE_HAVING                                                  = 92,    
   /// <c> &lt;Having Clause&gt; ::=  </c>
   RULE_HAVINGCLAUSE                                                         = 93,    
   /// <c> &lt;Expression&gt; ::= &lt;And Exp&gt; OR &lt;Expression&gt; </c>
   RULE_EXPRESSION_OR                                                        = 94,    
   /// <c> &lt;Expression&gt; ::= &lt;And Exp&gt; </c>
   RULE_EXPRESSION                                                           = 95,    
   /// <c> &lt;And Exp&gt; ::= &lt;Not Exp&gt; AND &lt;And Exp&gt; </c>
   RULE_ANDEXP_AND                                                           = 96,    
   /// <c> &lt;And Exp&gt; ::= &lt;Not Exp&gt; </c>
   RULE_ANDEXP                                                               = 97,    
   /// <c> &lt;Not Exp&gt; ::= NOT &lt;Pred Exp&gt; </c>
   RULE_NOTEXP_NOT                                                           = 98,    
   /// <c> &lt;Not Exp&gt; ::= &lt;Pred Exp&gt; </c>
   RULE_NOTEXP                                                               = 99,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; BETWEEN &lt;Add Exp&gt; AND &lt;Add Exp&gt; </c>
   RULE_PREDEXP_BETWEEN_AND                                                  = 100,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; NOT BETWEEN &lt;Add Exp&gt; AND &lt;Add Exp&gt; </c>
   RULE_PREDEXP_NOT_BETWEEN_AND                                              = 101,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Value&gt; IS NOT NULL </c>
   RULE_PREDEXP_IS_NOT_NULL                                                  = 102,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Value&gt; IS NULL </c>
   RULE_PREDEXP_IS_NULL                                                      = 103,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; LIKE StringLiteral </c>
   RULE_PREDEXP_LIKE_STRINGLITERAL                                           = 104,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; IN &lt;Tuple&gt; </c>
   RULE_PREDEXP_IN                                                           = 105,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; = &lt;Add Exp&gt; </c>
   RULE_PREDEXP_EQ                                                           = 106,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt;&gt; &lt;Add Exp&gt; </c>
   RULE_PREDEXP_LTGT                                                         = 107,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; != &lt;Add Exp&gt; </c>
   RULE_PREDEXP_EXCLAMEQ                                                     = 108,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &gt; &lt;Add Exp&gt; </c>
   RULE_PREDEXP_GT                                                           = 109,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &gt;= &lt;Add Exp&gt; </c>
   RULE_PREDEXP_GTEQ                                                         = 110,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt; &lt;Add Exp&gt; </c>
   RULE_PREDEXP_LT                                                           = 111,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; &lt;= &lt;Add Exp&gt; </c>
   RULE_PREDEXP_LTEQ                                                         = 112,    
   /// <c> &lt;Pred Exp&gt; ::= &lt;Add Exp&gt; </c>
   RULE_PREDEXP                                                              = 113,    
   /// <c> &lt;Add Exp&gt; ::= &lt;Add Exp&gt; + &lt;Mult Exp&gt; </c>
   RULE_ADDEXP_PLUS                                                          = 114,    
   /// <c> &lt;Add Exp&gt; ::= &lt;Add Exp&gt; - &lt;Mult Exp&gt; </c>
   RULE_ADDEXP_MINUS                                                         = 115,    
   /// <c> &lt;Add Exp&gt; ::= &lt;Mult Exp&gt; </c>
   RULE_ADDEXP                                                               = 116,    
   /// <c> &lt;Mult Exp&gt; ::= &lt;Mult Exp&gt; * &lt;Negate Exp&gt; </c>
   RULE_MULTEXP_TIMES                                                        = 117,    
   /// <c> &lt;Mult Exp&gt; ::= &lt;Mult Exp&gt; / &lt;Negate Exp&gt; </c>
   RULE_MULTEXP_DIV                                                          = 118,    
   /// <c> &lt;Mult Exp&gt; ::= &lt;Negate Exp&gt; </c>
   RULE_MULTEXP                                                              = 119,    
   /// <c> &lt;Negate Exp&gt; ::= - &lt;Value&gt; </c>
   RULE_NEGATEEXP_MINUS                                                      = 120,    
   /// <c> &lt;Negate Exp&gt; ::= &lt;Value&gt; </c>
   RULE_NEGATEEXP                                                            = 121,    
   /// <c> &lt;Value&gt; ::= &lt;Tuple&gt; </c>
   RULE_VALUE                                                                = 122,    
   /// <c> &lt;Value&gt; ::= Id </c>
   RULE_VALUE_ID                                                             = 123,    
   /// <c> &lt;Value&gt; ::= IntegerLiteral </c>
   RULE_VALUE_INTEGERLITERAL                                                 = 124,    
   /// <c> &lt;Value&gt; ::= RealLiteral </c>
   RULE_VALUE_REALLITERAL                                                    = 125,    
   /// <c> &lt;Value&gt; ::= StringLiteral </c>
   RULE_VALUE_STRINGLITERAL                                                  = 126,    
   /// <c> &lt;Value&gt; ::= NULL </c>
   RULE_VALUE_NULL                                                           = 127,    
   /// <c> &lt;Tuple&gt; ::= ( &lt;Select Stm&gt; ) </c>
   RULE_TUPLE_LPARAN_RPARAN                                                  = 128,    
   /// <c> &lt;Tuple&gt; ::= ( &lt;Expr List&gt; ) </c>
   RULE_TUPLE_LPARAN_RPARAN2                                                 = 129,    
   /// <c> &lt;Expr List&gt; ::= &lt;Expression&gt; , &lt;Expr List&gt; </c>
   RULE_EXPRLIST_COMMA                                                       = 130,    
   /// <c> &lt;Expr List&gt; ::= &lt;Expression&gt; </c>
   RULE_EXPRLIST                                                             = 131,    
   /// <c> &lt;ID List&gt; ::= &lt;Id Member&gt; , &lt;ID List&gt; </c>
   RULE_IDLIST_COMMA                                                         = 132,    
   /// <c> &lt;ID List&gt; ::= &lt;Id Member&gt; </c>
   RULE_IDLIST                                                               = 133,    
   /// <c> &lt;Id Member&gt; ::= Id </c>
   RULE_IDMEMBER_ID                                                          = 134,    
   /// <c> &lt;Id Member&gt; ::= Id Id </c>
   RULE_IDMEMBER_ID_ID                                                       = 135     
};
