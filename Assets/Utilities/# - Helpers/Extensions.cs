using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using dnSR_Coding.Utilities.Interfaces;
using Object = UnityEngine.Object;
using UnityEditor;

namespace dnSR_Coding.Utilities.Helpers
{
    ///<summary> 
    /// Extensions helps to create custom method used by object(s) throughout the project. 
    /// <summary>
    public static class Extensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        #region GameObject

        [MethodImpl( INLINE )]
        public static void Display( this GameObject gameObject )
        {
            if ( gameObject.IsActive() ) return;

            gameObject.SetActive( true );

            Debug.Log( gameObject.name.ToLogComponent() + " has been displayed." );

        }
        [MethodImpl( INLINE )]
        public static void Hide( this GameObject gameObject )
        {
            if ( !gameObject.IsActive() ) return;

            gameObject.SetActive( false );
            Debug.Log( gameObject.name.ToLogComponent() + " has been hidden." );
        }
        [MethodImpl( INLINE )]
        public static void Toggle( this GameObject gameObject )
        {
            gameObject.SetActive( !gameObject.activeSelf );
            Debug.Log( gameObject.name.ToLogComponent() + " has been toggled." );
        }
        [MethodImpl( INLINE )] public static bool IsActive( this GameObject gameObject ) { return gameObject.activeInHierarchy; }
        [MethodImpl( INLINE )]
        public static void DestroyInRuntimeOrEditor( this GameObject gameObject )
        {
            if ( Application.isPlaying ) Object.Destroy( gameObject );
            else Object.DestroyImmediate( gameObject );
        }

        #endregion

        #region Transform

        [MethodImpl( INLINE )] public static bool HasChild( this Transform transform ) { return transform.childCount > 0; }
        [MethodImpl( INLINE )] public static bool HasNoChild( this Transform transform ) { return transform.childCount == 0; }
        [MethodImpl( INLINE )]
        public static Transform GetFirstChild( this Transform transform )
        {
            if ( transform.HasNoChild() ) { return null; }

            return transform.GetChild( 0 );
        }
        [MethodImpl( INLINE )]
        public static int GetExactChildCount( this Transform transform, bool countInactive = false )
        {
            if ( transform.HasNoChild() ) { return 0; }

            int childCount = 0;

            foreach ( Transform trs in transform )
            {
                if ( countInactive && !trs.gameObject.IsActive() )
                {
                    childCount++;
                    continue;
                }

                if ( trs.gameObject.IsActive() )
                {
                    childCount++;
                }
            }

            return childCount;
        }

        [MethodImpl( INLINE )]
        public static void FaceCamera( this Transform trs, Camera cameraToFace )
        {
            cameraToFace = Helper.GetMainCamera();

            if ( trs.IsNull() || cameraToFace.IsNull<Camera>() ) { return; }

            trs.LookAt(
                trs.position + cameraToFace.transform.rotation * Vector3.forward,
                cameraToFace.transform.rotation * Vector3.up );
        }

        #endregion

        #region Image

        /// <summary>
        /// Set Image go sprite, you can choose to set the sprite or the overrideSprite.
        /// </summary>
        /// <param name="image"> the image you want to modify</param>
        /// <param name="sprite"> the sprite you want to assign</param>
        /// <param name="overrides"> defines if you use the default sprite or the overridesprite</param>
        [MethodImpl( INLINE )]
        public static void SetSprite( this Image image, Sprite sprite, bool overrides )
        {
            if ( sprite.IsNull<Sprite>() )
            {
                Debug.LogError( "The sprite you're trying to set is null, this means something is wrong." );
            }

            if ( !overrides )
            {
                if ( image.sprite != sprite )
                {
                    image.sprite = sprite;
                }
                return;
            }

            if ( image.overrideSprite != sprite )
            {
                image.overrideSprite = sprite;
            }
        }

        /// <summary>
        /// Set Image go a new color.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="color">the color you want to set</param>
        [MethodImpl( INLINE )]
        public static void SetColor( this Image image, Color color )
        {
            if ( image.color != color ) image.color = color;
        }

        /// <summary>
        /// Set Image go maskable property.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="state">defines if maskable or not</param>
        [MethodImpl( INLINE )]
        public static void SetMaskable( this Image image, bool state )
        {
            if ( image.maskable != state ) image.maskable = state;
        }

