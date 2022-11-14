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

        #region Enumerator things
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetDelay( float time )
        {
            if ( WaitDictionary.TryGetValue( time, out var wait ) ) return wait;

            WaitDictionary [ time ] = new WaitForSeconds( time );
            return WaitDictionary [ time ];
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

        public static void SetTimeScale( float value )
        {
            if ( Time.timeScale == value ) return;

            Time.timeScale = value;

            Debug.Log( "TimeScale".ToLogComponent( true ) + " value is: " + value.ToString().ToLogValue() );
        }
    }
}