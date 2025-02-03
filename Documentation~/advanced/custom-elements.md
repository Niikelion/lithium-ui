# Custom UI Toolkit Elements

## Intro

Although Lithium provides substitutes for most of the UI Toolkit elements, there may be cases where you need to use some element that is not available in Lithium (for example from 3rd party library).
Fortunately, Lithium provides simple way to achieve that: `UI.Li.Common.Element`.

## Exposing Element

### Wrapper

First, we need to create wrapper class:

```csharp
public class Button: Element<UnityEngine.UIElements.Button>
{
    [NotNull] private readonly IComponent content;
    [NotNull] private readonly Action onClick;
    
    public Button([NotNull] Action onClick, [NotNull] IComponent content, IEnumerable<IManipulator> manipulators) : base(manipulators)
    {
        this.content = content;
        this.onClick = EventUtils.WrapEventHandler(onClick);
    }
}
```

Note, that generic argument of `Element<>` is the type of the element that you want to expose.

`manipulators` argument can be in omitted, although we recommend adding it and passing it to base constructor to make it easier to add manipulators.

To learn why we used `EventUtils.WrapEventHandler` read about [manipulators](manipulators.md).

### Preparing Element Before Render

At this point our `Button` can be used as a component, and it would render as a simple button, but we still need to attach callback and children to it.

To do this, override `UIButton PrepareElement(UIButton button)` method:

```csharp
protected override UIButton PrepareElement(UIButton button)
{
    var contentElement = content.Render();
    
    button.clicked += onClick;
    CompositionContext.ElementUserData.AppendCleanupAction(button, () => button.clicked -= onClick);
    
    if (button.childCount == 1 && button[0] == contentElement) return button;
    
    button.Clear();
    button.Add(contentElement);
    
    return button;
}
```

Note, that after attaching callback to the element, we add cleanup action to detach it from the element before it will be reused.

### Propagating Updates

Now, we are almost done. We just need to update our child component when we are being updated in `void OnState(CompositionContext context)`:

```csharp
protected override void OnState(CompositionContext context) => content.Recompose(context);
```

If you need to persist any state, use appropriate methods of `context` in `OnState` and save references to them in your class.

### Preventing State Errors

In case your state have different state based on component arguments, you need to implement `StateLayoutEquals` method to tell Lithium whether the shape changed or not.
For our button, it is as simple as checking if other component is the same type. Note, that usually it is not needed, as `Element<T>` checks it by default.

```csharp
public override bool StateLayoutEquals(IComponent other) => other is Button;
```

### Creating Function Wrapper

Last thing that we can improve it the syntax to use the component. Right now, to use it we need to do:

```csharp
public IComponent Foo() => new Button(() => {}, Text(""), Array.Empty<IManipulator>());
```

Create static class:

```csharp
public static class Components
{
    Button Button([NotNull] Action onClick, [NotNull] IComponent content, params IManipulator[] manipulators) => new Button(onClick, content, manipulators);
}
```

Which enables us to do:

```csharp
using static Components;

public class Foo
{
    public IComponent Bar() => Button(() => Debug.Log("bar"), Text("bar"));
}
```

But why stop here? We can add simple variant for convenience:

```csharp
public static class Components
{
    Button Button([NotNull] Action onClick, [NotNull] IComponent content, params IManipulator[] manipulators) => new Button(onClick, content, manipulators);
    Button Button([NotNull] Action onClick, [NotNull] string text, params IManipulator[] manipulators) => new Button(onClick, Text(text), manipulators);
}
```

Now, we can even do this:

```csharp
using static Components;

public class Foo
{
    public IComponent Bar() => Button(() => Debug.Log("bar"), "bar");
}
```

## More Examples

More element implementations can be found [here](../../Runtime/Common).

## What's next?

Next up on the list is, [learning about custom variable types](../advanced/custom-elements.md).