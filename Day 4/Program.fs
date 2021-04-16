// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions

[<EntryPoint>]
let main argv =    
    
    // Split the input file into the individual passports and then extract the key-value pairs.
    let passport_info =
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
    let required_fields =
        let is_digits min max (x: string) =
            match (Int32.TryParse x) with
            | (true, value) -> (value >= min) && (value <= max)
            | _ -> false

        // First attempt at using active patterns.
        let (|Inches|Cm|Neither|) (x: string) =
            let re_match = Regex.Match(x, @"^(\d{2,3})(in|cm)$")

            if re_match.Success then
                match Int32.TryParse re_match.Groups.[1].Value with
                | (true, value) ->
                    match re_match.Groups.[2].Value with
                    | "in" -> Inches value
                    | "cm" -> Cm value
                    | _ -> Neither

                | _ -> Neither

            else
                Neither      

        [("byr", is_digits 1920 2002)
         ("iyr", is_digits 2010 2020)
         ("eyr", is_digits 2020 2030)
         ("hgt",
            function
            | Inches x -> (x >= 59) && (x <= 76)
            | Cm x -> (x >= 150) && (x <= 193)
            | Neither -> false)
         ("hcl", fun x -> Regex.IsMatch(x, @"^#[0-9a-f]{6}$"))
         ("ecl", fun x -> Regex.IsMatch(x, @"^(amb|blu|brn|gry|grn|hzl|oth)$"))
         ("pid", fun x -> Regex.IsMatch(x, @"^\d{9}$"))]
         |> Map.ofSeq
    
    let required_keys =
        required_fields
        |> Map.toSeq
        |> Seq.map fst
        |> Set

    // This will contain only those passports with the required fields.
    let valid_passports =
        let has_required_fields =
            Map.toSeq
            >> Seq.map fst
            >> Set
            >> Set.isSubset required_keys

        passport_info
        |> Array.filter has_required_fields

    valid_passports
    // Retain on those elements which are true.
    |> Array.length
    |> printfn "Part 1 answer = %i"

    let all_fields_valid passport =
        let field_valid field value =
            required_fields
            |> Map.tryFind field
            |> function
                | Some pred -> pred value
                // If the field is not in the required list then we don't care about it.
                | None -> true

        passport
        |> Map.forall field_valid

    valid_passports
    |> Seq.filter all_fields_valid 
    |> Seq.length
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
