module FunBasic.Runtime

open System.Reflection

let Run (code, ffi) =
   let program = Parser.parse(code+"\r\n") |> Array.unzip |> snd
   Interpreter.run ffi program
