// Learn more about F# at http://fsharp.org

open System
open System.IO


type CellType =
    | Floor
    | EmptySeat
    | OccupiedSeat

    static member fromChar =
        function
        | '.' -> Floor
        | 'L' -> EmptySeat
        | '#' -> OccupiedSeat
        | _ -> failwith "Invalid puzzle input."


[<EntryPoint>]
let main argv =
    printfn "Starting program..."

    let starting_grid =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (Array.ofSeq >> (Array.map CellType.fromChar))  

    let coords =
        starting_grid
        |> Array.mapi (
            fun y row ->
                row
                |> Array.mapi (fun x _ -> (x, y)))

    let surrounding_deltas =
        Array.allPairs [|-1; 0; 1|] [|-1; 0; 1|]
        |> Array.except [|(0, 0)|]

    let (max_x, max_y) =
        (coords |> Array.concat |> Array.map fst |> Array.max,
         coords |> Array.concat |> Array.map snd |> Array.max) 

    let surrounding_coords =              
        coords
        |> Array.map (
            Array.map (
                fun (x, y) ->
                    surrounding_deltas
                    |> Array.map(fun (dx, dy) -> (x + dx, y + dy))
                    |> Array.filter (fun (x2, y2) -> (x2 >= 0) && (y2 >= 0) && (x2 <= max_x) && (y2 <= max_y))))


    let rule_part1 grid (cell_x, cell_y) =
        let surrounding_occupied =
            surrounding_coords
            |> Array.item cell_y
            |> Array.item cell_x
            |> Array.map (fun (x, y) -> grid |> Array.item y |> Array.item x)
            |> Array.filter(function | OccupiedSeat -> true | _ -> false)
            |> Array.length

        let cell =
            (grid |> Array.item cell_y |> Array.item cell_x)

        match cell with
        | EmptySeat when surrounding_occupied = 0 -> OccupiedSeat
        | OccupiedSeat when surrounding_occupied >= 4 -> EmptySeat
        | _ -> cell


    let rule_part2 grid (cell_x, cell_y) =
        let surrounding_occupied =
            surrounding_deltas
            |> Array.map (
                fun (dx, dy) ->
                    Seq.initInfinite id
                    |> Seq.skip 1
                    |> Seq.map (fun scalar -> (cell_x + scalar * dx, cell_y + scalar * dy))
                    |> Seq.takeWhile (fun (sx, sy) -> (sx >= 0) && (sy >= 0) && (sx <= max_x) && (sy <= max_y))
                    |> Seq.map (fun (sx, sy) -> grid |> Array.item sy |> Array.item sx)
                    |> Seq.tryFind (function | Floor -> false | _ -> true))
            |> Array.sumBy (function | Some OccupiedSeat -> 1 | _ -> 0)

        let cell =
            (grid |> Array.item cell_y |> Array.item cell_x)
                
        match cell with
        | EmptySeat when surrounding_occupied = 0 -> OccupiedSeat
        | OccupiedSeat when surrounding_occupied >= 5 -> EmptySeat
        | _ -> cell


    let iterate_grid rule_callback grid =
        let iterate_cell (cell_x, cell_y) =
            rule_callback grid (cell_x, cell_y)

        Array.map (Array.map iterate_cell) coords

    let iterate_grid_part1 = iterate_grid rule_part1
    let iterate_grid_part2 = iterate_grid rule_part2

    let find_stable grid_iterator =
        Seq.initInfinite id
        |> Seq.scan (fun prev_grid _ -> grid_iterator prev_grid) starting_grid
        |> Seq.pairwise
        |> Seq.skipWhile ((<||) (<>))
        |> Seq.head
        |> fst
        |> Array.concat
        |> Array.sumBy (function | OccupiedSeat -> 1 | _ -> 0)
            
    find_stable iterate_grid_part1
    |> printfn "Part 1 answer = %i"

    find_stable iterate_grid_part2
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
