
open System.IO

[<EntryPoint>]
let main argv =

    // Read in all numbers from the puzzle input.
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> List.ofArray
        |> List.map int
        |> List.sort

    // We take each number within 'nums', and then check to see if there is another number further on that adds to the required target.
    // Although the use of the 'option' type isn't really needed, it was a good opportunity to try it out.
    let rec find1 target nums =
        match nums with
        // If we have a list with one or no items... We have a problem!
        | [] | [_] -> None
        | n::ns ->
            // Find the next number, that when added, is greater than or equal to the target.
            // We could have used tryFind but that would scan the entire remaining list.
            // We only need to go as far as the point at which we equal/exceed the target.
            let found = 
                ns
                |> Seq.skipWhile ((>) (target - n))
                |> Seq.tryHead

            // Check to see if we have reached the target.
            match found with
            | Some x when n + x = target -> Some (n, x)
            | _ -> find1 target ns

    // We can simply re-use to logic above.
    let rec find2 target nums =
        match nums with
        | [] | [_] -> None
        | n::ns ->
            match find1 (target - n) nums with
            | Some (x,y) -> Some (n,x,y)
            | None -> find2 target ns

                   
    inputs
    |> find1 2020
    |> function
    | Some (x,y) -> printfn "Answer to part 1 = %i" (x*y)
    | _ -> printfn "No answer found to part 1."


    inputs
    |> find2 2020
    |> function
    | Some (x,y,z) -> printfn "Answer to part 2 = %i" (x*y*z)
    | _ -> printfn "No answer found to part 2."

    0 // return an integer exit code
