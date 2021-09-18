
open System
open System.IO


[<AutoOpen>]
module State =
    [<Measure>] type degrees

    type ShipState =
        | Ship of X: int * Y: int * direction: int<degrees> option

    type WaypointState =
        | Waypoint of relX: int * relY: int


[<AutoOpen>]
module Actions =

    type Action =
        | MoveNorth
        | MoveSouth
        | MoveEast
        | MoveWest
        | TurnLeft
        | TurnRight
        | MoveForward

    type Instruction =
        | Instruction of Action * extent: int

    let parseRow (row: string) =
        let instChar = row.[0]
        let extent = int row.[1..]

        let inst =
            match instChar with
            | 'N' -> MoveNorth
            | 'S' -> MoveSouth
            | 'E' -> MoveEast
            | 'W' -> MoveWest
            | 'L' -> TurnLeft
            | 'R' -> TurnRight
            | 'F' -> MoveForward
            | _ -> failwith $"Unexpected action '{instChar}'."

        Instruction (inst, extent)

    let private updateDirection current (delta: int<degrees>) =
        let adjustForNeg dir =
            if dir < 0<degrees> then 360<degrees> + (dir % 360<degrees>) else dir

        let capAt360 dir =
            dir % 360<degrees>

        current
        |> (+) delta
        |> adjustForNeg
        |> capAt360

    let private rotateWaypoint ((Waypoint (relX, relY)) as waypoint) (delta: int) =
        let rotateAnticlock90 (relX', relY') _ = (-relY', relX')
        let rotateClock90 (relX', relY') _ = (relY', -relX')

        let numApply = Math.Abs(delta) / 90

        if delta > 0 then
            Seq.fold rotateClock90 (relX, relY) [1..numApply]
            |> Waypoint
        elif delta < 0 then
            Seq.fold rotateAnticlock90 (relX, relY) [1..numApply]
            |> Waypoint
        else
            waypoint


    let private directionToPositionDelta =
        Map.ofArray [|(0<degrees>, (0, 1)); (90<degrees>, (1, 0)); (180<degrees>, (0, -1)); (270<degrees>, (-1, 0))|]

    let executeActionPart1 (Ship (x, y, direction)) (Instruction (action, extent)) =
        match direction with        
        | None -> failwith "Direction must be supplied."
        | Some _direction ->
            match action with
            | MoveNorth -> Ship (x, y + extent, direction)
            | MoveSouth -> Ship (x, y - extent, direction)
            | MoveEast -> Ship (x + extent, y, direction)
            | MoveWest -> Ship (x - extent, y, direction)
            | TurnLeft -> Ship (x, y, Some (updateDirection _direction (-extent * 1<degrees>)))
            | TurnRight -> Ship (x, y, Some (updateDirection _direction (extent * 1<degrees>)))
            | MoveForward ->
                let pos_delta =
                    directionToPositionDelta.[_direction]

                Ship (x + (fst pos_delta) * extent, y + (snd pos_delta)* extent, direction)

    let executeActionPart2
            ((Ship (x, y, direction) as ship), (Waypoint (relX, relY) as waypoint)) 
            (Instruction (action, extent)) =

        match direction with
        | Some _ -> failwith "No direction can be set."
        | None ->
            match action with
            | MoveNorth -> ship, Waypoint (relX, relY + extent)
            | MoveSouth -> ship, Waypoint (relX, relY - extent)
            | MoveEast -> ship, Waypoint (relX + extent, relY)
            | MoveWest -> ship, Waypoint (relX - extent, relY)
            | TurnLeft -> ship, (rotateWaypoint waypoint -extent)
            | TurnRight -> ship, (rotateWaypoint waypoint extent)
            | MoveForward -> Ship (x + relX * extent, y + relY * extent, None), waypoint


[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map parseRow

    let manhattanDistance (Ship (x, y, _)) =
        Math.Abs(x) + Math.Abs(y)

    inputs
    |> Array.fold executeActionPart1 (Ship (0, 0, Some 90<degrees>))
    |> manhattanDistance
    |> printfn "Part 1 answer = %i"

    inputs
    |> Array.fold executeActionPart2 ((Ship (0, 0, None)), (Waypoint (10, 1)))
    |> fst
    |> manhattanDistance
    |> printfn "Part 2 answer = %i"

    0