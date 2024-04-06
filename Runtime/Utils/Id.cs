using JetBrains.Annotations;

namespace UI.Li.Utils
{
    public class IdWrapper : Wrapper
    {
        private readonly int id;

        [NotNull] public static IdWrapper V([NotNull] IComponent component, int id) => new (component, id);
        private IdWrapper([NotNull] IComponent innerComponent, int id): base(innerComponent) => this.id = id;
        protected override void BeforeInnerRecompose(CompositionContext context) => context.SetNextEntryId(id);
    }

}