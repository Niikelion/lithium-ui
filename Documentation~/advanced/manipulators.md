# Manipulators

## Description

Lithium manipulators are almost identical to the [UI Toolkit manipulators](https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-manipulators.html).
The first difference is that built-in manipulators in Lithium provide batching of updates inside callbacks, so that you don't have to worry about modifying multiple variables inside them.
The second difference is that they usually don't expose elements from the underlying UI Toolkit layer.

## Good practises

Easiest way to make a custom manipulator is to inherit from `UnityEngine.UIElements.Manipulator` and implement `void RegisterCallbacksOnTarget()` and `void UnregisterCallbacksFromTarget()` methods:

```csharp
public class Repaintable : Manipulator
{
    protected override void RegisterCallbacksOnTarget() {}
    protected override void UnregisterCallbacksFromTarget() {}
}
```

To make callbacks consistent with built-in ones, wrap them with `UI.Li.Utils.EventUtils.WrapEventHandler`:

```csharp
[NotNull] private readonly Action<MeshGenerationContext> onRepaint;

public Repaintable([NotNull] Action<MeshGenerationContext> onRepaint) => this.onRepaint = EventUtils.WrapEventHandler(onRepaint);
```

When your callbacks have more than one argument, you need to wrap them yourself:

```csharp
private Scroll(/* ... */ Action<float, Orientation> onScroll /* ... */)
{
    /* ... */
    var defer = ComponentState.GetDeferrer();
    this.onScroll = (value, orientation) => defer(() => onScroll(value, orientation));
}
```

A couple of remarks:
* `ComponentState.GerDeferrer` returns function that takes argument-less delegate and runs it compressing all updates that may occur during execution into at most one update
* since `defer` accepts argument-less delegate, we need to create function without arguments that calls our callback and provides arguments from outside, hence `() => onScroll(value, orientation)`
* arguments are only available in out outer callback, so we need to create function calling `onScroll` inside outer function, hence `(value, orientation) => defer(() => ...)`

Complete Repaintable example:

```csharp
public class Repaintable : Manipulator
{
    [NotNull] private readonly Action<MeshGenerationContext> onRepaint;

    public Repaintable([NotNull] Action<MeshGenerationContext> onRepaint) => this.onRepaint = EventUtils.WrapEventHandler(onRepaint);
    
    protected override void RegisterCallbacksOnTarget() => target.generateVisualContent += onRepaint;
    protected override void UnregisterCallbacksFromTarget() => target.generateVisualContent -= onRepaint;
}
```

## More Examples

More manipulator implementations can be found [here](../../Runtime/Utils/Events.cs).

## What's next?

Equipped with the knowledge of manipulators, [you can learn about implementing custom elements](../advanced/custom-elements.md).