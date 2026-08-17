namespace LlmClient;

using Prompts;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

public class Client
{
    private readonly ChatClient _chatClient;
    public static string QueryApi(PromptConfig pc, string schema)
    {
        string SystemPrompt = pc.SystemPrompt;
        string[] fewShotExamples = pc.FewShotExamples;
        ModelSettings Modelsettings = pc.ModelSettings;

        return "blank";

        // to be completed
    }


}