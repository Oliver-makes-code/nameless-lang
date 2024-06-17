namespace Lang;

using System.Diagnostics;
using Lang.SyntaxTree.Rules;
using Lang.SyntaxTree.Visitor;
using Lang.Tokenize;

public static class Program {
    public static void Main() {
        var tokens = Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")).ToArray();

        var match = (Matchlet.List) AstRules.Start.Match(tokens);

        var tree = OperatorVisitor.Instance.Visit(match.Matches[0]).Unwrap();

        Console.WriteLine(tree.PrettyString());
    }
}
