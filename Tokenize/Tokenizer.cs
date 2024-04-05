using Lang.Util;

namespace Lang.Tokenize;

public static class Tokenizer {
    public static IEnumerable<Token> Tokenize(string source) {
        var parser = new StringParser(source);

        while (parser.Current() != null)
            yield return ParseSignleToken(parser);

        parser.Checkout();
        yield return new Token(TokenType.Eof, parser.Commit());
    }

    private static Token ParseSignleToken(StringParser parser) {
        parser.Checkout();

        if (parser.IsAny([' ', '\t'])) {
            do
                parser.Next();
            while (parser.IsAny([' ', '\t']));
            return new Token(TokenType.Whitespace, parser.Commit());
        }

        if (TokenType.Symbol.Is(parser, out var symbol))
            return new Token(symbol, parser.Commit());

        if (parser.IsFunc(c => char.IsAsciiLetter(c) || c == '_')) {
            parser.ConsumeFunc(c => char.IsAsciiLetterOrDigit(c) || c == '_');
            var view = parser.Commit();
            var value = view.Value;
            if (TokenType.Keyword.Is(value, out var keyword))
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

                    value = parser.Commit();
                    return new Token(new TokenType.Number<double>(double.Parse(value.Value)), value);
                }
                parser.Rollback();
            }

            value = parser.Commit();
            return new Token(new TokenType.Number<long>(long.Parse(value.Value)), value);
        }

        throw new NotImplementedException();
    }
}
