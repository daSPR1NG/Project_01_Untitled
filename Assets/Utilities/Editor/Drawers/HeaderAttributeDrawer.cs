using UnityEngine;
using UnityEditor;

namespace dnSR_Coding.Attributes.Drawer
{
    ///<summary> 
    /// Redraws the overridden Header attribute (found at HeaderAttribute) styling it.
    ///<summary>
    [CustomPropertyDrawer( typeof( HeaderAttribute ), true )]
    public class HeaderDrawer : DecoratorDrawer
    {
        ///<inheritdoc/>
        public override void OnGUI( Rect position )
        {
            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
            GUIStyle style = new GUIStyle( EditorStyles.boldLabel ) { richText = true };

            GUIContent label = new(
               $"<color=lightblue><size=13>{( attribute as HeaderAttribute )?.Header}</size></color>" );

            GUI.Label( position, label, style );
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }
    }
}