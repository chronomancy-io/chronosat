# Performance Analysis: ChronoSAT

## Complexity Classification

### Algorithm Complexity

| Operation | Time | Space |
|-----------|------|-------|
| Literal evaluation | O(1) | O(1) |
| Clause check | O(k) | O(1) |
| Full verification | O(m × k) | O(m × k + n) |

Where:

- n = number of variables
- m = number of clauses
- k = literals per clause (3 for 3-SAT)

### Complexity Proof: Verification

**Theorem:** CDE verifies satisfiability in O(m × k) time.

**Proof:**

1. **Clause check:** For each clause, evaluate k literals
   - Each `eval` is O(1): array lookup + conditional
   - Sum k values: O(k)
   - Compare to threshold: O(1)
   - Total per clause: O(k)

2. **All clauses:** Iterate m clauses
   - m × O(k) = O(m × k)

3. **For 3-SAT:** k = 3 (constant)
   - O(m × 3) = O(m)

**Space:**

- Assignment array: n bytes
- Clause storage: m × k × 2 bytes (16-bit indices)
- Total: O(m × k + n)

### Comparison: Search vs Verification

| Approach | Time | Space |
|----------|------|-------|
| Brute force search | O(2ⁿ) | O(n) |
| DPLL (worst case) | O(2ⁿ) | O(n) |
| CDE verification | O(m × k) | O(m × k + n) |

**Key insight:** CDE verifies a *given* assignment. It does not search.

## Benchmarking Methodology

### Test Environment

```text
Platform: Commodore 64
CPU: MOS 6510 @ 1.023 MHz (NTSC)
Emulator: VICE x64sc
Timer: Jiffy clock ($A1-$A2)
```

### Timing Procedure

```forth
: benchmark
  timer@ t0 !        \ Start timer
  sat?               \ Run verification
  timer@ t1 !        \ End timer
  t1 @ t0 @ - j>ms   \ Convert to milliseconds
  ;
```

### Jiffy Clock Conversion

```forth
: j>ms ( jiffies -- ms )
  17 um* drop ;      \ ~16.67ms per jiffy (60 Hz)
```

## Empirical Results

### Scaling by Variable Count

| Variables | Clauses | Storage | Time | Search Space |
|-----------|---------|---------|------|--------------|
| 32 | 16 | 96 B | 0.05s | 2^32 |
| 96 | 48 | 288 B | 0.15s | 2^96 |
| 192 | 96 | 576 B | 0.5s | 2^192 |
| 1024 | 512 | 3 KB | 2.7s | 2^1024 |
| 2048 | 1024 | 6 KB | 6.3s | 2^2048 |
| 4096 | 2048 | 12 KB | 10.7s | 2^4096 |

### Operations Count

| Configuration | Operations (m × k) | Time | Ops/Second |
|---------------|-------------------|------|------------|
| 32 vars | 48 | 50ms | 960 |
| 96 vars | 144 | 150ms | 960 |
| 1024 vars | 1,536 | 2.7s | 569 |
| 2048 vars | 3,072 | 6.3s | 488 |
| 4096 vars | 6,144 | 10.7s | 574 |

Average: ~570 operations/second on 1 MHz 6502.

### Search Space Comparison

```text
2^32   = 4.3 × 10⁹ (billions)
2^96   = 7.9 × 10²⁸ (more than atoms in Earth)
2^1024 = 1.8 × 10³⁰⁸ (308-digit number)
2^2048 = 3.2 × 10⁶¹⁶ (617-digit number)
2^4096 = 1.0 × 10¹²³³ (1234-digit number)
```

At 1 billion checks/second, brute force for 2^96: 2.5 × 10¹² years.

CDE verification: 0.15 seconds.

## Memory Profile

### Per-Component Usage

| Component | Formula | 2048 vars |
|-----------|---------|-----------|
| Assignment array | n | 2,048 bytes |
| Clause storage | m × k × 2 | 6,144 bytes |
| Variables (temps) | ~20 | 20 bytes |
| Code | ~500 | 500 bytes |
| **Total** | | 8,712 bytes |

### C64 Memory Budget

```text
Available dictionary: ~38 KB
ChronoSAT usage: ~9 KB
Remaining: ~29 KB

Maximum configuration:
- Variables: 16,384 (16 KB assignment)
- Clauses: 4,096 (24 KB storage)
- Total: ~41 KB (requires BASIC ROM disable)
```

## Optimization Techniques

### 1. Sparse Literal Storage

Only store variable indices, not full coefficient matrix:

```forth
\ 6 bytes per clause (3 literals × 2 bytes)
\ vs 2n bytes for full coefficient row
```

**Savings:** For n=2048, m=1024:

- Full matrix: 2 × 2048 × 1024 = 4 MB
- Sparse: 6 × 1024 = 6 KB
- Ratio: 700:1

### 2. Bit-Packed Negation

Store negation in high bit of variable index:

```forth
8000 constant neg-bit   \ bit 15 = negation
7fff constant var-mask  \ bits 0-14 = var index
```

**Benefit:** No separate negation array needed.

### 3. Early Exit

Stop verification on first failing clause:

```forth
: sat? ( -- flag )
  m 0 do
    i chk 0= if 0 unloop exit then  \ Early exit
  loop -1 ;
```

**Best case:** O(1) for unsatisfiable assignment.

### 4. Zero Page Usage

ChronoForth's split stack puts data stack in zero page:

- Faster indexed access
- Single-byte stack pointer

## Cycle Count Analysis

### Per-Literal Evaluation

```forth
: eval ( literal -- 0|1 )
  dup var-mask and    \ 2 cycles: DUP, AND
  assign@             \ 6 cycles: indexed load
  swap neg-bit and    \ 2 cycles: SWAP, AND
  if ... then ;       \ 4-8 cycles: conditional
                      \ Total: ~16 cycles
```

### Per-Clause Check

```forth
: chk ( clause -- flag )
  dup 0 lit@          \ ~20 cycles
  eval                \ ~16 cycles
  over 1 lit@ eval    \ ~36 cycles
  + swap 2 lit@ eval  \ ~38 cycles
  + 1 >= ;            \ ~10 cycles
                      \ Total: ~120 cycles
```

### Full Verification (1024 clauses)

```text
1024 clauses × 120 cycles = 122,880 cycles
At 1 MHz: 0.123 seconds (compute only)
Actual: ~6 seconds (includes interpreter overhead)
```

Interpreter overhead: ~50x. Native 6502 would be ~50x faster.

---

*Standardized with [chronoboiler](https://github.com/the-chronomancer/chronoboiler) v1.0.0*
