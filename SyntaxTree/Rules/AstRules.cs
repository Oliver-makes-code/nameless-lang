using Lang.Tokenize;

namespace Lang.SyntaxTree.Rules;

public static class AstRules {
    public static readonly Matcher Null = new NullMatcher("Null");

    public static readonly Matcher Start = new ListMatcher("Start", () => [
        ExpressionRules.Expression,
        TokenType.Eof
    ]);

    public static readonly Matcher Type = new OrMatcher("Type", () => [
        TokenType.Keyword.Var,
        TokenType.Keyword.I8,
        TokenType.Keyword.I16,
        TokenType.Keyword.I32,
        TokenType.Keyword.I64,
        TokenType.Keyword.Ui8,
        TokenType.Keyword.Ui16,
        TokenType.Keyword.Ui32,
        TokenType.Keyword.Ui64,
        TokenType.Keyword.F32,
        TokenType.Keyword.F64,
        TokenType.Identifier.TypeMatcher
    ]);
}

public static class PatternRules {
    public static readonly Matcher Pattern = new OrMatcher("Pattern", () => [
        Array,
        Variable,
        Discard
    ]);
    
    public static readonly Matcher Variable = StatementRules.PartialDeclarator;
    public static readonly Matcher Discard = TokenType.Keyword.Discard;

    public static readonly Matcher Array = new OrMatcher("Array", () => [
        ArrayEmpty,
        ArrayFull,
        ArrayImpliedAll,
        ArrayImpliedStart,
        ArrayImpliedMiddle,
        ArrayImpliedEnd
    ]);

    public static readonly Matcher ArrayPattern = new ListMatcher("ArrayPattern", () => [
        Pattern,
        TokenType.Symbol.Comma
    ]);

    public static readonly Matcher ArrayPatternList = new ListMatcher("ArrayPatternList", () => [
        new OptionalMatcher("ArrayPatternRepeatOpt", () => new RepeatingMatcher("ArrayPatternRepeat", () => ArrayPattern)),
        Pattern
    ]);

    public static readonly Matcher ArrayImplied = new ListMatcher("ArrayImplied", () => [
        TokenType.Symbol.Elipsis,
        Pattern
    ]);

    public static readonly Matcher ArrayEmpty = new ListMatcher("ArrayEmpty", () => [
        TokenType.Symbol.BracketOpen,
        TokenType.Symbol.BracketClose
    ]);

    public static readonly Matcher ArrayFull = new ListMatcher("ArrayFull", () => [
        TokenType.Symbol.BracketOpen,
        ArrayPatternList,
        TokenType.Symbol.BracketClose
    ]);

    public static readonly Matcher ArrayImpliedEnd = new ListMatcher("ArrayImpliedEnd", () => [
        TokenType.Symbol.BracketOpen,
        ArrayPatternList,
        TokenType.Symbol.Comma,
        ArrayImplied,
        TokenType.Symbol.BracketClose
    ]);

    public static readonly Matcher ArrayImpliedAll = new ListMatcher("ArrayImpliedAll", () => [
        TokenType.Symbol.BracketOpen,
        ArrayImplied,
        TokenType.Symbol.BracketClose
    ]);

    public static readonly Matcher ArrayImpliedMiddle = new ListMatcher("ArrayImpliedMiddle", () => [
        TokenType.Symbol.BracketOpen,
        ArrayPatternList,
        TokenType.Symbol.Comma,
        ArrayImplied,
        TokenType.Symbol.Comma,
        ArrayPatternList,
        TokenType.Symbol.BracketClose
    ]);

    public static readonly Matcher ArrayImpliedStart = new ListMatcher("ArrayImpliedStart", () => [
        TokenType.Symbol.BracketOpen,
        ArrayImplied,
        TokenType.Symbol.Comma,
        ArrayPatternList,
        TokenType.Symbol.BracketClose
    ]);
}

public static class StatementRules {
    public static readonly Matcher PartialDeclarator = new ListMatcher("PartialDeclarator", () => [
        AstRules.Type,
        TokenType.Identifier.TypeMatcher
    ]);
    public static readonly Matcher FullDeclarator = new ListMatcher("FullDeclarator", () => [
        PartialDeclarator,
        TokenType.Symbol.Equal,
        ExpressionRules.Expression
    ]);

    public static readonly Matcher Return = new ListMatcher("Return", () => [
        TokenType.Keyword.Return,
        new OptionalMatcher("ReturnValue", () => ExpressionRules.Expression)
    ]);

    public static readonly Matcher SemicolonTerminated = new OrMatcher("SemicolonTerminated", () => [
        ExpressionRules.Expression,
        Return,
        TokenType.Keyword.Break
    ]);

    public static readonly Matcher SemicolonlessStatement = new OrMatcher("SemicolonlessStatement", () => [
        While,
        Loop,
        ForIn,
        For,
        SemicolonTerminated,
    ]);

