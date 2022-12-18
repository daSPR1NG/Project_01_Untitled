using dnSR_Coding.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> ValidationHighlighter description <summary>
    [InitializeOnLoad]
    public class ValidationHighlighter
    {
        static readonly Color k_backgroundColor = new ( 0.7843f, 0.7843f, 0.7843f );
        static readonly Color k_backgroundProColor = new (0.2196f,0.2196f,0.2196f);
        static readonly Color k_backgroundSelectedColor = new ( 0.22745f, 0.447f, 0.6902f );
        static readonly Color k_backgroundSelectedProColor = new ( 0.1725f, 0.3647f, 0.5294f );

        static ValidationHighlighter()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            bool isSelected = Selection.instanceIDs.Contains( instanceID );
            GameObject obj = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;

            if ( !obj.IsNull() )
            {
                IValidatable validatable = obj.GetComponent<IValidatable>();

                if ( !validatable.IsNull() && !validatable.IsValid )
                {
                    selectionRect.x += 18.5f;

                   Color backgroundColor = EditorGUIUtility.isProSkin ? k_backgroundProColor : k_backgroundColor;

                    if ( isSelected )
                        backgroundColor = EditorGUIUtility.isProSkin ? k_backgroundSelectedProColor : k_backgroundSelectedColor;

                    GUIStyle textStyle = new ( GUI.skin.label );

                    textStyle.normal.textColor = Color.red;

                    float width = textStyle.CalcSize( new GUIContent( obj.name ) ).x;

                    Rect backgroundRect = selectionRect;
                    backgroundRect.width = width;

                    EditorGUI.DrawRect( backgroundRect, backgroundColor );

                    EditorGUI.LabelField( selectionRect, obj.name, textStyle );

                    //EditorApplication.RepaintHierarchyWindow();
                }
                    
            }
        }
    }     
}