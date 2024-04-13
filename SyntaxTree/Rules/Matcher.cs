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
        Diagnostic diagnostic = new Diagnostic.Empty();

        foreach (var matcher in Matches.Value) {
            var value = matcher.Match(tokens, n + advance);
            matches.Add(value);
            diagnostic = value.Diagnostic;

            if (value is Matchlet.Error)
                return new Matchlet.Error(this, diagnostic, matches, value.MaxPeek);
            advance += value.Advance;
            maxPeek = value.MaxPeek;
        }

        return new Matchlet.List(this, diagnostic, matches, maxPeek, advance);
    }
}

public record OrMatcher(string Name, LazyInitializer<List<Matcher>> Matches) : Matcher(Name) {
    public OrMatcher(string Name, LazyInitializer<List<Matcher>>.Initializer init) : this(Name, new LazyInitializer<List<Matcher>>(init)) {}

    public override Matchlet Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        var children = new List<Matchlet>();
        Diagnostic diagnostic = new Diagnostic.Empty();

        foreach (var matcher in Matches.Value) {
            var match = matcher.Match(tokens, n);
            children.Add(match);
            diagnostic = match.Diagnostic;

            if (match is not Matchlet.Error)
                return new Matchlet.Or(this, diagnostic, children, match.MaxPeek, match.Advance);
            maxPeek = match.MaxPeek;
        }
        return new Matchlet.Error(this, diagnostic, children, maxPeek);
    }
}

public record RepeatingMatcher(string Name, LazyInitializer<Matcher> Matcher) : Matcher(Name) {
    public RepeatingMatcher(string Name, LazyInitializer<Matcher>.Initializer init) : this(Name, new LazyInitializer<Matcher>(init)) {}

    public override Matchlet Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        int advance = 0;
        var matches = new List<Matchlet>();

        while (true) {
            var value = Matcher.Value.Match(tokens, n + advance);
            matches.Add(value);

            if (value is Matchlet.Error)
                return new Matchlet.Repeat(this, value.Diagnostic, matches, maxPeek, advance);
            advance += value.Advance;
            maxPeek = value.MaxPeek;
        }
    }
}

public record NullMatcher(string Name) : Matcher(Name) {
    public override Matchlet Match(Token[] tokens, int n = 0)
        => new Matchlet.Null(this, new Diagnostic.Empty(), n);
}
