using UI.Li;

public class MutationObserver
{
    public bool Updated => updated;
    
    private bool updated;

    public MutationObserver(IMutableValue mutable) => mutable.OnValueChanged += OnUpdate;

    public void Reset() => updated = false;

    private void OnUpdate() => updated = true;
}