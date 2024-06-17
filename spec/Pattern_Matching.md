# PAttern Matching

Pattern matching is used for making control flow easier,
instead of checking value after value, you can check for a specific pattern.

```
if Optional.Some(var value) = DoSomething() {
    //...
}
```

There are a couple main types of pattern matching, including

- Object (Tuple)
    ```
    Object(var value)
    Object(_)
    Object(true)
    ```
- Object (Struct)
    ```
    Object { var value }
    Object { value: _ }
    Object { value: var t }
    ```
- Value
    ```
    15
    "Hello, World!"
    ```
- Array Deconstruction
    ```
    [var first, var second, var third, ..._]
    [var first, ...var others, var last]
    [..._, var last]
    [..._, true]
    ```
- Discard
    ```
    _
    ```
- Multi-value
    ```
    [true, ..._], [..._, true]
    1, 2
    "Hello", "World"
    Obj(true, var x), Obj(false, var x)
    ```
    - Note: Variables must have the same name and types when using multi-value patterns

Patterns are
