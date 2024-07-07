using UI.Li;
using UI.Li.Common;
using UI.Li.Editor;
using UnityEditor;

using static UI.Li.Common.Common;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.ComponentState;

public class TestWindow: ComposableWindow
{
    [MenuItem("Lithium/Examples/TestWindow")]
    public static void ShowWindow() => GetWindow<TestWindow>();

    protected override string WindowName => "Window Test";

    protected override IComponent Layout() => Row(
        ToggleButton(),
        ToggleButton()
    );

    private static IComponent ToggleButton() => WithState(() =>
    {
        var isOn = Remember(false);
        
        return Button(
            content: isOn ? "On" : "Off",
            onClick: () => isOn.Value = !isOn
        ).WithStyle(new (flexGrow: 1));
    });
}