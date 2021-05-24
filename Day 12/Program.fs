
open System
open System.IO

type ShipState = Ship of x: int * y: int * direction: int option

type WaypointState = Waypoint of rel_x: int * rel_y: int

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

    type Instruction = Instruction of Action * extent: int

    let ParseRow (row: string) =
        let inst_chr = row.[0]
        let extent = int row.[1..]

        let inst =
            match inst_chr with
            | 'N' -> MoveNorth
            | 'S' -> MoveSouth
            | 'E' -> MoveEast
            | 'W' -> MoveWest
            | 'L' -> TurnLeft
            | 'R' -> TurnRight
            | 'F' -> MoveForward
            | _ -> failwith $"Unexpected action '{inst_chr}'."

        Instruction (inst, extent)

    let UpdateDirection current delta =
        let adjust_for_neg dir =
            if dir < 0 then 360 + (dir % 360) else dir

        let cap_at_360 dir =
            dir % 360

        current
        |> (+) delta
        |> adjust_for_neg
        |> cap_at_360

    let RotateWaypoint ((Waypoint (rel_x, rel_y)) as waypoint) (delta: int) =
        let rotate_anticlock_90 (_rel_x, _rel_y) _ = (-_rel_y, _rel_x)
        let rotate_clock_90 (_rel_x, _rel_y) _ = (_rel_y, -_rel_x)

        let num_apply = Math.Abs(delta) / 90

        if delta > 0 then
            Seq.fold rotate_clock_90 (rel_x, rel_y) [1..num_apply]
            |> Waypoint
        elif delta < 0 then
            Seq.fold rotate_anticlock_90 (rel_x, rel_y) [1..num_apply]
            |> Waypoint
        else
            waypoint


    let direction_to_pos_deltas =
        Map.ofArray [|(0, (0, 1)); (90, (1, 0)); (180, (0, -1)); (270, (-1, 0))|]


    let ExecuteAction_Part1 (Ship (x, y, direction)) (Instruction (action, extent)) =
        match direction with        
        | None -> failwith "Direction must be supplied."
        | Some _direction ->
            match action with
            | MoveNorth -> Ship (x, y + extent, direction)
            | MoveSouth -> Ship (x, y - extent, direction)
            | MoveEast -> Ship (x + extent, y, direction)
            | MoveWest -> Ship (x - extent, y, direction)
            | TurnLeft -> Ship (x, y, Some (UpdateDirection _direction -extent))
            | TurnRight -> Ship (x, y, Some (UpdateDirection _direction extent))
            | MoveForward ->
                let pos_delta =
                    direction_to_pos_deltas.[_direction]

                Ship (x + (fst pos_delta) * extent, y + (snd pos_delta)* extent, direction)

    let ExecuteAction_Part2
            ((Ship (x, y, direction) as ship), (Waypoint (rel_x, rel_y) as waypoint)) 
            (Instruction (action, extent)) =

        match direction with
        | Some _ -> failwith "No direction can be set."
        | None ->
            match action with
            | MoveNorth -> ship, Waypoint (rel_x, rel_y + extent)
            | MoveSouth -> ship, Waypoint (rel_x, rel_y - extent)
            | MoveEast -> ship, Waypoint (rel_x + extent, rel_y)
            | MoveWest -> ship, Waypoint (rel_x - extent, rel_y)
            | TurnLeft -> ship, (RotateWaypoint waypoint -extent)
            | TurnRight -> ship, (RotateWaypoint waypoint extent)
            | MoveForward -> Ship (x + rel_x * extent, y + rel_y * extent, None), waypoint

let ManhattanDistance (Ship (x, y, _)) =
    Math.Abs(x) + Math.Abs(y)


[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map Actions.ParseRow

    inputs
    |> Array.fold Actions.ExecuteAction_Part1 (Ship (0, 0, Some 90))
    |> ManhattanDistance
    |> printfn "Part 1 answer = %i"

    inputs
    |> Array.fold Actions.ExecuteAction_Part2 ((Ship (0, 0, None)), (Waypoint (10, 1)))
    |> function | Ship (_, _, _) as ship, _ -> ship
    |> ManhattanDistance
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
