module FunBasic.Interpreter

open AST

type CancelToken() =
   let cancelled = ref false
   member __.IsCancelled = !cancelled
   member __.Cancel() = cancelled := true

/// Foreign Function Interface
type IFFI =
   abstract MethodInvoke : ns:string * name:string * value:obj[] -> obj
   abstract PropertyGet : ns:string * name:string -> obj
   abstract PropertySet : ns:string * name:string * value:obj -> unit
   abstract EventAdd: ns:string * name:string * handler:System.EventHandler -> unit

/// Converts value to obj
let fromObj (x:obj) =
    match x with
    | :? bool as x -> Bool x
    | :? int as x -> Int x
    | :? double as x -> Double x
    | null -> Int 0
    | :? string as x -> String x
    | x -> raise (new System.NotSupportedException(x.ToString()))
/// Converts value to obj
let toObj = function
    | Bool x -> box x
    | Int x -> box x
    | Double x -> box x
    | String x -> box x
    | Array xs -> box xs
/// Converts value to int
let toInt = function
    | Int x -> x
    | Double x -> int x
    | String "" -> 0
    | String x -> int x
    | Bool _ | Array _ -> raise (new System.NotSupportedException())
let (|AsBool|_|) = function
   | "True" | "true" -> Some true
   | "False" | "false" -> Some false
   | _ -> None
/// Converts value to bool
let toBool = function
    | Bool x -> x
    | String(AsBool x) -> x
    | x -> raise (new System.NotSupportedException())
let (|AsInt|_|) s =
    if s = "" then Some 0
    else
       match System.Int32.TryParse(s) with
       | true, n -> Some n
       | false,_ -> None
let (|AsDouble|_|) s =
    if s = "" then Some 0.0
    else
       match System.Double.TryParse(s) with
       | true, n -> Some n
       | false,_ -> None
/// Coerces a tuple of numeric values to double
let (|AsDoubles|_|) = function
    | Double l, Double r -> Some(l,r)
    | Int l, Double r -> Some(double l,r)
    | Double l, Int r -> Some(l,double r)
    | String(AsDouble l), Double r -> Some(l,r)
    | Double l, String(AsDouble r) -> Some(l,r)
    | String(AsDouble l), Int r -> Some(l,double r)
    | Int l, String(AsDouble r) -> Some(double l,r)
    | _, _ -> None
/// Compares values
let rec compare lhs rhs =
    match lhs, rhs with
    | Bool l, Bool r -> l.CompareTo(r)
    | Int l, Int r -> l.CompareTo(r)
    | AsDoubles (l,r) -> l.CompareTo(r)
    | String l, String r -> l.CompareTo(r)
    | String(AsInt l), Int r -> l.CompareTo(r)
    | Int l, String(AsInt r) -> l.CompareTo(r)
    | Array l, Array r ->
       if l.Count = r.Count && 
          l.Keys |> Seq.forall(fun key ->
             match r.TryGetValue(key) with
             | true, x -> compare l.[key] x = 0
             | false, _ -> false) 
       then 0
       else -1
    | _ -> raise (new System.NotSupportedException(sprintf "%A %A" lhs rhs))

open System.Collections.Generic

type VarLookup = Dictionary<identifier,value>
type ArrayLookup = Dictionary<identifier,Dictionary<value,value>>

let comparer =
   { new IEqualityComparer<value> with
      member __.Equals(a,b) =
         compare a b = 0
      member __.GetHashCode(x) =
         (x |> toObj).ToString().GetHashCode()
   }

/// Converts string literal to array
let toArray (s:string) =   
   let xs = HashTable(comparer)
   let rec parse startIndex index =
      if index < s.Length then readKey startIndex index
   and readKey startIndex index =
      if s.[index] = ';' then parse (index+1) (index+1)
      elif s.[index] = '='
      then 
         let key = s.Substring(startIndex,index-startIndex)
         readValue key (index+1) (index+1)
      else parse startIndex (index+1)
   and readValue key startIndex index =  
      if index = s.Length || s.[index] = ';' 
      then 
         let value = s.Substring(startIndex,index-startIndex)
         match System.Int32.TryParse(key) with
         | true, n -> xs.Add(Int n,String value)
         | false,_ -> xs.Add(String key,String value)
         parse (index+1) (index+1)
      else readValue key startIndex (index+1)
   parse 0 0
   xs

