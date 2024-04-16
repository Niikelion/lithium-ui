using System;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace UI.Li
{
    [PublicAPI] public class ComponentState
    {
        private readonly WeakReference<CompositionContext> ctx;
        [NotNull] private CompositionContext Ctx => ctx.TryGetTarget(out var context) ? context : throw new InvalidOperationException("context missing");
        
        public ComponentState(CompositionContext context) => ctx = new (context);

        [NotNull]
        public T Use<T>(T value) where T : class, IMutableValue => Ctx.Use(value);

        [NotNull]
        public T Use<T>(CompositionContext.FactoryDelegate<T> factory) where T : class, IMutableValue =>
            Ctx.Use(factory);
        
        [NotNull]
        public MutableValue<T> Remember<T>(T value) =>
            Ctx.Remember(value);

        [NotNull]
        public MutableValue<T> RememberF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberF(factory);

        [NotNull]
        public ValueReference<T> RememberRef<T>(T value) =>
            Ctx.RememberRef(value);

        [NotNull]
        public ValueReference<T> RememberRefF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberRefF(factory);

        [NotNull]
        public MutableList<T> RememberList<T>(IEnumerable<T> collection) => Ctx.RememberList(collection);

        [NotNull]
        public MutableList<T> RememberList<T>([NotNull] CompositionContext.FactoryDelegate<IEnumerable<T>> factory) =>
            Ctx.RememberList(factory);

        [NotNull]
        public MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) =>
            Ctx.RememberDictionary(dictionary);

        [NotNull]
        public MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(
            [NotNull] CompositionContext.FactoryDelegate<IDictionary<TKey, TValue>> factory) =>
            Ctx.RememberDictionary(factory);

        public void OnInit([NotNull] Action onInit) =>
            Ctx.OnInit(onInit);

        public void OnInit([NotNull] Func<Action> onInit) =>
            Ctx.OnInit(onInit);

        public void OnDestroy([NotNull] Action onDestroy) =>
            Ctx.OnDestroy(onDestroy);

        public void ProvideContext<T>(T context) => Ctx.ProvideContext(context);

        public T UseContext<T>() => Ctx.UseContext<T>();
        
        public CompositionContext.BatchScope BatchOperations() => Ctx.BatchOperations();

        public void BatchOperations([NotNull] Action function)
        {
            Debug.Assert(function != null);
            
            using var _ = Ctx.BatchOperations();
            function();
        }
    }
}