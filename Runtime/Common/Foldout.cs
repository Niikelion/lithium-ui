using System;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    /// <summary>
    /// Component simulating <see cref="UnityEngine.UIElements.Foldout"/>.
    /// </summary>
    public static class Foldout
    {
        /// <summary>
        /// Creates <see cref="Foldout"/> instance.
        /// </summary>
        /// <param name="header">content of the header</param>
        /// <param name="content">content of the body</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">reduce toggle area to nob</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <param name="nob">nob component</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent V(
            [NotNull] IComponent header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new(),
            Func<bool, Action, IComponent> nob = null
        ) => new Component(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            var nobFunc = nob ?? FoldoutNob;

            void ToggleFoldout() => state.Value = !state;

            return CU.Flex(
                direction: FlexDirection.Column,
                content: new IComponent[]
                {
                    CU.Flex(
                        direction: FlexDirection.Row,
                        content: new[] { nobFunc(state, nobToggleOnly ? ToggleFoldout : null), header },
                        data: new ( onClick: nobToggleOnly ? null : ToggleFoldout )
                    ),
                    CU.Box(
                        data: new(
                            padding: new(left: 13),
                            display: state ? DisplayStyle.Flex : DisplayStyle.None
                        ),
                        content: content
                    )
                }
            );
        }, isStatic: true);

        /// <summary>
        /// Creates <see cref="Foldout"/> instance.
        /// </summary>
        /// <param name="headerText">text for the header</param>
        /// <param name="content">content of the dropdown</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">reduce toggle area to nob</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <param name="nob">nob component</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent V(
            [NotNull] string headerText,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new(),
            Func<bool, Action, IComponent> nob = null
        ) => V(CU.Text(headerText), content, initiallyOpen, nobToggleOnly, data, nob);

        private static readonly ushort[] nobIndices = { 0, 1, 2 };

        [PublicAPI]
        [NotNull]
        public static IComponent FoldoutNob(bool open, [NotNull] Action onClick) =>
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