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

        [Title( "DEFAULT SETTINGS", 12, "white" )]

        [SerializeField] protected Projection ProjectionType = Projection.Perspective;

        private bool _isCameraOrthographic = false;        

        private void ChangeProjectionType( Projection projectionType )
        {
            SetProjectionType( projectionType );

            switch ( projectionType )
            {
                case Projection.Perspective:
                    Helper.MainCamera().orthographic = false;
                    break;
                case Projection.Orthographic:
                    Helper.MainCamera().orthographic = true;
                    break;
            }            
        }

        protected void SetProjectionType( Projection projectionType )
        {
            if ( ProjectionType == projectionType ) { return; }

            ProjectionType = projectionType;

            Helper.Log( this, "Camera projection is " + projectionType.ToLogComponent() + " ." );
        }

        protected bool IsCameraOrthographic() { return _isCameraOrthographic; }

        #region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            foreach ( PlayerCamera_DefaultSettings item in transform.GetComponentsInChildren<PlayerCamera_DefaultSettings>() )
            {
                item.ChangeProjectionType( ProjectionType );
                item._isCameraOrthographic = ProjectionType == Projection.Orthographic;
            }
        }
#endif

        #endregion
    }
}