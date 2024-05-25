using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStudio.SyntaxHighlight
{
    class SQLSyntaxHighlight : SyntaxHighlightBase
    {
        protected override bool IsKeyWord(string identifier)
        {
            switch (identifier.ToLower())
            {
                case "select":
                case "from":
                case "where":
                case "and":
                case "or":
                case "like":
                case "inner":
                case "outer":
                case "join":
                case "left":
                case "right":
                case "full":
                case "on":
                case "insert":
                case "into":
                case "values":
                case "delete":
                case "update":
                case "set":
                case "distinct":
                case "avg":
                case "min":
                case "max":
                case "order":
                case "by":
                case "asce":
                case "desc":
                case "create":
                case "table":
                case "drop":
                    return true;
                default:
                    return false;
            }
        }
    }
}