let resolveArray (variables:HashTable<_,_>) name =
   match variables.TryGetValue("Array."+name) with
   | true, Array array -> array
   | true, String s -> 
      let array = toArray s
      variables.["Array."+name] <- Array array
      array
   | true, _ ->
      invalidOp "Expecting array"
   | false, _ ->
      if name.Contains(";") then
         toArray name
      else
         let array = HashTable(comparer)
         variables.Add("Array."+name,Array array)
         array

/// Obtains array for specified identifier
let obtainArray (variables:HashTable<_,_>) identifier =
   match variables.TryGetValue(identifier) with
   | true, Array array -> array
   | true, String s -> 
      let array = toArray s
      variables.[identifier] <- Array array
      array
   | true, _ ->
      invalidOp "Expecting array"
   | false, _ -> 
      let array = HashTable(comparer)
      variables.Add(identifier,Array array)
      array

/// Evaluates expressions
let rec eval state (expr:expr) =
    let (globals:VarLookup), (locals:VarLookup), _, _ = state
    match expr with
    | Literal x -> x
    | Identifier identifier -> 
       match locals.TryGetValue(identifier) with
       | true, value -> value
       | false, _ -> 
            match globals.TryGetValue(identifier) with
            | true, value -> value
            | false, _ -> String ""
    | GetAt(Location(identifier,indices)) ->
       let rec getAt (array:HashTable<_,_>) = function
          | (x,_)::[] ->
              let index = eval state x
              match array.TryGetValue(index) with
              | true, value -> value
              | false, _ -> String ""
          | (x,_)::xs ->
              let index = eval state x
              match array.TryGetValue(index) with
              | true, Array array -> getAt array xs
              | true, String s -> getAt (toArray s) xs                 
              | true, _ -> invalidOp "Expecting array"
              | false, _ -> String ""
          | _ -> invalidOp "Expecting array index"
       let array = 
          match globals.TryGetValue identifier with
          | true, Array array -> array
          | true, String s -> toArray s
          | true, _ -> invalidOp "Expecting array"
          | false, _ -> toArray ""
       getAt array indices       
    | Func(call) -> invoke state call
    | Neg(x,_) -> arithmetic (eval state x) Multiply (Int(-1))
    | Arithmetic((l,_),op,(r,_)) -> arithmetic (eval state l) op (eval state r)
    | Comparison((l,_),op,(r,_)) -> comparison (eval state l) op (eval state r) |> fromObj
    | Logical((l,_),op,(r,_)) -> logical (eval state l) op (eval state r)
    | NewTuple(xs) ->
       let table = HashTable()
       xs |> List.iteri (fun i (e,_) -> table.[Int i] <- eval state e)
       Array table      
and comparison lhs op rhs =
    let x = compare lhs rhs
    match op with
    | Eq -> x = 0   | Ne -> x <> 0
    | Lt -> x < 0   | Gt -> x > 0
    | Le -> x <= 0  | Ge -> x >= 0
and arithmetic lhs op rhs =
    match op, (lhs, rhs) with
    | Add, (Int l,Int r) -> Int(l + r)
    | Add, (String(AsInt l), Int r) -> Int(l + r)
    | Add, (Int l, String(AsInt r)) -> Int(l + r)
    | Add, (String(AsInt l), String(AsInt r)) -> Int(l + r)
    | Add, AsDoubles (l,r) -> Double(l + r)    
    | Add, (String(AsDouble l), String(AsDouble r)) -> Double(l+r)
    | Add, (String l, String r) -> String(l + r)
    | Add, (String l, r) -> String(l + (r |> toObj).ToString())
    | Add, (l, String r) -> String((l |> toObj).ToString() + r)
    | Subtract, (Int l,Int r) -> Int(l - r)
    | Subtract, AsDoubles (l,r) -> Double(l - r)
    | Subtract, (String(AsInt l), Int r) -> Int(l - r)
    | Subtract, (Int l, String(AsInt r)) -> Int(l - r)
    | Subtract, (String(AsInt l), String(AsInt r)) -> Int(l - r)
    | Subtract, (String(AsDouble l), String(AsDouble r)) -> Double(l - r)
    | Multiply, (Int l,Int r) -> Int(l * r)
    | Multiply, AsDoubles (l,r) -> Double(l * r)
    | Multiply, (String(AsInt l), Int r) -> Int(l * r)
    | Multiply, (Int l, String(AsInt r)) -> Int(l * r)
    | Multiply, (String(AsInt l), String(AsInt r)) -> Int(l * r)
    | Divide, (Int l,Int 0) -> Int(0)
    | Divide, (Int l,Int r) -> Double(double l / double r)
    | Divide, AsDoubles (l,r) when r = 0.0 -> Double(0.0)
    | Divide, AsDoubles (l,r) -> Double(l / r)
    | Divide, (String(AsInt l), Int r) -> Double(double l / double r)
    | Divide, (Int l, String(AsInt r)) -> Double(double l / double r)
    | Divide, (String(AsInt l), String(AsInt r)) -> Double(double l / double r)
    | _ -> raise (System.NotImplementedException(sprintf "%A %A %A" lhs op rhs))
