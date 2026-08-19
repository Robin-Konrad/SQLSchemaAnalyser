namespace Models;

public record LlmUsageData (
    string Model,
    int InputTokens,
    int OutputTokens,
    double LatencyMs,
    string PromptVersion,
    DateTime Timestamp
);