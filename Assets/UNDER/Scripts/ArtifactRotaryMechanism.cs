using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Under.VerticalSlice
{
    /// <summary>
    /// Converts a direct hand selection into a constrained, snapping collar rotation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtifactRotaryMechanism : XRSimpleInteractable
    {
        [SerializeField] ArtifactLockController m_Controller;
        [SerializeField] Transform m_Frame;
        [SerializeField] Transform m_RotatingPart;
        [SerializeField] Vector3 m_LocalAxis = Vector3.forward;
        [SerializeField, Min(10f)] float m_TravelDegrees = 90f;
        [SerializeField, Range(0.5f, 1f)] float m_SnapThreshold = 0.72f;

        IXRSelectInteractor m_Interactor;
        Quaternion m_ClosedRotation;
        Vector3 m_StartVector;
        float m_StartDegrees;
        float m_CurrentDegrees;
        bool m_Initialized;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnSelected);
            selectExited.AddListener(OnReleased);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(OnSelected);
            selectExited.RemoveListener(OnReleased);
            m_Interactor = null;
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || m_Interactor == null)
                return;

            var currentVector = GetPlanarHandVector();
            if (currentVector.sqrMagnitude < 0.0001f || m_StartVector.sqrMagnitude < 0.0001f)
                return;

            var axis = m_LocalAxis.normalized;
            var delta = Vector3.SignedAngle(m_StartVector, currentVector, axis);
            m_CurrentDegrees = Mathf.Clamp(m_StartDegrees + delta, 0f, m_TravelDegrees);
            ApplyRotation();
        }

        public void Configure(
            ArtifactLockController controller,
            Transform frame,
            Transform rotatingPart,
            float travelDegrees = 90f)
        {
            m_Controller = controller;
            m_Frame = frame;
            m_RotatingPart = rotatingPart;
            m_TravelDegrees = travelDegrees;
            m_Initialized = false;
            Initialize();
            ResetMechanism();
        }

        public void SetAvailable(bool available)
        {
            enabled = available;
        }

        public void ResetMechanism()
        {
            Initialize();
            m_CurrentDegrees = 0f;
            m_Interactor = null;
            ApplyRotation();
        }

        void OnSelected(SelectEnterEventArgs args)
        {
            if (m_Controller == null || !m_Controller.CanUse(ArtifactLockController.LockStage.AlignCollar))
                return;

            m_Interactor = args.interactorObject;
            m_StartDegrees = m_CurrentDegrees;
            m_StartVector = GetPlanarHandVector();
        }

        void OnReleased(SelectExitEventArgs args)
        {
            if (args.interactorObject != m_Interactor)
                return;

            m_Interactor = null;
            if (m_CurrentDegrees >= m_TravelDegrees * m_SnapThreshold)
            {
                m_CurrentDegrees = m_TravelDegrees;
                ApplyRotation();
                m_Controller?.CompleteCollarAlignment();
            }
            else
            {
                m_CurrentDegrees = 0f;
                ApplyRotation();
            }
        }

        Vector3 GetPlanarHandVector()
        {
            if (m_Interactor == null || m_Frame == null || m_RotatingPart == null)
                return Vector3.zero;

            var handWorld = m_Interactor.GetAttachTransform(this).position;
            var handLocal = m_Frame.InverseTransformPoint(handWorld);
            var pivotLocal = m_Frame.InverseTransformPoint(m_RotatingPart.position);
            return Vector3.ProjectOnPlane(handLocal - pivotLocal, m_LocalAxis.normalized).normalized;
        }

        void Initialize()
        {
            if (m_Initialized || m_RotatingPart == null)
                return;

            if (m_Frame == null)
                m_Frame = transform.parent;
            m_ClosedRotation = m_RotatingPart.localRotation;
            m_Initialized = true;
        }

        void ApplyRotation()
        {
            if (m_RotatingPart != null)
                m_RotatingPart.localRotation = m_ClosedRotation * Quaternion.AngleAxis(m_CurrentDegrees, m_LocalAxis.normalized);
        }
    }
}
