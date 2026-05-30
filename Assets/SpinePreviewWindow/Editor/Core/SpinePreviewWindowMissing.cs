#if UNITY_EDITOR && !SPINE_EXIST

using UnityEditor;
using UnityEngine;

public class SpinePreviewWindowMissing : EditorWindow
{
    [MenuItem("Tools/Spine Preview")]
    public static void ShowWindow()
    {
        GetWindow<SpinePreviewWindowMissing>("Spine Preview");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "This tool requires Spine 2D. Please import Spine 2D and add the scripting define symbol: SPINE_EXIST.",
            MessageType.Warning
        );

        if (GUILayout.Button("Download Spine 2D"))
        {
            Application.OpenURL("https://esotericsoftware.com/spine-unity-download");
        }
    }
}

#endif