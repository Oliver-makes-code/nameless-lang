using System.Net.Http.Headers;
using Lang.Tokenize;
using Lang.Util;

namespace Lang.SyntaxTree.Rules;

public abstract record Diagnostic {
    public record UnexpectedToken(Token Token) : Diagnostic() {
        public override string ToString()
            => $"UnexpectedToken({Token})";
    }

    public record Empty() : Diagnostic() {
        public override string ToString()
            => $"Empty";
    }

    public override abstract string ToString();
}
