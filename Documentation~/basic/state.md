# State management

Because of Lithium architecture, the state of a component should be accessed within a component method.
To define state for our component, we need to wrap it with `WithState`:

```csharp
IComponent Toggle() => WithState(() => {
    var toggleState = Remember(false);
    
    return Button(
        onClick: () => toggleState.Value = !toggleState,
        content: toggleState ? "On" : "Off"
    ); 
});
```

What can we store in a state? We have variables, callbacks and contexts.

## Variables

Variables are used to store some values between re-renders.
Every variable is an instance of the `IMutableValue` interface. There is a couple of build-in types for your convenience:

* `MutableValue<T>` - triggers update when `Value` property is assigned.
* `MutableReference<T>` - same as `MutableValue` but does not trigger updates.
* `MutableList<T>` - triggers update when the list(or any of the elements if they implement `IMutableValue`) changes.
* `MutableDictionary<TKey, TValue>` - triggers update when the dictionary(or any of the elements if they implement `IMutableValue`) changes.

To make it easier to use, `ComponentStateExtensions` provides a couple of methods to use state variables:

* `MutableValue<T> Remember<T>(T value)` - remembers value in the state on first render and returns current value of a variable.
* `MutableValue<T> RememberF<T>(Func<T> factory)` - same as `Remember`, but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.
* `ValueReference<T> RememberRef<T>(T value)` - remembers value in the state but does not track its changes.
* `ValueReference<T> RememberRefF<T>(Func<T> factory)` - same as `RememberRef` but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.
* `MutableList<T> RememberList<T>(IEnumerable<T> collection = null)` - same as `Remember`, but stores list instead of single value.
* `MutableList<T> RememberList<T>(Func<IEnumerable<T>> factory)` - same as `RememberList(IEnumerable<T>)`, but uses `factory` to create initial value on first render.
* `MutableDictionary<TKey, TValue> RememberDictionary(IDictionary<TKey, TValue> dictionary = null)` - same as `Remember`, but stores dictionary instead of single value.
* `MutableDictionary<TKey, TValue> RememberDictionary(Func<IDictionary<TKey, TValue>> factory)` - same as `RememberDictionary(IDictionary<TKey, TValue>)`, but uses `factory` to create initial value of first render.
* `T Cache<T>(Func<T> factory, params object[] vars)` - uses `factory` to create value, only recalculates when content of vars changes between renders.

## Callbacks

Available callback:

* `void OnInit(Action onInit)` - calls `onInit` on first render.
* `void OnDestroy(Action onDestroy)` - calls `onDestroy` before component is destroyed.
* `void OnInit(Func<Action> onInit)` - calls `onInit` on first render and then calls value returned by it before component is destroyed.
* `void OnChange(Action onChanged, params object[] vars)` - calls `onChanged` during render when any item of the `vars` array is not equal to corresponding item of `vars` from previous render. Can be used to call action when any of the dependencies changes.

## Contexts

Lithium allows you to provide a value of any type as a context that can be retrieved from any point further down in the hierarchy which is very useful for data propagation.

* `void ProvideContext<T>(T value)` - provides `value` as context of type `T` and passes it down the hierarchy.
* `T UseContext<T>()` - retrieves context of type `T` (throws exception if none is visible from this point in hierarchy).

## Ordering

Note, that every function except for `UseContext` needs to be called in the exactly same order on every render, so for example:

```csharp
if (some_condition)
    Remember(5);
```

is not allowed when `some_condition` might change during component instance lifetime, because the `Remember` call may not happen for every render.
This is mainly because variables and callbacks are not named, so order of the calls is used as identification.
`UseContext` fetches global value, so it is not affected by the order of the calls.

## What's next?

Not it's time, [to learn about layouts](./layout.md).