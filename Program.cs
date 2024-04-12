namespace Lang;

using Lang.SyntaxTree.Rules;
using Lang.Tokenize;
using Lang.Util;

public static class Program {
    public static readonly Matcher Number = new OrMatcher("Number", () => [
        new TokenTypeMatcher<TokenType.Number<double>>("Float"),
        new TokenTypeMatcher<TokenType.Number<long>>("Int")
    ]);
    public static readonly Matcher String = new TokenTypeMatcher<TokenType.String>("String");
    public static readonly Matcher Value = new OrMatcher("Value", () => [
        Number,
        String
    ]);
    public static readonly Matcher Operator = new OrMatcher("Operator", () => [
        new TokenValueMatcher("Add", TokenType.Symbol.Add),
        new TokenValueMatcher("Sub", TokenType.Symbol.Sub),
        new TokenValueMatcher("Mul", TokenType.Symbol.Mul),
        new TokenValueMatcher("Div", TokenType.Symbol.Div),
        new TokenValueMatcher("Mod", TokenType.Symbol.Mod)
    ]);
    public static readonly Matcher Term = new ListMatcher("Term", () => [
        Operator,
        Value
    ]);

    public static readonly Matcher Expression = new ListMatcher("Expression", () => [
        Value,
        new RepeatingMatcher("Terms", () => Term)
    ]);

    public static void Main() {
        var tokens = Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")).ToArray();

        var match = Expression.Match(tokens);

        if (match is Option<Matchlet>.Some { Value: var m })
            Console.WriteLine(m.PrettyString());
    }
}
