module FunBasic.Runtime

open System.Reflection

let Run (code, ffi, cancelToken) =
   // Replace While( & If(
   let program = Parser.parse(code+"\r\n")
   Interpreter.run ffi program cancelToken
