namespace Lang;

using Lang.SyntaxTree;
using Lang.SyntaxTree.Rules;
using Lang.Tokenize;
using Lang.Util;

public static class Program {
    public static void Main() {
        var tokens = Tokenizer.Tokenize("test.txt", File.ReadAllText("test.txt")).ToArray();

        var match = AstRules.Start.Match(tokens);

        Console.WriteLine(match.PrettyString());
    }
}
