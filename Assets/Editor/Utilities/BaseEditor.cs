//using UnityEngine;
//using UnityEditor;
//using dnSR_Coding.Utilities;
//using System.Reflection;

//namespace dnSR_Coding
//{
//    ///<summary> Base Editor <summary>
//    [CustomEditor( typeof(MonoBehaviour), true )]
//    public class BaseEditor : Editor
//    {
//        static GUIStyle m_titleStyle = null;
//        ComponentAttribute m_componentAttribute = null;

//        static Texture2D m_logo = null;

//        bool m_isUnityNameSpace = false;

//        private void OnEnable()
//        {
//            string targetName = target.GetType().Namespace;

//            if ( !string.IsNullOrEmpty( targetName ) )
//            {
//                m_isUnityNameSpace = targetName.StartsWith( "Unity" );
//            }

//            if ( m_componentAttribute.IsNull() )
//                m_componentAttribute = GetComponentAttribute( target );
//        }

//        public override void OnInspectorGUI()
//        {
//            if ( !m_isUnityNameSpace )
//            {
//                //LogoGUI( m_componentAttribute );
//                HeaderGUI( m_componentAttribute );
//            }

//            base.OnInspectorGUI();
//        }

//        public static void HeaderGUI( ComponentAttribute componentAttribute )
//        {
//            if ( !componentAttribute.IsNull() )
//            {
//                GUILayout.Space( 10f );

//                if ( m_titleStyle.IsNull() )
//                {
//                    m_titleStyle = new GUIStyle( GUI.skin.label )
//                    {
//                        fontSize = 15,
//                        fontStyle = FontStyle.Bold,
//                        alignment = TextAnchor.MiddleCenter,
//                        richText = true
//                    };
//                }

//                using ( new EditorGUILayout.HorizontalScope() )
//                {
//                    GUILayout.FlexibleSpace();

//                    GUILayout.Label( componentAttribute.Name, m_titleStyle );
//                    //GUILayout.Label( $"- <color=#FF8C00>{componentAttribute.Name}</color> -", m_titleStyle );

//                    GUILayout.FlexibleSpace();
//                }

//                if ( !string.IsNullOrEmpty( componentAttribute.Description ) )
//                {
//                    using ( new EditorGUILayout.HorizontalScope() )
//                    {
//                        GUILayout.FlexibleSpace();

//                        GUILayout.Box( componentAttribute.Description, GUILayout.Width( Screen.width * .8f ) );

//                        GUILayout.FlexibleSpace();
//                    }
//                }

//                GUILayout.Space( 20f );
//            }
//        }

//        public static void LogoGUI( ComponentAttribute componentAttribute )
//        {
//            if ( !componentAttribute.IsNull() )
//            {
//                if ( m_logo.IsNull() )
//                    m_logo = AssetDatabase.LoadAssetAtPath<Texture2D>( "Assets/Textures/Iconography/PersonalLogo.png" );

//                if ( !m_logo.IsNull() )
//                {
//                    using ( new EditorGUILayout.HorizontalScope() )
//                    {
//                        const float padding = 4f;
//                        const float size = 20f;

//                        GUI.Label( new Rect( padding, padding, size, size ), m_logo );
//                        GUILayout.FlexibleSpace();
//                    }
//                }
//            } 
//        }

//        public static ComponentAttribute GetComponentAttribute( Object obj )
//        {
//            return obj.GetType().GetCustomAttribute<ComponentAttribute>() /*?? new ComponentAttribute( obj.GetType().ToString() )*/;
//        }
//    }
//}