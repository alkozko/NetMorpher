namespace NetMorpher.Analyser

open StringUtils
open NetMorpher.Prediction
open NetMorpher.MapBuilder

type NetMorpher() = 
    let path =  __SOURCE_DIRECTORY__ 
    let data, index = BinaryDataLoader.LoadAll |> TrieMapBuilder.BuildMap
    
    member this.Index = index

    member this.AnnalyzeToken (token: string) = 
        let lowerToken = token.NormalizeToken()
        let result = data.FuzzySearch <| lowerToken.Reverse()
        match result.Found with
            | true -> result.Records 
                        |> Seq.collect (fun x-> x.Record |> Seq.map (fun r -> r.GetFinal())) 
                        |> Seq.toList
            | false -> Predictor.MakePredictions result.Records result.Prefix lowerToken

    member this.LemmatizeToken token = 
        let lemmas = this.AnnalyzeToken token
        lemmas.Head.Lemma
