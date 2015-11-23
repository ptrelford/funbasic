// [snippet:Parser]
module FunBasic.Parser

open AST
open FParsec

type UserState = { Ends:instruction list; InFunction:bool } with
   static member Default = { Ends = []; InFunction=false }
let expectEnd ``end`` = updateUserState (fun us -> {us with Ends = ``end``::us.Ends})
let handleEnd ``end`` =
   userStateSatisfies (fun us -> not (List.isEmpty us.Ends))
   >>. userStateSatisfies (fun us ->
      let expected = us.Ends |> List.head in
      ``end`` = expected || ``end`` = End)
   >>. updateUserState (fun us -> 
      { us with
         InFunction = if ``end`` = EndFunction then false else us.InFunction 
         Ends = List.tail us.Ends
       }
   )

let pnumvalue: Parser<value, UserState> =
    let numberFormat = NumberLiteralOptions.AllowFraction
    numberLiteral numberFormat "number"
    |>> fun nl ->
            if nl.IsInteger then Int (int nl.String)
            else Double(float nl.String)

let ws = skipManySatisfy (fun c -> c = ' ' || c = '\t' || c='\r') // spaces
let str_ws s = pstringCI s .>> ws
let str_ws1 s = pstringCI s .>> spaces1

let pstring = between (pstring "\"") (pstring "\"") (manySatisfy (fun x -> x <> '"'))
let pstringvalue = pstring |>> (fun s -> String(s))

let pvalue = pnumvalue <|> pstringvalue

let pidentifier =
    let isIdentifierFirstChar c = isLetter c || c = '_'
    let isIdentifierChar c = isLetter c || isDigit c || c = '_'
    many1Satisfy2L isIdentifierFirstChar isIdentifierChar "identifier"
let pidentifier_ws = pidentifier .>> ws

let pinvoke, pinvokeimpl = createParserForwardedToRef ()
let pfunc = pinvoke |>> (fun x -> Func(x))

let plocation, plocationimpl = createParserForwardedToRef ()
let pgetat = plocation |>> (fun loc -> GetAt(loc))

let patom = 
    choice [
        pvalue |>> (fun x -> Literal(x))
        attempt pgetat;attempt pfunc
        attempt (pidentifier |>> (fun x -> Identifier(x)))
    ]

let patominfo = pipe3 getPosition patom getPosition 
                 (fun p1 e p2 -> e, {Start=int p1.Column;End=int p2.Column})

type Assoc = Associativity

let opp = new OperatorPrecedenceParser<exprInfo,_,_>()
let pterm = opp.ExpressionParser
let term = (patominfo .>> ws) <|> between (str_ws "(") (str_ws ")") pterm
opp.TermParser <- term

let addInfix name pri assoc f = 
   opp.AddOperator(
      InfixOperator(name, ws, pri, assoc, (fun (x,p1) (y,p2) -> f (x,p1) (y,p1), {Start=p1.Start;End=p2.End})))
addInfix "And" 1 Assoc.Left (fun x y -> Logical(x,And,y))
addInfix "and" 1 Assoc.Left (fun x y -> Logical(x,And,y))
addInfix "AND" 1 Assoc.Left (fun x y -> Logical(x,And,y))
addInfix "Or" 1 Assoc.Left (fun x y -> Logical(x,Or,y))
addInfix "or" 1 Assoc.Left (fun x y -> Logical(x,Or,y))
addInfix "OR" 1 Assoc.Left (fun x y -> Logical(x,Or,y))
addInfix "+" 3 Assoc.Left (fun x y -> Arithmetic(x, Add, y))
addInfix "-" 3 Assoc.Left (fun x y -> Arithmetic(x, Subtract, y))
addInfix "*" 4 Assoc.Left (fun x y -> Arithmetic(x, Multiply, y))
addInfix "/" 4 Assoc.Left (fun x y -> Arithmetic(x, Divide, y))
opp.AddOperator(PrefixOperator("-", ws, 3, true, fun (x,p) -> Neg(x,p), {Start=p.Start-1;End=p.End}))
let comparisons = ["=",Eq; "<>",Ne; "<=",Le; ">=",Ge; "<",Lt; ">",Gt]
for s,op in comparisons do
    addInfix s 2 Assoc.Left (fun x y -> Comparison(x, op, y))

let pnewtuple, pnewtupleimpl = createParserForwardedToRef ()
let pnewrecord, pnewrecordimpl = createParserForwardedToRef ()
let pnewarray, pnewarrayimpl = createParserForwardedToRef ()

let pconstruct = attempt pterm <|> attempt pnewtuple <|> attempt pnewrecord <|> attempt pnewarray
let ptupleItems = between (str_ws "(") (str_ws ")") (sepBy1 pconstruct (str_ws ","))
pnewtupleimpl :=
    pipe3 getPosition ptupleItems getPosition
      (fun p1 xs p2 -> NewTuple(xs), {Start=int p1.Column;End=int p2.Column})
