using UI.Li;
using UI.Li.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

public class TestWindow: ComposableWindow
{
    [MenuItem("Lithium/Examples/TestWindow")]
    public static void ShowWindow()
    {
        var window = GetWindow<TestWindow>();
        window.titleContent = new GUIContent("Window Test");
    }

    protected override IComposition Layout() => CU.Flex(
        direction: FlexDirection.Row,
        content: new [] { ToggleButton(), ToggleButton() }
    );

    private static Composition ToggleButton() => new(state =>
    {
        var isOn = state.Remember(false);
        
        return CU.Button(
            data: new( flexGrow: 1 ),
            content: isOn ? "On" : "Off",
            onClick: () => isOn.Value = !isOn
        );
    });
}