
# <ins>Day 6: Custom Customs</ins>

**URL:** https://adventofcode.com/2020/day/6

##### Part 1

The goal is to count the number of distinct questions to which anyone answered positively to within each group.

The sum of this count over all groups is the required answer.

##### Part 2

Within each group, determine how many questions to which all members of the group answered positively.

As above, the some of this count over all groups is the answer.

##### Inputs

The [puzzle input](Inputs.txt) provides a list of responses.

As an example, the puzzle below has:
* 2 groups of 1 and 3 people respectively.
* The sole member of the first group responded positively to questions a, b and c.
* The three members of the second group responded positively to questions a, b and c respectively.

> abc

> a  
> b  
> c


##### Approach (Part 1)

1. Read in the group responses and split by blank links (corresponds to group level).
2. Convert the member level responses within each group into an overall group level response.
3. Count the number of distinct responses within each group.
4. Sum these over all groups.

##### Approach (Part 2)

1. Convert the member level responses within each group into a set.
2. Determine the group level intersection of those sets.
3. Extract the count for each group and sum this over all groups.

##### Code comments

* For part 2, converted member level responses into sets so that Set.intersectMany could be used.