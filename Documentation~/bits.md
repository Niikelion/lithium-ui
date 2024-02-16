# Quick bits
This section contains minimal samples of code for different Lithium features. You can find complete examples [here](../Samples~/README.md).

1. [Starting points](#starting-points)
2. [State](#state)
3. [Lifecycle events](#lifecycle-events)
4. [Optimization](#optimization)

## Starting points

### EditorWindow
```csharp
public class TestWindow: ComposableWindow
{
    [MenuItem("Lithium/Examples/TestWindow")]
    public static void ShowWindow() => GetWindow<TestWindow>();
    
    protected override string WindowName => "Window Test";
    
    protected override IComponent Layout() => [...]
}
```

### PropertyDrawer

```csharp
[CustomPropertyDrawer(typeof(ExampleType))]
public class ExampleTypePropertyDrawer: ComposablePropertyDrawer
{
    protected override IComponent Layout(SerializedProperty property) => [...]
}
```

### Editor

```csharp
[CustomEditor(typeof(ExampleType))]
public class ExampleTypeEditor: ComposableEditor
{
    protected override IComponent Layout() => [...]
}
```

### VisualElement

```csharp
public class SomeView: VisualElement
{
    public SomeView()
    {
        var renderer = new ComponentRenderer(Layout(), "SomeView");
        Add(renderer.UpdateAndRender());
    }
    
    private void IComponent Layout() => [...]
}
```

## State

### Simple value

```csharp
var counter = ctx.Remember(0);
[...]
counter.Value++;
```

### Value with factory

```csharp
var content = ctx.RememberF(() => someExpensiveFunction(data));
```

### Value reference

```csharp
var contentRef = ctx.RememberRef(content);
[...]
contentRef.Value = secondContent; //does not trigger ui render
```

### Value reference with factory

```csharp
var contentRef = ctx.RememberRef(() => someExpensiveFactory(data));
```

### List

```csharp
var list = ctx.RememberList(Enumerable.Empty<int>());
[...]
list.Add(4);
```

### Dictionary

```csharp
var dict = ctx.RememberDictionary(new Dictionary<string, int>());
[...]
dict.Add("test", 6);
```

### Context

```csharp
ctx.ProvideContext<int>(6);
[...]
var numberCtx = ctx.UseContext<int>();
```

## Lifecycle events

### Init
```csharp
ctx.OnInit(() => {
    [...]
});
```

### Destroy
```csharp
ctx.OnDestroy(() => {
    [...]
});
```

### Init and Destroy combined

```csharp
ctx.OnInit(() => {
    [...]
    return () => {
        [...]
    };
});
```

## Optimization

### Batching updates

```csharp
{
    using var _ = ctx.BatchOperations();
    var1.Value = 5;
    var2.Value = "test";
}
```

or

```csharp
ctx.BatchOperations(() => {
    var1.Value = 5;
    var2.Value = "test";
});
```
