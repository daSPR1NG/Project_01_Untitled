using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> Rotator component permits that this object rotates in x, y, or z axis <summary>
    [Component("Rotator", "")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Rotator : MonoBehaviour, IDebuggable
    {
        [Title( "SETTINGS", 12, "white" )]

        [SerializeField] private bool _applyRotation = true;
        [SerializeField] private Vector3 _rotationAxis = Vector3.zero;
        [SerializeField] private float _rotationSpeed = 5f;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        void Update() => ApplyRotationAtRuntime();

        private void ApplyRotationAtRuntime()
        {
            if ( !_applyRotation || _rotationAxis == Vector3.zero ) { return; }

            transform.Rotate( _rotationSpeed * Time.deltaTime * _rotationAxis );

        }
    }
}