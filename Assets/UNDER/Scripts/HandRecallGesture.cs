using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Under.Foundation
{
    /// <summary>
    /// Recognizes an empty, upward-facing palm with the outer fingers extended and an index pinch held.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HandRecallGesture : MonoBehaviour
    {
        static readonly List<XRHandSubsystem> s_HandSubsystems = new List<XRHandSubsystem>();

        [SerializeField]
        Handedness m_Handedness;

        [SerializeField]
        Transform m_RecallAnchor;

        [SerializeField, Min(0.1f)]
        float m_HoldDuration = 0.5f;

        [SerializeField, Range(-1f, 1f)]
        float m_PalmUpDotThreshold = 0.55f;

        [SerializeField]
        Vector3 m_ArrivalWorldOffset = new Vector3(0f, 0.08f, 0f);

        XRHandSubsystem m_Subsystem;
        XRBaseInteractor[] m_HandInteractors;
        Transform m_OriginTransform;
        float m_GestureStartTime = -1f;
        bool m_TriggeredThisGesture;

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
            ResetGesture();
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
                ResetGesture();
                return;
            }

            var hand = m_Handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            var gestureActive = hand.isTracked &&
                IsHandEmpty() &&
                IsPalmFacingUp(hand) &&
                AreOuterFingersExtended(hand) &&
                IsIndexPinching(hand);

            if (!gestureActive)
            {
                ResetGesture();
                return;
            }

            if (m_GestureStartTime < 0f)
                m_GestureStartTime = Time.unscaledTime;

            if (m_TriggeredThisGesture || Time.unscaledTime - m_GestureStartTime < m_HoldDuration)
                return;

            m_TriggeredThisGesture = true;
            Debug.Log($"Recall gesture recognized: {m_Handedness} hand.", this);
            RecallableObject.TryRecallNearest(m_RecallAnchor, m_ArrivalWorldOffset, m_Handedness);
        }

        bool IsHandEmpty()
        {
            foreach (var interactor in m_HandInteractors)
            {
                if (interactor != null && interactor.hasSelection)
                    return false;
            }

            return true;
        }

        bool IsPalmFacingUp(XRHand hand)
        {
            var palmDirection = hand.rootPose.rotation * Vector3.down;
            if (m_OriginTransform != null)
                palmDirection = m_OriginTransform.TransformDirection(palmDirection);

            return Vector3.Dot(palmDirection.normalized, Vector3.up) >= m_PalmUpDotThreshold;
        }

        static bool AreOuterFingersExtended(XRHand hand)
        {
            return IsFingerExtended(hand, XRHandJointID.MiddleProximal, XRHandJointID.MiddleTip) &&
                IsFingerExtended(hand, XRHandJointID.RingProximal, XRHandJointID.RingTip) &&
                IsFingerExtended(hand, XRHandJointID.LittleProximal, XRHandJointID.LittleTip);
        }

        static bool IsFingerExtended(XRHand hand, XRHandJointID proximalId, XRHandJointID tipId)
        {
            if (!hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose) ||
                !hand.GetJoint(proximalId).TryGetPose(out var proximalPose) ||
                !hand.GetJoint(tipId).TryGetPose(out var tipPose))
            {
                return false;
            }

            return (tipPose.position - wristPose.position).sqrMagnitude >
                (proximalPose.position - wristPose.position).sqrMagnitude;
        }

        bool IsIndexPinching(XRHand hand)
        {
            var aimHand = m_Handedness == Handedness.Left ? MetaAimHand.left : MetaAimHand.right;
            if (aimHand != null && aimHand.indexPressed != null)
                return aimHand.indexPressed.isPressed;

            if (!hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbTip) ||
                !hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexTip))
            {
                return false;
            }

            const float fallbackPinchDistance = 0.025f;
            return (thumbTip.position - indexTip.position).sqrMagnitude <=
                fallbackPinchDistance * fallbackPinchDistance;
        }

        void ResetGesture()
        {
            m_GestureStartTime = -1f;
            m_TriggeredThisGesture = false;
        }
    }
}
