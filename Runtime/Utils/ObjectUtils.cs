using System;
using JetBrains.Annotations;

namespace UI.Li.Utils.Continuations
{
    public static class ObjectUtils
    {
        [PublicAPI] public static TResult Let<TArg, TResult>(this TArg obj, Func<TArg, TResult> translator) => obj is null ? default : translator(obj);
        [PublicAPI] public static TResult Let<TArg, TResult>(this TArg obj, Func<TResult> translator) => obj is null ? default : translator();

        [PublicAPI] public static void Run<T>(this T obj, Action<T> action)
        {
            if (obj is not null)
                action(obj);
        }
        
        [PublicAPI] public static void Run<T>(this T obj, Action action)
        {
            if (obj is not null)
                action();
        }
    }
}