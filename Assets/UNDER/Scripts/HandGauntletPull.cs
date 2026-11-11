using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Under.Foundation
{
    /// <summary>
    /// Selects a marked object in a narrow palm-forward cone, then recognizes a
    /// deliberate open-palm retraction before starting the shared Recall movement.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HandGauntletPull : MonoBehaviour
    {
        static readonly List<XRHandSubsystem> s_HandSubsystems = new List<XRHandSubsystem>();
        static readonly List<GauntletPullable> s_Pullables = new List<GauntletPullable>();

        [Header("Hand")]
        [SerializeField]
        Handedness m_Handedness;

        [SerializeField]
        Transform m_PullAnchor;

        [SerializeField]
        Vector3 m_ArrivalWorldOffset = new Vector3(0f, 0.08f, 0f);

        [Header("Targeting")]
        [SerializeField, Min(0f)]
        float m_MinTargetRange = 0.30f;

        [SerializeField, Min(0.1f)]
        float m_MaxTargetRange = 2.50f;

        [SerializeField, Range(2f, 45f)]
        float m_TargetConeHalfAngle = 12f;

        [SerializeField, Range(10f, 90f)]
        [Tooltip("Maximum angle between the presented palm and an acquired target while arming and pulling.")]
        float m_PalmFacingAngleThreshold = 35f;

        [SerializeField, Min(0f)]
        float m_TargetGraceDuration = 0.30f;

        [SerializeField]
        bool m_RequireLineOfSight = true;

        [Header("Pull Gesture")]
        [SerializeField, Min(0.05f)]
        [Tooltip("How long the same candidate must remain stably targeted before a yank can trigger the pull.")]
        float m_AcquisitionHoldTime = 0.20f;

        [SerializeField, Min(0.02f)]
        float m_MinRetractionDistance = 0.07f;

        [SerializeField, Min(0.1f)]
        [Tooltip("Object travel speed in meters per second after the pull gesture succeeds.")]
        float m_PullSpeed = 6.0f;

        XRHandSubsystem m_Subsystem;
        XRBaseInteractor[] m_HandInteractors;
        Transform m_OriginTransform;
        GauntletPullable m_Candidate;
        Vector3 m_FurthestWristPosition;
        Vector3 m_ArmedForward;
        float m_AcquisitionStartTime = -1f;
        float m_LastCandidateSeenTime;
        bool m_AcquisitionArmed;

        public void Configure(Handedness handedness, Transform pullAnchor)
        {
            m_Handedness = handedness;
            m_PullAnchor = pullAnchor;
        }

        void Awake()
        {
            m_HandInteractors = GetComponentsInChildren<XRBaseInteractor>(true);
            var origin = GetComponentInParent<XROrigin>();
            m_OriginTransform = origin != null && origin.Origin != null ? origin.Origin.transform : null;
        }

        void OnEnable()
        {
            TrySubscribeToHandSubsystem();
        }

        void OnDisable()
        {
            UnsubscribeFromHandSubsystem();
            ClearCandidate();
        }

        void Update()
        {
            if (m_Subsystem == null || !m_Subsystem.running)
                TrySubscribeToHandSubsystem();
        }

        void TrySubscribeToHandSubsystem()
        {
            s_HandSubsystems.Clear();
            SubsystemManager.GetSubsystems(s_HandSubsystems);
            foreach (var subsystem in s_HandSubsystems)
            {
                if (!subsystem.running)
                    continue;

                if (m_Subsystem == subsystem)
                    return;

                UnsubscribeFromHandSubsystem();
                m_Subsystem = subsystem;
                m_Subsystem.updatedHands += OnUpdatedHands;
                return;
            }
        }

        void UnsubscribeFromHandSubsystem()
        {
            if (m_Subsystem != null)
                m_Subsystem.updatedHands -= OnUpdatedHands;
            m_Subsystem = null;
        }

        void OnUpdatedHands(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
            XRHandSubsystem.UpdateType updateType)
        {
            if (updateType != XRHandSubsystem.UpdateType.Dynamic)
                return;

            var requiredFlag = m_Handedness == Handedness.Left
                ? XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints
                : XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;
            if ((updateSuccessFlags & requiredFlag) != requiredFlag)
            {
                ClearCandidate();
                return;
            }

            var hand = m_Handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            if (!hand.isTracked || IsHandHoldingSomething() ||
                !TryGetTargetingPose(hand, out var wristPosition, out var palmDirection))
            {
                ClearCandidate();
                return;
            }

            if (!IsHandOpen(hand))
            {
                ClearCandidate();
                return;
            }

            UpdateCandidate(wristPosition, palmDirection);

            if (m_Candidate == null)
                return;

            if (!m_Candidate.CanTargetFrom(wristPosition))
            {
                ClearCandidate();
                return;
            }

            var toCandidate = m_Candidate.targetPosition - wristPosition;
            if (!IsPalmFacingDirection(palmDirection, toCandidate))
            {
                ClearCandidate();
                return;
            }

            if (!m_AcquisitionArmed)
            {
                if (m_AcquisitionStartTime < 0f ||
                    Time.unscaledTime - m_AcquisitionStartTime < m_AcquisitionHoldTime)
                {
                    return;
                }

                m_AcquisitionArmed = true;
                m_FurthestWristPosition = wristPosition;
                m_ArmedForward = toCandidate.normalized;
                Debug.Log($"Gauntlet target armed: {m_Candidate.name} by {m_Handedness} open palm.", this);
            }

            var forwardExtension = Vector3.Dot(wristPosition - m_FurthestWristPosition, m_ArmedForward);
            if (forwardExtension > 0f)
                m_FurthestWristPosition = wristPosition;

            var retraction = Vector3.Dot(m_FurthestWristPosition - wristPosition, m_ArmedForward);
            if (retraction < m_MinRetractionDistance)
                return;

            var pulled = m_Candidate.TryPull(
                m_PullAnchor,
                m_ArrivalWorldOffset,
                m_Handedness,
                m_PullSpeed);
            if (pulled)
                Debug.Log($"Gauntlet pull started: {m_Candidate.name} requested by {m_Handedness} hand.", this);

            ClearCandidate();
        }

        void UpdateCandidate(Vector3 wristPosition, Vector3 forward)
        {
            var best = FindBestCandidate(wristPosition, forward);
            if (best == null)
            {
                if (!m_AcquisitionArmed)
                    m_AcquisitionStartTime = -1f;

                if (m_Candidate != null && Time.unscaledTime - m_LastCandidateSeenTime > m_TargetGraceDuration)
                    ClearCandidate();
                return;
            }

            m_LastCandidateSeenTime = Time.unscaledTime;
            if (best == m_Candidate)
            {
                if (!m_AcquisitionArmed && m_AcquisitionStartTime < 0f)
                    m_AcquisitionStartTime = Time.unscaledTime;
                return;
            }

            ClearCandidate();
            m_Candidate = best;
            m_Candidate.SetTargeted(this, true);
            m_FurthestWristPosition = wristPosition;
            m_ArmedForward = (m_Candidate.targetPosition - wristPosition).normalized;
            m_AcquisitionStartTime = Time.unscaledTime;
            Debug.Log($"Gauntlet target acquired: {m_Candidate.name} by {m_Handedness} open palm.", this);
        }

        GauntletPullable FindBestCandidate(Vector3 origin, Vector3 forward)
        {
            GauntletPullable best = null;
            var bestScore = float.PositiveInfinity;
            var minimumDot = Mathf.Cos(m_TargetConeHalfAngle * Mathf.Deg2Rad);
            GauntletPullable.GetActivePullables(s_Pullables);

            foreach (var pullable in s_Pullables)
            {
                if (pullable == null || !pullable.CanTargetFrom(origin))
                    continue;

                var toTarget = pullable.targetPosition - origin;
                var distance = toTarget.magnitude;
                if (distance < m_MinTargetRange || distance > m_MaxTargetRange || distance <= Mathf.Epsilon)
                    continue;

                var direction = toTarget / distance;
                var alignment = Vector3.Dot(forward, direction);
                if (alignment < minimumDot || (m_RequireLineOfSight && !HasLineOfSight(pullable, origin, direction, distance)))
                    continue;

                var angularPenalty = 1f - alignment;
                var score = angularPenalty * 8f + distance / Mathf.Max(0.1f, m_MaxTargetRange);
                if (score >= bestScore)
                    continue;

                best = pullable;
                bestScore = score;
            }

            return best;
        }

        bool HasLineOfSight(GauntletPullable pullable, Vector3 origin, Vector3 direction, float distance)
        {
            var hits = Physics.RaycastAll(origin, direction, distance, ~0, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform))
                    continue;
                return pullable.ContainsCollider(hit.collider);
            }

            return true;
        }

        bool TryGetTargetingPose(XRHand hand, out Vector3 wristPosition, out Vector3 palmDirection)
        {
            wristPosition = default;
            palmDirection = default;
            if (!hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose))
                return false;

            wristPosition = wristPose.position;
            palmDirection = hand.rootPose.rotation * Vector3.down;
            if (m_OriginTransform != null)
            {
                wristPosition = m_OriginTransform.TransformPoint(wristPosition);
                palmDirection = m_OriginTransform.TransformDirection(palmDirection);
            }

            palmDirection.Normalize();
            return palmDirection.sqrMagnitude > 0.5f;
        }

        bool IsPalmFacingDirection(Vector3 palmDirection, Vector3 targetDirection)
        {
            if (targetDirection.sqrMagnitude <= Mathf.Epsilon)
                return false;

            var minimumDot = Mathf.Cos(m_PalmFacingAngleThreshold * Mathf.Deg2Rad);
            return Vector3.Dot(palmDirection, targetDirection.normalized) >= minimumDot;
        }

        bool IsHandHoldingSomething()
        {
            foreach (var interactor in m_HandInteractors)
            {
                if (interactor != null && interactor.hasSelection)
                    return true;
            }
            return false;
        }

        static bool IsHandOpen(XRHand hand)
        {
            var extendedCount = 0;
            if (IsFingerExtended(hand, XRHandJointID.IndexProximal, XRHandJointID.IndexTip)) extendedCount++;
            if (IsFingerExtended(hand, XRHandJointID.MiddleProximal, XRHandJointID.MiddleTip)) extendedCount++;
            if (IsFingerExtended(hand, XRHandJointID.RingProximal, XRHandJointID.RingTip)) extendedCount++;
            if (IsFingerExtended(hand, XRHandJointID.LittleProximal, XRHandJointID.LittleTip)) extendedCount++;
            return extendedCount >= 3;
        }

        static bool IsFingerExtended(XRHand hand, XRHandJointID proximalId, XRHandJointID tipId)
        {
            if (!hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) ||
                !hand.GetJoint(proximalId).TryGetPose(out var proximalPose) ||
                !hand.GetJoint(tipId).TryGetPose(out var tipPose))
                return false;

            return (tipPose.position - wristPose.position).sqrMagnitude >
                (proximalPose.position - wristPose.position).sqrMagnitude;
        }

        void ClearCandidate()
        {
            if (m_Candidate != null)
                m_Candidate.SetTargeted(this, false);
            m_Candidate = null;
            m_AcquisitionStartTime = -1f;
            m_AcquisitionArmed = false;
        }
    }
}
