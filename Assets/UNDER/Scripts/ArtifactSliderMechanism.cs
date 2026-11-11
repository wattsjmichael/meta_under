using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Under.VerticalSlice
{
    /// <summary>
    /// Maps a direct hand pull onto one large, constrained linear spine.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtifactSliderMechanism : XRSimpleInteractable
    {
        [SerializeField] ArtifactLockController m_Controller;
        [SerializeField] Transform m_Frame;
        [SerializeField] Transform m_MovingPart;
        [SerializeField] Vector3 m_LocalAxis = Vector3.up;
        [SerializeField, Min(0.02f)] float m_TravelDistance = 0.11f;
        [SerializeField, Range(0.5f, 1f)] float m_SnapThreshold = 0.7f;

        IXRSelectInteractor m_Interactor;
        Vector3 m_ClosedPosition;
        Vector3 m_StartHandPosition;
        float m_StartProgress;
        float m_Progress;
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
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || m_Interactor == null || m_Frame == null)
                return;

            var handPosition = m_Frame.InverseTransformPoint(m_Interactor.GetAttachTransform(this).position);
            var distance = Vector3.Dot(handPosition - m_StartHandPosition, m_LocalAxis.normalized);
            m_Progress = Mathf.Clamp01(m_StartProgress + distance / m_TravelDistance);
            ApplyPosition();
        }

        public void Configure(
            ArtifactLockController controller,
            Transform frame,
            Transform movingPart,
            float travelDistance = 0.11f)
        {
            m_Controller = controller;
            m_Frame = frame;
            m_MovingPart = movingPart;
            m_TravelDistance = travelDistance;
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
            m_Progress = 0f;
            m_Interactor = null;
            ApplyPosition();
        }

        void OnSelected(SelectEnterEventArgs args)
        {
            if (m_Controller == null || !m_Controller.CanUse(ArtifactLockController.LockStage.DrawSpine))
                return;

            m_Interactor = args.interactorObject;
            m_StartProgress = m_Progress;
            m_StartHandPosition = m_Frame.InverseTransformPoint(m_Interactor.GetAttachTransform(this).position);
        }

        void OnReleased(SelectExitEventArgs args)
        {
            if (args.interactorObject != m_Interactor)
                return;

            m_Interactor = null;
            if (m_Progress >= m_SnapThreshold)
            {
                m_Progress = 1f;
                ApplyPosition();
                m_Controller?.CompleteSpineDraw();
            }
            else
            {
                m_Progress = 0f;
                ApplyPosition();
            }
        }

        void Initialize()
        {
            if (m_Initialized || m_MovingPart == null)
                return;

            if (m_Frame == null)
                m_Frame = transform.parent;
            m_ClosedPosition = m_MovingPart.localPosition;
            m_Initialized = true;
        }

        void ApplyPosition()
        {
            if (m_MovingPart != null)
                m_MovingPart.localPosition = m_ClosedPosition + m_LocalAxis.normalized * (m_TravelDistance * m_Progress);
        }
    }
}
