#!/bin/bash
# Add ChronoSAT to a ChronoForth/DurexForth disk image
#
# Usage: ./add-to-disk.sh /path/to/chronoforth.d64

DISK="${1:-chronoforth.d64}"
SRC="$(dirname "$0")/src/chronosat.fs"

if [ ! -f "$DISK" ]; then
    echo "Error: Disk image not found: $DISK"
    echo "Usage: $0 /path/to/chronoforth.d64"
    exit 1
fi

if [ ! -f "$SRC" ]; then
    echo "Error: Source not found: $SRC"
    exit 1
fi

# Convert to PETSCII and add to disk
TMPFILE=$(mktemp)
printf aa | cat - "$SRC" | petcat -text -w2 -o "$TMPFILE" -
c1541 -attach "$DISK" -write "$TMPFILE" chronosat
rm "$TMPFILE"

echo "Added ChronoSAT to $DISK"
echo "In ChronoForth: INCLUDE CHRONOSAT"
echo "Then run: DEMO"

