namespace Lang.Util;

public class ParseException(string message, StringView view) : Exception(
$@"{message}
    In {view.FilePath}({view.LineNum},{view.ColNum})
        {view.Line}
        " + new string(' ', view.ColNum-1) + "^ here\n"
);

public class TokenizeException(StringView view) : ParseException($"Unexpected character", view);
