
# <ins>Day 9: Handheld Halting</ins>

**URL:** https://adventofcode.com/2020/day/9

##### Part 1

Given a list of numbers, we had to find the first number in the series that was not the sum of any two numbers in the preamble.

The preamble for a given was the 25 numbers immediately before it in the series.

##### Part 2

To find a contiguous set of at least 2 numbers which sum up to give the part 1 answer above.

##### Inputs

The [puzzle input](Inputs.txt) had 1,000 numbers listed.

A subset of these is shown below:
```
...
2834836
3083482
3881565
3840097
4254961
3652975
3613632
...
```


##### Approach (Part 1)

Using a window of length 26, this was relatively easy to accomplish.

##### Approach (Part 2)

Similar (if not same) approach used as for the day 1 challenge.

##### Code comments

* Minor use of functional composition (>>) and (<<) to make intentions (hopefully) clearer.