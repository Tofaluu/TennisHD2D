using UnityEngine;

namespace TennisHD2D.Rendering
{
    /// <summary>
    /// HD-2D billboard: yaws around world Y to match the camera, plus an optional partial
    /// lean toward the camera's pitch. Sprites stay upright (feet planted, shadows sane)
    /// instead of copying the full camera rotation and tipping over.
    /// Put the sprite's pivot at its feet so the lean rotates around the ground contact.
    /// </summary>
    [ExecuteAlways]
    public class Billboard : MonoBehaviour
    {
        [Tooltip("Camera to face. Falls back to Camera.main when empty.")]
        [SerializeField] private Camera targetCamera;

        [Tooltip("0 = fully upright (Y-axis only), 1 = match the camera's pitch completely.")]
        [Range(0f, 1f)]
        [SerializeField] private float tilt = 0f;

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
                if (targetCamera == null) return;
            }

            Transform cam = targetCamera.transform;

            // Camera forward flattened onto the ground plane; use camera up when looking straight down.
            Vector3 flatForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
            if (flatForward.sqrMagnitude < 1e-6f)
                flatForward = Vector3.ProjectOnPlane(cam.up, Vector3.up);
            flatForward.Normalize();

            Quaternion upright = Quaternion.LookRotation(flatForward, Vector3.up);

            // Signed camera pitch (positive = looking down), scaled by tilt.
            float pitch = Vector3.SignedAngle(flatForward, cam.forward, upright * Vector3.right);
            transform.rotation = upright * Quaternion.Euler(pitch * tilt, 0f, 0f);
        }
    }
}
