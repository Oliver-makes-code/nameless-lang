# Functions / Variables

Functions and variables are defined similarly to how they are in C#.
They use C-style declaration syntax.

```
u32 x = 5;

bool DoSomething() {}
```

Variables are immutable by default.
You can make them mutable by using the `mut` keyword.

```
mut i32 x = 5;

x++;
```

Value types can be passed by-reference into functions by using the `ref` keyword.

```
void DoSomething(ref u32 x) {
    Print(x);
}
```

If you wanna modify the referenced variable, you need to have a `ref mut`

```
void Increment(ref mut u32 x) {
    x++;
}
```
