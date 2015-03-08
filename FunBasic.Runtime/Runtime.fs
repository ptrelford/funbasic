module FunBasic.Runtime

open AST
open System.Reflection
open System.Collections.Generic

let private GetMemberInfo(line:string, index:int, lookup:IDictionary<string,(string * string)[]>) =
   let methodInfo (ns,name) =
      match lookup.TryGetValue(ns) with
      | true, members ->
         match members |> Array.tryFind (fun (m,_) -> name = m) with
         | Some(m,s) -> Some s
         | None -> None
      | false, _ -> None
   let rec invokeInfo = function
      | Call(name,args) -> argInfo args 
      | Method(ns,name,args) ->
         match argInfo args with
         | Some info -> Some info
         | None -> methodInfo (ns,name)
      | PropertyGet(ns,name) -> methodInfo (ns,name)
   and argInfo args =
      let arg = args |> List.tryFind(fun (_,pos) -> index>=pos.Start && index<pos.End)
      match arg with
      | Some(Func(invoke),_) -> invokeInfo invoke
      | _ -> None
   // Parse line
   match Parser.parseLine line with
   | Some(pos,instruction) when index >= pos.StartCol && index < pos.EndCol -> 
      match instruction with
      | Action(invoke) -> invokeInfo invoke        
      | PropertySet(ns,name,_) -> methodInfo (ns,name)             
      | _ -> None
   | _ -> None

let GetInfo(line:string, index:int, lookup:IDictionary<string,(string * string)[]>) =
   match GetMemberInfo(line,index,lookup) with
   | Some s -> s
   | None -> null

let Run (code, ffi, cancelToken) =
   let program = Parser.parse(code+"\r\n")
   Interpreter.run ffi program cancelToken
