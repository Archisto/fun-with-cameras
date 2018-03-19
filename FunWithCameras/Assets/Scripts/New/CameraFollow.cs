using UnityEngine;

namespace TankGame
{
    public class CameraFollow : MonoBehaviour, ICameraFollow
    {
        [SerializeField]
        private float angle;

        [SerializeField]
        private float distance;

        [SerializeField]
        private Transform target;

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if (target != null)
            {
                GoToPosition();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GoToPosition()
        {
            Quaternion newRotation = transform.rotation;
            Vector3 newPosition = target.position;

            float angleRad = Mathf.Deg2Rad * angle;

            float a = Mathf.Sin(angleRad) * distance;
            float b = Mathf.Cos(angleRad) * distance;

            newPosition -= target.forward * a;
            newPosition += Vector3.up * b;

            float angleX = -1 * angle + 90;

            newRotation = Quaternion.Euler(angleX, target.rotation.eulerAngles.y, 0);

            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        /// <summary>
        /// Sets the camera's angle.
        /// </summary>
        /// <param name="angle">an angle for the camera
        /// (0 = down, 90 = straight forward)</param>
        public void SetAngle(float angle)
        {
            this.angle = angle;
        }

        /// <summary>
        /// Sets the camera's distance from the target transform.
        /// </summary>
        /// <param name="distance">a distance</param>
        public void SetDistance(float distance)
        {
            this.distance = distance;
        }

        /// <summary>
        /// Sets the target transform at which the camera looks.
        /// </summary>
        /// <param name="targetTransform">a target transform</param>
        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
        }
    }
}
