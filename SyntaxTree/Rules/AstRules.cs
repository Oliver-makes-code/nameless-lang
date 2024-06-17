using Lang.Tokenize;

namespace Lang.SyntaxTree.Rules;

public static class AstRules {
    public static readonly Matcher Null = new NullMatcher("Null");

    public static readonly Matcher Start = new ListMatcher("Start", () => [
        ExpressionRules.Expression,
        TokenType.Eof.Matcher
    ]);

    public static readonly Matcher EmptyParens = new ListMatcher("EmptyParens", () => [
        TokenType.Symbol.ParenOpen.Matcher,
        TokenType.Symbol.ParenClose.Matcher
    ]);

    public static readonly Matcher Type = new OrMatcher("Type", () => [
        TokenType.Keyword.Var.Matcher,
        TokenType.Keyword.I8.Matcher,
        TokenType.Keyword.I16.Matcher,
        TokenType.Keyword.I32.Matcher,
        TokenType.Keyword.I64.Matcher,
        TokenType.Keyword.Ui8.Matcher,
        TokenType.Keyword.Ui16.Matcher,
        TokenType.Keyword.Ui32.Matcher,
        TokenType.Keyword.Ui64.Matcher,
        TokenType.Keyword.F32.Matcher,
        TokenType.Keyword.F64.Matcher,
        TokenType.Identifier.TypeMatcher
    ]);
}

public static class StatementRules {
    public static readonly Matcher PartialDeclarator = new ListMatcher("PartialDeclarator", () => [
        AstRules.Type,
        TokenType.Identifier.TypeMatcher
    ]);
    public static readonly Matcher FullDeclarator = new ListMatcher("FullDeclarator", () => [
        PartialDeclarator,
        TokenType.Symbol.Equal.Matcher,
        ExpressionRules.Expression
    ]);

    public static readonly Matcher ExpressionLine = new ListMatcher("Line", () => [
        ExpressionRules.Expression,
        TokenType.Symbol.Semicolon.Matcher
    ]);

    public static readonly Matcher Statement = new OrMatcher("Statement", () => [
        While,
        Loop,
        ForIn,
        For,
        ExpressionLine
    ]);

    public static readonly Matcher For = new ListMatcher("For", () => [
        TokenType.Keyword.For.Matcher,
        FullDeclarator,
        TokenType.Symbol.Comma.Matcher,
        ExpressionRules.Expression,
        TokenType.Symbol.Comma.Matcher,
        Statement,
        Block
    ]);

    public static readonly Matcher ForIn = new ListMatcher("ForIn", () => [
        TokenType.Keyword.For.Matcher,
        TokenType.Identifier.TypeMatcher,
        TokenType.Keyword.In.Matcher,
        ExpressionRules.Expression,
        Block
    ]);

    public static readonly Matcher While = new ListMatcher("While", () => [
        TokenType.Keyword.While.Matcher,
        ExpressionRules.Expression,
        Block
    ]);

    public static readonly Matcher Loop = new ListMatcher("Loop", () => [
        TokenType.Keyword.Loop.Matcher,
        Block
    ]);

    public static readonly Matcher If = new ListMatcher("If", () => [
        IfSingle,
        ElseIfRepeat,
        new OptionalMatcher("Else", () => ElseSingle)
    ]);

    public static readonly Matcher ElseSingle = new ListMatcher("ElseSingle", () => [
        TokenType.Keyword.Else.Matcher,
        Block
    ]);

    public static readonly Matcher ElseIfSingle = new ListMatcher("ElseIfSingle", () => [
        TokenType.Keyword.Else.Matcher,
        IfSingle
    ]);

    public static readonly Matcher ElseIfRepeat = new RepeatingMatcher("ElseIfRepeat", () => ElseIfSingle);

    public static readonly Matcher IfSingle = new ListMatcher("IfSingle", () => [
        TokenType.Keyword.If.Matcher,
        ExpressionRules.Expression,
        Block
    ]);

    public static readonly Matcher Block = new ListMatcher("Block", () => [
        TokenType.Symbol.BraceOpen.Matcher,
        // TODO
        TokenType.Symbol.BraceClose.Matcher
    ]);
}

public static class ExpressionRules {
    public static readonly Matcher Parenthesis = new ListMatcher("Parenthesis", () => [
        TokenType.Symbol.ParenOpen.Matcher,
        Expression!,
        TokenType.Symbol.ParenClose.Matcher
    ]);

    public static readonly Matcher Value = new OrMatcher("Value", () => [
        TokenType.Number<double>.TypeMatcher,
        TokenType.Number<long>.TypeMatcher,
        TokenType.String.TypeMatcher,
        TokenType.Identifier.TypeMatcher,
        Parenthesis
    ]);

    public static readonly Matcher ValueInvoke = new ListMatcher("ValueInvoke", () => [
        Value,
        new OptionalMatcher("Invoke", () => FuncArgs)
    ]);

    public static readonly Matcher Expression = OperatorRules.BinPrecedence1;

    public static readonly Matcher FuncArgs = new ListMatcher("FuncArgs", () => [
        TokenType.Symbol.ParenOpen.Matcher,
        new OptionalMatcher("Inner", () => FunArgsInner),
        TokenType.Symbol.ParenClose.Matcher
    ]);

