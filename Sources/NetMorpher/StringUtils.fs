module StringUtils

open System
open System.Text
open System.Globalization
open System.IO


type String with 
    member str.Reverse() =
        let si = StringInfo(str)
        let teArr = Array.init si.LengthInTextElements (fun i -> si.SubstringByTextElements(i,1))
        Array.Reverse(teArr) //in-place reversal better performance than Array.rev
        String.Join("", teArr)

    member str.NormalizeToken() =
        str .ToLowerInvariant()
            .Replace("ё", "е")