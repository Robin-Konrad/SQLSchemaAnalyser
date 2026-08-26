namespace Evaluation;
using Models;

public record EvaluationResult(
    string TestCaseName,
    double DetectionRate,
    double Correctness,
    List<ExpectedFinding> Missed,
    List<Finding> Unexpected
);