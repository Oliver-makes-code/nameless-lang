using Lang.Util;

namespace Lang.SyntaxTree.Ast;

public interface AstNode : IntoValueString {
    public T As<T>() where T : AstNode {
        if (this is not T)
            throw new ArgumentException($"Expected {typeof(T).Name}, got {GetType().Name}");
        return (T) this;
    }
}

public record MatchSingleNode(ExpressionNode Value, PatternNode Pattern) : AstNode {
    public ValueString IntoValueString()
        => new ValueStringBuilder.Struct("MatchSingleNode")
            .Field("Value", Value)
            .Field("Pattern", Pattern)
            .Build();
}
