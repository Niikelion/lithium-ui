# Custom Variables

## Intro

Sometimes you need to store some other data structure than dictionary or list.
In that case, simply using `Remember` family of functions may not suffice, because element updates will not trigger component updates.

## Implementing IMutableValue

To solve that, we need to implement `IMutableValue` interface.
As the example, we will implement mutable array type:

```csharp
public class MutableArray<T>: IMutableValue
{
    public event Action OnValueChanged;
    
    private readonly T[] values;
    
    public MutableArray(T[] values) => this.values = values;
    
    public void Dispose() => OnValueChanged = null;
}
```

Now, all we have to do is implement array operations remembering to invoke the event every time we modify the array:

```csharp
public T this[int index]
{
    get => values[index];
    set
    {
        values[index] = value;
        OnValueChanged?.Invoke();
    }
}
public int Count => values.Count;
```

Now you can use your custom variable type:

```csharp
Use(new MutableArray<int>(new[] { 0, 1 }));
```

If you want to be fancy, you can create wrapper to make syntax easier:

```csharp
public static class Variables
{
    public static MutableArray<T> RememberArray(T[] array) => new MutableArray<T>(array);
}
```

And then:

```csharp
using static Variables;

/* ... */
var arr = RememberArray<int>(new[] { 0 , 1 });
```

And that's all!