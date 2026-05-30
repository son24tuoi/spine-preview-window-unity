#if UNITY_EDITOR && SPINE_EXIST

using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

using Animation = Spine.Animation;
using Event = UnityEngine.Event;
using System.IO;
using UnityEngine.UI;
using System;

namespace One.Editor.Spine
{
    public class SpinePreviewController
    {
        public SkeletonDataAsset skeletonDataAsset;
        private TextAsset spineJsonAsset;
        public SkeletonData skeletonData;
        public SkeletonAnimation skeletonAnimation;
        public MeshFilter meshFilter;
        public Mesh mesh;
        public GameObject previewGO;
        public PreviewRenderUtility previewRenderUtility;

        private Vector2 eventScroll;

        private SpineEventMarker selectedEventMarker;

        private string newEventName;
        public string[] animationNames;
        public string[] skinNames;

        public int selectedAnimationIndex;
        public int selectedSkinIndex;

        private float newEventTime;
        private float duration = 0;
        private float currentTime = 0;
        private float playSpeed = 1f;

        private bool hasBinaryExtension;
        private bool showTriangleMesh = false;
        private bool showBones = false;
        private bool loopAnimation = true;
        private bool fitCamera = false;
        private bool isPlaying = false;

        public List<SpineEventMarker> eventMarkers = new List<SpineEventMarker>();

        public float Duration => duration;

        public float CurrentTime => currentTime;

        public float FPS => skeletonData.Fps;

        public Camera Camera => previewRenderUtility.camera;




        public SpinePreviewController(PreviewRenderUtility previewRenderUtility)
        {
            this.previewRenderUtility = previewRenderUtility;
        }








        #region Load

        public void LoadSkeletonDataAsset()
        {
            EditorGUI.BeginChangeCheck();

            skeletonDataAsset = EditorGUILayout.ObjectField(
                "Skeleton Data Asset",
                skeletonDataAsset,
                typeof(SkeletonDataAsset),
                false
                ) as SkeletonDataAsset;

            if (skeletonDataAsset != null)
            {
                EditorGUILayout.LabelField("Loaded", skeletonDataAsset.name);

                spineJsonAsset = skeletonDataAsset.skeletonJSON;

                skeletonData = skeletonDataAsset.GetSkeletonData(false);

                ExposedList<Animation> animations = skeletonData.Animations;

                animationNames = new string[animations.Count];

                for (int i = 0; i < animations.Count; i++)
                {
                    animationNames[i] = animations.Items[i].Name;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                LoadPreviewSA();
                LoadTotalFrames();
                LoadEvents();
                LoadSkins();
            }
        }

        public void LoadSpineJsonAsset()
        {
            EditorGUI.BeginChangeCheck();

            spineJsonAsset = EditorGUILayout.ObjectField(
                "Spine Json Asset",
                spineJsonAsset,
                typeof(TextAsset),
                false
                ) as TextAsset;

            if (spineJsonAsset != null)
            {
                EditorGUILayout.LabelField("Loaded", spineJsonAsset.name);

                hasBinaryExtension = spineJsonAsset.name.ToLower().Contains(".skel");

                EditorGUILayout.LabelField("Has Spine Json Data", (!hasBinaryExtension).ToString());
            }

            if (EditorGUI.EndChangeCheck())
            {

            }
        }

        public void LoadPreviewSA()
        {
            if (previewGO != null)
            {
                GameObject.DestroyImmediate(previewGO);
            }

            if (skeletonDataAsset == null)
                return;

            previewGO = new GameObject("SpinePreview");
            skeletonAnimation = previewGO.AddComponent<SkeletonAnimation>();
            skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
            skeletonAnimation.Initialize(true);

            meshFilter = skeletonAnimation.GetComponent<MeshFilter>();
            mesh = meshFilter.sharedMesh;

            previewRenderUtility.AddSingleGO(previewGO);

            fitCamera = false;

            StopAnimation();
            selectedAnimationIndex = 0;
            skeletonAnimation.AnimationState.SetAnimation(0, animationNames[selectedAnimationIndex], loopAnimation);
        }

        public void LoadTotalFrames()
        {
            if (skeletonData == null)
                return;

            Animation anim = skeletonData.Animations.Items[selectedAnimationIndex];
            duration = anim.Duration;
        }

        public void LoadEvents()
        {
            eventMarkers.Clear();

            if (skeletonData == null)
                return;

            Animation anim = skeletonData.Animations.Items[selectedAnimationIndex];

            foreach (Timeline timeline in anim.Timelines)
            {
                if (timeline is EventTimeline eventTimeline)
                {
                    for (int i = 0; i < eventTimeline.Events.Length; i++)
                    {
                        eventMarkers.Add(new SpineEventMarker(
                            eventTimeline.Events[i].Data.Name,
                            eventTimeline.Events[i].Time
                        ));
                    }
                }
            }
        }

        public void LoadSkins()
        {
            if (skeletonData == null)
                return;

            ExposedList<Skin> skins = skeletonData.Skins;

            skinNames = new string[skins.Count];

            for (int i = 0; i < skins.Count; i++)
            {
                skinNames[i] = skins.Items[i].Name;
            }

            selectedSkinIndex = 0;
            SetSkin(selectedSkinIndex);
        }

        private void LoadAnimation()
        {
            LoadTotalFrames();
            LoadEvents();

            StopAnimation();

            skeletonAnimation.AnimationState.ClearTracks();
            skeletonAnimation.Skeleton.SetToSetupPose();

            skeletonAnimation.AnimationState.SetAnimation(
                0,
                animationNames[selectedAnimationIndex],
                loopAnimation
            );

            // skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
            // skeletonAnimation.Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);

            skeletonAnimation.LateUpdate();

            Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);
        }

