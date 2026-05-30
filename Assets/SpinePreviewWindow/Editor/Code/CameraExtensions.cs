#if UNITY_EDITOR && SPINE_EXIST

using Spine;
using Spine.Unity;
using UnityEngine;

using Physics = Spine.Skeleton.Physics;

namespace One.Editor.Spine
{
    public static class CameraExtensions
    {
        public static void FitToSkeleton(this Camera camera, SkeletonAnimation skeletonAnimation, float padding = 0.5f)
        {
            if (camera == null ||
                skeletonAnimation == null ||
                skeletonAnimation.Skeleton == null)
            {
                return;
            }

            Skeleton skeleton = skeletonAnimation.Skeleton;

            skeleton.UpdateWorldTransform(Physics.Update);

            float x;
            float y;
            float width;
            float height;
            float[] temp = null;

            skeleton.GetBounds(out x, out y, out width, out height, ref temp);

            Vector3 center = new Vector3(
                x + width * 0.5f,
                y + height * 0.5f,
                0f
            );

            camera.transform.position = new Vector3(
                center.x,
                center.y,
                Settings.CameraZPosition
            );

            camera.orthographic = true;

            float verticalSize = height * 0.5f + padding;

            float horizontalSize = (width * 0.5f + padding) / camera.aspect;

            camera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}

#endif