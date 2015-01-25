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

/// Evaluates expressions
let rec eval state (expr:expr) =
    let (vars:VarLookup), (arrays:ArrayLookup), _, _ = state
    match expr with
    | Literal x -> x
    | Identifier identifier -> vars.[identifier]
    | GetAt(Location(identifier,[index])) -> arrays.[identifier].[eval state index]
    | GetAt(Location(identifier,_)) -> raise (System.NotSupportedException())
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
    | Divide, (Int l,Int r) -> Int(l - r)
    | Divide, AsDoubles (l,r) -> Double(l - r)
    | _ -> raise (System.NotImplementedException())
and logical lhs op rhs =
    match op, lhs, rhs with
    | And, Bool l, Bool r -> Bool(l && r)
    | Or, Bool l, Bool r -> Bool(l || r)
    | _, _, _ -> raise (System.NotImplementedException())
and invoke state invoke =
    let _, _, gosub,(ffi:IFFI) = state
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
    /// Array lookup
    let arrays = ArrayLookup()
    /// For from EndFor lookup
    let forLoops = Dictionary<index, index * identifier * expr * expr>()
    /// While from EndWhile lookup
    let whileLoops = Dictionary<index, index>()
    /// Call stack for Gosubs
    let callStack = Stack<index>()
    /// Finds first index of instructions
    let findFirstIndex start (inc,dec) instructions =
        let mutable i = start
        let mutable nest = 0
        while nest > 0 || instructions |> List.exists ((=) program.[i]) |> not do 
            if inc program.[i] then nest <- nest + 1
            if nest > 0 && dec program.[i] then nest <- nest - 1
            i <- i + 1
        i
    /// Finds index of instruction
    let findIndex start (inc,dec) instruction =
        findFirstIndex start (inc,dec) [instruction]
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
    let state = variables, arrays, gosub, ffi
    /// Evaluates expression with variables
    let eval = eval state
    /// Assigns result of expression to variable
    let assign (Set(identifier,expr)) = variables.[identifier] <- eval expr
    /// Instruction step
    let step () =
        let instruction = program.[!pi]
        match instruction with
        | Assign(set) -> assign set
        | PropertySet(ns,name,x) -> ffi.PropertySet(ns,name,eval x |> toObj)
        | SetAt(Location(identifier,[index]),expr) ->
            let array = 
                match arrays.TryGetValue(identifier) with
                | true, array -> array
                | false, _ -> 
                    let array = Dictionary<value,value>()
                    arrays.Add(identifier,array)
                    array
            array.[eval index] <- eval expr
        | SetAt(Location(_,_),expr) -> raise (System.NotSupportedException())
        | Action(call) -> invoke state call |> ignore
        | If(condition) ->            
            if eval condition |> toBool |> not then
                let index = findFirstIndex (!pi+1) (isIf, isEndIf) [Else;EndIf]
                pi := index
        | Else ->
            let index = findIndex !pi (isIf,isEndIf) EndIf
            pi := index
        | ElseIf(_) -> raise (System.NotSupportedException())        
        | EndIf -> ()
        | For((Set(identifier,expr) as from), target, step) ->
            assign from
            let index = findIndex (!pi+1) (isFor,isEndFor) EndFor
            forLoops.[index] <- (!pi, identifier, target, step)
            if toInt(variables.[identifier]) > toInt(eval target) 
            then pi := index
        | EndFor ->
            let start, identifier, target, step = forLoops.[!pi]
            let x = variables.[identifier]
            variables.[identifier] <- arithmetic x Add (eval step)
            if toInt(variables.[identifier]) <= toInt(eval target) 
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
