using Lang.SyntaxTree.Ast;
using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Visitor;

public interface Visitor {
    public Result<AstNode, Diagnostic> Visit(Matchlet match);
}
