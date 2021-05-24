
open System.IO

[<EntryPoint>]
let main argv =

    // Convert the input grid into a 2D ragged array.
    let grid =        
        File.ReadAllLines "Inputs.txt"
        |> Array.map (Seq.map (function | '#' -> true | _ -> false) >> Array.ofSeq)

    let gridWidth, gridHeight =
        (Array.length grid.[0]), (Array.length grid)

    // Count the number of trees encountered when starting in the top left and moving dx/dy each time.
    let trees_encountered (dx, dy) =          
        let trees_at_location (x, y) =
            if grid.[y].[x] then 1 else 0

        Seq.initInfinite (fun idx -> ((idx * dx) % gridWidth, idx * dy))
        |> Seq.takeWhile (fun (_, y) -> y < gridHeight)
        |> Seq.sumBy trees_at_location

    trees_encountered (3, 1)
    |> printfn "Part 1 answer = %i"

    [(1,1); (3,1); (5,1); (7,1); (1,2)]
    |> Seq.map trees_encountered
    |> Seq.reduce (*)
    |> printfn "Part 2 answer = %A"  

    0 // return an integer exit code
