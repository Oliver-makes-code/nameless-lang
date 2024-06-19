using Lang.SyntaxTree.Ast;
using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Visitor;

public class MatchSingleVisitor : Visitor {
    public static MatchSingleVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        var optExpr = OperatorVisitor.Instance.Visit(list[0]);

        if (optExpr.IsErr)
            return optExpr;
        
        var expr = optExpr.Unwrap();
        

        throw new("h");
    }
}