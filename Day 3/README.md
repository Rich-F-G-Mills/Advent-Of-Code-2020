
# <ins>Day 3: Toboggan Trajectory</ins>

**URL:** https://adventofcode.com/2020/day/3

##### Part 1

The provided puzzle input provides the location of trees on a landscape.

The pattern can be assumed to repeat to the right infinitely many times.

From a given start position, moving in a specified direction, the aim is to find the number of trees encountered.

##### Part 2

As above, but checking additional directions.

##### Inputs

The [puzzle input](Inputs.txt) provides a list of numbers such as those below:  

```
...........#..............##...  
...#....................#......  
.....####...........#.#..#.#...  
....##.#.......................  
.......#.##......#.###.........  
.#.....#.......##.......#.....#
```


##### Approach (Part 1)

1. Read in grid and convert into a 2D (ragged) array of booleans.
2. Define helper function that (effectively) duplicates grid going right.
3. Count the trees encountered.

##### Approach (Part 2)

_Similar to above._

##### Code comments

* No techniques worthy of note used.