        #endregion Load







        #region Setup

        public void SetCurrentTime(float time)
        {
            currentTime = Mathf.Clamp(time, 0, duration);

            Seek(time);
        }

        public void SetupPose()
        {
            if (skeletonAnimation == null)
                return;

            skeletonAnimation.AnimationState.ClearTracks();
            skeletonAnimation.Skeleton.SetToSetupPose();

            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
            skeletonAnimation.Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);

            Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);

            isPlaying = false;
        }

        public void Seek(float time)
        {
            if (skeletonAnimation == null)
                return;

            TrackEntry entry = skeletonAnimation.AnimationState.GetCurrent(0);

            if (entry == null)
                return;

            time = Mathf.Clamp(time, 0f, entry.Animation.Duration);

            entry.TrackTime = time;

            // skeletonAnimation.Skeleton.SetToSetupPose();

            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
            skeletonAnimation.Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);

            // skeletonAnimation.LateUpdate();
        }

        private void SetSkin(int index)
        {
            if (skeletonAnimation == null || skeletonData == null)
                return;

            index = Mathf.Clamp(index, 0, skinNames.Length - 1);

            Skin skin = skeletonData.Skins.Items[index];

            skeletonAnimation.Skeleton.SetSkin(skin);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();

            Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);

            SetCurrentTime(currentTime);
        }

        private void AddCustomEvent(string name, float time)
        {
            if (skeletonData == null)
                return;

            if (string.IsNullOrWhiteSpace(name))
                return;

            if (time < 0f || time > duration)
                return;

            bool hasSameTime = eventMarkers.Exists(e =>
                Mathf.Abs(e.time - time) < 0.0001f
            );

            if (hasSameTime)
            {
                EditorUtility.DisplayDialog(
                    "Duplicate Time",
                    "An event already exists at this time.",
                    "OK"
                );

                return;
            }

            eventMarkers.Add(new SpineEventMarker(name, time, true));

            eventMarkers.Sort();

            newEventName = string.Empty;
            newEventTime = 0f;
        }

        private void SaveEvents()
        {
            if (hasBinaryExtension)
                return;

            JObject root = JObject.Parse(spineJsonAsset.text);

            if (root == null)
                return;

            DefineEvents(root);

            string animationName = animationNames[selectedAnimationIndex];
            JObject animation = (JObject)root["animations"][animationName];

            JArray eventsArray = new JArray();

            foreach (SpineEventMarker eventMarker in eventMarkers)
            {
                JObject eventObject = new JObject
                {
                    ["time"] = eventMarker.time,
                    ["name"] = eventMarker.name
                };

                eventsArray.Add(eventObject);
            }

            animation["events"] = eventsArray;

            string path = AssetDatabase.GetAssetPath(spineJsonAsset);

            // File.WriteAllText(path, root.ToString());

            string newPath = Path.Combine(
                Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + "_" +
                DateTime.Now.ToString("yyyyMMddHHmm") +
                Path.GetExtension(path)
            );

            File.WriteAllText(newPath, root.ToString());

            AssetDatabase.Refresh();

            LoadEvents();
        }

        private void DefineEvents(JObject root)
        {
            JObject eventDefinitions = (JObject)root["events"];

            if (eventDefinitions == null)
                return;

            foreach (SpineEventMarker eventMarker in eventMarkers)
            {
                if (!eventDefinitions.ContainsKey(eventMarker.name))
                {
                    eventDefinitions[eventMarker.name] = new JObject();
                }
            }
        }

        #endregion Setup









        #region Draw

        public void DrawPreview(Rect rect)
        {
            previewRenderUtility.BeginPreview(rect, GUIStyle.none);

            if (previewGO != null)
            {
                previewGO.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                previewGO.transform.localScale = Vector3.one;

                if (!fitCamera)
                {
                    Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);
                    fitCamera = true;
                }

                previewRenderUtility.Render();

                if (showTriangleMesh)
                {
                    DrawTriangleMesh(rect);
                }

                if (showBones)
                {
                    DrawBones(rect);
                }
            }

            previewRenderUtility.EndAndDrawPreview(rect);
        }

        public void DrawAnimationTool()
        {
            if (skeletonDataAsset != null)
            {
                showTriangleMesh = EditorGUILayout.Toggle("Show Triangle Mesh", showTriangleMesh);

                showBones = EditorGUILayout.Toggle("Show Bones", showBones);

                if (GUILayout.Button("Fit Camera"))
                {
                    Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);
                }
            }

            DrawSkinOptions();

            DrawAnimationOptions();

            DrawMediaControls();
        }

        private void DrawAnimationOptions()
        {
            if (skeletonDataAsset == null)
                return;

            if (animationNames != null && animationNames.Length > 0)
            {
                if (GUILayout.Button("Setup Pose"))
                {
                    SetupPose();
                }

                DrawAnimationList();

                DrawLoopAnimation();

                DrawCurrentTime();

                EditorGUILayout.LabelField("Time ", $"{currentTime:0.000}s / {duration:0.000}s");
            }
            else
            {
                GUILayout.Label("No animations");
            }
        }

        private void DrawAnimationList()
        {
            EditorGUI.BeginChangeCheck();

            selectedAnimationIndex = EditorGUILayout.Popup(
                "Animations",
                selectedAnimationIndex,
                animationNames);

            if (EditorGUI.EndChangeCheck())
            {
                LoadAnimation();
            }
        }

        private void DrawLoopAnimation()
        {
            EditorGUI.BeginChangeCheck();

            loopAnimation = EditorGUILayout.Toggle("Loop", loopAnimation);

            if (EditorGUI.EndChangeCheck())
            {
                StopAnimation();

                skeletonAnimation.AnimationState.SetAnimation(
                    0,
                    animationNames[selectedAnimationIndex],
                    loopAnimation
                );

                Camera.FitToSkeleton(skeletonAnimation, Settings.CameraPadding);
            }
        }

        private void DrawCurrentTime()
        {
            EditorGUI.BeginChangeCheck();

            currentTime = EditorGUILayout.FloatField("Current Time", currentTime);

            if (EditorGUI.EndChangeCheck())
            {
                SetCurrentTime(currentTime);
            }
        }

        private void DrawMediaControls()
        {
            if (skeletonAnimation == null)
                return;

            EditorGUI.BeginChangeCheck();

            playSpeed = EditorGUILayout.Slider("Speed", playSpeed, Settings.MinPlaySpeed, Settings.MaxPlaySpeed);

            if (EditorGUI.EndChangeCheck())
            {
                if (isPlaying)
                    skeletonAnimation.AnimationState.TimeScale = playSpeed;
            }

            EditorGUILayout.BeginHorizontal();

            if (isPlaying)
            {
                if (GUILayout.Button("Pause"))
                {
                    PauseAnimation();
                }
            }
            else
            {
                if (GUILayout.Button("Play"))
                {
                    PlayAnimation();
                }
            }

            if (GUILayout.Button("Stop"))
            {
                StopAnimation();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSkinOptions()
        {
            if (skeletonDataAsset == null)
                return;

            if (skinNames != null && skinNames.Length > 0)
            {
                EditorGUI.BeginChangeCheck();

                selectedSkinIndex = EditorGUILayout.Popup(
                    "Skins",
                    selectedSkinIndex,
                    skinNames);

                if (EditorGUI.EndChangeCheck())
                {
                    SetSkin(selectedSkinIndex);
                }
            }
        }

        private void DrawTriangleMesh(Rect rect)
        {
            if (skeletonAnimation == null || mesh == null)
                return;

            Handles.SetCamera(rect, Camera);

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            Handles.color = Settings.MeshColor;

            Matrix4x4 oldMatrix = Handles.matrix;
            Handles.matrix = skeletonAnimation.transform.localToWorldMatrix;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = vertices[triangles[i]];
                Vector3 b = vertices[triangles[i + 1]];
                Vector3 c = vertices[triangles[i + 2]];

                Handles.DrawLine(a, b);
                Handles.DrawLine(b, c);
                Handles.DrawLine(c, a);
            }

            Handles.matrix = oldMatrix;
        }

        private void DrawBones(Rect rect)
        {
            if (skeletonAnimation == null)
                return;

            Handles.SetCamera(rect, Camera);

            Matrix4x4 oldMatrix = Handles.matrix;
            Handles.matrix = skeletonAnimation.transform.localToWorldMatrix;

            foreach (Bone bone in skeletonAnimation.Skeleton.Bones)
            {
                Vector3 start = new Vector3(bone.WorldX, bone.WorldY, 0f);

                float length = bone.Data.Length;

                Vector3 end = start + new Vector3(
                    bone.A * length,
                    bone.C * length,
                    0f
                );

                Handles.color = Settings.BoneColor;
                Handles.DrawAAPolyLine(5f, start, end);

                Handles.color = Settings.BoneJointColor;
                Handles.DrawSolidDisc(start, Vector3.forward, 0.025f);
            }

            Handles.matrix = oldMatrix;
        }

        public void DrawEventArea()
        {
            // if (eventMarkers == null || eventMarkers.Count == 0)
            // {
            //     EditorGUILayout.HelpBox("No events", MessageType.Info);
            //     return;
            // }

            eventScroll = EditorGUILayout.BeginScrollView(eventScroll);

            foreach (SpineEventMarker eventMarker in this.eventMarkers)
            {
                DrawEventItem(eventMarker);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(8);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Create Event", EditorStyles.boldLabel);

            newEventName = EditorGUILayout.TextField("Name", newEventName);
            newEventTime = EditorGUILayout.FloatField("Time", newEventTime);

            if (GUILayout.Button("Add Event"))
            {
                AddCustomEvent(newEventName, newEventTime);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Save Events"))
            {
                SaveEvents();
            }
        }

        private void DrawEventItem(SpineEventMarker eventMarker)
        {
            Color oldColor = GUI.backgroundColor;

            if (eventMarker.time == currentTime)
            {
                GUI.backgroundColor = Settings.SelectedEventMarkerColor;
            }

            Rect rect = EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            string title = $"{eventMarker.name} {(eventMarker.isCustom ? "[Custom]" : "")}";

            EditorGUILayout.LabelField(title, eventMarker.isCustom ? Settings.YellowStyle : EditorStyles.boldLabel);

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Delete Event",
                    $"Delete event \"{eventMarker.name}\" at time {eventMarker.time}?",
                    "Yes, Delete",
                    "Cancel"
                );

                if (confirm)
                {
                    eventMarkers.Remove(eventMarker);
                    GUIUtility.ExitGUI();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField($"Time: {eventMarker.time}s");

            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.MouseDown &&
                rect.Contains(Event.current.mousePosition))
            {
                selectedEventMarker = eventMarker;

                SetCurrentTime(eventMarker.time);

                Event.current.Use();
            }

            GUI.backgroundColor = oldColor;
        }

        #endregion Draw











        #region Input

        public void HandleInput(Rect rect)
        {
            if (skeletonAnimation == null)
                return;

            Event e = Event.current;

            if (!rect.Contains(e.mousePosition))
                return;

            HandleZoom(e);
            HandlePan(e);
        }

        private void HandleZoom(Event e)
        {
            if (e.type != EventType.ScrollWheel)
                return;

            float zoomDelta = e.delta.y * 0.1f;

            Camera.orthographicSize += zoomDelta;
            Camera.orthographicSize = Mathf.Clamp(
                Camera.orthographicSize,
                Settings.MinCameraSize,
                Settings.MaxCameraSize);

            e.Use();
        }

        private void HandlePan(Event e)
        {
            bool isDrag = e.type == EventType.MouseDrag && e.button == 0;

            if (!isDrag)
                return;

            Vector2 delta = e.delta;

            float speed = Camera.orthographicSize * Settings.CameraMoveSpeed * Time.deltaTime;

            Camera.transform.position -= new Vector3(
                delta.x * speed,
                -delta.y * speed,
                0f);

            e.Use();
        }

        #endregion Input













        #region Update

        public void Update(float deltaTime)
        {
            if (skeletonAnimation == null)
                return;

            skeletonAnimation.Update(deltaTime);
            UpdateCurrentFrame();

            if (isPlaying && !loopAnimation && currentTime >= duration)
            {
                PauseAnimation();
            }
        }

        public void UpdateCurrentFrame()
        {
            if (skeletonAnimation == null || skeletonData == null)
                return;

            TrackEntry entry = skeletonAnimation.AnimationState.GetCurrent(0);

            if (entry == null)
                return;

            float time;

            if (loopAnimation)
            {
                if (entry.TrackTime <= 0f)
                {
                    time = 0f;
                }
                else
                {
                    time = (entry.TrackTime % entry.Animation.Duration == 0f) ?
                        entry.Animation.Duration :
                        (entry.TrackTime % entry.Animation.Duration);
                }
            }
            else
            {
                time = Mathf.Min(entry.TrackTime, entry.Animation.Duration);
            }

            currentTime = time;
        }

        #endregion Update
















        #region Media Controls

        public void PlayAnimation()
        {
            if (skeletonAnimation == null)
                return;

            if (animationNames == null || animationNames.Length == 0)
                return;

            if (skeletonAnimation.AnimationState.GetCurrent(0) == null)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, animationNames[selectedAnimationIndex], loopAnimation);

                // Cập nhật current time nếu vừa Setup Pose
                SetCurrentTime(currentTime);
            }

            if (!loopAnimation && currentTime >= duration)
            {
                SetCurrentTime(0f);
            }

            skeletonAnimation.timeScale = 1f;
            skeletonAnimation.AnimationState.TimeScale = playSpeed;

            isPlaying = true;
        }

        public void PauseAnimation()
        {
            if (skeletonAnimation == null)
                return;

            skeletonAnimation.timeScale = 0f;
            skeletonAnimation.AnimationState.TimeScale = 0f;

            isPlaying = false;
        }

        public void StopAnimation()
        {
            if (skeletonAnimation == null)
                return;

            skeletonAnimation.timeScale = 0f;
            skeletonAnimation.AnimationState.TimeScale = 0f;

            isPlaying = false;
            currentTime = 0;

            TrackEntry entry = skeletonAnimation.AnimationState.GetCurrent(0);
            if (entry != null)
            {
                entry.TrackTime = 0f;
            }

            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
            skeletonAnimation.Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        #endregion Media Controls

    }
}

#endif