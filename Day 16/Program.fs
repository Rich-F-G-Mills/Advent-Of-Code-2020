
open System
open System.IO
open FSharpx
open FSharpx.Text
open FSharpx.Text.Regex

type Constraint =
    { fieldName: string
      validValues: int64 Set }

[<EntryPoint>]
let main argv =
    let inputs =
        let splitOnBlankLines =
            String.splitString [|"\r\n\r\n"|] StringSplitOptions.None

        let splitOnNewLines =
            String.splitString [|"\r\n"|] StringSplitOptions.None

        let parts =
            File.ReadAllText "INPUTS.TXT"
            |> splitOnBlankLines
        
        let constraints =
            parts.[0]
            |> splitOnNewLines
            |> Seq.map (tryMatch @"^([a-z ]+):\s(\d+)-(\d+)\sor\s(\d+)-(\d+)$")
            |> Seq.map (function
                | Some { GroupValues = [ fieldName; start1; end1; start2; end2 ] } ->
                    let validValues =
                        seq {
                            yield! { int64 start1..int64 end1 }
                            yield! { int64 start2..int64 end2 }
                        } |> Set

                    { fieldName = fieldName
                      validValues = validValues }

                | _ -> failwith "Unable to parse constraint.")
            |> Seq.toArray

        let currentTicket =              
            parts.[1]
            |> tryMatch @"^your\sticket:\r\n([0-9,]+)$"
            |> function
                | Some { GroupValues = [fieldValueLine] } ->
                    fieldValueLine
                    |> String.splitChar [|','|]
                    |> Array.map int64
                | _ -> failwith "Unable to parse current ticket."

        let nearByTickets =
            parts.[2]
            |> tryMatch @"^nearby\stickets:\r\n([0-9,\r\n]+)$"
            |> function
                | Some { GroupValues = [nearbyTicketsBlock] } ->
                    nearbyTicketsBlock
                    |> splitOnNewLines
                    |> Array.map (String.splitChar [|','|])
                    |> Array.map (Array.map int64)
                | _ -> failwith "Unable to parse nearby tickets."

        {| constraints = constraints; currentTicket = currentTicket; nearbyTickets = nearByTickets |}

    let validValues =
        inputs.constraints
        |> Seq.map (fun c -> c.validValues)
        |> Set.unionMany

    let validTickets, invalidTickets =
        inputs.nearbyTickets
        |> Array.partition (fun vs -> Set.difference (Set vs) validValues |> Set.isEmpty)
            
    invalidTickets
    |> Seq.concat
    |> Seq.where (not << validValues.Contains)
    |> Seq.sum
    |> printfn "Part 1 answer = %i"

    let getPossibleFields value =
        inputs.constraints
        |> Seq.where (fun c -> c.validValues.Contains value)
        |> Seq.map (fun c -> c.fieldName)
        |> Set

    let rec assignFields (possibilities: string Set array) =
        possibilities
        |> Array.exists (fun p -> p.Count > 1)
        |> function
            | true ->
                let found =
                    possibilities
                    |> Seq.where (fun p -> p.Count = 1)
                    |> Set.unionMany

                possibilities
                |> Array.map (fun p -> if p.Count > 1 then Set.difference p found else p)
                |> assignFields

            | false ->
                possibilities
                |> Array.map (Seq.exactlyOne)

    let assignedFields =
        validTickets
        |> Array.transpose
        |> Array.map (Seq.map getPossibleFields >> Set.intersectMany)
        |> assignFields
    
    assignedFields
    |> Seq.zip inputs.currentTicket
    |> Seq.where (snd >> String.startsWith "departure")
    |> Seq.map fst
    |> Seq.reduce (*)
    |> printfn "Part 2 answer = %i"

    0