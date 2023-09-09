/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using InfinityCode.UltimateEditorEnhancer.Attributes;
using InfinityCode.UltimateEditorEnhancer.Interceptors;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.UltimateEditorEnhancer.ComponentHeader
{
    [InitializeOnLoad]
    public static class RuntimeSaveButton
    {
        private const string FIELD_SEPARATOR = "~«§";
        private static Dictionary<string, object> savedFields = new Dictionary<string, object>();

        static RuntimeSaveButton()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            PropertyHandlerInterceptor.OnAddMenuItems += OnAddMenuItems;
        }

        [ComponentHeaderButton]
        public static bool Draw(Rect rectangle, Object[] targets)
        {
            Object target = targets[0];
            if (!Validate(target)) return false;

            if (GUI.Button(rectangle, EditorIconContents.saveActive, Styles.iconButton))
            {
                Component c = target as Component;
                if (c == null) return true;
                
                SerializedObject so = new SerializedObject(target);
                SerializedProperty p = so.GetIterator();
                if (p.Next(true))
                {
                    do
                    {
                        savedFields[c.GetInstanceID() + FIELD_SEPARATOR + p.propertyPath] = SerializedPropertyHelper.GetBoxedValue(p.Copy());
                    }
                    while (p.NextVisible(true));
                }

                Debug.Log($"{c.gameObject.name}/{ObjectNames.NicifyVariableName(target.GetType().Name)} component state saved.");
            }

            return true;
        }

        private static void OnAddMenuItems(SerializedProperty property, GenericMenu menu)
        {
            if (!EditorApplication.isPlaying) return;

            Component target = property.serializedObject.targetObject as Component;
            if (target == null) return;
            if (target.gameObject.scene.name == null) return;

            int instanceID = target.GetInstanceID();
            string path = property.propertyPath;

            SerializedProperty prop = property.Copy();

            menu.AddItem(TempContent.Get("Save Field Value"), false, () =>
            {
                savedFields[instanceID + FIELD_SEPARATOR + path] = SerializedPropertyHelper.GetBoxedValue(prop);
            });
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                savedFields.Clear();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                RestoreSavedValues();
            }
        }

        private static void RestoreSavedValues()
        {
            Undo.SetCurrentGroupName("Set saved state");
            int group = Undo.GetCurrentGroup();

            foreach (KeyValuePair<string, object> pair in savedFields)
            {
                string[] parts = pair.Key.Split(new[]{FIELD_SEPARATOR}, StringSplitOptions.None);
                int id;
                if (!int.TryParse(parts[0], out id)) continue;

                Object obj = EditorUtility.InstanceIDToObject(id);
                if (obj == null) continue;

                Undo.RecordObject(obj, "Set saved state");
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty prop = so.FindProperty(parts[1]);
                so.Update();
                SerializedPropertyHelper.SetBoxedValue(prop, pair.Value);
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj);
            }

            Undo.CollapseUndoOperations(@group);
        }

        private static bool Validate(Object target)
        {
            if (!Prefs.componentExtraHeaderButtons || !Prefs.saveComponentRuntime) return false;
            if (!EditorApplication.isPlaying) return false;
            Component component = target as Component;
            if (component == null) return false;
            if (component.gameObject.scene.name == null) return false;
            return true;
        }
    }
}
