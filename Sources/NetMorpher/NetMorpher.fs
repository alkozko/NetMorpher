namespace NetMorpher.Analyser

open System.Collections.Generic
open StringUtils
open NetMorpher.Prediction
open NetMorpher.Structures.TrieMap
open NetMorpher.Structures

type NetMorpher(data:TrieMap<int[]>, grIndex: string[], index: LemmaStruct[]) = 

    static member Load(loader) =
        let data = loader()
        NetMorpher(data) 
  
    member this.AnnalyzeToken (token: string) = 
        let lowerToken = token.NormalizeToken()
        let result = data.FuzzySearch <| lowerToken.Reverse()
        let records = result.Records |> Array.map (fun r -> r.Prefix, (r.Record |> Array.map (fun r -> index.[r])))
        let struc = match result.Found with
                        | true -> records 
                                    |> Seq.collect (fun (x,r) -> r |> Seq.map (fun r -> r.GetFinal())) 
                                    |> Seq.toList
                        | false -> Predictor.MakePredictions records result.Prefix lowerToken
        struc |> List.map (fun s -> s.CreateInfo(grIndex))

    member this.LemmatizeToken token = 
        let lemmas = this.AnnalyzeToken token
        lemmas.Head.Lemma