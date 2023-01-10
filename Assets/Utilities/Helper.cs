using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace dnSR_Coding.Utilities
{
    ///<summary> Helper helps to create custom method to globalise method(s) that can be used throughout the project. <summary>
    public static class Helper
    {
        #region Camera datas + Cinemachine

        private static Camera _mainCamera;
        public static Camera GetMainCamera()
        {
            if ( _mainCamera.IsNull() ) { _mainCamera = Camera.main; }
            return _mainCamera;
        }

        private static Transform _playerCamera;
        public static Transform GetPlayerCamera()
        {
            if ( _playerCamera.IsNull() ) { _playerCamera = GameObject.FindGameObjectWithTag( "PlayerCamera" ).transform; }

            return _playerCamera;
        }
        #endregion

        #region Cursor
        public static Vector3 GetCursorClickPosition( LayerMask layerMask )
        {
            Ray rayFromMainCameraToCursorPosition = Camera.main.ScreenPointToRay( Input.mousePosition );
            Vector3 hitPointPos = Vector3.zero;

            if ( Physics.Raycast( rayFromMainCameraToCursorPosition, out RaycastHit hit, Mathf.Infinity, layerMask ) )
            {
                hitPointPos = hit.point;
            }

            Debug.Log( "Cursor clicked position : " + hitPointPos );

            return hitPointPos;
        }

        public static void SetCursorLockMode( CursorLockMode lockMode )
        {
            if ( Cursor.lockState == lockMode ) { return; }

            Cursor.lockState = lockMode;
        }

        public static void SetCursorVisibility( bool state )
        {

            if ( Cursor.visible == state ) { return; }

            Cursor.visible = state;
        }

        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        public static bool IsOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData( EventSystem.current ) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll( _eventDataCurrentPosition, _results );
            return _results.Count > 0;
        }
        
        #endregion

        #region Debug

        public enum LogType { None, Warning, Error }

        public static void Log<T>( this T user, object message, LogType logType = LogType.None )
        {
#if UNITY_EDITOR
            if ( user is not IDebuggable ) return;

            IDebuggable debuggable = user as IDebuggable;
            Object context = ( Object ) debuggable;

            if ( debuggable.IsDebuggable )
            {
                switch ( logType )
                {
                    case LogType.None:
                        Debug.Log( message, context );
                        break;
                    case LogType.Warning:
                        Debug.LogWarning( message, context );
                        break;
                    case LogType.Error:
                        Debug.LogError( message, context );
                        break;
                }                
            }
#endif
        }

        #endregion

        #region Time scale

        public static void SetTimeScale( float value )
        {
            if ( Time.timeScale == value ) return;

            if ( Time.timeScale < 0 ) { Time.timeScale = 0; }

            Time.timeScale = value;

            Debug.Log( "TimeScale".ToLogComponent( true ) + " value is: " + value.ToString().ToLogValue() );
        }

        private static float deltaTime;
        /// <summary>
        /// Returns the real value of delta time depending if it ignores timeScale, scaled by it.
        /// </summary>
        public static float RealDeltaTime( bool ignoreTimeScale )
        {
            deltaTime = ignoreTimeScale ? Time.deltaTime : Time.deltaTime * Time.timeScale;
            return deltaTime;
        }

        #endregion

        #region Enum

        public static System.Array GetEnumToArray( System.Type type )
        {
            return System.Enum.GetValues( type );
        }

        public static int GetEnumLength( System.Type type )
        {
            return System.Enum.GetValues( type ).Length;
        }

        #endregion

        #region OnGui helpers

        #region Drawing a button - surcharge options

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUIStyle style = default,                                                // Style
           GUILayoutOption [] options = null,                                       // Size
           Color backgroundColor = default,                                         // Color
           System.Action OnClickingButton = null )                                  // Execution
        {
            GUI.backgroundColor = backgroundColor;

            if ( GUILayout.Button( content, style, options ) )
            {
                OnClickingButton?.Invoke();
            }

            // Reset background color to only affect this button
            GUI.backgroundColor = Color.white;
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUIStyle style = default,                                                // Style
           GUILayoutOption [] options = null,                                       // Size
           System.Action OnClickingButton = null )                                  // Execution
        {
            if ( GUILayout.Button( content, style, options ) )
            {
                OnClickingButton?.Invoke();
            }
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUILayoutOption [] options = null,                                       // Size
           Color backgroundColor = default,                                         // Color
           System.Action OnClickingButton = null )                                  // Execution
        {
            GUI.backgroundColor = backgroundColor;

            if ( GUILayout.Button( content, options ) )
            {
                OnClickingButton?.Invoke();
            }

            // Reset background color to only affect this button
            GUI.backgroundColor = Color.white;
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUILayoutOption [] options = null,                                       // Size
           System.Action OnClickingButton = null )                                  // Execution
        {
            if ( GUILayout.Button( content, options ) )
            {
                OnClickingButton?.Invoke();
            }
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           System.Action OnClickingButton = null )                                  // Execution
        {
            if ( GUILayout.Button( content ) )
            {
                OnClickingButton?.Invoke();
            }
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUIStyle style = default,                                                // Style
           float widthOffset = 0,                                                   // Size Offset
           System.Action OnClickingButton = null )                                  // Execution
        {
            float width = style.CalcSize( content ).x + widthOffset;

            if ( GUILayout.Button( content, style,
                new GUILayoutOption [] { GUILayout.Width( width ) } ) )
            {
                OnClickingButton?.Invoke();
            }
        }

        #endregion

        public static void DrawLabel( string content, bool toUpper = false, GUIStyle style = default, int yOffset = 0 )
        {
            GUIContent labelContent = new()
            {
                text = toUpper ? content.ToUpper() : content,
            };

            GUILayout.Space( yOffset );

            GUILayout.Label( labelContent, style );
        }

        public static void DrawLabel( string content, bool toUpper = false )
        {
            GUIContent labelContent = new()
            {
                text = toUpper ? content.ToUpper() : content,
            };

            GUILayout.Label( labelContent );
        }

        public static void DrawTexture( Texture texture, int size, float totalWidth, float totalHeight, float xOffset, float yOffset )
        {
            float xPos = totalWidth - size / 2;
            float yPos = totalHeight - size / 2;

            xPos -= xOffset;
            yPos -= yOffset;

            Rect rect = new( xPos, yPos, width: size, height: size );

            Graphics.DrawTexture( rect, texture );
        }

        #endregion
    }
}