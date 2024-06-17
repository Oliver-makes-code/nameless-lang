# Objects

We have standard OO classes, in order to interoperate with the host runtime.
These are only ever reference types.

```
pub class YourClass {
    pub i32 x;
    pub string y;
}
```

We also have structs, which are value types

```
pub struct YourStruct {
    pub i32 x;
    pub string y;
}
```

Objects are constructed at a single moment, with all values being initialized at once

```
var value = YourClass {
    x: 15,
    y: "uwu"
};
```

If you're extending a class, you can initialize that class with the `base` property

```
class ChildClass : YourClass {
    pub bool z;
}

var value = YourClass {
    base: {
        x: 15,
        y: "owo"
    },
    z: false
};
```

If you're constructing an externally defined object, you use the same syntax

```
// C#
pub class ExternalClass {
    pub ExternalClass(int x, int y) {
        Console.WriteLine(x);
        Console.WriteLine(y);
    }
}

// Our language
var value = ExternalClass {
    x: 1,
    y: 2
};

// Turns into
var value = new ExternalClass(1, 2);
```

We have monadic types, made using the `variants` keyword

```
where TOk, TErr
variants Result {
    Ok(TOk),
    Err(TErr)
}
```