    public static readonly Matcher FunArgsInner = new ListMatcher("FuncArgsInner", () => [
        new RepeatingMatcher("FuncParams", () => new ListMatcher("FuncParam", () => [
            Expression,
            TokenType.Symbol.Comma.Matcher
        ])),
        Expression
    ]);
}

public static class OperatorRules {
    public static readonly Matcher BinPrecedence1 = new ListMatcher("BinPrecedence1", () => [
        BinPrecedence2,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence1Ops,
            BinPrecedence2
        ]))
    ]);

    public static readonly Matcher BinPrecedence2 = new ListMatcher("BinPrecedence2", () => [
        BinPrecedence3,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence2Ops,
            BinPrecedence3
        ]))
    ]);

    public static readonly Matcher BinPrecedence3 = new ListMatcher("BinPrecedence3", () => [
        BinPrecedence4,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence3Ops,
            BinPrecedence4
        ]))
    ]);

    public static readonly Matcher BinPrecedence4 = new ListMatcher("BinPrecedence4", () => [
        BinPrecedence5,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence4Ops,
            BinPrecedence5
        ]))
    ]);

    public static readonly Matcher BinPrecedence5 = new ListMatcher("BinPrecedence5", () => [
        BinPrecedence6,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence5Ops,
            BinPrecedence6
        ]))
    ]);

    public static readonly Matcher BinPrecedence6 = new ListMatcher("BinPrecedence6", () => [
        BinPrecedence7,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence6Ops,
            BinPrecedence7
        ]))
    ]);

    public static readonly Matcher BinPrecedence7 = new ListMatcher("BinPrecedence7", () => [
        BinPrecedence8,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence7Ops,
            BinPrecedence8
        ]))
    ]);

    public static readonly Matcher BinPrecedence8 = new ListMatcher("BinPrecedence8", () => [
        BinPrecedence9,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence8Ops,
            BinPrecedence9
        ]))
    ]);

    public static readonly Matcher BinPrecedence9 = new ListMatcher("BinPrecedence9", () => [
        BinPrecedence10,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence9Ops,
            BinPrecedence10
        ]))
    ]);

    public static readonly Matcher BinPrecedence10 = new ListMatcher("BinPrecedence10", () => [
        BinPrecedence11,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence10Ops,
            BinPrecedence11
        ]))
    ]);

    public static readonly Matcher BinPrecedence11 = new ListMatcher("BinPrecedence11", () => [
        ExpressionRules.ValueInvoke,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence11Ops,
            ExpressionRules.ValueInvoke
        ]))
    ]);

    public static readonly Matcher BinPrecedence1Ops = new OrMatcher("BinPrecedence1Ops", () => [
        TokenType.Symbol.Add.Matcher,
        TokenType.Symbol.Sub.Matcher,
    ]);

    public static readonly Matcher BinPrecedence2Ops = new OrMatcher("BinPrecedence2Ops", () => [
        TokenType.Symbol.Mul.Matcher,
        TokenType.Symbol.Div.Matcher,
        TokenType.Symbol.Mod.Matcher,
    ]);

    public static readonly Matcher BinPrecedence3Ops = new OrMatcher("BinPrecedence3Ops", () => [
        TokenType.Symbol.Shl.Matcher,
        TokenType.Symbol.Shr.Matcher,
    ]);

    public static readonly Matcher BinPrecedence4Ops = new OrMatcher("BinPrecedence4Ops", () => [
        TokenType.Symbol.GreaterEqual.Matcher,
        TokenType.Symbol.LessEqual.Matcher,
        TokenType.Symbol.AngleOpen.Matcher,
        TokenType.Symbol.AngleClose.Matcher,
    ]);

    public static readonly Matcher BinPrecedence5Ops = new OrMatcher("BinPrecedence5Ops", () => [
        TokenType.Symbol.Equal.Matcher,
        TokenType.Symbol.NotEqual.Matcher,
    ]);

    public static readonly Matcher BinPrecedence6Ops = new OrMatcher("BinPrecedence6Ops", () => [
        TokenType.Symbol.BitAnd.Matcher,
    ]);

    public static readonly Matcher BinPrecedence7Ops = new OrMatcher("BinPrecedence7Ops", () => [
        TokenType.Symbol.BitXor.Matcher,
    ]);

    public static readonly Matcher BinPrecedence8Ops = new OrMatcher("BinPrecedence8Ops", () => [
        TokenType.Symbol.BitOr.Matcher,
    ]);

    public static readonly Matcher BinPrecedence9Ops = new OrMatcher("BinPrecedence9Ops", () => [
        TokenType.Symbol.BoolAnd.Matcher,
    ]);

    public static readonly Matcher BinPrecedence10Ops = new OrMatcher("BinPrecedence10Ops", () => [
        TokenType.Symbol.BoolXor.Matcher,
    ]);

    public static readonly Matcher BinPrecedence11Ops = new OrMatcher("BinPrecedence11Ops", () => [
        TokenType.Symbol.BoolOr.Matcher,
    ]);
}
