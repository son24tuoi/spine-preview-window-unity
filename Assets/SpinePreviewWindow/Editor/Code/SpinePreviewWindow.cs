#if UNITY_EDITOR && SPINE_EXIST

using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace One.Editor.Spine
{
    public class SpinePreviewWindow : EditorWindow
    {
        private PreviewRenderUtility previewRenderUtility;

        private SpinePreviewController previewController;

        private SpineTimeline timeline;

        private double lastTime;

        private float ToolAreaWidth => Mathf.Max(position.width * Settings.ToolAreaRatio, Settings.ToolAreaMinWidth);

        private float TimelineAreaHeight => Settings.TimelineAreaMaxHeight;

        private void OnEnable()
        {
            previewRenderUtility = new PreviewRenderUtility();

            previewController = new SpinePreviewController(previewRenderUtility);

            timeline = new SpineTimeline(previewController);

            if (previewController.skeletonDataAsset != null)
            {
                previewController.LoadPreviewSA();
            }

            lastTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += UpdateCallback;
        }

        private void OnDisable()
        {
            previewRenderUtility.Cleanup();

            if (previewController.previewGO != null)
            {
                DestroyImmediate(previewController.previewGO);
            }

            EditorApplication.update -= UpdateCallback;
        }

        private void UpdateCallback()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - lastTime);
            lastTime = currentTime;

            previewController.Update(deltaTime);
            timeline.Update();

            Repaint();
        }

        [MenuItem("Tools/Spine Preview")]
        public static void ShowWindow()
        {
            SpinePreviewWindow window = GetWindow<SpinePreviewWindow>("Spine Preview");
            window.titleContent = new GUIContent("Spine Preview");
            window.minSize = new Vector2(Settings.MinWidth, Settings.MinHeight);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            DrawPreviewLayout();

            DrawToolArea();

            GUILayout.EndHorizontal();
        }

        private void DrawPreviewLayout()
        {
            EditorGUILayout.BeginVertical();

            DrawPreviewArea();

            DrawTimelineArea();

            EditorGUILayout.EndVertical();
        }


        private void DrawToolArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(ToolAreaWidth));

            previewController.LoadSkeletonDataAsset();

            previewController.LoadSpineJsonAsset();

            EditorGUILayout.Space(10);

            LoadToolArea();

            EditorGUILayout.Space(10);

            DrawEventArea();

            EditorGUILayout.EndVertical();
        }

        private void DrawPreviewArea()
        {
            Rect rect = GUILayoutUtility.GetRect(
                Settings.MinRectSize,
                Settings.MinRectSize,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            EditorGUI.DrawRect(rect, Settings.PreviewAreaColor);

            previewController.DrawPreview(rect);
            previewController.HandleInput(rect);

            GUI.Label(rect, "Preview Area");
        }

        private void DrawTimelineArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(TimelineAreaHeight));

            Rect rect = GUILayoutUtility.GetRect(
                Settings.MinRectSize,
                Settings.MinRectSize,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            timeline.Draw(rect);
            timeline.HandleInput(rect);

            EditorGUILayout.EndVertical();
        }

        private void DrawEventArea()
        {
            EditorGUILayout.LabelField("Event List", EditorStyles.boldLabel);

            previewController.DrawEventArea();
        }

        private void LoadToolArea()
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            previewController.DrawAnimationTool();
        }


    }
}
#endif