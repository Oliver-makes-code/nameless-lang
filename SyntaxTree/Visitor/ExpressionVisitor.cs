using System.Reflection;
using Lang.SyntaxTree.Ast;
using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Visitor;

public class OperatorVisitor : Visitor {
    public static readonly OperatorVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        if (list.Matches.Last() is Matchlet.Error err)
            return err.Diagnostic;
        
        bool end = list.Matches[0].Rule == ExpressionRules.ValueInvoke;

        Func<Matchlet, Result<AstNode, Diagnostic>> visit = end ? ValueFuncVisitor.Instance.Visit : Visit;

        var value = visit(list.Matches[0]);

        if (value.IsErr)
            return value;
        
        var expr = value.Unwrap();

        var repeat = list.Matches[1].As<Matchlet.Repeat>();

        foreach (var val in repeat.Matches) {
            if (val is not Matchlet.List l)
                break;
            
            var op = l.Matches[0].As<Matchlet.Or>().Matches.Last().As<Matchlet.Token>();
            var v = visit(l.Matches[1]).Unwrap();
            expr = new BinOpNode(expr.As<ExpressionNode>(), op, v.As<ExpressionNode>());
        }

        return Result<AstNode, Diagnostic>.FromOk(expr);
    }
}

public class ValueFuncVisitor : Visitor {
    public static readonly ValueFuncVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        var value = ValueVisitor.Instance.Visit(list.Matches[0]);

        var optInvoke = list.Matches[1];

        if (optInvoke is Matchlet.Empty || value.IsErr)
            return value;

        var invoke = optInvoke.As<Matchlet.List>();
        var optArgs = invoke.Matches[1];

        if (optArgs is Matchlet.Empty)
            return new InvokeNode(value.Unwrap().As<ExpressionNode>(), []);
        
        var args = optArgs.As<Matchlet.List>();
        var former = args.Matches[0].As<Matchlet.Repeat>();

        var argsList = new List<ExpressionNode>();

        for (int i = 0; i < former.Matches.Count - 1; i++) {
            var arg = OperatorVisitor.Instance.Visit(former.Matches[i].As<Matchlet.List>().Matches[0]);

            if (arg.IsErr)
                return arg;
            
            argsList.Add(arg.Unwrap().As<ExpressionNode>());
        }

        var last = OperatorVisitor.Instance.Visit(args.Matches[1]);

        if (last.IsErr)
            return last;
        
        argsList.Add(last.Unwrap().As<ExpressionNode>());
        
        return new InvokeNode(value.Unwrap().As<ExpressionNode>(), argsList);
    }
}

public class ValueVisitor : Visitor {
    public static readonly ValueVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var or = match.As<Matchlet.Or>();

        var matched = or.Matches.Last();

        if (matched is Matchlet.Error err)
            return err.Diagnostic;
        
        if (matched is Matchlet.Token token)
            return new ValueNode(token);
        
        return ParenthesisVisitor.Instance.Visit(matched);
    }
}

public class ParenthesisVisitor : Visitor {
    public static readonly ParenthesisVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        return OperatorVisitor.Instance.Visit(list.Matches[1]);
    }
}
