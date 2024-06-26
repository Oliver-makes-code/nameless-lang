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
        
        bool end = list[0].Rule == ExpressionRules.ValueInvoke;

        Func<Matchlet, Result<AstNode, Diagnostic>> visit = end ? ValueFuncVisitor.Instance.Visit : Visit;

        var value = visit(list[0]);

        if (value.IsErr)
            return value;
        
        var expr = value.Unwrap();

        if (list[1] is not Matchlet.Empty) {
            var repeat = list[1].As<Matchlet.Repeat>();

            foreach (var val in repeat.Matches) {
                if (val is not Matchlet.List l)
                    break;
                
                var op = l[0].As<Matchlet.Or>().Matches.Last().As<Matchlet.Token>();
                var v = visit(l[1]).Unwrap();
                expr = new BinOpExpression(expr.As<ExpressionNode>(), op, v.As<ExpressionNode>());
            }
        }

        return Result<AstNode, Diagnostic>.FromOk(expr);
    }
}

public class ValueFuncVisitor : Visitor {
    public static readonly ValueFuncVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        var value = ValueVisitor.Instance.Visit(list[0]);

        var optAccesses = list[1];

        if (optAccesses is Matchlet.Empty || value.IsErr)
            return value;

        var output = value.Unwrap().As<ExpressionNode>();

        var accesses = optAccesses.As<Matchlet.Repeat>();
        
        for (int i = 0; i < accesses.Matches.Count - 1; i++) {
            var access = accesses.Matches[i].As<Matchlet.Or>();

            if (access.Matches.Count == 2 && access.Matches[1] is not Matchlet.Error) {
                var token = access.Matches[1].As<Matchlet.List>().Matches[1].As<Matchlet.Token>();
                if (output is ObjectAccessExpression obj)
                    obj.Parameters.Add(token);
                else
                    output = new ObjectAccessExpression(output, [token]);
            } else {
                var optInvoke = access.Matches[0].As<Matchlet.List>()[1];

                if (optInvoke is Matchlet.Empty) {
                    output = new InvokeExpression(output, []);
                    continue;
                }

                var invoke = optInvoke.As<Matchlet.List>();

                var args = new List<ExpressionNode>();
                var optArgsList = invoke[0];

                if (optArgsList is not Matchlet.Empty) {
                    var argsList = optArgsList.As<Matchlet.Repeat>();

                    for (int j = 0; j < argsList.Matches.Count - 1; j++) {
                        var arg = OperatorVisitor.Instance.Visit(argsList.Matches[j].As<Matchlet.List>()[0]);

                        if (arg.IsErr)
                            return arg;
                        
                        args.Add(arg.Unwrap().As<ExpressionNode>());
                    }
                }
                
                var final = OperatorVisitor.Instance.Visit(invoke[1]);

                if (final.IsErr)
                    return final;
                
                args.Add(final.Unwrap().As<ExpressionNode>());

                output = new InvokeExpression(output, args);
            }
        }

        return Result<AstNode, Diagnostic>.FromOk(output);
    }
}

public class ValueVisitor : Visitor {
    public static readonly ValueVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var or = match.As<Matchlet.Or>();

        var matched = or.Successful;

        if (matched is Matchlet.Error err)
            return err.Diagnostic;
        
        if (matched is Matchlet.Token token)
            return new ValueExpression(token);
        
        return ParenthesisVisitor.Instance.Visit(matched);
    }
}

public class ParenthesisVisitor : Visitor {
    public static readonly ParenthesisVisitor Instance = new();

    public Result<AstNode, Diagnostic> Visit(Matchlet match) {
        var list = match.As<Matchlet.List>();

        return OperatorVisitor.Instance.Visit(list[1]);
    }
}
