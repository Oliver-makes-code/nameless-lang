using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.SyntaxTree.Ast;

public interface ExpressionNode : AstNode;

public record BinOpExpression(
    ExpressionNode Left,
    Matchlet.Token Op,
    ExpressionNode Right
) : ExpressionNode {
    public ValueString IntoValueString()
        => new ValueStringBuilder.Struct("BinOpExpression")
            .Field("Left", Left)
            .Field("Op", Op)
            .Field("Right", Right)
            .Build();
}

public record ValueExpression(
    Matchlet.Token Value
) : ExpressionNode {
    public ValueString IntoValueString()
        => Value.IntoValueString();
}

public record InvokeExpression(
    ExpressionNode Value,
    List<ExpressionNode> Parameters
) : ExpressionNode {
    public ValueString IntoValueString()
        => new ValueStringBuilder.Struct("InvokeExpression")
            .Field("Value", Value)
            .ArrayField("Parameters", [..Parameters])
            .Build();
}

public record ObjectAccessExpression(
    ExpressionNode Value,
    List<Matchlet.Token> Parameters
) : ExpressionNode {
    public ValueString IntoValueString()
        => new ValueStringBuilder.Struct("ObjectAccessExpression")
            .Field("Value", Value)
            .ArrayField("Parameters", [..Parameters])
            .Build();
}
