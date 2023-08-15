using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace dnSR_Coding.Utilities.Helpers
{
    public enum DebugType { None, Warning, Error }

    ///<summary> 
    /// Helper helps to create custom method to globalise method(s) that can be used throughout the project. 
    ///<summary>
    public static class Helper
    {
        #region Inputs

        public static bool IsLeftClickPressed()
        {
            if ( Input.GetMouseButtonDown( 0 ) ) { return true; }
            return false;
        }

        public static bool IsLeftClickUnpressed()
        {
            if ( Input.GetMouseButtonUp( 0 ) ) { return true; }
            return false;
        }

        #endregion

        #region Camera datas + Cinemachine

        private static Camera _mainCamera;
        public static Camera GetMainCamera()
        {
            if ( _mainCamera.IsNull<Camera>() ) {
                _mainCamera = Camera.main; 
            }
            return _mainCamera;
        }

        private static Transform _playerCameraPivot;
        public static Transform GetPlayerCameraPivot()
        {
            if ( _playerCameraPivot.IsNull<Transform>() ) { 
                _playerCameraPivot = GameObject.FindGameObjectWithTag( "PlayerCamera" ).transform; 
            }

            return _playerCameraPivot;
        }
        #endregion

        #region Cursor
        public static Vector3 GetCursorClickPosition()
        {
            Ray rayFromMainCameraToCursorPosition = Camera.main.ScreenPointToRay( Input.mousePosition );
            Vector3 hitPointPos = Vector3.zero;

            if ( Physics.Raycast( rayFromMainCameraToCursorPosition, out RaycastHit hit, Mathf.Infinity ) )
            {
                hitPointPos = hit.point;
            }

            UnityEngine.Debug.Log( "Cursor clicked position : " + hitPointPos );

            return hitPointPos;
        }
        public static Vector3 GetCursorClickPosition( LayerMask layerMask )
        {
            Ray rayFromMainCameraToCursorPosition = Camera.main.ScreenPointToRay( Input.mousePosition );
            Vector3 hitPointPos = Vector3.zero;

            if ( Physics.Raycast( rayFromMainCameraToCursorPosition, out RaycastHit hit, Mathf.Infinity, layerMask ) )
            {
                hitPointPos = hit.point;
            }

            UnityEngine.Debug.Log( "Cursor clicked position : " + hitPointPos );

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

        #region Time scale

        public static void SetTimeScale( float value )
        {
            if ( Time.timeScale == value ) return;

            if ( Time.timeScale < 0 ) { Time.timeScale = 0; }

            Time.timeScale = value;

            UnityEngine.Debug.Log( "TimeScale".ToLogComponent() + " value is: " + value.ToString().ToLogValue() );
        }

        private static float deltaTime;
        /// <summary>
        /// Returns the real value of delta time depending if it ignores timeScale, scaled by it.
        /// </summary>
        public static float GetDeltaTime( bool ignoreTimeScale = false )
        {
            deltaTime = ignoreTimeScale ? Time.deltaTime : Time.deltaTime * Time.timeScale;
            return deltaTime;
        }

        private static float fixedDeltaTime;
        /// <summary>
        /// Returns the real value of delta time depending if it ignores timeScale, scaled by it.
        /// </summary>
        public static float GetFixedDeltaTime( bool ignoreTimeScale = false )
        {
            fixedDeltaTime = ignoreTimeScale ? Time.fixedDeltaTime : Time.fixedDeltaTime * Time.timeScale;
            return fixedDeltaTime;
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
           Color backgroundColor = default,                                         // EditorColor
           System.Action OnClickingButton = null )                                  // Execution
        {
            GUI.backgroundColor = backgroundColor;

            if ( GUILayout.Button( content, style, options ) )
            {
                OnClickingButton?.Invoke();
            }

            // ResetExp background color to only affect this button
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
           GUIStyle style = default,                                                // Style
           System.Action OnClickingButton = null )                                  // Execution
        {
            if ( GUILayout.Button( content, style ) )
            {
                OnClickingButton?.Invoke();
            }
        }

        public static void DrawButton(
           GUIContent content = default,                                            // Content
           GUILayoutOption [] options = null,                                       // Size
           Color backgroundColor = default,                                         // EditorColor
           System.Action OnClickingButton = null )                                  // Execution
        {
            GUI.backgroundColor = backgroundColor;

            if ( GUILayout.Button( content, options ) )
            {
                OnClickingButton?.Invoke();
            }

            // ResetExp background color to only affect this button
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

        #region Generic

        /// <summary>
        /// Quit the game : close the application in a build context and stops playmod in Editor.
        /// </summary>
        public static void QuitApplication()
        {
#if UNITY_EDITOR
            if ( Application.isEditor )
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            Application.Quit();
        }

        private static Transform _playerTransform;
        private const string PLAYER_TAG = "Player";
        /// <summary>
        /// Finds the player transform performing a FindGOWithTag.
        /// </summary>
        /// <returns> The player transform. </returns>
        public static Transform GetPlayerTransformReference()
        {
            if ( _playerTransform.IsNull<Transform>() ) {
                _playerTransform = GameObject.FindGameObjectWithTag( PLAYER_TAG ).transform;
            }
            return _playerTransform;
        }

        #endregion

        #region Type

        public static string GetTypeName( Type type ) {
            return type.Name.ToString();
        }

        #endregion

        #region Color Utility

        public static Color GetColorFromHexCode( string hexCode )
        {
            Color color = Color.white;

            if ( ColorUtility.TryParseHtmlString( hexCode, out Color c ) ) {
                color = c;
            }

            return color;
        }

        #endregion
    }
}