# Contributing to ChronoSAT

## Code Review Checklist

- [ ] Mathematical correctness of CDE encoding
- [ ] Complexity claims accurate and proven
- [ ] Forth style follows conventions
- [ ] Stack effects documented

## Development Setup

```bash
# Build ChronoForth system
cd ../chronoforth && make chronoforth.d64

# Add ChronoSAT and test
cd ../chronosat
./add-to-disk.sh ../chronoforth/chronoforth.d64
x64sc ../chronoforth/chronoforth.d64
```

```forth
INCLUDE CHRONOSAT
DEMO  \ Run tests
```

## Requirements

- VICE C64 emulator
- ChronoForth system
- LaTeX (optional, for papers)

## Code Style

- 2-space indentation
- Stack effects: `( input -- output )`
- Lowercase Forth words
- Comments with `\`

## Commit Format

```
type(scope): description
```

Types: `feat`, `fix`, `proof`, `perf`, `docs`
