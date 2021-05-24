
open System
open System.IO
open System.Text.RegularExpressions
open FSharpx.Text.Regex
open FSharpx


[<ReferenceEquality>]
type Label =
    { Ingredients: Set<string>
      Allergens: Set<string> }


[<EntryPoint>]
let main argv =
    let inputs =
        let processLine line =
            let (|SplitRow|_|) =
                (|Match|_|) RegexOptions.None @"^([a-z ]+)\s+\(contains\s([a-z, ]+)\)$"

            let (ingredients, allergens) =
                match line with
                | SplitRow { GroupValues = [ingredients; allergens] } -> (ingredients, allergens)
                | _ -> failwith "Unable to parse row."

            let splitIngredients =
                String.splitChar [|' '|] ingredients
                |> Set

            let splitAllergens =
                String.splitString [|", "|] StringSplitOptions.None allergens
                |> Set

            { Ingredients = splitIngredients
              Allergens = splitAllergens }

        File.ReadAllLines "INPUTS.TXT"
        |> Array.map processLine

    let iterateSolution (labels: Label[], found: Map<string, string>) =
        let generateIntersections (labels: Label[]) =
            let intersectLabels (label1, label2) =
                { Ingredients = Set.intersect label1.Ingredients label2.Ingredients
                  Allergens = Set.intersect label1.Allergens label2.Allergens }
            let notEmpty label = label.Ingredients.Count > 0 && label.Allergens.Count > 0
            let intersectAllIngredients (allergens, labels) =
                { Ingredients = labels |> Seq.map (fun label -> label.Ingredients) |> Set.intersectMany
                  Allergens = allergens }

            labels
            |> Array.allPairs labels
            |> Array.where ((<||) (<>))
            |> Array.map intersectLabels        
            |> Array.append labels
            |> Array.groupBy (fun label -> label.Allergens)
            |> Array.map intersectAllIngredients
            |> Array.where notEmpty

        let allergenIsMapped label =
            label.Ingredients.Count = 1 && label.Allergens.Count = 1              

        let newFound =
            let getSingleAllergenAndIngredient label =
                (label.Allergens |> Seq.exactlyOne, label.Ingredients |> Seq.exactlyOne)  

            let addToCurrentFound found' (allergen, ingredient) =
                found' |> Map.add allergen ingredient

            labels
            |> Seq.where allergenIsMapped
            |> Seq.map getSingleAllergenAndIngredient
            |> Seq.fold addToCurrentFound found

        let removeFoundItems label =
            newFound
            |> Map.fold (fun label' allergen ingredient ->
                { Ingredients = label'.Ingredients |> Set.remove ingredient
                  Allergens = label'.Allergens |> Set.remove allergen }) label

        let newLabels =
            labels
            |> Array.where (not << allergenIsMapped)
            |> Array.map removeFoundItems
            |> generateIntersections

        (newLabels, newFound)   

    // Determine the allergen to ingredient mappings.
    let allergenMapping = 
        let rec generateIterations (labels, found) =
            seq {
                let result = iterateSolution (labels, found)
                yield result
                yield! generateIterations result
            }

        generateIterations (inputs, Map.empty)
        |> Seq.skipWhile (fun (labels, _) -> labels.Length > 0)
        |> Seq.head
        |> snd

    // Get a list of ingredients which correspond to allergens.
    let allergenIngredients =
        allergenMapping
        |> Map.toSeq
        |> Seq.map snd
        |> Set.ofSeq

    inputs
    // Get a contiguous list of ingredients.
    |> Seq.collect (fun label -> label.Ingredients)
    // Determine which do not correspond to an allergen.
    |> Seq.where (fun ingredient -> not (Set.contains ingredient allergenIngredients))
    // Determine how many of these there are.
    |> Seq.length
    |> printfn "Part 1 answer = %i"

    allergenMapping
    |> Map.toSeq
    |> Seq.sortBy fst
    |> Seq.map snd
    |> String.concat ","
    |> printfn "Part 2 answer = %s"

    0