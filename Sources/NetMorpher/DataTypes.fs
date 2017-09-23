namespace NetMorpher.DataTypes

    type LemmasTuple = int*string*string list*string list*(int option) 
    type GramemesTuple = string*string*string*string 
    type LemmaLinkTypeTuple = int*string 


namespace NetMorpher.Persistent
    open System.IO

    module DataFolder =
        let private dataFolderPath =  __SOURCE_DIRECTORY__ + "../../Data"
        let GetPath fileName =
            Path.Combine(dataFolderPath, fileName)

        module Data =
            let lemmas = GetPath @"dict.opcorpora.lemmas.bin"
            let grammemes = GetPath @"dict.opcorpora.gramemes.bin"
            let types = GetPath @"dict.opcorpora.linktypes.bin"
