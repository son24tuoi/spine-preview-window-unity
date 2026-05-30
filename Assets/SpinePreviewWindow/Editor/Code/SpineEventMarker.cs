#if UNITY_EDITOR && SPINE_EXIST

using System;

namespace One.Editor.Spine
{
    public struct SpineEventMarker : IComparable<SpineEventMarker>
    {
        public string name;
        public float time;
        public bool isCustom;

        public SpineEventMarker(string name, float time, bool isCustom = false)
        {
            this.name = name;
            this.time = time;
            this.isCustom = isCustom;
        }

        public int CompareTo(SpineEventMarker other)
        {
            return time.CompareTo(other.time);
        }

        public readonly bool Equal(SpineEventMarker other) => name == other.name && time == other.time;
    }
}

#endif