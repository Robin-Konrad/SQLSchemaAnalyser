namespace SqlSchemaAnalyser.Analysers;
public interface IAnalyser
    {
        public string Category {get;}  // tells what type of analysing the analyser is doing
        
        public Task<string> AnalyseAsync(string schema); //analyse schema and return findings

    }