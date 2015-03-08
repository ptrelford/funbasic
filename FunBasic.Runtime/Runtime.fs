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
      | Some(arg,_) -> 
         match arg with
         | Literal _ -> None
         | Identifier _ -> None
         | GetAt(Location(_,xs)) -> argInfo xs
         | Neg(x) -> argInfo [x]
         | Arithmetic(x,_,y) | Comparison(x,_,y) | Logical(x,_,y) -> argInfo [x;y]
         | NewTuple(xs) -> argInfo xs
         | Func(invoke) -> invokeInfo invoke
      | _ -> None
   // Parse line
   match Parser.parseLine line with
   | Some(pos,instruction) when index >= pos.StartCol && index < pos.EndCol -> 
      match instruction with
      | Assign(Set(_,x)) -> argInfo [x]
      | Action(invoke) -> invokeInfo invoke
      | PropertySet(ns,name,x) -> 
         match argInfo [x] with
         | Some arg -> Some arg
         | None -> methodInfo (ns,name)
      | If x | ElseIf x | While x | Select x -> argInfo [x]
      | For(Set(_,from),``to``,step) -> argInfo [from;``to``;step]
      | _ -> None
   | _ -> None

let GetInfo(line:string, index:int, lookup:IDictionary<string,(string * string)[]>) =
   match GetMemberInfo(line,index,lookup) with
   | Some s -> s
   | None -> null

let Run (code, ffi, cancelToken) =
   let program = Parser.parse(code+"\r\n")
   Interpreter.run ffi program cancelToken
