
open System.IO
open System.Text.RegularExpressions
open System.Diagnostics

// Does 'chr' appear the required number of times within the password?
let isValid1 (min, max, chr, pwd) =
    let count =
        pwd
        |> Seq.filter ((=) chr)
        |> Seq.length

    (count >= min) && (count <= max)

// Exactly one of min or max must contain the required character.
let isValid2 (min, max, chr, pwd: string) =
    (pwd.[min-1] = chr) <> (pwd.[max-1] = chr)

[<EntryPoint>]
let main argv =
    let reMatch = Regex "^(\d+)-(\d+)\s(\w):\s(\w+)$"

    // Convert the matches into the required types.
    let convert (elem: string array) =
        (int elem.[0], int elem.[1], char elem.[2], elem.[3])

    // Apply the regex match against each line in the input.
    let constraints =
        File.ReadAllLines "Inputs.txt"
        
    // Takes in a list of constraints and splits out into a tuple.
    let processConstraints constraints =
        constraints
        |> Array.map reMatch.Match
        |> Array.map (
            fun x ->
                x.Groups.Values
                |> Seq.skip 1
                |> Seq.map (fun y -> y.Value)
                |> Array.ofSeq)
        |> Array.map convert

    // Convert the individual matches into a tuple array of the required type.
    let elems =
        processConstraints constraints      

    elems
    |> Array.filter isValid1
    |> Array.length
    |> printfn "Answer part 1 = %i"

    elems
    |> Array.filter isValid2
    |> Array.length
    |> printfn "Answer part 2 = %i"

    0 // return an integer exit code
