// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (
            String.map (
                function
                | 'F' | 'L' -> '0'
                | 'B' | 'R' -> '1'
                | _ -> failwith "Unexpected character."))
        |> Array.map (
            fun x -> (Convert.ToInt32(x.[..6], 2), Convert.ToInt32(x.[7..], 2)))
        |> Array.map (fun (row, col) -> row * 8 + col)

    inputs    
    |> Array.max
    |> printfn "Part 1 answer = %A"

    inputs
    |> Array.sort
    |> Array.pairwise
    |> Array.filter (fun (x, y) -> y = x + 2)
    |> Array.map (fun (x, _) -> x + 1)
    |> Array.exactlyOne
    |> printfn "Part 2 answer = %A"
    
    0 // return an integer exit code
