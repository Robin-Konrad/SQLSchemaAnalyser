// find correct prompt and load into PromptConfig JSON
using Prompts;
using System.Text.Json;

public class PromptLoader
{
    // loads from specified category
    public static PromptConfig? LoadPrompt(string category, string version)
    {
        if (new[] { "indexes", "normalization", "naming" }.Contains(category)) {

            return LoadPath($"Prompts/{version}/{category}.json");

        } else
        {

            throw new ArgumentException($"Non existant category: {category}");

        }

    }

    // load from a specific prompt filepath 
    public static PromptConfig? LoadPath(string path)
    {
        string indexString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<PromptConfig>(indexString, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    }
}