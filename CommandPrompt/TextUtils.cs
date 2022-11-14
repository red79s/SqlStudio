namespace FormatTextControl
{
    public class TextUtils
    {
        public static int GetNearestWordStart(string text, int index)
        {
            if (index >= text.Length)
                index = text.Length - 1;

            if (index < 0)
            {
                return 0;
            }

            if (index > 0 && (text[index] == ' ' || text[index] == ',') && text[index - 1] == ' ')
                return index;

            for (int i = index - 1; i >= 0; i--)
            {
                if (text[i] == ' ' || text[i] == '\n' || text[i] == ',')
                    return i + 1;
            }
            return 0;
        }
    }
}
