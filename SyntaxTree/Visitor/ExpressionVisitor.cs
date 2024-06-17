using Lang.SyntaxTree.Ast;
using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Visitor;

public class OperatorVisitor : Visitor {
    public static readonly OperatorVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        if (match is not Matchlet.List list)
            throw new ArgumentException("Should be Matchlet.List", nameof(match));

        if (list.Matches.Last() is Matchlet.Error err)
            return new Result<AstNode, Diagnostic>.Err(err.Diagnostic);
        
        bool end = list.Matches[0].Rule == ExpressionRules.ValueInvoke;

        Func<Matchlet, Result<AstNode, Diagnostic>> visit = end ? ValueFuncVisitor.Instance.Visit : Visit;

        var value = visit(list.Matches[0]);

        if (value.IsErr)
            return value;
        
        var expr = value.Unwrap();

        var repeat = ((Matchlet.Repeat) list.Matches[1])!;

        foreach (var val in repeat.Matches) {
            if (val is not Matchlet.List l)
                break;
            
            var op = (Matchlet.Token)((Matchlet.Or) l.Matches[0]).Matches.Last();
            var v = visit(l.Matches[1]).Unwrap();
            expr = new BinOpNode((ExpressionNode) expr, op, (ExpressionNode) v);
        }

        return new Result<AstNode, Diagnostic>.Ok(expr);
    }
}

public class ValueFuncVisitor : Visitor {
    public static readonly ValueFuncVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        if (match is not Matchlet.Or or)
            throw new ArgumentException("Should be Matchlet.Or", nameof(match));
        
        if (or.Matches.Count == 2)
            return ValueVisitor.Instance.Visit(or.Matches[1]);
        
        var func = or.Matches[0];

        if (func is not Matchlet.List list)
            throw new ArgumentException("Should be Matchlet.list", "match.Matches[0]");
        
        var value = ValueVisitor.Instance.Visit(list.Matches[0]);

        if (value.IsErr)
            return value;
        
        return new Result<AstNode, Diagnostic>.Ok(new InvokeNode((value.Unwrap() as ExpressionNode)!));
    }
}

public class ValueVisitor : Visitor {
    public static readonly ValueVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        if (match is not Matchlet.Or or)
            throw new ArgumentException("Should be Matchlet.Or", nameof(match));

        var matched = or.Matches.Last();

        if (matched is Matchlet.Error err)
            return new Result<AstNode, Diagnostic>.Err(err.Diagnostic);
        
        if (matched is Matchlet.Token token)
            return new Result<AstNode, Diagnostic>.Ok(new ValueNode(token));
        
        return ParenthesisVisitor.Instance.Visit(matched);
    }
}

public class ParenthesisVisitor : Visitor {
    public static readonly ParenthesisVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        if (match is not Matchlet.List list)
            throw new ArgumentException("Should be Matchlet.List", nameof(match));

        return OperatorVisitor.Instance.Visit(list.Matches[1]);
    }
}
