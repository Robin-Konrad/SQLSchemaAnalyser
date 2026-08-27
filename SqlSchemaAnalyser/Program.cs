using LlmClient;
using Parsers;
using Prompts;
using DotNetEnv;
using Models;
using System.Text.Json;
using Analysers;
using Evaluation;
using Pipeline;

Env.Load(); 


// select what the application should run:
RunMode mode = RunMode.Analyse;  // RunMode.Analyse: Analyse the SQL string below,  RunMode.Evaluate: Run the full evaluation on the latest prompts

const string PromptVersion = "";   // Prompt version to use for analysis/evaluation.   e.g. "v1.0".    leave "" to use the latest version.


// SQL schema to analyse when using RunMode.Analyse.  
// replace this string with the schema you want to analyse
const string sql = """     
    CREATE TABLE users (
        user_id INT PRIMARY KEY,
        username VARCHAR(50) NOT NULL,
        email VARCHAR(100) UNIQUE,
        created_at TIMESTAMP
    );

    CREATE INDEX idx_users_email ON users(email);
    """;

string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")
    ?? throw new InvalidOperationException("AZURE_OPENAI_KEY not set in .env");

string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not set in .env");

string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT not set in .env");

AnalysisClient client = new AnalysisClient(apiKey, endpoint, deploymentName);

var analysers = new List<IAnalyser>
{
    new IndexAnalyser(client, PromptVersion),     
    new NamingAnalyser(client, PromptVersion),
    new NormalizationAnalyser(client, PromptVersion)
};

switch (mode)
{
    case RunMode.Analyse:
        var pipeline = new AnalysisPipeline(analysers);
        Console.WriteLine(await pipeline.Run(sql));
        break;

    case RunMode.Evaluate:
        Console.WriteLine("\n\n-------------------   Evaluation Testing:  --------------\n");
        var evaluator = new Evaluator();
        await evaluator.Evaluate(analysers);
        break;
}

enum RunMode
{
    Analyse,
    Evaluate
}







