# ChronoSAT

A 3-SAT verifier implementing Coleman Dimensional Encoding (CDE), a geometric
interpretation of Boolean satisfiability that transforms clauses into hyperplane
constraints and variable assignments into points in n-dimensional space.

## Key Results

| Variables | Clauses | Time | Search Space |
|-----------|---------|------|--------------|
| 192 | 96 | 0.5s | 2^192 |
| 1024 | 512 | 2.7s | 2^1024 |
| 2048 | 1024 | 6.3s | 2^2048 (617 digits) |
| 4096 | 2048 | 10.7s | 2^4096 (1234 digits) |

All times measured on a 1 MHz 6502 processor (Commodore 64).

## CDE Implementation

### Dimensions

- Clause structure: CNF clause list with variable indices and polarity.
- Assignment state: current partial assignment bitset.
- Implication graph: directed edges between assignments induced by propagation.
- Heuristics: per-variable activity scores and decision levels.

### Query Workload

ChronoSAT is optimized for:

- “Given a CNF formula, determine satisfiability and produce a model if one exists.”
- “Given the current assignment, compute unit propagations and detect conflicts.”
- “Given a conflict, produce a learned clause and backtrack level.”

### Minimal Sufficient Statistic

A CDCL solver’s behavior is entirely driven by:

- Clause structure,
- Current assignment,
- Implication graph, and
- Heuristic scores.

This combination is sufficient to answer any query about search progress or satisfiability. Encoding exactly these yields a CDE layout that is MSS for SAT solving.

## What is CDE?

Coleman Dimensional Encoding reframes SAT verification as polytope membership:

- **Clauses become Linear Inequalities**: Each clause becomes a constraint
- **Assignment becomes Point**: Variable assignment is a point in {0,1}^n
- **SAT Check becomes Membership**: Does the point satisfy all constraints?

Verification is O(m×k) where m=clauses, k=literals per clause.

## Features

- Proper 3-SAT with negation support
- Sparse storage (6 bytes per clause)
- Polynomial-time verification O(n)
- SAT and UNSAT detection
- Runs on 1970s hardware

## Quick Start

### Requirements

- [VICE](https://vice-emu.sourceforge.io/) C64 emulator
- [ChronoForth](https://github.com/the-chronomancer/chronoforth) disk image

### Build and Run

```bash

# Build ChronoForth
cd ../chronoforth
make chronoforth.d64

# Add ChronoSAT to disk
cd ../chronosat
./add-to-disk.sh ../chronoforth/chronoforth.d64

# Run in emulator
x64sc ../chronoforth/chronoforth.d64
```text

### In the Emulator

```forth
INCLUDE CHRONOSAT
DEMO
```text

This runs:

1. **SAT test** - Verifies a satisfying assignment
2. **UNSAT test** - Correctly rejects a broken assignment

## Repository Structure

```text
chronosat/
├── README.md           # This file
├── ARCHITECTURE.md     # System design
├── PERFORMANCE.md      # Complexity analysis
├── PROOF.md            # Mathematical proof
├── CDE_VS_DP.md        # CDE vs Dynamic Programming
├── src/
│   └── chronosat.fs    # Forth implementation
├── paper/
│   └── chronosat-paper.tex  # LaTeX paper
└── examples/
    └── *.cnf           # Example problems
```text

## Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) - Component design and data flow
- [PERFORMANCE.md](PERFORMANCE.md) - Complexity proofs and benchmarks
- [PROOF.md](PROOF.md) - Complete mathematical explanation
- [CDE_VS_DP.md](CDE_VS_DP.md) - Why CDE is not Dynamic Programming

## The Paper

See `paper/chronosat-paper.tex` for formal mathematical treatment.

```bash
cd paper
pdflatex chronosat-paper.tex
```text

## What CDE Is (and Is Not)

**CDE IS:**

- A geometric interpretation of SAT verification
- Polynomial-time validation of candidate solutions
- A novel perspective on satisfiability

**CDE IS NOT:**

- A polynomial-time SAT solver (that would be P=NP)
- Faster than standard verification asymptotically
- A replacement for DPLL/CDCL solvers

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Author

Jacob Coleman

## Acknowledgments

- Built on [ChronoForth](https://github.com/the-chronomancer/chronoforth)
- Forked from [DurexForth](https://github.com/jkotlinski/durexforth)

---

*Standardized with [chronoboiler](https://github.com/the-chronomancer/chronoboiler) v1.0.0*
