
# <ins>Day 8: Handheld Halting</ins>

**URL:** https://adventofcode.com/2020/day/8

##### Part 1

In order to answer this puzzle, we need a way to execute a series of very simple instructions.

These instructions include:
* Nop (no-op) - does nothing
* Acc(umulations) - change the value of a single accumulation register.
* J(u)mp - Jumps to the instruction at a specified line #.

Throughout execution, the following states need to be tracked at all times.
* Current value of the accumulation register.
* Current line number.
* Record of which lines have already been executed (in order to determine whether we are returning to a given line multiple times).
* Whether the program has terminated due to the line number being placed beyond the last instruction.

We need to determine the value of the accumulation register just prior to any line being run the second time.

##### Part 2

Tracking the same states above, we need to determine which instruction that, when switched, eventually leads to an orderly program termination (rather than an infinite loop).

Taking a single line only, we can swap a Nop with Jmp or vice-versa.

There is no change to Acc instructions.

##### Inputs

The [puzzle input](Inputs.txt) has ~600 lines of instructions.

An example is shown below:
```
nop +81
acc -17
jmp +1
acc +31
jmp +211
```


##### Approach (Part 1)

As we are tracking whether any given line has already been run, it is simple to determine at what point we need to stop execution and retrieve the value within the accumulation register.


##### Approach (Part 2)

1. Convert the member level responses within each group into a set.
2. Determine the group level intersection of those sets.
3. Extract the count for each group and sum this over all groups.

##### Code comments

* Use of discriminated unions to represent the individual op-codes and also the overall instructions.
* The Seq.scan member easily allowed for the tracking of state after the execution of every instruction.