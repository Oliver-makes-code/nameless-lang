using Lang.Tokenize;

namespace Lang.Util;

public class Lookahead<T> {
    private readonly Queue<T> Queue = [];
    private readonly IEnumerator<T> Enumerator;

    public Lookahead(IEnumerable<T> enumerable) {
        Enumerator = enumerable.GetEnumerator();
        Enumerator.MoveNext();
    }

    public T Peek(int count) {
        if (Queue.Count > count)
            return Queue.ElementAt(count);
        while (Queue.Count <= count) {
            Queue.Enqueue(Enumerator.Current);
            Enumerator.MoveNext();
        }
        return Queue.ElementAt(count);
    }

    public T Next() {
        if (Queue.Count == 0) {
            var value = Enumerator.Current;
            Enumerator.MoveNext();
            return value;
        }
        return Queue.Dequeue();
    }
}
