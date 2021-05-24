// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (
            // Convert the F/L and B/R characters to 0/1 respectively.
            String.map (
                function
                | 'F' | 'L' -> '0'
                | 'B' | 'R' -> '1'
                | _ -> failwith "Unexpected character."))
        // Extract the first 7 and last 3 characters and convert to integers from binary.
        |> Array.map (
            fun x -> (Convert.ToInt32(x.[..6], 2), Convert.ToInt32(x.[7..], 2)))
        // Puzzle specifies that seat ID is as per below.
        |> Array.map (fun (row, col) -> row * 8 + col)

    // Simply extract the largest seat ID.
    inputs    
    |> Seq.max
    |> printfn "Part 1 answer = %A"

    inputs
    |> Seq.sort
    |> Seq.pairwise
    // Identify where there is a gap in the seat IDs.
    |> Seq.filter (fun (x, y) -> y = x + 2)
    // There should be just one of these.
    |> Seq.exactlyOne
    // Extract the seat prior to the gap.
    |> fst
    // Add 1 to get the missing seat ID.
    |> (+) 1
    |> printfn "Part 2 answer = %A"
    
    0 // return an integer exit code
