# Toc
- [Getting started](#getting-started)
- [Core concepts](#core-concepts)
- [Helper functions](#helper-functions)
- [State](#state)
  - [Variables](#variables)
  - [Callback](#callbacks)
  - [Contexts](#contexts)
  - [Ordering](#ordering)
- [Built-in component](#built-in-components)
- [Hierarchy and state loss](#hierarchy-and-state-loss)
- [Styling](#styling)
  - [Styling elements](#styling-elements)
- [Scope functions](#scope-functions)
  - [Let](#let)
  - [Run](#run)
  - [When](#when)
- [Portals](#portals)
- [Advanced](#advanced)
- [Summary](#summary)

## Getting started

You can find quick startup guide [here](bootstrap.md).

## Core concepts

In lithium, ui hierarchy is built using `components`.

A `component` is any function with return type of `IComponent` or any type implementing this interface. This allows you to create multiple components inside a single class and easily compose them together to create desired hierarchy without unnecessary classes.

## Helper functions

Since c# does not allow global functions, Lithium cannot provide functions like `Text` or `WithState` directly.
Fortunately, it can be achieved by static using statements. For your convenience, all important helper functions are provided in corresponding static classes.
To include all functions available, paste this into your code:

```csharp
using static UI.Li.Common.Common;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.ComponentState;
using static UI.Li.Fields.Fields;
using static UI.Li.Utils.Utils;
using static UI.Li.Async.Async;
```

What if you do not want to import functions that you will not use?

* `UI.Li.Common.Common` provides most commonly used components, like `Button` and `Text`,
* `UI.Li.Common.Layout.Layout` provides components used for layouts, like `Row` and `Col`,
* `UI.Li.ComponentState` provides wrappers for creating components with state,
* `UI.Li.Fields.Fields` provides basic fields, like `Toggle` and `TextField`,
* `UI.Li.Utils.Utils` provides utilities for conditional rendering, like `Switch` and `If`,
* `UI.Li.Async.Async` provides utilities for async execution and loading states.

## State

Because of Lithium architecture, state of a component cannot be stored as a field of a class. In order to get around this problem, we need to wrap our component with `WithState` wrapper:

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

### Variables

Variable is an instance of the `IMutableValue` interface. There is a couple of build-in types for your convenience:

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

### Callbacks

Available callback:

* `void OnInit(Action onInit)` - calls `onInit` on first render.
* `void OnDestroy(Action onDestroy)` - calls `onDestroy` before component is destroyed.
* `void OnInit(Func<Action> onInit)` - calls `onInit` on first render and then calls value returned by it before component is destroyed.
* `void OnChange(Action onChanged, params object[] vars)` - calls `onChanged` during render when any item of the `vars` array is not equal to corresponding item of `vars` from previous render. Can be used to call action when any of the dependencies changes.

### Contexts

Lithium allows you to provide a value of any type as a context that can be retrieved from any point further down in the hierarchy which is very useful for data propagation.

* `void ProvideContext<T>(T value)` - provides `value` as context of type `T` and passes it down the hierarchy.
* `T UseContext<T>()` - retrieves context of type `T` (throws exception if none is visible from this point in hierarchy).

### Ordering

Note, that every function except for `UseContext` needs to be called in the exactly same order on every render, so for example:

```csharp
if (some_condition)
    Remember(5);
```

is not allowed when `some_condition` might change during component instance lifetime, because the `Remember` call may not happen for every render.
This is mainly because variables and callbacks are not named, so order of the calls is used as identification.
`UseContext` fetches global value, so it is not affected by the order of the calls.

## Built-in components

To further simplify the process, Lithium provides variety of components that you can build your ui from.
Core library exposes most of the components available in UI Toolkit.
If you want to browse exposed components, you can look at the documentation for classes mentioned [here](#helper-functions).

## Hierarchy and state loss

Lithium has two ways of identifying components in the hierarchy - id and place in the children list of the parent component.

Because components are not classes, and single function may render different layouts based on parameters, Lithium may not be able to deduce the developers intent.
When its ambiguous, the state is discarded and component re-rendered.

To preserve the state, we can assign id to our elements to make them more distinguishable and better communicate to lithium when the state should be preserved and when discarded. Remember, that there can not be two components with the same id under same direct parent.

Furthermore, there are some helper function you can use to make your intent clearer for the framework:

* `Switch` - layout may change between some predefined options based on some variable,
* `If` - depending on the condition, layout may or may not be rendered,
* `Let` - you want to render layout based on some nullable value, and provide some fallback option when value is `null`.
* `Id` - this element is unique and its type/shape will not change.

For example:
```csharp
private IComponent Children(List<int> elements) => Let(
    elements,
    elems => Col(elems.Select(Child)),
    () => Text("No children")
);

private IComponent Child(int value) => Switch(
    value == 0
    () => Text("null"),
    () => Text(value.ToString())
);

override protected IComponent Layout() => WithState(() => {
    var children = Remember<List<int>>(null);
    
    void Init() => children.value = new List<int> { 1, 0, 2 };
    
    return Col(
        Children(children),
        If(children?.Count ?? 0 == 0, () => Button(Init, "initialize list"))
    );
});
```

## Styling

`Style` type represents styles that can be applied to the element.
It contains all styles from the `IStyle` interface from `UI Toolkit`.

### Styling elements

Functions that can be used for styling:

- `WithStyle(Style style)` - applies given style to the element.
- `WithConditionalStyle(bool condition, Style style)` - applies given style to the element if `condition` is `true`.

For example:

```csharp
Text("Red text").WithStyle(new (color: Color.red));

bool disabled = [...];
ext("Some text").WithConditionalStyle(disabled, new (color: Color.gray));
```

## Scope functions

Scope functions are defined in the `ObjectUtils` static class and are heavily inspired by Kotlin scope functions.
They are designed to make working with Lithium in the object-oriented language easier.

### Let
`Let` can be used to transform value if it is not null:

```csharp
Transform child = [...];
return child?.Let(c => c.name) ?? "Empty";

// but also:
return child?.Let(() => child.name) ?? "Empty";
```

Note, that callback argument is guaranteed not to be null.

### Run

`Run` is same as `Let` but does not return any value:

```csharp
Transform child = [...];
child?.Run(c => c.parent = transform);

// but also:
child?.Run(() => child.parent = transform);
```

### When

`When` can be used to transform the value when the condition is met, otherwise return original value:

```csharp
Style gray = new (color: Color.gray);
bool disabled = [...];

Text("Test").When(disabled, c => c.WithStyle(gray));
```

Note, that this example is only to demonstrate how it works.
For conditional styles use `WithConditionalStyle`.

## Portals

Portals are the proper way to render elements between the context boundary or even outside the Lithium system.

To use them, simply create `Portal.Link` and pass it to `Portal.Achor.V`(container) and `Portal.V`(content).
They will be linked and content will be the direct child of the container.
If you wish to render Lithium content outside of Lithium, or attach the content from outside Lithium to the context,
omit adding one of the components and instead of them use properties `Content` and `Container` properties of the link directly.
For example, to use custom container just set `Container` to this container and use `Portal.V` inside Lithium context.

Note, that use properties of `Portal.Link` directly is only intended for linking outside Lithium ecosystem.
Please use `Portal.V` and `Portal.Anchor.V` when possible.

## Advanced

If you need to do something more complicated, here is a list of more advanced topics and guides:

* [Manipulators](advanced/manipulators.md) - adding functionality for underlying `VisualElement`
* [Custom UI Toolkit Elements](advanced/custom-elements.md) - exposing UI Toolkit type to Lithium
* [Custom Variable Types](advanced/custom-variables.md) - creating custom variable types

## Summary

If you are still unsure how to use mentioned features or need to see some more examples, you can view [Samples](../Samples~/README.md) or explore some [Bits](bits.md).


