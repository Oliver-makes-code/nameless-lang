using Lang.Util;

namespace Lang.SyntaxTree.Types;

public abstract record Expr(StringView View) : Node(View);

public record NumberLiteral<T>(T Value, StringView View) : Expr(View);

public record StringLiteral(string Value, StringView View) : Expr(View);

public record BooleanLiteral(bool Value, StringView View) : Expr(View);
