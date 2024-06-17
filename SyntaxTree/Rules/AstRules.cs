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

    public static readonly Matcher Expression = new ListMatcher("Expression", () => [
        OperatorRules.BinPrecedence1
    ]);
}

public static class FunctionCallRules {
    public static readonly Matcher FuncArgs = new OrMatcher("FuncArgs", () => [
        new ListMatcher("FuncArgsFull", () => [
            TokenType.Symbol.ParenOpen.Matcher,
            new RepeatingMatcher("FuncParams", () => new ListMatcher("FuncParam", () => [
                ExpressionRules.Expression,
                TokenType.Symbol.Comma.Matcher
            ])),
            ExpressionRules.Expression,
            TokenType.Symbol.ParenClose.Matcher
        ]),
        AstRules.EmptyParens
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
        ExpressionRules.Value,
        new RepeatingMatcher("Terms", () => new ListMatcher("Term", () => [
            BinPrecedence11Ops,
            ExpressionRules.Value
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