    public static readonly Matcher Statement = new OrMatcher("Statement", () => [
        While,
        Loop,
        ForIn,
        For,
        MatchElse,
        new ListMatcher("Line", () => [
            SemicolonTerminated,
            TokenType.Symbol.Semicolon
        ])
    ]);

    public static readonly Matcher MatchElse = new ListMatcher("MatchElse", () => [
        MatchExpr,
        TokenType.Keyword.Else,
        Block
    ]);

    public static readonly Matcher Pattern = new OrMatcher("Pattern", () => [
        TokenType.Keyword.Discard
    ]);

    public static readonly Matcher MatchArm = new ListMatcher("MatchArm", () => [
        Pattern,
        TokenType.Symbol.Arrow,
        Statement
    ]);

    public static readonly Matcher MatchSingle = new ListMatcher("MatchSingle", () => [
        ExpressionRules.Expression,
        TokenType.Symbol.Arrow,
        Pattern
    ]);

    public static readonly Matcher For = new ListMatcher("For", () => [
        TokenType.Keyword.For,
        FullDeclarator,
        TokenType.Symbol.Comma,
        ExpressionRules.Expression,
        TokenType.Symbol.Comma,
        SemicolonlessStatement,
        Block
    ]);

    public static readonly Matcher ForIn = new ListMatcher("ForIn", () => [
        TokenType.Keyword.For,
        TokenType.Identifier.TypeMatcher,
        TokenType.Keyword.In,
        ExpressionRules.Expression,
        Block
    ]);

    public static readonly Matcher MatchExpr = new ListMatcher("MatchExpr", () => [
        TokenType.Keyword.Match,
        MatchSingle,
    ]);

    public static readonly Matcher MatchOrExpr = new OrMatcher("MatchOrExpr", () => [
        MatchExpr,
        ExpressionRules.Expression
    ]);

    public static readonly Matcher While = new ListMatcher("While", () => [
        TokenType.Keyword.While,
        MatchOrExpr,
        Block
    ]);

    public static readonly Matcher Loop = new ListMatcher("Loop", () => [
        TokenType.Keyword.Loop,
        Block
    ]);

    public static readonly Matcher If = new ListMatcher("If", () => [
        IfSingle,
        ElseIfRepeat,
        new OptionalMatcher("Else", () => ElseSingle)
    ]);

    public static readonly Matcher ElseSingle = new ListMatcher("ElseSingle", () => [
        TokenType.Keyword.Else,
        Block
    ]);

    public static readonly Matcher ElseIfSingle = new ListMatcher("ElseIfSingle", () => [
        TokenType.Keyword.Else,
        IfSingle
    ]);

    public static readonly Matcher ElseIfRepeat = new RepeatingMatcher("ElseIfRepeat", () => ElseIfSingle);

    public static readonly Matcher IfSingle = new ListMatcher("IfSingle", () => [
        TokenType.Keyword.If,
        MatchOrExpr,
        Block
    ]);

    public static readonly Matcher Block = new ListMatcher("Block", () => [
        TokenType.Symbol.BraceOpen,
        // TODO
        TokenType.Symbol.BraceClose
    ]);
}

public static class ExpressionRules {
    public static readonly Matcher Parenthesis = new ListMatcher("Parenthesis", () => [
        TokenType.Symbol.ParenOpen,
        Expression!,
        TokenType.Symbol.ParenClose
    ]);

    public static readonly Matcher Value = new OrMatcher("Value", () => [
        TokenType.Number<double>.TypeMatcher,
        TokenType.Number<long>.TypeMatcher,
        TokenType.String.TypeMatcher,
        TokenType.Identifier.TypeMatcher,
        Parenthesis
    ]);

    public static readonly Matcher ObjectReference = new ListMatcher("ObjectPath", () => [
        TokenType.Symbol.Dot,
        TokenType.Identifier.TypeMatcher,
    ]);

    public static readonly Matcher ValueInvoke = new ListMatcher("ValueInvoke", () => [
        Value,
        new OptionalMatcher("Optional", () => new RepeatingMatcher("ObjectInvoke", () => FuncAccess))
    ]);

    public static readonly Matcher FuncAccess = new OrMatcher("FuncAccess", () => [
        FuncArgs,
        ObjectReference
    ]);

    public static readonly Matcher Expression = OperatorRules.BinPrecedence1;

    public static readonly Matcher FuncArgs = new ListMatcher("FuncArgs", () => [
        TokenType.Symbol.ParenOpen,
        new OptionalMatcher("Inner", () => FunArgsInner),
        TokenType.Symbol.ParenClose
    ]);

