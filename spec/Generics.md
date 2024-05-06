# Generics

Generics here are slightly different than other languages.
They're made more declarative, using `where` syntax.

```
where
    T : value + Add<T, T>
T AddSelf(T n) {
    return n + n;
}
```

Aside from the syntax, generics work mostly like they do in C#.
Instead of `struct` for value types, we use `value`.

Having multiple generics is defined as follows

```
where
    TFirst : Thing,
    TSecond : OtherThing
TFirst DoSomething(TSecond) {
    //...
}
```
