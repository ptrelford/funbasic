module FunBasic.Runtime

open System.Reflection

let Parse(line) =
   match Parser.parseLine line with
   | Some(pos,instruction) -> instruction.ToString()
   | None -> ""

let Run (code, ffi, cancelToken) =
   // Replace While( & If(
   let program = Parser.parse(code+"\r\n")
   Interpreter.run ffi program cancelToken
