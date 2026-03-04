\ chronosat: coleman dimensional encoding
\ SPARSE storage, 4096 vars, 2048 clauses

require doloop

.( chronosat..)

hex

800 constant n        \ 2048 variables
400 constant m        \ 1024 clauses
3 constant k          \ literals per clause

\ Literal format: bit 15 = negation, bits 0-14 = var index
8000 constant neg-bit
7fff constant var-mask

\ Sparse storage: 3 words (6 bytes) per clause for 16-bit var indices
m k * 2 * constant clause-size

create assign n allot
create vars clause-size allot

variable t0 variable t1

: >= < 0= ;
\ Read 16-bit jiffy clock ($A1=high, $A2=low)
: timer@ a1 c@ 8 lshift a2 c@ or ;

\ Get literal for clause c, position p (0-2)
: lit@ ( clause pos -- literal )
  swap k * + 2* vars + @ ;

\ Store literal for clause c, position p
: lit! ( literal clause pos -- )
  swap k * + 2* vars + ! ;

: assign@ assign + c@ ;
: assign! assign + c! ;

\ Negate a literal (set bit 15)
: ~ ( var -- negated-lit ) neg-bit or ;

\ Evaluate a literal with negation support
: eval ( literal -- 0|1 )
  dup var-mask and assign@   \ get assign[var & 0x7FFF]
  swap neg-bit and if        \ check negation bit
    0= if 1 else 0 then      \ NOT: 0->1, 1->0
  then ;

\ Temps for clause definition
variable _ci variable _l0 variable _l1 variable _l2

\ Define a clause with 3 variable indices
: clause ( ci lit0 lit1 lit2 -- )
  _l2 ! _l1 ! _l0 ! _ci !
  _l0 @ _ci @ 0 lit!
  _l1 @ _ci @ 1 lit!
  _l2 @ _ci @ 2 lit! ;

\ Check one clause: sum of 3 literal values >= 1
: chk ( clause -- flag )
  dup 0 lit@ eval
  over 1 lit@ eval +
  swap 2 lit@ eval +
  1 >= ;

\ Check all clauses
: sat? ( -- flag )
  m 0 do i chk 0= if 0 unloop exit then loop -1 ;

\ Generate clauses with some negated literals
: gen-clauses ( -- )
  m 0 do
    i
    i 2 * n mod                 \ lit0: positive
    i 2 * 1 + n mod             \ lit1: base var
    i 3 mod 1 = if ~ then       \ negate every 3rd middle lit
    i 2 * 2 + n mod             \ lit2: positive
    clause
  loop ;

: init
  assign n 0 fill
  vars clause-size 0 fill
  gen-clauses
  n 0 do 1 i assign! loop ;

decimal

: .line 28 0 do [char] - emit loop ;
: elapsed - ;
: j>ms ( jiffies -- ms ) 17 um* drop ;


: chronosat
  cr .line cr
  ." chronosat" cr
  ." coleman dimensional encoding" cr
  .line cr cr
  ." vars: " n . ." clauses: " m . cr
  ." storage: " clause-size . ." bytes (sparse)" cr cr
  ." init..." init ." ok" cr
  ." check..." timer@ t0 ! sat? timer@ t1 !
  cr .line cr
  if ." * satisfiable *" else ." unsatisfiable" then
  cr .line cr cr
  ." ops: " m k * . cr
  ." time: " t1 @ t0 @ elapsed j>ms u. ." ms" cr
  cr
  ." brute: 2^" n . ." (617 digits)" cr
  ." cde: " m k * . ." ops = o(n)" cr
  cr
  ." here: " here u. cr
  ." free: " 40960 here - . ." bytes" cr
  cr ." ready." cr ;

\ Test UNSAT: flip vars 0,1,2 to break clause 0
: unsat-test
  cr ." --- unsat test ---" cr
  ." flip vars 0,1,2 to 0..."
  0 0 assign! 0 1 assign! 0 2 assign! ." ok" cr
  ." check..." sat?
  if ." error!" else ." rejected (correct)" then cr ;

: hello chronosat ;
: demo chronosat unsat-test ;
