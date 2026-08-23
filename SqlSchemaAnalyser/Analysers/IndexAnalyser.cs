namespace Analysers;

using LlmClient;
using Models;
using Prompts;
using System.Text.Json;
using Parsers;

public class IndexAnalyser : IAnalyser
{
    public string Category { get; } = "indexes";
    private AnalysisClient AC;
    private string promptVersion;

    public IndexAnalyser(AnalysisClient AC, string promptVersion = "") {
        this.AC = AC;
        this.promptVersion = promptVersion;
    }

    public async Task<(List<Finding> Findings, LlmUsageData Usage)> AnalyseAsync(string schema) {

        // filter schema to only include CREATE TABLE or ALTER TABLE statements as these are the only ones relevant to index analysing
        string[] allStatements = Parser.ParseStatements(schema);

        List<string> relevantStatements = allStatements
            .Where(s => s.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase)
                    || s.StartsWith("ALTER TABLE", StringComparison.OrdinalIgnoreCase))
            .ToList();

        string filteredSchema = string.Join("\n", relevantStatements);

        PromptConfig indexPrompt = PromptLoader.LoadPrompt("indexes", this.promptVersion); // load correct PromptConfig

        var response = await this.AC.QueryApi(indexPrompt, filteredSchema);  // make call to azure client with PromptConfig and schema (response.ReponseText response.UsageData)

        List<Finding> findings = JsonSerializer.Deserialize<List<Finding>>(response.ResponseText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException($"Failed to deserialize response text: \n{response.ResponseText}");

        return (findings, response.UsageData);
    }
}