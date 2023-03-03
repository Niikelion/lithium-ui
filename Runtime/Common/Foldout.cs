using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    public static class Foldout
    {
        [PublicAPI]
        [NotNull]
        public static IComposition V(
            [NotNull] IComposition header,
            [NotNull] IComposition content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new(),
            Func<bool, Action, IComposition> nob = null
        ) => new Composition(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            var nobFunc = nob ?? FoldoutNob;

            void ToggleFoldout() => state.Value = !state.Value;

            return CU.Flex(
                direction: FlexDirection.Column,
                content: new IComposition[]
                {
                    CU.Flex(
                        direction: FlexDirection.Row,
                        content: new[] { nobFunc(state.Value, nobToggleOnly ? ToggleFoldout : null), header },
                        data: new ( onClick: nobToggleOnly ? null : ToggleFoldout )
                    ),
                    CU.Box(
                        data: new(
                            padding: new(left: 13),
                            display: state.Value ? DisplayStyle.Flex : DisplayStyle.None
                        ),
                        content: content
                    )
                }
            );
        }, isStatic: true);

        [PublicAPI]
        [NotNull]
        public static IComposition V(
            [NotNull] string headerText,
            [NotNull] IComposition content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new()
        ) => V(CU.Text(headerText), content, initiallyOpen, nobToggleOnly, data);

        private static readonly ushort[] nobIndices = { 0, 1, 2 };

        private static IComposition FoldoutNob(bool open, Action onClick) =>
            CU.Box(
                content: CU.Box(data: new(
                    flexGrow: 1,
                    margin: new(2),
                    onRepaint: mgc =>
                    {
                        var color = mgc.visualElement.resolvedStyle.color;

                        var rect = mgc.visualElement.contentRect;
                        var center = rect.center;
                        var size = rect.size;

                        float maxDist = Mathf.Min(size.x, size.y) / 2;
                        float revDist = maxDist / 2;
                        float armOff = revDist * Mathf.Sqrt(3);

                        var mesh = mgc.Allocate(3, 3);
                        mesh.SetAllIndices(nobIndices);

                        if (open)
                        {
                            mesh.SetAllVertices(new[]
                            {
                                new Vertex { position = center - Vector2.down * maxDist, tint = color },
                                new Vertex { position = center - new Vector2(armOff, revDist), tint = color },
                                new Vertex { position = center - new Vector2(-armOff, revDist), tint = color }
                            });
                        }
                        else
                        {
                            mesh.SetAllVertices(new[]
                            {
                                new Vertex { position = center + Vector2.right * maxDist, tint = color },
                                new Vertex { position = center + new Vector2(-revDist, armOff), tint = color },
                                new Vertex { position = center + new Vector2(-revDist, -armOff), tint = color }
                            });
                        }
                    }
                )),
                data: new(
                    width: 13,
                    onClick: onClick
                ));
    }
}