# Runtime UI

There are two main approaches when implementing your ui using Lithium for your game.

## Composable Document

The first one is to do everything from within the framework.
To achieve that, just extend `ComposableDocument` class, implement `protected override IComponent Layout()` and then add your behaviour to object on scene.
And that it! You are done, and your ui should be visible in the game.

## Composable Element

But probably that is not the ideal approach.
Usually you want your designers to create most of the ui and developers to only add interactivity.

Due to this, you probably want to use lithium to create small parts of ui that designer can embed inside some document.

To do that, extend `ComposableElement` and call `SetContent` in attach to panel event or constructor.
Since `ComposableElement` extends `UnityEngine.UIElements.VisualElement`, if you implement factory for it, you will be able to use it from inside the UI Toolkit ui builder.

## What's next?

With knowledge about where you can render your ui, [you can learn about state management](./state.md).