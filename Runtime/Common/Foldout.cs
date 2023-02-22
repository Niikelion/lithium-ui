using JetBrains.Annotations;
using UnityEngine.UIElements;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    //TODO: create custom VisualElement for nob and implement foldout
    public static class Foldout
    {
        [PublicAPI]
        [NotNull]
        public static IComposition V(
            [NotNull] IComposition header,
            [NotNull] IComposition content,
            bool initiallyOpen = true,
            Element.Data data = new()
        ) => new Composition(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            return CU.WithId(1, CU.Flex(
                direction: FlexDirection.Column,
                content: new IComposition[]
                {
                    CU.Flex(
                        direction: FlexDirection.Row,
                        content: new[] { FoldoutNob(state.Value), header }
                    ),
                    CU.Box(
                        data: new(
                            padding: new(left: 20),
                            display: state.Value ? DisplayStyle.Flex : DisplayStyle.None
                        ),
                        content: content
                    )
                }
            ));
        }, isStatic: true);

        [PublicAPI]
        [NotNull]
        public static IComposition V(
            [NotNull] string headerText,
            [NotNull] IComposition content,
            bool initiallyOpen = false,
            Element.Data data = new()
        ) => V(CU.Text(headerText), content, initiallyOpen, data);

        private static IComposition FoldoutNob(bool open) =>
            CU.Box(data: new(
                width: 13,
                height: 13
            ));
    }
}