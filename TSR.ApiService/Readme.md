# Patterns to use with Client/Server upgrade project

| Pattern | Goal | Key Benefit in Migration |
| :--- | :--- | :--- |
| **Strangler Fig** | Incremental replacement | Safe, low-risk deployment of new features in parallel with the old system. |
| **Anti-Corruption Layer (ACL)** | Domain separation | Insulates the new Web API from the legacy data structures and logic. |
| **Bulkhead** | Service isolation/Resilience | Prevents failure in the legacy system/shared database from crashing the new API. |