# Lithium UI - reactive ui framework for Unity Editor

[Description]()

## Description

Lithium is an ui framework built on top of UI Elements designed to simplify editor tools creation by minimising time and effort required to develop ui for such tool.
By combining advantages of both ImGui and UI Elements, lithium provides alternative way to define your ui.
For more information see [documentation](Documentation~/ui.lithium.md).

## Getting started

Quick start guide is available [here](Documentation~/bootstrap.md).

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

## Roadmap

Currently planned features in no particullar order:

* fully-featured layout debugger with support for viewing state and contexts,
* styling system,
* list component
* port of every component available in UI Elements Builder,
* port of every style available in UI Elemenets Builder,
* support for named variables,
* pooling of VisualElements for performance boost.

## UI Elements interoperability

Lithium is built on top of UI Elements, which allows for almost seemles interoperability between these systems.

`CompositionRenderer` can be used to generate `VisualElement`s structure from Lithium layout, which you can later insert into your ui document.

Using `VisualElements`s inside Lithium on the other hands require a bit more work. You need to implement custom `Composition`.
To see how is can be done take a look at [text implementation](Runtime/Common/Text.cs).

