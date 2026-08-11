namespace Models;

public record LlmUsageData (
    string LlmModel,
    int InputTokens,
    int OutputTokens,
    double LatencyMs,
    string PromptVersion,
    DateTime TimeStamp
);