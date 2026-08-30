namespace Observability;

using Models;
using System.Text.Json;

public class UsageLogger
{
    private const string logPath = "Observability/Logs/usage_log.jsonl";

    public static void Log(LlmUsageData usage)    // add a new LlmUsageDate line to usage_log.jsonl file
    {
        string line = JsonSerializer.Serialize(usage);
        File.AppendAllText(logPath, line + "\n");
    }
}