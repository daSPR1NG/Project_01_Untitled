using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    ///<summary> Rotator component permits that this object rotates in x, y, or z axis <summary>
    [Component("Rotator", "")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Rotator : MonoBehaviour, IDebuggable
    {
        [Header( "SETTINGS" )]

        [SerializeField] private bool _applyRotation = true;
        [SerializeField] private Vector3 _rotationAxis = Vector3.zero;
        [SerializeField] private float _rotationSpeed = 5f;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        void Update() => ApplyRotationAtRuntime( _rotationAxis, _rotationSpeed );

        /// <summary>
        /// Rotates this transform around a given axis, at given speed.
        /// </summary>
        /// <param name="axis"> The axis this transform will rotate around </param>
        /// <param name="speed"> The speed at which this transform will rotate </param>
        private void ApplyRotationAtRuntime( Vector3 axis, float speed )
        {
            if ( !_applyRotation || axis == Vector3.zero ) { return; }

            transform.Rotate( speed * Helper.GetRealDeltaTime( true ) * axis );
        }
    }
}