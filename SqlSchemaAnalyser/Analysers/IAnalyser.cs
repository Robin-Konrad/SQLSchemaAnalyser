namespace Analysers;

using Models;

public interface IAnalyser
    {
        string Category { get; }
        Task<(List<Finding> Findings, LlmUsageData Usage)> AnalyseAsync(List<string> statements); //analyse schema and return findings
        
    }