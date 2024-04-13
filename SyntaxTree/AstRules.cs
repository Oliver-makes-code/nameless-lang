using Lang.SyntaxTree.Rules;
using Lang.Tokenize;

namespace Lang.SyntaxTree;

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
    ]);

    public static readonly Matcher Term = new ListMatcher("Term", () => [
        BinOp,
        Expression!
    ]);

    public static readonly Matcher Expression = new ListMatcher("Expression", () => [
        Value,
        new OrMatcher("Terms", () => [
            Term,
            Null
        ])
    ]);

    public static readonly Matcher Start = new ListMatcher("Start", () => [
        Expression,
        TokenType.Eof.Matcher
    ]);
}