and logical lhs op rhs =
    match op, lhs, rhs with
    | And, Bool l, Bool r -> Bool(l && r)
    | And, String(AsBool l), Bool r -> Bool(l && r)
    | And, Bool l, String(AsBool r) -> Bool(l && r)
    | And, String(AsBool l), String(AsBool r) -> Bool(l && r)
    | Or, Bool l, Bool r -> Bool(l || r)
    | Or, String(AsBool l), Bool r -> Bool(true || r)
    | Or, Bool l, String(AsBool r) -> Bool(true || r)
    | Or, String(AsBool l), String(AsBool r) -> Bool(true || r)
    | _, _, _ -> raise (System.NotImplementedException())
and invoke state invoke =
    let globals,locals,call,(ffi:IFFI) = state
    match invoke with
    | Call(name,args) ->
        let returnValue = call (name,[for (arg,_) in args -> eval state arg])
        match returnValue with
        | Some value -> value
        | None ->
            match globals.TryGetValue(name) with
            | true, value -> value
            | false, _ -> String ""
    | Method("Array","GetValue", [name,_; index]) ->
        let name = eval state name
        match name with
        | String name ->
            eval state (GetAt(Location("Array."+name, [index])))        
        | _ -> invalidOp "Expecting array name"
    | Method("Array","SetValue",[name,_;index,_;value,_]) ->
        let name = eval state name
        let array =
            match name with
            | String name -> resolveArray globals name
            | Array array -> array
            | _ -> invalidOp "Expecting array name"
        let index = eval state index
        array.[index] <- eval state value
        String ""
    | Method("Array","RemoveValue",[name,_;index,_]) ->
        let name = eval state name
        let array =
            match name with
            | String name -> resolveArray globals name
            | Array array -> array
            | _ -> invalidOp "Expecting array name"
        array.Remove(eval state index) |> ignore
        String ""
    | Method("Array", "GetItemCount", [name,_]) ->
        let name = eval state name
        let array =
            match name with
            | String name -> resolveArray globals name
            | Array array -> array
            | _ -> invalidOp "Expecting array name"
        Int array.Count
    | Method("Array", "GetAllIndices", [name,_]) ->
        let name = eval state name
        let array =
            match name with
            | String name -> resolveArray globals name
            | Array array -> array
            | _ -> invalidOp "Expecting array name"
        let keys = array.Keys |> Seq.mapi (fun i k -> Int (i+1),k)
        let indices = HashTable<_,_>()
        for i, k in keys do indices.Add(i,k)
        Array indices
    | Method(ns,name,args) ->
        let args = [for (arg,_) in args -> eval state arg |> toObj]
        ffi.MethodInvoke(ns,name,args |> List.toArray)
        |> fromObj
    | PropertyGet(ns,name) ->
        ffi.PropertyGet(ns,name) |> fromObj       

open System.Threading

