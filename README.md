<p style="text-align: center"><img src="Assets~/logo.png" alt="lithium-ui-logo" width="256" height="256"/></p>

# Lithium UI - reactive ui framework for Unity Editor

## Description

Lithium is an ui framework built on top of UI Elements designed to simplify editor tools creation by minimising time and effort required to develop ui for such tool.
By combining advantages of both ImGui and UI Elements, lithium provides alternative way to define your ui.
For more information see [documentation](Documentation~/ui.lithium.md).

## Why to use Lithium

There are a couple of reasons why you should give Lithium a try in your next project, it:

* is more readable than ImGui,
* provides easy way to define your editor ui from code alongside its functionality,
* eliminates the need of querying elements in hierarchy which helps to detect structure errors on compile-time,
* allows you to reduce data explicitly passed to elements and remove singletons thanks to context system,
* considerably reduces amount of code needed to implement given ui compared to UI Elements
* simplifies ui logic by defining ui structure based on current state rather than defining initial state and update logic.

## Why not to use Lithium

That being said, Lithium does not work in every scenario.
Because layouts are defined from code it is not very ui-designer-friendly.
This makes it not the best choice for in-game interfaces.

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

// Assuming we have MonoBehaviour "TestBehaviour"
[CustomEditor(typeof(TestBehaviour))]
public class TestBehaviourEditor: ComposableEditor
{
    protected override IComponent Layout() => Col(
        DefaultInspector.V(this),
        Button(content: "Tick", onClick: () => Debug.Log("Tack"))
    );
}
```

## Getting started

Ready to learn more? Quick start guide is available [here](Documentation~/bootstrap.md).

## Roadmap

V1 roadmap:

- [X] support for manipulators.
- [X] remove `Element.Data` object, replace it with style system and `IManipulator` list
- [X] ui portals
- [X] remove OnUpdate event from `IComponent`
- [x] styling system
- [x] port of every style available in UI Elements Builder
- [x] port of most of the components available in UI Elements Builder
- [x] list component
- [X] basic layout debugger with support for viewing state

V2 roadmap:

- [ ] fully featured layout debugger with support for modifying variables and handling contexts
- [ ] async handling to make working with network resources and files easier
- [ ] remove/hide OnRender event from `IComponent`

Future releases:

- [ ] optimization of styling system
- [ ] support for named variables
- [ ] pooling of `VisualElement` for performance boost

## UI Elements interoperability

Lithium is built on top of UI Elements, which allows for almost seamless interoperability between them.

`CompositionRenderer` can be used to generate `VisualElement`s structure from Lithium layout, which you can later insert into your ui document.

Using `VisualElements`s inside Lithium on the other hand requires a bit more work. You need to implement custom `IComponent` or extend `Element`.
To see how is can be done take a look at [text implementation](Runtime/Common/Text.cs).

## Contributing

If you feel like some feature is missing, or you find any bug, feel free to open a new issue following the community guidelines.

## License

You can find license [here](LICENSE).
