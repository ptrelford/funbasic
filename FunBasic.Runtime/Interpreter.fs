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
/// Converts value to bool
let toBool = function
    | Bool x -> x
    | String "False" -> false
    | String "True" -> true
    | x -> raise (new System.NotSupportedException())
let (|AsInt|_|) s =
    if s = "" then Some 0
    else
       match System.Int32.TryParse(s) with
       | true, n -> Some n
       | false,_ -> None
/// Coerces a tuple of numeric values to double
let (|AsDoubles|_|) = function
    | Double l, Double r -> Some(l,r)
    | Int l, Double r -> Some(double l,r)
    | Double l, Int r -> Some(l,double r)
    | String(AsInt l), Double r -> Some(double l,r)
    | Double l, String(AsInt r) -> Some(l, double r)
    | _, _ -> None
/// Compares values
let compare lhs rhs =
    match lhs, rhs with
    | Bool l, Bool r -> l.CompareTo(r)
    | Int l, Int r -> l.CompareTo(r)
    | AsDoubles (l,r) -> l.CompareTo(r)
    | String l, String r -> l.CompareTo(r)
    | String(AsInt l), Int r -> l.CompareTo(r)
    | Int l, String(AsInt r) -> l.CompareTo(r)
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

/// Obtains array for specified identifier
let obtainArray (variables:HashTable<_,_>) identifier =
   match variables.TryGetValue(identifier) with
   | true, Array array -> array
   | true, String s -> 
      let array = toArray s
      variables.[identifier] <- Array array
      array
   | _, _ -> 
      let array = HashTable(comparer)
      variables.Add(identifier,Array array)
      array

/// Evaluates expressions
let rec eval state (expr:expr) =
    let (vars:VarLookup), _, _ = state
    match expr with
    | Literal x -> x
    | Identifier identifier -> 
       match vars.TryGetValue(identifier) with
       | true, value -> value
       | false, _ -> invalidOp (identifier+" not defined")
    | GetAt(Location(identifier,indices)) ->
       let rec getAt (array:HashTable<_,_>) = function
          | x::[] ->
              let index = eval state x
              match array.TryGetValue(index) with
              | true, value -> value
              | false, _ -> String ""
          | x::xs ->
              let index = eval state x
              match array.TryGetValue(index) with
              | true, Array array -> getAt array xs
              | true, String s -> getAt (toArray s) xs                 
              | true, _ -> invalidOp "Expecting array"
              | false, _ -> String ""
          | _ -> invalidOp "Expecting array index"
       let array = 
         match vars.TryGetValue identifier with
         | true, Array array -> array
         | true, String s -> toArray s
         | _, _ -> invalidOp "Not found"
       getAt array indices       
    | Func(call) -> invoke state call
    | Neg x -> arithmetic (eval state x) Multiply (Int(-1))
    | Arithmetic(l,op,r) -> arithmetic (eval state l) op (eval state r)
    | Comparison(l,op,r) -> comparison (eval state l) op (eval state r)
    | Logical(l,op,r) -> logical (eval state l) op (eval state r)
    | NewTuple(xs) -> raise (System.NotSupportedException())
and comparison lhs op rhs =
    let x = compare lhs rhs
    match op with
    | Eq -> x = 0   | Ne -> x <> 0
    | Lt -> x < 0   | Gt -> x > 0
    | Le -> x <= 0  | Ge -> x >= 0
    |> fromObj
and arithmetic lhs op rhs =
    match op, (lhs, rhs) with
    | Add, (Int l,Int r) -> Int(l + r)
    | Add, AsDoubles (l,r) -> Double(l + r)
    | Add, (String l, String r) -> String(l + r)
    | Add, (String(AsInt l), Int r) -> Int(l + r)
    | Add, (Int l, String(AsInt r)) -> Int(l + r)
    | Add, (String l, Int r) -> String(l + r.ToString())
    | Subtract, (Int l,Int r) -> Int(l - r)
    | Subtract, AsDoubles (l,r) -> Double(l - r)
    | Subtract, (String(AsInt l), Int r) -> Int(l - r)
    | Subtract, (Int l, String(AsInt r)) -> Int(l - r)
    | Multiply, (Int l,Int r) -> Int(l * r)
    | Multiply, AsDoubles (l,r) -> Double(l * r)
    | Multiply, (String(AsInt l), Int r) -> Int(l * r)
    | Multiply, (Int l, String(AsInt r)) -> Int(l * r)
    | Divide, (Int l,Int r) -> Int(l / r)
    | Divide, AsDoubles (l,r) -> Double(l / r)
    | Divide, (String(AsInt l), Int r) -> Int(l / r)
    | Divide, (Int l, String(AsInt r)) -> Int(l / r)
    | _ -> raise (System.NotImplementedException(sprintf "%A %A %A" lhs op rhs))
and logical lhs op rhs =
    match op, lhs, rhs with
    | And, Bool l, Bool r -> Bool(l && r)
    | Or, Bool l, Bool r -> Bool(l || r)
    | _, _, _ -> raise (System.NotImplementedException())
and invoke state invoke =
    let vars,gosub,(ffi:IFFI) = state
    match invoke with
    | Call(name,args) -> gosub (name(*,args*)) |> fromObj
    | Method("Array","GetValue", [name; index]) ->
        let name = eval state name
        match name with
        | String name ->
            eval state (GetAt(Location(name, [index])))
        | _ -> invalidOp "Expecting array name"
    | Method("Array","SetValue",[name;index;value]) ->
        let name = eval state name
        match name with
        | String name ->
            let array = obtainArray vars name
            let index = eval state index
            array.[index] <- eval state value        
            String ""
         | _ -> invalidOp "Expecting array name"
    | Method(ns,name,args) ->
        let args = args |> List.map (eval state >> toObj)
        ffi.MethodInvoke(ns,name,args |> List.toArray)
        |> fromObj
    | PropertyGet(ns,name) ->
        ffi.PropertyGet(ns,name) |> fromObj       

