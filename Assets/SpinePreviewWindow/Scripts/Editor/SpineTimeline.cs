#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace One.Editor.Spine
{
    public class SpineTimeline
    {
        public float timelineWidth = 12f;
        public float pixelPerSecond = 10f;

        private bool isMouseEvent;

        private SpinePreviewController previewController;

        public float Duration => previewController.Duration;

        public float CurrentTime => previewController.CurrentTime;







        public SpineTimeline(SpinePreviewController previewController)
        {
            this.previewController = previewController;
        }










        #region Draw

        public void Draw(Rect rect)
        {
            LoadPixelPerSecond(rect);

            DrawBackground(rect);
            DrawTicks(rect);
            DrawPlayhead(rect);
            DrawEventMarker(rect);

            GUI.Label(rect, "Timeline Area");
        }

        private void DrawBackground(Rect rect)
        {
            EditorGUI.DrawRect(rect, Settings.TimelineAreaColor);
        }

        private void DrawTicks(Rect rect)
        {
            bool isLessThan1 = Duration <= 10f;
            int amount = isLessThan1 ? Mathf.FloorToInt(Duration * 10f) : Mathf.FloorToInt(Duration);

            for (int i = 0; i <= amount; i++)
            {
                float x = rect.x + i * (isLessThan1 ? (0.1f * pixelPerSecond) : pixelPerSecond);

                EditorGUI.DrawRect(
                    new Rect(x - 0.5f, rect.y, 1f, rect.height),
                    i % 5 == 0 ? Settings.TimelineMajorTickColor : Settings.TimelineTickColor);
            }
        }

        private void DrawPlayhead(Rect rect)
        {
            if (Duration <= 0)
                return;

            float x = rect.x + CurrentTime * pixelPerSecond;

            EditorGUI.DrawRect(new Rect(x - 0.5f, rect.y, 1f, rect.height), Settings.TimelinePlayheadColor);
        }

        private void DrawEventMarker(Rect rect)
        {
            if (previewController.eventMarkers == null ||
                previewController.eventMarkers.Count == 0)
                return;

            foreach (SpineEventMarker eventMarker in previewController.eventMarkers)
            {
                float x = rect.x + eventMarker.time * pixelPerSecond;

                Rect markerRect = new Rect(x - 3, rect.y, 6, 15);

                EditorGUI.DrawRect(
                    markerRect,
                    eventMarker.isCustom ?
                        Settings.TimelineCustomEventMarkerColor :
                        Settings.TimelineEventMarkerColor
                );

                if (markerRect.Contains(Event.current.mousePosition))
                {
                    DrawEventTooltip(markerRect, eventMarker);
                }
            }
        }

        private void DrawEventTooltip(Rect markerRect, SpineEventMarker eventMarker)
        {
            string tooltip = $"{eventMarker.name}\n{eventMarker.time:0.000}s";
            GUIContent content = new GUIContent(tooltip);

            Vector2 size = EditorStyles.helpBox.CalcSize(content);

            Rect tooltipRect = new Rect(
                markerRect.x,
                markerRect.y - size.y,
                size.x,
                size.y
            );

            GUI.Box(tooltipRect, tooltip, EditorStyles.helpBox);
        }

        #endregion Draw









        #region Input

        public void HandleInput(Rect rect)
        {
            Event e = Event.current;

            isMouseEvent = e.type == EventType.MouseDown || e.type == EventType.MouseDrag;

            if (!isMouseEvent)
                return;

            if (!rect.Contains(e.mousePosition))
                return;

            float localX = e.mousePosition.x - rect.x;

            float time = localX / pixelPerSecond;

            previewController.StopAnimation();
            previewController.SetCurrentTime(time);

            e.Use();
        }

        #endregion Input







        #region Load

        public void LoadPixelPerSecond(Rect rect)
        {
            pixelPerSecond = (Duration == 0) ? rect.width : (rect.width / Duration);
        }

        #endregion












        public void Update()
        {

        }
    }
}

#endif