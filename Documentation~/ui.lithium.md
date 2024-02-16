# Getting started

You can find quick startup guide [here](bootstrap.md).

# Core concepts

In lithium, ui hierarchy is built using `components`.

A `component` is any function with return type of `IComponent` or any type implementing this interface. This allows you to create multiple components inside a single class and easily compose them together to create desired hierarchy without unnecessary classes.

# State

Because of this approach, state of a component cannot be stored as a field of a class. In order to get around this problem, we need to wrap our component with `Component` object:

```cs
Component ComponentWithState() => new (state => {
    //body of the component goes here
});
```

Note, that we can access our state via `state` object of type `ComponentState`.



What can we store in a state? We have variables, callbacks and contexts.

### Variables

Variable is an instance of the `IMutableValue` interface. There is a couple of build-in types for your convenience:

* `MutableValue<T>` - triggers update when `Value` property is assigned.
* `MutableReference<T>` - same as `MutableValue` but does not trigger updates.
* `MutableList<T>` - triggers update when the list(or any of the elements if they implement `IMutableValue`) changes.
* `MutableDictionary<TKey, TValue>` - triggers update when the dictionary(or any of the elements if they implement `IMutableValue`) changes.

To make it easier to use, `ComponentState` provides a couple of methods to use state variables:

* `MutableValue<T> Remember<T>(T value)` - remembers value in the state on first render and returns current value of a variable.
* `MutableValue<T> RememberF<T>(Func<T> factory)` - same as `Remember`, but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.
* `ValueReference<T> RememberRef<T>(T value)` - remembers value in the state but does not track its changes.
* `ValueReference<T> RememberRefF<T>(Func<T> factory)` - same as `RememberRef` but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.
* `MutableList<T> RememberList<T>(IEnumerable<T> collection = null)` - same as `Remember`, but stores list instead of single value.
* `MutableList<T> RememberList<T>(Func<IEnumerable<T>> factory)` - same as `RememberList(IEnumerable<T>)`, but uses `factory` to create initial value on first render.
* `MutableDictionary<TKey, TValue> RememberDictionary(IDictionary<TKey, TValue> dictionary = null)` - same as `Remember`, but stores dictionary instead of single value.
* `MutableDictionary<TKey, TValue> RememberDictionary(Func<IDictionary<TKey, TValue>> factory)` - same as `RememberDictionary(IDictionary<TKey, TValue>)`, but uses `factory` to create initial value of first render.

### Callbacks

Available callback:

* `void OnInit(Action onInit)` - calls `onInit` on first render.
* `void OnDestroy(Action onDestroy)` - calls `onDestroy` before component is destroyed.
* `void OnInit(Func<Action> onInit)` - calls `onInit` on first render and then calls value returned by it before component is destroyed.

### Contexts

Lithium allows you to provide value of any type as a context that can be retrieved from any point further down in the hierarchy which is very useful for data propagation.

* `void ProvideContext<T>(T value)` - provides `value` as context of type `T` and passes it down the hierarchy.
* `T UseContext<T>()` - retrieves context of type `T` (throws exception if none is visible from this point in hierarchy).

### Ordering

Note, that every function except for `UseContext` needs to be called in the exactly same order on every render, so for example:

```cs
if (some_condition)
    ctx.Remember(5);
```

is not allowed when `some_condition` might change during component instance lifetime, because the `Remember` call may not happen with every render.
This is mainly because variables and callbacks are not named, so order of the calls is used as identification.
`UseContext` fetches global value so it is not affected by the order of the calls.

# Built-in components

To further simplify the process, lithium provides convenient utility class that serves as factory for other components.

Place this using right after imports to write `CU` instead of `CompositionUtils`:

```cs
using CU = UI.Lt.Utils.CompositionUtils;
```

Now, we can reduce:

```cs
Component Toggle() => new (state => {
    var toggleState = state.Remember(false);
    
    return UI.Lt.Common.Button.V(
        onClick: () => toggleState.Value = !toggleState.Value,
        content: toggleState.Value ? "On" : "Off"
    ); 
});
```

to:

```cs
Component Toggle() => new (state => {
    var toggleState = state.Remember(false);
    
    return CU.Button(
        onClick: () => toggleState.Value = !toggleState.Value,
        content: toggleState.Value ? "On" : "Off"
    ); 
});
```

`CompositionUtils` provides:

* `WithId(int id, IComposition composition)` - sets `id` for `composition`.

* `Switch`

* `Text`

* `Button`

* `Flex`

* `Foldout`

* `Box`

To learn more about how to use them, you can read the documentation for the members of `CompositionUtils` that should be accessible through your idee of choice.

## Hierarchy and state loss

Lithium has two ways of identifying components in the hierarchy - id and place in the children list of the parent component.

Because the latter one is not very reliable, lithium discards the state a component when it is not 100% sure it belongs to it.

To preserve the state, we can assign id to our elements to make them more distinguishable and better communicate to lithium when the state should be preserved and when discarded. Remember, that there can not be two components with the same id under same direct parent.

If you have a component that does not change its layout, you can mark it as static to further improve the state preservation:

```cs
Component StaticExample() =>
    new (ctx => CU.Text("Some text"), isStatic: true);
```

## Summary

If you are still unsure how to use mentioned features or need to see some more examples, you can view [Samples](../Samples~/README.md) or explore some [Bits](bits.md).


