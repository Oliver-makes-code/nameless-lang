namespace Lang.Util;

public record Option<T> {
    public record Some(T Value) : Option<T>;
    public static readonly Option<T> None = new();

    public bool IsSome => this is Some;
    public bool IsNone => this is not Some;

    private Option() {}

    public T Unwrap()
        => this is Some s ? s.Value : throw new ArgumentNullException("Called Unwrap on None Optional.");
    
    public T UnwrapOr(T value)
        => this is Some s ? s.Value : value;
}