        /// <summary>
        /// Set Image go raycast target property.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="state">defines if is a raycast target or not</param>
        [MethodImpl( INLINE )]
        public static void SetRaycastTarget( this Image image, bool state )
        {
            if ( image.raycastTarget != state ) { image.raycastTarget = state; }
        }

        /// <summary>
        /// Set Image go raycast padding.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="value">the padding value</param>
        [MethodImpl( INLINE )]
        public static void SetRaycastPaddingV4Input( this Image image, Vector4 value )
        {
            if ( image.raycastPadding != value ) { image.raycastPadding = value; }
        }

        /// <summary>
        /// Set Image go raycast padding inputing a Vector2 as a value.
        /// X value applies for right and left, Y for up and down.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="offset">the padding value</param>
        [MethodImpl( INLINE )]
        public static void SetRaycastPaddingV2Input( this Image image, Vector2 value )
        {
            Vector4 offset = new( value.x, value.y, value.x, value.y );
            if ( image.raycastPadding != offset ) { image.raycastPadding = offset; }
        }

        /// <summary>
        /// Set Image go raycast padding inputing a float as a value.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="offset">the padding value</param>
        [MethodImpl( INLINE )]
        public static void SetRaycastPaddingFloatInput( this Image image, float value )
        {
            Vector4 offset = new( value, value, value, value );
            if ( image.raycastPadding != offset ) { image.raycastPadding = offset; }
        }

        /// <summary>
        /// Set Image go _type, and wether if it preserves aspect or not.
        /// </summary>
        /// <param name="type">the image _type you want the image to be</param>
        /// <param name="isSpriteOverriden">defines if you use the default sprite or the overridesprite</param>
        /// <param name="preserveAspect">defines if the image sprite preserves its aspect or not</param>
        [MethodImpl( INLINE )]
        public static void SetImageType( this Image image, Image.Type type, bool isSpriteOverriden, bool preserveAspect = true )
        {
            if ( image.sprite.IsNull<Sprite>() || isSpriteOverriden && image.overrideSprite.IsNull<Sprite>() )
            {
                Debug.LogError( "The sprite you're trying to modify is null, this means something is wrong." );
                return;
            }

            if ( image.type != type ) { image.type = type; }

            if ( type == Image.Type.Simple && image.preserveAspect != preserveAspect )
            {
                image.preserveAspect = preserveAspect;
            }
        }

        /// <summary>
        /// SetToDefault Image go properties.
        /// </summary>
        /// <param name="image">the image you want to modify</param>
        /// <param name="isSpriteOverriden">defines if you're using the overriden sprite or not</param>
        [MethodImpl( INLINE )]
        public static void Reset( this Image image, bool isSpriteOverriden )
        {
            image.SetSprite( null, isSpriteOverriden );
            image.SetColor( Color.white );
            image.SetMaskable( true );

            image.SetRaycastTarget( true );
            image.SetRaycastPaddingV4Input( Vector4.zero );

            image.SetImageType( Image.Type.Simple, isSpriteOverriden, false );
        }

        #endregion

        #region Input

#if ENABLE_LEGACY_INPUT_MANAGER

        /// <summary>
        /// Check if this key is pressed, replacing the Input.GetKeyDown call.
        /// </summary>
        /// <param name="key"> The key you want to check</param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static bool IsPressed( this KeyCode key ) { return Input.GetKeyDown( key ); }

        /// <summary>
        /// Check if this key is held down, replacing the Input.GetKey call.
        /// </summary>
        /// <param name="key"> The key you want to check</param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static bool IsHeld( this KeyCode key ) { return Input.GetKey( key ); }

        /// <summary>
        /// Check if wether, this key is pressed or held down, replacing the Input.GetKey/Down calls.
        /// </summary>
        /// <param name="key"> The key you want to check</param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static bool IsActionned( this KeyCode key ) { return key.IsPressed() || key.IsHeld(); }

        /// <summary>
        /// Check if this key has been released, replacing the Input.GetKeyUp call.
        /// </summary>
        /// <param name="key"> The key you want to check</param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static bool IsReleased( this KeyCode key ) { return Input.GetKeyUp( key ); }
#endif

        #endregion

        #region Clamping

        /// <summary>Returns the value clamped between <c>min</c> and <c>max</c></summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [MethodImpl( INLINE )]
        public static float Clamped( this float value, float min, float max ) => value < min ? min : value > max ? max : value;

