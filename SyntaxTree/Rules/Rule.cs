using Lang.Tokenize;
using Lang.Util;

namespace Lang.SyntaxTree.Rules;

public abstract record Matchlet(Matcher Rule, int MaxPeek, int Advance) {
    public record Null(Matcher Rule, int MaxPeek) : Matchlet(Rule, MaxPeek, 0) {
        public override string PrettyString(int indent = 0)
            => ToString();

        public override string ToString()
            => $"Null<{Rule.Name}>";
    }

    public record Token(Matcher Rule, Tokenize.Token token, int MaxPeek, int Advance) : Matchlet(Rule, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0)
            => ToString();
        
        public override string ToString()
            => $"Token<{Rule.Name}>({token})";
    }

    public record List(Matcher Rule, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0) {
            var s = $"List<{Rule.Name}>(";
            for (int i = 0; i < Matches.Count; i++) {
                s += "\n";
                s += new string(' ', (indent+1) * 2);
                s += Matches[i].PrettyString(indent+1);
                if (i < Matches.Count - 1)
                    s += "," ;
            }
            s += "\n";
            s += new string(' ', indent * 2);
            s += ")";
            return s;
        }

        public override string ToString()
            => $"List<{Rule.Name}>({string.Join(", ", Matches.Select(it => it.ToString()).ToArray())})";
    }

    public record Reference(Matcher Rule, Matchlet Child) : Matchlet(Rule, Child.MaxPeek, Child.Advance) {
        public override string PrettyString(int indent = 0) {
            var s = $"Reference<{Rule.Name}>(\n";
            s += new string(' ', (indent+1) * 2);
            s += Child.PrettyString(indent+1);
            s += "\n";
            s += new string(' ', indent * 2);
            s += ")";
            return s;
        }
    }

    public abstract string PrettyString(int indent = 0);
}

public abstract record Matcher(string Name) {
    public abstract Option<Matchlet> Match(Token[] tokens, int n = 0);
}

public record TokenTypeMatcher<T>(string Name) : Matcher(Name) where T : TokenType {
    public override Option<Matchlet> Match(Token[] tokens, int n = 0) {
        if (tokens[n].Type is T)
            return new Option<Matchlet>.Some(new Matchlet.Token(this, tokens[n], n, 1));
        return Option<Matchlet>.None;
    }
}

public record TokenValueMatcher(string Name, TokenType Token) : Matcher(Name) {
    public override Option<Matchlet> Match(Token[] tokens, int n = 0) {
        if (tokens[n].Type == Token)
            return new Option<Matchlet>.Some(new Matchlet.Token(this, tokens[n], n, 1));
        return Option<Matchlet>.None;
    }
}

public record ListMatcher(string Name, LazyInitializer<List<Matcher>> Matches) : Matcher(Name) {
    public ListMatcher(string Name, LazyInitializer<List<Matcher>>.Initializer init) : this(Name, new LazyInitializer<List<Matcher>>(init)) {}

    public override Option<Matchlet> Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        int advance = 0;
        var matches = new List<Matchlet>();

        foreach (var matcher in Matches.Value) {
            var value = matcher.Match(tokens, n + advance);

            if (value is not Option<Matchlet>.Some { Value: var match })
                return value;
            advance += match.Advance;
            maxPeek = match.MaxPeek;
            matches.Add(match);
        }

        return new Option<Matchlet>.Some(new Matchlet.List(this, matches, maxPeek, advance));
    }
}

public record RepeatingMatcher(string Name, LazyInitializer<Matcher> Repeat) : Matcher(Name) {
    public RepeatingMatcher(string Name, LazyInitializer<Matcher>.Initializer init) : this(Name, new LazyInitializer<Matcher>(init)) {}

    public override Option<Matchlet> Match(Token[] tokens, int n = 0) {
        int maxPeek = n;
        int advance = 0;
        var matches = new List<Matchlet>();

        while (true) {
            var value = Repeat.Value.Match(tokens, n + advance);

            if (value is not Option<Matchlet>.Some { Value: var match })
                return new Option<Matchlet>.Some(new Matchlet.List(this, matches, maxPeek, advance));;
            advance += match.Advance;
            maxPeek = match.MaxPeek;
            matches.Add(match);
        }
    }
}

public record OrMatcher(string Name, LazyInitializer<List<Matcher>> Matches) : Matcher(Name) {
    public OrMatcher(string Name, LazyInitializer<List<Matcher>>.Initializer init) : this(Name, new LazyInitializer<List<Matcher>>(init)) {}

    public override Option<Matchlet> Match(Token[] tokens, int n = 0) {
        foreach (var matcher in Matches.Value) {
            var match = matcher.Match(tokens, n);
            if (match is Option<Matchlet>.Some { Value: var m })
                return new Option<Matchlet>.Some(new Matchlet.Reference(this, m));
        }
        return Option<Matchlet>.None;
    }
}

public record NullMatcher(string Name) : Matcher(Name) {
    public override Option<Matchlet> Match(Token[] tokens, int n = 0)
        => new Option<Matchlet>.Some(new Matchlet.Null(this, n));
}
