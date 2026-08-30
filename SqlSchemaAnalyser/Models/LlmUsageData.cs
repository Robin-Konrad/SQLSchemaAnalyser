namespace Models;

public record LlmUsageData (
    string Model,
    string Category, 
    int InputTokens,
    int OutputTokens,
    double LatencyMs,
    string PromptVersion,
    DateTime Timestamp
);