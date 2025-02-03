# Core concepts

## Definitions

In lithium, ui hierarchy is built using `components`.

A `component` is a function with return type of `IComponent` or any type implementing this interface.
This allows you to create multiple components inside a single class and easily compose them together to create desired hierarchy without unnecessary classes.
You can think of a component as function that takes some arguments, can access internal state and using this data returns how the ui should look like.

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

## What's next?

Now that you know basic concepts, [you can learn about implementing custom elements](../advanced/custom-elements.md).