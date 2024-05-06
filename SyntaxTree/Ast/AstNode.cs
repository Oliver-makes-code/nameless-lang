using Lang.SyntaxTree.Rules;

namespace Lang.SyntaxTree.Ast;

public interface AstNode;

public record ExpressionNode : AstNode;

public record BinOpNode(
    ExpressionNode left,
    Matchlet.Token op,
    ExpressionNode right
) : ExpressionNode;

public record ValueNode(
    Matchlet.Token value
) : ExpressionNode;
