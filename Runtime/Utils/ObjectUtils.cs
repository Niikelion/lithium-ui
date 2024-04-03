using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Common;

namespace UI.Li.Utils.Continuations
{
    [PublicAPI] public static class ObjectUtils
    {
        public static TResult Let<TArg, TResult>(this TArg obj, Func<TArg, TResult> translator) =>
            obj is null ? default : translator(obj);
        public static TResult Let<TArg, TResult>(this TArg obj, Func<TResult> translator) =>
            obj is null ? default : translator();
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
        
        public static TArg When<TArg>(this TArg obj, bool condition, Func<TArg, TArg> action) =>
            obj is null ? default : condition ? action(obj) : obj;
        
        public static TRes When<TArg, TRes>(this TArg obj, bool condition, Func<TRes, TRes> action) where TArg : TRes =>
            obj is null ? default : condition ? action(obj) : obj;
        
        public static IComponent When<TArg>(this TArg obj, bool condition, StyleFunc action) where TArg : IComponent =>
            obj is null ? default : condition ? action(obj) : obj;

        public static IEnumerable<T> PrependTo<T>(this T arg, IEnumerable<T> collection) =>
            (collection??Enumerable.Empty<T>()).Prepend(arg);
    }
}