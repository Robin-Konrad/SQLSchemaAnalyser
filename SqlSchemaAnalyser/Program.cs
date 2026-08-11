using System.Text;

// stripping comments from sample test raw sql string
string sqlstring = @"
/* =========================================================
   Database Initialization Script
   Target Engine: PostgreSQL / MySQL / Transact-SQL
   Description: Creates tables, seeds test data, and runs query.
   ========================================================= */

-- Step 1: Clean up existing test tables if they exist
DROP TABLE IF EXISTS audit_logs;
DROP TABLE IF EXISTS users;

/* Step 2: Create main users table
   Includes basic profile fields and status flags. */
CREATE TABLE users (
    user_id INT PRIMARY KEY AUTO_INCREMENT, -- Unique identifier
    username VARCHAR(50) NOT NULL,          /* Desired login name */
    email VARCHAR(100) UNIQUE,              -- Contact email address
    bio TEXT DEFAULT '/* Not provided */',  -- Test: Comment marker inside string literal!
    status VARCHAR(20) DEFAULT 'active',    -- Options: 'active', 'suspended', 'pending'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Step 3: Insert initial sample records
INSERT INTO users (username, email, bio) VALUES
('alice_d', 'alice@example.com', 'Dev Engineer -- SQL expert'), -- Inline comment lookalike inside string
('bob_b', 'bob@example.com', 'Product Manager /* Lead */'),     -- Multi-line comment lookalike inside string
('charlie_c', 'charlie@example.com', NULL);                    /* User without a bio */

/* ---------------------------------------------------------
   Step 4: Execute analytical query with inline joins
   --------------------------------------------------------- */
SELECT 
    u.user_id,
    u.username, -- Retrieve username
    /* u.email, -- Temporarily disabled for privacy */
    u.status,
    COUNT(a.log_id) AS total_activity -- Count associated log entries
FROM users u
LEFT JOIN audit_logs a ON u.user_id = a.user_id
WHERE u.status = 'active' /* Filter for active users only */
GROUP BY u.user_id, u.username, u.status
ORDER BY u.user_id ASC; -- Sort sequentially by ID
";

static string StripComments(string sqlstring)
{
    StringBuilder sb = new StringBuilder();
    int comment = 0; // 0 = code, 1 = single-line comment, 2 = multi-line comment
    char? inQuote = null; // tracks open quote (' or ")

    for (int i = 0; i < sqlstring.Length; i++)
    {
        char c = sqlstring[i];
        char next = (i + 1 < sqlstring.Length) ? sqlstring[i + 1] : '\0';

        if (comment == 0)
        {
            // Track string literals (skip comment detection inside quotes)
            if ((c == '\'' || c == '"') && (inQuote == null || inQuote == c))
            {
                // Toggle quote state (handles escaped quotes like '')
                if (inQuote == c && next == c)
                {
                    sb.Append(c);
                    sb.Append(next);
                    i++; // Skip second quote character
                    continue;
                }
                inQuote = inQuote == null ? c : null;
                sb.Append(c);
            }
            // Check for single-line comment start (only outside quotes)
            else if (inQuote == null && c == '-' && next == '-')
            {
                comment = 1;
                i++; // Skip second '-'
            }
            // Check for multi-line comment start (only outside quotes)
            else if (inQuote == null && c == '/' && next == '*')
            {
                comment = 2;
                i++; // Skip '*'
            }
            else
            {
                sb.Append(c);
            }
        }
        else if (comment == 1) // Inside single-line comment
        {
            if (c == '\n')
            {
                sb.Append('\n'); // Preserve line breaks for formatting
                comment = 0;
            }
        }
        else if (comment == 2) // Inside multi-line comment
        {
            if (c == '*' && next == '/')
            {
                comment = 0;
                i++; // skip '/' as we know we are no longer in comment
            }
        }
    }
    return sb.ToString();
}


static string[] ParseStatements(string sqlstring)   // assumes comments already stripped from string
{
    sqlstring = StripComments(sqlstring);  // strip string of comments before parsing

    List<string> statements = [];
    StringBuilder current = new StringBuilder();   // current statement
    char? quoteChar = null;
    for (int i=0; i<sqlstring.Length; i++)
    {  
        char c = sqlstring[i];  // current char

        if ((c == '\'' || c == '"') && (quoteChar == null || quoteChar == c))   // checks if its inside string currently
        {
            current.Append(c);
            quoteChar = quoteChar == null ? c : null;  // set it to c if its == null,   else toggle it to null
        }
        else if (c == ';' && quoteChar == null)
        {
            current.Append(c);
            statements.Add(current.ToString());
            current.Clear();
        } 
        else
        {
            current.Append(c);
        }
    }
    return statements.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();  // remove empty statements 
}


// testing
var statements = ParseStatements(sqlstring);
foreach (string s in statements)
{
    Console.WriteLine(s);
}