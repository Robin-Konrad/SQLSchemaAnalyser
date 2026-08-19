using LlmClient;
using Parsers;
using Prompts;
using DotNetEnv;
using Models;
using System.Text.Json;

Env.Load(); 

string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")
    ?? throw new InvalidOperationException("AZURE_OPENAI_KEY not set in .env");

string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not set in .env");

string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT not set in .env");

// test
string sqlstring = @"
/* =========================================================
   Database Initialization Script
   Target Engine: PostgreSQL / MySQL / Transact-SQL
   Description: Creates tables, seeds test data, and runs query.
   ========================================================= */

-- Step 1: Clean up existing test tables if they exist
DROP TABLE IF EXISTS audit_logs;
DROP TABLE IF EXISTS users;

/* Step 2: Create main users table
   Includes basic profile fields and status flags. */
CREATE TABLE users (
    user_id INT PRIMARY KEY AUTO_INCREMENT, -- Unique identifier
    username VARCHAR(50) NOT NULL,          /* Desired login name */
    email VARCHAR(100) UNIQUE,              -- Contact email address
    bio TEXT DEFAULT '/* Not provided */',  -- Test: Comment marker inside string literal!
    status VARCHAR(20) DEFAULT 'active',    -- Options: 'active', 'suspended', 'pending'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Step 3: Insert initial sample records
INSERT INTO users (username, email, bio) VALUES
('alice_d', 'alice@example.com', 'Dev Engineer -- SQL expert'), -- Inline comment lookalike inside string
('bob_b', 'bob@example.com', 'Product Manager /* Lead */'),     -- Multi-line comment lookalike inside string
('charlie_c', 'charlie@example.com', NULL);                    /* User without a bio */

/* ---------------------------------------------------------
   Step 4: Execute analytical query with inline joins
   --------------------------------------------------------- */
SELECT 
    u.user_id,
    u.username, -- Retrieve username
    /* u.email, -- Temporarily disabled for privacy */
    u.status,
    COUNT(a.log_id) AS total_activity -- Count associated log entries
FROM users u
LEFT JOIN audit_logs a ON u.user_id = a.user_id
WHERE u.status = 'active' /* Filter for active users only */
GROUP BY u.user_id, u.username, u.status
ORDER BY u.user_id ASC; -- Sort sequentially by ID
";

Console.WriteLine("\n\n-------------------   Statement Parsing Test:  --------------\n");
var statements = Parser.ParseStatements(sqlstring);
foreach (string s in statements)
{
    Console.WriteLine(s);
}

// testing json deserialization in LoadPrompt
Console.WriteLine("\n\n-------------------   JSON deserialization Test:  --------------\n");

PromptConfig? indexPrompt = PromptLoader.LoadPrompt("indexes","v1.0");
if (indexPrompt != null)
{
    Console.WriteLine(indexPrompt);
    Console.WriteLine("\n\n-------------------   FewShotExamples:  --------------\n");

    foreach (string eg in indexPrompt.FewShotExamples)
    {
        Console.WriteLine(eg);
    }


    Console.WriteLine("\n\n-------------------   Azure OpenAi Client Test:  --------------\n");
    // testing azure OpenAi client

    AnalysisClient client = new AnalysisClient(apiKey, endpoint, deploymentName);
    string response = await client.QueryApi(indexPrompt, sqlstring);
    // string response = File.ReadAllText("../../testing.txt");          // temporary local testing purposes------------------------------------

    // parse raw response into findings.cs    
    List<Finding>? findings = JsonSerializer.Deserialize<List<Finding>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (findings == null)
    {
        Console.WriteLine("Error: null response from OpenAI Client");
    }
    else
    {
        foreach (Finding finding in findings)
        {
            Console.WriteLine(finding);
        }
    }
}