let parrayItems = between (str_ws "[") (str_ws "]") (sepBy pconstruct (str_ws ","))
pnewarrayimpl :=
    pipe3 getPosition parrayItems getPosition
      (fun p1 xs p2 -> NewTuple(xs), {Start=int p1.Column;End=int p2.Column})
let pnamedItem = pstring .>> ws .>> str_ws ":" .>>. pconstruct
let precordItems = between (str_ws "{") (str_ws "}") (sepBy pnamedItem (str_ws ","))
pnewrecordimpl :=
    pipe3 getPosition precordItems getPosition
      (fun p1 xs p2 -> NewRecord xs, {Start=int p1.Column;End=int p2.Column})

let pexpr = pconstruct

let pmember = pipe3 (pidentifier_ws) (pchar '.') (pidentifier_ws) (fun tn _ mn -> tn,mn)
let pargs = between (str_ws "(") (str_ws ")") (sepBy pexpr (str_ws ","))
let pmemberinvoke =
    pipe2 pmember (opt pargs)
        (fun (tn,mn) args -> 
        match args with
        | Some args -> Method(tn, mn, args)
        | None -> PropertyGet(tn,mn)
        )
let pcall = pidentifier_ws .>>. pargs |>> (fun (name,args) -> Call(name, args))

pinvokeimpl := attempt pcall <|> attempt pmemberinvoke 

let paction = pinvoke |>> (fun x -> Action(x))
let pset = pipe3 pidentifier_ws (str_ws "=") pexpr (fun id _ e -> Set(id, e))
let passign = pipe3 pidentifier_ws (str_ws "=") pexpr (fun id _ e -> Assign(Set(id, e)))
let ppropertyset = pipe3 pmember (str_ws "=") pexpr (fun (tn,pn) _ e -> PropertySet(tn,pn,e))

let pindex = str_ws "[" >>. pexpr .>> str_ws "]"
let pindices = many1 pindex
plocationimpl := pipe2 pidentifier_ws pindices (fun id xs -> Location(id,xs))
let psetat = pipe3 plocation (str_ws "=") pexpr (fun loc _ e -> SetAt(loc, e))

let pfor =
    let pfrom = str_ws1 "For" >>. pset
    let pto = str_ws1 "To" >>. pexpr .>> expectEnd EndFor
    let pstep = str_ws1 "Step" >>. pexpr
    let toStep = function None -> (Literal(Int(1)),{Start=0;End=0}) | Some s -> s
    pipe3 pfrom pto (opt pstep) (fun f t s -> For(f, t, toStep s))
let pendfor = str_ws "EndFor" .>> handleEnd EndFor |>> (fun _ -> EndFor)

let pwhile = (attempt (str_ws1 "While" >>. pexpr)) <|> 
             (str_ws "While" >>. (between (str_ws "(") (str_ws ")") pexpr))
             .>> expectEnd EndWhile
             |>> (fun e -> While(e))
let pendwhile = str_ws "EndWhile" .>> handleEnd EndWhile |>> (fun _ -> EndWhile)

let pif = (attempt (str_ws1 "If" >>. pexpr)) <|> 
          (str_ws "If" >>. (between (str_ws "(") (str_ws ")") pexpr)) 
          .>> str_ws "Then"
          .>> expectEnd EndIf
          |>> (fun e -> If(e))
let pelseif = 
   (attempt (str_ws1 "ElseIf" >>. pexpr)) <|>
   (str_ws "ElseIf" >>. (between (str_ws "(") (str_ws ")") pexpr))
   .>> str_ws "Then" 
   |>> (fun e -> ElseIf(e))
let pelse = str_ws "Else" |>> (fun _ -> Else)
let pendif = str_ws "EndIf" .>> handleEnd EndIf |>> (fun _ -> EndIf)

let pparams = between (str_ws "(") (str_ws ")") (sepBy pidentifier_ws (str_ws ","))
let pmethod = pidentifier_ws .>>. opt pparams
              |>> (fun (name,ps) -> name, match ps with Some ps -> ps | None -> [])

let psub = str_ws1 "Sub" >>. pmethod .>> expectEnd EndSub |>> (fun (name,ps) -> Sub(name,ps))
let pendsub = str_ws "EndSub" .>> handleEnd EndSub |>> (fun _ -> EndSub)

let plabel = pidentifier_ws .>> str_ws ":" |>> (fun label -> Label(label))
let pgoto = str_ws1 "Goto" >>. pidentifier |>> (fun label -> Goto(label))

let pfunction = 
   (str_ws1 "Function" <|> str_ws1 "def") >>. pmethod
   .>> expectEnd EndFunction
   .>> updateUserState (fun us -> {us with InFunction = true})
   |>> (fun (name,ps) -> Function(name,ps))
let pendfunction = 
   str_ws "EndFunction" .>> handleEnd EndFunction 
   |>> (fun _ -> EndFunction)
