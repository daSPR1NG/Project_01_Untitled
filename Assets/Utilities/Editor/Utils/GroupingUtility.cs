using UnityEngine;
using UnityEditor;

namespace dnSR_Coding.Utilities.Editor
{
    public class GroupingUtility
    {
        private const string UNDO_OPERATION_NAME = "Group";

        [MenuItem( "GameObject/Group it %g" )]
        private static void GroupSelection()
        {
            // If there's nothing selected then return
            if ( !Selection.activeTransform ) { return; }

            // Create parent gameobject
            GameObject go = new GameObject
            {
                name = "Group_Parent"
            };

            Undo.RegisterCreatedObjectUndo( go, UNDO_OPERATION_NAME );

            // Set parent to selection
            go.transform.SetParent( Selection.activeTransform.parent, false );

            Vector3 mediumPosition = new Vector3( 0, 0, 0 );
            int numberOfObjectsSelected = 0;

            // Iterate over each selection and add it to parent gameobject created
            foreach ( Transform transform in Selection.transforms )
            {
                mediumPosition += transform.position;
                numberOfObjectsSelected++;
                Undo.SetTransformParent( transform, go.transform, UNDO_OPERATION_NAME );
            }

            // Set parent in medium position
            go.transform.position = mediumPosition / numberOfObjectsSelected;

            // Select parent gameobject
            Selection.activeGameObject = go;
        }
    }
}