    public static readonly Matcher FunArgsInner = new ListMatcher("FuncArgsInner", () => [
        new OptionalMatcher("OptionalArgs", () => new RepeatingMatcher("FuncParams", () => new ListMatcher("FuncParam", () => [
            Expression,
            TokenType.Symbol.Comma
        ]))),
        Expression
    ]);
}

public static class OperatorRules {
    public static readonly Matcher BinPrecedence1 = new ListMatcher("BinPrecedence1", () => [
        BinPrecedence2,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence1Ops,
            BinPrecedence2
        ])))
    ]);

    public static readonly Matcher BinPrecedence2 = new ListMatcher("BinPrecedence2", () => [
        BinPrecedence3,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence2Ops,
            BinPrecedence3
        ])))
    ]);

    public static readonly Matcher BinPrecedence3 = new ListMatcher("BinPrecedence3", () => [
        BinPrecedence4,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence3Ops,
            BinPrecedence4
        ])))
    ]);

    public static readonly Matcher BinPrecedence4 = new ListMatcher("BinPrecedence4", () => [
        BinPrecedence5,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence4Ops,
            BinPrecedence5
        ])))
    ]);

    public static readonly Matcher BinPrecedence5 = new ListMatcher("BinPrecedence5", () => [
        BinPrecedence6,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence5Ops,
            BinPrecedence6
        ])))
    ]);

    public static readonly Matcher BinPrecedence6 = new ListMatcher("BinPrecedence6", () => [
        BinPrecedence7,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence6Ops,
            BinPrecedence7
        ])))
    ]);

    public static readonly Matcher BinPrecedence7 = new ListMatcher("BinPrecedence7", () => [
        BinPrecedence8,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence7Ops,
            BinPrecedence8
        ])))
    ]);

    public static readonly Matcher BinPrecedence8 = new ListMatcher("BinPrecedence8", () => [
        BinPrecedence9,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence8Ops,
            BinPrecedence9
        ])))
    ]);

    public static readonly Matcher BinPrecedence9 = new ListMatcher("BinPrecedence9", () => [
        BinPrecedence10,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence9Ops,
            BinPrecedence10
        ])))
    ]);

    public static readonly Matcher BinPrecedence10 = new ListMatcher("BinPrecedence10", () => [
        BinPrecedence11,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence10Ops,
            BinPrecedence11
        ])))
    ]);

    public static readonly Matcher BinPrecedence11 = new ListMatcher("BinPrecedence11", () => [
        ExpressionRules.ValueInvoke,
        new OptionalMatcher("Opt", () => new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence11Ops,
            ExpressionRules.ValueInvoke
        ])))
    ]);

    public static readonly Matcher BinPrecedence1Ops = new OrMatcher("BinPrecedence1Ops", () => [
        TokenType.Symbol.Add,
        TokenType.Symbol.Sub,
    ]);

    public static readonly Matcher BinPrecedence2Ops = new OrMatcher("BinPrecedence2Ops", () => [
        TokenType.Symbol.Mul,
        TokenType.Symbol.Div,
        TokenType.Symbol.Mod,
    ]);

    public static readonly Matcher BinPrecedence3Ops = new OrMatcher("BinPrecedence3Ops", () => [
        TokenType.Symbol.Shl,
        TokenType.Symbol.Shr,
    ]);

    public static readonly Matcher BinPrecedence4Ops = new OrMatcher("BinPrecedence4Ops", () => [
        TokenType.Symbol.GreaterEqual,
        TokenType.Symbol.LessEqual,
        TokenType.Symbol.AngleOpen,
        TokenType.Symbol.AngleClose,
    ]);

    public static readonly Matcher BinPrecedence5Ops = new OrMatcher("BinPrecedence5Ops", () => [
        TokenType.Symbol.Equal,
        TokenType.Symbol.NotEqual,
    ]);

    public static readonly Matcher BinPrecedence6Ops = new OrMatcher("BinPrecedence6Ops", () => [
        TokenType.Symbol.BitAnd,
    ]);

    public static readonly Matcher BinPrecedence7Ops = new OrMatcher("BinPrecedence7Ops", () => [
        TokenType.Symbol.BitXor,
    ]);

    public static readonly Matcher BinPrecedence8Ops = new OrMatcher("BinPrecedence8Ops", () => [
        TokenType.Symbol.BitOr,
    ]);

    public static readonly Matcher BinPrecedence9Ops = new OrMatcher("BinPrecedence9Ops", () => [
        TokenType.Symbol.BoolAnd,
    ]);

    public static readonly Matcher BinPrecedence10Ops = new OrMatcher("BinPrecedence10Ops", () => [
        TokenType.Symbol.BoolXor,
    ]);

    public static readonly Matcher BinPrecedence11Ops = new OrMatcher("BinPrecedence11Ops", () => [
        TokenType.Symbol.BoolOr,
    ]);
}
