# Styling

## Inline Styles

Simplest option to quickly add some styles without additional configuration is to add inline styles.

To create inline style, create `Style` instance. You can use named arguments of the constructor to specify only needed styles:

```csharp
var style = new Style(color: Color.red);
```

Then, you can apply it to the element with `WithStyle`:

```csharp
Text("text").WithStyle(style);
```

Alternatively, you can apply state conditionally:

```csharp
Text("text").WithConditionalStyle(isSelected, selectedStyle);
```

## Static Styles

More performance friendly and flexible way of styling your components is static styling.

It converts code defined styles to UI Toolkit stylesheet assets.

First, you need to create `Lithium > UssSettings` asset, then specify resulting stylesheet name and namespaces to gather your styles from.
If done correctly, all static styles from specified namespaces will be extracted at compile time.
Attach it to the root of your hierarchy with `WithStylesheets` method of component.

But how do we create this static styles?

Create static field of type `USS`:

```csharp
private static readonly USS staticStyle = USS.Style;
```

If we want to specify class name by hand, you can use `USS.NamedStyle("customClassName")` instead of `USS.Style`.

To add some properties to our style, simply call corresponding method, for example:

```csharp
private static readonly USS staticStyle = USS.Style.Color(Color.red);
```

You can also create sub-rules for your styles. This way, you can style nested elements without creating multiple styles:

```csharp
private static readonly USS staticStyle = USS.Style
    .Select("& .header", USS.Style.FontSize(20))
    .Select("& .description", USS.Style.FontSize(14));
```

Or to create variants:

```csharp
private static readonly USS statusStyle = USS.Style
    .Color(Color.black)
    .Select("&.ok", USS.Style.Color(Color.green))
    .Select("&.warning", USS.Style.Color(Color.yellow))
    .Select("&.error", USS.Style.Color(Color.red));
```

As selector, you can pass any valid uss selector string.
Additionally, special identifier `&` refers to element with current style.

## What's next?

If you feel brave enough, [you can learn about implementing element manipulators](../advanced/manipulators.md).