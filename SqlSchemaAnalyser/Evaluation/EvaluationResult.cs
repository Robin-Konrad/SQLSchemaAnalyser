namespace Evaluation;
using Models;

public record EvaluationResult(
    string TestCaseName,
    string Category,
    double DetectionRate,
    double Correctness,
    List<ExpectedFinding> Missed,
    List<Finding> Unexpected
);