# Core concepts

In lithium, ui hierarchy is built using `components`.

A `component` is any function with return type of `IComposition`. This allows you to create multiple components inside a single class and easily compose them together to create desired hierarchy without unnecessary syntax.

# Store

Because of this approach, state of a component cannot be stored as a field of a class. In order to achieve this, we need to wrap our component with `Composition` object:

```cs
Composition ComponentWithState() => new (ctx => {
    //body of component goes here
});
```

Note, that we can access our state via `ctx` object of type `CompositionContext`.



What can we store in a state? We have variables, callbacks and contexts.

### Variables

Variable is an instance of the `IMutableValue` interface. There is a couple of build-in types for your convienience:

* `MutableValue<T>`(value reference) - triggers update when `Value` property is reassigned.

* `MutableReference<T>`(same as `MutableReference` but does not trigger updates).

* `MutableList<T>`(list) - triggers update when the list(or any of the elements if they implement `IMutableValue`) changes.

### Callbacks

To simplify initialization and cleanup, lithium allows you to specify callbacks run on first rander or during cleanup.

### Contexts

Lithium allows you to provide value of any type as a context that can be retrieved from any point further down in the hierarchy which is very usefull for data propagation.

### Helpers

Also, there are some methods in `CompositionContext` that can help you manage your state:

* `T Remember<T>(T value)` - remembers value in the state on first render and returns current value of a variable.

* `T RememberF<T>(Func<T> factory)` - same as `Remember`, but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.

* `T RememberRef<T>(T value)` - remembers value in the state but does not track its changes.

* `T RememberRefF<T>(Func<T> factory)` - same as `RememberRef` but instead of storing provided value, executes `factory` on the first render and stores result as value in the state.

* `OnInit(Action onInit)` - executes given callback on the first render.

* `OnInit(Func<Action> onInit)` - executes given callback on the first render and stores result as a destruction callback.

* `OnDestroy(Action)` - registers action to be executed during destruction of the component.

* `ProvideContext<T>(T value)` - provides `value` as context of type `T`.

* `T UseContext<T>()` - gets current value of the context of type `T`(throws exception if none is visible from this point in hierarchy).

Note, that every function except for `UseContext` needs to be called in the same order and with same amount with every render, so for example:

```cs
if (some_condition)
    ctx.Remember(5);
```

is not allowed, because the `Remember` call may not happen with every render. This is mainly because our variables and callbacks are not named, so order of the calls serves as identification. `UseContext` fetches global value so its excluded from this rule.

# Built-in components

To further simplify the process, lithium provides convienient utility class that serves as shortcuts for other components.

In order to use it to the fullest, place this using right after imports:

```cs
using CU = UI.Lt.Utils.CompositionUtils;
```

Now, we can reduce:

```cs
Composition Toggle() => new (ctx => {
    var toggleState = ctx.Remember(false);
    
    return UI.Lt.Common.Button.V(
        onClick: () => toggleState.Value = !toggleState.Value,
        content: toggleState.Value ? "On" : "Off"
    ); 
});
```

to:

```cs
Composition Toggle() => new (ctx => {
    var toggleState = ctx.Remember(false);
    
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

To learn more about how to use them, you can read the documentation for the members of `CompositionUtils` that should be accesible through your idee of choice.

## Hierarchy and state loss

Lithium has two ways of identifying components in the hierarchy - id and place in the children list of the parent component.

Because the latter one is not very reliable, lithium discards state of every component when it is not 100% sure it belongs to it.

In order to preserve state, we can assign id to our elements to make them more distinguishable and better communicate to lithium when the state should be preserved and when discarded. Remember, that there can not be two components with same id and parent.

Note, that some components, like `List` automatically assign ids for their children.

If you have component that does not change its layout, you can mark it as static to further improve state preservation:

```cs
Composition StaticExample() =>
    new (ctx => CU.Text("Some text"), isStatic: true);
```

## Summary

If you are still unsure how to use mentioned features or need to see some more examples, you can view Samples or explore some bits.


