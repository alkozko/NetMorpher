#load "../../../references.fsx"
open NetMorpher.Structures.TrieMap
open NetMorpher.Structures
open StringUtils
open NetMorpher.DataTypes
open NetMorpher.Persistent


let BuildMap (lemmas: LemmasTuple list, gramemmes: GramemesTuple list, types: LemmaLinkTypeTuple list) =

    let gramemesIndex = gramemmes |> List.map (fun (gn,_,_,_) -> gn) |> List.toArray
    let gramemesInvertedIndex = gramemesIndex |> Array.indexed |> Array.map (fun (i, gn) -> gn, i) |> dict 

    let lemmasIndex = lemmas
                    |> Seq.map (fun (id, text, gr, forms, link) -> 
                                let found, lid = false, []
                                let count = if found then (lid |> Seq.length) else 0
                                let grs =  gr |> List.map (fun g -> byte gramemesInvertedIndex.[g]) |> List.toArray
                                (LemmaStruct(id, text, grs, count), forms, link))
                    |> Seq.toArray

    let lemmasInvertedIndex = lemmasIndex |> Seq.indexed
                                |> Seq.map (fun (pos, (l,_,_)) -> l.Id, pos)
                                |> dict

    for (info, _, link) in lemmasIndex do
        if link.IsSome then
            let lemma, _, _ = lemmasIndex.[lemmasInvertedIndex.[link.Value]]
            info.Parent <- lemma
                

    let wordForms = lemmasIndex
                    |> Seq.collect (fun (lemma, forms, _) -> forms |> Seq.map (fun f -> (f.NormalizeToken(), lemma)))
                    |> Seq.groupBy (fun (x, y) -> x)
            
    let map = new TrieMap<int[]>()    
    for form, lemmas in wordForms do
        let formLemmas = lemmas 
                            |> Seq.map (fun (x,y) -> y) 
                            |> Seq.sortByDescending (fun l -> l.Count) 
                            |> Seq.map (fun l -> lemmasInvertedIndex.[l.Id]) 
                            |> Seq.toArray
        map.Add (form.Reverse(), formLemmas)

    let index = lemmasIndex
                |> Seq.map (fun (l,_,_) -> l)
                |> Seq.toArray

    (map, gramemesIndex, index)         
