# UI Layouts

Lithium provides a couple components for easier laying out of the ui.

## Box

The simplest one is `Box(IComponent content = null)`.
It just wraps content with a single element.

## Flex

Component that you will probably use the most will be `Flex([NotNull] IEnumerable<IComponent> content, FlexDirection direction)`.
For further convenience, it comes with two additional variants: `Row(params IComponent[] content)`
and `Col(params IComponent[] content)` for rows and columns respectively.

## Scroll

In case your content does not fit the screen, you can wrap it with `Scroll(IComponent content, ScrollViewMode mode = ScrollViewMode.Vertical)`.
It will add vertical, horizontal or both scroll bars depending on passed arguments.

## What's next?

With layouts taken care of, [you can start styling your ui](../advanced/manipulators.md).