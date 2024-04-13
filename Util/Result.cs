namespace Lang.Util;

public record Result<T, E> {
    public record Ok(T Value) : Result<T, E>;
    public record Err(E Value) : Result<T, E>;

    public bool IsOk => this is Ok;
    public bool IsErr => this is Err;

    private Result() {}

    public T Unwrap()
        => this is Ok { Value: var o } ? o : throw new ArgumentNullException("Called Unwrap on Err Result.");

    public T UnwrapOr(T value)
        => this is Ok { Value: var o } ? o : value;

    public E UnwrapErrOr(E value)
        => this is Err { Value: var o } ? o : value;
    
    public E UnwrapErr()
        =>this is Err { Value: var o } ? o : throw new ArgumentNullException("Called UnwrapErr on Ok Result.");
}
