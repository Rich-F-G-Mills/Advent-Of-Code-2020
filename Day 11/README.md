
# <ins>Day 11: Seating System</ins>

**URL:** https://adventofcode.com/2020/day/11

##### Part 1

This is very similar to [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) except that rather than simulating "cells" on a grid, we are simulating the state of seating on a ferry.

After each iteration of the seating grid, each seat changes to empty of filled depending on the statuses of adjacent seats. 

##### Part 2

Similar to above, however it is not the immediately adjacent cells that necessarily influence the new state of a given seat. Instead, each cell can look over empty spaces until a seat (whether occupied or not) is found.

There is also a slight tweak to the rule which decides whether a given seat becomes empty.

##### Inputs

The first 5 rows of the [puzzle input](Inputs.txt) are shown below...
```
L.LLLLLLLLLL.LLLLLLLLL.LLLLLLLLL.LLL.LL.LLLLLLLLL.LLLLLL.LLLL.LL..LLLL.LLLL.LLL.LL.LLLLLLL
.LLLL.L.LLLLLLL.LLLLLLLLLLLLLLL..LLLLLL.LLLLLLLLL.LLLLLL.LLLLLLL.LLLLLLLLLL.LLLLLLLLLLLLLL
LLLLLLLL.LLL.LLLL.LLLL.LLLLLLLLL.LLLLLLLLLLLLLLLL.LLLLLL.LLLLLLLLL.LLL.LLLL.LLLLLLLLLLLLLL
LLLLL..LL.LL.LLLLLLLLLLLLLLLLLLLLLLLLLL.LLLLLLLLL.LLLLLLLLLLLLLL.LLLLLLL.LLLLLLLLL.LLLLLLL
LLLLL.L.LLLLLLLL.LLLLL.LLLLLLLLLLLLLLLL.LLLLLLLLLLLLLLLL.LLLLLLL.LLLLL.LLLLLLLLLLLLLLLLLLL
```

The following legend is used:

* `L` = Empty seat
* `#` = Occupied seat
* `.` = Floor space.


##### Approach (Part 1)



##### Approach (Part 2)



##### Code comments

* 