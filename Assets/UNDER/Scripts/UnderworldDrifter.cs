using UnityEngine;

namespace Under.Foundation
{
    /// <summary>
    /// Provides a small, deterministic distant motion cue for the Foundation 002 depth test.
    /// </summary>
    public sealed class UnderworldDrifter : MonoBehaviour
    {
        [SerializeField, Min(1f)]
        float m_CycleDuration = 9f;

        [SerializeField]
        Vector3 m_MotionExtents = new Vector3(2.2f, 0.35f, 1.1f);

        [SerializeField, Range(0f, 1f)]
        float m_Phase;

        Vector3 m_Center;

        void Awake()
        {
            m_Center = transform.localPosition;
        }

        void Update()
        {
            var angle = ((Time.time / Mathf.Max(1f, m_CycleDuration)) + m_Phase) * Mathf.PI * 2f;
            transform.localPosition = m_Center + new Vector3(
                Mathf.Sin(angle) * m_MotionExtents.x,
                Mathf.Sin(angle * 2f) * m_MotionExtents.y,
                Mathf.Cos(angle) * m_MotionExtents.z);
        }
    }
}
