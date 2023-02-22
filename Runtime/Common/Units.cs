using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.Units
{
    public static class Units
    {
        [PublicAPI] public static StyleLength Px(this float value) => value;
        [PublicAPI] public static StyleLength Px(this int value) => value;
    }
}