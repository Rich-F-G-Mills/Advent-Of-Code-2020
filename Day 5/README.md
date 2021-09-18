
# <ins>Day 5: Binary Boarding</ins>

**URL:** https://adventofcode.com/2020/day/5

##### Part 1

The goal is to decode a list of boarding pass numbers to determine the largest seat ID.

The first 7 characters specify the row and the remaining characters specify the column.

##### Part 2

Given all of the other seat IDs occupied on the plane, determine which one has yet to be scanned and is therefore your seat.

##### Inputs

The [puzzle input](Inputs.txt) provides a list of boarding pass codes to be decoded.

```
BFBFFFFLLL
BBFBBFBRLL
BFBBFFFRRR
FBFFBBBLLL
FFBBFBBRLL
```


##### Approach (Part 1)

1. Read in all of the boarding pass numbers.
2. The boarding pass numbers (effectively) represent the concatenation of two binary sequences with F/L -> 0 and B/R -> 1.
3. These can be easily extracted to generate the overal seat ID for every pass.
4. The largest seat ID is chosen.

##### Approach (Part 2)

1. Of all the seat IDs read in, there is one missing.
2. By sorting the list and then considering them pairwise, we can find out where there is a gap.
3. This gap is the answer!

##### Code comments

* Nothing worthy of note.