namespace Models;

// record for LLM findings for each table in output
public record Finding(
    string Table,
    string Column,
    string Issue,
    string Suggestion,
    string Severity
);


