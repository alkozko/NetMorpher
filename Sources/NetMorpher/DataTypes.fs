namespace NetMorpher.DataTypes

    type LemmasTuple = int*string*string list*string list*(int option) 
    type GramemesTuple = string*string*string*string 
    type LemmaLinkTypeTuple = int*string 


namespace NetMorpher.Persistent
    open System.IO
    open MBrace.FsPickler
    open NetMorpher.Structures.TrieMap
    open NetMorpher.Structures
    open System.Collections.Generic

    module DataFolder =
        let private dataFolderPath =  __SOURCE_DIRECTORY__ + "../../../Data"
        let GetPath fileName =
            Path.Combine(dataFolderPath, fileName)

        let BinaryLoader()= 
            let binarySerializer = FsPickler.CreateBinarySerializer()

            let maps = using (File.OpenRead <| GetPath @"map.bin")
                                (fun stream -> binarySerializer.DeserializeSequence<TrieMapNode<int[]>>(stream) |> Seq.toArray)
            let index = using (File.OpenRead <| GetPath @"lemmas.bin")
                                (fun stream -> binarySerializer.DeserializeSequence<LemmaStruct>(stream) |> Seq.toArray)
            let grIndex = using (File.OpenRead <| GetPath @"grammemes.bin")
                                (fun stream -> binarySerializer.DeserializeSequence<string>(stream) |> Seq.toArray)

            let data = TrieMap(maps)

            data, grIndex, index