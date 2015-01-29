module FunBasic.Interpreter

open AST

/// Foreign Function Interface
type IFFI =
   abstract MethodInvoke : string * string * obj[] -> obj
   abstract PropertyGet : string * string -> obj
   abstract PropertySet : string * string * obj -> unit

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
    | String x -> int x
    | Bool _ | Array _ -> raise (new System.NotSupportedException())
/// Converts value to bool
let toBool = function
    | Bool x -> x
    | _ -> raise (new System.NotSupportedException())
/// Coerces a tuple of numeric values to double
let (|AsDoubles|_|) = function
    | Double l, Double r -> Some(l,r)
    | Int l, Double r -> Some(double l,r)
    | Double l, Int r -> Some(l,double r)
    | _, _ -> None
/// Compares values
let compare lhs rhs =
    match lhs, rhs with
    | Bool l, Bool r -> l.CompareTo(r)
    | Int l, Int r -> l.CompareTo(r)
    | AsDoubles (l,r) -> l.CompareTo(r)
    | String l, String r -> l.CompareTo(r)
    | _ -> raise (new System.NotSupportedException(sprintf "%A %A" lhs rhs))

open System.Collections.Generic

type VarLookup = Dictionary<identifier,value>
type ArrayLookup = Dictionary<identifier,Dictionary<value,value>>

/// Converts string literal to array
let toArray (s:string) =   
   let xs = HashTable()
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

/// Evaluates expressions
let rec eval state (expr:expr) =
    let (vars:VarLookup), _, _ = state
    match expr with
    | Literal x -> x
    | Identifier identifier -> vars.[identifier]
    | GetAt(Location(identifier,indices)) ->
       let rec getAt (array:HashTable<_,_>) = function
          | x::[] -> array.[eval state x]
          | x::xs ->
              match array.[eval state x] with
              | Array array -> getAt array xs
              | String s -> getAt (toArray s) xs                 
              | _ -> invalidOp "Expecting array"
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
    | Subtract, (Int l,Int r) -> Int(l - r)
    | Subtract, AsDoubles (l,r) -> Double(l - r)
    | Multiply, (Int l,Int r) -> Int(l * r)
    | Multiply, AsDoubles (l,r) -> Double(l * r)
    | Divide, (Int l,Int r) -> Int(l / r)
    | Divide, AsDoubles (l,r) -> Double(l / r)
    | _ -> raise (System.NotImplementedException())
and logical lhs op rhs =
    match op, lhs, rhs with
    | And, Bool l, Bool r -> Bool(l && r)
    | Or, Bool l, Bool r -> Bool(l || r)
    | _, _, _ -> raise (System.NotImplementedException())
and invoke state invoke =
    let _, gosub,(ffi:IFFI) = state
    match invoke with
    | Call(name,args) -> gosub (name(*,args*)) |> fromObj
    | Method(ns,name,args) ->
        let args = args |> List.map (eval state >> toObj)
        ffi.MethodInvoke(ns,name,args |> List.toArray)
        |> fromObj
    | PropertyGet(ns,name) ->
        ffi.PropertyGet(ns,name) |> fromObj       
/// Runs program
let run (ffi:IFFI) (program:instruction[]) =
    /// Program index
    let pi = ref 0
    /// Variable lookup   
    let variables = VarLookup()
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
    let gosub (identifier) : obj =      
        let index = findIndex 0 (isFalse, isFalse) (Sub(identifier,[]))
        callStack.Push(!pi)
        pi := index
        null
    /// Current state
    let state = variables, gosub, ffi
    /// Evaluates expression with variables
    let eval = eval state
    /// Assigns result of expression to variable
    let assign (Set(identifier,expr)) = variables.[identifier] <- eval expr
    /// Obtains array for specified identifier
    let obtainArray identifier =
        match variables.TryGetValue(identifier) with
        | true, Array array -> array
        | true, String s -> toArray s
        | _, _ -> 
            let array = Dictionary<value,value>()
            variables.Add(identifier,Array array)
            array
    /// Obtains sub array from specified array
    let obtainSubArray (array:HashTable<_,_>) key =
        match array.TryGetValue(key) with
        | true, Array array -> array
        | true, String s -> toArray s
        | _, _ ->
           let newArray = HashTable()
           array.[key] <- Array newArray
           newArray
    /// Instruction step
    let step () =
        let instruction = program.[!pi]
        match instruction with
        | Assign(set) -> assign set
        | PropertySet(ns,name,x) -> ffi.PropertySet(ns,name,eval x |> toObj)       
        | SetAt(Location(identifier,indices),expr) ->
            let rec setAt (array:HashTable<_,_>) = function
               | x::[] -> array.[eval x] <- eval expr
               | x::xs -> 
                  let key = eval x
                  let array = obtainSubArray array key
                  setAt array xs
               | [] -> invalidOp "Expecting array index"
            let array = obtainArray identifier            
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
            pi := callStack.Pop()
        | Label(label) -> ()
        | Goto(label) -> pi := findIndex 0 (isFalse,isFalse) (Label(label))
        // Extensions
        | Deconstruct(pattern, e) -> raise (System.NotSupportedException())        
        | Function(name,ps) -> raise (System.NotSupportedException())
        | EndFunction -> raise (System.NotSupportedException())
        | Select(e) -> raise (System.NotSupportedException())
        | Case(clauses) -> raise (System.NotSupportedException())
        | EndSelect -> raise (System.NotSupportedException())
    while !pi < program.Length do step (); incr pi
