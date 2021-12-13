
# <ins>Day 1: Report Repair</ins>

**URL:** https://adventofcode.com/2020/day/1

##### Part 1

Within the puzzle input, find the two numbers that add to 2020 and then multiply them together.

##### Part 2

As above, but find the three numbers that sum to 2020.

##### Inputs

The [puzzle input](Inputs.txt) provides a list of numbers such as those below:  
> 1470  
> 1577  
> 1054  
> 1962

##### Approach (Part 1)

1. Read in numbers above and convert to 32 bit integers.
2. Sort numbers ascending.
3. Taking each number in order, find the next number that when added, is equal to or greater than the target.
4. If target is exceeded, go back to 3.

##### Approach (Part 2)

_Similar to above._

##### Code comments

* No techniques worthy of note used.