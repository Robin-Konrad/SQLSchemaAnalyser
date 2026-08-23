namespace Analysers;

using Models;

public interface IAnalyser
    {
        
        Task<(List<Finding> Findings, LlmUsageData Usage)> AnalyseAsync(string schema); //analyse schema and return findings
        
    }