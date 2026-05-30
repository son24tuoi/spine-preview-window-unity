#if UNITY_EDITOR && !SPINE_EXIST

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace One.Editor.Spine
{
    [InitializeOnLoad]
    public static class SpineDefineInstaller
    {
        private const string Define = "SPINE_EXIST";

        static SpineDefineInstaller()
        {
            EditorApplication.delayCall += UpdateDefine;
        }

        private static void UpdateDefine()
        {
            bool hasSpine =
                Type.GetType("Spine.Skeleton, spine-csharp") != null ||
                Type.GetType("Spine.Unity.SkeletonAnimation, spine-unity") != null;

            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            bool containsDefine = defines.Contains(Define);

            if (hasSpine && !containsDefine)
            {
                defines = string.IsNullOrEmpty(defines)
                    ? Define
                    : defines + ";" + Define;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
                Debug.Log("Scripting Define Symbol \"SPINE_EXIST\" added.");
            }
        }
    }
}

#endif