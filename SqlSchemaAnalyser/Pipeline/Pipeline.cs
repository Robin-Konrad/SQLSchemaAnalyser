using Models;
using Parsers;
using Analysers;
using System.Text;

public class AnalysisPipeline
{
    private List<IAnalyser> analysers;

    public AnalysisPipeline(List<IAnalyser> analysers) {
        this.analysers = analysers;
    }
    
    public async Task<string> Run(string sqlstring){

        List<string> statements = PreProcess(sqlstring);   // strip comments and get list of statements

        var analysis = await Analyse(statements);          // get analysis on list of statements

        string report = BuildReport(analysis.allFindings);          // build markdown report of findings

        return report;
    }

    private List<string> PreProcess(string sqlstring)
    {
        sqlstring = CommentStripper.StripComments(sqlstring);
        List<string> statements = Parser.ParseStatements(sqlstring);

        int tablecount = CountTables.Count(statements);
        // only analyse schemas with at least 1 table to not waste API calls on non existent schemas
        if (tablecount < 1)
        {
            throw new ArgumentException("No CREATE TABLE statements found in schema.");
        }

        return statements;
    }

    private async Task<(List<Finding> allFindings, List<LlmUsageData> allUsageData)> Analyse(List<string> statements)
    {
        List<Finding> allFindings = new List<Finding>();
        List<LlmUsageData> allUsageData = new List<LlmUsageData>();

        foreach (IAnalyser analyser in this.analysers)
        {
            var results = await analyser.AnalyseAsync(statements);
            allFindings.AddRange(results.Findings);  // adds all items in list Findings to list allFindings
            allUsageData.Add(results.Usage);
        }

        return (allFindings, allUsageData);
    }

    private string BuildReport(List<Finding> findings)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("# SQL Schema Analysis Report\n");

        string[] severityOrder = { "warning", "suggestion", "info" };

        var groupedFindings = findings                  
            .GroupBy(f => f.Severity)
            .OrderBy(g => Array.IndexOf(severityOrder, g.Key.ToLower()));      // make report always be in order warning, suggestion, info

        foreach (var group in groupedFindings)
        {
            sb.AppendLine($"## {group.Key} findings\n");
            sb.AppendLine("| Table | Column | Issue | Suggestion |");
            sb.AppendLine("|---|---|---|---|");

            foreach (Finding finding in group)
            {
                sb.AppendLine($"| {finding.Table} | {finding.Column} | {finding.Issue} | {finding.Suggestion} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
