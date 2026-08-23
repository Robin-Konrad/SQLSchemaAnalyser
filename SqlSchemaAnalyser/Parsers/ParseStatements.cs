namespace Parsers;
using System.Text;
public class Parser 
{
    public static string[] ParseStatements(string sqlstring)   // assumes comments already stripped from string
    {
        sqlstring = CommentStripper.StripComments(sqlstring);  // strip string of comments before parsing

        List<string> statements = [];
        StringBuilder current = new StringBuilder();   // current statement
        char? quoteChar = null;
        for (int i=0; i<sqlstring.Length; i++)
        {  
            char c = sqlstring[i];  // current char

            if ((c == '\'' || c == '"') && (quoteChar == null || quoteChar == c))   // checks if its inside string currently
            {
                current.Append(c);
                quoteChar = quoteChar == null ? c : null;  // set it to c if its == null,   else toggle it to null
            }
            else if (c == ';' && quoteChar == null)
            {
                current.Append(c);
                statements.Add(current.ToString());
                current.Clear();
            } 
            else
            {
                current.Append(c);
            }
        }
        return statements.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();  // remove empty statements 
    }
}