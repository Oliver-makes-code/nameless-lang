# Objects

We also have structs, which are value types

```
pub struct YourStruct {
    pub i32 x;
    pub string y;
}
```

Objects are constructed at a single moment, with all values being initialized at once

```
var value = YourStruct {
    x: 15,
    y: "uwu"
};
```

We have monadic types, made using the `enum` keyword

```
where TOk, TErr
enum Result {
    Ok(TOk),
    Err(TErr)
}
```
