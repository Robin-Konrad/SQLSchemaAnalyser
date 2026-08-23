namespace Analysers;

using Models;

public interface IAnalyser
    {

        Task<(List<Finding> Findings, LlmUsageData Usage)> AnalyseAsync(List<string> statements); //analyse schema and return findings
        
    }