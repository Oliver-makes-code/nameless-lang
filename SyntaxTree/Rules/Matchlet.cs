using System.Runtime.InteropServices;
using Lang.Util;

namespace Lang.SyntaxTree.Rules;

public abstract record Matchlet(Matcher Rule, Diagnostic Diagnostic, int MaxPeek, int Advance) : IntoValueString {
    public record Null(Matcher Rule, Diagnostic Diagnostic, int MaxPeek) : Matchlet(Rule, Diagnostic, MaxPeek, 0) {
        public override ValueString IntoValueString()
            => new ValueStringBuilder.Struct("Null", Rule.Name, Diagnostic.ToString()).Build();
    }

    public record Token(Matcher Rule, Diagnostic Diagnostic, Tokenize.Token Value, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public override ValueString IntoValueString()
            => Value.IntoValueString();
    }

    public record List(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public Matchlet this[int idx] => Matches[idx];

        public override ValueString IntoValueString()
            => new ValueStringBuilder.Tuple("List", Rule.Name, Diagnostic.ToString())
                .ArrayField([..Matches])
                .Build();
    }

    public record Or(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {
        public Matchlet Successful => Matches[Matches.Count-1];

        public override ValueString IntoValueString()
            => new ValueStringBuilder.Tuple("List", Rule.Name, Diagnostic.ToString())
                .ArrayField([..Matches])
                .Build();
    }

    public record Repeat(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek, int Advance) : Matchlet(Rule, Diagnostic, MaxPeek, Advance) {

        public override ValueString IntoValueString()
            => new ValueStringBuilder.Tuple("List", Rule.Name, Diagnostic.ToString())
                .ArrayField([..Matches])
                .Build();
    }

    public record Reference(Matcher Rule, Diagnostic Diagnostic, Matchlet Child) : Matchlet(Rule, Diagnostic, Child.MaxPeek, Child.Advance) {
        public override ValueString IntoValueString()
            => Child.IntoValueString();
    }

    public record Error(Matcher Rule, Diagnostic Diagnostic, List<Matchlet> Matches, int MaxPeek) : Matchlet(Rule, Diagnostic, MaxPeek, 0) {
        public override ValueString IntoValueString()
            => new ValueStringBuilder.Tuple("List", Rule.Name, Diagnostic.ToString())
                .ArrayField([..Matches])
                .Build();
    }

    public record Empty(Matcher Rule, Diagnostic Diagnostic, Matchlet? FailedMatch, int MaxPeek) : Matchlet(Rule, Diagnostic, MaxPeek, 0) {
        public override ValueString IntoValueString() {
            var builder = new ValueStringBuilder.Tuple("Empty", Rule.Name, Diagnostic.ToString());
            if (FailedMatch != null)
                builder.Field(FailedMatch);
            return builder.Build();
        }
    }

    public abstract ValueString IntoValueString();

    public T As<T>() where T : Matchlet {
        if (this is not T)
            throw new ArgumentException($"Expected {typeof(T).Name}, got {GetType().Name}");
        return (T) this;
    }
}
