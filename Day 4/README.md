
# <ins>Day 4: Passport Processing</ins>

**URL:** https://adventofcode.com/2020/day/4

##### Part 1

The goal is to identify which of the passports in the supplied inputs file have all of the required fields present.

##### Part 2

As above, but the respective fields must of a particular format.

##### Inputs

The [puzzle input](https://github.com/Rich-F-G-Mills/Advent-Of-Code-2020-/blob/master/Day%204/Inputs.txt) provides a list of numbers such as those below:  

The sample below shows the relevant information for 3x passports.

Passports are always seperated by a blank line; however, the fields for a given passport can span multiple lines.

```
hgt:159cm
pid:561068005 eyr:2025 iyr:2017 cid:139 ecl:blu hcl:#ceb3a1
byr:1940

iyr:2014
byr:1986 pid:960679613 eyr:2025 ecl:hzl

cid:211 ecl:blu hcl:#7d3b0c iyr:2011 pid:006632702
byr:1982 eyr:2023 hgt:68in

```


##### Approach (Part 1)

1. Read in the passport data and split by blank lines.
2. Within the resulting elements, replace new-lines with a single space; this ensures that the fields for a given passport are on a single line only.
3. Split the fields within each passport into a collection of key/value pairs.
4. A seperate mapping is created which specifies the field names required and the predicate to check that the value is in the required format (the latter part of which is used in the second part).
5. Identify which passports have at least the required fields.

##### Approach (Part 2)

1. Similar to above; however, the format checking predicate is also applied for each required field.
2. Any passports which, although having the required fields, fail the format predicate are excluded.

##### Code comments

* Used a mapping to both represent the required field names and the format predicate (keys/values respectively).
* Functional composition opperator used to string functional applications together (>>).
* Format predicates employed using:
  - Partially applied functions (such as 'is_digits')
  - Lambda functions used for requirements specified via regex.
  - Active patterns to check measurement units.