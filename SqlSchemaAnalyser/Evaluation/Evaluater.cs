namespace Evaluation;

using Models;
using Analysers;
using System.Text.Json;
using Parsers;

public class Evaluator
{
    private const string TestCasesDir = "Evaluation/TestCases";

    public async Task Evaluate(List<IAnalyser> analysers) {

        List<EvaluationResult> results = await RunAsync(analysers);  // get a list of EvaluationResults records

        Console.WriteLine("\n\n-------------------   Evaluation Run:  --------------\n");

        // for each record write to console the results
        foreach (var result in results) 
        {
            Console.WriteLine(
                $"{result.TestCaseName}: DetectionRate={result.DetectionRate}, Correctness={result.Correctness}");

            if (result.Missed.Count > 0)
                Console.WriteLine(
                    $"  Missed: {string.Join(", ", result.Missed.Select(m => $"{m.Table}.{m.Column}"))}");

            if (result.Unexpected.Count > 0)
                Console.WriteLine(
                    $"  Unexpected: {string.Join(", ", result.Unexpected.Select(u => $"{u.Table}.{u.Column}"))}");
        }


        Console.WriteLine("\n-------------------   Category Averages:  -------------------\n");

        // for each category calculate the average DetectionRate and Correctness scores and write to console
        var categories = new[] { "indexes", "naming", "normalization" };

        foreach (var category in categories)
        {
            var categoryResults = results
                .Where(r => r.Category.Contains(category))
                .ToList();

            if (categoryResults.Count == 0)
            {
                Console.WriteLine($"{category}: No results");
                continue;
            }

            var averageDetectionRate = categoryResults.Average(r => r.DetectionRate);
            var averageCorrectness = categoryResults.Average(r => r.Correctness);

            Console.WriteLine(
                $"{category}: " +
                $"Average DetectionRate={averageDetectionRate:F2}, " +
                $"Average Correctness={averageCorrectness:F2}");
        }
    }

    private async Task<List<EvaluationResult>> RunAsync(List<IAnalyser> analysers)
    {
        List<EvaluationResult> results = new List<EvaluationResult>();  // list for all the test cases EvalutionResults

        List<TestCase> testCases = LoadTestCases();  // load each test case and expected findings pair into a TestCase record

        foreach (TestCase testCase in testCases)
        {
            string schema = File.ReadAllText(testCase.SchemaPath);
            List<string> statements = Parser.ParseStatements(schema);

            foreach (IAnalyser analyser in analysers)
            {

                var (rawFindings, usage) = await analyser.AnalyseAsync(statements); // run IAnalyser and get findings

                // filter to warning/suggestion findings
                List<Finding> actualFindings = rawFindings
                    .Where(f => f.Severity == "warning" || f.Severity == "suggestion")
                    .ToList();

                
                EvaluationResult result = ScoreTestCase(testCase, analyser.Category, actualFindings); // compute scores and create a EvaluationResult record
                results.Add(result);
            }
        }

        return results;
    }

    private List<TestCase> LoadTestCases()
    {
        List<TestCase> testCases = new List<TestCase>();

        var sqlFiles = Directory.GetFiles(TestCasesDir, "case*.sql");

        foreach (string sqlPath in sqlFiles)
        {
            string expectedFindingsPath = sqlPath.Replace(".sql", ".json");   // path for the expected findings json pair for the sql test case

            if (!File.Exists(expectedFindingsPath))
            {
                throw new InvalidOperationException($"Missing the expected findings file for {sqlPath} sql test case");
            }

            List<ExpectedFinding> expectedFindings = JsonSerializer.Deserialize<List<ExpectedFinding>>(
                File.ReadAllText(expectedFindingsPath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidOperationException($"Failed to parse {expectedFindingsPath}");

            testCases.Add(new TestCase(
                Name: Path.GetFileNameWithoutExtension(sqlPath),
                SchemaPath: sqlPath,
                ExpectedFindings: expectedFindings
            ));
        }

        return testCases;
    }

    private EvaluationResult ScoreTestCase(TestCase testCase, string category, List<Finding> actualFindings)
    {
        List<ExpectedFinding> categoryExpectedFindings  = testCase.ExpectedFindings    // filter to only expected findings for this category
            .Where(e => e.Category == category)
            .ToList();

        // get count of where expected table-column pair suggestions/warnings were correctly identified
        int truePositives = categoryExpectedFindings.Count(e =>                
            actualFindings.Any(a => a.Table == e.Table && a.Column == e.Column));


        // get table-column pair suggestions/warnings in categoryExpectedFindings which were not identified
        List<ExpectedFinding> missed = categoryExpectedFindings       
            .Where(e => !actualFindings.Any(a => a.Table == e.Table && a.Column == e.Column))
            .ToList();

        // get table-column pair suggestions/warnings in actualFindings which were not in categoryExpectedFindings
        List<Finding> unexpected = actualFindings
            .Where(a => !categoryExpectedFindings.Any(e => e.Table == a.Table && e.Column == a.Column))
            .ToList();

        double detectionRate = categoryExpectedFindings.Count == 0 ? 1.0 : (double)truePositives / categoryExpectedFindings.Count;
        double correctness = actualFindings.Count == 0 ? 1.0 : (double)truePositives / actualFindings.Count;

        return new EvaluationResult(
            TestCaseName: $"{testCase.Name}",
            Category: category,
            DetectionRate: detectionRate,
            Correctness: correctness,
            Missed: missed,
            Unexpected: unexpected
        );
    }
}