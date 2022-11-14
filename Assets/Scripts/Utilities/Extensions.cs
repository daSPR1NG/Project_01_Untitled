using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace dnSR_Coding.Utilities
{
    ///<summary> Extensions helps to create custom method used by object(s) throughout the project. <summary>
    public static class Extensions
    {

        #region GameObject

        public static void TryToDisplay( this GameObject gameObject ) 
        {
            if ( gameObject.IsActive() ) return;

            gameObject.SetActive( true );
            Helper.Log( gameObject, gameObject.ToLogComponent() + " has been displayed." );

        }
        public static void TryToHide( this GameObject gameObject ) 
        {
            if ( !gameObject.IsActive() ) return;

            gameObject.SetActive( false );
            Helper.Log( gameObject, gameObject.ToLogComponent() + " has been hidden." );
        }

        public static void Toggle( this GameObject gameObject ) 
        { 
            gameObject.SetActive( !gameObject.activeSelf );
            Helper.Log( gameObject, gameObject.ToLogComponent() + " has been toggled." );
        }

        public static bool IsActive ( this GameObject gameObject ) { return gameObject.activeInHierarchy; } 

        public static void DestroyInRuntimeOrEditor( this GameObject gameObject )
        {
            if ( Application.isPlaying ) Object.Destroy( gameObject );
            else Object.DestroyImmediate( gameObject );
        }

        public static void Instantiate( this GameObject gameObject, Vector3 pos, Quaternion rot ) 
        {
            Object.Instantiate( gameObject, pos, rot ); 
        }
        public static void DontDestroyOnLoad( this GameObject gameObject ) { Object.DontDestroyOnLoad( gameObject ); }

        #endregion

        #region Transform

        public static void Reset( this Transform transform, bool isLocal ) 
        {
            transform.localScale = new Vector3( 1, 1, 1 );

            if ( isLocal )
            {
                transform.SetLocalPositionAndRotation( Vector3.zero, Quaternion.identity );
                return;
            }

            transform.SetPositionAndRotation( Vector3.zero, Quaternion.identity );
        }

        public static bool HasNoChild( this Transform transform ) { return transform.childCount == 0; }

        public static Transform GetFirstChild( this Transform transform )
        {
            if ( transform.HasNoChild() ) { return null; }

            return transform.GetChild( 0 );
        }

        public static Transform GetLastChild( this Transform transform) 
        {
            if ( transform.HasNoChild() ) { return null; }

            return transform.GetChild( transform.childCount - 1 ); 
        }

        #endregion

        #region Image

        public static void SetSprite( this Image image, Sprite sprite ) { if ( image.sprite != sprite ) image.sprite = sprite; }
        public static void SetColor( this Image image, Color color ) { if ( image.color != color ) image.color = color; }
        public static void SetMaskable( this Image image, bool state ) { if ( image.maskable != state ) image.maskable = state; }

        public static void SetRaycastTarget( this Image image, bool state ) { if ( image.raycastTarget != state ) image.raycastTarget = state; }
        public static void SetRaycastPadding( this Image image, Vector4 offset ) { if ( image.raycastPadding != offset ) image.raycastPadding = offset; }

        public static void SetImageType( this Image image, Image.Type type, bool preserveAspect = true ) 
        {
            if ( image.SpriteIsNull() ) return;

            if ( image.type != type ) image.type = type;

            if ( type == Image.Type.Simple && image.preserveAspect != preserveAspect )
            {
                image.preserveAspect = preserveAspect;
            }
        }

        public static void Reset( this Image image) 
        { 
            image.SetSprite( null );
            image.SetColor( Color.white );
            image.SetMaskable( true );

            image.SetRaycastTarget( true );
            image.SetRaycastPadding( Vector4.zero );

            image.SetImageType( Image.Type.Simple );
        }

        public static bool SpriteIsNull( this Image image ) { return image.sprite == null; }

        #endregion

        #region AudioClip or AudioSource

        #endregion

        #region Input

#if ENABLE_LEGACY_INPUT_MANAGER
        public static bool IsPressed( this KeyCode key ) { return Input.GetKeyDown( key ); }
        public static bool IsHeld( this KeyCode key ) { return Input.GetKey( key ); }
        public static bool IsActionned( this KeyCode key ) { return key.IsPressed() || key.IsHeld(); }

        public static bool IsReleased( this KeyCode key ) { return Input.GetKeyUp( key ); }
#endif

        #endregion

        #region Int

        public static int Clamped( this int i, int min, int max)
        {
            i = Mathf.Clamp( i, min, max );
            return i;
        }

        #endregion

        #region Float

        public static float Clamped( this float f, float min, float max ) {  return Mathf.Clamp( f, min, max ); }
        public static float Min( this float f, float a, float b ) {  return Mathf.Min( a, b ); }
        public static float Max( this float f, float a, float b ) {  return Mathf.Max( a, b ); }

        public static float Sqrt( this float f ) {  return Mathf.Sqrt( f ); }

        public static float Floored( this float f ) {  return Mathf.Floor( f ); }
        public static float FlooredInt( this float f ) {  return Mathf.FloorToInt( f ); }

        public static float Ceiled( this float f ) {  return Mathf.Ceil( f ); }
        public static float CeiledInt( this float f ) {  return Mathf.CeilToInt( f ); }

        #endregion

        #region String

        public static string InMinutesAndSeconds (this string s, float value)
        {
            string minutes = Mathf.Floor( value / 60 ).ToString( "0" );
            string seconds = Mathf.Floor( value % 60 ).ToString( "00" );

            return ( minutes + " : " + seconds );
        }

        public static string ToLogValue( this object obj)
        {
            return $"<b><color=#ff7f00>{"[" + obj + "]"}</color></b>";
        }
        
        public static string ToLogComponent( this object obj, bool toUpper = false)
        {
            string str = $"<b><color=#289900>{obj}</color></b>";
            if ( toUpper ) str = str.ToString().ToUpper();

            return str;
        }

        #endregion

        #region NavMeshAgent

        public static void SetStoppingDistance( this NavMeshAgent nma, float distance, float offset ) { nma.stoppingDistance = nma.radius + ( distance * offset ); }

        public static void ResetDestination( this NavMeshAgent nma )
        {
            if ( nma.IsNull() || !nma.enabled ) { return; }

            nma.isStopped = true;

            nma.path.ClearCorners();
            nma.ResetPath();
        }

        #endregion

        #region RectTransform

        public static Vector2 GetWorldPosition( this RectTransform rectTransform )
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle( rectTransform, rectTransform.position, Helper.GetMainCamera(), out var result );
            return result;
        }

        #endregion

        #region Pointer event data

        public static bool RemoveMe = false;
        public static bool RemoveMe1 = false;
        public static bool RemoveMe2 = false;
        public static bool RemoveMe3 = false;
        public static bool RemoveMe4 = false;
        public static bool IsCorrectButton ( this PointerEventData pointer, PointerEventData.InputButton button )
        {
            return pointer.button == button;
        }

        #endregion

        #region AudioClip

        public static string GetName( this AudioClip clip )
        {
            string clipName;
            clipName = clip.ToString().Replace( "(UnityEngine.AudioClip)", "" );

            return clipName;
        }

        #endregion

        #region List

        public static void AddItem<T>( this List<T> list, T t, bool debugMessage = false )
        {
            if ( list.Contains( t ) )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This list already contains this item." +
                    " | List name: " +
                    list.ToLogComponent() +
                    " | Item name: " +
                    t.ToLogComponent() );
                }
                
                return;
            }

            list.Add( t );
        }

        public static void RemoveItem<T>( this List<T> list, T t, bool debugMessage = false )
        {
            if ( list.IsEmpty() || !list.Contains( t ) )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This list is empty or the item you want to remove is not in the list." +
                    " | List name: " +
                    list.ToLogComponent() +
                    " | Item name: " +
                    t.ToLogComponent() );
                }
                
                return;
            }

            list.Remove( t );
        }

        public static bool IsEmpty<T>( this List<T> list ) { return list.Count == 0; }

        #endregion

        #region Generic

        public static bool IsNull( this object obj )
        {
            return obj is null || obj.Equals( null ) || obj == null;
        }

        public static void Enable( this Behaviour b ) { if ( !b.enabled ) b.enabled = true; }
        public static void Disable( this Behaviour b ) { if ( b.enabled ) b.enabled = false; }

        public static T SafeDestroy<T>( T obj ) where T : Object
        {
            if ( Application.isEditor )
                Object.DestroyImmediate( obj );
            else
                Object.Destroy( obj );

            return null;
        }
        public static T SafeDestroyGameObject<T>( T component ) where T : Component
        {
            if ( component != null )
                SafeDestroy( component.gameObject );
            return null;
        }        

        #endregion
    }
}