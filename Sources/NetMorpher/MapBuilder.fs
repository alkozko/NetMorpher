namespace NetMorpher.MapBuilder

open System.IO
open NetMorpher.Structures.TrieMap
open NetMorpher.Types
open StringUtils
open MBrace.FsPickler
open NetMorpher.DataTypes
open NetMorpher.Persistent

module BinaryDataLoader =

    let loadAll = 
        let binarySerializer = FsPickler.CreateBinarySerializer()

        let lemmas = using (File.OpenRead <| DataFolder.Data.lemmas)
                            (fun stream -> binarySerializer.DeserializeSequence<LemmasTuple>(stream) |> Seq.toList)
        let gramemmes = using (File.OpenRead <| DataFolder.Data.grammemes)
                            (fun stream -> binarySerializer.DeserializeSequence<GramemesTuple>(stream) |> Seq.toList)
        let types = using (File.OpenRead <| DataFolder.Data.types)
                            (fun stream -> binarySerializer.DeserializeSequence<LemmaLinkTypeTuple>(stream) |> Seq.toList)

        lemmas, gramemmes, types

module TrieMapBuilder =
    let buildMap (lemmas: LemmasTuple list, gramemmes: GramemesTuple list, types: LemmaLinkTypeTuple list) =
        let lemmasDict = lemmas
                        |> Seq.map (fun (id, text, gr, forms, link) -> 
                                    let found, lid = false, []
                                    let count = if found then (lid |> Seq.length) else 0
                                    (id, (LemmaInfo(id, text, gr |> List.toArray, count), forms, link)))
                        |> dict

        for (info, _, link) in lemmasDict.Values do
            if link.IsSome then
                let lemma, _, _ = lemmasDict.[link.Value]
                info.Parent <- Some lemma
                    

        let wordForms = lemmasDict.Values
                        |> Seq.collect (fun (lemma, forms, _) -> forms |> Seq.map (fun f -> (f.NormalizeToken(), lemma)))
                        |> Seq.groupBy (fun (x, y) -> x)
                
        let map = new TrieMap<(LemmaInfo) list>()    
        for form, lemmas in wordForms do
            let formLemmas = (lemmas |> Seq.map (fun (x,y) -> y) |> Seq.sortByDescending (fun l -> l.Count) |> Seq.toList)
            map.Add (form.Reverse(), formLemmas)

        let index = lemmasDict.Values
                    |> Seq.map ((fun (l,_,_) -> l) >> (fun l -> (l.Id, l)))
                    |> dict

        (map, index)                