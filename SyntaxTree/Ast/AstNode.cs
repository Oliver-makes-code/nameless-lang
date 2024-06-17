using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Ast;

public interface AstNode : IntoPretty;

public interface ExpressionNode : AstNode;

public record BinOpNode(
    ExpressionNode Left,
    Matchlet.Token Op,
    ExpressionNode Right
) : ExpressionNode {
    public string PrettyString(int indent = 0) {
        string output = "BinOp(\n";
        output += new string(' ', (indent+1) * 2);
        output += "left: ";
        output += Left.PrettyString(indent+1);
        output += ",\n";
        output += new string(' ', (indent+1) * 2);
        output += "op: ";
        output += Op.PrettyString(indent+1);
        output += ",\n";
        output += new string(' ', (indent+1) * 2);
        output += "right: ";
        output += Right.PrettyString(indent+1);
        output += "\n";
        output += new string(' ', indent * 2);
        output += ")";
        return output;
    }
}

public record ValueNode(
    Matchlet.Token Value
) : ExpressionNode {
    public string PrettyString(int indent = 0)
        => Value.PrettyString(indent);
}

public record InvokeNode(
    ExpressionNode Value
) : ExpressionNode {
    public string PrettyString(int indent = 0) {
        string output = "Invoke(\n";
        output += new string(' ', (indent+1) * 2);
        output += "value: ";
        output += Value.PrettyString(indent+1);
        output += "\n";
        output += new string(' ', indent * 2);
        output += ")";
        return output;
    }
}
