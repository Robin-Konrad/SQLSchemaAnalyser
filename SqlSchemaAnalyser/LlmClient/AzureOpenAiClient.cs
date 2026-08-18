namespace LlmClient;

using Prompts;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

public class AnalysisClient
{
    private ChatClient client;
    public AnalysisClient(string apiKey, string endpoint, string deploymentName)
    {
        var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        client = azureClient.GetChatClient(deploymentName);
    }
    
    public async Task<string> QueryApi(PromptConfig pc, string schema)
    {
        string systemPrompt = pc.SystemPrompt;
        string[] fewShotExamples = pc.FewShotExamples;
        ModelSettings modelSettings = pc.ModelSettings;

        var options = new ChatCompletionOptions
            {
                Temperature = modelSettings.Temperature,
                MaxOutputTokenCount = modelSettings.MaxTokens
            };

        // add fewShotExamples into the systemprompt
        string fullSystemPrompt = systemPrompt + "\n\nExamples:\n" + string.Join("\n\n", fewShotExamples);
        
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(fullSystemPrompt),
            new UserChatMessage(schema)
        };

        ChatCompletion completion = await client.CompleteChatAsync(messages, options);  // make network call to azure openai

        return completion.Content[0].Text;   // just asking for plain text back, so Content collection will only have one item
    }


}