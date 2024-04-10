namespace Lang.Util;

public record Option<T> {
    public record Some(T Value) : Option<T>;
    public static readonly Option<T> None = new();

    public bool IsSome => this is Some;
    public bool IsNone => this !is Some;

    private Option() {}

    public T Unwrap()
        => this is Some s ? s.Value : throw new ArgumentNullException("Called unwrap on None optional.");
    
    public T UnwrapOr(T value)
        => this is Some s ? s.Value : value;
}
