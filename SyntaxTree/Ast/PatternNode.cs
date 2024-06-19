using Lang.Util;

namespace Lang.SyntaxTree.Ast;

public interface PatternNode : AstNode;

public record DiscardPattern : PatternNode {
    public ValueString IntoValueString()
        => new ValueString.Ident("DiscardPattern");
}
