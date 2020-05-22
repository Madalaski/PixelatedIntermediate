using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
    [CustomPropertyDrawer(typeof(NoiseSettingsPropertyAttribute))]
    internal sealed class NoiseSettingsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            RebuildProfileList();

            float iconSize = rect.height + 4;
            rect.width -= iconSize;
            int preset = sNoisePresets.IndexOf((NoiseSettings)property.objectReferenceValue);
            preset = EditorGUI.Popup(rect, label, preset, sNoisePresetNames);
            string labelText = label.text;
            NoiseSettings newProfile = preset < 0 ? null : sNoisePresets[preset] as NoiseSettings;
            if ((NoiseSettings)property.objectReferenceValue != newProfile)
            {
                property.objectReferenceValue = newProfile;
                property.serializedObject.ApplyModifiedProperties();
            }
            rect.x += rect.width; rect.width = iconSize; rect.height = iconSize;
            if (GUI.Button(rect, EditorGUIUtility.IconContent("_Popup"), GUI.skin.label))
            {
                GenericMenu menu = new GenericMenu();
                if (property.objectReferenceValue != null)
                {
                    menu.AddItem(new GUIContent("Edit"), false, () 
                        => Selection.activeObject = property.objectReferenceValue);
                    menu.AddItem(new GUIContent("Clone"), false, () => 
                        {
                            NoiseSettings pp = CreateProfile(
                                property, labelText,
                                (NoiseSettings)property.objectReferenceValue);
                            if (pp != null)
                            {
                                property.objectReferenceValue = pp;
                                property.serializedObject.ApplyModifiedProperties();
                                InvalidateProfileList();
                            }
                        });
                    menu.AddItem(new GUIContent("Locate"), false, () 
                        => EditorGUIUtility.PingObject(property.objectReferenceValue));
                }
                menu.AddItem(new GUIContent("New"), false, () => 
                    { 
                        //Undo.RecordObject(Target, "Change Noise Profile");
                        NoiseSettings pp = CreateProfile(property, labelText, null);
                        if (pp != null)
                        {
                            property.objectReferenceValue = pp;
                            property.serializedObject.ApplyModifiedProperties();
                            InvalidateProfileList();
                        }
                    });
                menu.ShowAsContext();
            }
        }

        static List<ScriptableObject> sNoisePresets;
        static GUIContent[] sNoisePresetNames;
        static float sLastPresetRebuildTime = 0;

        public static void InvalidateProfileList()
        {
            sNoisePresets = null;
            sNoisePresetNames = null;
        }

        static void RebuildProfileList()
        {
            if (sLastPresetRebuildTime < Time.realtimeSinceStartup - 5)
                InvalidateProfileList();
            if (sNoisePresets != null && sNoisePresetNames != null)
                return;

            sNoisePresets = FindAssetsByType<NoiseSettings>();
#if UNITY_2018_1_OR_NEWER
            InspectorUtility.AddAssetsFromPackageSubDirectory(
                typeof(NoiseSettings), sNoisePresets, "Presets/Noise");
#endif
            sNoisePresets.Insert(0, null);
            List<GUIContent> presetNameList = new List<GUIContent>();
            foreach (var n in sNoisePresets)
                presetNameList.Add(new GUIContent((n == null) ? "(none)" : n.name));
            sNoisePresetNames = presetNameList.ToArray();
            sLastPresetRebuildTime = Time.realtimeSinceStartup;
        }

        static List<ScriptableObject> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<ScriptableObject> assets = new List<ScriptableObject>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<T>(assetPath) as ScriptableObject;
                if (asset != null)
                    assets.Add(asset);
            }
            return assets;
        }

        NoiseSettings CreateProfile(SerializedProperty property, string label, NoiseSettings copyFrom)
        {
            string path = GetObjectName(property) + " " + label;
            path = EditorUtility.SaveFilePanelInProject(
                    "Create Noise Profile asset", path, "asset", 
                    "This asset will generate a procedural noise signal");
            if (!string.IsNullOrEmpty(path))
            {
                NoiseSettings profile = null;
                if (copyFrom != null)
                {
                    string fromPath = AssetDatabase.GetAssetPath(copyFrom);
                    if (AssetDatabase.CopyAsset(fromPath, path))
                    {
                        profile = AssetDatabase.LoadAssetAtPath(
                            path, typeof(NoiseSettings)) as NoiseSettings;
                    }
                }
                else
                {
                    profile = ScriptableObjectUtility.CreateAt(
                        typeof(NoiseSettings), path) as NoiseSettings;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return profile;
            }
            return null;
        }

        static string GetObjectName(SerializedProperty property)
        {
            // A little hacky here, as we favour virtual cameras...
            var obj = property.serializedObject.targetObject;
            GameObject go = obj as GameObject;
            if (go == null)
            {
                var component = obj as Component;
                if (component != null)
                    go = component.gameObject;
            }
            if (go != null)
            {
                var vcam = go.GetComponentInParent<CinemachineVirtualCameraBase>();
                if (vcam != null)
                    return vcam.Name;
                return go.name;
            }
            return obj.name;
        }
    }
}