/// Runs program
let rec runWith (ffi:IFFI) (program:instruction[]) pc globals locals (token:CancelToken) (countdown:CountdownEvent) =
    /// Program index
    let pi = ref pc
    /// For from EndFor lookup
    let forLoops = Dictionary<index, index * identifier * expr * expr>()
    /// While from EndWhile lookup
    let whileLoops = Dictionary<index, index>()
    /// Return value
    let returnValue = ref None

    /// Finds first index of instructions
    let findFirstIndex start (inc,dec) condition =
        let mutable i = start
        let mutable nest = 0
        while nest > 0 || condition program.[i] |> not do 
            if inc program.[i] then nest <- nest + 1
            if nest > 0 && dec program.[i] then nest <- nest - 1
            i <- i + 1
        i
    /// Finds index of instruction
    let findIndex start (inc,dec) (instruction:instruction) =
        findFirstIndex start (inc,dec) ((=)instruction)
    let isIf = function If(_) -> true | _ -> false
    let isEndIf = (=) EndIf
    let isFor = function For(_,_,_) -> true | _ -> false
    let isEndFor = (=) EndFor
    let isWhile = function While(_) -> true | _ -> false
    let isEndWhile = (=) EndWhile
    let isSelect = function Select _ -> true | _ -> false
    let isEndSelect = (=) EndSelect
    let isFalse _ = false    
    let findSubIndex (identifier) =
        let ignoreCase = System.StringComparison.OrdinalIgnoreCase
        let condition = function
           | Sub(name,_) | Function(name,_)
               when System.String.Compare(name,identifier,ignoreCase) = 0 -> true
           | _ -> false
        findFirstIndex 0 (isFalse, isFalse) condition
    let call (identifier, args:value list) : value option =
        let index = findSubIndex identifier
        let ps =
           match program.[index] with
           | Sub(_,ps) | Function(_,ps) -> ps
           | _ -> invalidOp "Expecting sub or function"
        let locals = VarLookup()
        let xs = List.zip ps args
        for (p,arg) in xs do
            locals.[p] <- arg
        runWith ffi program (index+1) globals locals token countdown
    /// Current state
    let state () = globals, locals, call, ffi
    /// Evaluates expression with variables
    let eval = eval (state ())
    /// Assigns result of expression to variable
    let assign (Set(identifier,(expr,_))) =
        globals.[identifier] <- eval expr
    /// Obtains sub array from specified array
    let obtainSubArray (array:HashTable<_,_>) key =
        match array.TryGetValue(key) with
        | true, Array array -> array
        | true, String s -> 
            let newArray = toArray s
            array.[key] <- Array newArray
            newArray
        | _, _ ->
           let newArray = HashTable(comparer)
           array.[key] <- Array newArray
           newArray
    /// Creates event handler for specified sub name
    let toEventHandler name =
       let index = findSubIndex name
       fun _ _ ->
           countdown.TryAddCount() |> ignore
           try                 
              try
                 runWith ffi program (index+1) globals locals token countdown
                 |> ignore
              with e ->
                  System.Diagnostics.Debug.WriteLine(e.Message)
                  token.Cancel()                        
           finally
               try countdown.Signal() |> ignore
               with e -> System.Diagnostics.Debug.WriteLine(e.Message)
    /// Instruction step
    let step () =
        let instruction = program.[!pi]
        match instruction with
        | Assign(set) -> 
            assign set
        | PropertySet(ns,name,(expr,_)) ->
            match expr with
            | Identifier sub when not(globals.ContainsKey(sub)) ->
               let handler = toEventHandler sub
               ffi.EventAdd(ns,name, System.EventHandler(handler))
            | _ ->
               ffi.PropertySet(ns,name,eval expr |> toObj)       
        | SetAt(Location(identifier,indices),(expr,_)) ->
            let rec setAt (array:HashTable<_,_>) = function
               | (x,_)::[] -> array.[eval x] <- eval expr
               | (x,_)::xs -> 
                  let key = eval x
                  let subArray = obtainSubArray array key                  
                  setAt subArray xs
               | [] -> invalidOp "Expecting array index"
            let array = obtainArray globals identifier            
            setAt array indices
        | Action(Call(name,args)) -> call (name, [for (arg,_) in args -> eval arg]) |> ignore    
        | Action(call) -> invoke (state()) call |> ignore
        | If(condition,_) ->
            let rec check condition =
               if eval condition |> toBool |> not then
                  let next = function Else | ElseIf(_) | EndIf -> true | _ -> false
                  let index = findFirstIndex (!pi+1) (isIf, isEndIf) next
                  pi := index
                  match program.[index] with
                  | ElseIf(condition,_) -> check condition
                  | _ -> ()
            check condition
        | Else | ElseIf(_) ->
            let index = findIndex !pi (isIf,isEndIf) EndIf
            pi := index
        | EndIf -> ()
        | For((Set(identifier,(expr,_)) as from), (target,_), (step,_)) ->
            assign from
            let index = findIndex (!pi+1) (isFor,isEndFor) EndFor
            forLoops.[index] <- (!pi, identifier, target, step)
            let a = toInt(globals.[identifier])
            let b = toInt(eval target)
            let step = toInt(eval step)
            if (step >= 0 && a > b) || (step < 0 && a < b)
            then pi := index
        | EndFor ->
            let start, identifier, target, step = forLoops.[!pi]
            let x = globals.[identifier]            
            let step = eval step
            globals.[identifier] <- arithmetic x Add step
            let a = toInt(globals.[identifier])
            let b =  toInt(eval target) 
            let step = toInt(step)
            if (step >= 0 && a <= b) || (step < 0 && a >= b)
            then pi := start
        | While(condition,_) ->
            let index = findIndex (!pi+1) (isWhile,isEndWhile) EndWhile
            whileLoops.[index] <- !pi 
            if eval condition |> toBool |> not then pi := index
        | EndWhile ->
            pi := whileLoops.[!pi] - 1
        | Sub(identifier, ps) ->
            pi := findIndex (!pi+1) (isFalse, isFalse) EndSub
        | Function(name,ps) ->
            pi := findIndex (!pi+1) (isFalse, isFalse) EndFunction
        | EndSub | EndFunction ->
            pi := program.Length
        | Return(e) ->            
            returnValue := e |> Option.map (eval << fst)
            pi := program.Length
        | Label(label) -> ()
        | Goto(label) -> pi := findIndex 0 (isFalse,isFalse) (Label(label))
        // Language Extensions
        | Deconstruct(pattern, (e,_)) ->
            let rec deconstruct e = function
               | Bind("_") -> ()
               | Bind(name) -> globals.[name] <- e                  
               | Clause(_) -> raise (System.NotImplementedException())
               | Tuple(xs) ->
                    let table =
                       match e with
                       | Array ar -> ar
                       | _ -> invalidOp "Expecting tuple"
                    xs |> List.iteri (fun i x ->
                       let item = table.[Int i]
                       deconstruct item x
                    )
            deconstruct (eval e) pattern
        | Select(e,_) ->
            let rec check value = function
               | Any -> true
               | Is(op,x) -> comparison value op x
               | Range(from,until) -> 
                  comparison value Ge from && comparison value Le until
               | Pattern(Tuple(patterns)) ->
                  checkTuple patterns value       
               | Pattern(_) -> failwith "Not supported"
            and checkTuple patterns = function
               | Array table when table.Count = patterns.Length ->
                  let items = [for i in 0..(patterns.Length-1) -> table.[Int i]]
                  List.forall2 checkItem items patterns
               | _ -> false 
            and checkItem value = function
               | Bind("_") -> true
               | Bind( _ ) -> failwith "Not supported"
               | Clause(clause) -> check value clause
               | Tuple(patterns) -> checkTuple patterns value
            let next = function Case _ | EndSelect -> true | _ -> false
            let value = eval e
            let rec tryNext () =
               let index = findFirstIndex (!pi+1) (isSelect,isEndSelect) next
               pi := index
               match program.[index] with              
               | Case(clauses) -> if clauses |> List.forall (check value) |> not then tryNext()                                
               | _ -> ()
            tryNext ()                   
        | Case(clauses) ->
            let index = findIndex !pi (isSelect,isEndSelect) EndSelect
            pi := index            
        | EndSelect -> ()
        | End -> invalidOp "For internal use only"
    while not token.IsCancelled && !pi < program.Length do step (); incr pi
    !returnValue

