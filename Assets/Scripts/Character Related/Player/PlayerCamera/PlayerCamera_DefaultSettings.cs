using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using System;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum Projection { Perspective, Orthographic }

    ///<summary> 
    /// PlayerCamera_DefaultSettings contains the camera features that will be shared by other purpose camera components. 
    ///<summary>
    public abstract class PlayerCamera_DefaultSettings : MonoBehaviour, IDebuggable
    {
        // Stay this block at top
        #region Debug

        [Space( 10 )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        [HorizontalLine( .5f, EColor.Gray )]

        #endregion

        [Header( "DEFAULT SETTINGS" )]

        [SerializeField] protected Projection ProjectionType = Projection.Orthographic;
        private bool _isCameraOrthographic;

        protected void SetProjectionType( Projection projectionType )
        {
            if ( ProjectionType == projectionType ) { return; }

            ProjectionType = projectionType;

            Helper.Log( this, "Camera projection is " + projectionType.ToLogComponent() + " ." );
        }
        private void ApplyProjectionType( Projection projectionType )
        {
            SetProjectionType( projectionType );

            switch ( projectionType )
            {
                case Projection.Perspective:
                    Helper.GetMainCamera().orthographic = false;
                    _isCameraOrthographic = false;
                    break;
                case Projection.Orthographic:
                    Helper.GetMainCamera().orthographic = true;
                    _isCameraOrthographic = true;
                    break;
            }            
        }

        protected bool IsCameraOrthographic() { return _isCameraOrthographic; }

        #region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            ApplyProjectionType( ProjectionType );
        }

#endif
        #endregion
    }
}