module Utils

open System
open System.Globalization
open System.IO

let reverseString str =
    let si = StringInfo(str)
    let teArr = Array.init si.LengthInTextElements (fun i -> si.SubstringByTextElements(i,1))
    Array.Reverse(teArr) //in-place reversal better performance than Array.rev
    String.Join("", teArr)

let normalizeString (str:string) =
    str.Replace("ё", "е")

let rec readLines (reader:StreamReader) = 
    seq {
        let rawLine = reader.ReadLine()
        match rawLine with 
            | null -> ()
            | _ -> 
                yield rawLine
                yield! readLines reader
    }