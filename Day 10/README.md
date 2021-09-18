
# <ins>Day 10: Adapter Array</ins>

**URL:** https://adventofcode.com/2020/day/10

##### Part 1

Question provides a list of adapter _joltages_.

We needed to find a way to connect them to eachother so that **all** adapters were used.

The only requirement was that the maximum joltage difference between two directly connected adapters was 3 jolts.

##### Part 2

Rather than using all of the adapters, we needed to find the number of distinct ways in which the adapters could be connected whilst still achieving the target joltage.

##### Inputs

The [puzzle input](Inputs.txt) provides a list of adapter joltages such as those below:  
```
28
33
18
42
31
```


##### Approach (Part 1)

This was easily done by sorting the list of joltages and counting the number of each distinct joltage difference between pairwise elements.

##### Approach (Part 2)

To do this considering the whole list in one go was not possible.

However, splitting up the list where the difference between adjacent (and sorted) joltages is equal to 3 jolts means we can instead consider a number of smaller sub-problems instead.

Multiplying together the number of combinations for these sub-groups then gives the overall total.

##### Code comments

* Use of recursion to determine the number of combinations for a give sub-group.