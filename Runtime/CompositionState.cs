using System;
using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace UI.Li
{
    public class CompositionState
    {
        private readonly WeakReference<CompositionContext> ctx;
        [NotNull] private CompositionContext Ctx => ctx.TryGetTarget(out var context) ? context : throw new InvalidOperationException("context missing");
        
        public CompositionState(CompositionContext context) => ctx = new (context);

        [PublicAPI]
        [NotNull]
        public T Use<T>(T value) where T : class, IMutableValue => Ctx.Use(value);

        [PublicAPI]
        [NotNull]
        public T Use<T>(CompositionContext.FactoryDelegate<T> factory) where T : class, IMutableValue =>
            Ctx.Use(factory);
        
        [PublicAPI]
        [NotNull]
        public MutableValue<T> Remember<T>(T value) =>
            Ctx.Remember(value);

        [PublicAPI]
        [NotNull]
        public MutableValue<T> RememberF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberF(factory);

        [PublicAPI]
        [NotNull]
        public ValueReference<T> RememberRef<T>(T value) =>
            Ctx.RememberRef(value);

        [PublicAPI]
        [NotNull]
        public ValueReference<T> RememberRefF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberRefF(factory);

        [PublicAPI]
        [NotNull]
        public MutableList<T> RememberList<T>(IEnumerable<T> collection) => Ctx.RememberList(collection);

        [PublicAPI]
        [NotNull]
        public MutableList<T> RememberList<T>([NotNull] CompositionContext.FactoryDelegate<IEnumerable<T>> factory) =>
            Ctx.RememberList(factory);

        [PublicAPI]
        [NotNull]
        public MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) =>
            Ctx.RememberDictionary(dictionary);

        [PublicAPI]
        [NotNull]
        public MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(
            [NotNull] CompositionContext.FactoryDelegate<IDictionary<TKey, TValue>> factory) =>
            Ctx.RememberDictionary(factory);

        [PublicAPI]
        public void OnInit([NotNull] Action onInit) =>
            Ctx.OnInit(onInit);

        [PublicAPI]
        public void OnInit([NotNull] Func<Action> onInit) =>
            Ctx.OnInit(onInit);

        [PublicAPI]
        public void OnDestroy([NotNull] Action onDestroy) =>
            Ctx.OnDestroy(onDestroy);

        [PublicAPI]
        public void ProvideContext<T>(T context) => Ctx.ProvideContext(context);

        [PublicAPI]
        public T UseContext<T>() => Ctx.UseContext<T>();
        
        [PublicAPI]
        public CompositionContext.BatchScope BatchOperations() => Ctx.BatchOperations();

        [PublicAPI]
        public void BatchOperations([NotNull] Action function)
        {
            Debug.Assert(function != null);
            
            using var _ = Ctx.BatchOperations();
            function();
        }
    }
}