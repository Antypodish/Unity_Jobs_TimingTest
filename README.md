# Unity_Jobs_BranchingTimingTest
Testing timing of different branching approaches.

Jobs Branching Timing test has following results:

Iterating 1 million of elements.

Main thread no burst
- Branching using ? 2 ms
- No branching 5 ms
- No branchign with if 6 ms
- Branching if else 7 ms

Parallel For With burst
- Branching using ? 9 ms
- No branching 22 ms
- No branchign with if 10 ms
- Branching if else 10 ms