        /// <summary>Clamps each go between <c>min</c> and <c>max</c></summary>
        [MethodImpl( INLINE )]
        public static Vector2 Clamped( this Vector2 v, Vector2 min, Vector2 max ) =>
            new Vector2(
                v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
                v.y < min.y ? min.y : v.y > max.y ? max.y : v.y
            );

        /// <inheritdoc cref="ExtMathfs.Clamp(Vector2,Vector2,Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector3 Clamped( this Vector3 v, Vector3 min, Vector3 max ) =>
            new Vector3(
                v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
                v.y < min.y ? min.y : v.y > max.y ? max.y : v.y,
                v.z < min.z ? min.z : v.z > max.z ? max.z : v.z
            );

        /// <inheritdoc cref="ExtMathfs.Clamp(Vector2,Vector2,Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector4 Clamped( this Vector4 v, Vector4 min, Vector4 max ) =>
            new Vector4(
                v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
                v.y < min.y ? min.y : v.y > max.y ? max.y : v.y,
                v.z < min.z ? min.z : v.z > max.z ? max.z : v.z,
                v.w < min.w ? min.w : v.w > max.w ? max.w : v.w
            );

        /// <inheritdoc cref="ExtMathfs.Clamp(float,float,float)"/>
        [MethodImpl( INLINE )]
        public static int Clamped( this int value, int min, int max ) => value < min ? min : value > max ? max : value;

        /// <summary>Returns the value clamped between 0 and 1</summary>
        [MethodImpl( INLINE )]
        public static float Clamped01( this float value ) => value < 0f ? 0f : value > 1f ? 1f : value;

        /// <summary>Clamps each go between 0 and 1</summary>
        [MethodImpl( INLINE )]
        public static Vector2 Clamped01( this Vector2 v ) =>
            new Vector2(
                v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
                v.y < 0f ? 0f : v.y > 1f ? 1f : v.y
            );

        /// <inheritdoc cref="ExtMathfs.Clamp01(Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector3 Clamped01( this Vector3 v ) =>
            new Vector3(
                v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
                v.y < 0f ? 0f : v.y > 1f ? 1f : v.y,
                v.z < 0f ? 0f : v.z > 1f ? 1f : v.z
            );

        /// <inheritdoc cref="ExtMathfs.Clamp01(Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector4 Clamped01( this Vector4 v ) =>
            new Vector4(
                v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
                v.y < 0f ? 0f : v.y > 1f ? 1f : v.y,
                v.z < 0f ? 0f : v.z > 1f ? 1f : v.z,
                v.w < 0f ? 0f : v.w > 1f ? 1f : v.w
            );

        /// <summary>Clamps the value between -1 and 1</summary>
        [MethodImpl( INLINE )] public static float ClampedNeg1to1( this float value ) => value < -1f ? -1f : value > 1f ? 1f : value;

        /// <summary>Clamps each go between -1 and 1</summary>
        [MethodImpl( INLINE )]
        public static Vector2 ClampedNeg1to1( this Vector2 v ) =>
            new Vector2(
                v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
                v.y < -1f ? -1f : v.y > 1f ? 1f : v.y
            );

        /// <summary>Clamps each go between -1 and 1</summary>
        [MethodImpl( INLINE )]
        public static Vector3 ClampedNeg1to1( this Vector3 v ) =>
            new Vector3(
                v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
                v.y < -1f ? -1f : v.y > 1f ? 1f : v.y,
                v.z < -1f ? -1f : v.z > 1f ? 1f : v.z
            );

        /// <summary>Clamps each go between -1 and 1</summary>
        [MethodImpl( INLINE )]
        public static Vector4 ClampedNeg1to1( this Vector4 v ) =>
            new Vector4(
                v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
                v.y < -1f ? -1f : v.y > 1f ? 1f : v.y,
                v.z < -1f ? -1f : v.z > 1f ? 1f : v.z,
                v.w < -1f ? -1f : v.w > 1f ? 1f : v.w
            );

        #endregion

        #region Floor

        /// <summary>Rounds the value down to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Floor( float value ) => ( float ) Math.Floor( value );

