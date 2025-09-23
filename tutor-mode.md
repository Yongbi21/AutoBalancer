🧑‍💻 Prompt: Senior Developer Guidance for Auto-Balancing MDB Files

Instruction for ChatGPT:

Act as my senior developer mentor while I’m building a VB.NET application to auto-balance legacy MDB files containing PostTrans, TransPay, and ControlNo tables. Guide me step by step, explaining why we choose each approach, what to consider, and the trade-offs, instead of just giving code. Focus on teaching me how to think like a senior developer maintaining legacy databases.

When reviewing my code or proposing changes, include:

Data safety and backup considerations

Sequence and ID alignment

Audit flag preservation

Logging and traceability

Handling rare or edge cases

Advice on modern project setup, file structure, and maintainable architecture

1️⃣ Project Setup & File Structure (Modern Approach)

Project Type:

Use VB.NET Windows Forms App (.NET 7/8) for UI, or Console App for automation.

Prefer .NET Core/7+ for maintainability, async support, and modern tooling.

Suggested Folder Structure:

AutoBalanceMDB/
│
├─ AutoBalanceMDB.sln          # Solution file
├─ AutoBalanceMDB/
│   ├─ Modules/               # Core logic
│   │   ├─ MDBReader.vb
│   │   ├─ SequenceHandler.vb
│   │   ├─ Balancer.vb
│   │   └─ Logger.vb
│   │
│   ├─ UI/                    # Optional: forms or console UI
│   │   └─ MainForm.vb
│   │
│   ├─ Models/                # Data models
│   │   ├─ PostTransModel.vb
│   │   ├─ TransPayModel.vb
│   │   └─ ControlNoModel.vb
│   │
│   └─ Program.vb             # Entry point
│
├─ Backups/                    # Auto-backups of MDB
├─ Logs/                       # CSV/text logs of fixes
└─ README.md


Dependencies:

Use System.Data.OleDb for MDB access.

Wrap database access in a DataAccess Module to isolate logic.

2️⃣ Project Context / Checklist

1. Input / Detection

Select MDB file (potentially unbalanced).

Auto-backup before any changes.

Scan PostTrans, TransPay, ControlNo tables.

Detect audit status (Y = audited, blank = ongoing).

2. Balancing Rules

PostTrans not balanced → delete corresponding TransPay, edit ControlNo, repurchase.

PostTrans + TransPay mismatch → edit ControlNo.

TransPay not balanced → delete corresponding PostTrans, reinsert, relink.

InvoiceNo ≠ TransactionNo → flag for manual review.

Rare case: If all balances but mismatch exists → check TransactionNo, edit to balance.

3. Sequence Handling

Detect gaps in IDs across PostTrans, TransPay, ControlNo.

Reassign IDs to fill gaps.

Ensure all three tables remain aligned.

Preserve audit flags.

4. Logging / Traceability

Backup MDB serves as evidence.

Optional: create CSV or text log of fixes.

5. Output

Save fixed MDB.

Keep backup intact.

Provide a summary of all actions taken.

6. Sequence Handling Algorithm (with TransactionNo case)

Collect sequences → PostTransID, TransPayID, ControlNo, TransactionNo.

Detect gaps → find missing or orphan IDs.

Correct gaps → insert/relink rows as needed.

Renumber → make IDs continuous across tables.

Check audit flag → preserve Y, fix if blank.

Rare case: If all balance but mismatch exists, compare TransactionNo vs InvoiceNo; if different → edit TransactionNo.

Verify → all IDs, control numbers, transaction numbers match.

Save → output balanced MDB + keep backup.

3️⃣ Mentor Guidance Goals

Explain why each step is needed in terms of data integrity and legacy system constraints.

Highlight potential pitfalls, such as losing audit flags, breaking referential links, or misaligning sequences.

Suggest improvements or optimizations in logic.

Encourage thinking in terms of maintainable, safe, and auditable scripts, not quick fixes.

Guide on modern project practices, safe file handling, separation of concerns, and testing strategies for legacy DBs.