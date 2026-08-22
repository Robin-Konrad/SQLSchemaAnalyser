// find correct prompt and load into PromptConfig JSON
using Prompts;
using System.Text.Json;

public class PromptLoader
{
    // loads from specified category
    public static PromptConfig LoadPrompt(string category, string version = "")
    {

        if (new[] { "indexes", "normalization", "naming" }.Contains(category)) {

            if (version == "") {  
                return LoadLatest(category);
            } 

            return LoadPath($"Prompts/{version}/{category}.json");

        } else {

            throw new ArgumentException($"Non existant category: {category}");

        }
    }


    // load from a specific prompt filepath (public static to allow custom PromptConfig paths loading)
    public static PromptConfig LoadPath(string path)
    {
        string indexString = File.ReadAllText(path);
        return JsonSerializer.Deserialize<PromptConfig>(indexString, new JsonSerializerOptions {PropertyNameCaseInsensitive = true}) 
            ?? throw new InvalidOperationException($"Failed to deserialize prompt config from {path}");
    }


    // load latest prompt version for that category 
    private static PromptConfig LoadLatest(string category) {
        var directories = Directory.GetDirectories("Prompts");  // get list of directories in Prompts/  eg v1.0/  v2.0/

        var latestVersion = directories
            .Select(dir => Path.GetFileName(dir))
            .Where(name => name.StartsWith("v"))     // only get directories starting with v
            .Select(name => name.Substring(1))       // remove leading v
            .Select(versionString => Version.Parse(versionString))    // parse string into a versionString variable using Linq.Version
            .Max();   // get highest
    

        string filePath = $"Prompts/v{latestVersion}/{category}.json";

        if (!File.Exists(filePath)) {
            throw new InvalidOperationException(
                $"No latest version (v{latestVersion}) prompt json file found for category '{category}', please specify an older version instead"
            );
        }

        return LoadPath(filePath);
    }

    
}