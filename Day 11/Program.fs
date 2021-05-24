
open System.IO

// Represents the value for each cell in the grid.
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
    // Read in the starting grid.
    let startingGrid =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (Array.ofSeq >> (Array.map CellType.fromChar))  

    // Generate a matrix of grid coordinates for each cell in the grid.
    let coords =
        startingGrid
        |> Array.mapi (
            fun y row ->
                row
                |> Array.mapi (fun x _ -> (x, y)))

    // Generate the adjacent offsets.
    let surroundingDeltas =
        Array.allPairs [|-1; 0; 1|] [|-1; 0; 1|]
        |> Array.except [|(0, 0)|]

    // Get the grid size.
    let (xMax, yMax) =
        (coords |> Array.concat |> Array.map fst |> Array.max,
         coords |> Array.concat |> Array.map snd |> Array.max)

    let isWithinGrid (x, y) =
        (x >= 0) && (y >= 0) && (x <= xMax) && (y <= yMax)

    // Generate the adjacent coordinates for each cell in the grid.
    let surroundingCoords =          
        coords
        |> Array.map (
            Array.map (
                fun (x, y) ->
                    surroundingDeltas
                    |> Array.map(fun (dx, dy) -> (x + dx, y + dy))
                    |> Array.filter isWithinGrid))

    // Determine the new state for a cell at a given location (according to part 1).
    let rulePart1 grid (cell_x, cell_y) =
        let surroundingOccupied =
            surroundingCoords
            |> Array.item cell_y
            |> Array.item cell_x
            |> Array.map (fun (x, y) -> grid |> Array.item y |> Array.item x)
            |> Array.map (function | OccupiedSeat -> 1 | _ -> 0)
            |> Array.sum

        let cell =
            grid
            |> Array.item cell_y
            |> Array.item cell_x

        match cell with
        | EmptySeat when surroundingOccupied = 0 -> OccupiedSeat
        | OccupiedSeat when surroundingOccupied >= 4 -> EmptySeat
        | _ -> cell

    // Determine the new state for a cell at a given location (according to part 2).
    let rulePart2 grid (cell_x, cell_y) =
        let surroundingOccupied =
            surroundingDeltas
            |> Array.map (
                fun (dx, dy) ->
                    Seq.initInfinite id
                    |> Seq.skip 1
                    |> Seq.map (fun scalar -> (cell_x + scalar * dx, cell_y + scalar * dy))
                    |> Seq.takeWhile isWithinGrid
                    |> Seq.map (fun (sx, sy) -> grid |> Array.item sy |> Array.item sx)
                    |> Seq.tryFind (function | Floor -> false | _ -> true))
            |> Array.sumBy (function | Some OccupiedSeat -> 1 | _ -> 0)

        let cell =
            grid
            |> Array.item cell_y
            |> Array.item cell_x
                
        match cell with
        | EmptySeat when surroundingOccupied = 0 -> OccupiedSeat
        | OccupiedSeat when surroundingOccupied >= 5 -> EmptySeat
        | _ -> cell

    // A wrapper to repeatedly apply given logic to each cell in a grid.
    let rec generateIterations ruleLogic grid =
        seq {
            let result = Array.map (Array.map (ruleLogic grid)) coords
            yield result
            yield! generateIterations ruleLogic result
        }

    let iterateGridPart1 = generateIterations rulePart1
    let iterateGridPart2 = generateIterations rulePart2

    // Allows us to find the point at which the iterations become stable.
    let findStableSolution gridIterator =
        gridIterator startingGrid
        |> Seq.pairwise
        |> Seq.skipWhile ((<||) (<>))
        |> Seq.head
        |> fst
        |> Array.concat
        |> Array.sumBy (function | OccupiedSeat -> 1 | _ -> 0)
            
    findStableSolution iterateGridPart1
    |> printfn "Part 1 answer = %i"

    findStableSolution iterateGridPart2
    |> printfn "Part 2 answer = %i"

    0
