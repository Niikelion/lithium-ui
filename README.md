<p align="center"><img src="Assets~/logo.png" alt="lithium-ui-logo"/></p>

# Lithium UI - reactive ui framework for Unity Editor

## Description

Lithium is an ui framework built on top of UI Elements designed to simplify editor tools creation by minimising time and effort required to develop ui for such tool.
By combining advantages of both ImGui and UI Elements, lithium provides alternative way to define your ui.
For more information see [documentation](Documentation~/ui.lithium.md).

## Why to use Lithium

There are a couple of reasons why you should give Lithium a try in your next project, it:

* is more readable than ImGui,
* provides easy way to define your editor ui from code alongside its functionality,
* eliminates the need for querying elements in hierarchy which helps to detect structure errors on compile-time,
* allows you to reduce data explicitly passed to elements and remove singletons thanks to context system,
* considerably reduces amount of code needed to implement given ui compared to UI Elements
* simplifies ui logic by defining ui structure based on current state rather than defining initial state and update logic.

## Why not to use Lithium

That being said, Lithium does not work in every scenario.
Because layouts are defined from code it is not very ui-designer-friendly.
This makes it not the best choice for in-game interfaces for now.

## Examples

### Hello world window

```csharp
using UI.Li;
using UI.Li.Editor;
using UnityEditor;
using UnityEngine;

using static UI.Li.Common.Common;

public class HelloWindow: ComposableWindow
{
    [MenuItem("Test/HelloWindow")]
    public static void ShowWindow() => GetWindow<HelloWindow>();

    protected override string WindowName => "Hello world!";
    
    protected override IComponent Layout() => Text("Hello world!");
}
```

### Add action to inspector

```csharp
using UI.Li;
using UI.Li.Editor;
using UnityEditor;
using UnityEngine;

using static UI.Li.Common.Common;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.Editor.Fields;

// Assuming we have MonoBehaviour "TestBehaviour"
[CustomEditor(typeof(TestBehaviour))]
public class TestBehaviourEditor: ComposableEditor
{
    protected override IComponent Layout() => Col(
        Inspector(this),
        Button(content: "Tick", onClick: () => Debug.Log("Tack"))
    );
}
```

## Getting started

Ready to learn more? Quick start guide is available [here](Documentation~/bootstrap.md).

## Roadmap

V1 roadmap:

- [x] support for manipulators.
- [x] remove `Element.Data` object, replace it with style system and `IManipulator` list
- [x] ui portals
- [x] remove OnUpdate event from `IComponent`
- [x] styling system
- [x] port of every style available in UI Elements Builder
- [x] port of most of the components available in UI Elements Builder
- [x] list component
- [x] basic layout debugger with support for viewing state

V2 roadmap:

- [x] implement `useMemo` equivalent from ReactJS(`Cache`)
- [x] implement `useEffect` equivalent from ReactJS(`OnUpdate`)
- [x] rework methods for inspecting composition context
- [x] fully featured layout debugger with support for modifying variables and handling contexts
- [x] basic async handling to make working with network resources and files easier
- [x] remove/hide OnRender event from `IComponent`
- [x] ensure support for unity tailwind
- [x] component for stylesheet loading

V3 roadmap:

- [x] optimize debugger to prevent creating large amounts of scriptable objects
- [ ] visual ui builder
- [ ] code generator for easier wrapping around ui toolkit templates
- [x] better error handling
- [ ] auto-batching re-renders caused by updates from built-in events
- [ ] unit tests to improve stability

Future releases:

- [ ] optimization of styling system
- [ ] support for named variables
- [ ] global pooling of `VisualElement`s
- [ ] pooling of components
- [ ] further caching optimizations
- [ ] static style extraction
- [ ] better async handling with tasks that can report status

## UI Elements interoperability

Lithium is built on top of UI Elements, which allows for almost seamless interoperability between them.

`CompositionRenderer` can be used to generate `VisualElement`s structure from Lithium layout, which you can later insert into your ui document.

Using `VisualElements`s inside Lithium on the other hand requires a bit more work. You need to implement custom `IComponent` or extend `Element`.
To see how is can be done take a look at [text implementation](Runtime/Common/Text.cs).

## Motivation

Lithium is heavily inspired by the web framework named ReactJS.
It borrows the idea of ui component as a function of state and parameters that returns the ui.
Since we have html-like ui system in the Unity, we can imitate web frameworks to get similar benefits.

## Contributing

If you feel like some feature is missing, or you find any bug, feel free to open a new issue following the community guidelines.

## License

You can find license [here](LICENSE).
