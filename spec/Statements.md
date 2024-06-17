# Statements

Statements are one of the following

- Expression
- If statement
    ```
    if thing {
        //...
    } else if otherThing {
        //...
    } else {
        //...
    }
    ```
- Match statement
    ```
    match thing {
        5 => DoSomething(),
        1, 2 => DoSomethingElse(),
        _ => Wildcard()
    }
    ```
- While statement
    ```
    while thing {
        DoSomething();
    }
    ```
- Loop statement
    ```
    loop {
        DoSomething();
    }
    ```
- For each statement
    ```
    for i in 0..5 {
        Print(i);
    }
    ```
- For statement
    ```
    for i32 i = 0, i < 5, i++ {
        Print(i);
    }
    ```
- Return statement
    ```
    return thing;
    ```
- Break statement
    ```
    break;
    ```