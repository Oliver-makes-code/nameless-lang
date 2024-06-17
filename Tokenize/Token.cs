using System.Data;
using System.Diagnostics.CodeAnalysis;
using Lang.SyntaxTree.Rules;
using Lang.Util;

namespace Lang.Tokenize;

public record Token(TokenType Type, StringView Src) {
    public override string? ToString()
        => Type.ToString();
}

public record TokenType {
    public readonly string Name;

    public readonly Matcher Matcher;
    
    private TokenType(string name) {
        Name = name;

        Matcher = new TokenValueMatcher(Name, this);
    }

    public record Keyword : TokenType {
        public static readonly Dictionary<string, Keyword> Values = [];

        // Type Definitions / Generics
        public static readonly Keyword Struct = new("struct");
        public static readonly Keyword Enum   = new("enum");
        public static readonly Keyword Where  = new("where");

        // Modifiers
        public static readonly Keyword Pub = new("pub");
        public static readonly Keyword Mut = new("mut");
        public static readonly Keyword Ref = new("ref");

        // Variables / Functions
        public static readonly Keyword Var = new("var");
        public static readonly Keyword As  = new("as");
        public static readonly Keyword Func = new("func");
        public static readonly Keyword Discard = new("_");
        
        // Control Flow
        public static readonly Keyword If    = new("if");
        public static readonly Keyword Else  = new("else");
        public static readonly Keyword For   = new("for");
        public static readonly Keyword While = new("while");
        public static readonly Keyword Loop  = new("loop");
        public static readonly Keyword In    = new("in");
        public static readonly Keyword Is    = new("is");
        public static readonly Keyword Match = new("match");

        // Boolean
        public static readonly Keyword True  = new("true");
        public static readonly Keyword False = new("false");

        // Builtin types
        public static readonly Keyword Bool = new("bool");
        public static readonly Keyword I8   = new("i8");
        public static readonly Keyword I16  = new("i16");
        public static readonly Keyword I32  = new("i32");
        public static readonly Keyword I64  = new("i64");
        public static readonly Keyword Ui8  = new("ui8");
        public static readonly Keyword Ui16 = new("ui16");
        public static readonly Keyword Ui32 = new("ui32");
        public static readonly Keyword Ui64 = new("ui64");
        public static readonly Keyword F32  = new("f32");
        public static readonly Keyword F64  = new("f64");

        public readonly string Value;

        private Keyword(string value) : base(value) {
            Value = value;
            Values[value] = this;
        }

        public static bool IsValue(string value, [NotNullWhen(true)] out Keyword? keyword)
            => Values.TryGetValue(value, out keyword);

        public override string ToString()
            => $"Keyword(\"{Value}\")";
    }

    public record Symbol : TokenType {
        public static Symbol[] Values { get; private set; } = [];

        public static readonly Symbol Semicolon = new("Semicolon", ";");
        public static readonly Symbol Arrow = new("Arrow", "=>");

        public static readonly Symbol BracketOpen  = new("BracketOpen",  "[");
        public static readonly Symbol BracketClose = new("BracketClose", "]");
        public static readonly Symbol BraceOpen    = new("BraceOpen",    "{");
        public static readonly Symbol BraceClose   = new("BraceClose",   "}");
        public static readonly Symbol ParenOpen    = new("ParenOpen",    "(");
        public static readonly Symbol ParenClose   = new("ParenClose",   ")");
        public static readonly Symbol AngleClose   = new("AngleClose",   ">");
        public static readonly Symbol AngleOpen    = new("AndleOpen",    "<");

        public static readonly Symbol Not     = new("Not",     "!");
        public static readonly Symbol Add     = new("Add",     "+");
        public static readonly Symbol Sub     = new("Sub",     "-");
        public static readonly Symbol Mul     = new("Mul",     "*");
        public static readonly Symbol Div     = new("Div",     "/");
        public static readonly Symbol Mod     = new("Mod",     "%");
        public static readonly Symbol Inc     = new("Inc",     "++");
        public static readonly Symbol Dec     = new("Dec",     "--");
        public static readonly Symbol Shl     = new("Shl",     "<<");
        public static readonly Symbol Shr     = new("Shr",     ">>");
        public static readonly Symbol BitOr   = new("BitOr",   "|");
        public static readonly Symbol BitAnd  = new("BitAnd",  "&");
        public static readonly Symbol BitXor  = new("BitXor",  "^");
        public static readonly Symbol BoolOr  = new("BoolOr",  "||");
        public static readonly Symbol BoolAnd = new("BoolAnd", "&&");
        public static readonly Symbol BoolXor = new("BoolXor", "^^");

        public static readonly Symbol Assign        = new("Assign",    "=");
        public static readonly Symbol AddAssign     = new("AddAssign", "+=");
        public static readonly Symbol SubAssign     = new("SubAssign", "-=");
        public static readonly Symbol MulAssign     = new("MulAssign", "*=");
        public static readonly Symbol DivAssign     = new("DivAssign", "/=");
        public static readonly Symbol ModAssign     = new("ModAssign", "%=");
        public static readonly Symbol ShlAssign     = new("ShlAssign", "<<=");
        public static readonly Symbol ShrAssign     = new("ShrAssign", ">>=");
        public static readonly Symbol BitAndAssign  = new("BitAndAssign", "&=");
        public static readonly Symbol BitOrAssign   = new("BitOrAssign",  "|=");
        public static readonly Symbol BitXorAssign  = new("BitXorAssign", "^=");
        public static readonly Symbol BoolAndAssign = new("BoolAndAssign", "&&=");
        public static readonly Symbol BoolOrAssign  = new("BoolOrAssign",  "||=");
        public static readonly Symbol BoolXorAssign = new("BoolXorAssign", "^^=");

        public static readonly Symbol Equal        = new("Equal",        "==");
        public static readonly Symbol NotEqual     = new("NotEqual",     "!=");
        public static readonly Symbol GreaterEqual = new("GreaterEqual", ">=");
        public static readonly Symbol LessEqual    = new("LessEqual",    "<=");

        public static readonly Symbol Comma = new("Comma", ",");

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

    public record Number<T>(T Value) : TokenType("Number") where T : struct {
        public static readonly Matcher TypeMatcher = new TokenTypeMatcher<Number<T>>("Number");

        public override string ToString()
            => $"Number({Value})";
    }

    public record String(string Value) : TokenType("String") {
        public static readonly Matcher TypeMatcher = new TokenTypeMatcher<String>("String");

        public override string ToString()
            => $"String(\"{Value}\")";
    }

    public record Identifier(string Value) : TokenType("Identifier") {
        public static readonly Matcher TypeMatcher = new TokenTypeMatcher<Identifier>("Identifier");

        public override string ToString()
            => $"Identifier(\"{Value}\")";
    }

    public static readonly TokenType Eof = new("Eof");

    public override string? ToString()
        => Name ?? base.ToString();
}