        /// <summary>Rounds the vector components down to the nearest integer</summary>
        [MethodImpl( INLINE )] public static Vector2 Floor( Vector2 value ) => new Vector2( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ) );

        /// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3 Floor( Vector3 value ) => new Vector3( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ), ( float ) Math.Floor( value.z ) );

        /// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector4 Floor( Vector4 value ) => new Vector4( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ), ( float ) Math.Floor( value.z ), ( float ) Math.Floor( value.w ) );

        /// <summary>Rounds the value down to the nearest integer, returning an int value</summary>
        [MethodImpl( INLINE )] public static int FloorToInt( float value ) => ( int ) Math.Floor( value );

        /// <summary>Rounds the vector components down to the nearest integer, returning an integer vector</summary>
        [MethodImpl( INLINE )] public static Vector2Int FloorToInt( Vector2 value ) => new Vector2Int( ( int ) Math.Floor( value.x ), ( int ) Math.Floor( value.y ) );

        /// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3Int FloorToInt( Vector3 value ) => new Vector3Int( ( int ) Math.Floor( value.x ), ( int ) Math.Floor( value.y ), ( int ) Math.Floor( value.z ) );

        #endregion

        #region Float

        [MethodImpl( INLINE )]
        public static string InMinutesAndSeconds( this float value )
        {
            string minutes = Mathf.Floor( value / 60 ).ToString( "0" );
            string seconds = Mathf.Floor( value % 60 ).ToString( "00" );

            return ( minutes + " : " + seconds );
        }

        #endregion

        #region String        

        [MethodImpl( INLINE )]
        public static string ToLogValue( this string obj )
        {
            return obj.ToUpper().Bolded().InColor( Color.cyan );
        }

        [MethodImpl( INLINE )]
        public static string ToLogComponent( this string input, bool bolded = true )
        {
            input = bolded ? input.ToUpper().Bolded().InColor( Color.green ) : input.ToUpper().InColor( Color.green );
            return input;
        }

        [MethodImpl( INLINE )]
        public static string ToUpper( this string input )
        {
            return input.ToUpper();
        }

        [MethodImpl( INLINE )]
        public static string Bolded( this string input )
        {
            string name = $"<b>{input}</b>";

            return name;
        }

        [MethodImpl( INLINE )]
        public static string InColor( this string input, Color color )
        {
            ColorUtility.ToHtmlStringRGBA( color );
            string name = string.Format( "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA( color ), input );

            return name;
        }

        #endregion

        #region NavMeshAgent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nma"></param>
        /// <param name="distance"></param>
        /// <param name="offset"></param>
        [MethodImpl( INLINE )]
        public static void SetStoppingDistance( this NavMeshAgent nma, float distance, float offset ) { nma.stoppingDistance = nma.radius + ( distance * offset ); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nma"></param>
        [MethodImpl( INLINE )]
        public static void ResetDestination( this NavMeshAgent nma )
        {
            if ( nma.IsNull<NavMeshAgent>() || !nma.enabled ) { return; }

            nma.isStopped = true;

            nma.path.ClearCorners();
            nma.ResetPath();
        }

        #endregion

        #region RectTransform

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static Vector2 GetWorldPosition( this RectTransform rectTransform )
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle( rectTransform, rectTransform.position, Helper.GetMainCamera(), out var result );
            return result;
        }

        #endregion

        #region AudioClip

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static string GetName( this AudioClip clip )
        {
            string clipName;
            clipName = clip.ToString().Replace( "(UnityEngine.AudioClip)", "" );

            return clipName;
        }

        #endregion

        #region Color manipulation

        /// <summary>Returns the same color, but with the specified alpha value</summary>
        /// <param name="c">The source color</param>
        /// <param name="a">The new alpha value</param>
        [MethodImpl( INLINE )] public static Color WithAlpha( this Color c, float a ) => new Color( c.r, c.g, c.b, a );

        /// <summary>Returns the same color and alpha, but with RGB multiplied by the given value</summary>
        /// <param name="c">The source color</param>
        /// <param name="m">The multiplier for the RGB channels</param>
        [MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, float m ) => new Color( c.r * m, c.g * m, c.b * m, c.a );

        /// <summary>Returns the same color and alpha, but with the RGB values multiplief by another color</summary>
        /// <param name="c">The source color</param>
        /// <param name="m">The color to multiply RGB by</param>
        [MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, Color m ) => new Color( c.r * m.r, c.g * m.g, c.b * m.b, c.a );

        /// <summary>Returns the same color, but with the alpha channel multiplied by the given value</summary>
        /// <param name="c">The source color</param>
        /// <param name="m">The multiplier for the alpha</param>
        [MethodImpl( INLINE )] public static Color MultiplyA( this Color c, float m ) => new Color( c.r, c.g, c.b, c.a * m );

        /// <summary>Converts this color to the nearest 32 bit hex string, including the alpha channel.
        /// A pure red color of (1,0,0,1) returns "FF0000FF"</summary>
        /// <param name="c">The color to get the hex string of</param>
        /// <returns></returns>
        [MethodImpl( INLINE )] public static string ToHexString( this Color c ) => ColorUtility.ToHtmlStringRGBA( c );

        #endregion

        #region List

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <param name="debugMessage"></param>
        [MethodImpl( INLINE )]
        public static void AppendItem<T>( this List<T> list, T type, bool debugMessage = true )
        {
            if ( type.IsNull<T>() )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This item does not exists: "
                        + type.ToString().ToLogComponent() );
                }

                return;
            }

            if ( list.Contains( type ) )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This list already contains this item." +
                    " | List name: " +
                    list.ToString().ToLogComponent() +
                    " | Item name: " +
                    type.ToString().ToLogComponent() );
                }

                return;
            }

            list.Add( type );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <param name="debugMessage"></param>
        [MethodImpl( INLINE )]
        public static void RemoveItem<T>( this List<T> list, T type, bool debugMessage = true )
        {
            if ( type.IsNull<T>() )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This item does not exists: "
                        + type.ToString().ToLogComponent() );
                }

                return;
            }

            if ( list.IsEmpty() || !list.Contains( type ) )
            {
                if ( debugMessage )
                {
                    Debug.Log( "This list is empty or the item you want to remove is not in the list." +
                    " | List name: " +
                    list.ToString().ToLogComponent() +
                    " | Item name: " +
                    type.ToString().ToLogComponent() );
                }

                return;
            }

            list.Remove( type );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static bool IsEmpty<T>( this List<T> list ) { return list.Count == 0; }

        [MethodImpl( INLINE )]
        public static void LogIsEmpty<T>( this List<T> list )
        {
            if ( list.IsEmpty() )
            {
                Debug.Log( $"{list.ToString().Bolded()} is empty." );
            }
        }

        #endregion

        #region Generic

        /// <summary>
        /// This IsNull is used to throw an additional log, informing what type is concerned.
        /// </summary>
        /// <param name="obj"> This is the object you are checking weither it is null or not. </param>
        /// <returns> True if the object is null, equals null or matches the operand == null </returns>
        [MethodImpl( INLINE )]
        public static bool IsNull<T>( this object obj )
        {
            bool isNull = obj is null || obj.Equals( null ) || obj == null;

            if ( isNull )
            {
                Debug.Log(
                    $"Component is null. {Helper.GetTypeName( typeof( T ) ).ToLogComponent()}",
                    ( Object ) obj );
            }

            return isNull;
        }

        /// <summary>
        /// This IsNull is used when not wanting a particular log message.
        /// </summary>
        /// <param name="obj"> This is the object you are checking weither it is null or not. </param>
        /// <returns> True if the object is null, equals null or matches the operand == null </returns>
        public static bool IsNull( this object obj )
        {
            return obj is null || obj.Equals( null ) || obj == null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        [MethodImpl( INLINE )]
        public static void Enable( this Behaviour b )
        {
            if ( !b.enabled ) b.enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        [MethodImpl( INLINE )]
        public static void Disable( this Behaviour b )
        {
            if ( b.enabled ) b.enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl( INLINE )]
        public static T SafeDestroy<T>( T obj ) where T : Object
        {
            if ( Application.isEditor )
                Object.DestroyImmediate( obj );
            else
                Object.Destroy( obj );

            return null;
        }

        [MethodImpl( INLINE )]
        public static void LogNullException<T>( this T type )
        {
            Debug.Log( $"Wanted object is null." );
        }

        #endregion

        #region Log

        /// <summary>
        /// Throws a message if the user is debuggable and not null.
        /// </summary>
        /// <param name="user"> The object using this debug. 
        ///     When clicking on it in the log window, it'll be selected </param>
        /// <param name="message"> The message you want to display. </param>
        /// <param name="debugType"> The type defining how the message is displayed
        ///     (ex: DebuhType.Error => the message is red). </param>
        [MethodImpl( INLINE )]
        public static void Debugger( this IDebuggable user, object message, DebugType debugType = DebugType.None )
        {
#if UNITY_EDITOR
            if ( user.IsNull() )
            {
                user.LogNullException();
                return;
            }

            if ( user.IsDebuggable )
            {
                switch ( debugType )
                {
                    case DebugType.None:
                        Debug.Log( message, user as Object );
                        break;
                    case DebugType.Warning:
                        Debug.LogWarning( message, user as Object );
                        break;
                    case DebugType.Error:
                        Debug.LogError( message, user as Object );
                        break;
                }
            }
#endif
        }

        #endregion

        #region Editor 
        [MethodImpl( INLINE )]
        public static Vector2 GetGUIContentSize( this GUIStyle style, GUIContent content )
        {
            return style.CalcSize( content );
        }

        [MethodImpl( INLINE )]
        public static bool IsPropertyTypeOf( this SerializedProperty property, SerializedPropertyType type )
        {
            return property.propertyType == type;
        }

        #endregion
    }

    public static class ExtMathfs
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        #region Absolute Values

        /// <summary>Returns the absolute value. Basically makes negative numbers positive</summary>
        [MethodImpl( INLINE )]
        public static float Abs( float value ) => Math.Abs( value );

        /// <inheritdoc cref="ExtMathfs.Abs(float)"/>
        [MethodImpl( INLINE )]
        public static int Abs( int value ) => Math.Abs( value );

        /// <summary>Returns the absolute value, per go. Basically makes negative numbers positive</summary>
        [MethodImpl( INLINE )]
        public static Vector2 Abs( Vector2 v ) => new Vector2( Abs( v.x ), Abs( v.y ) );

        /// <inheritdoc cref="ExtMathfs.Abs(Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector3 Abs( Vector3 v ) => new Vector3( Abs( v.x ), Abs( v.y ), Abs( v.z ) );

        /// <inheritdoc cref="ExtMathfs.Abs(Vector2)"/>
        [MethodImpl( INLINE )]
        public static Vector4 Abs( Vector4 v ) => new Vector4( Abs( v.x ), Abs( v.y ), Abs( v.z ), Abs( v.w ) );

        #endregion

        #region Min & Max

        /// <summary>Returns the smallest of the two values</summary>
        [MethodImpl( INLINE )]
        public static float Min( float a, float b ) => a < b ? a : b;

        /// <summary>Returns the smallest of the three values</summary>
        [MethodImpl( INLINE )]
        public static float Min( float a, float b, float c ) => Min( Min( a, b ), c );

        /// <summary>Returns the smallest of the four values</summary>
        [MethodImpl( INLINE )]
        public static float Min( float a, float b, float c, float d ) => Min( Min( a, b ), Min( c, d ) );

        /// <summary>Returns the largest of the two values</summary>
        [MethodImpl( INLINE )]
        public static float Max( float a, float b ) => a > b ? a : b;

        /// <summary>Returns the largest of the three values</summary>
        [MethodImpl( INLINE )]
        public static float Max( float a, float b, float c ) => Max( Max( a, b ), c );

        /// <summary>Returns the largest of the four values</summary>
        [MethodImpl( INLINE )]
        public static float Max( float a, float b, float c, float d ) => Max( Max( a, b ), Max( c, d ) );

        /// <summary>Returns the smallest of the two values</summary>
        [MethodImpl( INLINE )]
        public static int Min( int a, int b ) => a < b ? a : b;

        /// <summary>Returns the smallest of the three values</summary>
        [MethodImpl( INLINE )]
        public static int Min( int a, int b, int c ) => Min( Min( a, b ), c );

        /// <summary>Returns the smallest of the four values</summary>
        [MethodImpl( INLINE )]
        public static int Min( int a, int b, int c, int d ) => Min( Min( a, b ), Min( c, d ) );

        /// <summary>Returns the largest of the two values</summary>
        [MethodImpl( INLINE )]
        public static int Max( int a, int b ) => a > b ? a : b;

        /// <summary>Returns the largest of the three values</summary>
        [MethodImpl( INLINE )]
        public static int Max( int a, int b, int c ) => Max( Max( a, b ), c );

        /// <summary>Returns the largest of the four values</summary>
        [MethodImpl( INLINE )]
        public static int Max( int a, int b, int c, int d ) => Max( Max( a, b ), Max( c, d ) );

        /// <summary>Returns the smallest of the given values</summary>
        [MethodImpl( INLINE )]
        public static float Min( params float [] values ) => values.Min();

        /// <summary>Returns the largest of the given values</summary>
        [MethodImpl( INLINE )]
        public static float Max( params float [] values ) => values.Max();

        /// <summary>Returns the smallest of the given values</summary>
        [MethodImpl( INLINE )]
        public static int Min( params int [] values ) => values.Min();

        /// <summary>Returns the largest of the given values</summary>
        [MethodImpl( INLINE )]
        public static int Max( params int [] values ) => values.Max();

        /// <summary>Returns the minimum value of all components in the vector</summary>
        [MethodImpl( INLINE )]
        public static float Min( Vector2 v ) => Min( v.x, v.y );

        /// <inheritdoc cref="ExtMathfs.Min(Vector2)"/>
        [MethodImpl( INLINE )]
        public static float Min( Vector3 v ) => Min( v.x, v.y, v.z );

        /// <inheritdoc cref="ExtMathfs.Min(Vector2)"/>
        [MethodImpl( INLINE )]
        public static float Min( Vector4 v ) => Min( v.x, v.y, v.z, v.w );

        /// <summary>Returns the maximum value of all components in the vector</summary>
        [MethodImpl( INLINE )]
        public static float Max( Vector2 v ) => Max( v.x, v.y );

        /// <inheritdoc cref="ExtMathfs.Max(Vector2)"/>
        [MethodImpl( INLINE )]
        public static float Max( Vector3 v ) => Max( v.x, v.y, v.z );

        /// <inheritdoc cref="ExtMathfs.Max(Vector2)"/>
        [MethodImpl( INLINE )]
        public static float Max( Vector4 v ) => Max( v.x, v.y, v.z, v.w );

        #endregion

        #region Rounding

        /// <summary>Rounds the value down to the nearest integer</summary>
        [MethodImpl( INLINE )] public static float Floor( float value ) => ( float ) Math.Floor( value );

        /// <summary>Rounds the vector components down to the nearest integer</summary>
        [MethodImpl( INLINE )] public static Vector2 Floor( Vector2 value ) => new Vector2( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ) );

        /// <inheritdoc cref="ExtMathfs.Floor(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3 Floor( Vector3 value ) => new Vector3( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ), ( float ) Math.Floor( value.z ) );

        /// <inheritdoc cref="ExtMathfs.Floor(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector4 Floor( Vector4 value ) => new Vector4( ( float ) Math.Floor( value.x ), ( float ) Math.Floor( value.y ), ( float ) Math.Floor( value.z ), ( float ) Math.Floor( value.w ) );

        /// <summary>Rounds the value down to the nearest integer, returning an int value</summary>
        [MethodImpl( INLINE )] public static int FloorToInt( float value ) => ( int ) Math.Floor( value );

        /// <summary>Rounds the vector components down to the nearest integer, returning an integer vector</summary>
        [MethodImpl( INLINE )] public static Vector2Int FloorToInt( Vector2 value ) => new Vector2Int( ( int ) Math.Floor( value.x ), ( int ) Math.Floor( value.y ) );

        /// <inheritdoc cref="ExtMathfs.FloorToInt(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3Int FloorToInt( Vector3 value ) => new Vector3Int( ( int ) Math.Floor( value.x ), ( int ) Math.Floor( value.y ), ( int ) Math.Floor( value.z ) );

        /// <summary>Rounds the value up to the nearest integer</summary>
        [MethodImpl( INLINE )] public static float Ceil( float value ) => ( float ) Math.Ceiling( value );

        /// <summary>Rounds the vector components up to the nearest integer</summary>
        [MethodImpl( INLINE )] public static Vector2 Ceil( Vector2 value ) => new Vector2( ( float ) Math.Ceiling( value.x ), ( float ) Math.Ceiling( value.y ) );

        /// <inheritdoc cref="ExtMathfs.Ceil(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3 Ceil( Vector3 value ) => new Vector3( ( float ) Math.Ceiling( value.x ), ( float ) Math.Ceiling( value.y ), ( float ) Math.Ceiling( value.z ) );

        /// <inheritdoc cref="ExtMathfs.Ceil(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector4 Ceil( Vector4 value ) => new Vector4( ( float ) Math.Ceiling( value.x ), ( float ) Math.Ceiling( value.y ), ( float ) Math.Ceiling( value.z ), ( float ) Math.Ceiling( value.w ) );

        /// <summary>Rounds the value up to the nearest integer, returning an int value</summary>
        [MethodImpl( INLINE )] public static int CeilToInt( float value ) => ( int ) Math.Ceiling( value );

        /// <summary>Rounds the vector components up to the nearest integer, returning an integer vector</summary>
        [MethodImpl( INLINE )] public static Vector2Int CeilToInt( Vector2 value ) => new Vector2Int( ( int ) Math.Ceiling( value.x ), ( int ) Math.Ceiling( value.y ) );

        /// <inheritdoc cref="ExtMathfs.CeilToInt(Vector2)"/>
        [MethodImpl( INLINE )] public static Vector3Int CeilToInt( Vector3 value ) => new Vector3Int( ( int ) Math.Ceiling( value.x ), ( int ) Math.Ceiling( value.y ), ( int ) Math.Ceiling( value.z ) );

        /// <summary>Rounds the value to the nearest integer</summary>
        [MethodImpl( INLINE )] public static float Round( float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => ( float ) MathF.Round( value, midpointRounding );

        /// <summary>Rounds the vector components to the nearest integer</summary>
        [MethodImpl( INLINE )] public static Vector2 Round( Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2( ( float ) Math.Round( value.x, midpointRounding ), ( float ) Math.Round( value.y, midpointRounding ) );

        /// <inheritdoc cref="ExtMathfs.Round(Vector2,MidpointRounding)"/>
        [MethodImpl( INLINE )] public static Vector3 Round( Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3( ( float ) Math.Round( value.x, midpointRounding ), ( float ) Math.Round( value.y, midpointRounding ), ( float ) Math.Round( value.z, midpointRounding ) );

        /// <inheritdoc cref="ExtMathfs.Round(Vector2,MidpointRounding)"/>
        [MethodImpl( INLINE )] public static Vector4 Round( Vector4 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector4( ( float ) Math.Round( value.x, midpointRounding ), ( float ) Math.Round( value.y, midpointRounding ), ( float ) Math.Round( value.z, midpointRounding ), ( float ) Math.Round( value.w, midpointRounding ) );

        /// <summary>Rounds the value to the nearest value, snapped to the given interval size</summary>
        [MethodImpl( INLINE )] public static float Round( float value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => ( float ) Math.Round( value / snapInterval, midpointRounding ) * snapInterval;

        /// <summary>Rounds the vector components to the nearest value, snapped to the given interval size</summary>
        [MethodImpl( INLINE )] public static Vector2 Round( Vector2 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ) );

        /// <inheritdoc cref="ExtMathfs.Round(Vector2,float,MidpointRounding)"/>
        [MethodImpl( INLINE )] public static Vector3 Round( Vector3 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ), Round( value.z, snapInterval, midpointRounding ) );

        /// <inheritdoc cref="ExtMathfs.Round(Vector2,float,MidpointRounding)"/>
        [MethodImpl( INLINE )] public static Vector4 Round( Vector4 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector4( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ), Round( value.z, snapInterval, midpointRounding ), Round( value.w, snapInterval, midpointRounding ) );

        /// <summary>Rounds the value to the nearest integer, returning an int value</summary>
        [MethodImpl( INLINE )] public static int RoundToInt( float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => ( int ) Math.Round( value, midpointRounding );

        /// <summary>Rounds the vector components to the nearest integer, returning an integer vector</summary>
        [MethodImpl( INLINE )] public static Vector2Int RoundToInt( Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2Int( ( int ) Math.Round( value.x, midpointRounding ), ( int ) Math.Round( value.y, midpointRounding ) );

        /// <inheritdoc cref="ExtMathfs.RoundToInt(Vector2,MidpointRounding)"/>
        [MethodImpl( INLINE )] public static Vector3Int RoundToInt( Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3Int( ( int ) Math.Round( value.x, midpointRounding ), ( int ) Math.Round( value.y, midpointRounding ), ( int ) Math.Round( value.z, midpointRounding ) );

        #endregion
    }
}