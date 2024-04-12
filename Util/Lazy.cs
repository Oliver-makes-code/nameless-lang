namespace Lang.Util;

public class LazyInitializer<T> {
    public delegate T Initializer();

    private readonly Initializer Init;

    private T? _value;

    public T Value {
        get {
            if (_value == null)
                _value = Init();
            return _value;
        }
    }

    public LazyInitializer(Initializer init) {
        Init = init;
    }


}