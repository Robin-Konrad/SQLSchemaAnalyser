using Models;
public class AnalysisPipeline
{
    private static void PreProcess(string sqlstring)
    {
        // to be completed
    }

    private static void Analyse(string schema, int tablecount)
    {
        // only analyse schemas with at least 1 table to not waste API calls on non existent schemas
        if (tablecount < 1)
        {
            throw new ArgumentException("No CREATE TABLE statements found in schema.");
        }

        // to be completed
    }

    private static void BuildReport(List<Finding> findings)
    {
        // to be completed
    }
}
