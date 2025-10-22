using UnityEngine;

namespace KadaXuanwu.Utils.Runtime.Components {
    /// <summary>
    /// Component that makes a GameObject follow a target transform with optional offset in position and rotation.
    /// Updates in LateUpdate to ensure smooth following after all other updates.
    /// </summary>
    public class FollowTransformCode : MonoBehaviour {
        private Transform _targetTransform;
        private Vector3 _offsetPosition;
        private Quaternion _offsetRotation;

        /// <summary>
        /// Sets the target transform to follow without any offset.
        /// </summary>
        /// <param name="targetTransform">The transform to follow.</param>
        public void SetTargetTransform(Transform targetTransform) {
            _targetTransform = targetTransform;
            _offsetPosition = Vector3.zero;
            _offsetRotation = Quaternion.identity;
        }

        /// <summary>
        /// Sets the target transform to follow with position and rotation offsets.
        /// The offset position is applied in the target's local space.
        /// </summary>
        /// <param name="targetTransform">The transform to follow.</param>
        /// <param name="offsetPosition">Position offset in target's local space.</param>
        /// <param name="offsetRotation">Rotation offset applied after target's rotation.</param>
        public void SetTargetTransform(Transform targetTransform, Vector3 offsetPosition, Quaternion offsetRotation) {
            _targetTransform = targetTransform;
            _offsetPosition = offsetPosition;
            _offsetRotation = offsetRotation;
        }

        /// <summary>
        /// Clears the target transform and resets all offsets to default values.
        /// The GameObject will stop following any target.
        /// </summary>
        public void ResetTargetTransform() {
            _targetTransform = null;
            _offsetPosition = Vector3.zero;
            _offsetRotation = Quaternion.identity;
        }

        private void LateUpdate() {
            if (_targetTransform == null) {
                return;
            }

            Vector3 rotatedOffset = _targetTransform.rotation * _offsetPosition;
            transform.SetPositionAndRotation(_targetTransform.position + rotatedOffset, _targetTransform.rotation * _offsetRotation);
        }
    }
}
