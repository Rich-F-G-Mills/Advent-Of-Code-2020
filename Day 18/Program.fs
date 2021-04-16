// Learn more about F# at http://fsharp.org

open System
open System.IO
open FParsec

module Part1_Parser =
    // Part 1 grammar :-
    //      expr ::= ((expr)|number) ((+|*)expr)*

    type Op =
        | Add
        | Mul
    
    type Expression =
        | Expression of Expression * (Op * Expression) list
        | Number of int64

    let expr, exprRef = createParserForwardedToRef<Expression, Unit>()

    let number = pint64 |>> Number

    let paren = pchar '(' >>. expr .>> pchar ')'

    let non_pure_expr = (paren <|> number)

    let following_op =
        anyOf ['+'; '*']
        .>>. non_pure_expr
        |>> fun (op, _expr) ->
            match op with
            | '+' -> (Add, _expr)
            | '*' -> (Mul, _expr)
            | _ -> failwith "Unexpected error."

    do exprRef :=
        non_pure_expr
        .>>. many following_op
        |>> Expression

    let rec eval parsed =
        match parsed with
        | Expression (opening, op_list) ->
            let opening_value = eval opening

            op_list
            |> List.fold (
                fun accum next ->
                    match next with
                    | (Add, sub_exp) -> accum + (eval sub_exp)
                    | (Mul, sub_exp) -> accum * (eval sub_exp)) opening_value

        | Number n -> n

        

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (fun x -> x.Replace(" ", ""))

    let part1_parsed =
        inputs
        |> Array.map (run Part1_Parser.expr)
        |> Array.map (function | Success (r, _, _) -> r | _ -> failwith "Parsing failed.")

    part1_parsed
    |> Array.map Part1_Parser.eval
    |> Array.sum
    |> printfn "\n\nPart 1 answer = %i"

    0 // return an integer exit code
