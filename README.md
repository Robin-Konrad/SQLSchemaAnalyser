# SQLSchemaAnalyser

An AI powered tool that takes an SQL schema as input and outputs structured, actionable findings for **indexing**, **naming conventions**, and **normalization**. 

This project was built to practice and demonstrate applied AI engineering practices such as prompt engineering, structured LLM output, evaluation based iteration, and observability.

---

## Features

- **Specialised analysers** (`Indexes`, `Naming`, `Normalization`): each analyser implements a shared `IAnalyser` interface and use their own versioned prompt configuration.
- **Structured output**: every finding is a `table` / `column` / `issue` / `suggestion` / `severity` record, so results can be parsed, filtered, and evaluated systematically.
- **Evaluation harness**: a suite of realistic varied SQL schemas with expected findings to evaluate analysers on and score based on detection rate and correctness (recall/precision).
- **Results based prompt iteration**: prompt versions went through multiple iterations, each revision made in response to failure patterns from the evaluation. Full versioned results are kept in `Evaluation/Results/`.
- **Observability**: the model, token counts, latency, prompt version/category and timestamp are logged to a `.jsonl` file for every LLM call.
- **Prompt Preprocessing**: the pipeline strips schemas of comments, parses them into individual statements, and filters statements down to only the relevant types for the specific analyser before any call to an LLM is made, removing noise from prompts and reducing token cost.

---

## Prompt engineering approach

Each analyser prompt is a versioned JSON config (system prompt, few-shot examples, model settings), with iterations based on evaluation results.
- **Positive few-shot examples** teach the model the expected output format and reasoning.
- **Negative few-shot examples** show already optimal schema tables that should produce no findings to prevent over-flagging.
- **Inclusion/exclusion rules** address cases where the model applied reasonable-sounding logic outside the intended scope (e.g. flagging every generic column name as needing an index as its possibly used as a filter).
- **Severity criteria** are defined explicitly per category to keep warning/suggestion/info boundaries consistent across runs.

For this project, Evaluation result patterns are manually analysed to drive new prompt revisions. For a larger scale system **LLM-as-a-judge** would automate the verification of evaluation findings and recommend prompt optimizations.

---

## Example output

A single run produces a markdown report grouped by severity:

```markdown
## warning findings
| Table | Column | Issue | Suggestion |
|---|---|---|---|
| orders | customer_id | Foreign key column has no index — joins and lookups on this column will scan the full table. | Add INDEX (customer_id) or declare it as a FOREIGN KEY, which creates the index automatically. |
| employees | department_name | Duplicates data already available via department_id — if a department is renamed, this column can silently go out of sync. | Remove department_name and JOIN to the departments table via department_id instead. |

## suggestion findings
| Table | Column | Issue | Suggestion |
|---|---|---|---|
| orders | order_status | Column is likely used to filter orders (e.g. "pending", "shipped") but has no index. | Add INDEX (order_status) if this column is frequently used in WHERE clauses. |
| user | id | Table is named "user" (singular) while every other table in the schema is plural — inconsistent convention. | Rename to "users" to match the rest of the schema. |
```

---

### Prerequisites
- .NET 10 SDK
- An Azure OpenAI resource with a deployed LLM model (this project was built and tested against `gpt-5-mini`)

### Setup

1. Clone the repository
2. Copy `.env.example` to `.env` and fill in your Azure OpenAI credentials.
3. Set runmode (`Analyse` / `Evaluate`), prompt version, and sql schema constants in `Program.cs`.
4. Restore and run:
   ```bash
   dotnet restore
   dotnet run 
   ```
