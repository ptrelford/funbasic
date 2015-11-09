module FunBasic.AST

// [snippet:Abstract Syntax Tree]
// Type abbreviations
type label = string
type identifier = string
type index = int
type HashTable<'k,'v> = System.Collections.Generic.Dictionary<'k,'v>
/// Small Basic arithmetic operation
type arithmetic = Add | Subtract | Multiply | Divide
/// Small Basic comparison operaton
type comparison = Eq | Ne | Lt | Gt | Le | Ge
/// Small Basic logical operation
type logical = And | Or
/// Small Basic value
type value =
    | Bool of bool
    | Int of int
    | Double of double
    | String of string
    | Array of HashTable<value,value>
type pattern =
    | Bind of identifier
    | Clause of clause
    | Tuple of pattern list
and clause =
    | Any
    | Is of comparison  * value
    | Range of value * value
    | Pattern of pattern
type info = {Start:int;End:int}
/// Small Basic expression
type expr =
    | Literal of value
    | Identifier of identifier
    | GetAt of location
    | Func of invoke
    | Neg of exprInfo
    | Arithmetic of exprInfo * arithmetic * exprInfo
    | Comparison of exprInfo * comparison * exprInfo
    | Logical of exprInfo * logical * exprInfo
    | NewTuple of exprInfo list // Language extension
and location =
    | Location of identifier * exprInfo list
and invoke =
    | Call of string * exprInfo list // Language extension
    | Method of string * string * exprInfo list
    | PropertyGet of string * string
and exprInfo = expr * info
type assign =
    | Set of identifier * exprInfo
/// Small Basic instruction
type instruction =
    | Assign of assign
    | Deconstruct of pattern * exprInfo // Language extension
    | SetAt of location * exprInfo
    | PropertySet of string * string * exprInfo
    | Action of invoke
    | For of assign * exprInfo * exprInfo
    | EndFor
    | If of exprInfo
    | ElseIf of exprInfo
    | Else
    | EndIf
    | While of exprInfo
    | EndWhile
    | Sub of identifier * string list
    | EndSub
    | Label of label
    | Goto of label
    // Language extensions
    | Function of identifier * string list
    | EndFunction
    | Select of exprInfo
    | Case of clause list
    | EndSelect
    | End
/// Source position info
type position = {StartLn:int;StartCol:int;EndLn:int;EndCol:int}
// [/snippet]