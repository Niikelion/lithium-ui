using System;
using System.Collections.Generic;
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
        public delegate IComponent HeaderContainer([NotNull] IEnumerable<IComponent> content, Action onClick);

        public delegate IComponent ContentContainer([NotNull] IComponent content, bool visible);
        
        /// <summary>
        /// Creates <see cref="Foldout"/> instance.
        /// </summary>
        /// <param name="header">content of the header</param>
        /// <param name="content">content of the body</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">reduce toggle area to nob</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
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
            HeaderContainer headerContainer = null,
            ContentContainer contentContainer = null,
            Element.Data data = new(),
            Func<bool, Action, IComponent> nob = null
        ) => new Component(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            var nobFunc = nob ?? FoldoutNob;

            void ToggleFoldout() => state.Value = !state;

            return CU.Flex(
                direction: FlexDirection.Column,
                content: IComponent.Seq(
                    (headerContainer ?? DefaultHeaderContainer).Invoke(
                        IComponent.Seq(nobFunc(state, nobToggleOnly ? ToggleFoldout : null), header),
                        nobToggleOnly ? null : ToggleFoldout
                    ),
                    (contentContainer ?? DefaultContentContainer).Invoke(content, state)
                )
            );
        }, isStatic: true);

        /// <summary>
        /// Creates <see cref="Foldout"/> instance.
        /// </summary>
        /// <param name="headerText">text for the header</param>
        /// <param name="content">content of the dropdown</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">reduce toggle area to nob</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
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
            HeaderContainer headerContainer = null,
            ContentContainer contentContainer = null,
            Element.Data data = new(),
            Func<bool, Action, IComponent> nob = null
        ) => V(CU.Text(headerText), content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, data, nob);

        private static readonly ushort[] nobIndices = { 0, 1, 2 };

        private static readonly StyleFunc DefaultNobStyle = CU.Styled(new()
        {
            FlexGrow = 1,
            Margin = 2
        });

        private static readonly StyleFunc DefaultNobBoxStyle = CU.Styled(new()
        {
            Width = 13
        });

        [PublicAPI]
        [NotNull]
        public static IComponent FoldoutNob(bool open, [NotNull] Action onClick) =>
            DefaultNobBoxStyle(CU.Box(
                content: DefaultNobStyle(CU.Box(data: new(
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
                ))),
                data: new(
                    onClick: onClick
                )));

        private static IComponent DefaultHeaderContainer([NotNull] IEnumerable<IComponent> content, Action onClick) =>
            CU.Flex(
                direction: FlexDirection.Row,
                content: content,
                data: new(onClick: onClick)
            );

        private static IComponent DefaultContentContainer([NotNull] IComponent content, bool visible) =>
            CU.Box(
                data: new(
                    padding: new(left: 13),
                    display: visible ? DisplayStyle.Flex : DisplayStyle.None
                ),
                content: content
            );
    }
}