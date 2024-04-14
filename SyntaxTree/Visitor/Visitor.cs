using Lang.SyntaxTree.Rules;

namespace Lang.SyntaxTree.Visitor;

public abstract record Visitor(Matcher Rule) {
    public abstract void Visit(Matchlet match);
}
