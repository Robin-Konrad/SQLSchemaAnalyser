namespace SchemaLens.Parsers;
using System.Text.RegularExpressions;

public class CountTables
{
    public static int Count(List<string> statements)
    {
        Regex CreateTableRegex = new Regex(@"^\s*CREATE\s+TABLE\b", RegexOptions.IgnoreCase);
        return statements.Count(statement => CreateTableRegex.IsMatch(statement));
    }
}