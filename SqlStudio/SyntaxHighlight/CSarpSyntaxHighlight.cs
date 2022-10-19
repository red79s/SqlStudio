using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStudio.SyntaxHighlight
{
    class CSarpSyntaxHighlight : SyntaxHighlightBase
    {
        protected override bool IsKeyWord(string identifier)
        {
            switch (identifier)
            {
                case "int":
                case "float":
                case "for":
                case "foreach":
                case "do":
                case "while":
                case "class":
                case "private":
                case "protected":
                case "public":
                case "struct":
                case "string":
                case "if":
                case "else":
                case "return":
                case "void":
                case "using":
                case "namespace":
                case "partial":
                case "this":
                case "null":
                case "true":
                case "false":
                    return true;
                default:
                    return false;
            }
        }
    }
}
