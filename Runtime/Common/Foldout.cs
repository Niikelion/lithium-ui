﻿using System;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UI.Li.Utils;
using UI.Li.Utils.Continuations;
using UnityEngine.UIElements;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    /// <summary>
    /// Component simulating <see cref="UnityEngine.UIElements.Foldout"/>.
    /// </summary>
    [PublicAPI] public static class Foldout
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
        [NotNull] [Obsolete]
        public static IComponent V(
            [NotNull] IComponent header,
            [NotNull] IComponent content,
            bool initiallyOpen,
            bool nobToggleOnly,
            HeaderContainer headerContainer,
            ContentContainer contentContainer,
            Element.Data data,
            Func<bool, Action, IComponent> nob = null
        ) => new Component(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            var nobFunc = nob ?? FoldoutNob;

            return CU.Flex(
                direction: FlexDirection.Column,
                content: IComponent.Seq(
                    (headerContainer ?? DefaultHeaderContainer).Invoke(
                        IComponent.Seq(nobFunc(state, nobToggleOnly ? ToggleFoldout : null), header),
                        nobToggleOnly ? null : ToggleFoldout
                    ),
                    (contentContainer ?? DefaultContentContainer).Invoke(content, state)
                ),
                data: data
            );

            void ToggleFoldout() => state.Value = !state;
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
        [NotNull] [Obsolete]
        public static IComponent V(
            [NotNull] string headerText,
            [NotNull] IComponent content,
            bool initiallyOpen,
            bool nobToggleOnly,
            HeaderContainer headerContainer,
            ContentContainer contentContainer,
            Element.Data data,
            Func<bool, Action, IComponent> nob = null
        ) => V(CU.Text(headerText), content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, data, nob);

        /// <summary>
        /// Creates <see cref="Foldout"/> instance.
        /// </summary>
        /// <param name="header">content of the header</param>
        /// <param name="content">content of the body</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">reduce toggle area to nob</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <param name="nob">nob component</param>
        /// <returns></returns>
        [NotNull]
        public static IComponent V(
            [NotNull] IComponent header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            HeaderContainer headerContainer = null,
            ContentContainer contentContainer = null,
            Func<bool, Action, IComponent> nob = null,
            params IManipulator[] manipulators
        ) => new Component(ctx =>
        {
            var state = ctx.Remember(initiallyOpen);

            var nobFunc = nob ?? FoldoutNob;

            return CU.Flex(
                direction: FlexDirection.Column,
                content: IComponent.Seq(
                    (headerContainer ?? DefaultHeaderContainer).Invoke(
                        IComponent.Seq(nobFunc(state, nobToggleOnly ? ToggleFoldout : null), header),
                        nobToggleOnly ? null : ToggleFoldout
                    ),
                    (contentContainer ?? DefaultContentContainer).Invoke(content, state)
                ),
                manipulators: manipulators
            );

            void ToggleFoldout() => state.Value = !state;
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
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <param name="nob">nob component</param>
        /// <returns></returns>
        [NotNull]
        public static IComponent V(
            [NotNull] string headerText,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            HeaderContainer headerContainer = null,
            ContentContainer contentContainer = null,
            Func<bool, Action, IComponent> nob = null,
            params IManipulator[] manipulators
        ) => V(CU.Text(headerText), content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, nob, manipulators);
        
        private static readonly ushort[] nobIndices = { 0, 1, 2 };

        private static readonly StyleFunc defaultNobStyle = CU.Styled(new()
        {
            FlexGrow = 1,
            Margin = 2
        });

        private static readonly StyleFunc defaultNobBoxStyle = CU.Styled(new()
        {
            Width = 13
        });

        [NotNull]
        public static IComponent FoldoutNob(bool open, Action onClick) =>
            defaultNobBoxStyle(CU.Box(
                content: defaultNobStyle(CU.Box(manipulators: new Repaintable(
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
                ), content: null)),
                onClick?.Let(c => new Clickable(c))
            ));

        private static IComponent DefaultHeaderContainer([NotNull] IEnumerable<IComponent> content, Action onClick) =>
            CU.Flex(
                direction: FlexDirection.Row,
                content: content,
                manipulators: onClick?.Let(c => new Clickable(c))
            );

        private static IComponent DefaultContentContainer([NotNull] IComponent content, bool visible) =>
            CU.Box(content).WithStyle(new (
                padding: new (left: 13),
                display: visible ? DisplayStyle.Flex : DisplayStyle.None
            ));
    }
}