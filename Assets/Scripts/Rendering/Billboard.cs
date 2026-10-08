using UnityEngine;

namespace TennisHD2D.Rendering
{
    [ExecuteInEditMode] // So we can see it working in the Scene view
    public class Billboard : MonoBehaviour
    {
        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            // Standard billboard: face the camera's exact rotation. 
            // In a perspective HD-2D game, this ensures sprites don't warp or skew 
            // when the camera looks at them from an angle or tilts down at the court.
            transform.rotation = _mainCamera.transform.rotation;
        }
    }
}
