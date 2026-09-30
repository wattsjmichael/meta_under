using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Under.Foundation
{
    /// <summary>
    /// Marks an interactable as eligible for gesture recall and safely moves it back to a hand.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public sealed class RecallableObject : MonoBehaviour
    {
        static readonly HashSet<RecallableObject> s_Recallables = new HashSet<RecallableObject>();

        [Header("Return Movement")]
        [SerializeField, Min(0.05f)]
        [Tooltip("Time in seconds for the object to travel from its starting position to the requesting hand.")]
        float m_ReturnDuration = 0.25f;

        [SerializeField]
        [Tooltip("Normalized position over the return duration. The evaluated value is clamped to prevent overshoot.")]
        AnimationCurve m_ReturnEasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField, Min(0.001f)]
        [Tooltip("Distance from the moving hand target at which Recall completes early.")]
        float m_ArrivalDistance = 0.015f;

        [Header("Recall Eligibility")]
        [SerializeField, Min(0f)]
        [Tooltip("Minimum distance from the requesting hand before this object can be recalled.")]
        float m_MinimumRecallDistance = 0.35f;

        [SerializeField, Min(0f)]
        [Tooltip("How long the arrived object remains positioned within immediate direct-grab range.")]
        float m_ArrivalCatchDuration = 1f;

        Rigidbody m_Rigidbody;
        XRGrabInteractable m_GrabInteractable;
        Transform m_TargetAnchor;
        Vector3 m_TargetWorldOffset;
        Vector3 m_ReturnStartPosition;
        float m_ReturnStartTime;
        bool m_IsRecalling;
        bool m_IsWaitingForGrab;
        float m_ArrivalCatchEndTime;
        bool m_OriginalUseGravity;
        bool m_OriginalIsKinematic;
        bool m_OriginalDetectCollisions;
        bool m_OriginalInteractableEnabled;

        /// <summary>
        /// Whether this object is currently returning to a requesting hand.
        /// </summary>
        public bool isRecalling => m_IsRecalling || m_IsWaitingForGrab;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetRegistry()
        {
            s_Recallables.Clear();
        }

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_GrabInteractable = GetComponent<XRGrabInteractable>();
            m_GrabInteractable.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDestroy()
        {
            if (m_GrabInteractable != null)
                m_GrabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }

        void OnEnable()
        {
            s_Recallables.Add(this);
        }

        void OnDisable()
        {
            s_Recallables.Remove(this);
            if (isRecalling)
                RestoreNormalState();
        }

        void Update()
        {
            if (m_IsWaitingForGrab)
            {
                UpdateArrivalCatch();
                return;
            }

            if (!m_IsRecalling)
                return;

            if (m_TargetAnchor == null)
            {
                RestoreNormalState();
                return;
            }

            var targetPosition = GetTargetPosition();
            var normalizedTime = Mathf.Clamp01(
                (Time.unscaledTime - m_ReturnStartTime) / Mathf.Max(0.05f, m_ReturnDuration));
            var easedProgress = m_ReturnEasing == null
                ? Mathf.SmoothStep(0f, 1f, normalizedTime)
                : Mathf.Clamp01(m_ReturnEasing.Evaluate(normalizedTime));

            transform.position = Vector3.Lerp(m_ReturnStartPosition, targetPosition, easedProgress);

            if (normalizedTime >= 1f ||
                (transform.position - targetPosition).sqrMagnitude <= m_ArrivalDistance * m_ArrivalDistance)
            {
                transform.position = targetPosition;
                BeginArrivalCatch();
            }
        }

        void FixedUpdate()
        {
            if (!m_IsWaitingForGrab || m_TargetAnchor == null || m_GrabInteractable.isSelected)
                return;

            // The catch window follows the hand with a dynamic Rigidbody so XRI captures
            // the correct pre-grab physics state. Recall never stays kinematic once the
            // interactable becomes selectable again.
            ClearMotion();
            m_Rigidbody.MovePosition(GetTargetPosition());
        }

        /// <summary>
        /// Recalls the nearest eligible marked object to the supplied hand anchor.
        /// </summary>
        public static bool TryRecallNearest(
            Transform targetAnchor,
            Vector3 targetWorldOffset,
            Handedness requestingHand)
        {
            if (targetAnchor == null)
                return false;

            RecallableObject closest = null;
            var closestDistanceSquared = float.PositiveInfinity;
            foreach (var recallable in s_Recallables)
            {
                if (recallable == null || !recallable.CanRecallFrom(targetAnchor.position))
                    continue;

                var distanceSquared = (recallable.transform.position - targetAnchor.position).sqrMagnitude;
                if (distanceSquared >= closestDistanceSquared)
                    continue;

                closest = recallable;
                closestDistanceSquared = distanceSquared;
            }

            return closest != null && closest.BeginRecall(targetAnchor, targetWorldOffset, requestingHand);
        }

        bool CanRecallFrom(Vector3 requestingPosition)
        {
            if (!isActiveAndEnabled || isRecalling || m_GrabInteractable == null || m_GrabInteractable.isSelected)
                return false;

            var minimumDistanceSquared = m_MinimumRecallDistance * m_MinimumRecallDistance;
            return (transform.position - requestingPosition).sqrMagnitude >= minimumDistanceSquared;
        }

        bool BeginRecall(Transform targetAnchor, Vector3 targetWorldOffset, Handedness requestingHand)
        {
            if (targetAnchor == null || isRecalling || m_GrabInteractable.isSelected)
                return false;

            m_TargetAnchor = targetAnchor;
            m_TargetWorldOffset = targetWorldOffset;
            m_ReturnStartPosition = transform.position;
            m_ReturnStartTime = Time.unscaledTime;
            m_IsRecalling = true;

            m_OriginalUseGravity = m_Rigidbody.useGravity;
            m_OriginalIsKinematic = m_Rigidbody.isKinematic;
            m_OriginalDetectCollisions = m_Rigidbody.detectCollisions;
            m_OriginalInteractableEnabled = m_GrabInteractable.enabled;

            ClearMotion();
            m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.detectCollisions = false;
            m_GrabInteractable.enabled = false;

            Debug.Log($"Recall started: {name} requested by {requestingHand} hand.", this);
            return true;
        }

        Vector3 GetTargetPosition()
        {
            return m_TargetAnchor.position + m_TargetWorldOffset;
        }

        void BeginArrivalCatch()
        {
            m_IsRecalling = false;
            m_IsWaitingForGrab = true;
            m_ArrivalCatchEndTime = Time.unscaledTime + m_ArrivalCatchDuration;

            // Restore the complete normal Rigidbody state before XRI can select the object.
            // XRGrabInteractable snapshots this state on select and restores it before applying
            // detach velocity, so enabling the interactable while kinematic breaks throwing.
            RestoreRigidbodyPhysicsState();
            ClearMotion();
            m_GrabInteractable.enabled = m_OriginalInteractableEnabled;

            Debug.Log($"Recall arrived: {name} is within direct-grab range.", this);
        }

        void UpdateArrivalCatch()
        {
            if (m_TargetAnchor == null)
            {
                RestoreNormalState();
                return;
            }

            if (m_GrabInteractable.isSelected)
            {
                EndArrivalCatchControl();
                return;
            }

            if (Time.unscaledTime >= m_ArrivalCatchEndTime)
                RestoreNormalState();
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (m_IsWaitingForGrab)
                EndArrivalCatchControl();
        }

        void EndArrivalCatchControl()
        {
            m_IsWaitingForGrab = false;
            m_TargetAnchor = null;
            m_ArrivalCatchEndTime = 0f;
        }

        void RestoreNormalState()
        {
            m_IsRecalling = false;
            m_IsWaitingForGrab = false;
            m_TargetAnchor = null;

            RestoreRigidbodyPhysicsState();
            ClearMotion();
            m_GrabInteractable.enabled = m_OriginalInteractableEnabled;
        }

        void RestoreRigidbodyPhysicsState()
        {
            m_Rigidbody.detectCollisions = m_OriginalDetectCollisions;
            m_Rigidbody.isKinematic = m_OriginalIsKinematic;
            m_Rigidbody.useGravity = m_OriginalUseGravity;
        }

        void ClearMotion()
        {
            if (m_Rigidbody.isKinematic)
                return;

#if UNITY_2023_3_OR_NEWER
            m_Rigidbody.linearVelocity = Vector3.zero;
#else
            m_Rigidbody.velocity = Vector3.zero;
#endif
            m_Rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
