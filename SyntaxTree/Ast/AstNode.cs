using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Ast;

public interface AstNode : IntoPretty;

public interface ExpressionNode : AstNode;

public record BinOpNode(
    ExpressionNode left,
    Matchlet.Token op,
    ExpressionNode right
) : ExpressionNode {
    public string PrettyString(int indent = 0) {
        string output = "BinOp(\n";
        output += new string(' ', (indent+1) * 2);
        output += "left: ";
        output += left.PrettyString(indent+1);
        output += ",\n";
        output += new string(' ', (indent+1) * 2);
        output += "op: ";
        output += op.PrettyString(indent+1);
        output += ",\n";
        output += new string(' ', (indent+1) * 2);
        output += "right: ";
        output += right.PrettyString(indent+1);
        output += "\n";
        output += new string(' ', indent * 2);
        output += ")";
        return output;
    }
}


public record ValueNode(
    Matchlet.Token value
) : ExpressionNode {
    public string PrettyString(int indent = 0)
        => value.PrettyString(indent);
}

