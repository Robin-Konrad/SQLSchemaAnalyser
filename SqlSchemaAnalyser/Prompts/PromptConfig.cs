namespace Prompts;
public record PromptConfig(
    string Version,
    string Category,
    string SystemPrompt,
    string[] FewShotExamples,
    ModelSettings ModelSettings
);

public record ModelSettings(
    double Temperature,
    int MaxTokens
);

