using UI.Li.Common;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class aimed to simplify basic use of components.
    /// </summary>
    /// <remarks>Yeah, I know it's not pretty but we don't have functions outside classes soo...</remarks>
    [PublicAPI]
    public static class CompositionUtils
    {
        public static StyleFunc Styled(Style style) => component => StyleWrapper.V(component, style);
        public static IComponent WithStyle(Style style, [NotNull] IComponent component) => StyleWrapper.V(component, style);
        
        public static IEnumerable<IComponent> Seq(int startId = 1, params IComponent[] content) =>
            content.Select((c, i) => c.Id(i + startId));

        public static IEnumerable<IComponent> Seq(params IComponent[] content) =>
            Seq(1, content);
        
        public static IEnumerable<IComponent> Seq(IEnumerable<IComponent> content, int startId = 1) =>
            Seq(startId, content.ToArray());
    }
}