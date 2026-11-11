using System;
using UnityEngine;
using UnityEngine.Events;

namespace Under.VerticalSlice
{
    /// <summary>
    /// Owns the ordered state of UNDER's hero lock and exposes the one clean
    /// completion signal that later vertical-slice sequencing can consume.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtifactLockController : MonoBehaviour
    {
        public enum LockStage
        {
            AlignCollar,
            DrawSpine,
            BreakSeal,
            Open,
        }

        [Serializable]
        public sealed class ArtifactOpenedEvent : UnityEvent { }

        [Header("Mechanisms")]
        [SerializeField] ArtifactRotaryMechanism m_Rotary;
        [SerializeField] ArtifactSliderMechanism m_Slider;
        [SerializeField] ArtifactTwoHandSeal m_TwoHandSeal;

        [Header("Readable State")]
        [SerializeField] Transform m_InnerCore;
        [SerializeField] Renderer[] m_StageMarks = Array.Empty<Renderer>();
        [SerializeField] Material m_DormantMaterial;
        [SerializeField] Material m_ActiveMaterial;
        [SerializeField] Material m_CompleteMaterial;

        [Header("Output")]
        [SerializeField] ArtifactOpenedEvent m_OnArtifactOpened = new ArtifactOpenedEvent();

        LockStage m_Stage;
        Vector3 m_ClosedCoreScale;
        float m_OpenedAt;

        public LockStage stage => m_Stage;
        public bool isOpen => m_Stage == LockStage.Open;
        public ArtifactOpenedEvent onArtifactOpened => m_OnArtifactOpened;

        /// <summary>Code-level companion to the serialized UnityEvent.</summary>
        public event Action artifactOpened;

        void Awake()
        {
            if (m_InnerCore != null)
                m_ClosedCoreScale = m_InnerCore.localScale;

            ResetArtifact();
        }

        void Update()
        {
            if (!isOpen || m_InnerCore == null)
                return;

            var pulse = 1.18f + Mathf.Sin((Time.unscaledTime - m_OpenedAt) * 3.2f) * 0.035f;
            m_InnerCore.localScale = m_ClosedCoreScale * pulse;
        }

        public bool CanUse(LockStage requestedStage)
        {
            return m_Stage == requestedStage;
        }

        public void CompleteCollarAlignment()
        {
            if (m_Stage != LockStage.AlignCollar)
                return;

            SetStage(LockStage.DrawSpine);
        }

        public void CompleteSpineDraw()
        {
            if (m_Stage != LockStage.DrawSpine)
                return;

            SetStage(LockStage.BreakSeal);
        }

        public void CompleteTwoHandSeal()
        {
            if (m_Stage != LockStage.BreakSeal)
                return;

            SetStage(LockStage.Open);
            m_OpenedAt = Time.unscaledTime;
            Debug.Log("[UNDER ARTIFACT] Lock opened. OnArtifactOpened fired.", this);
            m_OnArtifactOpened.Invoke();
            artifactOpened?.Invoke();
        }

        [ContextMenu("Reset Artifact")]
        public void ResetArtifact()
        {
            m_Rotary?.ResetMechanism();
            m_Slider?.ResetMechanism();
            m_TwoHandSeal?.ResetMechanism();

            if (m_InnerCore != null && m_ClosedCoreScale != Vector3.zero)
                m_InnerCore.localScale = m_ClosedCoreScale;

            SetStage(LockStage.AlignCollar);
        }

        public void Configure(
            ArtifactRotaryMechanism rotary,
            ArtifactSliderMechanism slider,
            ArtifactTwoHandSeal twoHandSeal,
            Transform innerCore,
            Renderer[] stageMarks,
            Material dormantMaterial,
            Material activeMaterial,
            Material completeMaterial)
        {
            m_Rotary = rotary;
            m_Slider = slider;
            m_TwoHandSeal = twoHandSeal;
            m_InnerCore = innerCore;
            m_StageMarks = stageMarks ?? Array.Empty<Renderer>();
            m_DormantMaterial = dormantMaterial;
            m_ActiveMaterial = activeMaterial;
            m_CompleteMaterial = completeMaterial;
            m_ClosedCoreScale = m_InnerCore != null ? m_InnerCore.localScale : Vector3.one;
            ResetArtifact();
        }

        void SetStage(LockStage nextStage)
        {
            m_Stage = nextStage;
            m_Rotary?.SetAvailable(nextStage == LockStage.AlignCollar);
            m_Slider?.SetAvailable(nextStage == LockStage.DrawSpine);
            m_TwoHandSeal?.SetAvailable(nextStage == LockStage.BreakSeal);
            UpdateStageMarks();
        }

        void UpdateStageMarks()
        {
            for (var i = 0; i < m_StageMarks.Length; i++)
            {
                var mark = m_StageMarks[i];
                if (mark == null)
                    continue;

                if (i < (int)m_Stage)
                    mark.sharedMaterial = m_CompleteMaterial;
                else if (i == (int)m_Stage && m_Stage != LockStage.Open)
                    mark.sharedMaterial = m_ActiveMaterial;
                else
                    mark.sharedMaterial = m_DormantMaterial;
            }
        }
    }
}
