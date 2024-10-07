using System;
using UI.Li.Common;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class aimed to simplify basic use of components.
    /// </summary>
    /// <remarks>Yeah, I know it's not pretty, but we don't have functions outside classes soo...</remarks>
    [PublicAPI]
    public static class CompositionUtils //TODO: move to utils
    {
        [Obsolete]
        public static StyleFunc Styled(Style style) => component => StyleWrapper.V(component, style);
    }
}