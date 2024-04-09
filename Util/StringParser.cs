namespace Lang.Util;

public readonly struct StringView {
    public readonly string FilePath;
    public readonly string Source;
    public readonly string Value;
    public readonly string Line;
    public readonly int StartIdx;
    public readonly int EndIdx;
    public readonly int LineNum;
    public readonly int ColNum;
    public readonly int LineStart;
    public readonly int LineEnd;

    public StringView(string path, string source, int startIdx, int endIdx) {
        FilePath = path;
        Source = source;
        StartIdx = startIdx;
        EndIdx = endIdx;
        Value = Source[StartIdx..EndIdx];

        var split = Source[..StartIdx].Split('\n');
        LineNum = split.Length;
        ColNum = split[LineNum-1].Length + 1;
        LineStart = StartIdx - ColNum + 1;
        Line = Source[LineStart..].Split('\n')[0]!;
        LineEnd = Line.Length - 1;
    }

    public override string ToString()
        => Value;
}

public class StringParser {
    public readonly string Source;
    public readonly string FilePath;
    private readonly Stack<int> Snapshots = [];

    private int idx = 0;

    public StringParser(string path, string source) {
        Source = source;
        FilePath = path;
    }

    public void Checkout()
        => Snapshots.Push(idx);

    public void Rollback()
        => idx = Snapshots.Pop();

    public StringView Commit() {
        int startIdx = Snapshots.Pop();

        return new(FilePath, Source, startIdx, idx);
    }

    public char? Current() {
        try {
            return Source[idx];
        } catch (IndexOutOfRangeException) {
            return null;
        }
    }

    public char? Next() {
        idx++;
        if (idx >= Source.Length)
            return null;
        return Source[idx];
    }

    public char? Last() {
        if (idx == 0)
            return null;
        return Source[--idx];
    }

    public bool Is(string value) {
        Checkout();
        for (int i = 0; i < value.Length; i++) {
            if (Is(value[i])) {
                Next();
                continue;
            }
            Rollback();
            return false;
        }
        Commit();
        return true;
    }

    public bool Is(char value)
        => Current() == value;

    public bool IsAny(string[] values)
        => values.OrderBy(s => -s.Length).Any(Is);

    public bool IsAny(char[] values)
        => values.Any(Is);

    public bool IsFunc(Func<char, bool> f) {
        char? curr = Current();
        return curr is char c && f(c);
    }

    public void ConsumeFunc(Func<char, bool> f) {
        while (IsFunc(f))
            Next();
    }
}
