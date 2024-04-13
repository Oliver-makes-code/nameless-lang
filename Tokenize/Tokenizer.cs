using Lang.Util;

namespace Lang.Tokenize;

public static class Tokenizer {
    public static IEnumerable<Token> Tokenize(string path, string source) {
        var parser = new StringParser(path, source);

        while (parser.Current() != null) {
            var value = ParseSignleToken(parser);
            if (value != null)
                yield return value;
        }

        parser.Checkout();
        yield return new Token(TokenType.Eof, parser.Commit());
    }

    private static Token? ParseSignleToken(StringParser parser) {
        parser.Checkout();

        if (parser.IsAny([' ', '\t', '\n'])) {
            do
                parser.Next();
            while (parser.IsAny([' ', '\t']));
            parser.Commit();
            parser.Checkout();

            if (parser.Current() == null)
                return null;
        }

        if (TokenType.Symbol.Is(parser, out var symbol))
            return new Token(symbol, parser.Commit());

        if (parser.IsFunc(c => char.IsAsciiLetter(c) || c == '_')) {
            parser.ConsumeFunc(c => char.IsAsciiLetterOrDigit(c) || c == '_');
            var view = parser.Commit();
            var value = view.Value;
            if (TokenType.Keyword.IsValue(value, out var keyword))
                return new Token(keyword, view);
            return new Token(new TokenType.Identifier(value), view);
        }

        if (parser.IsFunc(char.IsAsciiDigit)) {
            parser.ConsumeFunc(char.IsAsciiDigit);

            StringView value;

            if (parser.Is('.')) {
                parser.Checkout();
                parser.Next();
                if (parser.IsFunc(char.IsAsciiDigit)) {
                    parser.ConsumeFunc(char.IsAsciiDigit);

                    parser.Commit();
                    value = parser.Commit();
                    return new Token(new TokenType.Number<double>(double.Parse(value.Value)), value);
                }
                parser.Rollback();
            }

            value = parser.Commit();
            return new Token(new TokenType.Number<long>(long.Parse(value.Value)), value);
        }

        if (parser.Is('"')) {
            do
                parser.Next();
            while (!parser.Is('"'));
            parser.Next();
            var value = parser.Commit();
            return new Token(new TokenType.String(value.Value[1..(value.Value.Length-1)]), value);
        }
        
        parser.Next();
        throw new TokenizeException(parser.Commit());
    }
}
