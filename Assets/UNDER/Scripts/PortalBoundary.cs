using UnityEngine;

namespace Under.Foundation
{
    /// <summary>
    /// Defines a finite, planar opening between the normal room and UNDER.
    /// The boundary is physics-independent: crossing objects keep one world-space pose.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PortalBoundary : MonoBehaviour
    {
        public enum Side
        {
            Under,
            Room,
        }

        [SerializeField]
        [Tooltip("World-space width and depth of the usable aperture, measured along this transform's right and forward axes.")]
        Vector2 m_ApertureSize = new Vector2(0.50f, 0.34f);

        public Vector2 apertureSize => m_ApertureSize;

        public float GetSignedDistance(Vector3 worldPosition)
        {
            return Vector3.Dot(worldPosition - transform.position, transform.up);
        }

        public bool ContainsProjectedPoint(Vector3 worldPosition, float padding = 0f)
        {
            var offset = worldPosition - transform.position;
            var halfWidth = Mathf.Max(0f, m_ApertureSize.x * 0.5f + padding);
            var halfDepth = Mathf.Max(0f, m_ApertureSize.y * 0.5f + padding);
            return Mathf.Abs(Vector3.Dot(offset, transform.right)) <= halfWidth &&
                Mathf.Abs(Vector3.Dot(offset, transform.forward)) <= halfDepth;
        }

        public bool TryGetCrossing(
            Vector3 fromWorldPosition,
            Vector3 toWorldPosition,
            out Side destinationSide,
            float edgePadding = 0f)
        {
            var fromDistance = GetSignedDistance(fromWorldPosition);
            var toDistance = GetSignedDistance(toWorldPosition);
            destinationSide = toDistance >= 0f ? Side.Room : Side.Under;

            if ((fromDistance < 0f && toDistance < 0f) ||
                (fromDistance > 0f && toDistance > 0f))
            {
                return false;
            }

            var denominator = fromDistance - toDistance;
            if (Mathf.Abs(denominator) <= Mathf.Epsilon)
                return false;

            var crossingProgress = Mathf.Clamp01(fromDistance / denominator);
            var crossingPoint = Vector3.Lerp(fromWorldPosition, toWorldPosition, crossingProgress);
            return ContainsProjectedPoint(crossingPoint, edgePadding);
        }

        public void Configure(Vector2 apertureSize)
        {
            m_ApertureSize = apertureSize;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.1f, 0.9f, 1f, 0.8f);
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(m_ApertureSize.x, 0.002f, m_ApertureSize.y));
            Gizmos.matrix = oldMatrix;
        }
    }
}
