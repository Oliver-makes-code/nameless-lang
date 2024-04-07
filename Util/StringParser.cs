namespace Lang.Util;

public struct StringView {
    public (int startIdx, int endIdx) location;
    public string source;

    public string value => source[location.startIdx..location.endIdx];

    public override string ToString()
        => value;
}

public class StringParser {
    public readonly string Source;
    private readonly Stack<int> Snapshots = [];

    private int idx = 0;

    public StringParser(string source) {
        Source = source;
    }

    public void Checkout()
        => Snapshots.Push(idx);

    public void Rollback()
        => idx = Snapshots.Pop();

    public StringView Commit() {
        int startIdx = Snapshots.Pop();

        return new() {
            location = (startIdx, idx),
            source = Source
        };
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
