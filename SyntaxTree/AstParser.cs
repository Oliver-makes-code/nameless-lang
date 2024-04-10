using Lang.SyntaxTree.Types;
using Lang.Tokenize;
using Lang.Util;
using SymbolType = Lang.Tokenize.TokenType.Symbol;

namespace Lang.SyntaxTree;

public static class AstParser {
    public static readonly SymbolType[][] InfixPrecedenceTable = [
        [SymbolType.Mul, SymbolType.Div, SymbolType.Mod],
        [SymbolType.Add, SymbolType.Sub],
        [SymbolType.Shl, SymbolType.Shr],
        [SymbolType.AngleOpen, SymbolType.AngleClose, SymbolType.GreaterEqual, SymbolType.LessEqual, SymbolType.Equal, SymbolType.NotEqual],
        [SymbolType.BitAnd],
        [SymbolType.BitXor],
        [SymbolType.BitOr],
        [SymbolType.BoolAnd],
        [SymbolType.BoolXor],
        [SymbolType.BoolOr],
    ];

    public static void GenerateAst(string path, string source) {
        var tokens = new Lookahead<Token>(Tokenizer.Tokenize(path, source));
        var expr = ParseExpression(tokens);
        while (expr is Option<Expr>.Some { Value: var value }) {
            Console.WriteLine(value);
            expr = ParseExpression(tokens);
        }
    }

    private static Option<Expr> ParseExpression(Lookahead<Token> tokens) {
        return ParseLiteral(tokens);
    }

    private static Option<Expr> ParseOperation(Lookahead<Token> tokens, int precedence = 0) {
        throw new NotImplementedException("fuck you");
    }

    private static Option<Expr> ParseLiteral(Lookahead<Token> tokens) {
        var peek = tokens.Peek(0);

        switch (peek.Type) {
            case TokenType.Number<double> d:
                tokens.Next();
                return new Option<Expr>.Some(new NumberLiteral<double>(d.Value, peek.Src));
            case TokenType.Number<long> l:
                tokens.Next();
                return new Option<Expr>.Some(new NumberLiteral<long>(l.Value, peek.Src));
            case TokenType.String s:
                tokens.Next();
                return new Option<Expr>.Some(new StringLiteral(s.Value, peek.Src));
            case var b when b == TokenType.Keyword.True || b == TokenType.Keyword.False:
                tokens.Next();
                return new Option<Expr>.Some(new BooleanLiteral(b == TokenType.Keyword.True, peek.Src));
            default:
                return Option<Expr>.None;
        }
    }
}
