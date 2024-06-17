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
        => this is Err { Value: var o } ? o : throw new ArgumentNullException("Called UnwrapErr on Ok Result.");

    public static Result<T, E> FromOk(T t)
        => t;

    public static Result<T, E> FromErr(E e)
        => e;

    public static implicit operator Result<T, E> (T t)
        => new Ok(t);
    
    public static implicit operator Result<T, E> (E e)
        => new Err(e);
}
