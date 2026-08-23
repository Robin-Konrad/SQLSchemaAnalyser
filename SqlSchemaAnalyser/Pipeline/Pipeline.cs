using Models;
using Parsers;

public class AnalysisPipeline
{
    private static void PreProcess(string sqlstring)
    {
        sqlstring = CommentStripper.StripComments(sqlstring);
        List<string> statements = Parser.ParseStatements(sqlstring);

        int tablecount = CountTables.Count(statements);
        // only analyse schemas with at least 1 table to not waste API calls on non existent schemas
        if (tablecount < 1)
        {
            throw new ArgumentException("No CREATE TABLE statements found in schema.");
        }

        Analyse(statements)
    }

    private static void Analyse(List<string> statements)
    {
        // to be completed
    }

    private static void BuildReport(List<Finding> findings)
    {
        // to be completed
    }
}
