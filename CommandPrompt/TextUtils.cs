using System;
using System.Collections.Generic;
using System.Text;

namespace FormatTextControl
{
    public class TextUtils
    {
        public static int GetNearestWordStart(string text, int index)
        {
            if (text[index] == ' ' || text[index] == ',')
                return -1;

            for (int i = index - 1; i >= 0; i--)
            {
                if (text[i] == ' ' || text[i] == '\n' || text[i] == ',')
                    return i + 1;
            }
            return 0;
        }
    }
}
