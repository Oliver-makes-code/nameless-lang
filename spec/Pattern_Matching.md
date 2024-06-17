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
    Object(var value) = thing
    Object(_) = thing
    Object(true) = thing
    ```
- Object (Struct)
    ```
    Object { var value } = thing
    Object { value: _ } = thing
    Object { value: var t } = thing
    ```
- Value
    ```
    15 = thing
    "Hello, World!" = thing
    ```
- Array Deconstruction
    ```
    [var first, var second, var third, ..._] = thing
    [var first, ...var others, var last] = thing
    [..._, var last] == thing
    [..._, true] = thing
    ```
