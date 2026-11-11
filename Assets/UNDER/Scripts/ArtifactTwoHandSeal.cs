using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Under.VerticalSlice
{
    /// <summary>
    /// Requires two simultaneous direct selections and opposing hand pulls before
    /// the shell halves separate and the lock can report OPEN.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtifactTwoHandSeal : MonoBehaviour
    {
        [SerializeField] ArtifactLockController m_Controller;
        [SerializeField] Transform m_Frame;
        [SerializeField] XRSimpleInteractable m_LeftHandle;
        [SerializeField] XRSimpleInteractable m_RightHandle;
        [SerializeField] Transform m_LeftShell;
        [SerializeField] Transform m_RightShell;
        [SerializeField, Min(0.04f)] float m_TravelDistance = 0.13f;
        [SerializeField, Range(0.5f, 1f)] float m_CompletionThreshold = 0.72f;

        IXRSelectInteractor m_LeftInteractor;
        IXRSelectInteractor m_RightInteractor;
        Vector3 m_LeftClosedPosition;
        Vector3 m_RightClosedPosition;
        Vector3 m_LeftStartHand;
        Vector3 m_RightStartHand;
        bool m_Armed;
        bool m_Opened;
        bool m_Initialized;

        void Awake()
        {
            Initialize();
        }

        void OnEnable()
        {
            Subscribe();
        }

        void OnDisable()
        {
            Unsubscribe();
        }

        void Update()
        {
            if (!m_Armed || m_Opened || m_LeftInteractor == null || m_RightInteractor == null || m_Frame == null)
                return;

            var leftHand = m_Frame.InverseTransformPoint(m_LeftInteractor.GetAttachTransform(m_LeftHandle).position);
            var rightHand = m_Frame.InverseTransformPoint(m_RightInteractor.GetAttachTransform(m_RightHandle).position);
            var leftProgress = Mathf.Clamp01(Vector3.Dot(leftHand - m_LeftStartHand, Vector3.left) / m_TravelDistance);
            var rightProgress = Mathf.Clamp01(Vector3.Dot(rightHand - m_RightStartHand, Vector3.right) / m_TravelDistance);

            ApplyShellPositions(leftProgress, rightProgress);
            if (leftProgress < m_CompletionThreshold || rightProgress < m_CompletionThreshold)
                return;

            m_Opened = true;
            m_Armed = false;
            ApplyShellPositions(1f, 1f);
            m_Controller?.CompleteTwoHandSeal();
            SetAvailable(false);
        }

        public void Configure(
            ArtifactLockController controller,
            Transform frame,
            XRSimpleInteractable leftHandle,
            XRSimpleInteractable rightHandle,
            Transform leftShell,
            Transform rightShell,
            float travelDistance = 0.13f)
        {
            Unsubscribe();
            m_Controller = controller;
            m_Frame = frame;
            m_LeftHandle = leftHandle;
            m_RightHandle = rightHandle;
            m_LeftShell = leftShell;
            m_RightShell = rightShell;
            m_TravelDistance = travelDistance;
            m_Initialized = false;
            Initialize();
            Subscribe();
            ResetMechanism();
        }

        public void SetAvailable(bool available)
        {
            if (m_LeftHandle != null)
                m_LeftHandle.enabled = available;
            if (m_RightHandle != null)
                m_RightHandle.enabled = available;

            if (!available && !m_Opened)
                ResetPull();
        }

        public void ResetMechanism()
        {
            Initialize();
            m_Opened = false;
            ResetPull();
            ApplyShellPositions(0f, 0f);
        }

        void OnLeftSelected(SelectEnterEventArgs args)
        {
            if (m_Controller == null || !m_Controller.CanUse(ArtifactLockController.LockStage.BreakSeal))
                return;

            m_LeftInteractor = args.interactorObject;
            TryArm();
        }

        void OnRightSelected(SelectEnterEventArgs args)
        {
            if (m_Controller == null || !m_Controller.CanUse(ArtifactLockController.LockStage.BreakSeal))
                return;

            m_RightInteractor = args.interactorObject;
            TryArm();
        }

        void OnLeftReleased(SelectExitEventArgs args)
        {
            if (args.interactorObject == m_LeftInteractor)
                m_LeftInteractor = null;
            if (!m_Opened)
                ResetPull();
        }

        void OnRightReleased(SelectExitEventArgs args)
        {
            if (args.interactorObject == m_RightInteractor)
                m_RightInteractor = null;
            if (!m_Opened)
                ResetPull();
        }

        void TryArm()
        {
            if (m_LeftInteractor == null || m_RightInteractor == null || m_Frame == null)
                return;

            m_LeftStartHand = m_Frame.InverseTransformPoint(m_LeftInteractor.GetAttachTransform(m_LeftHandle).position);
            m_RightStartHand = m_Frame.InverseTransformPoint(m_RightInteractor.GetAttachTransform(m_RightHandle).position);
            m_Armed = true;
        }

        void ResetPull()
        {
            m_Armed = false;
            m_LeftInteractor = null;
            m_RightInteractor = null;
            if (!m_Opened)
                ApplyShellPositions(0f, 0f);
        }

        void Initialize()
        {
            if (m_Initialized || m_LeftShell == null || m_RightShell == null)
                return;

            if (m_Frame == null)
                m_Frame = transform;
            m_LeftClosedPosition = m_LeftShell.localPosition;
            m_RightClosedPosition = m_RightShell.localPosition;
            m_Initialized = true;
        }

        void ApplyShellPositions(float leftProgress, float rightProgress)
        {
            if (m_LeftShell != null)
                m_LeftShell.localPosition = m_LeftClosedPosition + Vector3.left * (m_TravelDistance * leftProgress);
            if (m_RightShell != null)
                m_RightShell.localPosition = m_RightClosedPosition + Vector3.right * (m_TravelDistance * rightProgress);
        }

        void Subscribe()
        {
            if (m_LeftHandle != null)
            {
                m_LeftHandle.selectEntered.RemoveListener(OnLeftSelected);
                m_LeftHandle.selectExited.RemoveListener(OnLeftReleased);
                m_LeftHandle.selectEntered.AddListener(OnLeftSelected);
                m_LeftHandle.selectExited.AddListener(OnLeftReleased);
            }

            if (m_RightHandle != null)
            {
                m_RightHandle.selectEntered.RemoveListener(OnRightSelected);
                m_RightHandle.selectExited.RemoveListener(OnRightReleased);
                m_RightHandle.selectEntered.AddListener(OnRightSelected);
                m_RightHandle.selectExited.AddListener(OnRightReleased);
            }
        }

        void Unsubscribe()
        {
            if (m_LeftHandle != null)
            {
                m_LeftHandle.selectEntered.RemoveListener(OnLeftSelected);
                m_LeftHandle.selectExited.RemoveListener(OnLeftReleased);
            }

            if (m_RightHandle != null)
            {
                m_RightHandle.selectEntered.RemoveListener(OnRightSelected);
                m_RightHandle.selectExited.RemoveListener(OnRightReleased);
            }
        }
    }
}