open System.Threading

/// Runs program
let rec runWith (ffi:IFFI) (program:instruction[]) pc vars (token:CancelToken) (countdown:CountdownEvent) =
    /// Global Variable lookup   
    let variables = vars
    /// Program index
    let pi = ref pc
    /// For from EndFor lookup
    let forLoops = Dictionary<index, index * identifier * expr * expr>()
    /// While from EndWhile lookup
    let whileLoops = Dictionary<index, index>()
    /// Call stack for Gosubs
    let callStack = Stack<index>()

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
    let isFalse _ = false
    let findSub (identifier) =
        findIndex 0 (isFalse, isFalse) (Sub(identifier,[]))
    let gosub (identifier) : obj =
        let index = findSub identifier
        callStack.Push(!pi)
        pi := index
        null
    /// Current state
    let state = variables, gosub, ffi
    /// Evaluates expression with variables
    let eval = eval state
    /// Assigns result of expression to variable
    let assign (Set(identifier,expr)) =
        variables.[identifier] <- eval expr
    /// Obtains sub array from specified array
    let obtainSubArray (array:HashTable<_,_>) key =
        match array.TryGetValue(key) with
        | true, Array array -> array
        | true, String s -> toArray s
        | _, _ ->
           let newArray = HashTable(comparer)
           array.[key] <- Array newArray
           newArray
    /// Instruction step
    let step () =
        let instruction = program.[!pi]
        match instruction with
        | Assign(set) -> 
            assign set
        | PropertySet(ns,name,expr) ->
            match expr with
            | Identifier s when not(variables.ContainsKey(s)) ->
               let index = findSub s
               let handler _ _ =
                  countdown.TryAddCount() |> ignore
                  try                 
                     try     
                        runWith ffi program (index+1) vars token countdown
                     with e ->
                        System.Diagnostics.Debug.WriteLine(e.Message)
                        token.Cancel()                        
                  finally
                     try countdown.Signal() |> ignore
                     with e -> System.Diagnostics.Debug.WriteLine(e.Message)
               ffi.EventAdd(ns,name, System.EventHandler(handler))
            | _ ->
               ffi.PropertySet(ns,name,eval expr |> toObj)       
        | SetAt(Location(identifier,indices),expr) ->
            let rec setAt (array:HashTable<_,_>) = function
               | x::[] -> array.[eval x] <- eval expr
               | x::xs -> 
                  let key = eval x
                  let array = obtainSubArray array key
                  setAt array xs
               | [] -> invalidOp "Expecting array index"
            let array = obtainArray vars identifier            
            setAt array indices
        | Action(call) -> invoke state call |> ignore
        | If(condition) ->
            let rec check condition =
               if eval condition |> toBool |> not then
                  let next = function Else | ElseIf(_) | EndIf -> true | _ -> false
                  let index = findFirstIndex (!pi+1) (isIf, isEndIf) next
                  pi := index
                  match program.[index] with
                  | ElseIf(condition) -> check condition
                  | _ -> ()
            check condition
        | Else | ElseIf(_) ->
            let index = findIndex !pi (isIf,isEndIf) EndIf
            pi := index
        | EndIf -> ()
        | For((Set(identifier,expr) as from), target, step) ->
            assign from
            let index = findIndex (!pi+1) (isFor,isEndFor) EndFor
            forLoops.[index] <- (!pi, identifier, target, step)
            let a = toInt(variables.[identifier])
            let b = toInt(eval target)
            let step = toInt(eval step)
            if (step >= 0 && a > b) || (step < 0 && a < b)
            then pi := index
        | EndFor ->
            let start, identifier, target, step = forLoops.[!pi]
            let x = variables.[identifier]            
            let step = eval step
            variables.[identifier] <- arithmetic x Add step
            let a = toInt(variables.[identifier])
            let b =  toInt(eval target) 
            let step = toInt(step)
            if (step >= 0 && a <= b) || (step < 0 && a >= b)
            then pi := start
        | While condition ->
            let index = findIndex (!pi+1) (isWhile,isEndWhile) EndWhile
            whileLoops.[index] <- !pi 
            if eval condition |> toBool |> not then pi := index
        | EndWhile ->
            pi := whileLoops.[!pi] - 1
        | Sub(identifier, ps) ->
            pi := findIndex (!pi+1) (isFalse, isFalse) EndSub
        | EndSub ->
            pi := 
               if callStack.Count > 0 
               then callStack.Pop() 
               else program.Length
        | Label(label) -> ()
        | Goto(label) -> pi := findIndex 0 (isFalse,isFalse) (Label(label))
        // Language Extensions
        | Deconstruct(pattern, e) -> raise (System.NotSupportedException())        
        | Function(name,ps) -> raise (System.NotSupportedException())
        | EndFunction -> raise (System.NotSupportedException())
        | Select(e) -> raise (System.NotSupportedException())
        | Case(clauses) -> raise (System.NotSupportedException())
        | EndSelect -> raise (System.NotSupportedException())    
    while not token.IsCancelled && !pi < program.Length do step (); incr pi

let run ffi program token =
   let vars = VarLookup()
   let lines, program = program |> Array.unzip
   let countdown = new CountdownEvent(1)  
   let cancelled = runWith ffi program 0 vars token countdown
   countdown.Signal() |> ignore
   countdown.Wait(1000) |> ignore
   

