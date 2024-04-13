using Lang.Tokenize;

namespace Lang.SyntaxTree.Rules;

public static class AstRules {
    public static readonly Matcher Null = new NullMatcher("Null");
    public static readonly Matcher Number = new OrMatcher("Number", () => [
        TokenType.Number<double>.TypeMatcher,
        TokenType.Number<long>.TypeMatcher
    ]);

    public static readonly Matcher Parenthesis = new ListMatcher("Parenthesis", () => [
        TokenType.Symbol.ParenOpen.Matcher,
        Expression!,
        TokenType.Symbol.ParenClose.Matcher
    ]);

    public static readonly Matcher Value = new OrMatcher("Value", () => [
        Number,
        TokenType.String.TypeMatcher,
        TokenType.Identifier.TypeMatcher,
        Parenthesis
    ]);

    public static readonly Matcher BinOp = new OrMatcher("BinOp", () => [
        TokenType.Symbol.Add.Matcher,
        TokenType.Symbol.Sub.Matcher,
        TokenType.Symbol.Mul.Matcher,
        TokenType.Symbol.Div.Matcher,
        TokenType.Symbol.Mod.Matcher,
        TokenType.Symbol.Shl.Matcher,
        TokenType.Symbol.Shr.Matcher,
        TokenType.Symbol.BitOr.Matcher,
        TokenType.Symbol.BitAnd.Matcher,
        TokenType.Symbol.BitXor.Matcher,
        TokenType.Symbol.BoolOr.Matcher,
        TokenType.Symbol.BoolAnd.Matcher,
        TokenType.Symbol.BoolXor.Matcher,
        TokenType.Symbol.Equal.Matcher,
        TokenType.Symbol.NotEqual.Matcher,
        TokenType.Symbol.GreaterEqual.Matcher,
        TokenType.Symbol.LessEqual.Matcher,
        TokenType.Symbol.AngleOpen.Matcher,
        TokenType.Symbol.AngleClose.Matcher,
    ]);

    public static readonly Matcher Term = new ListMatcher("Term", () => [
        BinOp,
        Value
    ]);

    public static readonly Matcher Expression = new ListMatcher("Expression", () => [
        Value,
        new RepeatingMatcher("Terms", () => Term)
    ]);

    public static readonly Matcher Start = new ListMatcher("Start", () => [
        Expression,
        TokenType.Eof.Matcher
    ]);
}
