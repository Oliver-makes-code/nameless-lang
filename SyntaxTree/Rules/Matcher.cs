using Lang.Tokenize;
using Lang.Util;

namespace Lang.SyntaxTree.Rules;

public abstract record Matcher(string Name) {
    public abstract Matchlet Match(Token[] tokens, int n = 0);
}

public record TokenTypeMatcher<T>(string Name) : Matcher(Name) where T : TokenType {
    public override Matchlet Match(Token[] tokens, int n = 0) {
        if (tokens[n].Type is T)
            return new Matchlet.Token(this, new Diagnostic.Empty(), tokens[n], n, 1);
        return new Matchlet.Error(this, new Diagnostic.UnexpectedToken(tokens[n]), [], n);
    }
}

public record TokenValueMatcher(string Name, TokenType Token) : Matcher(Name) {
    public override Matchlet Match(Token[] tokens, int n = 0) {
        if (tokens[n].Type == Token)
            return new Matchlet.Token(this, new Diagnostic.Empty(), tokens[n], n, 1);
        return new Matchlet.Error(this, new Diagnostic.UnexpectedToken(tokens[n]), [], n);
    }
}

public record ListMatcher(string Name, LazyInitializer<List<Matcher>> Matches) : Matcher(Name) {
    public ListMatcher(string Name, LazyInitializer<List<Matcher>>.Initializer init) : this(Name, new LazyInitializer<List<Matcher>>(init)) {}

    public override Matchlet Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        int advance = 0;
        var matches = new List<Matchlet>();

        foreach (var matcher in Matches.Value) {
            var value = matcher.Match(tokens, n + advance);
            matches.Add(value);

            if (value is Matchlet.Error)
                return new Matchlet.Error(this, new Diagnostic.Empty(), matches, value.MaxPeek);
            advance += value.Advance;
            maxPeek = value.MaxPeek;
        }

        return new Matchlet.List(this, new Diagnostic.Empty(), matches, maxPeek, advance);
    }
}

public record OrMatcher(string Name, LazyInitializer<List<Matcher>> Matches) : Matcher(Name) {
    public OrMatcher(string Name, LazyInitializer<List<Matcher>>.Initializer init) : this(Name, new LazyInitializer<List<Matcher>>(init)) {}

    public override Matchlet Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        var children = new List<Matchlet>();

        foreach (var matcher in Matches.Value) {
            var match = matcher.Match(tokens, n);
            children.Add(match);

            if (match is not Matchlet.Error)
                return new Matchlet.Or(this, new Diagnostic.Empty(), children, match.MaxPeek, match.Advance);
            maxPeek = match.MaxPeek;
        }
        return new Matchlet.Error(this, new Diagnostic.Empty(), children, maxPeek);
    }
}

public record NullMatcher(string Name) : Matcher(Name) {
    public override Matchlet Match(Token[] tokens, int n = 0)
        => new Matchlet.Null(this, new Diagnostic.Empty(), n);
}
