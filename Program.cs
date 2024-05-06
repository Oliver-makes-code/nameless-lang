namespace Lang;

using Lang.SyntaxTree.Rules;
using Lang.SyntaxTree.Visitor;
using Lang.Tokenize;

public static class Program {
    public static void Main() {
        var tokens = Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")).ToArray();

        var match = (Matchlet.List) AstRules.Start.Match(tokens);

        Console.WriteLine(ExpressionVisitor.Instance.Visit(match.Matches[0]).Unwrap().PrettyString());
    }
}