let preturn = 
   attempt (str_ws1 "Return" >>. pexpr .>> userStateSatisfies (fun us -> us.InFunction) |>> (fun e -> Return (Some e)))
   <|> (str_ws "Return" .>> userStateSatisfies (fun us -> not us.InFunction) |>> (fun _ -> Return None))

let pselect = str_ws1 "Select" >>. str_ws1 "Case" >>. pexpr .>> expectEnd EndSelect
              |>> (fun e -> Select(e))

let ptuple, ptupleimpl = createParserForwardedToRef ()
let parray, parrayimpl = createParserForwardedToRef ()
let precord, precordimpl = createParserForwardedToRef ()

let prange = pvalue .>> ws .>> str_ws1 "To" .>>. pvalue |>> (fun (a,b) -> Range(a,b))
let pcomparison = choice [ for s,op in comparisons -> str_ws1 s |>> fun _ -> op]
let pis = str_ws1 "Is" >>. pcomparison .>>. pvalue |>> (fun (op,x) -> Is(op,x))
let pisequal = pvalue |>> (fun x -> Is(Eq,x))
let pany = str_ws "Else" |>> (fun _ -> Any)
let pclause = 
    attempt prange <|> attempt pis <|> attempt pisequal <|> attempt pany <|>
    attempt (ptuple |>> (fun x -> Pattern(x))) <|>
    attempt (parray |>> (fun x -> Pattern(x))) <|>
    attempt (precord |>> (fun x -> Pattern(x)))
let pcase =
    str_ws1 "Case" >>.
    sepBy (pclause .>> ws) (str_ws ",") 
    |>> (fun xs -> Case(xs))
let pendselect = str_ws "EndSelect" .>> handleEnd EndSelect |>> (fun _ -> EndSelect)

let pend = str_ws "End" .>> handleEnd End |>> (fun _ -> End)

let pbind = pidentifier_ws |>> (fun s -> Bind(s))
let ppattern =
    attempt ptuple <|> attempt precord <|> attempt parray <|>
    (attempt pclause |>> (fun c -> Clause(c))) <|>
    attempt pbind

let pnamedPattern = pstring .>> ws .>> str_ws ":" .>>. ppattern .>> ws
let precordPatterns = between (str_ws "{") (str_ws "}") (sepBy pnamedPattern (str_ws ","))
precordimpl :=
    precordPatterns
    |>> (fun xs -> Record xs)
ptupleimpl :=
    between (str_ws "(") (str_ws ")") (sepBy ppattern (str_ws ","))
    |>> (fun xs -> Tuple(xs))
parrayimpl :=
    between (str_ws "[") (str_ws "]") (sepBy ppattern (str_ws ","))
    |>> (fun xs -> Tuple(xs))

let pdeconstruct = pipe3 (ptuple <|> parray <|> precord) (str_ws "=") pexpr (fun p _ e -> Deconstruct(p,e))

let pinstruct = 
    [
        pfor;pendfor
        pwhile;pendwhile
        pif; pelseif; pelse; pendif
        pselect; pcase; pendselect
        psub; pendsub
        pfunction; pendfunction; preturn
        ppropertyset; passign; psetat; pdeconstruct
        paction
        plabel; pgoto
        pend
    ]
    |> List.map attempt
    |> choice

let toPosition (p1:Position) (p2:Position) = 
    {StartLn=int p1.Line;StartCol=int p1.Column;EndLn=int p2.Line;EndCol=int p2.Column}
type Line = Blank | Instruction of position * instruction
let pcomment = pchar '\'' >>. skipManySatisfy (fun c -> c <> '\n') >>. pchar '\n'
let peol = pcomment <|> (pchar '\n')
let pinstructpos = 
    pipe3 getPosition pinstruct getPosition (fun p1 i p2 -> toPosition p1 p2 , i)
let pinstruction = ws >>. pinstructpos .>> peol |>> (fun (pos,i) -> Instruction(pos,i))
let pblank = ws >>. peol |>> (fun _ -> Blank)

let parseExpression (text:string) =
   match runParserOnString pexpr UserState.Default "Expression" (text+"\r\n") with
   | Success ((e,info),_,_) -> Some e
   | _ -> None
let pline = attempt pinstruction <|> attempt pblank
let parseLine (line:string) =
   match runParserOnString pline UserState.Default "Line" (line+"\r\n") with
   | Success (Instruction(pos,i),_,_) -> Some (pos,i)
   | _ -> None
let plines = many pline .>> eof
let parse (program:string) =    
    match runParserOnString plines UserState.Default "Program" program with
    | Success(result, _, _)   -> 
        result 
        |> List.choose (function Instruction(pos,i) -> Some(pos,i) | Blank -> None) 
        |> List.toArray
    | Failure(errorMsg, e, s) -> failwith errorMsg
// [/snippet]