using System;
using JetBrains.Annotations;

namespace UI.Li.Utils.Continuations
{
    [PublicAPI] public static class ObjectUtils
    {
        public static TResult Let<TArg, TResult>(this TArg obj, Func<TArg, TResult> translator) => obj is null ? default : translator(obj);
        public static TResult Let<TArg, TResult>(this TArg obj, Func<TResult> translator) => obj is null ? default : translator();
        public static void Run<T>(this T obj, Action<T> action)
        {
            if (obj is not null)
                action(obj);
        }
        public static void Run<T>(this T obj, Action action)
        {
            if (obj is not null)
                action();
        }
    }
}