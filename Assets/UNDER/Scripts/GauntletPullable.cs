using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Under.Foundation
{
    /// <summary>
    /// Explicitly opts an existing RecallableObject into gauntlet targeting.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RecallableObject))]
    public sealed class GauntletPullable : MonoBehaviour
    {
        static readonly HashSet<GauntletPullable> s_ActivePullables = new HashSet<GauntletPullable>();

        [SerializeField]
        Transform m_TargetPoint;

        [SerializeField]
        GameObject m_TargetIndicator;

        readonly HashSet<int> m_TargetingHands = new HashSet<int>();
        RecallableObject m_Recallable;
        Collider[] m_Colliders;

        public Vector3 targetPosition
        {
            get
            {
                if (m_TargetPoint != null)
                    return m_TargetPoint.position;

                if (m_Colliders != null && m_Colliders.Length > 0)
                {
                    var bounds = m_Colliders[0].bounds;
                    for (var i = 1; i < m_Colliders.Length; i++)
                        bounds.Encapsulate(m_Colliders[i].bounds);
                    return bounds.center;
                }

                return transform.position;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetRegistry()
        {
            s_ActivePullables.Clear();
        }

        void Awake()
        {
            m_Recallable = GetComponent<RecallableObject>();
            m_Colliders = GetComponentsInChildren<Collider>(true);
            UpdateIndicator();
        }

        void OnEnable()
        {
            s_ActivePullables.Add(this);
        }

        void OnDisable()
        {
            s_ActivePullables.Remove(this);
            m_TargetingHands.Clear();
            UpdateIndicator();
        }

        public static void GetActivePullables(List<GauntletPullable> results)
        {
            results.Clear();
            foreach (var pullable in s_ActivePullables)
            {
                if (pullable != null)
                    results.Add(pullable);
            }
        }

        public bool CanTargetFrom(Vector3 handPosition)
        {
            return isActiveAndEnabled &&
                m_Recallable != null &&
                m_Recallable.CanRecallFrom(handPosition);
        }

        public bool ContainsCollider(Collider candidateCollider)
        {
            return candidateCollider != null && candidateCollider.transform.IsChildOf(transform);
        }

        public void SetTargeted(Object targetingHand, bool targeted)
        {
            if (targetingHand == null)
                return;

            if (targeted)
                m_TargetingHands.Add(targetingHand.GetInstanceID());
            else
                m_TargetingHands.Remove(targetingHand.GetInstanceID());

            UpdateIndicator();
        }

        public bool TryPull(
            Transform targetAnchor,
            Vector3 targetWorldOffset,
            Handedness requestingHand,
            float pullSpeed)
        {
            return m_Recallable != null &&
                m_Recallable.TryBeginDirectedPull(
                    targetAnchor,
                    targetWorldOffset,
                    requestingHand,
                    pullSpeed);
        }

        public void Configure(GameObject targetIndicator, Transform targetPoint = null)
        {
            m_TargetIndicator = targetIndicator;
            m_TargetPoint = targetPoint;
            UpdateIndicator();
        }

        void UpdateIndicator()
        {
            if (m_TargetIndicator != null)
                m_TargetIndicator.SetActive(isActiveAndEnabled && m_TargetingHands.Count > 0);
        }
    }
}
