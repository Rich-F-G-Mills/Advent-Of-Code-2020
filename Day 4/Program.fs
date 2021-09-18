
open System
open System.IO
open System.Text.RegularExpressions

[<EntryPoint>]
let main argv =    
    
    // Split the input file into the individual passports and then extract the key-value pairs.
    let passportInfo =
        let inputs =
            File.ReadAllText "Inputs.txt"

        // This splits by individual passports.
        inputs.Split "\r\n\r\n"
        |> Array.map (
            fun x ->
                // Replace new-lines with spaces so that everything is on the same line.
                x.Replace("\r\n", " ").Split(" ")
                |> Array.map (
                    fun y -> y.Split(':'))
                |> Array.map (
                    fun y -> (y.[0], y.[1])))
        |> Array.map (Map.ofArray)

    // Specifies a list of predicates that must be satisfied for each key type.
    let requiredFields =
        let isDigits min max (x: string) =
            match (Int32.TryParse x) with
            | (true, value) -> (value >= min) && (value <= max)
            | _ -> false

        // (First attempt at using active patterns)
        // Use an active pattern to match against the units (if specified).
        let (|Inches|Cm|Neither|) (x: string) =
            let reMatch = Regex.Match(x, @"^(\d{2,3})(in|cm)$")

            if reMatch.Success then
                match Int32.TryParse reMatch.Groups.[1].Value with
                | (true, value) ->
                    match reMatch.Groups.[2].Value with
                    | "in" -> Inches value
                    | "cm" -> Cm value
                    | _ -> Neither

                | _ -> Neither

            else
                Neither      

        // Construct the list of predicates to be returned.
        [("byr", isDigits 1920 2002)
         ("iyr", isDigits 2010 2020)
         ("eyr", isDigits 2020 2030)
         ("hgt",
            function
            | Inches x -> (x >= 59) && (x <= 76)
            | Cm x -> (x >= 150) && (x <= 193)
            | Neither -> false)
         ("hcl", fun x -> Regex.IsMatch(x, @"^#[0-9a-f]{6}$"))
         ("ecl", fun x -> Regex.IsMatch(x, @"^(amb|blu|brn|gry|grn|hzl|oth)$"))
         ("pid", fun x -> Regex.IsMatch(x, @"^\d{9}$"))]
         |> Map.ofSeq
    
    let requiredKeys =
        requiredFields
        |> Map.toSeq
        |> Seq.map fst
        |> Set

    // This will contain only those passports with the required fields.
    let validPassports =
        let hasRequiredFields =
            Map.toSeq
            >> Seq.map fst
            >> Set
            >> Set.isSubset requiredKeys

        passportInfo
        |> Array.filter hasRequiredFields

    // Check to see if all of the fields on the passport are valid.
    let allFieldsValid passport =
        // Check to see if a particular field is valid.
        let validField field value =
            requiredFields
            |> Map.tryFind field
            |> function
                | Some pred -> pred value
                // If the field is not in the required list then we don't care about it.
                | None -> true

        passport
        |> Map.forall validField

    validPassports
    |> Array.length
    |> printfn "Part 1 answer = %i"

    validPassports
    // As above but all fields must be valid.
    |> Seq.filter allFieldsValid 
    |> Seq.length
    |> printfn "Part 2 answer = %i"

    0
