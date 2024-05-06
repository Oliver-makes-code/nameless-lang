# Traits

Traits are basically our form of interfaces,
they compile down into an interface,
but they have certain special properties in our language.

Traits store a set of functions that objects implement.
All trait implementations on types defined within your compilation unit implement the underlying type natively,
otherwise the compiler will generate a wrapper class containing the definitions.
You cannot implement a trait from an external compilation unit on another external compilation unit.

Traits are defined similarly to how they're defined in Rust

```
trait YourTrait {
    void DoSomething();
}
```

Traits are implemented using the `impl` keyword

For any object (including classes):

```
impl YourTrait for SomeClass {
    void DoSomething() {
        print("owo");
    }
}
```

For classes:

```
class SomeClass impl YourTrait {
    void DoSomething() {
        print("owo");
    }
}
```
