
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace One.Editor.Spine
{
    public class Settings
    {
        public const float FPS = 30f;

        public const float MinPlaySpeed = 0f;
        public const float MaxPlaySpeed = 2f;

        public const float MinCameraSize = 0.1f;
        public const float MaxCameraSize = 50f;

        public const float CameraMoveSpeed = 0.1f;

        public const float MinWidth = 300f;
        public const float MinHeight = 300f;

        public const float PreviewAreaRatio = 0.6f;
        public const float ToolAreaRatio = 0.3f;

        public const float ToolAreaMinWidth = 300f;
        public const float TimelineAreaMaxHeight = 60f;

        public const float CameraZPosition = -10f;
        public const float CameraPadding = 5f;

        public const float MinRectSize = 10f;

        // public readonly static Color PreviewAreaColor = new Color(0.18f, 0.18f, 0.18f);
        public readonly static Color PreviewAreaColor = Color.gray;
        public readonly static Color TimelineAreaColor = new Color(0.15f, 0.15f, 0.15f);
        public readonly static Color TimelineTickColor = new Color(0.2f, 0.2f, 0.2f);
        public readonly static Color TimelineMajorTickColor = new Color(0.3f, 0.3f, 0.3f);
        public readonly static Color TimelinePlayheadColor = new Color(1f, 0.3f, 0.3f);
        public readonly static Color TimelineEventMarkerColor = Color.green;
        public readonly static Color TimelineCustomEventMarkerColor = Color.yellow;
        public readonly static Color MeshColor = new Color(1f, 0.647f, 0f);
        public readonly static Color BoneColor = Color.red;
        public readonly static Color BoneJointColor = Color.green;
        public readonly static Color SelectedEventMarkerColor = new Color(0.24f, 0.48f, 0.90f);

        public readonly static GUIStyle YellowStyle = new(EditorStyles.label)
        {
            normal = { textColor = Color.yellow }
        };

    }
}

#endif