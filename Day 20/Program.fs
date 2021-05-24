
open System
open System.IO
open System.Text.RegularExpressions
open FSharpx
open FSharpx.Text.Regex
open System.Diagnostics


// The main (non-transformed) tile type.
[<AutoOpen>]
type Tile = 
    { Id: int64
      Content: bool[,] }
    // We want to avoid creating slices.
    static member checkLeftRightCompatible (t1: Tile, t2: Tile) =
        { 0..9 }
        |> Seq.forall (fun idx -> t1.Content.[idx, 9] = t2.Content.[idx, 0])

    static member checkTopBottomCompatible (t1: Tile, t2: Tile) =
        { 0..9 }
        |> Seq.forall (fun idx -> t1.Content.[9, idx] = t2.Content.[0, idx])    


[<DebuggerDisplay("{TransformedId}")>]
type TransformedTile =
    { Tile: Tile
      TransformationName: string
      TransformedId: string }

// Defines a transformation that could be applied to a tile.
[<AutoOpen>]
type Transformation =
    { Invoke: bool[,] -> bool[,]
      Name: string }
    static member ApplyTransformation op tile =
        { Tile = { tile with Content = op.Invoke tile.Content }
          TransformationName = op.Name
          TransformedId = $"{tile.Id}[{op.Name}]" }

type TileWithLinks =
    { Tile: TransformedTile
      RightLink: TransformedTile option
      DownLink: TransformedTile option }

// This is a version of the (>>) function that can compose the above transformations.
let inline (|>>) (lhs: Transformation) (rhs: Transformation) =
    { Invoke = lhs.Invoke >> rhs.Invoke
      Name = $"{lhs.Name} -> {rhs.Name}" }
    
// Here we are defining some preset transformations that can be applied.
let rotate =
    { Invoke = fun (grid: bool [,]) ->
        let (height, width) = (Array2D.length1 grid, Array2D.length2 grid)
        
        let _rotate row col =
            grid.[height - col - 1, row]
        
        Array2D.init width height _rotate

      Name = "Rotate" }


let flip =
    { Invoke = fun (grid: bool [,]) ->
        let (height, width) = (Array2D.length1 grid, Array2D.length2 grid)
            
        let _flip row col =
            grid.[row, width - col - 1]
            
        Array2D.init width height _flip

      Name = "Flip" }


let null_op =
    { Invoke = id
      Name = "Null" }

// These are the distinct transformations that can be applied to a tile.
let uniqueOps =
    [ null_op
      rotate        
      rotate |>> rotate
      rotate |>> rotate |>> rotate
      rotate |>> flip
      rotate |>> rotate |>> flip
      flip
      flip |>> rotate ]


