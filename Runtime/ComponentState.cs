using System;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UI.Li.Internal;

namespace UI.Li
{
    [PublicAPI] public class ComponentState
    {
        public delegate void Deferrer(Action action);
        
        private static CompositionContext currentContext;
        
        [NotNull] private static CompositionContext Ctx =>
            currentContext ?? throw new InvalidOperationException("Accessing state outside context");

        public static void ProvideInternalContext(CompositionContext context) => currentContext = context;

        /// <summary>
        /// Provides state context to composer function.
        /// </summary>
        /// <param name="composer">function that creates component</param>
        /// <param name="isStatic">enables aggressive state preservation</param>
        /// <returns></returns>
        [NotNull]
        [Obsolete("Use variant with argument-less composer")] public static Component WithState([NotNull] Component.OldStatefulComponent composer, bool isStatic = false) =>
            new (composer, isStatic);
        
        /// <summary>
        /// Provides state context to composer function.
        /// </summary>
        /// <param name="composer">function that creates component</param>
        /// <param name="isStatic">enables aggressive state preservation</param>
        /// <returns></returns>
        [NotNull]
        public static Component WithState([NotNull] Component.StatefulComponent composer, bool isStatic = false) =>
            new (composer, isStatic);
        
        public static Deferrer GetDeferrer()
        {
            WeakReference<CompositionContext> ctxRef = new(Ctx);

            return action =>
            {
                if (!ctxRef.TryGetTarget(out var ctx))
                    return;
                
                ctx.EnterBatchScope();
                action();
                ctx.LeaveBatchScope();
            };
        }
        
        [NotNull]
        public static T Use<T>(T value) where T : class, IMutableValue => Ctx.Use(value);

        [NotNull]
        public static T Use<T>(CompositionContext.FactoryDelegate<T> factory) where T : class, IMutableValue =>
            Ctx.Use(factory);
        
        [NotNull]
        public static MutableValue<T> Remember<T>(T value) =>
            Ctx.Remember(value);

        [NotNull]
        public static MutableValue<T> RememberF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberF(factory);

        [NotNull]
        public static ValueReference<T> RememberRef<T>(T value) =>
            Ctx.RememberRef(value);

        [NotNull]
        public static ValueReference<T> RememberRefF<T>([NotNull] CompositionContext.FactoryDelegate<T> factory) =>
            Ctx.RememberRefF(factory);

        [NotNull]
        public static MutableList<T> RememberList<T>(IEnumerable<T> collection) => Ctx.RememberList(collection);

        [NotNull]
        public static MutableList<T> RememberList<T>([NotNull] CompositionContext.FactoryDelegate<IEnumerable<T>> factory) =>
            Ctx.RememberList(factory);

        [NotNull]
        public static MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) =>
            Ctx.RememberDictionary(dictionary);

        [NotNull]
        public static MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(
            [NotNull] CompositionContext.FactoryDelegate<IDictionary<TKey, TValue>> factory) =>
            Ctx.RememberDictionary(factory);

        public static void OnInit([NotNull] Action onInit) =>
            Ctx.OnInit(onInit);

        public static void OnInit([NotNull] Func<Action> onInit) =>
            Ctx.OnInit(onInit);

        public static void OnDestroy([NotNull] Action onDestroy) =>
            Ctx.OnDestroy(onDestroy);

        public static void ProvideContext<T>(T context) => Ctx.ProvideContext(context);

        public static T UseContext<T>() => Ctx.UseContext<T>();

        public static T UseMemo<T>(CompositionContext.FactoryDelegate<T> factory, params object[] vars)
        {
            var c = Ctx;
            var oldVars = c.RememberRef(vars);
            var oldResult = RememberRefF(factory);

            if (oldVars.Value.Length != vars.Length || !oldVars.Value.Zip(vars, Equals).All(v => v))
                oldResult.Value = factory();
            
            return oldResult.Value;
        }
    }

    [PublicAPI]
    public static class ComponentStateExtensions
    {
        /// <summary>
        /// Remembers given value in current component state.
        /// </summary>
        /// <remarks>Argument is ignored when value can be found in component state.</remarks>
        /// <param name="ctx">context</param>
        /// <param name="value">initial value</param>
        /// <typeparam name="T">type of remembered value</typeparam>
        /// <returns>Current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [NotNull] public static MutableValue<T> Remember<T>(
            this CompositionContext ctx,
            T value
        ) => ctx.Use(() => new ContextMutableValue<T>(value, ctx));

        /// <summary>
        /// Remembers given value in current component state.
        /// </summary>
        /// <remarks>Factory is only used when value cannot be found in component state.</remarks>
        /// <param name="ctx">context</param>
        /// <param name="factory">factory used to create initial value</param>
        /// <typeparam name="T">type of remembered value</typeparam>
        /// <returns>Current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [NotNull] public static MutableValue<T> RememberF<T>(
            this CompositionContext ctx,
            [NotNull] CompositionContext.FactoryDelegate<T> factory
        ) => ctx.Use(() => new ContextMutableValue<T>(factory(), ctx));

        [NotNull] public static ValueReference<T> RememberRef<T>(
            this CompositionContext ctx,
            T value
        ) => ctx.Use(() => new ValueReference<T>(value));
        
        [NotNull] public static ValueReference<T> RememberRefF<T>(
            this CompositionContext ctx,
            [NotNull] CompositionContext.FactoryDelegate<T> factory
        ) => ctx.Use(() => new ValueReference<T>(factory()));
        
        [NotNull] public static MutableList<T> RememberList<T>(
            this CompositionContext ctx,
            IEnumerable<T> collection = null
        ) => ctx.Use(() => new MutableList<T>(collection ?? Array.Empty<T>()));

        [NotNull] public static MutableList<T> RememberList<T>(
            this CompositionContext ctx,
            [NotNull] CompositionContext.FactoryDelegate<IEnumerable<T>> factory
        ) => ctx.Use(() => new MutableList<T>(factory()));

        [NotNull] public static MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(
            this CompositionContext ctx,
            IDictionary<TKey, TValue> dictionary = null
        ) => ctx.Use(() =>
            new MutableDictionary<TKey, TValue>(dictionary ?? new Dictionary<TKey, TValue>()));

        [NotNull] public static MutableDictionary<TKey, TValue> RememberDictionary<TKey, TValue>(
            this CompositionContext ctx,
            [NotNull] CompositionContext.FactoryDelegate<IDictionary<TKey, TValue>> factory
        ) => ctx.Use(() => new MutableDictionary<TKey, TValue>(factory()));
    }
}