# Editor UI

In Unity, you can customize editor ui by extending one of the three classes: `EditorWindow`, `Editor` and `PropertyDrawer`.

Lithium provides its own alternatives that behave in the same way, except that is uses Lithium to create user interface.

## Composable Window

To create editor window, extend `ComposableWindow` class and implement `IComponent Layout()` method:

```csharp
class TestWindow: ComposableWindow:
{
    protected override IComponent Layout() => Text("Text");
}
```

You can also customize the window name by overriding `protected virtual string WindowName { get; }` property.

## Composable Editor

To create inspector for given unity object, extend `ComposableEditor` class and implement `IComponent Layout()` method:

```csharp
[CustomEditor(typeof(TestComponent))]
class TestEditor: ComposableEditor
{
    protected override IComponent Layout() => Col(Inspector(this), Text("Text"));
}
```

## Composable Property Drawer

To create property drawer for your type, extend `ComposablePropertyDrawer` class and implement `IComponent Layout()` method:

```csharp
[CustomPropertyDrawer(typeof(TestType))]
public class TestTypeDrawer: ComposablePropertyDrawer
{
    protected override IComponent Layout() => Text("Property");
}
```

## Hiding context

If you want to hide your window, editor or drawer from being visible in the debugger, you can also override `protected virtual bool HideContext { get; }`.

## What's next?

Now, that you know how to make ui for editor, [you can learn how to do it in the game](./runtime.md).