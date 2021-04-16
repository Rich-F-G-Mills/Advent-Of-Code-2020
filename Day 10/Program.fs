// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map int

    let device_rating =
        inputs
        |> Array.max
        |> (+) 3

    inputs
    |> Array.append [|0; device_rating|]
    |> Array.sortDescending
    |> Array.pairwise
    |> Array.countBy ((<||) (-))
    |> Array.map snd
    |> Array.reduce (*)
    |> printfn "Part 1 answer = %A"

    let rec find_layouts accrued last_added remaining =
        match remaining with
        | [] -> failwith "Unexpected error."
        | [r] ->
            if r <= (last_added + 3) then 1L else 0L
        | r::rs ->
            if r <= (last_added + 3) then
                (find_layouts accrued last_added rs) + (find_layouts (r::accrued) r rs)
            else 0L                

    let groupings =
        inputs
        |> Array.append [|0; device_rating|]
        |> Array.sort
        |> Array.pairwise
        |> Array.map (fun (x, y) -> (y, y - x))
        |> Array.rev
        |> Array.scan (fun (_, x) (y, z) -> (y, x + if z = 3 then 1 else 0)) (0, 0)
        |> Array.tail
        |> Array.rev
        |> Array.groupBy snd
        |> Array.map (snd >> (Array.map fst) >> Array.toList)

    let prior_closes =
        groupings
        |> Array.map (List.last)
        |> Array.append [|0|]
        |> Array.take (Array.length groupings)

    groupings
    |> Array.zip prior_closes
    |> Array.map ((<||) (find_layouts []))
    |> Array.reduce (*)
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
