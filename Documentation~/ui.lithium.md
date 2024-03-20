# Toc
- [Getting started](#getting-started)
- [Core concepts](#core-concepts)
- [State](#state)
  - [Variables](#variables)
  - [Callback](#callbacks)
  - [Contexts](#contexts)
  - [Ordering](#ordering)
- [Built-in component](#built-in-components)
- [Hierarchy and state loss](#hierarchy-and-state-loss)
- [Styling](#styling)
  - [Styling elements](#styling-elements)
  - [Styling functions](#styling-functions)
  - [Styling component functions](#styling-component-functions)
- [Scope functions](#scope-functions)
  - [Let](#let)
  - [Run](#run)
  - [When](#when)
- [Portals](#portals)
- [Summary](#summary)

## Getting started

You can find quick startup guide [here](bootstrap.md).

## Core concepts

In lithium, ui hierarchy is built using `components`.

A `component` is any function with return type of `IComponent` or any type implementing this interface. This allows you to create multiple components inside a single class and easily compose them together to create desired hierarchy without unnecessary classes.

## State

Because of this approach, state of a component cannot be stored as a field of a class. In order to get around this problem, we need to wrap our component with `Component` object:

```csharp
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

```csharp
if (some_condition)
    ctx.Remember(5);
```

is not allowed when `some_condition` might change during component instance lifetime, because the `Remember` call may not happen with every render.
This is mainly because variables and callbacks are not named, so order of the calls is used as identification.
`UseContext` fetches global value so it is not affected by the order of the calls.

## Built-in components

To further simplify the process, lithium provides convenient utility class that serves as factory for other components.

Place this using right after imports to write `CU` instead of `CompositionUtils`:

```csharp
using CU = UI.Lt.Utils.CompositionUtils;
```

Now, we can reduce:

```csharp
Component Toggle() => new (state => {
    var toggleState = state.Remember(false);
    
    return UI.Lt.Common.Button.V(
        onClick: () => toggleState.Value = !toggleState.Value,
        content: toggleState.Value ? "On" : "Off"
    ); 
});
```

to:

```csharp
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
* `WithStyle`
* `Switch`
* `Text`
* `Button`
* `Flex`
* `TextField`
* `Dropdown`
* `Box`
* `Foldout`
* `SplitArea`
* `Toggle`
* `Scroll`

To learn more about how to use them, you can read the documentation for the members of `CompositionUtils` that should be accessible through your idee of choice.

## Hierarchy and state loss

Lithium has two ways of identifying components in the hierarchy - id and place in the children list of the parent component.

Because the latter one is not very reliable, lithium discards the state a component when it is not 100% sure it belongs to it.

To preserve the state, we can assign id to our elements to make them more distinguishable and better communicate to lithium when the state should be preserved and when discarded. Remember, that there can not be two components with the same id under same direct parent.

If you have a component that does not change its layout, you can mark it as static to further improve the state preservation:

```csharp
Component StaticExample() =>
    new (ctx => CU.Text("Some text"), isStatic: true);
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
CU.Text("Red text").WithStyle(new (color: Color.red));

bool disabled = [...];
CU.Text("Some text").WithConditionalStyle(disabled, new (color: Color.gray));
```

### Styling functions

Instead of using the style directly, you can convert it to the styling function:
```csharp
StyleFunc Red = CU.Styled(new (color: Color.red));
```

Now you can use it like this:
```csharp
Red(CU.Text("Red text"));
```

This is purely cosmetic and does not affect how styles are applied.

### Styling component functions

Instead of styling the element directly, you can attach styles to the component function.
In order to do this, you first need to convert your style to styling function, and then you can use:

- `S(StyleFunc s)` - wraps target component with style information from `s`.
- `Cs(bool condition, StyleFunc s)` - wraps target component with style information from `s` if `condition` is `true`.

This may look confusing, so let's look at some examples:

```csharp
Func<string, IManipulator[], IComponent> Text = CU.Text;
var style = CU.Styled(new (color: Color.gray));

var GrayText = style.S(Text);

bool disabled = [...];
var Text = style.Cs(disabled, Text);

[...]

IComponent SomeComponent() => GrayText("Text", Array.Empty<IManipulator>());
IComponent SomeComponent2() => Text("Text2", Array.Empty<IManipulator>());
```

Note:
- You loose all default values.
- `params` is flattened to simple array parameter.
- All parameter names are lost.

Because all of this, that feature may be removed in the future.

## Scope functions

Scope functions are defined in the `ObjectUtils` static class and are heavily inspired by Kotlin scope functions.
They are designed to make working with `lithium` in object oriented language easier.

### Let
`Let` can be used to transform value if it is not null:

```csharp
Transform child = [...];
return child?.Let(c => c.name) ?? "Empty";

// but also:
return child?.Let(() => child.name) ?? "Empty";
```

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
var gray = CU.Styled(new (color: Color.gray));
bool disabled = [...];

CU.Text("Test").When(disabled, gray);
```

Note that `When` can only change type to the base class.

## Portals

Portals are the proper way to render elements between the context boundary or even outside the lithium system.

To use them, simply create `Portal.Link` and pass it to `Portal.Achor.V`(container) and `Portal.V`(content).
They will be linked and content will be the direct child of the container.
If you wish to render lithium content outside lithium, or attach content from outside lithium to the context,
omit adding one of the components and instead of them use properties `Content` and `Container` directly.
For example, to use custom container just set `Container` to this container and use `Portal.V` inside lithium context.

Note, that this is not the recommended way to render some element in the different part of the same lithium context.
Adequate solution may depend on the situation, but it is usually better to use context to pass data,
including content rendered.

## Summary

If you are still unsure how to use mentioned features or need to see some more examples, you can view [Samples](../Samples~/README.md) or explore some [Bits](bits.md).