let inferEnds program =
   let stack = Stack()
   let push x = stack.Push x
   let pop () = 
      if stack.Count > 0 then stack.Pop()
      else invalidOp "Too many end statements"
   let mutable current = None
   for i = 0 to (Array.length program)-1 do
      let info, instruction = program.[i]
      match instruction with
      | For(_) -> push EndFor
      | While(_) -> push EndWhile
      | If(_) -> push EndIf
      | Sub(_) -> push EndSub
      | Function(_) -> push EndFunction
      | Select(_) -> push EndSelect
      | EndFor | EndWhile | EndIf | EndSub | EndFunction | EndSelect ->
         if instruction <> pop () then invalidOp "Mismatch on end statement"       
      | End -> program.[i] <- (info, pop ())
      | _ -> ()
   if stack.Count > 0 then invalidOp "Missing end statement"

let run ffi program token =
   inferEnds program
   let globals = VarLookup(System.StringComparer.OrdinalIgnoreCase)
   globals.["true"] <- Bool true
   globals.["false"] <- Bool false
   let locals = VarLookup(System.StringComparer.OrdinalIgnoreCase)
   let lines, program = program |> Array.unzip
   let countdown = new CountdownEvent(1)  
   runWith ffi program 0 globals locals token countdown |> ignore
   if token.IsCancelled then
      countdown.Signal() |> ignore
      countdown.Wait(1000) |> ignore
   

