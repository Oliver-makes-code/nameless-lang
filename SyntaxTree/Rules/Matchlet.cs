using Lang.Util;

namespace Lang.SyntaxTree.Rules;

public abstract record Matchlet(Matcher Rule, Diagnostic Diagnostic, int MaxPeek, int Advance) : IntoPretty {
    public record Null(Matcher Rule, Diagnostic Diagnostic, int MaxPeek) : Matchlet(Rule, Diagnostic, MaxPeek, 0) {
        public override string PrettyString(int indent = 0)
            => $"Null[{Diagnostic}]";

        public override string ToString()
            => PrettyString();
    }

    public record Token(Matcher Rule, Diagnostic Diagnostic, Tokenize.Token Value, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0)
            => $"Token<{Rule.Name}>[{Diagnostic}]({Value})";

        public override string ToString()
            => PrettyString();
    }

    public record List(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0) {
            if (Matches.Count == 0)
                return $"List<{Rule.Name}>[{Diagnostic}]";

            var s = $"List<{Rule.Name}>[{Diagnostic}](";
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
    }

    public record Or(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0) {
            if (Matches.Count == 0)
                return $"Or<{Rule.Name}>[{Diagnostic}]";

            var s = $"Or<{Rule.Name}>[{Diagnostic}](";
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
    }

    public record Repeat(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public override string PrettyString(int indent = 0) {
            if (Matches.Count == 0)
                return $"Repeat<{Rule.Name}>[{Diagnostic}]";

            var s = $"Repeat<{Rule.Name}>[{Diagnostic}](";
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
    }

    public record Reference(Matcher Rule, Diagnostic Diagnostic, Matchlet Child) : Matchlet(Rule, Diagnostic, Child.MaxPeek, Child.Advance) {
        public override string PrettyString(int indent = 0) {
            var s = $"Reference<{Rule.Name}>[{Diagnostic}](\n";
            s += new string(' ', (indent+1) * 2);
            s += Child.PrettyString(indent+1);
            s += "\n";
            s += new string(' ', indent * 2);
            s += ")";
            return s;
        }
    }

    public record Error(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek) : Matchlet(Rule, Diagnostic, MaxPeek, 0) {
        public override string PrettyString(int indent = 0) {
            if (Matches.Count == 0)
                return $"Error<{Rule.Name}>[{Diagnostic}]";

            var s = $"Error<{Rule.Name}>[{Diagnostic}](";
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
    }

    public abstract string PrettyString(int indent = 0);
}
