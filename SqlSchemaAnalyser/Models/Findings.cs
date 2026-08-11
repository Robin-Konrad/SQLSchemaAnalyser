namespace Models;

// record for LLM findings output
public record Findings(
    string Category,
    string Severity,
    string Message
);


