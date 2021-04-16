
# <ins>Day 2: Password Philosophy</ins>

**URL:** https://adventofcode.com/2020/day/2

##### Part 1

Within the puzzle input, determine how many passwords are valid.

##### Part 2

As above, but subject to a difference interpretation of the rules.

##### Inputs

The [puzzle input](https://github.com/Rich-F-G-Mills/Advent-Of-Code-2020-/blob/master/Day%202/Inputs.txt) provides a list of numbers such as those below:  
> 4-5 r: rrrjr   
> 9-10 x: pxcbpxxwkqjttx  
> 8-13 b: rjbbbbvgrbrfjx  
> 3-5 d: dtddsdddddsddddddwd

##### Approach (Part 1)

1. Define a regular expression to extract the components of the password.
2. Taking the sub-matches for each password, cast into a tuple of the required type.
3. Determine the validity of each password subject to the information above.

##### Approach (Part 2)

_Similar to above._

##### Code comments

* No techniques worthy of note used.