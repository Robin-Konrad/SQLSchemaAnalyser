namespace Parsers;
using System.Text;
public class CommentStripper 
{
    public static string StripComments(string sqlstring)
    {
        StringBuilder sb = new StringBuilder();
        int comment = 0; // 0 = code, 1 = single-line comment, 2 = multi-line comment
        char? inQuote = null; // tracks open quote (' or ")

        for (int i = 0; i < sqlstring.Length; i++)
        {
            char c = sqlstring[i];
            char next = (i + 1 < sqlstring.Length) ? sqlstring[i + 1] : '\0';

            if (comment == 0)
            {
                // Track string literals (skip comment detection inside quotes)
                if ((c == '\'' || c == '"') && (inQuote == null || inQuote == c))
                {
                    // Toggle quote state (handles escaped quotes like '')
                    if (inQuote == c && next == c)
                    {
                        sb.Append(c);
                        sb.Append(next);
                        i++; // Skip second quote character
                        continue;
                    }
                    inQuote = inQuote == null ? c : null;
                    sb.Append(c);
                }
                // Check for single-line comment start (only outside quotes)
                else if (inQuote == null && c == '-' && next == '-')
                {
                    comment = 1;
                    i++; // Skip second '-'
                }
                // Check for multi-line comment start (only outside quotes)
                else if (inQuote == null && c == '/' && next == '*')
                {
                    comment = 2;
                    i++; // Skip '*'
                }
                else
                {
                    sb.Append(c);
                }
            }
            else if (comment == 1) // Inside single-line comment
            {
                if (c == '\n')
                {
                    sb.Append('\n'); // Preserve line breaks for formatting
                    comment = 0;
                }
            }
            else if (comment == 2) // Inside multi-line comment
            {
                if (c == '*' && next == '/')
                {
                    comment = 0;
                    i++; // skip '/' as we know we are no longer in comment
                }
            }
        }
        return sb.ToString();
    }
}