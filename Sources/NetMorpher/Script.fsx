#load "../../references.fsx"
open NetMorpher.Analyser


let morpher = NetMorpher()

printf "%s" <| morpher.LemmatizeToken "хомякового"
