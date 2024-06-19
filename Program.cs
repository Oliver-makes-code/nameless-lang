namespace Lang;

using System.Diagnostics;
using Lang.SyntaxTree.Rules;
using Lang.SyntaxTree.Visitor;
using Lang.Tokenize;
using Lang.Util;

public static class Program {
    public static void Main() {
        var tokens = Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")).ToArray();

        var match = AstRules.Start.Match(tokens);

        if (match is Matchlet.Error) {
            Console.WriteLine(match.PrettyString());
            return;
        }

        var tree = OperatorVisitor.Instance.Visit(match.As<Matchlet.List>().Matches[0]).Unwrap();

        Console.WriteLine(tree.PrettyString());
    }
}