[<EntryPoint>]
let main args =    

    // Tiles extracted from the input file.
    let inputs =
        let extractParts =
            function
            | Match RegexOptions.None @"^Tile (\d+):([#.]+)$" matches ->
                let id =
                    int64 matches.GroupValues.[0]

                let rawContent = 
                    let symbolToBool = 
                        function
                        | '#' -> true
                        | '.' -> false
                        | _ -> failwith "Unexpected character."

                    matches.GroupValues.[1]
                    |> Seq.map symbolToBool
                    |> Seq.toArray

                let content =
                    Array2D.init 10 10 (fun row col -> rawContent.[col + row * 10])

                { Id = id; Content = content }: Tile

            | _ -> failwith "Unable to parse tile."
            |> Seq.map

        let splitOnBlankLines =
            String.splitString [|"\r\n\r\n"|] StringSplitOptions.None

        let removeNewLines =
            Seq.map (String.replace' "\r\n" "")                

        File.ReadAllText "INPUTS_TEST.TXT"
        |> splitOnBlankLines
        |> removeNewLines
        |> extractParts
        |> Seq.toList

    // Assume that the resulting overall grid is square.
    let gridDim =
        Math.Sqrt(double inputs.Length)
        |> int32

    // Take the tiles read in above and apply each of the unique transformations to each.
    let transformedTiles =
        inputs
        |> Seq.allPairs uniqueOps
        |> Seq.map (fun (op, tile) -> ApplyTransformation op tile)
        |> Seq.sortBy (fun tt -> tt.TransformedId)
        |> Seq.toArray

    // For each transformed tile, determine which ones (if any) fit to the right/below.
    let tileLinks =
        let haveDifferentIds (t1: TransformedTile, t2: TransformedTile) = t1.Tile.Id <> t2.Tile.Id

        let checkCompatible (t1: TransformedTile, t2: TransformedTile) =
            let check_lr = checkLeftRightCompatible(t1.Tile, t2.Tile)
            let check_tb = checkTopBottomCompatible(t1.Tile, t2.Tile)

            (t1, (if check_lr then Some t2 else None), (if check_tb then Some t2 else None))

        let getTransformedId ({ TransformedId = tid }, _, _) = tid

        let generateTileWithLinks (tt_key, links) =
            let tile =
                links
                |> Seq.head
                |> function | (t, _, _) -> t         

            let rightLink =
                links
                |> Seq.choose (fun (_, t, _) -> t)
                |> Seq.tryExactlyOne

            let downLink =
                links
                |> Seq.choose (fun (_, _, t) -> t)
                |> Seq.tryExactlyOne

            let tileWithLinks =
                {
                    Tile = tile
                    RightLink = rightLink
                    DownLink = downLink               
                }

            (tt_key, tileWithLinks)

        transformedTiles
        |> Seq.allPairs transformedTiles
        |> Seq.where haveDifferentIds
        |> Seq.map checkCompatible            
        |> Seq.groupBy getTransformedId
        |> Seq.map generateTileWithLinks
        |> Map.ofSeq

    // Determine the solution to part 1.
    let part1Solution =

        let initial_allocated =
            Array2D.zeroCreate<TransformedTile> gridDim gridDim

        let rec _part1_solution (x, y) (grid: TransformedTile[,]) (allocated: Set<int64>) (new_tile: TransformedTile) =

            grid.[x, y] <- new_tile
        
            let new_allocated =
                allocated
                |> Set.add new_tile.Tile.Id

            let next_xy =
                match (x, y) with
                | (x, y) when (x = gridDim-1) && (y = gridDim-1) -> None
                | (x, y) when (x = gridDim-1) -> Some (0, y+1)
                | (x, y) -> Some (x+1, y)

            match next_xy with
            | None -> Some grid
            | Some (next_x, next_y) ->

                let next_tile =
                    match (next_x, next_y) with
                    | (nx, 0) when nx > 0 ->
                        (tileLinks.[grid.[nx-1, 0].TransformedId].RightLink, tileLinks.[grid.[nx-1, 0].TransformedId].RightLink)

                    | (0, ny) when ny > 0 ->
                        (tileLinks.[grid.[0, ny-1].TransformedId].DownLink, tileLinks.[grid.[0, ny-1].TransformedId].DownLink)

                    | (nx, ny) when (nx > 0) && (ny > 0) ->
                        (tileLinks.[grid.[nx-1, ny].TransformedId].RightLink, tileLinks.[grid.[nx, ny-1].TransformedId].DownLink)

                    | _ -> failwith "Unexpected error."

                match next_tile with
                | (Some tt1, Some tt2) when tt1.TransformedId = tt2.TransformedId ->
                    if (Set.contains tt1.Tile.Id new_allocated) then
                        None
                    else
                        _part1_solution (next_x, next_y) grid new_allocated tt1

                | _ -> None

        // Take each of the transformed tiles as the starting (top-left) and find the first one that
        // is able to produce an overall grid of the required size.
        transformedTiles
        |> Seq.pick (_part1_solution (0, 0) initial_allocated Set.empty<int64>)


    let part1SolutionCorners =
        [ part1Solution.[0,0];
          part1Solution.[gridDim-1,0];
          part1Solution.[0,gridDim-1];
          part1Solution.[gridDim-1,gridDim-1] ]

    // Show the answer to part 1.
    part1SolutionCorners
    |> Seq.map (fun tt -> tt.Tile.Id)
    |> Seq.reduce (*)
    |> printfn "Part 1 answer = %i\n\n"

    // Generate the overall grid required to part 2.
    let overallGrid =
        let init =
            Array2D.zeroCreate<bool> (gridDim * 8) (gridDim * 8)

        let copySubGrid row col (tt: TransformedTile) =
            Array2D.blit tt.Tile.Content 1 1 init (row * 8) (col * 8) 8 8

        part1Solution
        |> Array2D.iteri copySubGrid         

        init

    let gridChars =
        let gridHeight = (Array2D.length1 overallGrid)
        let toGridRow row = overallGrid.[row,*]
        let boolToSymbol = function | true -> "#" | false -> "."
        let gridRowToChars slice = seq { yield! (slice |> Seq.map boolToSymbol); yield "\n" }

        Seq.init gridHeight id
        |> Seq.map toGridRow
        |> Seq.collect gridRowToChars
        |> String.Concat

    printfn "%s\n\n" gridChars

    let part2Solution =
        let overallGridHeight = overallGrid |> Array2D.length1
        let overallGridWidth = overallGrid |> Array2D.length2

        let seaMonster =
            File.ReadAllLines "SEAMONSTER.TXT"
            |> Seq.mapi (fun row slice ->
                Seq.mapi (fun col ->
                    function | '#' -> Some (row, col) | _ -> None) slice)
            |> Seq.concat
            |> Seq.choose id
            |> Seq.toArray

        let smWidth = seaMonster |> Seq.map snd |> Seq.max
        let smHeight = seaMonster |> Seq.map fst |> Seq.max
        
        let countSeaMonsters (grid: bool [,]) =           
            seq {
                for row in { 0..overallGridHeight-smHeight-1 } do
                    for col in { 0..overallGridWidth-smWidth-1 } do
                        if seaMonster |> Seq.forall (fun (relRow, relCol) -> grid.[row + relRow, col + relCol]) then
                            yield true
            } |> Seq.length

        let overallGridLayouts =
            uniqueOps
            |> Seq.map (fun op -> op.Invoke overallGrid)
            |> Seq.toArray

        let countSeaMonstersByGrid =
            overallGridLayouts
            |> Seq.map countSeaMonsters
            |> Seq.toArray

        countSeaMonstersByGrid

    part2Solution

    0