using System.Diagnostics.CodeAnalysis;
using Lang.Util;

namespace Lang.Tokenize;

public record Token(TokenType Type, StringView Src) {
    public override string? ToString()
        => Type.ToString();
}

public record TokenType {
    private readonly string? Name;

    private TokenType() {}

    private TokenType(string name) {
        Name = name;
    }

    public record Keyword : TokenType {
        public static readonly Dictionary<string, Keyword> Values = [];

        public static readonly Keyword Variants = new("variants");
        public static readonly Keyword Struct   = new("struct");

        public static readonly Keyword Mut = new("mut");
        public static readonly Keyword Var = new("var");
        public static readonly Keyword As  = new("As");

        public static readonly Keyword If   = new("if");
        public static readonly Keyword Else = new("else");

        public static readonly Keyword For   = new("for");
        public static readonly Keyword In    = new("in");
        public static readonly Keyword While = new("while");
        public static readonly Keyword Loop  = new("loop");

        public static readonly Keyword When = new("when");

        public static readonly Keyword True  = new("true");
        public static readonly Keyword False = new("false");

        public readonly string Value;
        private Keyword(string value) {
            Value = value;
            Values[value] = this;
        }

        public static bool Is(string value, [NotNullWhen(true)] out Keyword? keyword)
            => Values.TryGetValue(value, out keyword);

        public override string ToString()
            => $"Keyword(\"{Value}\")";
    }

    public record Symbol : TokenType {
        public static Symbol[] Values { get; private set; } = [];

        public static readonly Symbol BracketOpen  = new("BracketOpen",  "[");
        public static readonly Symbol BracketClose = new("BracketClose", "]");
        public static readonly Symbol BraceOpen    = new("BraceOpen",    "{");
        public static readonly Symbol BraceClose   = new("BraceClose",   "}");
        public static readonly Symbol ParenOpen    = new("ParenOpen",    "(");
        public static readonly Symbol ParenClose   = new("ParenClose",   ")");
        public static readonly Symbol AngleClose   = new("AngleClose",   ">");
        public static readonly Symbol AngleOpen    = new("AndleOpen",    "<");

        public static readonly Symbol Or  = new("Or" , "|");
        public static readonly Symbol And = new("And", "&");
        public static readonly Symbol Xor = new("Xor", "^");
        public static readonly Symbol Not = new("Not", "!");
        public static readonly Symbol Add = new("Add", "+");
        public static readonly Symbol Sub = new("Sub", "-");
        public static readonly Symbol Mul = new("Mul", "*");
        public static readonly Symbol Div = new("Div", "/");
        public static readonly Symbol Mod = new("Mod", "%");
        public static readonly Symbol Pow = new("Pow", "^^");

        public static readonly Symbol Assign    = new("Assign",    "=");
        public static readonly Symbol AndAssign = new("AndAssign", "&=");
        public static readonly Symbol OrAssign  = new("OrAssign",  "|=");
        public static readonly Symbol XorAssign = new("XorAssign", "^=");
        public static readonly Symbol AddAssign = new("AddAssign", "+=");
        public static readonly Symbol SubAssign = new("SubAssign", "-=");
        public static readonly Symbol MulAssign = new("MulAssign", "*=");
        public static readonly Symbol DivAssign = new("DivAssign", "/=");
        public static readonly Symbol ModAssign = new("ModAssign", "%=");
        public static readonly Symbol PowAssign = new("PowAssign", "^^=");

        public static readonly Symbol Equal        = new("Equal",        "==");
        public static readonly Symbol NotEqual     = new("NotEqual",     "!=");
        public static readonly Symbol GreaterEqual = new("GreaterEqual", ">=");
        public static readonly Symbol LessEqual    = new("LessEqual",    "<=");

        public static readonly Symbol Dot         = new("Dot",         ".");
        public static readonly Symbol Range       = new("Range",       "..");
        public static readonly Symbol RangeFrom   = new("RangeFrom",   "<..");
        public static readonly Symbol RangeTo     = new("RangeTo",     "..=");
        public static readonly Symbol RangeFromTo = new("RangeFromTo", "<..=");

        public readonly string Value;
        private Symbol(string name, string value) : base(name) {
            Value = value;
            Values = [..Values, this];
            Values = [..Values.OrderBy(it => -it.Value.Length)];
        }

        public static bool Is(StringParser parser, [NotNullWhen(true)] out Symbol? symbol) {
            foreach (var value in Values) {
                if (parser.Is(value.Value)) {
                    symbol = value;
                    return true;
                }
            }
            symbol = null;
            return false;
        }

        public override string ToString()
            => $"Symbol({Name}: \"{Value}\")";
    }
    
    public record Number<T>(T Value) : TokenType where T : struct {
        public override string ToString()
            => $"Number({Value})";
    }

    public record String(string Value) : TokenType {
        public override string ToString()
            => $"String(\"{Value}\")";
    }

    public record Identifier(string Value) : TokenType{
        public override string ToString()
            => $"Identifier(\"{Value}\")";
    }

    public static readonly TokenType Eof = new("Eof");

    public static readonly TokenType Whitespace = new("Whitespace");

    public static readonly TokenType Newline = new("Newline");

    public override string? ToString()
        => Name ?? base.ToString();
}
